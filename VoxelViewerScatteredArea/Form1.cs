using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using ILNumerics.Toolboxes;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace VoxelViewerScatteredArea {
    public partial class Form1 : Form {

        // some helper arrays (caches) 
        private Array<float> m_V = localMember<float>();
        private Array<float> m_xn = localMember<float>();
        private Array<float> m_x = localMember<float>();
        private Array<float> m_Xn = localMember<float>();
        private Array<float> m_Yn = localMember<float>();
        private int m_nrSlices;
        private Array<float> m_slice = localMember<float>();
        private Sphere[] m_dots = new Sphere[4];
        private float[] m_dotPositions = new float[4];
        private bool[] m_dotsSelectd = new bool[4];
        private int res = 256;

        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {

            // create example dataset
            // Reference: http://www.volvis.org/ 
            m_nrSlices = 256;

            // read the whole file at once
            using (var stream = new MemoryStream(Resource1.skull)) {
                m_V.a = tosingle(loadBinary<byte>(stream, res, res, res * m_nrSlices));
            }
            m_V.a = reshape<float>(m_V, res, res, m_nrSlices);
            // m_V holds all image slices / voxels in a 3D array now: 256 x 256 x 256

            // setup the scene
            // Visualize some slices as fast surface 
            Array<float> grid = zeros<float>(res, res);
            var pc1 = ilPanel1.Scene.Add(new PlotCube("pc1", twoDMode: false));
            Array<float> C; 
            for (int i = 0; i < m_nrSlices; i += m_nrSlices / 4) {
                using (Scope.Enter()) {
                    C = m_V[full, full, i];
                    pc1.Add(new Surface(grid + i, C, colormap: Colormaps.Bone) 
                    { 
                        Wireframe = { Visible = false },
                        // Alpha = 0.1f // <- transparency slows down the rendering significantly!
                    });
                }
            }
            // add the last slice also
            C = m_V[full, full, end];
            pc1.Add(new Surface(grid + m_nrSlices-1, C, colormap: Colormaps.Bone) {
                Wireframe = { Visible = false },
                //Alpha = 0.1f // <- transparency slows down the rendering significantly!
            });


            // a slice is visualized by a fast surface and placed inside a group for easy transformation/ rotation
            pc1.Add(new Group("rotateSlice") {

                new FastSurface(tag: "dynslice")

            }); 

            // there are 4 dots set up as moveable control handles for the interpolation slice 
            m_dots = new Sphere[4];

            for (int i = 0; i < 4; i++) {
                // create a dot
                m_dots[i] = new Sphere(tag: i, resolution: 3) {
                    Wireframe = { Visible = false },
                };
                // front: 0 (start of Z axis), back: 1 (end of Z)
                m_dotPositions[i] = i % 2;
                // we want to place the dots directly on the Z axis lines. ILPlotCube hides this 
                // group but we can access it as follows: 
                pc1.Plots.Parent.Add(m_dots[i]);
                // individual dots are de-/selected with the mouse and moved by keys afterwards
                m_dots[i].Fill.MouseClick += (_s, _a) => {
                    var fill = _s as Triangles;
                    if (!_a.DirectionUp || fill == null) { return; }
                    // the index of the dot clicked is stored in the tag property 
                    int a = int.Parse(fill.Parent.Tag.ToString());
                    // the selection state is maintained in a local attribute
                    m_dotsSelectd[a] = !m_dotsSelectd[a];
                    // just set the color of the dot
                    fill.Color = (m_dotsSelectd[a]) ? Color.Red : Color.Green;
                    fill.Configure();
                    // show immediately
                    _a.Refresh = true;
                    // stop processing further (parent) events (plot cube rotation etc.)
                    _a.Cancel = true;
                };
                // the following disables the default rotation action for ILPlotCube when the mouse starts dragging on the dots.
                m_dots[i].Fill.MouseDown += (_s, _a) => { _a.Cancel = true; };
                m_dots[i].Fill.MouseMove += (_s, _a) => { _a.Cancel = true; };
            }

            // prepare some helper data for faster interpolation
            // defining the original data positions in V
            m_x.a = linspace<float>(0, 1, res);
            // grid for interpolation (tune /*2 factor for higher resolution)
            m_xn.a = linspace<float>(0, 1, res / 2);
            // interpolation grid in meshgrid format
            m_Yn.a = 1; m_Xn.a = meshgrid(m_xn, m_xn, m_Yn);

            UpdateDots(); 
            // handle +/- key strokes
            this.KeyPress += Form1_KeyPress;
            this.Shown += new EventHandler((_,_e) => 
             MessageBox.Show("The green balls are corners of a slicing area. Select individual corners with the mouse! Selected corners turn red and can be moved along the Z axis with the '+' and '-' keys. This moves the interpolation area cutting through the whole volume. You may also rotate the scene arbitrarily.")); 

        }

        void Form1_KeyPress(object sender, KeyPressEventArgs e) {
            float offs = 0;
            if (e.KeyChar == '+') {
                offs = 0.01f;
            } else if (e.KeyChar == '-') {
                offs = -0.01f;
            } else 
                return; 
            for (int i = 0; i < 4; i++) {
                if (m_dotsSelectd[i]) {
                    m_dotPositions[i] += offs; 
                    m_dotPositions[i] = Math.Max(Math.Min(1, m_dotPositions[i]), 0);
                }
            }
            UpdateDots();
            UpdateSlice();
            ilPanel1.Refresh(); 
        }
        /// <summary>
        /// The dots positions are recalculated based on m_dotPositions. This moves the spheres along the Z axis. 
        /// </summary>
        private void UpdateDots() {
            for (int i = 0; i < 4; i++) {
                m_dots[i].Transform = Matrix4.Translation(i / 2, i % 2, m_dotPositions[i]) * Matrix4.ScaleTransform(.02, .02, .02); 
            }
        }
        /// <summary>
        /// Update the cutting area. This is where the work is done. 
        /// </summary>
        private void UpdateSlice() {
            using (Scope.Enter()) {
                // first we use interp2 to create the coordinates of the scattered points for the slice. 
                // We see the cutting area as a single tile from [0...1] in both, X and Y directions. Its V matrix corresponds to the 
                // current values of m_dotPositions. interp2 creates the intermediate Z positions used below. 
                Array<float> Zn = array<float>(new float[] { m_dotPositions[0], m_dotPositions[1], m_dotPositions[2], m_dotPositions[3] }, size(2,2));
                Array<float> X = new float[] { 0, 1 }; 
                Zn.a = Interpolation.interp2(Zn, X, X, Xn1: m_xn, Xn2: m_xn);
                // Zn now is a matrix with m x n entries according to the sample resolution (default: 128 x 128). Its 4 corners correspond 
                // to the current position of the 4 dots on the Z - axes. Values in between are linearly interpolated. 
                // The X and Y positions do not change and are cached. (Could be improved even further)

                // Points on the cutting area are considered scattered points, because the area is not (necessarily) plain. However, V 
                // is a grid. interp3s interpolates the scattered points very efficiently. 
                // Note how the shape of the coordinate arrays Xn, Yn and Zn is not important. interp3s takes their elements in sequential order. 
                // The output is a vector of interpolated values. (We gonna reshape it below.)
                Array < float> Z = Interpolation.interp3s(m_V, m_x, m_x, m_x, m_Xn, m_Yn, Zn, method: InterpolationMethod.cubic);

                // let's plot! We get a reference to the fast surface
                var fsurf = ilPanel1.Scene.First<FastSurface>("dynslice"); 
                if (fsurf != null) {
                    // first time setup only: provide the full coordinates of X and V. Here it is sufficient to provide grid vectors. 
                    if (fsurf.Columns == 0) {
                        fsurf.Update(X: m_xn * res, Y: m_xn * res, Z: Zn * res, C: reshape(Z, Zn.shape), colormap: Colormaps.Hot);
                    } else {
                        // the grid was configured already and did not change. we save some recomputing by ommiting the X and Y coordinates. 
                        fsurf.Update(Z: Zn * res, C: reshape(Z, Zn.shape), colormap: Colormaps.Hot);
                    }
                }
                fsurf.Configure();
                ilPanel1.Refresh(); 
            }
        }


    }
}

#define FILLEDSLICE 
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;
namespace FinancialSlicedDataLarge {
    /// <summary>
    /// This example demonstrates many advanced aspects of ILNumerics Visualization Engine: efficient filled slices rendering, custom axis configuration, picking, CSV data reading, n-dim array handling
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {

            // our data sizes
            int sliceCount = 500;
            int sliceLength = 10; 

            // load sample data
            var datastring = File.ReadAllText("Data.csv");
            Array<float> A = csvread<float>(datastring, startCol: 1, endCol: 1, startRow: 0, endRow: sliceCount * sliceLength - 1) / 10000000;
            Array<long> dates = csvread<long>(datastring, startCol: 3, endCol: 3, startRow: 0, endRow: 9, 
                                                    elementConverter: s => DateTime.Parse(s).ToFileTimeUtc()); 
            
            // transform dates to a more reasonable floating point range (for stability reasons)

            Array<float> datesNorm = tosingle(todouble(dates - min(dates)) / todouble(max(dates - min(dates)))); 

            // bring A into correct matrix shape
            A = reshape(A, sliceLength, sliceCount);

            // regardless of the way we decide for rendering the data, the fastest will be to limit the number of 
            // render objects used. It is always better to have a few large objects than to render a huge number 
            // of small object. Therefore, we bring the data in a shape which allows us to render them as one big 
            // object. NaN is used to split individual slices. Any vertices with NaN position will not be rendered. 
            // We _add_ a line of NaNs as the last row of A (index 10 -> 11th row).
            A[sliceLength, full] = float.NaN;
 
            // let's create explicit vertex data for X and Y axes! It allows us to resort to most efficient basic 
            // shape objects later. Otherwise we could use higher level plot objects, like ILLinePlot. But since 
            // our data are potentially large, we aim at the best possible performance from the start.
            Array<float> Y = 1, X = meshgrid(arange<float>(1,sliceCount), datesNorm.Concat(float.NaN,0), Y); 

            // In order to make the slices better visible, we color them according to a colormap. 
            var colormap = new Colormap(Colormaps.Winter);  // try other colormaps: Hot, Jet, Winter, ILNumerics, Lines

#if !FILLEDSLICE

            // color the line's vertices according to the colormap
            Array<float> colorLines = colormap.Map(A).T; 
            // We want individual slices for each of the columns. This rules out regular surface plots.  As a first 
            // attempt we simply use a line for each slice. Lines bring the advantage that they might be usable even 
            // with too large data than the display can handle. Furthermore, they are the fastest way to render. 

            Array<float> vertLines = zeros<float>(3, A.S.NumberOfElements); 
            // Note, for left side subarray assignements, the right side does not need to have the exact same shape. 
            // Only the number of elements must match. 
            vertLines["0;:"] = Y; vertLines["1;:"] = X; vertLines["2;:"] = A;

            // create the lines shape, provide positions and colors
            var lines = new LineStrip() { 
                Positions = vertLines,
                Colors = colorLines,
            }; 
            // create a plot cube and add the lines shape to it
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) { lines });


#else
            //====================== filled area rendering =============================
            // this section is for rendering filled slices instead of lines
            // Filled areas are efficiently rendered with triangle strips. We need twice as much 
            // as for lines. The vertices of each slice are organized as follows: 
            // [1]-[3]-[5]-...
            //  | \ | \ | 
            // [0]-[2]-[4]-... 
            // 
            // If we feed a TriangleStrip with these vertices, it will come out as a filled area just fine. 
            //
            // Each vertex is stored in memory as [X, Y, Z] coordinate (float precision). In order to 
            // prepare the vertex buffer, the n-dimensional array features come in very handy. We use 
            // a 4 dimensional array to prepare the buffer data. Afterwards we just have to reshape it 
            // to get the correct shape needed for the buffer. 
            Array<float> vertTriangles = zeros<float>(3, 2, A.S[0], A.S[1]); 
            // here, the dimensions in vertTriangles are: [X,Y,Z],[lower/upper row for triangle strips],[length of slices],[number of slices]

            // define X coordinates, note the flipped order of X and Y dimensions, caused by meshgrid!
            vertTriangles["1;0;:;:"] = X;
            vertTriangles["1;1;:;:"] = X;
            // define Y coordinates
            vertTriangles["0;0;:;:"] = Y;
            vertTriangles["0;1;:;:"] = Y;

            vertTriangles["2;1;:;:"] = A;
            // set the base line to the value of the first point of each slice
            vertTriangles["2;0;:;:"] = repmat(vertTriangles["2;1;0;:"], 1,1,11,1);
            vertTriangles["2;:;:;:"] = sort(vertTriangles["2;:;:;:"], dim: 1, descending: true); 

            Array<float> colorsTriangles = colormap.Map(vertTriangles["2;:;:;:"], NaNColor: new Vector4(0,0,0,1));
            // we could use transparency here. But this would cause artefacts during rotation (because the triangles 
            // are ill positioned regarding the necessary center sorting). So better use opaque colors!
            //colorsTriangles["3;:"] = 0.3f; 
            
            var triangles = new TrianglesStrip() {
                Positions = reshape(vertTriangles, 3, vertTriangles.S.NumberOfElements / 3),
                Colors = colorsTriangles.T,
                AutoNormals = true
            };
            // create a plot cube and add the triangles shape to it
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) { triangles });
#endif
            //===================end of section: filled area rendering=================

            // configures the scene with a plot cube and add the shape
            ilPanel1.Scene.First<PlotCube>().AspectRatioMode = AspectRatioMode.StretchToFill;

            // configure the dates axis (here: X axis) 
            var axis = ilPanel1.Scene.First<PlotCube>().Axes.XAxis;
            axis.Ticks.Mode = TickMode.Manual;
            axis.Label.Visible = false; 
            for (int i = 0; i < 10; i++) {
                var lab = (ILNumerics.Drawing.Label)axis.Ticks.DefaultLabel.Copy(); 
                lab.Text = DateTime.FromFileTimeUtc(dates.GetValue(i)).ToShortDateString(); 
                lab.Rotation = 0.8f; 
                lab.Anchor = new PointF(1,0); 
                axis.Ticks.Add(datesNorm.GetValue(i), lab); 
            }
            ilPanel1.Scene.First<PlotCube>().Axes.YAxis.Label.Text = "Slice #"; 

            // Camera position
            ilPanel1.Scene.First<PlotCube>().Rotation =
                // rotate around the X  axis 
                Matrix4.Rotation(Vector3.UnitX, Math.PI / 2.5) *
                // rotate around the Z  axis 
                Matrix4.Rotation(Vector3.UnitZ, -Math.PI / 3.5);

            // picking 
            ilPanel1.Scene.First<PlotCube>().MouseClick += (_s, _a) => {
                // shortcut optimization to prevent from multiple pickings 
                if (_a.Target == null || !_a.DirectionUp || !_a.ControlPressed) return;
                // show working state to the user (large scenes take longer to pick)
                Cursor = Cursors.WaitCursor;
                try {
                    // acquire detailed information about all primitives under the cursor
#if FILLEDSLICE
                    using (var pick = ilPanel1.PickPrimitiveAt(triangles, _a.Location)) {
#else
                    using (var pick = ilPanel1.PickPrimitiveAt(lines, _a.Location)) {
#endif
                        using (Scope.Enter()) {
                            Array<float> vert = pick.VerticesWorld;
                            // check if picking successfull, primitives and vertices were found
                            if (!isnull(vert) && !vert.IsEmpty && vert.S[1] > 0) {

                                // The layout of the pick.VerticesWorld array when picking triangles: 
                                // x1 y1 z1 t  
                                // x2 y2 z2 s
                                // x3 y3 z3 v 
                                // 
                                // This corresponds to a single primitive. All primitives under the cursor 
                                // are sorted by ascending distance to the camera and lined up along the 
                                // third dimension in VerticesWorld. 
                                // So, the primitive closest to the camera is the one the user clicked on. 
                                // The slice number is simply the Y coordinate. The way we have designed 
                                // the slices y1, y2, and y3 are all equal.
                                Text = "Clicked on slice #" + vert.GetValue(0, 1, 0).ToString();
                            }
                        }
                    }
                } finally {
                    // reset mouse cursor
                    Cursor = Cursors.Arrow; 
                }
            }; 
        }

        // Export the scene as PNG image file
        private void button1_Click(object sender, EventArgs e) {
            // create a new gdi render, define the width and height of the resulting image
            using (var gdi = new GDIDriver(800,600, new Scene() { 
                // add the root of the scene which is currently shown in the form. This includes all interactivity (rotation etc.) 
                // potentially done by the user. 
                ilPanel1.SceneSyncRoot 
            })) {
                // this renders the scene to the backbuffer of the gdi renderer
                gdi.Render();
                // save the backbuffer with the rendered scene to a file.
                gdi.BackBuffer.Bitmap.Save("export.png");
                // ok - let the user know that it went fine.
                MessageBox.Show("Exported scene to: " + Path.Combine(Environment.CurrentDirectory, "export.png"));
            }
        }
    }
}

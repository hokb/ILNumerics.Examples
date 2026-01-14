using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.IO;
using System.Windows.Forms;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace VoxelViewerFastSurface {
    public partial class Form1 : Form {

        Array<byte> m_V = localMember<byte>();
        int m_curSlice = 0;
        private int m_nrSlices;

        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {

            // create example dataset
            // Reference: http://www.volvis.org/ 
            m_nrSlices = 256;
            
            // read the whole file at once
            using (var stream = new MemoryStream(Resource1.skull)) {
                m_V.a = loadBinary<byte>(stream, 256, 256, 256 * m_nrSlices);
            }
            m_V.a = reshape<byte>(m_V, 256, 256, m_nrSlices);
            // m_V holds all image slices / voxels in a 3D array now: 256 x 256 x 256

            // setup key control: "a" -> move to next slice, "b" -> move to previous slice
            KeyDown += Form1_KeyDown;
            // draw the current slice
            Draw();

            this.Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e) {
            MessageBox.Show($"Use the A and B keys to navigate in the volume and select the next / previous slice!"); 
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.A) {
                if (++m_curSlice >= m_nrSlices) {
                    m_curSlice = m_nrSlices - 1; 
                }
                Draw(); 
            } else if (e.KeyData == Keys.B) {
                if (--m_curSlice < 0) {
                    m_curSlice = 0;
                }
                Draw();
            }
        }

        private void Draw() {
            using (Scope.Enter(ArrayStyles.ILNumericsV4)) {
                // extract the current slice, they are stored in the 3rd dimension
                Array<byte> v = squeeze(m_V[full, full, m_curSlice]);

                // find the surface plot. Reusing is much more efficient!
                var imagesc = ilPanel1.Scene.First<FastSurface>();
                if (imagesc == null) {
                    // first time: setup the fast surface + reset the plot cube
                    imagesc = ilPanel1.Scene.Add(new PlotCube(twoDMode: false)).Add(new FastSurface());
                    imagesc.Update(Z: tosingle(v), colormap: Colormaps.Copper);
                    ilPanel1.Scene.First<PlotCube>().Reset();
                } else {
                    // update with the new data only
                    imagesc.Update(Z: tosingle(v), colormap: Colormaps.Copper);
                }
                // always obligatory: after updates to any buffers Configure signals the renderer to synchronize with the scene. 
                imagesc.Configure();
                ilPanel1.Refresh();
                Text = "Slice: " + m_curSlice + " of " + m_nrSlices;
            }
        }
    }
}

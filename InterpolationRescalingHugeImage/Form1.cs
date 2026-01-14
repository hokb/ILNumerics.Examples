using ILNumerics;
using ILNumerics.Drawing.Plotting;
using ILNumerics.Toolboxes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;


namespace InterpolationRescalingHugeImage {
    /// <summary>
    /// Resampling example of image data via Interpolation.interp2, dynamic display via ILFastSurface.
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // Changes to the NumericUpDown control trigger a resampling and a redraw 
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            Redraw();
        }

        // the only drawing function, triggers updates
        private void Redraw() {
            // fetch reference to the plot object
            var isc = ilPanel1.Scene.First<FastSurface>();
            if (isc != null && !m_fullImage.IsEmpty) {
                // indicate working state by a wait cursor
                Cursor = Cursors.WaitCursor; 
                // compute factor for number of samples from the current value of the numericupdown control. 
                int divisor = (int)numericUpDown1.Value;
                float fact = 10f / divisor; 

                // have the artificial scope for ILArray
                using (Scope.Enter()) {
                    // prepare grid vectors for target resolution grid 
                    Array<float> xn1 = linspace<float>(0, m_fullImage.S[1], m_fullImage.S[1] * fact);
                    Array<float> xn2 = linspace<float>(0, m_fullImage.S[0], m_fullImage.S[0] * fact);
                    // interpolate (up/down sampling)
                    Array<float> Vresamp = Interpolation.interp2(m_fullImage, Xn1: xn1, Xn2: xn2, method: InterpolationMethod.linear); // try other methods: InterpolationMethod.spline or cubic!

                    // update the surface plot. In general: do reuse objects! It is much faster than recreating them. 
                    isc.Update(Z: Vresamp, X: xn1, Y: xn2);

                    // Obligatory after all changes to any plot object: Call Configure() to commit all changes for redraw. 
                    isc.Configure();
                    // trigger a refresh of the panel to immediately show the new data.
                    ilPanel1.Refresh();

                    // Indicate the new resolution in the title bar. 
                    Text = String.Format("Image Resolution. Original: {0}x{1}, Resampled: {2}x{3}", m_fullImage.S[1], m_fullImage.S[0], Vresamp.S[1], Vresamp.S[0]);
                    // Switch back the cursor to standard
                    Cursor = Cursors.Arrow; 
                }
            }
        }

        private readonly Array<float> m_fullImage = localMember<float>(); 
        private void ilPanel1_Load(object sender, EventArgs e) {
            // load the full image from application path
            string filename = "hs-2009-25-aw-full_jpg.jpg"; // <- image courtesy of NASA.gov
            // Credits: NASA's Goddard Space Flight Center
            m_fullImage.a = tosingle(loadImage(filename));

            // setup the scene
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) { 
                new FastSurface()
            }); 
            Redraw();
            ilPanel1.Scene.First<PlotCube>().Reset(); 
        }
    }
}

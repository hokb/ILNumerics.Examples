using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections;
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


namespace Dynamic_Surface_Morph {
    /// <summary>
    /// This example creates a surface which is getting updated / modified dynamically on each rendering frame. The data are 'blended' between 
    /// two predefined datasets. In addition to that, a button is used to switch the colormaps used for coloring the surface. 
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            int rows = 200, cols = 200;
            // we keep two data matrices and blend between them later
            Array<float> A = SpecialData.sincf(rows, cols) * 1f;
            // the second is created off-centered
            Array<float> B = SpecialData.sincf(rows * 2, cols * 2, 3f)[r(1, rows), r(1, cols)];
            // add a new plot cube, a surface and a colorbar
            var plotCube = ilPanel1.Scene.Add(
                new PlotCube(twoDMode: false) {
            new Surface(A) { 
                new Colorbar() 
            }
        });

            // attach an event handler to be called on each frame
            ilPanel1.BeginRenderFrame += (send, args) => {
                // this gets called very frequently! Important 
                // to clean everthing up here: ILNumerics’ scoping helps: 
                using (Scope.Enter()) {
                    // calculate time varying factor -1...1
                    float time = (float)Math.Sin(args.Parameter.Time_ms / 500f);
                    // blend between A and B 
                    Array<float> C = A * time + B * (1 - time);
                    // update the surface 
                    var sf = (send as ILNumerics.Drawing.Panel).Scene.First<Surface>();
                    sf.UpdateColormapped(C, dataValues: C);
                }
            };
            // drag a new button on the form: button1 is used to switch colormaps:
            IEnumerator colormaps = Enum.GetValues(typeof(Colormaps)).GetEnumerator();
            // attach button event handler
            button1.Click += (send, args) => {
                // get the next colormap from the enumeration
                if (!colormaps.MoveNext()) {
                    // cycle from beginning 
                    colormaps.Reset();
                    colormaps.MoveNext();
                }
                // displays the name of the current colormap in window title
                this.Text = "Current Colormap: " + colormaps.Current.ToString();
                // set the colormap to the surface
                var sf = ilPanel1.Scene.First<Surface>();
                sf.Colormap = (Colormaps)colormaps.Current;
            };
            // start the clock for updates
            ilPanel1.Clock.Running = true;
        }
    }
}

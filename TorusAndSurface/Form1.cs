using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TorusAndSurface {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        private float CFunction(float a, float b) {
            return (float)Math.Sin(a * a) + (float)Math.Cos(b * b) + a * a * a + 3 * b * b * a; 
        }
        private float ZFunction(float a, float b) {
            return (float)Math.Sin(a * b) * (float)Math.Cos(1 / (a + b + 0.1f));
        }
 
        private void ilPanel3_Load(object sender, EventArgs e) {
            ilPanel3.Scene.First<ILNumerics.Drawing.Label>().Visible = false;
            ilPanel3.Scene.Add(new PlotCube(twoDMode: false) {
                DataScreenRect = new RectangleF(0.1f,0.05f,0.8f,.8f),
                Children = {
                    new Surface(ZFunction, -2,1,30,-4,0,20,CFunction, colormap:Colormaps.Hot)
                }
            });
            ilPanel3.Configure();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            ilPanel2.Scene.First<ILNumerics.Drawing.Label>().Visible = false;
            ilPanel1.Scene.Camera.Add(new Surface(SpecialData.torus(stepsPoloidal: 50, stepsToroidal: 50), colormap: Colormaps.Winter) {
                new Colorbar()
            }); 
        }

        private void ilPanel2_Load(object sender, EventArgs e) {
            ilPanel2.Scene.First<ILNumerics.Drawing.Label>().Visible = false;
            ilPanel2.Scene.Add(new PlotCube(twoDMode: false) {
                DataScreenRect = new RectangleF(0.1f, 0.05f, 0.8f, .8f),
                Children = {
                    new Surface(ZFunction, -2,1,30,-4,0,20,CFunction, colormap:Colormaps.Lines)
                }
            });
            ilPanel2.Configure();


        }
    }
}

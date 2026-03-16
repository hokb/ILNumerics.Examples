using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using ILNumerics.Toolboxes;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace WindowsFormsApplication1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {

            // load terrain data
            Array<float> terr = tosingle(SpecialData.terrain[r(20,40),r(20,45)]) + 2000;
            // actual grid positions
            Array<float> X1 = arange<float>(20, 40), X2 = arange<float>(20, 45);
            // new positions for smoothed, interpolated grid
            Array<float> Xn1 = linspace<float>(20, 40, 100), Xn2 = linspace<float>(20, 45, 100);
            // compute smoothed interpolated version with 2D spline interpolation
            Array<float> smooth = Interpolation.splinen(terr, cellv(X1, X2), cellv(Xn1, Xn2));
            // note: we have used splinen for demonstration purposes only. The same result could 
            // be computed with interp2. 

            // plotting original against smoothed version 
            var scene = new Scene {
                new PlotCube(twoDMode: false) {
                    // plot smoothed version, translates away in Z 
                    new Surface(smooth, Xn2, Xn1, colormap: Colormaps.Jet) {
                        // some surface configuration: specular lights
                        UseLighting = true,  Alpha = 0.5f, Wireframe = { Visible = false },
                        // have a colorba showing the colormap
                        Children = {
                            new Colorbar() { Alpha = 0.8f }
                        }
                    } ,
                    // plot the original data (kind of linear interpolation) at Z = 4000, just below the smoothed surface 
                    new Surface(zeros<float>(terr.S) + 4000, X2, X1, C: terr, colormap: Colormaps.Jet) 
                }
            };
            // configure Z axis label
            scene.First<PlotCube>().Axes.ZAxis.Label.Text = "Height [m]";
            scene.First<PlotCube>().Axes.ZAxis.LabelPosition = new Vector3(1,1,0);
            scene.First<PlotCube>().Axes.ZAxis.LabelAnchor = new PointF(1,1);

            // rotate the plot cube for convenient default view
            scene.First<PlotCube>().Rotation = Matrix4.Rotation(Vector3.UnitX, .8) * Matrix4.Rotation(Vector3.UnitZ, 0.7);
            // add the unit to the tick labels of the colorbar
            scene.First<Colorbar>().Axis.Ticks.LabelTransformFunc = (i, p) => p.ToString() + " m";

            // set the new scene as scene for rendering
            ilPanel1.Scene = scene; 
            
             
        }
    }
}

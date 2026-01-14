using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static ILNumerics.Globals;
using static ILNumerics.ILMath;

namespace ContourPlotConfiguration {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void panel1_Load(object sender, EventArgs e) {
            var scene = new Scene();
            // get terrain data, convert to single precision
            Array<float> A = tosingle(SpecialData.terrain[r(120,end),r(0,310)]);
            scene.Add(
              // create plot cube 
              new PlotCube(twoDMode: false) {
	            // create contour plot
	            new ContourPlot(A, create3D: true,
                    levels: new List<ContourLevel> {
			            // configure individual contour levels
			            new ContourLevel() { Text = "Coast", Value = 5, LineWidth = 3, LabelColor = Color.Azure },
                        new ContourLevel() { Text = "Plateau", Value = 1000, LineWidth = 3},
                        new ContourLevel() { Text = "Basis 1", Value = 1500, LineWidth = 3, LineStyle = DashStyle.PointDash },
                        new ContourLevel() { Text = "High", Value = 3000, LineWidth = 3, LineColor = 0 },
                        new ContourLevel() { Text = @"\fontname{Colonna MT}\bf\fontsize{+4}Rescue", Value = 4200, LineWidth = 3,
                                                          LineStyle = DashStyle.Dotted },
                        new ContourLevel() { Text = "Peak", Value = 5000, LineWidth = 3},
                    }), 
	            // add surface with the same data
	            new Surface(A) {
	  	            // disable wireframe 
		            Wireframe = { Visible = false },
                    UseLighting = true,
                    Children = {
                      new Legend { Location = new PointF(1f,.1f) },
                      new Colorbar {
                          Location = new PointF(1,.4f),
                          Anchor = new PointF(1,0)
                      }
                    }
                }
            });
            // combine a plot cube rotation around Z with one around X
            scene.First<PlotCube>().Rotation
              // note how the order is inversed 
              = Matrix4.Rotation(new Vector3(1, 0, 0), Math.PI / 5)
              // the rotation around Z is applied first !
              * Matrix4.Rotation(new Vector3(0, 0, 1), 0.2);

            scene.First<PlotCube>().AspectRatioMode = AspectRatioMode.StretchToFill; 
            panel1.Scene = scene;
        }
    }
}

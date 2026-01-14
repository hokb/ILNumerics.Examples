using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Drawing;
using System.Windows.Forms;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace WindowsFormsApplication6 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            
            // generate some test data
            Array<float> A = tosingle(randn(1, 20));
            Array<float> B = A.T + arange<float>(1, 20);
            // add two lines
            var pc = ilPanel1.Scene.Add(new PlotCube(tag: "myPlotCube1", twoDMode: true) {
                Children = {
                    new LinePlot(A, lineWidth:1, markerStyle: MarkerStyle.Circle),
                    new LinePlot(B, lineColor: Color.Red, lineWidth: 3), 
                },

                // configure some axis label properties
                Axes = {
                    YAxis = {
                        // place the label closer to the ticks
                        LabelAnchor = new PointF(1, 0),
                        // configure the default style for the ticks
                        Ticks = {
                            // Ticks and tick labels are auto-generated at runtime. 
                            // A DefaultLabel serves as template for their style.
                            DefaultLabel = { 
                                Font = new Font(ILNumerics.Drawing.Label.DefaultFont, FontStyle.Bold),
                                Color = Color.Red,
                            } 
                        }
                    },
                    // let's explicitly disable the Z axis. Otherwise rounding errors might cause it to show up. 
                    ZAxis = {
                        Visible = false,
                    }
                }
            });
            // do the reverse
            ReverseYAxis(); 
            // register the double click event to overwrite default behavior (reset the view)
            pc.MouseDoubleClick += (_s, _e) => {
                if (!_e.DirectionUp) return;
                ReverseYAxis();
                // redraw to show result
                _e.Refresh = true;
                // disable default mouse handler 
                _e.Cancel = true;
            }; 

        }

        /// <summary>
        /// Reset the plot cubes view and reverse the Y axis
        /// </summary>
        protected void ReverseYAxis() {
            var plotcube = ilPanel1.Scene.First<PlotCube>("myPlotCube1");
            if (plotcube != null) {
                // reset view (zoom fully out and center)
                plotcube.Reset(); 
                // rotate the plotcube so the Y axis appears in top down direction
                // we need to move the plotcube back or the rotation would rotate the y axis labels out of the viewing frustum
                // since the origin lays in (0,0,0) of the plotcube.
                plotcube.Rotation = Matrix4.Translation(0, 0, -2) * Matrix4.Rotation(Vector3.UnitX, pif);
            }
        }
    }
}

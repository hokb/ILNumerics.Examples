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
using ILNumerics.Toolboxes;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace HistogrammPlots {
    /// <summary>
    /// Histogram example. Various methods of computing histogram data are demonstrated. The results are plotted in a bar plot - manner. 
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        #region button event handlers
        private void button1_Click(object sender, EventArgs e) {
            PlotHist("hist"); 
        }
        private void button2_Click(object sender, EventArgs e) {
            PlotHist("histn"); 
        }

        private void button3_Click(object sender, EventArgs e) {
            PlotHist("histc");
        }

        private void button4_Click(object sender, EventArgs e) {
            PlotHist("histnc");
        }
        #endregion

        #region example plots
        private void PlotHist(string method) {
            // create some data
            Array<double> A = randn(1, 10001);
            // create the histogram of A, 10 bins

            Array<float> h = null;
            switch (method) {
                case "hist":
                    h = tosingle(Statistics.hist(A, 10));
                    break;
                case "histn":
                    h = tosingle(Statistics.histn(A, 10));
                    break;
                case "histc":
                    h = tosingle(Statistics.histc(A, 10));
                    break;
                case "histnc":
                    h = tosingle(Statistics.histnc(A, 10));
                    break;
            }
            
            // wipe (replace) the existing scene 
            ilPanel1.Scene = new Scene() { 
                // add a new plot cube
                new PlotCube(twoDMode: true) {
                    // add markers for the bin values (optional)
                    new LinePlot(h, lineColor: Color.Empty, markerStyle: MarkerStyle.Dot)
                }
            };
            // create "box-plot"
            var pc = ilPanel1.Scene.First<PlotCube>();
            if (pc != null) {
                pc.Axes.XAxis.Ticks.Mode = TickMode.Manual;
                for (int i = 0; i < h.Length; i++) {
                    // add a box for this bin
                    var g = pc.Add(new Group(translate: new Vector3((float)i - 0.4, 0, 0), scale: new Vector3(.8, h.GetValue(i), 1)) { 
                        Shapes.RectangleFilled, Shapes.RectangleWireframe 
                    });
                    // add the label for this bin
                    pc.Axes.XAxis.Ticks.Add(new Tick(i, new ILNumerics.Drawing.Label(text: i.ToString()) {
                        Anchor = new PointF(.5f, -0.5f)
                    }));
                }
                // add a title 
                //pc.Add(new ILTitle(method + " Example", Positioning.MiddleRight));
                pc.Add(new Legend("\\bf" + method + "(A)")); 
            }
            // trigger a redraw of the scene
            ilPanel1.Configure();
            ilPanel1.Refresh();
        }
        #endregion
    }
}

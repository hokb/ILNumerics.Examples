using System;
using System.Drawing;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace TwoXYLinePlotsAndLegend {
    /// <summary>
    /// This is an introductory example. It creates data for two line plots and shows them in various configurations in a single plot cube as XY plots. 
    /// </summary>
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // create sine and cosine test data: 100 samples, 2 periods
            Array<float> n = linspace<float>(-6.2f, 6.2f, 100);
            // we create a single data array since this makes it more convenient to
            // provide the X and Y data later. 
            Array<float> data = n.Concat(sin(n), 0).Concat(cos(n), 0);
            // add some noise to the sin row
            data[1, full] = data[1, full] + tosingle(rand(1, data.S[1]));
            // data is a 3 x 100 element matrix now

            // add the data as 2D line plots
            ilPanel1.Scene.Add(new PlotCube {
                // pick first and second row from data for the sin-XY plot 
                new LinePlot(data["0,1;:"], lineColor: Color.Red, markerStyle: MarkerStyle.Plus),
                // the cos plot combines the same X data with the 3rd row 
                new LinePlot(data["0,2;:"], lineColor: Color.Green, markerStyle: MarkerStyle.Diamond, markerColor: Color.Green),
                // add a legend 
                new Legend("sin(data)", "cos(data)")
            });
        }
    }
}
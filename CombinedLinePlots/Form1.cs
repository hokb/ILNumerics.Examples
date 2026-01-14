    using System.Windows.Forms;
    using ILNumerics;
    using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.ILMath;

    namespace LargeDataExample {
        public partial class Form1 : Form {
        
            public Form1() {
                InitializeComponent();
            }

            private class Computation {
                // helper function to create some data, 3 series of random data with mean of 10, 30 and 50 
                public static RetArray<float> MakeRandom(int n) {
                    using (Scope.Enter()) {
                        // create random data, uniform distribution, 3 rows, add mean
                        Array<float> A = tosingle(randn(3,n)) + vector(10f, 30f, 50f);
                        return A; 
                    }
                }
            }

            private void ilPanel1_Load(object sender, System.EventArgs e) {
                // create a plot cube 
                ilPanel1.Scene.Add(new PlotCube(twoDMode: true) {
                    // Create one plot for each row returned from MakeRandom
                    LinePlot.CreateXPlots(Computation.MakeRandom(100000)),
                    // add + configure legend
                    new Legend("Data 1", "Data 2", "Data 3")
                }); 
                //set the windows title
                Text = "3 x 100.000 element lines"; 
            }
        }
    }

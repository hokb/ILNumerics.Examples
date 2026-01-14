using ILNumerics;
using ILNumerics.Drawing;
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

namespace LinearLeastSquaresMethods {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void btnPinv_Click(object sender, EventArgs e) {
            // fit a linear model to measurement data [T,Y] 
            // The 'hidden' (unknown) model parameters: slope and offset 
            Array<float> sl = -2.4f, of = 1.5f; 
            // construct 'measurement data': linear model + random noise
            Array<float> T = linspace<float>(-4, 6, 100);
            Array<float> Y = T * sl + of 
                // add some noise to simulate measurement errors
                + 4 * tosingle(randn(1, T.Length)); 

            // compute the mean of the measurement data
            Array<float> mt = mean(T); 
            Array<float> my = mean(Y); 

            // Fitting a linear model via pinv
            // ======================================
            // find the slope of the underlying line model
            Array<float> M = multiply(Y - my, pinv(T - mt)); 

            // create plotting data for the model
            // we take the same data range as above
            Array<float> fit = T.C;   
            
            // for computing the model output for our input data, we must  
            // take into account, that the "model" expects all data to be centered 
            // in all dimensions! So we must compensate for that using the precomputed mean: 
            fit["1;:"] = ((T - mt) * M) + my; 
            // prepare the original data for plotting
            T["1;:"] = Y; 

            // plot data and fitted model 
            var scene = new Scene() {
                new PlotCube {
                    new LinePlot(T, lineColor: Color.Empty, markerStyle: MarkerStyle.Dot), 
                    new LinePlot(fit, lineColor: Color.Red, lineWidth: 3), 
                    new Legend("measurement", "fitted model")
                }
            };
            scene.Configure(); 
            ilPanel1.Scene = scene;
            Text = "Simple Linear Regression via pinv"; 

            ilPanel1.Refresh(); 
        }

        private void btnPolynomial_Click(object sender, EventArgs e) {
            // fit polynomial models to measurement data [T,Y] (here: tan function)

            // construct 'measurement data': tangens model + random noise
            Array<double> T = linspace(-1.4, 1.4, 100);
            Array<double> Y = tan(T) 
                // add some noise to simulate measurement errors
                + 0.25 * randn(1, T.Length);

            // the plotting range 
            Array<double> pR = linspace(-1.6, 1.6, 50);
            // define variables to hold the predicted data
            Array<double> mod1 = pR.C, mod2 = pR.C, mod3 = pR.C;

            // linear model
            using (var model = MachineLearning.ridge_regression(T, Y, 1, 1e-4)) {
                mod1["1;:"] = model.Apply(pR, null);
                // predict other data here as needed ... 
            }
            // quadratic model
            using (var model = MachineLearning.ridge_regression(T, Y, 2, 1e-4)) {
                mod2["1;:"] = model.Apply(pR, null);
                // predict other data here as needed ... 
            }
            // cubic model
            using (var model = MachineLearning.ridge_regression(T, Y, 3, 1e-4)) {
                mod3["1;:"] = model.Apply(pR, null);
                // predict other data here as needed ... 
            }

            // prepare the original data for plotting
            T["1;:"] = Y;

            // plot data and fitted model 
            var scene = new Scene() {
                new PlotCube {
                    new LinePlot(tosingle(T), lineColor: Color.Empty, markerStyle: MarkerStyle.Circle), 
                    new LinePlot(tosingle(mod1), lineColor: Color.Red, lineWidth: 4), 
                    new LinePlot(tosingle(mod2), lineColor: Color.Blue, lineWidth: 4), 
                    new LinePlot(tosingle(mod3), lineColor: Color.Green, lineWidth: 4), 
                    new Legend("measurement", "linear model", "quadratic model", "cubic model")
                }
            };
            scene.Configure();
            ilPanel1.Scene = scene;
            Text = "Polynomial Regression via ridge_regression";

            ilPanel1.Refresh();
        }
    }
}

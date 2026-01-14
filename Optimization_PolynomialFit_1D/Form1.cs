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

namespace Optimization_PolynomialFit_1D {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }
        private void ilPanel1_Load(object sender, EventArgs e) {
            // generate some fake 'measurement' data
            Array<double> X = linspace(0, 6.7, 100);
            // sin(x) + some measurement noise
            Array<double> Y = sin(X) + rand(1, X.S[1]) * 0.5;

            // Fitting a polynomial Model:
            // initial guess: this will determine the dimensionality of the 
            // solution / the degree of the polynomial model and gives a 
            // starting point.  We give an vector of length 5, which corresponds 
            // to an polynom of degree 4.
            Array<double> guess = ones(5, 1);
            // minimize the distance between the values produced by the model 
            // and the actual measurement data. By defining the objective 
            // function as lambda expression the data array Y is captured and 
            // available from inside the function. (Potential changes to Y will 
            // be visible to the objective function)
            Array<double> solution = Optimization.leastsq_pdl(x => {
                using (Scope.Enter(x)) {
                    return Y - poly(x, X);
                }
            }, guess);

            // plot the original signal and the fitted function
            // first convert the solution to an object[], used in legend label
            var vals = solution.Select(x => (object)x).ToArray();
            // we can use the simple tex formating of ILNumerics labels: 
            var label = string.Format("Fitted: {0:f2}+{1:f2}x+{2:f2}x^2+{3:f2}x^3+{4:f2}x^4", vals);

            // create the scene
            var scene = new Scene() {
                new PlotCube() {
                    new LinePlot(tosingle(Y)),
                    new LinePlot(
                        tosingle(poly(solution, X)), 
                        tag: solution, lineColor: Color.Red),
                    new Legend("Original: sin(x)", label)
                }
            };
            // assign the scene to the panel
            ilPanel1.Scene = scene;
        }
    }
}

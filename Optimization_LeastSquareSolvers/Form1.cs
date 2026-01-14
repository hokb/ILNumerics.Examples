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

namespace Optimization_LeastSquareSolvers {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
        }

        private void button1_Click(object sender, EventArgs e) {
            // measurement times ...
            Array<double> t = new double[] { 0, 0.0909, 0.1818, 0.2727, 0.3636, 0.4545, 0.5455, 0.6364, 0.7273, 0.8182, 0.9091, 1, 1.0909, 1.1818, 1.2727, 1.3636, 1.4545, 1.5455, 1.6364, 1.7273, 1.8182, 1.9091, 2, 2.0909, 2.1818, 2.2727, 2.3636, 2.4545, 2.5455, 2.6364, 2.7273, 2.8182, 2.9091, 3, 3.0909, 3.1818, 3.2727, 3.3636, 3.4545, 3.5455, 3.6364, 3.7273, 3.8182, 3.9091, 4, 4.0909, 4.1818, 4.2727, 4.3636, 4.4545, 4.5455, 4.6364, 4.7273, 4.8182, 4.9091, 5, 5.0909, 5.1818, 5.2727, 5.3636, 5.4545, 5.5455, 5.6364, 5.7273, 5.8182, 5.9091, 6, 6.0909, 6.1818, 6.2727, 6.3636, 6.4545, 6.5455, 6.6364, 6.7273, 6.8182, 6.9091, 7, 7.0909, 7.1818, 7.2727, 7.3636, 7.4545, 7.5455, 7.6364, 7.7273, 7.8182, 7.9091, 8, 8.0909, 8.1818, 8.2727, 8.3636, 8.4545, 8.5455, 8.6364, 8.7273, 8.8182, 8.9091, 9 }; 
            
            // measured data 
            //ILArray<double> Y = sin(0.05 * times) * exp(-0.5 * times) + 0.005 * rand(1, times.Length);
            Array<double> Y = new double[] { 0.004539073,0.005304616,0.008928807,0.014724801,0.015965915,0.018598558,0.022182483,0.026682905,0.028915295,0.031175491,0.033759777,0.032987223,0.035473349,0.035101036,0.036219321,0.035810564,0.039017442,0.039160084,0.038318565,0.037953717,0.040355838,0.041414701,0.040759151,0.039536583,0.039586551,0.038109767,0.039023269,0.037411212,0.039985689,0.036584313,0.035053269,0.038675204,0.036623271,0.035275427,0.033451906,0.032558396,0.035375542,0.032704459,0.031717203,0.034436913,0.031892071,0.031372305,0.032672952,0.028124261,0.030736270,0.029148031,0.028834449,0.027801364,0.025745456,0.027213385,0.023321857,0.025398769,0.022869529,0.021536303,0.025177847,0.025125520,0.021680111,0.021979267,0.023477949,0.022231138,0.020251554,0.021995736,0.020708495,0.018350734,0.019358500,0.015732903,0.019608029,0.016210277,0.015591750,0.015685850,0.015684905,0.014921621,0.014534768,0.014574777,0.015466792,0.012203830,0.010873728,0.013768866,0.014020742,0.009824341,0.012900276,0.009154852,0.008970946,0.009509728,0.009487281,0.010419402,0.011159062,0.010073862,0.009759822,0.007702829,0.010004975,0.007306626,0.007951461,0.008054240,0.007623640,0.008228156,0.008912234,0.007507954,0.006907845,0.008119266 }; 

            // define the objective function F
            Optimization.ObjectiveFunction<double> F = A => {
                using (Scope.Enter(A)) {
                    return Y - sin(A[0] * t) * exp(A[1] * t);
                }
            };

            // solve the least squares problem
            Array<double> min = Optimization.leastsq_levm(F, ones(2, 1) * 0.1); 

            // plot the data + result
            Array<float> orig = tosingle(horzcat(t, Y)).T;

            Array<float> model = tosingle(t.C).T;

            model[1,full] = tosingle(sin(min[0] * t) * exp(min[1] * t)); 

            var scene = new Scene {
                new PlotCube {
                    new LinePlot(orig, lineColor: Color.Empty, markerStyle: MarkerStyle.Circle),
                    new LinePlot(model, lineColor: Color.Red, lineWidth: 3),
                    new Legend("measured", String.Format(@"model: \alpha_0: {0:f2}, \alpha_1:{1:f2}", min.GetValue(0), min.GetValue(1)))
                }
            }; 
            scene.Configure();

            // refresh the panel
            // for your Windows.Forms app you would do: 
            ilPanel1.Scene = scene; 
            ilPanel1.Refresh();
            Text = "Least Square Minimization, sin * exp model";

        }

        #region Custom Jacobian

        /// <summary>
        /// Vectorial implementation of the Beale function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        RetArray<double> BealeVectorized(InArray<double> x) {
            using (Scope.Enter(x)) {
                Array<double> f = zeros<double>(3, 1);
                f[0] = (1.5 - x[0] + x[0] * x[1]);
                f[1] = (2.25 - x[0] + x[0] * x[1] * x[1]);
                f[2] = (2.625 - x[0] + x[0] * x[1] * x[1] * x[1]);
                fEvalCount++; 
                return f;
            }
        }

        RetArray<double> CustomBealeJacobian(Optimization.ObjectiveFunction<double> F, InArray<double> A, InArray<double> Y) {
            using (Scope.Enter(A, Y)) {
                Array<double> J = zeros(3, 2);
                J[0, 0] = -1 + A[1];        J[0, 1] = A[0]; 
                J[1, 0] = -1 + A[1] * A[1]; J[1, 1] = 2 * A[0] * A[1];
                J[2, 0] = -1 + pow(A[1], 3); J[2, 1] = 3 * A[0] * A[1] * A[1];
                // test: compare with jacobian_fast
                //ILArray<double> check = norm(J - Optimization.jacobian_fast(F, A, null));
                //System.Diagnostics.Debug.Assert(check < 1e-5); 
                return J; 
            }
        }

        private int fEvalCount = 0; 
        private void btnJacobian_Click(object sender, EventArgs e) {
            Array<int> itCount = 1;
            Array<double> x_min, steps = 1; 
            // find the minimum using an analytical derivative
            fEvalCount = 0;
            string msg = "leastsq_pdl Jacobian: {0} Iterations: {1}  FuncEvals: {2}";
            x_min = Optimization.leastsq_pdl(BealeVectorized, ones(2, 1), iterationCount: itCount);
            System.Diagnostics.Debug.WriteLine(string.Format(msg, "default (jacobian_prec)", itCount, fEvalCount));
            // solve with fast jacobian estimate 
            fEvalCount = 0;
            x_min = Optimization.leastsq_pdl(BealeVectorized, ones(2, 1), iterationCount: itCount, jacobianFunc: Optimization.jacobian_fast);
            System.Diagnostics.Debug.WriteLine(string.Format(msg, "fast (jacobian_fast)   ", itCount, fEvalCount));
            // solve with exact jacobian (custom analytical implementation) 
            fEvalCount = 0;
            x_min = Optimization.leastsq_pdl(BealeVectorized, ones(2, 1), iterationCount: itCount, jacobianFunc: CustomBealeJacobian, iterations: steps);
            System.Diagnostics.Debug.WriteLine(string.Format(msg, "exact custom function  ", itCount, fEvalCount));
            /*
            Jacobian: default (jacobian_prec) Iterations: 8  Evaluations: 161
            Jacobian: fast (jacobian_fast)    Iterations: 8  Evaluations: 35
            Jacobian: exact custom function   Iterations: 8  Evaluations: 17
            */

            // plot function and solution
            steps[2,full] = Optimization.Beale(steps.T); 
            var scene = new Scene {
                new PlotCube(twoDMode: false) {
                    new Surface((x,y) => (float)Optimization.Beale(new double[] {x,y}).GetValue(0),
                        xmin: -3.5f, xmax: 3.5f, ymin: -3.5f, ymax: 3.5f), 
                    new LinePlot(tosingle(steps), markerStyle: MarkerStyle.Dot)
                }
            };
            scene.Configure();

            // refresh the panel
            // for your Windows.Forms app you would do: 
            ilPanel1.Scene = scene;
            ilPanel1.Refresh();
            Text = "Beale Function, Minimization via leastsq_pdl, various derivatives";
        }
        #endregion

        #region gather iterations

        #endregion
    }
}
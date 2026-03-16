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

namespace ConstrainedOptimizationExample_Visual {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        #region 1d bound constraints
        // define objective function
        RetArray<double> myFunc(InArray<double> x) {
            using (Scope.Enter(x)) {
                return (x - 1) * (x - 1);
            }
        }
        // inequalty constraint
        RetArray<double> ineqConst1D(InArray<double> x) {
            using (Scope.Enter(x)) {
                return -x[0] + 2.1;
            }
        }

        private void btn1DBoundConstraint_Click(object sender, EventArgs e) {
            // define the bounds and start 
            Array<double> bounds = new double[] { 2, 3 };
            Array<double> start = new double[] { 1 };
            // find minimum inside bounds, start at 0
            Array<double> m = Optimization.fmin(myFunc, start,
                lowerBound: bounds[0], upperBound: bounds[1], InequalityConstraint: ineqConst1D, tol: 1e-5);

            //// alternative: define objective function as anonymous lambda expression
            //ILArray<double> m = Optimization.fmin(x => {
            //    using (ILScope.Enter(x)) { return (x - 1) * (x - 1); }
            //}, start, lowerBound: bounds[0], upperBound: bounds[1]);

            // create data for plotting 
            Array<double> t = linspace<double>(-3, 4, 100);
            t[1, full] = myFunc(t);
            bounds[full ,1] = myFunc(bounds);
            m[1] = myFunc(m);
            m[full, 1] = m.C; // hack to enable single marker 'line' plots
            start[1] = myFunc(start);
            start[full, 1] = start.C;


            // create the scene
            var scene = new Scene()
            {
                new PlotCube {
                    // plot function
                    new LinePlot(tosingle(t)), 
                    // plot bounds
                    new LinePlot(tosingle(bounds.T), 
                                markerStyle: MarkerStyle.Square, 
                                markerColor: Color.Red) { Line = { Visible = false } },
                    // plot start
                   new LinePlot(tosingle(start)) {
                        Line = { Visible = false },
                        Marker = { 
                            Size = 15, 
                            Style = MarkerStyle.Circle, 
                            Border = { Color = Color.Green, Width = 3 }
                        }
                    },
                    // plot minimum
                    new LinePlot(tosingle(m)) {
                        Line = { Visible = false },
                        Marker = { 
                            Size = 15, 
                            Style = MarkerStyle.Circle, 
                            Border = { Color = Color.Blue, Width = 3 }
                        }
                    },
                    // add legend 
                    new Legend("(x - 1)^2", "bounds", "start", "min(f)" ), 
                }
            };
            ilPanel1.Scene = scene;
            ilPanel1.Scene.Configure();
            ilPanel1.Refresh();
            Text = "1D Bound + Inequalty Constraint: x > 2.1"; 
        }
        #endregion

        #region 2d equalty constraints
        // objective function definition
        RetArray<double> myFunc2DEqual(InArray<double> x) {
            using (Scope.Enter(x)) {
                return (x[0, full] - 1) * (x[0, full] - 1) + (x[1, full] + 2) * (x[1, full] + 2) - 4;
            }
        }
        //Equality constraint function definition
        RetArray<double> eqConst(InArray<double> x) {
            using (Scope.Enter(x)) {
                return 2 * x[0, full] - x[1, full];
            }
        }
        private void btn2DEqualty_Click(object sender, EventArgs e) {
            // Initial guess
            Array<double> x0 = vector(-1.5, 1.5);
            // fetch intermediate iterations
            Array<double> iter = 1; 

            // Minimizer computation
            Array<double> m = Optimization.fmin(myFunc2DEqual, x0, EqualityConstraint: eqConst, iterations: iter);

            // create data for plotting 
            Array<double> t = linspace<double>(-2, 2, 50); 
            Array<double> y = 1, x = meshgrid(t, t, y);
            // reshape, vectorized function evaluation
            Array<double> z = reshape(myFunc2DEqual(x[full].Concat(y[full], 1).T).T,t.Length,t.Length);

            // The equalty const. is a line: y = 2x 
            // we plot it as a line in 3D: 
            Array<double> cp = t.C;
            cp[1, full] = 2 * t;
            cp[2, full] = myFunc2DEqual(cp); 

            // minimum for plotting
            m[2] = myFunc2DEqual(m);
            m[full, 1] = m.C; // hack to enable single marker 'line' plots

            // create the scene
            var scene = new Scene()
            {
                new PlotCube(twoDMode: true) {
                    Children = {
                        // plot function
                        new Surface(z,x,y) { new Colorbar() },
                        // plot intermediate positions
                        new LinePlot(tosingle(iter)) {
                            Line = { Visible = false },
                            Marker = { 
                                Size = 10, 
                                Style = MarkerStyle.Cross, 
                            }
                        },
                        // plot equalty constraint
                        new LinePlot(tosingle(cp)),
                        // plot minimum
                        new LinePlot(tosingle(m)) {
                            Line = { Visible = false },
                            Marker = { 
                                Size = 15, 
                                Style = MarkerStyle.Circle, 
                                Border = { Color = Color.Blue, Width = 3 }
                            }
                        },
                        // add legend 
                        new Legend("positions", "eq. const.", "min( (x-1)^2 + (y+2)^2 - 4 )"), 
                    }, 
                    Limits = { XMin = -2, YMin = -2, XMax = 2, YMax = 2 }
                }
            };
            ilPanel1.Scene = scene;
            ilPanel1.Scene.Configure();
            ilPanel1.Refresh();
            // label the form 
            Text = "2D Equalty Constraint Example"; 
        }
        #endregion

        #region 2D Inequalty Constraints
        // objective function definition
        RetArray<double> myFunc2DInEqual(InArray<double> x) {
            using (Scope.Enter(x)) {
                return sum(x * x * vector(1.0,-1.0), 0) + prod(x, 0); 
            }
        }
        //Inequality constraint function definition
        RetArray<double> ineqConst(InArray<double> x) {
            using (Scope.Enter(x)) {
                // reformulate (x - 1)^2 + (y - 1)^2 < 1 
                // =>          (x - 1)^2 + (y - 1)^2 - 1 < 0
                Array<double> xmin1 = x - 1;
                return sum(xmin1 * xmin1, 0) - 1;
            }
        }
        private void btn2DInequal_Click(object sender, EventArgs e) {
            ComputeAndDrawInequaltyBounded();
        }

        /// <summary>
        /// Implement callback for fmin() iterations
        /// </summary>
        /// <param name="info">info class provided by fmin()</param>
        void callback(FminCallbackInfo<double> info) {
            // implement your functionality here...
            Console.Write("Current iteration position: " + info.XSub); 
        }

        private void ComputeAndDrawInequaltyBounded(double? upperBound = null, string title = "2D Inequalty Constraint Example") {
            // Initial guess
            Array<double> x0 = vector(-1.5, 1.5);
            // fetch intermediate iterations
            Array<double> iter = 1;

            // Minimizer computation
            Array<double> m = Optimization.fmin(myFunc2DInEqual, x0,
                InequalityConstraint: ineqConst,
                iterations: iter,
                upperBound: upperBound.HasValue ? ones(2, 1) * upperBound.Value : null,
                callback: callback);  

            // create data for plotting 
            Array<double> t = linspace<double>(-2, 2, 50);
            Array<double> y = 1, x = meshgrid(t, t, y);
            // reshape, vectorized function evaluation
            Array<double> z = reshape(myFunc2DInEqual(x[":"].Concat(y[":"], 1).T).T, t.Length, t.Length);

            // The inequalty constraint is a circle around [1,1] 
            // we plot it as such:
            Circle c = new Circle() { Fill = { Visible = false } };
            Array<float> cP = c.Border.Positions.Storage;
            cP[r(0,1), full] = cP[r(0,1), full] + 1;
            cP[2, full] = tosingle(myFunc2DInEqual(todouble(cP[r(0,1), full])));
            c.Border.Positions.Update(cP);

            // minimum for plotting
            m[2] = myFunc2DInEqual(m);
            m[full, 1] = m.C; // hack to enable single marker 'line' plots

            iter[2, full] = myFunc2DInEqual(iter); 

            // create the scene
            var scene = new Scene()
            {
                new PlotCube(twoDMode: true) {
                    Children = {
                        // plot function
                        // new ILSurface(z,x,y) { new ILColorbar() },
                        new Surface((xl,yl) => (float)myFunc2DInEqual(vector<double>(xl, yl)).GetValue(0),
                            xmin: -2, xmax: 2, ymin: -2, ymax: 2) { new Colorbar() },
                        // plot intermediate positions
                        new LinePlot(tosingle(iter)) {
                            Line = { Visible = false },
                            Marker = { 
                                Size = 10, 
                                Style = MarkerStyle.Cross, 
                            }
                        },
                        // plot inequalty constraint
                        c,
                        // plot minimum
                        new LinePlot(tosingle(m)) {
                            Line = { Visible = false },
                            Marker = { 
                                Size = 15, 
                                Style = MarkerStyle.Circle, 
                                Border = { Color = Color.Blue, Width = 3 }
                            }
                        },
                        // add legend 
                        new Legend("positions", "solution"), 
                    }, 
                    Limits = { XMin = -2, YMin = -2, XMax = 2, YMax = 2 }, 
                    Axes = { 
                        XAxis = { Label = { Text = "x_0"}},
                        YAxis = { Label = { Text = "x_1"}},
                    }
                }
            };
            ilPanel1.Scene = scene;
            ilPanel1.Scene.Configure();
            ilPanel1.Refresh();
            // label the form 
            Text = title;
        }
        #endregion

        #region inequal bounded 
        private void btnInequalBounded_Click(object sender, EventArgs e) {
            ComputeAndDrawInequaltyBounded(1.5, "2D Inequal Constraint, y <= 1.5"); 
        }
        #endregion


    }
}

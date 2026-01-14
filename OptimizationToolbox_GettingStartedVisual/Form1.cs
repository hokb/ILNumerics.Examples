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

namespace OptimizationToolbox_GettingStartedVisual {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            return; 
            // prepare the plot shown at startup 
            var scene = new Scene() {
                    // plot the rosenbrock function
                    new PlotCube(twoDMode: false) {
                        //ScaleModes = { ZAxisScale = AxisScale.Logarithmic },
                        Children = {
                            new Surface((x,y) => {
                                // lambda converting between system.value types and ILArray
                                using (Scope.Enter()) {
                                    return Optimization.Rosenbrock(vector(x, y)).GetValue(0); 
                                }
                            }, xmin: -2, xmax: 2, ymin: -3, ymax: 5, colormap: Colormaps.Jet) {
                                Children = {
                                    new Colorbar() { 
                                        // right align the colorbar to window
                                        Location = new PointF(1,.5f), 
                                        Anchor = new PointF(1.05f, .5f)
                                    }
                                }, 
                                Alpha = 0.8f
                            }, 
                        },
                        // configure some axis properties
                        Axes = { 
                            XAxis = { Label = { Text = "X_1 dimension" } },
                            YAxis = { Label = { Text = "X_2 dimension" } },
                            ZAxis = { Label = { Text = "Rosenbrock Value" } } 
                        },
                    }, 
                };
            // assign the scene + trigger rendering
            ilPanel1.Scene = scene;
            ilPanel1.Configure();
            ilPanel1.Refresh(); 
        }

        #region BFGS 2D Solve

        private void btnBFGS_Solve2D_Click(object sender, EventArgs e) {

            // find minimum, provide initial guess, fast jacobian estimates
            Array<double> M = Optimization.fminunconst_bfgs(ObjFuncBFGS2D, vector(1.0, 3.9),
                                                        gradientFunc: Optimization.jacobian_fast);

            // evaluate the function at the minimizer
            M[2] = ObjFuncBFGS2D(M);

            // draw the result
            var scene = new Scene() {
                new PlotCube(twoDMode: false) {
                    new Surface((x,y)=> x * x + y * y + 2) {
                        Children = {
                            new Colorbar() { 
                                // right align the colorbar to window
                                Location = new PointF(1,.5f), 
                                Anchor = new PointF(1.05f, .5f)
                            }
                        }, 
                        Alpha = 0.8f
                    },
                    new Points() { 
                        Positions = tosingle(M),
                        Size = 6
                    }
                }
            };
            // assign the scene (replacing existing scene) + trigger rendering
            ilPanel1.Scene = scene;
            ilPanel1.Scene.Configure(); 
            ilPanel1.Refresh();
        }

        /// <summary>
        /// Example objective function
        /// </summary>
        /// <param name="X">current position</param>
        /// <returns>scalar result</returns>
        RetArray<double> ObjFuncBFGS2D(InArray<double> X) {
            using (Scope.Enter(X)) {
                // X is expected as vector from R^2
                return X[0] * X[0] + X[1] * X[1] + 2;
            }
        }

        #endregion

        #region BFGS 2D Solve Gradient

        private void btnBFGS_Solve2DGrad_Click(object sender, EventArgs e) {

            // find minimum, provide initial guess, fast jacobian estimates
            Array<double> M = Optimization.fminunconst_bfgs(ObjFuncBFGS2D, vector(1.0, 3.9),
                                                        gradientFunc: ObjFuncBFGS2DGradient);

            // evaluate the function at the minimizer
            M[2] = ObjFuncBFGS2D(M);

            // draw the result
            var scene = new Scene() {
                new PlotCube(twoDMode: false) {
                    new Surface((x,y)=> x * x + y * y + 2) {
                        Children = {
                            new Colorbar() { 
                                // right align the colorbar to window
                                Location = new PointF(1,.5f), 
                                Anchor = new PointF(1.05f, .5f)
                            }
                        }, 
                        Alpha = 0.8f
                    },
                    new Points() { 
                        Positions = tosingle(M),
                        Size = 6
                    }
                }
            };
            ilPanel1.Scene = scene;
            ilPanel1.Refresh();
        }

        RetArray<double> ObjFuncBFGS2DGradient(
                                Optimization.ObjectiveFunction<double> func, 
                                InArray<double> X, InArray<double> fx) {
            using (Scope.Enter(X, fx)) {
                // we do not need func or fx here. We implement the 
                // gradient of x^2 + y^2 + 2 directly: 
                return (2 * X).T; 
                // make sure to return a row vector!
            }
        }

        #endregion

        #region BFGS Non Convex
        /// <summary>
        /// This finds a local MAXIMUM of a non-convex function. Which extrema to find is determined by the initial guess.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBFGS_Non_Convex_Click(object sender, EventArgs e) {

            // compute the minimizer
            Array<double> start = vector(1.0, 3.9);
            Array<double> M = Optimization.fminunconst_bfgs(ObjFuncBFGSNonConvex, start, tol: 1e-15);

            // evaluate the function at the interesting points
            M[2] = -ObjFuncBFGSNonConvex(M);
            start[2] = -ObjFuncBFGSNonConvex(start); 

            // draw the result in a new scene
            var scene = new Scene() {
                // ... with a plot cube
                new PlotCube(twoDMode: false) {
                    // ... and a surface plot of the same function ...
                    new Surface((x,y) => (float)(Math.Cos(x) + Math.Sin(y))) {
                        Children = {
                            new Colorbar() { 
                                // right align the colorbar to window
                                Location = new PointF(1,.5f), 
                                Anchor = new PointF(1.05f, .5f)
                            }
                        }, 
                        Alpha = 0.8f
                    },
                    // ... and a big red dot, marking the start position ... 
                    new Points("start") { 
                        Positions = tosingle(start),
                        Size = 6, Color = Color.Red
                    }, 
                    // ... and a blue one for the minimum found
                    new Points("solution") { 
                        Positions = tosingle(M),
                        Size = 6, Color = Color.Blue
                    }

                }
            };
            ilPanel1.Scene = scene;
            ilPanel1.Scene.Configure(); 
            ilPanel1.Refresh();
            // title 
            Text = String.Format("Non convex maximization - Start: [{0};{1};{2}] Maximum:[{3};{4};{5}]",
                    start.Concat(M,1).Select(v => v.ToString("f2")).ToArray()); 
        }
        /// <summary>
        /// Example objective function
        /// </summary>
        /// <param name="X">current position</param>
        /// <returns>scalar function result</returns>
        RetArray<double> ObjFuncBFGSNonConvex(InArray<double> X) {
            using (Scope.Enter(X)) {
                // we want the _maximum_: hence the minus: 
                return -Math.Cos((double)X[0]) - Math.Sin((double)X[1]);
            }
        }

        #endregion

        #region Gathering Detailed Information

        private void btnCamel_Click(object sender, EventArgs e) {
            MinimizeAndPlotDetails(Optimization.Camel3, "Camel3", 2);
        }

        private void btnRosenbrock_Click(object sender, EventArgs e) {
            MinimizeAndPlotDetails(Optimization.Rosenbrock, "Rosenbrock");
        }
        
        private void MinimizeAndPlotDetails(Optimization.ObjectiveFunction<double> objFunc, string funcName, float pRange = 3) {
            using (Scope.Enter()) {
                // declare local variables for providing as output parameters
                Array<int> countBFGS = 1, countLBFGS = 1;
                Array<double> iterBFGS = 1, iterLBFGS = 1;
                Array<double> gnormBFGS = 1, gnormLBFGS = 1;
                Array<double> start = vector(-2.0, -2.0);
                // minimize with BFGS
                Array<double> min = Optimization.fminunconst_bfgs(objFunc, start,
                    iterationCount: countBFGS, iterations: iterBFGS, gradientNorm: gnormBFGS);
                // minimize with L-BFGS
                min = Optimization.fminunconst_lbfgs(objFunc, start,
                    iterationCount: countLBFGS, iterations: iterLBFGS, gradientNorm: gnormLBFGS);
                // number of iterations
                var itMsg = String.Format("Iterations BFGS: {0} L-BFGS: {1}", (int)countBFGS, (int)countLBFGS);
                // set the caption of the form
                Text = itMsg;
                // countBFGS:  111
                // countLBFGS: 275
                // min: [1.0, 1.0]

                // draw the function with results
                // the iterations parameter of fmin(l)bfgs return a matrix: each column corresponds to 
                // one intermediate step postiion (x,y coordinates). 
                // evaluate z-coordinate of intermediate steps for plotting
                iterBFGS[2,full] = objFunc(iterBFGS.T);
                iterLBFGS[2,full] = objFunc(iterLBFGS.T);
                // construct the scene
                var scene = new Scene() {
                    // plot the objective function
                    new PlotCube(twoDMode: false) {
                        ScreenRect = new RectangleF(0,.3f,1,.7f),
                        //ScaleModes = { ZAxisScale = AxisScale.Logarithmic },
                        Children = {
                            new Surface((x,y) => {
                                using (Scope.Enter()) {
                                    return (float)objFunc(vector((double)x, (double)y)).GetValue(0); 
                                }
                            }, xmin: -pRange, xmax: pRange, ymin: -pRange, ymax: pRange) {
                                Alpha = 0.8f, 
                                Children = {
                                    new Colorbar() { 
                                        Location = new PointF(1,.5f), 
                                        Anchor = new PointF(1.05f, .5f)
                                    }
                                }
                            }, 
                            // draw paths for iterations. We simply provide the intermediate step positions as columns: 
                            new LinePlot(tosingle(iterBFGS), lineColor: Color.Red, markerStyle: MarkerStyle.Dot),
                            new LinePlot(tosingle(iterLBFGS), lineColor: Color.Blue, markerStyle: MarkerStyle.Circle),
                            new Legend("BFGS", "L-BFGS") { 
                                Location = new PointF(1,.1f), 
                                Anchor = new PointF(1.05f, 0f)
                            }
                        },
                        Axes = { 
                            XAxis = { Label = { Text = "X_1 dimension" } },
                            YAxis = { Label = { Text = "X_2 dimension" } },
                            ZAxis = { Label = { Text = funcName + " Value" } } 
                        },

                    }, 
                    // draw gradient norms
                    new PlotCube() {
                        ScreenRect = new RectangleF(0,0,1f,0.4f), 
                        Children = {
                            // the norm of the gradient for each iteration step is a scalar. 
                            // gnormBFGS will be plotted as a common 2D line plot. 
                            new LinePlot(tosingle(gnormBFGS), lineColor: Color.Red), 
                            new LinePlot(tosingle(gnormLBFGS), lineColor: Color.Blue), 
                            new Legend("Gradient norm BFGS", "Gradient norm L-BFGS")
                        },
                        // we log the y axis for better view resolution
                        ScaleModes = { YAxisScale = AxisScale.Logarithmic }, 
                        Axes = { 
                            XAxis = { Label = { Text = "Iterations" } },
                            YAxis = { Label = { Text = "log ||grad (f)||^2" } } 
                        },
                    }
                };
                // apply the scene, replacing any existing one
                ilPanel1.Scene = scene;
                // ... and cause a refresh of the panel
                ilPanel1.Configure();
                ilPanel1.Refresh();
            }
        }
        #endregion

        #region Newton 
        
        private void btnNewton_Click(object sender, EventArgs e) {
            using (Scope.Enter()) {
                float pRange = 3; 
                // declare local variables for providing as output parameters
                Array<int> countBFGS = 1, countNewton = 1;
                Array<double> iterBFGS = 1, iterNewton = 1;
                Array<double> gnormBFGS = 1, gnormNewton = 1;
                Array<double> start = vector(-1, -1.5);
                // minimize with BFGS
                Array<double> min = Optimization.fminunconst_bfgs(Optimization.Rosenbrock, start,
                    iterationCount: countBFGS, iterations: iterBFGS, gradientNorm: gnormBFGS);
                // minimize with classical newton method, give hessian function
                min = Optimization.fminunconst_newton(Optimization.Rosenbrock, start,  
                    iterationCount: countNewton, iterations: iterNewton, gradientNorm: gnormNewton);
                // number of iterations
                var itMsg = String.Format("Iterations BFGS: {0} Newton: {1}", (int)countBFGS, (int)countNewton);
                // set the caption of the form
                Text = itMsg;
                // countBFGS:  111
                // countNewton: 275
                // min: [1.0, 1.0]

                // draw the function with results
                // the iterations parameter of fminunconst_bfgs and fminunconst_newton return a matrix: each column corresponds to 
                // one intermediate step position (x,y coordinates). 
                // For plotting we need to evaluate z-coordinates of these intermediate steps
                iterBFGS[2,full] = Optimization.Rosenbrock(iterBFGS.T);
                iterNewton[2,full] = Optimization.Rosenbrock(iterNewton.T);

                // construct the scene
                var scene = new Scene() {
                    // plot the objective function
                    new PlotCube(twoDMode: false) {
                        ScreenRect = new RectangleF(0,.3f,1,.7f),
                        ScaleModes = { ZAxisScale = AxisScale.Logarithmic },
                        Children = {
                            new Surface((x,y) => {
                                using (Scope.Enter()) {
                                    return (float)Optimization.Rosenbrock(vector((double)x, (double)y)).GetValue(0); 
                                }
                            }, xmin: -pRange, xmax: pRange, ymin: -pRange, ymax: pRange) {
                                //Alpha = 0.8f, 
                                UseLighting = true, 
                                Children = {
                                    new Colorbar() { 
                                        Location = new PointF(1,.5f), 
                                        Anchor = new PointF(1.05f, .5f)
                                    }
                                }
                            }, 
                            // draw paths for iterations. We simply provide the intermediate step positions as columns: 
                            new LinePlot(tosingle(iterBFGS), lineColor: Color.Red, markerStyle: MarkerStyle.Dot),
                            new LinePlot(tosingle(iterNewton), lineColor: Color.Blue, markerStyle: MarkerStyle.Circle),
                            new Legend("BFGS", "Newton") { 
                                Location = new PointF(1,.1f), 
                                Anchor = new PointF(1.05f, 0f)
                            }
                        },
                        Axes = { 
                            XAxis = { Label = { Text = "X_1 dimension" } },
                            YAxis = { Label = { Text = "X_2 dimension" } },
                            ZAxis = { Label = { Text = "Rosenbrock Function Value" } } 
                        },

                    }, 
                    // draw gradient norms
                    new PlotCube() {
                        ScreenRect = new RectangleF(0,0,1f,0.4f), 
                        Children = {
                            // the norm of the gradient for each iteration step is a scalar. 
                            // gnormBFGS will be plotted as a common 2D line plot. 
                            new LinePlot(tosingle(gnormBFGS), lineColor: Color.Red), 
                            new LinePlot(tosingle(gnormNewton), lineColor: Color.Blue), 
                            new Legend("Gradient norm BFGS", "Gradient norm Newton")
                        },
                        // we log the y axis for better view resolution
                        ScaleModes = { YAxisScale = AxisScale.Logarithmic }, 
                        Axes = { 
                            XAxis = { Label = { Text = "Iterations" } },
                            YAxis = { Label = { Text = "log ||grad (f)||^2" } } 
                        },
                    }
                };
                // apply the scene, replacing any existing one
                ilPanel1.Scene = scene;
                // ... and cause a refresh of the panel
                ilPanel1.Configure();
                ilPanel1.Refresh();
            }
        }
        
        #endregion

        private void ilPanel1_Load(object sender, EventArgs e) {
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) {
                //new ILSurface((x,y) => (x + y) * (x + y) *(x + y), xmin: -2, xmax: 2, ymin: -2, ymax: 2)
                new Surface((x,y) => (float)ToyFunction(vector<double>(x,y)).GetValue(0), xmin: -2, xmax: 2, ymin: -2, ymax: 2)
            }); 
        }
        public static RetArray<double> ToyFunction(InArray<double> xp) {
            using (Scope.Enter(xp)) {
                Array<double> x = xp;
                if (x.IsColumnVector) {
                    x = x.T;
                }
                double val = (double)(2 * x[full, 0] * x[full, 0] + x[full, 0] * x[full, 1] 
                            + 2 * x[full, 1] * x[full, 1] - 6 * x[full, 0] - 6 * x[full, 1] + 15);
                return val;
            }
        }

    }
}

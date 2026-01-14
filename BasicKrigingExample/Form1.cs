using System;
using System.Drawing;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using ILNumerics.Toolboxes;
using static ILNumerics.ILMath;
using static ILNumerics.Globals; 

namespace BasicInterpolationExample {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            using (Scope.Enter()) {
                // generate some values based on a function of sin and cos
                Array<double> Y = 1, X = Computation.Generate1DData(500, Y);
                // pick some values and use them for interpolation
                Array<long> _pickInd = Statistics.randperm(X.S.Longest)[r(0,19)];
                // interpolate intermediate values
                Array<double> Yinterm = Interpolation.kriging(Y[full, _pickInd], X[full, _pickInd], X);

                // let's plot! 
                var pc = (ilPanel1.Scene = new Scene()).Add(new PlotCube(twoDMode: true));
                // plot the points used for interpolation 
                pc.Add(new LinePlot(X[full, _pickInd], Y[full, _pickInd], lineColor: Color.Empty, markerStyle: MarkerStyle.Dot));
                // plot the original function
                pc.Add(new LinePlot(X, Y, lineColor: Color.Black));
                // plot the interpolated function
                pc.Add(new LinePlot(X, Yinterm, lineColor: Color.Red));

                pc.Add(new Legend("Support Points", "Original Function", "Kriging Interpolation"));

                ilPanel1.Configure();
                ilPanel1.Refresh();
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            using (Scope.Enter()) {
                // generate some values based on a function of sin and cos
                Array<double> Y = 1, X = Computation.Generate1DData(500, Y);
                // pick some values and use them for interpolation
                Array<int> _pickInd = toint32(arange(0, 25, X.Length-1) );
                // err will hold error information returned from the interpolation.
                Array<double> err = 1; 
                Array<double> Yinterm = Interpolation.kriging(Y[full, _pickInd], X[full, _pickInd], Xn: X, error: err);

                // let's plot! 
                var pc = (ilPanel1.Scene = new Scene()).Add(new PlotCube(twoDMode: true));
                // plot the points used for interpolation 
                pc.Add(new LinePlot(X[full, _pickInd], Y[full, _pickInd], lineColor: Color.Empty, markerStyle: MarkerStyle.Dot));
                // plot the original function
                pc.Add(new LinePlot(X, Y, lineColor: Color.Black));
                // plot the interpolated function
                pc.Add(new LinePlot(X, Yinterm, lineColor: Color.Red));

                // plot the upper error
                pc.Add(new LinePlot(X, Yinterm + err.T / 2.0, lineColor: Color.LightSkyBlue));
                // plot the lower error
                pc.Add(new LinePlot(X, Yinterm - err.T / 2.0, lineColor: Color.LightGreen));

                pc.Add(new Legend("Support Points", "Original Function", "Kriging Interpolation", "Upper Error", "Lower Error"));

                ilPanel1.Configure();
                ilPanel1.Refresh();
            }
        }
        private void button3_Click(object sender, EventArgs e) {
            using (Scope.Enter(ArrayStyles.ILNumericsV4)) {
                // define a custom function of sin, cos and exp
                Func<InArray<double>, InArray<double>, RetArray<double>> myFunc = (x, y) => {
                    using (Scope.Enter(x, y)) {
                        return sin(x) * cos(y) * exp(-(x * x * y * y) / 4);
                    }
                };
                // generate a grid with true values based on the custom function for surface plotting 
                Array<double> Y = 1, X = meshgrid(arange(-3.0, 0.2f, 3.0), arange(-3.0, 0.2f, 3.0), Y);
                Array<double> Grid = reshape(X.C, X.S[0], X.S[1], 1);
                Grid[ellipsis, 1] = Y;
                Grid[ellipsis, 2] = myFunc(X, Y);

                // pick only some values as source for interpolation
                Array<double> scatteredPositions = rand(2, 100) * 6 - 3;
                Array<double> scatteredValues = myFunc(scatteredPositions[0, full], scatteredPositions[1, full]);

                // we compute interpolated values for all grid points used for plotting the true function also
                Array<double> interpPositions = X[full].T.Concat(Y[full].T, 0);
                // now interpPositions are layed out as follows: 
                // x1, x2, x3, x4, ...
                // y1, y2, y3, y4, ...
                
                // err will hold error information returned from the interpolation.
                Array<double> err = 1;
                Array<double> interpValues = Interpolation.kriging(scatteredValues, scatteredPositions, interpPositions, error: err);

                // bring the values back into shape for plotting
                interpValues.a = reshape(interpValues,X.shape );
                err.a = reshape(err, X.shape);

                // let's plot it! 
                var scene = new Scene();
                var pc = scene.Add(new PlotCube(twoDMode: false));
                // plot the points used for interpolation. We could use a line plot (marker only) here just like for line plots. 
                // But markers are displayed "always on top" and we want the surface to hide hidden markers. So 
                // let's resort to a more general ILPoints shape:
                Array<float> interpValues4Plot = tosingle(scatteredPositions.Concat(scatteredValues, 0));
                pc.Add(new Points() {
                    Positions = interpValues4Plot,
                    Color = Color.LightPink,
                    Size = 7
                });
                // plot the original function (surface only, no wireframe)
                pc.Add(new Surface(Grid[ellipsis, 2], X, Y, colormap: Colormaps.Gray) {
                    Wireframe = { Visible = false, Color = Color.Black },
                    UseLighting = true
                });
                // plot the _interpolated_ grid (wireframe only, no surface fill)
                // the color indicates the error for the interpolation. The color is based on 
                // the Jet colormap and is displayed as colorbar.
                var surf1 = pc.Add(new Surface(interpValues, X, Y, colormap: Colormaps.Jet, C: err) {
                    Fill = { Visible = false },
                    Wireframe = { Width = 2, Color = null }, // 'Color' controls the solid color. 'null' activates individual color mapped colors. 
                    Children = {
                    new Colorbar() {
                        // give the colorbar a title
                        new ILNumerics.Drawing.Label(text: @"Error, (\vartheta)") {
                            Anchor = new PointF(-.4f,1.1f)
                        }
                    }
                }
                });
                ilPanel1.Scene = scene; 
                // don't forget to prepare the scene for rendering + trigger a redraw!
                ilPanel1.Configure();
                ilPanel1.Refresh();
            }
        }
        private void button4_Click(object sender, EventArgs e) {
            using (Scope.Enter()) {
                // define a custom function of sin, cos and exp
                Func<InArray<double>, InArray<double>, RetArray<double>> myFunc = (x, y) => {
                    using (Scope.Enter(x, y)) {
                        return sin(x) * cos(y) * exp(-(x * x * y * y) / 4);
                    }
                };

                // define a custom variogram function: spherical variogram
                Func<InArray<double>, RetArray<double>> variogram = l => {
                    using (Scope.Enter(l)) {
                        return 0.72 * (1 - 1.5 * (l / 4425) + 0.5 * pow(l / 4425, 3));
                    }
                };

                // generate a grid with true values based on the custom function for surface plotting 
                Array<double> Y = 1, X = meshgrid(arange(-3.0, 0.2, 3.0), arange(-3.0, 0.2, 3.0), Y);
                Array<double> Grid = X.C;
                Grid[":;:;1"] = Y;
                Grid[":;:;2"] = myFunc(X, Y);

                // pick only some values as source for interpolation
                Array<double> scatteredPositions = rand(2, 100) * 6 - 3;
                Array<double> scatteredValues = myFunc(scatteredPositions["0;:"], scatteredPositions["1;:"]);

                // err will hold error information returned from the interpolation.
                Array<double> err = 1;
                // we compute interpolated values for all grid points used for plotting the true function also
                Array<double> interpPositions = X[":"].T.Concat(Y[":"].T, 0);
                // now interpPositions is layed out as follows (points in columns): 
                // x1, x2, x3, x4, ...
                // y1, y2, y3, y4, ...

                // the actual interpolation call provides the custom variogram function and requests error information:  
                Array<double> interpValues = Interpolation.kriging(scatteredValues, scatteredPositions, interpPositions, variogram, err);

                // bring the values back into shape for plotting
                interpValues.a = reshape(interpValues, X.shape);
                err.a = reshape(err, X.shape);

                // let's plot it! 
                var pc = (ilPanel1.Scene = new Scene()).Add(new PlotCube(twoDMode: false));
                // plot the points used for interpolation. We could use a line plot (marker only) here just like for line plots. 
                // But markers are displayed "always on top" and we want the surface to hide hidden markers. So 
                // let's resort to a more general ILPoints shape:
                Array<float> interpValues4Plot = tosingle(scatteredPositions.Concat(scatteredValues, 0));
                pc.Add(new Points() {
                    Positions = interpValues4Plot,
                    Color = Color.LightPink,
                    Size = 7
                });
                // plot the original function (surface only, no wireframe)
                pc.Add(new Surface(Grid[":;:;2,0,1"], colormap: Colormaps.Gray) { UseLighting = true });

                // plot the _interpolated_ grid (wireframe only, no surface fill)
                // the color indicates the error for the interpolation. The color is based on 
                // the Jet colormap and is displayed as colorbar.
                var surf1 = pc.Add(new Surface(interpValues, X, Y, colormap: Colormaps.Jet, C: err) {
                    Fill = { Visible = false },
                    Wireframe = { Width = 2, Color = null }, // 'Color' controls the solid color. 'null' activates individual color mapped colors. 
                    Children = {
                    new Colorbar() {
                        // give the colorbar a title
                        new ILNumerics.Drawing.Label(text: @"Error, (\vartheta)") {
                            Anchor = new PointF(-.4f,1.1f)
                        }
                    }
                }
                });


                pc.Axes.XAxis.Ticks.DefaultLabel.Font = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold);
                pc.Axes.YAxis.Ticks.DefaultLabel.Font = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold);
                pc.Axes.ZAxis.Ticks.DefaultLabel.Font = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold);
                //pc.Axes.XAxis.Ticks.DefaultLabel.Color = Color.White;
                //pc.Axes.YAxis.Ticks.DefaultLabel.Color = Color.White;
                //pc.Axes.ZAxis.Ticks.DefaultLabel.Color = Color.White;
                //pc.Axes.XAxis.Ticks.DefaultLabel.Fringe.Width = 0;
                //pc.Axes.YAxis.Ticks.DefaultLabel.Fringe.Width = 0;
                //pc.Axes.ZAxis.Ticks.DefaultLabel.Fringe.Width = 0;
                pc.Axes.XAxis.Label.Font = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold);
                pc.Axes.YAxis.Label.Font = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold);
                pc.Axes.ZAxis.Label.Font = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold);
                //pc.Axes.XAxis.Label.Color = Color.White;
                //pc.Axes.YAxis.Label.Color = Color.White;
                //pc.Axes.ZAxis.Label.Color = Color.White;
                //pc.Axes.XAxis.Label.Fringe.Width = 0;
                //pc.Axes.YAxis.Label.Fringe.Width = 0;
                //pc.Axes.ZAxis.Label.Fringe.Width = 0;
                pc.Rotation = Matrix4.Rotation(Vector3.UnitX, .8f)
                                                   * Matrix4.Rotation(Vector3.UnitZ, .6f);
                //ilPanel1.BackColor = Color.Black;
                pc.Projection = Projection.Perspective;
                pc.Plots.Clipping = null; 
                pc.Lines.Visible = false;
                pc.Axes.Visible = false; 

                ilPanel1.Configure();
                ilPanel1.Refresh();
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            var pc = ilPanel1.SceneSyncRoot.First<PlotCube>();
            if (pc != null) {
                bool state = ((CheckBox)sender).Checked;
                pc.Lines.Visible = state;
                pc.Axes.Visible = state;
                pc.Plots.Clipping = (state) ? new ClipParams() : null; 
                pc.Configure();
                ilPanel1.Refresh();
            }
        }


        private class Computation {
            internal static RetArray<double> Generate1DData(int len, OutArray<double> Y) {
                using (Scope.Enter()) {
                    Array<double> X = linspace(-10, 10, len);
                    Y.a = sin(X / 2) * 3.1 + cos(X / 5) * -4.1;
                    return X;
                }
            }
        }

    }
}

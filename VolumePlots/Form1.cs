using System;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace VolumePlots {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {

        }

        private class Computation {

            public static RetArray<float> CreateData3D(int len, OutArray<float> X, OutArray<float> Y, OutArray<float> Z) {
                using (Scope.Enter()) {
                    Array<float> rng = linspace<float>(-5, 5, len);
                    Z.a = 1; Y.a = 1;
                    X.a = meshgrid(rng, rng, rng, Y, Z);

                    // apply the function
                    return sin(X * X) + cos(Y) + cos(Z);
                }
            }
            public static RetArray<float> CreateData3DVec(int len, OutArray<float> X, OutArray<float> Y, OutArray<float> Z) {
                using (Scope.Enter()) {
                    Array<float> rng = linspace<float>(-5, 5, len);
                    Z.a = 1; Y.a = 1;
                    X.a = meshgrid(rng, rng, rng, Y, Z);

                    // apply the function
                    Array<float> ret = zeros<float>(len, len, len, 3);
                    ret[full, full, full, 0] = sin(X) * cos(Y) * cos(Z);
                    ret[full, full, full, 1] = cos(.5f * X * X) * cos(.5f * Y * Z) * sin(Z * X * Y);
                    ret[full, full, full, 2] = cos(X / 2f) * cos( X * Z * 3) * cos(Z);
                    return ret * 0.5f; 
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            // surface slices colormapped
            int len = 25;
            Array<float> X = 1, Y = 1, Z = 1;
            Array<float> D = Computation.CreateData3D(len, X, Y, Z);

            // plotting 
            var pc = (ilPanel1.Scene = new Scene()).Add(new PlotCube(twoDMode: false));
            // plot slices
            // define data range
            var dataRange = Tuple.Create(-2f, 2f);
            for (int i = 0; i < len; i += len / 4) {
                var sf = pc.Add(new Surface(0.0));
                sf.UpdateColormapped(Z[full, full, i].Concat(X[full, full, i], 2).Concat(Y[full, full, i], 2), dataValues: D[full, full, i], dataRange: dataRange);
            }
            for (int i = 0; i < len; i += len / 4) {
                var sf = pc.Add(new Surface(0.0));
                sf.UpdateColormapped(
                    Z[full, i, full].Reshape(len, len, 1)
                    .Concat(X[full, i, full].Reshape(len, len, 1), 2)
                    .Concat(Y[full, i, full].Reshape(len, len, 1), 2),
                    dataValues: D[full, i, full].Reshape(len, len),
                    dataRange: dataRange);
            }
            pc.Projection = ILNumerics.Drawing.Projection.Perspective;
            pc.Configure();
            ilPanel1.Refresh(); 

        }

        private void button2_Click(object sender, EventArgs e) {
            // vector fields
            int len = 20;
            Array<float> X = 1, Y = 1, Z = 1;
            Array<float> D = Computation.CreateData3DVec(len, X, Y, Z);
            var minmaxRange = Tuple.Create(-0.5f, .5f);
            var cm = new Colormap(Colormaps.Jet); 
            var sc = ilPanel1.Scene = new Scene(); 
            var pc = sc.Add(new PlotCube(twoDMode: false));
            for (int i = 0; i < len; i += len / 4) {
                var vp = pc.Add(new VectorPlot());
                vp.Update(
                    reshape(X[full, full, i], len, len),
                    reshape(Y[full, full, i], len, len),
                    reshape(Z[full, full, i], len, len),
                    reshape(D[full, full, i, 0], len, len),
                    reshape(D[full, full, i, 1], len, len),
                    reshape(D[full, full, i, 2], len, len),
                    minmaxRange, cm);
            }
            for (int i = 0; i < len; i += len / 3) {
                var vp = pc.Add(new VectorPlot());
                vp.Update(
                    reshape(X[full,i, full], len, len),
                    reshape(Y[full,i, full], len, len),
                    reshape(Z[full,i, full], len, len),
                    reshape(D[full,i, full, 0], len, len),
                    reshape(D[full,i, full, 1], len, len),
                    reshape(D[full,i, full , 2], len, len),
                    minmaxRange, cm);
            }
            pc.Add(new Colorbar() { ColormapProvider = new StaticColormapProvider(cm, minmaxRange.Item1, minmaxRange.Item2) }); 
            pc.Reset(); 
            ilPanel1.Configure();
            ilPanel1.Refresh(); 
        }
    }
}

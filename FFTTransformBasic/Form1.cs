using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace FFTTransformBasic {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {

            // replace with your own wav file here: 
            var file = @"No23_Record_17.03.2016_18.20.39_.wav";
            using (var fstream = File.Open(file, FileMode.Open)) {
                Array<short> samples_raw = loadBinary<short>(fstream, 1, 1054 * 270, 1);
                // cut 'garbage' (header) from the start and convert to double. Production: use wav input reader lib!
                Array<float> samples = tosingle(samples_raw["100:end"]);
                // plot it
                var pc1 = ilPanel1.Scene.Add(new PlotCube("time plot") {
                    new LinePlot(samples)
                });
                pc1.Add(new Title("time domain"));
                pc1.Axes.XAxis.Label.Text = "time [sample #]";
                pc1.Axes.YAxis.Label.Text = "Level [Abs]";
                // size and position the plot cube
                pc1.ScreenRect = new RectangleF(0, 0, 1, 0.33f);

                // compute all signals: FFT first
                Array<fcomplex> fftresult = fft(samples);

                // The fft variable now holds the intensities for the frequencies as
                // cosine (real part) and sine (imaginary part). 
                var pc2 = ilPanel1.Scene.Add(new PlotCube() {
                    new LinePlot(real(fftresult)),
                    new LinePlot(imag(fftresult)),
                    new Legend("real", "imag")
                });
                pc2.Add(new Title("freq domain"));
                pc2.Axes.XAxis.Label.Text = "frequency [bin #]";
                pc2.Axes.YAxis.Label.Text = "Level [Abs]";
                pc2.ScreenRect = new RectangleF(0, 0.33f, 1, 0.33f);

                var pc3 = ilPanel1.Scene.Add(new PlotCube() {
                    new LinePlot(abs(fftresult)),
                    new LinePlot(atan2(imag(fftresult), real(fftresult))),
                    new Legend("magnitude", "angle")
                });
                pc3.Add(new Title("freq domain"));
                pc3.Axes.XAxis.Label.Text = "frequency [bin #]";
                pc3.Axes.YAxis.Label.Text = "Mag [log]";
                pc3.ScaleModes.YAxisScale = AxisScale.Logarithmic;
                pc3.ScreenRect = new RectangleF(0, 0.66f, 1, 0.33f);


            }
        }
    }
}

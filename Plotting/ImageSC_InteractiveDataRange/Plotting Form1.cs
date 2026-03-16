using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace WindowsFormsApplication2 {
    public partial class Plotting_Form1 : Form {

        public Plotting_Form1() {
            InitializeComponent();
        }

        // Initial plot setup, modify this as needed
        private void ilPanel1_Load(object sender, EventArgs e) {

            Array<float> A = tosingle(SpecialData.terrain);

            var scene = new Scene { 
                new PlotCube(twoDMode: false) {
                    new ImageSCPlot(A, Colormaps.Hot, new Tuple<float,float>(3000,6000)) {
                        new Colorbar()
                    }

                }
            };

            ilPanel1.Scene = scene; 

            // configure trackbars
            float min, max; 
            A.GetLimits(out min, out max);
            trackBar1.Maximum = (int)(max * 2);
            trackBar2.Maximum = (int)(max * 2); 
            trackBar1.Minimum = (int)(min / 2);
            trackBar2.Minimum = (int)(min / 2); 
            trackBar1.ValueChanged += (s_, a_) => { UpdateDataRange(trackBar1.Value, trackBar2.Value); };
            trackBar2.ValueChanged += (s_, a_) => { UpdateDataRange(trackBar1.Value, trackBar2.Value); };
        }

        private void UpdateDataRange(float p1, float p2) {
            var imageSC = ilPanel1.Scene.First<ImageSCPlot>();
            if (imageSC != null) {
                imageSC.MinMaxDataRange = Tuple.Create(p1, p2);
                imageSC.Configure();
                ilPanel1.Refresh();
            }
        }

    }
}

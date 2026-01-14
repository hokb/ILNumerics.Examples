using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;

namespace Box_Plot
{
    public partial class Plotting_Form1 : Form
    {

        public Plotting_Form1()
        {
            InitializeComponent();
        }

        // Initial plot setup, modify this as needed
        private void ilPanel1_Load(object sender, EventArgs e)
        {
            // Generate data: Each column contains a data set 
            Array<float> Data01 = ILMath.tosingle(ILMath.randn(100, 3));
            Array<float> Data02 = ILMath.tosingle(ILMath.randn(100, 3)) + 1;
            Array<float> Data03 = ILMath.tosingle(ILMath.randn(100, 3));
            Array<float> Data04 = ILMath.tosingle(ILMath.randn(100, 3));

            // Generate Corresponding Horizontal Positions: Each array contains three elements
            Array<float> X01 = new float[] { 1, 2.25f, 3.5f };
            Array<float> X02 = X01 + 0.25f;
            Array<float> X03 = X01 + 0.5f;
            Array<float> X04 = X01 + 0.75f;

            // Create Plot Cube
            var plotCube = ilPanel1.Scene.Add(new PlotCube());
            // Create Box Plots
            // Plot first group of data sets and specify colors
            plotCube.Add(new BoxPlot(X01.T, Data01, fillAreaColor: Color.FromArgb(255, 105, 89, 205),
                frameColor: Color.FromArgb(255, 71, 60, 139), whiskerColor: Color.FromArgb(255, 71, 60, 139),
                pointColor: Color.FromArgb(255, 71, 60, 139), medianColor: Color.FromArgb(255, 71, 60, 139),
                whiskerWidth: 2, medianWidth: 2, frameWidth: 2, boxWidth: 0.1));

            // Plot second group of data sets and specify colors
            plotCube.Add(new BoxPlot(X02.T, Data02, fillAreaColor: Color.FromArgb(255, 24, 116, 205),
                frameColor: Color.FromArgb(255, 16, 78, 139), whiskerColor: Color.FromArgb(255, 16, 78, 139),
                pointColor: Color.FromArgb(255, 16, 78, 139), medianColor: Color.FromArgb(255, 16, 78, 139), 
                whiskerWidth: 2, medianWidth: 2, frameWidth: 2, boxWidth: 0.1));

            // Plot third group of data sets and specify colors
            plotCube.Add(new BoxPlot(X03.T, Data03, fillAreaColor: Color.FromArgb(255, 0, 197, 205),
                frameColor: Color.FromArgb(255, 0, 134, 139), whiskerColor: Color.FromArgb(255, 0, 134, 139),
                pointColor: Color.FromArgb(255, 0, 134, 139), medianColor: Color.FromArgb(255, 0, 134, 139), 
                whiskerWidth: 2, medianWidth: 2, medianStyle: DashStyle.Dotted, frameWidth: 2, boxWidth: 0.1));

            // Plot fourth group of data sets and specify colors
            plotCube.Add(new BoxPlot(X04.T, Data04, fillAreaColor: Color.FromArgb(255, 102, 205, 170),
                frameColor: Color.FromArgb(255, 69, 139, 116), whiskerColor: Color.FromArgb(255, 69, 139, 116),
                pointColor: Color.FromArgb(255, 69, 139, 116), medianColor: Color.FromArgb(255, 69, 139, 116), 
                whiskerWidth: 2, medianWidth: 2, frameWidth: 2, boxWidth: 0.1));

            plotCube.MouseClick += (send, args) => {
                if (!args.DirectionUp) return; 
                var bp = args.Target?.Parent.FirstUp<BoxPlot>(); 
                if (bp != null) {
                    bp.Update(ILMath.ones<float>(1, 10));
                    bp.Configure();
                    args.Refresh = true;
                }
            };

            this.Text = "Box Plots Example"; 

        }


    }
}

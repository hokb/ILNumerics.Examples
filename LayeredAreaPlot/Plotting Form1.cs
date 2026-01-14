using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using ILNumerics.Toolboxes;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace Auto_Resolution
{
    public partial class Plotting_Form1 : Form
    {
        public Plotting_Form1() {
            InitializeComponent();
            ilPanel1 = new ILNumerics.Drawing.Panel();
            ilPanel1.Dock = DockStyle.Fill;
            ilPanel1.Load += new EventHandler(ilPanel1_Load);
            Controls.Add(ilPanel1); 
        }

        private ILNumerics.Drawing.Panel ilPanel1;

        // Initial plot setup, modify this as needed
        private void ilPanel1_Load(object sender, EventArgs e)
        {
            Array<float> Y = new float[] { 1, 1, 1, 2, 2, 3, 4, 5, 20, 20, 11, 9, 9, 4, 3, 2, 1, 5, 15, 12, 8, 8, 3, 2, 2, 2, 4, 5, 5, 12, 12, 9, 3, 2, 2, 1, 1 };
            Array<float> Xn = linspace<float>(0, Y.S[0] - 1, 200);
            Array<float> Yn = Interpolation.interp1(Y, Xn: Xn, method: InterpolationMethod.linear).T;
            Yn = repmat(Yn + tosingle(rand(1, 200)) * 1.5f, 3, 1);
            Yn = vector(1.5f, 2.25f, 3.0f) * Yn;
            Array<int> transparency = new int[] { 155, 205, 255 };
            // Create Scene 
            var scene = new Scene();
            // Create Plot Cube
            var plotCube = scene.Add(new PlotCube(twoDMode: false));
            // Create Stacked Area Plot
            var stackedAreaPlot = plotCube.Add(new StackedAreaPlot(Xn, Yn, isLayeredAreaPlot: true, fillColor: Color.Beige, spacing: 2,
                transparency: transparency, lineColor: Color.CornflowerBlue, lineWidth: 2));

            // Get IDs of Area Plots
            Array<int> areaPlotID = zeros<int>(3, 1);
            var i = 0;
            foreach (var areaPlot in stackedAreaPlot.Find<FillArea>())
            {
                areaPlotID[i] = areaPlot.ID;
                i++;
            }
            // Change Area Color of Second Area Plot
            stackedAreaPlot.FindById<FillArea>((int)areaPlotID[1]).Fill.Color = Color.CornflowerBlue;
            // Change Line Color of First Area Plot
            stackedAreaPlot.FindById<FillArea>((int)areaPlotID[0]).Border.Color = Color.OrangeRed;
            // Change Line Style of First Area Plot
            stackedAreaPlot.FindById<FillArea>((int)areaPlotID[0]).Border.DashStyle = DashStyle.Dotted;
            ilPanel1.Scene = scene;

        }
    }
}

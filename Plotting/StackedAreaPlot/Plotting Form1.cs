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

        public Plotting_Form1()
        {
            InitializeComponent();
        }

        // Initial plot setup, modify this as needed
        private void ilPanel1_Load(object sender, EventArgs e)
        {
            // Generate Data
            Array<float> Y = new float[] { 0, 0.125f, 0.25f, 0.5f, 1, 2, 2, 1.5f, 2, 3, 4, 4, 3.5f, 5, 5.5f, 5, 4, 4,
                7, 7, 5, 4, 4.5f, 5, 5, 4, 3.5f, 3.5f, 3, 2.5f, 3, 3, 2, 1, 0.75f, 0.75f, 0.5f, 0.25f, 0.125f, 0.125f, 0 };
            Y = repmat(Y, 1, 5) + tosingle(rand(41, 5));
            Array<float> Xn = linspace<float>(0, Y.S[0] - 1, 200);
            Array<float> Yn = Interpolation.interp1(Y, Xn: Xn, method: InterpolationMethod.spline).T;
            // Create Scene
            var scene = new Scene();
            // Create Plot Cube
            var plotCube = scene.Add(new PlotCube(twoDMode: false));
            // Create Stacked Area Plot
            var stackedAreaPlot = plotCube.Add(new StackedAreaPlot(Xn, Yn, colormap: Colormaps.Summer));
            ilPanel1.Scene = scene;

            Text = "Stacked Area Plot Example"; 
        }
    }
}

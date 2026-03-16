using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;

namespace CustomColorMap
{
    public partial class Plotting_Form1 : Form
    {

        public Plotting_Form1()
        {
            InitializeComponent();
            ilPanel1 = new ILNumerics.Drawing.Panel();
            ilPanel1.Dock = DockStyle.Fill;
            ilPanel1.Load += ilPanel1_Load;
            Controls.Add(ilPanel1); 
        }

        private ILNumerics.Drawing.Panel ilPanel1;

        // Initial plot setup, modify this as needed
        private void ilPanel1_Load(object sender, EventArgs e)
        {
            // Create scene
            var scene = new Scene();
            // Generate Data
            Array<float> XYZ = new float[,] { { 1, 2, 3, 4, 2, 3, 4, 3, 4, 2, 3, 6, 7, 8, 7, 8, 9, 10, 8, 9, 10, 7, 8, 9, 14, 15, 16, 15, 16, 17, 16, 17, 14, 15, 16, 17 },
                { 1, 1, 1, 1, 2, 2, 2, 3, 3, 4, 4, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 9, 10, 10, 10, 10 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
            Array<float> Trait = new float[] { 1, 1, 2, 1, 2, 2, 2, 1, 2, 3, 3, 3, 3, 2, 3, 4, 4, 4, 3, 4, 3, 4, 4, 5, 4, 5, 5, 4, 5, 5, 5, 6, 6, 5, 5, 6 };
            Array<float> GoldenColors = new float[,] { { 0, 255, 222, 173, 255 }, { 0.3f * 255, 188, 143, 143, 255 }, { 0.6f * 255, 210, 105, 30, 255 }, { 255, 128, 0, 0, 255 } };
            // Create Colormap
            var CM = new Colormap(Colormaps.Custom);
            // Assign Data
            CM.Data = GoldenColors / 255;
            // Get Min and Max values
            float min, max;
            Trait.GetLimits(out min, out max);
            // Map Values to Colors
            Array<float> Colors = CM.Map(Trait, Tuple.Create(min, max)).T;
            // Create Plot Cube
            var plotCube = scene.Add(new PlotCube());
            // Change Axes Labels
            plotCube.Axes.XAxis.Label.Text = "Width [m]";
            plotCube.Axes.YAxis.Label.Text = "Height [m]";
            // Create Points
            var points = plotCube.Add(new Points());
            // Set Positions
            points.Positions.Update(XYZ.T);
            // Set Size
            points.Size = 10;
            // Set Individual Colors
            points.Colors = Colors;
            // Set Common Color to null
            points.Color = null;
            // Create Color Bar
            var colorBar = plotCube.Add(new Colorbar());
            // Provide Color Bar with information
            colorBar.ColormapProvider = new StaticColormapProvider(CM, min, max);
            // Create label for color bar
            var colorBarLabel = colorBar.Add(new ILNumerics.Drawing.Label("Length [m]"));
            // Determine relative position of label 
            colorBarLabel.Position = new Vector3(0.5f, -0.05f, 0);
            
            ilPanel1.Scene = scene;

        }

    }
}

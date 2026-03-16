using System;
using System.Drawing;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Controls;
using ILNumerics.Drawing.Plotting;

namespace AxisLinesExample {
    /// <summary>
    /// A simple plot cube example, showing how to configure axis lines, tick lines, plot cube lines (width and color). 
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // add some example data (2D)
            Array<float> a = SpecialData.sincos1Df(150, 2.0);
            var plotCube = ilPanel1.Scene.Add(new PlotCube(twoDMode: false) { 
                new LinePlot(a.T, lineColor: Color.SaddleBrown) 
            });

            // configure X axis grid 
            var axis = plotCube.Axes[0]; 
            axis.GridMinor.Visible = false; 
            axis.GridMajor.Color = Color.Red; 
            axis.GridMajor.DashStyle =  DashStyle.PointDash;

            // configure X axis ticks
            axis.Ticks.Color = Color.Green;
            axis.Ticks.Width = 3;           // thickness of the ticks
            axis.Ticks.TickLength = 1;      // as fraction of default font height

            // configure plot cube lines 
            plotCube.Lines.Color = Color.RoyalBlue;
            plotCube.Lines.Width = 3; 

            // one could even color each plot cube corner individually: 
            //plotCube.Lines.Color = null;
            //plotCube.Lines.Colors = new float[,] {
            //    {0,0,1}, {0,1,0}, {1,0,0}, {0,1,1}, {1,0,1}, {1,1,0}, {1,1,1}, {0,0,0} };

            // configure Y axis
            axis = plotCube.Axes[AxisNames.YAxis];
            axis.GridMinor.Visible = false;
            axis.GridMajor.Width = 3;
            axis.GridMajor.Color = Color.FromArgb(0, 255, 12);
        }
    }
}
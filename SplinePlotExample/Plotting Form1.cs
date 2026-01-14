using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Toolboxes;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;


namespace SplinePlotVisualize {
    
    // This example shows how to use ILSplinePlot
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }
                        
        private void ilPanel1_OnLoad(object sender, EventArgs e) 
        {
            
            // Define some sample data: Points array values            
            Array<float> Points = tosingle(rand(1, 10));            

            // Create the scene to plot
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false){

                // Add a ILLinePlot, straight line connected given values, without markers
                new LinePlot(Points, lineColor: Color.Blue, markerStyle: MarkerStyle.Dot),

                // Add ILSplinePlot, smooth spline connects given values and draws original markers
                new SplinePlot(Points, lineColor: Color.Red, lineWidth:3, markerStyle: MarkerStyle.Dot, resolution:null, useSplinePathFor1D:false),
                
                // Add a legend
                new Legend("LinePlot", "SplinePlot")
            });
                        
        }


        /// <summary>
        /// Event listener for Radiobutton on form. Updates plot points on change
        /// </summary>        
        private void radioButton3_CheckedChanged(object sender, EventArgs e) 
        {
            // Define another one dataset of points
            Array<float> Points = linspace<float>(0, pif * 6, 20);
            Points[2,full] = Points.C;
            Points[1,full] = 5 + 5 * sin(Points[2,full]);
            Points[0,full] = 10 + 5 * cos(Points[2,full]);

            // Update points positions
            ilPanel1.Scene.First<SplinePlot>().Resolution = 10;
            ilPanel1.Scene.First<SplinePlot>().Update(Points);
            ilPanel1.Scene.First<LinePlot>().Update(Points);

            // Reset ILPlotCube and Refresh defined ILPanel on Form
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) {
            Array<float> Points = tosingle(rand(1,10));
            
            ilPanel1.Scene.First<SplinePlot>().Resolution = null;
            ilPanel1.Scene.First<SplinePlot>().Update(Points);
            ilPanel1.Scene.First<LinePlot>().Update(Points);

            // Reset ILPlotCube and Refresh defined ILPanel on Form
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        /// <summary>
        /// Event listener for Radiobutton on form. Updates plot points on change
        /// </summary>
        private void radioButton2_CheckedChanged(object sender, EventArgs e) 
        {            
            // Define another one dataset of points
            Array<float> Points = zeros<float>(2, 7);
            Points[0,full] = linspace<float>((float)-pi, (float)pi, 7);
            Points[1,full] = sin(Points[0,full]);
            
            ilPanel1.Scene.First<SplinePlot>().Resolution = null;
            ilPanel1.Scene.First<SplinePlot>().Update(Points);
            ilPanel1.Scene.First<LinePlot>().Update(Points);

            // Reset ILPlotCube and Refresh defined ILPanel on Form
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

       
     
        /// <summary>
        /// Eventlistener for Trackbar. Changes ILSplinePlot resolution.
        /// </summary>        
        private void trackBar1_ValueChanged(object sender, EventArgs e) {
            
            // Get a trackBar current value and set it to ILSplinePlot resolution. 
            // Spline will be updated automatically
            ilPanel1.Scene.First<SplinePlot>().Resolution = trackBar1.Value;

            // Reset ILPlotCube and Refresh defined ILPanel on Form
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

                
       
    }
}

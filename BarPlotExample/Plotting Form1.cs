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


namespace BarPlotVisualize {

    // This example shows how to use BarPlot
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }

        Array<double> Points, X;
        private void ilPanel1_OnLoad(object sender, EventArgs e) 
        {
            Points = ILMath.rand(3, 5);
            X = ILMath.counter<double>(10, 1, 1, 3);
            
            // setup plot cube            
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false)
            {
                // add bar plot
                new BarPlot(Points,X) 
                {                                        
                    Ticks = {
                       new Tick(10, "") { Label = new ILNumerics.Drawing.Label("Measures #1"){ Rotation = 0.0f, Anchor = new PointF(0.5f, -0.5f), Color = Color.Red } },
                       new Tick(11, "") { Label = new ILNumerics.Drawing.Label("Measures #2"){ Rotation = 0.0f, Anchor = new PointF(0.5f, -0.5f), Color = Color.Green } },
                       new Tick(12, "") { Label = new ILNumerics.Drawing.Label("Measures #3"){ Rotation = 0.0f, Anchor = new PointF(0.5f, -0.5f),Color = Color.Blue } },                       
                    }
                },                                 
                new Legend("Group1","Group2","Group3","Group4","Group5")

            });                        
            
        }

        private void button1_Click(object sender, EventArgs e) 
        {
            // Update bar plot values
            Points = ILMath.rand(Points.shape);
            ilPanel1.Scene.First<BarPlot>().Update(Points,X);                                    

            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // change base value
            ilPanel1.Scene.First<BarPlot>().BaseValue = (float)(trackBar1.Value * 0.1f);
            ilPanel1.Scene.First<BarPlot>().Configure();
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            // change bar width
            ilPanel1.Scene.First<BarPlot>().BarWidth = (float)(trackBar2.Value * 0.1f);
            ilPanel1.Scene.First<BarPlot>().Configure();
            
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // change bar drawing style
            ilPanel1.Scene.First<BarPlot>().Style = BarPlotStyle.grouped;
            ilPanel1.Scene.First<BarPlot>().Configure();

            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // change bar drawing style
            ilPanel1.Scene.First<BarPlot>().Style = BarPlotStyle.stacked;
            ilPanel1.Scene.First<BarPlot>().Configure();
            
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // change bar drawing style
            ilPanel1.Scene.First<BarPlot>().Style = BarPlotStyle.hist;
            ilPanel1.Scene.First<BarPlot>().Configure();
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // change bar drawing style
            ilPanel1.Scene.First<BarPlot>().Style = BarPlotStyle.detached;
            ilPanel1.Scene.First<BarPlot>().Configure();
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // change bar drawing style
            if (checkBox1.Checked) ilPanel1.Scene.First<BarPlot>().Draw3D = true;                
                else ilPanel1.Scene.First<BarPlot>().Draw3D = false;

            ilPanel1.Scene.First<BarPlot>().Configure();
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // change bar drawing style
            if (checkBox2.Checked) ilPanel1.Scene.First<BarPlot>().Horizontal = true;
            else ilPanel1.Scene.First<BarPlot>().Horizontal = false;
            ilPanel1.Scene.First<BarPlot>().Configure();
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }
    }
}

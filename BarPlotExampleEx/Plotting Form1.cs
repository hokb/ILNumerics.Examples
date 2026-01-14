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


namespace BarPlotVisualize
{

    // This example shows how to use ILBarPlotEx
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void ilPanel1_OnLoad(object sender, EventArgs e)
        {
            // create a scene with plot cube
            ilPanel1.Scene.Add(new PlotCube(tag: "barplotcube", twoDMode: false)
            {
                // add new ILBarPlotEx object to scene and customize parameters
                new BarPlotEx(Z: SpecialData.sincf(15, 15, 1.5f))
                {
                    // set initial bar width
                    BarWidth = 1.0f,                    
                                        
                    // set colormode as Colormapped
                    ColorMode = BarPlotEx.ColorModes.Colormapped,
                    
                    // add color bar to visualize colormap
                    Children = { new Colorbar() }
                }
            });

            var pc = ilPanel1.Scene.First<PlotCube>("barplotcube");
            pc.Rotation = Matrix4.Rotation(new Vector3(0, 0, 1), 0.3f);
            pc.RotationMethod = RotationMethods.AltitudeAzimuth; 

            // prepare a list of posible colormaps for user to select using ComboBox
            for (int i = 0; i < Enum.GetNames(typeof(Colormaps)).Length; i++)
            {
                comboBox1.Items.Add(Enum.GetName(typeof(Colormaps), i));
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // set bar width
            ilPanel1.Scene.First<BarPlotEx>().BarWidth = (float)(trackBar1.Value * 0.1f);
            ilPanel1.Scene.First<BarPlotEx>().Configure();

            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            // set base value width
            ilPanel1.Scene.First<BarPlotEx>().BaseValue = (float)(trackBar2.Value * 0.1f);
            ilPanel1.Scene.First<BarPlotEx>().Configure();

            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // set colormode state
            ilPanel1.Scene.First<BarPlotEx>().ColorMode = BarPlotEx.ColorModes.Colormapped;
            ilPanel1.Scene.First<BarPlotEx>().Configure();

            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            // set colormode state
            ilPanel1.Scene.First<BarPlotEx>().ColorMode = BarPlotEx.ColorModes.Solidmapped;
            ilPanel1.Scene.First<BarPlotEx>().Configure();

            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // set a colormap
            ilPanel1.Scene.First<BarPlotEx>().Colormap = (Colormaps)Enum.ToObject(typeof(Colormaps), comboBox1.SelectedIndex);
            ilPanel1.Scene.First<BarPlotEx>().Configure();

            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }
    }
}
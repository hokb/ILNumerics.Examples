using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;


namespace LogAxisTicks {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            //ILNumerics.Settings.OpenGL31_FIX_GL_CLIPVERTEX = true;
            
            Array<float> A = tosingle(linspace(1.0, 20.0, 10));

            ilPanel1.Scene.Add(new PlotCube(twoDMode:false) {
                Children = { 
                    new LinePlot(exp(A)) 
                },
                ScaleModes = {
                    YAxisScale = AxisScale.Logarithmic,
                    //XAxisScale = AxisScale.Logarithmic,
                },
                Axes = {
                    XAxis = { Visible = true },
                    YAxis = { Ticks = { TickLength = -1 }}
                },
                //Projection = Projection.Perspective
            });

        }

        private void button1_Click(object sender, EventArgs e) {
            var pc = ilPanel1.Scene.First<PlotCube>();
            if (pc.ScaleModes.YAxisScale == AxisScale.Linear) {
                pc.ScaleModes.YAxisScale = AxisScale.Logarithmic;
            } else {
                pc.ScaleModes.YAxisScale = AxisScale.Linear; 
            }
            pc.Axes.YAxis.Label.Text = "Y Axis (" + pc.ScaleModes.YAxisScale + ")";
            ilPanel1.Refresh(); 
        }

        private void button2_Click(object sender, EventArgs e) {
            var pc = ilPanel1.Scene.First<PlotCube>();
            if (pc.ScaleModes.XAxisScale == AxisScale.Linear) {
                pc.ScaleModes.XAxisScale = AxisScale.Logarithmic;
            } else {
                pc.ScaleModes.XAxisScale = AxisScale.Linear;
            }
            pc.Axes.XAxis.Label.Text = "X Axis (" + pc.ScaleModes.XAxisScale + ")"; 
            ilPanel1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e) {
            if (ilPanel1.RendererType == RendererTypes.GDI) {
                ilPanel1.RendererType = RendererTypes.OpenGL;
            } else {
                ilPanel1.RendererType = RendererTypes.GDI;
            }
            button3.Text = ilPanel1.RendererType.ToString(); 
        }

        private void trackBar1_Scroll(object sender, EventArgs e) {
            var ticks = ilPanel1.Scene.First<PlotCube>().Axes.YAxis.Ticks;
            if (ticks != null) {
                ticks.TickLength = trackBar1.Value / 10f;
                ilPanel1.Refresh(); 
            }
        }

    }
}

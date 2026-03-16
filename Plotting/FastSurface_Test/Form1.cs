using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SurfaceType = ILNumerics.Drawing.Plotting.FastSurface;
//using SurfaceType = ILNumerics.Drawing.Plotting.ILSurface; 

namespace FastSurface_Test {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            A1500 = ILMath.localMember<float>();
            A500 = ILMath.localMember<float>(); 

        }

        SurfaceType m_surf;
         
        private readonly Array<float> A1500 , A500; 
        private void ilPanel1_Load(object sender, EventArgs e) {
            using (Scope.Enter()) {

                A500.a = SpecialData.sincf(500, 500, 10);
                A1500.a = SpecialData.sincf(1500, 1500, 10);

                m_surf = new SurfaceType(tag: "surface");

                ilPanel1.Scene.Add(new PlotCube(twoDMode: false) {
                    m_surf
                });
                button1_Click(this, null);
                ilPanel1.Scene.First<PlotCube>().Reset();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            var sw = new Stopwatch(); 
            sw.Start();            
            m_surf.Update(Z:A500, colormap: Colormaps.Hot);
            Text = "500²: " + sw.ElapsedMilliseconds.ToString() + "ms";
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor; 
            var sw = new Stopwatch();
            sw.Start();
            m_surf.Update(Z:A1500, colormap: Colormaps.Jet);
            Text = "1500²: " + sw.ElapsedMilliseconds.ToString() + "ms";
            ilPanel1.Scene.First<PlotCube>().Reset(); 
            ilPanel1.Refresh();
            Cursor = Cursors.Arrow; 

        }
    }
}

using ILNumerics;
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

namespace RemoveContentFromPlotCubeCSharp {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false)); 
            button1_Click(null, EventArgs.Empty);
            Text = "Add / Remove Content from PlotCube"; 
        }

        private void button1_Click(object sender, EventArgs e) {
            // modify the global scene: 
            var surf = ilPanel1.Scene.First<Surface>();

            if (surf != null) {
                // remove surface: use the parent node.
                surf.Parent.Remove(surf); 
            } else {
                // add new surface 
                using (Scope.Enter()) {
                    Array<float> A = SpecialData.sincf(30, 60);
                    ilPanel1.Scene.First<PlotCube>().Add(new Surface(A));
                }
            }
            ilPanel1.Scene.Configure();
            ilPanel1.Refresh(); 
        }
    }
}

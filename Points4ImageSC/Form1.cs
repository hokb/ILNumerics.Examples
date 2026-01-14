using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace WindowsFormsApplication2 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

    private void ilPanel1_Load(object sender, EventArgs e) {
        using (Scope.Enter()) {
            // some 'input matrix'
            Array<float> Z = SpecialData.sincf(40, 50);
            // do some reordering: prepare vertices
            Array<float> Y = 1, X = meshgrid(
                                arange<float>(1, Z.S[1]), 
                                arange<float>(1,Z.S[0]),
                                Y); 
            // reallocate the vertex positions matrix
            Array<float> pos = zeros<float>(3, X.S.NumberOfElements); 
            // fill in values
            pos[0,full] = X[full]; 
            pos[1,full] = Y[full]; 
            pos[2,full] = Z[full]; 

            // colormap used to map the values to colors
            Colormap cmap = new Colormap(Colormaps.Hot);
            // setup the scene
            ilPanel1.Scene.Add(new PlotCube {
                new Points() {
                    Positions = pos, 
                    Colors = cmap.Map(Z).T, 
                    Color = null
                }
            }); 
        }
    }
    }
}

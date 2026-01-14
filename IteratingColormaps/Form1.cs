using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Controls;
using ILNumerics.Algorithms;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace IteratingColormaps {
    /// <summary>
    /// The Iterating Colormaps Example shows how to create a surface from 3D data, show a colorbar, select a colormap and iterate the colormaps for the surface by utilizing two regular Windows.Forms buttons in a simple GUI. 
    /// </summary>
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // create some data: a möbius strip
            Array<double> X = 0, Y = 0, Z = 0; 
            SpecialData.moebius(15,5,10, X, Y, Z);

            // create plotcube and surface with colorbar
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) {
                new Surface(tosingle(Z),tosingle(X), tosingle(Y)) {
                    new Colorbar()
                }
            }); 
        }

        /// <summary>
        /// Handle Click events for the left button: Show previous colormap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e) {
            // replace current colormap with the previous in order from existing colormaps enumeration
            setColormap(-1);
        }
        /// <summary>
        /// Handle Click events for the right button: Show next colormap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e) {
            // replace current colormap with the next in order from existing colormaps enumeration
            setColormap(1);
        }

        private void setColormap(int p) {
            // get the surface from the scene
            var surf = ilPanel1.Scene.First<Surface>();
            if (surf != null) {
                // get the current colormap
                var cur = (int)surf.Colormap.Type;
                // increase / decrease
                int maxVal = Enum.GetNames(typeof(Colormaps)).Length;
                int newVal = (cur + p) % maxVal;
                if (newVal < 0) newVal = maxVal - 1; 
                // set the new colormap. surf.Colormaps expects an ILColormap. When assigning a value from 
                // the Colormaps.Enumeration it gets converted to ILColormap implicitly. 
                surf.Colormap = (Colormaps)newVal;
                // update the forms title
                Text = "Colormap: " + surf.Colormap.Type.ToString(); 
                // redraw: always call configure to commit all changes which potentially affect any 
                // rendering buffers. 
                surf.Configure();
                // cause immediate redraw of the panel
                ilPanel1.Refresh(); 
            }
        }
    }
}
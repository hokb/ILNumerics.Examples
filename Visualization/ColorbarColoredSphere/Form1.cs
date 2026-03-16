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

namespace ColorbarColoredSphere {
    /// <summary>
    /// This example creates a common sphere and colors its vertices according to their X coordinate 
    /// using a predefined colormap. A colorbar is used to indicate the mapping from color to data. 
    /// </summary>
    /// <remarks>The following concepts are shown in the example: Adding predefined shapes to the scene, 
    /// creating colormaps and using a colormap to color a shape's Colors buffer.</remarks>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            
            // configure a scene, here: a simple colored sphere
            var sphere = new Sphere();
            Array<float> pos = sphere.Fill.Positions.Storage;
            
            // we color the vertices according to their X coordinates 
            // by mapping the X values to colors in the jet colormap.
            var cm = new Colormap(Colormaps.Jet);
            
            // ILColormap provides the handy Map() function. It outputs 
            // a row of mapped color data for each entry in the input vector. 
            // We use those colors directly as vertex colors for the 
            // ILTriangles.Colors buffer. Since it expects a matrix of 4xn
            // we must transpose the result from Map().             
            sphere.Fill.Colors = cm.Map(pos["0;:"]).T;  // Must transpose!
            
            // Deactivate single color (it takes priority otherwise)
            sphere.Fill.Color = null;
            
            // Adding a colorbar: Our custom object (a sphere) does not know anything about 
            // colomaps nor how to handle a colorbar. Therefore, we simply use a 
            // static colormap provider. It keeps all information needed by the colorbar: 
            float min, max; 
            // We need the colorbar limits (here: min, max of the X coordinates) ... 
            pos["0;:"].GetLimits(out min, out max); 
            // ... and the actual colormap used
            var cmp = new StaticColormapProvider(cm, min, max); 

            // add objects to the scene
            ilPanel1.Scene.Camera.Add(sphere);
            // let's add a tripod so we see which direction goes along X 
            ilPanel1.Scene.Camera.Add(new Tripod());
            ilPanel1.Scene.Camera.Add(new Colorbar() { 
                ColormapProvider = cmp,
                // the following line will only be needed for ILNumerics version < 4.1
                //Axis = { Max = max, Min = min }
            });

            // configure the colobars label fonts
            ilPanel1.Scene.First<Colorbar>().First<Axis>().Ticks.DefaultLabel.Font = new Font("Courier", 8, FontStyle.Bold); 
            ilPanel1.Scene.Screen.First<ILNumerics.Drawing.Label>().Font = new Font("Arial", 12, FontStyle.Bold); 
        }
    }
}

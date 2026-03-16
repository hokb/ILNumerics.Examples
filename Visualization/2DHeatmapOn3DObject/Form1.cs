using ILNumerics;
using static ILNumerics.ILMath;
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

namespace _2DHeatmapOn3DObject {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // add cylinder
            ilPanel1.Scene.Camera.Add(new Group(scale: new Vector3(1,1,10)) { Shapes.Cylinder200 }); 

            // create heatmap, here: 5 x 5 values 
            Array<float> hm = new float[,] {
               {1, 2, 1, 2, 3}, // this will be the first column(!)
               {2, 3, 4, 3, 3},
               {2, 2, 4, 4, 2},
               {1, 2, 3, 2, 1},
               {2, 3, 5, 3, 2},
            };
            // create X and Y coordinates for the capacity sensors 
            Array<float> Y = 1, X = meshgrid(
                linspace<float>(-1, 1, 5),
                linspace<float>(-1, 1, 5),
                Y);  
            // create surface based heatmap and X, Y, color values 
            // Note that the Z,X and Y positions here are computed based on 
            // some trigonometric formulae. It would be fine to simply specify 
            // discrete positions for Z, X and Y coordinates.
            var surf = new Surface(Y + 2, 1.5f * sin(X), -cos(X) * 1.5f, hm);
            ilPanel1.Scene.Camera.Add(surf); 

            // for subsequent updates, one would use surf.UpdateColormapped() later ...

            // position the camera (optional) 
            ilPanel1.Scene.Camera.Position = new Vector3(-2, -9, 3);
            ilPanel1.Scene.Camera.LookAt = new Vector3(0, 0, 2);
            ilPanel1.Scene.Camera.Top = new Vector3(1, 0, 0); 

            // You might want to hide the wireframe? 
            // surf.Wireframe.Visible = false; 

            // You might want to use discrete color values instead of a colormap? 
            // surf.UpdateRGBA(RGBAcolors: [5 X 5 X 3] or [5 x 5 x 4]) for RGB(A) colors

        }

    }
}

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

            // generate a new scene
            var scene = new Scene();
            // create a new brick instance
            var brick = new Brick(borderColor: Color.Blue);
            // add the brick 100 times to the scene, apply translation / values
            for (int i = 0; i < 100; i++) {
                var newBrick = scene.Camera.Add(brick);
                // offset the new brick to order bricks in a matrix
                newBrick.Translate(i % 10, 0, -i / 10);
                // give each brick an individual value/color
                newBrick.Value = (double)rand(1); 
            }
            // add a colorbar 
            scene.Camera.Add(new Colorbar() {
                // we use a static colormap provided here
                ColormapProvider = new StaticColormapProvider(Colormaps.ILNumerics, 0, 1)
            }); 
            // place the camera
            scene.Camera.LookAt = new Vector3(5, 0, -5);
            scene.Camera.RotateX(Math.PI / 2); 

            // assign the whole scene to the ILPanel for rendering
            ilPanel1.Scene = scene; 
        }
    }
}

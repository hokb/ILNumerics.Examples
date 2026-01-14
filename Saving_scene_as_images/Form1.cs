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

namespace Saving_scene_as_images {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {

            //Image saving:
            // We fetch the current scene from the panel. Make sure, that the panel actually SHOWS the scene! 
            // If you have just assigned the scene to the panel, it is only shown, after the panel received a 
            // Paint event. In order to trigger the same actions done by the Paint event - but without actually triggering Paint - 
            // one can use: 
            //ilPanel1.Configure(0);
            //ilPanel1.Render(0);  

            Scene sceneR = ilPanel1.GetCurrentScene();

            var drv = new GDIDriver(1200, 800, sceneR);
            drv.Render();
            string filename = "shown.png"; 
            drv.BackBuffer.Bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);

            MessageBox.Show($"Image saved to: {System.IO.Path.Combine(Environment.CurrentDirectory, filename)}"); 

        }

        private void button2_Click(object sender, EventArgs e) {
            // We create a new scene and save it directly to file. No panel involved!

            var scene = new Scene();
            var cube = new PlotCube(twoDMode: false) 
            {
                new Surface(SpecialData.sincf(30,40), colormap: Colormaps.Jet)
                {
                    new Colorbar() 
                },
            };
            scene.Add(cube).Rotation = Matrix4.Rotation(Vector3.UnitZ, 0.1);
            scene.Screen.Add(new ILNumerics.Drawing.Label("This was not shown") {
                Position = new Vector3(0, 1, 0),
                Anchor = new PointF(0, 1),
                Color = Color.Red,
            });
            scene.Configure(); 

            //Image saving:
            var drv = new GDIDriver(1200, 800, scene);
            drv.Render();
            string filename = "not_shown.png";
            drv.BackBuffer.Bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);

            MessageBox.Show($"Image saved to: {System.IO.Path.Combine(Environment.CurrentDirectory, filename)}");

        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // creates the scene which is shown at startup

            var scene = new Scene();
            var cube = new PlotCube(twoDMode: false) 
            {
                new Surface(SpecialData.sincf(30,40))
                {
                    new Colorbar() 
                },
            };
            scene.Add(cube).Rotation = Matrix4.Rotation(Vector3.UnitZ, 0.1);
            scene.Screen.Add(new ILNumerics.Drawing.Label("This was shown") {
                Position = new Vector3(0, 1, 0),
                Anchor = new PointF(0, 1), 
                Color = Color.Red,
            }); 
            ilPanel1.Scene = scene; 

        }
    }
}

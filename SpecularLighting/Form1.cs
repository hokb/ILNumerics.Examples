using ILNumerics.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpecularLighting {
    /// <summary>
    /// The example shows a sphere with a specular light applied which shines slightly red.
    /// </summary>
    /// <remarks>This example demonstrates how to apply and modify light to filled objects. The color and shape of the specular reflection is
    /// modified, the position of the main (default) light in the scene is altered in order to move the reflection to a place where it is more obvious.</remarks>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // setup the scene
            ilPanel1.Scene.Camera.Add(new Sphere() {
                // light always works on an 'area' rather than on lines or points. The Fill property of the sphere gives access to 
                // the Triangles shape the spheres filled area is made of. 
                Fill = {
                    // this determines in which color the material reflects the specular part of the light 
                    SpecularColor = Color.Red,
                    // the size of the specular reflection, smaller values create a smaller reflection spot
                    Shininess = 0.17f
                },
                // some configuration for the wireframe, this does not affect the lighting 
                Wireframe = { Color = Color.FromArgb(10,Color.DarkGreen) }
            });
            // relocate the default light in order to have to specular reflection on the sphere move as well
            ilPanel1.Scene.First<PointLight>().Position = new Vector3(15,15,5); 
        }
    }
}

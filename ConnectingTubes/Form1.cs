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

namespace ConnectingTubes {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // set the background color to black
            ilPanel1.BackColor = Color.Black;

            var group1 = ilPanel1.Scene.Camera.Add(new Group("group1"));
            // adding a new shape to the scene creates a shallow (lazy) copy of the shape: 
            // https://ilnumerics.net/drawable-nodes-shapes.html 
            var cylinder1 = group1.Add(Shapes.Cylinder200);
            // Note how cylinder1 is now a new object: we can control its properties without affecting the 
            // Shapes.Cylinder200 source object! 
            cylinder1.Color = Color.Red;
            // the parent group of the shape allows to position / scale / rotate the shapes contained
            // https://ilnumerics.net/scene-graph-group-nodes-transformations.html 
            group1.Scale(1, 1, 5);
            group1.Translate(0, 2, 0);

            // 2nd cylinder object: 
            var group2 = ilPanel1.Scene.Camera.Add(new Group("group2"));
            var cylinder2 = group2.Add(Shapes.Cylinder200);
            cylinder2.Color = Color.Green;
            // the parent group of the shape allows to position / scale / rotate the shapes contained
            group2.Scale(1, 1, 5);
            group2.Translate(0, -2, 0);

            // for the connecting peace we use a torus (parametric surface). 
            Array<float> A = SpecialData.torus(2f, 1f);
            var torus = ilPanel1.Camera.Add(new Surface(A));
            // we only want half a torus: cutting away the right half
            // see: https://ilnumerics.net/nodes-clipping.html
            torus.Clipping = new ClipParams {
                Plane0 = new Vector4(-1, 0, 0, 0)
            };
            torus.Wireframe.Visible = false;
            torus.Fill.Color = Color.Yellow;
            torus.Fill.AutoNormals = true;
            // orienting the torus to connect both tubes
            torus.Rotate(new Vector3(0, 1, 0), pif / 2);

            // let's reuse the torus! We just re-add the same object to the scene. It will be (lazy) copied automatically: 

            var torus2 = ilPanel1.Camera.Add(torus);
            // the new torus object derives all properties from the source 'torus'. But we can change 
            // them individually now. Here we just use another position / orientation:     
            torus2.Rotate(new Vector3(0, -1, 0), pif);
            torus2.Rotate(new Vector3(0, 0, -1), pif / 2);
            torus2.Translate(-2, 2, 5);

            // individual objects within the copied object can be modified as well: 
            torus2.Fill.Color = Color.Blue; 

            // scale the whole scene (camera content) to make all better visible:
            ilPanel1.Camera.Scale(0.2f, .2f, .2f);  

        }
    }
}

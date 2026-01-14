using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ILNumerics.Globals;
using ILNumerics;
using ILNumerics.Drawing; 

namespace SimpleTriangles {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void panel1_Load(object sender, EventArgs e) {
            
            // we create a cube, assemble all sides out of triangles
            var cube = new Triangles();

            // define the corner point coordinates
            Array<float> pos = new float[,] {
                  {-1,+1,-1,+1,-1,+1,-1,+1},
                  {-1,-1,-1,-1,+1,+1,+1,+1},
                  {-1,-1,+1,+1,-1,-1,+1,+1}
                };

            // scaling via corrdinates 
            pos *= 0.5f;  
            // indices of coordinates are later used to connect the right points for each triangle
            Array<int> ind = new int[] { 0, 2, 1, 1, 2, 3, 0, 4, 2, 2, 4, 6, 2, 6, 3, 3, 6, 7, 3, 7, 1, 1, 7, 5, 1, 5, 0, 0, 5, 4, 5, 6, 4, 5, 7, 6 };

            // now configure the cube (triangles object)
            cube.Positions.Update(pos.T);
            cube.Indices.Update(ind);
            cube.Color = Color.Red;
            // a little light makes it more realistic
            // this simple form of lighting will still look a little ... 'strange', though, since normals of the sides are shared and no sharp corners will appear this way. 
            cube.AutoNormals = true;

            // add some edges as lines
            var wire = new LineStrip() {
                Color = Color.Black
            };
            wire.Positions.Update(pos); 
            // a linestrip defines a new line segment with every new index
            wire.Indices.Update(new int[] { 0, 1, 3, 2, 0, 4, 6, 2, 6, 7, 3, 7, 5, 1, 5, 4, 0 }); 

            // add the cube to the scene
            panel1.Scene.Camera.Add(cube); 
            panel1.Scene.Camera.Add(wire);
        }
    }
}

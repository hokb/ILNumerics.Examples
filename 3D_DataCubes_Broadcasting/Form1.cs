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
using static ILNumerics.ILMath;
using ILNumerics.Drawing; 

namespace _3D_DataCubes_Broadcasting {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {

            // general setup

            // number of cubes to create: 4x5x6
            int r1 = 4, r2 = 5, r3 = 6;

            for (int i1 = 0; i1 < r1; i1++) {
                for (int i2 = 0; i2 < r2; i2++) {
                    for (int i3 = 0; i3 < r3; i3++) {

                        // each cube's data are replicated from the singleton instance in the predefined Shapes collections. 
                        // All copies are placed into individual group nodes. The group allows individual configuration 
                        // for all visible cubes - while the cubes data are still shared! 
                        // The general properties (like position, size, color / opacity) are configured via the group node: 
                        var group = ilPanel1.Scene.Camera.Add(
                            new Group(translate: new Vector3(i1, i2, i3), scale: new Vector3(.8, .8, .8)) {
                                Shapes.UnitCubeWireframe
                            });

                        // acquire a (shallow / -> cheap) copy of a standard unit cube
                        var fill = Shapes.UnitCubeFilledLit;
                        // the copy allow some properties to be set - without affecting the other cubes (while all share the same vertex data!)
                        fill.Color = (i1 == 0 && i2 == 0) ? Color.FromArgb(255, Color.Green) : Color.FromArgb(230, Color.Gray);
                        
                        // colorize cubes of one index red
                        if (i3 == 5) {
                            fill.Color = Color.FromArgb(255, Color.Red);
                        }

                        group.Add(fill);

                    }
                }
            }
            ilPanel1.Scene.Camera.Projection = Projection.Perspective;

            // register "DoubleClick" event to position the camera & look at the center of the cubes
            ilPanel1.SceneSyncRoot.First<Camera>().MouseDoubleClick += Camera_MouseDoubleClick;
            ResetCamera();
        }

        private void ResetCamera() {
            // what we actually see is the synchronized copy of the scene! Interactive properties 
            // like Camera positions / orientations etc. must be set on the synchronized scene graph: 
            ilPanel1.SceneSyncRoot.First<Camera>().Position = new Vector3(2, 2.5, 25);
            ilPanel1.SceneSyncRoot.First<Camera>().LookAt = new Vector3(2, 2.5, 3);
            ilPanel1.SceneSyncRoot.First<Camera>().Top = new Vector3(0, 1, 0);
            // you may also want to (re)set Camera.Near / Far - they realize the current state of zooming.
        }

        private void Camera_MouseDoubleClick(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            ResetCamera();
            e.Cancel = true;  // prevents the default double click handling 
            e.Refresh = true;  // causes immediate redraw of the scene
        }
    }
}

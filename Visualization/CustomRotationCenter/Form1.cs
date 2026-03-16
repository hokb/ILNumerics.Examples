using ILNumerics;
using ILNumerics.Drawing;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomRotationCenter {
    
    /// <summary>
    /// This example shows the application of arbitrary rotational centers for scenes based on ILCamera.
    /// </summary>
    public partial class Form1 : Form {
        
        public Form1() {
            InitializeComponent();
        }

        // tag identifying the marker (ILPoints) shape in the scene
        private static readonly string PointsTag = "CenterMarkerPoint";
        // property helping us later to access the points shape used for marking the rot. center
        public Points Marker { get { return ilPanel1.Scene.First<Points>(PointsTag); } }
        
        // the setup functionality
        private void ilPanel1_Load(object sender, EventArgs e) {

            // add a points shape for marking the center
            ilPanel1.Scene.Camera.Add(new Points(tag: PointsTag)); 

            // construct the 'scene'
            ilPanel1.Scene.Camera.Add(new Sphere() { Wireframe = { Color = Color.FromArgb(020, Color.DarkGreen) } }); 

            // wire up the mouse handler 
            ilPanel1.Scene.Camera.MouseDown += panel_MouseDown;

            this.Shown += Form_GotFocus;
        }

        private void Form_GotFocus(object sender, EventArgs e) {
            MessageBox.Show("Click on the sphere with the left mouse button and while holding down the ALT key! This places a new rotational center and marks it with a red dot. Afterwards, rotating the sphere (left mouse and drag) rotates around this point.");
        }

        // everything happens in the mouse event handler: 
        void panel_MouseDown(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            // filter for event travel direction (priority), mouse button and alt key
            if (e.DirectionUp && e.Button == ILNumerics.Drawing.MouseButtons.Left && e.AltPressed) {
                // clicked on some shape? this example handles triangles only
                var target = e.Target as Drawable;
                if (target is Triangles) {  // <- this can be extended to all shapes by using Drawable or Shape here and adopting the center computation below
                    // get extended picking infor
                    var pick = ilPanel1.PickPrimitiveAt(target, e.Location);
                    // able to acquire the info? (This can fail in certain situations)
                    if (pick.NextVertex.IsEmpty) return;

                    // acquire barycentric coordinates of the top most visual
                    float s = pick.VerticesWorld.GetValue(0, 3);
                    float v = pick.VerticesWorld.GetValue(1, 3);
                    // The third barycentric coordinate is not stored. You can compute it easily 
                    // using the following formula: 
                    float t = 1 - s - v;
                    // interpolate: here, we interpolate the position on the triangle. (Adopt this for lines)
                    Array<float> interp = pick.VerticesWorld["0;0:2;0"] * s
                                          + pick.VerticesWorld["1;0:2;0"] * v
                                          + pick.VerticesWorld["2;0:2;0"] * t;
                    // move the marker to the picking point
                    Marker.Positions.Update(interp[":"]);

                    // set the new rotational center to this point
                    var rotCenter = new Vector3(interp.GetValue(0), interp.GetValue(1), interp.GetValue(2)); 
                    // the Camera.RotationCenter property is available from version 4.8
                    ilPanel1.SceneSyncRoot.First<Camera>().RotationCenter = rotCenter;

                    // Don't forget to re-configure after changes to any buffers! 
                    ilPanel1.Configure();
                    // redraw the scene after the event was processed
                    e.Refresh = true; 
                }
            }
        }

    }
}

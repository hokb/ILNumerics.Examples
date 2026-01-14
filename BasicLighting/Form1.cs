using ILNumerics.Drawing;
using ILNumerics; 
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace BasicLighting {

    /// <summary>
    /// Basic Lighting Example shows how lighting is configured and how it affects the scene. Also shows advanced configuration of interactivity with shapes and coordinate transforms.
    /// </summary>
    /// <remarks>A hemisphere and a gear shape is lit by an additional (to the default light) blue light. The position of the 
    /// blue light is marked with a fat blue point shape. The user can use the blue point to drag the light around. The dragging 
    /// works in 3D so any position can be reached by the drag operation. Double clicking on the blue point toogles the lights visible state.
    /// <para>The following concepts are demonstrated in the example: mouse interaction, custom mouse handlers, mouse handlers on
    /// scene graph hierarchies, adding and configuring point lights, transforming between coordinate systems, dynamic updates 
    /// to scene graph objects.</para></remarks>
    public partial class Form1 : Form { 
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // setup some objects
            ilPanel1.Scene.Camera.Add(new Group() {
                Shapes.Gear25Wireframe,Shapes.Gear25, Shapes.Hemisphere, Shapes.HemisphereWireframe
            });

            Lines.PolygonOffset = 0.0001f; 
            // Configure the gear fill area with a gray color
            ilPanel1.Scene.Find<Triangles>().ElementAt(0).Color = Color.Gray;
            // make the sphere slightly transparent gray
            ilPanel1.Scene.Find<Triangles>().ElementAt(1).Color = Color.FromArgb (200, Color.Gray);
            // give the hemisphere an red emissive color 
            ilPanel1.Scene.Find<Triangles>().ElementAt(1).Shininess = 0.04f;
            ilPanel1.Scene.Find<Triangles>().ElementAt(1).EmissionColor = Color.Peru; 
            // add another (blue) light
            var lightpos = new Vector3(0.9f,.5f,.5f); 
            var point = ilPanel1.Scene.Camera.Add(new Group("blueLightGroup") {
                // this adds a point shape which will mark the light position
                new Points("point") {
                    Positions = new float[,] { { lightpos.X, lightpos.Y, lightpos.Z } },
                    Color = Color.Blue, 
                    Size = 14,
                }, 
                // add the actual light
                new PointLight("blueLight") {
                    Color = Color.Blue,
                    Position = lightpos,
                    Intensity = 10
                }
            }); 
            // Register an event handler on the point shape, so that the user can drag the light around. 
            point.MouseMove += point_MouseMove;
            point.MouseDoubleClick += (_s, _e) => { 
                // toogle the lights visibility 
                _e.Target.Parent.First<PointLight>().Visible = !_e.Target.Parent.First<PointLight>().Visible;
                _e.Cancel = true; 
                _e.Refresh = true; 
            };
            MessageBox.Show("Drag the blue dot with the mouse to reposition the (invisible) blue light in the scene! Try to rotate the scene first."); 
        }

        void point_MouseMove(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (e.Button != ILNumerics.Drawing.MouseButtons.Left) return; 
            // compute how to move the light:
            // Transform the mouse movement from screen coords to model coords. 
            // Take current transform (camera rotation etc.) into account.
            var point = e.Target as Points;
            if (point != null) {
                // get the synched camera. This is the one affected by mouse interaction!
                var cam = ilPanel1.SceneSyncRoot.First<Camera>();
                // accumulate the transformation by the camera
                var camTF = cam.ProjectionTransform * cam.PositionTransform;
                // get the current point position in its model coordinates
                var pointPos = point.Positions.GetPositionAt(0); 
                // transform the clip coordinates to get the true Z component of the point
                var screen = camTF * pointPos;
                // calculate clip coordinate of current mouse position as if the mouse was next to the point
                Vector3 endClip = new Vector3(e.LocationF.X * 2f - 1,(1 - e.LocationF.Y) * 2f - 1, screen.Z);
                // get model coordinates of the mouse, we need the _inverse_ transform matrix
                var modelPos =  Matrix4.Invert(camTF) * endClip;
                // calculate distance mouse -> point, in model coords
                var curPos = pointPos + (modelPos - pointPos);
                // update the points model coordinates
                point.Positions.Update(0, 1, new float[] { curPos.X, curPos.Y, curPos.Z });
                // update the light position
                point.Parent.First<PointLight>().Position = curPos; 
                // commit changes for rendering
                point.Configure();
            }   
            // cause immediate redraw
            e.Refresh = true;
            // deactivate default mouse handler
            e.Cancel = true; 
        }
    }
}

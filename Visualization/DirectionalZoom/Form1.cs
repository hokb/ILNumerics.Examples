using System;
using System.Windows.Forms;
using ILNumerics; 
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.Globals;
using static ILNumerics.ILMath;

namespace ILNumerics.Examples.DirectionalZoom {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void panel2_Load(object sender, EventArgs e) {
            Array<float> X = 0, Y = 0, Z = CreateData(X, Y);
            var surface = new Surface(Z, X, Y, colormap: Colormaps.Jet);
            surface.UseLighting = true;
            surface.Wireframe.Visible = false; 
            panel2.Scene.Camera.Add(surface);

            // setup mouse handlers
            panel2.Scene.Camera.Projection = Projection.Orthographic;
            surface.Fill.Shininess = 0.1f;
            surface.Fill.SpecularColor = System.Drawing.Color.NavajoWhite;
            
            //panel2.Scene.Camera.Projection = Projection.Perspective;
            panel2.Scene.Camera.MouseDoubleClick += Camera_MouseDoubleClick;
            panel2.Scene.Camera.MouseWheel += Camera_MouseWheel;

            // initial zoom all
            ShowAll(panel2.Scene.Camera); 
        }

        private void Camera_MouseWheel(object sender, Drawing.MouseEventArgs e) {

            // Update: added comments. 

            // the next conditionals help to sort out some calls not needed. Helpful for performance.
            if (!e.DirectionUp) return;
            if (!(e.Target is Triangles)) return;

            // make sure to start with the SceneSyncRoot - the copy of the scene which receives 
            // user interaction and is eventually used for rendering. See: https://ilnumerics.net/scene-management.html 
            var cam = panel2.SceneSyncRoot.First<Camera>();

            if (Equals(cam, null)) return; // TODO: error handling. (Should not happen in regular setup, though.)

            // in case the user has configured limited interaction
            if (!cam.AllowZoom) return;
            if (!cam.AllowPan) return; // this kind of directional zoom "comprises" a pan operation, to some extent. 

            // find mouse coordinates. Works only if mouse is over a Triangles shape (surfaces, but not wireframes): 
            using (var pick = panel2.PickPrimitiveAt(e.Target as Drawable, e.Location)) {

                if (pick.NextVertex.IsEmpty) return;

                // acquire the target vertex coordinates (world coordinates) of the mouse
                Array<float> vert = pick.VerticesWorld[pick.NextVertex[0], r(0, 2), 0];
                // and transform them into a Vector3 for easier computations
                var vertVec = new Vector3(vert.GetValue(0), vert.GetValue(1), vert.GetValue(2));

                // perform zoom: we move the camera closer to the target
                float scale = Math.Sign(e.Delta) * (e.ShiftPressed ? 0.01f : 0.2f);  // adjust for faster / slower zoom
                var offs = (cam.Position - vertVec) * scale;  // direction on the line cam.Position -> target vertex
                cam.Position += offs;   // move the camera on that line
                cam.LookAt += offs;     // keep the camera orientation
                
                cam.ZoomFactor *= (1 + scale); 
                // TODO: consider adding: the lookat point now moved away from the center / the surface due to our zoom.
                // In order for better rotations it makes sense to place the lookat point back to the surface, 
                // by adjusting cam.LookAt appropriately. Otherwise, one could use cam.RotationCenter.
                
                e.Cancel = true; // don't execute common mouse wheel handlers
                e.Refresh = true; // immediate redraw at the end of event handling
            }
        }

        private void Camera_MouseDoubleClick(object sender, Drawing.MouseEventArgs e) {
            var cam = panel2.Scene.Camera; 
            ShowAll(cam);
            e.Cancel = true;
            e.Refresh = true; 
        }

        // Some sample data. Replace this with your own data! 
        private static RetArray<float> CreateData(OutArray<float> Xout, OutArray<float> Yout) {
            using (Scope.Enter()) {
                Array<float> x_ = linspace<float>(0, 20, 100);
                Array<float> y_ = linspace<float>(0, 18, 80);
                Array<float> Y = 1, X = meshgrid(x_, y_, Y);

                Array<float> Z = abs(sin(sin(X) + cos(Y))) + .01f * abs(sin(X * Y));
                if (!isnull(Xout)) {
                    Xout.a = X;
                }
                if (!isnull(Yout)) {
                    Yout.a = Y;
                }
                return -Z; 
            }
        }
        // See: https://ilnumerics.net/examples.php?exid=7b0b4173d8f0125186aaa19ee8e09d2d
        public static double ShowAll(Camera cam) {
            // Update: adjusts the camera Position too. 

            // this example works only with orthographic projection. You will need to take the view frustum 
            // into account, if you want to make this method work with perspective projection also. however, 
            // the general functioning would be similar....
            if (cam.Projection != Projection.Orthographic) {
                throw new NotImplementedException();
            }
            // get the overall extend of the cameras scene content
            var limits = cam.GetLimits();
            // take the maximum of width/ height
            var maxExt = limits.HeightF > limits.WidthF ? limits.HeightF : limits.WidthF;
            // make sure the camera looks at the unrotated bounding box
            cam.Reset();
            // center the camera view
            cam.LookAt = limits.CenterF;
            cam.Position = cam.LookAt + Vector3.UnitZ * 10; 
            // apply the zoom factor: the zoom factor will scale the 'left', 'top', 'bottom', 'right' limits 
            // of the view. In order to fit exactly, we must take the "radius"
            cam.ZoomFactor = maxExt * .50;
            return cam.ZoomFactor;
        }

    }
}

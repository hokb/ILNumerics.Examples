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

namespace CustomMouseWheelHandler
{
    /// <summary>
    /// The example demonstrates how the behavior of the mouse wheel can be changed on a regular camera. It registers a custom mouse wheel event handler 
    /// on the camera and moves the camera forward or backwards, depending on the mouse wheel movements. The allowed range of the distnace of the 
    /// camera from the object is limited, too.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // setup a simple scene
            ilPanel1.Scene.Camera.Add(new Sphere());
            // register a custom event handler on the camera
            ilPanel1.Scene.Camera.MouseWheel += Camera_MouseWheel;
            // this will only work as expected for perspective projection, obviously!
            ilPanel1.Scene.Camera.Projection = Projection.Perspective;
            // show when a property was changed
            ilPanel1.SceneSyncRoot.First<Camera>().PropertyChanged += (_s, _a) => {
                Text = (_s as Camera).ToString();
            };
        }

        void Camera_MouseWheel(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            var cam = sender as Camera;
            if (cam != null && e.DirectionUp) {
                float scale = (e.ShiftPressed) ? 0.01f : 0.1f;
                
                cam.Position = cam.Position + (cam.Position - cam.LookAt) * Math.Sign(e.Delta) * scale;
                // limit the Z 'distance' of the camera
                cam.Position = new Vector3(cam.Position.X, cam.Position.Y, Math.Max(Math.Min(cam.Position.Z, 11f), 5f)); 
                e.Cancel = true; // this will prevent the regular (ZoomFactor) handling in the camera class
                e.Refresh = true; // this will cause an immediate redraw
            }


        }
    }
}

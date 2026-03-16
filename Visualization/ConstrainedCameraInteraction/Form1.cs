using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConstrainedCameraInteraction {
    /// <summary>
    /// This example demonstrates how to implement a simple filter on scene graph objects. A PropertyChangedEvent handler is registered to 
    /// the common camera in the scene. It is used to control the setting of its LookAt point and to modify the camera interactively and 
    /// programmatically. 
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
           
            // we create a regular camera which reacts on changes to it by mouse interaction. 
            // The rule we are going to implement: 
            // The camera lookat point is clamped to a distance of 0.5 from the center of the scene.  

            ilPanel1.Scene.Camera.Add(new Sphere());
            // mouse interaction does affect the synchronized scene only! 
            // see: http://ilnumerics.net/scene-management.html
            ilPanel1.SceneSyncRoot.First<Camera>().PropertyChanged += Camera_PropertyChanged;

            MessageBox.Show("Pan the sphere with the right mouse button! The camera will be prevented from looking too far away from the center.");
        }

        void Camera_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            // always get the actual object! It is provided by the sender argument. 
            var cam = sender as Camera;
            // filter for changes to the ILCamera.LookAt property
            if (cam != null && e.PropertyName == "LookAt") {
                
                // implement the constrain
                if (cam.LookAt.Length > 0.5) {
                    var v = Vector3.Normalize(cam.LookAt) * 0.5f;
                    // reassign the camera lookat point
                    cam.LookAt = v; 
                }

            }
            
        }
    }
}

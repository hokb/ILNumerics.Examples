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
using ILNumerics.Drawing;

namespace ZoomFactorExample
{
    /// <summary>
    /// This examples demonstrates how to apply different zoom settings to an ILCamera based scene. 
    /// It allows the user to zoom by mouse wheel as well as with help of two buttons attached to the form.  
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e)
        {

            // setup the scene
            ilPanel1.Scene.Camera.Add(new Cylinder());

            // apply further configuration to the scene. This does not affect the zooming as such but is included here for 
            // demonstration purposes only. 
            ilPanel1.Scene.Camera.Projection = Projection.Orthographic;
            ilPanel1.Scene.Camera.Add(new Tripod());

        }
        // the button handlers are registered in the forms designer. This handles clicks on the ZoomIn Button
        private void zoomInButton_Click(object sender, EventArgs e)
        {
            // zoom in
            doZoomStepFirstCamera(true);
        }

        // Button handler for zoom out button
        private void zoomOutButton_Click(object sender, EventArgs e)
        {
            // zoom out
            doZoomStepFirstCamera(false);
        }

        // this method implements the actual zoom operation. It is called by the button click handlers.
        private void doZoomStepFirstCamera(bool zoomin)
        {
            // get the current zoom factor: fetch it from the camera in the synchronized scene. This is the 
            // one which is individually maintained by each panel (driver) and getting altered by the mouse, 
            // independently from the global scene. See: http://ilnumerics.net/scene-management.html
            var camera = ilPanel1.SceneSyncRoot.First<Camera>();
            if (camera != null) {

                // update the camera zoom factor by adding the update value
                // refer to: http://ilnumerics.net/apidoc/html/P_ILNumerics_Drawing_ILCamera_ZoomFactor.htm
                camera.ZoomFactor += (zoomin ? -.1 : .1);
                // trigger immediate redraw
                ilPanel1.Refresh();
            }
        }

        private void buttonShowAll_Click(object sender, EventArgs e) {
            // get the camera relevant for interactions 
            var camera = ilPanel1.SceneSyncRoot.First<Camera>();
            if (camera != null) {
                // call the extension method on the camera
                camera.ShowAll(); 
                // trigger immediate redraw
                ilPanel1.Refresh();
            }
        }
    }
    public static class Extensions {
        /// <summary>
        /// Extension method to set the zoom factor of a camera object to show all content (orthographic zoom only)
        /// </summary>
        /// <param name="camera"></param>
        public static void ShowAll(this Camera cam) {
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
            // apply the zoom factor: the zoom factor will scale the 'left', 'top', 'bottom', 'right' limits 
            // of the view. In order to fit exactly, we must take the "radius"
            cam.ZoomFactor = maxExt * .50;

        }

    }

}

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

namespace ShowAllOrthographicZoom {
    // This example shows how to compute and apply a zoom factor to a scene with orthographic projection in order 
    // to make the camera show the whole content of the scene.
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // scene setup 
            ilPanel1.Scene.Camera.Add(new Group(scale: new Vector3(3, 4, 5)) {
                Shapes.Gear2, Shapes.Gear2Wireframe
            });
            // orthographic projection only. For perspective projection to work, modification to ShowAll() 
            // will be necessary...
            ilPanel1.Scene.Camera.Projection = Projection.Orthographic;
            ilPanel1.Scene.Camera.LookAt = new Vector3(0.5, 0.4, 0.3); 

        }
        public static double ShowAll(Camera cam) {
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
            return cam.ZoomFactor; 
        }
        //This handler triggers the ShowAll zoom setting
        private void button1_Click(object sender, EventArgs e) {
            // fetch the camera from the synchronized scene
            var cam = ilPanel1.SceneSyncRoot.First<Camera>();
            if (cam != null) {
                // compute + apply "show all" zoom setting
                Text = ShowAll(cam).ToString();
                // cause immediate redraw
                ilPanel1.Refresh(); 
            }
        }
    }


}

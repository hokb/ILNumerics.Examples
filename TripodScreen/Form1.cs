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

namespace Tripod {
    
    /// <summary>
    /// Sample class showing how to create a screen object which is always drawn on top of the regular scene and can be rotated / scaled, moved with the mouse.
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // second attempt
        private void ilPanel1_Load(object sender, EventArgs e) {
            // this is our regular 'scene' (here: a simple sphere)
            ilPanel1.Scene.Camera.Add(new Sphere());

            // we create a regular group holding the tripod. The trick is to
            // set its target to 'Screen2DFar'. This will place the content of the group 
            // on top of all regular 'World3D' objects.
            // The content is also scaled a little to make it always visible. 
            var tripodGroup = new Group(target: RenderTarget.Screen2DFar, scale: new Vector3(.5,.5,.5)) {

                new Lines("tripod_lines") {
                    Positions = new float[,] { {0,0,0},{1,0,0},{0,1,0},{0,0,1} },
                    Indices = new int[] {0,1,0,2,0,3},
                    Width = 3,
                    Color = Color.DarkBlue
                }, 
                new ILNumerics.Drawing.Label(tag: "tripod_xLabel", text: "X") { Position = new Vector3(1.2f,0,0) },
                new ILNumerics.Drawing.Label(tag: "tripod_yLabel", text: "Y") { Position = new Vector3(0,1.2f,0) },
                new ILNumerics.Drawing.Label(tag: "tripod_zLabel", text: "Z") { Position = new Vector3(0,0,1.2f) },
            };
            var tripCamera = ilPanel1.Scene.Add(new Camera("tripod_camera") {
                ScreenRect = new RectangleF(0,0.6f,0.4f,0.4f),
                Children = { tripodGroup },
            });
 
            // disable mouse interaction for the tripod camera
            tripCamera.MouseMove += (_s1,_a1) => { _a1.Cancel = true; };
            // connect the tripod camera with the main camera
            ilPanel1.SceneSyncRoot.First<Camera>().PropertyChanged += (_s, _a) => {
                var sourceCam = _s as Camera;
                if (sourceCam != null) {
                    // we could use 'ILCamera.CopyFrom'. But it would copy _all_ properties, even zoom & pan
                    //tripCamera.CopyFrom(ilPanel1.SceneSyncRoot.First<ILCamera>(), false);

                    // ... we only need the normalized position and top vectors: 
                    tripCamera.Top = sourceCam.Top;
                    tripCamera.Position = Vector3.Normalize((sourceCam.Position - sourceCam.LookAt)) * 10;
                    tripCamera.LookAt = new Vector3();
                    // show the new tripod 
                    ilPanel1.Refresh();
                }
            }; 
        }
    }
}

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

namespace Arrow3D_Configuration {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {

            ilPanel1.Scene.Camera.Add(new Group { // (this group is only needed for the object initializer syntax below)
                // the 'scene': 
                new Group("Content Group") { 
                    new Gear(toothCount:15) { 
                        Fill = { Color = Color.LightGray } 
                    },
                    // add other content here as needed
                }, 

                // add a default tripod: lower left corner, 3D arrows
                new Tripod(),

                // create one tripod on the right bottom quadrant, use black color for X arrow
                new Tripod(location: CameraQuadrant.BottomRightFront, size:.5f) {
                    XArrow = { ColorOverride = Color.Black }
                }, 

                // add another tripod in center position, demonstrating arbitrary screen position, back position
                new Tripod(style: TripodStyle.TripodSimple) {
                    // set a world target or use any 'Back' CameraQuadrant as location. This 
                    // 'embedds' the tripod into the 3D scene. It will be hideable by the 3D shapes.
                    Target = RenderTarget.World3D, 
                    // arbitrary screen recangles; position and size the tripod on the scene
                    ScreenRect = new RectangleF(.23f,.3f,.5f,.5f),
                    // color all tripod lines yellow
                    ColorOverride = Color.Yellow, 
                },
            });
            // add yet one tripod which shows a minaturized version of the scene
            var tripod4 = ilPanel1.Scene.Camera.Add(
                // create the tripod without any content (style: TripodEmpty)
                new Tripod(style: TripodStyle.TripodEmpty, 
                    // locate it in the upper top corner, size of .1 will greatly shrink the tripod content later 
                             location: CameraQuadrant.TopRightFront, size: .1f) {
                    // this time we do not allow any interaction through the tripod
                    AllowPan = false, AllowRotation = false, AllowZoom = false });
            
            // transfer the scene: simply re-add all shapes to the tripod 
            foreach (var shape in ilPanel1.Scene.First<Group>("Content Group").Find<Shape>()) {
                tripod4.Add(shape); 
            } 
        }
    }
}

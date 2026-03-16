using ILNumerics;
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
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace EulerAngles {
    /// <summary>
    /// The example shows both major options for rotating objects: Rotating the camera to produce the _impression_ of rotating the scene. And 
    /// rotating the scene or parts of it. The example creates 3 cameras in different quadrants of the panel. Each shows the same content (a 
    /// green hemisphere with a big red dot at the edge). Each displays the content rotated in a different manner: 
    /// The upper left quadrant shows the hemisphere unrotated. Both, camera and model are shown as they are created by default. 
    /// The upper right quadrant rotates the _model_ by applying several rotations to the ILCamera.Transform property, which is derived from ILGroup.
    /// In the lower right quadrant the hemisphere was rotated by rotating the camera itself. Similar transformations have to be applied in order 
    /// to get the same result as for the upper quadrant. The transformations, however differ in sign and order. 
    /// </summary>
    /// <remarks><para>While producing similar results, using one method or the other is not the same: if you double click on the lowe hemisphere 
    /// (the one, which was rotated using the camera) the camera postion is reset and the hemisphere rotates back to initial state. Double clicking 
    /// on the upper right hemispher on the other side, does not reset the hemisphere, because the corresponding camera has not been involved into the 
    /// rotation.</para></remarks>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // setup three cameras: left upper quadrant will hold the original (no rotation)
            var cam1 = ilPanel1.Scene.Add(new Camera("cam1") { ScreenRect = new RectangleF(0, 0, .5f, 0.5f) });
            // right upper quadrant: rotation via ILCamera.Transform (rotate the model)
            var cam2 = ilPanel1.Scene.Add(new Camera("cam2") { ScreenRect = new RectangleF(.5f, 0, .5f, 0.5f) });
            // lower right quadrant: rotation via camera position (modifies the camera position)
            var cam3 = ilPanel1.Scene.Add(new Camera("cam2") { ScreenRect = new RectangleF(.5f, .5f, .5f, 0.5f) });

            // add some object which help us to get the rotation visually
            var obj = new Group { Shapes.Hemisphere, Shapes.HemisphereWireframe }; 
            // a red dot makes the rotation more clear 
            obj.Add(new Points { 
                Positions = Shapes.Hemisphere.Positions, 
                Indices = new[] { 0 }, 
                Color = Color.Red, Size = 15 
            });
            
            cam3.Add(obj);
            cam2.Add(obj);
            cam1.Add(obj);
            // rotation angles 
            float rX = pif / 3, rY = pif, rZ = -pif / 2;
            cam2.Transform =
                // Left hand rule. Last transform comes first! 
                // last we rotate around Z
                Matrix4.Rotation(new Vector3(0, 0, 1), rZ) *
                // 2nd we rotate around Y
                Matrix4.Rotation(new Vector3(0, 1, 0), rY) *
                // first we rotate around x
                Matrix4.Rotation(new Vector3(1, 0, 0), rX) * 
                Matrix4.Identity;

            // same result, but via camera transform. Here in natural order: 
            // first we rotate around x; inversed effect! 
            cam3.RotateX(-rX);
            // 2nd we rotate around Y; inversed effect!
            cam3.RotateY(-rY);
            // last we rotate around Z. Camera Z points into the screen! This is the opposite 
            // direction as in model space. Hence the 'inversed effect' (negation of rotation) 
            // is neutralized:
            cam3.RotateZ(rZ);

            #region add some label for reporting
            string t2 = String.Format("cam2.Transform:\r\nZ:{0}\r\nY:{1}\r\nX:{2}", rZ, rY, rX);
            cam2.Add(new ScreenObject(width: 110, height: 60) {
                Children = { new ILNumerics.Drawing.Label(text: t2) { Anchor = new PointF(-.05f,-.05f) } },
                Location = new PointF(),
                Anchor = new PointF(-.1f,-.1f)
            });
            string t3 = String.Format("cam3.Rotate?:\r\nX:{0}\r\nY:{1}\r\nZ:{2}", -rX, -rY, rZ);
            cam3.Add(new ScreenObject(width: 110, height: 60) {
                Children = { new ILNumerics.Drawing.Label(text: t3) { Anchor = new PointF(-.05f, -.05f) } },
                Location = new PointF(),
                Anchor = new PointF()
            });
            #endregion
        }
               
                
    }
}

using ILNumerics.Drawing;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomMouseToolForILCamera {
    /// <summary>
    /// The example demonstrates the implementation of a custom mouse tool: the user can switch between standard mouse handling (rotating) for 
    /// a common Camera scene and a custom mouse tool. Latter draws a selection rectangle over the scene in a 2D style. The example prepares 
    /// for handling the 2D coordinates after the mouse is released. 
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        public bool CustomMouseActions { get { return checkBox1.Checked; } }
        // default tags for custom objects, used for easier and reliable referencing
        private static readonly string SelectionRectangleTag = "SelectionRectangle";
        private static readonly string SelectionGroupTag = "SelectionGroup";
        // a local variable stores the starting coordinates of the mouse for the custom mouse tool
        PointF? startLocation; 

        // convenient access to a custom object in the scene without holding a distinct reference as class member variable:
        public Lines SelectionRectangle { get { return ilPanel1.SceneSyncRoot.First<Lines>(SelectionRectangleTag); } }

        private void ilPanel1_Load(object sender, EventArgs e) {
            
            // setup a simple scene
            ilPanel1.Scene.Camera.Add(new Group() {
                Shapes.Gear10, Shapes.Gear10Wireframe, Shapes.Circle50, 
            });

            // add a lines shape as selection rectangle to the screen subscene.
            // The line is wrapped in a group node for easier resizing. In a production
            // environment you may want to set the Visible property to false at first. Here, 
            // we leave the line visible for demonstration purposes.
            ilPanel1.Scene.Screen.Add(
                new Group(SelectionGroupTag, target: RenderTarget.Screen2DFar) {
                    new LineStrip(SelectionRectangleTag) {
                        // the line uses 0->1 coordinate space in both directions (X,Y). 
                        Positions = new float[,] {
                            {0,0,0},{0,1,0},{1,1,0},{1,0,0},{0,0,0}
                        }, 
                        Color = Color.Red,
                        Width = 2,
                        DashStyle = DashStyle.Dashed,
                    }
                }); 
            // catch all needed mouse event handlers. The events are 
            // registered on the camera! It allows to define individual 
            // actions for individual camera.ScreenRect areas on the panel. 
            ilPanel1.Scene.Camera.MouseDown += Camera_MouseDown;
            ilPanel1.Scene.Camera.MouseMove += Camera_MouseMove;
            ilPanel1.Scene.Camera.MouseUp += Camera_MouseUp;

            //ilPanel1.Scene.Camera.Projection = Projection.Perspective;
            //ilPanel1.Scene.Camera.AspectRatioMode = AspectRatioMode.StretchToFill; 
            MessageBox.Show("Rotate the scene with the mouse. Switch to [Custom Mouse Action] by pressing the button. Drag rectangle with the mouse.");

        }

        void Camera_MouseUp(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (!CustomMouseActions || !e.DirectionUp) return; // default processing
            // TODO: trigger custom action (zoom) here...
            MessageBox.Show("Rectangle (2D Viewport Coordinates): \r\nStart: " + startLocation.ToString() + "\r\nEnd: " + e.LocationF.ToString());

            // ... clean up
            // prevent from default processing
            e.Cancel = true;
            // reset the selection rectangle
            startLocation = null;
            SelectionRectangle.Visible = false;
            // cause immediate redraw of the scene
            e.Refresh = true;

        }

        void Camera_MouseMove(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (!CustomMouseActions) return; // default processing

            // Note: we only process this event, if an earlier mouse down event occured. We detect this
            // by checking for the startLocation variable to have a value assigned. 
            // Furthermore, in order to prevent from a redundant rendering frame, we only process the event 
            // in the bubbling phase of the event handling. (Otherwise, the processing would happen 
            // twice: namely in the capture AND the bubbling phase. See: http://ilnumerics.net/mouse-events.html)
            if (startLocation.HasValue && e.DirectionUp) {
                // modify the selection rectangle shape
                SetSelectionRectangle(startLocation.GetValueOrDefault(), e.LocationF);
                // make sure the rectangle is shown
                SelectionRectangle.Visible = true;
                // cause immediate redraw of the scene
                e.Refresh = true;
                // prevent from default processing
                e.Cancel = true;
            }
        }

        void Camera_MouseDown(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (!CustomMouseActions || !e.DirectionUp) return;
            // prevent from default processing
            e.Cancel = true;
            // remember the mouse down position
            startLocation = e.LocationF;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            // this simply sets the checkkbox text to reflect the current setting
            var cb = sender as CheckBox;
            if (cb != null) {
                cb.Text = cb.Checked ? "Custom Mouse Action" : "Standard Mouse Action";
            }
        }

        private void SetSelectionRectangle(PointF p1, PointF p2) {
            // We want to prevent from altering any buffers for performance reasons. 
            // That's why we put the line into a group node, so we can easily scale it
            // by means of the Group.Transform property.
            SelectionRectangle.Parent.Transform = Matrix4.Identity
                // note the reversed order of such combined transform steps! 
                // (2) secondly we move the rectangle.
                * Matrix4.Translation(p1.X, p1.Y, 0)
                // (1) first we scale ... 
                * Matrix4.ScaleTransform(p2.X - p1.X, p2.Y - p1.Y, 1); 
        }

    }
}

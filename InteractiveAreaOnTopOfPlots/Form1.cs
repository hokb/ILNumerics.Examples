using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
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

namespace WindowsFormsApplication28 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // setup the surface / imagesc plot
            Array<float> A = tosingle(rand(2,100)); 
            var surf = ilPanel1.Scene.Add(
                new PlotCube(twoDMode: false) {
                    new ImageSCPlot(A)
            }); 
            // setup single clickable areas
            // The areas 'live' in the same space as the surface. This eases 
            // synchronization between both. Also, that way, the area uses 
            // the same clipping planes as the surface does: it moves out of 
            // the visible area automatically.
            var areas = ilPanel1.Scene.First<PlotCube>().Add(
                // By placing the new area objects into a new group having the 
                // target parameter set to Screen2DFar we make sure that the area will always display
                // on top of the World3D surface content, regardless of its actual depth.
                new Group("areas", target: RenderTarget.Screen2DFar));
            var a1 = areas.Add(new Area("a1", new RectangleF(10, 0, 20, 1)));
            var a2 = areas.Add(new Area("a2", new RectangleF(40, 0, 20, 1)));
            
            // configure some mouse event handlers
            EventHandler<ILNumerics.Drawing.MouseEventArgs> clickAction = (s, arg) => 
            {
                var a = s as Group;
                if (a != null) {
                    Text = a.Tag + " clicked! " + arg.Target.Tag;
                }
            };
            a1.MouseClick += clickAction;
            a2.MouseClick += clickAction;  
          
            // we can also move the area programmatically: 
            a1.MouseClick += (_s, _a) => {
                Array<float> r = tosingle(rand(1, 2));
                a1.SetPosition(new RectangleF(r.GetValue(0) * 60, r.GetValue(1), 20, 1));
            }; 
            // Note, for 2D objects which can also be dragged with the mouse by the user at runtime, see: ScreenObject. 
            
        }

        /// <summary>
        /// Simple area class: a filled rectangle, a border and a label
        /// </summary>
        /// <remarks>This class serves as an example only. It demonstrates how to create a compound shapes class which 
        /// allows the creation and configuration of many similar behaved drawing objects. 
        /// <para>This class does only implement the most important features. Read here for more details and how to 
        /// get even more flexibility for your class: http://ilnumerics.net/custom-scene-graph-objects.html </para>
        /// </remarks>
        class Area : Group {

            private static readonly string BorderTag = "AreaBorder";
            private static readonly string FillTag = "AreaFill";
            private static readonly string LabelTag = "AreaLabel";
            public Lines Border { get { return First<Lines>(BorderTag); } }
            public Triangles Fill { get { return First<Triangles>(FillTag); } }
            public ILNumerics.Drawing.Label Label { get { return First<ILNumerics.Drawing.Label>(LabelTag); } }

            public Area(object tag = null, RectangleF? size = null, string label = null) : base(tag: tag) {
                // setup the area objects, regular ILNumerics.Drawing shapes
                if (size.HasValue) {
                    // set the size and position of the area
                    SetPosition(size.Value); 
                }
                // set up the label
                string labText = label != null ? label : (tag != null ? tag.ToString() : "area");
                Add(new ILNumerics.Drawing.Label(tag: LabelTag, text: labText) {
                    // center the label
                    Anchor = new PointF(.5f,.5f),
                    Position = new Vector3(.5,.5,0)
                });
                // set up the border and the fill.
                Add(Shapes.RectangleWireframe, tag: BorderTag);
                Border.Width = 3; Border.Color = Color.DarkBlue; 
                Add(Shapes.RectangleFilled, tag: FillTag);

                // configure the area
                // Do not use Fill.Visible = false here! It would hide the background and 
                // disable the catching of mouse events for it. Instead, use Color.Empty or a transparent color! 
                Fill.Color = Color.FromArgb(150, Color.White);

            }

            /// <summary>
            /// Set the position and size of the area relative to the parent coordinate system
            /// </summary>
            /// <param name="size">rectangle determining the new position and size of the area</param>
            public void SetPosition(RectangleF size) {
                // first reset the existing transform. we do not want transform matrices to accumulate!
                Transform = Matrix4.Identity; 
                Scale(new Vector3(size.Width, size.Height, 1));
                Translate(new Vector3(size.Left, size.Top, 0));
            }

        }
    }
}

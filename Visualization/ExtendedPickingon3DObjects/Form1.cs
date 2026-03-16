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


namespace ExtendedPickingon3DObjects {
    // Picking Example. The application creates a surface in a plot cube. It registers several event handler 
    // demonstrating the picking of primitives and the interactive manipulation of the scene to display 
    // the picking results. A small blue dot is moving below the mouse at interpolated positions. A larger red dot 
    // is placed at the nearest vertex to the mouse. Hovering over the red dot displays a tool tip with the position 
    // value of the vertex in a screen overlay.  
    // 
    // Create a new WindowsForms application and paste the content of the following class into it. Start the application
    // and move the mouse over the surface. Watch the values and positions of the red and blue dot change. Hover 
    // over the red dot. A tool tip window is shown displaying the values of its position. 
    public partial class Form1 : Form {
        public Form1() {
            Array<double> A = 0; // ILMath.zeros<double>(1,30);
            A = A + 1; 
            InitializeComponent();
        }

        // attributes: red point marking vertices, blue for interpolated value
        Points m_pointsRed, m_pointsBlue;
        // a screen object is used as tool tip label
        ScreenObject m_labelObj;

        // this is the regular panel load event handler
        private void ilPanel1_Load(object sender, EventArgs e) {

            // setup objects
            m_pointsRed = new Points("MarkVertex") {
                Color = Color.Red,
                Size = 10
            };
            m_pointsBlue = new Points("MarkPosition") {
                Color = Color.Blue,
                Size = 3
            };
            // the tooltip label is placed inside an ILScreenObject
            m_labelObj = new ScreenObject("LabelObj") {
                Children = { 
                    new ILNumerics.Drawing.Label(tag: "Label") {
                        // centered in the screen object
                        Position = new Vector3(.5f,.5f,0),
                        Anchor = new PointF(.5f,.5f)
                    }
                },
                // disable interactive movement of the screen object
                Movable = false,
                // displace slightly away from the mouse 
                Anchor = new PointF(-0.05f, -0.05f),
                Width = 150,
                Height = 32,
                Visible = false 
            };
            // create some test data
            Array<float> C = SpecialData.sincf(20, 30, 3);
            C["2:5;10:14"] = 1;  // make it more interesting

            Array<double> A = vector<double>(1, 2, 3, 4, 5) + 1; 


            var a = new byte[28]; 

            // add objects to the scene
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) {
                new Surface(C),
                // dots and tool tip go into a seperated  group
                // so we can render them on top of the surface
                new Group(target: RenderTarget.Screen2DFar) { 
                    m_pointsRed, m_pointsBlue, m_labelObj
                }
            });
            // wire up events 
            ilPanel1.Scene.First<PlotCube>().MouseMove += ilSurface1_MouseMove;
            m_pointsRed.MouseEnter += m_pointsRed_MouseEnter;
            m_pointsRed.MouseLeave += m_pointsRed_MouseLeave;
        }

        void m_pointsRed_MouseEnter(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            // fetch reference to the tool tip label
            var label = m_labelObj.First<ILNumerics.Drawing.Label>(tag: "Label");
            if (label != null) {
                // display the position of the red dot in the tool tip
                label.Text = m_pointsRed.Positions.GetPositionAt(0).ToString();
                m_labelObj.Location = e.LocationF;
                m_labelObj.Visible = true;
                // always need to Configure() after changes
                m_labelObj.Configure();
                // redraw the scene
                ilPanel1.Refresh();
            }
        }
        void m_pointsRed_MouseLeave(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            m_labelObj.Visible = false;
            // always need to Configure() after changes
            m_labelObj.Configure();
            // redraw the scene
            ilPanel1.Refresh();
        }

        void ilSurface1_MouseMove(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (!(e.Target is Triangles)) return; 
            // perform picking of the primitives below the cursor
            using (var pick = ilPanel1.PickPrimitiveAt(e.Target as Drawable, e.Location)) {

                // TODO: we do not check for pick containing data for triangles here. In 
                // a production environment, you must do all common checks!
                if (pick.NextVertex.IsEmpty) return; 
                try {
                    // Move the red dot to nearest vertex. pick.NextVertex[0] gives the 
                    // index for the closest primitive in pick.VerticesWorld (row index). 
                    // "0:2" addresses the X,Y and Z values for the vertex in the corrresponding row. 
                    // The third dimension '0' means: the first primtive, which will always be the 
                    // one closest to the camera. 
                    m_pointsRed.Positions.Update(pick.VerticesWorld[pick.NextVertex[0], "0:2", 0][":"]);
                    // compute interpolated point: use the barycentric coordinates provided in 
                    // Vertices["0,1;3;0"] in order to interpolate any value over the triangle 
                    // for the picked point. 
                    float s = pick.VerticesWorld.GetValue(0, 3);
                    float v = pick.VerticesWorld.GetValue(1, 3);
                    // The third barycentric coordinate is not stored. You can compute it easily 
                    // using the following formula: 
                    float t = 1 - s - v;
                    // interpolate: here, we interpolate the position 
                    Array<float> interp = pick.VerticesWorld["0;0:2;0"] * s
                                          + pick.VerticesWorld["1;0:2;0"] * v
                                          + pick.VerticesWorld["2;0:2;0"] * t;
                    m_pointsBlue.Positions.Update(interp[":"]);
                    Text = interp.GetValue(2).ToString();
                    m_pointsRed.Configure();
                    m_pointsBlue.Configure();
                    ilPanel1.Refresh();
                } catch(IndexOutOfRangeException) {
                } catch (ArgumentException) { }

            }
        }
    }
}

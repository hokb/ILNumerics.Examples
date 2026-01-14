using ILNumerics.Drawing.Plotting;
using ILNumerics.Drawing;
using ILNumerics;
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

namespace ScreenObjectCoordTransform {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // some predefined tag names for identifying our objects within the scene graph later
        readonly static string DefaultClosePolygonTag = "ClosePolygon";
        readonly static string DefaultAlignedPolygonTag = "AlignedPolygon";
        readonly static string DefaultRelevantAreaTag = "RelevantArea"; 
        readonly static string DefaultScreenGroupTag = "ScreenGroup";

        private void ilPanel1_Load(object sender, EventArgs e) {
            // create a common surface, based on some example data
            Array<float> A = SpecialData.sincf(40, 30); 
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) { 
                // check if modified screen rect is working 
                //ScreenRect = new RectangleF(0.3f,0,0.5f,1),
                Children = {
                    new Surface(A) { 
                        // colorbars should go above EVERYTHING else. Even above the the yellow/blue lines.
                        new Colorbar()
                    } 
                }
            });
            // let's reuse some data points from the surface ...
            A.a = ilPanel1.Scene.First<Surface>().Fill.Positions.Storage;
            // ... for adding the "interesting area" (line)
            ilPanel1.Scene.First<Surface>().Add(
                new LineStrip(DefaultRelevantAreaTag) {
                    Positions = A[":;210,230,1030,1010"],
                    Indices = new int[] { 0, 1, 2, 3, 0 },
                    Color = Color.Red,
                    Width = 3,
                });

            // add the screen rectangles to be computed
            ilPanel1.Scene.Screen.Add(
                new Group(tag: DefaultScreenGroupTag) {
                    // The Target property indicates the order for objects on screen.
                    // Objects contained in a groupd configured for Target=Screen2DFar are
                    // drawn on top of objects, contained in 'regular' groups, 
                    // configured for Target='World3D'. 
                    Target = RenderTarget.Screen2DFar,
                    Children = {
                        // adds a blue rectangle
                        new LineStrip(DefaultClosePolygonTag) { 
                            Positions = Shapes.RectangleWireframe.Positions,
                            Indices = Shapes.RectangleWireframe.Indices,
                            Color = Color.Blue, 
                            Width = 1
                        },
                        // add a yellow rectangle
                        new LineStrip(DefaultAlignedPolygonTag) { 
                            Positions = Shapes.RectangleWireframe.Positions.Storage,
                            Indices = Shapes.RectangleWireframe.Indices,
                            Color = Color.Yellow, 
                            Width = 3,
                        },
                    }
                });
            // perspective projection makes it all a little trickier
            ilPanel1.Scene.First<PlotCube>().Projection = Projection.Perspective; 
            // catch event for rotating the plot cube
            ilPanel1.SceneSyncRoot.First<PlotCube>().MouseMove += PlotCube_MouseMove;
        }

        void PlotCube_MouseMove(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (e.DirectionUp ) { //&& e.Button == System.Windows.Forms.MouseButtons.Left) {
                using (Scope.Enter()) {
                    // collect and accumulate all transformations from the target node up to the camera (plot cube)
                    Matrix4 modl = Matrix4.Identity; 
                    Matrix4 proj = Matrix4.Identity;
                    Matrix4 view = Matrix4.Identity;
                    // start at the 3D object. Note, that in order to find the actual live object (sender)
                    // within the scene we need to search inside the "SceneSyncRoot" - a dynamic, synchronized 
                    // copy of the original scene graph, which handles user interaction (rotation, zoom etc.). 
                    var node = ilPanel1.SceneSyncRoot.First<Node>(tag: DefaultRelevantAreaTag);
                    var nodes = new Stack<Group>();
                    // collect all nodes up to root, accumulate transform matrices
                    while (node != null && (node = node.Parent) != null) {
                        nodes.Push(node as Group); 
                    }
                    // we collect the matrices in exactly the same way a renderer would do: walk from the 
                    // camera downwards to the shape, save the camera specific transforms (projection, viewtransform) 
                    // and accumulate the model transforms:
                    while (nodes.Count > 0) {
                        var g = nodes.Pop();
                        if (g is Camera) {
                            // we start at the camera
                            proj = (g as Camera).ProjectionTransform;
                            view = (g as Camera).ViewTransform
                                // ^- here we would end up in clip coords: -1 ... 1
                                // we need 0...1 range for the Scene.Screen node
                                .Translate(1, -1, 0).Scale(0.5, -0.5, 1);
                            modl = (g as Camera).PositionTransform;
                        }
                        // accumulate model transforms
                        modl = modl * g.Transform; 
                    }
                    // simulate the rendering pipeline
                    Array<float> A = proj * (modl * ilPanel1.Scene.First<Lines>(tag: DefaultRelevantAreaTag).Positions.Storage);
                    // do the perspective divide and go to normalized screen coords
                    A.a = Matrix4.ViewTransformWithPerspDivide(view, A)[r(0,2),full]; 

                    // configure area in screen coords (blue)
                    var line = ilPanel1.Scene.First<LineStrip>(tag: DefaultClosePolygonTag);
                    line.Positions.Update(A);
                    // configure hull (yellow), pick min & max from screen coords
                    A.a = sort(A, dim: 1);
                    A = reshape(A[vector<int>(0,1,2,0,10,2,9,10,2,9,1,2)],3,4);
                    line = ilPanel1.Scene.First<LineStrip>(tag: DefaultAlignedPolygonTag);
                    line.Positions.Update(A);
                    // commit changes for drawing, we only need to configure the first common parent of the changed shapes
                    ilPanel1.Scene.Screen.First<Group>(tag: DefaultScreenGroupTag).Configure();
                    e.Refresh = true;  
                }
            }
        }
    }
}

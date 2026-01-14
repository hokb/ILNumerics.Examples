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

namespace RectangleOverlayinPlotCube {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            Array<double> A = rand(20, 30);
            Array<float> Af = tosingle(A); 
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // add some plot data
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) {
                new LinePlot(sin(linspace<float>(-2f,12f,13)), markerStyle: MarkerStyle.Dot)
            });
            // add the first overlay
            AddOverlay("overlay1", Color.FromArgb(200, Color.LightBlue));
            // add the second overlay
            AddOverlay("overlay2", Color.FromArgb(100, Color.Red));
            // scale and position the overlay rectangle. We mark the area between 4f .. 5f. 
            SetOverlaySize("overlay1", 4, 5);
            // The second area ...
            SetOverlaySize("overlay2", 7.5f, 9f);
        
            // the same repositioning must be done whenever the plot cube limits change: 
            ilPanel1.SceneSyncRoot.First<PlotCube>().Limits.Changed += (_s, _a) => {
                SetOverlaySize("overlay1", 4, 5);
                SetOverlaySize("overlay2", 7.5f, 9f);
            };
            Text = "RectangleOverlayinPlotCube"; 
        }

        private void AddOverlay(string tag, Color color) {
            // the overlay will be a rectangular ILTriangles shape... 
            var rect = Shapes.RectangleFilled;
            // ... with the color specified. 
            rect.Color = color; 

            var plotCube = ilPanel1.Scene.First<PlotCube>();
            if (plotCube != null) {
                // It is more convenient to place the overlay in the same group as 
                // the axes objects. The coord system there always runs from 0...1 
                // which makes it easy to recompute the overlay extent. However, 
                // in order to access that group, we cannot use plotCube.Children or 
                // plotCube.Add, since this would add the objects as new plot object. 
                // Hence we locate the target group as the parent of the axis collection. 
                plotCube.Axes.Parent.Add(
                    // The overlay is added into a seperate group node. We use 
                    // it in order to realize clipping which prevents the overlay 
                    // to run out of the plotcube on interactive pan operations
                    new Group("ClipGroup") {
                        Clipping = new ClipParams() { 
                            // since the overlay will only move in x direction, 
                            // it is sufficient to add clipping planes for the 
                            // horizontal limits only. 
                            Plane0 = new Vector4(-1, 0, 0, 1f),
                            Plane1 = new Vector4(1, 0, 0, 0),
                        },
                        Children = {
                            // Put the actual rectangle overlay shape into another group node.
                            // This one will be responsible for all transformations: size and position. 
                            // It is the one which will be retrieved via the tag identifier later. 
                            // We use target: Screen2DNear for the group. This is only needed,
                            // in order to display the overlay on top of the markers also. Keep
                            // in mind: markers are 2DFar objects, hence without defining a 
                            // render target here, the markers would go on top of the overlay. 
                            new Group(tag, target: RenderTarget.Screen2DNear) {
                                // add the overlay rectangle here
                                rect 
                            }
                        }, 
                    });

            }
        }
        /// <summary>
        /// Rescale and reposition the overlay rectangle identified by tag to match the data range given in x1 and x2
        /// </summary>
        /// <param name="tag">tag used to identify the overlay</param>
        /// <param name="x1">lower data range to mark. This is the X coordinate of the data in the line plot.</param>
        /// <param name="x2">upper data range to mark. This is the X coordinate of the data in the line plot.</param>
        private void SetOverlaySize(string tag, float x1, float x2) {
            // fetch references for the plot cube and the overlay group
            var ol = ilPanel1.SceneSyncRoot.First<Group>(tag);
            var pc = ilPanel1.SceneSyncRoot.First<PlotCube>(); 
            if (ol != null && pc != null) {
                // recompute the overlay group transform: scaling and translation
                float w = pc.Limits.WidthF, min = pc.Limits.XMin;
                // The group content is still its initial size: a rectangle spanning from 0..1 in X and Y. 
                // Scaling and translation is done in the group holding the rectangle shape. The task 
                // is to translate from the 'true' data values given in X1 and x2 to the coordinate 
                // system, the rectangle lives in: 0..1. 
                // First the content is scaled in X according to the needed range. 
                // Then the area is moved to the correct position. 
                ol.Transform = Matrix4.ScaleTransform((x2 - x1) / w, 1, 1).Translate((x1 - min) / w, 0, .9999);
            }

        }
    }
}

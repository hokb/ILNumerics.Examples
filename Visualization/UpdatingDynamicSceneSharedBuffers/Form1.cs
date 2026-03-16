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


namespace UpdatingDynamicSceneSharedBuffers {
    /// <summary>
    /// Example demonstrating how dynamic updates are done to shapes. 
    /// The application creates a gear shape (preconfigured Triangles) and a line strip which is used to mark some vertices of the gear. 
    /// Lines and triangles share their Positions buffer. 
    /// A UI slider is utilized in order to dynamically modify vertices of the triangles. Since both shapes share their buffers, 
    /// updating the positions buffer of the triangles does at the same time also updates the line. 
    /// </summary>
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }
        // we use a class attribute in order to store original (not deformed) positions
        private readonly Array<float> m_originalPositions = localMember<float>(); 

        private void ilPanel1_Load(object sender, EventArgs e) {
            // create a gear shape (Triangles)
            var gearTriangles = Shapes.CreateGear(thickness: 2, inR: 0.5f);
            // keep the intitial positions safe 
            m_originalPositions.a = gearTriangles.Positions.Storage; 

            // create a Lines shape for marking a single tooth
            var gearLines = new LineStrip("Lines") {
                // share position buffer with the triangles
                Positions = gearTriangles.Positions,
                // pick indices to mark: one tooth ... 
                Indices = new[] { 3, 5, 7, 9, 11, 13, 15, 
                                 // ... and some vertices on the inner radius (demonstration only)
                                  0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30 },
                Color = Color.Red, 
                Width = 3
            }; 
            ilPanel1.Scene.Camera.Add(new Group("my_scene") {
                // add below the camera 
                gearLines, gearTriangles
            });

            ilPanel1.Scene.Camera.ZoomFactor = 0.85f;
            //ilPanel1.Scene.Camera.Add(new PointLight("light1", new Vector3(-20, 20, 20)) { Intensity = 1.6f });
            ilPanel1.Scene.Add(new PointLight("light2", new Vector3(20, 20, 0)) { Intensity = 0.6f });
        }

        private void trackBar1_Scroll(object sender, EventArgs e) {
            // update the vertices according to some function
            using (Scope.Enter()) {
                var tri = ilPanel1.Scene.First<Triangles>();  // you may need to filter for the tag in more complex scenes!
                if (tri != null) {
                    var trackBar = sender as TrackBar; 
                    // compute the (X,Y) distance of the intial positions to origin
                    Array<float> xy = m_originalPositions["0,1;:"]; 
                    Array<float> dist = sqrt(sum(xy * xy, 0));
                    Array<float> pos = m_originalPositions.C;
                    pos[full, dist < 0.6f] = pos[full, dist < 0.6f] * (trackBar.Maximum - trackBar.Value) / 100f; 
                    // reapply to the positions buffer
                    tri.Positions.Update(pos); 
                    // note, since the triangles shape positions buffer is 
                    // shared with the lines shape, there is no need to 
                    // also update the line shape with the new vertices.

                    // We finished all modifications. So call Configure() to commit the changes for rendering. 
                    tri.Configure();
                    // redraw the scene so the changes become visible
                    ilPanel1.Refresh(); 
                }
            }
        }
    }
}

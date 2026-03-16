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

namespace WindowsFormsApplication1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            Array<int> I = 1, WireI = 1; 
            Array<float> cols = 1; 
            Array<float> A = Computation.CreateCubesData(25000, I, WireI, cols);
            
            // optimization: create a single positions buffer to be shared by Triangles and Lines shapes.
            // This way memory footprint is decreased and rendering speed improved. 
            PositionsBuffer positionsBufferShared = A;
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) {
                Children = {
                    // setting up fast rendering with a single Triangles shape. 
                    // Data for Positions and Indices buffers are produced in CreateCubesData() method.
                    new Triangles() {
                        Positions = positionsBufferShared,
                        Indices = I,  // selecting individual indices as vertices for the triangles
                        Color = null, // Color.Green,
                        Colors = cols, // use individueal 
                        AutoNormals = true
                    },
                    new Lines() {
                        Positions = positionsBufferShared,
                        Indices = WireI,  // selecting individual indices as vertices for the lines
                        Color = Color.Black,  // use a single, solid color for the cubes edges
                    }
                },
                // allow plot cube content to be visible outside the 3D space defined by plotCube.Limits.
                Plots = { Clipping = null },
                // prevents the plot cube from rescaling its limits when new objects are added. Default: true.
                AutoScaleOnAdd = false,
            }); 
        }

        private class Computation { 

            /// <summary>
            /// Create multiple cubes data for efficient rendering.
            /// </summary>
            /// <param name="p">Number of cubes to create.</param>
            /// <param name="I">[Output] indices, prepared for rendering the cubes fill area (triangles).</param>
            /// <param name="WireInd">[Output] Indices, prepared for rendering the cubes edges wireframe (lines).</param>
            /// <param name="colors">[Output] Color data for the cubes edges. Individual colors for each vertex.</param>
            /// <returns></returns>
            internal static RetArray<float> CreateCubesData(int p, OutArray<int> I = null, OutArray<int> WireInd = null, OutArray<float> colors = null) {
                using (Scope.Enter()) {
                    // create cubes 
                    // UnitCubeLighting use individual corner vertices for each face. While this creates better lighting effects,
                    // it also increases the memory requirement. For this example we could easily use a more lightweight UnitCube.
                    // But let's keep things realistic and use the more sophisticated version:
                    Array<float> cube = PositionsBuffer.UnitCubeLighting.Storage;  
                    I.a = IndicesBuffer.UnitCubeLighting.Storage;
                    // pick indices for Lines wireframe, individual lines
                    WireInd.a = new int[] { 0,9, 6,9, 3,6, 3,0, 3,15, 15,12, 12,0, 15,18, 18,6, 18,21, 21,9, 12,21 }; 
                    
                    // replicate the cube p times
                    Array<float> pos = repmat(cube[":"], 1, p);

                    Array<int> mult = arange<int>(0, p - 1).T * (int)cube.S[1];

                    // Note, commonly, output parameters in ILNumerics (OutArray<T>) are optional. Hence, in a production code 
                    // you would always check the existence of the parameter before using it: 
                    if (!isnull(I)) I.a = repmat(I[":"], 1, p) + mult;
                    if (!isnull(WireInd)) WireInd.a = repmat(WireInd[":"], 1, p) + mult; 
                    
                    // displace cubes. Here, we modify the vertex positions so that each cube is placed at a random position.
                    float spread = (float)Math.Sqrt(p);
                    Array<float> r = tosingle(rand(3, p) * spread - spread / 2);
                    // each cube consumes 24 vertices: 4 for each corner by 6 faces.
                    pos.a = reshape(pos + repmat(r, 24, 1), 3, 24 * p);

                    // color all vertices individually (here: position dependent color (+ random disturbance))
                    if (!isnull(colors)) colors.a = abs(pos) / max(abs(pos), dim: 0); // + tosingle(rand(3, pos.S[1]));

                    return pos;  
                }
            }
        }
    }
}

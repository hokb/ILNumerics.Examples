using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO; 
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Controls;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.Globals;
using static ILNumerics.ILMath; 

namespace _3DCadViewGearExample {
    /// <summary>
    /// This example creates a simple form with 4 individual areas, one for each quadrant of the panel. 3 plot cubes display 2D views of an STL model. 
    /// The 4th view is a 3D view of the same model which is individually rotatable by the user. The Camera.ScreenRect property is used to 
    /// distribute the models over the quadrants. The same model is loaded only once and reused for all views. No memory will be copied for the views. 
    /// </summary>
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }
        private void ilPanel1_Load(object sender, EventArgs e) {

            // Load the STL model from file into a Triangles 3D shape; we will reuse it for all views. 

            // This STL is owned by http://www.thingiverse.com/vladac/about and distributed under creative commons attributions share alike license
            Triangles triangles = LoadSTL(@"WallHingePart.stl"); 
            //// This STL is owned by http://www.thingiverse.com/jsteuben/about and distributed under creative commons attributions share alike license
            // 
            // uncomment the next line for an alternative model or replace with your own model
            //Triangles triangles = Computation.LoadSTL(@"skewgear_involute_24_90.stl"); // path to your STL (ASCII) text file here ... 

            // create 3 2d views, distribute Screen Rectangle over each quadrant
            var pc1 = ilPanel1.Scene.Add(new PlotCube(twoDMode: false) { Children = { triangles }, ScreenRect = new RectangleF(0, 0, .5f, .5f) });
            var pc2 = ilPanel1.Scene.Add(new PlotCube(twoDMode: false) { Children = { triangles }, ScreenRect = new RectangleF(.5f, 0, .5f, .5f) });
            var pc3 = ilPanel1.Scene.Add(new PlotCube(twoDMode: false) { Children = { triangles }, ScreenRect = new RectangleF(.5f, .5f, .5f, .5f) });

            // rotate the plot cubes. Double clicking will reset this. Consider overriding the double click event if you want this permanently set!
            pc1.RotateX(Math.PI / 2);
            pc2.Rotation = Matrix4.Rotation(Vector3.UnitY, Math.PI / 2);
            pc3.Rotation = Matrix4.Rotation(Vector3.UnitY, Math.PI / 4).Rotate(Vector3.UnitX, -Math.PI / 180 * 35.264);
            // 3D view for the default camera

            // use lower left quadrant 
            ilPanel1.Scene.Camera.ScreenRect = new RectangleF(0, .5f, .5f, .5f);
            // add a tripod for convenience
            ilPanel1.Scene.Camera.Add(new Tripod());
            ilPanel1.Scene.Add(new PointLight("light2", new Vector3(-20,20,20)) { Intensity = 0.6f }); 
            // scale the model down (optionally) 

            var scaleGroup = new Group(scale: new Vector3(.1, .1, .1)) { triangles };
            scaleGroup.First<Triangles>().Color = Color.Orange;
            // add it to the camera node
            ilPanel1.Scene.Camera.Add(scaleGroup);
            // zoom all for the 3D view
            var limits = scaleGroup.GetLimits(includeRootTransform: true);
            ilPanel1.Scene.Camera.LookAt = limits.CenterF;
            ilPanel1.Scene.Camera.Position = limits.CenterF +  4 * limits.SphereRadius;
            //ilPanel1.Scene.Camera.ZoomFactor = .5;

            // disable user rotation: plot cube 1 only
            pc1.AllowRotation = false; pc1.AllowPan = false; pc1.AllowZoom = false; 

        }

        public static Triangles LoadSTL(string par1) {
            using (Scope.Enter()) {

                #region parse and load vertices
                string[] lines;
                // modify according the number format in the stl file! Here: 0.12345
                System.Globalization.NumberFormatInfo nfi =
                    new System.Globalization.CultureInfo("en-US").NumberFormat;
                if (String.IsNullOrEmpty(par1.Trim()) || !System.IO.File.Exists(par1)) {
                    throw new ArgumentException("Could not load stl file"); 
                } else {
                    lines = System.IO.File.ReadAllLines(par1);
                }
                // predefine array (for performance)
                Array<float> vertexData = LoadSTLFromText(lines, nfi);
                if (vertexData.S[1] == 0) {
                    // try to load as binary 
                    vertexData.a = LoadSTLFromBinary(par1); 
                }
                #endregion

                #region create + configure triangles shape
                Triangles ret = new Triangles("Triangles:" + par1) {
                    Positions = vertexData["0:2;:"],    // all position components
                    AutoNormals = false,                // take normals from stl 
                    Normals = vertexData["3:5;:"],     
                    Color = Color.Gray
                }; 
                #endregion

                return ret;
            }
        }

        private static RetArray<float> LoadSTLFromBinary(string filename) {
            using (Scope.Enter()) {
                byte[] buffer = new byte[100];
                using (BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open))) {
                    // read header (80 bytes)
                    br.Read(buffer, 0, 80);

                    // number triangles 
                    uint count = br.ReadUInt32() * 3; // number vertices
                    Array<float> ret = zeros<float>(6, (int)count);
                    for (int c = 0; c < count; ) {
                        float nx = br.ReadSingle();
                        float ny = br.ReadSingle();
                        float nz = br.ReadSingle();

                        ret[full, c++] = new float[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), nx, ny, nz };
                        ret[full, c++] = new float[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), nx, ny, nz };
                        ret[full, c++] = new float[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), nx, ny, nz };

                        br.ReadUInt16();
                    }
                    return ret;
                }
            }
        }

        private static RetArray<float> LoadSTLFromText(string[] lines, System.Globalization.NumberFormatInfo nfi) {
            using (Scope.Enter()) {
                Array<float> vertexData = zeros<float>(6, (int)(lines.Length / 2.0));
                int count = 0;
                float NX = 0, NY = 0, NZ = 0;
                foreach (string line in lines) {
                    int start = line.IndexOf("vertex");
                    if (start >= 0) {
                        // vertex found
                        start += 6;
                        string[] valsS = line.Substring(start).Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (valsS != null && valsS.Length >= 3) {
                            float X, Y, Z;
                            // vertex is valid (may be..) 
                            X = float.Parse(valsS[0], nfi);
                            Y = float.Parse(valsS[1], nfi);
                            Z = float.Parse(valsS[2], nfi);
                            // store for later 
                            vertexData.SetValue(X, 0, count);
                            vertexData.SetValue(Y, 1, count);
                            vertexData.SetValue(Z, 2, count);
                            vertexData.SetValue(NX, 3, count);
                            vertexData.SetValue(NY, 4, count);
                            vertexData.SetValue(NZ, 5, count);
                            count++;
                        }
                    } else {
                        start = line.IndexOf("normal");
                        if (start >= 0) {
                            // normal entry found
                            start += 6;
                            string[] valsN = line.Substring(start).Trim().Split(' ');
                            if (valsN != null && valsN.Length == 3) {
                                // normal is valid, cache it
                                NX = float.Parse(valsN[0], nfi);
                                NY = float.Parse(valsN[1], nfi);
                                NZ = float.Parse(valsN[2], nfi);
                            }
                        }
                    }
                }
                // truncate temp array, remove trail not needed 
                vertexData.a = vertexData[full, slice(0,count)];
                return vertexData;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ILNumerics;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;
using System.IO;
using ILNumerics.Drawing;
using System.Diagnostics;
using ILNumerics.Drawing.Plotting;

namespace STL_Viewer {
    public partial class Form1 : Form {

        ILNumerics.Drawing.Panel Panel;
        Triangles m_model;
        private const string GroupTag = "STLGroup";

        public Form1() {
            InitializeComponent();
            var menu = new MenuStrip() { Name = "menuStrip1" }; 
            this.Controls.Add(menu);
            var open = new ToolStripMenuItem("Open", null, new EventHandler(openMenuItem_Click));
            menu.Items.Add(open); 
        }
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            Panel = new ILNumerics.Drawing.Panel();
            Panel.Dock = DockStyle.Fill;
            Panel.Load += Panel_Load;
            Controls.Add(Panel);

        }

        private void Panel_Load(object sender, EventArgs e) {
            try {

                // load STL data, Source: http://graphics.stanford.edu/data/3Dscanrep/ 
                setModel(LoadSTL("bunny.stl"));

                Text = "Click me - I'll tell you where!";

            } catch (Exception exc) {
                Trace.WriteLine(exc.ToString());
            }
        }

        private void setModel(Triangles triangles) {
            if (m_model != null) {
                m_model.MouseClick -= Triangles_MouseClick;
            }
            m_model = triangles;

            // We display the triangles in the regular Camera node.
            // Scale it down to unit cube: 
            var sr = 2.0 / m_model.Limits.SphereRadius;
            var tr =
                Matrix4.ScaleTransform(sr, sr, sr) *
                Matrix4.Translation(-m_model.Limits.CenterF);


            var gr = Panel.Scene.Camera.First<Group>(GroupTag);
            if (gr == null) {
                Panel.Scene.Camera.Add(new Group(GroupTag));
                gr = Panel.Scene.Camera.First<Group>(GroupTag);
            }
            gr.Children.Clear();
            gr.Add(m_model);
            gr.Transform = tr;  // the group's Transform property now scales & centers its content

            // setup picking
            m_model.MouseClick += Triangles_MouseClick;

            // adopt this at your will: (these values are optimized for bunny.stl)
            Panel.Scene.Camera.Position = new Vector3(-5.5, -7, 4);
            Panel.Scene.Camera.Top = new Vector3(0.5, 0.7, 0.5);
            Panel.Scene.Camera.ZoomFactor = 0.6;
            Panel.Configure();
            Panel.Refresh();
        }

        private void Triangles_MouseClick(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            var pick = Panel.PickPrimitiveAt(sender as Drawable, e.Location);
            if (!isempty(pick.NextVertex)) {
                // display position of the vertex under the mouse closest to the camera
                Text = pick.VerticesWorld[pick.NextVertex[0], r(0, 2), 0].ToString();
            }
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
                    Positions = vertexData[r(0, 2), r(0, end)],    // all position components
                    AutoNormals = false,                // take normals from stl 
                    Normals = vertexData[r(3, 5), r(0, end)],
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
                    for (int c = 0; c < count;) {
                        float nx = br.ReadSingle();
                        float ny = br.ReadSingle();
                        float nz = br.ReadSingle();

                        ret[full, c + 0] = new float[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), nx, ny, nz };
                        ret[full, c + 1] = new float[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), nx, ny, nz };
                        ret[full, c + 2] = new float[] { br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), nx, ny, nz };
                        c += 3;

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
                vertexData.a = vertexData[full, slice(0, count)];
                return vertexData;
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }
        private void openMenuItem_Click(object sender, EventArgs e) {
            var openFileDialog1 = new OpenFileDialog(); 
            openFileDialog1.DefaultExt = "*.stl";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog1.FileName) && File.Exists(openFileDialog1.FileName)) {
                setModel(LoadSTL(openFileDialog1.FileName));
            }
        }
    }
}

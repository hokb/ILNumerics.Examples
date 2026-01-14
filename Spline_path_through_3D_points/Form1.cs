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
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using ILNumerics.Toolboxes;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace Spline_path_through_3D_points {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private readonly Array<float> m_points = localMember<float>(); 
        private Group MyObjects { get { return ilPanel1.Scene.First<Group>("myGroup"); } }
        private float m_lbDeriv = 0; 

        private void ilPanel1_Load(object sender, EventArgs e) {

            // setup the plot cube ... 
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) {

                new Group("myGroup") {
                    // ... render original points as markers (line color: "empty" hides the line of the line plot)
                    new LinePlot(1f, lineColor: Color.Empty, markerStyle: MarkerStyle.Dot, tag: "Points"),
                    new LinePlot(1f, lineColor: Color.Blue, markerStyle: MarkerStyle.None, tag: "Line")
                }
            });
            Text = "ILNumerics Interpolation Toolbox Example: Spline Path in 3D"; 
            Shuffle(); 
        }

        private void button1_Click(object sender, EventArgs e) {
            Shuffle();
            Draw();
        }

        public void Shuffle() {
            // create 21 random points in 3d space
            m_points.a = tosingle(rand(3, 21));
            Draw(); 
        }

        public void Draw() {

            // remove all existing labels
            foreach (var lab in MyObjects.Find<ILNumerics.Drawing.Label>()) MyObjects.Remove(lab);

            // redraw the points
            MyObjects.First<LinePlot>("Points").Update(m_points); 
            
            // setup labels next to the points
            for (int i = 0; i < m_points.S[1]; i++) {
                // position for the label 
                Vector3 pos = new Vector3(m_points.GetValue(0, i), m_points.GetValue(1, i), m_points.GetValue(2, i));
                // add new label 
                MyObjects.Add(
                    new ILNumerics.Drawing.Label(i.ToString() + " ") {
                        Position = pos,
                        // create some margin 
                        Anchor = new PointF(1.5f, .5f)
                    });
            }

            // generate spline interpolated values for a smoothed path through the points
            Array<float> spl = Interpolation.splinepath(m_points, lbDeriv: ones<float>(3,1) * m_lbDeriv / 365 * 2 - 1);
            // draw the path as regular line plot
            MyObjects.First<LinePlot>("Line").Update(spl);

            ilPanel1.Scene.Configure();
            ilPanel1.Scene.First<PlotCube>().Reset();
            ilPanel1.Refresh(); 

        }

        private void trackBar1_Scroll(object sender, EventArgs e) {
            m_lbDeriv = trackBar1.Value;
            Draw(); 
        }
    }
}

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
using static ILNumerics.ILMath;
using static ILNumerics.Globals; 

namespace WindowsFormsApp1 {
    public partial class Form1 : Form {

        ILNumerics.Drawing.Panel m_panel;
        bool m_isHorizontal = true;
        /// <summary>
        /// Now, that the plot cube is not controlling its render size automatically anymore, we 
        /// have to create free space around the plot cube for labels etc. Find a value best matching your needs. 
        /// Note: for best results in interactive scenes, it might be necessary to implement _dynamic_ padding. 
        /// I.e.: the value for the Padding property may be adjusted according to the current Size of the Form, 
        /// the overall aspect ratio of the scene, the scaling of labels and axis ticks etc... (basically, this 
        /// is what PlotCube normally relieves you from having to deal with).
        /// </summary>
        public float Padding { get; set; } = 0.1f; 

        PlotCube PlotCube {
            get { return m_panel?.Scene.First<PlotCube>(); }
        }
        public Form1() {
            InitializeComponent();
            m_panel = new ILNumerics.Drawing.Panel();
            m_panel.Load += M_panel_Load;
            m_panel.Dock = DockStyle.Fill;
            Controls.Add(m_panel);
        }

        private void M_panel_Load(object sender, EventArgs e) {
            // load terrain data: aspect ratio 2.5
            Array<float> data = tosingle(ILNumerics.SpecialData.terrain[r(1, 50), r(1, 125)]) ;
            var pc = m_panel.Scene.Add(new PlotCube(twoDMode: false));
            
            var sf = new Surface(data) { UseLighting = true };
            pc.Add(sf);
            pc.AllowRotation = false;
            pc.AllowZoom = false;
            pc.AspectRatioMode = AspectRatioMode.StretchToFill;

            pc.Reset();
            pc.MouseDoubleClick += (s, e) => e.Cancel = true;

            SizeChanged += Form1_SizeChanged;
            setDataScreenRect(); 
        }

        private void Form1_SizeChanged(object sender, EventArgs e) {
            setDataScreenRect();
            m_panel.Refresh(); 
        }

        private void button1_Click(object sender, EventArgs e) {
            // toogle orientation
            m_isHorizontal = !m_isHorizontal;
            setDataScreenRect();
            m_panel.Refresh(); 
        }

        private void setDataScreenRect() {
            // PlotCube.DataScreenRect marks the exact extent of the inner plot cube content. 
            // Labels and (outer) axis ticks are not considered for DataScreenRect. 
            // The plot cube will always tightly fit into the area specified by this rectangle. 
            System.Drawing.Size outerSize = m_panel.Size; 
            double angle = m_isHorizontal ? 0 : Math.PI / 2;
            var contentSize = PlotCube.GetLimits();
            double cAR = contentSize.HeightF / contentSize.WidthF;
            // consider angle (TODO: prepare for all angles ?)
            cAR = m_isHorizontal ? cAR : 1f / cAR; 
            double oAR = ((double)outerSize.Height / outerSize.Width) * ((double)PlotCube.ScreenRect.Height / PlotCube.ScreenRect.Width);
            double aR = cAR / oAR;
            // make the DataScreeRect have the same AR as the content
            RectangleF rect;
            if (aR >= 1) {
                float h = 1 - Padding; 
                float w = h / (float)aR;
                rect = new RectangleF((1 - w) / 2f, (1 - h) / 2f, w, h);
            } else {
                float w = 1f - Padding; 
                float h = w * (float)aR;
                rect = new RectangleF((1 - w) / 2f, (1 - h) / 2f, w, h);
            }
            PlotCube.DataScreenRect = rect;
            PlotCube.Rotation = Matrix4.Rotation(Vector3.UnitZ, angle);
            Text = $"Angle: {angle.ToString()}. AR: {aR}."; 
        }
    }
}

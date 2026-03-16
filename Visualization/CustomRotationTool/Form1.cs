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
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;

namespace CustomRotationTool {
    public partial class Form1 : Form {

        System.Drawing.Point? m_mouseStart;
        PointF? m_rotation;  // in radians

        public Form1() {
            InitializeComponent();
        }

        private void panel1_Load(object sender, EventArgs e) {
            var pc = panel1.Scene.Add(new PlotCube(twoDMode: false) {
                new Surface(tosingle(SpecialData.terrain[r(0,99),r(0,99)])) {
                    UseLighting = true
                }  
            });

            pc.MouseDown += Pc_MouseDown;
            pc.MouseMove += Pc_MouseMove;
            pc.MouseUp += Pc_MouseUp;
            pc.MouseDoubleClick += Pc_MouseDoubleClick;

            pc.Projection = Projection.Perspective;

            Text = "ILNumerics Custom Rotation Tool"; 

        }

        private void Pc_MouseDoubleClick(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            m_mouseStart = null;
            m_rotation = null;
            SetRotation(0, 0);
            e.Refresh = true;
            e.Cancel = false; 
        }

        private void Pc_MouseUp(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (m_mouseStart.HasValue) {
                float offsX, offsY;
                GetOffset(e, out offsX, out offsY);

                m_rotation = new PointF(offsX + m_rotation.GetValueOrDefault().X, offsY + m_rotation.GetValueOrDefault().Y);
            }
            m_mouseStart = null;
        }

        private void Pc_MouseMove(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (m_mouseStart.HasValue) {
                float offsX, offsY;
                GetOffset(e, out offsX, out offsY);

                SetRotation(offsX, offsY);

                e.Refresh = true;
                e.Cancel = true;
                Text = $"X:{offsX} Y:{offsY}";
            }
        }

        private void SetRotation(float offsX, float offsY) {
            panel1.SceneSyncRoot.First<PlotCube>().Rotation =
                Matrix4.Rotation(new Vector3(1, 0, 0), -(offsY + m_rotation.GetValueOrDefault().Y)) *
                Matrix4.Rotation(new Vector3(0, 0, 1), -(offsX + m_rotation.GetValueOrDefault().X));
        }

        private void Pc_MouseDown(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            m_mouseStart = e.Location;
        }
        private void GetOffset(ILNumerics.Drawing.MouseEventArgs e, out float offsX, out float offsY) {
            offsX = (float)(e.Location.X - m_mouseStart.GetValueOrDefault().X) / ClientSize.Width * 2;
            offsY = (float)(e.Location.Y - m_mouseStart.GetValueOrDefault().Y) / ClientSize.Height * 2;
            offsX %= (float)(Math.PI * 2.0);
            offsY %= (float)Math.PI;
        }

    }
}

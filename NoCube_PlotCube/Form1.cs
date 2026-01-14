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

namespace NoCube_PlotCube {
    public partial class Form1 : Form {

        ILNumerics.Drawing.Panel panel;

        PlotCube PlotCube { get { return panel?.Scene.First<PlotCube>(); } }

        public Form1() {
            InitializeComponent();
            panel = new ILNumerics.Drawing.Panel();
            Controls.Add(panel);
            panel.Dock = DockStyle.Fill;
            panel.Load += Panel_Load;
        }

        private void Panel_Load(object sender, EventArgs e) {

            Array<float> data = tosingle(ILNumerics.SpecialData.terrain[r(1, 50), r(1, 125)]);
            var pc = panel.Scene.Add(new PlotCube(twoDMode: false));

            var sf = new Surface(data) { UseLighting = true, Colormap = Colormaps.Gray };
            pc.Add(sf);

            // configure plot cube axes aspect ratio for ContentFitMode = Manual: 
            pc.ContentFitRatios = new Vector3(2, 4, 1);  // Z axis will be 4 times smaller than Y axis. X axis is 2 times smaller than Y axis. 
            
            // start display with "UnitCube" setting (was changed to "Manual" by line above)
            pc.ContentFitMode = ContentFitModes.UnitCube;

            // alternative rotation method: 
            pc.RotationMethod = RotationMethods.AltitudeAzimuth; 

            RefreshButtonTexts();

        }

        private void button1_Click(object sender, EventArgs e) {
            PlotCube.AspectRatioMode = (AspectRatioMode)((int)(PlotCube.AspectRatioMode + 1) % 2);
            RefreshButtonTexts();
            panel.Refresh(); 
        }

        private void button2_Click(object sender, EventArgs e) {
            PlotCube.ContentFitMode = (ContentFitModes)((int)(PlotCube.ContentFitMode + 1) % Enum.GetValues(typeof(ContentFitModes)).Length);
            RefreshButtonTexts();
            panel.Refresh();
        }

        private void RefreshButtonTexts() {
            button1.Text = $"AspectRatioMode:\r\n{PlotCube.AspectRatioMode}";
            if (PlotCube.ContentFitMode == ContentFitModes.Manual) {
                button2.Text = $"ContentFitMode:\r\n{PlotCube.ContentFitMode} [{PlotCube.ContentFitRatios.ToString()}]";
            } else {
                button2.Text = $"ContentFitMode:\r\n{PlotCube.ContentFitMode}";
            }
        }
    }
}

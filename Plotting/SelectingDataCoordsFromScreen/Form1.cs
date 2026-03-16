using System;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;
using System.Drawing;

namespace SelectingDataCoordsFromScreen {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // The local panel hosting the scene
        ILNumerics.Drawing.Panel m_panel;
        
        private void Form1_Load(object sender, EventArgs e) {

            // create, connect and configure the ILNumerics.Drawing.Panel: 
            m_panel = new ILNumerics.Drawing.Panel();
            // use Panel_Load for configuring your scene!
            m_panel.Load += Panel_Load;

            Controls.Add(m_panel);
            m_panel.Dock = DockStyle.Fill;
            
        }

        private void Panel_Load(object sender, EventArgs e) {

            // add a new plot cube:
            var pc = m_panel.Scene.Add(new PlotCube(twoDMode: false));
            // add a sine line plot: 
            var lineplot = pc.Add(
                new LinePlot(
                    SpecialData.sincos1Df(150, 1.5).T[0,full], 
                    tag: "MyLinePlot", lineWidth: 4));

            // wire-up a mouse-move handler
            pc.MouseMove += (sendr, args) => {

                // only catch the event once. See: https://ilnumerics.net/mouse-events.html 
                if (args.DirectionUp) {
                    // We must work on the synchronized scene! Only here 
                    // all transforms and effects by interaction are available (auto DataScreenRect, etc.) ! 
                    // The sender object from the synchronized scene is the plotcube: 
                    var pcS = sendr as PlotCube;
                    // The line plot defining the target coordinate system is found by its tag: 
                    var targS = pcS.First<LinePlot>("MyLinePlot");

                    // The conversion is performed by the plotcube.ScreenToData() method. 
                    // It uses pipeline inversion to exactly compute data coordinates from the current viewport. 
                    Text = $"Location: {pcS.ScreenToData(args.LocationF, targS)}";
                }
            };
        }
    }
}

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

namespace WindowsFormsApp1 {
    public partial class Form1 : Form {

        ILNumerics.Drawing.Panel m_panel; 
        public Form1() {
            InitializeComponent();

            // create & configure the panel
            m_panel = new ILNumerics.Drawing.Panel();
            Controls.Add(m_panel);
            m_panel.Dock = DockStyle.Fill;
            
            // create the plot cube
            var plotcube = new PlotCube(twoDMode: false) {
                // add a stretched + translated sphere
                new Group(translate: new Vector3(0,0,8), scale: new Vector3(1,1,4)) {
                    new Sphere()
                },
                // add a stretched cylinder
                new Group(scale: new Vector3(0.5f,0.5f,13)) { 
                    new Cylinder() 
                } 
            };
            m_panel.Scene.Add(plotcube);

            // This locks the aspect ratio of the plot cube contents during rotations: 
            plotcube.AspectRatioMode = AspectRatioMode.MaintainRatios;  // (<- default since 5.4). Try StretchToFill!

            // configure the starting limits of the plot cube (in a way you like / which looks appropriate for your content..)
            plotcube.Limits.Set(new Vector3(-2, -2, - 2), new Vector3(2, 2, 15));

            // all aspect ratios add up: form, controls size, client area, panel size, plot cube screen rect, [data screen rect] ! 
            // In order to have a sphere appearing like a sphere, all these sizes must add up to a final aspect ratio of X : Y = 1!
            ClientSize = new Size(600, 600); 
        }

    }
}

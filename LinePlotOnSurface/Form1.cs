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
    /// <summary>
    /// Stacking objects on top of each other
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        
        private void ilPanel1_Load(object sender, EventArgs e) {
            // the whole setup is done in a single instruction 
            ilPanel1.Scene.Add(
                new Group {
                    // this group establishes a Screen2DFar subtree. Everything within it will be drawn on top of other scene content.
                    new Group(target: RenderTarget.Screen2DFar) {
                        // add several regular line plots from the sample terrain data 
                        new PlotCube(twoDMode: true) {
                            Children = {
                                LinePlot.CreateXPlots(tosingle(SpecialData.terrain["100:110;100:160"]))
                            },
                            // configure the area it occupies on the viewport 
                            DataScreenRect = new RectangleF(.55f,.15f,.4f,.3f),
                            // let's use logarithmic scales 
                            ScaleModes = { XAxisScale = AxisScale.Logarithmic },
                        }
                    },
                    // the 'main scene' content, regular 3D world target 
                    new PlotCube(twoDMode: false) {
                        Children = {
                            new ImageSCPlot(tosingle(SpecialData.terrain["100:140;100:160"]))
                        },
                        // remove some of the empty space arount the default plot cube
                        DataScreenRect = new RectangleF(.15f,.05f,.8f,.8f), 
                    }
                });
        }
    }
}

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

namespace CustomizingColormaps {
    /// <summary>
    /// The example takes a predefined colormap and modifies it. The altered colormap it than used to color a sphere. It is getting displayed 
    /// in a line plot overlay over the sphere and in a common colorbar next to the sphere. 
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {

            ilPanel1.Scene.Camera.Projection = Projection.Perspective; 
            // our scene will consists out of a simply sphere
            var sphere = ilPanel1.Scene.Camera.Add(new Sphere()); 

            // create a custom colormap, based on Colormaps.Jet
            var cm = new Colormap(Colormaps.Jet);
            Array<float> A = cm.Data; 
            //A is now:
            //<Single> [6,5]
            //    [0]:          0          0          0     0,5625          1 
            //    [1]:     0,1094          0          0     0,9375          1 
            //    [2]:     0,3594          0     0,9375          1          1 
            //    [3]:     0,6094     0,9375          1     0,0625          1 
            //    [4]:     0,8594          1     0,0625          0          1 
            //    [5]:          1     0,5000          0          0          1 

            // we modify the colormap: insert an data area which gets colored flat, magenta
            A["5:7;:"] = A["3:5;:"];
            A["3;:"] = vector<float>(A.GetValue(2, 0), 1f, 0f, 1f, 1f);
            A["4;:"] = vector<float>(A.GetValue(5, 0), 1f, 0f, 1f, 1f);
            
            //A is now:
            //<Single> [8,5]
            //    [0]:          0          0          0     0,5625          1 
            //    [1]:     0,1094          0          0     0,9375          1 
            //    [2]:     0,3594          0     0,9375          1          1 
            //    [3]:     0,3594          1          0          1          1 
            //    [4]:     0,6094          1          0          1          1 
            //    [5]:     0,6094     0,9375          1     0,0625          1 
            //    [6]:     0,8594          1     0,0625          0          1 
            //    [7]:          1     0,5000          0          0          1 

            //make sure to place keypoint positions inside the range 0..1! The following 
            // keypoint would create a distorted colorbar, due to position '2f':
            //A["8;:"] = array<float>(2f, 1f, 0f, 1f, 1f);

            // set values to colormap
            cm.SetData(A); 

            // use the colormap to color the sphere
            sphere.Fill.Colors = cm.Map(sphere.Fill.Positions.Storage["1;:"]).T;
            // deactivate solid color (it would take precedence otherwise!)
            sphere.Fill.Color = null; 

            // show the colormap as colorbar ... 
            ilPanel1.Scene.Camera.Add(new Colorbar() {
                ColormapProvider = new StaticColormapProvider(cm,-1,1),
                Axis = { Min = -1, Max = 1 }  // <-- needed for ILNumerics prior 4.1 only
            }); 

            // ... and as line plot
            ilPanel1.Scene.Add(new PlotCube {
                Children = {
                    new LinePlot(A.T["0;:"],lineColor: Color.Black, markerStyle: MarkerStyle.Dot), 
                    new LinePlot(A.T["1;:"],lineColor: Color.Red, markerStyle: MarkerStyle.Dot, lineWidth: 3), 
                    new LinePlot(A.T["2;:"],lineColor: Color.Green, markerStyle: MarkerStyle.Dot, lineWidth: 3), 
                    new LinePlot(A.T["3;:"],lineColor: Color.Blue, markerStyle: MarkerStyle.Dot, lineWidth: 3), 
                    new Legend("Position","Red","Green","Blue")
                },
                ScreenRect = new RectangleF(0,.5f,.5f,.5f)
            }); 
            
        }
    }
}

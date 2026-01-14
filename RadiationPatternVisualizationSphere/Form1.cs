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

namespace RadiationPatternVisualizationSphere {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            using (Scope.Enter()) { // follow the ILNumerics function rules: https://ilnumerics.net/GeneralRules.html
                
                // set up the 'ground' plate
                var bg = ilPanel1.Scene.Camera.Add(
                    new Group(translate: new Vector3(-.5, -.5, 0)) {
                        Children = {
                        // a predefined rectangle 
                        Shapes.RectangleFilled
                        },
                        // color all children of the group
                        ColorOverride = Color.BurlyWood
                    });

                // read the radiation pattern data 
                var csv = System.IO.File.ReadAllText("Test_measured_Data.csv");
                Array<float> data = csvread<float>(csv, 1);
                // bring the data into the shape required by surface plots
                data.a = reshape(data, 181, 360, 3);

                // Note: here we can already inspect the data conveniently with the Array Visualizer. 
                // In a debug session enter 'data[":;:;2"]' into the Array Visualizer Expression Window
                // and choose a surface as output. See: https://ilnumerics.net/Examples/RadiationPatternVisualizationSphere/b_ArrayVisualizer_debug_data_surface_visualization.png

                // convert from spherical coordinates to cartesian 
                Array<float> Y = 0, Z = 0, X = sphere2cart(
                    ones<float>(181, 360) + data[full, full, 2] / 100f, // creates a relief with the magnitude data
                    data[full, full, 0] / 180.0f * pif,
                    data[full, full, 1] / -180.0f * pif, Y, Z);  // TODO: check '-'! 
                Array<float> ZXY = zeros<float>(data.S);
                ZXY[full, full, 0] = Z + 1.01f; // 'moves' the sphere up above the 'ground' rectangle
                ZXY[full, full, 1] = X;
                ZXY[full, full, 2] = Y;

                // create color data
                var cm = new Colormap(Colormaps.ILNumerics);
                Array<float> C = reshape(cm.Map(data[full, full, 2]), 181, 360, 4);
                // add sphere (parametric surface)
                ilPanel1.Scene.Camera.Add(new Surface(ZXY, C: C) {
                    Wireframe = { Visible = false },
                    UseLighting = true
                });

                // compute the rotation center: the average of coordinates along the 1st two dimensions of ZXY: 
                Array<float> midPoint = sum(sum(ZXY, dim: 0), dim: 1) / (ZXY.S.NumberOfElements / 3);
                // note the order: Z comes first: 
                var center = new Vector3(midPoint.GetValue(1), midPoint.GetValue(2), midPoint.GetValue(0));
                
                ilPanel1.Scene.Camera.RotationCenter = center;
                ilPanel1.Scene.Camera.LookAt = center;
                ilPanel1.Scene.Camera.ZoomFactor = 0.8f; 
                // show the colormap as colorbar ... 
                float min, max;
                data[full, full, 2].GetLimits(out min, out max);
                ilPanel1.Scene.Camera.Add(new Colorbar() {
                    ColormapProvider = new StaticColormapProvider(cm, min, max),
                });

                // todo: add wireframe, interpolate data, tune magnitude / relief height etc...
            }
        }
    }
}

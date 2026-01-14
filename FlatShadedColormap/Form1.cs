using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace FlatShadedColormap {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // create some terrain data
            Array<float> A = tosingle(SpecialData.terrain["0:400;0:400"]);
            // derive a 'flat shaded' colormap from Jet colormap: 
            // at first, we fetch create the colormap ... 
            var cm = new Colormap(Colormaps.Jet);
            // ... and fetch the data (keypoints) from it. 
            Array<float> cmData = cm.Data;
            // a computing module is used to turn that into a keypoints representing a flat shaded colormap based on the Jet template. 
            // We simply re-use the same ILArray variable here. By utilizing the .a property for assignment we make sure that the 
            // memory formerly used up by cmData is returned to the pool for immediate reuse. 
            cmData.a = Computation.CreateFlatShadedColormap(cmData);
            // reapply the data to the colormap
            cm.SetData(cmData); 
            // display interpolating colormap
            ilPanel1.Scene.Add(new PlotCube() { 
                // add a surface plot as child of the plotcube
                Plots = {
                    new Surface(A, colormap: Colormaps.Jet) {
                        Children = { new Colorbar() },
                        Wireframe = { Visible = false }
                    }
                }, 
                // dock the plot cube to the upper half of the form
                ScreenRect = new RectangleF(0,-0.05f,1,0.6f)
            }); 

            // display flat shading colormap
            ilPanel1.Scene.Add(new PlotCube() {
                // add anothere surface, this time using our modified colormap
                Plots = {
                    new Surface(A, colormap: cm) {
                        Children = { new Colorbar() },
                        Wireframe = { Visible = false }
                    }
                },
                // dock the plot cube to the lower half of the panel
                ScreenRect = new RectangleF(0, 0.40f, 1, 0.6f)
            });
            // a title for the plot
            Text = "Flat Shaded Colormap"; 
        }

        /// <summary>
        /// a computing module for more convenient (shorter) algorithm implementations
        /// </summary>
        private class Computation {
            public static RetArray<float> CreateFlatShadedColormap(InArray<float> cm) {
                // for details of the storage scheme for colormap keypoints see here: 
                // http://ilnumerics.net/managing-colormaps.html
                using (Scope.Enter(cm)) {
                    // create array large enough to hold new colormap
                    Array<float> ret = zeros<float>(cm.S[0] * 2 - 1, cm.S[1]);
                    // copy the original
                    ret[r(0, cm.S[0] - 1), full] = cm; 
                    // double original keypoints, give small offset (may not even be needed?) 
                    ret[r(cm.S[0], end), 0] = cm[r(1, end), 0] - epsf;
                    ret[r(cm.S[0], end), r(1, end)] = cm[r(0, end - 1), r(1, end)];
                    // reorder to sort keypoints in ascending order
                    Array<long> I = 1;
                    sort(ret[full, 0], Indices: I);
                    return ret[I, full];
                }

            }
        }
    }
}

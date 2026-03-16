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

namespace BarGraph {
    public partial class Form1 : Form {
        /// <summary>
        /// Creates a simple bar plot in 3D, similar to an ImageSC plot. Each element of the input matrix creates a bar, mapping the value of the element to the height and color of the bar. 
        /// All bars are created by reusing and scaling a single unit cube shape. All bar share the same set of vertex buffers. However, they are still 
        /// individually configurable: here we apply an individual color to each bar. 
        /// </summary>
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {

           Array<float> AA = SpecialData.sincf(20, 10);
            AA["4;:"] = 0; 
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) { 
                 new BarPlotEx(Z: SpecialData.sincf(10, 10, 1.5f))
                {
                    BarWidth = 0.8f,
                    BaseValue = -0.5f,
                    Colormap = Colormaps.Jet,
                    ColorMode = BarPlotEx.ColorModes.Colormapped,
                    Border = { Antialiasing = false , Color = Color.FromArgb(21, Color.Black) },
                }
                // new ILFastSurface(tag: "BarEx")
            });
            ilPanel1.Scene.First<PlotCube>().Lines.Visible = false;
            ilPanel1.Scene.First<PlotCube>().Axes.Visible = false;


            //ilPanel1.Scene.First<ILFastSurface>().Update(Z: AA.T); 
            return; 
            // create some sample data
            Array<float> A = SpecialData.sincf(10, 25);
            // create a colormap, used for mapping the values to colors later
            Colormap cm = new Colormap(Colormaps.Jet); 
            // determine the overall range of values in A
            float min, max; 
            A.GetLimits(out min, out max); 
            // create the range (used later for mapping the colors) 
            var range = Tuple.Create(min, max); 

            // setup the plot cube 
            var pc = ilPanel1.Scene.Add(new PlotCube(twoDMode: false));
            // this is going to be the shape which is reused for each bar. Every time it is used, a shallow copy will be 
            // created from it. The buffers will be shared with the old shape. 
            var cube = Shapes.UnitCubeFilledLit; 
            // iterate the elements of A and create boxes for each element
            for (int c = 0; c < A.S[1]; c++) {
                for (int r = 0; r < A.S[0]; r++) {
                    // use the value as height for scaling
                    float a = A.GetValue(r, c); 
                    // add the box in a group. The group is used to move the box to the position of the current element 
                    // and to scale the box along Z according to the value of the current element. 
                    var boxGroup = pc.Add(new Group(translate: new Vector3(c,r,0), scale: new Vector3(1,1,a)) { 
                        // add the box (unit cube) to the group. All bars share the same shape buffers here, so we will 
                        // not copy any memory, hence improve performance. 
                        cube 
                    });
                    // color the box according to its height (value of A); we fetch the only 
                    // triangles shape (the one we just created from 'cube' and simply set 
                    // its color to a value, mapped from the value of the element. 
                    // Since we map each bars color individually, we must provide the overall element value 
                    // range to the Map() function.
                    boxGroup.First<Triangles>().Color = cm.Map(a, range).ToColor();
                }
            }
        }
    }
}

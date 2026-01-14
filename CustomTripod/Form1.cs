using ILNumerics;
using ILNumerics.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomTripod {
    public partial class Form1 : Form {
        /// <summary>
        /// The example implements a custom tripod for your scene. The default tripod is taken as a starting point. The first wireframe of the actual scene 
        /// is added to the tripod. As the result, a miniaturized version of the scene is shown as part of and act as the tripod. 
        /// </summary>
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // a simple scene
            ilPanel1.Scene.Camera.Add(new Group { Shapes.Gear2Wireframe, Shapes.Gear2 });

            // adding a common tripod to the scenes camera 
            ilPanel1.Scene.Camera.Add(new Tripod(style:TripodStyle.Tripod3D) {  // you could use style: TripodStyle.TripodEmpty here if you do not want the default tripod also
                // within the tripod we add a scaled down version of the gear shape wireframe. We take it directly from the scene. So any potential changes 
                // to the shape would show up here as well. 
                new Group(scale: new Vector3(0.3f, .3f, .3f)) {
                    ilPanel1.Scene.Camera.First<Lines>() 
                }
            });
            Array<complex> A = ILMath.ccomplex(ILMath.rand(10, 20), ILMath.randn(10, 20));
            Array<System.Numerics.Complex> C = ILMath.reinterpret_cast<complex,System.Numerics.Complex>(A); 
            Array<complex> C_iln = ILMath.reinterpret_cast<System.Numerics.Complex, complex>(C);
        }
    }
}

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

namespace Titles_on_Plots {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // let's give the scene some useful content first. Titles will always go on top of everything else. 
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) { new Surface(SpecialData.sincf(40, 30)) });

            // generate titles for all predefined ILTitle positioning values: 
            var vals = Enum.GetValues(typeof(Positioning));
            foreach (Positioning v in vals) {
                ilPanel1.Scene.Add(new Title(v.ToString() + (v == Positioning.TopRight ? " (default)" : ""), position: v));
            }
            // one can still position the title freely on the viewport. Just omit the initial positioning enum and provide 
            // custom values for Location and Anchor: 
            var position = new PointF(.3f, .3f);
            var anchor = new PointF(.5f, .5f);
            ilPanel1.Scene.Add(new Title("Custom Position: \r\n" + position.ToString()) {
                Location = position,
                Anchor = anchor
            });
            // configure custom padding. we add some more space at the bottom and add a shape to the title
            var title = ilPanel1.Scene.Add(new Title("Large Title with Shapes") {
                new Group(scale: new Vector3(.12,.2,.3), translate: new Vector3(.45,.65,.5), rotateAxis: new Vector3(1,1,2), angle: .2) {
                    Shapes.Gear10, Shapes.Gear10Wireframe
                },
                new Group(scale: new Vector3(.12,.2,.3), translate: new Vector3(.15,.55,.5), rotateAxis: new Vector3(1,1,2), angle: .2) {
                    Shapes.TriangleEquilateralInterpLit
                },
                new Group(scale: new Vector3(.1,.15,.3), translate: new Vector3(.8,.55,.5), rotateAxis: new Vector3(1,1,1), angle: .5) {
                    Shapes.UnitCubeFilledLit, Shapes.UnitCubeWireframe
                }

            });
            title.Padding = new ILNumerics.Drawing.Padding(10, 10, 10, 60);
            title.Location = new PointF(.8f, .3f); 
            // modify the font and color of the title text
            title.Label.Font = new System.Drawing.Font("Courier", 14); 
            // modify the title border color
            title.Border.Color = Color.LightBlue; 
            
            // forms title 
            Text = "Titles Example";
            // one can change the text of the title at runtime. Trigger the change by double clicking on the title. 
            // You may implement another logic and might take the new text from an input box or the like...
            title.MouseDoubleClick += title_MouseDoubleClick;
        }

        /// <summary>
        /// Callback changing the text of the title 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void title_MouseDoubleClick(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            var title = sender as Title;
            if (title != null) {
                // the text is appended by a varying number of random length to demonstrate the adaption
                // of the titles size to the text.
                title.Label.Text = "New TEXT! \r\n----- LOOOOOOONNNGGGGG.... " + Environment.TickCount.ToString().Substring(0, Environment.TickCount % 10);
                title.Configure();
                ilPanel1.Refresh(); 
            }
        }
    }
}

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

namespace WindowsFormsApplication1
{
    /// <summary>
    /// This example demonstrates how a global scene can be shared between two panels and how events are registered on objects of that scene. 
    /// It registers event handlers on both: the global scene as well as on the synchronized scenes of both panels. Clicking on any object will 
    /// show the events in the output window of Visual Studio. 
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            MessageBox.Show("Click on a box and see which events are fired from the Visual Studio Output Window. (Start this in debug mode!)"); 
            // setup the scene
            scene.Camera.Add(Shapes.UnitCubeFilledLit);
            scene.Camera.Add(Shapes.UnitCubeWireframe);
            // register a mouse click event on the global scene, the one which is later used for both panels. 
            scene.First<Triangles>("UnitCubeFilledLit").MouseClick += (_s, _a) => { System.Diagnostics.Debug.WriteLine("Global Scene"); };
            // Don't forget to call configure! Always. (The only place where this is not needed is in the load event handler of ilpanel. There we do it for you.)
            scene.Configure();
        }

        // our 'global' scene
        Scene scene = new Scene(); 

        private void ilPanel2_Load(object sender, EventArgs e) {

            // This method is called BEFORE ilPanel1_Load, since ilPanel2 was dragged last to the form. The VS designer 
            // adds subsequent controls to the BEGINNING of the controls list. Hence, panel2 gets loaded first. 
            // See Form1.Designer.cs for details... 

            // Just assign the global scene. This will not copy anything yet! The scene has not been used yet so 
            // the assignement will do just that: assign the same reference.
            ilPanel2.Scene = scene;

            // Note, we do not center the camera lookat point here. Instead we will do it later (below) in ilPanel1_Load. 
            //ilPanel2.Scene.Camera.LookAt = new Vector3(.5, .5, .5); 

            // fetch the cube shape from the synchronized scene, specific for this panel2.
            var tri = ilPanel2.SceneSyncRoot.First<Triangles>("UnitCubeFilledLit");
            tri.MouseClick += (_s, _a) => { System.Diagnostics.Debug.WriteLine("Panel2 SceneSynchedRoot"); };

            // add a label to the panel. We want the label to be unique for each panel. Hence we use the LocalScene 
            // property of ilPanel2 in order to add the label. This can be seen as a 2nd scene, destined to be used for 
            // all objects which are not to be shared with other drivers (like the the global scene does). 
            ilPanel2.LocalScene.Add(new ScreenObject(width: 100, height: 40) {
                Location = new PointF(.5f,.1f),
                Children = {
                    new ILNumerics.Drawing.Label("Panel 2", tag: "label") {
                        Anchor = new PointF(.5f,.5f)
                    }
                }
            });
        }

        private void ilPanel1_Load(object sender, EventArgs e) {
            // This method gets called AFTER ilPanel2_Load! See above... 

            // just assign the global scene. This will assign the scene scene object used for ilPanel2 already.
            ilPanel1.Scene = scene;
            // place the lookat point of the camera to the center of the cube. 
            // ILPanel.Scene refers to the global scene! So here were are about to 
            // modify the global camera which affects both: ilPanel1 and ilPanel2! 
            ilPanel1.Scene.Camera.LookAt = new Vector3(.5, .5, .5);
            // fetch the cube shape from the synchronized scene. This corresponds to the synchronized 
            // copy which is, specific for this panel1 only.
            var tri = ilPanel1.SceneSyncRoot.First<Triangles>("UnitCubeFilledLit");
            // registering a mouse event handler on this synched copy will only react to clicks to this panel1
            tri.MouseClick += (_s, _a) => { System.Diagnostics.Debug.WriteLine("Panel1 SceneSynchedRoot"); };

            // add a label to the panel. We want the label to be unique for each panel. Hence we use the LocalScene 
            // property of ilPanel1 in order to add the label. Note how using ilPanel1.LocalScene instead of ilpanel1.Scene
            // does not affect the global scene which is used by both panels. 
            ilPanel1.LocalScene.Add(new ScreenObject(width: 100, height: 40) {
                Location = new PointF(.5f, .1f),
                Children = {
                    new ILNumerics.Drawing.Label("Panel 1", tag: "label")
                }
            });

        }

    }
}

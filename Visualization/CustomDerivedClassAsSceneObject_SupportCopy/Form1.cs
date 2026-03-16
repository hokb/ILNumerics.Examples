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

namespace CustomDerivedClassAsSceneObject_SupportCopy {
    /// <summary>
    /// This example creates a custom scene graph object class which is able to get shallow copied by the 
    /// ILNumerics Visualization Engine. One can use a single object and add it multiple times to a scene.
    /// The resulting copies of the object / its children will still reflect the actual type of the custom class. 
    /// This is usefull, if the class manages complex scenes and provides properties and functions for updates which 
    /// need to be accessible at runtime. 
    /// The example defines a custom ScalableCube class which exposes two properties: Text and Color. A new instance 
    /// of the class is added to the scene twice: to a plotcube and to the default camera. The properties of both resulting 
    /// instances are than used interactively in order to change properties of the class via a color button and a text box. 
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // this is our global instance of a custom scene graph object class. 
        // we have two instances: 
        ScalableCube m_scalableCube_PlotCube;
        ScalableCube m_scalableCube_Camera; 

        private void ilPanel1_Load(object sender, EventArgs e) {
            // Create two areas on the panel. The default camera on the left:
            ilPanel1.Scene.Camera.ScreenRect = new RectangleF(0, 0, .5f, 1); 
            // .. and a plot cube on the right: 
            var pc = ilPanel1.Scene.Add(new PlotCube(twoDMode: false) { 
                ScreenRect = new RectangleF(.5f,0,.5f,1) 
            }); 

            // create a custom scene object
            m_scalableCube_PlotCube = new ScalableCube();  
            // add the object to the plot cube. This is the first instance in any scene 
            // (it has no Parent yet). Hence it is directly used in the scene - no copy needed.
            var dummy = pc.Add(m_scalableCube_PlotCube); 
            // Assert: dummy == m_scalableCube 
            // ... and to the default camera. Since m_scalableCube_PlotCube is already part 
            // of the scene, a copy will be done and returned. 
            m_scalableCube_Camera = ilPanel1.Camera.Add(m_scalableCube_PlotCube);
            
        }

        private void button1_Click(object sender, EventArgs e) {
            if (button1.Text == "Red") {
                m_scalableCube_PlotCube.Color = Color.Red;
                m_scalableCube_Camera.Color = Color.Red;
                button1.Text = "Blue";
                button1.BackColor = Color.Blue; 
            } else {
                m_scalableCube_PlotCube.Color = Color.Blue;
                m_scalableCube_Camera.Color = Color.Blue;
                button1.Text = "Red";
                button1.BackColor = Color.Red;
            }
            ilPanel1.Refresh(); 
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            m_scalableCube_PlotCube.Text = (sender as TextBox).Text;
            m_scalableCube_Camera.Text = (sender as TextBox).Text;
            ilPanel1.Refresh(); 
        }
    }
}

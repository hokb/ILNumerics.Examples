using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.Globals;
using static ILNumerics.ILMath;

namespace ColormappedMatrixChart
{
    public partial class Plotting_Form1 : Form
    {

        public Plotting_Form1()
        {
            InitializeComponent();
            ilPanel1 = new ILNumerics.Drawing.Panel();
            ilPanel1.Load += new EventHandler(ilPanel1_Load);
            Controls.Add(ilPanel1);
            ilPanel1.Dock = DockStyle.Fill; 
        }

        private ILNumerics.Drawing.Panel ilPanel1;

        // Initial plot setup, modify this as needed
        private void ilPanel1_Load(object sender, EventArgs e)
        {
            // Create scene
            var scene = new Scene();
            // Array containing a third feature
            Array<float> A = ILMath.tosingle(ILMath.rand(7, 12) * 1000);
            // Create a colormap
            var cm = new Colormap(Colormaps.Winter);
            // Get min and max values
            A.GetLimits(out float min, out float max);
            // Map values to colors
            Array<float> colors = cm.Map(A, Tuple.Create(min, max)).T;
            // Create data for labels
            Array<string> weekdays = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday",
                                         "Saturday", "Sunday" };
            Array<string> months = new string[] { "January", "February", "March", "April", "May", "June",
                                       "July", "August", "September", "October", "November", "December" };
            int k = 0;
            // Rows of matrix chart
            for (int j = 0; j < 7; j++)
            {
                // Columns of matrix chart
                for (int i = 0; i < 12; i++)
                {
                    // Create rectangles
                    var rectangle = scene.Camera.Add(new RoundedRectangle());
                    // Assign color
                    rectangle.Triangles.Color = Color.FromArgb(255, (int)(255 * colors.GetValue(0, k)), (int)(255 * colors.GetValue(1, k)), (int)(255 * colors.GetValue(2, k)));
                    rectangle.Border.Visible = true;
                   
                    rectangle.Scale(0.05, 0.05, 0);
                    rectangle.Translate(-0.6 + i * 0.11, 0.33 - j * 0.11, 0);
                    k = k + 1;
                    // Create vertical labels
                    if (j.Equals(0))
                    {
                        var label1 = scene.Camera.Add(new ILNumerics.Drawing.Label((string)months[i]));
                        label1.Position = new Vector3(-0.6 + i * 0.11, 0.45, 0);
                        label1.Color = Color.Gray;
                        label1.Font = new Font("Helvetica", 11);
                        label1.Rotation = -pi / 2;
                        label1.Anchor = new PointF(0, 0.5f);
                    }
                }
                // Create horizontal labels
                var label2 = scene.Camera.Add(new ILNumerics.Drawing.Label((string)weekdays[j]));
                label2.Position = new Vector3(-1.0, 0.33 - j * 0.11, 0);
                label2.Color = Color.Gray;
                label2.Font = new Font("Helvetica", 11);
                label2.Anchor = new PointF(0, 0.5f);
            }
            // Create color bar
            var colorBar = scene.Camera.Add(new Colorbar());
            colorBar.ColormapProvider = new StaticColormapProvider(cm, min, max);
            colorBar.Location = new PointF(0.9f, 0.875f);
            colorBar.Anchor = new PointF(.5f, 1.0f); 

            // position the camera - controls the display area, position / size of chart
            scene.Camera.LookAt = new Vector3(0, 0.2, 0);
            scene.Camera.ZoomFactor = 1.3;

            ilPanel1.Scene = scene;

        }

    }
}

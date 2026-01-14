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

namespace ColorbarWithCustomObjects {

    /// <summary>
    /// This example demonstrates two ways to make colorbars available to arbitrary objects.  
    /// </summary>
    /// <remarks><para>Colorbar requires several information at runtime: The colormap used and the upper/lower value 
    /// limits of the axis. These information need to be provided to the colorbar at runtime.</para>
    /// <para>The simple approach is implemented in the ilPanel1_Load handler method. It uses a predefined 
    /// colormap to color a common points shape. 'StaticColormapProvider' is utilized in order 
    /// to provide the colorbar with the needed information at runtime. </para>
    /// <para>The second Panel is setup with another approach. Here a custom scene graph object is created: 'ColoredPoints'. 
    /// This object is capable of participating in the synchronization scheme in ILNumerics Visualization Engine. It implements 
    /// all methods needed in order to get populated to the synchronized scene used for rendering. Therefore, it is able to 
    /// serve as IILColormapProvider itself. This enables you, to get full control about all properties of colormaps, axis parameters 
    /// and even to implement individual custom interactive functionality on the scene object. </para>
    /// <para>This is feasible, if such extended options are needed. If all you need is a statically colored object and a colorbar 
    /// next to it, StaticColormapProvider is the way to go.</para></remarks>
    public partial class MainForm : Form {
        public MainForm() {
            System.Diagnostics.TextWriterTraceListener listener = new System.Diagnostics.TextWriterTraceListener("Trace.Log");
            System.Diagnostics.Trace.Listeners.Add(listener);
            System.Diagnostics.Trace.WriteLine($"ILNumerics : Trace log listener installed."); 
            System.Diagnostics.Trace.WriteLine($"Running version: {typeof(ILMath).Assembly.GetName().Version.ToString()} - Computer: \"{Environment.MachineName}\" - User: \"{Environment.UserName}\" - Domain: \"{Environment.UserDomainName}\""); 
            InitializeComponent();
        }

        private void ilPanel1_Load(object sender, EventArgs e) {

            // We need to provide colormap data to the colorbar at runtime. 
            // We simply take a static colormap provider here. It is sufficient 
            // in order to show static colormap colored data and a colorbar next to it. 
            var cmProv = new StaticColormapProvider(Colormaps.ILNumerics, 0, 1);
            // Position data for plotting
            Array<float> A = ILMath.tosingle(ILMath.rand(3, 1000));
            // create the points
            ilPanel1.Scene.Add(
                new PlotCube(twoDMode: false) {

                   new Points("myPoints") {
                        Positions = A,
                        // since we want to show a colorbar, we need to put the points colors under colormap control
                        Colors = cmProv.Colormap.Map(A["1;:"]).T,
                        // deactivate single color rendering
                        Color = null
                    },
                    // add the colorbar (somewhere) and give it the colormap provider
                    new Colorbar() {
                        ColormapProvider = cmProv
                    }
                }); 
        }

        private void ilPanel2_Load(object sender, EventArgs e) {

            // This time we use our fancy ColormappedPoints class. It allows to 
            // handle colormaps at runtime without the help of StaticColormapProvider. 
            // Instead it provides the colormap itself. This eases the utilization of the 
            // class significantly: The user just needs to add the colorbar to its children. 
            // All colorbar enabled plots, like Surface, Contourplot etc. use exactly 
            // this scheme in order to support IILColormapProvider and other interfaces. 
            // However, that convenience comes to the price of higher complexity inside the 
            // class - as can be seen from ColormappedPoints.cs.
            // Position data for plotting
            Array<float> A = ILMath.tosingle(ILMath.rand(3, 1000));
            // create the points
            ilPanel2.Scene.Add(
                new PlotCube(twoDMode: false) {
                   // having a "true" scene graph object brings some more convenience to the user: 
                   new ColormappedPoints(A) { new Colorbar() } 
                });
        }
    }
}

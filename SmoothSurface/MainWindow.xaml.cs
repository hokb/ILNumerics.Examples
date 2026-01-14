#define FIXED_POINTS   // This configures to use 9 fixed points as scattered test data. Other options: TERRAIN_POINTS, CSV_POINTS (see below)

using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace ScatteredDataSurfaceExample {

    /// <summary>
    /// This is an example demonstrating the use of a custom scattered data surface class in WPF with .NET Core. Please read the introduction 
    /// (getting started guides) first: https://ilnumerics.net/first-drawing-module.html#manualsetup 
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        /// <summary>
        ///  Convenience property to easily access the ILNumerics panel.
        /// </summary>
        internal ILNumerics.Drawing.Panel Panel {
            get {
                return WFHost.Child as ILNumerics.Drawing.Panel;
            }
        }
        /// <summary>
        ///  Convenience property to easily access the plot cube of the scene.
        /// </summary>
        internal PlotCube PlotCube {
            get {
                return Panel?.Scene.First<PlotCube>();
            }
        }
        /// <summary>
        ///  Convenience property to easily access the surface object with smoothed data.
        /// </summary>
        internal SmoothSurface Surface { 
            get {
                return PlotCube?.First<SmoothSurface>(); 
            } 
        }

        private void WFHost_Loaded(object sender, RoutedEventArgs e) {

            // CREATE THE ILNumerics PANEL 
            var panel = new ILNumerics.Drawing.Panel();
            panel.RendererType = RendererTypes.GDI;  // did you know that we can run w/o hardware acceleration, too? 

            // WPF: ASSIGN THE PANEL AS CHILD OF THE WindowsFormsHost
            WFHost.Child = panel;

            // ADD A PlotCube TO THE SCENE. It is later referred to by the local property 'PlotCube' (defined above). 
            Panel.Scene.Add(new PlotCube(twoDMode: false) { AspectRatioMode = AspectRatioMode.StretchToFill });
            CallMe(); 
            return; 
            // CREATE SCATTERED DATA POINTS

#if FIXED_POINTS
            Array<float> xyPoints = new float[,] {
              {0.04f,0.01f,-0.02f,0.51f,0.492f,0.501f,1.02f,0.982f,1},
              {0.01f,1,1.992f,0.02f,1.01f,1.982f,0.08f,1.01f,2.02f}
            };
            Array<float> Z = vector<float>(1, 1, 1, 1, 2, 1, 1, 1, 1).T;
            
#elif TERRAIN_POINTS
            // Scattered Data: we pick some random values from the built-in SpecialData.terrain data set. 
            Array<uint> pickValues = touint32(rand(1, 200) * 401 * 401);  // sequential indices into terrain matrix
            Array<float> xyPoints = tosingle(vertcat(pickValues / 401, // X 'positions' of sequential indices in terrain matrix
                                            pickValues % 401)); // Y 'positions' of sequential indices in terrain matrix
            Array<float> Z = tosingle(SpecialData.terrain[pickValues]);  // pick random values by sequential indices. They serve as our 'scattered data' here.

#elif CSV_POINTS
            // OR, YOU COULD ALSO READ DATA FROM CSV FILES: 
            //var txt = File.ReadAllText("Filename.txt");
            //Array<float> read = ILMath.csvread<float>(txt, 0, 0, elementSeparator: "\t");   <-- here: x,y,z data in rows, tab-separated. Adjust for you own format!
            //xyPoints = read.T[r(0, 1), r(0, 50, end)];  // too many data? pick every 50th sample only!
            //Z = read.T[2, r(0, 50, end)];
#endif

            // CREATE THE SMOOTH SURFACE ...
            var smooth = new SmoothSurface(xyPoints, Z,
                             margin: new System.Drawing.SizeF(.5f, .1f),
                             resolution: new System.Drawing.Size(100, 100),
                             smoothingFactor: 0);
            // ... AND ADD IT TO THE PlotCube:
            PlotCube.Add(smooth);

            // some plot cube configuration (optional)
            PlotCube.Rotation = Matrix4.Rotation(new Vector3(1, 0, 0), 1.0) *
                       Matrix4.Rotation(new Vector3(0, 0, 1), 1.0); 
            PlotCube.AspectRatioMode = AspectRatioMode.StretchToFill;

            //change the color and size of the original points(optional)
            smooth.PointsOriginal.Color = System.Drawing.Color.OrangeRed;
            smooth.PointsOriginal.Size = 3;

            // some more configuration to the surface (optional)
            smooth.Surface.Colormap = Colormaps.Jet;
            smooth.Surface.Add(new Colorbar() { Location = new System.Drawing.PointF(8.12f, 0.1f) });

            // make the plot cube show the whole data            
            PlotCube.Reset();
            
            // after finishing your scene configuration always call Configure()!
            Panel.Configure(); 

        }
        void CallMe() {
            Array<float> xyPoints = new float[,] {
  {0,0,0,0.5f,0.5f,0.5f,1,1,1},
  {0,1,2,0,1,2,0,1,2}
};
            Array<float> Z = vector<float>(1, 1, 1, 1, 2, 1, 1, 1, 1).T;

            // NOW, CREATE THE SMOOTH SURFACE
            var smooth = new SmoothSurface(xyPoints, Z,
                            // create 30 points into X-direction and 100 points into Y direction			   
                            resolution: new System.Drawing.Size(30, 100),
                            smoothingFactor: 15);

            // ... AND ADD IT TO THE PlotCube:
            var scene = new Scene() {
  new PlotCube(twoDMode: false) {
    Children = { smooth },
    AspectRatioMode = AspectRatioMode.StretchToFill
  }
};

            // change the color of the original points (optional)
            smooth.PointsOriginal.Color = System.Drawing.Color.Red;
            smooth.PointsOriginal.Size = 8;

            // DO SOME CONFIGURATIONS TO THE SURFACE (optional)
            smooth.Surface.Colormap = Colormaps.Jet;
            smooth.Surface.Add(new Colorbar() { Location = new System.Drawing.PointF(0.92f, 0.1f) });

            // after finishing configuration always call Configure() on the scene! 
            scene.Configure();
            // PlotCube configuration: show all data 
            scene.First<PlotCube>().Reset();
            // set initial plotcube rotation 
            scene.First<PlotCube>().Rotation = Matrix4.Rotation(new Vector3(1, 0, 0), 1.0) *
                                               Matrix4.Rotation(new Vector3(0, 0, 1), 1.0);

            Panel.Scene = scene; 
            //Panel.Configure(); 
        }
        private void ResolutionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (!object.Equals(Surface, null)) {
                // Set the new resolution value. This recomputes the interpolation grid, based on the same scattered data points. 
                Surface.Resolution = new System.Drawing.Size((int)e.NewValue, (int)e.NewValue);
                // update the visuals 
                lblResolution.Content = $"Resolution: {(int)e.NewValue}";
                Surface.Configure();  // dont forget to call Configure() after all configuration to an ILNumerics object are done.
                Panel.Refresh();
            }
        }
        private void AvgSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (!object.Equals(Surface, null)) {
                uint sf = (uint)e.NewValue ;  // selecting only even values for SmoothingFactor. This way a nicly centered view is maintained 
                                                 // while stepping through individual values.
                Surface.SmoothingFactor = sf; 
                // update the visuals
                lblAvg.Content = $"Smoothness: {sf}"; 
                Surface.Configure();    // dont forget to call Configure() after all configuration to an ILNumerics object are done.
                Panel.Refresh();
            }
        }
    }
}

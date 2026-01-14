using ILNumerics.Drawing.Plotting;
using ILNumerics.Drawing; 
using ILNumerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ILNumerics.IO.HDF5;
using System.IO;

namespace HDF5_PackageData_FillDataset_Iteratively {
    
    /// <summary>
    /// This example demonstrate how to sequentially write to H5Dataset. An imagesc plot is drawn, filling the 
    /// whole form area. The user will drag selection rectangles on the image data. Every time the dragging 
    /// is ended by releasing the left mouse button, start and end positions of the mouse are appended to 
    /// an HDF5 file. The common selection rectangle is reused for the dragging operations. However, it is 
    /// reconfigured so it will not trigger a zoom event and the scene stays static.
    /// </summary>
    /// <remarks>Concepts explained: Creating HDF5 files, creating HDF5 datasets, 
    /// Locating datasets within H5File, determining the chunk size of datasets, determinining 
    /// the size of datasets, writing to datasets as partial writes, expanding datasets by appending 
    /// to an existing dataset content, creating imagesc plots, controlling the data screen size for 
    /// rendering plot cube content, deactivating certain standard mouse event handlers for plot cubes,
    /// handling mouse events by use of custom mouse event handlers.
    /// </remarks>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // we store the mouse down location here:
        System.Drawing.Point? m_startPoint; 
        // some constants we are using within the example
        public static readonly string FILENAME = "clicks.h5";
        public static readonly string DSNAME = "myDataset"; 

        /// <summary>
        /// Main setup method gets called when the panel is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ilPanel1_Load(object sender, EventArgs e) {
            // the data to show comes from a predefined example dataset contained in ILNumerics
            Array<float> A = ILMath.tosingle(SpecialData.terrain["0:140;0:240"]);
            // we fetch to min and max values for tight limits
            float min, max;
            A.GetLimits(out min, out max);
            // scene setup: add a plotcube in 2D mode...
            var pc = ilPanel1.Scene.Add(new PlotCube(twoDMode: true) {
                // add an imagesc plot to the plot cube
                Children = {
                    new ImageSCPlot(A)
                },
                // configure the datascreen rect. This makes the plotcube 
                // content fill the whole panel area
                DataScreenRect = new RectangleF(0,0,1,1),
                // configure the plot cube limits to show no margins
                Limits = {
                    XMin = 0, YMin = 0, ZMin = min,
                    XMax = A.S[1] - 1, YMax = A.S[0] - 1, ZMax = max + 1
                }
            }); 

            // We want the user to be able to drag a selection rectangle over the 
            // image plot. When she releases the mouse an event gets fired and the 
            // data are written to the file. But we want the scene to stay the way it is. 
            // Therefore, we deactivate zooming and pan, but retain the selection rectangle.
            pc.MouseWheel += (_s, _a) => { _a.Cancel = true; }; 
            pc.AllowPan = false; 
            // When the user starts dragging we simply keep the mouse down position
            pc.MouseDown += (_s, _a) => {
                m_startPoint = _a.Location;
            }; 
            // all logic goes into the mouse up event handler
            pc.MouseUp += pc_MouseUp;
            
            // make sure there will be no filename conflict (CAUTION! THIS WILL DELETE THE FILE FROM DISK!)
            if (File.Exists(FILENAME)) {
                File.Delete(FILENAME); 
            }
        }

        void pc_MouseUp(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            using (Scope.Enter()) {
                // prepare the data to store into the h5file. 
                // The shape is arbitrary. Here we store records 
                // of vector shape [4 x 1]. You might as well append 
                // matrices or n-dimensional arrays along arbitrary 
                // dimensions.
                Array<float> data = ILMath.zeros<float>(4,1);
                data["0;0"] = m_startPoint.Value.X;
                data["1;0"] = m_startPoint.Value.Y;
                data["2;0"] = e.Location.X;
                data["3;0"] = e.Location.Y; 

                // write data into hdf5 file. Open the file:
                using (var file = new H5File(FILENAME)) {
                    // locate the dataset: 
                    var ds = file.First<H5Dataset>(DSNAME);
                    // dataset found? 
                    if (ds != null) {
                        // append to end of dataset
                        ds.Set(data, ":", ds.Size[1]);
                    } else {
                        // this is the first time we write to the dataset. 
                        // The size of the array 'data' will determine the 
                        // chunksize of the dataset! This is a persistant setting
                        // for the whole lifetime of the dataset. Keep this in 
                        // mind when selecting the 'first time data' to give to 
                        // the constructor of H5Dataset.
                        file.Add(new H5Dataset(DSNAME, data)); 
                    }
                    // check the new dataset size by displaying the size as the forms title
                    Text = "Dataset Size: " + file.First<H5Dataset>(DSNAME).Size.ToString();
                }
                
                // We want to prevent other default mouse handlers to react on the same event. 
                // Failing to do so would trigger the default zoom action in the plot cube. But 
                // we want the scene to stay the way it is. 
                e.Cancel = true;
                // reset the mouse down location cache.
                m_startPoint = null; 

            }
        }
    }
}

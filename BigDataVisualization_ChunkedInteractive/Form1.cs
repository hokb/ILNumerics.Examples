using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using ILNumerics.IO.HDF5;
using System;
using System.Linq;
using System.Windows.Forms;
using static ILNumerics.Globals;
using static ILNumerics.ILMath;
using h5elementType = System.Single;

namespace BigDataVisualization_ChunkedInteractive {

    /// <summary>
    /// A big data file in HDF5 format is visualized in chunks.
    /// </summary> 
    /// <remarks>The data in the file are way too big to load them into memory at once. 
    /// This example reads equally spaced chunks from the HDF5 file and displays them in a regular plot cube. 
    /// The view can be panned in X direction by dragging with the right mouse button. Three buttons are provided
    /// to jump to the next (right) chunk, the previous chunk or to a random section in the file. 
    /// Mouse interaction is restricted to the X-direction. All other mouse events on the scene are disabled. 
    /// The example generates random sample data at the first start. Data are automatically recreated when the user 
    /// changes one of the relevant parameters. The default data size is ~1M x 512. 
    /// FastSurface is used to visualize the data mesh. Colormapped rendering on the surface and the data limits are configured 
    /// only at startup. Subsequent render frames do only update those parameters/data which have really changed. 
    /// The surface updates it s Z values only. In order to place the surface over the visible section of the data space 
    /// the surface is translated in the plotcube by help of an Group object. This saves one from having to update 
    /// X axis in each frame. 
    /// </remarks> 
    public partial class Form1 : Form {

        private readonly string plotcubetag = "plotcube";
        private readonly string surfacetag = "surface";
        private readonly string windowgrouptag = "windowgroup";  
        private readonly string datasettag = "data";
        private readonly string filename = "testdata.h5";
        private int curstart = 0;
        private int curend = 100;
        private int currows = 0;
        private int curcols = 0; 
        private static bool updating;
        private int? mouseDownX = null; 

        public Form1() {
            InitializeComponent();
            MessageBox.Show(@"This example creates a big HDF5 file with random data, first. 
This may take some seconds. Please be patient! 
In the viewer chunks of 512 data rows will be shown. The selected 
region is dragged with the right mouse button to show earlier / later 
parts in the file in realtime. This way even largest data are handled efficiently."); 
        }

        /// <summary>
        /// Easy access to the surface object in our scene.
        /// </summary>
        private FastSurface Surface {
            get {
                return ilPanel1.SceneSyncRoot.First<FastSurface>(surfacetag);
            }
        }
        private Group WindowGroup {
            get {
                return ilPanel1.SceneSyncRoot.First<Group>(windowgrouptag);
            }
        }
        private PlotCube PlotCube {
            get {
                return ilPanel1.SceneSyncRoot.First<PlotCube>(plotcubetag);
            }
        }
        private void ilPanel1_Load(object sender, EventArgs e) {

            #region generate data file with sample data
            // following properties can be modified
            int filesizeMB = 2100; // ~2 GB
            int chunksizeMB = 10;
            currows = 512;

            int colsperchunk;
            // delete the file to force recreation of the data!
            using (var f = new H5File(filename)) {
                // approximate sizes here only...
                colsperchunk = (chunksizeMB * 1024 * 1024) / (sizeof(h5elementType) * currows);
                int chunksperfile = filesizeMB / chunksizeMB;
                curcols = chunksperfile * colsperchunk;

                H5Dataset ds = f.First<H5Dataset>(datasettag);
                if (ds == null
                    || ds.Attributes[nameof(h5elementType)].Get<string>().GetValue(0) != typeof(h5elementType).Name
                    || !ds.Size.Equals(size(currows, curcols))) {
                    // need to create the file 
                    if (ds != null) {
                        // old file but unmatching parameters
                        f.Remove(ds);
                    }

                    // generate data file 
                    Text = "Generating data. Please wait...";
                    Show();

                    // create the dataset in the file. This call will determine the element type 
                    // and the chunk size of the dataset! Here, since we present a Double array
                    // the dataset will store all elements in Double precision. Later (below) 
                    // we give single elements. But HDF5 will store them as Double anyway. 
                    // (this is why the file is actually twice as big as selected)
                    f.Add(ds = new H5Dataset(datasettag, rand(currows, colsperchunk)) {
                        Attributes = {
                            { nameof(chunksperfile), chunksperfile },
                            { nameof(currows), currows },
                            { nameof(colsperchunk), colsperchunk },
                            { nameof(h5elementType), typeof(h5elementType).Name }
                        }
                    });
                    // now iterate over each chunk in the file and fill with data. 
                    for (int i = 1; i < chunksperfile; i++) {
                        using (Scope.Enter())
                        {
                            // We start with the 2nd chunk (index 1). While the first / initial  
                            // chunk was pure random data only, we mark the other chunks 
                            // with vertical lines, simulating real-world data. 
                            Array<h5elementType> A = tosingle(rand(currows, colsperchunk));
                            A[":", A["0;:"] > 0.8f] = 1;  // 0.9f -> fewer lines 
                            // the chunk is written to the file. The same syntax is used as for Array<T> 
                            // subarray expressions. The dataset is automatically expanded.
                            ds.Set(A, ":", r(i * colsperchunk, (i + 1) * colsperchunk - 1));
                        }
                    }
                }
            }
            #endregion data generation

            #region MyRegion
            Text = $"Data: {filesizeMB}MB, {currows} x {curcols}";
            ClientSize = new System.Drawing.Size(ClientSize.Width, currows);

            // setup scene
            ilPanel1.Scene.Add(new PlotCube(tag: plotcubetag) {
                new Group(windowgrouptag) {
                    new FastSurface(surfacetag)
                }
            });

            // some one-time configuarations on the surface: 
            var surf = ilPanel1.Scene.First<FastSurface>();
            using (var f = new H5File(filename))
            {
                curend = ClientSize.Width;
                curstart = 0;
                Array<h5elementType> A = f.First<H5Dataset>(datasettag).Get<h5elementType>(full, r(curstart, curend));
                surf.Update(
                    Z: A,
                    minmaxColorRange: new Tuple<h5elementType, h5elementType>(0, 1),
                    colormap: Colormaps.Jet);
            }
            surf.Configure();
            ilPanel1.Scene.First<PlotCube>().Reset(); 

            #endregion

            // disable picking (this actually disables ALL user interaction on Panel)
             ilPanel1.Enabled = false; 

            // we handle the mouse events via the form on our own. (needs more consideration for better precision)
            // Note: the main advantage of this alternative event handling via the form (and not via Panel) is 
            // to prevent the panel from continous picking operations. They may conflict with the moving surface 
            // and lead to hickups in the display. In a production version you would carefully make a plan of 
            // all events needed and decide for each one how / where to realize it. 
            this.MouseDown += (_s, _a) => { mouseDownX = _a.Location.X; };
            this.MouseMove += (_s, _a) =>
            {
                if (_a.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    System.Diagnostics.Debug.Assert(mouseDownX.HasValue);
                    int offsX = mouseDownX.GetValueOrDefault() - _a.Location.X;
                    mouseDownX = _a.Location.X;
                    Show(curstart + offsX, curend + offsX);
                }
            };

        }

        // this function acts on the synchronized scene directly!
        private void Show(int s, int e) {
            if (!updating) {
                updating = true; 
                using (Scope.Enter())
                // load data 
                using (var f = new H5File(filename)) {
                    if (s < 0) {
                        e -= s;
                        s = 0; 
                    } else if (e >= curcols) {
                        s -= (e - curcols);
                        e = curcols - 1; 
                    }
                    // load data from HDF5 file
                    Array<h5elementType> A = f.First<H5Dataset>(datasettag).Get<h5elementType>(full, r(s, e));

                    // new display limits
                    Vector3 min = new Vector3(s, 0, -10);
                    Vector3 max = new Vector3(e, currows, 10); 

                    Surface.Update(Z: A, minLimit: min, maxLimit: max); // providing data and limits helps performance
                    // move the window (Group) to relocate the surface to the correct x position
                    WindowGroup.Transform = Matrix4.Translation(s, 0, 0);
                    PlotCube.Limits.Set(min, max);
                }
                ilPanel1.Refresh();
                Text = $"Showing {e - s} columns: {s}-{e} x {currows}";
                curstart = s;
                curend = e;
                updating = false; 
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // next page
            int offs = curend - curstart;
            Show(curstart + offs, curend + offs);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // previous page
            int offs = curend - curstart;
            Show(curstart - offs, curend - offs);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int offs = curend - curstart;
            int s = new Random().Next(0, curcols - offs); 
            Show(s, s + offs);
        }
    }
}

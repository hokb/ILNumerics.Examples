using ILNumerics;
using ILNumerics.Toolboxes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ILNumerics.Drawing.Plotting;
using ILNumerics.Drawing;
using System.Diagnostics;
using prec = System.Double;   // one out of "Single" or "Double"
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace DownsampleHugePointCloud_1 {

    /// <summary>
    /// A scalable data fitter for large scattered data point clouds. Kriging interpolation is used to fit 
    /// a surface to a given set of scattered (random) data points in 2D. Too many points are provided to 
    /// be handled at once by kriging. Therefore, the set is split into multiple tiles and each tile is handled 
    /// individually.
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        Array<prec> A = localMember<prec>();
        System.Drawing.Size worldLimits = new System.Drawing.Size(1000, 1000);
        int numScatteredSamples = 10000; // try to change this! Set prec = System.Single if you get NaNs in double precision. Consider to target x64. 

        // split along x and y directions to create equally sized tiles. You will 
        // adopt this to your data to create "good" tiles: nearly square sized of reasonable small size.
        int tilesX = 30;
        int tilesY = 30;
        // blending: n grid points
        int blend = 3;

        // the final fitted surface will have that many points: 
        int sampleGridRows = 150;
        int sampleGridCols = 150;

        private void ilPanel1_Load(object sender, EventArgs e) {

            var pb = ilPanel1.Scene.Add(new ILProgressBar(tag: "ProgressBar") {
                Visible = false
            });

            A.a = CreateData(numScatteredSamples, worldLimits);  

            // Set up the scene 
            PlotCube pl = ilPanel1.Scene.Add(new PlotCube(twoDMode: false));

            FastSurface surf = pl.Add(new FastSurface(1.0) {
                Fill = { AutoNormals = true }
            });

            // plot the source sample points, scattered, red
            Points points = pl.Add(new Points() {
                Size = 1,
                Color = Color.Red, 
                Positions = tosingle(A)
            });

            // reset the plot cube to show all points
            pl.Configure(); 
            pl.Reset();

        }

        private void button1_Click(object sender, EventArgs e) {
            Downsample();
        }

        public async void Downsample() {

            // keep current value of configured number of threads. We will change it and have to reset it later. 
            int oldThreads = (int)Settings.MaxNumberThreads;

            try {
                Cursor = Cursors.WaitCursor;
                Stopwatch sw = Stopwatch.StartNew();

                using (Scope.Enter()) {

                    #region Rendering, prepare output shapes for reporting intermediate progress

                    // find the progress bar to report progress of interpolation
                    var progress = ilPanel1.Scene.First<ILProgressBar>("ProgressBar");
                    progress.Value = 0;
                    progress.Visible = true;

                    // in S we maintain the output result 
                    Array<prec> S = zeros<prec>(sampleGridRows, sampleGridCols);
                    var surf = ilPanel1.Scene.First<FastSurface>();
                    surf.Update(
                        X: linspace<float>(0,worldLimits.Width,sampleGridCols),
                        Y: linspace<float>(0,worldLimits.Height,sampleGridRows),
                        Z: tosingle(S),
                        colormap: Colormaps.ILNumerics, 
                        minmaxColorRange: new Tuple<float, float>(0,1));

                    ilPanel1.Refresh(); // show current scene

                    // how to manage our threads? 
                    // We must prevent from parallel threads doing things in parallel. So we either allow mouse events 
                    // but have to make sure that the updates to the scene are done single threaded. Or we disable mouse 
                    // events and allow the processing here to use multiple threads. 

#if ENABLE_INTERACTION
                    // ** disable multithreading but mouse interaction etc. 
                    Settings.MaxNumberThreads = 1; 

#else
                    // ** disable mouse events during processing 
                    EventHandler<ILNumerics.Drawing.MouseEventArgs> mouseOff = (_s, _a) => { _a.Cancel = true; };
                    ilPanel1.Scene.MouseMove += mouseOff;
#endif


#endregion

                    await Task.Run(() => {
                        using (Scope.Enter(S)) {
                            // everything in here is getting executed in a background thread: 


                            // Since here we assume that the number of input samples is too large to 
                            // be used  at once with kriging, we split the input data and do individual 
                            // interpolations based on fewer points. There are two methods implemented: 
                            // splitting geometrically as neighboring regions and splitting statistically by randomly selected points.

                            if (true) {
#region splitting regions by tiles (default) 
                                // 
                                // The whole sample area is devided in equally sized tiles. Each tile covers all samples 
                                // in its inner area and interpolates the meshgrid for the corresponding inner grid points. 
                                // Care must be taken for the transition between individual tiles. A cross over region is defined
                                // and used to blend between adjacent tiles. Here, in order to simplify things we do not 
                                // actually 'blend' the tiles but we compute the new grid points of a tile based on its inner 
                                // points _plus_ some outer points around the tile up to the distance as determined by "blend". 
                                // Those outer points help to make rel. smooth crossings between the tiles by considering neighboring 
                                // points for interpolation also. 

                                // find limits of the data in X/Y plane
                                prec minx, maxx, miny, maxy;
                                Array<prec> Ax = A[0,full], Ay = A[1,full];
                                Ax.GetLimits(out minx, out maxx);
                                Ay.GetLimits(out miny, out maxy);

                                SizeF gridSize = new SizeF((float)worldLimits.Width / (sampleGridCols - 1), (float)worldLimits.Height / (sampleGridRows - 1));
                                int gridPointsPerTileX = sampleGridCols / tilesX;
                                int gridPointsPerTileY = sampleGridRows / tilesY;
                                SizeF tileSize = new SizeF(gridSize.Width * gridPointsPerTileX, gridSize.Height * gridPointsPerTileY);
                                SizeF blendSize = new SizeF(gridSize.Width * blend, gridSize.Height * blend);

                                // adjust tile count to fill the world area
                                tilesX = (int)(worldLimits.Width / tileSize.Width) + 1; 
                                tilesY = (int)(worldLimits.Height / tileSize.Height) + 1;

                                if (surf != null) {
                                    surf.Update(Z: tosingle(S));
                                    //surf.Fill.AutoComputeNormals(); 
                                    surf.Configure();
                                }

                                // template grid for all tiles
                                Array<prec> Y = 0, X = meshgrid(
                                    //linspace<prec>(0, tileWidth - worldLimits.Width / sampleGridCols, gridPointsPerTileX),
                                    //linspace<prec>(0, tileHeight - worldLimits.Height / sampleGridRows, gridPointsPerTileY), Y);
                                    linspace<prec>(0, tileSize.Width, gridPointsPerTileX),
                                    linspace<prec>(0, tileSize.Height, gridPointsPerTileY), Y);

                                Array < prec> tempGridXY = empty<prec>(2, gridPointsPerTileY * gridPointsPerTileX);
                                tempGridXY[0, full] = X;
                                tempGridXY[1, full] = Y;

                                // iterate over tiles 
                                for (int xi = 0; xi < tilesX; xi++) {
                                    for (int yi = 0; yi < tilesY; yi++) {

                                        using (Scope.Enter()) {

                                            // create the coords limits of the current tile (this must be adopted for sample points in other areas than [0...1])
                                            // We use the template tile grid, adding an offset for the current tile. 
                                            int thisTileCountX =(int) Math.Min(S.S[1] - gridPointsPerTileX * xi, gridPointsPerTileX); 
                                            int thisTileCountY =(int) Math.Min(S.S[0] - gridPointsPerTileY * yi, gridPointsPerTileY);
                                            Array<prec> tileGrid = tempGridXY
                                                // 'move' the current tile to its correct position
                                                + vector((prec)tileSize.Width * xi, (prec)tileSize.Height * yi);

                                            // at the edges we may have to cut some grid points
                                            tempGridXY[or(tempGridXY[0,full] > worldLimits.Height + epsf, tempGridXY[1,full] > worldLimits.Width + epsf)] = null; 

                                            // select all points from A which lay inside the tile, include neighboring region 'blend'
                                            Array<long> ind = find(and(
                                                and(Ax >= xi * tileSize.Width - blendSize.Width, Ax <= (xi + 1) * tileSize.Width + blendSize.Width),
                                                and(Ay >= yi * tileSize.Height - blendSize.Height, Ay <= (yi + 1) * tileSize.Height + blendSize.Height)));

                                            if (ind.IsEmpty || ind.Length < 2) {
                                                continue; // ?? not enough given points in reach..
                                            }

                                            // interpolate via kriging
                                            Array<prec> tmp = Interpolation.kriging(A[2, ind], A["0,1", ind], tileGrid);

                                            // fit into S
                                            System.Diagnostics.Debug.Assert((yi + 1) * gridPointsPerTileY - 1 < S.S[0]); 
                                            System.Diagnostics.Debug.Assert((xi + 1) * gridPointsPerTileX - 1 < S.S[1]);
                                            S[r(yi * gridPointsPerTileY, yi * gridPointsPerTileY + thisTileCountY - 1),
                                                r(xi * gridPointsPerTileX, xi * gridPointsPerTileX + thisTileCountX - 1)] = tmp;

                                            // draw the current interpolation result S
                                            if (surf != null) {
                                                surf.Update(Z: tosingle(S));
                                                //surf.Fill.AutoComputeNormals(); 
                                                surf.Configure();
                                            }

                                            // update the progress bar's value and present it to the user
                                            progress.Value = (int)(((prec)(xi * tilesY + yi) / (tilesY * tilesX)) * 100);
                                            // Control.Refresh must be called from the main UI thread!
                                            ilPanel1.BeginInvoke((MethodInvoker)(() => ilPanel1.Refresh()));

                                        }
                                    }
                                }
#endregion
                            } else {
#region splitting randomly (obsolete)
                                // Perform each interpolation as if fewer points had been sampled 
                                // from the start. The whole final grid area is created from these points. 
                                // Individual interpolations are than merged (averaged) at the end. 
                                // This implementation is left here for documentation purposes only. It can produce 
                                // quite large errors and is not recommended for production use! 

                                //// output grid point coordinates
                                //ILArray<prec> X = linspace<prec>(0, 1, sampleGridCols);
                                //ILArray<prec> Y = linspace<prec>(0, 1, sampleGridRows);

                                //// grid points for the output grid
                                //ILArray<prec> YYn = 0, XXn = meshgrid(X, Y, YYn);

                                //// bring meshgrid into right shape for interpolation
                                //ILArray<prec> Xn = zeros<prec>(2, sampleGridCols * sampleGridRows);
                                //Xn["0;:"] = XXn[":"];
                                //Xn["1;:"] = YYn[":"];

                                //// splitting the input data by chunks of randomly selected points: 
                                //ILArray<int> ind = Statistics.randperm(num);
                                //// ind contains ALL point's indices in random order. We will use them in 
                                //// subsequent chunks below

                                //int iterCount = 0;
                                //for (int i = 0; i < num;) {
                                //    using (ILScope.Enter()) {

                                //        // 
                                //        if (i + chunkSize > num) {
                                //            chunkSize = num - i;
                                //        }
                                //        // pick the points for this iteration
                                //        ILArray<int> curInd = ind[r(i, i + chunkSize - 1)];
                                //        ILArray<prec> pos = A["0,1", curInd];
                                //        ILArray<prec> data = A[2, curInd];

                                //        ILArray<prec> interp = Interpolation.kriging(data, pos, Xn);

                                //        // accumulate output results
                                //        S.a = S + interp;
                                //        iterCount++;
                                //        i += chunkSize;

                                //        // update the progress bar's value and present it to the user
                                //        progress.Value = (int)(i / (prec)num * 100);
                                //        // Control.Refresh must be called from the main UI thread!
                                //        ilPanel1.BeginInvoke((MethodInvoker)(() => ilPanel1.Refresh()));
                                //    }
                                //}

                                //// average and bring S into rectangular grid format
                                //S = reshape(S / iterCount, sampleGridRows, sampleGridCols);
                                //S[":;:;2"] = YYn; S[":;:;1"] = XXn;
#endregion
                            }

#region post processing: hide the progress bar

                            progress.Visible = false;

#endregion
                        }
                    });

                    // this is getting executed in the main UI thread: 
                    if (surf != null) {
                        surf.Update(Z: tosingle(S));
                    }

#if !ENABLE_INTERACTION
                    ilPanel1.Scene.MouseMove -= mouseOff;
#endif 
                    ilPanel1.Configure();
                    ilPanel1.Refresh();
                    Text = $"Down-sampled in {sw.ElapsedMilliseconds} ms.";

                }
            } finally {
                Cursor = Cursors.Arrow;
                Settings.MaxNumberThreads = (uint)oldThreads; 
            }
        }

        private static RetArray<prec> CreateData(int num, System.Drawing.Size worldSize) {
                Array<prec> ret = 0; 
#if RESTORE_DATA
            using (ILScope.Enter()) {
                // for debugging purposes only: restore A's values from last session
                if (!System.IO.File.Exists("lastA.m")) {
#endif
                    ret.a = convert<Double, prec>(rand(3, num));
                    // the heigth function (here: a function. In real world this is often a set of measured height values)
                    ret[2,full] = sin(ret[0,full] * ret[0,full] + ret[1,full] * ret[1,full]);  // sin(x*x + y*y)
                    ret[0,full] = ret[0,full] * worldSize.Width; // stretch the data along the X plane
                    ret[1,full] = ret[1,full] * worldSize.Height; // stretch the data along the Y plane
#if RESTORE_DATA
            // store for next run (eases debugging ... ;) ) 
            using (var f = new ILMatFile()) {
                        f["A"] = ret;
                        f.Write("lastA.m");
                    }
                } else {
                    try {
                        ret.a = loadArray<prec>("lastA.m", "A");
                    } catch (ILNumerics.Exceptions.ILArgumentException) {
                        System.IO.File.Delete("lastA.m");
                        ret.a = convert<Double, prec>(rand(3, num));
                        // the heigth function (here: a function. In real world this is often a set of measured height values)
                        ret["2;:"] = sin(ret["0;:"] * ret["0;:"] + ret["1;:"] * ret["1;:"]);  // sin(x*x + y*y)
                        ret["0;:"] = ret["0;:"] * worldSize.Width; // stretch the data along the X plane
                        ret["1;:"] = ret["1;:"] * worldSize.Height; // stretch the data along the Y plane

                        using (var f = new ILMatFile()) {
                            f["A"] = ret;
                            f.Write("lastA.m");
                        }
                    }
                }
#endif
                return ret; 
        }

    }
}

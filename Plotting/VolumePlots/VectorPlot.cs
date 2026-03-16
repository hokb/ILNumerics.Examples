using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace VolumePlots {
    public class VectorPlot : Group {

        public static readonly string GridLinesTag = "GridLines"; 
        public static readonly string ArrowLinesTag = "ArrowLines";

        public LineStrip GridLines {
            get { return First<LineStrip>(GridLinesTag); }
        }

        public Lines ArrowLines {
            get { return First<Lines>(ArrowLinesTag); }
        }

        public VectorPlot() {
            // setup
            Add(new LineStrip(GridLinesTag)); 
            Add(new Lines(ArrowLinesTag));

            GridLines.Color = Color.LightGray; 

        }

        /// <summary>
        /// plot vector field on a gridded area
        /// </summary>
        /// <param name="X">X coordinates of base grid, [m x n] matrix</param>
        /// <param name="Y">Y coordinates of base grid, [m x n] matrix</param>
        /// <param name="Z">Z coordinates of base grid, [m x n] matrix</param>
        /// <param name="VX">X coordinates of vector data, [m x n] matrix</param>
        /// <param name="VY">Y coordinates of vector data, [m x n] matrix</param>
        /// <param name="VZ">Z coordinates of vector data, [m x n] matrix</param>
        public void Update(InArray<float> X, InArray<float> Y, InArray<float> Z, InArray<float> VX, InArray<float> VY, InArray<float> VZ, Tuple<float,float> minmaxRange, Colormap cm) {
            using (Scope.Enter(X, Y, Z, VX, VY))
            using (Scope.Enter(VZ, ArrayStyles.ILNumericsV4)) {
                if (X.S != Y.S || Y.S != Z.S || Z.S != VX.S || VY.S != VZ.S || !X.IsMatrix) {
                    throw new ArgumentException("All input arrays must be of the same size: m x n matrices");
                }
                // Update the vertex data
                int m = (int)X.S[0];
                int n = (int)X.S[1];
                int stop = m * n * 2;

                Array<float> gdata = zeros<float>(3, m * n * 2 + 1);
                // first half of the buffer is filled with grid points
                gdata[0, r(0, m * n - 1)] = X[full];
                gdata[1, r(0, m * n - 1)] = Y[full];
                gdata[2, r(0, m * n - 1)] = Z[full];
                // 2nd half of the buffer is filled with arrow aims
                gdata[0, r(m * n, end - 1)] = gdata[0, r(0, m * n - 1)] + VX[full].T;
                gdata[1, r(m * n, end - 1)] = gdata[1, r(0, m * n - 1)] + VY[full].T;
                gdata[2, r(m * n, end - 1)] = gdata[2, r(0, m * n - 1)] + VZ[full].T;
                
                // add a stop marker (for interrupting line strips) 
                gdata[full, end] = float.NaN; 

                ArrowLines.Positions.Update(gdata);
                // both plots share their positions buffer
                GridLines.Positions = ArrowLines.Positions;

                // prepare grid: indices
                Array<int> gindH = array<int>(stop, size(m + 1, n));
                Array<int> gindV = array<int>(stop, size(n + 1, m));
                Array<int> count = counter<int>(0, 1, size(m, n)); 
                gindH[r(0, m - 1), r(0, n - 1)] = count;
                gindV[r(0, n - 1), r(0, m - 1)] = count.T;

                GridLines.Indices.Update(0, (int)gindH.S.NumberOfElements, gindH);
                GridLines.Indices.Update((int)gindH.S.NumberOfElements, (int)gindV.S.NumberOfElements, gindV);

                // prepare arrow lines
                count.a = counter<int>(0, 1, size(1, m * n));
                count[1, full] = count + (m * n);
                ArrowLines.Indices.Update(count);

                Array<float> colors = zeros<float>(4, m * n * 2) + vector<float>(.8f,.8f,.8f,1);
                // @grid point: black, tip: colormapped length
                Array<float> lengths = sqrt(VX * VX + VY * VY + VZ * VZ);
                colors[full, r(m * n, end)] = cm.Map(lengths, minMaxRange: minmaxRange).T; 
                ArrowLines.Colors.Update(colors);
                ArrowLines.Color = null;
                ArrowLines.Width = 3; 
                Configure(); 
            }
        }



    }
}

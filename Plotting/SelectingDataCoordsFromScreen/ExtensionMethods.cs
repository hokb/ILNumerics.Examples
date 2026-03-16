using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectingDataCoordsFromScreen {
    public static class ExtensionMethods {

        /// <summary>
        /// Translates 2D <paramref name="screen"/> coordinates into coordinates of the data space of the <paramref name="targetNode"/>.
        /// </summary>
        /// <param name="screen">2D coordinates in range [0...1], relative to the current viewport. Often used: mouse coordinates as provided by <see cref="MouseEventArgs.LocationF"/>.</param>
        /// <param name="target">The plot object providing the data space to transform <paramref name="screen"/> into.</param>
        /// <returns>The 3D data space coordinates, relative to the specified target system.</returns>
        public static Vector3 ScreenToDataExt(this PlotCube plotcube, PointF screen, Group target) {

            Vector4 clipCoords = new Vector4(
                screen.X * 2 - 1,
                screen.Y * -2 + 1, 0, 1);
            Matrix4 transf = target.Transform;
            while (target.Parent != null && target.Parent != plotcube) {
                target = target.Parent;
                transf = target.Transform * transf;
            }
            transf = plotcube.ProjectionTransform * transf;
            transf = Matrix4.Invert(transf);
            Vector4 dataCoords = transf * clipCoords;
            if (dataCoords.W != 0) {
                dataCoords /= dataCoords.W;
            }
            return dataCoords.Xyz;
        }

    }
}

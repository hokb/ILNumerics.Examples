using ILNumerics;
using ILNumerics.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownsampleHugePointCloud_1 {
    public class ILProgressBar : ScreenObject {

        #region attributes 
        // Tag used to identify the border lines object in the scene graph. 
        public static readonly string BarBorderTag = "ProgressBarGroupBorder";
        // Tag used to identify the inner colored fill (triangles) of the progress bar object in the scene graph. 
        public static readonly string BarFillTag = "ProgressBarGroupFill";
        // Tag used to identify the ILGroup grouping together the progress bar inner fill and its border object in the scene graph. 
        public static readonly string BarGroupTag = "ProgressBarGroup";
        // Tag used to identify the ILGroup containing the progress bar inner fill in the scene graph. 
        public static readonly string BarFillGroupTag = "ProgressBarFillGroup";

        #endregion

        #region properties  

        public Lines BarBorder {
            get {
                return First<Lines>(BarBorderTag);
            }
        }
        public Triangles BarFill {
            get {
                return First<Triangles>(BarFillTag);
            }
        }
        public Group BarGroup {
            get {
                return First<Group>(BarGroupTag);
            }
        }
        public int Value {
            get {
                return (int)(BarFill.Parent.Transform.M11 * 100); 
            }
            set {
                setValue(value); 
            }
        }

        #endregion

        #region constructor

        public ILProgressBar(Positioning position = Positioning.BottomLeft, object tag = null) : 
            base (tag: tag, width: 250f, height: 20f) {

            Array<float> A = new float[,] 
            {
                { 0, 0, 0 },
                { 1, 0, 0 },
                { 1, 1, 0 },
                { 0, 1, 0 }
            };
            Array<int> I = new int[] { 0, 1, 2, 3, 0 }; 

            Add(new Group(BarGroupTag) {
                new Group(BarFillGroupTag) {
                    new TrianglesStrip(BarFillTag)
                }, 
                new LineStrip(BarBorderTag)
            });

            BarBorder.Positions.Update(A.T);
            BarBorder.Indices.Update(I);
            BarBorder.Color = Color.FromArgb(100, Color.Gray); 

            BarFill.Positions = BarBorder.Positions; 
            BarFill.Indices = BarBorder.Indices;
            BarFill.Parent.Scale(0.2f, 1, 1);
            // let's fancy-up the fill by a nice gradient! 
            BarFill.Color = null;
            Array<float> colors = new float[,] {
                //{ 252f, 125, 169 },
                { 252f, 175, 219 },
                { 175f, 90, 118 },
                { 175f, 90, 118 },
                { 252f, 175, 219 },
            }; 
            BarFill.Colors.Update(colors.T / 255); 

            BarGroup.Scale(.95, .6, 1);
            BarGroup.Translate(.025, .2, 0);

            Width = 100;
            WidthUnit = Units.Pixels;

            Height = 10;
            HeightUnit = Units.Pixels;

            base.MinimumSize = new SizeF(250, 30); 
                        
        }

        #endregion

        #region helper functions 

        private void setValue(int val) {

            if (val < 0) {
                val = 0; 
            }
            if (val > 100) {
                val = 100; 
            }

            BarFill.Parent.Transform = Matrix4.ScaleTransform(val / 100f, 1, 1);
            BarFill.Parent.Configure(); 

        }
        #endregion

    }
}

using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Windows.Forms;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace InteractiveColorbar {

    /// <summary>
    /// Example modifying the standard ILColorbar screen object for extended interaction. The data range for the colorbar is 
    /// editable with the mouse. The color data range of the attached surface object is changed with the color bar range and 
    /// presented on the fly. 
    /// <para>This example demonstrates the following aspects: scene setup with surface and colorbar, mouse event registration 
    /// on the colorbar, implementing a custom IILColormapProvider, event handling for dynamically updating the colorbar 
    /// provider data, updating surface objects for dynamic colormapped data ranges.</para>
    /// </summary>
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // Attributes: 
        // initial mouse drag position
        int? m_mouseStartY;
        // factor increasing the step size for changes of upper /lower limits. Higher values: higher steps / pixel mouse movement
        float m_dragFactor = 10;

        readonly Array<float> A = localMember<float>(); 

        private void ilPanel1_Load(object sender, EventArgs e) {
            // our 'data'
            A.a = tosingle(SpecialData.terrain["0:300;0:300"]);

            // set up the scene
            ilPanel1.Scene.Add(new PlotCube(twoDMode: false) {
                new FastSurface() {
                    new Colorbar() {
                        // the colorbar will automatically attach to the surface. 

                        // Deactivate the option to move the colorbar. It would 
                        // conflict with our dragging operations. A more sophisticated 
                        // solution could split the mouse regions in half: left side 
                        // dragging -> data range update; right side dragging -> move colorbar.
                        Movable = false
                    }
                }
            });

            ilPanel1.Scene.First<FastSurface>().Update(Z: A, colormap: Colormaps.Jet);

            // setup / attach mouse events, yes we need them  (almost) all ..
            ilPanel1.Scene.First<Colorbar>().MouseLeave += colorbar_mouseLeave;
            ilPanel1.Scene.First<Colorbar>().MouseDown += colorbar_mouseDown;
            ilPanel1.Scene.First<Colorbar>().MouseUp += colorbar_mouseUp;
            ilPanel1.Scene.First<Colorbar>().MouseMove += colorbar_mouseMove;

            Text = "Interactive Colorbar Example - use colorbar to adjust values";
            ilPanel1.Scene.First<PlotCube>().Reset(); 
        }

        /// <summary>
        /// Updates the cursor icon while hovering over the color bar area.
        /// </summary>
        private void UpdateCursor(ILNumerics.Drawing.MouseEventArgs e, Colorbar cb)
        {
            if (m_mouseStartY.HasValue) return;

            float h = (e.LocationF.Y - cb.Location.Y) / cb.Height.GetValueOrDefault();
            // split the colorbar into 3 parts: 
            if (h < .3)
            {
                // upper part: change the upper limit only
                Cursor = Cursors.PanNorth;
            }
            else if (h < .7)
            {
                // mid part: change upper AND lower limits
                Cursor = Cursors.SizeNS;
            }
            else {
                // lower part: change lower limit only
                Cursor = Cursors.PanSouth;
            }
            // note that we "store" the state of the limit to change in the Cursor here. A production
            // application would implement a more sophisticated solution, mostly.
        }

        #region mouse event handlers

        private void colorbar_mouseLeave(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (sender is Colorbar) {
                Cursor = Cursors.Default;
            }
        }
        private void colorbar_mouseDown(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (sender is Colorbar) {
                m_mouseStartY = e.Location.Y;
            }
        }
        private void colorbar_mouseUp(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (sender is Colorbar) {
                m_mouseStartY = null;
            }
        }
        // the move event does most of the work
        private void colorbar_mouseMove(object sender, ILNumerics.Drawing.MouseEventArgs e) {
            if (sender is Colorbar && e.DirectionUp) {

                var cb = sender as Colorbar;
                
                float cbCenter = (ilPanel1.Width * cb.Location.X) - (cb.Width.GetValueOrDefault() * 0.5f);

                if (e.Location.X > cbCenter)
                {
                    ilPanel1.Scene.First<Colorbar>().Movable = false;
                    // update cursor position 
                    UpdateCursor(e, cb);
                    // are we in a dragging operation? 
                    if (m_mouseStartY.HasValue)
                    {
                        // compute offset data 
                        var diff = -(m_mouseStartY.GetValueOrDefault() - e.Location.Y) * m_dragFactor;
                        m_mouseStartY = e.Location.Y;

                        updatePlot(
                                (Cursor == Cursors.PanSouth || Cursor == Cursors.SizeNS) ? diff : 0,
                                (Cursor == Cursors.PanNorth || Cursor == Cursors.SizeNS) ? diff : 0);

                        // do not process further events
                        e.Cancel = true;
                        // trigger a redraw of the scene
                        e.Refresh = true;
                    }
                }
                else
                {
                    Cursor = Cursors.SizeAll;
                    ilPanel1.Scene.First<Colorbar>().Movable = true;
                }               
            }
        }
        #endregion

        /// <summary>
        /// Update the plot and colorbar with new range data
        /// </summary>
        /// <param name="minOffs">range offset, to be added to the existing lower range limit</param>
        /// <param name="maxOffs">range offset, to be added to the existing upper range limit</param>
        void updatePlot(float minOffs, float maxOffs) {
            // update the colorbar 
            var surface = ilPanel1.SceneSyncRoot.First<FastSurface>();
            if (surface != null) {

                float min = surface.GetRangeMinValue(AxisNames.CAxis) + minOffs;
                float max = surface.GetRangeMaxValue(AxisNames.CAxis) + maxOffs;

                // update the surface plot 
                surface.Update(minmaxColorRange: Tuple.Create(min, max));

            }
        }
    }
}

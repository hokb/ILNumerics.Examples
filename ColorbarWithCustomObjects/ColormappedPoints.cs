using ILNumerics; 
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorbarWithCustomObjects {
    /// <summary>
    /// This class creates a custom drawable object (derived from Group) which will hold some points for rendering. 
    /// It is special, because the object (type) will be populated to the synchronized tree. Instead of the 
    /// common base types (Group), this class adds all overloads which are necessary in order to find the 
    /// actual type (ColormappedPoints) in the synchronized render tree at runtime. 
    /// The type than provides all data to the colorbar. This is for demonstration purpose only! Use the 
    /// Colorbar.ColormapProvider property to assign a (much simpler) static colormap provider object! 
    /// </summary>
    /// <seealso cref="ILNumerics.Drawing.Plotting.StaticColormapProvider"/>
    class ColormappedPoints : ILNumerics.Drawing.Group, ILNumerics.Drawing.Plotting.IColormapProvider {

        #region attributes
        // store the tags in const attributes for easier reference
        private static readonly string DefaultTag = "ColormappedGroup";
        private static readonly string DefaultPointsTag = "Points"; 
        // the only attribute stores the current colormap
        Colormap m_colormap = new Colormap(Colormaps.Jet);
        #endregion

        #region general properties
        /// <summary>
        ///  Gets a reference to the points shape.
        /// </summary>
        public Points Points {
            get {
                // always store the shapes in the group only! Do not use local attributes or even anonymous groups! Those attributes 
                // would not show up in the synchronized scene unless you implement precautions to do so (in Synchronize() method). 
                return First<Points>(DefaultPointsTag);
            }
        }
        #endregion

        /// <summary>
        /// Setup a new ColormappedPoints object.
        /// </summary>
        /// <param name="tag">[Optional] Tag used to identify the object in the scene.</param>
        public ColormappedPoints(InArray<float> points, object tag = null, int nrPoints = 1000, Colormap colormap = null)
            : base(tag ?? DefaultTag) {
                using (Scope.Enter(points)) {
                    m_colormap = colormap ?? new Colormap(Colormaps.Hot);
                    // create the points
                    Add(new Points(DefaultPointsTag) {
                        Positions = points,
                        // since we want to show the colorbar, we need to put the points colors under colormap control
                        // The Y coordinates are used as data values which are mapped to colors. 
                        Colors = m_colormap.Map(points["1;:"]).T,
                        // deactivate single color rendering (it would take preceedence otherwise)
                        Color = null
                    });
                }
        }
 
        #region overrides needed for populating the class to synched tree
        /// <summary>
        /// This constructor is only called in order to create a new node in the synched scene
        /// </summary>
        private ColormappedPoints() { }
        /// <summary>
        /// copy constructor, used by Copy()
        /// </summary>
        /// <param name="source">the other colormappedpoints instance to copy from</param>
        protected ColormappedPoints(ColormappedPoints source) 
            : base(source) {
            m_colormap = source.Colormap; 
        }
        // this ensures that copies of the object (done by re-adding the object to a scene) are created with the actual type (instead of the base type). 
        public override Node Copy() {
            return new ColormappedPoints(this);
        }
        // this method is needed to synchronize the attributes which are introduced by this type between global and synched scene.
        public override Node Synchronize(Node copy, SyncParams syncParams) {
            // use this scheme for own implementations: 
            // base.Synchronize creates / synchronizes all base attributes 
            var ret = (ColormappedPoints)base.Synchronize(copy, syncParams);
            if (copy == null || Version != ret.SynchedVersion) {
                // first time called after the synched scene has been established or after changes: (re-)synchronize all attributes
                if (m_colormap != null)
                    ret.m_colormap = m_colormap.Synchronize(ret.m_colormap);
            }
            return ret;
        }
        // this method ensures the actual type (ColormappedPoints) to show up in the synched tree
        protected override Node CreateSynchedCopy(Node source) {
            return new ColormappedPoints();
        }
        #endregion

        // in order to support colorbars, just add this interface. For this example, having the ability to 
        // add this interface and therefore colorbar support at runtime is the only reason why we need 
        // to implement CreateSynchedCopy, Synchronize, Copy and the copy constructor. This makes sure 
        // that the actual type ends up in the sychronized scene used for rendering. Otherwise, just the 
        // base class would exist in the synchronized scene (Group). But this does not implement IColormapProvider, hence no colorbar support ... 
        #region ColormapProvider interface implementation
        /// <summary>
        /// The colormap used to map values to colors
        /// </summary>
        public Colormap Colormap {
            get {
                return m_colormap; 
            }
            set {
                // The set accessor is added only for convenience. It is technically not needed. 
                if (value != m_colormap && value != null) {
                    Colormap.SetData(value.Data);
                    OnPropertyChanged("Colormap");  // signal property changed on changes! Ommiting this will fail to populate changes to the synched scene.
                }
            }
        }

        /// <summary>
        /// Determine, if the object currently uses the colormap for displaying colors
        /// </summary>
        public bool IsColormapped {
            get { return true; }
        }

        /// <summary>
        /// Lower limit for the colorbar
        /// </summary>
        /// <param name="AxisName"></param>
        /// <returns></returns>
        public float GetRangeMinValue(ILNumerics.Drawing.AxisNames AxisName) {
            return 0; 
            // in a production environment you would want to replace this with something like that: 
            float val, dummy;
            Points.Positions.Storage["1;:"].GetLimits(out val, out dummy); 
        }

        /// <summary>
        /// Upper limit for the colorbar
        /// </summary>
        /// <param name="AxisName"></param>
        /// <returns></returns>
        public float GetRangeMaxValue(ILNumerics.Drawing.AxisNames AxisName) {
            return 1;
            // in a production environment you would want to replace this with something like that: 
            float val, dummy;
            Points.Positions.Storage["1;:"].GetLimits(out dummy, out val);
        }

        /// <summary>
        /// Axis scaling for the colorbar (not used) 
        /// </summary>
        /// <param name="AxisName">A colorbar will call this method and provide CAxis here</param>
        /// <returns>Scaling for the axis: linear</returns>
        public ILNumerics.Drawing.AxisScale ScaleMode(ILNumerics.Drawing.AxisNames AxisName) {
            return AxisScale.Linear; 
        }
        #endregion


    }
}

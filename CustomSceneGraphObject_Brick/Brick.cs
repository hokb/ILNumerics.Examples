using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ILNumerics.ILMath;
using static ILNumerics.Globals;

namespace ILNumerics {
    /// <summary>
    /// A custom scene graph implementation example. The Brick class implements a scene graph object which combines 
    /// several easier objects (lines, triangles, labels) and allows reusing with individual properties and a convenient API. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// A single Brick creates a cube consisting out of 6 faces and 12 edge lines. The faces are assembled out of an ILTriangles shape, the lines are made of ILLines. 
    /// Custom objects allow a user to wrap rather complicated objects into an individual class for easier reusing. Those classes usually derive from ILGroup and are the root 
    /// of a subtree with arbitrary scene graph objects and individual configurations. This example demonstrates which methods need to be implemented and overidden in 
    /// order to give the class certain capabilities. For details for the creation of custom scene graph objects see: http://ilnumerics.net/custom-scene-graph-objects.html
    /// </para>
    /// <para>The topics handled here in the ILNumerics online documentation: http://ilnumerics.net/advanced-custom-scene-graph-objects.html </para></remarks>
    public class Brick : Group {

        #region attributes
        // tags used to identify this node and its children within the scene graph
        private static readonly string DefaultTag = "Brick";
        private static readonly string DefaultBorderTag = "BrickBorder";
        private static readonly string DefaultFacesTag = "BrickFaces";
        private static readonly string DefaultLabelTag = "BrickLabel";
        // the colormap used for coloring the bricks
        private static readonly Colormap Colormap = Colormaps.ILNumerics;

        // the only instance attribute: backing the Value property
        #endregion

        #region properties
        private double m_value;
        // Gets the value represented by this brick or sets it
        public double Value {
            get { return m_value; }
            set {
                if (m_value != value) {
                    m_value = value;
                    Fill.Color = Colormap.Map((float)value).ToColor();
                    Label.Text = value.ToString("F2"); 
                    // Do not forget to publish the change! This will allow users to catch modifications to the brick
                    // and ensure that the change is getting synchronized with the synched copy used for rendering. 
                    OnPropertyChanged("Value"); 
                }
            }
        }

        /// <summary>
        /// Gets a reference to the ILLines shape representing the edges of the brick
        /// </summary>
        public Lines Border {
            get {
                return First<Lines>(DefaultBorderTag);
            }
        }
        /// <summary>
        /// Gets a reference to the ILTriangles shape representing the faces of the brick
        /// </summary>
        public Triangles Fill {
            get {
                return First<Triangles>(DefaultFacesTag);
            }
        }

        /// <summary>
        /// Gets a reference to the label of the tool tip object
        /// </summary>
        public Label Label {
            get {
                return First<Label>(DefaultLabelTag);
            }
        }
        #endregion

        #region constructor
        /// <summary>
        /// This constructor is needed by CreateSynchedCopy only (see below)
        /// </summary>
        private Brick() { }

        /// <summary>
        /// Copy constructor, used for Copy() operations
        /// </summary>
        /// <param name="source">The source for the copy</param>
        protected Brick(Brick source)
            : base(source) {
            // if the class maintains any private attributes - this is 
            // the place to copy them: 
            m_value = source.m_value;
            // myAttrib1 = source.myAttrib; 
            // ...
        }

        /// <summary>
        /// Public constructor API. Creates new brick shape, optionally define colors
        /// </summary>
        /// <param name="borderColor">Edges Color</param>
        public Brick(object tag = null, Color? borderColor = null)
            : base(tag ?? DefaultTag) {
            // add the faces (triangles) and edges shapes (lines). Use our default tag strings in order to 
            // identify the children easily and find them later again. 
            // In order to add the shapes to our Brick class, we simply utilize 
            // the ILGroup.Add method of the base class. 
            Add(new Triangles(tag: DefaultFacesTag));
            Add(new Lines(tag: DefaultBorderTag));
            Add(new Group(target: RenderTarget.Screen2DFar) { new Label(tag: DefaultLabelTag) });

            // Borders and faces use the same vertex position data. We share them here. 
            Fill.Positions = Border.Positions = PositionsBuffer.UnitCubeLighting;

            // Define the index buffers, picking the right vertices for the faces and the border lines 
            Fill.Indices = IndicesBuffer.UnitCubeLighting.Storage;
            Border.Indices = IndicesBuffer.UnitCube.Storage * 3; // picking every 3rd vertex turns out to select just the right vertices

            // some configuration
            Value = (double)rand(1); 
            Border.Color = borderColor ?? Color.Black;

        }
        #endregion

        /// <summary>
        /// ToString override for better debug output
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return String.Format("Brick - Fill:{0} Border:{1}", Fill.Color ?? Color.Empty, Border.Color ?? Color.Empty);
        }

        #region synchronizing functions
        /// <summary>
        /// This function is used by the base.Synchronize() method only. It creates a new, empty instance of the Brick class 
        /// </summary>
        /// <param name="source">source (global scene) object</param>
        /// <returns>Empty, new instance of a brick</returns>
        /// <remarks>This is the standard implementation of this method. One should always return an empty (!) instance of this class. 
        /// The properties of the class are afterwards synchronized in the Synchronize() method. 
        /// It is recommended to implement a private constructor withour any arguments in order to realize such an empty instance. 
        /// </remarks>
        protected override Node CreateSynchedCopy(Node source) {
            return new Brick();
        }
        /// <summary>
        /// This method synchronized all relevant properties / attributes of this class
        /// </summary>
        /// <param name="copy">The existint synchronized copy or null if this is the first call for this instance</param>
        /// <param name="syncParams">Extended synchronization parameters (internal use)</param>
        /// <returns>synchronized copy</returns>
        /// <remarks><para>This method creates and maintains synchronized copies of the scene graph objects. Such copies assemble the 
        /// synchronized tree used for rendering by the drivers of ILNumerics Visualization Engine. This method gets called in every 
        /// rendering frame in order to update the synchronized tree with all changes potentially have happened to the global scene. 
        /// Each implementation of Synchronize does only synchronize the properties of its own class. Base class implementations 
        /// handle those properties which are derived from the base class.</para>
        /// <para>The below scheme is recommended for own implementations. It ensures that the synchronization takes place only when 
        /// changes have really happened. </para></remarks>
        public override Node Synchronize(Node copy, SyncParams syncParams) {
            var synchedCopy = base.Synchronize(copy, syncParams) as Brick;
            if (copy == null || copy.Version != SynchedVersion) { 
                // first time synchronized or changes happened
                synchedCopy.m_value = m_value; 
            }
            return synchedCopy; 
        }
        #endregion

        #region copying functions
        /// <summary>
        /// Enable automatic copies for the class
        /// </summary>
        /// <returns>Copy of this brick</returns>
        /// <remarks>Without overriding this method copies will be made by the base class, neglecting all attributes of Brick and 
        /// creating objects of type ILGroup. 
        /// It is recommended to use a copy constructor in order to implement this method.</remarks>
        public override Node Copy() {
            return new Brick(this);
        }
        #endregion

        #region special function manipulating the rendering pipeline (specific for this Brick example)
        /// <summary>
        /// This function gets called just before the renderer driver starts to render the object
        /// </summary>
        /// <param name="parameter">extended render parameters</param>
        /// <returns>true if the object is eligible for rendering</returns>
        protected override bool BeginVisit(RenderParameter parameter) {
            if (parameter.CurrentPassCount == 1) {
                // position the label to the front-most vertex of the brick. Take the current rotation into account. 
                using (Scope.Enter()) {
                    Array<float> vertices = Fill.Positions.Storage;
                    // transform to clip coords
                    vertices.a = parameter.ProjectionTransform * (parameter.CurrentModel2CameraTransform * vertices);
                    // perspective divide + viewport transform
                    vertices.a = parameter.ViewTransform * (vertices / vertices["end;:"]);
                    // find the front-most vertex (the largest z coordinate)
                    Array<long> Imin = 1; // holds the index of the nearest vertex
                    min(vertices["2;:"], I: Imin);
                    // set the label position to the nearest vertex
                    vertices.a = Fill.Positions.Storage[":", Imin[0]];
                    Label.Position = new Vector3(vertices.GetValue(0), vertices.GetValue(1), vertices.GetValue(2));
                }
            }
            return base.BeginVisit(parameter);
        }

        #endregion

    }
}

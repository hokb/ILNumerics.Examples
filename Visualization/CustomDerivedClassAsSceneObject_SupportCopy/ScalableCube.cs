using ILNumerics.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomDerivedClassAsSceneObject_SupportCopy {
    /// <summary>
    /// This class is able to survive Copy() operations within a scene graph. Meaning: you can add several instances of 
    /// ScalableCube to a scene graph; ILNumerics will create recursive shallow copies of the instances. If you wouldn't 
    /// override the Copy() method shown below, the resulting copy would only be of the base type ILNumerics.Drawing.Group. In order to have all 
    /// your class logic available for the target copies, you must override at least the methods marked below. This will ensure
    /// that after copying your ScalableCube, instances of ScalableCube are created and stored in the scene. 
    /// </summary>
    /// <remarks>ILNumerics Version required: 4.3</remarks>
    class ScalableCube : Group {

        #region attributes
        public static readonly string DefaultGroupTag = "ScalableCube";
        public static readonly string DefaultCubeTag = "CubeLines";
        public static readonly string DefaultLabelTag = "CubeLabel";

        // this is a class attribute. The text is stored in the class instance and used to label a cube corner.
        // m_text is the only attribute of the class in this example which needs to be copied in Copy()
        private String m_text; 

        #endregion

        #region properties
        /// <summary>
        /// Text used to label a cubes corner
        /// </summary>
        public String Text {
            get { return m_text; }
            set {
                if (m_text != value) {
                    setText(value);
                    OnPropertyChanged("Text"); 
                }
            }
        }
        // this is a class property. It represents the color of the lines. In difference 
        // to Text it does not store the value itself but only retrieves and delegates 
        // the value to the child Cube object. Therefore, it exposes a more convenient 
        // interface to users of the class.
        public Color Color {
            get {
                return Cube.Color ?? Color.Empty;
            }
            set {
                if (Color != value) {
                    setColor(value);
                    OnPropertyChanged("Color");
                }
            }
        }


        /// <summary>
        /// Gets a reference to the lines object realizing the (unscaled) cube within this ScalableCube
        /// </summary>
        public Lines Cube {
            get { return First<Lines>(DefaultCubeTag); }
        }
        /// <summary>
        /// Gets a reference to the label object within this ScalableCube
        /// </summary>
        public Label Label {
            get { return First<Label>(DefaultLabelTag); }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Custom constructor. Construct a new scalable cube custom scene object. (arbitrary signature)
        /// </summary>
        /// <param name="tag">[optional] tag used to identify the object within the scene</param>
        public ScalableCube(object tag = null, object add_more_parameters_here_if_needed = null)
            : base(tag ?? DefaultGroupTag) {
            Add(Shapes.UnitCubeWireframe, DefaultCubeTag);
            Cube.Width = 3;

            Add(new Group(target: RenderTarget.Screen2DNear) { new Label(tag: DefaultLabelTag) });
            Label.Color = Color.Red;
            Label.Anchor = new PointF(.5f, 2f);

            // initial values
            setText("Instance ID: " + ID);
            setColor(Color.Blue);
        }

        /// <summary>
        /// The copy constructor is the recommended way to implement the Copy() function
        /// </summary>
        /// <param name="source"></param>
        protected ScalableCube(ScalableCube source)
            : base(source) {
            // copy over all attributes from the source. Note, that only real 
            // attributes need to be taken into account here! All properties 
            // which store their values in child objects (like 'Color') are 
            // copied automatically. 
            Text = source.Text;
            // feel free to change the logic here! You can give the copied instance any value...
            // Also, one may establish a synchronization scheme here: copied instanced could 
            // register for changes on the source instance and at runtime follow them automatically a.s.o ...
            Text = $"ID: {ID} (Copy)";  
        }
        #endregion

        #region helper methods
        private void setColor(System.Drawing.Color color) {
            Cube.Color = color;
            Configure(); 
        }

        private void setText(string p) {
            m_text = p; 
            Label.Text = m_text;
            Configure(); 
        }
        #endregion

        /// <summary>
        /// Copy function, used for (auto) shallow copying the custom class. 
        /// </summary>
        /// <returns>A shallow copy: all 'cheap' objects like scalar attributes are copied, buffers are shared.</returns>
        /// <remarks>This is the only method which is actually *required* to get implemented. This method gets called 
        /// by ILNumerics Visualization Engine when a user attempts to add a scene node more than once. It is responsible 
        /// for doing the shallow copy.
        /// By implementing this method, we can make sure to return the actual type of our custom scene object. It 
        /// is recommended (but not obligatory) to implement the method by help of a copy constructor, as shown.</remarks>
        public override Node Copy() {
            return new ScalableCube(this); 
        }

    }
}

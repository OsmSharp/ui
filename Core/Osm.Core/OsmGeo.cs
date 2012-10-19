using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Tools.Math.Shapes;

namespace Osm.Core
{
    /// <summary>
    /// Base class for all the osm data that represents an object on the map.
    /// 
    /// Nodes, Ways and Relations
    /// </summary>
    [Serializable]
    public abstract class OsmGeo : OsmBase
    {
        internal OsmGeo(long id)
            :base(id)
        {
            this.Visible = true;
            this.UserId = null;
            this.User = null;
        }

        #region Shape - Interpreter

        /// <summary>
        /// The interpreter for these objects.
        /// </summary>
        public static IShapeInterpreter ShapeInterperter = new SimpleShapeInterpreter(); // set a default shape interpreter.

        /// <summary>
        /// The shape this osm geo object represents.
        /// </summary>
        private ShapeF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> _shape;

        /// <summary>
        /// Returns the shape of this object represents.
        /// </summary>
        public ShapeF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> Shape
        {
            get
            {
                if (_shape == null)
                {
                    _shape = OsmGeo.ShapeInterperter.Interpret(this);
                }
                return _shape;
            }
        }

        /// <summary>
        /// Make sure the shape of this objects will be recalculated the next time it is requested.
        /// </summary>
        public void ResetShape()
        {
            _shape = null;
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// The bounding box of object.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get
            {
                return this.Shape.BoundingBox;
            }
        }

        /// <summary>
        /// Gets/Sets the changeset id.
        /// </summary>
        public long? ChangeSetId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the visible flag.
        /// </summary>
        public bool Visible { get; set; }

        #endregion
    }
}

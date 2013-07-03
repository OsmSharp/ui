using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Geo.Attributes;
using OsmSharp.Math.Geo;

namespace OsmSharp.Geo.Geometries
{
    /// <summary>
    /// Base class for all geometries.
    /// </summary>
    public abstract class Geometry
    {
        /// <summary>
        /// Returns the smallest possible bounding box containing this geometry.
        /// </summary>
        public abstract GeoCoordinateBox Box
        {
            get;
        }

        /// <summary>
        /// Returns true if this geometry is inside the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public abstract bool IsInside(GeoCoordinateBox box);

        /// <summary>
        /// Gets/sets the geometry attribute id.
        /// </summary>
        public uint GeometryAttributeId { get; set; }

        /// <summary>
        /// Returns the attribute collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GeometryAttributeCollection GetAttributes(IGeometryAttributesIndex index)
        {
            return index.Get(this.GeometryAttributeId);
        }
    }
}

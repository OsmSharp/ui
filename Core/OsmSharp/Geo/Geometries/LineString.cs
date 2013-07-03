using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Geo;

namespace OsmSharp.Geo.Geometries
{
    /// <summary>
    /// Represents a linestring, a simple sequence of line-segments.
    /// </summary>
    public class LineString : Geometry
    {
        /// <summary>
        /// Creates a new linestring.
        /// </summary>
        public LineString()
        {
            this.Coordinates = new List<GeoCoordinate>();
        }

        /// <summary>
        /// Creates a new linestring.
        /// </summary>
        /// <param name="coordinates">The coordinates in this linestring.</param>
        public LineString(IEnumerable<GeoCoordinate> coordinates)
        {
            this.Coordinates = new List<GeoCoordinate>(coordinates);
        }

        /// <summary>
        /// Creates a new linestring.
        /// </summary>
        /// <param name="coordinates">The coordinates in this linestring.</param>
        public LineString(params GeoCoordinate[] coordinates)
        {
            this.Coordinates = new List<GeoCoordinate>(coordinates);
        }

        /// <summary>
        /// Returns the list of coordinates.
        /// </summary>
        public List<GeoCoordinate> Coordinates { get; private set; }

        /// <summary>
        /// Returns the smallest possible bounding box containing this geometry.
        /// </summary>
        public override GeoCoordinateBox Box
        {
            get { return new GeoCoordinateBox(this.Coordinates.ToArray()); }
        }

        /// <summary>
        /// Returns true if this linestring is inside the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public override bool IsInside(GeoCoordinateBox box)
        {
            throw new NotImplementedException();
        }
    }
}

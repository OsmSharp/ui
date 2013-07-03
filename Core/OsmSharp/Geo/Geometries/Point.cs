using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Geo;

namespace OsmSharp.Geo.Geometries
{
    /// <summary>
    /// Represents a single point.
    /// </summary>
    public class Point : Geometry
    {
        /// <summary>
        /// Creates a new point geometry.
        /// </summary>
        /// <param name="coordinate"></param>
        public Point(GeoCoordinate coordinate)
        {
            this.Coordinate = coordinate;
        }

        /// <summary>
        /// Gets/sets the coordinate.
        /// </summary>
        public GeoCoordinate Coordinate { get; set; }

        /// <summary>
        /// Returns the smallest possible bounding box containing this geometry.
        /// </summary>
        public override GeoCoordinateBox Box
        {
            get { return new GeoCoordinateBox(this.Coordinate, this.Coordinate); }
        }

        /// <summary>
        /// Returns true if this point is visible inside the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public override bool IsInside(GeoCoordinateBox box)
        {
            if (box == null) { throw new ArgumentNullException(); }

            return box.IsInside(this.Coordinate);
        }
    }
}

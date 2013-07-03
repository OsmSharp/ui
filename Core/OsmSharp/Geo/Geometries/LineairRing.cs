using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Geo;

namespace OsmSharp.Geo.Geometries
{
    /// <summary>
    /// Represents a lineair ring, a polygon without holes.
    /// </summary>
    public class LineairRing : LineString
    {
        /// <summary>
        /// Creates a new lineair ring.
        /// </summary>
        public LineairRing()
        {

        }

        /// <summary>
        /// Creates a new lineair ring.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        public LineairRing(IEnumerable<GeoCoordinate> coordinates)
            : base(coordinates)
        {

        }

        /// <summary>
        /// Creates a new lineair ring.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        public LineairRing(params GeoCoordinate[] coordinates)
            : base(coordinates)
        {

        }
    }
}

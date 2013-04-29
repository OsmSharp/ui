using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.UI.Map.Elements
{
    /// <summary>
    /// Represents a point.
    /// </summary>
    public class ElementPoint : ElementBase
    {
        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="radius"></param>
        public ElementPoint(GeoCoordinate location, float radius)
        {
            this.Location = location;
            this.Radius = radius;
        }

        /// <summary>
        /// The location of this point.
        /// </summary>
        public GeoCoordinate Location { get; private set; }

        /// <summary>
        /// The radius.
        /// </summary>
        public float Radius { get; private set; }

        /// <summary>
        /// Returns true if this point is visible.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public override bool IsVisibleIn(GeoCoordinateBox box)
        {
            if (box != null &&
                this.Location != null)
            {
                return box.IsInside(this.Location);
            }
            return false;
        }

        /// <summary>
        /// Returns the shortest distance to this point.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public override double ShortestDistanceTo(GeoCoordinate coordinate)
        {
            if (coordinate != null &&
                this.Location != null)
            { // the given coordinate existis.
                return coordinate.Distance(this.Location);
            }
            return double.MaxValue;
        }
    }
}
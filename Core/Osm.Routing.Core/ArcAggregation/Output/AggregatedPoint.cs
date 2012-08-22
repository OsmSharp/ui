using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Angle;
using Tools.Math.Geo;
using Tools.Math.Geo.Meta;

namespace Osm.Routing.Core.ArcAggregation.Output
{
    /// <summary>
    /// Represents a point in an aggregated route.
    /// </summary>
    public class AggregatedPoint : Aggregated
    {
        /// <summary>
        /// The physical location of this point.
        /// </summary>
        public GeoCoordinate Location { get; set; }

        /// <summary>
        /// The arc following this point.
        /// </summary>
        public AggregatedArc Next { get; set; }

        public override Aggregated GetNext()
        {
            return this.Next;
        }

        #region Properties

        /// <summary>
        /// The angle between the end of the previous arc and the beginning of the next arc.
        /// </summary>
        public RelativeDirection Angle { get; set; }
        
        ///// <summary>
        ///// The tags/properties.
        ///// </summary>
        //public List<KeyValuePair<string, string>> Tags { get; set; }

        /// <summary>
        /// The point of points at this location.
        /// </summary>
        public List<PointPoi> Points { get; set; }

        #endregion

        #region Arcs-not-taken

        /// <summary>
        /// List of the arcs not taken and their angle with respect to the end of the previous arc.
        /// </summary>
        public List<KeyValuePair<RelativeDirection, AggregatedArc>> ArcsNotTaken { get; set; }

        #endregion
    }

    /// <summary>
    /// Represents a point that is being routed to/from and it's properties.
    /// </summary>
    public class PointPoi
    {
        /// <summary>
        /// The angle between the direction of the latest arc and the direction of this poi.
        /// </summary>
        public RelativeDirection Angle { get; set; }

        /// <summary>
        /// The physical location of this point.
        /// </summary>
        public GeoCoordinate Location { get; set; }

        /// <summary>
        /// The name of the point.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The tags/properties.
        /// </summary>
        public List<KeyValuePair<string, string>> Tags { get; set; }
    }
}

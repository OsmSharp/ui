using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Distance;

namespace Osm.Routing.Core.ArcAggregation.Output
{
    /// <summary>
    /// Represents an arc in the aggregated route.
    /// </summary>
    public class AggregatedArc : Aggregated
    {
        /// <summary>
        /// The end point of this arc.
        /// </summary>
        public AggregatedPoint Next { get; set; }

        public override Aggregated GetNext()
        {
            return this.Next;
        }

        /// <summary>
        /// The distance in meter.
        /// </summary>
        public Meter Distance { get; set; }

        #region Properties

        /// <summary>
        /// The default name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The name in different languages.
        /// </summary>
        public List<KeyValuePair<string, string>> Names { get; set; }

        /// <summary>
        /// The tags/properties.
        /// </summary>
        public List<KeyValuePair<string, string>> Tags { get; set; }

        #endregion
    }
}

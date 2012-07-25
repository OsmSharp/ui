using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Osm.Routing.Core.ArcAggregation.Output;

namespace Osm.Routing.Instructions
{
    /// <summary>
    /// Represents an instruction.
    /// </summary>
    public class Instruction
    {
        /// <summary>
        /// Creates a new instruction with only a location.
        /// </summary>
        /// <param name="location"></param>
        public Instruction(GeoCoordinateBox location)
        {
            this.Location = location;
        }

        /// <summary>
        /// Creates a new instruction with a location and points of interest.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="pois"></param>
        public Instruction(GeoCoordinateBox location, List<PointPoi> pois)
        {
            this.Location = location;
            this.Pois = pois;
        }

        /// <summary>
        /// The points of interest for this instruction.
        /// </summary>
        public List<PointPoi> Pois { get; private set; }

        /// <summary>
        /// The location of this instruction.
        /// </summary>
        public GeoCoordinateBox Location { get; private set; }

        /// <summary>
        /// The instruction text.
        /// </summary>
        public string Text { get; set; }
    }
}

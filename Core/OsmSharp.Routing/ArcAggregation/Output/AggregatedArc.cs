// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Units.Distance;

namespace OsmSharp.Routing.ArcAggregation.Output
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

        /// <summary>
        /// Returns the next aggregated.
        /// </summary>
        /// <returns></returns>
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

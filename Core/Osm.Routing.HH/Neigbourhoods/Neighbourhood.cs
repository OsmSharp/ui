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
using System.Collections.Generic;
using Osm.Routing.HH.Primitives;

namespace Osm.Routing.HH.Neigbourhoods
{
    /// <summary>
    /// Represents the neighbourhood of a vertex.
    /// </summary>
    internal class Neighbourhood
    {
        /// <summary>
        /// The source of the neigbourhood.
        /// </summary>
        public HighwayVertex Source { get; set; }

        /// <summary>
        /// Holds all vertices in the forward neighbourhood.
        /// </summary>
        public HashSet<long> ForwardNeighbourhood { get; set; }

        /// <summary>
        /// Holds all vertices in the backward neighbourhood.
        /// </summary>
        public HashSet<long> BackwardNeighbourhood { get; set; }

        /// <summary>
        /// The neigbourhood radius.
        /// </summary>
        public float Radius { get; set; }
    }
}

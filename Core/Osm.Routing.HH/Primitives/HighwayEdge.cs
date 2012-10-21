// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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

namespace Osm.Routing.HH.Primitives
{
    /// <summary>
    /// An highway edge.
    /// </summary>
    public class HighwayEdge
    {
        /// <summary>
        /// The distance of this edge.
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// The id of the vertex at the other end of this vertex.
        /// </summary>
        public int VertexId { get; set; }

        /// <summary>
        /// Holds the forward flag.
        /// </summary>
        public bool Forward { get; set; }

        /// <summary>
        /// Holds the backward flag.
        /// </summary>
        public bool Backward { get; set; }        

        /// <summary>
        /// Holds the highest level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Holds the highest core level.
        /// </summary>
        public int LevelCore { get; set; }

        /// <summary>
        /// Holds the highest contracted level.
        /// </summary>
        public int LevelContracted { get; set; }

        /// <summary>
        /// The tags of this edge.
        /// </summary>
        public string Tags { get; set; }
    }
}

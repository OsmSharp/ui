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
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.DynamicGraph;

namespace OsmSharp.Routing.CH.PreProcessing
{
    /// <summary>
    /// Represents the data on a CH edge.
    /// </summary>
    public class CHEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Forward flag.
        /// </summary>
        public bool Forward { get; set; }

        /// <summary>
        /// Backward flag.
        /// </summary>
        public bool Backward { get; set; }

        /// <summary>
        /// Weight.
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// Returns true if this edge is a shortcut.
        /// </summary>
        public bool HasContractedVertex
        {
            get
            {
                return this.ContractedVertexId > 0;
            }
        }

        /// <summary>
        /// The vertex contracted by this edge.
        /// </summary>
        public uint ContractedVertexId { get; set; }

        /// <summary>
        /// Returns true if this edge has tags.
        /// </summary>
        public bool HasTags { get; set; }

        /// <summary>
        /// Returns the tags.
        /// </summary>
        public uint Tags { get; set; }

        /// <summary>
        /// Returns the weight.
        /// </summary>
        double IDynamicGraphEdgeData.Weight
        {
            get { return this.Weight; }
        }
    }

    /// <summary>
    /// Comparer for CH edges.
    /// </summary>
    public class CHEdgeDataComparer : IDynamicGraphEdgeComparer<CHEdgeData>
    {
        /// <summary>
        /// Return true if the existence of edge1 makes edge2 useless.
        /// </summary>
        /// <param name="edge1"></param>
        /// <param name="edge2"></param>
        /// <returns></returns>
        public bool Overlaps(CHEdgeData edge1, CHEdgeData edge2)
        {
            if (!edge1.Backward && edge2.Backward)
            {
                return false;
            }
            if (!edge1.Forward && edge1.Forward)
            {
                return false;
            }
            if (edge1.Weight > edge2.Weight)
            {
                return false;
            }
            return true;
        }
    }
}

// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Routing.CH.PreProcessing
{
    /// <summary>
    /// Represents the data on a CH edge.
    /// </summary>
    public struct CHEdgeData : IGraphEdgeData
    {
        /// <summary>
        /// Holds the contracted direction.
        /// </summary>
        /// <remarks>0: uncontracted, 1: tohigher, 2: tolower</remarks>
        private byte _contractedDirection;

        /// <summary>
        /// Gets/sets the value.
        /// </summary>
        internal byte ContractedDirectionValue
        {
            get
            {
                return _contractedDirection;
            }
            set
            {
                _contractedDirection = value;
            }
        }

        /// <summary>
        /// Sets the contracted direction flags.
        /// </summary>
        /// <param name="toHigher"></param>
        /// <param name="toBackward"></param>
        public void SetContractedDirection(bool toHigher, bool toBackward)
        {
            if(!toHigher && !toBackward)
            {
                _contractedDirection = 0;
            }
            else if(toHigher)
            {
                _contractedDirection = 1;
            }
            else
            {
                _contractedDirection = 2;
            }
        }

        /// <summary>
        /// Gets the to higher flag.
        /// </summary>
        public bool ToHigher
        {
            get
            {
                return _contractedDirection == 1;
            }
        }

        /// <summary>
        /// Gets the to higher flag.
        /// </summary>
        public bool ToLower
        {
            get
            {
                return _contractedDirection == 2;
            }
        }

        /// <summary>
        /// Gets the forward flag.
        /// </summary>
        public bool Forward
        {
            get
            {
                return this.ForwardWeight != float.MaxValue;
            }
        }

        /// <summary>
        /// Gets the backward flag.
        /// </summary>
        public bool Backward
        {
            get
            {
                return this.BackwardWeight != float.MaxValue;
            }
        }

        /// <summary>
        /// Holds the forward weight.
        /// </summary>
        public float ForwardWeight { get; set; }

        /// <summary>
        /// Holds the forward contracted id.
        /// </summary>
        public uint ForwardContractedId { get; set; }

        /// <summary>
        /// Holds the backward weight.
        /// </summary>
        public float BackwardWeight { get; set; }

        /// <summary>
        /// Holds the backward contracted id.
        /// </summary>
        public uint BackwardContractedId { get; set; }

        /// <summary>
        /// Returns true if the backward or forward edge represents a neighbour relation.
        /// </summary>
        public bool RepresentsNeighbourRelations
        {
            get
            {
                return (this.Backward && this.BackwardContractedId == 0) ||
                       (this.Forward && this.ForwardContractedId == 0);
            }
        }

        /// <summary>
        /// Contains a value that represents tagsId and forward flag [forwardFlag (true when zero)][tagsIdx].
        /// </summary>
        private uint _value;

        /// <summary>
        /// Gets/sets the value.
        /// </summary>
        internal uint TagsValue
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Flag indicating if the tags are forward relative to this edge or not.
        /// </summary>
        public bool TagsForward
        {
            get
            { // true when first bit is 0.
                return _value % 2 == 0;
            }
            set
            {
                if (_value % 2 == 0)
                { // true already.
                    if (!value) { _value = _value + 1; }
                }
                else
                { // false already.
                    if (value) { _value = _value - 1; }
                }
            }
        }

        /// <summary>
        /// The properties of this edge.
        /// </summary>
        public uint Tags
        {
            get
            {
                return _value / 2;
            }
            set
            {
                if (_value % 2 == 0)
                { // true already.
                    _value = value * 2;
                }
                else
                { // false already.
                    _value = (value * 2) + 1;
                }
            }
        }


        /// <summary>
        /// Returns the exact inverse of this edge.
        /// </summary>
        /// <returns></returns>
        public IGraphEdgeData Reverse()
        {
            var reverse = new CHEdgeData();

            // forward/backward specific info.
            reverse.BackwardWeight = this.ForwardWeight;
            reverse.BackwardContractedId = this.ForwardContractedId;
            reverse.ForwardContractedId = this.BackwardContractedId;
            reverse.ForwardWeight = this.BackwardWeight;

            // tags.
            reverse.Tags = this.Tags;
            reverse.TagsForward = !this.TagsForward;

            // contracted direction info.
            reverse._contractedDirection = this._contractedDirection;
            switch(_contractedDirection)
            {
                case 1:
                    reverse._contractedDirection = 2;
                    break;
                case 2:
                    reverse._contractedDirection = 1;
                    break;
            }
            return reverse;
        }

        /// <summary>
        /// Returns true if the given edge equals this edge.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IGraphEdgeData other)
        {
            var otherEdge = (CHEdgeData)other;
            return (otherEdge._contractedDirection == this._contractedDirection &&
                otherEdge._value == this._value &&
                otherEdge.BackwardContractedId == this.BackwardContractedId &&
                otherEdge.BackwardWeight == this.BackwardWeight &&
                otherEdge.ForwardWeight == this.ForwardWeight &&
                otherEdge.ForwardContractedId == this.ForwardContractedId);
        }
    }

    /// <summary>
    /// Contains extensions related to the CHEdgeData.
    /// </summary>
    public static class CHExtensions
    {
        /// <summary>
        /// Removes all contracted edges.
        /// </summary>
        /// <param name="edges"></param>
        public static List<Edge<CHEdgeData>> KeepUncontracted(this List<Edge<CHEdgeData>> edges)
        {
            var result = new List<Edge<CHEdgeData>>(edges.Count);
            foreach (var edge in edges)
            {
                if ((edge.EdgeData.Backward && edge.EdgeData.BackwardContractedId == 0) || (edge.EdgeData.Forward && edge.EdgeData.ForwardContractedId == 0))
                {
                    result.Add(edge);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the arcs that point to higher vertices.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public static List<Edge<CHEdgeData>> GetArcsHigher(this IGraph<CHEdgeData> graph,
            uint vertexId)
        {
            var arcs = graph.GetEdges(vertexId).ToList();
            var higherArcs = new List<Edge<CHEdgeData>>();
            for (int idx = 0; idx < arcs.Count; idx++)
            {
                if (arcs[idx].EdgeData.ToHigher)
                {
                    higherArcs.Add(arcs[idx]);
                }
            }
            return higherArcs;
        }

        /// <summary>
        /// Returns the arcs that point to lower vertices.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public static List<Edge<CHEdgeData>> GetArcsLower(this IGraph<CHEdgeData> graph,
            uint vertexId)
        {
            var arcs = graph.GetEdges(vertexId).ToList();
            var higherArcs = new List<Edge<CHEdgeData>>();
            for (int idx = 0; idx < arcs.Count; idx++)
            {
                if (!arcs[idx].EdgeData.ToHigher)
                {
                    higherArcs.Add(arcs[idx]);
                }
            }
            return higherArcs;
        }
    }
}

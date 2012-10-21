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

namespace Tools.Math.Graph.Routing
{
    /// <summary>
    /// Represents a route entry; one atomic part of a route.
    /// </summary>
    /// <typeparam name="EdgeType"></typeparam>
    /// <typeparam name="VertexType"></typeparam>
    public class GraphRouteEntry<EdgeType, VertexType>
    {
        /// <summary>
        /// The edge the route travels along in this atomic part.
        /// </summary>
        private EdgeType _edge;

        /// <summary>
        /// The vertex being travelled to.
        /// </summary>
        private GraphRouteEntryPoint<VertexType> _to;

        /// <summary>
        /// The weight of this entry.
        /// </summary>
        private float _weight;

        /// <summary>
        /// Creates a new route entry.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="to"></param>
        public GraphRouteEntry(
            GraphRouteEntryPoint<VertexType> to,
            EdgeType edge,
            float weight)
        {
            _edge = edge;
            _to = to;
            _weight = weight;
        }

        /// <summary>
        /// Returns the edge being travelled along.
        /// </summary>
        public EdgeType Edge
        {
            get
            {
                return _edge;
            }
        }

        /// <summary>
        /// Returns the vertex being travelled to.
        /// </summary>
        public GraphRouteEntryPoint<VertexType> To
        {
            get
            {
                return _to;
            }
        }

        /// <summary>
        /// Returns the weight of this entry.
        /// </summary>
        public float Weight
        {
            get
            {
                return _weight;
            }
        }
    }
}

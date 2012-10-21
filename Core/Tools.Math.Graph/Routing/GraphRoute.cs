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
    /// Represents a route along entry points.
    /// </summary>
    /// <typeparam name="EdgeType"></typeparam>
    /// <typeparam name="VertexType"></typeparam>
    public class GraphRoute<EdgeType, VertexType>
    {
        /// <summary>
        /// Holds the entry point the route starts at.
        /// </summary>
        private GraphRouteEntryPoint<VertexType> _from;

        /// <summary>
        /// Holds the list of entry the route travel along.
        /// </summary>
        private IList<GraphRouteEntry<EdgeType, VertexType>> _entry_list;

        /// <summary>
        /// Holds the weight of the route.
        /// </summary>
        private float _weight;

        /// <summary>
        /// Creates a new route.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="entry_list"></param>
        public GraphRoute(
            GraphRouteEntryPoint<VertexType> from,
            IList<GraphRouteEntry<EdgeType, VertexType>> entry_list,
            float weight)
        {
            _from = from;
            _entry_list = new List<GraphRouteEntry<EdgeType, VertexType>>(entry_list);
            _weight = weight;
        }

        /// <summary>
        /// Returns the entry point this route starts at.
        /// </summary>
        public GraphRouteEntryPoint<VertexType> From
        {
            get
            {
                return _from;
            }
        }

        /// <summary>
        /// Returns the list of entries this route follows.
        /// </summary>
        public IList<GraphRouteEntry<EdgeType, VertexType>> Entries
        {
            get
            {
                return _entry_list;
            }
        }

        /// <summary>
        /// Returns the weight of this route.
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

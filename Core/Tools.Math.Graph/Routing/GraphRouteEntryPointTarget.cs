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
    /// Represents a route entry at a routing target.
    /// </summary>
    /// <typeparam name="VertexType"></typeparam>
    public class GraphRouteEntryPointTarget<VertexType> : GraphRouteEntryPoint<VertexType>
    {
        /// <summary>
        /// Holds the routing target of this route entry point.
        /// </summary>
        private GraphRouteTarget<VertexType> _target;

        /// <summary>
        /// Creates a new route entry target point.
        /// </summary>
        /// <param name="target"></param>
        public GraphRouteEntryPointTarget(GraphRouteTarget<VertexType> target)
            : base(target.Vertex, true)
        {
            _target = target;
        }

        /// <summary>
        /// Returns the routing target of this route entry.
        /// </summary>
        public GraphRouteTarget<VertexType> Target
        {
            get
            {
                return _target;
            }
        }
    }
}

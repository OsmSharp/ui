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

namespace Tools.Math.Graph.Routing
{
    /// <summary>
    /// Represents a routing target.
    /// </summary>
    /// <typeparam name="VertexType"></typeparam>
    public abstract class GraphRouteTarget<VertexType>
    {
        /// <summary>
        /// Holds the vertex the target exists at.
        /// </summary>
        private VertexType _vertex;

        /// <summary>
        /// Creates a new route target at the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        protected GraphRouteTarget(VertexType vertex)
        {
            _vertex = vertex;
        }

        /// <summary>
        /// Returns the vertex this target exists at.
        /// </summary>
        public VertexType Vertex
        {
            get
            {
                return _vertex;
            }
        }
    }
}

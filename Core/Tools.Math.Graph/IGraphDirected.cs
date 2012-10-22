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

namespace Tools.Math.Graph
{
    /// <summary>
    /// Basic interface representing a directed graph.
    /// </summary>
    /// <typeparam name="VertexType"></typeparam>
    public interface IGraphDirected<VertexType> : IGraph<VertexType>
        where VertexType : class, IEquatable<VertexType>
    {
        /// <summary>
        /// Returns all the neighbours for the given vertex.
        /// </summary>
        /// <param name="vertex_id">The vertex to return the neighbours for.</param>
        /// <returns>The vertices with their associated weights.</returns>
        Dictionary<long, float> GetNeighboursReversed(long vertex_id, HashSet<long> exceptions);

        /// <summary>
        /// Returns all the neighbours for the given vertex as if this graph was undirected.
        /// </summary>
        /// <param name="vertex_id">The vertex to return the neighbours for.</param>
        /// <returns>The vertices with their associated weights; counter directed edges have negative weights returned.</returns>
        Dictionary<long, float> GetNeighboursUndirected(long vertex_id, HashSet<long> exceptions);
    }
}

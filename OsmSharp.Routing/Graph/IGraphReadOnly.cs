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

using OsmSharp.Collections.Coordinates;
using OsmSharp.Math.Geo.Simple;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// Abstracts an graph implementation. 
    /// </summary>
    public interface IGraphReadOnly<TEdgeData>
        where TEdgeData : IGraphEdgeData
    {
        /// <summary>
        /// Gets an existing vertex.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        bool GetVertex(uint id, out float latitude, out float longitude);

        /// <summary>
        /// Returns true if the given edge exists.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        bool ContainsEdge(uint vertexId, uint neighbour);

        /// <summary>
        /// Returns all arcs for the given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        IEdgeEnumerator<TEdgeData> GetEdges(uint vertexId);

        /// <summary>
        /// Gets the data associated with the given edge and returns true if the edge exists.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool GetEdge(uint vertex1, uint vertex2, out TEdgeData data);

        /// <summary>
        /// Gets the shape associated with the given edge and returns true if the edge exists.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        bool GetEdgeShape(uint vertex1, uint vertex2, out ICoordinateCollection shape);

        /// <summary>
        /// Returns the total number of vertices.
        /// </summary>
        uint VertexCount
        {
            get;
        }
    }
}
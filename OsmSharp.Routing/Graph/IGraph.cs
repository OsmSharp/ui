// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// Abstracts a graph implementation.
    /// </summary>
    public interface IGraph<TEdgeData> : IGraphWriteOnly<TEdgeData>
        where TEdgeData : IGraphEdgeData
    {
        /// <summary>
        /// Removes all edges adjacent to the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        void RemoveEdges(uint vertex);

        /// <summary>
        /// Removes all the edges between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        void RemoveEdge(uint from, uint to);

        /// <summary>
        /// Removes the edge between the two given vertices and with matching data.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        void RemoveEdge(uint from, uint to, TEdgeData data);
    }
}
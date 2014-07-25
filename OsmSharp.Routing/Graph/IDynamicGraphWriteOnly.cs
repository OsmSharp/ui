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

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// Abstracts a graph implementation that is write-only.
    /// </summary>
    public interface IDynamicGraphWriteOnly<TEdgeData> : IDynamicGraphReadOnly<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Adds a vertex.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        uint AddVertex(float latitude, float longitude);

        /// <summary>
        /// Sets the vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        void SetVertex(uint vertex, float latitude, float longitude);

        /// <summary>
        /// Adds an edge with associated data.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        void AddEdge(uint from, uint to, TEdgeData data);

        /// <summary>
        /// Adds an edge with associated data.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        /// <param name="comparer"></param>
        void AddEdge(uint from, uint to, TEdgeData data, IDynamicGraphEdgeComparer<TEdgeData> comparer);

        /// <summary>
        /// Trims this graph to it's smallest possible size.
        /// </summary>
        void Trim();
    }
}
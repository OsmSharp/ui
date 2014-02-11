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
    /// Abstracts edge information.
    /// </summary>
    public interface IDynamicGraphEdgeData
    {
        /// <summary>
        /// Returns the forward flag.
        /// </summary>
        bool Forward { get; }

        /// <summary>
        /// Returns true if this edge represents a neighbour relation.
        /// </summary>
        bool RepresentsNeighbourRelations { get; }

        /// <summary>
        /// Returns the tags identifier.
        /// </summary>
        uint Tags
        {
            get;
        }
    }

    /// <summary>
    /// Abstract a comparer for edges.
    /// </summary>
    public interface IDynamicGraphEdgeComparer<in TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Returns true if the data in the edge2 is useless if the data in edge1 is present.
        /// </summary>
        /// <param name="edge1"></param>
        /// <param name="edge2"></param>
        /// <returns></returns>
        bool Overlaps(TEdgeData edge1, TEdgeData edge2);
    }
}
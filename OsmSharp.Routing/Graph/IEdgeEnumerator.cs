// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
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
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// Represents an abstract edge enumerator.
    /// </summary>
    public interface IEdgeEnumerator<TEdgeData>
        where TEdgeData : IGraphEdgeData
    {
        /// <summary>
        /// Move to the next edge.
        /// </summary>
        /// <returns></returns>
        bool MoveNext();

        /// <summary>
        /// Returns the current neighbour.
        /// </summary>
        uint Neighbour
        {
            get;
        }

        /// <summary>
        /// Returns the edge data.
        /// </summary>
        TEdgeData EdgeData
        {
            get;
        }

        /// <summary>
        /// Returns the intermediates.
        /// </summary>
        GeoCoordinateSimple[] Intermediates
        {
            get;
        }

        /// <summary>
        /// Returns and calculates the count.
        /// </summary>
        int Count();
    }

    /// <summary>
    /// Holds extensions methods for the edge enumerator.
    /// </summary>
    public static class EdgeEnumeratorExtensions
    {
        /// <summary>
        /// Converts the given edge enumerator to an array of key-value pairs.
        /// </summary>
        /// <typeparam name="TEdgeData"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static KeyValuePair<uint, TEdgeData>[] ToKeyValuePairs<TEdgeData>(this IEdgeEnumerator<TEdgeData> enumerator)
            where TEdgeData : IGraphEdgeData
        {
            var pairs = new List<KeyValuePair<uint, TEdgeData>>();
            while(enumerator.MoveNext())
            {
                pairs.Add(new KeyValuePair<uint, TEdgeData>(enumerator.Neighbour, enumerator.EdgeData));
            }
            return pairs.ToArray();
        }
    }
}
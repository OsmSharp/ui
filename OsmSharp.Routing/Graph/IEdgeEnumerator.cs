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

using OsmSharp.Collections.Coordinates.Collections;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// Represents an abstract edge enumerator, enumerable and edge.
    /// </summary>
    public interface IEdgeEnumerator<TEdgeData> : IEnumerable<Edge<TEdgeData>>, IEnumerator<Edge<TEdgeData>>
        where TEdgeData : IGraphEdgeData
    {
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
        /// The edge data is inverted by default.
        /// </summary>
        bool isInverted
        {
            get;
        }

        TEdgeData InvertedEdgeData
        {
            get;
        }

        /// <summary>
        /// Returns the intermediates.
        /// </summary>
        ICoordinateCollection Intermediates
        {
            get;
        }

        /// <summary>
        /// Returns true if the count is known without enumeration.
        /// </summary>
        bool HasCount
        {
            get;
        }

        /// <summary>
        /// Returns the count if known.
        /// </summary>
        int Count
        {
            get;
        }
    }

    /// <summary>
    /// Abstract representation of an edge.
    /// </summary>
    /// <typeparam name="TEdgeData"></typeparam>
    public class Edge<TEdgeData>
        where TEdgeData : IGraphEdgeData
    {
        /// <summary>
        /// Creates a new edge.
        /// </summary>
        public Edge()
        {

        }

        /// <summary>
        /// Creates a new edge by copying the given edge.
        /// </summary>
        /// <param name="edge"></param>
        public Edge(Edge<TEdgeData> edge)
        {
            this.Neighbour = edge.Neighbour;
            this.EdgeData = edge.EdgeData;
            this.Intermediates = edge.Intermediates;
        }

        /// <summary>
        /// Creates a new edge keeping the current stage of the given enumerator.
        /// </summary>
        /// <param name="enumerator"></param>
        public Edge(IEdgeEnumerator<TEdgeData> enumerator)
        {
            this.Neighbour = enumerator.Neighbour;
            this.EdgeData = enumerator.EdgeData;
            this.Intermediates = enumerator.Intermediates;
        }

        /// <summary>
        /// Returns the current neighbour.
        /// </summary>
        public uint Neighbour
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the edge data.
        /// </summary>
        public TEdgeData EdgeData
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the intermediates.
        /// </summary>
        public ICoordinateCollection Intermediates
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a string representing this edge.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}",
                this.Neighbour,
                this.EdgeData.ToInvariantString());
        }
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
            enumerator.Reset();
            var pairs = new List<KeyValuePair<uint, TEdgeData>>();
            while (enumerator.MoveNext())
            {
                pairs.Add(new KeyValuePair<uint, TEdgeData>(enumerator.Neighbour, enumerator.EdgeData));
            }
            return pairs.ToArray();
        }


        /// <summary>
        /// Converts the given edge enumerator to an array of key-value pairs.
        /// </summary>
        /// <typeparam name="TEdgeData"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static KeyValuePair<uint, KeyValuePair<TEdgeData, ICoordinateCollection>>[] ToKeyValuePairsAndShapes<TEdgeData>(this IEdgeEnumerator<TEdgeData> enumerator)
            where TEdgeData : IGraphEdgeData
        {
            enumerator.Reset();
            var pairs = new List<KeyValuePair<uint, KeyValuePair<TEdgeData, ICoordinateCollection>>>();
            while (enumerator.MoveNext())
            {
                pairs.Add(new KeyValuePair<uint, KeyValuePair<TEdgeData, ICoordinateCollection>>(enumerator.Neighbour, 
                    new KeyValuePair<TEdgeData, ICoordinateCollection>(enumerator.EdgeData, enumerator.Intermediates)));
            }
            return pairs.ToArray();
        }
    }
}
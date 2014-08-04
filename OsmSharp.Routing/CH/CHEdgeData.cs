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

using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graph;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.CH.PreProcessing
{
    /// <summary>
    /// Represents the data on a CH edge.
    /// </summary>
    public struct CHEdgeData : IGraphEdgeData
    {
        /// <summary>
        /// Creates a new CHEdge data class.
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="forward"></param>
        /// <param name="backward"></param>
        public CHEdgeData(float weight, bool forward, bool backward)
            : this()
        {
            this.Weight = weight;
            this.SetDirection(forward, backward);
        }

        /// <summary>
        /// Creates a new CHEdge data class.
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="forward"></param>
        /// <param name="backward"></param>
        /// <param name="toHigher"></param>
        public CHEdgeData(float weight, bool forward, bool backward, bool toHigher)
            : this()
        {
            this.Weight = weight;
            this.SetDirection(forward, backward, toHigher);
        }

        /// <summary>
        /// Creates a new CHEdge data class.
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="forward"></param>
        /// <param name="backward"></param>
        /// <param name="toHigher"></param>
        /// <param name="contractedVertexId"></param>
        /// <param name="tags"></param>
        public CHEdgeData(float weight, bool forward, bool backward, bool toHigher, uint contractedVertexId, uint tags)
            : this()
        {
            this.Weight = weight;
            this.SetDirection(forward, backward, toHigher);
            this.ContractedVertexId = contractedVertexId;
            this.Tags = tags;
        }

        /// <summary>
        /// Direction flag to higher vertex: ( 0=bidirectional,  1=forward,  2=backward,  3=not forward and not backward).
        ///                to lower  vertex: ( 4=bidirectional,  5=forward,  6=backward,  7=not forward and not backward).
        ///                no low/high info: ( 8=bidirectional,  9=forward, 10=backward, 11=not forward and not backward).
        /// </summary>
        public byte Direction { get; set; }

        /// <summary>
        /// Sets the direction without lower/higher info.
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="backward"></param>
        public void SetDirection(bool forward, bool backward)
        {
            if (forward && backward)
            {
                this.Direction = 8;
            }
            else if (forward)
            {
                this.Direction = 9;
            }
            else if (backward)
            {
                this.Direction = 10;
            }
            else
            {
                this.Direction = 11;
            }
        }

        /// <summary>
        /// Sets the direction.
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="backward"></param>
        /// <param name="toHigher"></param>
        public void SetDirection(bool forward, bool backward, bool toHigher)
        {
            if (toHigher)
            {
                if (forward && backward)
                {
                    this.Direction = 0;
                }
                else if (forward)
                {
                    this.Direction = 1;
                }
                else if (backward)
                {
                    this.Direction = 2;
                }
                else
                {
                    this.Direction = 3;
                }
            }
            else
            {
                if (forward && backward)
                {
                    this.Direction = 4;
                }
                else if (forward)
                {
                    this.Direction = 5;
                }
                else if (backward)
                {
                    this.Direction = 6;
                }
                else
                {
                    this.Direction = 7;
                }
            }
        }

        /// <summary>
        /// Gets the forwardflag.
        /// </summary>
        public bool Forward
        {
            get
            {
                return this.Direction == 0 
                    || this.Direction == 1 
                    || this.Direction == 4
                    || this.Direction == 5
                    || this.Direction == 8
                    || this.Direction == 9;
            }
        }

        /// <summary>
        /// Gets the backwardflag.
        /// </summary>
        public bool Backward
        {
            get
            {
                return this.Direction == 0 
                    || this.Direction == 2
                    || this.Direction == 4
                    || this.Direction == 6
                    || this.Direction == 8
                    || this.Direction == 10;
            }
        }

        /// <summary>
        /// Gets the to higher flag.
        /// </summary>
        public bool ToHigher
        {
            get
            {
                return this.Direction == 0 
                    || this.Direction == 1 
                    || this.Direction == 2 
                    || this.Direction == 3;
            }
        }

        /// <summary>
        /// Gets the to lower flag.
        /// </summary>
        public bool ToLower
        {
            get
            {
                return this.Direction == 4
                    || this.Direction == 5
                    || this.Direction == 6
                    || this.Direction == 7;
            }
        }

        /// <summary>
        /// Weight.
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// Returns true if this edge is a shortcut.
        /// </summary>
        public bool HasContractedVertex
        {
            get
            {
                return this.ContractedVertexId > 0;
            }
        }

        /// <summary>
        /// The vertex contracted by this edge.
        /// </summary>
        public uint ContractedVertexId { get; set; }

        /// <summary>
        /// Gets or sets the tags id.
        /// </summary>
        public uint Tags { get; set; }

        /// <summary>
        /// Returns intermediate coordinates (if any).
        /// </summary>
        public GeoCoordinateSimple[] Coordinates
        {
            get { return null; }
        }

        /// <summary>
        /// Returns true if this edge represents a neighbour-relation.
        /// </summary>
        public bool RepresentsNeighbourRelations
        {
            get { return !this.HasContractedVertex; }
        }

        /// <summary>
        /// Creates the exact reverse of this edge.
        /// </summary>
        /// <returns></returns>
        public IGraphEdgeData Reverse()
        {
            // to higher vertex: ( 0=bidirectional,  1=forward,  2=backward,  3=not forward and not backward).
            // to lower  vertex: ( 4=bidirectional,  5=forward,  6=backward,  7=not forward and not backward).
            // no low/high info: ( 8=bidirectional,  9=forward, 10=backward, 11=not forward and not backward).          
            var data = new CHEdgeData();
            data.Tags = this.Tags;
            data.ContractedVertexId = this.ContractedVertexId;
            data.Weight = this.Weight;
            data.Direction = this.Direction;
            switch(this.Direction)
            {
                case 0: // to higher bidirectional.
                    data.Direction = 4; // to lower bidirectional.
                    break;
                case 1: // to higher forward.
                    data.Direction = 6; // to lower backward.
                    break;
                case 2: // to higher backward.
                    data.Direction = 5; // to lower forward.
                    break;
                case 3: // to higher no directional info.
                    data.Direction = 7; // to lower no directional info.
                    break;
                case 4: // to lower bidirectional.
                    data.Direction = 0; // to higher bidirectional.
                    break;
                case 5: // to lower forward.
                    data.Direction = 2; // to higher backward.
                    break;
                case 6: // to lower backward.
                    data.Direction = 1; // to higher forward.
                    break;
                case 7: // to lower no directional info.
                    data.Direction = 3; // to higher no directional info.
                    break;
                case 9: // no low/high forward.
                    data.Direction = 10; // no low/high backward.
                    break;
                case 10: // no low/high backward.
                    data.Direction = 9; // no low/high forward.
                    break;
            }
            return data;
        }

        /// <summary>
        /// Returns true if the other edge represents the same information than this edge.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IGraphEdgeData other)
        {
            if (other is CHEdgeData)
            { // ok, type is the same.
                var otherCH = (CHEdgeData)other;
                if (otherCH.Direction != this.Direction &&
                    otherCH.ContractedVertexId != this.ContractedVertexId &&
                    otherCH.Weight != this.Weight &&
                    otherCH.Tags != this.Tags)
                { // basic info different.
                    return false;
                }

                // only the coordinates can be different now.
                if (this.Coordinates != null)
                { // both have to contain the same coordinates.
                    if (this.Coordinates.Length != otherCH.Coordinates.Length)
                    { // impossible, different number of coordinates.
                        return false;
                    }

                    for (int idx = 0; idx < otherCH.Coordinates.Length; idx++)
                    {
                        if (this.Coordinates[idx].Longitude != otherCH.Coordinates[idx].Longitude ||
                            this.Coordinates[idx].Latitude != otherCH.Coordinates[idx].Latitude)
                        { // oeps, coordinates are different!
                            return false;
                        }
                    }
                    return true;
                }
                else
                { // both are null.
                    return otherCH.Coordinates == null;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the other edge represents the same geographical information than this edge.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool EqualsGeometrically(IGraphEdgeData other)
        {
            if (other is CHEdgeData)
            { // ok, type is the same.
                var otherCH = (CHEdgeData)other;

                // only the coordinates can be different now.
                if (this.Coordinates != null)
                { // both have to contain the same coordinates.
                    if (this.Coordinates.Length != otherCH.Coordinates.Length)
                    { // impossible, different number of coordinates.
                        return false;
                    }

                    for (int idx = 0; idx < otherCH.Coordinates.Length; idx++)
                    {
                        if (this.Coordinates[idx].Longitude != otherCH.Coordinates[idx].Longitude ||
                            this.Coordinates[idx].Latitude != otherCH.Coordinates[idx].Latitude)
                        { // oeps, coordinates are different!
                            return false;
                        }
                    }
                    return true;
                }
                else
                { // both are null.
                    return otherCH.Coordinates == null;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Comparer for CH edges.
    /// </summary>
    public class CHEdgeDataComparer : IDynamicGraphEdgeComparer<CHEdgeData>
    {
        /// <summary>
        /// Return true if the existence of edge1 makes edge2 useless.
        /// </summary>
        /// <param name="edge1"></param>
        /// <param name="edge2"></param>
        /// <returns></returns>
        public bool Overlaps(CHEdgeData edge1, CHEdgeData edge2)
        {
            if (edge1.ToHigher != edge2.ToHigher)
            { // the one is to a higher vertex, the other to a lower: edge1 can never overlap edge2.
                return false;
            }
            if (!edge1.Backward && edge2.Backward)
            { // the first edge does not have a backward flag: edge1 can never overlap edge2.
                return false;
            }
            if (!edge1.Forward && edge2.Forward)
            { // the first edge does not have a forward flag: edge1 can never overlap edge2.
                return false;
            }
            if (edge1.Weight > edge2.Weight)
            { // the first edge eight is larger than the second edge weight: edge1 can never overlap edge2
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Contains extensions related to the CHEdgeData.
    /// </summary>
    public static class CHExtensions
    {
        /// <summary>
        /// Removes all contracted edges.
        /// </summary>
        /// <param name="edges"></param>
        public static KeyValuePair<uint, CHEdgeData>[] KeepUncontracted(this KeyValuePair<uint, CHEdgeData>[] edges)
        {
            List<KeyValuePair<uint, CHEdgeData>> result = new List<KeyValuePair<uint, CHEdgeData>>();
            foreach (KeyValuePair<uint, CHEdgeData> edge in edges)
            {
                if (!edge.Value.HasContractedVertex)
                {
                    result.Add(edge);
                }
            }
            return result.ToArray();
        }

        ///// <summary>
        ///// Adds all downward edges.
        ///// </summary>
        ///// <param name="graph"></param>
        //public static void AddDownwardEdges(this IDynamicGraph<CHEdgeData> graph)
        //{ // add the reverse edges to get a easy depth-first search.
        //    for (uint vertexId = 1; vertexId < graph.VertexCount + 1; vertexId++)
        //    {
        //        List<KeyValuePair<uint, CHEdgeData>> arcs =
        //            new List<KeyValuePair<uint, CHEdgeData>>(graph.GetEdges(vertexId));
        //        foreach (KeyValuePair<uint, CHEdgeData> arc in arcs)
        //        {
        //            if (arc.Value.ToHigher)
        //            {
        //                // create severse edge.
        //                var reverseEdge = new CHEdgeData();
        //                reverseEdge.SetDirection(arc.Value.Backward, arc.Value.Forward, false);
        //                reverseEdge.Weight = arc.Value.Weight;

        //                graph.AddEdge(arc.Key, vertexId, reverseEdge, null);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Returns the arcs that point to higher vertices.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public static KeyValuePair<uint, CHEdgeData>[] GetArcsHigher(this IGraph<CHEdgeData> graph,
            uint vertexId)
        {
            KeyValuePair<uint, CHEdgeData>[] arcs = graph.GetEdges(vertexId);
            KeyValuePair<uint, CHEdgeData>[] higherArcs = new KeyValuePair<uint, CHEdgeData>[arcs.Length];
            int higherIdx = 0;
            for (int idx = 0; idx < arcs.Length; idx++)
            {
                if (arcs[idx].Value.ToHigher)
                {
                    higherArcs[higherIdx] = arcs[idx];
                    higherIdx++;
                }
            }
            Array.Resize(ref higherArcs, higherIdx);
            return higherArcs;
        }

        /// <summary>
        /// Returns the arcs that point to lower vertices.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public static KeyValuePair<uint, CHEdgeData>[] GetArcsLower(this IGraph<CHEdgeData> graph,
            uint vertexId)
        {
            KeyValuePair<uint, CHEdgeData>[] arcs = graph.GetEdges(vertexId);
            KeyValuePair<uint, CHEdgeData>[] higherArcs = new KeyValuePair<uint, CHEdgeData>[arcs.Length];
            int higherIdx = 0;
            for (int idx = 0; idx < arcs.Length; idx++)
            {
                if (!arcs[idx].Value.ToHigher)
                {
                    higherArcs[higherIdx] = arcs[idx];
                    higherIdx++;
                }
            }
            Array.Resize(ref higherArcs, higherIdx);
            return higherArcs;
        }
    }
}

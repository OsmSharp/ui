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

using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Vehicles;
namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// A router data source baseclass that wraps a graph but also contains other meta-data for routing.
    /// </summary>
    /// <typeparam name="TEdgeData"></typeparam>
    public abstract class RouterDataSourceBase<TEdgeData> : GraphBase<TEdgeData>, IRoutingAlgorithmData<TEdgeData>
        where TEdgeData : struct, IGraphEdgeData
    {

        public abstract bool SupportsProfile(Vehicle vehicle);

        public abstract void AddSupportedProfile(Vehicle vehicle);

        public abstract INeighbourEnumerator<TEdgeData> GetEdges(Math.Geo.GeoCoordinateBox box);

        public abstract System.Collections.Generic.IEnumerable<Edge<TEdgeData>> GetDirectNeighbours(uint vertex);

        public abstract Collections.Tags.Index.ITagsIndex TagsIndex
        {
            get;
        }

        public abstract void AddRestriction(uint[] route);

        public abstract void AddRestriction(string vehicleType, uint[] route);

        public abstract bool TryGetRestrictionAsStart(Vehicle vehicle, uint vertex, out System.Collections.Generic.List<uint[]> routes);

        public abstract bool TryGetRestrictionAsEnd(Vehicle vehicle, uint vertex, out System.Collections.Generic.List<uint[]> routes);

        public override abstract bool IsDirected
        {
            get;
        }

        public override abstract bool CanHaveDuplicates
        {
            get;
        }

        public override abstract bool GetVertex(uint id, out float latitude, out float longitude);

        public override abstract bool ContainsEdges(uint vertexId, uint neighbour);

        public override abstract bool ContainsEdge(uint vertexId, uint neighbour, TEdgeData data);

        public override abstract EdgeEnumerator<TEdgeData> GetEdgeEnumerator();

        public override abstract EdgeEnumerator<TEdgeData> GetEdges(uint vertexId);

        public override abstract EdgeEnumerator<TEdgeData> GetEdges(uint vertex1, uint vertex2);

        public override abstract bool GetEdge(uint vertex1, uint vertex2, out TEdgeData data);

        public override abstract bool GetEdgeShape(uint vertex1, uint vertex2, out Collections.Coordinates.Collections.ICoordinateCollection shape);

        public override abstract uint VertexCount
        {
            get;
        }
    }
}
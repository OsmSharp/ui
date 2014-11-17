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

using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Structures;
using OsmSharp.Math.Structures.QTree;
using OsmSharp.Routing.Graph.Router;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// A router data source that uses a IDynamicGraph as it's main datasource.
    /// </summary>
    /// <typeparam name="TEdgeData"></typeparam>
    public class DynamicGraphRouterDataSource<TEdgeData> : IDynamicGraphRouterDataSource<TEdgeData>
        where TEdgeData : IGraphEdgeData
    {
        /// <summary>
        /// Holds the basic graph.
        /// </summary>
        private readonly IGraph<TEdgeData> _graph;

        /// <summary>
        /// Holds the index of vertices per bounding box.
        /// </summary>
        private ILocatedObjectIndex<GeoCoordinate, uint> _vertexIndex;

        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private readonly ITagsCollectionIndexReadonly _tagsIndex;

        /// <summary>
        /// Holds the supported vehicle profiles.
        /// </summary>
        private readonly HashSet<Vehicle> _supportedVehicles;
        /// <summary>
        /// Creates a new osm memory router data source.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public DynamicGraphRouterDataSource(ITagsCollectionIndexReadonly tagsIndex)
        {
            if (tagsIndex == null) throw new ArgumentNullException("tagsIndex");

            _graph = new MemoryGraph<TEdgeData>();
            _vertexIndex = null; // do not create an index initially.
            _tagsIndex = tagsIndex;

            _supportedVehicles = new HashSet<Vehicle>();
        }

        /// <summary>
        /// Creates a new osm memory router data source.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public DynamicGraphRouterDataSource(ITagsCollectionIndexReadonly tagsIndex, int initSize)
        {
            if (tagsIndex == null) throw new ArgumentNullException("tagsIndex");

            _graph = new MemoryGraph<TEdgeData>(initSize);
            _vertexIndex = null; // do not create an index initially.
            _tagsIndex = tagsIndex;

            _supportedVehicles = new HashSet<Vehicle>();
        }

        /// <summary>
        /// Creates a new osm memory router data source.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="tagsIndex"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DynamicGraphRouterDataSource(IGraph<TEdgeData> graph, ITagsCollectionIndexReadonly tagsIndex)
        {
            if (graph == null) throw new ArgumentNullException("graph");
            if (tagsIndex == null) throw new ArgumentNullException("tagsIndex");

            _graph = graph;
            _vertexIndex = null; // do not create an index initially.
            _tagsIndex = tagsIndex;

            _supportedVehicles = new HashSet<Vehicle>();
        }

        /// <summary>
        /// Returns true if the given vehicle profile is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool SupportsProfile(Vehicle vehicle)
        {
            return _supportedVehicles.Contains(vehicle); // for backwards compatibility.
        }

        /// <summary>
        /// Adds one more supported profile.
        /// </summary>
        /// <param name="vehicle"></param>
        public void AddSupportedProfile(Vehicle vehicle)
        {
            _supportedVehicles.Add(vehicle);
        }

        /// <summary>
        /// Returns all supported profiles.
        /// </summary>
        public IEnumerable<Vehicle> GetSupportedProfiles()
        {
            return _supportedVehicles;
        }

        /// <summary>
        /// Deactivates the vertex index.
        /// </summary>
        public void DropVertexIndex()
        {
            _vertexIndex = null;
        }

        /// <summary>
        /// Rebuilds the vertex index.
        /// </summary>
        public void RebuildVertexIndex()
        {
            _vertexIndex = new QuadTree<GeoCoordinate, uint>();
            for (uint vertex = 0; vertex <= _graph.VertexCount; vertex++)
            {
                float latitude, longitude;
                if (_graph.GetVertex(vertex, out latitude, out longitude))
                {
                    _vertexIndex.Add(new GeoCoordinate(latitude, longitude),
                        vertex);
                }
            }
        }


        /// <summary>
        /// Returns all arcs inside the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public KeyValuePair<uint, KeyValuePair<uint, TEdgeData>>[] GetEdges(
            GeoCoordinateBox box)
        {
            if (_vertexIndex == null) {
                // rebuild on-the-fly.
                this.RebuildVertexIndex();
            }

            // get all the vertices in the given box.
            var vertices = _vertexIndex.GetInside(
                box);

            // loop over all vertices and get the arcs.
            var arcs = new List<KeyValuePair<uint, KeyValuePair<uint, TEdgeData>>>();
            foreach (uint vertex in vertices)
            {
                var localArcs = this.GetEdges(vertex);
                foreach (var localArc in localArcs)
                {
                    arcs.Add(new KeyValuePair<uint, KeyValuePair<uint, TEdgeData>>(
                        vertex, new KeyValuePair<uint, TEdgeData>(localArc.Neighbour, localArc.EdgeData)));
                }
            }
            return arcs.ToArray();
        }

        /// <summary>
        /// Returns true if a given vertex is in the graph.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool GetVertex(uint id, out float latitude, out float longitude)
        {
            return _graph.GetVertex(id, out latitude, out longitude);
        }

        /// <summary>
        /// Returns all arcs starting at a given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public IEdgeEnumerator<TEdgeData> GetEdges(uint vertexId)
        {
            return _graph.GetEdges(vertexId);
        }

        /// <summary>
        /// Returns true if the given vertex has neighbour as a neighbour.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public bool ContainsEdge(uint vertexId, uint neighbour)
        {
            return _graph.ContainsEdge(vertexId, neighbour);
        }

        /// <summary>
        /// Returns true if the given vertex has the given neighbour.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool GetEdge(uint vertex1, uint vertex2, out TEdgeData data)
        {
            return _graph.GetEdge(vertex1, vertex2, out data);
        }

        /// <summary>
        /// Returns true if the given vertex has the given neighbour.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool GetEdgeShape(uint vertex1, uint vertex2, out ICoordinateCollection shape)
        {
            return _graph.GetEdgeShape(vertex1, vertex2, out shape);
        }

        /// <summary>
        /// Adds a new vertex.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public uint AddVertex(float latitude, float longitude)
        {
            uint vertex = _graph.AddVertex(latitude, longitude);
            if (_vertexIndex != null)
            {
                _vertexIndex.Add(new GeoCoordinate(latitude, longitude),
                    vertex);
            }
            return vertex;
        }

        /// <summary>
        /// Sets a vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void SetVertex(uint vertex, float latitude, float longitude)
        {
            _graph.SetVertex(vertex, latitude, longitude);
        }

        /// <summary>
        /// Adds a new edge.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        public void AddEdge(uint vertex1, uint vertex2, TEdgeData data)
        {
            _graph.AddEdge(vertex1, vertex2, data, null);
        }

        /// <summary>
        /// Adds a new edge.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <param name="coordinates"></param>
        public void AddEdge(uint vertex1, uint vertex2, TEdgeData data, ICoordinateCollection coordinates)
        {
            _graph.AddEdge(vertex1, vertex2, data, coordinates);
        }

        /// <summary>
        /// Adds a new edge.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <param name="coordinates"></param>
        /// <param name="comparer"></param>
        public void AddEdge(uint vertex1, uint vertex2, TEdgeData data, ICoordinateCollection coordinates, IDynamicGraphEdgeComparer<TEdgeData> comparer)
        {
            _graph.AddEdge(vertex1, vertex2, data, coordinates, comparer);
        }

        /// <summary>
        /// Removes all arcs starting at vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void RemoveEdges(uint vertex)
        {
            _graph.RemoveEdges(vertex);
        }

        /// <summary>
        /// Deletes an arc.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void RemoveEdge(uint from, uint to)
        {
            _graph.RemoveEdge(from, to);
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        public ITagsCollectionIndexReadonly TagsIndex
        {
            get
            {
                return _tagsIndex;
            }
        }

        /// <summary>
        /// Compresses the internal of the graph, freeing new space.
        /// </summary>
        public void Compress()
        {
            _graph.Compress();

            // rebuild index.
            if (_vertexIndex != null)
            {
                float latitude, longitude;
                _vertexIndex.Clear();
                for (uint idx = 0; idx < _graph.VertexCount; idx++)
                {
                    if (_graph.GetVertex(idx, out latitude, out longitude))
                    {
                        _vertexIndex.Add(new GeoCoordinate(latitude, longitude), idx);
                    }
                }
            }
        }

        /// <summary>
        /// Trims all internal data structures.
        /// </summary>
        public void Trim()
        {
            _graph.Trim();
        }

        /// <summary>
        /// Resizes the internal data structures of the graph to handle the number of vertices/edges estimated.
        /// </summary>
        /// <param name="vertexEstimate"></param>
        /// <param name="edgeEstimate"></param>
        public void Resize(long vertexEstimate, long edgeEstimate)
        {
            _graph.Resize(vertexEstimate, edgeEstimate);
        }

        /// <summary>
        /// Returns the number of vertices in this graph.
        /// </summary>
        public uint VertexCount
        {
            get { return _graph.VertexCount; }
        }

        #region Restriction

        /// <summary>
        /// Holds the restricted routes that apply to all vehicles.
        /// </summary>
        private Dictionary<uint, List<uint[]>> _restrictedRoutes;

        /// <summary>
        /// Holds the restricted routes that apply to one vehicle profile.
        /// </summary>
        private Dictionary<string, Dictionary<uint, List<uint[]>>> _restricedRoutesPerVehicle;

        /// <summary>
        /// Adds a restriction to this graph by prohibiting the given route.
        /// </summary>
        /// <param name="route"></param>
        public void AddRestriction(uint[] route)
        {
            if (route == null) { throw new ArgumentNullException(); }
            if (route.Length == 0) { throw new ArgumentOutOfRangeException("Restricted route has to contain at least one vertex."); }

            if (_restrictedRoutes == null)
            { // create dictionary.
                _restrictedRoutes = new Dictionary<uint, List<uint[]>>();
            }
            List<uint[]> routes;
            if (!_restrictedRoutes.TryGetValue(route[0], out routes))
            {
                routes = new List<uint[]>();
                _restrictedRoutes.Add(route[0], routes);
            }
            routes.Add(route);
        }

        /// <summary>
        /// Adds a restriction to this graph by prohibiting the given route for the given vehicle.
        /// </summary>
        /// <param name="vehicleType"></param>
        /// <param name="route"></param>
        public void AddRestriction(string vehicleType, uint[] route)
        {
            if (route == null) { throw new ArgumentNullException(); }
            if (route.Length == 0) { throw new ArgumentOutOfRangeException("Restricted route has to contain at least one vertex."); }

            if (_restricedRoutesPerVehicle == null)
            { // create dictionary.
                _restricedRoutesPerVehicle = new Dictionary<string, Dictionary<uint, List<uint[]>>>();
            }
            Dictionary<uint, List<uint[]>> restrictedRoutes;
            if (!_restricedRoutesPerVehicle.TryGetValue(vehicleType, out restrictedRoutes))
            { // the vehicle does not have any restrictions yet.
                restrictedRoutes = new Dictionary<uint, List<uint[]>>();
                _restricedRoutesPerVehicle.Add(vehicleType, restrictedRoutes);
            }
            List<uint[]> routes;
            if (!restrictedRoutes.TryGetValue(route[0], out routes))
            {
                routes = new List<uint[]>();
                restrictedRoutes.Add(route[0], routes);
            }
            routes.Add(route);
        }

        /// <summary>
        /// Returns all restricted routes that start in the given vertex.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="vertex"></param>
        /// <param name="routes"></param>
        /// <returns></returns>
        public bool TryGetRestrictionAsStart(Vehicle vehicle, uint vertex, out List<uint[]> routes)
        {
            Dictionary<uint, List<uint[]>> restrictedRoutes;
            routes = null;
            foreach (var vehicleType in vehicle.VehicleTypes)
            {
                List<uint[]> routesPerVehicle;
                if (_restricedRoutesPerVehicle != null &&
                    _restricedRoutesPerVehicle.TryGetValue(vehicleType, out restrictedRoutes) &&
                    restrictedRoutes.TryGetValue(vertex, out routesPerVehicle))
                {
                    routes = routesPerVehicle;
                }
                List<uint[]> routesAll;
                if (_restrictedRoutes != null &&
                    _restrictedRoutes.TryGetValue(vertex, out routesAll))
                {
                    if (routes == null)
                    {
                        routes = routesAll;
                    }
                    else
                    {
                        routes.AddRange(routesAll);
                    }
                }
                if(routes != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if there is a restriction that ends with the given vertex.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="vertex"></param>
        /// <param name="routes"></param>
        /// <returns></returns>
        public bool TryGetRestrictionAsEnd(Vehicle vehicle, uint vertex, out List<uint[]> routes)
        {
            routes = null;
            return false;
        }

        #endregion
    }
}
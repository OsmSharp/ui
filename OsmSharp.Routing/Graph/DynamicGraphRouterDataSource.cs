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

using System;
using System.Collections.Generic;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Structures;
using OsmSharp.Math.Structures.QTree;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Collections.Tags.Index;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// A router data source that uses a IDynamicGraph as it's main datasource.
    /// </summary>
    /// <typeparam name="TEdgeData"></typeparam>
    public class DynamicGraphRouterDataSource<TEdgeData> : IDynamicGraphRouterDataSource<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the basic graph.
        /// </summary>
        private readonly IDynamicGraph<TEdgeData> _graph;

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

            _graph = new MemoryDynamicGraph<TEdgeData>();
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

            _graph = new MemoryDynamicGraph<TEdgeData>(initSize);
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
        public DynamicGraphRouterDataSource(IDynamicGraph<TEdgeData> graph, ITagsCollectionIndexReadonly tagsIndex)
        {
            if (graph == null) throw new ArgumentNullException("graph");
            if (tagsIndex == null) throw new ArgumentNullException("tagsIndex");

            _graph = graph;
            _vertexIndex = null; // do not create an index initially.
            _tagsIndex = tagsIndex;

            _supportedVehicles = new HashSet<Vehicle>();

            // add the current graph's vertices to the vertex index.
            for (uint newVertexId = 1; newVertexId < graph.VertexCount + 1; newVertexId++)
            {
                // add to the CHRegions.
                float latitude, longitude;
                graph.GetVertex(newVertexId, out latitude, out longitude);
                _vertexIndex.Add(new GeoCoordinate(latitude, longitude), newVertexId);
            }
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
        public KeyValuePair<uint, KeyValuePair<uint, TEdgeData>>[] GetArcs(
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
                var localArcs = this.GetArcs(vertex);
                foreach (var localArc in localArcs)
                {
                    arcs.Add(new KeyValuePair<uint, KeyValuePair<uint, TEdgeData>>(
                        vertex, localArc));
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
        public KeyValuePair<uint, TEdgeData>[] GetArcs(uint vertexId)
        {
            return _graph.GetArcs(vertexId);
        }

        /// <summary>
        /// Returns true if the given vertex has neighbour as a neighbour.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public bool HasArc(uint vertexId, uint neighbour)
        {
            return _graph.HasArc(vertexId, neighbour);
        }

        /// <summary>
        /// Adds a new vertex to this graph.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="neighboursEstimate"></param>
        /// <returns></returns>
        public uint AddVertex(float latitude, float longitude, byte neighboursEstimate)
        {
            uint vertex = _graph.AddVertex(latitude, longitude, neighboursEstimate);
            if (_vertexIndex != null)
            {
                _vertexIndex.Add(new GeoCoordinate(latitude, longitude),
                    vertex);
            }
            return vertex;
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
        /// Adds a new arc.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        /// <param name="comparer"></param>
        public void AddArc(uint from, uint to, TEdgeData data, IDynamicGraphEdgeComparer<TEdgeData> comparer)
        {
            _graph.AddArc(from, to, data, comparer);
        }

        /// <summary>
        /// Removes all arcs starting at vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void DeleteArc(uint vertex)
        {
            _graph.DeleteArc(vertex);
        }

        /// <summary>
        /// Deletes an arc.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void DeleteArc(uint from, uint to)
        {
            _graph.DeleteArc(from, to);
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
        /// Trims this graph.
        /// </summary>
        /// <param name="max"></param>
        public void Trim(uint vertices)
        {
            _graph.Trim(vertices);

            // rebuild index.
            if (_vertexIndex != null)
            {
                float latitude, longitude;
                _vertexIndex.Clear();
                for (uint idx = 0; idx < vertices; idx++)
                {
                    if (_graph.GetVertex(idx, out latitude, out longitude))
                    {
                        _vertexIndex.Add(new GeoCoordinate(latitude, longitude), idx);
                    }
                }
            }
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
        private Dictionary<Vehicle, Dictionary<uint, List<uint[]>>> _restricedRoutesPerVehicle;

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
        /// <param name="vehicle"></param>
        /// <param name="route"></param>
        public void AddRestriction(Vehicle vehicle, uint[] route)
        {
            if (route == null) { throw new ArgumentNullException(); }
            if (route.Length == 0) { throw new ArgumentOutOfRangeException("Restricted route has to contain at least one vertex."); }

            if (_restricedRoutesPerVehicle == null)
            { // create dictionary.
                _restricedRoutesPerVehicle = new Dictionary<Vehicle, Dictionary<uint, List<uint[]>>>();
            }
            Dictionary<uint, List<uint[]>> restrictedRoutes;
            if (!_restricedRoutesPerVehicle.TryGetValue(vehicle, out restrictedRoutes))
            { // the vehicle does not have any restrictions yet.
                restrictedRoutes = new Dictionary<uint, List<uint[]>>();
                _restricedRoutesPerVehicle.Add(vehicle, restrictedRoutes);
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
            return _restricedRoutesPerVehicle.TryGetValue(vehicle, out restrictedRoutes) &&
                restrictedRoutes.TryGetValue(vertex, out routes);
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
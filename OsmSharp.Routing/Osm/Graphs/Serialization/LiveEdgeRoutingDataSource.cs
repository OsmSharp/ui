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

using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Math.Structures;
using OsmSharp.Math.Structures.QTree;
using OsmSharp.Osm.Tiles;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Osm.Graphs.Serialization
{
    /// <summary>
    /// A router data source that dynamically loads data.
    /// </summary>
    internal class RouterLiveEdgeDataSource : IBasicRouterDataSource<LiveEdge>
    {
        /// <summary>
        /// Holds all graph data.
        /// </summary>
        private readonly SparseArray<Vertex> _vertices;

        /// <summary>
        /// Holds the coordinates of the vertices.
        /// </summary>
        private readonly SparseArray<Location> _coordinates;

        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private readonly ITagsCollectionIndex _tagsIndex;

        /// <summary>
        /// Holds the vertex index.
        /// </summary>
        private readonly ILocatedObjectIndex<GeoCoordinate, uint> _vertexIndex;

        /// <summary>
        /// Holds the list of vehicles.
        /// </summary>
        private readonly HashSet<string> _vehicles;

        /// <summary>
        /// Creates a new router data source.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="compressed"></param>
        /// <param name="tileMetas"></param>
        /// <param name="zoom"></param>
        /// <param name="v1RoutingDataSourceSerializer"></param>
        /// <param name="vehicles"></param>
        /// <param name="initialCapacity"></param>
        internal RouterLiveEdgeDataSource(
            Stream stream, bool compressed,
            RoutingDataSourceLiveEdgeSerializer.SerializableGraphTileMetas tileMetas,
            int zoom, RoutingDataSourceLiveEdgeSerializer v1RoutingDataSourceSerializer,
            IEnumerable<string> vehicles,
            int initialCapacity = 1000)
        {
            _tagsIndex = new TagsTableCollectionIndex();
            _vertices = new SparseArray<Vertex>(initialCapacity);
            _coordinates = new SparseArray<Location>(initialCapacity);
            _vehicles = new HashSet<string>(vehicles);

            _vertexIndex = new QuadTree<GeoCoordinate, uint>();

            _graphTileMetas = new Dictionary<Tile, TileStreamPosition>();
            for (int tileIdx = 0; tileIdx < tileMetas.TileX.Length; tileIdx++)
            {
                _graphTileMetas.Add(
                    new Tile(tileMetas.TileX[tileIdx], tileMetas.TileY[tileIdx], zoom),
                    new TileStreamPosition()
                        {
                            Offset = tileMetas.Offset[tileIdx],
                            Length = tileMetas.Length[tileIdx]
                        });
            }

            _loadedTiles = new HashSet<Tile>();
            _tilesPerVertex = new Dictionary<uint, Tile>();
            _zoom = zoom;
            _routingDataSourceSerializer = v1RoutingDataSourceSerializer;
            _stream = stream;
            _compressed = compressed;
        }

        /// <summary>
        /// Returns true if the given profile is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool SupportsProfile(Vehicle vehicle)
        {
            return _vehicles.Contains(vehicle.UniqueName);
        }

        /// <summary>
        /// Returns true if the given profile is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public void AddSupportedProfile(Vehicle vehicle)
        {
            throw new InvalidOperationException("Cannot add extra vehicle profiles to a read-only source.");
        }

        /// <summary>
        /// Returns all edges inside the given boundingbox.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public KeyValuePair<uint, KeyValuePair<uint, Osm.Graphs.LiveEdge>>[] GetEdges(
            GeoCoordinateBox box)
        {
            // load the missing tiles.
            this.LoadMissingTile(box);

            // get all the vertices in the given box.
            IEnumerable<uint> vertices = _vertexIndex.GetInside(
                box);

            // loop over all vertices and get the arcs.
            var arcs = new List<KeyValuePair<uint, KeyValuePair<uint, Osm.Graphs.LiveEdge>>>();
            foreach (uint vertexId in vertices)
            {
                var location = _coordinates[(int)vertexId];
                if (location != null)
                {
                    // load tile if needed.
                    this.LoadMissingTile(new GeoCoordinate(
                        location.Latitude, location.Longitude));

                    // get the arcs and return.
                    if (_vertices.Length > vertexId)
                    {
                        var vertex = _vertices[(int)vertexId];
                        if (vertex != null &&
                            vertex.Arcs != null)
                        {
                            var localArcs = vertex.Arcs;
                            foreach (var localArc in localArcs)
                            {
                                arcs.Add(new KeyValuePair<uint, KeyValuePair<uint, Osm.Graphs.LiveEdge>>(
                                    vertexId, new KeyValuePair<uint, Osm.Graphs.LiveEdge>(localArc.Item1, localArc.Item2)));
                            }
                        }
                    }
                }
            }
            return arcs.ToArray();
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        public ITagsCollectionIndexReadonly TagsIndex
        {
            get { return _tagsIndex; }
        }

        /// <summary>
        /// Returns the location of the vertex with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool GetVertex(uint id, out float latitude, out float longitude)
        {
            Tile tile;
            if (_tilesPerVertex.TryGetValue(id, out tile))
            {
                // load missing tile if needed.
                this.LoadMissingTile(tile);
                _tilesPerVertex.Remove(id);
            }

            if (id > 0 && _vertices.Length > id)
            {
                Location location = _coordinates[(int)id];
                if (location != null)
                {
                    latitude = location.Latitude;
                    longitude = location.Longitude;
                    return true;
                }
            }
            latitude = float.MaxValue;
            longitude = float.MaxValue;
            return false;
        }

        /// <summary>
        /// Returns all vertices in this router data source.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<uint> GetVertices()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns all arcs for the given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public IEdgeEnumerator<LiveEdge> GetEdges(uint vertexId)
        {
            Tile tile;
            if (_tilesPerVertex.TryGetValue(vertexId, out tile))
            {
                // load missing tile if needed.
                this.LoadMissingTile(tile);
                _tilesPerVertex.Remove(vertexId);
            }

            // get the arcs and return.
            if (_vertices.Length > vertexId)
            {
                var vertex = _vertices[(int)vertexId];
                if (vertex != null &&
                    vertex.Arcs != null)
                {
                    var arcs = new KeyValuePair<uint, Osm.Graphs.LiveEdge>[vertex.Arcs.Length];
                    for(int idx = 0; idx< vertex.Arcs.Length; idx++)
                    {
                        arcs[idx] = new KeyValuePair<uint, LiveEdge>(
                            vertex.Arcs[idx].Item1, vertex.Arcs[idx].Item2);
                    }
                    return arcs;
                }
            }
            return new KeyValuePair<uint, LiveEdge>[0];
        }

        /// <summary>
        /// Returns true if the given vertex has the given neighbour.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public bool ContainsEdge(uint vertexId, uint neighbour)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the given vertex has the given neighbour.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool GetEdge(uint vertex1, uint vertex2, out LiveEdge data)
        {
            Tile tile;
            if (_tilesPerVertex.TryGetValue(vertex1, out tile))
            {
                // load missing tile if needed.
                this.LoadMissingTile(tile);
                _tilesPerVertex.Remove(vertex1);
            }

            // get the arcs and return.
            if (_vertices.Length > vertex1)
            {
                var vertex = _vertices[(int)vertex1];
                if (vertex != null &&
                    vertex.Arcs != null)
                {
                    for (int idx = 0; idx < vertex.Arcs.Length; idx++)
                    {
                        if (vertex.Arcs[idx].Item1 == vertex2)
                        {
                            data = vertex.Arcs[idx].Item2;
                            return true;
                        }
                    }
                }
            }
            data = default(LiveEdge);
            return false;
        }

        /// <summary>
        /// Returns true if the given vertex has the given neighbour.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool GetEdgeShape(uint vertex1, uint vertex2, out GeoCoordinateSimple[] shape)
        {
            Tile tile;
            if (_tilesPerVertex.TryGetValue(vertex1, out tile))
            {
                // load missing tile if needed.
                this.LoadMissingTile(tile);
                _tilesPerVertex.Remove(vertex1);
            }

            // get the arcs and return.
            if (_vertices.Length > vertex1)
            {
                var vertex = _vertices[(int)vertex1];
                if (vertex != null &&
                    vertex.Arcs != null)
                {
                    for (int idx = 0; idx < vertex.Arcs.Length; idx++)
                    {
                        if(vertex.Arcs[idx].Item1 == vertex2)
                        {
                            shape = vertex.Arcs[idx].Item3;
                            return true;
                        }
                    }
                }
            }
            shape = null;
            return false;
        }

        /// <summary>
        /// Returns the vertex count.
        /// </summary>
        public uint VertexCount
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Represents a simple vertex.
        /// </summary>
        internal class Vertex
        {
            /// <summary>
            /// Holds an array of edges starting at this vertex.
            /// </summary>
            public Tuple<uint, LiveEdge, GeoCoordinateSimple[]>[] Arcs;
        }

        /// <summary>
        /// Represents the location.
        /// </summary>
        internal class Location
        {
            /// <summary>
            /// Gets/sets the latitude.
            /// </summary>
            public float Latitude { get; set; }

            /// <summary>
            /// Gets/sets the longitude.
            /// </summary>
            public float Longitude { get; set; }
        }

        #region Dynamic Tile Loading

        /// <summary>
        /// Holds the stream containing the graph data.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Holds flag indicating that the data in the stream is compressed.
        /// </summary>
        private bool _compressed;

        /// <summary>
        /// Holds the routing serializer.
        /// </summary>
        private readonly RoutingDataSourceLiveEdgeSerializer _routingDataSourceSerializer;

        /// <summary>
        /// Holds the tile metas.
        /// </summary>
        private readonly Dictionary<Tile, TileStreamPosition> _graphTileMetas;

        /// <summary>
        /// Holds the loaded tiles.
        /// </summary>
        private readonly HashSet<Tile> _loadedTiles;

        /// <summary>
        /// Holds the tile to get the current vertex.
        /// </summary>
        private readonly Dictionary<uint, Tile> _tilesPerVertex;

        /// <summary>
        /// The zoom level of the cached tiles.
        /// </summary>
        private readonly int _zoom;

        /// <summary>
        /// A tile stream position.
        /// </summary>
        private class TileStreamPosition
        {
            /// <summary>
            /// Gets/sets the offset.
            /// </summary>
            public long Offset { get; set; }

            /// <summary>
            /// Gets/sets the length.
            /// </summary>
            public int Length { get; set; }
        }
        /// <summary>
        /// Resize if needed.
        /// </summary>
        /// <param name="size"></param>
        private void Resize(uint size)
        {
            if (_coordinates.Length < size)
            {
                _coordinates.Resize((int)size); // increasing a sparse array size is very cheap.
                _vertices.Resize((int)size); // increasing a sparse array size is very cheap.
            }
        }

        /// <summary>
        /// Loads all missing tiles.
        /// </summary>
        /// <param name="box"></param>
        private void LoadMissingTile(GeoCoordinateBox box)
        {
            // creates a tile range.
            TileRange tileRange = TileRange.CreateAroundBoundingBox(box, _zoom);
            foreach (var tile in tileRange)
            {
                this.LoadMissingTile(tile);
            }
        }

        /// <summary>
        /// Loads the missing tile at the given coordinate.
        /// </summary>
        /// <param name="coordinate"></param>
        private void LoadMissingTile(GeoCoordinate coordinate)
        {
            this.LoadMissingTile(Tile.CreateAroundLocation(coordinate, _zoom));
        }

        /// <summary>
        /// Loads the missing tiles.
        /// </summary>
        /// <param name="tile"></param>
        internal void LoadMissingTile(Tile tile)
        {
            if (!_loadedTiles.Contains(tile))
            { // the tile was not loaded yet.
                TileStreamPosition meta;
                if (_graphTileMetas.TryGetValue(tile, out meta))
                { // the meta data is available.
                    var tileData = _routingDataSourceSerializer.DeserializeTile(_stream, meta.Offset, meta.Length, _compressed);
                    double top = tile.TopLeft.Latitude;
                    double left = tile.TopLeft.Longitude;
                    for(int vertexIdx = 0; vertexIdx < tileData.Ids.Length; vertexIdx++)
                    {
                        // resize.
                        this.Resize(tileData.Ids[vertexIdx] + 1);

                        // create the location and calculate lat/lon.
                        var vertexLocation = new Location();
                        vertexLocation.Latitude = (float)(top - (((double)tileData.Latitude[vertexIdx] / (double)ushort.MaxValue) 
                            * tile.Box.DeltaLat));
                        vertexLocation.Longitude = (float)((((double)tileData.Longitude[vertexIdx] / (double)ushort.MaxValue)
                            * tile.Box.DeltaLon) +
                                          left);
                        _coordinates[(int)tileData.Ids[vertexIdx]] = vertexLocation;

                        // convert the arcs.
                        if (tileData.Arcs[vertexIdx] != null)
                        {
                            var arcs = new Tuple<uint, Osm.Graphs.LiveEdge, GeoCoordinateSimple[]>[tileData.Arcs[vertexIdx].DestinationId.Length];
                            for (int idx = 0; idx < tileData.Arcs[vertexIdx].DestinationId.Length; idx++)
                            {
                                // create the tags collection.
                                var tagsCollection = new TagsCollection();
                                for (int tagsIdx = 0;
                                     tagsIdx < tileData.Arcs[vertexIdx].Tags[idx].Keys.Length;
                                     tagsIdx++)
                                {
                                    string key = tileData.StringTable[tileData.Arcs[vertexIdx].Tags[idx].Keys[tagsIdx]];
                                    string value = tileData.StringTable[tileData.Arcs[vertexIdx].Tags[idx].Values[tagsIdx]];

                                    tagsCollection.Add(key, value);
                                }
                                uint tags = _tagsIndex.Add(tagsCollection);

                                // create the liveedge.
                                var edge = new Osm.Graphs.LiveEdge();
                                edge.Forward = tileData.Arcs[vertexIdx].Forward[idx];
                                edge.Tags = tags;
                                var coordinates = RoutingDataSourceLiveEdgeSerializer.SerializableCoordinate.ToSimpleArray(
                                    tileData.Arcs[vertexIdx].Intermediates[idx].Coordinates);
                                edge.Distance = tileData.Arcs[vertexIdx].Distances[idx];

                                // convert the arc.
                                arcs[idx] = new Tuple<uint, Osm.Graphs.LiveEdge, GeoCoordinateSimple[]>(
                                    tileData.Arcs[vertexIdx].DestinationId[idx], edge, coordinates);

                                // store the target tile.
                                var targetTile = new Tile(tileData.Arcs[vertexIdx].TileX[idx],
                                                           tileData.Arcs[vertexIdx].TileY[idx], _zoom);
                                if (!targetTile.Equals(tile) && !_loadedTiles.Contains(targetTile))
                                {
                                    _tilesPerVertex[tileData.Arcs[vertexIdx].DestinationId[idx]] = targetTile;
                                }
                            }
                            _vertices[(int)tileData.Ids[vertexIdx]] = new Vertex()
                            {
                                Arcs = arcs
                            };
                        }
                        _vertexIndex.Add(new GeoCoordinate(vertexLocation.Latitude,
                            vertexLocation.Longitude), tileData.Ids[vertexIdx]);
                    }
                }

                _loadedTiles.Add(tile); // tile is loaded.
            }
        }

        #endregion


        public void AddRestriction(uint[] route)
        {
            throw new NotImplementedException();
        }

        public void AddRestriction(Vehicle vehicle, uint[] route)
        {
            throw new NotImplementedException();
        }

        public bool TryGetRestrictionAsStart(Vehicle vehicle, uint vertex, out List<uint[]> routes)
        {
            throw new NotImplementedException();
        }

        public bool TryGetRestrictionAsEnd(Vehicle vehicle, uint vertex, out List<uint[]> routes)
        {
            throw new NotImplementedException();
        }
    }
}
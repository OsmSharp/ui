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
using OsmSharp.Collections.Coordinates.Collections;
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
        public INeighbourEnumerator<LiveEdge> GetEdges(
            GeoCoordinateBox box)
        {
            // load the missing tiles.
            this.LoadMissingTile(box);

            // get all the vertices in the given box.
            var vertices = _vertexIndex.GetInside(box);

            // loop over all vertices and get the arcs.
            var neighbours = new List<Tuple<uint, uint, uint, LiveEdge>>();
            foreach (uint vertexId in vertices)
            {
                var location = _coordinates[(int)vertexId];
                if (location != null)
                {
                    //// load tile if needed.
                    //this.LoadMissingTile(new GeoCoordinate(
                    //    location.Latitude, location.Longitude));

                    // get the arcs and return.
                    if (_vertices.Length > vertexId)
                    {
                        var vertex = _vertices[(int)vertexId];
                        if (vertex != null &&
                            vertex.Arcs != null)
                        {
                            var localArcs = vertex.Arcs;
                            for (int arcIdx = 0; arcIdx < vertex.Arcs.Length; arcIdx++)
                            {
                                neighbours.Add(new Tuple<uint, uint, uint, LiveEdge>(vertexId, (uint)arcIdx, localArcs[arcIdx].Item1, localArcs[arcIdx].Item2));
                            }
                        }
                    }
                }
            }
            return new NeighbourEnumerator(this, neighbours);
        }

        /// <summary>
        /// Gets the shape for the arc in from the given vertex and at the given index.
        /// </summary>
        /// <param name="vertexId">The vertex id.</param>
        /// <param name="arcIdx">The index of the arc.</param>
        /// <param name="shape">The shape.</param>
        /// <returns>True if there was a shape.</returns>
        private bool GetShapeForArc(uint vertexId, uint arcIdx, out ICoordinateCollection shape)
        {
            var vertex = _vertices[(int)vertexId];
            var arcCoordinates = vertex.Arcs[arcIdx];
            shape = null;
            if (arcCoordinates.Item3 != null)
            {
                shape = new CoordinateArrayCollection<GeoCoordinateSimple>(arcCoordinates.Item3);
            }
            return true;
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
        /// Returns an empty enumerator.
        /// </summary>
        /// <returns></returns>
        public EdgeEnumerator<LiveEdge> GetEdgeEnumerator()
        {
            return new RouterLiveEdgeDataSource.EdgeEnumerator(this);
        }

        /// <summary>
        /// Returns all edges adjacent to the given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public EdgeEnumerator<LiveEdge> GetEdges(uint vertexId)
        {
            var enumerator = new RouterLiveEdgeDataSource.EdgeEnumerator(this);
            enumerator.MoveTo(vertexId);
            return enumerator;
        }

        /// <summary>
        /// Returns true if the given vertex has the given neighbour.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public EdgeEnumerator<LiveEdge> GetEdges(uint vertex1, uint vertex2)
        {
            var enumerator = new RouterLiveEdgeDataSource.EdgeEnumerator(this);
            enumerator.MoveTo(vertex1, vertex2);
            return enumerator;
        }

        /// <summary>
        /// Returns all edges adjacent to the given vertex in an array for key-value pairs.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        private KeyValuePair<uint, LiveEdge>[] GetEdgesFor(uint vertexId)
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
                    for (int idx = 0; idx < vertex.Arcs.Length; idx++)
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
        /// Returns all edges adjacent to the given vertex1 in an array for key-value pairs but only those that lead to vertex2.
        /// </summary>
        /// <param name="vertexId"></param>
        private KeyValuePair<uint, LiveEdge>[] GetEdgesFor(uint vertex1, uint vertex2)
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
                            return new KeyValuePair<uint, LiveEdge>[]{
                                new KeyValuePair<uint, LiveEdge>(vertex.Arcs[idx].Item1, vertex.Arcs[idx].Item2)
                            };
                        }
                    }
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
        public bool ContainsEdges(uint vertexId, uint neighbour)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the given vertex has the given neighbour.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ContainsEdge(uint vertexId, uint neighbour, LiveEdge data)
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
        public bool GetEdgeShape(uint vertex1, uint vertex2, out ICoordinateCollection shape)
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
                            shape = null;
                            if (vertex.Arcs[idx].Item3 != null)
                            {
                                shape = new CoordinateArrayCollection<GeoCoordinateSimple>(vertex.Arcs[idx].Item3);
                            }
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

        /// <summary>
        /// An edge enumerator.
        /// </summary>
        private class EdgeEnumerator : EdgeEnumerator<LiveEdge>
        {
            /// <summary>
            /// Holds the edges.
            /// </summary>
            private KeyValuePair<uint, LiveEdge>[] _edges = null;

            /// <summary>
            /// Holds the vertex currently being enumerated.
            /// </summary>
            private uint _vertex1;

            /// <summary>
            /// Holds the neighbour.
            /// </summary>
            private uint _vertex2;

            /// <summary>
            /// Holds the source-graph.
            /// </summary>
            private RouterLiveEdgeDataSource _graph;

            /// <summary>
            /// Holds the current position.
            /// </summary>
            private int _current = -1;

            /// <summary>
            /// Creates a new enumerator.
            /// </summary>
            /// <param name="graph"></param>
            public EdgeEnumerator(RouterLiveEdgeDataSource graph)
            {
                _graph = graph;
                _edges = null;
                _vertex1 = 0;
                _vertex2 = 0;
            }

            /// <summary>
            /// Moves to the next edge.
            /// </summary>
            /// <returns></returns>
            public override bool MoveNext()
            {
                _current++;
                return _edges.Length > _current;
            }

            /// <summary>
            /// Returns the current neighbour.
            /// </summary>
            public override uint Neighbour
            {
                get { return _edges[_current].Key; }
            }

            /// <summary>
            /// Returns the current edge data.
            /// </summary>
            public override LiveEdge EdgeData
            {
                get { return _edges[_current].Value; }
            }

            /// <summary>
            /// Returns true if the edge data is inverted by default.
            /// </summary>
            public override bool isInverted
            {
                get { return false; }
            }

            /// <summary>
            /// Returns the inverted edge data.
            /// </summary>
            public override LiveEdge InvertedEdgeData
            {
                get { return (LiveEdge)this.EdgeData.Reverse(); }
            }

            /// <summary>
            /// Returns the current intermediates.
            /// </summary>
            public override ICoordinateCollection Intermediates
            {
                get { return null; }
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public override void Reset()
            {
                _current = -1;
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            public override IEnumerator<Edge<LiveEdge>> GetEnumerator()
            {
                this.Reset();
                return this;
            }

            /// <summary>
            /// Returns the current edge.
            /// </summary>
            public override Edge<LiveEdge> Current
            {
                get { return new Edge<LiveEdge>(this); }
            }

            /// <summary>
            /// Returns true if the count is known without enumeration.
            /// </summary>
            public override bool HasCount
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Returns the count if known.
            /// </summary>
            public override int Count
            {
                get
                {
                    return _edges.Length;
                }
            }

            /// <summary>
            /// Dipose of all resources associated with this enumerable.
            /// </summary>
            public override void Dispose()
            {

            }

            /// <summary>
            /// Moves this enumerator to the given vertex.
            /// </summary>
            /// <param name="vertex"></param>
            public override void MoveTo(uint vertex)
            {
                _edges = _graph.GetEdgesFor(vertex);
                this.Reset();
            }

            /// <summary>
            /// Moves this enumerator to the given vertex1 and enumerate only edges that lead to vertex2.
            /// </summary>
            /// <param name="vertex1">The vertex to enumerate edges for.</param>
            /// <param name="vertex2">The neighbour.</param>
            public override void MoveTo(uint vertex1, uint vertex2)
            {
                _edges = _graph.GetEdgesFor(vertex1, vertex2);
                this.Reset();
            }
        }

        /// <summary>
        /// A neighbour enumerators.
        /// </summary>
        private class NeighbourEnumerator : INeighbourEnumerator<LiveEdge>
        {
            /// <summary>
            /// Holds the edge and neighbours.
            /// </summary>
            private List<Tuple<uint, uint, uint, LiveEdge>> _neighbours;

            /// <summary>
            /// Holds the source.
            /// </summary>
            private RouterLiveEdgeDataSource _source;

            /// <summary>
            /// Holds the current position.
            /// </summary>
            private int _current = -1;

            /// <summary>
            /// Creates a new enumerators.
            /// </summary>
            /// <param name="source">The datasource the edges come from.</param>
            /// <param name="edges">The edge data.</param>
            public NeighbourEnumerator(RouterLiveEdgeDataSource source, 
                List<Tuple<uint, uint, uint, LiveEdge>> neighbours)
            {
                _source = source;
                _neighbours = neighbours;
            }

            /// <summary>
            /// Moves to the next coordinate.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                _current++;
                return _neighbours.Count > _current;
            }

            /// <summary>
            /// Gets the first vector.
            /// </summary>
            public uint Vertex1
            {
                get { return _neighbours[_current].Item1; }
            }

            /// <summary>
            /// Gets the second vector.
            /// </summary>
            public uint Vertex2
            {
                get { return _neighbours[_current].Item3; }
            }

            /// <summary>
            /// Gets the edge data.
            /// </summary>
            public LiveEdge EdgeData
            {
                get { return _neighbours[_current].Item4; }
            }

            /// <summary>
            /// Gets the current intermediates.
            /// </summary>
            public ICoordinateCollection Intermediates
            {
                get
                {
                    ICoordinateCollection shape;
                    if (_source.GetShapeForArc(_neighbours[_current].Item1, _neighbours[_current].Item2, out shape))
                    {
                        return shape;
                    }
                    return null;
                }
            }

            /// <summary>
            /// Returns true if this enumerator has a pre-calculated count.
            /// </summary>
            public bool HasCount
            {
                get { return true; }
            }

            /// <summary>
            /// Returns the count if any.
            /// </summary>
            public int Count
            {
                get { return _neighbours.Count; }
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _current = -1;
            }

            public IEnumerator<Neighbour<LiveEdge>> GetEnumerator()
            {
                this.Reset();
                return this;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                this.Reset();
                return this;
            }

            public Neighbour<LiveEdge> Current
            {
                get { return new Neighbour<LiveEdge>(this); }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this; }
            }

            public void Dispose()
            {

            }
        }

        public void AddRestriction(uint[] route)
        {

        }

        public void AddRestriction(string vehicleType, uint[] route)
        {

        }

        public bool TryGetRestrictionAsStart(Vehicle vehicle, uint vertex, out List<uint[]> routes)
        {
            routes = null;
            return false;
        }

        public bool TryGetRestrictionAsEnd(Vehicle vehicle, uint vertex, out List<uint[]> routes)
        {
            routes = null;
            return false;
        }

        public bool IsDirected
        {
            get { return false; }
        }

        public bool CanHaveDuplicates
        {
            get { return false; }
        }

        public IEnumerable<Edge<LiveEdge>> GetDirectNeighbours(uint vertex)
        {
            return this.GetEdges(vertex);
        }
    }
}
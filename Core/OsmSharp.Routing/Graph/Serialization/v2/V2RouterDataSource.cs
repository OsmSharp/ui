using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OsmSharp.Osm;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Math;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Math.Structures;
using OsmSharp.Math.Structures.QTree;

namespace OsmSharp.Routing.Graph.Serialization.v2
{
    /// <summary>
    /// A router data source that dynamically loads data.
    /// </summary>
    internal class V2RouterDataSource : IBasicRouterDataSource<LiveEdge>
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
        private readonly ITagsIndex _tagsIndex;

        /// <summary>
        /// Holds the vertex index.
        /// </summary>
        private readonly ILocatedObjectIndex<GeoCoordinate, uint> _vertexIndex;

        /// <summary>
        /// Creates a new router data source.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tileMetas"></param>
        /// <param name="zoom"></param>
        /// <param name="v1RoutingSerializer"></param>
        /// <param name="initialCapacity"></param>
        internal V2RouterDataSource(
            Stream stream,
            V2RoutingSerializer.SerializableGraphTileMetas tileMetas,
            int zoom, V2RoutingSerializer v1RoutingSerializer,
            int initialCapacity = 1000)
        {
            _tagsIndex = new SimpleTagsIndex();
            _vertices = new SparseArray<Vertex>(initialCapacity);
            _coordinates = new SparseArray<Location>(initialCapacity);

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
            _routingSerializer = v1RoutingSerializer;
            _stream = stream;
        }

        /// <summary>
        /// Returns true if the given profile is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool SupportsProfile(VehicleEnum vehicle)
        {
            // TODO: also save the profiles.
            return true;
        }

        /// <summary>
        /// Returns true if the given profile is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public void AddSupportedProfile(VehicleEnum vehicle)
        {
            // TODO: also save the profiles.
        }

        /// <summary>
        /// Returns all edges inside the given boundingbox.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public KeyValuePair<uint, KeyValuePair<uint, LiveEdge>>[] GetArcs(
            GeoCoordinateBox box)
        {
            // load the missing tiles.
            this.LoadMissingTile(box);

            // get all the vertices in the given box.
            IEnumerable<uint> vertices = _vertexIndex.GetInside(
                box);

            // loop over all vertices and get the arcs.
            var arcs = new List<KeyValuePair<uint, KeyValuePair<uint, LiveEdge>>>();
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
                            KeyValuePair<uint, LiveEdge>[] localArcs = vertex.Arcs;
                            foreach (KeyValuePair<uint, LiveEdge> localArc in localArcs)
                            {
                                arcs.Add(new KeyValuePair<uint, KeyValuePair<uint, LiveEdge>>(
                                    vertexId, localArc));
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
        public ITagsIndex TagsIndex
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
        public KeyValuePair<uint, LiveEdge>[] GetArcs(uint vertexId)
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
                    return vertex.Arcs;
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
        public bool HasNeighbour(uint vertexId, uint neighbour)
        {
            throw new NotImplementedException();
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
            public KeyValuePair<uint, LiveEdge>[] Arcs;
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
        /// Holds the routing serializer.
        /// </summary>
        private readonly V2RoutingSerializer _routingSerializer;

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
                    V2RoutingSerializer.SerializableGraphTile tileData =
                        _routingSerializer.DeserializeTile(_stream, meta.Offset, meta.Length);
                    double top = tile.TopLeft.Latitude;
                    double left = tile.TopLeft.Longitude;
                    for(int vertexIdx = 0; vertexIdx < tileData.Ids.Length; vertexIdx++)
                    {
                        // resize.
                        this.Resize(tileData.Ids[vertexIdx] + 1);

                        // create the location and calculate lat/lon.
                        var vertexLocation = new Location();
                        vertexLocation.Latitude = (float)((((double) tileData.Latitude[vertexIdx]*tile.Box.DeltaLat)/ushort.MaxValue) +
                                          top);
                        vertexLocation.Longitude = (float)((((double)tileData.Longitude[vertexIdx] * tile.Box.DeltaLon) / ushort.MaxValue) +
                                          left);
                        _coordinates[(int)tileData.Ids[vertexIdx]] = vertexLocation;

                        // convert the arcs.
                        if (tileData.Arcs[vertexIdx] != null)
                        {
                            var arcs = new KeyValuePair<uint, LiveEdge>[tileData.Arcs[vertexIdx].DestinationId.Length];
                            for (int idx = 0; idx < tileData.Arcs[vertexIdx].DestinationId.Length; idx++)
                            {
                                // create the tags collection.
                                TagsCollection tagsCollection = new SimpleTagsCollection();
                                for (int tagsIdx = 0;
                                     tagsIdx < tileData.Arcs[vertexIdx].Tags[idx].Keys.Length;
                                     tagsIdx++)
                                {
                                    string key = tileData.StringTable[tileData.Arcs[vertexIdx].Tags[idx].Keys[idx]];
                                    string value = tileData.StringTable[tileData.Arcs[vertexIdx].Tags[idx].Values[idx]];

                                    tagsCollection.Add(key, value);
                                }
                                uint tags = _tagsIndex.Add(tagsCollection);

                                // create the liveedge.
                                var edge = new LiveEdge();
                                edge.Forward = tileData.Arcs[vertexIdx].Forward[idx];
                                edge.Tags = tags;

                                // convert the arc.
                                arcs[idx] = new KeyValuePair<uint, LiveEdge>(
                                    tileData.Arcs[vertexIdx].DestinationId[idx], edge);

                                // store the target tile.
                                _tilesPerVertex[tileData.Arcs[vertexIdx].DestinationId[idx]] =
                                    new Tile(tileData.Arcs[vertexIdx].TileX[idx], tileData.Arcs[vertexIdx].TileY[idx], _zoom);
                            }
                            _vertices[(int)tileData.Ids[vertexIdx]] = new Vertex()
                            {
                                Arcs = arcs
                            };
                        }
                        _vertexIndex.Add(new GeoCoordinate(tileData.Latitude[vertexIdx],
                            tileData.Longitude[vertexIdx]), tileData.Ids[vertexIdx]);
                    }
                }

                _loadedTiles.Add(tile); // tile is loaded.
            }
        }

        #endregion
    }
}
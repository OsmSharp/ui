//using System;
//using System.Collections.Generic;
//using System.Linq;
//using OsmSharp.Osm.Data;
//using OsmSharp.Osm.Data.Streams;
//using OsmSharp.Osm.Data.Streams.Collections;
//using OsmSharp.Osm.Data.Streams.Filters;
//using OsmSharp.Osm.Tiles;
//using OsmSharp.Routing.Graph;
//using OsmSharp.Routing.Graph.Router;
//using OsmSharp.Collections.Tags;
//using OsmSharp.Math.Geo;
//using OsmSharp.Osm;
//using OsmSharp.Routing.Osm.Data.Processing;
//using OsmSharp.Routing.Interpreter;

//namespace OsmSharp.Routing.Osm.Graphs
//{
//    /// <summary>
//    /// A routing data source graph that wraps around a standard IDataSourceReadOnly source.
//    /// </summary>
//    public class OsmSourceRouterDataSource : IBasicRouterDataSource<LiveEdge>
//    {
//        /// <summary>
//        /// Holds the data source.
//        /// </summary>
//        private readonly IDataSourceReadOnly _source;

//        /// <summary>
//        /// Holds a cache of data.
//        /// </summary>
//        private readonly DynamicGraphRouterDataSource<LiveEdge> _dataCache;

//        /// <summary>
//        /// Holds the tags index.
//        /// </summary>
//        private readonly ITagsIndex _tagsIndex;

//        /// <summary>
//        /// Holds the edge interpreter.
//        /// </summary>
//        private readonly IRoutingInterpreter _interpreter;

//        /// <summary>
//        /// Creates a OSM dynamic graph.
//        /// </summary>
//        /// <param name="tagsIndex">The tags index.</param>
//        /// <param name="source">The OSM data source.</param>
//        /// <param name="interpreter">The routing interpreter.</param>
//        public OsmSourceRouterDataSource(IRoutingInterpreter interpreter, ITagsIndex tagsIndex, 
//            IDataSourceReadOnly source)
//        {
//            _source = source;
//            _tagsIndex = tagsIndex;
//            _interpreter = interpreter;

//            _dataCache = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
//            _loadedTiles = new HashSet<Tile>();
//            _loadedVertices = null;
//            _useLoadedSet = false;
//            _zoom = 14;
//            _idTranformations = new Dictionary<long, uint>();
//        }

//        /// <summary>
//        /// Returns true if the given vehicle profile is supported.
//        /// </summary>
//        /// <param name="vehicle"></param>
//        /// <returns></returns>
//        public bool SupportsProfile(Vehicle vehicle)
//        {
//            return true; // any IBasicRouterDataSource<LiveEdge> support all vehicle types.
//        }

//        /// <summary>
//        /// Returns all arcs with a given bounding box.
//        /// </summary>
//        /// <param name="box"></param>
//        /// <returns></returns>
//        public KeyValuePair<uint, KeyValuePair<uint, LiveEdge>>[] GetArcs(GeoCoordinateBox box)
//        {
//            // load if needed.
//            this.LoadMissingIfNeeded(box);

//            return _dataCache.GetArcs(box);
//        }

//        /// <summary>
//        /// Returns true if the vertex is found.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <param name="latitude"></param>
//        /// <param name="longitude"></param>
//        /// <returns></returns>
//        public bool GetVertex(uint id, out float latitude, out float longitude)
//        {
//            return _dataCache.GetVertex(id, out latitude, out longitude);
//        }

//        /// <summary>
//        /// Returns all the arcs for a given vertex.
//        /// </summary>
//        /// <param name="vertexId"></param>
//        /// <returns></returns>
//        public KeyValuePair<uint, LiveEdge>[] GetArcs(uint vertexId)
//        {
//            float latitude, longitude;
//            if(_dataCache.GetVertex(vertexId, out latitude, out longitude))
//            {
//                // load if needed.
//                this.LoadMissingIfNeeded(vertexId, latitude, longitude);

//                // load the arcs.
//                return _dataCache.GetArcs(vertexId);
//            }
//            throw new Exception(string.Format("Vertex with id {0} not found!",
//                    vertexId));
//        }

//        /// <summary>
//        /// Returns true if the given vertex has neighbour as a neighbour.
//        /// </summary>
//        /// <param name="vertexId"></param>
//        /// <param name="neighbour"></param>
//        /// <returns></returns>
//        public bool HasNeighbour(uint vertexId, uint neighbour)
//        {
//            return this.GetArcs(vertexId).Any(arc => arc.Key == neighbour);
//        }

//        /// <summary>
//        /// Returns the tags index.
//        /// </summary>
//        public ITagsIndex TagsIndex
//        {
//            get 
//            {
//                return _tagsIndex;
//            }
//        }

//        #region Loading Strategy

//        /// <summary>
//        /// The zoom level to cache at.
//        /// </summary>
//        private readonly int _zoom;

//        /// <summary>
//        /// When true uses a hashset to prevent duplicate calculations.
//        /// </summary>
//        private readonly bool _useLoadedSet;

//        /// <summary>
//        /// Holds an index of all loaded tiles.
//        /// </summary>
//        private readonly HashSet<Tile> _loadedTiles;

//        /// <summary>
//        /// Holds an index of all vertices that have been validated with loaded tiles.
//        /// </summary>
//        private readonly HashSet<long> _loadedVertices;

//        /// <summary>
//        /// Holds the id tranformations.
//        /// </summary>
//        private readonly IDictionary<long, uint> _idTranformations;

//        /// <summary>
//        /// Load the missing tile this vertex is in if needed.
//        /// </summary>
//        /// <param name="vertex"></param>
//        /// <param name="latitude"></param>
//        /// <param name="longitude"></param>
//        private void LoadMissingIfNeeded(long vertex, float latitude, float longitude)
//        {
//            if (_loadedVertices == null || 
//                !_loadedVertices.Contains(vertex))
//            { // vertex not validated yet!
//                Tile tile1 = Tile.CreateAroundLocation(new GeoCoordinate(latitude, longitude), _zoom);

//                var range = new TileRange(tile1.X, tile1.Y,
//                    tile1.X, tile1.Y, tile1.Zoom);

//                foreach (Tile tile in range)
//                {
//                    if (!_loadedTiles.Contains(tile))
//                    { // the tile need to be loaded!
//                        this.LoadTile(tile);

//                        // add the tile to the loaded tiles index.
//                        _loadedTiles.Add(tile);
//                    }
//                }

//                // add the vertex (if needed)
//                if (_loadedVertices != null && _useLoadedSet)
//                {
//                    _loadedVertices.Add(vertex);
//                }
//            }
//        }

//        /// <summary>
//        /// Loads missing tiles for the bounding box if needed.
//        /// </summary>
//        /// <param name="box"></param>
//        private void LoadMissingIfNeeded(GeoCoordinateBox box)
//        {
//            // calculate the tile range.
//            TileRange tileRange = TileRange.CreateAroundBoundingBox(box, _zoom);

//            // load all the needed tiles.
//            foreach (Tile tile in tileRange)
//            {
//                if (!_loadedTiles.Contains(tile))
//                { // the tile need to be loaded!
//                    this.LoadTile(tile);

//                    // add the tile to the loaded tiles index.
//                    _loadedTiles.Add(tile);
//                }
//            }
//        }

//        /// <summary>
//        /// Loads data located in a tile.
//        /// </summary>
//        /// <param name="tile"></param>
//        private void LoadTile(Tile tile)
//        {
//            // load data.
//            GeoCoordinateBox box = tile.Box.Resize(0.00001);
//            IList<OsmGeo> data = _source.Get(box, OsmSharp.Osm.Filters.Filter.Any());

//            // process the list of data just loaded.
//            var targetData = new LiveGraphOsmStreamWriter(
//                _dataCache, _interpreter, _tagsIndex, _idTranformations);
//            var dataProcessorSource =
//                new OsmGeoListDataProcessorSource(data);
//            var sorter = new OsmStreamFilterSort();
//            sorter.RegisterSource(dataProcessorSource);
//            targetData.RegisterSource(sorter);
//            targetData.Pull();
//        }

//        #endregion

//        /// <summary>
//        /// Returns the number of vertices currently in this graph.
//        /// </summary>
//        public uint VertexCount
//        {
//            get { return _dataCache.VertexCount; }
//        }
//    }
//}

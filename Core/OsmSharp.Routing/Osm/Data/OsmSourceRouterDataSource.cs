using System;
using System.Collections.Generic;
using System.Linq;
using OsmSharp.Osm.Data;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Router;
using OsmSharp.Tools.Collections;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math;
using OsmSharp.Osm;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Osm.Data.Core.Processor.ListSource;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Graph.DynamicGraph.PreProcessed;

namespace OsmSharp.Routing.Osm.Data
{
    /// <summary>
    /// A Dynamic graph with extended possibilities to allow resolving points.
    /// </summary>
    public class OsmSourceRouterDataSource : IBasicRouterDataSource<PreProcessedEdge>
    {
        /// <summary>
        /// Holds the data source.
        /// </summary>
        private readonly IDataSourceReadOnly _source;

        /// <summary>
        /// Holds a cache of data.
        /// </summary>
        private readonly DynamicGraphRouterDataSource<PreProcessedEdge> _dataCache;

        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private readonly ITagsIndex _tagsIndex;

        /// <summary>
        /// Holds the edge interpreter.
        /// </summary>
        private readonly IRoutingInterpreter _interpreter;

        /// <summary>
        /// Holds the supported vehicle profile.
        /// </summary>
        private readonly VehicleEnum _vehicle;

        /// <summary>
        /// Creates a OSM dynamic graph.
        /// </summary>
        /// <param name="tagsIndex">The tags index.</param>
        /// <param name="source">The OSM data source.</param>
        /// <param name="interpreter">The routing interpreter.</param>
        /// <param name="vehicle">The vehicle profile being targetted.</param>
        public OsmSourceRouterDataSource(IRoutingInterpreter interpreter, ITagsIndex tagsIndex, 
            IDataSourceReadOnly source, VehicleEnum vehicle)
        {
            _source = source;
            _vehicle = vehicle;
            _tagsIndex = tagsIndex;
            _interpreter = interpreter;

            _dataCache = new DynamicGraphRouterDataSource<PreProcessedEdge>(tagsIndex);
            _loadedTiles = new HashSet<Tile>();
            _loadedVertices = null;
            _useLoadedSet = false;
            _zoom = 14;
            _idTranformations = new Dictionary<long, uint>();
        }

        /// <summary>
        /// Returns true if the given vehicle profile is supported.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <returns></returns>
        public bool SupportsProfile(VehicleEnum vehicle)
        {
            return vehicle == _vehicle;
        }

        /// <summary>
        /// Adds one more supported profile.
        /// </summary>
        /// <param name="vehicle"></param>
        public void AddSupportedProfile(VehicleEnum vehicle)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns all arcs with a given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public KeyValuePair<uint, KeyValuePair<uint, PreProcessedEdge>>[] GetArcs(GeoCoordinateBox box)
        {
            // load if needed.
            this.LoadMissingIfNeeded(box);

            return _dataCache.GetArcs(box);
        }

        /// <summary>
        /// Returns true if the vertex is found.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool GetVertex(uint id, out float latitude, out float longitude)
        {
            return _dataCache.GetVertex(id, out latitude, out longitude);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<uint> GetVertices()
        {
            throw new NotSupportedException("Enumerating vertices in a OsmSourceRouterDataSource is not possible!");
        }

        /// <summary>
        /// Returns all the arcs for a given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public KeyValuePair<uint, PreProcessedEdge>[] GetArcs(uint vertex)
        {
            float latitude, longitude;
            if(_dataCache.GetVertex(vertex, out latitude, out longitude))
            {
                // load if needed.
                this.LoadMissingIfNeeded(vertex, latitude, longitude);

                // load the arcs.
                return _dataCache.GetArcs(vertex);
            }
            throw new Exception(string.Format("Vertex with id {0} not found!",
                    vertex));
        }

        /// <summary>
        /// Returns true if the given vertex has neighbour as a neighbour.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public bool HasNeighbour(uint vertex, uint neighbour)
        {
            return this.GetArcs(vertex).Any(arc => arc.Key == neighbour);
        }


        /// <summary>
        /// Returns the tags index.
        /// </summary>
        public ITagsIndex TagsIndex
        {
            get 
            {
                return _tagsIndex;
            }
        }

        #region Loading Strategy

        /// <summary>
        /// The zoom level to cache at.
        /// </summary>
        private readonly int _zoom;

        /// <summary>
        /// When true uses a hashset to prevent duplicate calculations.
        /// </summary>
        private readonly bool _useLoadedSet;

        /// <summary>
        /// Holds an index of all loaded tiles.
        /// </summary>
        private readonly HashSet<Tile> _loadedTiles;

        /// <summary>
        /// Holds an index of all vertices that have been validated with loaded tiles.
        /// </summary>
        private readonly HashSet<long> _loadedVertices;

        /// <summary>
        /// Holds the id tranformations.
        /// </summary>
        private readonly IDictionary<long, uint> _idTranformations;

        /// <summary>
        /// Load the missing tile this vertex is in if needed.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        private void LoadMissingIfNeeded(long vertex, float latitude, float longitude)
        {
            if (_loadedVertices == null || 
                !_loadedVertices.Contains(vertex))
            { // vertex not validated yet!
                Tile tile1 = Tile.CreateAroundLocation(new GeoCoordinate(latitude, longitude), _zoom);

                var range = new TileRange(tile1.X, tile1.Y,
                    tile1.X, tile1.Y, tile1.Zoom);

                foreach (Tile tile in range)
                {
                    if (!_loadedTiles.Contains(tile))
                    { // the tile need to be loaded!
                        this.LoadTile(tile);

                        // add the tile to the loaded tiles index.
                        _loadedTiles.Add(tile);
                    }
                }

                // add the vertex (if needed)
                if (_loadedVertices != null && _useLoadedSet)
                {
                    _loadedVertices.Add(vertex);
                }
            }
        }

        /// <summary>
        /// Loads missing tiles for the bounding box if needed.
        /// </summary>
        /// <param name="box"></param>
        private void LoadMissingIfNeeded(GeoCoordinateBox box)
        {
            // calculate the tile range.
            TileRange tileRange = TileRange.CreateAroundBoundingBox(box, _zoom);

            // load all the needed tiles.
            foreach (Tile tile in tileRange)
            {
                if (!_loadedTiles.Contains(tile))
                { // the tile need to be loaded!
                    this.LoadTile(tile);

                    // add the tile to the loaded tiles index.
                    _loadedTiles.Add(tile);
                }
            }
        }

        /// <summary>
        /// Loads data located in a tile.
        /// </summary>
        /// <param name="tile"></param>
        private void LoadTile(Tile tile)
        {
            // load data.
            GeoCoordinateBox box = tile.Box.Resize(0.00001);
            IList<OsmGeo> data = _source.Get(box, OsmSharp.Osm.Filters.Filter.Any());

            // process the list of data just loaded.
            // TODO: fix this vehicle profile mess!
            var targetData = new PreProcessedDataGraphProcessingTarget(
                _dataCache, _interpreter, _tagsIndex, OsmSharp.Routing.VehicleEnum.Car, _idTranformations);
            var dataProcessorSource =
                new OsmGeoListDataProcessorSource(data);
            var sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();
        }

        #endregion

        /// <summary>
        /// Returns the number of vertices currently in this graph.
        /// </summary>
        public uint VertexCount
        {
            get { return _dataCache.VertexCount; }
        }
    }
}

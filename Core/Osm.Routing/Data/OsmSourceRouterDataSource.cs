using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Data;
using OsmSharp.Routing.Core.Router;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Core.Router.Memory;
using OsmSharp.Tools.Math;
using OsmSharp.Osm.Core;
using OsmSharp.Routing.Core.Interpreter.Roads;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Osm.Data.Core.Processor.ListSource;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;

namespace OsmSharp.Osm.Routing.Data.Source
{
    /// <summary>
    /// A Dynamic graph with extended possibilities to allow resolving points.
    /// </summary>
    public class OsmSourceRouterDataSource : IRouterDataSource<OsmEdgeData>
    {
        /// <summary>
        /// Holds the data source.
        /// </summary>
        private IDataSourceReadOnly _source;

        /// <summary>
        /// Holds a cache of data.
        /// </summary>
        private MemoryRouterDataSource<OsmEdgeData> _data_cache;

        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private ITagsIndex _tags_index;

        /// <summary>
        /// Holds the edge interpreter.
        /// </summary>
        private IRoutingInterpreter _interpreter;

        /// <summary>
        /// Creates a OSM dynamic graph.
        /// </summary>
        /// <param name="source"></param>
        public OsmSourceRouterDataSource(IRoutingInterpreter interpreter, ITagsIndex tags_index, IDataSourceReadOnly source)
        {
            _source = source;
            _tags_index = tags_index;
            _interpreter = interpreter;

            _data_cache = new MemoryRouterDataSource<OsmEdgeData>(tags_index);
            _loaded_tiles = new HashSet<Tile>();
            _loaded_vertices = null;
            _use_loaded_set = false;
            _zoom = 14;
            _id_tranformations = new Dictionary<long, uint>();
        }

        /// <summary>
        /// Returns all arcs with a given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public KeyValuePair<uint, KeyValuePair<uint, OsmEdgeData>>[] GetArcs(GeoCoordinateBox box)
        {
            // load if needed.
            this.LoadMissingIfNeeded(box);

            return _data_cache.GetArcs(box);
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
            return _data_cache.GetVertex(id, out latitude, out longitude);
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
        public KeyValuePair<uint, OsmEdgeData>[] GetArcs(uint vertex)
        {
            float latitude, longitude;
            if(_data_cache.GetVertex(vertex, out latitude, out longitude))
            {
                // load if needed.
                this.LoadMissingIfNeeded(vertex, latitude, longitude);

                // load the arcs.
                return _data_cache.GetArcs(vertex);
            }
            throw new Exception(string.Format("Vertex with id {0} not found!",
                    vertex));
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        public ITagsIndex TagsIndex
        {
            get 
            {
                return _tags_index;
            }
        }

        #region Loading Strategy

        /// <summary>
        /// The zoom level to cache at.
        /// </summary>
        private int _zoom;

        /// <summary>
        /// When true uses a hashset to prevent duplicate calculations.
        /// </summary>
        private bool _use_loaded_set;

        /// <summary>
        /// Holds an index of all loaded tiles.
        /// </summary>
        private HashSet<Tile> _loaded_tiles;

        /// <summary>
        /// Holds an index of all vertices that have been validated with loaded tiles.
        /// </summary>
        private HashSet<long> _loaded_vertices;

        /// <summary>
        /// Holds the id tranformations.
        /// </summary>
        private IDictionary<long, uint> _id_tranformations;

        /// <summary>
        /// Load the missing tile this vertex is in if needed.
        /// </summary>
        /// <param name="vertex"></param>
        private void LoadMissingIfNeeded(long vertex, float latitude, float longitude)
        {
            if (_loaded_vertices == null || 
                !_loaded_vertices.Contains(vertex))
            { // vertex not validated yet!
                Tile tile1 = Tile.CreateAroundLocation(new GeoCoordinate(latitude, longitude), _zoom);

                TileRange range = new TileRange(tile1.X - 1, tile1.Y - 1,
                    tile1.X + 1, tile1.Y + 1, tile1.Zoom);

                foreach (Tile tile in range)
                {
                    if (!_loaded_tiles.Contains(tile))
                    { // the tile need to be loaded!
                        this.LoadTile(tile);

                        // add the tile to the loaded tiles index.
                        _loaded_tiles.Add(tile);
                    }
                }

                // add the vertex (if needed)
                if (_loaded_vertices != null && _use_loaded_set)
                {
                    _loaded_vertices.Add(vertex);
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
            TileRange tile_range = TileRange.CreateAroundBoundingBox(box, _zoom);

            // load all the needed tiles.
            foreach (Tile tile in tile_range)
            {
                if (!_loaded_tiles.Contains(tile))
                { // the tile need to be loaded!
                    this.LoadTile(tile);

                    // add the tile to the loaded tiles index.
                    _loaded_tiles.Add(tile);
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
            IList<OsmGeo> data = _source.Get(box, OsmSharp.Osm.Core.Filters.Filter.Any());

            // process the list of data just loaded.
            OsmEdgeDataGraphProcessingTarget target_data = new OsmEdgeDataGraphProcessingTarget(
                _data_cache, _interpreter, _tags_index, _id_tranformations);
            OsmGeoListDataProcessorSource data_processor_source =
                new OsmGeoListDataProcessorSource(data);
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();
        }

        #endregion

        /// <summary>
        /// Returns the number of vertices currently in this graph.
        /// </summary>
        public uint VertexCount
        {
            get { return _data_cache.VertexCount; }
        }
    }
}

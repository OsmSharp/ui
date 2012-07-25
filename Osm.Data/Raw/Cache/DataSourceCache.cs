using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Tools.Math.Units.Angle;
using Tools.Math.Geo;
using Osm.Core.Filters;

namespace Osm.Data.Cache
{
    /// <summary>
    /// Class used for caching data using bounding boxes.
    /// 
    /// TODO: implement a way to expire cached data!!!!!!
    /// </summary>
    public class DataSourceCache : IDataSourceReadOnly
    {
        /// <summary>
        /// The zoom level of the boxes to cache at.
        /// </summary>
        private int _zoom_level;

        /// <summary>
        /// The data source.
        /// </summary>
        private IDataSourceReadOnly _source;

        /// <summary>
        /// The unique id of this data source.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Creates a new datasource cache.
        /// </summary>
        /// <param name="source">The data source being wrapped and cached.</param>
        /// <param name="zoom_level">The zoom level to use as a caching structure.</param>
        public DataSourceCache(IDataSourceReadOnly source,int zoom_level)
        {
            _zoom_level = zoom_level;
            _source = source;
            _id = Guid.NewGuid();

            this.InitializeCache();
        }

        #region Caching Functions


        /// <summary>
        /// Initializes the data cache.
        /// </summary>
        private void InitializeCache()
        {
            _tiles_cache = new Dictionary<int, IDictionary<int, IList<OsmBase>>>();
            _nodes = new Dictionary<long, Node>();
            _ways = new Dictionary<long, Way>();
            _relations = new Dictionary<long, Relation>();
            _ways_per_node = new Dictionary<long, IList<Way>>();
        }


        #region Tiles Cache

        /// <summary>
        /// Holds the cached objects based on the same calculations as standard tile generations.
        /// 
        /// A zoom level can be chosen to calculate the size of the caching strategy.
        /// </summary>
        private Dictionary<int, IDictionary<int, IList<OsmBase>>> _tiles_cache;

        /// <summary>
        /// Returns all osm object in a bounding box from cache or from the wrapped data source.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private IList<OsmBase> GetObjectsInBox(int x, int y)
        {
            IDictionary<int,IList<OsmBase>> objects_in_box = null;
            if (!_tiles_cache.TryGetValue(x, out objects_in_box))
            {
                objects_in_box = new Dictionary<int, IList<OsmBase>>();
                _tiles_cache.Add(x, objects_in_box);
            }
            IList<OsmBase> objects = null;
            if (!objects_in_box.TryGetValue(y, out objects))
            {
                objects = this.LoadObjectsInBox(x, y);
                objects_in_box.Add(y, objects);
            }
            return objects;
        }

        private IList<OsmBase> LoadObjectsInBox(int x, int y)
        {
            //Tools.Core.Output.OutputTextStreamHost.WriteLine(string.Format("Loading box {0}-{1} at zoom {2}", x, y, _zoom_level));

            // make box slightly bigger.
            GeoCoordinateBox box = this.CreateBoxFor(x,y);
            box = new GeoCoordinateBox(
                new GeoCoordinate(box.MaxLat + 0.00001, box.MaxLon + 0.00001),
                new GeoCoordinate(box.MinLat - 0.00001, box.MinLon - 0.00001));


            return _source.Get(box, Filter.Any());
        }

        private GeoCoordinateBox CreateBoxFor(int x, int y)        
        {

            // calculate the tiles bounding box and set its properties.
            GeoCoordinate top_left = TileToWorldPos(x, y, _zoom_level);
            GeoCoordinate bottom_right = TileToWorldPos(x + 1, y + 1, _zoom_level);

            return new GeoCoordinateBox(top_left, bottom_right);
        }

        private IList<OsmBase> IndexObjectsInBox(int x, int y)
        {
            IList<OsmBase> objects_in_box = null;
            if (_tiles_cache.ContainsKey(x)
                && _tiles_cache[x].TryGetValue(y, out objects_in_box))
            {

            }
            else
            {
                // get all the objects in box x,y.
                objects_in_box = this.GetObjectsInBox(x, y);

                // get the box x-y.
                GeoCoordinateBox box = this.CreateBoxFor(x, y);

                // make box slightly smaller to prevent rounding errors.
                box = new GeoCoordinateBox(
                    new GeoCoordinate(box.MaxLat - 0.00001, box.MaxLon - 0.00001),
                    new GeoCoordinate(box.MinLat + 0.00001, box.MinLon + 0.00001)); 

                // post-process elements to add to the elements caches!
                Dictionary<long, List<Way>> ways_per_node = new Dictionary<long, List<Way>>();
                List<Node> nodes_outside = new List<Node>();
                foreach (OsmBase base_object in objects_in_box)
                {
                    if (base_object is Node)
                    {
                        Node other_node = this.NodeCacheTryGet(base_object.Id);
                        if (other_node == null)
                        {
                            this.NodeCachePut(base_object as Node);
                        }
                    }
                    else if (base_object is Way)
                    {
                        Way other_way = this.WayCacheTryGet(base_object.Id);
                        if (other_way == null)
                        {
                            this.WayCachePut(base_object as Way);
                        }

                        foreach (Node node in (base_object as Way).Nodes)
                        {
                            Node other_node = this.NodeCacheTryGet(node.Id);
                            if (other_node == null)
                            {
                                this.NodeCachePut(node);
                            }

                            if (box.IsInside(node.Coordinate))
                            {
                                if (this.WaysPerNodeCacheTryGet(node.Id) == null)
                                {
                                    List<Way> ways = null;
                                    if (!ways_per_node.TryGetValue(node.Id, out ways))
                                    {
                                        ways = new List<Way>();
                                        ways_per_node.Add(node.Id, ways);
                                    }
                                    if (!ways.Contains((base_object as Way)))
                                    {
                                        ways.Add((base_object as Way));
                                    }
                                }
                            }
                            else
                            {
                                nodes_outside.Add(node);
                            }
                        }
                    }
                    else if (base_object is Relation)
                    {
                        // TODO: cache the relation members!
                        Relation other_relation = this.RelationCacheTryGet(base_object.Id);
                        if (other_relation == null)
                        {
                            this.RelationCachePut(base_object as Relation);
                        }
                    }
                }

                foreach (KeyValuePair<long, List<Way>> pair in ways_per_node)
                {
                    //IList<Way> ways = this.GetWaysFor(this.GetNode(pair.Key));
                    //if (ways.Count != pair.Value.Count)
                    //{
                    //    throw new Exception();
                    //}
                    this.WaysPerNodeCachePut(pair.Key, pair.Value);
                }
                //foreach (Node node_outside in nodes_outside)
                //{
                //    if (this.WaysPerNodeCacheTryGet(node_outside.Id) == null)
                //    {
                //        IList<Way> ways = this.GetWaysFor(node_outside, false);
                //        //this.WaysPerNodeCachePut(node_outside.Id, ways);
                //    }
                //}
            }

            return objects_in_box;
        }

        private void IndexObjectsInBox(GeoCoordinateBox box)
        {
            TileRange range = this.GetTileToLoadFor(box, _zoom_level);

            for (int x = range.XMin; x < range.XMax + 1; x++)
            {
                for (int y = range.YMin; y < range.YMax + 1; y++)
                {
                    this.IndexObjectsInBox(x, y);
                }
            }
        }

        #endregion

        #region Objects Cache

        private Dictionary<long, Node> _nodes;
        private void NodeCachePut(Node node)
        {
            _nodes.Add(node.Id,node);
        }
        private Node NodeCacheTryGet(long id)
        {
            Node output = null;
            _nodes.TryGetValue(id, out output);
            return output;
        }

        private Dictionary<long, Way> _ways;
        private void WayCachePut(Way way)
        {
            _ways.Add(way.Id, way);
        }
        private Way WayCacheTryGet(long id)
        {
            Way output = null;
            _ways.TryGetValue(id, out output);
            return output;
        }

        private Dictionary<long, Relation> _relations;
        private void RelationCachePut(Relation relation)
        {
            _relations.Add(relation.Id, relation);
        }
        private Relation RelationCacheTryGet(long id)
        {
            Relation output = null;
            _relations.TryGetValue(id, out output);
            return output;
        }

        private Dictionary<long, IList<Way>> _ways_per_node;
        private void WaysPerNodeCachePut(long node_id, IList<Way> ways)
        {
            _ways_per_node.Add(node_id, ways);
        }
        private IList<Way> WaysPerNodeCacheTryGet(long node_id)
        {
            IList<Way> output = null;
            _ways_per_node.TryGetValue(node_id, out output);
            return output;
        }

        #endregion

        /// <summary>
        /// Converts the tile position to longitude and latitude coordinates.
        /// </summary>
        /// <param name="tile_x"></param>
        /// <param name="tile_y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private GeoCoordinate TileToWorldPos(double tile_x, double tile_y, int zoom)
        {
            double n = System.Math.PI - ((2.0 * System.Math.PI * tile_y) / System.Math.Pow(2.0, zoom));

            double longitude = (double)((tile_x / System.Math.Pow(2.0, zoom) * 360.0) - 180.0);
            double latitude = (double)(180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(n)));

            return new GeoCoordinate(latitude, longitude);
        }

        /// <summary>
        /// Returns a range of tiles for a bounding box.
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private TileRange GetTileToLoadFor(GeoCoordinateBox bbox,
            int zoom)
        {
            int n = (int)System.Math.Floor(System.Math.Pow(2, zoom));

            Radian rad = new Degree(bbox.MaxLat);

            int x_tile_min = (int)(((bbox.MinLon + 180.0f) / 360.0f) * (double)n);
            int y_tile_min = (int)(
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);


            rad = new Degree(bbox.MinLat);
            int x_tile_max = (int)(((bbox.MaxLon + 180.0f) / 360.0f) * (double)n);
            int y_tile_max = (int)(
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);

            TileRange range = new TileRange();
            range.XMax = x_tile_max;
            range.XMin = x_tile_min;
            range.YMax = y_tile_max;
            range.YMin = y_tile_min;
            return range;
        }

        #endregion

        #region IDataSourceReadOnly Members

        public Tools.Math.Geo.GeoCoordinateBox BoundingBox
        {
            get 
            {
                return _source.BoundingBox;
            }
        }

        public string Name
        {
            get 
            { 
                return string.Format("Cached source for {0}",_source.Name); 
            }
        }

        public Guid Id
        {
            get 
            { 
                return _id; 
            }
        }

        public bool HasBoundinBox
        {
            get 
            { 
                return _source.HasBoundinBox; 
            }
        }

        public bool IsReadOnly
        {
            get 
            { 
                return true; 
            }
        }

        /// <summary>
        /// Returns a node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Osm.Core.Node GetNode(long id)
        {
            Node node = this.NodeCacheTryGet(id);
            if (node == null)
            {
                node =  _source.GetNode(id);
                this.NodeCachePut(node);
                
                // index the bb of this object.
                this.IndexObjectsInBox(node.BoundingBox);                                
            }
            return node;
        }

        /// <summary>
        /// Returns a list of nodes matching the given id's.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Osm.Core.Node> GetNodes(IList<long> ids)
        {
            IList<Node> ret_list = new List<Node>(ids.Count);

            for (int idx = 0; idx < ids.Count; idx++)
            {
                long id = ids[idx];
                ret_list.Add(this.GetNode(id));
            }

            return ret_list;
        }

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Osm.Core.Relation GetRelation(long id)
        {
            Relation relation = this.RelationCacheTryGet(id);
            if (relation == null)
            {
                relation = _source.GetRelation(id);
                this.RelationCachePut(relation);

                // index the bb of this object.
                // TODO: investigate if this is need; maybe only for smaller objects.
                //this.IndexObjectsInBox(relation.BoundingBox);         
            }
            return relation;
        }

        /// <summary>
        /// Returns a list of relations matching the given id's.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Osm.Core.Relation> GetRelations(IList<long> ids)
        {
            IList<Relation> ret_list = new List<Relation>(ids.Count);

            for (int idx = 0; idx < ids.Count; idx++)
            {
                long id = ids[idx];
                ret_list.Add(this.GetRelation(id));
            }

            return ret_list;
        }

        /// <summary>
        /// Returns a list of relations for a given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IList<Osm.Core.Relation> GetRelationsFor(Osm.Core.OsmBase obj)
        {
            // TODO: return cached version?
            //return _source.GetRelationsFor(obj);
            return new List<Osm.Core.Relation>();
        }

        /// <summary>
        /// Returns a way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Osm.Core.Way GetWay(long id)
        {
            Way way = this.WayCacheTryGet(id);
            if (way == null)
            {
                way = _source.GetWay(id);
                this.WayCachePut(way);

                // index the bb of this object.
                this.IndexObjectsInBox(way.BoundingBox);  
            }
            return way;
        }

        /// <summary>
        /// Returns a list of ways for the given id's.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Osm.Core.Way> GetWays(IList<long> ids)
        {
            IList<Way> ret_list = new List<Way>(ids.Count);

            for (int idx = 0; idx < ids.Count; idx++)
            {
                long id = ids[idx];
                ret_list.Add(this.GetWay(id));
            }

            return ret_list;
        }

        /// <summary>
        /// Returns a list of ways for a given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IList<Osm.Core.Way> GetWaysFor(Osm.Core.Node node)
        {
            return this.GetWaysFor(node,true);
        }

        /// <summary>
        /// Returns a list of ways for a given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IList<Osm.Core.Way> GetWaysFor(Osm.Core.Node node, bool cache)
        {
            // index the bb of this object.
            if (cache)
            {
                this.IndexObjectsInBox(node.BoundingBox);
            }

            IList<Way> ways = this.WaysPerNodeCacheTryGet(node.Id);
            if (ways == null)
            {
                ways = _source.GetWaysFor(node);
                this.WaysPerNodeCachePut(node.Id, new List<Way>(ways));
            }
            return ways;
        }

        /// <summary>
        /// Returns a list of objects inside a bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<Osm.Core.OsmBase> Get(Tools.Math.Geo.GeoCoordinateBox box, Osm.Core.Filters.Filter filter)
        {
            List<OsmBase> base_objects = new List<OsmBase>();

            TileRange range = this.GetTileToLoadFor(box,_zoom_level);

            for (int x = range.XMin; x < range.XMax + 1; x++)
            {
                for (int y = range.YMin; y < range.YMax + 1; y++)
                {
                    IList<OsmBase> objects_in_box = this.IndexObjectsInBox(x, y);

                    foreach (OsmBase obj in objects_in_box)
                    {
                        if (obj is OsmGeo
                            && (obj as OsmGeo).Shape.Inside(box))
                        {
                            base_objects.Add(obj);
                        }
                    }
                }
            }

            return base_objects;
        }

        #endregion
        
        private class TilePosition
        {
            public int X { get; set; }

            public int Y { get; set; }
        }

        private class TileRange
        {
            public int XMin { get; set; }

            public int YMin { get; set; }

            public int XMax { get; set; }

            public int YMax { get; set; }

            internal bool IsBorder(int x, int y)
            {
                return (x == this.XMin) || (x == this.XMax) 
                    || (y == this.YMin) || (y == this.YMin);
            }
        }
    }
}

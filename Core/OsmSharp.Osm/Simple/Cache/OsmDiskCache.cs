using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OsmSharp.Math.Geo;

namespace OsmSharp.Osm.Simple.Cache
{
    /// <summary>
    /// An osm data cache for simple OSM objects kept in memory.
    /// </summary>
    public class OsmDiskCache : OsmDataCache
    {
        /// <summary>
        /// Holds the info of the base directory.
        /// </summary>
        private DirectoryInfo _info;

        /// <summary>
        /// Creates a new Osm disk cache.
        /// </summary>
        /// <param name="path"></param>
        public OsmDiskCache(string path)
        {
            _info = new DirectoryInfo(path);

            _osmDataCaches = new Dictionary<Tile, OsmDataCache>();
            _nodesPerTile = new Dictionary<long, Tile>();
            _waysPerTile = new Dictionary<long, Tile>();
            _relationsPerTile = new Dictionary<long, Tile>();
        }

        #region OsmDataCache-implementation

        /// <summary>
        /// Adds a node to this cache.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(SimpleNode node)
        {
            var point = new GeoCoordinate(node.Latitude.Value, node.Longitude.Value);
            this.Add(node, new GeoCoordinateBox(point, point));
        }

        /// <summary>
        /// Tries to get the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool TryGetNode(long id, out SimpleNode node)
        {
            Tile tile;
            node = null;
            if (_nodesPerTile.TryGetValue(id, out tile))
            { // tries to get the node.
                node = this.GetFromTile(tile, SimpleOsmGeoType.Node, id) as SimpleNode;
                return node != null;
            }
            return false;
        }

        /// <summary>
        /// Returns an enumerable of all nodes.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<SimpleNode> GetNodes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a way to this cache.
        /// </summary>
        /// <param name="simpleWay"></param>
        public override void AddWay(SimpleWay simpleWay)
        {
            var way = Way.CreateFrom(simpleWay, this);

            IList<GeoCoordinate> coordinates = way.GetCoordinates();
            if (coordinates != null &&
                coordinates.Count > 0)
            {
                var box = new GeoCoordinateBox(
                    way.GetCoordinates());
                this.Add(simpleWay, box);
            }
        }

        /// <summary>
        /// Tries to get the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="way"></param>
        /// <returns></returns>
        public override bool TryGetWay(long id, out SimpleWay way)
        {
            Tile tile;
            way = null;
            if (_waysPerTile.TryGetValue(id, out tile))
            { // tries to get the node.
                way = this.GetFromTile(tile, SimpleOsmGeoType.Way, id) as SimpleWay;
                return way != null;
            }
            return false;
        }

        /// <summary>
        /// Returns an enumerable of all ways.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<SimpleWay> GetWays()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a relation to this cache.
        /// </summary>
        /// <param name="simpleRelation"></param>
        public override void AddRelation(SimpleRelation simpleRelation)
        {
            var relation = Relation.CreateFrom(simpleRelation, this);

            IList<GeoCoordinate> coordinates = relation.GetCoordinates();
            if (coordinates != null &&
                coordinates.Count > 0)
            {
                var box = new GeoCoordinateBox(
                    relation.GetCoordinates());
                this.Add(simpleRelation, box);
            }
        }

        /// <summary>
        /// Tries to get the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public override bool TryGetRelation(long id, out SimpleRelation relation)
        {
            Tile tile;
            relation = null;
            if (_relationsPerTile.TryGetValue(id, out tile))
            { // tries to get the node.
                relation = this.GetFromTile(tile, SimpleOsmGeoType.Relation, id) as SimpleRelation;
                return relation != null;
            }
            return false;
        }

        /// <summary>
        /// Returns an enumerable of all relations.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<SimpleRelation> GetRelations()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Holds the nodes per tile.
        /// </summary>
        private Dictionary<long, Tile> _nodesPerTile;

        /// <summary>
        /// Holds the ways per tile.
        /// </summary>
        private Dictionary<long, Tile> _waysPerTile;

        /// <summary>
        /// Holds the relations per tile.
        /// </summary>
        private Dictionary<long, Tile> _relationsPerTile; 

        /// <summary>
        /// Holds the data caches per tile.
        /// </summary>
        private Dictionary<Tile, OsmDataCache> _osmDataCaches; 

        /// <summary>
        /// Adds a simple Osm-geo objects (Nodes, Ways and Relations).
        /// </summary>
        /// <param name="simpleOsmGeo"></param>
        /// <param name="coordinateBox"></param>
        private void Add(SimpleOsmGeo simpleOsmGeo, GeoCoordinateBox coordinateBox)
        {
            var tile = new Tile(0, 0, 0);
            while (true)
            { // keep looping until.
                // see if one of the sub-tiles contains the given box.
                bool improve = false;
                foreach (var subtile in tile.SubTiles)
                {
                    if (subtile.Box.IsInside(coordinateBox))
                    { // the tile completely envelops the given box.
                        tile = subtile;
                        improve = true;
                        break;
                    }
                }
                if (!improve)
                { // there was no improvement.
                    break;
                }
            }

            // add to the tile.
            this.AddToTile(tile, simpleOsmGeo);
        }

        /// <summary>
        /// Adds an object to the given tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="geo"></param>
        private void AddToTile(Tile tile, SimpleOsmGeo geo)
        {
            OsmDataCache data;
            if (!_osmDataCaches.TryGetValue(tile, out data))
            {
                // the osm data cache is not there yet.
                data = new OsmDataCacheMemory();
                _osmDataCaches.Add(tile, data);
            }
            if (geo is SimpleNode)
            {
                data.AddNode(geo as SimpleNode);
                _nodesPerTile.Add(geo.Id.Value, tile);
            }
            else if (geo is SimpleWay)
            {
                data.AddWay(geo as SimpleWay);
                _waysPerTile.Add(geo.Id.Value, tile);
            }
            else if (geo is SimpleRelation)
            {
                data.AddRelation(geo as SimpleRelation);
                _relationsPerTile.Add(geo.Id.Value, tile);
            }
        }

        /// <summary>
        /// Returns an object from the given tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        private SimpleOsmGeo GetFromTile(Tile tile, SimpleOsmGeoType type, long id)
        {
            OsmDataCache data;
            if (_osmDataCaches.TryGetValue(tile, out data))
            {
                switch (type)
                {
                    case SimpleOsmGeoType.Node:
                        return data.GetNode(id);
                    case SimpleOsmGeoType.Way:
                        return data.GetWay(id);
                    case SimpleOsmGeoType.Relation:
                        return data.GetRelation(id);
                }
            }
            return null;
        }
    }
}
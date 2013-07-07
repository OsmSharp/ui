// OsmSharp - OpenStreetMap tools & library.
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
using System.Linq;
using System.Text;
using OsmSharp.Osm;
using OsmSharp.Math.Units.Angle;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Filters;
using OsmSharp.Osm.Tiles;
using OsmSharp.Osm.Simple;
using OsmSharp.Collections.Cache;

namespace OsmSharp.Osm.Data.Cache
{
    /// <summary>
    /// Class used for caching data using bounding boxes.
    /// </summary>
    public class DataSourceCache : IDataSourceReadOnly
    {
        /// <summary>
        /// Holds the id of this datasource.
        /// </summary>
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Holds the datasource being cached.
        /// </summary>
        private IDataSourceReadOnly _source;

        /// <summary>
        /// Creates a new datasource cache.
        /// </summary>
        /// <param name="source"></param>
        public DataSourceCache(IDataSourceReadOnly source)
        {
            _source = source;
        }

        /// <summary>
        /// Holds the lru cache for the nodes.
        /// </summary>
        private LRUCache<long, SimpleNode> _nodesCache = new LRUCache<long,SimpleNode>(10000);

        /// <summary>
        /// Holds the lru cache for the ways.
        /// </summary>
        private LRUCache<long, SimpleWay> _waysCache = new LRUCache<long, SimpleWay>(5000);

        /// <summary>
        /// Holds the lru cache for the relations.
        /// </summary>
        private LRUCache<long, SimpleRelation> _relationsCache = new LRUCache<long, SimpleRelation>(1000);

        /// <summary>
        /// Returns the boundingbox.
        /// </summary>
        public GeoCoordinateBox BoundingBox
        {
            get { return _source.BoundingBox; }
        }

        /// <summary>
        /// Returns the id of this datasource.
        /// </summary>
        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Returns true if this datasource is bounded.
        /// </summary>
        public bool HasBoundinBox
        {
            get { return _source.HasBoundinBox; }
        }

        /// <summary>
        /// Returns true.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleNode GetNode(long id)
        {
            SimpleNode node;
            if(!_nodesCache.TryGet(id, out node))
            { // cache miss.
                node = _source.GetNode(id);
                _nodesCache.Add(id, node);
            }
            return node;
        }

        /// <summary>
        /// Returns all the nodes in the same order than the given list.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<SimpleNode> GetNodes(IList<long> ids)
        {
            List<SimpleNode> nodes = new List<SimpleNode>(ids.Count);
            for (int idx = 0; idx < nodes.Count; idx++)
            {
                nodes.Add(this.GetNode(ids[idx]));
            }
            return nodes;
        }

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleRelation GetRelation(long id)
        {
            SimpleRelation relation;
            if (!_relationsCache.TryGet(id, out relation))
            { // cache miss.
                relation = _source.GetRelation(id);
                _relationsCache.Add(id, relation);
            }
            return relation;
        }

        /// <summary>
        /// Returns all the relations in the same order than the given list.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<SimpleRelation> GetRelations(IList<long> ids)
        {
            List<SimpleRelation> relations = new List<SimpleRelation>(ids.Count);
            for (int idx = 0; idx < relations.Count; idx++)
            {
                relations.Add(this.GetRelation(ids[idx]));
            }
            return relations;
        }

        /// <summary>
        /// Returns all relations for the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IList<SimpleRelation> GetRelationsFor(SimpleOsmGeo obj)
        {
            IList<SimpleRelation> relations = _source.GetRelationsFor(obj);
            foreach (SimpleRelation relation in relations)
            {
                _relationsCache.Add(relation.Id.Value, relation);
            }
            return relations;
        }

        /// <summary>
        /// Returns the way for the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleWay GetWay(long id)
        {
            SimpleWay way;
            if (!_waysCache.TryGet(id, out way))
            { // cache miss.
                way = _source.GetWay(id);
                _waysCache.Add(id, way);
            }
            return way;
        }

        /// <summary>
        /// Returns all the ways in the same order than the given list.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<SimpleWay> GetWays(IList<long> ids)
        {
            List<SimpleWay> ways = new List<SimpleWay>(ids.Count);
            for (int idx = 0; idx < ways.Count; idx++)
            {
                ways.Add(this.GetWay(ids[idx]));
            }
            return ways;
        }

        /// <summary>
        /// Returns all ways containing the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IList<SimpleWay> GetWaysFor(SimpleNode node)
        {
            IList<SimpleWay> ways = _source.GetWaysFor(node);
            foreach (SimpleWay way in ways)
            {
                _waysCache.Add(way.Id.Value, way);
            }
            return ways;
        }

        /// <summary>
        /// Returns all data in the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<SimpleOsmGeo> Get(GeoCoordinateBox box, Filter filter)
        {
            IList<SimpleOsmGeo> objects = _source.Get(box, filter);
            foreach (SimpleOsmGeo osmGeo in objects)
            {
                switch(osmGeo.Type)
                {
                    case SimpleOsmGeoType.Node:
                        _nodesCache.Add(osmGeo.Id.Value, osmGeo as SimpleNode);
                        break;
                    case SimpleOsmGeoType.Way:
                        _waysCache.Add(osmGeo.Id.Value, osmGeo as SimpleWay);
                        break;
                    case SimpleOsmGeoType.Relation:
                        _relationsCache.Add(osmGeo.Id.Value, osmGeo as SimpleRelation);
                        break;
                }
            }
            return objects;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Collections;

namespace OsmSharp.Osm.Simple.Cache
{
    /// <summary>
    /// An osm data cache for simple OSM objects kept in memory.
    /// </summary>
    public class OsmDataCacheMemory : OsmDataCache
    {
        private Dictionary<long, SimpleNode> _nodes;
        private Dictionary<long, SimpleWay> _ways;
        private Dictionary<long, SimpleRelation> _relations;

        /// <summary>
        /// Creates a new osm data cache for simple OSM objects kept in memory.
        /// </summary>
        public OsmDataCacheMemory()
        {
            _nodes = new Dictionary<long, SimpleNode>();
            _ways = new Dictionary<long, SimpleWay>();
            _relations = new Dictionary<long, SimpleRelation>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(SimpleNode node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (node.Id == null) throw new Exception("node.Id is null");

            _nodes[node.Id.Value] = node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool TryGetNode(long id, out SimpleNode node)
        {
            return _nodes.TryGetValue(id, out node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<SimpleNode> GetNodes()
        {
            return _nodes.Values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(SimpleWay way)
        {
            if (way == null) throw new ArgumentNullException("way");
            if (way.Id == null) throw new Exception("way.Id is null");

            _ways[way.Id.Value] = way;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="way"></param>
        /// <returns></returns>
        public override bool TryGetWay(long id, out SimpleWay way)
        {
            return _ways.TryGetValue(id, out way);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<SimpleWay> GetWays()
        {
            return _ways.Values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(SimpleRelation relation)
        {
            if (relation == null) throw new ArgumentNullException("relation");
            if (relation.Id == null) throw new Exception("relation.Id is null");

            _relations[relation.Id.Value] = relation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public override bool TryGetRelation(long id, out SimpleRelation relation)
        {
            return _relations.TryGetValue(id, out relation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<SimpleRelation> GetRelations()
        {
            return _relations.Values;
        }
    }
}

// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Filters;
using OsmSharp.Osm.Data.Core.Processor;

namespace OsmSharp.Osm.Data.Core.Memory
{
    /// <summary>
    /// An in-memory data repository of OSM data primitives.
    /// </summary>
    public class MemoryDataSource : IDataSourceReadOnly
    {
        /// <summary>
        /// Creates a new memory data structure.
        /// </summary>
        public MemoryDataSource()
        {
            // initialize the data structures.
            this.InitializeDataStructures();
        }

        /// <summary>
        /// Initializes the data cache.
        /// </summary>
        private void InitializeDataStructures()
        {
            _id = Guid.NewGuid(); // creates a new Guid.

            _nodes = new Dictionary<long, Node>();
            _ways = new Dictionary<long, Way>();
            _relations = new Dictionary<long, Relation>();
            _ways_per_node = new Dictionary<long, IList<Way>>();
        }

        #region Objects Cache

        private Dictionary<long, Node> _nodes;
        private void NodeCachePut(Node node)
        {
            _nodes.Add(node.Id, node);
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
        /// Holds the current bounding box.
        /// </summary>
        private GeoCoordinateBox _box = null;

        /// <summary>
        /// Returns the bounding box around all nodes.
        /// </summary>
        public GeoCoordinateBox BoundingBox
        {
            get { return _box; }
        }

        /// <summary>
        /// Gets/Sets the name of this source.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Holds the current Guid.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Returns the id.
        /// </summary>
        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Returns true if there is a bounding box.
        /// </summary>
        public bool HasBoundinBox
        {
            get { return true; }
        }

        /// <summary>
        /// Returns true if this source is readonly.
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
        public Node GetNode(long id)
        {
            Node node = null;
            _nodes.TryGetValue(id, out node);
            return node;
        }

        /// <summary>
        /// Returns the node(s) with the given id(s).
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Node> GetNodes(IList<long> ids)
        {
            List<Node> nodes = new List<Node>();
            if (ids != null)
            { // get all the ids.
                for (int idx = 0; idx < ids.Count; idx++)
                {
                    nodes.Add(this.GetNode(ids[idx]));
                }
            }
            return nodes;
        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(Node node)
        {
            _nodes[node.Id] = node;
        }

        /// <summary>
        /// Removes a node.
        /// </summary>
        /// <param name="id"></param>
        public void RemoveNode(long id)
        {
            _nodes.Remove(id);
        }

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Relation GetRelation(long id)
        {
            Relation relation = null;
            _relations.TryGetValue(id, out relation);
            return relation;
        }

        /// <summary>
        /// Returns the relation(s) with the given id(s).
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Relation> GetRelations(IList<long> ids)
        {
            List<Relation> relations = new List<Relation>();
            if (ids != null)
            { // get all the ids.
                for (int idx = 0; idx < ids.Count; idx++)
                {
                    relations.Add(this.GetRelation(ids[idx]));
                }
            }
            return relations;
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="relation"></param>
        public void AddRelation(Relation relation)
        {
            _relations[relation.Id] = relation;
        }

        /// <summary>
        /// Removes a relation.
        /// </summary>
        /// <param name="id"></param>
        public void RemoveRelation(long id)
        {
            _relations.Remove(id);
        }

        /// <summary>
        /// Returns all relations that have the given object as a member.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IList<Relation> GetRelationsFor(OsmBase obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Way GetWay(long id)
        {
            Way way = null;
            _ways.TryGetValue(id, out way);
            return way;
        }

        /// <summary>
        /// Returns all the way(s) with the given id(s).
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Way> GetWays(IList<long> ids)
        {
            List<Way> relations = new List<Way>();
            if (ids != null)
            { // get all the ids.
                for (int idx = 0; idx < ids.Count; idx++)
                {
                    relations.Add(this.GetWay(ids[idx]));
                }
            }
            return relations;
        }

        /// <summary>
        /// Returns all the ways for a given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IList<Way> GetWaysFor(Node node)
        {
            IList<Way> ways = null;
            _ways_per_node.TryGetValue(node.Id, out ways);
            return ways;
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="way"></param>
        public void AddWay(Way way)
        {
            _ways[way.Id] = way;

            foreach (Node node in way.Nodes)
            {
                if (node != null)
                {
                    this.AddNode(node);

                    IList<Way> ways = null;
                    if (!_ways_per_node.TryGetValue(node.Id, out ways))
                    {
                        ways = new List<Way>();
                        _ways_per_node.Add(node.Id, ways);
                    }
                    ways.Add(way);
                }
            }
        }

        /// <summary>
        /// Removes a way.
        /// </summary>
        /// <param name="id"></param>
        public void RemoveWay(long id)
        {
            _ways.Remove(id);
        }

        /// <summary>
        /// Returns all the objects within a given bounding box and filtered by a given filter.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
        {
            return new List<OsmGeo>();
        }

        /// <summary>
        /// Adds all objects from the given source to this memory data source.
        /// </summary>
        /// <param name="source"></param>
        public void PullFromSource(DataProcessorSource source)
        {
            // create a special memory data source target.
            MemoryDataSourceProcessorTarget target = new MemoryDataSourceProcessorTarget(this);
            target.RegisterSource(source); // register the given source as the source.
            target.Pull(); // pull the data from the source into the created target.
        }
    }
}
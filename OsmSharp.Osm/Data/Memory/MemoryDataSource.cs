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
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Filters;
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Interpreter;
using System.IO;

namespace OsmSharp.Osm.Data.Memory
{
    /// <summary>
    /// An in-memory data repository of OSM data primitives.
    /// </summary>
    public class MemoryDataSource : IDataSourceReadOnly
    {
        /// <summary>
        /// Holds geometry interpreter.
        /// </summary>
        private GeometryInterpreter _geometryInterpreter = null;

        /// <summary>
        /// Creates a new memory data structure using the default geometry interpreter.
        /// </summary>
        public MemoryDataSource() : this(GeometryInterpreter.DefaultInterpreter) { }

        /// <summary>
        /// Creates a new memory data structure.
        /// </summary>
        /// <param name="geometryInterpreter">The geometry interpreter.</param>
        public MemoryDataSource(GeometryInterpreter geometryInterpreter)
        {
            _geometryInterpreter = geometryInterpreter;

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
            _nodes.Add(node.Id.Value, node);
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
            _ways.Add(way.Id.Value, way);
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
            _relations.Add(relation.Id.Value, relation);
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
        /// Returns all nodes in this memory datasource.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Node> GetNodes()
        {
            return _nodes.Values;
        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(Node node)
        {
            _nodes[node.Id.Value] = node;
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
        /// Returns all relations in this memory datasource.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Relation> GetRelations()
        {
            return _relations.Values;
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="relation"></param>
        public void AddRelation(Relation relation)
        {
            _relations[relation.Id.Value] = relation;
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
        public IList<Relation> GetRelationsFor(OsmGeo obj)
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
        /// Returns all ways in this memory datasource.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Way> GetWays()
        {
            return _ways.Values;
        }

        /// <summary>
        /// Returns all the ways for a given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IList<Way> GetWaysFor(Node node)
        {
            IList<Way> ways = null;
            _ways_per_node.TryGetValue(node.Id.Value, out ways);
            return ways;
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="way"></param>
        public void AddWay(Way way)
        {
            _ways[way.Id.Value] = way;
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
            IList<OsmGeo> res = new List<OsmGeo>();
            foreach (Node node in _nodes.Values)
            {
                if ((filter == null || filter.Evaluate(node)) && 
                    _geometryInterpreter.Interpret(node, this).IsInside(box))
                {
                    res.Add(node);
                }
            }
            foreach (Way way in _ways.Values)
            {
                if ((filter == null || filter.Evaluate(way)) &&
                    _geometryInterpreter.Interpret(way, this).IsInside(box))
                {
                    res.Add(way);
                }
            }
            foreach (Relation relation in _relations.Values)
            {
                if ((filter == null || filter.Evaluate(relation)) &&
                    _geometryInterpreter.Interpret(relation, this).IsInside(box))
                {
                    res.Add(relation);
                }
            }

            return res;
        }

        #region Create Functions

        /// <summary>
        /// Creates a new memory data source from all the data in the given osm-stream.
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <returns></returns>
        public static MemoryDataSource CreateFrom(OsmStreamSource sourceStream)
        {
            // reset if possible.
            if (sourceStream.CanReset) { sourceStream.Reset(); }

            // enumerate all objects and add them to a new datasource.
            MemoryDataSource dataSource = new MemoryDataSource();
            foreach (var osmGeo in sourceStream)
            {
                if (osmGeo != null)
                {
                    switch(osmGeo.Type)
                    {
                        case OsmGeoType.Node:
                            dataSource.AddNode(osmGeo as Node);
                            break;
                        case OsmGeoType.Way:
                            dataSource.AddWay(osmGeo as Way);
                            break;
                        case OsmGeoType.Relation:
                            dataSource.AddRelation(osmGeo as Relation);
                            break;
                    }
                }
            }
            return dataSource;
        }

#if !WINDOWS_PHONE
        /// <summary>
        /// Creates a new memory data source from all the data in the given osm xml stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MemoryDataSource CreateFromXmlStream(Stream stream)
        {
            return MemoryDataSource.CreateFrom(new Xml.Processor.XmlOsmStreamReader(stream));
        }
#endif

        /// <summary>
        /// Creates a new memory data source from all the data in the given osm pbf stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MemoryDataSource CreateFromPBFStream(Stream stream)
        {
            return MemoryDataSource.CreateFrom(new PBF.Processor.PBFOsmStreamSource(stream));
        }

        #endregion
    }
}
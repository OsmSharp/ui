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
using OsmSharp.Osm.Simple;

namespace OsmSharp.Osm.Data.Core.Processor.SimpleSource
{
    /// <summary>
    /// Keeps data in the form of simple primitives in memory.
    /// </summary>
    public class SimpleMemoryData : ISimpleSourceData
    {
        /// <summary>
        /// Holds all the nodes.
        /// </summary>
        private Dictionary<long, SimpleNode> _nodes;

        /// <summary>
        /// Holds all ways.
        /// </summary>
        private Dictionary<long, SimpleWay> _ways;

        /// <summary>
        /// Holds all relations.
        /// </summary>
        private Dictionary<long, SimpleRelation> _relation;

        /// <summary>
        /// Creates a new simple memory data source.
        /// </summary>
        public SimpleMemoryData()
        {
            _nodes = new Dictionary<long, SimpleNode>();
            _ways = new Dictionary<long, SimpleWay>();
            _relation = new Dictionary<long, SimpleRelation>();
        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(SimpleNode node)
        {
            _nodes[node.Id.Value] = node;
        }

        /// <summary>
        /// Updates a node.
        /// </summary>
        /// <param name="node"></param>
        public void UpdateNode(SimpleNode node)
        {
            _nodes[node.Id.Value] = node;
        }

        /// <summary>
        /// Deletes a node.
        /// </summary>
        /// <param name="node_id"></param>
        public void DeleteNode(long node_id)
        {
            _nodes.Remove(node_id);
        }

        /// <summary>
        /// Adds a way.
        /// </summary>
        /// <param name="way"></param>
        public void AddWay(SimpleWay way)
        {
            _ways[way.Id.Value] = way;
        }

        /// <summary>
        /// Updates a way.
        /// </summary>
        /// <param name="way"></param>
        public void UpdateWay(SimpleWay way)
        {
            _ways[way.Id.Value] = way;
        }

        /// <summary>
        /// Deletes a way.
        /// </summary>
        /// <param name="way_id"></param>
        public void DeleteWay(long way_id)
        {
            _ways.Remove(way_id);
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="relation"></param>
        public void AddRelation(SimpleRelation relation)
        {
            _relation[relation.Id.Value] = relation;
        }

        /// <summary>
        /// Updates a relation.
        /// </summary>
        /// <param name="relation"></param>
        public void UpdateRelation(SimpleRelation relation)
        {
            _relation[relation.Id.Value] = relation;
        }

        /// <summary>
        /// Deletes a relation.
        /// </summary>
        /// <param name="relation_id"></param>
        public void DeleteRelation(long relation_id)
        {
            _relation.Remove(relation_id);
        }

        /// <summary>
        /// Returns a node.
        /// </summary>
        /// <param name="node_id"></param>
        /// <returns></returns>
        public SimpleNode GetNode(long node_id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a way.
        /// </summary>
        /// <param name="way_id"></param>
        /// <returns></returns>
        public SimpleWay GetWay(long way_id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all ways for the given node.
        /// </summary>
        /// <param name="node_id"></param>
        /// <returns></returns>
        public IEnumerable<SimpleWay> GetForNode(long node_id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a relation.
        /// </summary>
        /// <param name="relation_id"></param>
        /// <returns></returns>
        public SimpleRelation GetRelation(long relation_id)
        {
            throw new NotImplementedException();
        }
    }
}

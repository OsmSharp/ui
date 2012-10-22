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
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor.SimpleSource
{
    /// <summary>
    /// Keeps data in the form of simple primitives in memory.
    /// </summary>
    public class SimpleMemoryData : ISimpleSourceData
    {
        private Dictionary<long, SimpleNode> _nodes;

        private Dictionary<long, SimpleWay> _ways;

        private Dictionary<long, SimpleRelation> _relation;

        public SimpleMemoryData()
        {
            _nodes = new Dictionary<long, SimpleNode>();
            _ways = new Dictionary<long, SimpleWay>();
            _relation = new Dictionary<long, SimpleRelation>();
        }

        public void AddNode(SimpleNode node)
        {
            _nodes[node.Id.Value] = node;
        }

        public void UpdateNode(SimpleNode node)
        {
            _nodes[node.Id.Value] = node;
        }

        public void DeleteNode(long node_id)
        {
            _nodes.Remove(node_id);
        }

        public void AddWay(SimpleWay way)
        {
            _ways[way.Id.Value] = way;
        }

        public void UpdateWay(SimpleWay way)
        {
            _ways[way.Id.Value] = way;
        }

        public void DeleteWay(long way_id)
        {
            _ways.Remove(way_id);
        }

        public void AddRelation(SimpleRelation relation)
        {
            _relation[relation.Id.Value] = relation;
        }

        public void UpdateRelation(SimpleRelation relation)
        {
            _relation[relation.Id.Value] = relation;
        }

        public void DeleteRelation(long relation_id)
        {
            _relation.Remove(relation_id);
        }

        public SimpleNode GetNode(long node_id)
        {
            throw new NotImplementedException();
        }

        public SimpleWay GetWay(long way_id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SimpleWay> GetForNode(long node_id)
        {
            throw new NotImplementedException();
        }

        public SimpleRelation GetRelation(long relation_id)
        {
            throw new NotImplementedException();
        }
    }
}

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
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Factory;
using OsmSharp.Tools.Collections;

namespace OsmSharp.Osm.Data.Core.Memory
{
    /// <summary>
    /// A memory data source processor target.
    /// </summary>
    internal class MemoryDataSourceProcessorTarget : DataProcessorTarget
    {
        /// <summary>
        /// Holds the memory data source.
        /// </summary>
        private MemoryDataSource _source;

        /// <summary>
        /// Holds the string table.
        /// </summary>
        private ObjectTable<string> _string_table;

        /// <summary>
        /// Creates a memory data processor target.
        /// </summary>
        /// <param name="source"></param>
        public MemoryDataSourceProcessorTarget(MemoryDataSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Creates a memory data processor target.
        /// </summary>
        /// <param name="string_table"></param>
        /// <param name="source"></param>
        public MemoryDataSourceProcessorTarget(ObjectTable<string> string_table, MemoryDataSource source)
        {
            _source = source;
            _string_table = string_table;
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {
            _nodes = new Dictionary<long, Node>();
            _ways = new Dictionary<long, Way>();
            _relations = new Dictionary<long, Relation>();
        }

        /// <summary>
        /// Applying changesets is not supported.
        /// </summary>
        /// <param name="change"></param>
        public override void ApplyChange(SimpleChangeSet change)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Holds all nodes.
        /// </summary>
        private Dictionary<long, OsmSharp.Osm.Node> _nodes; 

        /// <summary>
        /// Adds a given node.
        /// </summary>
        /// <param name="simple_node"></param>
        public override void AddNode(SimpleNode simple_node)
        {
            Node node= OsmBaseFactory.CreateNodeFrom(_string_table, simple_node);
            if (node != null)
            {
                _nodes[node.Id] = node;

                _source.AddNode(node);
            }
        }

        /// <summary>
        /// Holds all ways.
        /// </summary>
        private Dictionary<long, OsmSharp.Osm.Way> _ways; 

        /// <summary>
        /// Adds a given way.
        /// </summary>
        /// <param name="simple_way"></param>
        public override void AddWay(SimpleWay simple_way)
        {
            Way way = OsmBaseFactory.CreateWayFrom(_string_table, simple_way, _nodes);
            if (way != null)
            {
                _ways[way.Id] = way;

                _source.AddWay(way);
            }
        }

        /// <summary>
        /// Holds all relations.
        /// </summary>
        private Dictionary<long, OsmSharp.Osm.Relation> _relations; 

        /// <summary>
        /// Adds a given relation.
        /// </summary>
        /// <param name="simple_relation"></param>
        public override void AddRelation(SimpleRelation simple_relation)
        {
            Relation relation = OsmBaseFactory.CreateRelationFrom(_string_table, simple_relation, _nodes, _ways, _relations);
            if (relation != null)
            {
                _relations[relation.Id] = relation;

                _source.AddRelation(relation);
            }
        }
    }
}

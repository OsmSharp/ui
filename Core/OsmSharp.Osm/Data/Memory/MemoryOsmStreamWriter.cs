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
using OsmSharp.Osm.Data.Core.Memory;
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Osm.Simple;
using OsmSharp.Collections;

namespace OsmSharp.Osm.Data.Memory
{
    /// <summary>
    /// A memory OSM stream writer.
    /// </summary>
    internal class MemoryOsmStreamWriter : OsmStreamWriter
    {
        /// <summary>
        /// Holds the memory data source.
        /// </summary>
        private readonly MemoryDataSource _dataSource;

        /// <summary>
        /// Holds the string table.
        /// </summary>
        private readonly ObjectTable<string> _stringTable;

        /// <summary>
        /// Creates a memory data processor target.
        /// </summary>
        /// <param name="dataSource"></param>
        public MemoryOsmStreamWriter(MemoryDataSource dataSource)
        {
            _dataSource = dataSource;
			_stringTable = new ObjectTable<string> (false);
        }

        /// <summary>
        /// Creates a memory data processor target.
        /// </summary>
        /// <param name="stringTable"></param>
        /// <param name="dataSource"></param>
        public MemoryOsmStreamWriter(ObjectTable<string> stringTable, MemoryDataSource dataSource)
        {
            _dataSource = dataSource;
            _stringTable = stringTable;
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
        /// Holds all nodes.
        /// </summary>
        private Dictionary<long, OsmSharp.Osm.Node> _nodes; 

        /// <summary>
        /// Adds a given node.
        /// </summary>
        /// <param name="simpleNode"></param>
        public override void AddNode(SimpleNode simpleNode)
        {
            Node node= Node.CreateFrom(_stringTable, simpleNode);
            if (node != null)
            {
                _nodes[node.Id] = node;

                _dataSource.AddNode(node);
            }
        }

        /// <summary>
        /// Holds all ways.
        /// </summary>
        private Dictionary<long, OsmSharp.Osm.Way> _ways; 

        /// <summary>
        /// Adds a given way.
        /// </summary>
        /// <param name="simpleWay"></param>
        public override void AddWay(SimpleWay simpleWay)
        {
            Way way = Way.CreateFrom(_stringTable, simpleWay, _nodes);
            if (way != null)
            {
                _ways[way.Id] = way;

                _dataSource.AddWay(way);
            }
        }

        /// <summary>
        /// Holds all relations.
        /// </summary>
        private Dictionary<long, OsmSharp.Osm.Relation> _relations; 

        /// <summary>
        /// Adds a given relation.
        /// </summary>
        /// <param name="simpleRelation"></param>
        public override void AddRelation(SimpleRelation simpleRelation)
        {
            Relation relation = Relation.CreateFrom(_stringTable, simpleRelation, _nodes, _ways, _relations);
            if (relation != null)
            {
                _relations[relation.Id] = relation;

                _dataSource.AddRelation(relation);
            }
        }
    }
}

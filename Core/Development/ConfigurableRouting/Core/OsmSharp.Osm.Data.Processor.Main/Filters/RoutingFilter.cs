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
using OsmSharp.Osm.Core.Simple;
using OsmSharp.Osm.Data.Core.Processor;

namespace OsmSharp.Osm.Data.Processor.Main.Filters
{
    public class RoutingFilter : DataProcessorFilter
    {
        public override void Initialize()
        {
            this.Source.Initialize();
            _nodes_cache = new Dictionary<long, SimpleNode>();
            _stack = new Stack<SimpleOsmGeo>();
            _highway_nodes = null;
        }

        /// <summary>
        /// The cache of nodes.
        /// </summary>
        private Dictionary<long, SimpleNode> _nodes_cache;

        /// <summary>
        /// Holds all the highway node's ids.
        /// </summary>
        private HashSet<long> _highway_nodes;

        /// <summary>
        /// The current stack of objects.
        /// </summary>
        private Stack<SimpleOsmGeo> _stack;

        /// <summary>
        /// Holds the current.
        /// </summary>
        private SimpleOsmGeo _current;

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            // first more the stack.
            if (_stack.Count > 0)
            {
                _current = _stack.Pop();
                return true;
            }

            // first get all highway nodes.
            if (_highway_nodes == null)
            {
                _highway_nodes = new HashSet<long>();
                // get new objects from source.
                int object_counter = 0;
                while (this.Source.MoveNext())
                {
                    object_counter++;

                    if (object_counter % 10000 == 0)
                    {
                        Console.WriteLine("Object {0}", object_counter);
                    }

                    SimpleOsmGeo simple = this.Source.Current();
                    switch (simple.Type)
                    {
                        case SimpleOsmGeoType.Way:
                            if (simple.Tags != null && simple.Tags.ContainsKey("highway"))
                            {
                                SimpleWay way = (simple as SimpleWay);
                                foreach (long node_id in way.Nodes)
                                { // 
                                    _highway_nodes.Add(node_id);
                                }
                            }
                            break;
                    }
                }
                this.Source.Reset();
            }

            // get new objects from source.
            while (this.Source.MoveNext())
            {
                SimpleOsmGeo simple = this.Source.Current();
                switch (simple.Type)
                {
                    case SimpleOsmGeoType.Node:
                        if (_highway_nodes.Contains(simple.Id.Value))
                        {
                            _nodes_cache[simple.Id.Value] = simple as SimpleNode;
                            if (_nodes_cache.Count % 10000 == 0)
                            {
                                Console.WriteLine("Cached Node {0}:{1}%", _nodes_cache.Count, System.Math.Round(((double)_nodes_cache.Count / (double)_highway_nodes.Count) * 100, 2));
                            }
                        }
                        break;
                    case SimpleOsmGeoType.Relation:
                        SimpleRelation relation = (simple as SimpleRelation);
                        if (relation.Tags != null && relation.Tags.ContainsKey("restriction"))
                        {
                            _stack.Push(relation);
                            return this.MoveNext();
                        }
                        break;
                    case SimpleOsmGeoType.Way:
                        SimpleWay way = (simple as SimpleWay);
                        _highway_nodes.Clear();
                        if (way.Tags != null && way.Tags.ContainsKey("highway"))
                        {
                            foreach (long node_id in way.Nodes)
                            { // get node from cache.
                                SimpleNode node = null;
                                if (_nodes_cache.TryGetValue(node_id, out node))
                                {
                                    _stack.Push(node);
                                }
                            }
                            _stack.Push(way);
                            return this.MoveNext();
                        }
                        break;
                }
            }
            return false;
        }

        public override SimpleOsmGeo Current()
        {
            return _current;
        }

        public override void Reset()
        {
            _nodes_cache = new Dictionary<long, SimpleNode>();
            this.Source.Reset();
            _stack = new Stack<SimpleOsmGeo>();
            _highway_nodes = null;
        }

        public override bool CanReset
        {
            get { return this.Source.CanReset; }
        }

    }
}

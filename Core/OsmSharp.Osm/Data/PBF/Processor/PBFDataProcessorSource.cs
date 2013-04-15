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
using System.Text;
using OsmSharp.Osm.Data.Core.Processor;
using System.IO;
using OsmSharp.Osm.Data.PBF.Dense;
using OsmSharp.Osm.Simple;

namespace OsmSharp.Osm.Data.PBF.Raw.Processor
{
    /// <summary>
    /// A source of PBF formatted OSM data.
    /// </summary>
    public class PBFDataProcessorSource : DataProcessorSource, IPBFOsmPrimitiveConsumer
    {
        /// <summary>
        /// Holds the source of the data.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Creates a new source of PBF formated OSM data.
        /// </summary>
        /// <param name="stream"></param>
        public PBFDataProcessorSource(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Initializes the current source.
        /// </summary>
        public override void Initialize()
        {
            _stream.Seek(0, SeekOrigin.Begin);

            this.InitializePBFReader();
        }

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            KeyValuePair<PrimitiveBlock, object> next_pbf_primitive = this.MoveToNextPrimitive();

            if (next_pbf_primitive.Value != null)
            { // there is a primitive.
                _current = this.Convert(next_pbf_primitive);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Holds the current object.
        /// </summary>
        private SimpleOsmGeo _current;

        /// <summary>
        /// Returns the current geometry.
        /// </summary>
        /// <returns></returns>
        public override SimpleOsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Resetting this data source 
        /// </summary>
        public override void Reset()
        {
            _current = null;
            _stream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                return _stream.CanSeek;
            }
        }

        #region Primitive Conversion

        /// <summary>
        /// Converts simple primitives.
        /// </summary>
        /// <param name="pbf_primitive"></param>
        /// <returns></returns>
        internal SimpleOsmGeo Convert(KeyValuePair<PrimitiveBlock, object> pbf_primitive)
        {
            if (pbf_primitive.Value == null || pbf_primitive.Key == null)
            {
                throw new ArgumentNullException("pbf_primitive");
            }

            PrimitiveBlock block = pbf_primitive.Key; // get the block properties this object comes from.
            if (pbf_primitive.Value is Node)
            {
                Node node = (pbf_primitive.Value as Node);
                SimpleNode simple_node = new SimpleNode();
                simple_node.ChangeSetId = node.info.changeset;
                simple_node.Id = node.id;
                simple_node.Latitude = .000000001 * ((double)block.lat_offset 
                    + ((double)block.granularity * (double)node.lat));
                simple_node.Longitude = .000000001 * ((double)block.lon_offset
                    + ((double)block.granularity * (double)node.lon));
                simple_node.Tags = new Dictionary<string, string>();
                for (int tag_idx = 0; tag_idx < node.keys.Count; tag_idx++)
                {
                    string key = ASCIIEncoding.ASCII.GetString(block.stringtable.s[(int)node.keys[tag_idx]]);
                    string value = ASCIIEncoding.ASCII.GetString(block.stringtable.s[(int)node.vals[tag_idx]]);

                    if (!simple_node.Tags.ContainsKey(key))
                    {
                        simple_node.Tags.Add(new KeyValuePair<string, string>(key, value));
                    }
                }
                simple_node.TimeStamp = OsmSharp.Tools.Utilities.FromUnixTime((long)node.info.timestamp * 
                    (long)block.date_granularity);
                simple_node.Visible = true;
                simple_node.Version = (uint)node.info.version;
                simple_node.UserId = node.info.uid;
                simple_node.UserName = ASCIIEncoding.ASCII.GetString(block.stringtable.s[node.info.user_sid]);
                simple_node.Version = (ulong)node.info.version;
                simple_node.Visible = true;

                return simple_node;
            }
            else if (pbf_primitive.Value is Way)
            {
                Way way = (pbf_primitive.Value as Way);

                SimpleWay simple_way = new SimpleWay();
                simple_way.Id = way.id;
                simple_way.Nodes = new List<long>(way.refs.Count);
                long node_id = 0;
                for (int node_idx = 0; node_idx < way.refs.Count; node_idx++)
                {
                    node_id = node_id + way.refs[node_idx];
                    simple_way.Nodes.Add(node_id);
                }
                simple_way.Tags = new Dictionary<string, string>();
                for (int tag_idx = 0; tag_idx < way.keys.Count; tag_idx++)
                {
                    string key = ASCIIEncoding.ASCII.GetString(block.stringtable.s[(int)way.keys[tag_idx]]);
                    string value = ASCIIEncoding.ASCII.GetString(block.stringtable.s[(int)way.vals[tag_idx]]);
                    
                    if (!simple_way.Tags.ContainsKey(key))
                    {
                        simple_way.Tags.Add(new KeyValuePair<string, string>(key, value));
                    }
                }
                if (way.info != null)
                { // add the metadata if any.
                    simple_way.ChangeSetId = way.info.changeset;
                    simple_way.TimeStamp = OsmSharp.Tools.Utilities.FromUnixTime((long)way.info.timestamp *
                        (long)block.date_granularity);
                    simple_way.UserId = way.info.uid;
                    simple_way.UserName = ASCIIEncoding.ASCII.GetString(block.stringtable.s[way.info.user_sid]);
                    simple_way.Version = (ulong)way.info.version;
                }
                simple_way.Visible = true;

                return simple_way;
            }
            else if (pbf_primitive.Value is Relation)
            {
                Relation relation = (pbf_primitive.Value as Relation);

                SimpleRelation simple_relation = new SimpleRelation();
                simple_relation.Id = relation.id;
                simple_relation.Members = new List<SimpleRelationMember>();
                long member_id = 0;
                for (int member_idx = 0; member_idx < relation.types.Count; member_idx++)
                {
                    member_id = member_id + relation.memids[member_idx];
                    string role = ASCIIEncoding.ASCII.GetString(
                        block.stringtable.s[relation.roles_sid[member_idx]]);
                    SimpleRelationMember member = new SimpleRelationMember();
                    member.MemberId = member_id;
                    member.MemberRole = role;
                    switch(relation.types[member_idx])
                    {
                        case Relation.MemberType.NODE:
                            member.MemberType = SimpleRelationMemberType.Node;
                            break;
                        case Relation.MemberType.WAY:
                            member.MemberType = SimpleRelationMemberType.Way;
                            break;
                        case Relation.MemberType.RELATION:
                            member.MemberType = SimpleRelationMemberType.Relation;
                            break;
                    }

                    simple_relation.Members.Add(member);
                }
                simple_relation.Tags = new Dictionary<string, string>();
                for (int tag_idx = 0; tag_idx < relation.keys.Count; tag_idx++)
                {
                    string key = ASCIIEncoding.ASCII.GetString(block.stringtable.s[(int)relation.keys[tag_idx]]);
                    string value = ASCIIEncoding.ASCII.GetString(block.stringtable.s[(int)relation.vals[tag_idx]]);

                    if (!simple_relation.Tags.ContainsKey(key))
                    {
                        simple_relation.Tags.Add(new KeyValuePair<string, string>(key, value));
                    }
                }
                if (relation.info != null)
                { // read metadata if any.
                    simple_relation.ChangeSetId = relation.info.changeset;
                    simple_relation.TimeStamp = OsmSharp.Tools.Utilities.FromUnixTime((long)relation.info.timestamp *
                        (long)block.date_granularity);
                    simple_relation.UserId = relation.info.uid;
                    simple_relation.UserName = ASCIIEncoding.ASCII.GetString(block.stringtable.s[relation.info.user_sid]);
                    simple_relation.Version = (ulong)relation.info.version;
                }
                simple_relation.Visible = true;

                return simple_relation;
            }
            throw new Exception(string.Format("PBF primitive with type {0} not supported!",
                pbf_primitive.GetType().ToString()));
        }

        #endregion

        #region PBF Blocks Reader

        /// <summary>
        /// Holds the PBF reader.
        /// </summary>
        private PBFReader _reader;

        /// <summary>
        /// Holds the primitives decompressor.
        /// </summary>
        private Decompressor _decompressor;

        /// <summary>
        /// Initializes the PBF reader.
        /// </summary>
        private void InitializePBFReader()
        {
            _reader = new PBFReader(_stream);
            _decompressor = new Decompressor(this);

            this.InitializeBlockCache();
        }

        /// <summary>
        /// Moves the PBF reader to the next primitive or returns one of the cached ones.
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<PrimitiveBlock, object> MoveToNextPrimitive()
        {
            KeyValuePair<PrimitiveBlock, object> next = this.DeQueuePrimitive();
            if (next.Value == null)
            {
                PrimitiveBlock block = _reader.MoveNext();
                if (block != null)
                {
                    _decompressor.ProcessPrimitiveBlock(block);
                    next = this.DeQueuePrimitive();
                }
            }
            return next;
        }

        #region Block Cache

        /// <summary>
        /// Holds the cached primitives.
        /// </summary>
        private Queue<KeyValuePair<PrimitiveBlock, object>> _cached_primitives;

        /// <summary>
        /// Initializes the block cache.
        /// </summary>
        private void InitializeBlockCache()
        {
            _cached_primitives = new Queue<KeyValuePair<PrimitiveBlock, object>>();
        }

        /// <summary>
        /// Queues the primitives.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="primitive"></param>
        private void QueuePrimitive(PrimitiveBlock block, object primitive)
        {
            _cached_primitives.Enqueue(new KeyValuePair<PrimitiveBlock, object>(block, primitive));
        }

        /// <summary>
        /// DeQueues a primitive.
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<PrimitiveBlock, object> DeQueuePrimitive()
        {
            if (_cached_primitives.Count > 0)
            {
                return _cached_primitives.Dequeue();
            }
            return new KeyValuePair<PrimitiveBlock, object>();
        }

        #endregion

        #endregion

        /// <summary>
        /// Processes a node.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="node"></param>
        void IPBFOsmPrimitiveConsumer.ProcessNode(PrimitiveBlock block, Node node)
        {
            this.QueuePrimitive(block, node);
        }

        /// <summary>
        /// Processes a way.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="way"></param>
        void IPBFOsmPrimitiveConsumer.ProcessWay(PrimitiveBlock block, Way way)
        {
            this.QueuePrimitive(block, way);
        }

        /// <summary>
        /// Processes a relation.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="relation"></param>
        void IPBFOsmPrimitiveConsumer.ProcessRelation(PrimitiveBlock block, Relation relation)
        {
            this.QueuePrimitive(block, relation);
        }
    }
}

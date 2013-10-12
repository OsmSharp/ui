// OsmSharp - OpenStreetMap (OSM) SDK
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
using OsmSharp.Osm.Cache;
using OsmSharp.Collections.LongIndex.LongIndex;

namespace OsmSharp.Osm.Streams.Complete
{
    /// <summary>
    /// Represents a complete stream source that converts a simple stream into a complete stream.
    /// </summary>
    public class OsmSimpleCompleteStreamSource : OsmCompleteStreamSource
    {
        /// <summary>
        /// Caches objects that are needed later to complete objects.
        /// </summary>
        private readonly OsmDataCache _dataCache;

        /// <summary>
        /// Holds the simple source of object.
        /// </summary>
        private readonly OsmStreamSource _simpleSource;

        /// <summary>
        /// Creates a new osm simple complete stream.
        /// </summary>
        /// <param name="source"></param>
        public OsmSimpleCompleteStreamSource(OsmStreamSource source)
        {
            // create an in-memory cache by default.
            _dataCache = new OsmDataCacheMemory();
            _simpleSource = source;

            _nodesToInclude = new LongIndex();
            //_nodesUsedTwiceOrMore = new Dictionary<long, int>();
            _waysToInclude = new LongIndex();
            //_waysUsedTwiceOrMore = new Dictionary<long, int>();
            _relationsToInclude = new LongIndex();
            //_relationsUsedTwiceOrMore = new Dictionary<long, int>();
        }

        /// <summary>
        /// Creates a new osm simple complete stream.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cache"></param>
        public OsmSimpleCompleteStreamSource(OsmStreamSource source, OsmDataCache cache)
        {
            _dataCache = cache;
            _simpleSource = source;

            _nodesToInclude = new LongIndex();
            //_nodesUsedTwiceOrMore = new Dictionary<long, int>();
            _waysToInclude = new LongIndex();
            //_waysUsedTwiceOrMore = new Dictionary<long, int>();
            _relationsToInclude = new LongIndex();
            //_relationsUsedTwiceOrMore = new Dictionary<long, int>();
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            _cachingDone = false;
            _nodesToInclude.Clear();
            //_nodesUsedTwiceOrMore.Clear();
            _waysToInclude.Clear();
            //_waysUsedTwiceOrMore.Clear();
            _relationsToInclude.Clear();
            //_relationsUsedTwiceOrMore.Clear();

            if (!_simpleSource.CanReset)
            { // the simple source cannot be reset, each object can be a child, no other option than caching everything!
                // TODO: support this scenario, can be usefull when streaming data from a non-seekable stream.
                throw new NotSupportedException("Creating a complete stream from a non-resettable simple stream is not supported. Wrap the source stream and create a resettable stream.");
            }
            else
            { // the simple source can be reset.

            }
        }

        /// <summary>
        /// Flag indicating that the caching was done.
        /// </summary>
        private bool _cachingDone;

        /// <summary>
        /// Holds the current complete object.
        /// </summary>
        private CompleteOsmGeo _current;

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            if (!_cachingDone)
            { // first seek & cache.
                this.Seek();
                this.Cache();
            }

            if (_simpleSource.MoveNext())
            { // there is data.
                OsmGeo currentSimple = _simpleSource.Current();

                switch (currentSimple.Type)
                {
                    case OsmGeoType.Node:
                        _current = CompleteNode.CreateFrom(currentSimple as Node);
                        break;
                    case OsmGeoType.Way:
                        _current = CompleteWay.CreateFrom(currentSimple as Way, _dataCache);
                        break;
                    case OsmGeoType.Relation:
                        _current = CompleteRelation.CreateFrom(currentSimple as Relation, _dataCache);
                        break;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// An index of extra nodes to include.
        /// </summary>
        private readonly LongIndex _nodesToInclude =
            new LongIndex();

        /// <summary>
        /// An index of extra relations to include.
        /// </summary>
        private readonly LongIndex _waysToInclude =
            new LongIndex();

        /// <summary>
        /// An index of extra ways to include.
        /// </summary>
        private readonly LongIndex _relationsToInclude =
            new LongIndex();

        /// <summary>
        /// Seeks for objects that are children of other objects.
        /// </summary>
        private void Seek()
        {
            foreach (OsmGeo osmGeo in _simpleSource)
            {
                switch (osmGeo.Type)
                {
                    case OsmGeoType.Way:
                        foreach (long nodeId in (osmGeo as Way).Nodes)
                        {
                            this.MarkNodeAsChild(nodeId);
                        }
                        break;
                    case OsmGeoType.Relation:
                        foreach (RelationMember member in (osmGeo as Relation).Members)
                        {
                            switch (member.MemberType.Value)
                            {
                                case OsmGeoType.Node:
                                    this.MarkNodeAsChild(member.MemberId.Value);
                                    break;
                                case OsmGeoType.Way:
                                    this.MarkWayAsChild(member.MemberId.Value);
                                    break;
                                case OsmGeoType.Relation:
                                    this.MarkRelationAsChild(member.MemberId.Value);
                                    break;
                            }
                        }
                        break;
                }
            }
            _simpleSource.Reset();
        }

        /// <summary>
        /// Marks the given node as child.
        /// </summary>
        /// <param name="nodeId"></param>
        private void MarkNodeAsChild(long nodeId)
        {
            if (_nodesToInclude.Contains(nodeId))
            { // increase the node counter.
                //int nodeCount;
                //if (!_nodesUsedTwiceOrMore.TryGetValue(nodeId, out nodeCount))
                //{ // the node is used twice or more.
                //    nodeCount = 1;
                //    _nodesUsedTwiceOrMore.Add(nodeId, nodeCount);
                //}
            }
            else
            {
                // just add the node.
                _nodesToInclude.Add(nodeId);
            }
        }

        /// <summary>
        /// Marks the given way as child.
        /// </summary>
        /// <param name="wayId"></param>
        private void MarkWayAsChild(long wayId)
        {
            if (_waysToInclude.Contains(wayId))
            { // increase the way counter.
                //int wayCount;
                //if (!_waysUsedTwiceOrMore.TryGetValue(wayId, out wayCount))
                //{ // the way is used twice or more.
                //    wayCount = 1;
                //    _waysUsedTwiceOrMore.Add(wayId, wayCount);
                //}
            }
            else
            {
                // just add the way.
                _waysToInclude.Add(wayId);
            }
        }

        /// <summary>
        /// Marks the given relation as child.
        /// </summary>
        /// <param name="relationId"></param>
        private void MarkRelationAsChild(long relationId)
        {
            if (_relationsToInclude.Contains(relationId))
            { // increase the relation counter.
                //int relationCount;
                //if (!_relationsUsedTwiceOrMore.TryGetValue(relationId, out relationCount))
                //{ // the relation is used twice or more.
                //    relationCount = 1;
                //    _relationsUsedTwiceOrMore.Add(relationId, relationCount);
                //}
            }
            else
            {
                // just add the relation.
                _relationsToInclude.Add(relationId);
            }
        }

        /// <summary>
        /// Cache all needed objects;
        /// </summary>
        private void Cache()
        {
            foreach (OsmGeo osmGeo in _simpleSource)
            {
                switch (osmGeo.Type)
                {
                    case OsmGeoType.Node:
                        if (this.DoesNodeNeedCache(osmGeo.Id.Value))
                        { // yep, cache node!
                            _dataCache.AddNode(osmGeo as Node);
                        }
                        break;
                    case OsmGeoType.Way:
                        if (this.DoesWayNeedCache(osmGeo.Id.Value))
                        { // yep, cache way!
                            _dataCache.AddWay(osmGeo as Way);
                        }
                        break;
                    case OsmGeoType.Relation:
                        if (this.DoesRelationNeedCache(osmGeo.Id.Value))
                        { // yep, cache relation!
                            _dataCache.AddRelation(osmGeo as Relation);
                        }
                        break;
                }
            }
            _simpleSource.Reset();
        }

        /// <summary>
        /// Returns true if the given node needs to be cached.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        private bool DoesNodeNeedCache(long nodeId)
        {
            return _nodesToInclude.Contains(nodeId);
        }

        /// <summary>
        /// Returns true if the given way needs to be cached.
        /// </summary>
        /// <param name="wayId"></param>
        /// <returns></returns>
        private bool DoesWayNeedCache(long wayId)
        {
            return _waysToInclude.Contains(wayId);
        }

        /// <summary>
        /// Returns true if the given relation needs to be cached.
        /// </summary>
        /// <param name="relationId"></param>
        /// <returns></returns>
        private bool DoesRelationNeedCache(long relationId)
        {
            return _relationsToInclude.Contains(relationId);
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override CompleteOsmGeo Current()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Resets this source.
        /// </summary>
        public override void Reset()
        {
            _cachingDone = false;

            _dataCache.Clear();
            _simpleSource.Reset();
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return _simpleSource.CanReset; }
        }
    }
}

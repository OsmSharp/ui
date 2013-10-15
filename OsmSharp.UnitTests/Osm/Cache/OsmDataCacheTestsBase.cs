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

using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Osm.Cache;

namespace OsmSharp.UnitTests.Osm.Cache
{
    /// <summary>
    /// Base class containing test code for any OsmDataCache implementation.
    /// </summary>
    public class OsmDataCacheTestsBase
    {
        /// <summary>
        /// Tests a simple node read/write operation.
        /// </summary>
        /// <param name="cache"></param>
        public void DoOsmDataCacheTestNode(OsmDataCache cache)
        {
            Node node = Node.Create(1, new SimpleTagsCollection(
                Tag.Create("node", "yes")), 1, 2);

            cache.AddNode(node);

            Assert.IsTrue(cache.ContainsNode(node.Id.Value));
            Node readNode = cache.GetNode(node.Id.Value);
            Assert.IsNotNull(readNode);
            Assert.AreEqual(1, readNode.Id.Value);
            Assert.AreEqual(1, readNode.Latitude.Value);
            Assert.AreEqual(2, readNode.Longitude.Value);
            Assert.IsNotNull(node.Tags);
            Assert.AreEqual(1, node.Tags.Count);
            Assert.AreEqual("yes", node.Tags["node"]);

            Assert.IsTrue(cache.TryGetNode(node.Id.Value, out readNode));
            Assert.IsNotNull(readNode);
            Assert.AreEqual(1, readNode.Id.Value);
            Assert.AreEqual(1, readNode.Latitude.Value);
            Assert.AreEqual(2, readNode.Longitude.Value);
            Assert.IsNotNull(node.Tags);
            Assert.AreEqual(1, node.Tags.Count);
            Assert.AreEqual("yes", node.Tags["node"]);

            cache.RemoveNode(node.Id.Value);
            Assert.IsFalse(cache.ContainsNode(node.Id.Value));
        }

        /// <summary>
        /// Tests a simple way read/write operation.
        /// </summary>
        /// <param name="cache"></param>
        public void DoOsmDataCacheTestWay(OsmDataCache cache)
        {
            Way way = Way.Create(1, new SimpleTagsCollection(
                Tag.Create("way", "yes")), 1, 2);

            cache.AddWay(way);

            Assert.IsTrue(cache.ContainsWay(way.Id.Value));
            Way readWay = cache.GetWay(way.Id.Value);
            Assert.IsNotNull(readWay);
            Assert.AreEqual(1, readWay.Id.Value);
            Assert.IsNotNull(way.Tags);
            Assert.AreEqual(1, way.Tags.Count);
            Assert.AreEqual("yes", way.Tags["way"]);

            Assert.IsTrue(cache.TryGetWay(way.Id.Value, out readWay));
            Assert.IsNotNull(readWay);
            Assert.AreEqual(1, readWay.Id.Value);
            Assert.IsNotNull(way.Tags);
            Assert.AreEqual(1, way.Tags.Count);
            Assert.AreEqual("yes", way.Tags["way"]);

            cache.RemoveNode(way.Id.Value);
            Assert.IsFalse(cache.ContainsNode(way.Id.Value));
        }

        /// <summary>
        /// Tests a simple relation read/write operation.
        /// </summary>
        /// <param name="cache"></param>
        public void DoOsmDataCacheTestRelation(OsmDataCache cache)
        {
            Relation relation = Relation.Create(1, new SimpleTagsCollection(
                Tag.Create("relation", "yes")), RelationMember.Create(1, "something", OsmGeoType.Node));

            cache.AddRelation(relation);

            Assert.IsTrue(cache.ContainsRelation(relation.Id.Value));
            Relation readRelation = cache.GetRelation(relation.Id.Value);
            Assert.IsNotNull(readRelation);
            Assert.AreEqual(1, readRelation.Id.Value);
            Assert.IsNotNull(relation.Tags);
            Assert.AreEqual(1, relation.Tags.Count);
            Assert.AreEqual("yes", relation.Tags["relation"]);

            Assert.IsTrue(cache.TryGetRelation(relation.Id.Value, out readRelation));
            Assert.IsNotNull(readRelation);
            Assert.AreEqual(1, readRelation.Id.Value);
            Assert.IsNotNull(relation.Tags);
            Assert.AreEqual(1, relation.Tags.Count);
            Assert.AreEqual("yes", relation.Tags["relation"]);

            cache.RemoveNode(relation.Id.Value);
            Assert.IsFalse(cache.ContainsNode(relation.Id.Value));
        }
    }
}

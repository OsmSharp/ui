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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OsmSharp.Osm.Data.Core.Memory;
using OsmSharp.Osm.Factory;
using OsmSharp.Osm;

namespace OsmSharp.Osm.UnitTests
{
    /// <summary>
    /// Does ome raw data memory tests.
    /// </summary>
    [TestFixture]
    public class DykstraMemoryDataSourceTests
    {
        /// <summary>
        /// Tests adding a node to the memory source.
        /// </summary>
        [Test]
        public void TestAddNode()
        {
            Node test_node = OsmBaseFactory.CreateNode(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddNode(test_node);

            // test if the node is actually there.
            Assert.AreEqual(test_node, source.GetNode(-1));

            // test if the node was not remove after getting it.
            Assert.AreEqual(test_node, source.GetNode(-1));
             
            // test if the node will be retrieved using a list of ids.
            List<long> ids = new List<long>();
            ids.Add(-1);
            IList<Node> nodes = source.GetNodes(ids);
            Assert.IsNotNull(nodes);
            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual(test_node, nodes[0]);
        }

        /// <summary>
        /// Tests removing a node.
        /// </summary>
        [Test]
        public void TestRemoveNode()
        {
            Node test_node = OsmBaseFactory.CreateNode(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddNode(test_node);

            // test if the node is actually there.
            Assert.AreEqual(test_node, source.GetNode(-1));

            // remove the node.
            source.RemoveNode(-1);

            // test if the node is actually gone.
            Assert.IsNull(source.GetNode(-1));
        }

        /// <summary>
        /// Tests adding a way to the memory source.
        /// </summary>
        [Test]
        public void TestAddWay()
        {
            Way test_way = OsmBaseFactory.CreateWay(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddWay(test_way);

            // test if the way is actually there.
            Assert.AreEqual(test_way, source.GetWay(-1));

            // test if the way was not remove after getting it.
            Assert.AreEqual(test_way, source.GetWay(-1));

            // test if the way will be retrieved using a list of ids.
            List<long> ids = new List<long>();
            ids.Add(-1);
            IList<Way> ways = source.GetWays(ids);
            Assert.IsNotNull(ways);
            Assert.AreEqual(1, ways.Count);
            Assert.AreEqual(test_way, ways[0]);
        }

        /// <summary>
        /// Tests removing a way.
        /// </summary>
        [Test]
        public void TestRemoveWay()
        {
            Way test_way = OsmBaseFactory.CreateWay(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddWay(test_way);

            // test if the way is actually there.
            Assert.AreEqual(test_way, source.GetWay(-1));

            // remove the way.
            source.RemoveWay(-1);

            // test if the way is actually gone.
            Assert.IsNull(source.GetWay(-1));
        }

        /// <summary>
        /// Tests adding a relation to the memory source.
        /// </summary>
        [Test]
        public void TestAddRelation()
        {
            Relation test_relation = OsmBaseFactory.CreateRelation(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddRelation(test_relation);

            // test if the relation is actually there.
            Assert.AreEqual(test_relation, source.GetRelation(-1));

            // test if the relation was not remove after getting it.
            Assert.AreEqual(test_relation, source.GetRelation(-1));

            // test if the relation will be retrieved using a list of ids.
            List<long> ids = new List<long>();
            ids.Add(-1);
            IList<Relation> relations = source.GetRelations(ids);
            Assert.IsNotNull(relations);
            Assert.AreEqual(1, relations.Count);
            Assert.AreEqual(test_relation, relations[0]);
        }

        /// <summary>
        /// Tests removing a relation.
        /// </summary>
        [Test]
        public void TestRemoveRelation()
        {
            Relation test_relation = OsmBaseFactory.CreateRelation(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddRelation(test_relation);

            // test if the relation is actually there.
            Assert.AreEqual(test_relation, source.GetRelation(-1));

            // remove the relation.
            source.RemoveRelation(-1);

            // test if the relation is actually gone.
            Assert.IsNull(source.GetRelation(-1));
        }

        /// <summary>
        /// Tests if adding a way with given nodes actually returns those nodes.
        /// </summary>
        [Test]
        public void TestWayNodeRelation()
        {
            Way test_way = OsmBaseFactory.CreateWay(-1);
            Node node1 = OsmBaseFactory.CreateNode(-1);
            test_way.Nodes.Add(node1);
            Node node2 = OsmBaseFactory.CreateNode(-2);
            test_way.Nodes.Add(node2);
            MemoryDataSource source = new MemoryDataSource();
            source.AddWay(test_way);

            // test if node1 is present.
            Assert.AreEqual(node1, source.GetNode(-1));
            Assert.AreEqual(node2, source.GetNode(-2));
            Assert.AreEqual(test_way, source.GetWay(-1));

            // test if the way is present in the node's way list.
            IList<Way> ways = source.GetWaysFor(node1);
            Assert.IsNotNull(ways);
            Assert.IsTrue(ways.Contains(test_way));
            ways = source.GetWaysFor(node2);
            Assert.IsTrue(ways.Contains(test_way));
        }
    }
}
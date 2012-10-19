using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osm.Data.Core.Raw.Memory;
using Osm.Core.Factory;
using Osm.Core;

namespace Osm.UnitTests
{
    [TestClass]
    public class RawMemoryDataSourceTests
    {
        /// <summary>
        /// Tests adding a node to the memory source.
        /// </summary>
        [TestMethod]
        public void TestAddNode()
        {
            Node test_node = OsmBaseFactory.CreateNode(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddNode(test_node);

            // test if the node is actually there.
            Assert.AreEqual<Node>(test_node, source.GetNode(-1));

            // test if the node was not remove after getting it.
            Assert.AreEqual<Node>(test_node, source.GetNode(-1));
             
            // test if the node will be retrieved using a list of ids.
            List<long> ids = new List<long>();
            ids.Add(-1);
            IList<Node> nodes = source.GetNodes(ids);
            Assert.IsNotNull(nodes);
            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual<Node>(test_node, nodes[0]);
        }

        /// <summary>
        /// Tests removing a node.
        /// </summary>
        [TestMethod]
        public void TestRemoveNode()
        {
            Node test_node = OsmBaseFactory.CreateNode(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddNode(test_node);

            // test if the node is actually there.
            Assert.AreEqual<Node>(test_node, source.GetNode(-1));

            // remove the node.
            source.RemoveNode(-1);

            // test if the node is actually gone.
            Assert.IsNull(source.GetNode(-1));
        }

        /// <summary>
        /// Tests adding a way to the memory source.
        /// </summary>
        [TestMethod]
        public void TestAddWay()
        {
            Way test_way = OsmBaseFactory.CreateWay(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddWay(test_way);

            // test if the way is actually there.
            Assert.AreEqual<Way>(test_way, source.GetWay(-1));

            // test if the way was not remove after getting it.
            Assert.AreEqual<Way>(test_way, source.GetWay(-1));

            // test if the way will be retrieved using a list of ids.
            List<long> ids = new List<long>();
            ids.Add(-1);
            IList<Way> ways = source.GetWays(ids);
            Assert.IsNotNull(ways);
            Assert.AreEqual(1, ways.Count);
            Assert.AreEqual<Way>(test_way, ways[0]);
        }

        /// <summary>
        /// Tests removing a way.
        /// </summary>
        [TestMethod]
        public void TestRemoveWay()
        {
            Way test_way = OsmBaseFactory.CreateWay(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddWay(test_way);

            // test if the way is actually there.
            Assert.AreEqual<Way>(test_way, source.GetWay(-1));

            // remove the way.
            source.RemoveWay(-1);

            // test if the way is actually gone.
            Assert.IsNull(source.GetWay(-1));
        }

        /// <summary>
        /// Tests adding a relation to the memory source.
        /// </summary>
        [TestMethod]
        public void TestAddRelation()
        {
            Relation test_relation = OsmBaseFactory.CreateRelation(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddRelation(test_relation);

            // test if the relation is actually there.
            Assert.AreEqual<Relation>(test_relation, source.GetRelation(-1));

            // test if the relation was not remove after getting it.
            Assert.AreEqual<Relation>(test_relation, source.GetRelation(-1));

            // test if the relation will be retrieved using a list of ids.
            List<long> ids = new List<long>();
            ids.Add(-1);
            IList<Relation> relations = source.GetRelations(ids);
            Assert.IsNotNull(relations);
            Assert.AreEqual(1, relations.Count);
            Assert.AreEqual<Relation>(test_relation, relations[0]);
        }

        /// <summary>
        /// Tests removing a relation.
        /// </summary>
        [TestMethod]
        public void TestRemoveRelation()
        {
            Relation test_relation = OsmBaseFactory.CreateRelation(-1);
            MemoryDataSource source = new MemoryDataSource();
            source.AddRelation(test_relation);

            // test if the relation is actually there.
            Assert.AreEqual<Relation>(test_relation, source.GetRelation(-1));

            // remove the relation.
            source.RemoveRelation(-1);

            // test if the relation is actually gone.
            Assert.IsNull(source.GetRelation(-1));
        }

        /// <summary>
        /// Tests if adding a way with given nodes actually returns those nodes.
        /// </summary>
        [TestMethod]
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
            Assert.AreEqual<Node>(node1, source.GetNode(-1));
            Assert.AreEqual<Node>(node2, source.GetNode(-2));
            Assert.AreEqual<Way>(test_way, source.GetWay(-1));

            // test if the way is present in the node's way list.
            IList<Way> ways = source.GetWaysFor(node1);
            Assert.IsNotNull(ways);
            Assert.IsTrue(ways.Contains(test_way));
            ways = source.GetWaysFor(node2);
            Assert.IsTrue(ways.Contains(test_way));
        }
    }
}
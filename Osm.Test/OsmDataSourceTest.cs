using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Tools.Xml.Sources;
using Osm.Core.Xml;
using Osm.Core;
using Osm.Core.Filters;
using Osm.Data;
using Osm.Data.Raw.XML.OsmSource;

namespace Osm.Test
{
    /// <summary>
    /// Summary description for OsmDataSourceTest
    /// </summary>
    [TestClass]
    public class OsmDataSourceTest
    {
        public OsmDataSourceTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        
         [ClassInitialize()]
         public static void MyClassInitialize(TestContext testContext)
         {
             OsmGeo.ShapeInterperter = new TestShapeInterpreter();
         }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Reads the osm data source.
        /// </summary>
        [TestMethod]
        public void ReadOsmDataSource()
        {            
            // instantiate and load the osm test document and datasource.
            XmlFileSource xml_source = new XmlFileSource(new FileInfo(@"test.osm"));
            OsmDocument document = new OsmDocument(xml_source);
            OsmDataSource data_source = new OsmDataSource(document);

            // test if the source has a bounding box.
            Assert.IsNotNull(data_source.BoundingBox, "Bounding box not loaded!");

            // get the objects from the source.
            IList<OsmBase> objects = data_source.Get(data_source.BoundingBox, Filter.Any());
            Assert.AreEqual(objects.Count, 21465,"Invalid number of objects found!");

            // closes the datasource.
            data_source = null;
            document.Close();
            xml_source.Close();
        }

        /// <summary>
        /// Test the writing of osm data and test if all objects are present in the new source(s).
        /// </summary>
        [TestMethod]
        public void WriteOsmDataSource()
        {
            // there are 24521 objects in this datasource.

            // instantiate and load the osm test document and datasource.
            XmlFileSource xml_source = new XmlFileSource(new FileInfo(@"test.osm"));
            OsmDocument document = new OsmDocument(xml_source);
            OsmDataSource data_source = new OsmDataSource(document);

            // test if the source has a bounding box.
            Assert.IsNotNull(data_source.BoundingBox, "Bounding box not loaded!");

            // get the objects from the source.
            IList<OsmBase> objects = data_source.Get(Filter.Any());
            Assert.AreEqual(objects.Count, 24521, "Invalid number of objects found!");

            // closes the datasource.
            data_source = null;
            document.Close();
            xml_source.Close();

            // create a new data source and write all the objects to it.
            xml_source = new XmlFileSource(new FileInfo(@"test.write.osm"));
            document = new OsmDocument(xml_source);
            data_source = new OsmDataSource(document);

            // adds all the objects to the data source.
            data_source.AddOsmBase(objects);
            objects = data_source.Get(Filter.Any());
            Assert.AreEqual(24521, objects.Count, "Invalid number of objects found!");

            // persist the datasource.
            data_source.Persist();
            data_source = null;
            document.Close();
            xml_source.Close();

            // create a new data source and read all the objects in it.
            xml_source = new XmlFileSource(new FileInfo(@"test.write.osm"));
            document = new OsmDocument(xml_source);
            data_source = new OsmDataSource(document);

            // adds all the objects to the data source.
            objects = data_source.Get(Filter.Any());
            Assert.AreEqual(objects.Count, 24521, "Invalid number of objects found!");
        }

        /// <summary>
        /// Tests the quering of the osm datasource.
        /// </summary>
        [TestMethod]
        public void QueryOsmDataSource()
        {
            // instantiate and load the osm test document and datasource.
            XmlFileSource xml_source = new XmlFileSource(new FileInfo(@"test.osm"));
            OsmDocument document = new OsmDocument(xml_source);
            IDataSource data_source = new OsmDataSource(document);

            // test all the query functions.
            IList<OsmBase> osm_base_list = 
                data_source.Get(data_source.BoundingBox, Filter.Any());
            Assert.AreEqual(21465, osm_base_list.Count);

            // test the node functions.
            Node node = data_source.GetNode(14327556);
            Assert.IsNotNull(node, "GetNode did not return node!");
            Assert.AreEqual(14327556, node.Id, "GetNode returned incorrect node!");
            IList<Node> nodes = data_source.GetNodes(
                new List<long>(new long[] { 14327556, 14327654, 14385127 }));
            Assert.IsNotNull(nodes, "GetNodes did not return results!");
            Assert.AreEqual(3, nodes.Count, "GetNodes did not return the correct number of nodes!");
            Assert.AreEqual(14327556, nodes[0].Id, "GetNodes returned incorrect node!");
            Assert.AreEqual(14327654, nodes[1].Id, "GetNodes returned incorrect node!");
            Assert.AreEqual(14385127, nodes[2].Id, "GetNodes returned incorrect node!");
            
            // test the way functions.
            Way way = data_source.GetWay(24739696);
            Assert.IsNotNull(way, "GetWay did not return way!");
            Assert.AreEqual(24739696, way.Id, "GetWay returned incorrect way!");
            IList<Way> ways = data_source.GetWays(
                new List<long>(new long[] { 24739696, 24739721, 24739789 }));
            Assert.IsNotNull(ways, "GetWays did not return results!");
            Assert.AreEqual(3, ways.Count, "GetWays did not return the correct number of ways!");
            Assert.AreEqual(24739696, ways[0].Id, "GetWays returned incorrect way!");
            Assert.AreEqual(24739721, ways[1].Id, "GetWays returned incorrect way!");
            Assert.AreEqual(24739789, ways[2].Id, "GetWays returned incorrect way!");
            node = data_source.GetNode(288503659);
            ways = data_source.GetWaysFor(node); 
            Assert.IsNotNull(ways, "GetWaysFor did not return results!");
            Assert.AreEqual(4, ways.Count, "GetWayss did not return the correct number of ways!");
            Assert.AreEqual(4904932, ways[0].Id, "GetWays returned incorrect way!");
            Assert.AreEqual(26333008, ways[1].Id, "GetWays returned incorrect way!");
            Assert.AreEqual(27814037, ways[2].Id, "GetWays returned incorrect way!");
            Assert.AreEqual(33892533, ways[3].Id, "GetWays returned incorrect way!");
            
            // test the relation functions.
            Relation relation = data_source.GetRelation(29681);
            Assert.IsNotNull(relation, "GetRelation did not return relation!");
            Assert.AreEqual(29681, relation.Id, "GetRelation returned incorrect relation!");
            IList<Relation> relations = data_source.GetRelations(
                new List<long>(new long[] { 29681, 29682, 30254 }));
            Assert.IsNotNull(relations, "GetRelations did not return results!");
            Assert.AreEqual(3, relations.Count, "GetRelations did not return the correct number of ways!");
            Assert.AreEqual(29681, relations[0].Id, "GetRelations returned incorrect way!");
            Assert.AreEqual(29682, relations[1].Id, "GetRelations returned incorrect way!");
            Assert.AreEqual(30254, relations[2].Id, "GetRelations returned incorrect way!");
            way = data_source.GetWay(23607286);
            relations = data_source.GetRelationsFor(way);
            Assert.IsNotNull(relations, "GetRelations did not return results!");
            Assert.AreEqual(1, relations.Count, "GetRelations did not return the correct number of ways!");
            Assert.AreEqual(29682, relations[0].Id, "GetRelations returned incorrect way!");
        }

        /// <summary>
        /// Creates new osm objects and save them.
        /// </summary>
        [TestMethod]
        public void CreateOsmDataSource()
        {
            // instantiate and load the osm test document and datasource.
            XmlFileSource xml_source = new XmlFileSource(new FileInfo(@"test.create.osm"));
            OsmDocument document = new OsmDocument(xml_source);
            IDataSource data_source = new OsmDataSource(document);

            // create node(s)
            Node node_1 = data_source.CreateNode();
            node_1.Coordinate = new Tools.Math.Geo.GeoCoordinate(
                50.882778167724609f,
                4.283745288848877f);
            Node node_2 = data_source.CreateNode();
            node_2.Coordinate = new Tools.Math.Geo.GeoCoordinate(
                50.8863410949707f,
                4.2801661491394043f);

            // create changeset.
            ChangeSet changes = data_source.CreateChangeSet();
            changes.Changes.Add(new Change(ChangeType.Create, node_1));
            changes.Changes.Add(new Change(ChangeType.Create, node_2));

            // apply the changes.
            data_source.ApplyChangeSet(changes);

            // save the data source.
            data_source.Persist();
        }

        /// <summary>
        /// Tests a modification made to an osm data source.
        /// </summary>
        [TestMethod]
        public void ModifyOsmDataSource()
        {
            Assert.Inconclusive();
        }
    }
}

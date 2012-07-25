using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tools.Xml;
using Tools.Xml.Sources;
using System.IO;
using Tools.Xml.Kml;
using Tools.Math.Geo;
using Osm.Core;
using Osm.Core.Filters;
using Osm.Data.Raw.XML.KmlSource;

namespace Osm.Test
{
    /// <summary>
    /// Summary description for KmlDataSourceTest
    /// </summary>
    [TestClass]
    public class KmlDataSourceTest
    {
        public KmlDataSourceTest()
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

        [TestMethod]
        public void Kmlv2_1ReadDataSource()
        {
            IXmlSource xml_source = new XmlFileSource(new FileInfo("test.v2.1.kml"));
            KmlDataSource kml_source = new KmlDataSource(new KmlDocument(xml_source));

            // check the bounding box.
            GeoCoordinateBox box = kml_source.BoundingBox;
            Assert.AreEqual(51.0206223f, box.MaxLat);
            Assert.AreEqual(6.116185f, box.MaxLon);
            Assert.AreEqual(50.34355f, box.MinLat);
            Assert.AreEqual(3.94341f, box.MinLon);

            // check the object counts!
            IList<OsmBase> nodes = kml_source.Get(Filter.Type(OsmType.Node));
            Assert.AreEqual(832, nodes.Count);
            IList<OsmBase> ways = kml_source.Get(Filter.Type(OsmType.Way));
            Assert.AreEqual(23, ways.Count);
            IList<OsmBase> relations = kml_source.Get(Filter.Type(OsmType.Relation));
            Assert.AreEqual(5, relations.Count);
        }

        [TestMethod]
        public void Kmlv2_0ResponseReadDataSource()
        {
            IXmlSource xml_source = new XmlFileSource(new FileInfo("test.v2.0.response.kml"));
            KmlDataSource kml_source = new KmlDataSource(new KmlDocument(xml_source));

            // check the bounding box.
            GeoCoordinateBox box = kml_source.BoundingBox;
            Assert.AreEqual(51.08422f, box.MaxLat);
            Assert.AreEqual(3.76681614f, box.MaxLon);
            Assert.AreEqual(50.92977f, box.MinLat);
            Assert.AreEqual(3.443305f, box.MinLon);

            // check the object counts!
            IList<OsmBase> nodes = kml_source.Get(Filter.Type(OsmType.Node));
            Assert.AreEqual(7, nodes.Count);
            IList<OsmBase> ways = kml_source.Get(Filter.Type(OsmType.Way));
            Assert.AreEqual(0, ways.Count);
            IList<OsmBase> relations = kml_source.Get(Filter.Type(OsmType.Relation));
            Assert.AreEqual(8, relations.Count);
        }

        [TestMethod]
        public void Kmlv2_0ReadDataSource()
        {
            IXmlSource xml_source = new XmlFileSource(new FileInfo("test.v2.0.kml"));
            KmlDataSource kml_source = new KmlDataSource(new KmlDocument(xml_source));

            // check the bounding box.
            GeoCoordinateBox box = kml_source.BoundingBox;
            Assert.AreEqual(40.7141724f, box.MaxLat);
            Assert.AreEqual(-74.00639f, box.MaxLon);
            Assert.AreEqual(40.7141724f, box.MinLat);
            Assert.AreEqual(-74.00639f, box.MinLon);

            IList<OsmBase> nodes = kml_source.Get(Filter.Type(OsmType.Node));
            Assert.AreEqual(1, nodes.Count);
            IList<OsmBase> ways = kml_source.Get(Filter.Type(OsmType.Way));
            Assert.AreEqual(0, ways.Count);
            IList<OsmBase> relations = kml_source.Get(Filter.Type(OsmType.Relation));
            Assert.AreEqual(1, relations.Count);
        }
    }
}

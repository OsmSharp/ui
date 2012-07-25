using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tools.Xml;
using Tools.Xml.Sources;
using System.IO;
using Tools.Xml.Gpx;
using Tools.Math.Geo;
using Osm.Core;
using Osm.Core.Filters;
using Osm.Data.Raw.XML.GpxSource;

namespace Osm.Test
{
    /// <summary>
    /// Summary description for GpxDataSourceTest
    /// </summary>
    [TestClass]
    public class GpxDataSourceTest
    {
        public GpxDataSourceTest()
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
        public void Gpxv1_0ReadDataSource()
        {
            IXmlSource xml_source = new XmlFileSource(new FileInfo("test.v1.0.gpx"));
            GpxDataSource kml_source = new GpxDataSource(new GpxDocument(xml_source));

            // check the bounding box.
            GeoCoordinateBox box = kml_source.BoundingBox;
            Assert.AreEqual(51.2689743f, box.MaxLat);
            Assert.AreEqual(4.80173969f, box.MaxLon);
            Assert.AreEqual(51.2611f, box.MinLat);
            Assert.AreEqual(4.78863668f, box.MinLon);

            IList<OsmBase> nodes = kml_source.Get(Filter.Type(OsmType.Node));
            Assert.AreEqual(424, nodes.Count);
            IList<OsmBase> ways = kml_source.Get(Filter.Type(OsmType.Way));
            Assert.AreEqual(1, ways.Count);
            IList<OsmBase> relations = kml_source.Get(Filter.Type(OsmType.Relation));
            Assert.AreEqual(1, relations.Count);
        }

        [TestMethod]
        public void Gpxv1_1ReadDataSource()
        {
            IXmlSource xml_source = new XmlFileSource(new FileInfo("test.v1.1.gpx"));
            GpxDataSource kml_source = new GpxDataSource(new GpxDocument(xml_source));

            // check the bounding box.
            GeoCoordinateBox box = kml_source.BoundingBox;
            Assert.AreEqual(47.6445465f, box.MaxLat);
            Assert.AreEqual(-122.3269f, box.MaxLon);
            Assert.AreEqual(47.6445465f, box.MinLat);
            Assert.AreEqual(-122.3269f, box.MinLon);

            IList<OsmBase> nodes = kml_source.Get(Filter.Type(OsmType.Node));
            Assert.AreEqual(3, nodes.Count);
            IList<OsmBase> ways = kml_source.Get(Filter.Type(OsmType.Way));
            Assert.AreEqual(1, ways.Count);
            IList<OsmBase> relations = kml_source.Get(Filter.Type(OsmType.Relation));
            Assert.AreEqual(2, relations.Count);
        }
    }
}

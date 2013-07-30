using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Osm.Interpreter;
using OsmSharp.Osm;
using OsmSharp.Geo.Geometries;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Collections.Tags;

namespace OsmSharp.UnitTests.Osm.Interpreter
{
    /// <summary>
    /// Contains tests for the default geometry interpreter class testing as many of the openstreetmap tags ->  geometry logic
    /// as possible.
    /// </summary>
    [TestFixture]
    public class SimpleGeometryInterpreterTests
    {
        /// <summary>
        /// Tests the interpretation of an area.
        /// Way(area=yes) => area
        /// 
        /// http://wiki.openstreetmap.org/wiki/Area#A_simple_area
        /// </summary>
        [Test]
        public void TestWayAreaIsYesArea()
        {
            Node node1 = new Node();
            node1.Id = 1;
            node1.Latitude = 0;
            node1.Longitude = 0;
            Node node2 = new Node();
            node2.Id = 2;
            node2.Latitude = 1;
            node2.Longitude = 0;
            Node node3 = new Node();
            node3.Id = 3;
            node3.Latitude = 0;
            node3.Longitude = 1;

            Way way = new Way();
            way.Id = 1;
            way.Nodes = new List<long>();
            way.Nodes.Add(1);
            way.Nodes.Add(2);
            way.Nodes.Add(3);
            way.Nodes.Add(1);
            way.Tags = new SimpleTagsCollection();
            way.Tags.Add("area", "yes");

            MemoryDataSource source = new MemoryDataSource();
            source.AddNode(node1);
            source.AddNode(node2);
            source.AddNode(node3);
            source.AddWay(way);

            // the use of natural=water implies an area-type.
            GeometryInterpreter interpreter = new SimpleGeometryInterpreter();
            GeometryCollection geometries = interpreter.Interpret(way, source);

            Assert.IsNotNull(geometries);
            Assert.AreEqual(1, geometries.Count);
            Geometry geometry = geometries[0];
            Assert.IsInstanceOf<LineairRing>(geometry);
        }

        /// <summary>
        /// Tests the interpretation of a water area.
        /// Way(natural=water) => area
        /// 
        /// http://wiki.openstreetmap.org/wiki/Tag:natural=water
        /// </summary>
        [Test]
        public void TestWayNaturalIsWaterArea()
        {
            Node node1 = new Node();
            node1.Id = 1;
            node1.Latitude = 0;
            node1.Longitude = 0;
            Node node2 = new Node();
            node2.Id = 2;
            node2.Latitude = 1;
            node2.Longitude = 0;
            Node node3 = new Node();
            node3.Id = 3;
            node3.Latitude = 0;
            node3.Longitude = 1;

            Way way = new Way();
            way.Id = 1;
            way.Nodes = new List<long>();
            way.Nodes.Add(1);
            way.Nodes.Add(2);
            way.Nodes.Add(3);
            way.Nodes.Add(1);
            way.Tags = new SimpleTagsCollection();
            way.Tags.Add("natural", "water");

            MemoryDataSource source = new MemoryDataSource();
            source.AddNode(node1);
            source.AddNode(node2);
            source.AddNode(node3);
            source.AddWay(way);

            // the use of natural=water implies an area-type.
            GeometryInterpreter interpreter = new SimpleGeometryInterpreter();
            GeometryCollection geometries = interpreter.Interpret(way, source);

            Assert.IsNotNull(geometries);
            Assert.AreEqual(1, geometries.Count);
            Geometry geometry = geometries[0];
            Assert.IsInstanceOf<LineairRing>(geometry);
        }
    }
}

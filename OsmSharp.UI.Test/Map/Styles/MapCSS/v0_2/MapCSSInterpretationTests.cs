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

using System.Collections.Generic;
using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.UI.Renderer.Scene;

namespace OsmSharp.UI.Test.Unittests.Map.Styles.MapCSS.v0_2
{
    /// <summary>
    /// Tests a few MapCSS interpretation as stated in:
    /// 
    /// http://josm.openstreetmap.de/wiki/Help/Styles/MapCSSImplementation
    /// </summary>
    [TestFixture]
    public class MapCSSInterpretationTests
    {
        /// <summary>
        /// Tests an empty CSS file.
        /// 
        /// Default settings for OsmSharp are: 
        /// canvas {
        ///     background-color: black;
        ///     default-points: false;
        ///     default-lines: false;
        /// }
        /// </summary>
        [Test]
        public void TestEmptyCSS()
        {
            // create 'test' objects.
            Node node1 = new Node();
            node1.Id = 1;
            node1.Latitude = 1;
            node1.Longitude = 1;

            Node node2 = new Node();
            node2.Id = 2;
            node2.Latitude = 2;
            node2.Longitude = 2;

            Way way = new Way();
            way.Id = 1;
            way.Nodes = new List<long>();
            way.Nodes.Add(1);
            way.Nodes.Add(2);

            // create the datasource.
            MemoryDataSource dataSource = new MemoryDataSource();
            dataSource.AddNode(node1);
            dataSource.AddNode(node2);
            dataSource.AddWay(way);

            // create the projection and scene objects.
            var mercator = new WebMercator();
            Scene2D scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), 16);

            // create the interpreter.
            MapCSSInterpreter interpreter = new MapCSSInterpreter(string.Empty,
                new MapCSSDictionaryImageSource());
            interpreter.Translate(scene, mercator, dataSource, node1);
            interpreter.Translate(scene, mercator, dataSource, node2);
            interpreter.Translate(scene, mercator, dataSource, way);

            // test the scene contents.
            Assert.AreEqual(0, scene.Count);
            Assert.AreEqual(SimpleColor.FromKnownColor(KnownColor.Black).Value, scene.BackColor);
        }

        /// <summary>
        /// Tests the canvas settings.
        /// </summary>
        [Test]
        public void TestCanvasSettingsCSS()
        {
            // create CSS.
            string css = "canvas { " +
                "fill-color: green; " +
                "} ";

            // create 'test' objects.
            Node node1 = new Node();
            node1.Id = 1;
            node1.Latitude = 1;
            node1.Longitude = 1;

            Node node2 = new Node();
            node2.Id = 2;
            node2.Latitude = 2;
            node2.Longitude = 2;

            Way way = new Way();
            way.Id = 1;
            way.Nodes = new List<long>();
            way.Nodes.Add(1);
            way.Nodes.Add(2);

            // create the datasource.
            MemoryDataSource dataSource = new MemoryDataSource();
            dataSource.AddNode(node1);
            dataSource.AddNode(node2);
            dataSource.AddWay(way);

            // create the projection and scene objects.
            var mercator = new WebMercator();
            Scene2D scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), 16);

            // create the interpreter.
            MapCSSInterpreter interpreter = new MapCSSInterpreter(css,
                new MapCSSDictionaryImageSource());
            interpreter.Translate(scene, mercator, dataSource, node1);
            interpreter.Translate(scene, mercator, dataSource, node2);
            interpreter.Translate(scene, mercator, dataSource, way);

            // test the scene contents.
            Assert.AreEqual(0, scene.Count);
            Assert.AreEqual(SimpleColor.FromKnownColor(KnownColor.Green).Value, scene.BackColor);
        }

        /// <summary>
        /// Tests the canvas settings.
        /// </summary>
        [Test]
        public void TestCanvasJOSMSettingsCSS()
        {
            // create CSS.
            string css = "canvas { " +
                "background-color: white; " +
                "default-points: true; " + // adds default points for every node (color: black, size: 2).
                "default-lines: true; " + // adds default lines for every way (color: red, width: 1).
                "} ";

            // create 'test' objects.
            Node node1 = new Node();
            node1.Id = 1;
            node1.Latitude = 1;
            node1.Longitude = 1;

            Node node2 = new Node();
            node2.Id = 2;
            node2.Latitude = 2;
            node2.Longitude = 2;

            Way way = new Way();
            way.Id = 1;
            way.Nodes = new List<long>();
            way.Nodes.Add(1);
            way.Nodes.Add(2);

            // create the datasource.
            MemoryDataSource dataSource = new MemoryDataSource();
            dataSource.AddNode(node1);
            dataSource.AddNode(node2);
            dataSource.AddWay(way);

            // create the projection and scene objects.
            var mercator = new WebMercator();
            Scene2D scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), 16);

            // create the interpreter.
            MapCSSInterpreter interpreter = new MapCSSInterpreter(css,
                new MapCSSDictionaryImageSource());
            interpreter.Translate(scene, mercator, dataSource, node1);
            interpreter.Translate(scene, mercator, dataSource, node2);
            interpreter.Translate(scene, mercator, dataSource, way);

            // test the scene contents.
            Assert.AreEqual(3, scene.Count);
            Assert.AreEqual(SimpleColor.FromKnownColor(KnownColor.White).Value, scene.BackColor);

            // test the scene point 1.
            Primitive2D primitive = scene.Get(0);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<Primitive2D>(primitive);
            Point2D pointObject = primitive as Point2D;
            Assert.AreEqual(2, pointObject.Size);
            Assert.AreEqual(SimpleColor.FromKnownColor(KnownColor.Black).Value, pointObject.Color);
            Assert.AreEqual(mercator.LongitudeToX(1), pointObject.X);
            Assert.AreEqual(mercator.LatitudeToY(1), pointObject.Y);

            // test the scene point 2.
            primitive = scene.Get(1);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<Point2D>(primitive);
            pointObject = primitive as Point2D;
            Assert.AreEqual(2, pointObject.Size);
            Assert.AreEqual(SimpleColor.FromKnownColor(KnownColor.Black).Value, pointObject.Color);
            Assert.AreEqual(mercator.LongitudeToX(2), pointObject.X);
            Assert.AreEqual(mercator.LatitudeToY(2), pointObject.Y);

            // test the scene line 2.
            primitive = scene.Get(2);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<Line2D>(primitive);
            Line2D line = primitive as Line2D;
            Assert.AreEqual(1, line.Width);
            Assert.AreEqual(SimpleColor.FromKnownColor(KnownColor.Red).Value, line.Color);
            Assert.IsNotNull(line.X);
            Assert.IsNotNull(line.Y);
            Assert.AreEqual(2, line.X.Length);
            Assert.AreEqual(2, line.Y.Length);
            Assert.AreEqual(mercator.LongitudeToX(1), line.X[0]);
            Assert.AreEqual(mercator.LatitudeToY(1), line.Y[0]);
            Assert.AreEqual(mercator.LongitudeToX(2), line.X[1]);
            Assert.AreEqual(mercator.LatitudeToY(2), line.Y[1]);
        }

        /// <summary>
        /// Does some tests to test the behaviour when using different layers.
        /// </summary>
        [Test]
        public void TestMapCSSClosedWay()
        { // tests map css interpretation of a closed way marked as an area.
            MemoryDataSource source = new MemoryDataSource(
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1, new TagsCollection(
                        Tag.Create("area", "yes")), 1, 2, 3, 1));

            // test closed way.
            string css = "way[area] { " +
                         "   fill-color: black; " +
                         "} ";

            // create the projection and scene objects.
            var mercator = new WebMercator();
            Scene2D scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), 16);

            // create the interpreter.
            MapCSSInterpreter interpreter = new MapCSSInterpreter(css,
                new MapCSSDictionaryImageSource());
            interpreter.Translate(scene, mercator, source, source.GetWay(1));

            // test the scene contents.
            Assert.AreEqual(1, scene.Count);
            Primitive2D primitive = scene.Get(0);
            Assert.IsInstanceOf<Polygon2D>(primitive);
        }

        /// <summary>
        /// Tests a simple MapCSS interpretation of an area.
        /// </summary>
        [Test]
        public void TestMapCSSArea()
        {
            MemoryDataSource source = new MemoryDataSource(
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1, new TagsCollection(
                        Tag.Create("area", "yes")), 1, 2, 3, 1));

            // test closed way.
            string css = "area { " +
                    "   fill-color: black; " +
                    "} ";

            // create the projection and scene objects.
            var mercator = new WebMercator();
            Scene2D scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), 16);

            // create the projection and scene objects.
            scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), 16);

            // create the interpreter.
            MapCSSInterpreter interpreter = new MapCSSInterpreter(css,
                new MapCSSDictionaryImageSource());
            interpreter.Translate(scene, mercator, source, source.GetWay(1));

            // test the scene contents.
            Assert.AreEqual(1, scene.Count);
            Primitive2D primitive = scene.Get(0);
            Assert.IsInstanceOf<Polygon2D>(primitive);
        }

        /// <summary>
        /// Tests a width eval functions on a way.
        /// </summary>
        [Test]
        public void TestMapCSSSimpleEval()
        {
            MemoryDataSource source = new MemoryDataSource(
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1, new TagsCollection(
                Tag.Create("width", "10")), 1, 2, 3, 1));

            // test closed way.
            string css = "way { " +
                "   width:  eval(\"tag('width')\"); " +
                "   color: green; " +
                "} ";

            // create the projection and scene objects.
            var mercator = new WebMercator();
            Scene2D scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), 16);

            // create the projection and scene objects.
            scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), 16);

            // create the interpreter.
            MapCSSInterpreter interpreter = new MapCSSInterpreter(css,
                                                                  new MapCSSDictionaryImageSource());
            interpreter.Translate(scene, mercator, source, source.GetWay(1));

            // test the scene contents.
            Assert.AreEqual(1, scene.Count);
            Primitive2D primitive = scene.Get(0);
            Assert.IsInstanceOf<Line2D>(primitive);
            Line2D line = (primitive as Line2D);
            Assert.AreEqual(10, line.Width);
        }
    }
}
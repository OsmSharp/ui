// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Streams.GeoJson;
using OsmSharp.Math.Geo;

namespace OsmSharp.Test.Unittests.Geo.Streams.GeoJson
{
    /// <summary>
    /// Tests the GeoJson converter.
    /// </summary>
    [TestFixture]
    public class GeoJsonConverterTests
    {
        /// <summary>
        /// Tests serializing a lineair ring.
        /// </summary>
        [Test]
        public void TestLineairRingSerialization()
        {
            var geometry = new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                });

            var serialized = geometry.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]}", 
                serialized);
        }

        /// <summary>
        /// Tests serializing a polygon.
        /// </summary>
        [Test]
        public void TestPolygonSerialization()
        { 
            // polygon, no holes.
            var geometry = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                }));

            var serialized = geometry.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]}",
                serialized);

            // polygons, one hole.
            geometry = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                }),
                new LineairRing[]
                {
                    new LineairRing(
                        new GeoCoordinate[]
                        {
                            new GeoCoordinate(0.25, 0.25),
                            new GeoCoordinate(0.25, 0.75),
                            new GeoCoordinate(0.75, 0.75),
                            new GeoCoordinate(0.75, 0.25),
                            new GeoCoordinate(0.25, 0.25)
                        })
                });

            serialized = geometry.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]],[[0.25,0.25],[0.75,0.25],[0.75,0.75],[0.25,0.75],[0.25,0.25]]]}",
                serialized);
        }

        /// <summary>
        /// Tests serializing a linestring.
        /// </summary>
        [Test]
        public void TestLineStringSerialization()
        {
            var geometry = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0)
                });

            var serialized = geometry.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"LineString\",\"coordinates\":[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]]}",
                serialized);
        }

        /// <summary>
        /// Tests serializing a point.
        /// </summary>
        [Test]
        public void TestPointSerialization()
        {
            var geometry = new Point(new GeoCoordinate(0, 1));

            var serialized = geometry.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}",
                serialized);
        }
    }
}
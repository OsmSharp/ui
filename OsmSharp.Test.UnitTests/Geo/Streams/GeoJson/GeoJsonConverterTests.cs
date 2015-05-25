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
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
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
        /// Tests serializing a multipolygon.
        /// </summary>
        [Test]
        public void TestMultiPolygonSerialization()
        {
            var geometry1 = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                }));
            var geometry2 = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 2),
                    new GeoCoordinate(2, 2),
                    new GeoCoordinate(2, 0),
                    new GeoCoordinate(0, 0)
                }));
            var geometry3 = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 3),
                    new GeoCoordinate(3, 3),
                    new GeoCoordinate(3, 0),
                    new GeoCoordinate(0, 0)
                }));
            var geometryCollection = new MultiPolygon(new Polygon[] { geometry1, geometry2, geometry3 });

            var serialized = geometryCollection.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("[" +
                "{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]}," +
                "{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0],[0.0,0.0]]]}," +
                "{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[3.0,0.0],[3.0,3.0],[0.0,3.0],[0.0,0.0]]]}" +
                "]",
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
        /// Tests serializing a multilinestring.
        /// </summary>
        [Test]
        public void TestMultiLineStringSerialization()
        {
            var geometry1 = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0)
                });
            var geometry2 = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 2),
                    new GeoCoordinate(2, 2),
                    new GeoCoordinate(2, 0)
                });
            var geometry3 = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 3),
                    new GeoCoordinate(3, 3),
                    new GeoCoordinate(3, 0)
                });
            var geometryCollection = new MultiLineString(new LineString[] { geometry1, geometry2, geometry3 });

            var serialized = geometryCollection.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"MultiLineString\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]],[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0]],[[0.0,0.0],[3.0,0.0],[3.0,3.0],[0.0,3.0]]]}",
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

        /// <summary>
        /// Tests serializing a multipoint.
        /// </summary>
        [Test]
        public void TestMultiPointSerialization()
        {
            var geometry1 = new Point(new GeoCoordinate(0, 1));
            var geometry2 = new Point(new GeoCoordinate(1, 1));
            var geometry3 = new Point(new GeoCoordinate(1, 0));
            var geometryCollection = new MultiPoint(new Point[] { geometry1, geometry2, geometry3 });

            var serialized = geometryCollection.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"MultiPoint\",\"coordinates\":[[1.0,0.0],[1.0,1.0],[0.0,1.0]]}",
                serialized);
        }

        /// <summary>
        /// Tests serializing a feature.
        /// </summary>
        [Test]
        public void TestFeatureSerialization()
        {
            // a feature with a point.
            var geometry = (Geometry)new Point(new GeoCoordinate(0, 1));
            var feature = new Feature(geometry);

            var serialized = feature.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}",
                serialized);

            feature = new Feature(geometry, new SimpleGeometryAttributeCollection(new GeometryAttribute[] 
            {
                new GeometryAttribute()
                {
                    Key = "key1",
                    Value = "value1"
                }
            }));

            serialized = feature.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Feature\",\"properties\":{\"key1\":\"value1\"},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}",
                serialized);

            // a feature with a linestring.
            geometry = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0)
                });
            feature = new Feature(geometry);

            serialized = feature.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"LineString\",\"coordinates\":[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]]}}",
                serialized);

            // a featurer with a linearring.
            geometry = new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                });
            feature = new Feature(geometry);

            serialized = feature.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]}}",
                serialized);

            // a featurer with a polygon.
            geometry = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                }));
            feature = new Feature(geometry);

            serialized = feature.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]}}",
                serialized);
        }

        /// <summary>
        /// Tests serializing a feature collection.
        /// </summary>
        [Test]
        public void TestFeatureCollectionSerialization()
        {
            var geometry = new Point(new GeoCoordinate(0, 1));
            var feature1 = new Feature(geometry);
            var feature2 = new Feature(geometry, new SimpleGeometryAttributeCollection(new GeometryAttribute[] 
            {
                new GeometryAttribute()
                {
                    Key = "key1",
                    Value = "value1"
                }
            }));

            var featureCollection = new FeatureCollection(new Feature[] { feature1, feature2 });

            var serialized = featureCollection.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"FeatureCollection\",\"features\":[" + 
                "{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}," +
                "{\"type\":\"Feature\",\"properties\":{\"key1\":\"value1\"},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}" + 
                "]}",
                serialized);
        }
    }
}
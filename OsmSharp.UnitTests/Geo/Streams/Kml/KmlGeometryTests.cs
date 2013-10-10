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
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Streams.Kml;

namespace OsmSharp.UnitTests.Streams.IO.Kml
{
    /// <summary>
    /// Contains test to read/write Kml files using geometry streams.
    /// </summary>
    [TestFixture]
    public class GpxGeometryTests
    {
        /// <summary>
        /// Test reads an embedded Kml files and converts it to geometries.
        /// </summary>
        [Test]
        public void KmlReadGeometryv2_0()
        {
            // initialize the geometry source.
            KmlGeoStreamSource kmlSource = new KmlGeoStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test.v2.0.kml"));

            // pull all the objects from the stream into the given collection.
            GeometryCollection kmlCollection = new GeometryCollection(kmlSource);
            List<Geometry> geometries = new List<Geometry>(kmlCollection);

            // test collection contents.
            Assert.AreEqual(1, geometries.Count);
            Assert.IsInstanceOf(typeof(Point), geometries[0]);
        }
        
        /// <summary>
        /// Test reads an embedded Kml files and converts it to geometries.
        /// </summary>
        [Test]
        public void KmlReadGeometryv2_0_response()
        {
            // initialize the geometry source.
            KmlGeoStreamSource kmlSource = new KmlGeoStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test.v2.0.response.kml"));

            // pull all the objects from the stream into the given collection.
            GeometryCollection kmlCollection = new GeometryCollection(kmlSource);
            List<Geometry> geometries = new List<Geometry>(kmlCollection);

            // test collection contents.
            Assert.AreEqual(7, geometries.Count);
            Assert.IsInstanceOf(typeof(Point), geometries[0]);
            Assert.IsInstanceOf(typeof(Point), geometries[1]);
            Assert.IsInstanceOf(typeof(Point), geometries[2]);
            Assert.IsInstanceOf(typeof(Point), geometries[3]);
            Assert.IsInstanceOf(typeof(Point), geometries[4]);
            Assert.IsInstanceOf(typeof(Point), geometries[5]);
            Assert.IsInstanceOf(typeof(Point), geometries[6]);
        }

        /// <summary>
        /// Test reads an embedded Kml files and converts it to geometries.
        /// </summary>
        [Test]
        public void KmlReadGeometryv2_1()
        {
            // initialize the geometry source.
            KmlGeoStreamSource kmlSource = new KmlGeoStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test.v2.1.kml"));

            // pull all the objects from the stream into the given collection.
            GeometryCollection kmlCollection = new GeometryCollection(kmlSource);
            List<Geometry> geometries = new List<Geometry>(kmlCollection);

            // test collection contents.
            Assert.AreEqual(23, geometries.Count);
            Assert.IsInstanceOf(typeof(LineString), geometries[0]);
            Assert.IsInstanceOf(typeof(LineString), geometries[1]);
            Assert.IsInstanceOf(typeof(LineString), geometries[2]);
            Assert.IsInstanceOf(typeof(LineString), geometries[3]);
            Assert.IsInstanceOf(typeof(LineString), geometries[4]);
            Assert.IsInstanceOf(typeof(LineString), geometries[5]);
            Assert.IsInstanceOf(typeof(LineString), geometries[6]);
            Assert.IsInstanceOf(typeof(LineString), geometries[7]);
            Assert.IsInstanceOf(typeof(LineString), geometries[8]);
            Assert.IsInstanceOf(typeof(LineString), geometries[9]);
            Assert.IsInstanceOf(typeof(LineString), geometries[10]);
            Assert.IsInstanceOf(typeof(LineString), geometries[11]);
            Assert.IsInstanceOf(typeof(LineString), geometries[12]);
            Assert.IsInstanceOf(typeof(LineString), geometries[13]);
            Assert.IsInstanceOf(typeof(LineString), geometries[14]);
            Assert.IsInstanceOf(typeof(LineString), geometries[15]);
            Assert.IsInstanceOf(typeof(LineString), geometries[16]);
            Assert.IsInstanceOf(typeof(LineString), geometries[17]);
            Assert.IsInstanceOf(typeof(LineString), geometries[18]);
            Assert.IsInstanceOf(typeof(LineString), geometries[19]);
            Assert.IsInstanceOf(typeof(LineString), geometries[20]);
            Assert.IsInstanceOf(typeof(LineString), geometries[21]);
            Assert.IsInstanceOf(typeof(LineString), geometries[22]);
        }
    }
}

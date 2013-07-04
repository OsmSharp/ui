using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Geo.IO.Gpx;
using OsmSharp.Geo.Geometries;
using System.Reflection;

namespace OsmSharp.UnitTests.Geo.IO.Gpx
{
    /// <summary>
    /// Contains test to read/write gpx files using geometry streams.
    /// </summary>
    [TestFixture]
    public class GpxGeometryTests
    {
        /// <summary>
        /// Test reads an embedded gpx files and converts it to geometries.
        /// </summary>
        [Test]
        public void GpxReadGeometryv1_0()
        {
            // initialize the geometry source.
            GpxGeometryStreamSource gpxSource = new GpxGeometryStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test.v1.0.gpx"),
                false);

            // pull all the objects from the stream into the given collection.
            GeometryCollection gpxCollection = new GeometryCollection(gpxSource);
            List<Geometry> geometries = new List<Geometry>(gpxCollection);

            // test collection contents.
            Assert.AreEqual(1, geometries.Count);
            Assert.IsInstanceOf(typeof(LineString), geometries[0]);
            Assert.AreEqual(424, (geometries[0] as LineString).Coordinates.Count);
        }
    }
}

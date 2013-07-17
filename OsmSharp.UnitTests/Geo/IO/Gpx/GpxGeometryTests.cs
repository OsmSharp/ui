// OsmSharp - OpenStreetMap tools & library.
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

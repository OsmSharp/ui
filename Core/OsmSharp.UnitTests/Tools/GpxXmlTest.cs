// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Tools.Xml.Gpx;
using OsmSharp.Tools.Xml.Sources;
using System.Reflection;

namespace OsmSharp.UnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class GpxXmlTest
    {
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

        #region v1.0

        /// <summary>
        /// Test reads an embedded file.
        /// </summary>
        [Test]
        public void GpxReadv1_0Test()
        {
            // instantiate and load the gpx test document.
            XmlStreamSource source = new XmlStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test.v1.0.gpx"));
            GpxDocument document = new GpxDocument(source);
            object gpx = document.Gpx;

            if (gpx is OsmSharp.Tools.Xml.Gpx.v1_0.gpx)
            { // all ok here!
                OsmSharp.Tools.Xml.Gpx.v1_0.gpx gpx_type = (gpx as OsmSharp.Tools.Xml.Gpx.v1_0.gpx);

                // test the gpx test file content.
                Assert.IsNotNull(gpx_type.trk, "Gpx has not track!");
                Assert.AreEqual(gpx_type.trk[0].trkseg.Length, 424, "Not the correct number of track segments found!");
            }
            else
            {
                Assert.Fail("No gpx data was read, or data was of the incorrect type!");
            }

            document.Close();
            source.Close();
        }

        ///// <summary>
        ///// Test writes an embedded file.
        ///// </summary>
        //[Test]
        //public void GpxWritev1_0Test()
        //{
        //    // instantiate and load the gpx test document.
        //    XmlFileSource source = new XmlFileSource(
        //        Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test.v1.0.gpx"));
        //    GpxDocument document = new GpxDocument(source);
        //    object gpx = document.Gpx;

        //    if (gpx is OsmSharp.Tools.Xml.Gpx.v1_0.gpx)
        //    { // all ok here!
        //        // get the target file.
        //        MemoryStream write_file = new MemoryStream();

        //        // create a new xml source.
        //        XmlFileSource write_source = new XmlFileSource(write_file);
        //        GpxDocument gpx_target = new GpxDocument(write_source);

        //        // set the target data the same as the source document.
        //        gpx_target.Gpx = gpx;

        //        // save the data.
        //        gpx_target.Save();

        //        // close the old document.
        //        document.Close();
        //        source.Close();

        //        // check to see if the data was writter correctly.
        //        // instantiate and load the gpx test document.
        //        write_file = new MemoryStream();
        //        source = new XmlFileSource(write_file);
        //        document = new GpxDocument(source);
        //        gpx = document.Gpx;

        //        if (gpx is OsmSharp.Tools.Xml.Gpx.v1_0.gpx)
        //        { // all ok here!
        //            OsmSharp.Tools.Xml.Gpx.v1_0.gpx gpx_type = (gpx as OsmSharp.Tools.Xml.Gpx.v1_0.gpx);

        //            // test the gpx test file content.
        //            Assert.IsNotNull(gpx_type.trk, "Gpx has not track!");
        //            Assert.AreEqual(gpx_type.trk[0].trkseg.Length, 424, "Not the correct number of track segments found!");
        //        }
        //        else
        //        {
        //            Assert.Fail("No gpx data was read, or data was of the incorrect type!");
        //        }

        //    }
        //    else
        //    {
        //        Assert.Fail("No gpx data was read, or data was of the incorrect type!");
        //    }

        //    document.Close();
        //    source.Close();
        //}

        #endregion

        #region v1.1

        /// <summary>
        /// Test reads an embedded file.
        /// </summary>
        [Test]
        public void GpxReadv1_1Test()
        {
            // instantiate and load the gpx test document.
            XmlStreamSource source = new XmlStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test.v1.1.gpx"));
            GpxDocument document = new GpxDocument(source);
            object gpx = document.Gpx;

            if (gpx is OsmSharp.Tools.Xml.Gpx.v1_1.gpxType)
            { // all ok here!
                OsmSharp.Tools.Xml.Gpx.v1_1.gpxType gpx_type = (gpx as OsmSharp.Tools.Xml.Gpx.v1_1.gpxType);

                // test the gpx test file content.
                Assert.IsNotNull(gpx_type.trk, "Gpx has not track!");
                Assert.AreEqual(gpx_type.trk[0].trkseg.Length, 1, "Not the correct number of track segments found!");
            }
            else
            {
                Assert.Fail("No gpx data was read, or data was of the incorrect type!");
            }

            document.Close();
            source.Close();
        }

        ///// <summary>
        ///// Test writes an embedded file.
        ///// </summary>
        //[Test]
        //public void GpxWritev1_1Test()
        //{
        //    // instantiate and load the gpx test document.
        //    XmlFileSource source = new XmlFileSource(new FileInfo(@"test.v1.1.gpx"));
        //    GpxDocument document = new GpxDocument(source);
        //    object gpx = document.Gpx;

        //    if (gpx is OsmSharp.Tools.Xml.Gpx.v1_1.gpxType)
        //    { // all ok here!
        //        // get the target file.
        //        FileInfo write_file = new FileInfo(@"test.written.v1.1.gpx");

        //        // create a new xml source.
        //        XmlFileSource write_source = new XmlFileSource(write_file);
        //        GpxDocument gpx_target = new GpxDocument(write_source);

        //        // set the target data the same as the source document.
        //        gpx_target.Gpx = gpx;

        //        // save the data.
        //        gpx_target.Save();

        //        // close the old document.
        //        document.Close();
        //        source.Close();

        //        // check to see if the data was writter correctly.
        //        // instantiate and load the gpx test document.
        //        source = new XmlFileSource(write_file);
        //        document = new GpxDocument(source);
        //        gpx = document.Gpx;
        //        if (gpx is OsmSharp.Tools.Xml.Gpx.v1_1.gpxType)
        //        { // all ok here!
        //            OsmSharp.Tools.Xml.Gpx.v1_1.gpxType gpx_type = (gpx as OsmSharp.Tools.Xml.Gpx.v1_1.gpxType);

        //            // test the gpx test file content.
        //            Assert.IsNotNull(gpx_type.trk, "Gpx has not track!");
        //            Assert.AreEqual(gpx_type.trk[0].trkseg.Length, 1, "Not the correct number of track segments found!");
        //        }
        //        else
        //        {
        //            Assert.Fail("No gpx data was read, or data was of the incorrect type!");
        //        }

        //    }
        //    else
        //    {
        //        Assert.Fail("No gpx data was read, or data was of the incorrect type!");
        //    }

        //    document.Close();
        //    source.Close();
        //}


        #endregion
    }
}

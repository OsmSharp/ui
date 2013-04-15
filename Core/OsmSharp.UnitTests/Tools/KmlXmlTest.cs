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
using OsmSharp.Tools.Xml.Kml;
using OsmSharp.Tools.Xml.Sources;
using System.Reflection;

namespace OsmSharp.UnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class KmlXmlTest
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
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

        #region v2.0

        /// <summary>
        /// Reads a v2.0 kml.
        /// </summary>
        [Test]
        public void KmlReadTestv2_0()
        {
            // instantiate and load the gpx test document.
            XmlStreamSource source = new XmlStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test.v2.0.kml"));
            KmlDocument document = new KmlDocument(source);
            object kml = document.Kml;

            if (kml is OsmSharp.Tools.Xml.Kml.v2_0.kml)
            { // all ok here!
                OsmSharp.Tools.Xml.Kml.v2_0.kml kml_type = (kml as OsmSharp.Tools.Xml.Kml.v2_0.kml);

                // test the gpx test file content.
                Assert.IsNotNull(kml_type.Item, "No item was found!");
                Assert.IsInstanceOf<OsmSharp.Tools.Xml.Kml.v2_0.Placemark>(kml_type.Item, "Incorrect item type!");

                OsmSharp.Tools.Xml.Kml.v2_0.Placemark type = (kml_type.Item as OsmSharp.Tools.Xml.Kml.v2_0.Placemark);
                Assert.AreEqual(type.Items.Length, 3, "Incorrect number of items in folder!");
            }
            else
            {
                Assert.Fail("No kml data was read, or data was of the incorrect type!");
            }

            document.Close();
            source.Close();
        }

        ///// <summary>
        ///// Write a v2.0 kml.
        ///// </summary>
        //[Test]
        //public void KmlWriteTestv2_0()
        //{
        //    // instantiate and load the gpx test document.
        //    XmlFileSource source = new XmlFileSource(
        //        Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test.v2.0.kml"));
        //    KmlDocument document = new KmlDocument(source);
        //    object kml = document.Kml;

        //    if (kml is OsmSharp.Tools.Xml.Kml.v2_0.kml)
        //    { // all ok here!

        //        // delete the target file.
        //        FileInfo write_file = new FileInfo(@"test.v2.0.written.kml");
        //        if (write_file.Exists)
        //        {
        //            write_file.Delete();
        //        }
        //        write_file = new FileInfo(@"test.v2.0.written.kml");

        //        // create a new xml source.
        //        XmlFileSource write_source = new XmlFileSource(write_file);
        //        KmlDocument kml_target = new KmlDocument(write_source);

        //        // set the target data the same as the source document.
        //        kml_target.Kml = kml;

        //        // save the data.
        //        kml_target.Save();

        //        // close the old document.
        //        document.Close();
        //        source.Close();

        //        // check to see if the data was writter correctly.
        //        // instantiate and load the osm test document.
        //        source = new XmlFileSource(write_file);
        //        document = new KmlDocument(source);
        //        kml = document.Kml;

        //        // check the result that was written and then read again.
        //        if (kml is OsmSharp.Tools.Xml.Kml.v2_0.kml)
        //        { // all ok here!
        //            OsmSharp.Tools.Xml.Kml.v2_0.kml kml_type = (kml as OsmSharp.Tools.Xml.Kml.v2_0.kml);

        //            // test the gpx test file content.
        //            Assert.IsNotNull(kml_type.Item, "No item was found!");
        //            Assert.IsInstanceOfType(kml_type.Item, typeof(OsmSharp.Tools.Xml.Kml.v2_0.Placemark), "Incorrect item type!");

        //            OsmSharp.Tools.Xml.Kml.v2_0.Placemark type = (kml_type.Item as OsmSharp.Tools.Xml.Kml.v2_0.Placemark);
        //            Assert.AreEqual(type.Items.Length, 3, "Incorrect number of items in folder!");
        //        }
        //        else
        //        {
        //            Assert.Fail("No kml data was read, or data was of the incorrect type!");
        //        }

        //        document.Close();
        //        source.Close();

        //    }
        //    else
        //    {
        //        Assert.Fail("No kml data was read, or data was of the incorrect type!");
        //    }

        //    document.Close();
        //    source.Close();
        //}


        #endregion

        #region v2.0.response

        /// <summary>
        /// Reads a v2.0 response kml.
        /// </summary>
        [Test]
        public void KmlReadTestv2_0_response()
        {
            // instantiate and load the gpx test document.
            XmlStreamSource source = new XmlStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test.v2.0.response.kml"));
            KmlDocument document = new KmlDocument(source);
            object kml = document.Kml;

            if (kml is OsmSharp.Tools.Xml.Kml.v2_0_response.kml)
            { // all ok here!
                OsmSharp.Tools.Xml.Kml.v2_0_response.kml kml_type = (kml as OsmSharp.Tools.Xml.Kml.v2_0_response.kml);

                // test the gpx test file content.
                Assert.IsNotNull(kml_type.Item, "No item was found!");
                Assert.IsInstanceOf<OsmSharp.Tools.Xml.Kml.v2_0_response.Response>(kml_type.Item, "Incorrect item type!");

                OsmSharp.Tools.Xml.Kml.v2_0_response.Response type = (kml_type.Item as OsmSharp.Tools.Xml.Kml.v2_0_response.Response);
                Assert.AreEqual(type.Items.Length, 8, "Incorrect number of items in response!");
            }
            else
            {
                Assert.Fail("No kml data was read, or data was of the incorrect type!");
            }

            document.Close();
            source.Close();
        }

        ///// <summary>
        ///// Write a v2.0 response kml.
        ///// </summary>
        //[Test]
        //public void KmlWriteTestv2_0_response()
        //{
        //    // instantiate and load the gpx test document.
        //    XmlFileSource source = new XmlFileSource(new FileInfo(@"test.v2.0.response.kml"));
        //    KmlDocument document = new KmlDocument(source);
        //    object kml = document.Kml;

        //    if (kml is OsmSharp.Tools.Xml.Kml.v2_0_response.kml)
        //    { // all ok here!

        //        // delete the target file.
        //        FileInfo write_file = new FileInfo(@"test.v2.0.response.written.kml");
        //        if (write_file.Exists)
        //        {
        //            write_file.Delete();
        //        }
        //        write_file = new FileInfo(@"test.v2.0.response.written.kml");

        //        // create a new xml source.
        //        XmlFileSource write_source = new XmlFileSource(write_file);
        //        KmlDocument kml_target = new KmlDocument(write_source);

        //        // set the target data the same as the source document.
        //        kml_target.Kml = kml;

        //        // save the data.
        //        kml_target.Save();

        //        // close the old document.
        //        document.Close();
        //        source.Close();

        //        // check to see if the data was writter correctly.
        //        // instantiate and load the osm test document.
        //        source = new XmlFileSource(write_file);
        //        document = new KmlDocument(source);
        //        kml = document.Kml;

        //        // check the result that was written and then read again.
        //        if (kml is OsmSharp.Tools.Xml.Kml.v2_0_response.kml)
        //        { // all ok here!
        //            OsmSharp.Tools.Xml.Kml.v2_0_response.kml kml_type = (kml as OsmSharp.Tools.Xml.Kml.v2_0_response.kml);

        //            // test the gpx test file content.
        //            Assert.IsNotNull(kml_type.Item, "No item was found!");
        //            Assert.IsInstanceOfType(kml_type.Item, typeof(OsmSharp.Tools.Xml.Kml.v2_0_response.Response), "Incorrect item type!");

        //            OsmSharp.Tools.Xml.Kml.v2_0_response.Response type = (kml_type.Item as OsmSharp.Tools.Xml.Kml.v2_0_response.Response);
        //            Assert.AreEqual(type.Items.Length, 8, "Incorrect number of items in response!");
        //        }
        //        else
        //        {
        //            Assert.Fail("No kml data was read, or data was of the incorrect type!");
        //        }

        //        document.Close();
        //        source.Close();

        //    }
        //    else
        //    {
        //        Assert.Fail("No kml data was read, or data was of the incorrect type!");
        //    }

        //    document.Close();
        //    source.Close();
        //}

        #endregion

        #region v2.1

        /// <summary>
        /// Reads a v2.1 kml.
        /// </summary>
        [Test]
        public void KmlReadTestv2_1()
        {
            // instantiate and load the gpx test document.
            XmlStreamSource source = new XmlStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test.v2.1.kml"));
            KmlDocument document = new KmlDocument(source);
            object kml = document.Kml;

            if (kml is OsmSharp.Tools.Xml.Kml.v2_1.KmlType)
            { // all ok here!
                OsmSharp.Tools.Xml.Kml.v2_1.KmlType kml_type = (kml as OsmSharp.Tools.Xml.Kml.v2_1.KmlType);

                // test the gpx test file content.
                Assert.IsNotNull(kml_type.Item, "No item was found!");
                Assert.IsInstanceOf<OsmSharp.Tools.Xml.Kml.v2_1.FolderType>(kml_type.Item, "Incorrect item type!");

                OsmSharp.Tools.Xml.Kml.v2_1.FolderType type = (kml_type.Item as OsmSharp.Tools.Xml.Kml.v2_1.FolderType);
                Assert.AreEqual(type.Items1.Length, 10, "Incorrect number of items in folder!");
            }
            else
            {
                Assert.Fail("No kml data was read, or data was of the incorrect type!");
            }

            document.Close();
            source.Close();
        }


        ///// <summary>
        ///// Write a v2.1 kml.
        ///// </summary>
        //[Test]
        //public void KmlWriteTestv2_1()
        //{
        //    // instantiate and load the gpx test document.
        //    XmlFileSource source = new XmlFileSource(new FileInfo(@"test.v2.1.kml"));
        //    KmlDocument document = new KmlDocument(source);
        //    object kml = document.Kml;

        //    if (kml is OsmSharp.Tools.Xml.Kml.v2_1.KmlType)
        //    { // all ok here!

        //        // delete the target file.
        //        FileInfo write_file = new FileInfo(@"test.v2.1.written.kml");
        //        if (write_file.Exists)
        //        {
        //            write_file.Delete();
        //        }
        //        write_file = new FileInfo(@"test.v2.1.written.kml");

        //        // create a new xml source.
        //        XmlFileSource write_source = new XmlFileSource(write_file);
        //        KmlDocument kml_target = new KmlDocument(write_source);

        //        // set the target data the same as the source document.
        //        kml_target.Kml = kml;

        //        // save the data.
        //        kml_target.Save();

        //        // close the old document.
        //        document.Close();
        //        source.Close();

        //        // check to see if the data was writter correctly.
        //        // instantiate and load the osm test document.
        //        source = new XmlFileSource(write_file);
        //        document = new KmlDocument(source);
        //        kml = document.Kml;

        //        // check the result that was written and then read again.
        //        if (kml is OsmSharp.Tools.Xml.Kml.v2_1.KmlType)
        //        { // all ok here!
        //            OsmSharp.Tools.Xml.Kml.v2_1.KmlType kml_type = (kml as OsmSharp.Tools.Xml.Kml.v2_1.KmlType);

        //            // test the gpx test file content.
        //            Assert.IsNotNull(kml_type.Item, "No item was found!");
        //            Assert.IsInstanceOfType(kml_type.Item, typeof(OsmSharp.Tools.Xml.Kml.v2_1.FolderType), "Incorrect item type!");

        //            OsmSharp.Tools.Xml.Kml.v2_1.FolderType type = (kml_type.Item as OsmSharp.Tools.Xml.Kml.v2_1.FolderType);
        //            Assert.AreEqual(type.Items1.Length, 10, "Incorrect number of items in folder!");
        //        }
        //        else
        //        {
        //            Assert.Fail("No kml data was read, or data was of the incorrect type!");
        //        }

        //        document.Close();
        //        source.Close();

        //    }
        //    else
        //    {
        //        Assert.Fail("No kml data was read, or data was of the incorrect type!");
        //    }

        //    document.Close();
        //    source.Close();
        //}
        #endregion

    }
}

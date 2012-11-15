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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Osm.Core.Xml;
using OsmSharp.Tools.Xml.Sources;
using System.IO;
using OsmSharp.Osm.Core.Xml.v0_6;
using System.Diagnostics;

namespace OsmSharp.Osm.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class OsmXmlTest
    {
        public OsmXmlTest()
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

        /// <summary>
        /// Test a read operation for an osm document.
        /// </summary>
        [TestMethod]
        public void TestOsmDocumentRead()
        {
            // instantiate and load the osm test document.
            XmlFileSource source = new XmlFileSource(new FileInfo(@"test.osm"));
            OsmDocument document = new OsmDocument(source);
            object osm = document.Osm;

            if (osm is osm)
            { // all ok here!
                osm osm_type = (osm as osm);

                // test the osm test file content.
                Assert.AreEqual(osm_type.node.Length, 22500,"Incorrect number of nodes found!");
                Assert.AreEqual(osm_type.relation.Length, 52, "Incorrent number of relations found!");
                Assert.AreEqual(osm_type.way.Length, 1995, "Incorrent number of relations found!");
                Assert.IsNull(osm_type.changeset, "Incorrent number of changesets found!");
            }
            else
            {
                Assert.Fail("No osm data was read, or data was of the incorrect type!");
            }

            document.Close();
            source.Close();
        }


        /// <summary>
        /// Test a read and write operation for an osm document.
        /// </summary>
        [TestMethod]
        public void TestOsmDocumentWrite()
        {
            // instantiate and load the osm test document.
            XmlFileSource source = new XmlFileSource(new FileInfo(@"test.osm"));
            OsmDocument document = new OsmDocument(source);
            object osm = document.Osm;

            if (osm is osm)
            { // all ok here!
                osm osm_type = (osm as osm);

                // delete the target file.
                FileInfo write_file = new FileInfo(@"test_written.osm");
                write_file = new FileInfo(@"test_written.osm");

                // create a new xml source.
                XmlFileSource write_source = new XmlFileSource(write_file);
                OsmDocument osm_target = new OsmDocument(write_source);

                // set the target data the same as the source document.
                osm_target.Osm = osm;

                // save the data.
                osm_target.Save();

                // close the old document.
                document.Close();
                source.Close();

                // check to see if the data was writter correctly.
                // instantiate and load the osm test document.
                write_file = new FileInfo(@"test_written.osm");
                source = new XmlFileSource(write_file);
                document = new OsmDocument(source);
                osm = document.Osm;

                if (osm is osm)
                { // all ok here!
                    // test the osm test file content.
                    Assert.AreEqual(osm_type.node.Length, 22500, "Incorrect number of nodes found!");
                    Assert.AreEqual(osm_type.relation.Length, 52, "Incorrent number of relations found!");
                    Assert.AreEqual(osm_type.way.Length, 1995, "Incorrent number of relations found!");
                    Assert.IsNull(osm_type.changeset, "Incorrent number of changesets found!");
                }
                else
                {
                    Assert.Fail("No osm data was read, or data was of the incorrect type!");
                }

                document.Close();
                source.Close();
            }
            else
            {
                Assert.Fail("No osm data was read, or data was of the incorrect type!");
            }
        }
    }
}

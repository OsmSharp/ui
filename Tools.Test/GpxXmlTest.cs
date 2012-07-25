using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tools.Xml.Gpx;
using Tools.Xml.Sources;

namespace Tools.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class GpxXmlTest
    {
        public GpxXmlTest()
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

        #region v1.0

        [TestMethod]
        public void GpxReadv1_0Test()
        {
            // instantiate and load the gpx test document.
            XmlFileSource source = new XmlFileSource(new FileInfo(@"test.v1.0.gpx"));
            GpxDocument document = new GpxDocument(source);
            object gpx = document.Gpx;

            if (gpx is Tools.Xml.Gpx.v1_0.gpx)
            { // all ok here!
                Tools.Xml.Gpx.v1_0.gpx gpx_type = (gpx as Tools.Xml.Gpx.v1_0.gpx);

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

        [TestMethod]
        public void GpxWritev1_0Test()
        {
            // instantiate and load the gpx test document.
            XmlFileSource source = new XmlFileSource(new FileInfo(@"test.v1.0.gpx"));
            GpxDocument document = new GpxDocument(source);
            object gpx = document.Gpx;

            if (gpx is Tools.Xml.Gpx.v1_0.gpx)
            { // all ok here!
                // get the target file.
                FileInfo write_file = new FileInfo(@"test.written.v1.0.gpx");

                // create a new xml source.
                XmlFileSource write_source = new XmlFileSource(write_file);
                GpxDocument gpx_target = new GpxDocument(write_source);

                // set the target data the same as the source document.
                gpx_target.Gpx = gpx;

                // save the data.
                gpx_target.Save();

                // close the old document.
                document.Close();
                source.Close();

                // check to see if the data was writter correctly.
                // instantiate and load the gpx test document.
                source = new XmlFileSource(write_file);
                document = new GpxDocument(source);
                gpx = document.Gpx;

                if (gpx is Tools.Xml.Gpx.v1_0.gpx)
                { // all ok here!
                    Tools.Xml.Gpx.v1_0.gpx gpx_type = (gpx as Tools.Xml.Gpx.v1_0.gpx);

                    // test the gpx test file content.
                    Assert.IsNotNull(gpx_type.trk, "Gpx has not track!");
                    Assert.AreEqual(gpx_type.trk[0].trkseg.Length, 424, "Not the correct number of track segments found!");
                }
                else
                {
                    Assert.Fail("No gpx data was read, or data was of the incorrect type!");
                }

            }
            else
            {
                Assert.Fail("No gpx data was read, or data was of the incorrect type!");
            }

            document.Close();
            source.Close();
        }

        #endregion

        #region v1.1

        [TestMethod]
        public void GpxReadv1_1Test()
        {
            // instantiate and load the gpx test document.
            XmlFileSource source = new XmlFileSource(new FileInfo(@"test.v1.1.gpx"));
            GpxDocument document = new GpxDocument(source);
            object gpx = document.Gpx;

            if (gpx is Tools.Xml.Gpx.v1_1.gpxType)
            { // all ok here!
                Tools.Xml.Gpx.v1_1.gpxType gpx_type = (gpx as Tools.Xml.Gpx.v1_1.gpxType);

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

        [TestMethod]
        public void GpxWritev1_1Test()
        {
            // instantiate and load the gpx test document.
            XmlFileSource source = new XmlFileSource(new FileInfo(@"test.v1.1.gpx"));
            GpxDocument document = new GpxDocument(source);
            object gpx = document.Gpx;

            if (gpx is Tools.Xml.Gpx.v1_1.gpxType)
            { // all ok here!
                // get the target file.
                FileInfo write_file = new FileInfo(@"test.written.v1.1.gpx");

                // create a new xml source.
                XmlFileSource write_source = new XmlFileSource(write_file);
                GpxDocument gpx_target = new GpxDocument(write_source);

                // set the target data the same as the source document.
                gpx_target.Gpx = gpx;

                // save the data.
                gpx_target.Save();

                // close the old document.
                document.Close();
                source.Close();

                // check to see if the data was writter correctly.
                // instantiate and load the gpx test document.
                source = new XmlFileSource(write_file);
                document = new GpxDocument(source);
                gpx = document.Gpx;
                if (gpx is Tools.Xml.Gpx.v1_1.gpxType)
                { // all ok here!
                    Tools.Xml.Gpx.v1_1.gpxType gpx_type = (gpx as Tools.Xml.Gpx.v1_1.gpxType);

                    // test the gpx test file content.
                    Assert.IsNotNull(gpx_type.trk, "Gpx has not track!");
                    Assert.AreEqual(gpx_type.trk[0].trkseg.Length, 1, "Not the correct number of track segments found!");
                }
                else
                {
                    Assert.Fail("No gpx data was read, or data was of the incorrect type!");
                }

            }
            else
            {
                Assert.Fail("No gpx data was read, or data was of the incorrect type!");
            }

            document.Close();
            source.Close();
        }


        #endregion
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tools.Xml.Sources;
using System.IO;
using Tools.Xml.Nomatim.Search;

namespace Tools.Test
{
    /// <summary>
    /// Summary description for NomatimXmlTest
    /// </summary>
    [TestClass]
    public class NomatimXmlTest
    {
        public NomatimXmlTest()
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


        #region v2.0

        /// <summary>
        /// Reads a v2.0 kml.
        /// </summary>
        [TestMethod]
        public void NomatimSearchReadTestv1()
        {
            // instantiate and load the gpx test document.
            XmlFileSource source = new XmlFileSource(new FileInfo(@"test_search.v1.xml"));
            SearchDocument document = new SearchDocument(source);
            object search = document.Search;

            if (search is Tools.Xml.Nomatim.Search.v1.searchresults)
            { // all ok here!
                Assert.IsTrue((search as Tools.Xml.Nomatim.Search.v1.searchresults).place.Length == 1);
                Assert.IsTrue((search as Tools.Xml.Nomatim.Search.v1.searchresults).place[0].country_code == "be");
            }
            else
            {
                Assert.Fail("No search data was read, or data was of the incorrect type!");
            }

            document.Close();
            source.Close();
        }

        #endregion

    }
}

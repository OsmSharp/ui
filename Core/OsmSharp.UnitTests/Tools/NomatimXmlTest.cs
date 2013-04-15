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
using NUnit.Framework;
using OsmSharp.Tools.Xml.Sources;
using System.IO;
using OsmSharp.Tools.Xml.Nominatim.Search;

namespace OsmSharp.UnitTests
{
    /// <summary>
    /// Summary description for NomatimXmlTest
    /// </summary>
    [TestFixture]
    public class NomatimXmlTest
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

        ///// <summary>
        ///// Reads a v2.0 kml.
        ///// </summary>
        //[Test]
        //public void NomatimSearchReadTestv1()
        //{
        //    // instantiate and load the gpx test document.
        //    XmlFileSource source = new XmlFileSource(new FileInfo(@"test_search.v1.xml"));
        //    SearchDocument document = new SearchDocument(source);
        //    object search = document.Search;

        //    if (search is OsmSharp.Tools.Xml.Nomatim.Search.v1.searchresults)
        //    { // all ok here!
        //        Assert.IsTrue((search as OsmSharp.Tools.Xml.Nomatim.Search.v1.searchresults).place.Length == 1);
        //        Assert.IsTrue((search as OsmSharp.Tools.Xml.Nomatim.Search.v1.searchresults).place[0].country_code == "be");
        //    }
        //    else
        //    {
        //        Assert.Fail("No search data was read, or data was of the incorrect type!");
        //    }

        //    document.Close();
        //    source.Close();
        //}

        #endregion

    }
}

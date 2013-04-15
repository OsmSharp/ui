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
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Geo.Lambert;
using OsmSharp.Tools.Math.Geo.Lambert.Ellipsoids;
using OsmSharp.Tools.Math.Units.Angle;
using OsmSharp.Tools.Math.Geo.Lambert.International.Belgium;

namespace OsmSharp.UnitTests
{
    /// <summary>
    /// Summary description for LambertProjectionTest
    /// </summary>
    [TestFixture]
    public class LambertProjectionTest
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

        /// <summary>
        /// Test a lamber coordinate conversion.
        /// </summary>
        [Test]
        public void TestConversion()
        {
            double x = 180246.6;
            double y = 217666.77;

            GeoCoordinate coordinate = LambertProjectionBase.Belgium1972LambertProjection.ConvertToWGS84(x, y);
            //return new GeoCoordinate(result2, result1);

        }
    }
}

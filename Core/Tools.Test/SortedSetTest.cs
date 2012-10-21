// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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
using Tools.Core.Collections;

namespace Tools.Test
{
    /// <summary>
    /// Summary description for SortedSetTest
    /// </summary>
    [TestClass]
    public class SortedSetTest
    {
        public SortedSetTest()
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

        [TestMethod]
        public void TestSortedSet()
        {
            // initialize a set with integers.
            Tools.Core.Collections.SortedSet<int> set = new Tools.Core.Collections.SortedSet<int>();

            // test initial conditions.
            Assert.AreEqual(0, set.Count, "Set count not 0 when empty!");

            // add two elements.
            set.Add(0);
            set.Add(10);
            List<int> elements = new List<int>(set);
            Assert.AreEqual(0, elements[0]);
            Assert.AreEqual(10, elements[1]);

            // add more elements.
            set.Add(-1);
            set.Add(11);
            set.Add(2);
            set.Add(4);
            elements = new List<int>(set);

        }
    }
}

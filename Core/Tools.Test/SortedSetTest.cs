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

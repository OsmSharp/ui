using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Collections;

namespace OsmSharp.UnitTests.Tools
{
    /// <summary>
    /// Holds sparse array tests.
    /// </summary>
    [TestFixture]
    public class SparseArrayTests
    {
        /// <summary>
        /// Does a simple sparse array test.
        /// </summary>
        [Test]
        public void TestSparseArraySimple()
        {
            // intialize.
            var array = new SparseArray<int>(10);

            // fill and resize in the process.
            for (int idx = 0; idx < 1000; idx++)
            {
                if (idx >= array.Length)
                {
                    array.Resize(idx + 100);
                }
                array[idx] = idx;
            }
            for (int idx = 5000; idx < 10000; idx++)
            {
                if (idx >= array.Length)
                {
                    array.Resize(idx + 100);
                }
                array[idx] = idx;
            }

            // test content.
            for (int idx = 0; idx < 1000; idx++)
            {
                Assert.AreEqual(idx, array[idx]);
            }
            for (int idx = 1001; idx < 5000; idx++)
            {
                Assert.AreEqual(0, array[idx]);
            }
            for (int idx = 5000; idx < 10000; idx++)
            {
                Assert.AreEqual(idx, array[idx]);
            }

            // test enumerator.
            var list = new List<int>(array);
            for (int idx = 0; idx < 1000; idx++)
            {
                Assert.AreEqual(idx, list[idx]);
            }
            for (int idx = 1001; idx < 5000; idx++)
            {
                Assert.AreEqual(0, list[idx]);
            }
            for (int idx = 5000; idx < 10000; idx++)
            {
                Assert.AreEqual(idx, list[idx]);
            }
        }
    }
}

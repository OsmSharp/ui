using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Tools.Core.Enumerations;

namespace OsmSharp.UnitTests.Tools
{
    /// <summary>
    /// Contains tests for the Permutation Enumeration class.
    /// </summary>
    [TestClass]
    public class PermutationEnumerationTests
    {
        /// <summary>
        /// Tests a sorted set.
        /// </summary>
        [TestMethod]
        public void TestPermutationCount()
        {
            int[] test_sequence = new int[] { 1, 2 };
            PermutationEnumerable<int> enumerator =
                new PermutationEnumerable<int>(test_sequence);
            List<int[]> set = new List<int[]>(enumerator);
            Assert.AreEqual(2, set.Count);

            test_sequence = new int[] { 1, 2, 3 };
            enumerator =
                new PermutationEnumerable<int>(test_sequence);
            set = new List<int[]>(enumerator);
            Assert.AreEqual(6, set.Count);

            test_sequence = new int[] { 1, 2, 3, 4 };
            enumerator =
                new PermutationEnumerable<int>(test_sequence);
            set = new List<int[]>(enumerator);
            Assert.AreEqual(24, set.Count);

            test_sequence = new int[] { 1, 2, 3, 4, 5 };
            enumerator =
                new PermutationEnumerable<int>(test_sequence);
            set = new List<int[]>(enumerator);
            Assert.AreEqual(120, set.Count);
        }
    }
}
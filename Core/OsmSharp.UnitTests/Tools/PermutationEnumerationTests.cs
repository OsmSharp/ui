using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Tools.Enumerations;

namespace OsmSharp.UnitTests.Tools
{
    /// <summary>
    /// Contains tests for the Permutation Enumeration class.
    /// </summary>
    [TestFixture]
    public class PermutationEnumerationTests
    {
        /// <summary>
        /// Tests the permutation counts.
        /// </summary>
        [Test]
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


        /// <summary>
        /// Tests the permutation contents.
        /// </summary>
        [Test]
        public void TestPermutationContent()
        {
            int[] test_sequence = new int[] { 1, 2 };
            PermutationEnumerable<int> enumerator =
                new PermutationEnumerable<int>(test_sequence);
            List<int[]> set = new List<int[]>(enumerator);
            Assert.AreEqual(2, set.Count);
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 1 }));

            test_sequence = new int[] { 1, 2, 3 };
            enumerator =
                new PermutationEnumerable<int>(test_sequence);
            set = new List<int[]>(enumerator);
            Assert.AreEqual(6, set.Count);
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 2, 3 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 3, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 1, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 2, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 3, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 1, 3 }));

            // 4 items tests all the crucial elements of the algorithm. (full code coverage)
            test_sequence = new int[] { 1, 2, 3, 4 };
            enumerator =
                new PermutationEnumerable<int>(test_sequence);
            set = new List<int[]>(enumerator);
            Assert.AreEqual(24, set.Count);
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 2, 3, 4 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 3, 2, 4 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 1, 2, 4 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 2, 1, 4 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 3, 1, 4 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 1, 3, 4 }));

            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 2, 4, 3 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 3, 4, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 1, 4, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 2, 4, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 3, 4, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 1, 4, 3 }));

            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 4, 2, 3 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 4, 3, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 4, 1, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 4, 2, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 4, 3, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 4, 1, 3 }));

            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 1, 2, 3 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 1, 3, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 3, 1, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 3, 2, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 2, 3, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 2, 1, 3 }));
        }

        private bool TestPermutationContent(List<int[]> permuations, int[] permutation)
        {
            foreach (int[] current in permuations)
            {
                bool equal = true;
                for (int idx = 0; idx < current.Length; idx++)
                {
                    if (current[idx] != permutation[idx])
                    {
                        equal = false;
                        break;
                    }
                }
                if (equal)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
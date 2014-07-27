using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Collections;
using OsmSharp.Math.Random;

namespace OsmSharp.Test.Unittests.Collections
{
    /// <summary>
    /// Contains tests for the huge array.
    /// </summary>
    [TestFixture]
    public class HugeArrayTests
    {
        /// <summary>
        /// A simple test for the huge array.
        /// </summary>
        [Test]
        public void HugeArraySimpleTest()
        {
            var stringArrayRef = new string[1000];
            var stringArray = new HugeArray<string>(1000, 300);

            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
            for (int idx = 0; idx < 1000; idx++)
            {
                if (randomGenerator.Generate(2.0) > 1)
                { // add data.
                    stringArrayRef[idx] = idx.ToString();
                    stringArray[idx] = idx.ToString();
                }
                else
                {
                    stringArrayRef[idx] = null;
                    stringArray[idx] = null;
                }
            }

            for (int idx = 0; idx < 1000; idx++)
            {
                Assert.AreEqual(stringArrayRef[idx], stringArray[idx]);
            }
        }

        /// <summary>
        /// A simple test resizing the huge array 
        /// </summary>
        [Test]
        public void HugeArrayResizeTests()
        {
            var stringArrayRef = new string[1000];
            var stringArray = new HugeArray<string>(1000, 300);

            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
            for (int idx = 0; idx < 1000; idx++)
            {
                if (randomGenerator.Generate(2.0) > 1)
                { // add data.
                    stringArrayRef[idx] = idx.ToString();
                    stringArray[idx] = idx.ToString();
                }
                else
                {
                    stringArrayRef[idx] = null;
                    stringArray[idx] = null;
                }
            }

            Array.Resize<string>(ref stringArrayRef, 335);
            stringArray.Resize(335);

            Assert.AreEqual(stringArrayRef.Length, stringArray.Length);
            for (int idx = 0; idx < stringArrayRef.Length; idx++)
            {
                Assert.AreEqual(stringArrayRef[idx], stringArray[idx]);
            }

            stringArrayRef = new string[1000];
            stringArray = new HugeArray<string>(1000, 300);

            for (int idx = 0; idx < 1000; idx++)
            {
                if (randomGenerator.Generate(2.0) > 1)
                { // add data.
                    stringArrayRef[idx] = idx.ToString();
                    stringArray[idx] = idx.ToString();
                }
                else
                {
                    stringArrayRef[idx] = null;
                    stringArray[idx] = null;
                }
            }

            Array.Resize<string>(ref stringArrayRef, 1235);
            stringArray.Resize(1235);

            Assert.AreEqual(stringArrayRef.Length, stringArray.Length);
            for (int idx = 0; idx < stringArrayRef.Length; idx++)
            {
                Assert.AreEqual(stringArrayRef[idx], stringArray[idx]);
            }
        }
    }
}
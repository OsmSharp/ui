using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.IO.ByteCache;
using OsmSharp.Math.Random;

namespace OsmSharp.UnitTests.IO.ByteCache
{
    /// <summary>
    /// Holds tests for the byte cache implementations.
    /// </summary>
    [TestFixture]
    public class ByteCacheTests
    {
        /// <summary>
        /// Does simple byte cache memory tests.
        /// </summary>
        [Test]
        public void ByteCacheMemorySimpleTests()
        {
            int total = 1000;
            var stringArrayRef = new string[total];
            var memoryByteCache = new MemoryByteCache();

            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
            for (int idx = 0; idx < total; idx++)
            {
                if (randomGenerator.Generate(2.0) > 1)
                { // add data.
                    stringArrayRef[idx] = idx.ToString();
                    memoryByteCache.Add(ASCIIEncoding.ASCII.GetBytes(idx.ToString()));
                }
                else
                {
                    stringArrayRef[idx] = "null";
                    memoryByteCache.Add(ASCIIEncoding.ASCII.GetBytes("null"));
                }
            }

            for (uint idx = 0; idx < total; idx++)
            {
                string content = ASCIIEncoding.ASCII.GetString(memoryByteCache.Get(idx));
                memoryByteCache.Remove(idx);
                Assert.AreEqual(total - idx - 1, memoryByteCache.Size);
                Assert.AreEqual(stringArrayRef[idx], content);
            }
        }

        /// <summary>
        /// Does simple byte cache disk tests.
        /// </summary>
        [Test]
        public void ByteCacheDiskSimpleTests()
        {
            int total = 1000;
            var stringArrayRef = new string[total];
            var memoryByteCache = new DiskByteCache();

            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
            for (int idx = 0; idx < total; idx++)
            {
                if (randomGenerator.Generate(2.0) > 1)
                { // add data.
                    stringArrayRef[idx] = idx.ToString();
                    memoryByteCache.Add(ASCIIEncoding.ASCII.GetBytes(idx.ToString()));
                }
                else
                {
                    stringArrayRef[idx] = "null";
                    memoryByteCache.Add(ASCIIEncoding.ASCII.GetBytes("null"));
                }
            }

            for (uint idx = 0; idx < total; idx++)
            {
                string content = ASCIIEncoding.ASCII.GetString(memoryByteCache.Get(idx + 1));
                memoryByteCache.Remove(idx + 1);
                Assert.AreEqual(total - idx - 1, memoryByteCache.Size);
                Assert.AreEqual(stringArrayRef[idx], content);
            }

            memoryByteCache.Dispose();
        }
    }
}

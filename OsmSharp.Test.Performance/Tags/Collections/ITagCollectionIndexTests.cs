using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Test.Performance.Tags.Collections
{
    /// <summary>
    /// Contains generic test cases for ITagCollectionIndex implementations.
    /// </summary>
    public static class ITagCollectionIndexTests
    {
        /// <summary>
        /// Executes the tests.
        /// </summary>
        public static void Test(string name, ITagsCollectionIndex index, int addCount, int accessCount)
        {
            // tests adding tag collection to the given index.
            ITagCollectionIndexTests.TestAdd(name,
                new TagsTableCollectionIndex(), addCount);

            // tests random access.
            ITagCollectionIndexTests.TestRandomAccess(name, index, accessCount);
        }

        /// <summary>
        /// Tests adding simple tags to the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="collectionCount"></param>
        public static void FillIndex(ITagsCollectionIndex index, int collectionCount)
        {
            for (int i = 0; i < collectionCount; i++)
            {
                SimpleTagsCollection tagsCollection = new SimpleTagsCollection();
                int tagCollectionSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(3) + 1;
                for (int idx = 0; idx < tagCollectionSize; idx++)
                {
                    int tagValue = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(100);
                    tagsCollection.Add(
                        string.Format("key_{0}", tagValue),
                        string.Format("value_{0}", tagValue));
                }
                int addCount = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(2) + 1;
                for (int idx = 0; idx < addCount; idx++)
                {
                    uint tagsId = index.Add(tagsCollection);
                }
            }
        }

        /// <summary>
        /// Tests adding simple tags to the given index.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="collectionCount"></param>
        public static void TestAdd(string name, ITagsCollectionIndex index,
            int collectionCount)
        {
            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer(string.Format("{0}.Add", name));
            performanceInfo.Start();
            performanceInfo.Report("Adding {0} tag collections...", collectionCount);

            ITagCollectionIndexTests.FillIndex(index, collectionCount);

            performanceInfo.Stop();

            Console.Write("", index.Max);
        }

        /// <summary>
        /// Tests random access to an already filled index.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="testCount"></param>
        public static void TestRandomAccess(string name, ITagsCollectionIndexReadonly index,
            int testCount)
        {
            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer(string.Format("{0}.Access", name));
            performanceInfo.Start();
            performanceInfo.Report("Random accessing tags index {0} times...", testCount);

            for (int idx = 0; idx < testCount; idx++)
            {
                uint collectionIndex = (uint)OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(index.Max);
                TagsCollection collection = index.Get(collectionIndex);
            }

            performanceInfo.Stop();
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Collections.Tags;
//using OsmSharp.Osm.Streams;
//using System.IO;
//using OsmSharp.Osm.PBF.Streams;
//using OsmSharp.Collections.Tags.Index;

//namespace OsmSharp.Test.Performance.Tags.Collections
//{
//    /// <summary>
//    /// Contains generic test cases for ITagCollectionIndex implementations.
//    /// </summary>
//    public static class ITagCollectionIndexTests
//    {
//        /// <summary>
//        /// Executes the tests.
//        /// </summary>
//        public static void Test(string name, ITagsIndex index, int addCount, int accessCount)
//        {
//            // tests adding tag collection to the given index.
//            ITagCollectionIndexTests.TestAdd(name,
//                new TagsIndex(), addCount);

//            // tests random access.
//            ITagCollectionIndexTests.TestRandomAccess(name, index, accessCount);
//        }

//        /// <summary>
//        /// Executes a test adding tags from a PBF file.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <param name="index"></param>
//        /// <param name="pbfFile"></param>
//        public static void Test(string name, ITagsIndex index, string pbfFile)
//        {
//            FileInfo testFile = new FileInfo(string.Format(@".\TestFiles\{0}",pbfFile));
//            Stream stream = testFile.OpenRead();
//            PBFOsmStreamSource source = new PBFOsmStreamSource(stream);
//            ITagCollectionIndexTests.TestAdd(name, index, source);
//        }

//        /// <summary>
//        /// Tests adding simple tags to the given index.
//        /// </summary>
//        /// <param name="index"></param>
//        /// <param name="collectionCount"></param>
//        public static void FillIndex(ITagsIndex index, int collectionCount)
//        {
//            for (int i = 0; i < collectionCount; i++)
//            {
//                TagsCollection tagsCollection = new TagsCollection();
//                int tagCollectionSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(3) + 1;
//                for (int idx = 0; idx < tagCollectionSize; idx++)
//                {
//                    int tagValue = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(100);
//                    tagsCollection.Add(
//                        string.Format("key_{0}", tagValue),
//                        string.Format("value_{0}", tagValue));
//                }
//                int addCount = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(2) + 1;
//                for (int idx = 0; idx < addCount; idx++)
//                {
//                    uint tagsId = index.Add(tagsCollection);
//                }
//            }
//        }

//        /// <summary>
//        /// Tests adding simple tags to the given index.
//        /// </summary>
//        /// <param name="index"></param>
//        /// <param name="source"></param>
//        public static void FillIndex(ITagsIndex index, OsmStreamSource source)
//        {
//            OsmStreamTargetTags tagsTarget = new OsmStreamTargetTags(index);
//            tagsTarget.RegisterSource(source);
//            tagsTarget.Pull();
//        }

//        /// <summary>
//        /// Tests adding simple tags to the given index.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <param name="index"></param>
//        /// <param name="collectionCount"></param>
//        public static void TestAdd(string name, ITagsIndex index,
//            int collectionCount)
//        {
//            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer(string.Format("{0}.Add", name));
//            performanceInfo.Start();
//            performanceInfo.Report("Adding {0} tag collections...", collectionCount);

//            ITagCollectionIndexTests.FillIndex(index, collectionCount);

//            performanceInfo.Stop();

//            Console.Write("", index.Max);
//        }

//        /// <summary>
//        /// Tests adding simple tags to the given index.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <param name="index"></param>
//        /// <param name="source"></param>
//        public static void TestAdd(string name, ITagsIndex index,
//            OsmStreamSource source)
//        {
//            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer(string.Format("{0}.Add", name));
//            performanceInfo.Start();
//            performanceInfo.Report("Adding tags from {0}...", source.ToString());

//            ITagCollectionIndexTests.FillIndex(index, source);

//            performanceInfo.Stop();

//            Console.Write("", index.Max);
//        }

//        /// <summary>
//        /// Tests random access to an already filled index.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <param name="index"></param>
//        /// <param name="testCount"></param>
//        public static void TestRandomAccess(string name, ITagsCollectionIndexReadonly index,
//            int testCount)
//        {
//            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer(string.Format("{0}.Access", name));
//            performanceInfo.Start();
//            performanceInfo.Report("Random accessing tags index {0} times...", testCount);

//            for (int idx = 0; idx < testCount; idx++)
//            {
//                uint collectionIndex = (uint)OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(index.Max);
//                TagsCollectionBase collection = index.Get(collectionIndex);
//            }

//            performanceInfo.Stop();
//        }
//    }
//}

//// OsmSharp - OpenStreetMap (OSM) SDK
//// Copyright (C) 2015 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

//using OsmSharp.Collections.Tags.Index;
//using System.IO;

//namespace OsmSharp.Test.Performance.Tags.Collections
//{
//    /// <summary>
//    /// Tests for the serialized blocked tags collection index.
//    /// </summary>
//    public static class BlockedTagsCollectionIndexTests
//    {
//        /// <summary>
//        /// Executes the tests.
//        /// </summary>
//        public static void Test()
//        {
//            TagsIndex index = new TagsIndex();

//            // first fill the index.
//            ITagCollectionIndexTests.FillIndex(index, 100000);

//            // serialize the index.
//            FileInfo testFile = new FileInfo(@"test.file");
//            testFile.Delete();
//            Stream writeStream = testFile.OpenWrite();
//            OsmSharp.Logging.Log.TraceEvent("Blocked", OsmSharp.Logging.TraceEventType.Information,
//                "Serializing blocked file....");
//            TagIndexSerializer.SerializeBlocks(writeStream, index, 100);
//            writeStream.Flush();
//            writeStream.Dispose();

//            OsmSharp.Logging.Log.TraceEvent("Blocked", OsmSharp.Logging.TraceEventType.Information,
//                string.Format("Serialized file: {0}KB", testFile.Length / 1024));


//            // deserialize the index.
//            Stream readStream = testFile.OpenRead();
//            ITagsCollectionIndexReadonly readOnlyIndex = TagIndexSerializer.DeserializeBlocks(readStream);

//            // test access.
//            OsmSharp.Logging.Log.TraceEvent("Blocked", OsmSharp.Logging.TraceEventType.Information,
//                "Started testing random access....");
//            ITagCollectionIndexTests.TestRandomAccess("Blocked", readOnlyIndex, 1000);

//            readStream.Dispose();
//            testFile.Delete();
//        }
//    }
//}

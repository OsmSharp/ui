// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.IO;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Osm.Streams.Filters;
using System;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Streams;

namespace OsmSharp.Test.Performance.Routing
{
    /// <summary>
    /// Contains test for the preprocessing step.
    /// </summary>
    public static class PreProcessorTest
    {
        /// <summary>
        /// Tests the pre-processor.
        /// </summary>
        public static void Test()
        {
            PreProcessorTest.TestPreprocessing("PreProcessor", "kempen-big.osm.pbf");
        }

        /// <summary>
        /// Tests preprocessing data from a PBF file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pbfFile"></param>
        public static void TestPreprocessing(string name, string pbfFile)
        {
            FileInfo testFile = new FileInfo(string.Format(@".\TestFiles\{0}", pbfFile));
            Stream stream = testFile.OpenRead();
            var progress = new OsmStreamFilterProgress();
            progress.RegisterSource(new PBFOsmStreamSource(stream));

            var performanceInfo = new PerformanceInfoConsumer("PreProcessor", 20000);
            performanceInfo.Start();
            performanceInfo.Report("Pulling from {0}...", testFile.Name);

            var tagsIndex = new TagsIndex(); // creates a tagged index.

            // read from the OSM-stream.
            var memoryData = new RouterDataSource<Edge>(new Graph<Edge>(), tagsIndex);
            var targetData = new GraphOsmStreamTarget(memoryData, new OsmRoutingInterpreter(), tagsIndex);
            targetData.RegisterSource(progress);
            targetData.Pull();

            stream.Dispose();

            performanceInfo.Stop();
            // make sure the router is still here after GC to note the memory difference.
            OsmSharp.Logging.Log.TraceEvent("PreProcessor", Logging.TraceEventType.Information, memoryData.ToString());
            memoryData = null;

            GC.Collect();
        }
    }
}
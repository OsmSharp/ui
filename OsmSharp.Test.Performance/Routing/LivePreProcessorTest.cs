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

namespace OsmSharp.Test.Performance.Routing
{
    /// <summary>
    /// Contains test for the Live preprocessing step.
    /// </summary>
    public static class LivePreProcessorTest
    {
        /// <summary>
        /// Tests the Live pre-processor.
        /// </summary>
        public static void Test()
        {
            LivePreProcessorTest.TestPreprocessing("LivePreProcessor", "belgium-latest.osm.pbf");
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
            PBFOsmStreamSource source = new PBFOsmStreamSource(stream);
            OsmStreamFilterProgress progress = new OsmStreamFilterProgress(source);

            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer("LivePreProcessor");
            performanceInfo.Start();
            performanceInfo.Report("Pulling from {0}...", testFile.Name);

            var router = Router.CreateLiveFrom(progress, new OsmRoutingInterpreter());

            stream.Dispose();

            performanceInfo.Stop();
            // make sure the route is still here after GC to note the memory difference.
            OsmSharp.Logging.Log.TraceEvent("LivePreProcessor", Logging.TraceEventType.Information, router.ToString());
            router = null;

            GC.Collect();
        }
    }
}
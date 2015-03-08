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

using OsmSharp.Routing;
using System.IO;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Vehicles;
namespace OsmSharp.Test.Performance.Routing.CH
{
    /// <summary>
    /// Contains test for the CH preprocessing step.
    /// </summary>
    public static class CHPreProcessorTest
    {
        /// <summary>
        /// Tests the CH pre-processor.
        /// </summary>
        public static void Test()
        {
            CHPreProcessorTest.TestPreprocessing("CHPreProcessor", "kempen-big.osm.pbf");
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
            var source = new OsmSharp.Osm.Streams.Filters.OsmStreamFilterProgress();
            source.RegisterSource(new PBFOsmStreamSource(stream));

            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer("CHPreProcessor.Pre");
            performanceInfo.Start();
            performanceInfo.Report("Pulling from {0}...", testFile.Name);

            Router.CreateCHFrom(source, new OsmRoutingInterpreter(), Vehicle.Car);

            stream.Dispose();

            performanceInfo.Stop();
        }
    }
}

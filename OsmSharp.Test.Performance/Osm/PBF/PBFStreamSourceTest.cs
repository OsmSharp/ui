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
using OsmSharp.Osm.Streams;

namespace OsmSharp.Test.Performance.Osm.PBF
{
    /// <summary>
    /// Tests performance of PBF reads.
    /// </summary>
    public static class PBFStreamSourceTest
    {
        /// <summary>
        /// Executes reading tests.
        /// </summary>
        public static void Test()
        {
            FileInfo testFile = new FileInfo(@".\TestFiles\kempen.osm.pbf");
            Stream stream = testFile.OpenRead();
            PBFOsmStreamSource source = new PBFOsmStreamSource(stream);
            OsmStreamTargetEmpty emptyTarget = new OsmStreamTargetEmpty();
            emptyTarget.RegisterSource(source);

            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer("PBFOsmStreamSource.Pull");
            performanceInfo.Start();
            performanceInfo.Report("Pulling from {0}...", testFile.Name);

            emptyTarget.Pull();
            stream.Dispose();

            performanceInfo.Stop();
        }
    }
}
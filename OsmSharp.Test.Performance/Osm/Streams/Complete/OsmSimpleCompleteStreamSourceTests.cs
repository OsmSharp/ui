using OsmSharp.Osm;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Osm.Streams.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.Test.Performance.Osm.Streams.Complete
{
    /// <summary>
    /// Tests performance of complete object source reads.
    /// </summary>
    public static class OsmSimpleCompleteStreamSourceTests
    {
        /// <summary>
        /// Executes reading tests.
        /// </summary>
        public static void Test()
        {
            var testFile = new FileInfo(@".\TestFiles\kempen-big.osm.pbf");
            var stream = testFile.OpenRead();
            var source = new PBFOsmStreamSource(stream);
            var completeSource = new OsmSharp.Osm.Streams.Complete.OsmSimpleCompleteStreamSource(new OsmStreamFilterProgress(source));

            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer("OsmSimpleCompleteStreamSourceTests.Pull");
            performanceInfo.Start();
            performanceInfo.Report("Pulling from {0}...", testFile.Name);

            var completeObjects = new List<ICompleteOsmGeo>(completeSource);

            stream.Dispose();

            performanceInfo.Stop();
        }
    }
}

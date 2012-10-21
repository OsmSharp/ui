using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Osm.Data.PBF;
using System.Diagnostics;
using Osm.Data.Core.Processor.Default;
using Osm.Data.PBF.Raw.Processor;

namespace Osm.Routing.Test.PBF
{
    public static class PBFTest
    {
        public static void TestPBFRead()
        {
            string pbf = @"C:\OSM\bin\albania.osm.pbf";
            FileStream file = File.OpenRead(pbf);

            //PBFReader reader = new PBFReader(file);
            //reader.ReadAll(new Consumer());
            DataProcessorTargetEmpty target = new DataProcessorTargetEmpty();
            target.RegisterSource(new PBFDataProcessorSource(file));
            target.Pull();
        }
    }

    class Consumer : IPBFPrimitiveBlockConsumer
    {

        public void ProcessPrimitiveBlock(PrimitiveBlock block)
        {
            foreach(PrimitiveGroup group in block.primitivegroup)
            {
                DenseNodes dense = group.dense;

                foreach (Node node in group.nodes)
                {
                    Trace.Write(node.ToString());
                }
            }
        }
    }
}

// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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

using OsmSharp.Osm.Core;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using System.Reflection;
using OsmSharp.Osm.Routing.Interpreter;
using System.IO;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Osm.Data.Core.Processor.Progress;
using System;
using OsmSharp.Osm.Data.XML.Processor;
namespace OsmSharp.Osm.Routing.Test.CH
{
    class CHTest
    {
        public static void Execute()
        {
            //CHTest.DoContraction(new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf").OpenRead(), true);
            //CHTest.DoContraction("OsmSharp.Osm.Routing.Test.TestData.matrix_big_area.osm", false);
            CHTest.DoContraction("OsmSharp.Osm.Routing.Test.TestData.moscow.osm", false);
        }


        private static void DoContraction(string xml_embedded, bool pbf)
        {
            CHTest.DoContraction(Assembly.GetExecutingAssembly().GetManifestResourceStream(xml_embedded), pbf);
        }

        private static void DoContraction(Stream data_stream, bool pbf)
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<CHEdgeData> osm_data =
                new MemoryRouterDataSource<CHEdgeData>(tags_index);
            CHEdgeDataGraphProcessingTarget target_data = new CHEdgeDataGraphProcessingTarget(
                osm_data, interpreter, osm_data.TagsIndex);
            DataProcessorSource data_processor_source;
            if (pbf)
            {
                data_processor_source = new PBFDataProcessorSource(data_stream);
            }
            else
            {
                data_processor_source = new XmlDataProcessorSource(data_stream);
            }
            data_processor_source = new ProgressDataProcessorSource(data_processor_source);
            target_data.RegisterSource(data_processor_source);
            target_data.Pull();

            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Finished Loading data!");

            long start = DateTime.Now.Ticks;

            // do the pre-processing part.
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(osm_data);
            CHPreProcessor pre_processor = new CHPreProcessor(osm_data,
                new EdgeDifferenceContractedSearchSpace(osm_data, witness_calculator), witness_calculator);
            pre_processor.Start();

            long stop = DateTime.Now.Ticks;

            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Pre-processing time:{0}s for {1} nodes!", (new TimeSpan(stop-start)).TotalSeconds, osm_data.VertexCount);
        }
    }
}

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
using OsmSharp.Osm.Data.XML.Raw.Processor;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using System.Reflection;
using OsmSharp.Osm.Routing.Interpreter;
using System.IO;
namespace OsmSharp.Osm.Routing.Test.CH
{
    class CHTest
    {
        public static void Execute()
        {
            CHTest.DoContraction("matrix");
        }

        private static void DoContraction(string name)
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            string xml_embedded = string.Format("OsmSharp.Osm.Routing.Test.TestData.{0}.osm", name);
            Stream data_stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(xml_embedded);
            bool pbf = false;

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

            target_data.RegisterSource(data_processor_source);
            target_data.Pull();

            // do the pre-processing part.
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(osm_data);
            CHPreProcessor pre_processor = new CHPreProcessor(osm_data,
                new SparseOrdering(osm_data), witness_calculator);
            pre_processor.Start();
        }
    }
}
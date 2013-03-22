using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Osm;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Routing.CH.PreProcessing;
using System.Reflection;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Graph.Memory;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Osm.Data.XML.Processor;

namespace OsmSharp.Routing.Osm.Test.ManyToMany
{
    /// <summary>
    /// Executes many-to-many calculation performance tests using the CH data format.
    /// </summary>
    public class ManyToManyCHTests : ManyToManyTests
    {
        /// <summary>
        /// Creates a raw router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        protected override IRouter<RouterPoint> CreateRouter(
           Stream data_stream, IRoutingInterpreter interpreter)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<CHEdgeData> data =
                new MemoryRouterDataSource<CHEdgeData>(tags_index);
            CHEdgeDataGraphProcessingTarget target_data = new CHEdgeDataGraphProcessingTarget(
                data, interpreter, data.TagsIndex, VehicleEnum.Car);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(data_stream);
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            // do the pre-processing part.
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);
            CHPreProcessor pre_processor = new CHPreProcessor(data,
                new EdgeDifference(data, witness_calculator), witness_calculator);
            pre_processor.Start();

            return new Router<CHEdgeData>(data, interpreter, new CHRouter(data));
        }
    }
}

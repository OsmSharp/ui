using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Osm.Data.XML.Raw.Processor;
using Osm.Data.Core.Processor.Filter.Sort;
using Osm.Core;
using Routing.Core.Interpreter;
using Routing.Core;
using Routing.Core.Router.Memory;
using Osm.Routing.Data.Processing;
using Routing.CH.PreProcessing;
using System.Reflection;
using Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using Routing.CH.PreProcessing.Witnesses;
using Routing.CH.Routing;

namespace Osm.Routing.Test.ManyToMany
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
                data, interpreter, data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(data_stream);
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            // do the pre-processing part.
            CHPreProcessor pre_processor = new CHPreProcessor(data,
                new SparseOrdering(data), new DykstraWitnessCalculator(data));
            pre_processor.Start();

            return new Router<CHEdgeData>(data, interpreter, new CHRouter(data));
        }
    }
}

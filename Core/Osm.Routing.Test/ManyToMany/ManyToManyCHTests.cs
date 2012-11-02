using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.CH.Routing;
using Osm.Routing.Core;
using System.IO;
using Osm.Routing.Core.Interpreter;
using Osm.Routing.Core.Constraints;
using Osm.Data.XML.Raw.Processor;
using Osm.Data.Core.Processor.Filter.Sort;
using Osm.Routing.CH.Processor;
using Osm.Routing.CH.PreProcessing;
using Osm.Routing.CH.PreProcessing.Witnesses;
using Osm.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;

namespace Osm.Routing.Test.ManyToMany
{
    /// <summary>
    /// Executes many-to-many calculation performance tests using the CH data format.
    /// </summary>
    public class ManyToManyCHTests : ManyToManyTests<CHResolvedPoint>
    {
        /// <summary>
        /// Creates a raw router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        protected override IRouter<CHResolvedPoint> CreateRouter(
           Stream data, RoutingInterpreterBase interpreter, IRoutingConstraints constraints)
        {
            // build the memory data source.
            CHDataSource ch_data = new CHDataSource();

            // load the data.
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(data);
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            CHDataProcessorTarget ch_target = new CHDataProcessorTarget(ch_data);
            ch_target.RegisterSource(sorter);
            ch_target.Pull();

            // do the pre-processing part.
            CHPreProcessor pre_processor = new CHPreProcessor(ch_data.Graph,
                new SparseOrdering(ch_data.Graph), new DykstraWitnessCalculator(ch_data.Graph));
            pre_processor.Start();

            // create the router from the contracted data.
            return new Router(ch_data);
        }
    }
}

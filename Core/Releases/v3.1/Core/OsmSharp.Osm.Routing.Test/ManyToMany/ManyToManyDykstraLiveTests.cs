using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Core;
using System.IO;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Osm.Core;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Routing.Core.Graph.DynamicGraph.SimpleWeighed;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Core.Graph.Router.Dykstra;
using OsmSharp.Osm.Data.XML.Processor;

namespace OsmSharp.Osm.Routing.Test.ManyToMany
{
    /// <summary>
    /// Does some tests on the many to many calculations of the dykstra live variant.
    /// </summary>
    public class ManyToManyDykstraLiveTests : ManyToManyTests
    {
        /// <summary>
        /// Creates a router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        protected override IRouter<RouterPoint> CreateRouter(Stream data, 
            IRoutingInterpreter interpreter)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<SimpleWeighedEdge> osm_data =
                new MemoryRouterDataSource<SimpleWeighedEdge>(tags_index);
            SimpleWeighedDataGraphProcessingTarget target_data = new SimpleWeighedDataGraphProcessingTarget(
                osm_data, interpreter, osm_data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(data);
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            return new Router<SimpleWeighedEdge>(osm_data, interpreter,
                new DykstraRoutingLive(osm_data.TagsIndex));
        }
    }
}

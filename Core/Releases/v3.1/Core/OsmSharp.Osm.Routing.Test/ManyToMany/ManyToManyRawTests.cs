using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OsmSharp.Osm.Data.Raw.XML.OsmSource;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Routing.Data;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Routing.Core.Graph.Router.Dykstra;
using OsmSharp.Routing.Core.Graph.DynamicGraph.PreProcessed;
using OsmSharp.Osm.Data.XML.Processor;

namespace OsmSharp.Osm.Routing.Test.ManyToMany
{
    /// <summary>
    /// Executes many-to-many calculation performance tests using the raw data format.
    /// </summary>
    public class ManyToManyRawTests : ManyToManyTests
    {
        /// <summary>
        /// Creates a raw router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        protected override IRouter<RouterPoint> CreateRouter(
           Stream data, IRoutingInterpreter interpreter)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<PreProcessedEdge> osm_data =
                new MemoryRouterDataSource<PreProcessedEdge>(tags_index);
            PreProcessedDataGraphProcessingTarget target_data = new PreProcessedDataGraphProcessingTarget(
                osm_data, interpreter, osm_data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(data);
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            return new Router<PreProcessedEdge>(osm_data, interpreter, 
                new DykstraRoutingPreProcessed(osm_data.TagsIndex));
        }
    }
}

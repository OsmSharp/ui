using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Osm.Data.Raw.XML.OsmSource;
using Routing.Core;
using Routing.Core.Interpreter;
using Osm.Core;
using Routing.Core.Router.Memory;
using Osm.Routing.Data;
using Osm.Routing.Data.Processing;
using Osm.Data.XML.Raw.Processor;
using Osm.Data.Core.Processor.Filter.Sort;

namespace Osm.Routing.Test.ManyToMany
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
            MemoryRouterDataSource<OsmEdgeData> osm_data =
                new MemoryRouterDataSource<OsmEdgeData>(tags_index);
            OsmEdgeDataGraphProcessingTarget target_data = new OsmEdgeDataGraphProcessingTarget(
                osm_data, interpreter, osm_data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(data);
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            return new Router<OsmEdgeData>(osm_data, interpreter);
        }
    }
}

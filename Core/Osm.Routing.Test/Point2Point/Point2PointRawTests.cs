using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Routing.Data;
using OsmSharp.Routing.Core.Router;
using System.IO;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Osm.Core;
using OsmSharp.Routing.Core.Router.Memory;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Core;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Data.PBF.Raw.Processor;

namespace OsmSharp.Osm.Routing.Test.Point2Point
{
    class Point2PointRawTests : Point2PointTest<OsmEdgeData>
    {
        public override IRouterDataSource<OsmEdgeData> BuildData(Stream data_stream, bool pbf,
            IRoutingInterpreter interpreter, GeoCoordinateBox box)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<OsmEdgeData> osm_data =
                new MemoryRouterDataSource<OsmEdgeData>(tags_index);
            OsmEdgeDataGraphProcessingTarget target_data = new OsmEdgeDataGraphProcessingTarget(
                osm_data, interpreter, osm_data.TagsIndex, box);
            DataProcessorSource data_processor_source;
            if(pbf)
            {
                data_processor_source = new PBFDataProcessorSource(data_stream);
            }
            else
            {
                data_processor_source = new XmlDataProcessorSource(data_stream);
            }

            //DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            //sorter.RegisterSource(data_processor_source);
            //data_processor_source =
            //    new OsmSharp.Osm.Data.Core.Processor.Progress.ProgressDataProcessorSource(
            //        data_processor_source);
            target_data.RegisterSource(data_processor_source);
            target_data.Pull();

            return osm_data;
        }

        public override IRouter<RouterPoint> BuildRouter(IRouterDataSource<OsmEdgeData> data, 
            IRoutingInterpreter interpreter)
        {
            return new Router<OsmEdgeData>(data, interpreter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Routing.Data;
using OsmSharp.Routing.Core.Router;
using System.IO;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Core;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Routing.Core.Graph.Router;
using OsmSharp.Routing.Core.Graph.Router.Dykstra;

namespace OsmSharp.Osm.Routing.Test.Point2Point
{
    class Point2PointDykstraBinairyHeapTests : Point2PointTest<OsmEdgeData>
    {
        public override IBasicRouterDataSource<OsmEdgeData> BuildData(Stream data_stream, bool pbf,
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

        public override IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<OsmEdgeData> data, 
            IRoutingInterpreter interpreter, IBasicRouter<OsmEdgeData> router_basic)
        {
            return new Router<OsmEdgeData>(data, interpreter, router_basic);
        }

        public override IBasicRouter<OsmEdgeData> BuildBasicRouter(IBasicRouterDataSource<OsmEdgeData> data)
        {
            return new DykstraRoutingBinairyHeap<OsmEdgeData>(data.TagsIndex);
        }

        public override IRouter<RouterPoint> BuildReferenceRouter(Stream data_stream, bool pbf, IRoutingInterpreter interpreter)
        { // no use using a reference router.
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<OsmEdgeData> osm_data =
                new MemoryRouterDataSource<OsmEdgeData>(tags_index);
            OsmEdgeDataGraphProcessingTarget target_data = new OsmEdgeDataGraphProcessingTarget(
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

            return new Router<OsmEdgeData>(osm_data, interpreter, new DykstraRouting<OsmEdgeData>(osm_data.TagsIndex));
        }
    }
}

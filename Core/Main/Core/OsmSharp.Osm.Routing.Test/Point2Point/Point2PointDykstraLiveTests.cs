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
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Core;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Routing.Core.Graph.Router;
using OsmSharp.Routing.Core.Graph.Router.Dykstra;
using OsmSharp.Routing.Core.Graph.DynamicGraph.SimpleWeighed;
using OsmSharp.Routing.Core.Graph.DynamicGraph.PreProcessed;
using OsmSharp.Osm.Data.XML.Processor;

namespace OsmSharp.Osm.Routing.Test.Point2Point
{
    class Point2PointDykstraLiveTests : Point2PointTest<SimpleWeighedEdge>
    {
        public override IBasicRouterDataSource<SimpleWeighedEdge> BuildData(Stream data_stream, bool pbf,
            IRoutingInterpreter interpreter, GeoCoordinateBox box)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            //SimpleWeighedDynamicGraph graph = new SimpleWeighedDynamicGraph();
            MemoryRouterDataSource<SimpleWeighedEdge> osm_data =
                new MemoryRouterDataSource<SimpleWeighedEdge>(tags_index);
            SimpleWeighedDataGraphProcessingTarget target_data = new SimpleWeighedDataGraphProcessingTarget(
                osm_data, interpreter, osm_data.TagsIndex, VehicleEnum.Car, box);
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

        public override IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<SimpleWeighedEdge> data,
            IRoutingInterpreter interpreter, IBasicRouter<SimpleWeighedEdge> router_basic)
        {
            return new Router<SimpleWeighedEdge>(data, interpreter, router_basic);
        }

        public override IBasicRouter<SimpleWeighedEdge> BuildBasicRouter(IBasicRouterDataSource<SimpleWeighedEdge> data)
        {
            return new DykstraRoutingLive(data.TagsIndex);
        }

        public override IRouter<RouterPoint> BuildReferenceRouter(Stream data_stream, bool pbf, IRoutingInterpreter interpreter)
        { // no use using a reference router.
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<PreProcessedEdge> osm_data =
                new MemoryRouterDataSource<PreProcessedEdge>(tags_index);
            PreProcessedDataGraphProcessingTarget target_data = new PreProcessedDataGraphProcessingTarget(
                osm_data, interpreter, osm_data.TagsIndex, VehicleEnum.Car);
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

            return new Router<PreProcessedEdge>(osm_data, interpreter, new DykstraRoutingPreProcessed(osm_data.TagsIndex));
        }
    }
}

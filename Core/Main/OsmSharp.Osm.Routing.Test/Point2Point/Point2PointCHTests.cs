using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Routing.Data;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Core.Router;
using OsmSharp.Routing.Core.Interpreter;
using System.IO;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.Core.Graph.Router;
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Core.Graph.Router.Dykstra;

namespace OsmSharp.Osm.Routing.Test.Point2Point
{
    /// <summary>
    /// Facilitaters CH tests.
    /// </summary>
    class Point2PointCHTests : Point2PointTest<CHEdgeData>
    {
        /// <summary>
        /// Build router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public override IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<CHEdgeData> data,
            IRoutingInterpreter interpreter, IBasicRouter<CHEdgeData> basic_router)
        {
            return new Router<CHEdgeData>(data, interpreter, basic_router);
        }

        /// <summary>
        /// Build data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pbf"></param>
        /// <param name="interpreter"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public override IBasicRouterDataSource<CHEdgeData> BuildData(Stream data_stream, bool pbf, 
            IRoutingInterpreter interpreter, GeoCoordinateBox box)
        {
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
            //CHPreProcessor pre_processor = new CHPreProcessor(osm_data,
            //    new SparseOrdering(osm_data), witness_calculator);
            CHPreProcessor pre_processor = new CHPreProcessor(osm_data,
                new EdgeDifferenceContractedSearchSpace(osm_data, witness_calculator), witness_calculator);
            pre_processor.Start();

            return osm_data;
        }

        /// <summary>
        /// Builds a basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IBasicRouter<CHEdgeData> BuildBasicRouter(IBasicRouterDataSource<CHEdgeData> data)
        {
            //return new DykstraRouting<CHEdgeData>(data.TagsIndex);
            return new CHRouter(data);
        }

        /// <summary>
        /// Builds a reference router.
        /// </summary>
        /// <param name="data_stream"></param>
        /// <param name="pbf"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public override IRouter<RouterPoint> BuildReferenceRouter(Stream data_stream, bool pbf, IRoutingInterpreter interpreter)
        {
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

            return new Router<OsmEdgeData>(osm_data, interpreter, new DykstraRoutingBinairyHeap<OsmEdgeData>(osm_data.TagsIndex));
        }
    }
}

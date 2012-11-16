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

namespace OsmSharp.Osm.Routing.Test.Point2Point
{
    /// <summary>
    /// Facilitaters CH tests.
    /// </summary>
    class Point2PointCHTests : Point2PointTest<CHEdgeData>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public override IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<CHEdgeData> data,
            IRoutingInterpreter interpreter)
        {
            return new Router<CHEdgeData>(data, interpreter);
        }

        /// <summary>
        /// 
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
            CHPreProcessor pre_processor = new CHPreProcessor(osm_data,
                new SparseOrdering(osm_data), new DykstraWitnessCalculator(osm_data));
            pre_processor.Start();

            return osm_data;
        }
    }
}

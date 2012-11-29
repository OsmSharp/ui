//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using OsmSharp.Osm.UnitTests.Routing;
//using OsmSharp.Routing.Core;
//using OsmSharp.Osm.Routing.Data;
//using OsmSharp.Routing.Core.Router;
//using OsmSharp.Routing.Core.Interpreter;
//using OsmSharp.Routing.Core.Graph.Router;
//using OsmSharp.Routing.Core.Graph.Router.Dykstra;
//using OsmSharp.Osm.Core;
//using OsmSharp.Routing.Core.Graph.Memory;
//using OsmSharp.Osm.Routing.Data.Processing;
//using OsmSharp.Osm.Data.XML.Raw.Processor;
//using System.Reflection;
//using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;

//namespace OsmSharp.UnitTests.Routing.Raw
//{
//    /// <summary>
//    /// Does some raw routing tests.
//    /// </summary>
//    [TestClass]
//    public class RawRoutingAccessTests : RoutingAccessTests<RouterPoint, OsmEdgeData>
//    {
//        /// <summary>
//        /// Builds a router.
//        /// </summary>
//        /// <returns></returns>
//        public override IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<OsmEdgeData> data, IRoutingInterpreter interpreter,
//            IBasicRouter<OsmEdgeData> basic_router)
//        {
//            // initialize the router.
//            return new Router<OsmEdgeData>(
//                    data, interpreter, basic_router);
//        }

//        /// <summary>
//        /// Builds a basic router.
//        /// </summary>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        public override IBasicRouter<OsmEdgeData> BuildBasicRouter(IBasicRouterDataSource<OsmEdgeData> data)
//        {
//            return new DykstraRoutingBinairyHeap<OsmEdgeData>(data.TagsIndex);
//        }

//        /// <summary>
//        /// Builds data source.
//        /// </summary>
//        /// <param name="interpreter"></param>
//        /// <returns></returns>
//        public override IBasicRouterDataSource<OsmEdgeData> BuildData(IRoutingInterpreter interpreter)
//        {
//            OsmTagsIndex tags_index = new OsmTagsIndex();

//            // do the data processing.
//            MemoryRouterDataSource<OsmEdgeData> data =
//                new MemoryRouterDataSource<OsmEdgeData>(tags_index);
//            OsmEdgeDataGraphProcessingTarget target_data = new OsmEdgeDataGraphProcessingTarget(
//                data, interpreter, data.TagsIndex);
//            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
//                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_segments.osm"));
//            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
//            sorter.RegisterSource(data_processor_source);
//            target_data.RegisterSource(sorter);
//            target_data.Pull();
//            return data;
//        }

//        /// <summary>
//        /// Test access restrictions.
//        /// </summary>
//        [TestMethod]
//        public void TestRawAccessHighways()
//        {
//            this.DoAccessTestsHighways();
//        }
//    }
//}

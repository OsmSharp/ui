using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Osm.UnitTests.Routing;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Data;
using OsmSharp.Routing.Router;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Osm;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Osm.Data.XML.Processor;
using System.Reflection;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Graph.DynamicGraph.SimpleWeighed;

namespace OsmSharp.UnitTests.Routing.Dykstra
{
    /// <summary>
    /// Does some raw routing tests.
    /// </summary>
    [TestFixture]
    public class DykstraLiveAccessTests : RoutingAccessTests<RouterPoint, SimpleWeighedEdge>
    {
        /// <summary>
        /// Builds a router.
        /// </summary>
        /// <returns></returns>
        public override IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<SimpleWeighedEdge> data, IRoutingInterpreter interpreter,
            IBasicRouter<SimpleWeighedEdge> basic_router)
        {
            // initialize the router.
            return new Router<SimpleWeighedEdge>(
                    data, interpreter, basic_router);
        }

        /// <summary>
        /// Builds a basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IBasicRouter<SimpleWeighedEdge> BuildBasicRouter(IBasicRouterDataSource<SimpleWeighedEdge> data)
        {
            return new DykstraRoutingLive(data.TagsIndex);
        }

        /// <summary>
        /// Builds data source.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="embedded_string"></param>
        /// <returns></returns>
        public override IBasicRouterDataSource<SimpleWeighedEdge> BuildData(IRoutingInterpreter interpreter,
                                                                            string embedded_string, VehicleEnum vehicle)
        {
            string key = string.Format("Dykstra.Routing.IBasicRouterDataSource<SimpleWeighedEdge>.OSM.{0}",
                                       embedded_string);
            IBasicRouterDataSource<SimpleWeighedEdge> data = StaticDictionary
                .Get<IBasicRouterDataSource<SimpleWeighedEdge>>(
                    key);

            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            DynamicGraphRouterDataSource<SimpleWeighedEdge> memory_data =
                new DynamicGraphRouterDataSource<SimpleWeighedEdge>(tags_index);
            SimpleWeighedDataGraphProcessingTarget target_data = new SimpleWeighedDataGraphProcessingTarget(
                memory_data, interpreter, memory_data.TagsIndex, vehicle);
            
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embedded_string));
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            data = memory_data;
            StaticDictionary.Add<IBasicRouterDataSource<SimpleWeighedEdge>>(key,
                                                                            data);
            return data;
        }

        /// <summary>
        /// Test access restrictions.
        /// </summary>
        [Test]
        public void TestDykstraLiveAccessHighways()
        {
            this.DoAccessTestsHighways();
        }
    }
}

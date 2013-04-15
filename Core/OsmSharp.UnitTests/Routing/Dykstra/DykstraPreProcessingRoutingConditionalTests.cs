using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using OsmSharp.Osm;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Osm.Data.Processor.Filter;
using OsmSharp.Osm.Data.XML.Processor;
using OsmSharp.Osm.UnitTests.Routing;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.DynamicGraph.PreProcessed;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Data.Processing;

namespace OsmSharp.UnitTests.Routing.Dykstra
{
    /// <summary>
    /// Does some raw routing tests testing for oneway constraint.
    /// </summary>
    [TestFixture]
    public class DykstraPreProcessingRoutingOneWayAccessTests : RoutingConditionalAccessTests<RouterPoint, PreProcessedEdge>
    {
        /// <summary>
        /// Builds a router.
        /// </summary>
        /// <returns></returns>
        public override IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<PreProcessedEdge> data, IRoutingInterpreter interpreter,
            IBasicRouter<PreProcessedEdge> basic_router)
        {
            // initialize the router.
            return new Router<PreProcessedEdge>(
                    data, interpreter, basic_router);
        }

        /// <summary>
        /// Builds a basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public override IBasicRouter<PreProcessedEdge> BuildBasicRouter(IBasicRouterDataSource<PreProcessedEdge> data, VehicleEnum vehicle)
        {
            return new DykstraRoutingPreProcessed(data.TagsIndex);
        }

        /// <summary>
        /// Builds data source.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public override IBasicRouterDataSource<PreProcessedEdge> BuildData(IRoutingInterpreter interpreter, VehicleEnum vehicle, 
            List<KeyValuePair<string, string>> access_tags)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            DynamicGraphRouterDataSource<PreProcessedEdge> data =
                new DynamicGraphRouterDataSource<PreProcessedEdge>(tags_index);

            PreProcessedDataGraphProcessingTarget target_data = new PreProcessedDataGraphProcessingTarget(
                data, interpreter, data.TagsIndex, vehicle);

            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_network_conditional.osm"));

            //DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            //sorter.RegisterSource(data_processor_source);

            DataProcessorFilterWithEvents events_filter = new DataProcessorFilterWithEvents(access_tags);
            events_filter.MovedToNextEvent += new DataProcessorFilterWithEvents.SimpleOsmGeoDelegate(events_filter_MovedToNextEvent);
            events_filter.RegisterSource(data_processor_source);

            target_data.RegisterSource(events_filter);
            target_data.Pull();

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simple_osm_geo"></param>
        void events_filter_MovedToNextEvent(Osm.Simple.SimpleOsmGeo simple_osm_geo, object param)
        {
            List<KeyValuePair<string, string>> access_tags = param as List<KeyValuePair<string, string>>;
            if (param != null &&
                simple_osm_geo.Id == -66)
            {
                simple_osm_geo.Tags.Clear();

                foreach (var keyValuePair in access_tags)
                {
                    simple_osm_geo.Tags.Add(keyValuePair);
                }
            }
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingOneWayShortestWithDirection()
        {
            var accessTags = new List<KeyValuePair<string, string>>
                                  {
                                      new KeyValuePair<string, string>("highway",
                                                                       "pedestrian"),
                                      new KeyValuePair<string, string>(
                                          "motor_vehicle:conditional",
                                          "delivery @ (22:00-06:00)")
                                  };

            this.DoTestShortestWithAccess(VehicleEnum.Pedestrian, accessTags);
            this.DoTestShortestWithoutAccess(VehicleEnum.Bicycle, accessTags); // only bicycles would have no access.
            this.DoTestShortestWithAccess(VehicleEnum.Moped, accessTags);
            this.DoTestShortestWithAccess(VehicleEnum.MotorCycle, accessTags);
            this.DoTestShortestWithAccess(VehicleEnum.Car, accessTags);
            this.DoTestShortestWithAccess(VehicleEnum.SmallTruck, accessTags);
            this.DoTestShortestWithAccess(VehicleEnum.BigTruck, accessTags);
            this.DoTestShortestWithAccess(VehicleEnum.Bus, accessTags);
        }
    }
}
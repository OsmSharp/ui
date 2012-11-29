using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Core.Graph.DynamicGraph;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.Core.Router;
using OsmSharp.Routing.Core.Graph.Router;
using OsmSharp.Osm.Routing.Interpreter;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Core.Route;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OsmSharp.UnitTests.Routing
{
    /// <summary>
    /// Generic tests to test access restrictions using different vehicles.
    /// </summary>
    public abstract class RoutingAccessTests<ResolvedType, EdgeData>
        where EdgeData : IDynamicGraphEdgeData
        where ResolvedType : IRouterPoint
    {
        /// <summary>
        /// Builds the router;
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="basic_router"></param>
        /// <returns></returns>
        public abstract IRouter<ResolvedType> BuildRouter(IBasicRouterDataSource<EdgeData> data,
            IRoutingInterpreter interpreter, IBasicRouter<EdgeData> basic_router);

        /// <summary>
        /// Builds the basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IBasicRouter<EdgeData> BuildBasicRouter(IBasicRouterDataSource<EdgeData> data);

        /// <summary>
        /// Builds the data.
        /// </summary>
        /// <returns></returns>
        public abstract IBasicRouterDataSource<EdgeData> BuildData(IRoutingInterpreter interpreter);

        /// <summary>
        /// Tests access restrictions on all different highway times.
        /// </summary>
        protected void DoAccessTestsHighways()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter);
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);

            double longitude_left = 4.7696568;
            double longitude_right = 4.8283861;

            GeoCoordinate footway_from = new GeoCoordinate(51.279, longitude_left);
            GeoCoordinate footway_to = new GeoCoordinate(51.279, longitude_right);

            GeoCoordinate cycleway_from = new GeoCoordinate(51.278, longitude_left);
            GeoCoordinate cycleway_to = new GeoCoordinate(51.278, longitude_right);

            GeoCoordinate bridleway_from = new GeoCoordinate(51.277, longitude_left);
            GeoCoordinate bridleway_to = new GeoCoordinate(51.277, longitude_right);

            GeoCoordinate path_from = new GeoCoordinate(51.276, longitude_left);
            GeoCoordinate path_to = new GeoCoordinate(51.276, longitude_right);

            GeoCoordinate pedestrian_from = new GeoCoordinate(51.275, longitude_left);
            GeoCoordinate pedestrian_to = new GeoCoordinate(51.275, longitude_right);

            GeoCoordinate road_from = new GeoCoordinate(51.274, longitude_left);
            GeoCoordinate road_to = new GeoCoordinate(51.274, longitude_right);

            GeoCoordinate living_street_from = new GeoCoordinate(51.273, longitude_left);
            GeoCoordinate living_street_to = new GeoCoordinate(51.273, longitude_right);

            GeoCoordinate residential_from = new GeoCoordinate(51.271, longitude_left);
            GeoCoordinate residential_to = new GeoCoordinate(51.271, longitude_right);

            GeoCoordinate unclassified_from = new GeoCoordinate(51.270, longitude_left);
            GeoCoordinate unclassified_to = new GeoCoordinate(51.270, longitude_right);

            GeoCoordinate tertiary_from = new GeoCoordinate(51.269, longitude_left);
            GeoCoordinate tertiary_to = new GeoCoordinate(51.269, longitude_right);

            GeoCoordinate secondary_from = new GeoCoordinate(51.268, longitude_left);
            GeoCoordinate secondary_to = new GeoCoordinate(51.268, longitude_right);

            GeoCoordinate primary_from = new GeoCoordinate(51.267, longitude_left);
            GeoCoordinate primary_to = new GeoCoordinate(51.267, longitude_right);

            GeoCoordinate trunk_from = new GeoCoordinate(51.266, longitude_left);
            GeoCoordinate trunk_to = new GeoCoordinate(51.266, longitude_right);

            GeoCoordinate motorway_from = new GeoCoordinate(51.265, longitude_left);
            GeoCoordinate motorway_to = new GeoCoordinate(51.265, longitude_right);

            // pedestrian
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                footway_from, footway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                cycleway_from, cycleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                bridleway_from, bridleway_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                path_from, path_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                pedestrian_from, pedestrian_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                road_from, road_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                living_street_from, living_street_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                residential_from, residential_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                unclassified_from, unclassified_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                tertiary_from, tertiary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                secondary_from, secondary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                primary_from, primary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                trunk_from, trunk_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                motorway_from, motorway_to, interpreter, basic_router, data));
        }

        /// <summary>
        /// Tests access for a given vehicle type and for a given network between two given points.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="interpreter"></param>
        /// <param name="basic_router"></param>
        /// <param name="data"></param>
        protected bool DoTestForVehicle(VehicleEnum vehicle, GeoCoordinate from, GeoCoordinate to,
            IRoutingInterpreter interpreter, IBasicRouter<EdgeData> basic_router, IBasicRouterDataSource<EdgeData> data)
        {
            IRouter<ResolvedType> router = this.BuildRouter(data, interpreter, basic_router);

            ResolvedType resolved_from = router.Resolve(vehicle, from);
            ResolvedType resolved_to = router.Resolve(vehicle, to);

            OsmSharpRoute route = router.Calculate(vehicle, resolved_from, resolved_to);
            return route != null;
        }
    }
}

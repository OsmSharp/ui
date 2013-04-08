using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Graph.DynamicGraph;
using OsmSharp.Routing;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Router;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Route;
using NUnit.Framework;

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
        public abstract IBasicRouterDataSource<EdgeData> BuildData(IRoutingInterpreter interpreter,
            string embedded_string);

        /// <summary>
        /// Tests access restrictions on all different highway times.
        /// </summary>
        protected void DoAccessTestsHighways()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(interpreter, "OsmSharp.UnitTests.test_segments.osm");
            IBasicRouter<EdgeData> basic_router = this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = this.BuildRouter(
                data, interpreter, basic_router);

            double longitude_left = 4.7696568;
            double longitude_right = 4.8283861;

            GeoCoordinate footway_from = new GeoCoordinate(51.2, longitude_left);
            GeoCoordinate footway_to = new GeoCoordinate(51.2, longitude_right);

            GeoCoordinate cycleway_from = new GeoCoordinate(51.1, longitude_left);
            GeoCoordinate cycleway_to = new GeoCoordinate(51.1, longitude_right);

            GeoCoordinate bridleway_from = new GeoCoordinate(51.0, longitude_left);
            GeoCoordinate bridleway_to = new GeoCoordinate(51.0, longitude_right);

            GeoCoordinate path_from = new GeoCoordinate(50.9, longitude_left);
            GeoCoordinate path_to = new GeoCoordinate(50.9, longitude_right);

            GeoCoordinate pedestrian_from = new GeoCoordinate(50.8, longitude_left);
            GeoCoordinate pedestrian_to = new GeoCoordinate(50.8, longitude_right);

            GeoCoordinate road_from = new GeoCoordinate(50.7, longitude_left);
            GeoCoordinate road_to = new GeoCoordinate(50.7, longitude_right);

            GeoCoordinate living_street_from = new GeoCoordinate(50.6, longitude_left);
            GeoCoordinate living_street_to = new GeoCoordinate(50.6, longitude_right);

            GeoCoordinate residential_from = new GeoCoordinate(50.5, longitude_left);
            GeoCoordinate residential_to = new GeoCoordinate(50.5, longitude_right);

            GeoCoordinate unclassified_from = new GeoCoordinate(50.4, longitude_left);
            GeoCoordinate unclassified_to = new GeoCoordinate(50.4, longitude_right);

            GeoCoordinate tertiary_from = new GeoCoordinate(50.3, longitude_left);
            GeoCoordinate tertiary_to = new GeoCoordinate(50.3, longitude_right);

            GeoCoordinate secondary_from = new GeoCoordinate(50.2, longitude_left);
            GeoCoordinate secondary_to = new GeoCoordinate(50.2, longitude_right);

            GeoCoordinate primary_from = new GeoCoordinate(50.1, longitude_left);
            GeoCoordinate primary_to = new GeoCoordinate(50.1, longitude_right);

            GeoCoordinate trunk_from = new GeoCoordinate(50.0, longitude_left);
            GeoCoordinate trunk_to = new GeoCoordinate(50.0, longitude_right);

            GeoCoordinate motorway_from = new GeoCoordinate(49.9, longitude_left);
            GeoCoordinate motorway_to = new GeoCoordinate(49.9, longitude_right);

            // pedestrian
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Pedestrian,
                footway_from, footway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Pedestrian,
                cycleway_from, cycleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Pedestrian,
                bridleway_from, bridleway_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Pedestrian,
                path_from, path_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Pedestrian,
                pedestrian_from, pedestrian_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Pedestrian,
                road_from, road_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Pedestrian,
                living_street_from, living_street_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Pedestrian,
                residential_from, residential_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Pedestrian,
                unclassified_from, unclassified_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Pedestrian,
                tertiary_from, tertiary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Pedestrian,
                secondary_from, secondary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Pedestrian,
                primary_from, primary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Pedestrian,
                trunk_from, trunk_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Pedestrian,
                motorway_from, motorway_to, interpreter, basic_router, data));

            // bicycle
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Bicycle,
                footway_from, footway_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bicycle,
                cycleway_from, cycleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Bicycle,
                bridleway_from, bridleway_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bicycle,
                path_from, path_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Bicycle,
                pedestrian_from, pedestrian_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bicycle,
                road_from, road_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bicycle,
                living_street_from, living_street_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bicycle,
                residential_from, residential_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bicycle,
                unclassified_from, unclassified_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bicycle,
                tertiary_from, tertiary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bicycle,
                secondary_from, secondary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bicycle,
                primary_from, primary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bicycle,
                trunk_from, trunk_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Bicycle,
                motorway_from, motorway_to, interpreter, basic_router, data));

            // moped
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Moped,
                footway_from, footway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Moped,
                cycleway_from, cycleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Moped,
                bridleway_from, bridleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Moped,
                path_from, path_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Moped,
                pedestrian_from, pedestrian_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Moped,
                road_from, road_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Moped,
                living_street_from, living_street_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Moped,
                residential_from, residential_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Moped,
                unclassified_from, unclassified_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Moped,
                tertiary_from, tertiary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Moped,
                secondary_from, secondary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Moped,
                primary_from, primary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Moped,
                trunk_from, trunk_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Moped,
                motorway_from, motorway_to, interpreter, basic_router, data));

            // moped
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.MotorCycle,
                footway_from, footway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.MotorCycle,
                cycleway_from, cycleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.MotorCycle,
                bridleway_from, bridleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.MotorCycle,
                path_from, path_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.MotorCycle,
                pedestrian_from, pedestrian_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.MotorCycle,
                road_from, road_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.MotorCycle,
                living_street_from, living_street_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.MotorCycle,
                residential_from, residential_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.MotorCycle,
                unclassified_from, unclassified_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.MotorCycle,
                tertiary_from, tertiary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.MotorCycle,
                secondary_from, secondary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.MotorCycle,
                primary_from, primary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.MotorCycle,
                trunk_from, trunk_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.MotorCycle,
                motorway_from, motorway_to, interpreter, basic_router, data));

            // car
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Car,
                footway_from, footway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Car,
                cycleway_from, cycleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Car,
                bridleway_from, bridleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Car,
                path_from, path_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Car,
                pedestrian_from, pedestrian_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Car,
                road_from, road_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Car,
                living_street_from, living_street_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Car,
                residential_from, residential_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Car,
                unclassified_from, unclassified_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Car,
                tertiary_from, tertiary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Car,
                secondary_from, secondary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Car,
                primary_from, primary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Car,
                trunk_from, trunk_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Car,
                motorway_from, motorway_to, interpreter, basic_router, data));

            // small truck
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.SmallTruck,
                footway_from, footway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.SmallTruck,
                cycleway_from, cycleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.SmallTruck,
                bridleway_from, bridleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.SmallTruck,
                path_from, path_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.SmallTruck,
                pedestrian_from, pedestrian_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.SmallTruck,
                road_from, road_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.SmallTruck,
                living_street_from, living_street_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.SmallTruck,
                residential_from, residential_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.SmallTruck,
                unclassified_from, unclassified_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.SmallTruck,
                tertiary_from, tertiary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.SmallTruck,
                secondary_from, secondary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.SmallTruck,
                primary_from, primary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.SmallTruck,
                trunk_from, trunk_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.SmallTruck,
                motorway_from, motorway_to, interpreter, basic_router, data));

            // big truck
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.BigTruck,
                footway_from, footway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.BigTruck,
                cycleway_from, cycleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.BigTruck,
                bridleway_from, bridleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.BigTruck,
                path_from, path_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.BigTruck,
                pedestrian_from, pedestrian_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.BigTruck,
                road_from, road_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.BigTruck,
                living_street_from, living_street_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.BigTruck,
                residential_from, residential_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.BigTruck,
                unclassified_from, unclassified_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.BigTruck,
                tertiary_from, tertiary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.BigTruck,
                secondary_from, secondary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.BigTruck,
                primary_from, primary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.BigTruck,
                trunk_from, trunk_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.BigTruck,
                motorway_from, motorway_to, interpreter, basic_router, data));

            // bus
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Bus,
                footway_from, footway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Bus,
                cycleway_from, cycleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Bus,
                bridleway_from, bridleway_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Bus,
                path_from, path_to, interpreter, basic_router, data));
            Assert.IsFalse(this.DoTestForVehicle(Vehicle.Bus,
                pedestrian_from, pedestrian_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bus,
                road_from, road_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bus,
                living_street_from, living_street_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bus,
                residential_from, residential_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bus,
                unclassified_from, unclassified_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bus,
                tertiary_from, tertiary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bus,
                secondary_from, secondary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bus,
                primary_from, primary_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bus,
                trunk_from, trunk_to, interpreter, basic_router, data));
            Assert.IsTrue(this.DoTestForVehicle(Vehicle.Bus,
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
        protected bool DoTestForVehicle(Vehicle vehicle, GeoCoordinate from, GeoCoordinate to,
            IRoutingInterpreter interpreter, IBasicRouter<EdgeData> basic_router, IBasicRouterDataSource<EdgeData> data)
        {
            IRouter<ResolvedType> router = this.BuildRouter(data, interpreter, basic_router);

            ResolvedType resolved_from = router.Resolve(vehicle, from);
            ResolvedType resolved_to = router.Resolve(vehicle, to);

            if (resolved_from != null && resolved_to != null)
            {
                OsmSharpRoute route = router.Calculate(vehicle, resolved_from, resolved_to);
                return route != null;
            }
            return false;
        }
    }
}

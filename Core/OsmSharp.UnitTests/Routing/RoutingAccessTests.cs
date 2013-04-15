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
            string embedded_string, VehicleEnum vehicle);

        /// <summary>
        /// Tests access restrictions on all different highway times.
        /// </summary>
        protected void DoAccessTestsHighways()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();

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
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                footway_from, footway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                cycleway_from, cycleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                bridleway_from, bridleway_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                path_from, path_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                pedestrian_from, pedestrian_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                road_from, road_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                living_street_from, living_street_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                residential_from, residential_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                unclassified_from, unclassified_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                tertiary_from, tertiary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                secondary_from, secondary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                primary_from, primary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                trunk_from, trunk_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                motorway_from, motorway_to, interpreter));

            // bicycle
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bicycle,
                footway_from, footway_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                cycleway_from, cycleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bicycle,
                bridleway_from, bridleway_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                path_from, path_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bicycle,
                pedestrian_from, pedestrian_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                road_from, road_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                living_street_from, living_street_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                residential_from, residential_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                unclassified_from, unclassified_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                tertiary_from, tertiary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                secondary_from, secondary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                primary_from, primary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                trunk_from, trunk_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bicycle,
                motorway_from, motorway_to, interpreter));

            // moped
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                footway_from, footway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                cycleway_from, cycleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                bridleway_from, bridleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                path_from, path_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                pedestrian_from, pedestrian_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                road_from, road_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                living_street_from, living_street_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                residential_from, residential_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                unclassified_from, unclassified_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                tertiary_from, tertiary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                secondary_from, secondary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                primary_from, primary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                trunk_from, trunk_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                motorway_from, motorway_to, interpreter));

            // moped
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                footway_from, footway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                cycleway_from, cycleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                bridleway_from, bridleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                path_from, path_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                pedestrian_from, pedestrian_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                road_from, road_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                living_street_from, living_street_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                residential_from, residential_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                unclassified_from, unclassified_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                tertiary_from, tertiary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                secondary_from, secondary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                primary_from, primary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                trunk_from, trunk_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                motorway_from, motorway_to, interpreter));

            // car
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Car,
                footway_from, footway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Car,
                cycleway_from, cycleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Car,
                bridleway_from, bridleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Car,
                path_from, path_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Car,
                pedestrian_from, pedestrian_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                road_from, road_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                living_street_from, living_street_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                residential_from, residential_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                unclassified_from, unclassified_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                tertiary_from, tertiary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                secondary_from, secondary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                primary_from, primary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                trunk_from, trunk_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                motorway_from, motorway_to, interpreter));

            // small truck
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                footway_from, footway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                cycleway_from, cycleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                bridleway_from, bridleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                path_from, path_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                pedestrian_from, pedestrian_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                road_from, road_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                living_street_from, living_street_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                residential_from, residential_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                unclassified_from, unclassified_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                tertiary_from, tertiary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                secondary_from, secondary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                primary_from, primary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                trunk_from, trunk_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                motorway_from, motorway_to, interpreter));

            // big truck
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.BigTruck,
                footway_from, footway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.BigTruck,
                cycleway_from, cycleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.BigTruck,
                bridleway_from, bridleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.BigTruck,
                path_from, path_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.BigTruck,
                pedestrian_from, pedestrian_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                road_from, road_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                living_street_from, living_street_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                residential_from, residential_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                unclassified_from, unclassified_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                tertiary_from, tertiary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                secondary_from, secondary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                primary_from, primary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                trunk_from, trunk_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                motorway_from, motorway_to, interpreter));

            // bus
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bus,
                footway_from, footway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bus,
                cycleway_from, cycleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bus,
                bridleway_from, bridleway_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bus,
                path_from, path_to, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bus,
                pedestrian_from, pedestrian_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                road_from, road_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                living_street_from, living_street_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                residential_from, residential_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                unclassified_from, unclassified_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                tertiary_from, tertiary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                secondary_from, secondary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                primary_from, primary_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                trunk_from, trunk_to, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                motorway_from, motorway_to, interpreter));
        }

        /// <summary>
        /// Tests access for a given vehicle type and for a given network between two given points.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="interpreter"></param>
        protected bool DoTestForVehicle(VehicleEnum vehicle, GeoCoordinate from, GeoCoordinate to,
            IRoutingInterpreter interpreter)
        {
            IBasicRouterDataSource<EdgeData> data = 
                this.BuildData(interpreter, "OsmSharp.UnitTests.test_segments.osm", vehicle);
            IBasicRouter<EdgeData> basic_router = 
                this.BuildBasicRouter(data);
            IRouter<ResolvedType> router = 
                this.BuildRouter(data, interpreter, basic_router);

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

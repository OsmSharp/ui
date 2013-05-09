using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Routers;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Route;
using NUnit.Framework;

namespace OsmSharp.UnitTests.Routing
{
    /// <summary>
    /// Generic tests to test access restrictions using different vehicles.
    /// </summary>
    public abstract class RoutingAccessTests<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Builds the router;
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="basicRouter"></param>
        /// <returns></returns>
        public abstract Router BuildRouter(IBasicRouterDataSource<TEdgeData> data,
            IRoutingInterpreter interpreter, IBasicRouter<TEdgeData> basicRouter);

        /// <summary>
        /// Builds the basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IBasicRouter<TEdgeData> BuildBasicRouter(IBasicRouterDataSource<TEdgeData> data);

        /// <summary>
        /// Builds the data.
        /// </summary>
        /// <returns></returns>
        public abstract IBasicRouterDataSource<TEdgeData> BuildData(IRoutingInterpreter interpreter,
            string embeddedString, VehicleEnum vehicle);

        /// <summary>
        /// Tests access restrictions on all different highway times.
        /// </summary>
        protected void DoAccessTestsHighways()
        {
            var interpreter = new OsmRoutingInterpreter();

            const double longitudeLeft = 4.7696568;
            const double longitudeRight = 4.8283861;

            var footwayFrom = new GeoCoordinate(51.2, longitudeLeft);
            var footwayTo = new GeoCoordinate(51.2, longitudeRight);

            var cyclewayFrom = new GeoCoordinate(51.1, longitudeLeft);
            var cyclewayTo = new GeoCoordinate(51.1, longitudeRight);

            var bridlewayFrom = new GeoCoordinate(51.0, longitudeLeft);
            var bridlewayTo = new GeoCoordinate(51.0, longitudeRight);

            var pathFrom = new GeoCoordinate(50.9, longitudeLeft);
            var pathTo = new GeoCoordinate(50.9, longitudeRight);

            var pedestrianFrom = new GeoCoordinate(50.8, longitudeLeft);
            var pedestrianTo = new GeoCoordinate(50.8, longitudeRight);

            var roadFrom = new GeoCoordinate(50.7, longitudeLeft);
            var roadTo = new GeoCoordinate(50.7, longitudeRight);

            var livingStreetFrom = new GeoCoordinate(50.6, longitudeLeft);
            var livingStreetTo = new GeoCoordinate(50.6, longitudeRight);

            var residentialFrom = new GeoCoordinate(50.5, longitudeLeft);
            var residentialTo = new GeoCoordinate(50.5, longitudeRight);

            var unclassifiedFrom = new GeoCoordinate(50.4, longitudeLeft);
            var unclassifiedTo = new GeoCoordinate(50.4, longitudeRight);

            var tertiaryFrom = new GeoCoordinate(50.3, longitudeLeft);
            var tertiaryTo = new GeoCoordinate(50.3, longitudeRight);

            var secondaryFrom = new GeoCoordinate(50.2, longitudeLeft);
            var secondaryTo = new GeoCoordinate(50.2, longitudeRight);

            var primaryFrom = new GeoCoordinate(50.1, longitudeLeft);
            var primaryTo = new GeoCoordinate(50.1, longitudeRight);

            var trunkFrom = new GeoCoordinate(50.0, longitudeLeft);
            var trunkTo = new GeoCoordinate(50.0, longitudeRight);

            var motorwayFrom = new GeoCoordinate(49.9, longitudeLeft);
            var motorwayTo = new GeoCoordinate(49.9, longitudeRight);

            // pedestrian
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                footwayFrom, footwayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                cyclewayFrom, cyclewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                bridlewayFrom, bridlewayTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                pathFrom, pathTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                pedestrianFrom, pedestrianTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                roadFrom, roadTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                livingStreetFrom, livingStreetTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                residentialFrom, residentialTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                unclassifiedFrom, unclassifiedTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                tertiaryFrom, tertiaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                secondaryFrom, secondaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                primaryFrom, primaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                trunkFrom, trunkTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Pedestrian,
                motorwayFrom, motorwayTo, interpreter));

            // bicycle
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bicycle,
                footwayFrom, footwayTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                cyclewayFrom, cyclewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bicycle,
                bridlewayFrom, bridlewayTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                pathFrom, pathTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bicycle,
                pedestrianFrom, pedestrianTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                roadFrom, roadTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                livingStreetFrom, livingStreetTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                residentialFrom, residentialTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                unclassifiedFrom, unclassifiedTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                tertiaryFrom, tertiaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                secondaryFrom, secondaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                primaryFrom, primaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bicycle,
                trunkFrom, trunkTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bicycle,
                motorwayFrom, motorwayTo, interpreter));

            // moped
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                footwayFrom, footwayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                cyclewayFrom, cyclewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                bridlewayFrom, bridlewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                pathFrom, pathTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                pedestrianFrom, pedestrianTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                roadFrom, roadTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                livingStreetFrom, livingStreetTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                residentialFrom, residentialTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                unclassifiedFrom, unclassifiedTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                tertiaryFrom, tertiaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                secondaryFrom, secondaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                primaryFrom, primaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Moped,
                trunkFrom, trunkTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Moped,
                motorwayFrom, motorwayTo, interpreter));

            // moped
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                footwayFrom, footwayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                cyclewayFrom, cyclewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                bridlewayFrom, bridlewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                pathFrom, pathTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                pedestrianFrom, pedestrianTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                roadFrom, roadTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                livingStreetFrom, livingStreetTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                residentialFrom, residentialTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                unclassifiedFrom, unclassifiedTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                tertiaryFrom, tertiaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                secondaryFrom, secondaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                primaryFrom, primaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                trunkFrom, trunkTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.MotorCycle,
                motorwayFrom, motorwayTo, interpreter));

            // car
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Car,
                footwayFrom, footwayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Car,
                cyclewayFrom, cyclewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Car,
                bridlewayFrom, bridlewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Car,
                pathFrom, pathTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Car,
                pedestrianFrom, pedestrianTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                roadFrom, roadTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                livingStreetFrom, livingStreetTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                residentialFrom, residentialTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                unclassifiedFrom, unclassifiedTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                tertiaryFrom, tertiaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                secondaryFrom, secondaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                primaryFrom, primaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                trunkFrom, trunkTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Car,
                motorwayFrom, motorwayTo, interpreter));

            // small truck
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                footwayFrom, footwayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                cyclewayFrom, cyclewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                bridlewayFrom, bridlewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                pathFrom, pathTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                pedestrianFrom, pedestrianTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                roadFrom, roadTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                livingStreetFrom, livingStreetTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                residentialFrom, residentialTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                unclassifiedFrom, unclassifiedTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                tertiaryFrom, tertiaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                secondaryFrom, secondaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                primaryFrom, primaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                trunkFrom, trunkTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.SmallTruck,
                motorwayFrom, motorwayTo, interpreter));

            // big truck
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.BigTruck,
                footwayFrom, footwayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.BigTruck,
                cyclewayFrom, cyclewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.BigTruck,
                bridlewayFrom, bridlewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.BigTruck,
                pathFrom, pathTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.BigTruck,
                pedestrianFrom, pedestrianTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                roadFrom, roadTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                livingStreetFrom, livingStreetTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                residentialFrom, residentialTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                unclassifiedFrom, unclassifiedTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                tertiaryFrom, tertiaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                secondaryFrom, secondaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                primaryFrom, primaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                trunkFrom, trunkTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.BigTruck,
                motorwayFrom, motorwayTo, interpreter));

            // bus
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bus,
                footwayFrom, footwayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bus,
                cyclewayFrom, cyclewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bus,
                bridlewayFrom, bridlewayTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bus,
                pathFrom, pathTo, interpreter));
            Assert.IsFalse(this.DoTestForVehicle(VehicleEnum.Bus,
                pedestrianFrom, pedestrianTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                roadFrom, roadTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                livingStreetFrom, livingStreetTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                residentialFrom, residentialTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                unclassifiedFrom, unclassifiedTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                tertiaryFrom, tertiaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                secondaryFrom, secondaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                primaryFrom, primaryTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                trunkFrom, trunkTo, interpreter));
            Assert.IsTrue(this.DoTestForVehicle(VehicleEnum.Bus,
                motorwayFrom, motorwayTo, interpreter));
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
            IBasicRouterDataSource<TEdgeData> data = 
                this.BuildData(interpreter, "OsmSharp.UnitTests.test_segments.osm", vehicle);
            IBasicRouter<TEdgeData> basicRouter = 
                this.BuildBasicRouter(data);
            Router router = 
                this.BuildRouter(data, interpreter, basicRouter);

            RouterPoint resolvedFrom = router.Resolve(vehicle, from);
            RouterPoint resolvedTo = router.Resolve(vehicle, to);

            if (resolvedFrom != null && resolvedTo != null)
            {
                OsmSharpRoute route = router.Calculate(vehicle, resolvedFrom, resolvedTo);
                return route != null;
            }
            return false;
        }
    }
}
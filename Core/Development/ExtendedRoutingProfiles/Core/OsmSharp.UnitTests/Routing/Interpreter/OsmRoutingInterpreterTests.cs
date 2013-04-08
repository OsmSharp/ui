using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Osm.Interpreter;
using NUnit.Framework;
using OsmSharp.Routing;

namespace OsmSharp.UnitTests.Routing.Interpreter
{
    /// <summary>
    /// Tests the OsmRoutingInterpreter.
    /// </summary>
    [TestFixture]
    public class OsmRoutingInterpreterTests
    {
        /// <summary>
        /// Tests the CanBeTraversedBy function.
        /// </summary>
        [Test]
        public void TestOsmRoutingInterpreterCanBeTraversedBy()
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();

            Dictionary<string, string> tags = new Dictionary<string, string>();
            tags["highway"] = "footway";
            Assert.IsTrue(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Moped.CanTraverse(tags));
            Assert.IsFalse(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Car.CanTraverse(tags));
            Assert.IsFalse(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsFalse(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "cycleway";
            Assert.IsFalse(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Moped.CanTraverse(tags));
            Assert.IsFalse(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Car.CanTraverse(tags));
            Assert.IsFalse(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsFalse(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "bridleway";
            Assert.IsFalse(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Moped.CanTraverse(tags));
            Assert.IsFalse(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Car.CanTraverse(tags));
            Assert.IsFalse(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsFalse(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "path";
            Assert.IsTrue(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Moped.CanTraverse(tags));
            Assert.IsFalse(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Car.CanTraverse(tags));
            Assert.IsFalse(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsFalse(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "pedestrian";
            Assert.IsTrue(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Moped.CanTraverse(tags));
            Assert.IsFalse(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Car.CanTraverse(tags));
            Assert.IsFalse(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsFalse(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "road";
            Assert.IsTrue(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Moped.CanTraverse(tags));
            Assert.IsTrue(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Car.CanTraverse(tags));
            Assert.IsTrue(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "living_street";
            Assert.IsTrue(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Moped.CanTraverse(tags));
            Assert.IsTrue(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Car.CanTraverse(tags));
            Assert.IsTrue(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "residential";
            Assert.IsTrue(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Moped.CanTraverse(tags));
            Assert.IsTrue(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Car.CanTraverse(tags));
            Assert.IsTrue(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "unclassified";
            Assert.IsTrue(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Moped.CanTraverse(tags));
            Assert.IsTrue(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Car.CanTraverse(tags));
            Assert.IsTrue(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "tertiary";
            Assert.IsTrue(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Moped.CanTraverse(tags));
            Assert.IsTrue(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Car.CanTraverse(tags));
            Assert.IsTrue(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "secondary";
            Assert.IsTrue(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Moped.CanTraverse(tags));
            Assert.IsTrue(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Car.CanTraverse(tags));
            Assert.IsTrue(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "primary";
            Assert.IsTrue(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Moped.CanTraverse(tags));
            Assert.IsTrue(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Car.CanTraverse(tags));
            Assert.IsTrue(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "trunk";
            Assert.IsTrue(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Moped.CanTraverse(tags));
            Assert.IsTrue(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Car.CanTraverse(tags));
            Assert.IsTrue(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bus.CanTraverse(tags));

            tags["highway"] = "motorway";
            Assert.IsFalse(Vehicle.Pedestrian.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Bicycle.CanTraverse(tags));
            Assert.IsFalse(Vehicle.Moped.CanTraverse(tags));
            Assert.IsTrue(Vehicle.MotorCycle.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Car.CanTraverse(tags));
            Assert.IsTrue(Vehicle.SmallTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.BigTruck.CanTraverse(tags));
            Assert.IsTrue(Vehicle.Bus.CanTraverse(tags));
        }
    }
}

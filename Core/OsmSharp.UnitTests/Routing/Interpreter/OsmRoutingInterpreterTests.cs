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
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "cycleway";
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "bridleway";
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "path";
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "pedestrian";
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "road";
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "living_street";
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "residential";
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "unclassified";
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "tertiary";
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "secondary";
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "primary";
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "trunk";
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));

            tags["highway"] = "motorway";
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Pedestrian));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bicycle));
            Assert.IsFalse(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Moped));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.MotorCycle));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Car));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.SmallTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.BigTruck));
            Assert.IsTrue(interpreter.EdgeInterpreter.CanBeTraversedBy(tags, VehicleEnum.Bus));
        }
    }
}

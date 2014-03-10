// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OsmSharp.Routing;
using OsmSharp.Math.Geo;
using OsmSharp.Units.Distance;

namespace OsmSharp.Test.Unittests.Routing
{
    /// <summary>
    /// Does some unittesting on the OsmSharpRoute data format.
    /// </summary>
    [TestFixture]
    public class RouteTests
    {
        /// <summary>
        /// Tests the presence of tags in a calculated route.
        /// </summary>
        [Test]
        public void RouteConcatenateTagsTest()
        {
            Route route1 = new Route();
            route1.Vehicle = Vehicle.Car;
            RoutePointEntry route1entry1 = new RoutePointEntry();
            route1entry1.Distance = 10;
            route1entry1.Latitude = -1;
            route1entry1.Longitude = -1;
            route1entry1.Metrics = null;
            route1entry1.Points = new RoutePoint[1];
            route1entry1.Points[0] = new RoutePoint();
            route1entry1.Points[0].Name = "TestPoint1";
            route1entry1.Points[0].Tags = new RouteTags[1];
            route1entry1.Points[0].Tags[0] = new RouteTags();
            route1entry1.Points[0].Tags[0].Value = "TestValue1";
            route1entry1.Points[0].Tags[0].Key = "TestKey1";
            route1entry1.SideStreets = null;
            route1entry1.Tags = new RouteTags[1];
            route1entry1.Tags[0] = new RouteTags();
            route1entry1.Tags[0].Key = "highway";
            route1entry1.Tags[0].Value = "residential";
            route1entry1.Time = 10;
            route1entry1.Type = RoutePointEntryType.Start;
            route1entry1.WayFromName = string.Empty;
            route1entry1.WayFromNames = null;

            RoutePointEntry route1entry2 = new RoutePointEntry();
            route1entry2.Distance = 10;
            route1entry2.Latitude = -1;
            route1entry2.Longitude = -1;
            route1entry2.Metrics = null;
            route1entry2.Points = new RoutePoint[1];
            route1entry2.Points[0] = new RoutePoint();
            route1entry2.Points[0].Name = "TestPoint2";
            route1entry2.Points[0].Tags = new RouteTags[1];
            route1entry2.Points[0].Tags[0] = new RouteTags();
            route1entry2.Points[0].Tags[0].Value = "TestValue2";
            route1entry1.Points[0].Tags[0].Key = "TestKey2";
            route1entry2.SideStreets = null;
            route1entry2.Tags = new RouteTags[1];
            route1entry2.Tags[0] = new RouteTags();
            route1entry2.Tags[0].Key = "highway";
            route1entry2.Tags[0].Value = "residential";
            route1entry2.Time = 10;
            route1entry2.Type = RoutePointEntryType.Start;
            route1entry2.WayFromName = string.Empty;
            route1entry2.WayFromNames = null;

            route1.Entries = new RoutePointEntry[2];
            route1.Entries[0] = route1entry1;
            route1.Entries[1] = route1entry2;


            Route route2 = new Route();
            route2.Vehicle = Vehicle.Car;
            RoutePointEntry route2entry1 = new RoutePointEntry();
            route2entry1.Distance = 10;
            route2entry1.Latitude = -1;
            route2entry1.Longitude = -1;
            route2entry1.Metrics = null;
            route2entry1.Points = new RoutePoint[1];
            route2entry1.Points[0] = new RoutePoint();
            route2entry1.Points[0].Name = "TestPoint3";
            route2entry1.Points[0].Tags = new RouteTags[1];
            route2entry1.Points[0].Tags[0] = new RouteTags();
            route2entry1.Points[0].Tags[0].Value = "TestValue3";
            route2entry1.Points[0].Tags[0].Key = "TestKey3";
            route2entry1.SideStreets = null;
            route2entry1.Tags = new RouteTags[1];
            route2entry1.Tags[0] = new RouteTags();
            route2entry1.Tags[0].Key = "highway";
            route2entry1.Tags[0].Value = "residential";
            route2entry1.Time = 10;
            route2entry1.Type = RoutePointEntryType.Start;
            route2entry1.WayFromName = string.Empty;
            route2entry1.WayFromNames = null;

            RoutePointEntry route2entry2 = new RoutePointEntry();
            route2entry2.Distance = 10;
            route2entry2.Latitude = -1;
            route2entry2.Longitude = -1;
            route2entry2.Metrics = null;
            route2entry2.Points = new RoutePoint[1];
            route2entry2.Points[0] = new RoutePoint();
            route2entry2.Points[0].Name = "TestPoint4";
            route2entry2.Points[0].Tags = new RouteTags[1];
            route2entry2.Points[0].Tags[0] = new RouteTags();
            route2entry2.Points[0].Tags[0].Value = "TestValue4";
            route2entry1.Points[0].Tags[0].Key = "TestKey4";
            route2entry2.SideStreets = null;
            route2entry2.Tags = new RouteTags[1];
            route2entry2.Tags[0] = new RouteTags();
            route2entry2.Tags[0].Key = "highway";
            route2entry2.Tags[0].Value = "residential";
            route2entry2.Time = 10;
            route2entry2.Type = RoutePointEntryType.Start;
            route2entry2.WayFromName = string.Empty;
            route2entry2.WayFromNames = null;

            route2.Entries = new RoutePointEntry[2];
            route2.Entries[0] = route2entry1;
            route2.Entries[1] = route2entry2;

            Route concatenated = Route.Concatenate(route1, route2);

            // test the result.
            Assert.IsNotNull(concatenated);
            Assert.IsNotNull(concatenated.Entries);
            Assert.AreEqual(route1.Vehicle, concatenated.Vehicle);
            Assert.AreEqual(3, concatenated.Entries.Length);
            Assert.AreEqual("TestPoint1", concatenated.Entries[0].Points[0].Name);
            Assert.AreEqual("TestPoint2", concatenated.Entries[1].Points[0].Name);
            Assert.AreEqual("TestPoint3", concatenated.Entries[1].Points[1].Name);
            Assert.AreEqual("TestPoint4", concatenated.Entries[2].Points[0].Name);
        }

        /// <summary>
        /// Tests the presence of identical tags in a calculated route.
        /// </summary>
        [Test]
        public void RouteConcatenateTagsIdenticalTest()
        {
            Route route1 = new Route();
            RoutePointEntry route1entry1 = new RoutePointEntry();
            route1entry1.Distance = 10;
            route1entry1.Latitude = -1;
            route1entry1.Longitude = -1;
            route1entry1.Metrics = null;
            route1entry1.Points = new RoutePoint[1];
            route1entry1.Points[0] = new RoutePoint();
            route1entry1.Points[0].Name = "TestPoint1";
            route1entry1.Points[0].Tags = new RouteTags[1];
            route1entry1.Points[0].Tags[0] = new RouteTags();
            route1entry1.Points[0].Tags[0].Value = "TestValue1";
            route1entry1.Points[0].Tags[0].Key = "TestKey1";
            route1entry1.SideStreets = null;
            route1entry1.Tags = new RouteTags[1];
            route1entry1.Tags[0] = new RouteTags();
            route1entry1.Tags[0].Key = "highway";
            route1entry1.Tags[0].Value = "residential";
            route1entry1.Time = 10;
            route1entry1.Type = RoutePointEntryType.Start;
            route1entry1.WayFromName = string.Empty;
            route1entry1.WayFromNames = null;

            RoutePointEntry route1entry2 = new RoutePointEntry();
            route1entry2.Distance = 10;
            route1entry2.Latitude = -1;
            route1entry2.Longitude = -1;
            route1entry2.Metrics = null;
            route1entry2.Points = new RoutePoint[1];
            route1entry2.Points[0] = new RoutePoint();
            route1entry2.Points[0].Name = "TestPoint2";
            route1entry2.Points[0].Tags = new RouteTags[1];
            route1entry2.Points[0].Tags[0] = new RouteTags();
            route1entry2.Points[0].Tags[0].Value = "TestValue2";
            route1entry2.Points[0].Tags[0].Key = "TestKey2";
            route1entry2.SideStreets = null;
            route1entry2.Tags = new RouteTags[1];
            route1entry2.Tags[0] = new RouteTags();
            route1entry2.Tags[0].Key = "highway";
            route1entry2.Tags[0].Value = "residential";
            route1entry2.Time = 10;
            route1entry2.Type = RoutePointEntryType.Start;
            route1entry2.WayFromName = string.Empty;
            route1entry2.WayFromNames = null;

            route1.Entries = new RoutePointEntry[2];
            route1.Entries[0] = route1entry1;
            route1.Entries[1] = route1entry2;


            Route route2 = new Route();
            RoutePointEntry route2entry1 = new RoutePointEntry();
            route2entry1.Distance = 10;
            route2entry1.Latitude = -1;
            route2entry1.Longitude = -1;
            route2entry1.Metrics = null;
            route2entry1.Points = new RoutePoint[1];
            route2entry1.Points[0] = new RoutePoint();
            route2entry1.Points[0].Name = "TestPoint2";
            route2entry1.Points[0].Tags = new RouteTags[1];
            route2entry1.Points[0].Tags[0] = new RouteTags();
            route2entry1.Points[0].Tags[0].Value = "TestValue2";
            route2entry1.Points[0].Tags[0].Key = "TestKey2";
            route2entry1.SideStreets = null;
            route2entry1.Tags = new RouteTags[1];
            route2entry1.Tags[0] = new RouteTags();
            route2entry1.Tags[0].Key = "highway";
            route2entry1.Tags[0].Value = "residential";
            route2entry1.Time = 10;
            route2entry1.Type = RoutePointEntryType.Start;
            route2entry1.WayFromName = string.Empty;
            route2entry1.WayFromNames = null;

            RoutePointEntry route2entry2 = new RoutePointEntry();
            route2entry2.Distance = 10;
            route2entry2.Latitude = -1;
            route2entry2.Longitude = -1;
            route2entry2.Metrics = null;
            route2entry2.Points = new RoutePoint[1];
            route2entry2.Points[0] = new RoutePoint();
            route2entry2.Points[0].Name = "TestPoint4";
            route2entry2.Points[0].Tags = new RouteTags[1];
            route2entry2.Points[0].Tags[0] = new RouteTags();
            route2entry2.Points[0].Tags[0].Value = "TestValue4";
            route2entry2.Points[0].Tags[0].Key = "TestKey4";
            route2entry2.SideStreets = null;
            route2entry2.Tags = new RouteTags[1];
            route2entry2.Tags[0] = new RouteTags();
            route2entry2.Tags[0].Key = "highway";
            route2entry2.Tags[0].Value = "residential";
            route2entry2.Time = 10;
            route2entry2.Type = RoutePointEntryType.Start;
            route2entry2.WayFromName = string.Empty;
            route2entry2.WayFromNames = null;

            route2.Entries = new RoutePointEntry[2];
            route2.Entries[0] = route2entry1;
            route2.Entries[1] = route2entry2;

            Route concatenated = Route.Concatenate(route1, route2);

            // test the result.
            Assert.IsNotNull(concatenated);
            Assert.IsNotNull(concatenated.Entries);
            Assert.AreEqual(3, concatenated.Entries.Length);
            Assert.AreEqual("TestPoint1", concatenated.Entries[0].Points[0].Name);
            Assert.AreEqual("TestPoint2", concatenated.Entries[1].Points[0].Name);
            Assert.AreEqual(1, concatenated.Entries[1].Points.Length);
            Assert.AreEqual("TestPoint4", concatenated.Entries[2].Points[0].Name);
        }

        /// <summary>
        /// Tests the position after of route.
        /// </summary>
        [Test]
        public void RoutePositionAfterRegressionTest1()
        {
            var delta = .00001;

            // a route of approx 30 m along the following coordinates:
            // 50.98624687752063, 2.902620979360633
            // 50.98624687752063, 2.9027639004471673 (10m)
            // 50.986156907620895, 2.9027639004471673  (20m)
            // 50.9861564788317, 2.902620884621392 (30m)

            Route route1 = new Route();
            route1.Vehicle = Vehicle.Car;
            RoutePointEntry route1entry1 = new RoutePointEntry();
            route1entry1.Distance = -1;
            route1entry1.Latitude = 50.98624687752063f;
            route1entry1.Longitude = 2.902620979360633f;
            route1entry1.Metrics = null;
            route1entry1.Points = new RoutePoint[1];
            route1entry1.Points[0] = new RoutePoint();
            route1entry1.Points[0].Name = "TestPoint1";
            route1entry1.Points[0].Tags = new RouteTags[1];
            route1entry1.Points[0].Tags[0] = new RouteTags();
            route1entry1.Points[0].Tags[0].Value = "TestValue1";
            route1entry1.Points[0].Tags[0].Key = "TestKey1";
            route1entry1.SideStreets = null;
            route1entry1.Tags = new RouteTags[1];
            route1entry1.Tags[0] = new RouteTags();
            route1entry1.Tags[0].Key = "highway";
            route1entry1.Tags[0].Value = "residential";
            route1entry1.Time = 10;
            route1entry1.Type = RoutePointEntryType.Start;
            route1entry1.WayFromName = string.Empty;
            route1entry1.WayFromNames = null;

            RoutePointEntry route1entry2 = new RoutePointEntry();
            route1entry2.Distance = -1;
            route1entry2.Latitude = 50.98624687752063f;
            route1entry2.Longitude = 2.9027639004471673f;
            route1entry2.Metrics = null;
            route1entry2.Points = new RoutePoint[1];
            route1entry2.Points[0] = new RoutePoint();
            route1entry2.Points[0].Name = "TestPoint2";
            route1entry2.Points[0].Tags = new RouteTags[1];
            route1entry2.Points[0].Tags[0] = new RouteTags();
            route1entry2.Points[0].Tags[0].Value = "TestValue2";
            route1entry1.Points[0].Tags[0].Key = "TestKey2";
            route1entry2.SideStreets = null;
            route1entry2.Tags = new RouteTags[1];
            route1entry2.Tags[0] = new RouteTags();
            route1entry2.Tags[0].Key = "highway";
            route1entry2.Tags[0].Value = "residential";
            route1entry2.Time = 10;
            route1entry2.Type = RoutePointEntryType.Start;
            route1entry2.WayFromName = string.Empty;
            route1entry2.WayFromNames = null;

            RoutePointEntry route1entry3 = new RoutePointEntry();
            route1entry3.Distance = -1;
            route1entry3.Latitude = 50.986156907620895f;
            route1entry3.Longitude = 2.9027639004471673f;
            route1entry3.Metrics = null;
            route1entry3.Points = new RoutePoint[1];
            route1entry3.Points[0] = new RoutePoint();
            route1entry3.Points[0].Name = "TestPoint3";
            route1entry3.Points[0].Tags = new RouteTags[1];
            route1entry3.Points[0].Tags[0] = new RouteTags();
            route1entry3.Points[0].Tags[0].Value = "TestValue3";
            route1entry1.Points[0].Tags[0].Key = "TestKey3";
            route1entry3.SideStreets = null;
            route1entry3.Tags = new RouteTags[1];
            route1entry3.Tags[0] = new RouteTags();
            route1entry3.Tags[0].Key = "highway";
            route1entry3.Tags[0].Value = "residential";
            route1entry3.Time = 10;
            route1entry3.Type = RoutePointEntryType.Start;
            route1entry3.WayFromName = string.Empty;
            route1entry3.WayFromNames = null;

            RoutePointEntry route1entry4 = new RoutePointEntry();
            route1entry4.Distance = -1;
            route1entry4.Latitude = 50.9861564788317f;
            route1entry4.Longitude = 2.902620884621392f;
            route1entry4.Metrics = null;
            route1entry4.Points = new RoutePoint[1];
            route1entry4.Points[0] = new RoutePoint();
            route1entry4.Points[0].Name = "TestPoint4";
            route1entry4.Points[0].Tags = new RouteTags[1];
            route1entry4.Points[0].Tags[0] = new RouteTags();
            route1entry4.Points[0].Tags[0].Value = "TestValue4";
            route1entry1.Points[0].Tags[0].Key = "TestKey4";
            route1entry4.SideStreets = null;
            route1entry4.Tags = new RouteTags[1];
            route1entry4.Tags[0] = new RouteTags();
            route1entry4.Tags[0].Key = "highway";
            route1entry4.Tags[0].Value = "residential";
            route1entry4.Time = 10;
            route1entry4.Type = RoutePointEntryType.Start;
            route1entry4.WayFromName = string.Empty;
            route1entry4.WayFromNames = null;

            route1.Entries = new RoutePointEntry[4];
            route1.Entries[0] = route1entry1;
            route1.Entries[1] = route1entry2;
            route1.Entries[2] = route1entry3;
            route1.Entries[3] = route1entry4;

            // first test position after.
            var positionAfter = route1.PositionAfter(5);
            Assert.IsNotNull(positionAfter);
            Assert.AreEqual(5.0, positionAfter.DistanceReal(new GeoCoordinate(route1.Entries[0].Latitude, route1.Entries[0].Longitude)).Value, .0001);
            positionAfter = route1.PositionAfter(15);
            Assert.IsNotNull(positionAfter);
            Assert.AreEqual(15.0, 
                new GeoCoordinate(route1.Entries[0].Latitude, route1.Entries[0].Longitude).DistanceReal(new GeoCoordinate(route1.Entries[1].Latitude, route1.Entries[1].Longitude)).Value +
                positionAfter.DistanceReal(new GeoCoordinate(route1.Entries[1].Latitude, route1.Entries[1].Longitude)).Value, .0001);
            positionAfter = route1.PositionAfter(25);
            Assert.IsNotNull(positionAfter);
            Assert.AreEqual(25.0,
                new GeoCoordinate(route1.Entries[0].Latitude, route1.Entries[0].Longitude).DistanceReal(new GeoCoordinate(route1.Entries[1].Latitude, route1.Entries[1].Longitude)).Value +
                new GeoCoordinate(route1.Entries[1].Latitude, route1.Entries[1].Longitude).DistanceReal(new GeoCoordinate(route1.Entries[2].Latitude, route1.Entries[2].Longitude)).Value +
                positionAfter.DistanceReal(new GeoCoordinate(route1.Entries[2].Latitude, route1.Entries[2].Longitude)).Value, .0001);

            // use position after to test project on.
            int entryIdx;
            GeoCoordinate projected;
            Meter distanceFromStart;

            var distance = 5.0;
            var location = route1.PositionAfter(distance);
            Assert.IsTrue(route1.ProjectOn(location, out projected, out entryIdx, out distanceFromStart));
            Assert.AreEqual(distance, distanceFromStart.Value, delta);
            Assert.AreEqual(location.Latitude, projected.Latitude, delta);
            Assert.AreEqual(location.Longitude, projected.Longitude, delta);
            Assert.AreEqual(0, entryIdx);

            location = route1.PositionAfter(distanceFromStart);
            Assert.AreEqual(location.Latitude, projected.Latitude, delta);
            Assert.AreEqual(location.Longitude, projected.Longitude, delta);

            distance = 15.0;
            location = route1.PositionAfter(distance);
            Assert.IsTrue(route1.ProjectOn(location, out projected, out entryIdx, out distanceFromStart));
            Assert.AreEqual(distance, distanceFromStart.Value, delta);
            Assert.AreEqual(location.Latitude, projected.Latitude, delta);
            Assert.AreEqual(location.Longitude, projected.Longitude, delta);
            Assert.AreEqual(1, entryIdx);

            location = route1.PositionAfter(distanceFromStart);
            Assert.AreEqual(location.Latitude, projected.Latitude, delta);
            Assert.AreEqual(location.Longitude, projected.Longitude, delta);

            distance = 25;
            location = route1.PositionAfter(distance);
            Assert.IsTrue(route1.ProjectOn(location, out projected, out entryIdx, out distanceFromStart));
            Assert.AreEqual(distance, distanceFromStart.Value, delta);
            Assert.AreEqual(location.Latitude, projected.Latitude, delta);
            Assert.AreEqual(location.Longitude, projected.Longitude, delta);
            Assert.AreEqual(2, entryIdx);

            location = route1.PositionAfter(distanceFromStart);
            Assert.AreEqual(location.Latitude, projected.Latitude, delta);
            Assert.AreEqual(location.Longitude, projected.Longitude, delta);
        }
    }
}

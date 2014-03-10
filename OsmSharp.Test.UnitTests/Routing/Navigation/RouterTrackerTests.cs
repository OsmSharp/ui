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
using OsmSharp.Routing.Instructions;
using OsmSharp.Routing.Navigation;
using OsmSharp.Routing.Osm.Interpreter;

namespace OsmSharp.Test.Unittests.Routing.Navigation
{
    /// <summary>
    /// Contains tests for the route tracking class.
    /// </summary>
    [TestFixture]
    public class RouteTrackerTests
    {
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
            route1entry1.SideStreets = new RoutePointEntrySideStreet[] {
                    new RoutePointEntrySideStreet() { 
                        Latitude = 51, 
                        Longitude = 3.999f,
                        Tags = new RouteTags[] {
                            new RouteTags() { Key = "name", Value = "Street B" },
                            new RouteTags() { Key = "highway", Value = "residential" }
                        },
                        WayName = "Street B"
                    }
                };
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
            route1entry2.SideStreets = new RoutePointEntrySideStreet[] {
                    new RoutePointEntrySideStreet() { 
                        Latitude = 51, 
                        Longitude = 3.999f,
                        Tags = new RouteTags[] {
                            new RouteTags() { Key = "name", Value = "Street B" },
                            new RouteTags() { Key = "highway", Value = "residential" }
                        },
                        WayName = "Street B"
                    }
                };
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
            route1entry3.SideStreets = new RoutePointEntrySideStreet[] {
                    new RoutePointEntrySideStreet() { 
                        Latitude = 51, 
                        Longitude = 3.999f,
                        Tags = new RouteTags[] {
                            new RouteTags() { Key = "name", Value = "Street B" },
                            new RouteTags() { Key = "highway", Value = "residential" }
                        },
                        WayName = "Street B"
                    }
                };
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
            route1entry4.SideStreets = new RoutePointEntrySideStreet[] {
                    new RoutePointEntrySideStreet() { 
                        Latitude = 51, 
                        Longitude = 3.999f,
                        Tags = new RouteTags[] {
                            new RouteTags() { Key = "name", Value = "Street B" },
                            new RouteTags() { Key = "highway", Value = "residential" }
                        },
                        WayName = "Street B"
                    }
                };
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

            // create the route tracker.
            var routeTracker = new RouteTracker(route1, new OsmRoutingInterpreter());

            var distance = 5.0;
            var location = route1.PositionAfter(distance);
            routeTracker.Track(location);
            Assert.AreEqual(distance, routeTracker.DistanceFromStart.Value, delta);
            Assert.AreEqual(location.Latitude, routeTracker.PositionRoute.Latitude, delta);
            Assert.AreEqual(location.Longitude, routeTracker.PositionRoute.Longitude, delta);
            Assert.AreEqual(new GeoCoordinate(route1.Entries[1].Latitude, route1.Entries[1].Longitude).DistanceReal(location).Value, routeTracker.DistanceNextInstruction.Value, delta);
            var locationAfter = routeTracker.PositionAfter(10.0);

            distance = 15.0;
            location = route1.PositionAfter(distance);
            Assert.AreEqual(location.Latitude, locationAfter.Latitude, delta);
            Assert.AreEqual(location.Longitude, locationAfter.Longitude, delta);
            routeTracker.Track(location);
            Assert.AreEqual(distance, routeTracker.DistanceFromStart.Value, delta);
            Assert.AreEqual(location.Latitude, routeTracker.PositionRoute.Latitude, delta);
            Assert.AreEqual(location.Longitude, routeTracker.PositionRoute.Longitude, delta);
            Assert.AreEqual(new GeoCoordinate(route1.Entries[2].Latitude, route1.Entries[2].Longitude).DistanceReal(location).Value, routeTracker.DistanceNextInstruction.Value, delta);

            location = new GeoCoordinate(route1.Entries[3].Latitude, route1.Entries[3].Longitude);
            Meter distanceFromStart;
            route1.ProjectOn(location, out distanceFromStart);
            distance = distanceFromStart.Value;
            routeTracker.Track(location);
            Assert.AreEqual(distance, routeTracker.DistanceFromStart.Value, delta);
            Assert.AreEqual(location.Latitude, routeTracker.PositionRoute.Latitude, delta);
            Assert.AreEqual(location.Longitude, routeTracker.PositionRoute.Longitude, delta);
            Assert.AreEqual(0, routeTracker.DistanceNextInstruction.Value, delta);
        }

        /// <summary>
        /// Tests the position after of route.
        /// </summary>
        [Test]
        public void RoutePositionAfterRegressionTest2()
        {
            var delta = .00001;

            // a route of approx 30 m along the following coordinates:
            // 50.98624687752063, 2.902620979360633
            // 50.98624687752063, 2.9027639004471673 (10m)
            // 50.986156907620895, 2.9027639004471673  (20m)
            // 50.9861564788317, 2.902620884621392 (30m)

            Route route1 = new Route();
            route1.Vehicle = Vehicle.Car;

            RoutePointEntry route1entry0 = new RoutePointEntry();
            route1entry0.Distance = -1;
            route1entry0.Latitude = 50.98624687752063f;
            route1entry0.Longitude = 2.902620979360633f;
            route1entry0.Metrics = null;
            route1entry0.Points = new RoutePoint[1];
            route1entry0.Points[0] = new RoutePoint();
            route1entry0.Points[0].Name = "TestPoint1";
            route1entry0.Points[0].Tags = new RouteTags[1];
            route1entry0.Points[0].Tags[0] = new RouteTags();
            route1entry0.Points[0].Tags[0].Value = "TestValue1";
            route1entry0.Points[0].Tags[0].Key = "TestKey1";
            route1entry0.SideStreets = new RoutePointEntrySideStreet[] {
                    new RoutePointEntrySideStreet() { 
                        Latitude = 51, 
                        Longitude = 3.999f,
                        Tags = new RouteTags[] {
                            new RouteTags() { Key = "name", Value = "Street B" },
                            new RouteTags() { Key = "highway", Value = "residential" }
                        },
                        WayName = "Street B"
                    }
                };
            route1entry0.Tags = new RouteTags[1];
            route1entry0.Tags[0] = new RouteTags();
            route1entry0.Tags[0].Key = "highway";
            route1entry0.Tags[0].Value = "residential";
            route1entry0.Time = 10;
            route1entry0.Type = RoutePointEntryType.Start;
            route1entry0.WayFromName = string.Empty;
            route1entry0.WayFromNames = null;

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
            route1entry1.SideStreets = new RoutePointEntrySideStreet[] {
                    new RoutePointEntrySideStreet() { 
                        Latitude = 51, 
                        Longitude = 3.999f,
                        Tags = new RouteTags[] {
                            new RouteTags() { Key = "name", Value = "Street B" },
                            new RouteTags() { Key = "highway", Value = "residential" }
                        },
                        WayName = "Street B"
                    }
                };
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
            route1entry2.SideStreets = new RoutePointEntrySideStreet[] {
                    new RoutePointEntrySideStreet() { 
                        Latitude = 51, 
                        Longitude = 3.999f,
                        Tags = new RouteTags[] {
                            new RouteTags() { Key = "name", Value = "Street B" },
                            new RouteTags() { Key = "highway", Value = "residential" }
                        },
                        WayName = "Street B"
                    }
                };
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
            route1entry3.SideStreets = new RoutePointEntrySideStreet[] {
                    new RoutePointEntrySideStreet() { 
                        Latitude = 51, 
                        Longitude = 3.999f,
                        Tags = new RouteTags[] {
                            new RouteTags() { Key = "name", Value = "Street B" },
                            new RouteTags() { Key = "highway", Value = "residential" }
                        },
                        WayName = "Street B"
                    }
                };
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
            route1entry4.SideStreets = new RoutePointEntrySideStreet[] {
                    new RoutePointEntrySideStreet() { 
                        Latitude = 51, 
                        Longitude = 3.999f,
                        Tags = new RouteTags[] {
                            new RouteTags() { Key = "name", Value = "Street B" },
                            new RouteTags() { Key = "highway", Value = "residential" }
                        },
                        WayName = "Street B"
                    }
                };
            route1entry4.Tags = new RouteTags[1];
            route1entry4.Tags[0] = new RouteTags();
            route1entry4.Tags[0].Key = "highway";
            route1entry4.Tags[0].Value = "residential";
            route1entry4.Time = 10;
            route1entry4.Type = RoutePointEntryType.Start;
            route1entry4.WayFromName = string.Empty;
            route1entry4.WayFromNames = null;

            route1.Entries = new RoutePointEntry[5];
            route1.Entries[0] = route1entry0;
            route1.Entries[1] = route1entry1;
            route1.Entries[2] = route1entry2;
            route1.Entries[3] = route1entry3;
            route1.Entries[4] = route1entry4;

            // create the route tracker.
            var routeTracker = new RouteTracker(route1, new OsmRoutingInterpreter());

            var distance = 5.0;
            var location = route1.PositionAfter(distance);
            routeTracker.Track(location);
            Assert.AreEqual(distance, routeTracker.DistanceFromStart.Value, delta);
            Assert.AreEqual(location.Latitude, routeTracker.PositionRoute.Latitude, delta);
            Assert.AreEqual(location.Longitude, routeTracker.PositionRoute.Longitude, delta);
            Assert.AreEqual(new GeoCoordinate(route1.Entries[2].Latitude, route1.Entries[2].Longitude).DistanceReal(location).Value, routeTracker.DistanceNextInstruction.Value, delta);
            var locationAfter = routeTracker.PositionAfter(10.0);

            distance = 15.0;
            location = route1.PositionAfter(distance);
            Assert.AreEqual(location.Latitude, locationAfter.Latitude, delta);
            Assert.AreEqual(location.Longitude, locationAfter.Longitude, delta);
            routeTracker.Track(location);
            Assert.AreEqual(distance, routeTracker.DistanceFromStart.Value, delta);
            Assert.AreEqual(location.Latitude, routeTracker.PositionRoute.Latitude, delta);
            Assert.AreEqual(location.Longitude, routeTracker.PositionRoute.Longitude, delta);
            Assert.AreEqual(new GeoCoordinate(route1.Entries[3].Latitude, route1.Entries[3].Longitude).DistanceReal(location).Value, routeTracker.DistanceNextInstruction.Value, delta);

            location = new GeoCoordinate(route1.Entries[4].Latitude, route1.Entries[4].Longitude);
            Meter distanceFromStart;
            route1.ProjectOn(location, out distanceFromStart);
            distance = distanceFromStart.Value;
            routeTracker.Track(location);
            Assert.AreEqual(distance, routeTracker.DistanceFromStart.Value, delta);
            Assert.AreEqual(location.Latitude, routeTracker.PositionRoute.Latitude, delta);
            Assert.AreEqual(location.Longitude, routeTracker.PositionRoute.Longitude, delta);
            Assert.AreEqual(0, routeTracker.DistanceNextInstruction.Value, delta);
        }
    }
}
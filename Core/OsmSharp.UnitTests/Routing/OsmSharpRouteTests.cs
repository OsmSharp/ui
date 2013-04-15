// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using OsmSharp.Routing.Route;

namespace OsmSharp.Osm.UnitTests
{
    /// <summary>
    /// Does some unittesting on the OsmSharpRoute data format.
    /// </summary>
    [TestFixture]
    public class OsmSharpRouteTests
    {
        /// <summary>
        /// Tests the presence of tags in a calculated route.
        /// </summary>
        [Test]
        public void RouteConcatenateTagsTest()
        {
            OsmSharpRoute route1 = new OsmSharpRoute();
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


            OsmSharpRoute route2 = new OsmSharpRoute();
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

            OsmSharpRoute concatenated = OsmSharpRoute.Concatenate(route1, route2);

            // test the result.
            Assert.IsNotNull(concatenated);
            Assert.IsNotNull(concatenated.Entries);
            Assert.AreEqual(3, concatenated.Entries.Length);
            Assert.AreEqual("TestPoint1", concatenated.Entries[0].Points[0].Name);
            Assert.AreEqual("TestPoint3", concatenated.Entries[1].Points[0].Name);
            Assert.AreEqual("TestPoint4", concatenated.Entries[2].Points[0].Name);
        }
    }
}

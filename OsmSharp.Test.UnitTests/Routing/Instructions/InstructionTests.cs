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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Routing;
using OsmSharp.Routing.Instructions;
using OsmSharp.Routing.Osm.Interpreter;

namespace OsmSharp.Test.Unittests.Routing.Instructions
{
    /// <summary>
    /// Holds a number of unittests for generating routing instructions for tiny pieces of routes.
    /// </summary>
    [TestFixture]
    public class InstructionTests
    {
        /// <summary>
        /// Tests a simple turn.
        /// </summary>
        [Test]
        public void TestSimpleTurn()
        {
            Route route = new Route();
            route.Entries = new RoutePointEntry[3];
            route.Entries[0] = new RoutePointEntry()
            {
                Distance = 0,
                Latitude = 50.999f,
                Longitude = 4,
                Points = new RoutePoint[] { 
                    new RoutePoint() 
                    {
                        Latitude = 50.999f,
                        Longitude = 4,
                        Name = "Start"
                    }},
                SideStreets = null,
                Type = RoutePointEntryType.Start
            };
            route.Entries[1] = new RoutePointEntry()
            {
                Distance = 0,
                Latitude = 51,
                Longitude = 4,
                Tags = new RouteTags[] {
                    new RouteTags() { Key = "name", Value = "Street A" },
                    new RouteTags() { Key = "highway", Value = "residential" }
                },
                Type = RoutePointEntryType.Along,
                SideStreets = new RoutePointEntrySideStreet[] {
                    new RoutePointEntrySideStreet() { 
                        Latitude = 51, 
                        Longitude = 3.999f,
                        Tags = new RouteTags[] {
                            new RouteTags() { Key = "name", Value = "Street B" },
                            new RouteTags() { Key = "highway", Value = "residential" }
                        },
                        WayName = "Street B"
                    }
                }
            };
            route.Entries[2] = new RoutePointEntry()
            {
                Distance = 0,
                Latitude = 51,
                Longitude = 4.001f,
                Tags = new RouteTags[] {
                    new RouteTags() { Key = "name", Value = "Street A" },
                    new RouteTags() { Key = "highway", Value = "residential" }
                },
                Type = RoutePointEntryType.Stop,
                Points = new RoutePoint[] { 
                    new RoutePoint() 
                    {
                        Latitude = 51,
                        Longitude = 4.001f,
                        Name = "Stop"
                    }},
            };

            // create the language generator.
            var languageGenerator = new LanguageTestGenerator();

            // generate instructions.
            List<Instruction> instructions = InstructionGenerator.Generate(route, new OsmRoutingInterpreter(), languageGenerator);
            Assert.AreEqual(3, instructions.Count);
            Assert.AreEqual("GenerateDirectTurn:0_Right_0", instructions[1].Text);
        }
    }
}

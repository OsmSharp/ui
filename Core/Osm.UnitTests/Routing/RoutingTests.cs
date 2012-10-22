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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osm.Routing.Core.Resolving;
using Osm.Routing.Core;
using Osm.Routing.Core.Interpreter;
using Osm.Routing.Core.Constraints;
using Tools.Math.Geo;
using Osm.Routing.Core.Route;

namespace Osm.UnitTests.Routing
{
    /// <summary>
    /// Base class with tests around IRouter<ResolvedType> objects.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class SimpleRoutingTests<ResolvedType> 
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Returns a router test object.
        /// </summary>
        /// <returns></returns>
        public abstract IRouter<ResolvedType> BuildRouter(RoutingInterpreterBase interpreter, IRoutingConstraints constraints);

        /// <summary>
        /// Tests that a router actually finds the shortest route.
        /// </summary>
        protected void DoTestShortestDefault()
        {
            IRouter<ResolvedType> router = this.BuildRouter(
                new Osm.Routing.Core.Interpreter.Default.DefaultVehicleInterpreter(VehicleEnum.Car), 
                new Osm.Routing.Core.Constraints.Cars.DefaultCarConstraints());
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0578532, 3.7192229));
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0576193, 3.7191801));

            OsmSharpRoute route = router.Calculate(source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(5, route.Entries.Length);

            Assert.AreEqual(51.0578532, route.Entries[0].Latitude, 0.00001);
            Assert.AreEqual(3.7192229, route.Entries[0].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Start, route.Entries[0].Type);

            Assert.AreEqual(51.0578518, route.Entries[1].Latitude, 0.00001);
            Assert.AreEqual(3.7195654, route.Entries[1].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[1].Type);

            Assert.AreEqual(51.0577299, route.Entries[2].Latitude, 0.00001);
            Assert.AreEqual(3.7197450, route.Entries[2].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[2].Type);

            Assert.AreEqual(51.0576193, route.Entries[3].Latitude, 0.00001);
            Assert.AreEqual(3.7196582, route.Entries[3].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[3].Type);

            Assert.AreEqual(51.0576193, route.Entries[4].Latitude, 0.00001);
            Assert.AreEqual(3.7191801, route.Entries[4].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Stop, route.Entries[4].Type);
        }

        /// <summary>
        /// Tests that a router preserves tags given to resolved points.
        /// </summary>
        protected void DoTestResolvedTags()
        {
            IRouter<ResolvedType> router = this.BuildRouter(
                new Osm.Routing.Core.Interpreter.Default.DefaultVehicleInterpreter(VehicleEnum.Car),
                new Osm.Routing.Core.Constraints.Cars.DefaultCarConstraints());
            ResolvedType source = router.Resolve(new GeoCoordinate(51.0578532, 3.7192229));
            source.Tags.Add(new KeyValuePair<string,string>("name", "source"));
            ResolvedType target = router.Resolve(new GeoCoordinate(51.0576193, 3.7191801));
            target.Tags.Add(new KeyValuePair<string, string>("name", "target"));

            OsmSharpRoute route = router.Calculate(source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(5, route.Entries.Length);

            Assert.AreEqual(51.0578532, route.Entries[0].Latitude, 0.00001);
            Assert.AreEqual(3.7192229, route.Entries[0].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Start, route.Entries[0].Type);
            Assert.IsNotNull(route.Entries[0].Points[0].Tags);
            Assert.AreEqual(1, route.Entries[0].Points[0].Tags.Length);
            Assert.AreEqual("source", route.Entries[0].Points[0].Tags[0].Value);

            Assert.AreEqual(51.0576193, route.Entries[4].Latitude, 0.00001);
            Assert.AreEqual(3.7191801, route.Entries[4].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Stop, route.Entries[4].Type);
            Assert.IsNotNull(route.Entries[4].Points[0].Tags);
            Assert.AreEqual(1, route.Entries[4].Points[0].Tags.Length);
            Assert.AreEqual("target", route.Entries[4].Points[0].Tags[0].Value);
        }
    }
}

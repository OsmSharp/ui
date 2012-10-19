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
    }
}

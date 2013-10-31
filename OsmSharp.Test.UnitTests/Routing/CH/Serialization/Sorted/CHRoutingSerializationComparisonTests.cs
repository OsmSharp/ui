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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using OsmSharp.Math.Geo;

namespace OsmSharp.Test.Unittests.Routing.CH.Serialization.Sorted
{
    /// <summary>
    /// Tests comparing routing before/after serilization.
    /// </summary>
    [TestFixture]
    public class CHRoutingSerializationComparisonTests : RoutingComparisonTests
    {
        /// <summary>
        /// Tests serializing/deserializing RoutingSerializationRoutingComparisonTest using the V1 routing serializer.
        /// </summary>
        [Test]
        public void RoutingSerializationCHSortedRoutingComparisonTest()
        {
            const string embeddedString = "OsmSharp.Test.Unittests.test_network_real1.osm";

            // creates a new interpreter.
            var interpreter = new OsmRoutingInterpreter();

            // do the data processing.
            var original = CHEdgeGraphOsmStreamTarget.Preprocess(new XmlOsmStreamSource(
                                                                   Assembly.GetExecutingAssembly()
                                                                           .GetManifestResourceStream(embeddedString)),
                                                               interpreter,
                                                               Vehicle.Car);

            // create serializer.
            var routingSerializer = new OsmSharp.Routing.CH.Serialization.Sorted.CHEdgeDataDataSourceSerializer(true);

            // serialize/deserialize.
            byte[] byteArray;
            using (var stream = new MemoryStream())
            {
                try
                {
                    routingSerializer.Serialize(stream, original);
                    byteArray = stream.ToArray();
                }
                catch (Exception)
                {
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                    throw;
                }
            }

            IBasicRouterDataSource<CHEdgeData> deserializedVersion =
                routingSerializer.Deserialize(new MemoryStream(byteArray));
            Assert.AreEqual(original.TagsIndex.Get(0), deserializedVersion.TagsIndex.Get(0));

            // create reference router.
            original = CHEdgeGraphOsmStreamTarget.Preprocess(new XmlOsmStreamSource(
                                                                   Assembly.GetExecutingAssembly()
                                                                           .GetManifestResourceStream(embeddedString)),
                                                               interpreter,
                                                               Vehicle.Car);
            var basicRouterOriginal = new CHRouter();
            Router referenceRouter = Router.CreateCHFrom(
                original, basicRouterOriginal, interpreter);

            // try to do some routing on the deserialized version.
            var basicRouter = new CHRouter();
            Router router = Router.CreateCHFrom(
                deserializedVersion, basicRouter, interpreter);

            // loop over all nodes and resolve their locations.
            var resolvedReference = new RouterPoint[original.VertexCount];
            var resolved = new RouterPoint[original.VertexCount];
            for (uint idx = 1; idx < original.VertexCount + 1; idx++)
            { // resolve each vertex.
                float latitude, longitude;
                if (original.GetVertex(idx, out latitude, out longitude))
                {
                    resolvedReference[idx - 1] = referenceRouter.Resolve(Vehicle.Car, new GeoCoordinate(latitude, longitude));
                    resolved[idx - 1] = router.Resolve(Vehicle.Car, new GeoCoordinate(latitude, longitude));
                }

                Assert.IsNotNull(resolvedReference[idx - 1]);
                Assert.IsNotNull(resolved[idx - 1]);

                Assert.AreEqual(resolvedReference[idx - 1].Location.Latitude,
                    resolved[idx - 1].Location.Latitude, 0.0001);
                Assert.AreEqual(resolvedReference[idx - 1].Location.Longitude,
                    resolved[idx - 1].Location.Longitude, 0.0001);
            }

            //    // check all the routes having the same weight(s).
            //    for (int fromIdx = 0; fromIdx < resolved.Length; fromIdx++)
            //    {
            //        for (int toIdx = 0; toIdx < resolved.Length; toIdx++)
            //        {
            //            OsmSharpRoute referenceRoute = referenceRouter.Calculate(VehicleEnum.Car,
            //                resolvedReference[fromIdx], resolvedReference[toIdx]);
            //            OsmSharpRoute route = router.Calculate(VehicleEnum.Car,
            //                resolved[fromIdx], resolved[toIdx]);

            //            Assert.IsNotNull(referenceRoute);
            //            Assert.IsNotNull(route);
            //            //Assert.AreEqual(referenceRoute.TotalDistance, route.TotalDistance, 0.1);
            //            // TODO: meta data is missing in some CH routing; see issue 
            //            //Assert.AreEqual(reference_route.TotalTime, route.TotalTime, 0.0001);
            //        }
            //    }
        }

        /// <summary>
        /// Not needed here.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="embeddedName"></param>
        /// <param name="contract"></param>
        /// <returns></returns>
        public override Router BuildRouter(IOsmRoutingInterpreter interpreter, string embeddedName, bool contract)
        {
            throw new NotImplementedException();
        }
    }
}
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Graphs.Serialization;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Routers;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Osm.Streams.Graphs;

namespace OsmSharp.Test.Unittests.Routing.Serialization
{
    /// <summary>
    /// Holds tests for the routing serialization.
    /// </summary>
    [TestFixture]
    public class RoutingSerializationTests
    {
        /// <summary>
        /// Tests serializing/deserializing DynamicGraphRouterDataSource using the V2 routing serializer.
        /// </summary>
        [Test]
        public void RoutingSerializationV2DataSourceTest()
        {
            const string embeddedString = "OsmSharp.Test.Unittests.test_network.osm";

            // create the tags index.
            var tagsIndex = new TagsTableCollectionIndex();

            // creates a new interpreter.
            var interpreter = new OsmRoutingInterpreter();

            // do the data processing.
            var original =
                new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamTarget(
                original, interpreter, tagsIndex);
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedString));
            targetData.RegisterSource(dataProcessorSource);
            targetData.Pull();

            // store some lat/lons.
            var verticesLocations = new List<GeoCoordinate>();
            for (uint vertex = 1; vertex <= 5; vertex++)
            {
                float latitude, longitude;
                if(original.GetVertex(vertex, out latitude, out longitude))
                {
                    verticesLocations.Add(
                        new GeoCoordinate(latitude, longitude));
                }
            }

            // create serializer.
            var routingSerializer = new V2RoutingDataSourceLiveEdgeSerializer(false);

            // serialize/deserialize.
            IBasicRouterDataSource<LiveEdge> deserializedVersion;
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
            using (var stream = new MemoryStream(byteArray))
            {
                try
                {
                    deserializedVersion = routingSerializer.Deserialize(stream, false);
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

            //Assert.AreEqual(original.VertexCount, deserializedVersion.VertexCount);
            Assert.AreEqual(original.TagsIndex.Get(0), deserializedVersion.TagsIndex.Get(0));

            for (uint vertex = 1; vertex <= 5; vertex++)
            {
                float latitude, longitude;
                if (deserializedVersion.GetVertex(vertex, out latitude, out longitude))
                {
                    Assert.AreEqual(verticesLocations[(int)vertex - 1].Latitude, latitude, 0.000001);
                    Assert.AreEqual(verticesLocations[(int)vertex - 1].Longitude, longitude, 0.000001);
                }
            }
        }

        /// <summary>
        /// Tests serializing/deserializing RoutingSerializationRoutingTest using the V1 routing serializer.
        /// </summary>
        [Test]
        public void RoutingSerializationV2RoutingTest()
        {
            const string embeddedString = "OsmSharp.Test.Unittests.test_network.osm";

            // create the tags index.
            var tagsIndex = new TagsTableCollectionIndex();

            // creates a new interpreter.
            var interpreter = new OsmRoutingInterpreter();

            // do the data processing.
            var original =
                new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamTarget(
                original, interpreter, tagsIndex);
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedString));
            targetData.RegisterSource(dataProcessorSource);
            targetData.Pull();

            // create serializer.
            var routingSerializer = new V2RoutingDataSourceLiveEdgeSerializer(false);

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

            IBasicRouterDataSource<LiveEdge> deserializedVersion =
                routingSerializer.Deserialize(new MemoryStream(byteArray));
            Assert.AreEqual(original.TagsIndex.Get(0), deserializedVersion.TagsIndex.Get(0));

            // try to do some routing on the deserialized version.
            var basicRouter = new DykstraRoutingLive();
            Router router = Router.CreateLiveFrom(deserializedVersion, basicRouter, interpreter);
            RouterPoint source = router.Resolve(Vehicle.Car,
                new GeoCoordinate(51.0578532, 3.7192229));
            RouterPoint target = router.Resolve(Vehicle.Car,
                new GeoCoordinate(51.0576193, 3.7191801));

            // calculate the route.
            Route route = router.Calculate(Vehicle.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(5, route.Entries.Length);

            float latitude, longitude;
            deserializedVersion.GetVertex(20, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[0].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[0].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Start, route.Entries[0].Type);

            deserializedVersion.GetVertex(21, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[1].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[1].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[1].Type);

            deserializedVersion.GetVertex(16, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[2].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[2].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[2].Type);

            deserializedVersion.GetVertex(22, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[3].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[3].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[3].Type);

            deserializedVersion.GetVertex(23, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[4].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[4].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Stop, route.Entries[4].Type);
        }

        /// <summary>
        /// Tests serializing/deserializing RoutingSerializationRoutingTest using the V1 routing serializer.
        /// </summary>
        [Test]
        public void RoutingSerializationV2CompressedRoutingTest()
        {
            const string embeddedString = "OsmSharp.Test.Unittests.test_network.osm";

            // create the tags index.
            var tagsIndex = new TagsTableCollectionIndex();

            // creates a new interpreter.
            var interpreter = new OsmRoutingInterpreter();

            // do the data processing.
            var original =
                new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamTarget(
                original, interpreter, tagsIndex);
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedString));
            targetData.RegisterSource(dataProcessorSource);
            targetData.Pull();

            // create serializer.
            var routingSerializer = new V2RoutingDataSourceLiveEdgeSerializer(true);

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

            IBasicRouterDataSource<LiveEdge> deserializedVersion =
                routingSerializer.Deserialize(new MemoryStream(byteArray));
            Assert.AreEqual(original.TagsIndex.Get(0), deserializedVersion.TagsIndex.Get(0));

            // try to do some routing on the deserialized version.
            var basicRouter = new DykstraRoutingLive();
            Router router = Router.CreateLiveFrom(deserializedVersion, basicRouter, interpreter);
            RouterPoint source = router.Resolve(Vehicle.Car,
                new GeoCoordinate(51.0578532, 3.7192229));
            RouterPoint target = router.Resolve(Vehicle.Car,
                new GeoCoordinate(51.0576193, 3.7191801));

            // calculate the route.
            Route route = router.Calculate(Vehicle.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(5, route.Entries.Length);

            float latitude, longitude;
            deserializedVersion.GetVertex(20, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[0].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[0].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Start, route.Entries[0].Type);

            deserializedVersion.GetVertex(21, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[1].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[1].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[1].Type);

            deserializedVersion.GetVertex(16, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[2].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[2].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[2].Type);

            deserializedVersion.GetVertex(22, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[3].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[3].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Along, route.Entries[3].Type);

            deserializedVersion.GetVertex(23, out latitude, out longitude);
            Assert.AreEqual(latitude, route.Entries[4].Latitude, 0.00001);
            Assert.AreEqual(longitude, route.Entries[4].Longitude, 0.00001);
            Assert.AreEqual(RoutePointEntryType.Stop, route.Entries[4].Type);
        }

        /// <summary>
        /// Tests serializing/deserializing RoutingSerializationRoutingComparisonTest using the V1 routing serializer.
        /// </summary>
        [Test]
        public void RoutingSerializationV2RoutingComparisonTest()
        {
            const string embeddedString = "OsmSharp.Test.Unittests.test_network_real1.osm";

            // creates a new interpreter.
            var interpreter = new OsmRoutingInterpreter();

            // do the data processing.
            var original = LiveGraphOsmStreamTarget.Preprocess(new XmlOsmStreamSource(
                                                                   Assembly.GetExecutingAssembly()
                                                                           .GetManifestResourceStream(embeddedString)),
                                                               interpreter);

            // create the original routing.
//            var basicRouterOriginal = new DykstraRoutingLive(original.TagsIndex);
//            Router referenceRouter = Router.CreateLiveFrom(
//                original, basicRouterOriginal, interpreter);

            // create serializer.
            var routingSerializer = new V2RoutingDataSourceLiveEdgeSerializer(false);

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

            IBasicRouterDataSource<LiveEdge> deserializedVersion =
                routingSerializer.Deserialize(new MemoryStream(byteArray));
            Assert.AreEqual(original.TagsIndex.Get(0), deserializedVersion.TagsIndex.Get(0));

//            // try to do some routing on the deserialized version.
//            var basicRouter =
//                new DykstraRoutingLive(deserializedVersion.TagsIndex);
//            Router router = Router.CreateLiveFrom(
//                deserializedVersion, basicRouter, interpreter);

            //// loop over all nodes and resolve their locations.
            //var resolvedReference = new RouterPoint[original.VertexCount];
            //var resolved = new RouterPoint[original.VertexCount];
            //for (uint idx = 1; idx < original.VertexCount + 1; idx++)
            //{ // resolve each vertex.
            //    float latitude, longitude;
            //    if (original.GetVertex(idx, out latitude, out longitude))
            //    {
            //        resolvedReference[idx - 1] = referenceRouter.Resolve(Vehicle.Car, new GeoCoordinate(latitude, longitude));
            //        resolved[idx - 1] = router.Resolve(Vehicle.Car, new GeoCoordinate(latitude, longitude));
            //    }

            //    Assert.IsNotNull(resolvedReference[idx - 1]);
            //    Assert.IsNotNull(resolved[idx - 1]);

            //    Assert.AreEqual(resolvedReference[idx - 1].Location.Latitude,
            //        resolved[idx - 1].Location.Latitude, 0.0001);
            //    Assert.AreEqual(resolvedReference[idx - 1].Location.Longitude,
            //        resolved[idx - 1].Location.Longitude, 0.0001);
            //}

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

    }
}
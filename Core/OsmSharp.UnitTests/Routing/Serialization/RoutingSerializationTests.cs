using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Osm.Data.Xml.Processor;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Graph.Serialization.v2;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Route;
using OsmSharp.Routing.Routers;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;

namespace OsmSharp.UnitTests.Routing.Serialization
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
            const string embeddedString = "OsmSharp.UnitTests.test_network.osm";

            // create the tags index.
            var tagsIndex = new SimpleTagsIndex();

            // creates a new interpreter.
            var interpreter = new OsmRoutingInterpreter();

            // do the data processing.
            var original =
                new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamWriter(
                original, interpreter, original.TagsIndex);
            var dataProcessorSource = new XmlOsmStreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedString));
            targetData.RegisterSource(dataProcessorSource);
            targetData.Pull();

            // create serializer.
            var routingSerializer = new V2RoutingSerializer();

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
                    deserializedVersion = routingSerializer.Deserialize(stream);
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
        }

        /// <summary>
        /// Tests serializing/deserializing RoutingSerializationRoutingTest using the V1 routing serializer.
        /// </summary>
        [Test]
        public void RoutingSerializationV2RoutingTest()
        {
            const string embeddedString = "OsmSharp.UnitTests.test_network.osm";

            // create the tags index.
            var tagsIndex = new SimpleTagsIndex();

            // creates a new interpreter.
            var interpreter = new OsmRoutingInterpreter();

            // do the data processing.
            var original =
                new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamWriter(
                original, interpreter, original.TagsIndex);
            var dataProcessorSource = new XmlOsmStreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedString));
            targetData.RegisterSource(dataProcessorSource);
            targetData.Pull();

            // create serializer.
            var routingSerializer = new V2RoutingSerializer();

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
            var basicRouter = new DykstraRoutingLive(deserializedVersion.TagsIndex);
            Router router = Router.CreateLiveFrom(deserializedVersion, basicRouter, interpreter);
            RouterPoint source = router.Resolve(VehicleEnum.Car,
                new GeoCoordinate(51.0578532, 3.7192229));
            RouterPoint target = router.Resolve(VehicleEnum.Car,
                new GeoCoordinate(51.0576193, 3.7191801));

            // calculate the route.
            OsmSharpRoute route = router.Calculate(VehicleEnum.Car, source, target);
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
            const string embeddedString = "OsmSharp.UnitTests.test_network_real1.osm";

            // create the tags index.
            var tagsIndex = new SimpleTagsIndex();

            // creates a new interpreter.
            var interpreter = new OsmRoutingInterpreter();

            // do the data processing.
            var original =
                new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamWriter(original, interpreter, original.TagsIndex);
            var dataProcessorSource = new XmlOsmStreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedString));
            targetData.RegisterSource(dataProcessorSource);
            targetData.Pull();

            // create the original routing.
            var basicRouterOriginal =
                new DykstraRoutingLive(original.TagsIndex);
            Router referenceRouter = Router.CreateLiveFrom(
                original, basicRouterOriginal, interpreter);

            // create serializer.
            var routingSerializer = new V2RoutingSerializer();

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
            var basicRouter =
                new DykstraRoutingLive(deserializedVersion.TagsIndex);
            Router router = Router.CreateLiveFrom(
                deserializedVersion, basicRouter, interpreter);

            // loop over all nodes and resolve their locations.
            var resolvedReference = new RouterPoint[original.VertexCount];
            var resolved = new RouterPoint[original.VertexCount];
            for (uint idx = 1; idx < original.VertexCount + 1; idx++)
            { // resolve each vertex.
                float latitude, longitude;
                if (original.GetVertex(idx, out latitude, out longitude))
                {
                    resolvedReference[idx - 1] = referenceRouter.Resolve(VehicleEnum.Car, new GeoCoordinate(latitude, longitude));
                    resolved[idx - 1] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(latitude, longitude));
                }

                Assert.IsNotNull(resolvedReference[idx - 1]);
                Assert.IsNotNull(resolved[idx - 1]);

                Assert.AreEqual(resolvedReference[idx - 1].Location.Latitude,
                    resolved[idx - 1].Location.Latitude, 0.0001);
                Assert.AreEqual(resolvedReference[idx - 1].Location.Longitude,
                    resolved[idx - 1].Location.Longitude, 0.0001);
            }

            // check all the routes having the same weight(s).
            for (int fromIdx = 0; fromIdx < resolved.Length; fromIdx++)
            {
                for (int toIdx = 0; toIdx < resolved.Length; toIdx++)
                {
                    OsmSharpRoute referenceRoute = referenceRouter.Calculate(VehicleEnum.Car,
                        resolvedReference[fromIdx], resolvedReference[toIdx]);
                    OsmSharpRoute route = router.Calculate(VehicleEnum.Car,
                        resolved[fromIdx], resolved[toIdx]);

                    Assert.IsNotNull(referenceRoute);
                    Assert.IsNotNull(route);
                    //Assert.AreEqual(reference_route.TotalDistance, route.TotalDistance, 0.0001);
                    // TODO: meta data is missing in some CH routing; see issue 
                    //Assert.AreEqual(reference_route.TotalTime, route.TotalTime, 0.0001);
                }
            }
        }
    }
}
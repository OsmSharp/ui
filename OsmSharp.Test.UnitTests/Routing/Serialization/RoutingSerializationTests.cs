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

using NUnit.Framework;
using OsmSharp.Collections;
using OsmSharp.Collections.Coordinates;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.Serialization;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Graph.Serialization;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OsmSharp.Test.Unittests.Routing.Serialization
{
    /// <summary>
    /// Holds tests for the routing serialization.
    /// </summary>
    [TestFixture]
    public class RoutingSerializationTests
    {
        /// <summary>
        /// Tests serializing/deserializing using the routing serializer.
        /// </summary>
        [Test]
        public void RoutingSerializationRoutingTest()
        {
            const string embeddedString = "OsmSharp.Test.Unittests.test_network.osm";

            // create the tags index.
            var tagsIndex = new TagsTableCollectionIndex();

            // creates a new interpreter.
            var interpreter = new OsmRoutingInterpreter();

            // do the data processing.
            var original =
                new RouterDataSource<Edge>(new Graph<Edge>(), tagsIndex);
            var targetData = new GraphOsmStreamTarget(
                original, interpreter, tagsIndex, null, false);
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedString));
            targetData.RegisterSource(dataProcessorSource);
            targetData.Pull();

            // create serializer.
            var routingSerializer = new RoutingDataSourceSerializer();

            // serialize/deserialize.
            TagsCollectionBase metaData = new TagsCollection();
            metaData.Add("some_key", "some_value");
            byte[] byteArray;
            using (var stream = new MemoryStream())
            {
                try
                {
                    routingSerializer.Serialize(stream, original, metaData);
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

            var deserializedVersion = routingSerializer.Deserialize(new MemoryStream(byteArray), out metaData);
            Assert.AreEqual(original.TagsIndex.Get(0), deserializedVersion.TagsIndex.Get(0));

            // try to do some routing on the deserialized version.
            var basicRouter = new Dykstra();
            var router = Router.CreateFrom(deserializedVersion, basicRouter, interpreter);
            var source = router.Resolve(Vehicle.Car,
                new GeoCoordinate(51.0578532, 3.7192229));
            var target = router.Resolve(Vehicle.Car,
                new GeoCoordinate(51.0576193, 3.7191801));

            // calculate the route.
            var route = router.Calculate(Vehicle.Car, source, target);
            Assert.IsNotNull(route);
            Assert.AreEqual(5, route.Segments.Length);

            float latitude, longitude;
            //deserializedVersion.GetVertex(20, out latitude, out longitude);
            Assert.AreEqual(51.0578537, route.Segments[0].Latitude, 0.00001);
            Assert.AreEqual(3.71922255, route.Segments[0].Longitude, 0.00001);
            Assert.AreEqual(RouteSegmentType.Start, route.Segments[0].Type);

            //deserializedVersion.GetVertex(21, out latitude, out longitude);
            Assert.AreEqual(51.0578537, route.Segments[1].Latitude, 0.00001);
            Assert.AreEqual(3.71956515, route.Segments[1].Longitude, 0.00001);
            Assert.AreEqual(RouteSegmentType.Along, route.Segments[1].Type);

            //deserializedVersion.GetVertex(16, out latitude, out longitude);
            Assert.AreEqual(51.05773, route.Segments[2].Latitude, 0.00001);
            Assert.AreEqual(3.719745, route.Segments[2].Longitude, 0.00001);
            Assert.AreEqual(RouteSegmentType.Along, route.Segments[2].Type);

            //deserializedVersion.GetVertex(22, out latitude, out longitude);
            Assert.AreEqual(51.05762, route.Segments[3].Latitude, 0.00001);
            Assert.AreEqual(3.71965766, route.Segments[3].Longitude, 0.00001);
            Assert.AreEqual(RouteSegmentType.Along, route.Segments[3].Type);

            deserializedVersion.GetVertex(23, out latitude, out longitude);
            Assert.AreEqual(51.05762, route.Segments[4].Latitude, 0.00001);
            Assert.AreEqual(3.71917963, route.Segments[4].Longitude, 0.00001);
            Assert.AreEqual(RouteSegmentType.Stop, route.Segments[4].Type);
        }

        /// <summary>
        /// Tests serializing/deserializing RoutingSerializationRoutingComparisonTest using the routing serializer.
        /// </summary>
        [Test]
        public void RoutingSerializationRoutingComparisonTest()
        {
            const string embeddedString = "OsmSharp.Test.Unittests.test_network_real1.osm";

            // creates a new interpreter.
            var interpreter = new OsmRoutingInterpreter();

            // do the data processing.
            var original = GraphOsmStreamTarget.Preprocess(new XmlOsmStreamSource(
                                                                   Assembly.GetExecutingAssembly()
                                                                           .GetManifestResourceStream(embeddedString)),
                                                               interpreter);

            // create serializer.
            var routingSerializer = new RoutingDataSourceSerializer();

            // serialize/deserialize.
            TagsCollectionBase metaData = new TagsCollection();
            metaData.Add("some_key", "some_value");
            byte[] byteArray;
            using (var stream = new MemoryStream())
            {
                try
                {
                    routingSerializer.Serialize(stream, original, metaData);
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

            var deserializedVersion =
                routingSerializer.Deserialize(new MemoryStream(byteArray), out metaData);
            Assert.AreEqual(original.TagsIndex.Get(0), deserializedVersion.TagsIndex.Get(0));

            //            // try to do some routing on the deserialized version.
            //            var basicRouter =
            //                new Dykstra(deserializedVersion.TagsIndex);
            //            Router router = Router.CreateFrom(
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

        /// <summary>
        /// Tests serializing/deserializing RouterDataSource using the serializer.
        /// </summary>
        [Test]
        public void RoutingSerialization()
        {
            const string embeddedString = "OsmSharp.Test.Unittests.test_network_real1.osm";

            // load the network.
            var referenceNetwork = GraphOsmStreamTarget.Preprocess(new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedString)), new OsmRoutingInterpreter());

            // serialize network.
            var routingSerializer = new RoutingDataSourceSerializer();
            TagsCollectionBase metaData = new TagsCollection();
            metaData.Add("some_key", "some_value");
            using (var stream = new MemoryStream())
            {
                routingSerializer.Serialize(stream, referenceNetwork, metaData);

                stream.Seek(0, SeekOrigin.Begin);
                var network = routingSerializer.Deserialize(stream, out metaData);

                // compare networks.
                Assert.IsNotNull(network);
                Assert.AreEqual(referenceNetwork.VertexCount, network.VertexCount);
                for (uint vertex = 0; vertex < network.VertexCount; vertex++)
                {
                    float referenceLatitude, referenceLongitude, latitude, longitude;
                    Assert.IsTrue(referenceNetwork.GetVertex(vertex, out referenceLatitude, out referenceLongitude));
                    Assert.IsTrue(network.GetVertex(vertex, out latitude, out longitude));
                    Assert.AreEqual(referenceLatitude, latitude);
                    Assert.AreEqual(referenceLongitude, longitude);

                    var referenceArcs = referenceNetwork.GetEdges(vertex).ToKeyValuePairs();
                    var arcs = network.GetEdges(vertex).ToKeyValuePairs();
                    Assert.AreEqual(referenceArcs.Length, arcs.Length);
                    for (int idx = 0; idx < referenceArcs.Length; idx++)
                    {
                        var referenceArc = referenceArcs[idx];
                        // find the same edge in the new arcs.
                        var arc = arcs.First((x) => { return x.Key == referenceArcs[idx].Key; });

                        Assert.AreEqual(referenceArc.Key, arc.Key);
                        Assert.AreEqual(referenceArc.Value.Distance, arc.Value.Distance);
                        Assert.AreEqual(referenceArc.Value.Forward, arc.Value.Forward);
                        Assert.AreEqual(referenceArc.Value.RepresentsNeighbourRelations, arc.Value.RepresentsNeighbourRelations);
                        Assert.AreEqual(referenceArc.Value.Tags, arc.Value.Tags);
                        ICoordinateCollection referenceCoordinates;
                        ICoordinateCollection coordinates;
                        if (referenceNetwork.GetEdgeShape(vertex, referenceArc.Key, out referenceCoordinates))
                        { // there is a shape.
                            Assert.IsTrue(network.GetEdgeShape(vertex, arc.Key, out coordinates));
                            if (referenceCoordinates == null)
                            { // reference shape is null, shape is null.
                                Assert.IsNull(coordinates);
                            }
                            else
                            { // reference shape is not null compare them.
                                Assert.IsNotNull(coordinates);
                                referenceCoordinates.Reset();
                                coordinates.Reset();
                                while (referenceCoordinates.MoveNext())
                                {
                                    Assert.IsTrue(coordinates.MoveNext());

                                    Assert.AreEqual(referenceCoordinates.Latitude, coordinates.Latitude);
                                    Assert.AreEqual(referenceCoordinates.Longitude, coordinates.Longitude);
                                }
                                Assert.IsFalse(coordinates.MoveNext());
                            }
                        }
                        else
                        { // there is no shape.
                            Assert.IsFalse(network.GetEdgeShape(vertex, arc.Key, out coordinates));
                        }

                        // check tags.
                        var referenceTags = referenceNetwork.TagsIndex.Get(referenceArc.Value.Tags);
                        var tags = network.TagsIndex.Get(arc.Value.Tags);
                        if (referenceTags == null)
                        { // other tags also have to be null.
                            Assert.IsNull(tags);
                        }
                        else
                        { // contents need to be the same.
                            Assert.AreEqual(referenceTags.Count, tags.Count);
                            foreach (var referenceTag in referenceTags)
                            {
                                Assert.IsTrue(tags.ContainsKeyValue(referenceTag.Key, referenceTag.Value));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests serializing/deserializing RouterDataSource using the serializer for CHEdge data.
        /// </summary>
        [Test]
        public void RoutingSerializationCHEdgeData()
        {
            const string embeddedString = "OsmSharp.Test.Unittests.test_network_real1.osm";

            // load the network.
            var referenceNetwork = CHEdgeGraphOsmStreamTarget.Preprocess(new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedString)), new OsmRoutingInterpreter(), Vehicle.Car);

            // serialize network.
            var routingSerializer = new CHEdgeSerializer();
            TagsCollectionBase metaData = new TagsCollection();
            metaData.Add("some_key", "some_value");
            using (var stream = new MemoryStream())
            {
                routingSerializer.Serialize(stream, referenceNetwork, metaData);

                var network = routingSerializer.Deserialize(stream, out metaData);
                // compare networks.
                Assert.IsNotNull(network);
                Assert.AreEqual(referenceNetwork.VertexCount, network.VertexCount);
                for (uint vertex = 1; vertex < referenceNetwork.VertexCount; vertex++)
                {
                    float referenceLatitude, referenceLongitude, latitude, longitude;
                    Assert.IsTrue(referenceNetwork.GetVertex(vertex, out referenceLatitude, out referenceLongitude));
                    Assert.IsTrue(network.GetVertex(vertex, out latitude, out longitude));
                    Assert.AreEqual(referenceLatitude, latitude);
                    Assert.AreEqual(referenceLongitude, longitude);

                    var referenceArcs = referenceNetwork.GetEdges(vertex).ToKeyValuePairsAndShapes();
                    var arcs = network.GetEdges(vertex).ToKeyValuePairsAndShapes();
                    Assert.AreEqual(referenceArcs.Length, arcs.Length);
                    for (int idx = 0; idx < referenceArcs.Length; idx++)
                    {
                        var referenceArc = referenceArcs[idx];
                        // find the same edge in the new arcs.
                        var arc = arcs.First((x) => { return x.Key == referenceArcs[idx].Key && x.Value.Key.Equals(referenceArcs[idx].Value.Key); });

                        Assert.AreEqual(referenceArc.Key, arc.Key);
                        Assert.AreEqual(referenceArc.Value.Key.Meta, arc.Value.Key.Meta);
                        Assert.AreEqual(referenceArc.Value.Key.Value, arc.Value.Key.Value);
                        Assert.AreEqual(referenceArc.Value.Key.Weight, arc.Value.Key.Weight);
                        Assert.AreEqual(referenceArc.Value.Key.RepresentsNeighbourRelations, arc.Value.Key.RepresentsNeighbourRelations);
                        Assert.AreEqual(referenceArc.Value.Key.Tags, arc.Value.Key.Tags);
                        var referenceCoordinates = referenceArc.Value.Value;
                        var coordinates = arc.Value.Value;

                        if (referenceCoordinates == null)
                        { // reference shape is null, shape is null.
                            Assert.IsNull(coordinates);
                        }
                        else
                        { // reference shape is not null compare them.
                            Assert.IsNotNull(coordinates);
                            referenceCoordinates.Reset();
                            coordinates.Reset();
                            while (referenceCoordinates.MoveNext())
                            {
                                Assert.IsTrue(coordinates.MoveNext());

                                Assert.AreEqual(referenceCoordinates.Latitude, coordinates.Latitude);
                                Assert.AreEqual(referenceCoordinates.Longitude, coordinates.Longitude);
                            }
                            Assert.IsFalse(coordinates.MoveNext());
                        }

                        // check tags.
                        if (!referenceArc.Value.Key.IsContracted)
                        {
                            var referenceTags = referenceNetwork.TagsIndex.Get(referenceArc.Value.Key.Tags);
                            var tags = network.TagsIndex.Get(arc.Value.Key.Tags);
                            if (referenceTags == null)
                            { // other tags also have to be null.
                                Assert.IsNull(tags);
                            }
                            else
                            { // contents need to be the same.
                                Assert.AreEqual(referenceTags.Count, tags.Count);
                                foreach (var referenceTag in referenceTags)
                                {
                                    Assert.IsTrue(tags.ContainsKeyValue(referenceTag.Key, referenceTag.Value));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
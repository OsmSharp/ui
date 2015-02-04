// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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
using System.IO;
using OsmSharp.Osm.Xml.Streams;
using System.Reflection;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using OsmSharp.Routing;
using System.Diagnostics;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Test.Unittests.Routing.Serialization
{
    /// <summary>
    /// Tests comparing routing before/after serilization.
    /// </summary>
    [TestFixture]
    public class RoutingSerializationComparisonTests : RoutingComparisonTests
    {
        /// <summary>
        /// Tests serializing/deserializing RoutingSerializationRoutingComparisonTest using the V1 routing serializer.
        /// </summary>
        [Test]
        public void RoutingSerializationV2CHRoutingComparisonTest()
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
            var routingSerializer = new OsmSharp.Routing.CH.Serialization.Sorted.CHEdgeDataDataSourceSerializer();

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

            IBasicRouterDataSource<CHEdgeData> deserializedVersion =
                routingSerializer.Deserialize(new MemoryStream(byteArray), out metaData);
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

            //this.TestCompareAll(original, referenceRouter, router);
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
            throw new NotImplementedException("Not needed here!");
        }
    }
}

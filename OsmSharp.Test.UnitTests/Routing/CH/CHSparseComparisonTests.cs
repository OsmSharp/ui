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

using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;

namespace OsmSharp.Test.Unittests.Routing.CH
{
    /// <summary>
    /// Tests the CH Sparse routing against a reference implementation.
    /// </summary>
    [TestFixture]
    public class CHSparseComparisonTests : RoutingComparisonTests
    {
        /// <summary>
        /// Holds the data.
        /// </summary>
        private Dictionary<string, DynamicGraphRouterDataSource<CHEdgeData>> _data = null;

        /// <summary>
        /// Returns a new router.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="embeddedName"></param>
        /// <returns></returns>
        public override Router BuildRouter(IOsmRoutingInterpreter interpreter, string embeddedName, bool contract)
        {
            if (_data == null)
            {
                _data = new Dictionary<string, DynamicGraphRouterDataSource<CHEdgeData>>();
            }
            DynamicGraphRouterDataSource<CHEdgeData> data = null;
            if (!_data.TryGetValue(embeddedName, out data))
            {
                var tagsIndex = new SimpleTagsIndex();

                // do the data processing.
                data =
                    new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
                var targetData = new CHEdgeGraphOsmStreamTarget(
                    data, interpreter, data.TagsIndex, Vehicle.Car);
                var dataProcessorSource = new XmlOsmStreamSource(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(
                    "OsmSharp.Test.Unittests.{0}", embeddedName)));
                var sorter = new OsmStreamFilterSort();
                sorter.RegisterSource(dataProcessorSource);
                targetData.RegisterSource(sorter);
                targetData.Pull();

                // do the pre-processing part.
                var preProcessor = new CHPreProcessor(data,
                    new SparseOrdering(data), new DykstraWitnessCalculator());
                preProcessor.Start();

                _data[embeddedName] = data;
            }
            return Router.CreateCHFrom(data, new CHRouter(), interpreter);
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [Test]
        public void TestCHSparseAgainstReference()
        {
            this.TestCompareAll("test_network.osm", true);
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [Test]
        public void TestCHSparseOneWayAgainstReference()
        {
            this.TestCompareAll("test_network_oneway.osm", true);
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [Test]
        public void TestCHSparseAgainstReferenceRealNetwork()
        {
            this.TestCompareAll("test_network_real1.osm", true);
        }
    }
}
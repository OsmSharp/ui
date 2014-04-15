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

using System.Reflection;
using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using OsmSharp.Collections.Tags.Index;

namespace OsmSharp.Test.Unittests.Routing.CH
{
    /// <summary>
    /// Tests the EdgeDifference calculator.
    /// </summary>
    [TestFixture]
    public class CHEdgeDifferenceTests
    {
        /// <summary>
        /// Builds the data source.
        /// </summary>
        /// <returns></returns>
        private DynamicGraphRouterDataSource<CHEdgeData> BuildData(IOsmRoutingInterpreter interpreter)
        {
            DynamicGraphRouterDataSource<CHEdgeData> data = null;
            if (data == null)
            {
                var tagsIndex = new TagsTableCollectionIndex();

                // do the data processing.
                data = new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
                var targetData = new CHEdgeGraphOsmStreamTarget(
                    data, interpreter, tagsIndex, Vehicle.Car);
                var dataProcessorSource = new XmlOsmStreamSource(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.Unittests.test_network.osm"));
                var sorter = new OsmStreamFilterSort();
                sorter.RegisterSource(dataProcessorSource);
                targetData.RegisterSource(sorter);
                targetData.Pull();
            }
            return data;
        }

        /// <summary>
        /// Builds the edge difference.
        /// </summary>
        private EdgeDifference BuildEdgeDifference(IOsmRoutingInterpreter interpreter)
        {
            DynamicGraphRouterDataSource<CHEdgeData> data = this.BuildData(interpreter);

            // do the pre-processing part.
            INodeWitnessCalculator witnessCalculator = new DykstraWitnessCalculator();
            return new EdgeDifference(
                data, witnessCalculator);
        }

        /// <summary>
        /// Builds the edge difference.
        /// </summary>
        private CHPreProcessor BuildCHPreProcessor(IOsmRoutingInterpreter interpreter)
        {
            DynamicGraphRouterDataSource<CHEdgeData> data = this.BuildData(interpreter);

            // do the pre-processing part.
            INodeWitnessCalculator witnessCalculator = new DykstraWitnessCalculator();
            var edgeDifference = new EdgeDifference(
                data, witnessCalculator);

            var preProcessor = new CHPreProcessor(
                data, edgeDifference, witnessCalculator);
            return preProcessor;
        }

        /// <summary>
        /// Tests all the edge difference calculations on the uncontracted test network.
        /// </summary>
        [Test]
        public void TestCHEdgeDifferenceNonContracted()
        {
            var interpreter = new OsmSharp.Routing.Osm.Interpreter.OsmRoutingInterpreter();
            var edgeDifference = this.BuildEdgeDifference(interpreter);

            Assert.AreEqual(5, edgeDifference.Calculate(1)); // witness paths from 2<->4.
            Assert.AreEqual(16, edgeDifference.Calculate(2));
            Assert.AreEqual(2, edgeDifference.Calculate(3));
            Assert.AreEqual(2, edgeDifference.Calculate(4));
            Assert.AreEqual(2, edgeDifference.Calculate(5));
            Assert.AreEqual(2, edgeDifference.Calculate(6));
            Assert.AreEqual(9, edgeDifference.Calculate(7));
            Assert.AreEqual(9, edgeDifference.Calculate(8));
            Assert.AreEqual(2, edgeDifference.Calculate(9));
            Assert.AreEqual(9, edgeDifference.Calculate(10));
            Assert.AreEqual(9, edgeDifference.Calculate(11));
            Assert.AreEqual(2, edgeDifference.Calculate(12));
            Assert.AreEqual(2, edgeDifference.Calculate(13));
            Assert.AreEqual(2, edgeDifference.Calculate(14));
            Assert.AreEqual(2, edgeDifference.Calculate(15));
            Assert.AreEqual(9, edgeDifference.Calculate(16));
            Assert.AreEqual(2, edgeDifference.Calculate(17));
            Assert.AreEqual(2, edgeDifference.Calculate(18));
            Assert.AreEqual(2, edgeDifference.Calculate(19));
            Assert.AreEqual(-1, edgeDifference.Calculate(20));
            Assert.AreEqual(2, edgeDifference.Calculate(21));
            Assert.AreEqual(2, edgeDifference.Calculate(22));
            Assert.AreEqual(-1, edgeDifference.Calculate(23));
        }

        /// <summary>
        /// Tests all the edge difference calculations on the uncontracted test network.
        /// </summary>
        [Test]
        public void TestCHEdgeDifferenceContractions()
        {
            var interpreter = new OsmSharp.Routing.Osm.Interpreter.OsmRoutingInterpreter();
            var processor = this.BuildCHPreProcessor(interpreter);
            var edgeDifference = processor.NodeWeightCalculator;

            Assert.AreEqual(5, edgeDifference.Calculate(1)); // witness paths from 2<->4.
            Assert.AreEqual(16, edgeDifference.Calculate(2));
            Assert.AreEqual(2, edgeDifference.Calculate(3));
            Assert.AreEqual(2, edgeDifference.Calculate(4));
            Assert.AreEqual(2, edgeDifference.Calculate(5));
            Assert.AreEqual(2, edgeDifference.Calculate(6));
            Assert.AreEqual(9, edgeDifference.Calculate(7));
            Assert.AreEqual(9, edgeDifference.Calculate(8));
            Assert.AreEqual(2, edgeDifference.Calculate(9));
            Assert.AreEqual(9, edgeDifference.Calculate(10));
            Assert.AreEqual(9, edgeDifference.Calculate(11));
            Assert.AreEqual(2, edgeDifference.Calculate(12));
            Assert.AreEqual(2, edgeDifference.Calculate(13));
            Assert.AreEqual(2, edgeDifference.Calculate(14));
            Assert.AreEqual(2, edgeDifference.Calculate(15));
            Assert.AreEqual(9, edgeDifference.Calculate(16));
            Assert.AreEqual(2, edgeDifference.Calculate(17));
            Assert.AreEqual(2, edgeDifference.Calculate(18));
            Assert.AreEqual(2, edgeDifference.Calculate(19));
            Assert.AreEqual(-1, edgeDifference.Calculate(20));
            Assert.AreEqual(2, edgeDifference.Calculate(21));
            Assert.AreEqual(2, edgeDifference.Calculate(22));
            Assert.AreEqual(-1, edgeDifference.Calculate(23));

            // contract 20.
            processor.Contract(20);

            Assert.AreEqual(5, edgeDifference.Calculate(1)); // witness paths from 2<->4.
            Assert.AreEqual(16, edgeDifference.Calculate(2));
            Assert.AreEqual(2, edgeDifference.Calculate(3));
            Assert.AreEqual(2, edgeDifference.Calculate(4));
            Assert.AreEqual(2, edgeDifference.Calculate(5));
            Assert.AreEqual(2, edgeDifference.Calculate(6));
            Assert.AreEqual(9, edgeDifference.Calculate(7));
            Assert.AreEqual(9, edgeDifference.Calculate(8));
            Assert.AreEqual(2, edgeDifference.Calculate(9));
            Assert.AreEqual(9, edgeDifference.Calculate(10));
            Assert.AreEqual(9, edgeDifference.Calculate(11));
            Assert.AreEqual(2, edgeDifference.Calculate(12));
            Assert.AreEqual(2, edgeDifference.Calculate(13));
            Assert.AreEqual(2, edgeDifference.Calculate(14));
            Assert.AreEqual(2, edgeDifference.Calculate(15));
            Assert.AreEqual(9, edgeDifference.Calculate(16));
            Assert.AreEqual(2, edgeDifference.Calculate(17));
            Assert.AreEqual(2, edgeDifference.Calculate(18));
            Assert.AreEqual(2, edgeDifference.Calculate(19));
            Assert.AreEqual(-1, edgeDifference.Calculate(21));
            Assert.AreEqual(2, edgeDifference.Calculate(22));
            Assert.AreEqual(-1, edgeDifference.Calculate(23));

            // contract 21.
            processor.Contract(21);

            Assert.AreEqual(5, edgeDifference.Calculate(1)); // witness paths from 2<->4.
            Assert.AreEqual(16, edgeDifference.Calculate(2));
            Assert.AreEqual(2, edgeDifference.Calculate(3));
            Assert.AreEqual(2, edgeDifference.Calculate(4));
            Assert.AreEqual(2, edgeDifference.Calculate(5));
            Assert.AreEqual(2, edgeDifference.Calculate(6));
            Assert.AreEqual(9, edgeDifference.Calculate(7));
            Assert.AreEqual(9, edgeDifference.Calculate(8));
            Assert.AreEqual(2, edgeDifference.Calculate(9));
            Assert.AreEqual(9, edgeDifference.Calculate(10));
            Assert.AreEqual(9, edgeDifference.Calculate(11));
            Assert.AreEqual(2, edgeDifference.Calculate(12));
            Assert.AreEqual(2, edgeDifference.Calculate(13));
            Assert.AreEqual(2, edgeDifference.Calculate(14));
            Assert.AreEqual(2, edgeDifference.Calculate(15));
            Assert.AreEqual(2, edgeDifference.Calculate(16));
            Assert.AreEqual(2, edgeDifference.Calculate(17));
            Assert.AreEqual(2, edgeDifference.Calculate(18));
            Assert.AreEqual(2, edgeDifference.Calculate(19));
            Assert.AreEqual(2, edgeDifference.Calculate(22));
            Assert.AreEqual(-1, edgeDifference.Calculate(23));

            // contract 23.
            processor.Contract(23);

            Assert.AreEqual(5, edgeDifference.Calculate(1)); // witness paths from 2<->4.
            Assert.AreEqual(16, edgeDifference.Calculate(2));
            Assert.AreEqual(2, edgeDifference.Calculate(3));
            Assert.AreEqual(2, edgeDifference.Calculate(4));
            Assert.AreEqual(2, edgeDifference.Calculate(5));
            Assert.AreEqual(2, edgeDifference.Calculate(6));
            Assert.AreEqual(9, edgeDifference.Calculate(7));
            Assert.AreEqual(9, edgeDifference.Calculate(8));
            Assert.AreEqual(2, edgeDifference.Calculate(9));
            Assert.AreEqual(9, edgeDifference.Calculate(10));
            Assert.AreEqual(9, edgeDifference.Calculate(11));
            Assert.AreEqual(2, edgeDifference.Calculate(12));
            Assert.AreEqual(2, edgeDifference.Calculate(13));
            Assert.AreEqual(2, edgeDifference.Calculate(14));
            Assert.AreEqual(2, edgeDifference.Calculate(15));
            Assert.AreEqual(2, edgeDifference.Calculate(16));
            Assert.AreEqual(2, edgeDifference.Calculate(17));
            Assert.AreEqual(2, edgeDifference.Calculate(18));
            Assert.AreEqual(2, edgeDifference.Calculate(19));
            Assert.AreEqual(-1, edgeDifference.Calculate(22));

            // contract 22.
            processor.Contract(22);

            Assert.AreEqual(5, edgeDifference.Calculate(1)); // witness paths from 2<->4.
            Assert.AreEqual(16, edgeDifference.Calculate(2));
            Assert.AreEqual(2, edgeDifference.Calculate(3));
            Assert.AreEqual(2, edgeDifference.Calculate(4));
            Assert.AreEqual(2, edgeDifference.Calculate(5));
            Assert.AreEqual(2, edgeDifference.Calculate(6));
            Assert.AreEqual(9, edgeDifference.Calculate(7));
            Assert.AreEqual(9, edgeDifference.Calculate(8));
            Assert.AreEqual(2, edgeDifference.Calculate(9));
            Assert.AreEqual(9, edgeDifference.Calculate(10));
            Assert.AreEqual(9, edgeDifference.Calculate(11));
            Assert.AreEqual(2, edgeDifference.Calculate(12));
            Assert.AreEqual(2, edgeDifference.Calculate(13));
            Assert.AreEqual(2, edgeDifference.Calculate(14));
            Assert.AreEqual(2, edgeDifference.Calculate(15));
            Assert.AreEqual(-1, edgeDifference.Calculate(16));
            Assert.AreEqual(2, edgeDifference.Calculate(17));
            Assert.AreEqual(2, edgeDifference.Calculate(18));
            Assert.AreEqual(2, edgeDifference.Calculate(19));

            // contract 16.
            processor.Contract(16);

            Assert.AreEqual(5, edgeDifference.Calculate(1)); // witness paths from 2<->4.
            Assert.AreEqual(16, edgeDifference.Calculate(2));
            Assert.AreEqual(2, edgeDifference.Calculate(3));
            Assert.AreEqual(2, edgeDifference.Calculate(4));
            Assert.AreEqual(2, edgeDifference.Calculate(5));
            Assert.AreEqual(2, edgeDifference.Calculate(6));
            Assert.AreEqual(9, edgeDifference.Calculate(7));
            Assert.AreEqual(9, edgeDifference.Calculate(8));
            Assert.AreEqual(2, edgeDifference.Calculate(9));
            Assert.AreEqual(9, edgeDifference.Calculate(10));
            Assert.AreEqual(9, edgeDifference.Calculate(11));
            Assert.AreEqual(2, edgeDifference.Calculate(12));
            Assert.AreEqual(2, edgeDifference.Calculate(13));
            Assert.AreEqual(2, edgeDifference.Calculate(14));
            Assert.AreEqual(2, edgeDifference.Calculate(15));
            Assert.AreEqual(-1, edgeDifference.Calculate(17));
            Assert.AreEqual(2, edgeDifference.Calculate(18));
            Assert.AreEqual(2, edgeDifference.Calculate(19));

            // contract 17.
            processor.Contract(17);

            Assert.AreEqual(5, edgeDifference.Calculate(1)); // witness paths from 2<->4.
            Assert.AreEqual(16, edgeDifference.Calculate(2));
            Assert.AreEqual(2, edgeDifference.Calculate(3));
            Assert.AreEqual(2, edgeDifference.Calculate(4));
            Assert.AreEqual(2, edgeDifference.Calculate(5));
            Assert.AreEqual(2, edgeDifference.Calculate(6));
            Assert.AreEqual(2, edgeDifference.Calculate(7));
            Assert.AreEqual(9, edgeDifference.Calculate(8));
            Assert.AreEqual(2, edgeDifference.Calculate(9));
            Assert.AreEqual(9, edgeDifference.Calculate(10));
            Assert.AreEqual(9, edgeDifference.Calculate(11));
            Assert.AreEqual(2, edgeDifference.Calculate(12));
            Assert.AreEqual(2, edgeDifference.Calculate(13));
            Assert.AreEqual(2, edgeDifference.Calculate(14));
            Assert.AreEqual(2, edgeDifference.Calculate(15));
            Assert.AreEqual(2, edgeDifference.Calculate(18));
            Assert.AreEqual(2, edgeDifference.Calculate(19));

            processor.Contract(3);

            Assert.AreEqual(5, edgeDifference.Calculate(1)); // witness paths from 2<->4.
            Assert.AreEqual(16, edgeDifference.Calculate(2));
            Assert.AreEqual(-2, edgeDifference.Calculate(4));
            Assert.AreEqual(2, edgeDifference.Calculate(5));
            Assert.AreEqual(2, edgeDifference.Calculate(6));
            Assert.AreEqual(2, edgeDifference.Calculate(7));
            Assert.AreEqual(9, edgeDifference.Calculate(8));
            Assert.AreEqual(2, edgeDifference.Calculate(9));
            Assert.AreEqual(9, edgeDifference.Calculate(10));
            Assert.AreEqual(9, edgeDifference.Calculate(11));
            Assert.AreEqual(2, edgeDifference.Calculate(12));
            Assert.AreEqual(2, edgeDifference.Calculate(13));
            Assert.AreEqual(2, edgeDifference.Calculate(14));
            Assert.AreEqual(2, edgeDifference.Calculate(15));
            Assert.AreEqual(2, edgeDifference.Calculate(18));
            Assert.AreEqual(2, edgeDifference.Calculate(19));

            processor.Contract(4);

            Assert.AreEqual(2, edgeDifference.Calculate(1)); // witness paths from 2<->4.
            Assert.AreEqual(9, edgeDifference.Calculate(2));
            Assert.AreEqual(2, edgeDifference.Calculate(5));
            Assert.AreEqual(2, edgeDifference.Calculate(6));
            Assert.AreEqual(2, edgeDifference.Calculate(7));
            Assert.AreEqual(9, edgeDifference.Calculate(8));
            Assert.AreEqual(2, edgeDifference.Calculate(9));
            Assert.AreEqual(9, edgeDifference.Calculate(10));
            Assert.AreEqual(9, edgeDifference.Calculate(11));
            Assert.AreEqual(2, edgeDifference.Calculate(12));
            Assert.AreEqual(2, edgeDifference.Calculate(13));
            Assert.AreEqual(2, edgeDifference.Calculate(14));
            Assert.AreEqual(2, edgeDifference.Calculate(15));
            Assert.AreEqual(2, edgeDifference.Calculate(18));
            Assert.AreEqual(2, edgeDifference.Calculate(19));

            processor.Contract(1);

            Assert.AreEqual(5, edgeDifference.Calculate(2));
            Assert.AreEqual(-2, edgeDifference.Calculate(5));
            Assert.AreEqual(2, edgeDifference.Calculate(6));
            Assert.AreEqual(2, edgeDifference.Calculate(7));
            Assert.AreEqual(9, edgeDifference.Calculate(8));
            Assert.AreEqual(2, edgeDifference.Calculate(9));
            Assert.AreEqual(9, edgeDifference.Calculate(10));
            Assert.AreEqual(9, edgeDifference.Calculate(11));
            Assert.AreEqual(2, edgeDifference.Calculate(12));
            Assert.AreEqual(2, edgeDifference.Calculate(13));
            Assert.AreEqual(2, edgeDifference.Calculate(14));
            Assert.AreEqual(2, edgeDifference.Calculate(15));
            Assert.AreEqual(2, edgeDifference.Calculate(18));
            Assert.AreEqual(2, edgeDifference.Calculate(19));

            processor.Contract(5);

            Assert.AreEqual(1, edgeDifference.Calculate(2)); // witness paths from 11<->5.
            Assert.AreEqual(-2, edgeDifference.Calculate(6));
            Assert.AreEqual(0, edgeDifference.Calculate(7));
            Assert.AreEqual(3, edgeDifference.Calculate(8));
            Assert.AreEqual(0, edgeDifference.Calculate(9));
            Assert.AreEqual(3, edgeDifference.Calculate(10));
            Assert.AreEqual(3, edgeDifference.Calculate(11));
            Assert.AreEqual(0, edgeDifference.Calculate(12));
            Assert.AreEqual(0, edgeDifference.Calculate(13));
            Assert.AreEqual(0, edgeDifference.Calculate(14));
            Assert.AreEqual(0, edgeDifference.Calculate(15));
            Assert.AreEqual(0, edgeDifference.Calculate(18));
            Assert.AreEqual(0, edgeDifference.Calculate(19));

            processor.Contract(6);

            Assert.AreEqual(0, edgeDifference.Calculate(2)); // witness paths from 11<->5.
            Assert.AreEqual(-1, edgeDifference.Calculate(7));
            Assert.AreEqual(3, edgeDifference.Calculate(8));
            Assert.AreEqual(0, edgeDifference.Calculate(9));
            Assert.AreEqual(3, edgeDifference.Calculate(10));
            Assert.AreEqual(3, edgeDifference.Calculate(11));
            Assert.AreEqual(0, edgeDifference.Calculate(12));
            Assert.AreEqual(0, edgeDifference.Calculate(13));
            Assert.AreEqual(0, edgeDifference.Calculate(14));
            Assert.AreEqual(0, edgeDifference.Calculate(15));
            Assert.AreEqual(0, edgeDifference.Calculate(18));
            Assert.AreEqual(0, edgeDifference.Calculate(19));

            processor.Contract(7);

            Assert.AreEqual(0, edgeDifference.Calculate(2)); // witness paths from 11<->5.
            Assert.AreEqual(3, edgeDifference.Calculate(8));
            Assert.AreEqual(0, edgeDifference.Calculate(9));
            Assert.AreEqual(3, edgeDifference.Calculate(10));
            Assert.AreEqual(0, edgeDifference.Calculate(11));
            Assert.AreEqual(0, edgeDifference.Calculate(12));
            Assert.AreEqual(0, edgeDifference.Calculate(13));
            Assert.AreEqual(0, edgeDifference.Calculate(14));
            Assert.AreEqual(0, edgeDifference.Calculate(15));
            Assert.AreEqual(0, edgeDifference.Calculate(18));
            Assert.AreEqual(0, edgeDifference.Calculate(19));

            processor.Contract(2);

            Assert.AreEqual(3, edgeDifference.Calculate(8));
            Assert.AreEqual(0, edgeDifference.Calculate(9));
            Assert.AreEqual(3, edgeDifference.Calculate(10));
            Assert.AreEqual(-2, edgeDifference.Calculate(11)); // witness paths from 18<->10.
            Assert.AreEqual(0, edgeDifference.Calculate(12));
            Assert.AreEqual(0, edgeDifference.Calculate(13));
            Assert.AreEqual(0, edgeDifference.Calculate(14));
            Assert.AreEqual(0, edgeDifference.Calculate(15));
            Assert.AreEqual(0, edgeDifference.Calculate(18));
            Assert.AreEqual(0, edgeDifference.Calculate(19));

            processor.Contract(11);

            Assert.AreEqual(3, edgeDifference.Calculate(8));
            Assert.AreEqual(0, edgeDifference.Calculate(9));
            Assert.AreEqual(1, edgeDifference.Calculate(10));
            Assert.AreEqual(0, edgeDifference.Calculate(12));
            Assert.AreEqual(0, edgeDifference.Calculate(13));
            Assert.AreEqual(0, edgeDifference.Calculate(14));
            Assert.AreEqual(0, edgeDifference.Calculate(15));
            Assert.AreEqual(-2, edgeDifference.Calculate(18)); // witness paths from 10<->8.
            Assert.AreEqual(0, edgeDifference.Calculate(19));

            processor.Contract(18);

            Assert.AreEqual(0, edgeDifference.Calculate(8));
            Assert.AreEqual(0, edgeDifference.Calculate(9));
            Assert.AreEqual(0, edgeDifference.Calculate(10));
            Assert.AreEqual(0, edgeDifference.Calculate(12));
            Assert.AreEqual(0, edgeDifference.Calculate(13));
            Assert.AreEqual(0, edgeDifference.Calculate(14));
            Assert.AreEqual(0, edgeDifference.Calculate(15));
            Assert.AreEqual(0, edgeDifference.Calculate(19));

            processor.Contract(8);

            Assert.AreEqual(0, edgeDifference.Calculate(9));
            Assert.AreEqual(0, edgeDifference.Calculate(10));
            Assert.AreEqual(0, edgeDifference.Calculate(12));
            Assert.AreEqual(0, edgeDifference.Calculate(13));
            Assert.AreEqual(0, edgeDifference.Calculate(14));
            Assert.AreEqual(0, edgeDifference.Calculate(15));
            Assert.AreEqual(0, edgeDifference.Calculate(19));

            processor.Contract(9);

            Assert.AreEqual(-2, edgeDifference.Calculate(10)); // witness paths from 19<->12.
            Assert.AreEqual(0, edgeDifference.Calculate(12));
            Assert.AreEqual(0, edgeDifference.Calculate(13));
            Assert.AreEqual(0, edgeDifference.Calculate(14));
            Assert.AreEqual(0, edgeDifference.Calculate(15));
            Assert.AreEqual(-2, edgeDifference.Calculate(19)); // witness paths from 15<->10.

            processor.Contract(10);

            Assert.AreEqual(-2, edgeDifference.Calculate(12));
            Assert.AreEqual(0, edgeDifference.Calculate(13));
            Assert.AreEqual(0, edgeDifference.Calculate(14));
            Assert.AreEqual(0, edgeDifference.Calculate(15));
            Assert.AreEqual(-2, edgeDifference.Calculate(19)); // witness paths from 15<->10.

            processor.Contract(12);

            Assert.AreEqual(-1, edgeDifference.Calculate(13));
            Assert.AreEqual(0, edgeDifference.Calculate(14));
            Assert.AreEqual(0, edgeDifference.Calculate(15));
            Assert.AreEqual(-1, edgeDifference.Calculate(19)); // witness paths from 15<->10.

            processor.Contract(13);

            Assert.AreEqual(-1, edgeDifference.Calculate(14));
            Assert.AreEqual(0, edgeDifference.Calculate(15));
            Assert.AreEqual(-1, edgeDifference.Calculate(19)); // witness paths from 15<->10.

            processor.Contract(14);

            Assert.AreEqual(-1, edgeDifference.Calculate(15));
            Assert.AreEqual(-1, edgeDifference.Calculate(19)); // witness paths from 15<->10.

            processor.Contract(15);

            Assert.AreEqual(0, edgeDifference.Calculate(19));

            processor.Contract(19);
        }
    }
}
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
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;

namespace OsmSharp.Test.Unittests.Routing.CH
{
    /// <summary>
    /// Executes the CH contractions while verifying each step.
    /// </summary>
    [TestFixture]
    public class CHVerifiedContractionBaseTests
    {
        /// <summary>
        /// Executes the tests.
        /// </summary>
        public static void Execute()
        {
            //CHVerifiedContractionBaseTests.ExecuteSparse("OsmSharp.Routing.Osm.Test.TestData.matrix.osm");
            CHVerifiedContractionBaseTests.ExecuteEdgeDifference("OsmSharp.Routing.Osm.Test.TestData.matrix.osm");
            //CHVerifiedContractionBaseTests.Execute("OsmSharp.Routing.Osm.Test.TestData.matrix_big_area.osm");
            //CHVerifiedContractionBaseTests.Execute("OsmSharp.Routing.Osm.Test.TestData.lebbeke.osm");
            //CHVerifiedContractionBaseTests.Execute("OsmSharp.Routing.Osm.Test.TestData.eeklo.osm");
            //CHVerifiedContractionBaseTests.Execute("OsmSharp.Routing.Osm.Test.TestData.moscow.osm");
        }

        /// <summary>
        /// Executes the tests.
        /// </summary>
        /// <param name="xml"></param>
        private static void ExecuteEdgeDifference(string xml)
        {
            CHVerifiedContractionBaseTests tester = new CHVerifiedContractionBaseTests();
            tester.DoTestCHEdgeDifferenceVerifiedContraction(xml);
        }

        /// <summary>
        /// Executes the tests.
        /// </summary>
        /// <param name="xml"></param>
        private static void ExecuteSparse(string xml)
        {
            CHVerifiedContractionBaseTests tester = new CHVerifiedContractionBaseTests();
            tester.DoTestCHSparseVerifiedContraction(xml);
        }

        #region Testing Code

        /// <summary>
        /// Holds the data.
        /// </summary>
        private DynamicGraphRouterDataSource<CHEdgeData> _data;

        /// <summary>
        /// Holds the interpreter.
        /// </summary>
        private IOsmRoutingInterpreter _interpreter;

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        /// <param name="xml"></param>
        private void DoTestCHSparseVerifiedContraction(string xml)
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(xml));
        }

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        /// <param name="stream"></param>
        public void DoTestCHSparseVerifiedContraction(Stream stream)
        {
            _interpreter = new OsmRoutingInterpreter();

            var tagsIndex = new SimpleTagsIndex();

            // do the data processing.
            _data = new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
            var targetData = new CHEdgeGraphOsmStreamTarget(
                _data, _interpreter, _data.TagsIndex, Vehicle.Car);
            var dataProcessorSource = new XmlOsmStreamSource(stream);
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            // do the pre-processing part.
            //INodeWitnessCalculator witness_calculator = new CHRouterWitnessCalculator(_data);
            INodeWitnessCalculator witnessCalculator = new DykstraWitnessCalculator(_data);
            var preProcessor = new CHPreProcessor(_data,
                new EdgeDifferenceContractedSearchSpace(_data, witnessCalculator), witnessCalculator);
            preProcessor.OnBeforeContractionEvent += new CHPreProcessor.VertexDelegate(pre_processor_OnBeforeContractionEvent);
            preProcessor.OnAfterContractionEvent += new CHPreProcessor.VertexDelegate(pre_processor_OnAfterContractionEvent);
            preProcessor.Start();
        }

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        /// <param name="xml"></param>
        private void DoTestCHEdgeDifferenceVerifiedContraction(string xml)
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(xml));
        }

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        /// <param name="stream"></param>
        public void DoTestCHEdgeDifferenceVerifiedContraction(Stream stream)
        {
            _interpreter = new OsmRoutingInterpreter();

            var tagsIndex = new SimpleTagsIndex();

            // do the data processing.
            _data = new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
            var targetData = new CHEdgeGraphOsmStreamTarget(
                _data, _interpreter, _data.TagsIndex, Vehicle.Car);
            var dataProcessorSource = new XmlOsmStreamSource(stream);
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            // do the pre-processing part.
            var witnessCalculator = new DykstraWitnessCalculator(
                _data);
            var preProcessor = new CHPreProcessor(_data,
                new EdgeDifference(_data, witnessCalculator), witnessCalculator);
            preProcessor.OnBeforeContractionEvent += 
                new CHPreProcessor.VertexDelegate(pre_processor_OnBeforeContractionEvent);
            preProcessor.OnAfterContractionEvent += 
                new CHPreProcessor.VertexDelegate(pre_processor_OnAfterContractionEvent);
            preProcessor.Start();
        }

        /// <summary>
        /// Holds the paths calculate before contraction.
        /// </summary>
        private Dictionary<uint, Dictionary<uint, PathSegment<long>>> _pathsBeforeContraction;

        /// <summary>
        /// Called right after the contraction.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edges"></param>
        void pre_processor_OnAfterContractionEvent(uint vertex, KeyValuePair<uint, CHEdgeData>[] edges)
        {
            // create a new CHRouter
            var router = new CHRouter(_data);

            // calculate all the routes between the neighbours of the contracted vertex.
            foreach (KeyValuePair<uint, CHEdgeData> from in edges)
            {
                // initialize the from-list.
                var fromList = new PathSegmentVisitList();
                fromList.UpdateVertex(new PathSegment<long>(from.Key));

                // initalize the from dictionary.
                Dictionary<uint, PathSegment<long>> fromDic = _pathsBeforeContraction[from.Key];
                foreach (KeyValuePair<uint, CHEdgeData> to in edges)
                {
                    // initialize the to-list.
                    var toList = new PathSegmentVisitList();
                    toList.UpdateVertex(new PathSegment<long>(to.Key));

                    // calculate the route.
                    PathSegment<long> route = router.Calculate(_data, _interpreter, OsmSharp.Routing.Vehicle.Car, fromList, toList, double.MaxValue);
                    if ((fromDic[to.Key] == null && route != null) ||
                        (fromDic[to.Key] != null && route == null) ||
                        ((fromDic[to.Key] != null && route != null) && fromDic[to.Key] != route))
                    { // the route match!
                        Assert.Fail("Routes are different before/after contraction!");
                    }
                }
            }
        }

        /// <summary>
        /// Called left before the contraction.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edges"></param>
        void pre_processor_OnBeforeContractionEvent(uint vertex, KeyValuePair<uint, CHEdgeData>[] edges)
        {
            // create a new CHRouter
            var router = new CHRouter(_data);

            // calculate all the routes between the neighbours of the contracted vertex.
            _pathsBeforeContraction =
                new Dictionary<uint, Dictionary<uint, PathSegment<long>>>();
            foreach (KeyValuePair<uint, CHEdgeData> from in edges)
            {
                // initialize the from-list.
                var fromList = new PathSegmentVisitList();
                fromList.UpdateVertex(new PathSegment<long>(from.Key));

                // initalize the from dictionary.
                var fromDic = new Dictionary<uint, PathSegment<long>>();
                _pathsBeforeContraction[from.Key] = fromDic;
                foreach (KeyValuePair<uint, CHEdgeData> to in edges)
                {
                    // initialize the to-list.
                    var toList = new PathSegmentVisitList();
                    toList.UpdateVertex(new PathSegment<long>(to.Key));

                    // calculate the route.
                    PathSegment<long> route = router.Calculate(_data, _interpreter, 
                        OsmSharp.Routing.Vehicle.Car, fromList, toList, double.MaxValue);
                    fromDic[to.Key] = route;
                }
            }
        }

        #endregion
    }
}
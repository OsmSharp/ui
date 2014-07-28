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
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
        /// <param name="xml"></param>
        private static void ExecuteEdgeDifference(string xml)
        {
            CHVerifiedContractionBaseTests tester = new CHVerifiedContractionBaseTests();
            tester.DoTestCHEdgeDifferenceVerifiedContraction(xml, false);
        }

        /// <summary>
        /// Executes the tests.
        /// </summary>
        /// <param name="xml"></param>
        private static void ExecuteSparse(string xml)
        {
            CHVerifiedContractionBaseTests tester = new CHVerifiedContractionBaseTests();
            tester.DoTestCHSparseVerifiedContraction(xml, false);
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
        /// Holds the reference router.
        /// </summary>
        private Router _referenceRouter;

        /// <summary>
        /// Builds a raw router to compare against.
        /// </summary>
        /// <returns></returns>
        public void BuildDykstraRouter(string embeddedName,
            IOsmRoutingInterpreter interpreter)
        {
            var tagsIndex = new TagsTableCollectionIndex();

            // do the data processing.
            var data = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamTarget(
                data, interpreter, tagsIndex, new Vehicle[] { Vehicle.Car });
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedName));
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            // initialize the router.
            _referenceRouter = Router.CreateLiveFrom(data, new DykstraRoutingLive(), interpreter);
        }

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="crazyVerification"></param>
        private void DoTestCHSparseVerifiedContraction(string xml, bool crazyVerification)
        {
            _referenceRouter = null;
            if (crazyVerification)
            {
                this.BuildDykstraRouter(xml, new OsmRoutingInterpreter());
            }
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

            var tagsIndex = new TagsTableCollectionIndex();

            // do the data processing.
            _data = new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
            var targetData = new CHEdgeGraphOsmStreamTarget(
                _data, _interpreter, tagsIndex, Vehicle.Car);
            var dataProcessorSource = new XmlOsmStreamSource(stream);
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            // do the pre-processing part.
            var witnessCalculator = new DykstraWitnessCalculator();
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
        /// <param name="crazyVerification"></param>
        internal void DoTestCHEdgeDifferenceVerifiedContraction(string xml, bool crazyVerification)
        {
            _referenceRouter = null;
            if (crazyVerification)
            {
                this.BuildDykstraRouter(xml, new OsmRoutingInterpreter());
            }
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

            var tagsIndex = new TagsTableCollectionIndex();

            // do the data processing.
            _data = new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
            var targetData = new CHEdgeGraphOsmStreamTarget(
                _data, _interpreter, tagsIndex, Vehicle.Car);
            var dataProcessorSource = new XmlOsmStreamSource(stream);
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            // do the pre-processing part.
            var witnessCalculator = new DykstraWitnessCalculator();
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
            var router = new CHRouter();

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
                    PathSegment<long> route = router.Calculate(_data, _interpreter, OsmSharp.Routing.Vehicle.Car, fromList, toList, double.MaxValue, null);
                    if ((fromDic[to.Key] == null && route != null) ||
                        (fromDic[to.Key] != null && route == null) ||
                        ((fromDic[to.Key] != null && route != null) && fromDic[to.Key] != route))
                    { // the route match!
                        Assert.Fail("Routes are different before/after contraction!");
                    }
                }
            }

            if (_referenceRouter != null)
            { // do crazy verification!
                Router chRouter = Router.CreateCHFrom(_data, router, new OsmRoutingInterpreter());

                // loop over all nodes and resolve their locations.
                var resolvedReference = new RouterPoint[_data.VertexCount - 1];
                var resolved = new RouterPoint[_data.VertexCount - 1];
                for (uint idx = 1; idx < _data.VertexCount; idx++)
                { // resolve each vertex.
                    float latitude, longitude;
                    if (_data.GetVertex(idx, out latitude, out longitude))
                    {
                        resolvedReference[idx - 1] = _referenceRouter.Resolve(Vehicle.Car, new GeoCoordinate(latitude, longitude));
                        resolved[idx - 1] = chRouter.Resolve(Vehicle.Car, new GeoCoordinate(latitude, longitude));
                    }

                    Assert.IsNotNull(resolvedReference[idx - 1]);
                    Assert.IsNotNull(resolved[idx - 1]);

                    Assert.AreEqual(resolvedReference[idx - 1].Location.Latitude,
                        resolved[idx - 1].Location.Latitude, 0.0001);
                    Assert.AreEqual(resolvedReference[idx - 1].Location.Longitude,
                        resolved[idx - 1].Location.Longitude, 0.0001);
                }

                // limit tests to a fixed number.
                int maxTestCount = 100;
                int testEveryOther = (resolved.Length * resolved.Length) / maxTestCount;
                testEveryOther = System.Math.Max(testEveryOther, 1);

                // check all the routes having the same weight(s).
                for (int fromIdx = 0; fromIdx < resolved.Length; fromIdx++)
                {
                    for (int toIdx = 0; toIdx < resolved.Length; toIdx++)
                    {
                        int testNumber = fromIdx * resolved.Length + toIdx;
                        if (testNumber % testEveryOther == 0)
                        {
                            Route referenceRoute = _referenceRouter.Calculate(Vehicle.Car,
                                resolvedReference[fromIdx], resolvedReference[toIdx]);
                            Route route = chRouter.Calculate(Vehicle.Car,
                                resolved[fromIdx], resolved[toIdx]);

                            if (referenceRoute != null)
                            {
                                Assert.IsNotNull(referenceRoute);
                                Assert.IsNotNull(route);
                                this.CompareRoutes(referenceRoute, route);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Compares the two given routes.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="route"></param>
        protected void CompareRoutes(Route reference, Route route)
        {
            if (reference.Entries == null)
            {
                Assert.IsNull(route.Entries);
            }
            else
            {
                Assert.AreEqual(reference.Entries.Length, route.Entries.Length);
                for (int idx = 0; idx < reference.Entries.Length; idx++)
                {
                    Assert.AreEqual(reference.Entries[idx].Distance,
                        route.Entries[idx].Distance);
                    Assert.AreEqual(reference.Entries[idx].Latitude,
                        route.Entries[idx].Latitude);
                    Assert.AreEqual(reference.Entries[idx].Longitude,
                        route.Entries[idx].Longitude);
                    Assert.AreEqual(reference.Entries[idx].Time,
                        route.Entries[idx].Time);
                    Assert.AreEqual(reference.Entries[idx].Type,
                        route.Entries[idx].Type);
                    Assert.AreEqual(reference.Entries[idx].WayFromName,
                        route.Entries[idx].WayFromName);
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
            var router = new CHRouter();

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
                    fromDic[to.Key] = router.Calculate(_data, _interpreter,
                        OsmSharp.Routing.Vehicle.Car, fromList, toList, double.MaxValue, null); ;
                }
            }
        }

        #endregion
    }
}
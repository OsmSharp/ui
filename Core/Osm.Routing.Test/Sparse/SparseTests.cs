//// OsmSharp - OpenStreetMap tools & library.
//// Copyright (C) 2012 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Osm.Core.Simple;
//using Osm.Data.Core.Sparse;
//using Osm.Data.Core.Sparse.Primitives;
//using Osm.Data.Raw.XML.OsmSource;
//using Osm.Data.Redis.Sparse;
//using Osm.Data.XML.Raw.Processor;
//using Osm.Routing.Core;
//using Osm.Routing.Core.Route;
//using Osm.Routing.Raw.Graphs.Interpreter;
//using Osm.Routing.Sparse.Memory;
//using Osm.Routing.Sparse.PreProcessor;
//using Osm.Routing.Sparse.Processor;
//using Osm.Routing.Sparse.Routing;
//using Tools.Math.Geo;
//using Tools.Xml.Sources;
//using Osm.Routing.Core.Interpreter.Default;

//namespace Osm.Routing.Test.Sparse
//{
//    /// <summary>
//    /// Contains tests for sparse data source and replication.
//    /// </summary>
//    public static class SparseTests
//    {
//        internal static void DoTests()
//        {
//            // test pre-processing.
//            SparseTests.DoPreProcessingTest();
            
//            // test routing.
//            SparseTests.DoRoutingTests();

//            // test performance over non-sparse.
//            SparseTests.DoRoutingPerformanceTests();

//            Console.ReadLine();
//        }

//        #region Routing Performance Tests

//        /// <summary>
//        /// Do routing test performance test.
//        /// </summary>
//        private static void DoRoutingPerformanceTests()
//        {
//            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

//            Sparse.SparseTests.DoRoutingPerformanceTests(new Sparse.SparseTests.MemorySparseDataFactory(),
//                string.Format("{0}\\Sparse\\{1}.osm", info.FullName, "matrix"),
//                string.Format("{0}\\Sparse\\{1}.csv", info.FullName, "matrix"), 10);

//            Sparse.SparseTests.DoRoutingPerformanceTests(new Sparse.SparseTests.MemorySparseDataFactory(),
//                string.Format("{0}\\Sparse\\{1}.osm", info.FullName, "matrix_big_area"),
//                string.Format("{0}\\Sparse\\{1}.csv", info.FullName, "matrix_big_area"), 10);

//            Sparse.SparseTests.DoRoutingPerformanceTests(new Sparse.SparseTests.MemorySparseDataFactory(),
//                string.Format("{0}\\Sparse\\{1}.osm", info.FullName, "moscow"),
//                string.Format("{0}\\Sparse\\{1}.csv", info.FullName, "moscow"), 2);

//            Sparse.SparseTests.DoRoutingPerformanceTests(new Sparse.SparseTests.MemorySparseDataFactory(),
//                string.Format("{0}\\Sparse\\{1}.osm", info.FullName, "eeklo"),
//                string.Format("{0}\\Sparse\\{1}.csv", info.FullName, "eeklo"), 2);
//        }

//        /// <summary>
//        /// Do performance tests on an OSM area.
//        /// </summary>
//        /// <param name="data_factory"></param>
//        /// <param name="xml"></param>
//        private static void DoRoutingPerformanceTests(ISparseDataFactory data_factory, string xml, string file,
//            int test_count)
//        {
//            Console.WriteLine("Routing performance test: {0}", new FileInfo(xml).Name);

//            // pre-process the data.
//            ISparseData data = data_factory.CreateData();
//            XmlDataProcessorSource source = new XmlDataProcessorSource(xml);
//            SparseDataProcessorTarget target = new SparseDataProcessorTarget(
//                new SparsePreProcessor(data));
//            target.RegisterSource(source);
//            target.Pull();

//            // create the raw router.
//            OsmDataSource osm_data = new OsmDataSource(
//                new Osm.Core.Xml.OsmDocument(new XmlFileSource(xml)));
//            Osm.Routing.Raw.Router raw_router = new Osm.Routing.Raw.Router(osm_data,
//                new DefaultVehicleInterpreter(VehicleEnum.Car));

//            // create the sparse router.            
//            Router sparse_router = new Router(data);

//            // do the tests.
//            long start;
//            long stop;
//            //int test_count = 10;

//            // the raw tests.
//            Console.Write("Raw:");
//            start = DateTime.Now.Ticks;
//            for (int idx = 0; idx < test_count; idx++)
//            {
//                DoRoutingPerformanceTestsRaw(raw_router, file);
//                //Console.Write(".");
//            }
//            stop = DateTime.Now.Ticks;
//            Console.WriteLine(" -> {0}", new TimeSpan((stop - start) / test_count).TotalSeconds.ToString());

//            // the sparse tests.
//            Console.Write("Sparse:");
//            start = DateTime.Now.Ticks;
//            for (int idx = 0; idx < test_count; idx++)
//            {
//                DoRoutingPerformanceTestsSparse(sparse_router, file);
//                //Console.Write(".");
//            }
//            stop = DateTime.Now.Ticks;
//            Console.WriteLine(" -> {0}", new TimeSpan((stop - start) / test_count).TotalSeconds.ToString());
//        }

//        private static void DoRoutingPerformanceTestsRaw(Osm.Routing.Raw.Router router, string file)
//        {
//            // resolve points from matrix.
//            // read matrix points.
//            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
//            DataSet csv = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
//                new System.IO.FileInfo(file), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
//            foreach (DataRow row in csv.Tables[0].Rows)
//            {
//                // be carefull with the parsing and the number formatting for different cultures.
//                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
//                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

//                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
//                coordinates.Add(point);
//            }

//            // try routing the data.
//            Raw.ResolvedPoint[] vertices = router.Resolve(coordinates.ToArray());
//            float[][] weights = router.CalculateManyToManyWeight(vertices, vertices);
//        }

//        private static void DoRoutingPerformanceTestsSparse(Router router, string file)
//        {
//            // resolve points from matrix.
//            // read matrix points.
//            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
//            DataSet csv = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
//                new System.IO.FileInfo(file), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
//            foreach (DataRow row in csv.Tables[0].Rows)
//            {
//                // be carefull with the parsing and the number formatting for different cultures.
//                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
//                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

//                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
//                coordinates.Add(point);
//            }

//            // try routing the data.
//            SparseResolvedPoint[] vertices = router.Resolve(coordinates.ToArray());
//            float[][] weights = router.CalculateManyToManyWeight(vertices, vertices);
//        }

//        #endregion

//        #region Routing Tests

//        internal static void DoRoutingTests()
//        {
//            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

//            //Sparse.SparseTests.RoutingTest(new Sparse.SparseTests.MemorySparseDataFactory(),
//            //    string.Format("{0}\\Sparse\\{1}.osm", info.FullName, "bug"));

//            Sparse.SparseTests.RoutingTest(new Sparse.SparseTests.MemorySparseDataFactory(),
//                string.Format("{0}\\Sparse\\{1}.osm", info.FullName, "wechel"));

//            Sparse.SparseTests.RoutingTest(new Sparse.SparseTests.MemorySparseDataFactory(),
//                string.Format("{0}\\Sparse\\{1}.osm", info.FullName, "matrix"),
//                string.Format("{0}\\Sparse\\{1}.csv", info.FullName, "matrix"));

//            Sparse.SparseTests.RoutingTest(new Sparse.SparseTests.MemorySparseDataFactory(),
//                string.Format("{0}\\Sparse\\{1}.osm", info.FullName, "moscow"),
//                string.Format("{0}\\Sparse\\{1}.csv", info.FullName, "moscow"));
//        }

//        internal static void RoutingTest(ISparseDataFactory data_factory, string xml)
//        {
//            Console.WriteLine(string.Format("Testing routing: {0}", xml));

//            // save the data.
//            ISparseData data = data_factory.CreateData();
//            XmlDataProcessorSource source = new XmlDataProcessorSource(xml);
//            SparseDataProcessorTarget target = new SparseDataProcessorTarget(
//                new SparsePreProcessor(data));
//            target.RegisterSource(source);
//            target.Pull();

//            // try routing the data.
//            Router router = new Router(data);
//            SparseResolvedPoint start = router.Resolve(new GeoCoordinate(51.2679954, 4.801928));
//            SparseResolvedPoint stop = router.Resolve(new GeoCoordinate(51.2610122, 4.7807138));
//            OsmSharpRoute route = router.Calculate(start, stop);

//        }
        
//        internal static void RoutingTest(ISparseDataFactory data_factory, string xml, string file)
//        {
//            Console.WriteLine(string.Format("Testing routing: {0}:{1}", xml, file));

//            // save the data.
//            ISparseData data = data_factory.CreateData();
//            XmlDataProcessorSource source = new XmlDataProcessorSource(xml);
//            SparseDataProcessorTarget target = new SparseDataProcessorTarget(
//                new SparsePreProcessor(data));
//            target.RegisterSource(source);
//            target.Pull();

//            // resolve points from matrix.
//            // read matrix points.
//            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
//            DataSet csv = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
//                new System.IO.FileInfo(file), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
//            foreach (DataRow row in csv.Tables[0].Rows)
//            {
//                // be carefull with the parsing and the number formatting for different cultures.
//                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
//                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

//                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
//                coordinates.Add(point);
//            }

//            // try routing the data.
//            Router router = new Router(data);
//            SparseResolvedPoint[] vertices = router.Resolve(coordinates.ToArray());
//            float[][] weights = router.CalculateManyToManyWeight(vertices, vertices);
//        }

//        #endregion

//        #region Pre-processing

//        /// <summary>
//        /// Does the pre-processing tests.
//        /// </summary>
//        internal static void DoPreProcessingTest()
//        {
//            Console.Write("Pre-processing tests...");
//            SparseTests.SparseTestMemory();
//            Console.WriteLine("Done!");
//        }

//        /// <summary>
//        /// Executes differents test agains the given data.
//        /// </summary>
//        internal static void SparseTest(ISparseDataFactory factory)
//        {
//            ISparseData data = factory.CreateData();
//            SparseTests.SparseTestSimpleNodes(data);

//            data = factory.CreateData();
//            SparseTests.SparseTestSimpleWay(data);

//            data = factory.CreateData();
//            SparseTests.SparseTestDeleteAndBB(data);

//            data = factory.CreateData();
//            SparseTests.SparseTestModification1(data);

//            data = factory.CreateData();
//            SparseTests.SparseTestModification2(data);

//            data = factory.CreateData();
//            SparseTests.SparseTestModification3(data);

//            data = factory.CreateData();
//            SparseTests.SparseTestModification4(data);

//            data = factory.CreateData();
//            SparseTests.SparseTestModification5(data);

//            data = factory.CreateData();
//            SparseTests.SparseTestT(data);

//            data = factory.CreateData();
//            SparseTests.SparseTestClosedWays(data);

//            data = factory.CreateData();
//            SparseTests.SparseTestClosedWayUpdate(data);
            
//            data = factory.CreateData();
//            SparseTests.SparseTestSelfIntersecting(data);

//            data = factory.CreateData();
//            SparseTests.SparseTestClosedWayUpdates(data);
//        }

//        /// <summary>
//        /// Executes different tests agains the redis data.
//        /// </summary>
//        private static void SparseTestMemory()
//        {
//            SparseTests.SparseTest(new MemorySparseDataFactory());
//        }

//        /// <summary>
//        /// Executes different tests agains the redis data.
//        /// </summary>
//        public static void SparseTestRedis()
//        {
//            SparseTests.SparseTest(new RedisSparseDataFactory());
//        }

//        /// <summary>
//        /// Executes a small loading test, delete and test if bb is empty.
//        /// 
//        /// Persist data and checks if inside bounding box.
//        /// </summary>
//        /// <param name="data"></param>
//        private static void SparseTestDeleteAndBB(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 51.2678325;
//            node1.Longitude = 4.8013565;
//            node1.Version = 1;
//            node1.Visible = true;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 51.2678853;
//            node2.Longitude = 4.8012948;
//            node2.Version = 1;
//            node2.Visible = true;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 51.2679652;
//            node3.Longitude = 4.8011975;
//            node3.Version = 1;
//            node3.Visible = true;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 51.2680544;
//            node4.Longitude = 4.8011002;
//            node4.Version = 1;
//            node4.Visible = true;

//            // persist the nodes.
//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);

//            // test the node's presence.
//            SimpleVertex node1_simple = data.GetSimpleVertex(node1.Id.Value);
//            Assert.IsNotNull(node1_simple);
//            Assert.AreEqual(node1_simple.Id, node1.Id);

//            SimpleVertex node2_simple = data.GetSimpleVertex(node2.Id.Value);
//            Assert.IsNotNull(node2_simple);
//            Assert.AreEqual(node2_simple.Id, node2.Id);

//            SimpleVertex node3_simple = data.GetSimpleVertex(node3.Id.Value);
//            Assert.IsNotNull(node3_simple);
//            Assert.AreEqual(node3_simple.Id, node3.Id);

//            SimpleVertex node4_simple = data.GetSimpleVertex(node4.Id.Value);
//            Assert.IsNotNull(node4_simple);
//            Assert.AreEqual(node4_simple.Id, node4.Id);

//            // define the way.
//            SimpleWay way = new SimpleWay();
//            way.Id = 1;
//            way.Nodes = new System.Collections.Generic.List<long>();
//            way.Nodes.Add(1);
//            way.Nodes.Add(2);
//            way.Nodes.Add(3);
//            way.Nodes.Add(4);
//            way.Tags = new Dictionary<string, string>();
//            way.Tags["highway"] = "residential";

//            // persist the way.
//            pre_processor.Process(way, SimpleChangeType.Create);

//            // queries bounding box.
//            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
//            coordinates.Add(new GeoCoordinate(node1.Latitude.Value, node1.Longitude.Value));
//            coordinates.Add(new GeoCoordinate(node2.Latitude.Value, node2.Longitude.Value));
//            coordinates.Add(new GeoCoordinate(node3.Latitude.Value, node3.Longitude.Value));
//            coordinates.Add(new GeoCoordinate(node4.Latitude.Value, node4.Longitude.Value));
//            GeoCoordinateBox box = new GeoCoordinateBox(coordinates.ToArray());
//            List<SparseSimpleVertex> vertices = data.GetSparseSimpleVertices(box);

//            SparseSimpleVertex node1_simple_vertex = data.GetSparseSimpleVertex(1);
//            Assert.IsNotNull(node1_simple_vertex);
//            SparseSimpleVertex node2_simple_vertex = data.GetSparseSimpleVertex(2);
//            Assert.IsNotNull(node2_simple_vertex);
//            SparseSimpleVertex node3_simple_vertex = data.GetSparseSimpleVertex(3);
//            Assert.IsNotNull(node3_simple_vertex);
//            SparseSimpleVertex node4_simple_vertex = data.GetSparseSimpleVertex(4);
//            Assert.IsNotNull(node4_simple_vertex);

//            Assert.IsTrue(vertices.Contains(node1_simple_vertex));
//            Assert.IsTrue(vertices.Contains(node2_simple_vertex));
//            Assert.IsTrue(vertices.Contains(node3_simple_vertex));
//            Assert.IsTrue(vertices.Contains(node4_simple_vertex));

//            // delete the way.
//            pre_processor.Process(way, SimpleChangeType.Delete);

//            // check vertices.
//            SparseVertex vertex1 = data.GetSparseVertex(1);
//            Assert.IsNull(vertex1);

//            SparseVertex vertex2 = data.GetSparseVertex(4);
//            Assert.IsNull(vertex2);

//            node1_simple_vertex = data.GetSparseSimpleVertex(1);
//            Assert.IsNull(node1_simple_vertex);

//            node2_simple_vertex = data.GetSparseSimpleVertex(2);
//            Assert.IsNull(node2_simple_vertex);

//            node3_simple_vertex = data.GetSparseSimpleVertex(3);
//            Assert.IsNull(node3_simple_vertex);

//            node4_simple_vertex = data.GetSparseSimpleVertex(4);
//            Assert.IsNull(node4_simple_vertex);

//            // delete the nodes.
//            pre_processor.Process(node1, SimpleChangeType.Delete);
//            pre_processor.Process(node2, SimpleChangeType.Delete);
//            pre_processor.Process(node3, SimpleChangeType.Delete);
//            pre_processor.Process(node4, SimpleChangeType.Delete);

//            // test the node's presence.
//            node1_simple = data.GetSimpleVertex(node1.Id.Value);
//            Assert.IsNull(node1_simple);

//            node2_simple = data.GetSimpleVertex(node2.Id.Value);
//            Assert.IsNull(node2_simple);

//            node3_simple = data.GetSimpleVertex(node3.Id.Value);
//            Assert.IsNull(node3_simple);

//            node4_simple = data.GetSimpleVertex(node4.Id.Value);
//            Assert.IsNull(node4_simple);

//            // test the boundingbox query.
//            vertices = data.GetSparseSimpleVertices(box);

//            Assert.AreEqual(vertices.Count, 0);

//        }

//        /// <summary>
//        /// Executes a small loading test.
//        /// 
//        /// Persists simple nodes.
//        /// </summary>
//        /// <param name="data"></param>
//        public static void SparseTestSimpleNodes(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 51.2678325;
//            node1.Longitude = 4.8013565;
//            node1.Version = 1;
//            node1.Visible = true;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 51.2678853;
//            node2.Longitude = 4.8012948;
//            node2.Version = 1;
//            node2.Visible = true;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 51.2679652;
//            node3.Longitude = 4.8011975;
//            node3.Version = 1;
//            node3.Visible = true;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 51.2680544;
//            node4.Longitude = 4.8011002;
//            node4.Version = 1;
//            node4.Visible = true;

//            // persist the nodes.
//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);

//            // test the node's presence.
//            SimpleVertex node1_simple = data.GetSimpleVertex(node1.Id.Value);
//            Assert.IsNotNull(node1_simple);
//            Assert.AreEqual(node1_simple.Id, node1.Id);

//            SimpleVertex node2_simple = data.GetSimpleVertex(node2.Id.Value);
//            Assert.IsNotNull(node2_simple);
//            Assert.AreEqual(node2_simple.Id, node2.Id);

//            SimpleVertex node3_simple = data.GetSimpleVertex(node3.Id.Value);
//            Assert.IsNotNull(node3_simple);
//            Assert.AreEqual(node3_simple.Id, node3.Id);

//            SimpleVertex node4_simple = data.GetSimpleVertex(node4.Id.Value);
//            Assert.IsNotNull(node4_simple);
//            Assert.AreEqual(node4_simple.Id, node4.Id);
            
//            // delete the nodes.
//            pre_processor.Process(node1, SimpleChangeType.Delete);
//            pre_processor.Process(node2, SimpleChangeType.Delete);
//            pre_processor.Process(node3, SimpleChangeType.Delete);
//            pre_processor.Process(node4, SimpleChangeType.Delete);

//            // test the node's presence.
//            node1_simple = data.GetSimpleVertex(node1.Id.Value);
//            Assert.IsNull(node1_simple);

//            node2_simple = data.GetSimpleVertex(node2.Id.Value);
//            Assert.IsNull(node2_simple);

//            node3_simple = data.GetSimpleVertex(node3.Id.Value);
//            Assert.IsNull(node3_simple);

//            node4_simple = data.GetSimpleVertex(node4.Id.Value);
//            Assert.IsNull(node4_simple);
//        }

//        /// <summary>
//        /// Executes a small loading test.
//        /// 
//        /// Persists one way.
//        /// </summary>
//        /// <param name="data"></param>
//        public static void SparseTestSimpleWay(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 51.2678325;
//            node1.Longitude = 4.8013565;
//            node1.Version = 1;
//            node1.Visible = true;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 51.2678853;
//            node2.Longitude = 4.8012948;
//            node2.Version = 1;
//            node2.Visible = true;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 51.2679652;
//            node3.Longitude = 4.8011975;
//            node3.Version = 1;
//            node3.Visible = true;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 51.2680544;
//            node4.Longitude = 4.8011002;
//            node4.Version = 1;
//            node4.Visible = true;

//            // persist the nodes.
//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);

//            // test the node's presence.
//            SimpleVertex node1_simple = data.GetSimpleVertex(node1.Id.Value);
//            Assert.IsNotNull(node1_simple);
//            Assert.AreEqual(node1_simple.Id, node1.Id);

//            SimpleVertex node2_simple = data.GetSimpleVertex(node2.Id.Value);
//            Assert.IsNotNull(node2_simple);
//            Assert.AreEqual(node2_simple.Id, node2.Id);

//            SimpleVertex node3_simple = data.GetSimpleVertex(node3.Id.Value);
//            Assert.IsNotNull(node3_simple);
//            Assert.AreEqual(node3_simple.Id, node3.Id);

//            SimpleVertex node4_simple = data.GetSimpleVertex(node4.Id.Value);
//            Assert.IsNotNull(node4_simple);
//            Assert.AreEqual(node4_simple.Id, node4.Id);

//            // define the way.
//            SimpleWay way = new SimpleWay();
//            way.Id = 1;
//            way.Nodes = new System.Collections.Generic.List<long>();
//            way.Nodes.Add(1);
//            way.Nodes.Add(2);
//            way.Nodes.Add(3);
//            way.Nodes.Add(4);
//            way.Tags = new Dictionary<string, string>();
//            way.Tags["highway"] = "residential";

//            // persist the way.
//            pre_processor.Process(way, SimpleChangeType.Create);

//            // check vertices.
//            SparseVertex vertex1 = data.GetSparseVertex(1);
//            Assert.IsNotNull(vertex1);
//            Assert.IsNotNull(vertex1.Neighbours);
//            Assert.IsTrue(vertex1.Neighbours.Length == 1);
//            Assert.AreEqual(vertex1.Neighbours[0].Id, 4);

//            SparseVertex vertex2 = data.GetSparseVertex(4);
//            Assert.IsNotNull(vertex2);
//            Assert.IsNotNull(vertex2.Neighbours);
//            Assert.IsTrue(vertex2.Neighbours.Length == 1);
//            Assert.AreEqual(vertex2.Neighbours[0].Id, 1);

//            SparseSimpleVertex simple_vertex1 = data.GetSparseSimpleVertex(1);
//            Assert.IsNotNull(simple_vertex1);
//            Assert.AreEqual(simple_vertex1.Neighbour1, 0);
//            Assert.AreEqual(simple_vertex1.Neighbour2, 0);

//            SparseSimpleVertex simple_vertex2 = data.GetSparseSimpleVertex(2);
//            Assert.IsNotNull(simple_vertex2);
//            Assert.IsTrue(simple_vertex2.Neighbour1 == 1 || simple_vertex2.Neighbour2 == 1);
//            Assert.IsTrue(simple_vertex2.Neighbour1 == 4 || simple_vertex2.Neighbour2 == 4);

//            SparseSimpleVertex simple_vertex3 = data.GetSparseSimpleVertex(3);
//            Assert.IsNotNull(simple_vertex3);
//            Assert.IsTrue(simple_vertex3.Neighbour1 == 1 || simple_vertex2.Neighbour2 == 1);
//            Assert.IsTrue(simple_vertex3.Neighbour1 == 4 || simple_vertex2.Neighbour2 == 4);

//            SparseSimpleVertex simple_vertex4 = data.GetSparseSimpleVertex(4);
//            Assert.IsNotNull(simple_vertex4);
//            Assert.AreEqual(simple_vertex4.Neighbour1, 0);
//            Assert.AreEqual(simple_vertex4.Neighbour2, 0);

//            // delete the way.
//            pre_processor.Process(way, SimpleChangeType.Delete);

//            // check vertices.
//            vertex1 = data.GetSparseVertex(1);
//            Assert.IsNull(vertex1);

//            vertex2 = data.GetSparseVertex(4);
//            Assert.IsNull(vertex2);

//            simple_vertex1 = data.GetSparseSimpleVertex(1);
//            Assert.IsNull(simple_vertex1);

//            simple_vertex2 = data.GetSparseSimpleVertex(2);
//            Assert.IsNull(simple_vertex2);

//            simple_vertex3 = data.GetSparseSimpleVertex(3);
//            Assert.IsNull(simple_vertex3);

//            simple_vertex4 = data.GetSparseSimpleVertex(4);
//            Assert.IsNull(simple_vertex4);

//            // check arc.
//            SimpleArc arc = data.GetSimpleArc(1);
//            Assert.IsNull(arc);

//            // delete the nodes.
//            pre_processor.Process(node1, SimpleChangeType.Delete);
//            pre_processor.Process(node2, SimpleChangeType.Delete);
//            pre_processor.Process(node3, SimpleChangeType.Delete);
//            pre_processor.Process(node4, SimpleChangeType.Delete);

//            // test the node's presence.
//            node1_simple = data.GetSimpleVertex(node1.Id.Value);
//            Assert.IsNull(node1_simple);

//            node2_simple = data.GetSimpleVertex(node2.Id.Value);
//            Assert.IsNull(node2_simple);

//            node3_simple = data.GetSimpleVertex(node3.Id.Value);
//            Assert.IsNull(node3_simple);

//            node4_simple = data.GetSimpleVertex(node4.Id.Value);
//            Assert.IsNull(node4_simple);
//        }
        
//        /// <summary>
//        /// Executes a small modification test.
//        /// 
//        /// Persists and modifies one way by removing one node.
//        /// </summary>
//        /// <param name="data"></param>
//        public static void SparseTestModification1(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 51.2678325;
//            node1.Longitude = 4.8013565;
//            node1.Version = 1;
//            node1.Visible = true;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 51.2678853;
//            node2.Longitude = 4.8012948;
//            node2.Version = 1;
//            node2.Visible = true;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 51.2679652;
//            node3.Longitude = 4.8011975;
//            node3.Version = 1;
//            node3.Visible = true;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 51.2680544;
//            node4.Longitude = 4.8011002;
//            node4.Version = 1;
//            node4.Visible = true;

//            // persist the nodes.
//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);

//            // define the way.
//            SimpleWay way = new SimpleWay();
//            way.Id = 1;
//            way.Nodes = new System.Collections.Generic.List<long>();
//            way.Nodes.Add(1);
//            way.Nodes.Add(2); // this one will be removed!
//            way.Nodes.Add(3);
//            way.Nodes.Add(4);
//            way.Tags = new Dictionary<string, string>();
//            way.Tags["highway"] = "residential";

//            // persist the way.
//            pre_processor.Process(way, SimpleChangeType.Create);

//            // modify the way.
//            way.Nodes.Remove(2);
//            pre_processor.Process(way, SimpleChangeType.Modify);

//            // check the modifications.
//            SparseVertex vertex1 = data.GetSparseVertex(1);
//            Assert.IsNotNull(vertex1);
//            Assert.IsNotNull(vertex1.Neighbours);
//            Assert.AreEqual(vertex1.Neighbours[0].Nodes.Length, 1);
//            Assert.AreEqual(vertex1.Neighbours[0].Nodes[0], 3);

//            SparseVertex vertex2 = data.GetSparseVertex(4);
//            Assert.IsNotNull(vertex2);
//            Assert.IsNotNull(vertex2.Neighbours);
//            Assert.AreEqual(vertex2.Neighbours[0].Nodes.Length, 1);
//            Assert.AreEqual(vertex2.Neighbours[0].Nodes[0], 3);

//            SparseSimpleVertex simple_vertex1 = data.GetSparseSimpleVertex(1);
//            Assert.IsNotNull(simple_vertex1);
//            Assert.AreEqual(simple_vertex1.Neighbour1, 0);
//            Assert.AreEqual(simple_vertex1.Neighbour2, 0);

//            SparseSimpleVertex simple_vertex2 = data.GetSparseSimpleVertex(2);
//            Assert.IsNull(simple_vertex2);

//            SparseSimpleVertex simple_vertex3 = data.GetSparseSimpleVertex(3);
//            Assert.IsNotNull(simple_vertex3);
//            Assert.IsTrue(simple_vertex3.Neighbour1 == 1 || simple_vertex3.Neighbour2 == 1);
//            Assert.IsTrue(simple_vertex3.Neighbour1 == 4 || simple_vertex3.Neighbour2 == 4);

//            SparseSimpleVertex simple_vertex4 = data.GetSparseSimpleVertex(4);
//            Assert.IsNotNull(simple_vertex4);
//            Assert.AreEqual(simple_vertex4.Neighbour1, 0);
//            Assert.AreEqual(simple_vertex4.Neighbour2, 0);

//            // delete the way.
//            pre_processor.Process(way, SimpleChangeType.Delete);

//            // delete the nodes.
//            pre_processor.Process(node1, SimpleChangeType.Delete);
//            pre_processor.Process(node2, SimpleChangeType.Delete);
//            pre_processor.Process(node3, SimpleChangeType.Delete);
//            pre_processor.Process(node4, SimpleChangeType.Delete);
//        }
        
//        /// <summary>
//        /// Executes a small modification test.
//        /// 
//        /// Persists and modifies one way by adding one node.
//        /// </summary>
//        /// <param name="data"></param>
//        public static void SparseTestModification2(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 51.2678325;
//            node1.Longitude = 4.8013565;
//            node1.Version = 1;
//            node1.Visible = true;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 51.2678853;
//            node2.Longitude = 4.8012948;
//            node2.Version = 1;
//            node2.Visible = true;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 51.2679652;
//            node3.Longitude = 4.8011975;
//            node3.Version = 1;
//            node3.Visible = true;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 51.2680544;
//            node4.Longitude = 4.8011002;
//            node4.Version = 1;
//            node4.Visible = true;
//            SimpleNode node5 = new SimpleNode();
//            node5.Id = 5;
//            node5.Latitude = 51.26791;
//            node5.Longitude = 4.80118;
//            node5.Version = 1;
//            node5.Visible = true;

//            // persist the nodes.
//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);
//            pre_processor.Process(node5, SimpleChangeType.Create);

//            // define the way.
//            SimpleWay way = new SimpleWay();
//            way.Id = 1;
//            way.Nodes = new System.Collections.Generic.List<long>();
//            way.Nodes.Add(1);
//            way.Nodes.Add(2);
//            //way.Nodes.Add(5); // this one will be added!
//            way.Nodes.Add(3);
//            way.Nodes.Add(4);
//            way.Tags = new Dictionary<string, string>();
//            way.Tags["highway"] = "residential";

//            // persist the way.
//            pre_processor.Process(way, SimpleChangeType.Create);

//            // modify the way.
//            way.Nodes.Insert(2, 5);
//            pre_processor.Process(way, SimpleChangeType.Modify);

//            // check the modifications.
//            SparseVertex vertex1 = data.GetSparseVertex(1);
//            Assert.IsNotNull(vertex1);
//            Assert.IsNotNull(vertex1.Neighbours);
//            Assert.AreEqual(vertex1.Neighbours[0].Nodes.Length, 3);
//            Assert.AreEqual(vertex1.Neighbours[0].Nodes[0], 2);
//            Assert.AreEqual(vertex1.Neighbours[0].Nodes[1], 5);
//            Assert.AreEqual(vertex1.Neighbours[0].Nodes[2], 3);
//            Assert.AreEqual(vertex1.Neighbours[0].Id, 4);

//            SparseVertex vertex2 = data.GetSparseVertex(4);
//            Assert.IsNotNull(vertex2);
//            Assert.IsNotNull(vertex2.Neighbours);
//            Assert.AreEqual(vertex2.Neighbours[0].Nodes.Length, 3);
//            Assert.AreEqual(vertex2.Neighbours[0].Nodes[0], 3);
//            Assert.AreEqual(vertex2.Neighbours[0].Nodes[1], 5);
//            Assert.AreEqual(vertex2.Neighbours[0].Nodes[2], 2);
//            Assert.AreEqual(vertex2.Neighbours[0].Id, 1);

//            SparseSimpleVertex simple_vertex1 = data.GetSparseSimpleVertex(1);
//            Assert.IsNotNull(simple_vertex1);
//            Assert.AreEqual(simple_vertex1.Neighbour1, 0);
//            Assert.AreEqual(simple_vertex1.Neighbour2, 0);

//            SparseSimpleVertex simple_vertex2 = data.GetSparseSimpleVertex(2);
//            Assert.IsNotNull(simple_vertex2);
//            Assert.IsTrue(simple_vertex2.Neighbour1 == 1 || simple_vertex2.Neighbour2 == 1);
//            Assert.IsTrue(simple_vertex2.Neighbour1 == 4 || simple_vertex2.Neighbour2 == 4);
            
//            SparseSimpleVertex simple_vertex5 = data.GetSparseSimpleVertex(5);
//            Assert.IsNotNull(simple_vertex5);
//            Assert.IsTrue(simple_vertex5.Neighbour1 == 1 || simple_vertex5.Neighbour2 == 1);
//            Assert.IsTrue(simple_vertex5.Neighbour1 == 4 || simple_vertex5.Neighbour2 == 4);

//            SparseSimpleVertex simple_vertex3 = data.GetSparseSimpleVertex(3);
//            Assert.IsNotNull(simple_vertex3);
//            Assert.IsTrue(simple_vertex3.Neighbour1 == 1 || simple_vertex3.Neighbour2 == 1);
//            Assert.IsTrue(simple_vertex3.Neighbour1 == 4 || simple_vertex3.Neighbour2 == 4);

//            SparseSimpleVertex simple_vertex4 = data.GetSparseSimpleVertex(4);
//            Assert.IsNotNull(simple_vertex4);
//            Assert.AreEqual(simple_vertex4.Neighbour1, 0);
//            Assert.AreEqual(simple_vertex4.Neighbour2, 0);

//            // delete the way.
//            pre_processor.Process(way, SimpleChangeType.Delete);

//            // delete the nodes.
//            pre_processor.Process(node1, SimpleChangeType.Delete);
//            pre_processor.Process(node2, SimpleChangeType.Delete);
//            pre_processor.Process(node3, SimpleChangeType.Delete);
//            pre_processor.Process(node4, SimpleChangeType.Delete);
//            pre_processor.Process(node5, SimpleChangeType.Delete);
//        }

//        /// <summary>
//        /// Does a sparse test of intersecting roads.
//        /// 
//        /// Tests two cross intersecting ways.
//        /// </summary>
//        /// <param name="data"></param>
//        private static void SparseTestModification3(ISparseData data)
//        {            
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = -0.0108;
//            node1.Longitude = 0.004;
//            node1.Version = 1;
//            node1.Visible = true;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 0;
//            node2.Longitude = 0.0092;
//            node2.Version = 1;
//            node2.Visible = true;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = -0.0231;
//            node3.Longitude = -0.00143;
//            node3.Version = 1;
//            node3.Visible = true;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = -0.02;
//            node4.Longitude = 0.128;
//            node4.Version = 1;
//            node4.Visible = true;
//            SimpleNode node5 = new SimpleNode();
//            node5.Id = 5;
//            node5.Latitude = -0.0016;
//            node5.Longitude = 0.0033;
//            node5.Version = 1;
//            node5.Visible = true;

//            SimpleWay way1 = new SimpleWay();
//            way1.Id = 1;
//            way1.Nodes = new List<long>();
//            way1.Nodes.Add(3);
//            way1.Nodes.Add(1);
//            way1.Nodes.Add(2);
//            way1.Tags = new Dictionary<string, string>();
//            way1.Tags["highway"] = "residential";

//            SimpleWay way2 = new SimpleWay();
//            way2.Id = 2;
//            way2.Nodes = new List<long>();
//            way2.Nodes.Add(4);
//            way2.Nodes.Add(1);
//            way2.Nodes.Add(5);
//            way2.Tags = new Dictionary<string, string>();
//            way2.Tags["highway"] = "residential";

//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);
//            pre_processor.Process(node5, SimpleChangeType.Create);

//            pre_processor.Process(way1, SimpleChangeType.Create);
//            pre_processor.Process(way2, SimpleChangeType.Create);

//            // test results.
//            SparseVertex sparse_vertex1 = data.GetSparseVertex(1);
//            Assert.IsNotNull(sparse_vertex1);
//            Assert.IsNotNull(sparse_vertex1.Neighbours);
//            Assert.AreEqual(sparse_vertex1.Neighbours.Length, 4);
            
//            SparseVertex sparse_vertex2 = data.GetSparseVertex(2);
//            Assert.IsNotNull(sparse_vertex2);
//            Assert.IsNotNull(sparse_vertex2.Neighbours);
//            Assert.AreEqual(sparse_vertex2.Neighbours.Length, 1);

//            SparseVertex sparse_vertex3 = data.GetSparseVertex(3);
//            Assert.IsNotNull(sparse_vertex3);
//            Assert.IsNotNull(sparse_vertex3.Neighbours);
//            Assert.AreEqual(sparse_vertex3.Neighbours.Length, 1);

//            SparseVertex sparse_vertex4 = data.GetSparseVertex(4);
//            Assert.IsNotNull(sparse_vertex4);
//            Assert.IsNotNull(sparse_vertex4.Neighbours);
//            Assert.AreEqual(sparse_vertex4.Neighbours.Length, 1);

//            SparseVertex sparse_vertex5 = data.GetSparseVertex(5);
//            Assert.IsNotNull(sparse_vertex5);
//            Assert.IsNotNull(sparse_vertex5.Neighbours);
//            Assert.AreEqual(sparse_vertex5.Neighbours.Length, 1);

//            // delete way2 again.
//            pre_processor.Process(way2, SimpleChangeType.Delete);

//            // test results.
//            sparse_vertex1 = data.GetSparseVertex(1);
//            Assert.IsNull(sparse_vertex1);
//            //Assert.IsNotNull(sparse_vertex1.Neighbours);
//            //Assert.AreEqual(sparse_vertex1.Neighbours.Length, 4);

//            sparse_vertex2 = data.GetSparseVertex(2);
//            Assert.IsNotNull(sparse_vertex2);
//            Assert.IsNotNull(sparse_vertex2.Neighbours);
//            Assert.AreEqual(sparse_vertex2.Neighbours.Length, 1);

//            sparse_vertex3 = data.GetSparseVertex(3);
//            Assert.IsNotNull(sparse_vertex3);
//            Assert.IsNotNull(sparse_vertex3.Neighbours);
//            Assert.AreEqual(sparse_vertex3.Neighbours.Length, 1);

//            sparse_vertex4 = data.GetSparseVertex(4);
//            Assert.IsNull(sparse_vertex4);
//            //Assert.IsNotNull(sparse_vertex4.Neighbours);
//            //Assert.AreEqual(sparse_vertex4.Neighbours.Length, 1);

//            sparse_vertex5 = data.GetSparseVertex(5);
//            Assert.IsNull(sparse_vertex5);
//            //Assert.IsNotNull(sparse_vertex5.Neighbours);
//            //Assert.AreEqual(sparse_vertex5.Neighbours.Length, 1);

//            SparseSimpleVertex sparse_simple_vertex1 = data.GetSparseSimpleVertex(1);
//            Assert.IsNotNull(sparse_simple_vertex1);
//            Assert.IsTrue(sparse_simple_vertex1.Neighbour1 == 2 || sparse_simple_vertex1.Neighbour1 == 3);
//            Assert.IsTrue(sparse_simple_vertex1.Neighbour2 == 3 || sparse_simple_vertex1.Neighbour2 == 2);

//            // delete the way.
//            pre_processor.Process(way1, SimpleChangeType.Delete);

//            // check vertices.
//            SparseVertex vertex1 = data.GetSparseVertex(2);
//            Assert.IsNull(vertex1);

//            SparseVertex vertex2 = data.GetSparseVertex(3);
//            Assert.IsNull(vertex2);

//            SparseSimpleVertex simple_vertex1 = data.GetSparseSimpleVertex(1);
//            Assert.IsNull(simple_vertex1);

//            SparseSimpleVertex simple_vertex2 = data.GetSparseSimpleVertex(2);
//            Assert.IsNull(simple_vertex2);

//            SparseSimpleVertex simple_vertex3 = data.GetSparseSimpleVertex(3);
//            Assert.IsNull(simple_vertex3);

//            SparseSimpleVertex simple_vertex4 = data.GetSparseSimpleVertex(4);
//            Assert.IsNull(simple_vertex4);

//            SparseSimpleVertex simple_vertex5 = data.GetSparseSimpleVertex(5);
//            Assert.IsNull(simple_vertex5);

//            // check arc.
//            SimpleArc arc = data.GetSimpleArc(1);
//            Assert.IsNull(arc);

//            // delete the nodes.
//            pre_processor.Process(node1, SimpleChangeType.Delete);
//            pre_processor.Process(node2, SimpleChangeType.Delete);
//            pre_processor.Process(node3, SimpleChangeType.Delete);
//            pre_processor.Process(node4, SimpleChangeType.Delete);
//            pre_processor.Process(node5, SimpleChangeType.Delete);

//            // test the node's presence.
//            SimpleVertex node1_simple = data.GetSimpleVertex(node1.Id.Value);
//            Assert.IsNull(node1_simple);

//            SimpleVertex node2_simple = data.GetSimpleVertex(node2.Id.Value);
//            Assert.IsNull(node2_simple);

//            SimpleVertex node3_simple = data.GetSimpleVertex(node3.Id.Value);
//            Assert.IsNull(node3_simple);

//            SimpleVertex node4_simple = data.GetSimpleVertex(node4.Id.Value);
//            Assert.IsNull(node4_simple);

//            SimpleVertex node5_simple = data.GetSimpleVertex(node5.Id.Value);
//            Assert.IsNull(node5_simple);
//        }

//        /// <summary>
//        /// Does a sparse test of multiple modifcations on one way.
//        /// 
//        /// Tests one way intersecting with multiple ways.
//        /// </summary>
//        /// <param name="data"></param>
//        private static void SparseTestModification4(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 0.001;
//            node1.Longitude = 0;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 0.001;
//            node2.Longitude = 0.001;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 0.001;
//            node3.Longitude = 0.002;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 0.001;
//            node4.Longitude = 0.003;

//            SimpleNode node5 = new SimpleNode();
//            node5.Id = 5;
//            node5.Latitude = 0;
//            node5.Longitude = 0.002;            
//            SimpleNode node6 = new SimpleNode();
//            node6.Id = 6;
//            node6.Latitude = 0;
//            node6.Longitude = 0.001;
            
//            SimpleNode node7 = new SimpleNode();
//            node7.Id = 7;
//            node7.Latitude = 0.002;
//            node7.Longitude = 0.002;
//            SimpleNode node8 = new SimpleNode();
//            node8.Id = 8;
//            node8.Latitude = 0.002;
//            node8.Longitude = 0.001;

//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);
//            pre_processor.Process(node5, SimpleChangeType.Create);
//            pre_processor.Process(node6, SimpleChangeType.Create);
//            pre_processor.Process(node7, SimpleChangeType.Create);
//            pre_processor.Process(node8, SimpleChangeType.Create);

//            SimpleWay way1 = new SimpleWay();
//            way1.Id = 1;
//            way1.Nodes = new List<long>();
//            way1.Nodes.Add(1);
//            way1.Nodes.Add(2);
//            way1.Nodes.Add(3);
//            way1.Nodes.Add(4);
//            way1.Tags = new Dictionary<string, string>();
//            way1.Tags["highway"] = "residential";

//            pre_processor.Process(way1, SimpleChangeType.Create);
            
//            SimpleWay way2 = new SimpleWay();
//            way2.Id = 2;
//            way2.Nodes = new List<long>();
//            way2.Nodes.Add(7);
//            way2.Nodes.Add(2);
//            way2.Nodes.Add(6);
//            way2.Tags = new Dictionary<string, string>();
//            way2.Tags["highway"] = "residential";

//            pre_processor.Process(way2, SimpleChangeType.Create);

//            SparseSimpleVertex sparse_simple_vertex3 = data.GetSparseSimpleVertex(3);
//            Assert.IsTrue(sparse_simple_vertex3.Neighbour1 == 2 || sparse_simple_vertex3.Neighbour1 == 4);
//            Assert.IsTrue(sparse_simple_vertex3.Neighbour2 == 2 || sparse_simple_vertex3.Neighbour2 == 4);            

//            SimpleWay way3 = new SimpleWay();
//            way3.Id = 2;
//            way3.Nodes = new List<long>();
//            way3.Nodes.Add(8);
//            way3.Nodes.Add(3);
//            way3.Nodes.Add(5);
//            way3.Tags = new Dictionary<string, string>();
//            way3.Tags["highway"] = "residential";

//            pre_processor.Process(way3, SimpleChangeType.Create);

//            sparse_simple_vertex3 = data.GetSparseSimpleVertex(3);
//            Assert.IsTrue(sparse_simple_vertex3.Neighbour1 == 0);
//            Assert.IsTrue(sparse_simple_vertex3.Neighbour2 == 0);   
//        }
        
//        /// <summary>
//        /// Does a sparse test of multiple modifcations on one way.
//        /// 
//        /// Test a way intersection with another and the nodes in between.
//        /// </summary>
//        /// <param name="data"></param>
//        private static void SparseTestModification5(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 0.003;
//            node1.Longitude = 0.001;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 0.002;
//            node2.Longitude = 0.001;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 0.001;
//            node3.Longitude = 0.001;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 0;
//            node4.Longitude = 0.001;

//            SimpleNode node5 = new SimpleNode();
//            node5.Id = 5;
//            node5.Latitude = 0.002;
//            node5.Longitude = 0;
//            SimpleNode node6 = new SimpleNode();
//            node6.Id = 6;
//            node6.Latitude = 0.002;
//            node6.Longitude = 0.002;

//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);
//            pre_processor.Process(node5, SimpleChangeType.Create);
//            pre_processor.Process(node6, SimpleChangeType.Create);

//            SimpleWay way1 = new SimpleWay();
//            way1.Id = 1;
//            way1.Nodes = new List<long>();
//            way1.Nodes.Add(1);
//            way1.Nodes.Add(2);
//            way1.Nodes.Add(3);
//            way1.Nodes.Add(4);
//            way1.Tags = new Dictionary<string, string>();
//            way1.Tags["highway"] = "residential";

//            pre_processor.Process(way1, SimpleChangeType.Create);

//            SimpleWay way2 = new SimpleWay();
//            way2.Id = 2;
//            way2.Nodes = new List<long>();
//            way2.Nodes.Add(5);
//            way2.Nodes.Add(2);
//            way2.Nodes.Add(6);
//            way2.Tags = new Dictionary<string, string>();
//            way2.Tags["highway"] = "residential";

//            pre_processor.Process(way2, SimpleChangeType.Create);

//            SparseVertex sparse_vertex2 = data.GetSparseVertex(2);
//            SparseVertexNeighbour sparse_vertex2_neighbour4 = sparse_vertex2.GetSparseVertexNeighbour(4);
//            Assert.IsNotNull(sparse_vertex2_neighbour4);
//            Assert.IsTrue(sparse_vertex2_neighbour4.Nodes.Length == 1);
//            Assert.IsTrue(sparse_vertex2_neighbour4.Nodes[0] == 3);
//            SparseVertexNeighbour sparse_vertex2_neighbour1 = sparse_vertex2.GetSparseVertexNeighbour(1);
//            Assert.IsNotNull(sparse_vertex2_neighbour1);
//            Assert.IsTrue(sparse_vertex2_neighbour1.Nodes.Length == 0);
//            //Assert.IsTrue(sparse_vertex2.Neighbour1 == 2 || sparse_vertex2.Neighbour1 == 4);
//            //Assert.IsTrue(sparse_vertex2.Neighbour2 == 2 || sparse_vertex2.Neighbour2 == 4);
//        }

//        /// <summary>
//        /// Does a sparse test with closed way.
//        /// </summary>
//        /// <param name="data"></param>
//        private static void SparseTestClosedWays(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 0;
//            node1.Longitude = 0;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 0.001;
//            node2.Longitude = 0;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 0.001;
//            node3.Longitude = 0.001;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 0;
//            node4.Longitude = 0.001;

//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);

//            SimpleWay way1 = new SimpleWay();
//            way1.Id = 1;
//            way1.Nodes = new List<long>();
//            way1.Nodes.Add(1);
//            way1.Nodes.Add(2);
//            way1.Nodes.Add(3);
//            way1.Nodes.Add(4);
//            way1.Nodes.Add(1);
//            way1.Tags = new Dictionary<string, string>();
//            way1.Tags["highway"] = "residential";

//            pre_processor.Process(way1, SimpleChangeType.Create);
//            SparseVertex sparse_vertex1 = data.GetSparseVertex(1);
//            foreach (SparseVertexNeighbour neighbour in sparse_vertex1.Neighbours)
//            {
//                Assert.AreEqual(1, neighbour.Id);
//                HashSet<long> neighbours = new HashSet<long>(neighbour.Nodes);
//                Assert.IsTrue(neighbours.Contains(2));
//                Assert.IsTrue(neighbours.Contains(3));
//                Assert.IsTrue(neighbours.Contains(4));
//            }
//        }

//        /// <summary>
//        /// Does a sparse test with closed way.
//        /// </summary>
//        /// <param name="data"></param>
//        private static void SparseTestSelfIntersecting(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 0;
//            node1.Longitude = 0.001;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 0.001;
//            node2.Longitude = 0.001;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 0.002;
//            node3.Longitude = 0;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 0.002;
//            node4.Longitude = 0.002;

//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);

//            SimpleWay way1 = new SimpleWay();
//            way1.Id = 1;
//            way1.Nodes = new List<long>();
//            way1.Nodes.Add(1);
//            way1.Nodes.Add(2);
//            way1.Nodes.Add(3);
//            way1.Nodes.Add(4);
//            way1.Nodes.Add(2);
//            way1.Tags = new Dictionary<string, string>();
//            way1.Tags["highway"] = "residential";

//            pre_processor.Process(way1, SimpleChangeType.Create);

//            SparseVertex sparse_vertex1 = data.GetSparseVertex(1);
//            SparseVertexNeighbour sparse_vertex1_neighbour2 = sparse_vertex1.GetSparseVertexNeighbour(2);
//            Assert.IsNotNull(sparse_vertex1_neighbour2);
//            Assert.AreEqual(2, sparse_vertex1_neighbour2.Id);
//            Assert.AreEqual(0, sparse_vertex1_neighbour2.Nodes.Length);

//            SparseVertex sparse_vertex2 = data.GetSparseVertex(2);
//            Assert.AreEqual(3, sparse_vertex2.Neighbours.Length);
//            foreach (SparseVertexNeighbour neighbour in sparse_vertex2.Neighbours)
//            {
//                if (neighbour.Id == 1)
//                {
//                    Assert.AreEqual(0, neighbour.Nodes.Length);
//                }
//                else if (neighbour.Id == 2)
//                {
//                    Assert.AreEqual(2, neighbour.Nodes.Length);
//                }
//            }
//        }

//        /// <summary>
//        /// Does a sparse test with closed way.
//        /// </summary>
//        /// <param name="data"></param>
//        private static void SparseTestClosedWayUpdate(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 0.001;
//            node1.Longitude = 0.001;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 0.002;
//            node2.Longitude = 0.001;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 0.002;
//            node3.Longitude = 0.003;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 0;
//            node4.Longitude = 0.003;
//            SimpleNode node5 = new SimpleNode();
//            node5.Id = 5;
//            node5.Latitude = 0;
//            node5.Longitude = 0.001;

//            SimpleNode node6 = new SimpleNode();
//            node6.Id = 6;
//            node6.Latitude = 0.001;
//            node6.Longitude = 0;
//            SimpleNode node7 = new SimpleNode();
//            node7.Id = 7;
//            node7.Latitude = 0.001;
//            node7.Longitude = 0.002;

//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);
//            pre_processor.Process(node5, SimpleChangeType.Create);
//            pre_processor.Process(node6, SimpleChangeType.Create);
//            pre_processor.Process(node7, SimpleChangeType.Create);
            
//            SimpleWay way1 = new SimpleWay();
//            way1.Id = 1;
//            way1.Nodes = new List<long>();
//            way1.Nodes.Add(1);
//            way1.Nodes.Add(2);
//            way1.Nodes.Add(3);
//            way1.Nodes.Add(4);
//            way1.Nodes.Add(5);
//            way1.Nodes.Add(1);
//            way1.Tags = new Dictionary<string, string>();
//            way1.Tags["highway"] = "residential";

//            pre_processor.Process(way1, SimpleChangeType.Create);
//            SparseVertex sparse_vertex1 = data.GetSparseVertex(1);
//            Assert.IsNotNull(sparse_vertex1);

//            SimpleWay way2 = new SimpleWay();
//            way2.Id = 1;
//            way2.Nodes = new List<long>();
//            way2.Nodes.Add(6);
//            way2.Nodes.Add(1);
//            way2.Nodes.Add(7);
//            way2.Nodes.Add(3);
//            way2.Tags = new Dictionary<string, string>();
//            way2.Tags["highway"] = "residential";

//            pre_processor.Process(way2, SimpleChangeType.Create);


//        }


//        /// <summary>
//        /// Does a sparse test with closed way.
//        /// </summary>
//        /// <param name="data"></param>
//        private static void SparseTestClosedWayUpdates(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 0.001;
//            node1.Longitude = 0.001;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 0.002;
//            node2.Longitude = 0.001;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 0.002;
//            node3.Longitude = 0.002;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 0.001;
//            node4.Longitude = 0.002;

//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);

//            SimpleWay way1 = new SimpleWay();
//            way1.Id = 1;
//            way1.Nodes = new List<long>();
//            way1.Nodes.Add(1);
//            way1.Nodes.Add(2);
//            way1.Nodes.Add(3);
//            way1.Nodes.Add(4);
//            way1.Nodes.Add(1);
//            way1.Tags = new Dictionary<string, string>();
//            way1.Tags["highway"] = "residential";

//            pre_processor.Process(way1, SimpleChangeType.Create);
//            SparseVertex sparse_vertex1 = data.GetSparseVertex(1);
//            foreach (SparseVertexNeighbour neighbour in sparse_vertex1.Neighbours)
//            {
//                Assert.AreEqual(1, neighbour.Id);
//                HashSet<long> neighbours = new HashSet<long>(neighbour.Nodes);
//                Assert.IsTrue(neighbours.Contains(2));
//                Assert.IsTrue(neighbours.Contains(3));
//                Assert.IsTrue(neighbours.Contains(4));
//            }

//            SimpleNode node5 = new SimpleNode();
//            node5.Id = 5;
//            node5.Latitude = 0;
//            node5.Longitude = 0.003;
//            SimpleNode node6 = new SimpleNode();
//            node6.Id = 6;
//            node6.Latitude = 0.003;
//            node6.Longitude = 0.003;
//            SimpleNode node7 = new SimpleNode();
//            node7.Id = 7;
//            node7.Latitude = 0.003;
//            node7.Longitude = 0;

//            pre_processor.Process(node5, SimpleChangeType.Create);
//            pre_processor.Process(node6, SimpleChangeType.Create);
//            pre_processor.Process(node7, SimpleChangeType.Create);

//            SimpleWay way2 = new SimpleWay();
//            way2.Id = 2;
//            way2.Nodes = new List<long>();
//            way2.Nodes.Add(2);
//            way2.Nodes.Add(5);
//            way2.Tags = new Dictionary<string, string>();
//            way2.Tags["highway"] = "residential";
//            pre_processor.Process(way2, SimpleChangeType.Create);

//            SparseVertex sparse_vertex2 = data.GetSparseVertex(2);
//            Assert.AreEqual(3, sparse_vertex2.Neighbours.Length);

//            SparseVertexNeighbour sparse_vertex2_neighbour5 = sparse_vertex2.GetSparseVertexNeighbour(5);
//            Assert.IsNotNull(sparse_vertex2_neighbour5);
//            Assert.AreEqual(0, sparse_vertex2_neighbour5.Nodes.Length);

//            foreach (SparseVertexNeighbour neighbour in sparse_vertex2.Neighbours)
//            {
//                if (neighbour.Id == 1)
//                {
//                    Assert.IsNotNull(neighbour.Nodes.Length == 0 || neighbour.Nodes.Length == 2);
//                    if (neighbour.Nodes.Length == 2)
//                    {
//                        Assert.AreEqual(3, neighbour.Nodes[0]);
//                        Assert.AreEqual(4, neighbour.Nodes[1]);
//                    }
//                }
//            }

//            SimpleWay way3 = new SimpleWay();
//            way3.Id = 3;
//            way3.Nodes = new List<long>();
//            way3.Nodes.Add(3);
//            way3.Nodes.Add(6);
//            way3.Tags = new Dictionary<string, string>();
//            way3.Tags["highway"] = "residential";
//            pre_processor.Process(way3, SimpleChangeType.Create);

//            SparseVertex sparse_vertex3 = data.GetSparseVertex(3);
//            SparseVertexNeighbour sparse_vertex3_neighbour2 = sparse_vertex3.GetSparseVertexNeighbour(2);
//            Assert.IsNotNull(sparse_vertex3_neighbour2);

//            SparseVertexNeighbour sparse_vertex3_neighbour1 = sparse_vertex3.GetSparseVertexNeighbour(1);
//            Assert.IsNotNull(sparse_vertex3_neighbour1);
//            Assert.AreEqual(1, sparse_vertex3_neighbour1.Nodes.Length);
//            Assert.AreEqual(4, sparse_vertex3_neighbour1.Nodes[0]);

//            SparseVertexNeighbour sparse_vertex3_neighbour6 = sparse_vertex3.GetSparseVertexNeighbour(6);
//            Assert.IsNotNull(sparse_vertex3_neighbour6);

//            SimpleWay way4 = new SimpleWay();
//            way4.Id = 1;
//            way4.Nodes = new List<long>();
//            way4.Nodes.Add(4);
//            way4.Nodes.Add(7);
//            way4.Tags = new Dictionary<string, string>();
//            way4.Tags["highway"] = "residential";
//            pre_processor.Process(way4, SimpleChangeType.Create);

//            SparseVertex sparse_vertex4 = data.GetSparseVertex(4);
//            Assert.IsNotNull(sparse_vertex4);
//            Assert.AreEqual(3, sparse_vertex4.Neighbours.Length);

//            SparseVertexNeighbour sparse_vertex4_neighbour3 = sparse_vertex4.GetSparseVertexNeighbour(3);
//            Assert.IsNotNull(sparse_vertex4_neighbour3);
//            Assert.AreEqual(3, sparse_vertex4_neighbour3.Id);
//            Assert.AreEqual(0, sparse_vertex4_neighbour3.Nodes.Length);

//            SparseVertexNeighbour sparse_vertex4_neighbour7 = sparse_vertex4.GetSparseVertexNeighbour(7);
//            Assert.IsNotNull(sparse_vertex4_neighbour7);
//            Assert.AreEqual(7, sparse_vertex4_neighbour7.Id);
//            Assert.AreEqual(0, sparse_vertex4_neighbour7.Nodes.Length);

//            SparseVertexNeighbour sparse_vertex4_neighbour1 = sparse_vertex4.GetSparseVertexNeighbour(1);
//            Assert.IsNotNull(sparse_vertex4_neighbour1);
//            Assert.AreEqual(1, sparse_vertex4_neighbour1.Id);
//            Assert.AreEqual(0, sparse_vertex4_neighbour1.Nodes.Length);

//            sparse_vertex1 = data.GetSparseVertex(1);
//            Assert.AreEqual(2, sparse_vertex1.Neighbours.Length);

//            SparseVertexNeighbour sparse_vertex1_neighbour2 = sparse_vertex1.GetSparseVertexNeighbour(2);
//            Assert.IsNotNull(sparse_vertex1_neighbour2);
//            Assert.AreEqual(2, sparse_vertex1_neighbour2.Id);
//            Assert.AreEqual(0, sparse_vertex1_neighbour2.Nodes.Length);

//            SparseVertexNeighbour sparse_vertex1_neighbour4 = sparse_vertex1.GetSparseVertexNeighbour(4);
//            Assert.IsNotNull(sparse_vertex1_neighbour4);
//            Assert.AreEqual(4, sparse_vertex1_neighbour4.Id);
//            Assert.AreEqual(0, sparse_vertex1_neighbour4.Nodes.Length);

//        }

//        /// <summary>
//        /// Does a sparse test on a t-junction.
//        /// 
//        /// Tests the neighbours of the node at the junction.
//        /// </summary>
//        /// <param name="data"></param>
//        private static void SparseTestT(ISparseData data)
//        {
//            // instantiate the pre-processor.
//            SparsePreProcessor pre_processor = new SparsePreProcessor(data);

//            // add two nodes.
//            SimpleNode node1 = new SimpleNode();
//            node1.Id = 1;
//            node1.Latitude = 0;
//            node1.Longitude = 0.001;
//            SimpleNode node2 = new SimpleNode();
//            node2.Id = 2;
//            node2.Latitude = 0.001;
//            node2.Longitude = 0.001;
//            SimpleNode node3 = new SimpleNode();
//            node3.Id = 3;
//            node3.Latitude = 0.002;
//            node3.Longitude = 0.001;
//            SimpleNode node4 = new SimpleNode();
//            node4.Id = 4;
//            node4.Latitude = 0.002;
//            node4.Longitude = 0.002;
//            SimpleNode node5 = new SimpleNode();
//            node5.Id = 5;
//            node5.Latitude = 0.002;
//            node5.Longitude = 0;

//            pre_processor.Process(node1, SimpleChangeType.Create);
//            pre_processor.Process(node2, SimpleChangeType.Create);
//            pre_processor.Process(node3, SimpleChangeType.Create);
//            pre_processor.Process(node4, SimpleChangeType.Create);
//            pre_processor.Process(node5, SimpleChangeType.Create);

//            SimpleWay way1 = new SimpleWay();
//            way1.Id = 1;
//            way1.Nodes = new List<long>();
//            way1.Nodes.Add(4);
//            way1.Nodes.Add(3);
//            way1.Nodes.Add(5);
//            way1.Tags = new Dictionary<string, string>();
//            way1.Tags["highway"] = "residential";

//            SimpleWay way2 = new SimpleWay();
//            way2.Id = 2;
//            way2.Nodes = new List<long>();
//            way2.Nodes.Add(1);
//            way2.Nodes.Add(2);
//            way2.Nodes.Add(3);
//            way2.Tags = new Dictionary<string, string>();
//            way2.Tags["highway"] = "residential";

//            pre_processor.Process(way1, SimpleChangeType.Create);
//            pre_processor.Process(way2, SimpleChangeType.Create);

//            SparseVertex sparse_vertex3 = data.GetSparseVertex(3);
//            Assert.IsNotNull(sparse_vertex3);
//            Assert.AreEqual(3, sparse_vertex3.Neighbours.Length);
//        }

//        #endregion

//        #region Sparse Data Factory

//        internal interface ISparseDataFactory
//        {
//            ISparseData CreateData();
//        }

//        internal class MemorySparseDataFactory : ISparseDataFactory
//        {
//            public ISparseData CreateData()
//            {
//                return new MemorySparseData();
//            }
//        }

//        internal class RedisSparseDataFactory : ISparseDataFactory
//        {
//            public ISparseData CreateData()
//            {
//                return new RedisSparseData();
//            }
//        }

//        #endregion
//    }
//}

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osm.Data.Core.CH;
using Osm.Data.Core.CH.Memory;
using Osm.Data.Core.CH.Primitives;
using Osm.Data.XML.Raw.Processor;
using Osm.Routing.CH.PreProcessing;
using Osm.Routing.CH.PreProcessing.Ordering;
using Osm.Routing.CH.PreProcessing.Witnesses;
using Osm.Routing.CH.Routing;
using Osm.Routing.CH.Processor;
using Osm.Routing.Core.Route;
using Tools.Math.Geo;
using System.Data;
using Osm.Data.Core.Sparse;
using Osm.Routing.Sparse.Processor;
using Osm.Routing.Sparse.PreProcessor;
using Osm.Data.Raw.XML.OsmSource;
using Tools.Xml.Sources;
using Osm.Routing.Core;
using Osm.Routing.Raw.Graphs.Interpreter;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Data.Oracle.CH;
using Osm.Data.Core.Processor.Progress;

namespace Osm.Routing.Test.CH
{
    public static class CHTests
    {

        internal static void DoTests()
        {
            //// test the infrastructure first.
            //Console.WriteLine("Intrastructure Tests: Started @ {0}", DateTime.Now.ToLongTimeString());
            //CHTests.TestCHInfrastructure();
            //Console.WriteLine("Intrastructure Tests: Done @ {0}", DateTime.Now.ToLongTimeString());

            //// test the pre-processing bit.
            //Console.WriteLine("Pre-Processing Tests: Started @ {0}", DateTime.Now.ToLongTimeString());
            //CHTests.CHTestMemoryProcessing();
            //Console.WriteLine("Pre-Processing Tests: Done @ {0}", DateTime.Now.ToLongTimeString());

            //// test the actual routing.
            //Console.WriteLine("Routing Tests: Started @ {0}", DateTime.Now.ToLongTimeString());
            //CHTests.DoRoutingTests();
            //Console.WriteLine("Routing Tests: Done @ {0}", DateTime.Now.ToLongTimeString());

            // test the actual routing.
            Console.WriteLine("Routing Peformance Tests: Started @ {0}", DateTime.Now.ToLongTimeString());
            CHTests.DoRoutingPerformanceTests();
            Console.WriteLine("Routing Peformance Tests: Done @ {0}", DateTime.Now.ToLongTimeString());


            Console.WriteLine("Testing Done!");
            Console.ReadLine();
        }

        #region Routing Tests

        internal static void DoRoutingTests()
        {
            CHTests.RoutingTestTiny();
            CHTests.RoutingTestWechel();

            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            //Sparse.SparseTests.RoutingTest(new Sparse.SparseTests.MemorySparseDataFactory(),
            //    string.Format("{0}\\Sparse\\{1}.osm", info.FullName, "bug"));

            CHTests.RoutingTest(new CHTests.MemoryCHDataFactory(),
                string.Format("{0}\\CH\\{1}.osm", info.FullName, "wechel"));

            CHTests.RoutingTest(new CHTests.MemoryCHDataFactory(),
                string.Format("{0}\\CH\\{1}.osm", info.FullName, "matrix"),
                string.Format("{0}\\CH\\{1}.csv", info.FullName, "matrix"));

            CHTests.RoutingTest(new CHTests.MemoryCHDataFactory(),
                string.Format("{0}\\CH\\{1}.osm", info.FullName, "matrix"),
                string.Format("{0}\\CH\\{1}.csv", info.FullName, "matrix"));

            //CHTests.RoutingTest(new CHTests.MemoryCHDataFactory(),
            //    string.Format("{0}\\CH\\{1}.osm", info.FullName, "moscow"),
            //    string.Format("{0}\\CH\\{1}.csv", info.FullName, "moscow"));
        }

        internal static void RoutingTestWechel()
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            ICHDataFactory data_factory = new MemoryCHDataFactory();
            string xml = string.Format("{0}\\CH\\{1}.osm", info.FullName, "wechel");

            Console.WriteLine(string.Format("Testing routing: {0}", xml));

            // save the data.
            ICHData data = data_factory.CreateData();
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);
            INodeWeightCalculator calculator = new EdgeDifference(
                witness_calculator);
            CHPreProcessor pre_processor = new CHPreProcessor(data, calculator, witness_calculator);
            XmlDataProcessorSource source = new XmlDataProcessorSource(xml);
            CHDataProcessorTarget target = new CHDataProcessorTarget(
                pre_processor);
            target.RegisterSource(source);
            target.Pull();

            // test some routes.
            Router router = new Router(data);

            CHVertex start = data.GetCHVertex(471625996);
            CHVertex stop = data.GetCHVertex(291738780);
            OsmSharpRoute route1 = router.Calculate(start, stop);
            route1.SaveAsGpx(new FileInfo("wechel1.gpx"));

            start = data.GetCHVertex(665820709);
            stop = data.GetCHVertex(1494149672);
            OsmSharpRoute route2 = router.Calculate(start, stop);
            route2.SaveAsGpx(new FileInfo("wechel2.gpx"));
        }

        internal static void RoutingTestTiny()
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            ICHDataFactory data_factory = new MemoryCHDataFactory();
            string xml = string.Format("{0}\\CH\\{1}.osm", info.FullName, "tiny");

            Console.WriteLine(string.Format("Testing routing: {0}", xml));

            // save the data.
            ICHData data = data_factory.CreateData();
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);
            INodeWeightCalculator calculator = new EdgeDifference(
                witness_calculator);
            CHPreProcessor pre_processor = new CHPreProcessor(data, calculator, witness_calculator);
            XmlDataProcessorSource source = new XmlDataProcessorSource(xml);
            CHDataProcessorTarget target = new CHDataProcessorTarget(
                pre_processor);
            target.RegisterSource(source);
            target.Pull();

            // test some routes.
            Router router = new Router(data);
            CHVertex start = data.GetCHVertex(471625996);
            CHVertex stop = data.GetCHVertex(291738780);
            OsmSharpRoute route = router.Calculate(start, stop);
            route.SaveAsGpx(new FileInfo("tiny.gpx"));
        }

        internal static void RoutingTest(ICHDataFactory data_factory, string xml)
        {
            Console.WriteLine(string.Format("Testing routing: {0}", xml));

            // save the data.
            ICHData data = data_factory.CreateData();
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);
            INodeWeightCalculator calculator = new EdgeDifference(
                new DykstraWitnessCalculator(data, 10));
            CHPreProcessor pre_processor = new CHPreProcessor(data, calculator, witness_calculator);
            XmlDataProcessorSource source = new XmlDataProcessorSource(xml);
            CHDataProcessorTarget target = new CHDataProcessorTarget(
                pre_processor);
            target.RegisterSource(source);
            target.Pull();

            // try routing the data.
            Router router = new Router(data);
            CHVertex start = router.Resolve(new GeoCoordinate(51.2679954, 4.801928));
            CHVertex stop = router.Resolve(new GeoCoordinate(51.2610122, 4.7807138));
            OsmSharpRoute route = router.Calculate(start, stop);

        }

        internal static void RoutingTest(ICHDataFactory data_factory, string xml, string file)
        {
            Console.WriteLine(string.Format("Testing routing: {0}:{1}", xml, file));

            // save the data.
            ICHData data = data_factory.CreateData();
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);
            INodeWeightCalculator calculator = new EdgeDifferenceContracted(
                new DykstraWitnessCalculator(data, 10));
            CHPreProcessor pre_processor = new CHPreProcessor(data, calculator, witness_calculator);
            XmlDataProcessorSource source = new XmlDataProcessorSource(xml);
            CHDataProcessorTarget target = new CHDataProcessorTarget(
                pre_processor);
            //ProgressDataProcessorTarget progress_target = new ProgressDataProcessorTarget(
            //    target);
            target.RegisterSource(source);
            target.Pull();

            // resolve points from matrix.
            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            DataSet csv = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new System.IO.FileInfo(file), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
            foreach (DataRow row in csv.Tables[0].Rows)
            {
                // be carefull with the parsing and the number formatting for different cultures.
                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
                coordinates.Add(point);
            }

            // try routing the data.
            Router router = new Router(data);
            CHVertex[] vertices = router.Resolve(coordinates.ToArray());
            float[][] weights = router.CalculateManyToManyWeight(vertices, vertices);
            //OsmSharpRoute route;
            //for (int x = 0; x < vertices.Length; x++)
            //{
            //    for (int y = 0; y < vertices.Length; y++)
            //    {
            //        route = router.Calculate(vertices[x], vertices[y]);
            //        //if (route != null)
            //        //{
            //        //    route.SaveAsGpx(new FileInfo(string.Format("route_{0}_{1}.gpx",
            //        //        x, y)));
            //        //}
            //    }
            //}
        }

        #endregion

        #region Routing Performance Tests

        public static void DoRoutingPerformanceTests()
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            FileInfo output_file = new FileInfo("route_performances.txt");
            StreamWriter output = output_file.AppendText();

            CHTests.DoRoutingPerformanceTests(new Sparse.SparseTests.MemorySparseDataFactory(),
                new MemoryCHDataFactory(),
                string.Format("{0}\\CH\\{1}.osm", info.FullName, "DM101"),
                string.Format("{0}\\CH\\{1}.csv", info.FullName, "DM101"), 1,
                output, false, true, true);

            CHTests.DoRoutingPerformanceTests(new Sparse.SparseTests.MemorySparseDataFactory(),
                new MemoryCHDataFactory(),
                string.Format("{0}\\CH\\{1}.osm", info.FullName, "DM103"),
                string.Format("{0}\\CH\\{1}.csv", info.FullName, "DM103"), 1,
                output, false, true, true);

            //CHTests.DoRoutingPerformanceTests(new Sparse.SparseTests.MemorySparseDataFactory(),
            //    new MemoryCHDataFactory(),
            //    string.Format("{0}\\CH\\{1}.osm", info.FullName, "matrix"),
            //    string.Format("{0}\\CH\\{1}.csv", info.FullName, "matrix"), 1,
            //    output, true, true, true);

            //CHTests.DoRoutingPerformanceTests(new Sparse.SparseTests.MemorySparseDataFactory(),
            //    new MemoryCHDataFactory(),
            //    string.Format("{0}\\CH\\{1}.osm", info.FullName, "matrix_big_area"),
            //    string.Format("{0}\\CH\\{1}.csv", info.FullName, "matrix_big_area"), 1,
            //    output, true, true, true);

            //CHTests.DoRoutingPerformanceTests(new Sparse.SparseTests.MemorySparseDataFactory(),
            //    new MemoryCHDataFactory(),
            //    string.Format("{0}\\CH\\{1}.osm", info.FullName, "eeklo"),
            //    string.Format("{0}\\CH\\{1}.csv", info.FullName, "eeklo"), 1,
            //    output, true, true, true);

            //CHTests.DoRoutingPerformanceTests(new Sparse.SparseTests.MemorySparseDataFactory(),
            //    new MemoryCHDataFactory(),
            //    string.Format("{0}\\CH\\{1}.osm", info.FullName, "lebbeke"),
            //    string.Format("{0}\\CH\\{1}.csv", info.FullName, "lebbeke"), 1,
            //    output, true, true, true);

            //CHTests.DoRoutingPerformanceTests(new Sparse.SparseTests.MemorySparseDataFactory(),
            //    new MemoryCHDataFactory(),
            //    string.Format("{0}\\CH\\{1}.osm", info.FullName, "moscow"),
            //    string.Format("{0}\\CH\\{1}.csv", info.FullName, "moscow"), 1,
            //    output, true, true, true);

        }

        /// <summary>
        /// Do performance tests on an OSM area.
        /// </summary>
        /// <param name="sparse_data_factory"></param>
        /// <param name="xml"></param>
        private static void DoRoutingPerformanceTests(Osm.Routing.Test.Sparse.SparseTests.ISparseDataFactory sparse_data_factory, 
            ICHDataFactory data_factory, string xml, string file, int test_count, StreamWriter output, 
            bool raw, bool sparse, bool ch)
        {
            Console.WriteLine("Routing performance test: {0}", new FileInfo(xml).Name);
            output.WriteLine("Routing performance test: {0}", new FileInfo(xml).Name);

            // do the tests.
            long before_processing;
            long start;
            long stop;
            long end_resolve;
            //int test_count = 10;

            if (raw)
            {
                //// the raw tests.
                //Console.Write("Raw:");
                //before_processing = DateTime.Now.Ticks;
                //// create the raw router.
                //OsmDataSource osm_data = new OsmDataSource(
                //    new Osm.Core.Xml.OsmDocument(new XmlFileSource(xml)));
                //Osm.Routing.Raw.Router raw_router = new Osm.Routing.Raw.Router(osm_data,
                //    new GraphInterpreterTime(osm_data, VehicleEnum.Car));

                //start = DateTime.Now.Ticks;
                //Raw.ResolvedPoint[] raw_points = CHTests.DoRoutingPerformanceTestsRawResolve(raw_router, file);
                //end_resolve = DateTime.Now.Ticks;
                //List<Osm.Core.Node> raw_nodes = new List<Osm.Core.Node>(osm_data.GetNodes());
                //for (int idx = 0; idx < test_count; idx++)
                //{
                //    DoRoutingPerformanceTestsRaw(raw_router, raw_points);
                //}
                //stop = DateTime.Now.Ticks;
                //Console.WriteLine("{0} ({1}/route) ({2} routes) ({3} resolved) ({4} preprocessing)",
                //    new TimeSpan((stop - end_resolve) / test_count).TotalSeconds.ToString(),
                //    new TimeSpan((stop - end_resolve) / (test_count * raw_points.Length)).TotalSeconds.ToString(),
                //    raw_points.Length,
                //    new TimeSpan((end_resolve - start) / test_count).TotalSeconds.ToString(),
                //    new TimeSpan((start - before_processing) / test_count).TotalSeconds.ToString());
                //output.WriteLine("{0} ({1}/route) ({2} routes) ({3} resolved) ({4} preprocessing)",
                //    new TimeSpan((stop - end_resolve) / test_count).TotalSeconds.ToString(),
                //    new TimeSpan((stop - end_resolve) / (test_count * raw_points.Length)).TotalSeconds.ToString(),
                //    raw_points.Length,
                //    new TimeSpan((end_resolve - start) / test_count).TotalSeconds.ToString(),
                //    new TimeSpan((start - before_processing) / test_count).TotalSeconds.ToString());
            }

            if (sparse)
            {
                // the sparse tests.
                Console.Write("Sparse:");
                output.Write("Sparse:");
                before_processing = DateTime.Now.Ticks;
                // create the sparse router.     
                ISparseData sparse_data = sparse_data_factory.CreateData();
                XmlDataProcessorSource sparse_source = new XmlDataProcessorSource(xml);
                SparseDataProcessorTarget sparse_target = new SparseDataProcessorTarget(
                    new SparsePreProcessor(sparse_data));
                sparse_target.RegisterSource(sparse_source);
                sparse_target.Pull();
                List<long> sparse_nodes = new List<long>(sparse_target.ProcessedNodes);
                Osm.Routing.Sparse.Routing.Router sparse_router = new Osm.Routing.Sparse.Routing.Router(sparse_data);
                start = DateTime.Now.Ticks;
                SparseVertex[] sparse_points = CHTests.DoRoutingPerformanceTestsSparseResolve(sparse_router, file);
                end_resolve = DateTime.Now.Ticks;
                for (int idx = 0; idx < test_count; idx++)
                {
                    DoRoutingPerformanceTestsSparse(sparse_router, sparse_points);
                    //Console.Write(".");
                }
                stop = DateTime.Now.Ticks;
                Console.WriteLine("{0} ({1}/route) ({2} routes) ({3} resolved) ({4} preprocessing)",
                    new TimeSpan((stop - end_resolve) / test_count).TotalSeconds.ToString(),
                    new TimeSpan((stop - end_resolve) / (test_count * (sparse_points.Length * sparse_points.Length))).TotalSeconds.ToString(),
                    sparse_points.Length,
                    new TimeSpan((end_resolve - start) / test_count).TotalSeconds.ToString(),
                    new TimeSpan((start - before_processing) / test_count).TotalSeconds.ToString());
                output.WriteLine("{0} ({1}/route) ({2} routes) ({3} resolved) ({4} preprocessing)",
                    new TimeSpan((stop - end_resolve) / test_count).TotalSeconds.ToString(),
                    new TimeSpan((stop - end_resolve) / (test_count * (sparse_points.Length * sparse_points.Length))).TotalSeconds.ToString(),
                    sparse_points.Length,
                    new TimeSpan((end_resolve - start) / test_count).TotalSeconds.ToString(),
                    new TimeSpan((start - before_processing) / test_count).TotalSeconds.ToString());
            }

            if (ch)
            {
                // the ch tests.
                Console.Write("CH:");
                output.Write("CH:");
                before_processing = DateTime.Now.Ticks;
                // create the CH router.
                ICHData data = data_factory.CreateData();
                INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data, 500);
                INodeWeightCalculator calculator = new EdgeDifferenceContracted(
                    witness_calculator);
                CHPreProcessor pre_processor = new CHPreProcessor(data, calculator, witness_calculator);
                XmlDataProcessorSource source = new XmlDataProcessorSource(xml);
                CHDataProcessorTarget target = new CHDataProcessorTarget(
                    pre_processor);
                target.RegisterSource(source);
                target.Pull();
                Router ch_router = new Router(data);
                start = DateTime.Now.Ticks;
                CHVertex[] ch_points = CHTests.DoRoutingPerformanceTestsCHResolve(ch_router, file);
                end_resolve = DateTime.Now.Ticks;
                for (int idx = 0; idx < test_count; idx++)
                {
                    DoRoutingPerformanceTestsCH(ch_router, ch_points);
                    //Console.Write(".");
                }
                stop = DateTime.Now.Ticks;
                Console.WriteLine("{0} ({1}/route) ({2} routes) ({3} resolved) ({4} preprocessing)",
                    new TimeSpan((stop - end_resolve) / test_count).TotalSeconds.ToString(),
                    new TimeSpan((stop - end_resolve) / (test_count * (ch_points.Length * ch_points.Length))).TotalSeconds.ToString(),
                    ch_points.Length,
                    new TimeSpan((end_resolve - start) / test_count).TotalSeconds.ToString(),
                    new TimeSpan((start - before_processing) / test_count).TotalSeconds.ToString());
                output.WriteLine("{0} ({1}/route) ({2} routes) ({3} resolved) ({4} preprocessing)",
                    new TimeSpan((stop - end_resolve) / test_count).TotalSeconds.ToString(),
                    new TimeSpan((stop - end_resolve) / (test_count * (ch_points.Length * ch_points.Length))).TotalSeconds.ToString(),
                    ch_points.Length,
                    new TimeSpan((end_resolve - start) / test_count).TotalSeconds.ToString(),
                    new TimeSpan((start - before_processing) / test_count).TotalSeconds.ToString());
            }

            output.Flush();
        }

        #region Raw Tests

        private static void DoRoutingPerformanceTestsRaw(Osm.Routing.Raw.Router router, string file)
        {
            CHTests.DoRoutingPerformanceTestsRaw(router,
                CHTests.DoRoutingPerformanceTestsRawResolve(router, file));
        }

        private static Raw.ResolvedPoint[] DoRoutingPerformanceTestsRawResolve(Osm.Routing.Raw.Router router, string file)
        {
            // resolve points from matrix.
            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            DataSet csv = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new System.IO.FileInfo(file), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
            foreach (DataRow row in csv.Tables[0].Rows)
            {
                // be carefull with the parsing and the number formatting for different cultures.
                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
                coordinates.Add(point);
            }

            // try routing the data.
            return router.Resolve(coordinates.ToArray());
            //float[][] weights = router.CalculateManyToManyWeight(vertices, vertices);

        }

        private static void DoRoutingPerformanceTestsRaw(Osm.Routing.Raw.Router router, 
            Raw.ResolvedPoint[] vertices)
        {
            //router.CalculateManyToManyWeight(vertices, vertices);

            //for (int idx = 0; idx < vertices.Length; idx++)
            //{
            //    Raw.ResolvedPoint from = vertices[0];
            //    Raw.ResolvedPoint to = vertices[idx];

            //    try
            //    {
            //        OsmSharpRoute route = router.Calculate(from, to);
            //        //Console.Write("-");
            //    }
            //    catch (Exception ex)
            //    {
            //        //Console.WriteLine(ex.Message);
            //    }
            //}
        }

        #endregion

        #region Sparse Tests

        private static void DoRoutingPerformanceTestsSparse(Osm.Routing.Sparse.Routing.Router router, SparseVertex[] vertices)
        {
            router.CalculateManyToManyWeight(vertices, vertices);

            //for (int idx = 0; idx < vertices.Length; idx++)
            //{
            //    SparseVertex from = vertices[0];
            //    SparseVertex to = vertices[idx];

            //    try
            //    {
            //        OsmSharpRoute route = router.Calculate(from, to);
            //        //Console.Write("-");
            //    }
            //    catch (Exception ex)
            //    {
            //        //Console.WriteLine(ex.Message);
            //    }
            //}
        }

        private static void DoRoutingPerformanceTestsSparse(Osm.Routing.Sparse.Routing.Router router, string file)
        {
            CHTests.DoRoutingPerformanceTestsSparse(router,
                CHTests.DoRoutingPerformanceTestsSparseResolve(router, file));
        }

        private static SparseVertex[] DoRoutingPerformanceTestsSparseResolve(Osm.Routing.Sparse.Routing.Router router, string file)
        {
            // resolve points from matrix.
            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            DataSet csv = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new System.IO.FileInfo(file), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
            foreach (DataRow row in csv.Tables[0].Rows)
            {
                // be carefull with the parsing and the number formatting for different cultures.
                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
                coordinates.Add(point);
            }

            // try routing the data.
            return router.Resolve(coordinates.ToArray());
        }

        #endregion

        #region CH Tests

        private static CHVertex[] DoRoutingPerformanceTestsCHResolve(Router router, string file)
        {
            // resolve points from matrix.
            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            DataSet csv = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new System.IO.FileInfo(file), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
            foreach (DataRow row in csv.Tables[0].Rows)
            {
                // be carefull with the parsing and the number formatting for different cultures.
                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
                coordinates.Add(point);
            }

            // try routing the data.
            return router.Resolve(coordinates.ToArray());
        }

        private static void DoRoutingPerformanceTestsCH(Router router, string file)
        {
            // try routing the data.
            CHTests.DoRoutingPerformanceTestsCH(router, 
                CHTests.DoRoutingPerformanceTestsCHResolve(router, file));            
        }

        private static void DoRoutingPerformanceTestsCH(Router router, CHVertex[] vertices)
        {
            router.CalculateManyToManyWeight(vertices, vertices);

            //for (int idx = 0; idx < vertices.Length; idx++)
            //{
            //    CHVertex from = vertices[0];
            //    CHVertex to = vertices[idx];

            //    try
            //    {
            //        OsmSharpRoute route = router.Calculate(from, to);
            //        //Console.Write("-");
            //    }
            //    catch (Exception ex)
            //    {
            //        //Console.WriteLine(ex.Message);
            //    }
            //}
        }

        #endregion

        #endregion

        #region CH Infrastructure

        /// <summary>
        /// Tests the CH infrastructure.
        /// </summary>
        public static void TestCHInfrastructure()
        {
            ICHDataFactory data_factory = new MemoryCHDataFactory();

            CHTests.TestCHPathSegment();
            CHTests.TestEdgeDifference(data_factory.CreateData());
        }

        /// <summary>
        /// Tests the edge difference calculator.
        /// </summary>
        /// <param name="data"></param>
        private static void TestEdgeDifference(ICHData data)
        {
            List<long> nodes = CHTests.BuildSimpleDenseNetwork(data); // build the network.

            // test the edge difference calculations.
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);
            INodeWeightCalculator weight_calculator = new EdgeDifference(
                witness_calculator);

            CHTests.TestWeightCalculation(weight_calculator, data, 1, -4, 0);
            CHTests.TestWeightCalculation(weight_calculator, data, 2, -2, 0);
            CHTests.TestWeightCalculation(weight_calculator, data, 3, -4, 0);
            CHTests.TestWeightCalculation(weight_calculator, data, 4, -4, 0);
            CHTests.TestWeightCalculation(weight_calculator, data, 5, 0, 0);
            CHTests.TestWeightCalculation(weight_calculator, data, 6, -6, 0);
            CHTests.TestWeightCalculation(weight_calculator, data, 7, -8, 0);
            CHTests.TestWeightCalculation(weight_calculator, data, 8, -6, 0);
            CHTests.TestWeightCalculation(weight_calculator, data, 9, -4, 0);

            // do some pre-processing.
            CHPreProcessor preprocessor = new CHPreProcessor(data,
                weight_calculator, witness_calculator);
            preprocessor.Enqueue(nodes.GetEnumerator());

            // test the queue.
            List<long> peek = preprocessor.Queue.PeekAll();
            Assert.AreEqual(7, preprocessor.Queue.Peek());
            Assert.AreEqual(1, peek.Count);
            Assert.IsTrue(peek.Contains(7));

            long next = preprocessor.SelectNext();
            preprocessor.Queue.Remove(next);
            preprocessor.Contract(next);

            // test current level.
            Assert.AreEqual(2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 1, -4, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 2, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 3, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 4, -4, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 5, 0, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 6, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 7, -8, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 8, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 9, -2, preprocessor.Level);

            //// select the next vertex.
            //next = preprocessor.SelectNext();

            //// test the queue.
            //peek = preprocessor.Queue.PeekAll();
            //Assert.AreEqual(2, peek.Count);
            //Assert.IsTrue(peek.Contains(1));
            //Assert.IsTrue(peek.Contains(4));

            // contract the vertex.
            preprocessor.Contract(1);

            // test current level.
            Assert.AreEqual(3, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 1, -4, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 2, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 3, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 4, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 5, 0, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 6, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 7, -8, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 8, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 9, -2, preprocessor.Level);

            // contract the vertex.
            preprocessor.Contract(2);

            // test current level.
            Assert.AreEqual(4, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 1, -4, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 2, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 3, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 4, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 5, 0, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 6, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 7, -8, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 8, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 9, -2, preprocessor.Level);

            // contract the vertex.
            preprocessor.Contract(3);

            // test current level.
            Assert.AreEqual(5, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 1, -4, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 2, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 3, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 4, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 5, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 6, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 7, -8, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 8, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 9, -4, preprocessor.Level);

            // contract the vertex.
            preprocessor.Contract(9);

            // test current level.
            Assert.AreEqual(6, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 1, -4, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 2, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 3, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 4, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 5, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 6, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 7, -8, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 8, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 9, -4, preprocessor.Level);

            // contract the vertex.
            preprocessor.Contract(4);

            // test current level.
            Assert.AreEqual(7, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 1, -4, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 2, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 3, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 4, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 5, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 6, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 7, -8, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 8, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 9, -4, preprocessor.Level);

            // contract the vertex.
            preprocessor.Contract(5);

            // test current level.
            Assert.AreEqual(8, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 1, -4, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 2, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 3, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 4, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 5, -2, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 6, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 7, -8, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 8, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 9, -4, preprocessor.Level);

            // contract the vertex.
            preprocessor.Contract(6);

            // test current level.
            Assert.AreEqual(9, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 1, -4, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 2, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 3, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 4, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 5, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 6, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 7, -8, preprocessor.Level);
            CHTests.TestWeightCalculation(weight_calculator, data, 8, 0, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 9, -4, preprocessor.Level);

            // contract the vertex.
            preprocessor.Contract(8);

            // test current level.
            Assert.AreEqual(10, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 1, -4, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 2, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 3, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 4, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 5, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 6, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 7, -8, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 8, -2, preprocessor.Level);
            //CHTests.TestWeightCalculation(weight_calculator, data, 9, -4, preprocessor.Level);
        }

        /// <summary>
        /// Tests the path segment functionalities.
        /// </summary>
        public static void TestCHPathSegment()
        {
            CHPathSegment segment = new CHPathSegment(1);
            segment = new CHPathSegment(2, 1, segment); // 1 -> 2

            Assert.AreEqual(1, segment.First().VertexId);
            Assert.AreEqual(2, segment.VertexId);
            Assert.AreEqual(2, segment.Length());

            CHPathSegment reverse_segment = segment.Reverse(); // 2 -> 1

            Assert.AreEqual(2, reverse_segment.First().VertexId);
            Assert.AreEqual(1, reverse_segment.VertexId);
            Assert.AreEqual(2, reverse_segment.Length());

            CHPathSegment second_segment = new CHPathSegment(2);
            second_segment = new CHPathSegment(3, 1, second_segment); // 2- > 3

            Assert.AreEqual(2, second_segment.First().VertexId);
            Assert.AreEqual(3, second_segment.VertexId);
            Assert.AreEqual(2, second_segment.Length());

            second_segment.ConcatenateAfter(segment); // 1 -> 2 -> 3
            Assert.AreEqual(3, second_segment.Length());
            Assert.AreEqual(1, second_segment.First().VertexId);
            Assert.AreEqual(3, second_segment.VertexId);
            Assert.AreEqual(2, second_segment.From.VertexId);
        }

        #endregion

        #region Pre-processing

        internal static void CHTestMemoryProcessing()
        {
            ICHDataFactory data_factory = new MemoryCHDataFactory();

            CHTests.CHTestVertex(data_factory.CreateData());
            CHTests.CHTestSmallNetwork(data_factory.CreateData());
            CHTests.CHTestSmallNetworkContracted(data_factory.CreateData());
            CHTests.CHTestSmallDenseNetworkContracted(data_factory.CreateData());
            CHTests.CHTestBiggerDenseNetworkContracted(data_factory.CreateData());
        }

        internal static void CHTestVertex(ICHData data)
        {
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);
            INodeWeightCalculator calculator = new EdgeDifference(
                witness_calculator);
            CHPreProcessor pre_processor = new CHPreProcessor(data, calculator, witness_calculator);

            CHVertex vertex1 = CHTests.CreateCHVertex(1);
            CHVertex vertex2 = CHTests.CreateCHVertex(2);
            CHVertex vertex3 = CHTests.CreateCHVertex(3);
            CHVertex vertex4 = CHTests.CreateCHVertex(4);

            CHTests.CreateCHArc(vertex2, vertex1, 1);
            CHTests.CreateCHArc(vertex3, vertex1, 1);
            CHTests.CreateCHArc(vertex4, vertex1, 1);

            data.PersistCHVertex(vertex1);
            data.PersistCHVertex(vertex2);
            data.PersistCHVertex(vertex3);
            data.PersistCHVertex(vertex4);
            
            int level = 0;
            double weight = calculator.Calculate(level, vertex1);

            //Assert.AreEqual(0, vertex1.ForwardNeighbours.Count);
            //Assert.AreEqual(3, vertex1.BackwardNeighbours.Count);
            Assert.AreEqual(-3, weight);
            //Assert.IsTrue(vertex1.BackwardNeighbours.Contains(arc1));
            //Assert.IsTrue(vertex1.BackwardNeighboursContains(arc2));
            //Assert.IsTrue(vertex1.BackwardNeighbours.Contains(arc3));

            CHTests.CreateCHArc(vertex1, vertex2, 1);
            CHTests.CreateCHArc(vertex1, vertex3, 1);
            CHTests.CreateCHArc(vertex1, vertex4, 1);

            data.PersistCHVertex(vertex1);
            data.PersistCHVertex(vertex2);
            data.PersistCHVertex(vertex3);
            data.PersistCHVertex(vertex4);

            level = 0;
            weight = calculator.Calculate(level, vertex1);

            //Assert.AreEqual(3, tos.Count);
            //Assert.AreEqual(3, froms.Count);
            Assert.AreEqual(0, weight);
            //Assert.IsTrue(froms.Contains(arc1));
            //Assert.IsTrue(froms.Contains(arc2));
            //Assert.IsTrue(froms.Contains(arc3));
            //Assert.IsTrue(tos.Contains(arc4));
            //Assert.IsTrue(tos.Contains(arc5));
            //Assert.IsTrue(tos.Contains(arc6));

            weight = calculator.Calculate(level, vertex2);

            //Assert.AreEqual(1, tos.Count);
            //Assert.AreEqual(1, froms.Count);
            Assert.AreEqual(-2, weight);
            //Assert.IsTrue(froms.Contains(arc4));
            //Assert.IsTrue(tos.Contains(arc1));

            weight = calculator.Calculate(level, vertex3);

            //Assert.AreEqual(1, tos.Count);
            //Assert.AreEqual(1, froms.Count);
            Assert.AreEqual(-2, weight);
            //Assert.IsTrue(froms.Contains(arc5));
            //Assert.IsTrue(tos.Contains(arc2));

            weight = calculator.Calculate(level, vertex4);

            //Assert.AreEqual(1, tos.Count);
            //Assert.AreEqual(1, froms.Count);
            Assert.AreEqual(-2, weight);
            //Assert.IsTrue(froms.Contains(arc6));
            //Assert.IsTrue(tos.Contains(arc3));

            // contract the node.
            pre_processor.Contract(2);

            level = 0;
            weight = calculator.Calculate(pre_processor.Level, vertex1);

            //Assert.AreEqual(2, tos.Count);
            //Assert.AreEqual(2, froms.Count);
            Assert.AreEqual(-2, weight);
            //Assert.IsTrue(froms.Contains(arc1));
            //Assert.IsTrue(froms.Contains(arc2));
            //Assert.IsTrue(froms.Contains(arc3));
            //Assert.IsTrue(tos.Contains(arc4));
            //Assert.IsTrue(tos.Contains(arc5));
            //Assert.IsTrue(tos.Contains(arc6));

            List<long> nodes = new List<long>();
            nodes.Add(2);
            nodes.Add(3);
            nodes.Add(4);
            nodes.Add(1);

            pre_processor.Start(nodes.GetEnumerator());
        }

        internal static void CHTestSmallNetwork(ICHData data)
        {
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);
            INodeWeightCalculator weight_calculator = new EdgeDifference(
                witness_calculator);

            CHVertex vertex1 = CHTests.CreateCHVertex(1);
            CHVertex vertex2 = CHTests.CreateCHVertex(2);
            CHVertex vertex3 = CHTests.CreateCHVertex(3);
            CHVertex vertex4 = CHTests.CreateCHVertex(4);
            CHVertex vertex5 = CHTests.CreateCHVertex(5);
            CHVertex vertex6 = CHTests.CreateCHVertex(6);
            CHVertex vertex7 = CHTests.CreateCHVertex(7);
            CHVertex vertex8 = CHTests.CreateCHVertex(8);
            CHVertex vertex9 = CHTests.CreateCHVertex(9);

            CHTests.CreateCHArc(vertex1, vertex2, 1);
            CHTests.CreateCHArc(vertex2, vertex1, 1);

            CHTests.CreateCHArc(vertex4, vertex2, 1);
            CHTests.CreateCHArc(vertex2, vertex4, 1);

            CHTests.CreateCHArc(vertex2, vertex5, 1);
            CHTests.CreateCHArc(vertex5, vertex2, 1);

            CHTests.CreateCHArc(vertex3, vertex5, 1);
            CHTests.CreateCHArc(vertex5, vertex3, 1);

            CHTests.CreateCHArc(vertex5, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex5, 1);

            CHTests.CreateCHArc(vertex6, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex6, 1);

            CHTests.CreateCHArc(vertex8, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex8, 1);

            CHTests.CreateCHArc(vertex9, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex9, 1);

            data.PersistCHVertex(vertex1);
            data.PersistCHVertex(vertex2);
            data.PersistCHVertex(vertex3);
            data.PersistCHVertex(vertex4);
            data.PersistCHVertex(vertex5);
            data.PersistCHVertex(vertex6);
            data.PersistCHVertex(vertex7);
            data.PersistCHVertex(vertex8);
            data.PersistCHVertex(vertex9);

            CHRouter router = new CHRouter(data);
            CHPathSegment route = router.Calculate(1, 9);

            List<long> nodes = new List<long>();
            nodes.Add(1);
            nodes.Add(2);
            nodes.Add(3);
            nodes.Add(4);
            nodes.Add(5);
            nodes.Add(6);
            nodes.Add(7);
            nodes.Add(8);
            nodes.Add(9);

            CHPreProcessor preprocessor = new CHPreProcessor(data, 
                weight_calculator, witness_calculator);
            preprocessor.Start(nodes.GetEnumerator());
        }

        internal static void CHTestSmallNetworkContracted(ICHData data)
        {
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);
            INodeWeightCalculator weight_calculator = new EdgeDifferenceContracted(
                witness_calculator);

            CHVertex vertex1 = CHTests.CreateCHVertex(1);
            CHVertex vertex2 = CHTests.CreateCHVertex(2);
            CHVertex vertex3 = CHTests.CreateCHVertex(3);
            CHVertex vertex4 = CHTests.CreateCHVertex(4);
            CHVertex vertex5 = CHTests.CreateCHVertex(5);
            CHVertex vertex6 = CHTests.CreateCHVertex(6);
            CHVertex vertex7 = CHTests.CreateCHVertex(7);
            CHVertex vertex8 = CHTests.CreateCHVertex(8);
            CHVertex vertex9 = CHTests.CreateCHVertex(9);

            data.PersistCHVertex(vertex1);
            data.PersistCHVertex(vertex2);
            data.PersistCHVertex(vertex3);
            data.PersistCHVertex(vertex4);
            data.PersistCHVertex(vertex5);
            data.PersistCHVertex(vertex6);
            data.PersistCHVertex(vertex7);
            data.PersistCHVertex(vertex8);
            data.PersistCHVertex(vertex9);

            CHTests.CreateCHArc(vertex1, vertex2, 1);
            CHTests.CreateCHArc(vertex2, vertex1, 1);

            CHTests.CreateCHArc(vertex4, vertex2, 1);
            CHTests.CreateCHArc(vertex2, vertex4, 1);

            CHTests.CreateCHArc(vertex2, vertex5, 1);
            CHTests.CreateCHArc(vertex5, vertex2, 1);

            CHTests.CreateCHArc(vertex3, vertex5, 1);
            CHTests.CreateCHArc(vertex5, vertex3, 1);

            CHTests.CreateCHArc(vertex5, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex5, 1);

            CHTests.CreateCHArc(vertex6, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex6, 1);

            CHTests.CreateCHArc(vertex8, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex8, 1);

            CHTests.CreateCHArc(vertex9, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex9, 1);


            data.PersistCHVertex(vertex1);
            data.PersistCHVertex(vertex2);
            data.PersistCHVertex(vertex3);
            data.PersistCHVertex(vertex4);
            data.PersistCHVertex(vertex5);
            data.PersistCHVertex(vertex6);
            data.PersistCHVertex(vertex7);
            data.PersistCHVertex(vertex8);
            data.PersistCHVertex(vertex9);

            CHRouter router = new CHRouter(data);
            CHPathSegment route_uncontracted = router.Calculate(1, 9);

            List<long> nodes = new List<long>();
            nodes.Add(1);
            nodes.Add(2);
            nodes.Add(3);
            nodes.Add(4);
            nodes.Add(5);
            nodes.Add(6);
            nodes.Add(7);
            nodes.Add(8);
            nodes.Add(9);

            CHPreProcessor preprocessor = new CHPreProcessor(data,
                weight_calculator, witness_calculator);
            preprocessor.Start(nodes.GetEnumerator());

            CHPathSegment route_contracted = router.Calculate(1, 9);
            CHPathSegment route1 = route_uncontracted;
            CHPathSegment route2 = route_contracted;
            while (route1.From != null)
            {
                Assert.AreEqual(route1.VertexId, route2.VertexId);
                Assert.AreEqual(route1.Weight, route2.Weight);

                route2 = route2.From;
                route1 = route1.From;
            }
        }

        internal static void CHTestSmallDenseNetworkContracted(ICHData data)
        {
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);
            INodeWeightCalculator weight_calculator = new EdgeDifferenceContracted(
                witness_calculator);

            CHVertex vertex1 = CHTests.CreateCHVertex(1);
            CHVertex vertex2 = CHTests.CreateCHVertex(2);
            CHVertex vertex3 = CHTests.CreateCHVertex(3);
            CHVertex vertex4 = CHTests.CreateCHVertex(4);
            CHVertex vertex5 = CHTests.CreateCHVertex(5);
            CHVertex vertex6 = CHTests.CreateCHVertex(6);
            CHVertex vertex7 = CHTests.CreateCHVertex(7);
            CHVertex vertex8 = CHTests.CreateCHVertex(8);
            CHVertex vertex9 = CHTests.CreateCHVertex(9);

            CHTests.CreateCHArc(vertex1, vertex2, 1);
            CHTests.CreateCHArc(vertex2, vertex1, 1);

            CHTests.CreateCHArc(vertex4, vertex2, 1);
            CHTests.CreateCHArc(vertex2, vertex4, 1);

            CHTests.CreateCHArc(vertex2, vertex5, 1);
            CHTests.CreateCHArc(vertex5, vertex2, 1);

            CHTests.CreateCHArc(vertex3, vertex5, 1);
            CHTests.CreateCHArc(vertex5, vertex3, 1);

            CHTests.CreateCHArc(vertex5, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex5, 1);

            CHTests.CreateCHArc(vertex6, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex6, 1);

            CHTests.CreateCHArc(vertex8, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex8, 1);

            CHTests.CreateCHArc(vertex9, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex9, 1);

            data.PersistCHVertex(vertex1);
            data.PersistCHVertex(vertex2);
            data.PersistCHVertex(vertex3);
            data.PersistCHVertex(vertex4);
            data.PersistCHVertex(vertex5);
            data.PersistCHVertex(vertex6);
            data.PersistCHVertex(vertex7);
            data.PersistCHVertex(vertex8);
            data.PersistCHVertex(vertex9);

            CHRouter router = new CHRouter(data);
            CHPathSegment route_uncontracted = router.Calculate(1, 9);


            List<long> nodes = new List<long>();
            nodes.Add(1);
            nodes.Add(2);
            nodes.Add(3);
            nodes.Add(4);
            nodes.Add(5);
            nodes.Add(6);
            nodes.Add(7);
            nodes.Add(8);
            nodes.Add(9);

            CHPreProcessor preprocessor = new CHPreProcessor(data,
                weight_calculator, witness_calculator);
            preprocessor.Start(nodes.GetEnumerator());

            CHPathSegment route_contracted = router.Calculate(1, 9);
            //CHPathSegment route1 = route_uncontracted;
            //CHPathSegment route2 = route_contracted;
            //while (route1.From != null)
            //{
            //    Assert.AreEqual(route1.VertexId, route2.VertexId);
            //    Assert.AreEqual(route1.Weight, route2.Weight);

            //    route2 = route2.From;
            //    route1 = route1.From;
            //}
            //Assert.IsNull(route2.From);
        }

        internal static void CHTestBiggerDenseNetworkContracted(ICHData data)
        {
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);
            INodeWeightCalculator weight_calculator = new EdgeDifferenceContracted(
                witness_calculator);

            CHVertex vertex1 = CHTests.CreateCHVertex(1);
            CHVertex vertex2 = CHTests.CreateCHVertex(2);
            CHVertex vertex3 = CHTests.CreateCHVertex(3);
            CHVertex vertex4 = CHTests.CreateCHVertex(4);
            CHVertex vertex5 = CHTests.CreateCHVertex(5);
            CHVertex vertex6 = CHTests.CreateCHVertex(6);
            CHVertex vertex7 = CHTests.CreateCHVertex(7);
            CHVertex vertex8 = CHTests.CreateCHVertex(8);
            CHVertex vertex9 = CHTests.CreateCHVertex(9);
            CHVertex vertex10 = CHTests.CreateCHVertex(10);
            CHVertex vertex11 = CHTests.CreateCHVertex(11);
            CHVertex vertex12 = CHTests.CreateCHVertex(12);
            CHVertex vertex13 = CHTests.CreateCHVertex(13);
            CHVertex vertex14 = CHTests.CreateCHVertex(14);
            CHVertex vertex15 = CHTests.CreateCHVertex(15);
            CHVertex vertex16 = CHTests.CreateCHVertex(16);

            CHTests.CreateCHArc(vertex1, vertex2, 1, null);
            CHTests.CreateCHArc(vertex1, vertex13, 1, null);
            CHTests.CreateCHArc(vertex2, vertex3, 1, null);
            CHTests.CreateCHArc(vertex3, vertex4, 1, null);
            CHTests.CreateCHArc(vertex3, vertex12, 1, null);
            CHTests.CreateCHArc(vertex4, vertex5, 1, null);
            CHTests.CreateCHArc(vertex5, vertex9, 1, null);
            CHTests.CreateCHArc(vertex5, vertex6, 1, null);
            CHTests.CreateCHArc(vertex6, vertex7, 1, null);
            CHTests.CreateCHArc(vertex7, vertex8, 1, null);
            CHTests.CreateCHArc(vertex8, vertex9, 1, null);
            CHTests.CreateCHArc(vertex8, vertex10, 1, null);
            CHTests.CreateCHArc(vertex8, vertex16, 1, null);
            CHTests.CreateCHArc(vertex9, vertex10, 1, null);
            CHTests.CreateCHArc(vertex10, vertex11, 1, null);
            CHTests.CreateCHArc(vertex11, vertex12, 1, null);
            CHTests.CreateCHArc(vertex12, vertex13, 1, null);
            CHTests.CreateCHArc(vertex12, vertex14, 1, null);
            CHTests.CreateCHArc(vertex13, vertex14, 1, null);
            CHTests.CreateCHArc(vertex14, vertex15, 1, null);
            CHTests.CreateCHArc(vertex15, vertex16, 1, null);

            data.PersistCHVertex(vertex1);
            data.PersistCHVertex(vertex2);
            data.PersistCHVertex(vertex3);
            data.PersistCHVertex(vertex4);
            data.PersistCHVertex(vertex5);
            data.PersistCHVertex(vertex6);
            data.PersistCHVertex(vertex7);
            data.PersistCHVertex(vertex8);
            data.PersistCHVertex(vertex9);
            data.PersistCHVertex(vertex10);
            data.PersistCHVertex(vertex11);
            data.PersistCHVertex(vertex12);
            data.PersistCHVertex(vertex13);
            data.PersistCHVertex(vertex14);
            data.PersistCHVertex(vertex15);
            data.PersistCHVertex(vertex16);

            CHRouter router = new CHRouter(data);
            CHPathSegment route_uncontracted_1_9 = router.Calculate(1, 9);
            CHPathSegment route_uncontracted_7_13 = router.Calculate(7, 13);

            List<long> nodes = new List<long>();
            nodes.Add(1);
            nodes.Add(2);
            nodes.Add(3);
            nodes.Add(4);
            nodes.Add(5);
            nodes.Add(6);
            nodes.Add(7);
            nodes.Add(8);
            nodes.Add(9);
            nodes.Add(10);
            nodes.Add(11);
            nodes.Add(12);
            nodes.Add(13);
            nodes.Add(14);
            nodes.Add(15);
            nodes.Add(16);

            CHPreProcessor preprocessor = new CHPreProcessor(data,
                weight_calculator, witness_calculator);
            preprocessor.Start(nodes.GetEnumerator());

            // test 1->9
            CHPathSegment route_contracted_1_9 = router.Calculate(1, 9);

            CHPathSegment route1 = route_uncontracted_1_9;
            CHPathSegment route2 = route_contracted_1_9;
            while (route1.From != null)
            {
                Assert.AreEqual(route1.VertexId, route2.VertexId);
                Assert.AreEqual(route1.Weight, route2.Weight);

                route2 = route2.From;
                route1 = route1.From;
            }
            Assert.IsNull(route2.From);

            // test 7->13
            CHPathSegment route_contracted_7_13 = router.Calculate(7, 13);

            route1 = route_uncontracted_7_13;
            route2 = route_contracted_7_13;
            while (route1.From != null)
            {
                Assert.AreEqual(route1.VertexId, route2.VertexId);
                Assert.AreEqual(route1.Weight, route2.Weight);

                route2 = route2.From;
                route1 = route1.From;
            }
            Assert.IsNull(route2.From);

        }

        #endregion

        #region Helper Functions

        internal static void TestWeightCalculation(INodeWeightCalculator wc, ICHData data, long vertex_id,
            double expected, int level)
        {
            CHVertex vertex = data.GetCHVertex(vertex_id);
            double weight = wc.Calculate(level, vertex);
            Assert.AreEqual(expected, weight);
        }


        internal static List<long> BuildSimpleDenseNetwork(ICHData data)
        {
            CHVertex vertex1 = CHTests.CreateCHVertex(1);
            CHVertex vertex2 = CHTests.CreateCHVertex(2);
            CHVertex vertex3 = CHTests.CreateCHVertex(3);
            CHVertex vertex4 = CHTests.CreateCHVertex(4);
            CHVertex vertex5 = CHTests.CreateCHVertex(5);
            CHVertex vertex6 = CHTests.CreateCHVertex(6);
            CHVertex vertex7 = CHTests.CreateCHVertex(7);
            CHVertex vertex8 = CHTests.CreateCHVertex(8);
            CHVertex vertex9 = CHTests.CreateCHVertex(9);

            CHTests.CreateCHArc(vertex1, vertex2, 1);
            CHTests.CreateCHArc(vertex2, vertex1, 1);

            CHTests.CreateCHArc(vertex4, vertex2, 1);
            CHTests.CreateCHArc(vertex2, vertex4, 1);

            CHTests.CreateCHArc(vertex2, vertex5, 1);
            CHTests.CreateCHArc(vertex5, vertex2, 1);

            CHTests.CreateCHArc(vertex3, vertex5, 1);
            CHTests.CreateCHArc(vertex5, vertex3, 1);

            CHTests.CreateCHArc(vertex5, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex5, 1);

            CHTests.CreateCHArc(vertex6, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex6, 1);

            CHTests.CreateCHArc(vertex8, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex8, 1);

            CHTests.CreateCHArc(vertex9, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex9, 1);

            CHTests.CreateCHArc(vertex5, vertex6, 1);
            CHTests.CreateCHArc(vertex6, vertex5, 1);

            CHTests.CreateCHArc(vertex8, vertex6, 1);
            CHTests.CreateCHArc(vertex6, vertex8, 1);

            CHTests.CreateCHArc(vertex8, vertex9, 1);
            CHTests.CreateCHArc(vertex9, vertex8, 1);

            CHTests.CreateCHArc(vertex3, vertex9, 1);
            CHTests.CreateCHArc(vertex9, vertex3, 1);

            CHTests.CreateCHArc(vertex1, vertex4, 1);
            CHTests.CreateCHArc(vertex4, vertex1, 1);

            data.PersistCHVertex(vertex1);
            data.PersistCHVertex(vertex2);
            data.PersistCHVertex(vertex3);
            data.PersistCHVertex(vertex4);
            data.PersistCHVertex(vertex5);
            data.PersistCHVertex(vertex6);
            data.PersistCHVertex(vertex7);
            data.PersistCHVertex(vertex8);
            data.PersistCHVertex(vertex9);


            List<long> nodes = new List<long>();
            nodes.Add(1);
            nodes.Add(2);
            nodes.Add(3);
            nodes.Add(4);
            nodes.Add(5);
            nodes.Add(6);
            nodes.Add(7);
            nodes.Add(8);
            nodes.Add(9);
            return nodes;
        }

        internal static List<long> BuildSimpleSparseNetwork(ICHData data)
        {
            CHVertex vertex1 = CHTests.CreateCHVertex(1);
            CHVertex vertex2 = CHTests.CreateCHVertex(2);
            CHVertex vertex3 = CHTests.CreateCHVertex(3);
            CHVertex vertex4 = CHTests.CreateCHVertex(4);
            CHVertex vertex5 = CHTests.CreateCHVertex(5);
            CHVertex vertex6 = CHTests.CreateCHVertex(6);
            CHVertex vertex7 = CHTests.CreateCHVertex(7);
            CHVertex vertex8 = CHTests.CreateCHVertex(8);
            CHVertex vertex9 = CHTests.CreateCHVertex(9);

            CHTests.CreateCHArc(vertex1, vertex2, 1);
            CHTests.CreateCHArc(vertex2, vertex1, 1);

            CHTests.CreateCHArc(vertex4, vertex2, 1);
            CHTests.CreateCHArc(vertex2, vertex4, 1);

            CHTests.CreateCHArc(vertex2, vertex5, 1);
            CHTests.CreateCHArc(vertex5, vertex2, 1);

            CHTests.CreateCHArc(vertex3, vertex5, 1);
            CHTests.CreateCHArc(vertex5, vertex3, 1);

            CHTests.CreateCHArc(vertex5, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex5, 1);

            CHTests.CreateCHArc(vertex6, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex6, 1);

            CHTests.CreateCHArc(vertex8, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex8, 1);

            CHTests.CreateCHArc(vertex9, vertex7, 1);
            CHTests.CreateCHArc(vertex7, vertex9, 1);            

            data.PersistCHVertex(vertex1);
            data.PersistCHVertex(vertex2);
            data.PersistCHVertex(vertex3);
            data.PersistCHVertex(vertex4);
            data.PersistCHVertex(vertex5);
            data.PersistCHVertex(vertex6);
            data.PersistCHVertex(vertex7);
            data.PersistCHVertex(vertex8);
            data.PersistCHVertex(vertex9);

            List<long> nodes = new List<long>();
            nodes.Add(1);
            nodes.Add(2);
            nodes.Add(3);
            nodes.Add(4);
            nodes.Add(5);
            nodes.Add(6);
            nodes.Add(7);
            nodes.Add(8);
            nodes.Add(9);
            return nodes;
        }

        private static void CreateCHArc(CHVertex from, CHVertex to, float weight)
        {
            CHTests.CreateCHArc(from, to, weight, true);
        }

        private static void CreateCHArc(CHVertex from, CHVertex to, float weight, bool? directed)
        {
            if (!directed.HasValue ||
                directed == true)
            {
                CHVertexNeighbour arc = new CHVertexNeighbour();
                //arc.VertexFromId = from;
                arc.Id = to.Id;
                arc.Weight = weight;
                arc.ContractedVertexId = -1;
                from.ForwardNeighbours.Add(arc);

                arc = new CHVertexNeighbour();
                //arc.VertexFromId = from;
                arc.Id = from.Id;
                arc.Weight = weight;
                arc.ContractedVertexId = -1;
                to.BackwardNeighbours.Add(arc);
            }

            if (!directed.HasValue ||
                directed == false)
            {
                CHVertexNeighbour arc = new CHVertexNeighbour();
                //arc.VertexFromId = from;
                arc.Id = to.Id;
                arc.Weight = weight;
                arc.ContractedVertexId = -1;
                from.BackwardNeighbours.Add(arc);

                arc = new CHVertexNeighbour();
                //arc.VertexFromId = from;
                arc.Id = from.Id;
                arc.Weight = weight;
                arc.ContractedVertexId = -1;
                to.ForwardNeighbours.Add(arc);
            }
        }

        private static CHVertex CreateCHVertex(long vertex_id)
        {
            CHVertex vertex = new CHVertex();
            vertex.Id = vertex_id;
            //vertex.Latitude = latitude;
            //vertex.Longitude = longitude;
            return vertex;
        }


        #endregion

        #region CHData Factory

        internal interface ICHDataFactory
        {
            ICHData CreateData();
        }

        internal class MemoryCHDataFactory : ICHDataFactory
        {
            public ICHData CreateData()
            {
                return new MemoryCHData();
            }
        }

        internal class OracleCHDataFactory : ICHDataFactory
        {
            public ICHData CreateData()
            {
                return new OracleCHData("Data source=TEST;User Id=OSM;Password=mixbeton;");
            }
        }

        #endregion
    }
}

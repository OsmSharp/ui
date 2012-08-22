//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Core;
//using Tools.Math.Graph.Routing;
//using Osm.Data;
//using Osm.Core.Xml;
//using Tools.Xml.Sources;
//using System.IO;
//using Tools.Math.Geo;
//using System.Data;
//using Osm.Data.Cache;
//using Osm.Routing.Raw;
//using Osm.Routing.Core.Route;
//using Osm.Data.XML.Raw.Processor;
//using Osm.Data.Core.Processor.Progress;
//using Osm.Data.Core.Sparse.Primitives;

//namespace Osm.Routing.Test.Redis
//{
//    class RedisTest
//    {
//        /// <summary>
//        /// Tests the internals of the router class.
//        /// </summary>
//        internal static void TestRedis()
//        {
//            //RedisTest.SimpleGraph("redis");
//        }

//        internal static RedisSparseData PreProcess()
//        {            
//            // create the data source.
//            string source_file = @"C:\OSM\bin\ovl_highway.osm";
//            //string source_file = "demo.osm";
//            XmlDataProcessorSource source = new XmlDataProcessorSource(source_file, false);

//            RedisSparseDataProcessorTarget redis_target = new RedisSparseDataProcessorTarget();
//            //redis_target.RegisterSource(source);
//            //redis_target.Pull();
//            ProgressDataProcessorTarget progress = new ProgressDataProcessorTarget(redis_target);
//            progress.RegisterSource(source);
//            progress.Pull();

//            //// pre-process.
//            RedisSparseData sparse_data = new RedisSparseData();
//            //Osm.Data.Redis.RedisSimpleSource redis_source = new Osm.Data.Redis.RedisSimpleSource();
//            //Osm.Data.Cache.DataSourceCache cache = new DataSourceCache(redis_source, 14);
//            //Osm.Routing.Raw.Graphs.Graph raw_graph = new Osm.Routing.Raw.Graphs.Graph(
//            //    new Osm.Routing.Raw.Graphs.Interpreter.GraphInterpreterTime(cache, Osm.Routing.Core.VehicleEnum.Car), cache);
//            //Osm.Routing.Sparse.PreProcessor.SparsePreProcessor pre_processor = new Osm.Routing.Sparse.PreProcessor.SparsePreProcessor(
//            //    sparse_data, raw_graph);
//            //pre_processor.Process(redis_target.ProcessedNodes);

//            return sparse_data;
//        }

//        internal static void SimpleGraph(string name, RedisSparseData sparse_data)
//        {
//            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

//            // read matrix points.
//            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
//            DataSet data = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
//                new System.IO.FileInfo(info.FullName + string.Format("\\Redis\\{0}.csv", name)), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
//            foreach (DataRow row in data.Tables[0].Rows)
//            {
//                // be carefull with the parsing and the number formatting for different cultures.
//                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
//                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

//                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
//                coordinates.Add(point);
//            }

//            //DataSourceCache data_source = new DataSourceCache(new Osm.Data.Redis.RedisSimpleSource(), 10);
//            //Osm.Data.Redis.RedisSimpleSource data_source = new Data.Redis.RedisSimpleSource();

//            //Osm.Routing.Sparse.Redis.RedisSparseData sparse_data = new Osm.Routing.Sparse.Redis.RedisSparseData();
//            long ticks = DateTime.Now.Ticks;
//            int test_count = 1;
//            int current_count = test_count;
//            while (current_count > 0)
//            {
//                // initialize data.
//                //IDataSource data_source = new OsmDataSource(
//                //    new OsmDocument(new XmlFileSource(info.FullName + string.Format("\\Sparse\\{0}.osm", name))));

//                for (int idx = 0; idx < coordinates.Count - 1; idx++)
//                {
//                    long route_ticks = DateTime.Now.Ticks;

//                    GeoCoordinate geo_from = coordinates[idx];
//                    GeoCoordinate geo_to = coordinates[idx + 1];

//                    // create router.
//                    Osm.Routing.Sparse.Routing.Router router = new Osm.Routing.Sparse.Routing.Router(sparse_data);


//                    SparseVertex from = router.Resolve(geo_from);
//                    SparseVertex to = router.Resolve(geo_to);

//                    OsmSharpRoute route = router.Calculate(from, to);
//                    long route_ticks_after = DateTime.Now.Ticks;
//                    Console.WriteLine("Calculate route {0}/{1}:{2}m in {3}s", idx, coordinates.Count - 1,
//                        route.TotalDistance, new TimeSpan(route_ticks_after - route_ticks).TotalSeconds);
//                }

//                current_count--;
//            }
//            long ticks_resolved = DateTime.Now.Ticks;
//            Console.WriteLine("Resolved in {0} seconds!", new TimeSpan(ticks_resolved - ticks).TotalSeconds);
//            Console.ReadLine();
//        }
//    }
//}

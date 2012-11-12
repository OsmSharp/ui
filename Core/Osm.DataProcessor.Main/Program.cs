// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using Osm.Data.Oracle;
using Osm.Data.Core.Processor.Filter;
using Osm.Data.XML.Raw.Processor.Replication;
using Osm.Data.Oracle.Raw;
using Osm.Data.Oracle.Raw.Processor.ChangeSets;
using Osm.Data.XML.Raw.Processor;
using Osm.Data.Core.Processor.Filter.Sort;
using Osm.Data.Oracle.Raw.Processor;
using Osm.Data.Core.Processor.Default;
using Osm.Data.XML.Raw.Processor.ChangeSets;
using Osm.Data.Redis.Sparse.Processor;

namespace Osm.Data.Processor.Main
{
    class Program
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //string source_file = @"C:\temp\RU-MOW.osm";
            string source_file = @"C:\OSM\bin\belgium.osm";
            //Program.TestImportRedisAndSparsePreProcessing(source_file);
            Program.TestImportOracleAndSparsePreProcessing(source_file);
            //var startTime = DateTime.Now;
            //redis_target.Pull();
            //var time = DateTime.Now - startTime;
            //Console.WriteLine(new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds).ToString("g"));
            

            ////OracleSimpleDataProcessorTarget test_oracle_target = new OracleSimpleDataProcessorTarget(connection_string);
            ////test_oracle_target.RegisterSource(source);
            ////test_oracle_target.Pull();

            string connection_string = "Data source=PROD;User Id=OSM;Password=mixbeton;";
            ////////string source_file = @"drongen.osm";
            //string source_file = @"\\dm-wks-200\c$\OSM\bin\belgium.osm";
            //TestImportBelgium(source_file, connection_string);


            double top = 51.6;
            double bottom = 50.5;
            double left = 2.3;
            double right = 6.3;

            DummyListener listenere = new DummyListener();

            int begin_nr = 20282;
            while (true)
            {
                //Replicator replicator = new Replicator(begin_nr, @"http://planet.openstreetmap.org/minute-replicate", 10000, 100);
                Replicator replicator = new Replicator(begin_nr, @"http://planet.openstreetmap.org/hour-replicate", 100, 100);

                DataProcessorChangeSetFilterBoundingBox box_filter = new DataProcessorChangeSetFilterBoundingBox(
                    new OracleSimpleSource(connection_string),
                    new Tools.Math.Geo.GeoCoordinateBox(new Tools.Math.Geo.GeoCoordinate(top, left), new Tools.Math.Geo.GeoCoordinate(bottom, right)),
                    listenere);
                box_filter.RegisterSource(replicator);

                OracleSimpleChangeSetApplyTarget change_target = new OracleSimpleChangeSetApplyTarget(connection_string, true);
                change_target.RegisterSource(box_filter);
                change_target.Pull();
                change_target.Close();

                replicator.Close();

                begin_nr = begin_nr + 99;
            }
        }


        private static void TestImportOracleAndSparsePreProcessing(string source_file)
        {
            //string source_file = @"drongen.osm";
            // import data
            XmlDataProcessorSource source = new XmlDataProcessorSource(source_file, false);

            //OracleSparseDataProcessorTarget redis_target = new OracleSparseDataProcessorTarget(
            //    "Data source=DEV;User Id=OSM_HH;Password=mixbeton;");
            ////redis_target.RegisterSource(source);
            ////redis_target.Pull();
            //ProgressDataProcessorTarget progress = new ProgressDataProcessorTarget(redis_target);
            //progress.RegisterSource(source);
            //progress.Pull();

            //// pre-process.
            //Osm.Routing.Sparse.Memory.MemorySparseData memory_sparse_data = new Routing.Sparse.Memory.MemorySparseData();
            //Osm.Data.Redis.RedisSimpleSource redis_source = new Data.Redis.RedisSimpleSource();
            //Osm.Routing.Raw.Graphs.Graph raw_graph = new Routing.Raw.Graphs.Graph(
            //    new Osm.Routing.Raw.Graphs.Interpreter.GraphInterpreterTime(redis_source, Routing.Core.VehicleEnum.Car), redis_source);
            //Osm.Routing.Sparse.PreProcessor.SparsePreProcessor pre_processor = new Routing.Sparse.PreProcessor.SparsePreProcessor(
            //    memory_sparse_data, raw_graph);
            //pre_processor.Process(redis_target.ProcessedNodes);
        }

        private static void TestImportRedisAndSparsePreProcessing(string source_file)
        {
            //string source_file = @"drongen.osm";
            // import data
            XmlDataProcessorSource source = new XmlDataProcessorSource(source_file, false);

            RedisSparseDataProcessorTarget redis_target = new RedisSparseDataProcessorTarget();
            redis_target.RegisterSource(source);
            redis_target.Pull();
            //ProgressDataProcessorTarget progress = new ProgressDataProcessorTarget(redis_target);
            //progress.RegisterSource(source);
            //progress.Pull();

            //// pre-process.
            //Osm.Routing.Sparse.Memory.MemorySparseData memory_sparse_data = new Routing.Sparse.Memory.MemorySparseData();
            //Osm.Data.Redis.RedisSimpleSource redis_source = new Data.Redis.RedisSimpleSource();
            //Osm.Routing.Raw.Graphs.Graph raw_graph = new Routing.Raw.Graphs.Graph(
            //    new Osm.Routing.Raw.Graphs.Interpreter.GraphInterpreterTime(redis_source, Routing.Core.VehicleEnum.Car), redis_source);
            //Osm.Routing.Sparse.PreProcessor.SparsePreProcessor pre_processor = 
            //    new Routing.Sparse.PreProcessor.SparsePreProcessor(
            //    memory_sparse_data);
            //pre_processor.Process(redis_target.ProcessedNodes);
        }

        private static void TestImportBelgium(string source_file,string connection_string)
        {
            // truncate
            OracleSimpleDataProcessorTruncateTarget truncate_target = new OracleSimpleDataProcessorTruncateTarget(connection_string);
            DataProcessorSourceEmpty empty_source = new DataProcessorSourceEmpty();
            truncate_target.RegisterSource(empty_source);
            truncate_target.Pull();
            Console.WriteLine("Truncated data!");

            // import data
            XmlDataProcessorSource source = new XmlDataProcessorSource(source_file,false);

            //double bottom = 49.35;
            //double top = 51.6;
            //double left = 1.99;
            //double right = 6.71;

            //DataProcessorFilterBoundingBox box_filter = new DataProcessorFilterBoundingBox(new Tools.Math.Geo.GeoCoordinateBox(new Tools.Math.Geo.GeoCoordinate(
            //    top, left), new Tools.Math.Geo.GeoCoordinate(bottom, right)));
            //box_filter.RegisterSource(source);

            //RoutingFilter routing_filter = new RoutingFilter();
            //routing_filter.RegisterSource(source);

            OracleSimpleDataProcessorTarget test_oracle_target = new OracleSimpleDataProcessorTarget(connection_string);
            test_oracle_target.RegisterSource(source);
            test_oracle_target.Pull();
        }

        private static void TestXmlWriter()
        {
            XmlDataProcessorSource source = new XmlDataProcessorSource(@"zand_small.osm");

            XmlDataProcessorTarget target = new XmlDataProcessorTarget(@"zand_small_test.osm");
            target.RegisterSource(source);
            target.Pull();
            //target.Close();
        }

        private static void TestBBFilter(string input_file,string output_file, double top, double bottom, double left, double right)
        {
            XmlDataProcessorSource source = new XmlDataProcessorSource(input_file);

            DataProcessorFilterBoundingBox box_filter = new DataProcessorFilterBoundingBox(new Tools.Math.Geo.GeoCoordinateBox(new Tools.Math.Geo.GeoCoordinate(
                top, left), new Tools.Math.Geo.GeoCoordinate(bottom, right)));
            box_filter.RegisterSource(source);

            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(box_filter);

            XmlDataProcessorTarget target = new XmlDataProcessorTarget(output_file);
            target.RegisterSource(sorter);
            target.Pull();
            //target.Close();
        }

        private static void Test()
        {
            // truncate
            OracleSimpleDataProcessorTruncateTarget truncate_target = new OracleSimpleDataProcessorTruncateTarget("");
            DataProcessorSourceEmpty empty_source = new DataProcessorSourceEmpty();
            truncate_target.RegisterSource(empty_source);
            truncate_target.Pull();
            Console.WriteLine("Truncated data!");

            // import test data
            XmlDataProcessorSource test_xml_source = new XmlDataProcessorSource(@"dohan_before.osm");
            OracleSimpleDataProcessorTarget test_oracle_target = new OracleSimpleDataProcessorTarget("");
            test_oracle_target.RegisterSource(test_xml_source);
            test_oracle_target.Pull();
            Console.WriteLine("Test data imported!");

            // apply changesets
            XmlDataProcessorChangeSetSource change_source = new XmlDataProcessorChangeSetSource("dohan_change1.osc");

            double top = 49.80672;
            double bottom = 49.78664;
            double left = 5.11993;
            double right = 5.15821;

            DataProcessorChangeSetFilterBoundingBox box_filter = new DataProcessorChangeSetFilterBoundingBox(
                new OracleSimpleSource(""),
                new Tools.Math.Geo.GeoCoordinateBox(new Tools.Math.Geo.GeoCoordinate(top, left), new Tools.Math.Geo.GeoCoordinate(bottom, right)));
            box_filter.RegisterSource(change_source);

            OracleSimpleChangeSetApplyTarget change_target = new OracleSimpleChangeSetApplyTarget("Data source=TEST;User Id=OSM;Password=not_the_real_password_haha;", true);
            change_target.RegisterSource(box_filter);
            change_target.Pull();

            // apply changesets again
            change_source = new XmlDataProcessorChangeSetSource("dohan_change2.osc");

            change_target = new OracleSimpleChangeSetApplyTarget("Data source=TEST;User Id=OSM;Password=not_the_real_password_haha;", true);
            change_target.RegisterSource(change_source);
            change_target.Pull();

            OracleSimpleDataProcessorSource source = new OracleSimpleDataProcessorSource("Data source=TEST;User Id=OSM;Password=not_the_real_password_haha;");

            XmlDataProcessorTarget target = new XmlDataProcessorTarget("output.osm");
            target.RegisterSource(source);
            target.Pull();
        }
    }
}

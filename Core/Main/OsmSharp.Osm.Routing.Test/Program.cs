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
using OsmSharp.Osm.Data;
using OsmSharp.Tools.Xml.Sources;
using OsmSharp.Osm.Core.Xml;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Core.Factory;
using System.IO;
using OsmSharp.Osm.Routing.Data.Source;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Osm.Routing.Interpreter;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using System.Reflection;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Osm.Routing.Data;
using OsmSharp.Routing.Core;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Routing.Core.VRP.NoDepot.MaxTime.Genetic;
using OsmSharp.Routing.Core.Route;
using OsmSharp.Osm.Data.Core.Processor.Progress;
using OsmSharp.Osm.Routing.Test.CH;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Data.API;
using OsmSharp.Osm.Core.Simple;

namespace OsmSharp.Osm.Routing.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // register the output stream to the console.
            OsmSharp.Tools.Core.Output.OutputStreamHost.RegisterOutputStream(
                new OsmSharp.Tools.Core.Output.ConsoleOutputStream());

            //PBFDataProcessorSource source = new PBFDataProcessorSource(new FileInfo(@"C:\OSM\bin\rawdata.osm.pbf").OpenRead());
            //OsmDataProcessorNodeTarget target = new OsmDataProcessorNodeTarget();
            //target.RegisterSource(source);
            //target.Pull();

            //Tools.Core.Output.OutputStreamHost.RegisterOutputStream(
            //    new OsmSharp.Tools.Core.Output.FileOutputStream(@"c:\temp\log.txt"));

            //PBF.PBFTest.Execute();
            //ManyToMany.ManyToManyExecution.Execute();
            Point2Point.Point2PointExecution.Execute();
            //CHTest.Execute();
            //Tsp.TspTest.Execute();
            //CHVerifiedContractionBaseTests.Execute();
            //Instructions.InstructionTestExecution.Execute();
            

            //// initialize the interpreters.
            //OsmRoutingInterpreter interpreter = 
            //    new OsmSharp.Osm.Routing.Interpreter.OsmRoutingInterpreter();

            //OsmTagsIndex tags_index = new OsmTagsIndex();

            //// do the data processing.
            //MemoryRouterDataSource<SimpleWeighedEdge> data =
            //    new MemoryRouterDataSource<SimpleWeighedEdge>(tags_index);
            //SimpleWeighedEdgeGraphProcessingTarget target_data = new SimpleWeighedEdgeGraphProcessingTarget(
            //    data, interpreter, data.TagsIndex);
            //PBFDataProcessorSource data_processor_source = new PBFDataProcessorSource((new FileInfo(
            //    @"c:\OSM\bin\belgium.osm.pbf")).OpenRead());
            //ProgressDataProcessorSource progress_source = new ProgressDataProcessorSource(data_processor_source);
            ////ProgressDataProcessorTarget processor_target = new ProgressDataProcessorTarget(
            ////    target_data);
            ////DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            ////sorter.RegisterSource(data_processor_source);
            //target_data.RegisterSource(progress_source);
            //target_data.Pull();

            //// initialize the router.
            //Router<Osm.Routing.Data.SimpleWeighedEdge> router = 
            //    new Router<SimpleWeighedEdge>(
            //        data, interpreter);

            //// 51.263634,4.785819: wechel centrum
            //// 51.267554,4.801354: wechel zand
            //// 51.270508,4.713893: zoersel centrum
            //// 51.240211,4.581928: schilde centrum
            //// 51.22011,4.459734: deurne centrum

            //RouterPoint wechel_centrum = router.Resolve(new GeoCoordinate(51.263634, 4.785819));
            //RouterPoint wechel_zand = router.Resolve(new GeoCoordinate(51.267554, 4.801354));
            //RouterPoint zoersel_centrum = router.Resolve(new GeoCoordinate(51.270508, 4.713893));
            //RouterPoint schilde_centrum = router.Resolve(new GeoCoordinate(51.240211, 4.581928));
            //RouterPoint deurne_centrum = router.Resolve(new GeoCoordinate(51.240211, 4.581928));

            //Tools.Core.Output.OutputStreamHost.WriteLine("Wechel...");
            //OsmSharpRoute route = router.Calculate(wechel_centrum, wechel_zand);
            //route.SaveAsGpx(new FileInfo(@"c:\temp\wechel.gpx"));
            //Tools.Core.Output.OutputStreamHost.WriteLine("Wechel zoersel...");
            //route = router.Calculate(wechel_centrum, zoersel_centrum);
            //route.SaveAsGpx(new FileInfo(@"c:\temp\wechel_zoersel.gpx"));
            //Tools.Core.Output.OutputStreamHost.WriteLine("Wechel schilde...");
            //route = router.Calculate(wechel_centrum, schilde_centrum);
            //route.SaveAsGpx(new FileInfo(@"c:\temp\wechel.gpx"));
            //Tools.Core.Output.OutputStreamHost.WriteLine("Wechel deurne...");
            //route = router.Calculate(wechel_centrum, deurne_centrum);
            //route.SaveAsGpx(new FileInfo(@"c:\temp\wechel_zoersel.gpx"));
            ////route = router.Calculate(wechel_centrum, wechel_zand);

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        public class OsmDataProcessorNodeTarget : DataProcessorTarget
        {
            private Api _api;

            private int _count = 0;

            private long _changeset_id;

            private int _count_total = 0;

            //private bool _go = false;

            private StreamWriter _output_stream;

            public override void Initialize()
            {
                _api = new Osm.Data.API.Api("http://api.openstreetmap.org",
                    "eurosha RCA", "Josephine1");
                _changeset_id = _api.ChangeSetCreate("Rectification of wrong offset (HOT).");

                _output_stream = new StreamWriter(new FileInfo(@"C:\OSM\bin\rawdata.osm.log").OpenWrite());
            }

            public override void ApplyChange(Osm.Core.Simple.SimpleChangeSet change)
            {

            }

            public override void AddNode(Osm.Core.Simple.SimpleNode old_node)
            {
                //if (old_node.Id == 1048982452)
                //{
                //    _go = true;
                //}
                //if (_go)
                //{
                    SimpleNode node = _api.NodeGet(old_node.Id.Value);

                    if (node != null && node.UserName != "Eurosha RCA")
                    {
                        node.Latitude = node.Latitude.Value + 0.00002;
                        node.Longitude = node.Longitude.Value + 0.00014;

                        _count_total++;
                        _count++;

                        try
                        {
                            _api.NodeUpdate(node);

                            _output_stream.WriteLine("{0};{1};{2};", _changeset_id, node.Id.Value, "1");
                            Tools.Core.Output.OutputStreamHost.WriteLine("[{0}]@ changset {1}:{2}/1000: {3};{4};", _count_total,
                                _changeset_id, _count, node.Id.Value, "1");
                        }
                        catch (Exception ex)
                        {
                            _output_stream.WriteLine("{0};{1};{2};", _changeset_id, node.Id.Value, "0");
                            Tools.Core.Output.OutputStreamHost.WriteLine("[{0}]@ changset {1}:{2}/1000: {3};{4};", _count_total,
                                _changeset_id, _count, node.Id.Value, "0");
                        }

                        _output_stream.Flush();
                        //System.Threading.Thread.Sleep(50);

                        if (_count > 1000)
                        {
                            _count = 0;

                            _api.ChangeSetClose();
                            _api = new Osm.Data.API.Api("http://api.openstreetmap.org",
                                "eurosha RCA", "Josephine1");
                            _changeset_id = _api.ChangeSetCreate("Rectification of wrong offset (HOT).");
                        }
                    }
                    else
                    {
                        if (node == null)
                        {
                            Tools.Core.Output.OutputStreamHost.WriteLine("[{0}]@ changset {1}:{2}/1000: {3};{4};", _count_total,
                                _changeset_id, _count, old_node.Id.Value, "2");
                        }
                        else
                        {
                            Tools.Core.Output.OutputStreamHost.WriteLine("[{0}]@ changset {1}:{2}/1000: {3};{4};", _count_total,
                                _changeset_id, _count, old_node.Id.Value, "3");
                        }
                    }


            }

            public override void AddWay(Osm.Core.Simple.SimpleWay way)
            {

            }

            public override void AddRelation(Osm.Core.Simple.SimpleRelation relation)
            {

            }

            public override void Close()
            {
                _api.ChangeSetClose();
            }
        }

        /// <summary>
        /// Reads all test points from the data stream.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IList<GeoCoordinate> ReadPoints(Stream data)
        {
            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            string[][] lines = OsmSharp.Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFileFromStream(
                data, OsmSharp.Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated);
            foreach (string[] row in lines)
            {
                // be carefull with the parsing and the number formatting for different cultures.
                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
                coordinates.Add(point);
            }
            return coordinates;
        }
    }
}

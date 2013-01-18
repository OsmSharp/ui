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
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Routing.Core;
using OsmSharp.Tools.Xml.Sources;
using OsmSharp.Tools.Math.Units.Time;
using OsmSharp.Osm.Core;
using System.IO;
using OsmSharp.Osm.Data.Raw.XML.OsmSource;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Route;
using OsmSharp.Routing.Core.Metrics.Time;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Routing.Core.Graph.DynamicGraph.SimpleWeighed;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using OsmSharp.Routing.Core.Graph.Router.Dykstra;
using OsmSharp.Osm.Routing.Interpreter;
using System.Reflection;
using OsmSharp.Routing.Core.VRP.NoDepot.MaxTime;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.Core.VRP.NoDepot.MaxTime.Genetic;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Core.VRP.NoDepot.MaxTime.CheapestInsertion;
using OsmSharp.Routing.Core.VRP.NoDepot.MaxTime.VNS;

namespace OsmSharp.Osm.Routing.Test.VRP
{
    public static class NoDepotTest
    {
        /// <summary>
        /// Start the MaxTime test VRP.
        /// </summary>
        public static void Execute()
        {
            //NoDepotTest.MaxTest("eeklo_500", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\eeklo\eeklo_500.csv",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\eeklo\eeklo.osm",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\eeklo\", 1500, 10, 3, 4, false);

            //NoDepotTest.MaxTest("DM850C", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM850C.csv", @"C:\OSM\bin\DM850C.osm",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\", 5400, 10, 3, 4, false);
            //NoDepotTest.MaxTest("DM101", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM101.csv", @"C:\OSM\bin\DM101.osm.pbf",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\", 3600, 10, 3, 4, true);

            NoDepotTest.MaxTest("384-54823", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\384-54823.csv",
                @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\384-54823.osm.pbf",
                @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\", 2500, 10, 3, 4, true);
            //NoDepotTest.MaxTest("492-65172", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\492-65172.csv",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\492-65172.osm.pbf",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\", 5400, 10, 3, 4, true);
            //NoDepotTest.MaxTest("596-64892", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\596-64892.csv",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\596-64892.osm.pbf",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\", 5400, 10, 3, 4, true);
            //NoDepotTest.MaxTest("1122-66765", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\1122-66765.csv",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\1122-66765.osm.pbf",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\", 5400, 10, 3, 4, true);
            //NoDepotTest.MaxTest("1258-10560", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\1258-10560.csv",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\1258-10560.osm.pbf",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\", 5400, 10, 3, 4, true);
            //NoDepotTest.MaxTest("2665-72235", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\2665-72235.csv",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\2665-72235.osm.pbf",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\", 5400, 10, 3, 4, true);
            //NoDepotTest.MaxTest("3280-72301", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\3280-72301.csv",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\3280-72301.osm.pbf",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\", 5400, 10, 3, 4, true);
            //NoDepotTest.MaxTest("4477-71181", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\4477-71181.csv",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\4477-71181.osm.pbf",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\", 5400, 10, 3, 4, true);
            //NoDepotTest.MaxTest("11403-70996", @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\11403-70996.csv",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\11403-70996.osm.pbf",
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Contracts\", 5400, 10, 3, 4, true);

            //Console.ReadLine();
        }

        /// <summary>
        /// Do one MaxTime VRP test.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="osm"></param>
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        public static void MaxTest(string name, string file, string osm, string output, Second max, Second delivery_time, int latitude_idx, int longitude_idx, bool pbf)
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();

            // get the source file.
            Stream data_stream = (new FileInfo(osm).OpenRead());
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<CHEdgeData> osm_data =
                new MemoryRouterDataSource<CHEdgeData>(tags_index);
            CHEdgeDataGraphProcessingTarget target_data = new CHEdgeDataGraphProcessingTarget(
                osm_data, interpreter, osm_data.TagsIndex);
            DataProcessorSource data_processor_source;
            if (pbf)
            {
                data_processor_source = new PBFDataProcessorSource(data_stream);
            }
            else
            {
                data_processor_source = new XmlDataProcessorSource(data_stream);
            }

            target_data.RegisterSource(data_processor_source);
            target_data.Pull();

            // do the pre-processing part.
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(osm_data);
            //CHPreProcessor pre_processor = new CHPreProcessor(osm_data,
            //    new SparseOrdering(osm_data), witness_calculator);
            CHPreProcessor pre_processor = new CHPreProcessor(osm_data,
                new EdgeDifferenceContractedSearchSpace(osm_data, witness_calculator), witness_calculator);
            pre_processor.Start();

            IRouter<RouterPoint> router = new Router<CHEdgeData>(osm_data, interpreter, new CHRouter(osm_data));

            // read the source files.
            string points_file = file;
            System.Data.DataSet data = OsmSharp.Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new FileInfo(points_file), OsmSharp.Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
            int cnt = -1;
            int max_count = 100000;
            int between = 1;
            List<RouterPoint> points = new List<RouterPoint>();
            Dictionary<string, List<RouterPoint>> points_per_route =
                new Dictionary<string, List<RouterPoint>>();
            foreach (System.Data.DataRow row in data.Tables[0].Rows)
            {
                cnt++;
                if (cnt < max_count && (cnt % between) == 0)
                {
                    string latitude_string = (string)row[latitude_idx];
                    string longitude_string = (string)row[longitude_idx];

                    string route_ud = (string)row[1];

                    double longitude = 0;
                    double latitude = 0;
                    if (double.TryParse(longitude_string, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out longitude) &&
                       double.TryParse(latitude_string, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out latitude))
                    {
                        GeoCoordinate point = new GeoCoordinate(latitude, longitude);

                        RouterPoint resolved = router.Resolve(VehicleEnum.Car, point);
                        if (resolved != null &&
                            router.CheckConnectivity(VehicleEnum.Car, resolved, 10))
                        {
                            points.Add(resolved);

                            List<RouterPoint> route_points;
                            if (!points_per_route.TryGetValue(route_ud, out route_points))
                            {
                                route_points = new List<RouterPoint>();
                                points_per_route.Add(route_ud, route_points);
                            }
                            route_points.Add(resolved);
                        }
                    }

                    OsmSharp.Tools.Core.Output.OutputStreamHost.ReportProgress(
                        data.Tables[0].Rows.IndexOf(row), data.Tables[0].Rows.Count, "NoDepotTest", "Processing points...");
                }
            }
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine(string.Format("Started {0}:", name));
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine(string.Format("{0} points", points.Count));

            //// calculate the old route distances.
            //TextWriter log_stream = new StreamWriter(new FileInfo(output + string.Format("{0}.before.log", name)).OpenWrite());
            //double total_time = 0;
            //foreach (KeyValuePair<string, List<RouterPoint>> route in points_per_route)
            //{
            //    OsmSharp.Osm.Routing.Core.TSP.Genetic.RouterTSPAEXGenetic<RouterPoint> tsp_route =
            //        new Core.TSP.Genetic.RouterTSPAEXGenetic<RouterPoint>(
            //            router);
            //    OsmSharpRoute old_route = tsp_route.CalculateTSP(VehicleEnum.Car, route.Value.ToArray());
            //    //TimeCalculator time_calculator = new TimeCalculator(interpreter);
            //    //Dictionary<string, double> metrics = time_calculator.Calculate(old_route);
            //    //old_route.SaveAsGpx(new FileInfo(output + string.Format("{0}.{1}.gpx", name, route.Key)));
            //    double time = double.Parse(old_route.Tags[0].Value, System.Globalization.CultureInfo.InvariantCulture) + 
            //        delivery_time.Value * route.Value.Count;
            //    total_time = total_time + time;
            //    OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Orginal Route [{0}]: {1}s with {2} points",
            //        route.Key, time, route.Value.Count);
            //    log_stream.WriteLine("{0};{1};{2}", route.Key, time, route.Value.Count);
            //}
            //OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Orginal Routes Total: {0} s", total_time);
            //log_stream.WriteLine("Total: {0}", total_time);
            //log_stream.Flush();

            // pre-calculate the weights 
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Calculating weights...");
            double[][] weights = router.CalculateManyToManyWeight(VehicleEnum.Car,
                points.ToArray(), points.ToArray());

            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Started testing VRP solvers:");
            long before_vrp_ticks = DateTime.Now.Ticks;

            int count = 10;

            RouterMaxTime<RouterPoint> vrp_router;

            //// create one router.
            //vrp_router = new RouterBestPlacement<RouterPoint>(
            //        router, max.Value, delivery_time.Value);
            //NoDepotTest.MaxTestRouterMaxTime(log, directory, name, interpreter, vrp_router, points.ToArray());

            //// create one router.
            //vrp_router = new CheapestInsertionSolverWithSeeds<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 5);
            //NoDepotTest.MaxTestRouterMaxTime(directory, name, interpreter, vrp_router, points.ToArray(), 2);

            //double elitism_percentage = 1;
            //double cross_percentage = 97;
            //double mutation_percentage = 1;

            //int population = 300;
            //int stagnation = 200;
            //vrp_router = new RouterGeneticSimple<RouterPoint>(
            //         router, max, delivery_time.Value, population, stagnation, elitism_percentage,
            //         cross_percentage, mutation_percentage, null);
            //NoDepotTest.MaxTestRouterMaxTime(output, name, interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new CheapestInsertionSolverWithImprovements<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 5, 0.05f, false, 0, false);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + "_no_seed_cost_zero_threshold", interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new CheapestInsertionSolverWithImprovements<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 5, 0.05f, false, .10f, false, 0);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + "_SLCI_000", interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new CheapestInsertionSolverWithImprovements<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 5, 0.05f, true, .10f, false, 0.25f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + "_SLCI_025", interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new CheapestInsertionSolverWithImprovements<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 5, 0.05f, true, .10f, true, 0.5f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + "_SLCI_050", interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new CheapestInsertionSolverWithImprovements<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 5, 0.05f, true, .10f, true, 0.75f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + "_SLCI_075", interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new CheapestInsertionSolverWithImprovements<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 5, 0.05f, true, .10f, true, 1);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + "_SLCI_100", interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new CheapestInsertionSolverWithImprovements<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 5, 0.05f, true);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + "_seed_cost", interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 0.10f, 0);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_lambda_0", 0),
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 0.10f, 0.1f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_lambda_0.1", 0),
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 0.10f, 0.25f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_lambda_0.25", 0),
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 0.10f, 0.5f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_lambda_0.5", 0),
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 0.10f, 1f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_lambda_1", 0),
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            vrp_router = new GuidedVNS<RouterPoint>(
                    router, max.Value, delivery_time.Value, 0.10f, 2f);
            NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("", 0),
                interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 0.10f, 5f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_lambda_5", 0),
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 0.10f, 10f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_lambda_10", 0),
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 0.10f, 100f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_lambda_100", 0),
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, .10f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_threshold_{0}", .10f), 
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, .25f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_threshold_{0}", .25f), 
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, .10f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_ex_{0}", "inf"),
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new GuidedVNS<RouterPoint>(
            //        router, max.Value, delivery_time.Value, .10f);
            //NoDepotTest.MaxTestRouterMaxTime(output, name + string.Format("_ex_{0}", "inf"),
            //    interpreter, vrp_router, points.ToArray(), weights, delivery_time, 1);

            //vrp_router = new OsmSharp.Routing.Core.VRP.NoDepot.MaxTime.SavingsHeuristics.SavingsHeuristicSolver<RouterPoint>(
            //        router, max.Value, delivery_time.Value);
            //NoDepotTest.MaxTestRouterMaxTime(directory, name, interpreter, vrp_router, points.ToArray(), 2);


            long after_vrp_ticks = DateTime.Now.Ticks;

            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Solved in {0}!", 
                (new TimeSpan(after_vrp_ticks - before_vrp_ticks)).ToString());
        }

        /// <summary>
        /// Test one router with the given points.
        /// </summary>
        /// <param name="vrp_router"></param>
        /// <param name="points"></param>
        private static void MaxTestRouterMaxTime(string directory, string name, IRoutingInterpreter interpreter,
            RouterMaxTime<RouterPoint> vrp_router, RouterPoint[] points, double[][] weights, Second delivery_time, int count)
        {
            TextWriter log_stream = new FileInfo(directory + string.Format("{0}.{1}.log", name,
                            vrp_router.Name)).AppendText();

            double best_time = double.MaxValue;
            int total_count = count;
            while (count > 0)
            {
                OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Started Testing: {0} {1}/{2}", vrp_router.Name, total_count - count + 1, total_count);

                double total_time = 0;
                long ticks_before = DateTime.Now.Ticks;
                OsmSharpRoute[] real_routes = vrp_router.CalculateNoDepot(VehicleEnum.Car, points, weights);


                for (int idx = 0; idx < real_routes.Length; idx++)
                {
                    OsmSharpRoute new_route = real_routes[idx];
                    double time = double.Parse(new_route.Tags[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                    //double time = metrics["Time_in_seconds"] + (int.Parse(new_route.Tags[0].Value) * delivery_time.Value);
                    new_route.TotalTime = time;
                    total_time = total_time + time;
                }
                long ticks_after = DateTime.Now.Ticks;

                log_stream.WriteLine("Total Time: {0}",
                    total_time);
                log_stream.WriteLine("Total Vehicles: {0}",
                    real_routes.Length);
                log_stream.WriteLine("Time: {0}", 
                    new TimeSpan(ticks_after - ticks_before).TotalSeconds);
                log_stream.Flush();
                if (total_time < best_time)
                {
                    best_time = total_time;

                    for (int idx = 0; idx < real_routes.Length; idx++)
                    {
                        OsmSharpRoute new_route = real_routes[idx];
                        new_route.SaveAsGpx(new FileInfo(directory + string.Format("{0}.{1}_{2}.gpx", name,
                            vrp_router.Name, idx)));
                        OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Route: {0}: {1} s", idx, new_route.TotalTime);
                    }
                    OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Routes Total: {0} s", total_time);
                }
                count--;
            }
        }

        //public static void MaxTest(IRouter<ResolvedType> router, string name, string csv, int latitude_idx, int longitude_idx)
        //{
        //    NoDepotTest<ResolvedType>.MaxTest(log, name, router, points_per_route, points.ToArray(), max,delivery_time, population, stagnation,
        //        elitism_percentage, cross_percentage, mutation_percentage);

        //    Console.ReadLine();
        //}

        //public static void MaxTest(StreamWriter log, RouterMaxTime<ResolvedType> vrp_router, string name, IRouter<ResolvedType> router, Dictionary<string, List<ResolvedType>> points_per_route,
        //    ResolvedType[] points, Second max, Second delivery_time, int population,
        //    int stagnation, double elitism_percentage, double cross_percentage, double mutation_percentage)
        //{

        //    //// first calculate the weights in seconds.
        //    //float[][] weights = router.CalculateManyToManyWeight(points, points);

        //    //// convert to ints.
        //    //for (int x = 0; x < weights.Length; x++)
        //    //{
        //    //    float[] weights_x = weights[x];
        //    //    for (int y = 0; y < weights_x.Length; y++)
        //    //    {
        //    //        weights_x[y] = (int)weights_x[y];
        //    //    }
        //    //}


        //    //// create the problem for the genetic algorithm.
        //    //List<int> customers = new List<int>();
        //    //for (int customer = 0; customer < points.Length; customer++)
        //    //{
        //    //    customers.Add(customer);
        //    //}
        //    //MatrixProblem matrix = new MatrixProblem(weights, false);

        //    //TSPLIBProblem tsp = OsmSharp.Tools.TSPLIB.Convertor.ATSP_TSP.ATSP_TSPConvertor.Convert(matrix, name, string.Empty);
        //    //Tools.TSPLIB.Parser.TSPLIBProblemGenerator.Generate(new FileInfo(string.Format("{0}.tsp", name)),
        //    //    tsp);

        //    //Osm.Routing.Core.VRP.NoDepot.MaxTime.MaxTimeProblem problem = new Core.VRP.NoDepot.MaxTime.MaxTimeProblem(
        //    //    max, 20, matrix);

        //    //Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement.RouterBestPlacementWithSeeds<ResolvedType> vrp_router
        //    //    = new OsmSharp.Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement.RouterBestPlacementWithSeeds<ResolvedType>(
        //    //        router, max.Value, delivery_time.Value);
        //    //Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic.RouterGeneticSimple<ResolvedType> vrp_router
        //    //     = new Core.VRP.NoDepot.MaxTime.Genetic.RouterGeneticSimple<ResolvedType>(
        //    //         router, max, 20, population, stagnation, elitism_percentage,
        //    //         cross_percentage, mutation_percentage, null);
        //    //Osm.Routing.Core.VRP.NoDepot.MaxTime.TSPPlacement.TSPPlacementSolver<ResolvedType> vrp_router
        //    //    = new Core.VRP.NoDepot.MaxTime.TSPPlacement.TSPPlacementSolver<ResolvedType>(
        //    //        router, 1500, 20, new EdgeAssemblyCrossOverSolver(population, stagnation,
        //    //         new _3OptGenerationOperation(),
        //    //          new EdgeAssemblyCrossover(30,
        //    //                 EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
        //    //                 true)));
        //    total_time = 0;
        //    OsmSharpRoute[] routes = vrp_router.CalculateNoDepot(points);
        //    for (int idx = 0; idx < routes.Length; idx++)
        //    {
        //        OsmSharpRoute new_route = routes[idx];
        //        new_route.SaveAsGpx(new FileInfo(string.Format("{0}_{1}.gpx",
        //            name, idx)));
        //        TimeCalculator time_calculator = new TimeCalculator();
        //        Dictionary<string, double> metrics = time_calculator.Calculate(new_route);
        //        double time = metrics["Time_in_seconds"];
        //        total_time = total_time + time;
        //        Console.WriteLine("Orginal Route: {0}: {1} s", idx, time);
        //        log.WriteLine("Orginal Route: {0}: {1} s", idx, time);
        //    }
        //    Console.WriteLine("Orginal Routes Total: {0} s", total_time);
        //    log.WriteLine("Orginal Routes Total: {0} s", total_time);
        //    log.Flush();
        //}
    }
}

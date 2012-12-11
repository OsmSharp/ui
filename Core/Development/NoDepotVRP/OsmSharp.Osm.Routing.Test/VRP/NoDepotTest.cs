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
using OsmSharp.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.Core.VRP.NoDepot.MaxTime.Genetic;

namespace OsmSharp.Osm.Routing.Test.VRP
{
    public static class NoDepotTest
    {
        /// <summary>
        /// Start the MaxTime test VRP.
        /// </summary>
        public static void Execute()
        {
            // set the console output stream.
            OsmSharp.Tools.Core.Output.OutputStreamHost.RegisterOutputStream(
                new OsmSharp.Tools.Core.Output.ConsoleOutputStream());

            //Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(xml_embedded)

            NoDepotTest.MaxTest("21313", "DM852", 88000, 20);
            //NoDepotTest.MaxTest("21313", "DM852", 5400, 20);
            //NoDepotTest.MaxTest("21313", "DM852", 5400, 20);
            //NoDepotTest.MaxTest("21313", "DM852", 5400, 20);
            //NoDepotTest.MaxTest("21313", "DM852", 5400, 20);
            //NoDepotTest.MaxTest("21313", "DM852", 5400, 20);
            //NoDepotTest.MaxTest("21313", "DM852", 5400, 20);

            Console.ReadLine();
        }

        /// <summary>
        /// Do one MaxTime VRP test.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="osm"></param>
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        public static void MaxTest(string name, string osm, Second max, Second delivery_time)
        {
            bool pbf = false;

            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();

            // define the directory.
            string directory = @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Subcontractors\";

            // get the source file.
            string source_file = string.Format(@"{0}{1}.osm", directory, osm);
            Stream data_stream = (new FileInfo(source_file).OpenRead());
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            //SimpleWeighedDynamicGraph graph = new SimpleWeighedDynamicGraph();
            MemoryRouterDataSource<SimpleWeighedEdge> osm_data =
                new MemoryRouterDataSource<SimpleWeighedEdge>(tags_index);
            SimpleWeighedDataGraphProcessingTarget target_data = new SimpleWeighedDataGraphProcessingTarget(
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

            // create the router.
            IRouter<RouterPoint> router = 
                new Router<SimpleWeighedEdge>(osm_data, 
                    interpreter, new 
                        DykstraRoutingLive(osm_data.TagsIndex));
        
            // read the source files.
            string points_file = directory + string.Format(@"\{0}.csv", name);
            int latitude_idx = 3;
            int longitude_idx = 4;
            StreamWriter log = new StreamWriter(directory + string.Format(@"\{0}.log", name));
            System.Data.DataSet data = OsmSharp.Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new FileInfo(points_file), OsmSharp.Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
            int cnt = -1;
            int max_count = 10000;
            List<RouterPoint> points = new List<RouterPoint>();
            Dictionary<string, List<RouterPoint>> points_per_route =
                new Dictionary<string, List<RouterPoint>>();
            foreach (System.Data.DataRow row in data.Tables[0].Rows)
            {
                cnt++;
                if (cnt < max_count)
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
                            router.CheckConnectivity(VehicleEnum.Car, resolved, 1000))
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

                    OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Processed {0}/{1}!",
                        data.Tables[0].Rows.IndexOf(row), data.Tables[0].Rows.Count);
                }
            }

            log.WriteLine(string.Format("Started {0}:", name));
            log.WriteLine(string.Format("{0} points", points.Count));

            // calculate the old route distances.
            double total_time = 0;
            foreach (KeyValuePair<string, List<RouterPoint>> route in points_per_route)
            {
                OsmSharp.Osm.Routing.Core.TSP.Genetic.RouterTSPAEXGenetic<RouterPoint> tsp_route =
                    new Core.TSP.Genetic.RouterTSPAEXGenetic<RouterPoint>(
                        router);
                OsmSharpRoute old_route = tsp_route.CalculateTSP(VehicleEnum.Car, route.Value.ToArray());
                TimeCalculator time_calculator = new TimeCalculator(interpreter);
                Dictionary<string, double> metrics = time_calculator.Calculate(old_route);
                old_route.SaveAsGpx(new FileInfo(directory + string.Format("{0}.{1}.gpx", name, route.Key)));
                double time = metrics["Time_in_seconds"];
                total_time = total_time + time;
                Console.WriteLine("Orginal Route: {0}: {1} s", route.Key, time);
                log.WriteLine("Orginal Route: {0}: {1} s", route.Key, time);
            }
            Console.WriteLine("Orginal Routes Total: {0} s", total_time);
            log.WriteLine("Orginal Routes Total: {0} s", total_time);
            log.Flush();

            RouterMaxTime<RouterPoint> vrp_router;

            //// create one router.
            //vrp_router = new RouterBestPlacement<RouterPoint>(
            //        router, max.Value, delivery_time.Value);
            //NoDepotTest.MaxTestRouterMaxTime(log, directory, name, interpreter, vrp_router, points.ToArray());

            //// create one router.
            //vrp_router = new RouterBestPlacementWithSeeds<RouterPoint>(
            //        router, max.Value, delivery_time.Value, 5);
            //NoDepotTest.MaxTestRouterMaxTime(log, directory, name, interpreter, vrp_router, points.ToArray());

            //double elitism_percentage = 1;
            //double cross_percentage = 97;
            //double mutation_percentage = 1;

            //int population = 400;
            //int stagnation = 500;
            //vrp_router = new RouterGeneticSimple<RouterPoint>(
            //         router, max, 20, population, stagnation, elitism_percentage,
            //         cross_percentage, mutation_percentage, null);
            //NoDepotTest.MaxTestRouterMaxTime(log, directory, name, interpreter, vrp_router, points.ToArray());

            vrp_router = new RouterBestPlacementWithImprovements<RouterPoint>(
                    router, max.Value, delivery_time.Value, 10, 0);
            NoDepotTest.MaxTestRouterMaxTime(log, directory, name, interpreter, vrp_router, points.ToArray());
        }

        /// <summary>
        /// Test one router with the given points.
        /// </summary>
        /// <param name="vrp_router"></param>
        /// <param name="points"></param>
        private static void MaxTestRouterMaxTime(StreamWriter log, string directory, string name, IRoutingInterpreter interpreter,
            RouterMaxTime<RouterPoint> vrp_router, RouterPoint[] points)
        {
            Console.WriteLine("Started Testing: {0}", vrp_router.Name);
            log.WriteLine("Started Testing: {0}", vrp_router.Name);

            double total_time = 0;
            OsmSharpRoute[] routes = vrp_router.CalculateNoDepot(VehicleEnum.Car, points);
            for (int idx = 0; idx < routes.Length; idx++)
            {
                OsmSharpRoute new_route = routes[idx];
                new_route.SaveAsGpx(new FileInfo(directory + string.Format("{0}.{1}_{2}.gpx", name, 
                    vrp_router.Name, idx)));
                TimeCalculator time_calculator = new TimeCalculator(interpreter);
                Dictionary<string, double> metrics = time_calculator.Calculate(new_route);
                double time = metrics["Time_in_seconds"];
                total_time = total_time + time;
                Console.WriteLine("Route: {0}: {1} s", idx, time);
                log.WriteLine("Route: {0}: {1} s", idx, time);
            }
            Console.WriteLine("Routes Total: {0} s", total_time);
            log.WriteLine("Routes Total: {0} s", total_time);
            log.Flush();
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

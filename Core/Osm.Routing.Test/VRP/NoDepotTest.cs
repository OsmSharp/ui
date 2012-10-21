// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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
using Tools.Math.Graph.Routing;
using Tools.Math.Geo;
using Osm.Routing.Core;
using Osm.Routing.Core.Route;
using Osm.Routing.Raw;
using Tools.Xml.Sources;
using Tools.Math.Units.Time;
using Osm.Core;
using Osm.Data.Raw.XML.OsmSource;
using Tools.Math.TSP.Problems;
using Tools.Math.TSP.EdgeAssemblyGenetic;
using Tools.Math.TSP.Genetic.Solver.Operations.Generation;
using Tools.Math.TSP.Genetic.Solver.Operations.CrossOver;
using Tools.TSPLIB.Problems;
using System.IO;
using Osm.Routing.Core.Metrics.Time;
using Osm.Routing.Core.VRP.NoDepot.MaxTime;
using Osm.Routing.Core.Interpreter.Default;

namespace Osm.Routing.Test.VRP
{
    public static class NoDepotTest
    {
        /// <summary>
        /// Start the MaxTime test VRP.
        /// </summary>
        public static void TestMaxTimeVRP()
        {
            // set the console output stream.
            Tools.Core.Output.OutputTextStreamHost.RegisterOutputStream(
                new Tools.Core.Output.ConsoleOutputStream());

            NoDepotTest.MaxTest("21313", "DM852", 3600, 20);
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
            // define the directory.
            string directory = @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Subcontractors\";

            // get the source file.
            string source_file = string.Format(@"C:\OSM\bin\{0}.osm", osm);
            OsmDataSource osm_data = new OsmDataSource(
                new Osm.Core.Xml.OsmDocument(new XmlFileSource(source_file)));
            Router router = new Router(osm_data,new DefaultVehicleInterpreter(VehicleEnum.Car));

            // read the source files.
            string points_file = directory + string.Format(@"\{0}.csv", name);
            int latitude_idx = 3;
            int longitude_idx = 4;
            StreamWriter log = new StreamWriter(directory + string.Format(@"\{0}.log", name));
            System.Data.DataSet data = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new FileInfo(points_file), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
            int cnt = -1;
            int max_count = 100000;
            List<ResolvedPoint> points = new List<ResolvedPoint>();
            Dictionary<string, List<ResolvedPoint>> points_per_route = new Dictionary<string, List<ResolvedPoint>>();
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

                        ResolvedPoint resolved = router.Resolve(point);
                        if (router.CheckConnectivity(resolved, 100))
                        {
                            points.Add(resolved);

                            List<ResolvedPoint> route_points;
                            if (!points_per_route.TryGetValue(route_ud, out route_points))
                            {
                                route_points = new List<ResolvedPoint>();
                                points_per_route.Add(route_ud, route_points);
                            }
                            route_points.Add(resolved);
                        }
                    }

                    Tools.Core.Output.OutputTextStreamHost.WriteLine("Processed {0}/{1}!",
                        data.Tables[0].Rows.IndexOf(row), data.Tables[0].Rows.Count);
                }
            }

            log.WriteLine(string.Format("Started {0}:", name));
            log.WriteLine(string.Format("{0} points", points.Count));

            // calculate the old route distances.
            double total_time = 0;
            foreach (KeyValuePair<string, List<ResolvedPoint>> route in points_per_route)
            {
                Osm.Routing.Core.TSP.Genetic.RouterTSPAEXGenetic<ResolvedPoint> tsp_route =
                    new Core.TSP.Genetic.RouterTSPAEXGenetic<ResolvedPoint>(
                        router);
                OsmSharpRoute old_route = tsp_route.CalculateTSP(route.Value.ToArray());
                TimeCalculator time_calculator = new TimeCalculator();
                Dictionary<string, double> metrics = time_calculator.Calculate(old_route);
                old_route.SaveAsGpx(new FileInfo(string.Format("{0}_{1}.gpx", name, route.Key)));
                double time = metrics["Time_in_seconds"];
                total_time = total_time + time;
                Console.WriteLine("Orginal Route: {0}: {1} s", route.Key, time);
                log.WriteLine("Orginal Route: {0}: {1} s", route.Key, time);
            }
            Console.WriteLine("Orginal Routes Total: {0} s", total_time);
            log.WriteLine("Orginal Routes Total: {0} s", total_time);
            log.Flush();


            RouterMaxTime<ResolvedPoint> vrp_router;

            // create one router.
            vrp_router = new Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement.RouterBestPlacement<ResolvedPoint>(
                    router, max.Value, delivery_time.Value);
            NoDepotTest.MaxTestRouterMaxTime(log, name, vrp_router, points.ToArray());

            // create one router.
            vrp_router = new Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement.RouterBestPlacementWithSeeds<ResolvedPoint>(
                    router, max.Value, delivery_time.Value, 5);
            NoDepotTest.MaxTestRouterMaxTime(log, name, vrp_router, points.ToArray());

            //double elitism_percentage = 1;
            //double cross_percentage = 97;
            //double mutation_percentage = 1;

            //int population = 400;
            //int stagnation = 500;
            //vrp_router = new Core.VRP.NoDepot.MaxTime.Genetic.RouterGeneticSimple<ResolvedPoint>(
            //         router, max, 20, population, stagnation, elitism_percentage,
            //         cross_percentage, mutation_percentage, null);
            //NoDepotTest.MaxTestRouterMaxTime(log, name, vrp_router, points.ToArray());

            vrp_router = new Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement.RouterBestPlacementWithImprovements<ResolvedPoint>(
                    router, max.Value, delivery_time.Value, 10, 0);
            NoDepotTest.MaxTestRouterMaxTime(log, name, vrp_router, points.ToArray());
        }

        /// <summary>
        /// Test one router with the given points.
        /// </summary>
        /// <param name="vrp_router"></param>
        /// <param name="points"></param>
        private static void MaxTestRouterMaxTime(StreamWriter log, string name, RouterMaxTime<ResolvedPoint> vrp_router, ResolvedPoint[] points)
        {
            Console.WriteLine("Started Testing: {0}", vrp_router.Name);
            log.WriteLine("Started Testing: {0}", vrp_router.Name);

            double total_time = 0;
            OsmSharpRoute[] routes = vrp_router.CalculateNoDepot(points);
            for (int idx = 0; idx < routes.Length; idx++)
            {
                OsmSharpRoute new_route = routes[idx];
                new_route.SaveAsGpx(new FileInfo(string.Format("{0}_{1}.gpx",
                    name, idx)));
                TimeCalculator time_calculator = new TimeCalculator();
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

        //    //TSPLIBProblem tsp = Tools.TSPLIB.Convertor.ATSP_TSP.ATSP_TSPConvertor.Convert(matrix, name, string.Empty);
        //    //Tools.TSPLIB.Parser.TSPLIBProblemGenerator.Generate(new FileInfo(string.Format("{0}.tsp", name)),
        //    //    tsp);

        //    //Osm.Routing.Core.VRP.NoDepot.MaxTime.MaxTimeProblem problem = new Core.VRP.NoDepot.MaxTime.MaxTimeProblem(
        //    //    max, 20, matrix);

        //    //Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement.RouterBestPlacementWithSeeds<ResolvedType> vrp_router
        //    //    = new Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement.RouterBestPlacementWithSeeds<ResolvedType>(
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

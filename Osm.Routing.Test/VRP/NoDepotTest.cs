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

namespace Osm.Routing.Test.VRP
{
    public static class NoDepotTest<ResolvedType>
        where ResolvedType : ILocationObject
    {
        public static void TestEeklo()
        {
            Tools.Core.Output.OutputTextStreamHost.RegisterOutputStream(
                new Tools.Core.Output.ConsoleOutputStream());

            string name = "21313";

            //string source_file = @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Post\Eeklo Osm\post.osm";
            //string source_file = @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Subcontractors\21313.osm";  
            string source_file = @"C:\OSM\bin\DM852.osm";             
            OsmDataSource osm_data = new OsmDataSource(
                new Osm.Core.Xml.OsmDocument(new XmlFileSource(source_file)));
            Router raw_router = new Router(osm_data, new Osm.Routing.Raw.Graphs.Interpreter.GraphInterpreterTime(osm_data, Osm.Routing.Core.VehicleEnum.Car));

            //string points_file = @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Post\Eeklo Osm\post.csv";
            string points_file = string.Format(@"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\Subcontractors\{0}.csv", name);   

            //int latitude_idx = 23;
            //int longitude_idx = 24;

            int latitude_idx = 3;
            int longitude_idx = 4;

            NoDepotTest<ResolvedPoint>.MinMaxTest(raw_router, name, points_file, latitude_idx, longitude_idx);
        }

        public static void MinMaxTest(IRouter<ResolvedType> router, string name, string csv, int latitude_idx, int longitude_idx)
        {
            StreamWriter log = new StreamWriter(string.Format("{0}.log", name));

            System.Data.DataSet data = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new System.IO.FileInfo(csv), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated,
                true, true);
            int cnt = -1;
            //int max_count = 250;
            int max_count = 100000;
            List<ResolvedType> points = new List<ResolvedType>();
            Dictionary<string, List<ResolvedType>> points_per_route = new Dictionary<string, List<ResolvedType>>();

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

                        ResolvedType resolved = router.Resolve(point);
                        if (router.CheckConnectivity(resolved, 100))
                        {
                            points.Add(resolved);

                            List<ResolvedType> route_points;
                            if (!points_per_route.TryGetValue(route_ud, out route_points))
                            {
                                route_points = new List<ResolvedType>();
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

            double elitism_percentage = 1;
            double cross_percentage = 97;
            double mutation_percentage = 1;

            //double min = 1000;
            double max = 3400;
            double delivery_time = 20;

            int population = 400;
            int stagnation = 500;

            NoDepotTest<ResolvedType>.MaxTest(log, name, router, points_per_route, points.ToArray(), max,delivery_time, population, stagnation,
                elitism_percentage, cross_percentage, mutation_percentage);

            Console.ReadLine();
        }

        public static void MaxTest(StreamWriter log, string name, IRouter<ResolvedType> router, Dictionary<string, List<ResolvedType>> points_per_route,
            ResolvedType[] points, Second max, Second delivery_time, int population,
            int stagnation, double elitism_percentage, double cross_percentage, double mutation_percentage)
        {
            // calculate the old route distances.
            double total_time = 0;
            foreach (KeyValuePair<string, List<ResolvedType>> route in points_per_route)
            {
                Osm.Routing.Core.TSP.Genetic.RouterTSPAEXGenetic<ResolvedType> tsp_route = new Core.TSP.Genetic.RouterTSPAEXGenetic<ResolvedType>(
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

            //// first calculate the weights in seconds.
            //float[][] weights = router.CalculateManyToManyWeight(points, points);

            //// convert to ints.
            //for (int x = 0; x < weights.Length; x++)
            //{
            //    float[] weights_x = weights[x];
            //    for (int y = 0; y < weights_x.Length; y++)
            //    {
            //        weights_x[y] = (int)weights_x[y];
            //    }
            //}


            //// create the problem for the genetic algorithm.
            //List<int> customers = new List<int>();
            //for (int customer = 0; customer < points.Length; customer++)
            //{
            //    customers.Add(customer);
            //}
            //MatrixProblem matrix = new MatrixProblem(weights, false);

            //TSPLIBProblem tsp = Tools.TSPLIB.Convertor.ATSP_TSP.ATSP_TSPConvertor.Convert(matrix, name, string.Empty);
            //Tools.TSPLIB.Parser.TSPLIBProblemGenerator.Generate(new FileInfo(string.Format("{0}.tsp", name)),
            //    tsp);

            //Osm.Routing.Core.VRP.NoDepot.MaxTime.MaxTimeProblem problem = new Core.VRP.NoDepot.MaxTime.MaxTimeProblem(
            //    max, 20, matrix);

            //Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement.RouterBestPlacementWithSeeds<ResolvedType> vrp_router
            //    = new Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement.RouterBestPlacementWithSeeds<ResolvedType>(
            //        router, max.Value, delivery_time.Value);
            Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement.RouterBestPlacementWithImprovements<ResolvedType> vrp_router
                = new Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement.RouterBestPlacementWithImprovements<ResolvedType>(
                    router, max.Value, delivery_time.Value, 10, 0.05f);
            //Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic.RouterGeneticSimple<ResolvedType> vrp_router
            //     = new Core.VRP.NoDepot.MaxTime.Genetic.RouterGeneticSimple<ResolvedType>(
            //         router, max, 20, population, stagnation, elitism_percentage,
            //         cross_percentage, mutation_percentage, null);
            //Osm.Routing.Core.VRP.NoDepot.MaxTime.TSPPlacement.TSPPlacementSolver<ResolvedType> vrp_router
            //    = new Core.VRP.NoDepot.MaxTime.TSPPlacement.TSPPlacementSolver<ResolvedType>(
            //        router, 1500, 20, new EdgeAssemblyCrossOverSolver(population, stagnation,
            //         new _3OptGenerationOperation(),
            //          new EdgeAssemblyCrossover(30,
            //                 EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
            //                 true)));
            //vrp_router.IntermidiateResult += new Core.VRP.RouterVRP<ResolvedType>.OsmSharpRoutesDelegate(vrp_router_IntermidiateResult);
            //for (int idx1 = 0; idx1 < tests; idx1++)
            //{
            total_time = 0;
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
                Console.WriteLine("Orginal Route: {0}: {1} s", idx, time);
                log.WriteLine("Orginal Route: {0}: {1} s", idx, time);
            }
            Console.WriteLine("Orginal Routes Total: {0} s", total_time);
            log.WriteLine("Orginal Routes Total: {0} s", total_time);
            log.Flush();
            //}
        }

        static void vrp_router_IntermidiateResult(OsmSharpRoute[] result, Dictionary<int, List<int>> solution)
        {

        }
    }
}

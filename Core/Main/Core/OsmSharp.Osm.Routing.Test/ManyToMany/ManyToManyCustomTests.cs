using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Core;
using System.IO;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Routing.Interpreter;

namespace OsmSharp.Osm.Routing.Test.ManyToMany
{
    /// <summary>
    /// Does some custom many-to-many tests.
    /// </summary>
    public abstract class ManyToManyCustomTests
    {
        /// <summary>
        /// Executes the tests.
        /// </summary>
        /// <param name="csv_export"></param>
        public void Execute(Stream data, string csv_export, bool pbf)
        {
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Started: {0}",
                csv_export);

            // read files.
            Dictionary<int, HashSet<GeoCoordinate>> test_data = this.ReadFile(csv_export);

            // create router.
            IRouter<RouterPoint> router = this.CreateRouter(data, new OsmRoutingInterpreter(), pbf);

            // do the single rounds tests.
            this.TestSingleRounds(router, test_data);
        }

        /// <summary>
        /// Creates a router.
        /// </summary>
        /// <param name="data">The data to use for OsmSharp.Routing.</param>
        /// <param name="interpreter">The interpreter.</param>
        /// <param name="constraints">The routing constraints.</param>
        /// <returns></returns>
        protected abstract IRouter<RouterPoint> CreateRouter(Stream data, IRoutingInterpreter interpreter, bool pbf);

        /// <summary>
        /// Tests all single rounds.
        /// </summary>
        public void TestSingleRounds(IRouter<RouterPoint> router, Dictionary<int, HashSet<GeoCoordinate>> results)
        {
            // resolve all the points.
            Dictionary<int, RouterPoint[]> resolved_points = new Dictionary<int, RouterPoint[]>();
            Dictionary<int, GeoCoordinateBox> boxes_per_round = new Dictionary<int, GeoCoordinateBox>();
            int total_boxes = 0;
            foreach (KeyValuePair<int, HashSet<GeoCoordinate>> results_pair in results)
            {
                HashSet<RouterPoint> points = new HashSet<RouterPoint>();
                HashSet<GeoCoordinate>  coordinates = new HashSet<GeoCoordinate>();
                foreach (GeoCoordinate coordinate in results_pair.Value)
                {
                    RouterPoint point = router.Resolve(VehicleEnum.Car, coordinate);
                    if(point != null)
                    {
                        // check connectivity.
                        if (router.CheckConnectivity(VehicleEnum.Car, point, 1000))
                        {
                            points.Add(point);
                            coordinates.Add(coordinate);
                        }
                    }
                }
                total_boxes = total_boxes + points.Count;
                resolved_points.Add(results_pair.Key, points.ToArray());
                boxes_per_round.Add(results_pair.Key, new GeoCoordinateBox(coordinates.ToArray()));
            }

            long before = DateTime.Now.Ticks;
            Dictionary<int, long> ticks_per_round = new Dictionary<int, long>();
            foreach (KeyValuePair<int, RouterPoint[]> resolved_points_pair in resolved_points)
            {
                long before_round = DateTime.Now.Ticks;
                double[][] weights = router.CalculateManyToManyWeight(VehicleEnum.Car,
                    resolved_points_pair.Value, resolved_points_pair.Value);
                long after_round = DateTime.Now.Ticks;
                ticks_per_round.Add(resolved_points_pair.Key, after_round - before_round);

                OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Round {0}: {1}s",
                    resolved_points_pair.Key, new TimeSpan(after_round - before_round).TotalSeconds);
            }
            long after = DateTime.Now.Ticks;

            // report.
            foreach (int round_id in resolved_points.Keys)
            {
                long ticks = ticks_per_round[round_id];
                double seconds = new TimeSpan(ticks).TotalSeconds;
                long customers = resolved_points[round_id].Length;
                
                // calculate surface.
                GeoCoordinateBox box = boxes_per_round[round_id];
                double x_meter = new GeoCoordinate(box.MaxLat, box.MaxLon).DistanceReal(
                    new GeoCoordinate(box.MinLat, box.MaxLon)).Value;
                double y_meter = new GeoCoordinate(box.MaxLat, box.MaxLon).DistanceReal(
                    new GeoCoordinate(box.MaxLat, box.MinLon)).Value;
                double max_meter = System.Math.Max(x_meter, y_meter);
                double surface = max_meter * max_meter;

                OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("{0};{1};{2};{3}",
                    round_id, ticks, customers, surface);
            }
            double total_seconds = new TimeSpan(after - before).TotalSeconds;
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("TOTAL: {0} rounds with {1} customers in {2}s",
                resolved_points.Keys.Count, total_boxes, total_seconds);
        }

        /// <summary>
        /// Tests calculations on single routes.
        /// </summary>
        /// <param name="csv_export"></param>
        public Dictionary<int, HashSet<GeoCoordinate>> ReadFile(string csv_export)
        {
            Dictionary<int, HashSet<GeoCoordinate>> results = new Dictionary<int, HashSet<GeoCoordinate>>();

            // open the file stream.
            Stream csv_export_data = (new FileInfo(csv_export)).OpenRead();

            // read matrix points.
            string[][] lines = OsmSharp.Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFileFromStream(
                csv_export_data, OsmSharp.Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated);
            foreach (string[] row in lines)
            {
                // get the round_id.
                int round_id = int.Parse(row[1].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                // check the dictionary.
                HashSet<GeoCoordinate> coordinates;
                if (!results.TryGetValue(round_id, out coordinates))
                { // 
                    coordinates = new HashSet<GeoCoordinate>();
                    results.Add(round_id, coordinates);
                }

                // be carefull with the parsing and the number formatting for different cultures.
                double latitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double longitude = double.Parse(row[4].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                coordinates.Add(new GeoCoordinate(latitude, longitude));
            }
            return results;
        }
    }
}

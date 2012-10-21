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
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tools.Math.Geo;
using System.Data;
using Osm.Data;
using Tools.Xml.Sources;
using Osm.Core.Xml;
using Osm.Routing.Raw;
using Osm.Data.Raw.XML.OsmSource;

namespace Osm.Routing.Test.KDTree
{
    class KDTreeTest
    {
        public static void TestMatrix()
        {
            KDTreeTest.Test("matrix", 1);
        }

        //public static void TestLebbeke()
        //{
        //    KDTreeTest.Test("lebbeke", 1);
        //}

        public static void Test(string name, int test_count)
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            DataSet data = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new System.IO.FileInfo(info.FullName + string.Format("\\Matrix\\{0}.csv", name)), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
            foreach (DataRow row in data.Tables[0].Rows)
            {
                // be carefull with the parsing and the number formatting for different cultures.
                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
                coordinates.Add(point);
            }

            long ticks_total_total = 0;
            long ticks_total_resolved = 0;
            long ticks_total_calculated = 0;
            long ticks_total_calculated2 = 0;

            int current_count = test_count;
            while (current_count > 0)
            {
                // initialize data.
                IDataSourceReadOnly data_source = new OsmDataSource(
                    new OsmDocument(new XmlFileSource(info.FullName + string.Format("\\Matrix\\{0}.osm", name))));
                data_source = new Osm.Data.Cache.DataSourceCache(data_source, 14);

                // create router.
                Router router = new Router(data_source);
                //router.RegisterProgressReporter(new ConsoleProgressReporter());

                // calculate matrix.
                long ticks = DateTime.Now.Ticks;
                List<ResolvedPoint> points = new List<ResolvedPoint>(router.Resolve(coordinates.ToArray()));
                long ticks_resolved = DateTime.Now.Ticks;
                Console.WriteLine("Resolved in {0} seconds!", new TimeSpan(ticks_resolved - ticks).TotalSeconds);
                ticks_total_resolved = ticks_total_resolved + (ticks_resolved - ticks);

                // index into the kd tree.
                Tools.Math.Structures.KDTree.Tree2D<ResolvedPoint> tree =
                    new Tools.Math.Structures.KDTree.Tree2D<ResolvedPoint>(points,
                        new Tools.Math.Structures.KDTree.Tree2D<ResolvedPoint>.Distance<ResolvedPoint>(Distance));

                // do some random nearest neighbour stuff.
                int iterations = 10000;
                for (int i = 0; i < iterations; i++)
                {
                    ResolvedPoint point =
                        points[Tools.Math.Random.StaticRandomGenerator.Get().Generate(points.Count)];

                    ResolvedPoint closest = tree.SearchNearestNeighbour(point);
                }
                long ticks_calculated = DateTime.Now.Ticks;
                ticks_total_calculated = ticks_total_calculated + (ticks_calculated - ticks_resolved);


                // do some random nearest neighbour stuff in a lineair way.
                for (int i = 0; i < iterations; i++)
                {
                    ResolvedPoint point =
                        points[Tools.Math.Random.StaticRandomGenerator.Get().Generate(points.Count)];

                    ResolvedPoint closest = points[0];
                    double distance = closest.Location.DistanceEstimate(point.Location).Value;
                    for (int idx = 1; idx < points.Count; idx++)
                    {
                        ResolvedPoint new_closest = points[idx];
                        double new_distance = closest.Location.DistanceEstimate(point.Location).Value;
                        if (new_distance < distance)
                        {
                            closest = new_closest;
                            distance = new_distance;
                        }
                    }
                }
                long ticks_calculated2 = DateTime.Now.Ticks;
                ticks_total_calculated2 = ticks_total_calculated2 + (ticks_calculated2 - ticks_calculated);
                
                ticks_total_total = ticks_total_calculated + ticks_total_resolved;

                current_count--;

                int k = 10;
                double max_time = 3600;
                List<List<ResolvedPoint>> rounds = new List<List<ResolvedPoint>>();
                while (points.Count > 0)
                { // loop until there are no more points to be placed.
                    // 1: start a new round.
                    List<ResolvedPoint> round = new List<ResolvedPoint>();

                    // 2: select a random customer.
                    ResolvedPoint seed =
                        points[
                            Tools.Math.Random.StaticRandomGenerator.Get().Generate(points.Count)];
                    points.Remove(seed);

                    // 3: insert it in the round.
                    round.Add(seed);

                    // 4: select some second point via tournament selection.
                    // select some other point in some manner that might be best.
                    ResolvedPoint second_seed =
                        points[Tools.Math.Random.StaticRandomGenerator.Get().Generate(points.Count)];
                    points.Remove(second_seed);

                    // 5: find the customer with the smallest insertion cost.
                    // how to find this customer quickly?
                    // maybe only search relatively nearby customers?

                    // 6: insert it into the round.

                    // 7: check if it still is within bounds.
                    //    NO: 7.1
                    //    YES:7.2
                    // 7.1: NO: optimize used all the usual operators.
                    //      HERE ANOTHER OPTIMISATION PART!
                    //      MAYBE LIN-KERIGNAN?
                    //      
                    //      Is there a more optimal solution?
                    //          YES:    7.1.1
                    //          NO:     7.1.2

                    // 7.1.1:   

                    // 7.1.2:   


                    // 7.2: goto step 5.
                }
            }

            Console.WriteLine("Resolved in {0} seconds!", new TimeSpan(ticks_total_resolved / test_count).TotalSeconds);
            Console.WriteLine("Calculated in {0} seconds!", new TimeSpan(ticks_total_calculated / test_count).TotalSeconds);
            Console.WriteLine("Calculated in {0} seconds!", new TimeSpan(ticks_total_calculated2 / test_count).TotalSeconds);
            Console.WriteLine("Total {0} seconds!", new TimeSpan(ticks_total_total / test_count).TotalSeconds);
            Console.ReadLine();
        }

        public static double Distance(ResolvedPoint p1, ResolvedPoint p2)
        {
            return p1.Location.DistanceEstimate(
                p2.Location).Value;
        }
    }
}

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
using Osm.Routing.Core.Route;
using Osm.Routing.Core.TSP.Genetic;
using Osm.Routing.Core.TSP.RandomizedArbitraryInsertion;
using Osm.Data.Raw.XML.OsmSource;

namespace Osm.Routing.Test.Tsp
{
    class TspTest
    {
        public static void Test(string name, int test_count)
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            DataSet data = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new System.IO.FileInfo(info.FullName + string.Format("\\Tsp\\{0}.csv", name)), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
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

            int current_count = test_count;
            while (current_count > 0)
            {
                // initialize data.
                IDataSource data_source = new OsmDataSource(
                    new OsmDocument(new XmlFileSource(info.FullName + string.Format("\\Tsp\\{0}.osm", name))));

                // create router.
                Router router = new Router(data_source);

                // calculate matrix.
                long ticks = DateTime.Now.Ticks;
                ResolvedPoint[] points = router.Resolve(coordinates.ToArray());
                long ticks_resolved = DateTime.Now.Ticks;
                Console.WriteLine("Resolved in {0} seconds!", new TimeSpan(ticks_resolved - ticks).TotalSeconds);
                ticks_total_resolved = ticks_total_resolved + (ticks_resolved - ticks);

                //router.CreateCachedRouter(points);
                //ResolvedPoint from = points[0];
                //ResolvedPoint to = points[0];
                //points.RemoveAt(0);
                //RouterTSPGenetic<ResolvedPoint> tsp_router = new RouterTSPGenetic<ResolvedPoint>(router);
                RouteTSPRAI<ResolvedPoint> tsp_router = new RouteTSPRAI<ResolvedPoint>(router);
                OsmSharpRoute route = tsp_router.CalculateTSP(points, false);
                route.SaveAsGpx(new FileInfo(string.Format("{0}{1}.gpx", name, current_count)));
                long ticks_calculated = DateTime.Now.Ticks;
                ticks_total_calculated = ticks_total_calculated + (ticks_calculated - ticks_resolved);
                Console.WriteLine("Calculated in {0} seconds!", new TimeSpan(ticks_calculated - ticks_resolved).TotalSeconds);
                Console.WriteLine("Total {0} seconds!", new TimeSpan(ticks_calculated - ticks).TotalSeconds);
                ticks_total_total = ticks_total_total + (ticks_calculated - ticks);
                current_count--;
            }

            Console.WriteLine("Resolved in {0} seconds!", new TimeSpan(ticks_total_resolved / test_count).TotalSeconds);
            Console.WriteLine("Calculated in {0} seconds!", new TimeSpan(ticks_total_calculated / test_count).TotalSeconds);
            Console.WriteLine("Total {0} seconds!", new TimeSpan(ticks_total_total / test_count).TotalSeconds);
            Console.ReadLine();

        }
    }
}

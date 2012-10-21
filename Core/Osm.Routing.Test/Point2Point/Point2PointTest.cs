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
using System.IO;
using Tools.Math.Geo;
using System.Data;
using Osm.Data;
using Osm.Core.Xml;
using Tools.Xml.Sources;
using Osm.Routing.Raw;
using Osm.Data.Raw.XML.OsmSource;

namespace Osm.Routing.Test.Point2Point
{
    class Point2PointTest
    {
        public static void TestSimple()
        {
            Point2PointTest.Test("matrix", 1);
        }


        public static void TestBerchem()
        {
            Point2PointTest.Test("berchem", 1);
        }


        public static void Test(string name, int test_count)
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            DataSet data = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new System.IO.FileInfo(info.FullName + string.Format("\\Point2Point\\{0}.csv", name)), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
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
                    new OsmDocument(new XmlFileSource(info.FullName + string.Format("\\Point2Point\\{0}.osm", name))));

                // create router.
                Router router = new Router(data_source);

                // calculate matrix.
                long ticks = DateTime.Now.Ticks;
                List<ResolvedPoint> points = new List<ResolvedPoint>(router.Resolve(coordinates.ToArray()));
                long ticks_resolved = DateTime.Now.Ticks;
                Console.WriteLine("Resolved in {0} seconds!", new TimeSpan(ticks_resolved - ticks).TotalSeconds);
                ticks_total_resolved = ticks_total_resolved + (ticks_resolved - ticks);

                router.Calculate(points[0], points[1]);
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

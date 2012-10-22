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
using Osm.Data;
using Tools.Xml.Sources;
using System.Data;
using System.IO;
using Tools.Math.Geo;
using Osm.Routing.Raw;
using Osm.Data.Raw.XML.OsmSource;
using Osm.Core.Xml;
using Tools.TSPLIB.Problems;

namespace Osm.Routing.Test.Matrix
{
    class MatrixTest
    {
        public static void TestSmall()
        {
            MatrixTest.Test("matrix");
        }
        //public static void TestLebbeke()
        //{
        //    MatrixTest.Test("lebbeke");
        //}

        public static void TestBigArea()
        {
            MatrixTest.Test("matrix_big_area");
        }

        internal static void TestAtomicSmall()
        {
            MatrixTest.Test("atomic");
        }

        internal static void TestMoscow()
        {
            MatrixTest.Test("moscow");
        }

        internal static void TestTiny()
        {
            MatrixTest.Test("tiny");
        }

        public static void Test(string name)
        {
            MatrixTest.Test(name, 10);
        }

        public static void Test(string name, int test_count)
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            DataSet data = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null, 
                new System.IO.FileInfo(info.FullName + string.Format("\\Matrix\\{0}.csv",name)), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
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
                IDataSourceReadOnly data_source = new OsmDataSource(
                    new OsmDocument(new XmlFileSource(info.FullName + string.Format("\\Matrix\\{0}.osm",name))));
                data_source = new Osm.Data.Cache.DataSourceCache(data_source, 12);

                // create router.
                Router router = new Router(data_source);
                //router.RegisterProgressReporter(new ConsoleProgressReporter());
                // calculate matrix.
                long ticks = DateTime.Now.Ticks;
                ResolvedPoint[] points = router.Resolve(coordinates.ToArray());
                long ticks_resolved = DateTime.Now.Ticks;
                Console.WriteLine("Resolved in {0} seconds!", new TimeSpan(ticks_resolved - ticks).TotalSeconds);
                ticks_total_resolved = ticks_total_resolved + (ticks_resolved - ticks);

                // check connectivity for all points.
                bool[] connectivity = router.CheckConnectivity(points, 1000);

                //router.CalculateManyToManyWeightsSparse(points);
                float[][] matrix = router.CalculateManyToManyWeight(points, points);
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

        public static void Test(string name, string file_name, IDataSourceReadOnly source)
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            DataSet data = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new System.IO.FileInfo(file_name), 
                Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
            foreach (DataRow row in data.Tables[0].Rows)
            {
                // be carefull with the parsing and the number formatting for different cultures.
                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
                coordinates.Add(point);
            }

            // create router.
            Router router = new Router(source);
            ResolvedPoint[] points = router.Resolve(coordinates.ToArray());
            List<ResolvedPoint> connected_points = new List<ResolvedPoint>();
            // check connectivity for all points.
            bool[] connectivity = router.CheckConnectivity(points, 300);
            for (int idx = 0; idx < connectivity.Length; idx++)
            {
                if (connectivity[idx])
                {
                    connected_points.Add(points[idx]);
                }
            }
            points = connected_points.ToArray();
            float[][] matrix = router.CalculateManyToManyWeight(points, points);

            TSPLIBProblem problem = new TSPLIBProblem(
                name, name, matrix.Length, matrix, TSPLIBProblemWeightTypeEnum.Explicit, TSPLIBProblemTypeEnum.ATSP);

            // save the atsp.
            Tools.TSPLIB.Parser.TSPLIBProblemGenerator.Generate(new FileInfo(string.Format("{0}.atsp", name)),
                problem);
            TSPLIBProblem tsp = Tools.TSPLIB.Convertor.ATSP_TSP.ATSP_TSPConvertor.Convert(problem);
            Tools.TSPLIB.Parser.TSPLIBProblemGenerator.Generate(new FileInfo(string.Format("{0}.tsp", name)),
                tsp);
        }
    }
}

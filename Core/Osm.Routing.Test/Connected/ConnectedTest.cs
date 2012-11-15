//// OsmSharp - OpenStreetMap tools & library.
//// Copyright (C) 2012 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using OsmSharp.Tools.Math.Geo;
//using System.Data;
//using OsmSharp.Osm.Data;
//using OsmSharp.Tools.Xml.Sources;
//using OsmSharp.Osm.Routing.Raw;
//using OsmSharp.Osm.Data.Raw.XML.OsmSource;
//using OsmSharp.Osm.Core.Xml;

//namespace OsmSharp.Osm.Routing.Test.Connected
//{
//    class ConnectedTest
//    {
//        public static void TestMatrix()
//        {
//            List<bool> connectivity = ConnectedTest.Test("matrix");
//        }

//        public static void TestMatrixBad()
//        {
//            List<bool> connectivity = ConnectedTest.Test("matrix_bad");
//        }

//        public static void TestSteendorp()
//        {
//            List<bool> connectivity = ConnectedTest.Test("error_steendorp");
//        }

//        public static List<bool> Test(string name)
//        {
//            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

//            // read matrix points.
//            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
//            string csv_path = Path.Combine(info.FullName, "Connected",string.Format("{0}.csv",name));
//            DataSet data = OsmSharp.Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
//                new System.IO.FileInfo(csv_path), OsmSharp.Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
//            foreach (DataRow row in data.Tables[0].Rows)
//            {
//                // be carefull with the parsing and the number formatting for different cultures.
//                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
//                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

//                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
//                coordinates.Add(point);
//            }

//            // initialize data.
//            string osm_path = Path.Combine(info.FullName, "Connected",string.Format("{0}.osm",name));
//            IDataSource data_source = new OsmDataSource(
//                new OsmDocument(new XmlFileSource(osm_path)));

//            // create router.
//            Router router = new Router(data_source);

//            // resolve all points
//            ResolvedPoint[] points = router.Resolve(coordinates.ToArray());

//            // check connectivity for all points.
//            bool[] connectivity = router.CheckConnectivity(points, 1000);

//            router.CalculateManyToManyWeight(points, points);

//            return new List<bool>(connectivity);
//        }
//    }
//}

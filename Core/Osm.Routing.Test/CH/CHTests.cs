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

//using OsmSharp.Osm.Routing.Core;
//using OsmSharp.Osm.Routing.Core.Interpreter;
//using OsmSharp.Osm.Routing.Core.Constraints;
//using OsmSharp.Osm.Data.XML.Raw.Processor;
//using System.IO;
//using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
//using OsmSharp.Osm.Routing.CH.Processor;
//using OsmSharp.Osm.Routing.CH.PreProcessing;
//using OsmSharp.Osm.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
//using OsmSharp.Osm.Routing.CH.PreProcessing.Witnesses;
//using System.Reflection;
//using OsmSharp.Tools.Math.Geo;
//using System.Collections.Generic;
//namespace OsmSharp.Osm.Routing.Test.CH
//{
//    class CHTest
//    {
//        /// <summary>
//        /// Does some testing!
//        /// </summary>
//        /// <param name="name"></param>
//        /// <param name="test_count"></param>
//        public static void Test(string name, int test_count)
//        {
//            CHTest.BuildRouter(string.Format("Osm.Routing.Test.CH.{0}.osm", name), string.Format("Osm.Routing.Test.CH.{0}.csv", name),
//                new OsmSharp.Osm.Routing.Core.Interpreter.Default.DefaultVehicleInterpreter(VehicleEnum.Car),
//                new OsmSharp.Osm.Routing.Core.Constraints.Cars.DefaultCarConstraints());
//        }

//        /// <summary>
//        /// Returns a new router.
//        /// </summary>
//        /// <param name="interpreter"></param>
//        /// <param name="constraints"></param>
//        /// <returns></returns>
//        public static void BuildRouter(string xml_embedded, string csv_embedded, 
//            RoutingInterpreterBase interpreter, IRoutingConstraints constraints)
//        {
//            // build the memory data source.
//            CHDataSource data = new CHDataSource();

//            // load the data.
//            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
//                Assembly.GetExecutingAssembly().GetManifestResourceStream(xml_embedded));
//            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
//            sorter.RegisterSource(data_processor_source);
//            CHDataProcessorTarget ch_target = new CHDataProcessorTarget(data);
//            ch_target.RegisterSource(sorter);
//            ch_target.Pull();

//            // do the pre-processing part.
//            CHPreProcessor pre_processor = new CHPreProcessor(data.Graph,
//                new SparseOrdering(data.Graph), new DykstraWitnessCalculator(data.Graph));
//            pre_processor.Start();

//            // create the router from the contracted data.
//            IRouter<CHResolvedPoint> router = new Router(data);

//            // read matrix points.
//            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
//            string[][] lines = OsmSharp.Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFileFromStream(
//                Assembly.GetExecutingAssembly().GetManifestResourceStream(csv_embedded), OsmSharp.Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated);
//            foreach (string[] row in lines)
//            {
//                // be carefull with the parsing and the number formatting for different cultures.
//                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
//                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

//                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
//                coordinates.Add(point);
//            }

//            // resolve the point(s).
//            CHResolvedPoint[] points = router.Resolve(coordinates.ToArray());

//            // calculate the matrix.
//            float[][] weights = router.CalculateManyToManyWeight(points, points);
//        }
//    }
//}
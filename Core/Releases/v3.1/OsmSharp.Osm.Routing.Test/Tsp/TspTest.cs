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
using System.IO;
using OsmSharp.Tools.Math.Geo;
using System.Data;
using OsmSharp.Osm.Data;
using OsmSharp.Tools.Xml.Sources;
using OsmSharp.Osm.Core.Xml;
using OsmSharp.Osm.Data.Raw.XML.OsmSource;
using System.Reflection;
using OsmSharp.Routing.Core;
using OsmSharp.Osm.Routing.Interpreter;
using OsmSharp.Osm.Core;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Osm.Routing.Data;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Core.Graph.Router.Dykstra;
using OsmSharp.Routing.Core.Route;
using OsmSharp.Routing.Core.Graph.DynamicGraph.PreProcessed;
using OsmSharp.Routing.Core.TSP;
using OsmSharp.Routing.Core.TSP.Genetic;
using OsmSharp.Osm.Data.XML.Processor;

namespace OsmSharp.Osm.Routing.Test.Tsp
{
    class TspTest
    {
        public static void Execute()
        {
            TspTest.Test("schendelbeke",
                Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.Osm.Routing.Test.TestData.schendelbeke.osm"),
                false);
        }

        public static void Test(string name, Stream data_stream, bool pbf)
        {
            // create the router.
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<PreProcessedEdge> osm_data =
                new MemoryRouterDataSource<PreProcessedEdge>(tags_index);
            PreProcessedDataGraphProcessingTarget target_data = new PreProcessedDataGraphProcessingTarget(
                osm_data, interpreter, osm_data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(data_stream);
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            IRouter<RouterPoint> router = new Router<PreProcessedEdge>(osm_data, interpreter,
                new DykstraRoutingPreProcessed(osm_data.TagsIndex));

            // read the source files.
            int latitude_idx = 2;
            int longitude_idx = 3;
            string[][] point_strings = OsmSharp.Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFileFromStream(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(@"OsmSharp.Osm.Routing.Test.TestData.{0}.csv", name)),
                Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated);
            List<RouterPoint> points = new List<RouterPoint>();
            int cnt = 10;
            foreach (string[] line in point_strings)
            {
                if (points.Count >= cnt)
                {
                    break;
                }
                string latitude_string = (string)line[latitude_idx];
                string longitude_string = (string)line[longitude_idx];

                //string route_ud = (string)line[1];

                double longitude = 0;
                double latitude = 0;
                if (double.TryParse(longitude_string, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out longitude) &&
                   double.TryParse(latitude_string, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out latitude))
                {
                    GeoCoordinate point = new GeoCoordinate(latitude, longitude);

                    RouterPoint resolved = router.Resolve(VehicleEnum.Car, point);
                    if (resolved != null && router.CheckConnectivity(VehicleEnum.Car, resolved, 100))
                    {
                        points.Add(resolved);
                    }
                }
            }

            RouterTSPWrapper<RouterPoint, RouterTSP> tsp_solver = new RouterTSPWrapper<RouterPoint, RouterTSP>(
                new RouterTSPAEXGenetic(), router);
            OsmSharpRoute tsp = tsp_solver.CalculateTSP(VehicleEnum.Car, points.ToArray());
            tsp.SaveAsGpx(new FileInfo(@"c:\temp\tsp.gpx"));
            OsmSharpRoute tsp_open_first = tsp_solver.CalculateTSP(VehicleEnum.Car, points.ToArray(), 0, false);
            tsp_open_first.SaveAsGpx(new FileInfo(@"c:\temp\tsp_open_first.gpx"));
            OsmSharpRoute tsp_open = tsp_solver.CalculateTSP(VehicleEnum.Car, points.ToArray(), false);
            tsp_open.SaveAsGpx(new FileInfo(@"c:\temp\tsp_open.gpx"));

            //double[][] weights = router.CalculateManyToManyWeight(VehicleEnum.Car, points.ToArray(), points.ToArray());

            //router.Calculate(VehicleEnum.Car, points[0], points[1]).SaveAsGpx(new FileInfo(@"c:\temp\fromO_to1.gpx"));
            //router.Calculate(VehicleEnum.Car, points[1], points[0]).SaveAsGpx(new FileInfo(@"c:\temp\from1_to0.gpx"));
            //router.Calculate(VehicleEnum.Car, points[0], points[9]).SaveAsGpx(new FileInfo(@"c:\temp\fromO_to9.gpx"));
            //router.Calculate(VehicleEnum.Car, points[9], points[0]).SaveAsGpx(new FileInfo(@"c:\temp\from9_to0.gpx"));
            ////router.Calculate(points[0], points[1]).SaveAsGpx(new FileInfo("fromO_to1.gpx"));
        }
    }
}
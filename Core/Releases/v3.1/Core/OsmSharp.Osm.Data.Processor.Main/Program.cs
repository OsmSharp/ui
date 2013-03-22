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
using OsmSharp.Osm.Data.Oracle;
using OsmSharp.Osm.Data.Core.Processor.Filter;
using OsmSharp.Osm.Data.Oracle.Raw;
using OsmSharp.Osm.Data.Oracle.Raw.Processor.ChangeSets;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Osm.Data.Oracle.Raw.Processor;
using OsmSharp.Osm.Data.Core.Processor.Default;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using System.IO;
using OsmSharp.Osm.Data.PostgreSQL.SimpleSchema.Processor;
using OsmSharp.Osm.Data.XML.Processor;
using OsmSharp.Osm.Data.XML.Processor.ChangeSets;

namespace OsmSharp.Osm.Data.Processor.Main
{
    class Program
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // register the output stream to the console.
            OsmSharp.Tools.Output.OutputStreamHost.RegisterOutputStream(
                new OsmSharp.Tools.Output.ConsoleOutputStream());

            string source_file = @"c:\OSM\bin\belgium.osm.pbf";
            string connection_string = 
                "Server=10.0.0.11;Port=5432;User Id=postgres;Password=mixbeton;Database=osmsharp_test;";

            TestImportBelgium(source_file, connection_string);
        }

        private static void TestImportBelgium(string source_file, string connection_string)
        {
            PBFDataProcessorSource source = new PBFDataProcessorSource(new FileInfo(source_file).OpenRead());
            PostgreSQLSimpleSchemaDataProcessorTarget test_target = new 
                PostgreSQLSimpleSchemaDataProcessorTarget(connection_string, true);
            test_target.RegisterSource(source);
            test_target.Pull();
        }

        private static void TestXmlWriter()
        {
            XmlDataProcessorSource source = new XmlDataProcessorSource(@"zand_small.osm");

            XmlDataProcessorTarget target = new XmlDataProcessorTarget(@"zand_small_test.osm");
            target.RegisterSource(source);
            target.Pull();
            //target.Close();
        }

        private static void TestBBFilter(string input_file,string output_file, double top, double bottom, double left, double right)
        {
            XmlDataProcessorSource source = new XmlDataProcessorSource(input_file);

            DataProcessorFilterBoundingBox box_filter = new DataProcessorFilterBoundingBox(new OsmSharp.Tools.Math.Geo.GeoCoordinateBox(new OsmSharp.Tools.Math.Geo.GeoCoordinate(
                top, left), new OsmSharp.Tools.Math.Geo.GeoCoordinate(bottom, right)));
            box_filter.RegisterSource(source);

            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(box_filter);

            XmlDataProcessorTarget target = new XmlDataProcessorTarget(output_file);
            target.RegisterSource(sorter);
            target.Pull();
            //target.Close();
        }

        private static void Test()
        {
            // truncate
            OracleSimpleDataProcessorTruncateTarget truncate_target = new OracleSimpleDataProcessorTruncateTarget("");
            DataProcessorSourceEmpty empty_source = new DataProcessorSourceEmpty();
            truncate_target.RegisterSource(empty_source);
            truncate_target.Pull();
            Console.WriteLine("Truncated data!");

            // import test data
            XmlDataProcessorSource test_xml_source = new XmlDataProcessorSource(@"dohan_before.osm");
            OracleSimpleDataProcessorTarget test_oracle_target = new OracleSimpleDataProcessorTarget("");
            test_oracle_target.RegisterSource(test_xml_source);
            test_oracle_target.Pull();
            Console.WriteLine("Test data imported!");

            // apply changesets
            XmlDataProcessorChangeSetSource change_source = new XmlDataProcessorChangeSetSource("dohan_change1.osc");

            double top = 49.80672;
            double bottom = 49.78664;
            double left = 5.11993;
            double right = 5.15821;

            DataProcessorChangeSetFilterBoundingBox box_filter = new DataProcessorChangeSetFilterBoundingBox(
                new OracleSimpleSource(""),
                new OsmSharp.Tools.Math.Geo.GeoCoordinateBox(new OsmSharp.Tools.Math.Geo.GeoCoordinate(top, left), new OsmSharp.Tools.Math.Geo.GeoCoordinate(bottom, right)));
            box_filter.RegisterSource(change_source);

            OracleSimpleChangeSetApplyTarget change_target = new OracleSimpleChangeSetApplyTarget("Data source=TEST;User Id=OSM;Password=not_the_real_password_haha;", true);
            change_target.RegisterSource(box_filter);
            change_target.Pull();

            // apply changesets again
            change_source = new XmlDataProcessorChangeSetSource("dohan_change2.osc");

            change_target = new OracleSimpleChangeSetApplyTarget("Data source=TEST;User Id=OSM;Password=not_the_real_password_haha;", true);
            change_target.RegisterSource(change_source);
            change_target.Pull();

            OracleSimpleDataProcessorSource source = new OracleSimpleDataProcessorSource("Data source=TEST;User Id=OSM;Password=not_the_real_password_haha;");

            XmlDataProcessorTarget target = new XmlDataProcessorTarget("output.osm");
            target.RegisterSource(source);
            target.Pull();
        }
    }
}

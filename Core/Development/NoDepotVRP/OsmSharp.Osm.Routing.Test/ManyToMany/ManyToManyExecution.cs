using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OsmSharp.Osm.Routing.Test.ManyToMany
{
    /// <summary>
    /// Executes the many-to-many tests.
    /// </summary>
    public static class ManyToManyExecution
    {
        /// <summary>
        /// Executes the actual many-to-many tests.
        /// </summary>
        public static void Execute()
        {
            //ManyToManyCustomTests tests = new ManyToManyCustomDykstraLiveTests();
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM101.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM103.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM104.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM105.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM105A.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM116.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM200.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM201.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM202.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM213.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM800.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM801.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM802.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM850.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM850C.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM851.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM851W.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM852.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM900.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM901.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM907.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM908.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM909.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM909A.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM911.csv", true);

            //tests = new ManyToManyCustomCHTests();
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM101.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM103.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM104.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM105.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM105A.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM116.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM200.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM201.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM202.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM213.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM800.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM801.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM802.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM850.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM850C.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM851.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM851W.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM852.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM900.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM901.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM907.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM908.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM909.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM909A.csv", true);
            //tests.Execute((new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Deltamedia\DM911.csv", true);

            //ManyToMany.ManyToManyRawTests raw_tests = new ManyToMany.ManyToManyRawTests();
            //raw_tests.TestFor("tiny");
            //raw_tests.TestFor("matrix");
            //raw_tests.TestFor("matrix_big_area");
            //raw_tests.TestFor("eeklo");
            //raw_tests.TestFor("lebbeke");

            //ManyToManyDykstraLiveTests live_tests = new ManyToManyDykstraLiveTests();
            //live_tests.TestFor("tiny");
            //live_tests.TestFor("matrix");
            //live_tests.TestFor("matrix_big_area");
            //live_tests.TestFor("eeklo");
            //live_tests.TestFor("lebbeke");
            //live_tests.TestFor("moscow");

            ManyToMany.ManyToManyCHTests ch_tests = new ManyToMany.ManyToManyCHTests();
            ch_tests.TestForAndCompare("tiny");
            ch_tests.TestForAndCompare("matrix");
            ch_tests.TestForAndCompare("matrix_big_area");
            //ch_tests.TestFor("tiny");
            //ch_tests.TestFor("matrix");
            //ch_tests.TestFor("matrix_big_area");
            //ch_tests.TestFor("eeklo");
            //ch_tests.TestFor("lebbeke");
            //ch_tests.TestFor("moscow");
        }

        private static void PrintResults(float[][] weights)
        {
            for (int x = 0; x < weights.Length; x++)
            {
                for (int y = 0; y < weights[x].Length; y++)
                {
                    OsmSharp.Tools.Core.Output.OutputStreamHost.Write(weights[x][y].ToString().PadRight(15));
                    OsmSharp.Tools.Core.Output.OutputStreamHost.Write(" ");
                }
                OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine();
            }
        }
    }
}

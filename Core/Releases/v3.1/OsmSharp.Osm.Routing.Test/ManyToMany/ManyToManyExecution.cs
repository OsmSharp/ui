using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            //ManyToMany.ManyToManyRawTests raw_tests = new ManyToMany.ManyToManyRawTests();
            //raw_tests.TestFor("tiny");
            //raw_tests.TestFor("matrix");
            //raw_tests.TestFor("matrix_big_area");
            //raw_tests.TestFor("eeklo");
            //raw_tests.TestFor("lebbeke");

            ManyToManyDykstraLiveTests live_tests = new ManyToManyDykstraLiveTests();
            //live_tests.TestFor("tiny");
            //live_tests.TestFor("matrix");
            //live_tests.TestFor("matrix_big_area");
            //live_tests.TestFor("eeklo");
            //live_tests.TestFor("lebbeke");
            //live_tests.TestFor("moscow");

            ManyToMany.ManyToManyCHTests ch_tests = new ManyToMany.ManyToManyCHTests();
            //ch_tests.TestForAndCompare("tiny");
            //ch_tests.TestForAndCompare("matrix");
            //ch_tests.TestFor("tiny");
            ch_tests.TestFor("matrix");
            ch_tests.TestFor("matrix_big_area");
            ch_tests.TestFor("eeklo");
            ch_tests.TestFor("lebbeke");
            ch_tests.TestFor("moscow");
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

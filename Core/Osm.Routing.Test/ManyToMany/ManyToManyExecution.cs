using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Test.ManyToMany
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
            ManyToMany.ManyToManyRawTests raw_tests = new ManyToMany.ManyToManyRawTests();
            PrintResults(raw_tests.TestFor("tiny"));

            ManyToMany.ManyToManyCHTests ch_tests = new ManyToMany.ManyToManyCHTests();
            PrintResults(ch_tests.TestFor("tiny"));
        }

        private static void PrintResults(float[][] weights)
        {
            for (int x = 0; x < weights.Length; x++)
            {
                for (int y = 0; y < weights[x].Length; y++)
                {
                    Tools.Core.Output.OutputTextStreamHost.Write(weights[x][y].ToString().PadRight(15));
                    Tools.Core.Output.OutputTextStreamHost.Write(" ");
                }
                Tools.Core.Output.OutputTextStreamHost.WriteLine();
            }
        }
    }
}

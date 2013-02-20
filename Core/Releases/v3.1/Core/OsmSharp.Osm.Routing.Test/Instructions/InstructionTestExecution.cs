using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Osm.Routing.Test.Instructions
{
    public class InstructionTestExecution
    {
        public static void Execute()
        {
            Point2PointDykstraBinairyHeapTests tester = new Point2PointDykstraBinairyHeapTests();
            tester.ExecuteTest("matrix", 100);
        }
    }
}

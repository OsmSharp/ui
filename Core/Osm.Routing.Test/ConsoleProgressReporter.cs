using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Core.Progress;

namespace Osm.Routing.Test
{
    class ConsoleProgressReporter : IProgressReporter
    {
        public void Report(ProgressStatus status)
        {
            Console.WriteLine(string.Format("[{0}]:{1}",
                status.ProgressPercentage, status.Message));
        }
    }
}

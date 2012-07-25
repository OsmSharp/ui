using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Processor.Filter;

namespace Osm.Data.Processor.Main
{
    class DummyListener : IChangeSetFilterListener
    {

        #region IChangeSetFilterListener Members

        public void ReportChangeDetected(Tools.Math.Geo.GeoCoordinateBox box)
        {
            Tools.Core.Output.OutputTextStreamHost.WriteLine(box.ToString());
        }

        #endregion
    }
}

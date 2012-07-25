using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;

namespace Osm.Data.Core.Processor.Filter
{
    public interface IChangeSetFilterListener
    {
        void ReportChangeDetected(GeoCoordinateBox box);
    }
}
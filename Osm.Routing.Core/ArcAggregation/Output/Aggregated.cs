using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Core.ArcAggregation.Output
{
    public abstract class Aggregated
    {
        public abstract Aggregated GetNext();
    }
}

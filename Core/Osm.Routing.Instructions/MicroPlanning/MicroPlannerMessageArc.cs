using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Core.ArcAggregation.Output;

namespace Osm.Routing.Instructions.MicroPlanning
{
    internal class MicroPlannerMessageArc : MicroPlannerMessage
    {
        public AggregatedArc Arc { get; set; }
    }
}

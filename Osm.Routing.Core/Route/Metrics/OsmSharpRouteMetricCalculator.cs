using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Osm.Routing.Core.Route;
using Osm.Routing.Core.ArcAggregation.Output;
using Osm.Routing.Core;

namespace Osm.Routing.Core.Metrics
{
    public abstract class OsmSharpRouteMetricCalculator
    {
        public Dictionary<string, double> Calculate(OsmSharpRoute route)
        {
            Osm.Routing.Core.ArcAggregation.ArcAggregator aggregator = new Osm.Routing.Core.ArcAggregation.ArcAggregator();
            AggregatedPoint p = aggregator.Aggregate(route);
            return this.Calculate(route.Vehicle, p);
        }

        public abstract Dictionary<string, double> Calculate(VehicleEnum vehicle, AggregatedPoint p);
    }
}

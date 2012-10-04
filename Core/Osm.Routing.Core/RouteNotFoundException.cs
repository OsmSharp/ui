using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core
{
    public class RouteNotFoundException : Exception
    {
        public IResolvedPoint From { get; set; }

        public IEnumerable<IResolvedPoint> To { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;
using OsmSharp.Routing.Core;
using OsmSharpService.Core.Routing.Primitives;

namespace OsmSharpService.Core.Routing
{
    /// <summary>
    /// Resolving service; request to resolve a point.
    /// </summary>
    [Route("/resolving", "GET,POST,PUT,OPTIONS")]
    public class ResolveOperation
    {
        /// <summary>
        /// The vehicle type.
        /// </summary>
        public VehicleEnum Vehicle { get; set; }

        /// <summary>
        /// The hooks to resolve.
        /// </summary>
        public RoutingHook[] Hooks { get; set; }
    }
}

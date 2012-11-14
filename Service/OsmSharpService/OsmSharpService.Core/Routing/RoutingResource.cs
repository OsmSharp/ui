using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace OsmSharpService.Core.Routing
{
    /// <summary>
    /// Routing resource; tells the services what to route.
    /// </summary>
    [Route("/routing", "GET,POST,PUT,OPTIONS")]
    [DataContract]
    public class RoutingResource
    {
        /// <summary>
        /// The hooks for the router to route on.
        /// </summary>
        public RoutingHook[] Hooks { get; set; }

        /// <summary>
        /// Returns only the weights when true.
        /// </summary>
        public bool OnlyWeights { get; set; }
    }

    /// <summary>
    /// A routing hook; a point route from/to.
    /// </summary>
    public class RoutingHook
    {
        /// <summary>
        /// The id of the hook.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The latitude.
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// The longitude
        /// </summary>
        public float Longitude { get; set; }
    }
}

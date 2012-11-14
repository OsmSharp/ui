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
    public class RoutingOperation
    {
        /// <summary>
        /// Holds the routing resource type.
        /// </summary>
        public RoutingOperationType Type { get; set; }

        /// <summary>
        /// The hooks for the router to route on.
        /// </summary>
        public RoutingHook[] Hooks { get; set; }
    }

    /// <summary>
    /// Represents the type of resource requested.
    /// </summary>
    public enum RoutingOperationType
    {
        /// <summary>
        /// Returns a route along all the points.
        /// </summary>
        Regular,
        /// <summary>
        /// Returns a shortest route along all the points.
        /// </summary>
        TSP,
        /// <summary>
        /// Returns all the weights between the given points.
        /// </summary>
        ManyToMany
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

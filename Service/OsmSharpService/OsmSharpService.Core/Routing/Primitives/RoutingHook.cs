using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharpService.Core.Routing.Primitives
{
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

        /// <summary>
        /// The routing hook tags.
        /// </summary>
        public RoutingHookTag[] Tags { get; set; }
    }
}
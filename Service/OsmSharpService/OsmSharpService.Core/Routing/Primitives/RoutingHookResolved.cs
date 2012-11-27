using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharpService.Core.Routing.Primitives
{
    /// <summary>
    /// A resolved routing hook; a point route from/to that was resolved to the closest connecting road.
    /// </summary>
    public class RoutingHookResolved
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
        /// The hook was succesfully resolved.
        /// </summary>
        public bool Succes { get; set; }
    }
}

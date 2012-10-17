using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Core.Roads.Tags;
using Osm.Routing.Core.Interpreter;

namespace Osm.Routing.Core.Resolving
{
    /// <summary>
    /// Interface used to match a coordinate to a configurable routable position.
    /// </summary>
    public interface IResolveMatcher
    {
        /// <summary>
        /// Returns true if the point is a suitable candidate as a point being resolved on the given way.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        bool Match(RoutingWayInterperterBase way_interpreter);
    }
}
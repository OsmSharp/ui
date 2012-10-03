using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Core.Resolving
{
    /// <summary>
    /// Interface used to match a coordinate to a configurable routable position.
    /// </summary>
    public interface IResolveMatcher
    {
        /// <summary>
        /// Returns true if the way is a suitable candidate for the point being resolved.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        bool MatchName(string name);
    }
}

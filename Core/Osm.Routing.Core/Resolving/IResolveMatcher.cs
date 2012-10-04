using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Core.Resolving
{
    /// <summary>
    /// Interface used to match a coordinate to a configurable routable position.
    /// </summary>
    public interface IResolveMatcher<ResolvedType>
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Returns true if the point is a suitable candidate as a point being resolved.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        bool MatchName(ResolvedType point);
    }
}

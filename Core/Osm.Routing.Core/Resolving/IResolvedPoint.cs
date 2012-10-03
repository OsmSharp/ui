using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Osm.Core;

namespace Osm.Routing.Core.Resolving
{
    /// <summary>
    /// Represents a resolved point. A hook for the router to route on.
    /// 
    /// The object represents a location and can be tagged.
    /// </summary>
    public interface IResolvedPoint : ILocationObject, ITaggedObject
    {

    }
}

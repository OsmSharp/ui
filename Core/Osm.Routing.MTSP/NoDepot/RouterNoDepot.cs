using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Core.Route;
using Osm.Core;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core.VRP.NoDepot
{
    /// <summary>
    /// Class to solve for a specific class of VRP problems: VRP problems with any depot.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class RouterNoDepot<ResolvedType> : RouterVRP<ResolvedType>
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Creates a VRP router without a depot.
        /// </summary>
        /// <param name="router"></param>
        public RouterNoDepot(IRouter<ResolvedType> router)
            :base(router)
        {

        }

        /// <summary>
        /// Calculates this VRP No Depot.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public abstract OsmSharpRoute[] CalculateNoDepot(ResolvedType[] points);
    }
}

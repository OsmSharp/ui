using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core
{
    public static class IRouterExtensions
    {
        /// <summary>
        /// Checks connectivity of all given points and returns only those that are valid.
        /// </summary>
        /// <typeparam name="ResolvedType"></typeparam>
        /// <param name="router"></param>
        /// <param name="resolved_points"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ResolvedType[] CheckConnectivityAndRemoveInvalid<ResolvedType>(
            this IRouter<ResolvedType> router, ResolvedType[] resolved_points, float weight)
                where ResolvedType : IResolvedPoint
        {
            List<ResolvedType> connected_points = new List<ResolvedType>();
            for(int idx = 0; idx < resolved_points.Length; idx++)
            {
                ResolvedType resolved_point = resolved_points[idx];
                if (resolved_point != null &&
                    router.CheckConnectivity(resolved_point, weight))
                { // the point is connected.
                    connected_points.Add(resolved_point);
                }

                // report progress.
                Tools.Core.Output.OutputStreamHost.ReportProgress(idx, resolved_points.Length, "Router.Core.CheckConnectivityAndRemoveInvalid",
                    "Checking connectivity...");
            }
            return connected_points.ToArray();
        }
    }
}

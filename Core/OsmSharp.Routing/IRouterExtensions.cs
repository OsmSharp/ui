// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Routing
{
    /// <summary>
    /// Contains common IRouter extensions.
    /// </summary>
    public static class IRouterExtensions
    {
        /// <summary>
        /// Checks connectivity of all given points and returns only those that are valid.
        /// </summary>
        /// <typeparam name="ResolvedType"></typeparam>
        /// <param name="router"></param>
        /// <param name="vehicle"></param>
        /// <param name="resolved_points"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static ResolvedType[] CheckConnectivityAndRemoveInvalid<ResolvedType>(
            this IRouter<ResolvedType> router, VehicleEnum vehicle, ResolvedType[] resolved_points, float weight)
                where ResolvedType : IRouterPoint
        {
            List<ResolvedType> connected_points = new List<ResolvedType>();
            for (int idx = 0; idx < resolved_points.Length; idx++)
            {
                ResolvedType resolved_point = resolved_points[idx];
                if (resolved_point != null &&
                    router.CheckConnectivity(vehicle, resolved_point, weight))
                { // the point is connected.
                    connected_points.Add(resolved_point);
                }

                // report progress.
                OsmSharp.Tools.Output.OutputStreamHost.ReportProgress(idx, resolved_points.Length, "Router.Core.CheckConnectivityAndRemoveInvalid",
                    "Checking connectivity...");
            }
            return connected_points.ToArray();
        }
    }
}

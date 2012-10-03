using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core;
using Tools.Math.Units.Time;
using Osm.Core;
using Tools.Math.VRP.Core.Routes;
using Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.VNS
{
    /// <summary>
    /// Uses a Variable Neighbourhood Search technique.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public class VNSSimple<ResolvedType> : RouterMaxTime<ResolvedType>
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Holds the router.
        /// </summary>
        private IRouter<ResolvedType> _router;

        public VNSSimple(IRouter<ResolvedType> router, Second max, Second delivery_time)
            : base(router, max, delivery_time)
        {

        }


        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "VNS";
            }
        }

        /// <summary>
        /// Executes a solver procedure.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override MaxTimeSolution Solve(MaxTimeProblem problem)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// Does the actual calculation.
        ///// </summary>
        ///// <param name="problem"></param>
        ///// <param name="customers"></param>
        ///// <param name="min"></param>
        ///// <param name="max"></param>
        ///// <returns></returns>
        //public override int[][] DoCalculation(MaxTimeProblem problem, ICollection<int> customers, 
        //    Second max)
        //{
        //    RouterBestPlacementWithImprovements<ResolvedType> vrp_router = 
        //        new RouterBestPlacementWithImprovements<ResolvedType>(
        //            _router, problem.Max, problem.DeliveryTime, 10, 0.25f);
        //    MaxTimeSolution routes = vrp_router.DoCalculationInternal(
        //        problem, customers, max);

        //    //while (customers_to_place.Count > 0)
        //    //{

        //    //}


        //    // convert output.
        //    int[][] solution = new int[routes.Count][];
        //    for (int idx = 0; idx < routes.Count; idx++)
        //    {
        //        IRoute current = routes.Route(idx);
        //        //IRoute current = routes[idx];
        //        List<int> route = new List<int>(current);
        //        if (current.IsRound)
        //        {
        //            route.Add(route[0]);
        //        }
        //        solution[idx] = route.ToArray();
        //    }
        //    return solution;
        //}
    }
}

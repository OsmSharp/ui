using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.LocalSearch;
using Tools.Math.VRP.Core.Routes;
using Osm.Core;
using Tools.Math.TSP;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core.TSP.RandomizedArbitraryInsertion
{
    /// <summary>
    /// A TSP router using a genetic algorithm.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public class RouteTSPRAI<ResolvedType> : RouterTSP<ResolvedType>
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Creates a new TSP router;
        /// </summary>
        /// <param name="router"></param>
        public RouteTSPRAI(IRouter<ResolvedType> router)
            : base(router)
        {

        }

        /// <summary>
        /// Creates a solver.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override ISolver DoCreateSolver()
        {
            return new RandomizedArbitraryInsertionSolver();
        }
    }
}

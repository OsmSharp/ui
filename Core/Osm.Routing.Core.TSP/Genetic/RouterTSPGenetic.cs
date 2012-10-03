using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes;
using Tools.Core.Progress;
using Osm.Core;
using Tools.Math.TSP;
using Tools.Math.TSP.Genetic;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core.TSP.Genetic
{
    /// <summary>
    /// A TSP router using a genetic algorithm.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public class RouterTSPGenetic<ResolvedType> : RouterTSP<ResolvedType>
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Creates a new TSP router;
        /// </summary>
        /// <param name="router"></param>
        public RouterTSPGenetic(IRouter<ResolvedType> router)
            :base(router)
        {

        }

        /// <summary>
        /// Creates a genetic solver.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override ISolver DoCreateSolver()
        {
            return new GeneticSolver();
        }
    }
}

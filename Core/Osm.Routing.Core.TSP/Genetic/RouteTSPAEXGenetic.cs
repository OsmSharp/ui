using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes;
using Tools.Core.Progress;
using Osm.Core;
using Tools.Math.TSP;
using Tools.Math.TSP.Genetic;
using Tools.Math.TSP.EdgeAssemblyGenetic;
using Tools.Math.TSP.Genetic.Solver.Operations.Generation;
using Tools.Math.TSP.Genetic.Solver.Operations.CrossOver;

namespace Osm.Routing.Core.TSP.Genetic
{
    /// <summary>
    /// A TSP router using a genetic algorithm.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public class RouterTSPAEXGenetic<ResolvedType> : RouterTSP<ResolvedType>
        where ResolvedType : ILocationObject
    {
        /// <summary>
        /// Creates a new TSP router;
        /// </summary>
        /// <param name="router"></param>
        public RouterTSPAEXGenetic(IRouter<ResolvedType> router)
            : base(router)
        {

        }

        /// <summary>
        /// Creates a genetic solver.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override ISolver DoCreateSolver()
        {
            return new EdgeAssemblyCrossOverSolver(100, 100,
                     new _3OptGenerationOperation(),
                      new EdgeAssemblyCrossover(30,
                             EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
                             true));
        }
    }
}

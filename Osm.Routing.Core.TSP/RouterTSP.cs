using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Core.Route;
using Tools.Math.TSP.Problems;
using Tools.Math.TSP;
using Tools.Math.VRP.Core.Routes;
using Osm.Core;

namespace Osm.Routing.Core.TSP
{
    /// <summary>
    /// Router that calculates TSP solutions.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class RouterTSP<ResolvedType>
        where ResolvedType : ILocationObject
    {
        /// <summary>
        /// Holds the basic router.
        /// </summary>
        private IRouter<ResolvedType> _router;

        /// <summary>
        /// Creates a new TSP router.
        /// </summary>
        /// <param name="router"></param>
        public RouterTSP(IRouter<ResolvedType> router)
        {
            _router = router;
        }

        /// <summary>
        /// Calculates a weight matrix for the given array of points.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected float[][] CalculateManyToManyWeight(ResolvedType[] points)
        {
            return _router.CalculateManyToManyWeight(points, points);
        }

        /// <summary>
        /// Calculates the shortest route along all given points starting and ending at the given points.
        /// </summary>
        /// <param name="points">The points to travel along.</param>
        /// <param name="first">The point to start from.</param>
        /// <param name="is_round">Return back to the first point or not.</param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(ResolvedType[] points, int first, bool is_round)
        {
            // calculate the weights.
            float[][] weights = this.CalculateManyToManyWeight(points);

            // create solver.
            ISolver solver = this.DoCreateSolver();
            solver.IntermidiateResult += new SolverDelegates.IntermidiateDelegate(solver_IntermidiateResult);

            // calculate the TSP.
            IRoute tsp_solution = solver.Solve(this.GenerateProblem(weights, first, null, is_round));

            // concatenate the route(s).
            return this.BuildRoute(points, tsp_solution);
        }

        /// <summary>
        /// Calculates the shortest route along all given points starting and ending at the given points.
        /// </summary>
        /// <param name="points">The points to travel along.</param>
        /// <param name="first">The point to start from.</param>
        /// <param name="last">The point to end at.</param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(ResolvedType[] points, int first, int last)
        {
            // calculate the weights.
            float[][] weights = this.CalculateManyToManyWeight(points);

            // create solver.
            ISolver solver = this.DoCreateSolver();
            solver.IntermidiateResult += new SolverDelegates.IntermidiateDelegate(solver_IntermidiateResult);

            // calculate the TSP.
            IRoute tsp_solution = solver.Solve(this.GenerateProblem(weights, first, last, false));

            // concatenate the route(s).
            return this.BuildRoute(points, tsp_solution);
        }

        /// <summary>
        /// Calcculates the shortest route along all given points.
        /// </summary>
        /// <param name="points">The points to travel along.</param>
        /// <param name="is_round">Make the route a round or not.</param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(ResolvedType[] points, bool is_round)
        {
            // calculate the weights.
            float[][] weights = this.CalculateManyToManyWeight(points);

            // create solver.
            ISolver solver = this.DoCreateSolver();
            solver.IntermidiateResult += new SolverDelegates.IntermidiateDelegate(solver_IntermidiateResult);

            // calculate the TSP.
            IRoute tsp_solution = solver.Solve(this.GenerateProblem(weights, null, null, is_round));

            // concatenate the route(s).
            return this.BuildRoute(points, tsp_solution);
        }

        /// <summary>
        /// Calculates the shortest route along all given points returning back to the first.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public OsmSharpRoute CalculateTSP(ResolvedType[] points)
        {
            return this.CalculateTSP(points, true);
        }

        /// <summary>
        /// Raise intermidiate result event.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="weight"></param>
        void solver_IntermidiateResult(int[] result, float weight)
        {
            if (this.CanRaiseIntermidiateResult())
            {
                this.RaiseIntermidiateResult(result, weight);
            }
        }

        /// <summary>
        /// Builds an OsmSharRoute.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="found_solution"></param>
        /// <returns></returns>
        public OsmSharpRoute BuildRoute(ResolvedType[] points, IRoute found_solution)
        {
            List<int> solution = new List<int>(found_solution); // TODO: improve this whole part to loop over the orginal route!

            OsmSharpRoute tsp = null;
            OsmSharpRoute route;
            for (int idx = 0; idx < solution.Count - 1; idx++)
            {
                route = _router.Calculate(points[solution[idx]],
                    points[solution[idx + 1]]);
                if (tsp == null)
                { // first route = start
                    tsp = route;
                }
                else
                { // concatenate.
                    tsp = OsmSharpRoute.Concatenate(tsp, route);
                }
            }
            if (found_solution.IsRound)
            {
                // concatenate the route from the last to the first point again.
                route = _router.Calculate(points[solution[solution.Count - 1]],
                            points[solution[0]]);
                return OsmSharpRoute.Concatenate(tsp, route);
            }
            return tsp;
        }

        /// <summary>
        /// Generates a problem definition.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="is_round"></param>
        /// <returns></returns>
        protected IProblem GenerateProblem(float[][] weights, int? first, int? last, bool is_round)
        {
            if (first.HasValue)
            {
                if (last.HasValue)
                {
                    return new MatrixProblem(weights, false, first.Value, last.Value);
                }
                else
                {
                    if (is_round)
                    {
                        return new MatrixProblem(weights, false, first.Value, first.Value);
                    }
                    else
                    {
                        return new MatrixProblem(weights, false, first.Value, null);
                    }
                }
            }
            if (is_round)
            {
                return new MatrixProblem(weights, false, 0, 0);
            }
            else
            {
                return new MatrixProblem(weights, false);
            }
        }

        /// <summary>
        /// Generates a solver.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal abstract ISolver DoCreateSolver();

        #region Intermidiate Results

        /// <summary>
        /// Delegate to pass on an intermidiate solution.
        /// </summary>
        /// <param name="result"></param>
        public delegate void IntermidiateDelegate(int[] result, float weight);

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        public event IntermidiateDelegate IntermidiateResult;

        /// <summary>
        /// Returns true when the event has to be raised.
        /// </summary>
        /// <returns></returns>
        protected bool CanRaiseIntermidiateResult()
        {
            return this.IntermidiateResult != null;
        }

        /// <summary>
        /// Raises the intermidiate results event.
        /// </summary>
        /// <param name="result"></param>
        protected void RaiseIntermidiateResult(int[] result, float weight)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result, weight);
            }
        }

        #endregion
    }
}

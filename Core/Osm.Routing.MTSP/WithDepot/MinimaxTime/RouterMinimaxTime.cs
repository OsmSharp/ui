using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Time;
using Osm.Routing.Core.Route;
using Tools.Math.TSP.Problems;
using Tools.Math.VRP.Core;
using Osm.Core;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core.VRP.WithDepot.MinimaxTime
{
    /// <summary>
    /// Class to solve VRP problems that have no depot but min-max time constraints on routes.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class RouterMinimaxTime<ResolvedType> : RouterDepot<ResolvedType>
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Creates a new min max VRP router.
        /// </summary>
        /// <param name="router"></param>
        public RouterMinimaxTime(IRouter<ResolvedType> router)
            : base(router)
        {
        }


        /// <summary>
        /// Calculates the actual VRP.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public override OsmSharpRoute[] CalculateDepot(ResolvedType[] clients, ResolvedType[] depots)
        {
            /// Keeps a local copy of the current calculation points.
            /// 
            /// TODO: find a better solution to make this thread-safe!
            _depots = depots;
            _clients = clients;

            ResolvedType[] points = new ResolvedType[clients.Length + depots.Length];
            depots.CopyTo(points, 0);
            clients.CopyTo(points, depots.Length);


            // first calculate the weights in seconds.
            float[][] weights = this.CalculateManyToManyWeigth(points);

            // create the problem for the genetic algorithm.
            List<int> depotsInt = new List<int>();
            for (int depot = 0; depot < depots.Length; depot++)
                depotsInt.Add(depot);


            List<int> custInt = new List<int>();
            for (int customer = depots.Length; customer < points.Length; customer++)
                custInt.Add(customer);


            MatrixProblem matrix = new MatrixProblem(weights, false);
            int[][] vrp_solution = this.DoCalculation(matrix, depotsInt, custInt);

            // construct and return solution.
            return this.ConstructSolution(vrp_solution, depots, clients);
        }

        #region Intermidiate Results

        /// <summary>
        /// Keeps a local copy of the current calculation points.
        /// 
        /// TODO: find a better solution to make this thread-safe!
        /// </summary>
        private ResolvedType[] _depots;
        private ResolvedType[] _clients;

        /// <summary>
        /// Called when an intermidiate result is available.
        /// </summary>
        /// <param name="vrp_solution"></param>
        protected void DoIntermidiateResult(int[][] vrp_solution)
        {
            if (this.CanRaiseIntermidiateResult())
            {
                OsmSharpRoute[] result = this.ConstructSolution(vrp_solution, _depots, _clients);

                Dictionary<int, List<int>> solution = new Dictionary<int, List<int>>();
                for (int i = 0; i < vrp_solution.Length; i++)
                    solution.Add(i, new List<int>(vrp_solution[i]));
                
                
                this.RaiseIntermidiateResult(result, solution);
            }
        }

        #endregion

        /// <summary>
        /// Implements the actual logic.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="customers"></param>
        /// <param name="second"></param>
        /// <param name="second_2"></param>
        /// <returns></returns>
        protected abstract int[][] DoCalculation(IProblemWeights problem,
            ICollection<int> depots, ICollection<int> customers);
    }
}

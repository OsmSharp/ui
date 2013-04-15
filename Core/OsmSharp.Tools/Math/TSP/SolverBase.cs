using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.TSP.Problems;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;

namespace OsmSharp.Tools.Math.TSP
{
    /// <summary>
    /// Baseclass for all TSP solver.
    /// </summary>
    public abstract class SolverBase : ISolver
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Executes the solver.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        public IRoute Solve(IProblem problem)
        {
            IProblem converted_problem = problem;

            bool first = problem.First.HasValue;
            bool last = problem.Last.HasValue;

            // convert the problem if needed.
            if (!first && !last)
            { // add a virtual customer with distance zero to all existing customers.
                double[][] new_weights = new double[problem.WeightMatrix.Length + 1][];
                new_weights[0] = new double[problem.WeightMatrix.Length + 1];
                for (int idx = 0; idx < problem.WeightMatrix.Length + 1; idx++)
                {
                    new_weights[0][idx] = 0;
                }
                for(int idx = 0; idx < problem.WeightMatrix.Length; idx++)
                {
                    new_weights[idx+1] = new double[problem.WeightMatrix.Length + 1];
                    new_weights[idx+1][0] = 0;
                    problem.WeightMatrix[idx].CopyTo(new_weights[idx+1], 1);
                }
                converted_problem = MatrixProblem.CreateATSP(new_weights, 0);
            }
            else if (!last)
            { // set all weights to the first customer to zero.
                for (int idx = 0; idx < problem.WeightMatrix.Length; idx++)
                {
                    problem.WeightMatrix[idx][problem.First.Value] = 0;
                }
                converted_problem = MatrixProblem.CreateATSP(problem.WeightMatrix, problem.First.Value);
            }

            // execute the solver on the converted problem.
            IRoute converted_route = this.DoSolve(converted_problem);

            // convert the route back.
            if (!first && !last)
            { // when a virtual customer was added the route needs converting.
                List<int> customers_list = converted_route.ToList<int>();
                customers_list.RemoveAt(0);
                for (int idx = 0; idx < customers_list.Count; idx++)
                {
                    customers_list[idx] = customers_list[idx] - 1;
                }
                converted_route = DynamicAsymmetricRoute.CreateFrom(customers_list, false);
            }
            else if (!last)
            { // the returned route will return to customer zero; convert the route.
                converted_route = DynamicAsymmetricRoute.CreateFrom(converted_route, false);
            }
            return converted_route;
        }

        /// <summary>
        /// Executes the actual solver code.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        protected abstract IRoute DoSolve(IProblem problem);

        /// <summary>
        /// Stops the solver.
        /// </summary>
        public virtual void Stop()
        {

        }

        #region Intermidiate Results

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        public event SolverDelegates.IntermidiateDelegate IntermidiateResult;

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
        protected void RaiseIntermidiateResult(int[] result)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result);
            }
        }

        #endregion
    }
}

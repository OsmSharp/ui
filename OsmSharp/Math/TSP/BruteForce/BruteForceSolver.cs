using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Collections;
using OsmSharp.Math.VRP.Core.Routes;
using OsmSharp.Math.TSP.Problems;
using OsmSharp.Math.VRP.Core.Routes.ASymmetric;

namespace OsmSharp.Math.TSP.BruteForce
{
    /// <summary>
    /// Implements a brute force solver by checking all possible combinations.
    /// </summary>
    public class BruteForceSolver : SolverBase
    {
        /// <summary>
        /// Returns a new for this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "BF";
            }
        }

        /// <summary>
        /// Solves the TSP.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        protected override IRoute DoSolve(IProblem problem)
        {
            // initialize.
            List<int> solution = new List<int>();
            for (int idx = 0; idx < problem.Size; idx++)
            { // add each customer again.
                solution.Add(idx);
            }

            // keep on looping until all the permutations 
            // have been considered.
            PermutationEnumerable<int> enumerator = new PermutationEnumerable<int>(
                solution.ToArray());
            int[] best = null;
            double bestWeight = double.MaxValue;
            foreach (int[] permutation in enumerator)
            {
                double weight = RouteExtensions.CalculateWeight(permutation, problem.First == problem.Last, problem);
                if (weight < bestWeight)
                { // the best weight has improved.
                    bestWeight = weight;
                    best = permutation;
                }
            }
            return new SimpleAsymmetricRoute(best.ToList<int>(), true);
        }
    }
}

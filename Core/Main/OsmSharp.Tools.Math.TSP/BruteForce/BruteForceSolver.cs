//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Tools.Math.VRP.Core.Routes;
//using OsmSharp.Tools.Math.TSP.Problems;

//namespace OsmSharp.Tools.Math.TSP.BruteForce
//{
//    /// <summary>
//    /// Implements a brute force solver by checking all possible combinations.
//    /// </summary>
//    public class BruteForceSolver : ISolver
//    {
//        /// <summary>
//        /// Returns a new for this solver.
//        /// </summary>
//        public string Name
//        {
//            get 
//            { 
//                return "BF"; 
//            }
//        }

//        /// <summary>
//        /// Solves the TSP.
//        /// </summary>
//        /// <param name="problem"></param>
//        /// <returns></returns>
//        public IRoute Solve(IProblem problem)
//        {
//            // initialize.
//            List<int> solution = new List<int>();
//            for (int idx = 0; idx < problem.Size; idx++)
//            { // add each customer again.
//                solution.Add(idx);
//            }

//            // keep on looping until all the permutations 
//            // have been considered.

//        }

//        /// <summary>
//        /// Move to the next permutation.
//        /// </summary>
//        /// <param name="solution"></param>
//        /// <returns></returns>
//        public bool MoveNext(List<int> solution)
//        {

//        }

//        /// <summary>
//        /// Stops the solver.
//        /// </summary>
//        public void Stop()
//        {

//        }

//        /// <summary>
//        /// Reports intermidiate results.
//        /// </summary>
//        public event SolverDelegates.IntermidiateDelegate IntermidiateResult;
//    }
//}

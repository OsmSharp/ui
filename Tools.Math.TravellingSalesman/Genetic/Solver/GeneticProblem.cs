using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver
{
    /// <summary>
    /// Class acting as a wrapper for the problem implementing the IProblem interface.
    /// </summary>
    public class GeneticProblem :  IProblem
    {
        /// <summary>
        /// The non-genetic problem.
        /// </summary>
        private Tools.Math.TSP.Problems.IProblem _problem;

        /// <summary>
        /// Creates a new genetic problem.
        /// </summary>
        /// <param name="problem"></param>
        public GeneticProblem(Tools.Math.TSP.Problems.IProblem problem)
        {
            _problem = problem;
        }

        public Tools.Math.TSP.Problems.IProblem BaseProblem
        {
            get
            {
                return _problem;
            }
        }

        /// <summary>
        /// Returns the weight between city1 and city2.
        /// </summary>
        /// <param name="city1"></param>
        /// <param name="city1"></param>
        /// <returns></returns>
        public float Weight(int city1, int city2)
        {
            return _problem.Weight(city1, city2);
        }

        /// <summary>
        /// Returns the first customer in a round.
        /// </summary>
        public int First
        {
            get
            {
                return _problem.First.Value;
            }
        }

        /// <summary>
        /// Returns the last customer in a round.
        /// </summary>
        public int Last
        {
            get
            {
                return _problem.Last.Value;
            }
        }

        private List<int> _along;
        public List<int> Along 
        {
            get
            {
                if (_along == null)
                {
                    _along = new List<int>();
                    for (int cust = 0; cust < _problem.Size; cust++)
                    {
                        if (cust != this.First && cust != this.Last)
                        {
                            _along.Add(cust);
                        }
                    }
                }
                return _along;
            }
        }
    }
}

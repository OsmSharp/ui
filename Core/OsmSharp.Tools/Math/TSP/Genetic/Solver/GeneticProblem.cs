// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.AI.Genetic;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver
{
    /// <summary>
    /// Class acting as a wrapper for the problem implementing the IProblem interface.
    /// </summary>
    public class GeneticProblem :  IProblem
    {
        /// <summary>
        /// The non-genetic problem.
        /// </summary>
        private OsmSharp.Tools.Math.TSP.Problems.IProblem _problem;

        /// <summary>
        /// Creates a new genetic problem.
        /// </summary>
        /// <param name="problem"></param>
        public GeneticProblem(OsmSharp.Tools.Math.TSP.Problems.IProblem problem)
        {
            _problem = problem;
        }

        /// <summary>
        /// Returns the base problem.
        /// </summary>
        public OsmSharp.Tools.Math.TSP.Problems.IProblem BaseProblem
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
        /// <param name="city2"></param>
        /// <returns></returns>
        public double Weight(int city1, int city2)
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

        /// <summary>
        /// Returns the 'along' customers.
        /// </summary>
        public List<int> Along 
        {
            get
            {
                if (_along == null)
                {
                    _along = new List<int>();
                    for (int cust = 0; cust < _problem.Size; cust++)
                    {
                        if ((!_problem.First.HasValue || cust != this.First) &&
                            (!_problem.Last.HasValue || cust != this.Last))
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

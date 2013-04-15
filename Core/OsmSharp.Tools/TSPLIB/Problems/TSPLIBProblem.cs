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
using OsmSharp.Tools.Math.TSP;
using System.IO;
using System.Text.RegularExpressions;
using OsmSharp.Tools.Math.TSP.Problems;
using OsmSharp.Tools.Math.VRP.Core;

namespace OsmSharp.Tools.TSPLIB.Problems
{
    /// <summary>
    /// Represents and parses a TSPLIB problem.
    /// </summary>
    public class TSPLIBProblem : IProblem
    {
        private int _size;

        private double[][] _weights;

        private int? _first;

        private int? _last;

        /// <summary>
        /// Creates a new TSP LIB problem.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="comment"></param>
        /// <param name="size"></param>
        /// <param name="weights"></param>
        /// <param name="weight_type"></param>
        /// <param name="problem_type"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        public TSPLIBProblem(string name, string comment, int size, double[][] weights,
            TSPLIBProblemWeightTypeEnum weight_type,
            TSPLIBProblemTypeEnum problem_type, int? first, int? last)
        {
            this.Name = name;
            this.Type = problem_type;
            this.WeightType = weight_type;
            _weights = weights;
            _size = size;

            _first = first;
            _last = last;
        }

        /// <summary>
        /// Returns type of TSP.
        /// </summary>
        public TSPLIBProblemTypeEnum Type { get; private set; }

        /// <summary>
        /// Returns weight.
        /// </summary>
        public TSPLIBProblemWeightTypeEnum WeightType { get; private set; }

        /// <summary>
        /// Returns weight matrix.
        /// </summary>
        public double[][] WeightMatrix
        {
            get 
            {
                return _weights; 
            }
        }

        /// <summary>
        /// Returns the size.
        /// </summary>
        public int Size
        {
            get 
            {
                return _size;
            }
        }

        /// <summary>
        /// Returns the weight between two customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public double Weight(int from, int to)
        {
            return _weights[from][to];
        }

        /// <summary>
        /// Returns first customer.
        /// </summary>
        public int? First
        {
            get 
            { 
                return _first; 
            }
        }

        /// <summary>
        /// Returns last customer.
        /// </summary>
        public int? Last
        {
            get 
            { 
                return _last; 
            }
        }

        /// <summary>
        /// Returns symmetric flag.
        /// </summary>
        public bool Symmetric
        {
            get 
            {
                return this.Type == TSPLIBProblemTypeEnum.TSP; 
            }
        }

        /// <summary>
        /// Returns euclidean.
        /// </summary>
        public bool Euclidean
        {
            get 
            { 
                return true; 
            }
        }

        /// <summary>
        /// Returns the best value.
        /// </summary>
        public double Best { get; set; }

        /// <summary>
        /// Returns the name of the problem.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns the comment of the problems.
        /// </summary>
        public string Comment { get; private set; }


        #region Nearest Neighbour

        /// <summary>
        /// Keeps the nearest neighbour list.
        /// </summary>
        private NearestNeighbours10[] _neighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public NearestNeighbours10 Get10NearestNeighbours(int v)
        {
            if (_neighbours == null)
            {
                _neighbours = new NearestNeighbours10[this.Size];
            }
            NearestNeighbours10 result = _neighbours[v];
            if (result == null)
            {
                SortedDictionary<double, List<int>> neighbours = new SortedDictionary<double, List<int>>();
                for (int customer = 0; customer < this.Size; customer++)
                {
                    if (customer != v)
                    {
                        double weight = this.WeightMatrix[v][customer];
                        List<int> customers = null;
                        if (!neighbours.TryGetValue(weight, out customers))
                        {
                            customers = new List<int>();
                            neighbours.Add(weight, customers);
                        }
                        customers.Add(customer);
                    }
                }

                result = new NearestNeighbours10();
                foreach (KeyValuePair<double, List<int>> pair in neighbours)
                {
                    foreach (int customer in pair.Value)
                    {
                        if (result.Count < 10)
                        {
                            if (result.Max < pair.Key)
                            {
                                result.Max = pair.Key;
                            }
                            result.Add(customer);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                _neighbours[v] = result;
            }
            return result;
        }

        #endregion
    }
}

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
using Tools.Math.VRP.Core;
using Tools.Math.Units.Time;

namespace Routing.Core.VRP.NoDepot.MaxTime
{
    /// <summary>
    /// Represents a max time problem.
    /// </summary>
    public class MaxTimeProblem : Tools.Math.AI.Genetic.IProblem, IProblemWeights
    {
        private IProblemWeights _weights;

        public MaxTimeProblem(Second max, Second delivery_time, IProblemWeights weights)
        {
            this.Max = max;
            this.DeliveryTime = delivery_time;

            _weights = weights;
        }


        //public MaxTimeProblem(Second max, Second delivery_time, IProblemWeights weights, float[] placement_solutions)
        //{
        //    this.Max = max;

        //    _weights = weights;
        //    _placement_solutions = placement_solutions;
        //}

        //public float[] PlacementSolutions
        //{
        //    get
        //    {
        //        return _placement_solutions;
        //    }
        //}

        /// <summary>
        /// Returns a list of customers.
        /// </summary>
        public List<int> Customers
        {
            get
            {
                // create the problem for the genetic algorithm.
                List<int> customers = new List<int>();
                for (int customer = 0; customer < this.Size; customer++)
                {
                    customers.Add(customer);
                }
                return customers;
            }
        }

        public Second Max { get; private set; }

        public Second DeliveryTime { get; private set; }

        public int Size
        {
            get
            {
                return _weights.Size;
            }
        }

        public IProblemWeights Weights
        {
            get
            {
                return _weights;
            }
        }

        public double Weight(int from, int to)
        {
            return _weights.Weight(from, to);
        }

        public bool Symmetric
        {
            get
            {
                return false;
            }
        }

        public bool Euclidean
        {
            get
            {
                return false;
            }
        }

        public double[][] WeightMatrix
        {
            get
            {
                return _weights.WeightMatrix;
            }
        }

        #region Nearest Neighbour

        /// <summary>
        /// Keeps the nearest neighbour list.
        /// </summary>
        private NearestNeighbours10[] _neighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <param name="customer"></param>
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

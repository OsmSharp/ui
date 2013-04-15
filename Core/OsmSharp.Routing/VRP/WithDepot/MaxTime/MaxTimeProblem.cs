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
using OsmSharp.Tools.Math.VRP.Core;
using OsmSharp.Tools.Math.Units.Time;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Routing.VRP.WithDepot.MaxTime
{
    /// <summary>
    /// Represents a max time problem.
    /// </summary>
    public class MaxTimeProblem : OsmSharp.Tools.Math.AI.Genetic.IProblem, IProblemWeights
    {
        private IProblemWeights _weights;

        private MaxTimeCalculator _calculator;

        private double _cost_per_second;

        private double _cost_per_vehicle;

        /// <summary>
        /// Creates a new max time problem.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        /// <param name="cost_per_second"></param>
        /// <param name="cost_per_vehicle"></param>
        public MaxTimeProblem(IProblemWeights weights, Second max, Second delivery_time,
            double cost_per_second, double cost_per_vehicle)
        {
            this.Max = max;
            this.DeliveryTime = delivery_time;
            _cost_per_second = cost_per_second;
            _cost_per_vehicle = cost_per_vehicle;

            _weights = weights;

            _calculator = new MaxTimeCalculator(this);
            _customer_positions = new List<GeoCoordinate>();
        }

        /// <summary>
        /// Holds all the customer positions.
        /// </summary>
        private List<GeoCoordinate> _customer_positions;

        /// <summary>
        /// Returns a list of customers.
        /// </summary>
        public List<GeoCoordinate> CustomerPositions
        {
            get
            {
                return _customer_positions;
            }
        }

        /// <summary>
        /// Returns the max time calculator.
        /// </summary>
        public MaxTimeCalculator MaxTimeCalculator
        {
            get
            {
                return _calculator;
            }
        }

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

        /// <summary>
        /// The max time of one route.
        /// </summary>
        public Second Max { get; private set; }

        /// <summary>
        /// The delivery time of one route.
        /// </summary>
        public Second DeliveryTime { get; private set; }

        /// <summary>
        /// Returns the size.
        /// </summary>
        public int Size
        {
            get
            {
                return _weights.Size;
            }
        }

        /// <summary>
        /// Returns the weights.
        /// </summary>
        public IProblemWeights Weights
        {
            get
            {
                return _weights;
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
            return _weights.Weight(from, to);
        }

        /// <summary>
        /// Returns true if the problem is symmetric.
        /// </summary>
        public bool Symmetric
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the problem is euclidean.
        /// </summary>
        public bool Euclidean
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the weight matrix.
        /// </summary>
        public double[][] WeightMatrix
        {
            get
            {
                return _weights.WeightMatrix;
            }
        }

        #region Penalizations

        /// <summary>
        /// Holds al penalizations.
        /// </summary>
        private Dictionary<Edge, double> _penalizations;

        /// <summary>
        /// Penalizes an edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="delta"></param>
        public void Penalize(Edge edge, double delta)
        {
            if (_penalizations == null)
            {
                _penalizations = new Dictionary<Edge, double>();
            }
            double total_delta;
            if (!_penalizations.TryGetValue(edge, out total_delta))
            {
                _penalizations[edge] = delta;
            }
            else
            {
                _penalizations[edge] = delta + total_delta;
            }

            this.WeightMatrix[edge.From][edge.To] = this.WeightMatrix[edge.From][edge.To] + delta;
        }

        /// <summary>
        /// Undoes all penalizations.
        /// </summary>
        public void ResetPenalizations()
        {
            if (_penalizations != null)
            {
                foreach (KeyValuePair<Edge, double> pair in _penalizations)
                {
                    this.WeightMatrix[pair.Key.From][pair.Key.To] =
                        this.WeightMatrix[pair.Key.From][pair.Key.To] - pair.Value;
                }
                _penalizations = null;
            }
        }

        #endregion

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

        #region Calculations

        /// <summary>
        /// Calculates the total time.
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public double Time(MaxTimeSolution solution)
        {
            double time = 0;
            for (int idx = 0; idx < solution.Count; idx++)
            {
                time = time + this.Time(solution.Route(idx));
            }
            return time;
        }

        /// <summary>
        /// Calculates the time of one route.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public double Time(IRoute route)
        {
            double time = 0;
            //Edge? first = null;
            //Edge? last = null;
            foreach (Edge edge in route.Edges())
            {
                //if (!first.HasValue)
                //{
                //    first = edge;
                //    time = time + this.WeightMatrix[0][edge.From];
                //}
                time = time + this.WeightMatrix[edge.From][edge.To];
                //last = edge;
            }
            //if (last.HasValue)
            //{
            //    time = time + this.WeightMatrix[last.Value.To][0];
            //}
            return this.Time(time, route.Count);
        }

        /// <summary>
        /// Calculates the time of one route given the travel time and the amount of customers.
        /// </summary>
        /// <param name="travel_time"></param>
        /// <param name="customers"></param>
        /// <returns></returns>
        public double Time(double travel_time, int customers)
        {
            return travel_time + this.DeliveryTime.Value * customers;
        }

        /// <summary>
        /// Calculates the cumulative times of a route indexed by customer.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public double[] TimeCumul(IRoute route)
        {
            // intialize the result array.
            double[] cumul = new double[route.Count + 1];

            int previous = -1; // the previous customer.
            double time = 0; // the current weight.
            int idx = 0; // the current index.
            foreach (int customer1 in route)
            { // loop over all customers.
                if (previous >= 0)
                { // there is a previous customer.
                    // add one customer and the distance to the previous customer.
                    time = time +
                        this.WeightMatrix[previous][customer1];
                    cumul[idx] = time;
                }
                else
                { // there is no previous customer, this is the first one.
                    cumul[idx] = 0;
                }

                idx++; // increase the index.
                previous = customer1; // prepare for next loop.
            }
            // handle the edge last->first.
            time = time +
                this.WeightMatrix[previous][route.First];
            cumul[idx] = time;
            return cumul;
        }

        /// <summary>
        /// Calculates the total weight.
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public double Weight(MaxTimeSolution solution)
        {
            double time = 0;
            double time_above_max = 0;
            for (int idx = 0; idx < solution.Count; idx++)
            {
                // calculate one route.
                double route_time = this.Time(solution.Route(idx));

                // add the total time about max.
                if (route_time > this.Max.Value)
                { // route time too big!
                    time_above_max = time_above_max + (route_time - this.Max.Value);
                }

                // add to the total time.
                time = time + route_time;
            }

            // the route count.
            double route_count = solution.Count;

            // the punished for breaking max.
            double punishment = System.Math.Pow(time_above_max, 4) * route_count;

            return route_count * time * _cost_per_second + route_count * _cost_per_vehicle + punishment;
        }

        #region Difference

        /// <summary>
        /// Calculates the weight difference after merging two routes given the cost to merge them.
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="merge_costs"></param>
        /// <returns></returns>
        public double WeightDifferenceAfterMerge(MaxTimeSolution solution, double merge_costs)
        {
            if (solution.Count < 2)
            { // the solution routes cannot be merged.
                return double.MaxValue;
            }
            // the route count.
            double route_count = solution.Count - 1;

            return route_count * merge_costs * _cost_per_second + route_count * _cost_per_vehicle;
        }

        #endregion

        #endregion
    }
}

// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.TSPPlacement
{
    /// <summary>
    /// Represents the equivalent TSP problem related to this VRP.
    /// </summary>
    public class TSPProblem : Tools.Math.TSP.Problems.IProblem
    {
        public TSPProblem(MaxTimeProblem vrp)
        {
            this.Size = vrp.Size;
            this.Symmetric = false;
            this.Euclidean = false;
            this.First = 0;
            this.Last = 0;
            this.WeightMatrix = vrp.WeightMatrix;
        }

        public int Size
        {
            get;
            private set;
        }

        public int? First
        {
            get;
            private set;
        }

        public int? Last
        {
            get;
            private set;
        }

        public bool Symmetric
        {
            get;
            private set;
        }

        public bool Euclidean
        {
            get;
            private set;
        }

        public float[][] WeightMatrix
        {
            get;
            private set;
        }

        public float Weight(int from, int to)
        {
            return this.WeightMatrix[from][to];
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
                SortedDictionary<float, List<int>> neighbours = new SortedDictionary<float, List<int>>();
                for (int customer = 0; customer < this.Size; customer++)
                {
                    if (customer != v)
                    {
                        float weight = this.WeightMatrix[v][customer];
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
                foreach (KeyValuePair<float, List<int>> pair in neighbours)
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

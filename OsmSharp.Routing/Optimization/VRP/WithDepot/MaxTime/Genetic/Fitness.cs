// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

namespace OsmSharp.Routing.Optimization.VRP.WithDepot.MinimaxTime.Genetic
{
    /// <summary>
    /// A fitness representation.
    /// </summary>
    public class Fitness : IComparable
    {
        /// <summary>
        /// The actual fitness value.
        /// </summary>
        public double ActualFitness { get; set; }

        /// <summary>
        /// Compares this fitness to another.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is Fitness)
            {
                return this.ActualFitness.CompareTo((obj as Fitness).ActualFitness);
            }
            return -1;
        }

        /// <summary>
        /// The maximum weight.
        /// </summary>
        public double MaxWeight { get; set; }

        /// <summary>
        /// The weights of each route.
        /// </summary>
        public List<double> Weights { get; set; }

        /// <summary>
        /// The vehicles.
        /// </summary>
        public int Vehicles { get; set; }

        /// <summary>
        /// The total time.
        /// </summary>
        public double TotalTime { get; set; }

        /// <summary>
        /// Returns a description of this fitness.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (this.Weights != null)
            {
                foreach (double weight in Weights)
                {
                    builder.Append(String.Format("{0:0.00}", weight));
                    builder.Append(" ");
                }
            }
            return string.Format("{0}: {1}s with {2} vehicles and {3} max range: {4}",
                this.ActualFitness, this.TotalTime, this.Vehicles, this.Range, builder.ToString());
        }

        /// <summary>
        /// Returns the range.
        /// </summary>
        public double Range { get; set; }
    }
}

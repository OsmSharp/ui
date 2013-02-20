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

namespace OsmSharp.Routing.Core.VRP.NoDepot.MaxTime.Genetic
{
    public class Fitness : IComparable
    {
        public double ActualFitness { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is Fitness)
            {
                return this.ActualFitness.CompareTo((obj as Fitness).ActualFitness);
            }
            return -1;
        }

        public double MaxWeight { get; set; }

        public List<double> Weights { get; set; }

        public int Vehicles { get; set; }

        public double TotalTime { get; set; }

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
            return string.Format("{0}: {1}s with {2} vehicles and {3} above max: {4}",
                this.ActualFitness, this.TotalTime, this.Vehicles, this.TotalAboveMax, builder.ToString());
        }

        public double TotalAboveMax { get; set; }
    }
}

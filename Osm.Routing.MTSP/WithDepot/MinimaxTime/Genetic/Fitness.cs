using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Core.VRP.WithDepot.MinimaxTime.Genetic
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
            return string.Format("{0}: {1}s with {2} vehicles and {3} max range: {4}",
                this.ActualFitness, this.TotalTime, this.Vehicles, this.Range, builder.ToString());
        }

        public double Range { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.MultiSalesman.Genetic
{
    public class Genome : List<int>, IEquatable<Genome>
    {
        public Genome()
        {

        }

        public Genome(IEnumerable<int> collection)
            : base(collection)
        {

        }

        public bool Equals(Genome other)
        {
            if (this.Count != other.Count)
            {
                return false;
            }
            else
            {
                for (int idx = 0; idx < this.Count; idx++)
                {
                    if (this[idx] != other[idx])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Core.Sparse.Primitives
{
    /// <summary>
    /// Represents a simple vertex that is part of the road network.
    /// </summary>
    public class SparseSimpleVertex
    {
        public long Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public long Neighbour1 { get; set; }

        public long Neighbour2 { get; set; }


        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Id.Equals((obj as SparseSimpleVertex).Id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Core.Sparse.Primitives
{
    /// <summary>
    /// Contains basic vertex information.
    /// </summary>
    public class SimpleVertex
    {
        public long Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

    }
}

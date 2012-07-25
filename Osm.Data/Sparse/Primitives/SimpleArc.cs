using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Core.Sparse.Primitives
{
    /// <summary>
    /// Contains basic vertex information.
    /// </summary>
    public class SimpleArc
    {
        public long Id { get; set; }

        public long[] Nodes { get; set; }
    }
}

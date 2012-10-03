using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Core.DynamicGraph
{
    /// <summary>
    /// Abstracts edge information.
    /// </summary>
    public interface IDynamicGraphEdgeData
    {
        /// <summary>
        /// Returns true if the edge can be followed only in the foward direction.
        /// </summary>
        bool Forward 
        { 
            get;
        }

        /// <summary>
        /// Returns true if the edge can be followed only in the backward direction.
        /// </summary>
        bool Backward
        {
            get;
        }
    }
}

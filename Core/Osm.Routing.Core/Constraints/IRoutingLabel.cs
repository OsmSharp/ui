using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Core.Constraints
{
    /// <summary>
    /// Represents a routing label used to implement routing constraints.
    /// </summary>
    public interface IRoutingLabel : IEquatable<IRoutingLabel>
    {
        /// <summary>
        /// The actual label.
        /// </summary>
        string Label
        {
            get;
        }
    }
}

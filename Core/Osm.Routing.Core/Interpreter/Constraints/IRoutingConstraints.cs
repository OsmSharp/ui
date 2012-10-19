using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;

namespace Osm.Routing.Core.Constraints
{
    /// <summary>
    /// Represents some routing constraints.
    /// </summary>
    /// <remarks>Objects of this type can be used to constrain routes that will be found to certain criteria.</remarks>
    public interface IRoutingConstraints
    {
        /// <summary>
        /// Translates some tags into some routing label.
        /// </summary>
        /// <param name="tagged_object"></param>
        /// <returns></returns>
        RoutingLabel GetLabelFor(ITaggedObject tagged_object);

        /// <summary>
        /// Returns true if a given (forward) sequence of labels is allowed under these constraints.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        bool ForwardSequenceAllowed(IList<RoutingLabel> sequence, RoutingLabel latest);

        /// <summary>
        /// Returns true if a given (backward) sequence of labels is allowed under these constraints.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        bool BackwardSequenceAllowed(IList<RoutingLabel> sequence, RoutingLabel latest);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using System.Text.RegularExpressions;

namespace Osm.Routing.Core.Constraints.Highways
{
    /// <summary>
    /// Handles default highway constraints.
    /// </summary>
    public class DefaultHighwayConstraints : IRoutingConstraints
    {
        /// <summary>
        /// Returns a label for different categories of highways.
        /// </summary>
        /// <param name="tagged_object"></param>
        /// <returns></returns>
        public RoutingLabel GetLabelFor(ITaggedObject tagged_object)
        {
            Roads.Tags.RoadTagsInterpreterBase tags_interpreter = new Roads.Tags.RoadTagsInterpreterBase(tagged_object.Tags);
            if (tags_interpreter.IsOnlyLocalAccessible())
            {
                return new RoutingLabel('L', "OnlyLocalAccessible"); // local
            }
            return new RoutingLabel('R', "GeneralAccessible"); // regular.
        }

        /// <summary>
        /// Returns true if the given sequence is allowed.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="latest"></param>
        /// <returns></returns>
        public bool ForwardSequenceAllowed(IList<RoutingLabel> sequence, RoutingLabel latest)
        {
            return Regex.IsMatch(sequence.CreateString(latest), "^L*R*L*$");
        }

        /// <summary>
        /// Returns true if the given sequence is allowed.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="latest"></param>
        /// <returns></returns>
        public bool BackwardSequenceAllowed(IList<RoutingLabel> sequence, RoutingLabel latest)
        {
            return Regex.IsMatch(sequence.CreateString(latest), "^L*R*L*$");
        }
    }
}
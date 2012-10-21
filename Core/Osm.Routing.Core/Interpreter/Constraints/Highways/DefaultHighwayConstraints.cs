// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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
        /// Holds the local label.
        /// </summary>
        private RoutingLabel _local_label = 
            new RoutingLabel('L', "OnlyLocalAccessible");

        /// <summary>
        /// Holds the general label.
        /// </summary>
        private RoutingLabel _general_label = 
            new RoutingLabel('R', "GeneralAccessible");

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
                return _local_label; // local
            }
            return _general_label; // regular.
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
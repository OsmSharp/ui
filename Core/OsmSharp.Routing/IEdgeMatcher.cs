// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
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
using OsmSharp.Routing.Interpreter;
using OsmSharp.Tools;

namespace OsmSharp.Routing
{
    /// <summary>
    /// Interface used to match a coordinate to a configurable routable position.
    /// </summary>
    public interface IEdgeMatcher
    {
        /// <summary>
        /// Returns true if the edge is a suitable candidate as a target for a point to be resolved on.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="point_tags"></param>
        /// <param name="edge_tags"></param>
        /// <returns></returns>
        bool MatchWithEdge(VehicleEnum vehicle, IDictionary<string, string> point_tags, IDictionary<string, string> edge_tags);
    }

    /// <summary>
    /// A default implementation of the edge matcher.
    /// </summary>
    public class DefaultEdgeMatcher : IEdgeMatcher
    {
        /// <summary>
        /// Returns true if the edge is a suitable candidate as a target for a point to be resolved on.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="point_tags"></param>
        /// <param name="edge_tags"></param>
        /// <returns></returns>
        public bool MatchWithEdge(VehicleEnum vehicle, 
            IDictionary<string, string> point_tags, IDictionary<string, string> edge_tags)
        {
            if (point_tags == null || point_tags.Count == 0)
            { // when the point has no tags it has no requirements.
                return true;
            }

            if (edge_tags == null || edge_tags.Count == 0)
            { // when the edge has no tags, no way to verify.
                return false;
            }

            string point_name, edge_name;
            if (point_tags.TryGetValue("name", out point_name) &&
                edge_tags.TryGetValue("name", out edge_name))
            { // both have names.
                return (point_name == edge_name);
            }
            return false;
        }
    }

    /// <summary>
    /// A Levenshtein matching implementation of the edge matcher.
    /// </summary>
    public class LevenshteinEdgeMatcher : IEdgeMatcher
    {
        /// <summary>
        /// Returns true if the edge is a suitable candidate as a target for a point to be resolved on.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="point_tags"></param>
        /// <param name="edge_tags"></param>
        /// <returns></returns>
        public bool MatchWithEdge(VehicleEnum vehicle,
            IDictionary<string, string> point_tags, IDictionary<string, string> edge_tags)
        {
            if (point_tags == null || point_tags.Count == 0)
            { // when the point has no tags it has no requirements.
                return true;
            }

            if (edge_tags == null || edge_tags.Count == 0)
            { // when the edge has no tags, no way to verify.
                return false;
            }

            string point_name, edge_name;
            if (point_tags.TryGetValue("name", out point_name) &&
                edge_tags.TryGetValue("name", out edge_name))
            { // both have names.
                return (point_name.LevenshteinMatch(edge_name, 90));
            }
            return false;
        }
    }
}
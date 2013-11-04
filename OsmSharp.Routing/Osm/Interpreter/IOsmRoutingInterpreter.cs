// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using System.Collections.Generic;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Routing.Interpreter;

namespace OsmSharp.Routing.Osm.Interpreter
{
    /// <summary>
    /// An abstract representation of an osm routing interpreter.
    /// </summary>
    public interface IOsmRoutingInterpreter : IRoutingInterpreter
    {
        /// <summary>
        /// Returns true if the given object possibly presents a restriction for any vehicle.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        bool IsRestriction(OsmGeoType type, TagsCollectionBase tags);

        /// <summary>
        /// Calculates all restrictions for the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        List<Vehicle> CalculateRestrictions(Node node);

        /// <summary>
        /// Calculates all restrictions for a given relation.
        /// </summary>
        /// <param name="completeRelation"></param>
        /// <returns></returns>
        List<KeyValuePair<Vehicle, long[]>> CalculateRestrictions(CompleteRelation completeRelation);
    }
}

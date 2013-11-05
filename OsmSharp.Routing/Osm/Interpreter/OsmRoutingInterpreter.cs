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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Constraints;
using OsmSharp.Routing.Interpreter.Roads;
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;
using OsmSharp.Units.Speed;

namespace OsmSharp.Routing.Osm.Interpreter
{
    /// <summary>
    /// A routing interpreter for OSM data.
    /// </summary>
    public class OsmRoutingInterpreter : IOsmRoutingInterpreter
    {
        /// <summary>
        /// Holds the edge interpreter.
        /// </summary>
        private readonly IEdgeInterpreter _edgeInterpreter;

        /// <summary>
        /// Holds the routing constraints.
        /// </summary>
        private readonly IRoutingConstraints _constraints;

        /// <summary>
        /// Holds the relevant keys.
        /// </summary>
        private HashSet<string> _relevantKeys; 

        /// <summary>
        /// Creates a new routing intepreter with default settings.
        /// </summary>
        public OsmRoutingInterpreter()
        {
            _edgeInterpreter = new Edge.EdgeInterpreter();
            _constraints = null;

            this.FillRelevantTags();
        }

        /// <summary>
        /// Creates a new routing interpreter with given constraints.
        /// </summary>
        /// <param name="constraints"></param>
        public OsmRoutingInterpreter(IRoutingConstraints constraints)
        {
            _edgeInterpreter = new Edge.EdgeInterpreter();
            _constraints = constraints;
            
            this.FillRelevantTags();
        } 
	        
        /// <summary>
        /// Creates a new routing interpreter a custom edge interpreter.
        /// </summary>
        /// <param name="interpreter"></param>
        public OsmRoutingInterpreter(IEdgeInterpreter interpreter)
        {
            _edgeInterpreter = interpreter;
            _constraints = null;
        }	  

        /// <summary>
        /// Builds the list of relevant tags.
        /// </summary>
        private void FillRelevantTags()
        {
            _relevantKeys = new HashSet<string> { "oneway", "highway", "name", "motor_vehicle", "bicycle", "foot", "access", "maxspeed", "junction" };
        }

        /// <summary>
        /// Returns true if the given tags is relevant.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsRelevant(string key)
        {
            return _relevantKeys.Contains(key);
        }

        /// <summary>
        /// Returns true if the given key value pair is relevant.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsRelevant(string key, string value)
        {
            KilometerPerHour speed;
            if (this.IsRelevant(key))
            { // check the value.
                switch(value)
                {
                    case "oneway":
                        return value == "yes" || value == "reverse";
                    case "maxspeed":
                        return TagExtensions.TryParseSpeed(value, out speed);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given vertices can be traversed in the given order.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="along"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanBeTraversed(long from, long along, long to)
        {
            return true;
        }

        /// <summary>
        /// Returns and edge interpreter.
        /// </summary>
        public IEdgeInterpreter EdgeInterpreter
        {
            get 
            {
                return _edgeInterpreter; 
            }
        }

        /// <summary>
        /// Returns the constraints.
        /// </summary>
        public IRoutingConstraints Constraints
        {
            get
            {
                return _constraints;
            }
        }

        /// <summary>
        /// Returns true if the given object can be a routing restriction.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsRestriction(OsmGeoType type, TagsCollectionBase tags)
        {
            return true;
        }

        /// <summary>
        /// Returns all restrictions that are represented by the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<Vehicle> CalculateRestrictions(Node node)
        {
            return new List<Vehicle>();
        }

        /// <summary>
        /// Returns all restrictions that are represented by the given node.
        /// </summary>
        /// <param name="completeRelation"></param>
        /// <returns></returns>
        public List<KeyValuePair<Vehicle, long[]>> CalculateRestrictions(CompleteRelation completeRelation)
        {
            return new List<KeyValuePair<Vehicle, long[]>>();
        }
    }
}
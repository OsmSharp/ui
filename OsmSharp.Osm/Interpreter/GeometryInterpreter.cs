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
using OsmSharp.Geo.Geometries;
using OsmSharp.Osm;
using OsmSharp.Osm.Data;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Osm.Interpreter
{
    /// <summary>
    /// Represents a geometry interpreter to convert OSM-objects to corresponding geometries.
    /// </summary>
    public abstract class GeometryInterpreter
    {
        /// <summary>
        /// Holds the default geometry interpreter.
        /// </summary>
        private static GeometryInterpreter _defaultInterpreter;

        /// <summary>
        /// Gets/sets the default interpreter.
        /// </summary>
        public static GeometryInterpreter DefaultInterpreter
        {
            get
            {
                if (_defaultInterpreter == null)
                { 
                    _defaultInterpreter = new SimpleGeometryInterpreter();
                }
                return _defaultInterpreter;
            }
            set
            {
                _defaultInterpreter = value;
            }
        }

        /// <summary>
        /// Interprets an OSM-object and returns the corresponding geometry.
        /// </summary>
        /// <param name="osmObject"></param>
        /// <returns></returns>
        public abstract GeometryCollection Interpret(CompleteOsmGeo osmObject);

        /// <summary>
        /// Returns true if the given tags collection contains potential area tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public abstract bool IsPotentiallyArea(TagsCollection tags);

        /// <summary>
        /// Interprets an OSM-object and returns the correctponding geometry.
        /// </summary>
        /// <param name="simpleOsmGeo"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual GeometryCollection Interpret(OsmGeo simpleOsmGeo, IDataSourceReadOnly data)
        {
            switch (simpleOsmGeo.Type)
            {
                case OsmGeoType.Node:
                    return this.Interpret(CompleteNode.CreateFrom(simpleOsmGeo as Node));
                case OsmGeoType.Way:
                    return this.Interpret(CompleteWay.CreateFrom(simpleOsmGeo as Way, data));
                case OsmGeoType.Relation:
                    return this.Interpret(CompleteRelation.CreateFrom(simpleOsmGeo as Relation, data));
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}
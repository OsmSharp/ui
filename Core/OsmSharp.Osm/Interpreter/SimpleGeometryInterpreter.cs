// OsmSharp - OpenStreetMap tools & library.
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
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data;
using OsmSharp.Osm.Simple;

namespace OsmSharp.Osm.Interpreter
{
    /// <summary>
    /// Simple implementation of OSM-data interpreter.
    /// </summary>
    public class SimpleGeometryInterpreter : GeometryInterpreter
    {
        /// <summary>
        /// Interprets an OSM-object and returns the corresponding geometry.
        /// </summary>
        /// <param name="osmObject"></param>
        /// <returns></returns>
        public override GeometryCollection Interpret(CompleteOsmGeo osmObject)
        {
            // DISCLAIMER: this is a very very very simple and inaccurate way of interpreting OSM-objects.
            //             Do not use this for any pupose other than testing or experimentation.

            GeometryCollection collection = new GeometryCollection();
            switch (osmObject.Type)
            {
                case CompleteOsmType.Node:
                    collection.Add(new Point((osmObject as CompleteNode).Coordinate));
                    break;
                case CompleteOsmType.Way:
                    if (osmObject.Tags.ContainsKey("area"))
                    { // area tags leads to simple polygon
                        collection.Add(
                            new LineairRing((osmObject as CompleteWay).GetCoordinates().ToArray<GeoCoordinate>()));
                    }
                    else
                    { // no area tag leads to just a line.
                        collection.Add(
                            new LineairRing((osmObject as CompleteWay).GetCoordinates().ToArray<GeoCoordinate>()));
                    }
                    break;
                case CompleteOsmType.Relation:
                    CompleteRelation relation = (osmObject as CompleteRelation);
                    foreach (var member in relation.Members)
                    {
                        collection.AddRange(this.Interpret(member.Member));
                    }
                    break;
            }
            return collection;
        }
    }
}

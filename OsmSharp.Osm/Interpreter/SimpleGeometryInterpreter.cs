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
using OsmSharp.Collections.Tags;

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
            // DISCLAIMER: this is a very very very simple geometry interpreter and
            // contains hardcoded all relevant tags.

            GeometryCollection collection = new GeometryCollection();
            TagsCollection tags;
            if (osmObject != null)
            {
                switch (osmObject.Type)
                {
                    case CompleteOsmType.Node:
                        SimpleTagsCollection newCollection = new SimpleTagsCollection(
                            osmObject.Tags);
                        newCollection.RemoveKey("FIXME");
                        newCollection.RemoveKey("node");
                        newCollection.RemoveKey("source");

                        if (newCollection.Count > 0)
                        { // there is still some relevant information left.
                            collection.Add(new Point((osmObject as CompleteNode).Coordinate));
                        }
                        break;
                    case CompleteOsmType.Way:
                        tags = osmObject.Tags;

                        bool isArea = false;
                        if (tags.ContainsKey("building") ||
                            tags.ContainsKey("landuse") ||
                            tags.ContainsKey("amenity") ||
                            tags.ContainsKey("harbour") ||
                            tags.ContainsKey("historic") ||
                            tags.ContainsKey("leisure") ||
                            tags.ContainsKey("man_made") ||
                            tags.ContainsKey("military") ||
                            tags.ContainsKey("natural") ||
                            tags.ContainsKey("office") ||
                            tags.ContainsKey("place") ||
                            tags.ContainsKey("power") ||
                            tags.ContainsKey("public_transport") ||
                            tags.ContainsKey("shop") ||
                            tags.ContainsKey("sport") ||
                            tags.ContainsKey("tourism") ||
                            tags.ContainsKey("waterway") ||
                            tags.ContainsKey("wetland") ||
                            tags.ContainsKey("water") ||
                            tags.ContainsKey("aeroway"))
                        { // these tags usually indicate an area.
                            isArea = true;
                        }

                        string areaValue;
                        if (tags.TryGetValue("area", out areaValue) &&
                            (areaValue == "yes" || areaValue == "1" || areaValue == "true"))
                        { // explicitly indicated that this is an area.
                            isArea = true;
                        }
                        else if (tags.TryGetValue("area", out areaValue) &&
                            (areaValue == "yes" || areaValue == "1" || areaValue == "true"))
                        { // explicitly indicated that this is not an area.
                            isArea = false;
                        }

                        if (isArea)
                        { // area tags leads to simple polygon
                            collection.Add(
                                new LineairRing((osmObject as CompleteWay).GetCoordinates().ToArray<GeoCoordinate>()));
                        }
                        else
                        { // no area tag leads to just a line.
                            collection.Add(
                                new LineString((osmObject as CompleteWay).GetCoordinates().ToArray<GeoCoordinate>()));
                        }
                        break;
                    case CompleteOsmType.Relation:
                        CompleteRelation relation = (osmObject as CompleteRelation);
                        tags = relation.Tags;

                        string typeValue;
                        if (tags.TryGetValue("type", out typeValue))
                        { // there is a type in this relation.
                            if (typeValue == "multipolygon")
                            { // this relation is a multipolygon.

                            }
                            else if (typeValue == "boundary")
                            { // this relation is a boundary.

                            }
                        }

                        foreach (var member in relation.Members)
                        {
                            collection.AddRange(this.Interpret(member.Member));
                        }
                        break;
                }
            }
            return collection;
        }
    }
}

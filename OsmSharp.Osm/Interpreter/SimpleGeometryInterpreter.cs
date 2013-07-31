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
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Attributes;

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

                        if (tags.IsTrue("area"))
                        { // explicitly indicated that this is an area.
                            isArea = true;
                        }
                        else if (tags.IsFalse("area"))
                        { // explicitly indicated that this is not an area.
                            isArea = false;
                        }

                        if (isArea)
                        { // area tags leads to simple polygon
                            LineairRing lineairRing = new LineairRing((osmObject as CompleteWay).GetCoordinates().ToArray<GeoCoordinate>());
                            lineairRing.Attributes = new SimpleGeometryAttributeCollection(tags);
                            collection.Add(lineairRing);
                        }
                        else
                        { // no area tag leads to just a line.
                            LineString lineString = new LineString((osmObject as CompleteWay).GetCoordinates().ToArray<GeoCoordinate>());
                            lineString.Attributes = new SimpleGeometryAttributeCollection(tags);
                            collection.Add(lineString);
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
                                Geometry geometry = this.InterpretMultipolygonRelation(relation);
                                if (geometry != null)
                                { // add the geometry.
                                    collection.Add(geometry);
                                }
                            }
                            else if (typeValue == "boundary")
                            { // this relation is a boundary.

                            }
                        }
                        break;
                }
            }
            return collection;
        }

        /// <summary>
        /// Tries to interpret a given multipolygon relation.
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        private Geometry InterpretMultipolygonRelation(CompleteRelation relation)
        {
            Geometry geometry = null;
            if (relation.Members == null)
            { // the relation has no members.
                return geometry;
            }

            // first 'compress' the inner- and outer-rings that consist out of multiple ways.
            List<LineairRing> rings = new List<LineairRing>();
            List<string> ringRoles = new List<string>();

            // parse all members to lineair rings.
            int idx = 0;
            while (idx < relation.Members.Count)
            {
                CompleteRelationMember member = relation.Members[idx];
                if (member.Role == "inner" || member.Role == "outer")
                { // the role at least is valid!
                    // check if the way is closed. if it is this member is just one lineair ring.
                    if (member.Member != null &&
                        member.Member is CompleteWay)
                    {
                        CompleteWay way = (member.Member as CompleteWay);
                        if (way.IsClosed())
                        { // the member is a way and it is closed.
                            LineairRing lineairRing = new LineairRing(way.GetCoordinates().ToArray<GeoCoordinate>());
                            rings.Add(lineairRing);
                            ringRoles.Add(member.Role);
                        }
                        else
                        { // the member is a way but unclosed.
                            List<GeoCoordinate> coordinates = way.GetCoordinates();
                            // move to the next way.
                            while (idx + 1 < relation.Members.Count &&
                                relation.Members[idx + 1].Role == member.Role &&
                                relation.Members[idx + 1].Member is CompleteWay)
                            {
                                // check if the way starting coordinate matches the way end coordinate.
                                CompleteWay nextWay = relation.Members[idx + 1].Member as CompleteWay;
                                if (!nextWay.IsClosed())
                                { // the next way is not closed.
                                    List<GeoCoordinate> nextCoordinates = nextWay.GetCoordinates();
                                    if (way.Nodes[way.Nodes.Count - 1].Id == nextWay.Nodes[0].Id)
                                    { // last node of the previous way is the first node of the next way.
                                        nextCoordinates = nextCoordinates.GetRange(1, way.Nodes.Count - 1);
                                    }
                                    else if (way.Nodes[way.Nodes.Count - 1].Id == nextWay.Nodes[nextWay.Nodes.Count - 1].Id)
                                    { // last node of the previous way is the last node of the next way.
                                        nextCoordinates.Reverse();
                                        nextCoordinates = nextCoordinates.GetRange(1, way.Nodes.Count - 1);
                                    }
                                    else
                                    { // the next way did not connect to the previous one.
                                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Osm.Interpreter.SimpleGeometryInterpreter", System.Diagnostics.TraceEventType.Error,
                                            "Unclosed way found in multipolygon relation without a way following it to close it.");
                                        break;
                                    }

                                    // add new coordinates.
                                    coordinates.AddRange(nextCoordinates);

                                    // check if polygon is closed now.
                                    if (coordinates.Count > 1 &&
                                        coordinates[0].Latitude == coordinates[coordinates.Count - 1].Latitude &&
                                        coordinates[0].Longitude == coordinates[coordinates.Count - 1].Longitude)
                                    { // ok create the lineair ring.
                                        LineairRing lineairRing = new LineairRing(coordinates.ToArray<GeoCoordinate>());
                                        rings.Add(lineairRing);
                                        ringRoles.Add(member.Role);
                                        break;
                                    }
                                }
                                else
                                { // the next way is closed, this should not be!
                                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Osm.Interpreter.SimpleGeometryInterpreter", System.Diagnostics.TraceEventType.Error,
                                        "Unclosed way found in multipolygon relation without a way following it to close it.");
                                    break;
                                }
                                idx++;
                            }
                        }
                    }
                }
                idx++;
            }

            // create polygons where needed.
            if (rings.Count == 1)
            { // there is just one ring.
                if (ringRoles[0] == "outer")
                { // just return the one ring.
                    geometry = rings[0];
                }
                else
                { // not an outer member.
                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Osm.Interpreter.SimpleGeometryInterpreter", System.Diagnostics.TraceEventType.Error,
                        "Invalid multipolygon relation: only one valid member detected but it does not have role 'outer'.");
                }
            }
            else
            { // there is more than one ring.
                MultiPolygon multiPolygon = new MultiPolygon();
                idx = 0;
                while (idx < rings.Count)
                {
                    if (ringRoles[idx] == "outer")
                    { // an outer role was found.
                        LineairRing outer = rings[idx];
                        // check for 'inner' holes.
                        List<LineairRing> inners = new List<LineairRing>();
                        idx++;
                        while (idx < rings.Count &&
                            ringRoles[idx] == "inner")
                        {
                            inners.Add(rings[idx]);
                            idx++;
                        }
                        multiPolygon.Add(new Polygon(outer, inners));
                    }
                }

                if (multiPolygon.Count == 1)
                { // just return the only polygon found.
                    geometry = multiPolygon[0];
                }
                else
                { // just return the entire multipolygon.
                    geometry = multiPolygon;
                }
            }
            if (geometry != null)
            {
                // converts the attributes.
                geometry.Attributes = new SimpleGeometryAttributeCollection(relation.Tags);
            }

            return geometry;
        }
    }
}
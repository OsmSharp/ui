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
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Shapes;
using OsmSharp.Tools.Math.Geo.Factory;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Interpreter translating OSM object to shapes.
    /// </summary>
    public class SimpleShapeInterpreter : IShapeInterpreter
    {
        #region IShapeInterpreter Members

        /// <summary>
        /// Returns an interpreted shape.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ShapeF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> Interpret(OsmGeo obj)
        {
            switch (obj.Type)
            {
                case OsmType.Node:
                    return new ShapeDotF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                        PrimitiveGeoFactory.Instance,
                        (obj as Node).Coordinate);
                case OsmType.Way:
                    if (obj.Tags.ContainsKey("area"))
                    {
                        return new ShapePolyGonF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                            PrimitiveGeoFactory.Instance,
                            (obj as Way).GetCoordinates().ToArray<GeoCoordinate>());
                    }
                    else
                    {
                        return new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                            PrimitiveGeoFactory.Instance,
                            (obj as Way).GetCoordinates().ToArray<GeoCoordinate>());
                    }
                case OsmType.Relation:
                    List<ShapeF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>> shapes = new List<ShapeF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>>();
                    foreach (RelationMember member in (obj as Relation).Members)
                    {
                        shapes.Add(this.Interpret(member.Member));
                    }

                    return new ShapeCombinedF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                        PrimitiveGeoFactory.Instance, shapes);
            }
            throw new InvalidOperationException(string.Format("Cannot interpret object {0}!", obj.ToString()));
        }

        #endregion
    }
}

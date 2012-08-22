using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Tools.Math.Shapes;
using Tools.Math.Geo.Factory;

namespace Osm.Core
{
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

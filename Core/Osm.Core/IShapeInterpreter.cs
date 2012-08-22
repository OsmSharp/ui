using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Tools.Math.Shapes;

namespace Osm.Core
{
    /// <summary>
    /// Interpreter interface.
    /// 
    /// Interprets osm objects and returns a shape object that represents the object.
    /// </summary>
    public interface IShapeInterpreter
    {
        /// <summary>
        /// Interprets a geo object and returns a shape.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ShapeF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> Interpret(OsmGeo obj);
    }
}

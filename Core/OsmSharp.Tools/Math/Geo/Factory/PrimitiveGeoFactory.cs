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
using OsmSharp.Tools.Math.Shapes.PrimitivesFactory;

namespace OsmSharp.Tools.Math.Geo.Factory
{
    /// <summary>
    /// A factory creating geo coordinate primitives.
    /// 
    /// Used in classes needing primitives of the geo type.
    /// </summary>
    public class PrimitiveGeoFactory : IPrimitivesFactory<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>
    {
        /// <summary>
        /// Creates a new primitive factory.
        /// </summary>
        private PrimitiveGeoFactory()
        {

        }

        #region Singleton

        /// <summary>
        /// Holds the instance of the primitives factory.
        /// </summary>
        private static PrimitiveGeoFactory _instance;

        /// <summary>
        /// Returns the instance of the primitives factory.
        /// </summary>
        public static PrimitiveGeoFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PrimitiveGeoFactory();
                }
                return _instance;
            }
        }

        #endregion

        #region IPrimitivesFactory<GeoCoordinate,GeoCoordinateBox> Members

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public GeoCoordinate CreatePoint(double[] values)
        {
            return new GeoCoordinate(values);
        }

        /// <summary>
        /// Creates a new geocoordinate box.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public GeoCoordinateBox CreateRectangle(GeoCoordinate[] points)
        {
            return new GeoCoordinateBox(points);
        }

        /// <summary>
        /// Creates a new geocoordinate line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public GeoCoordinateLine CreateLine(GeoCoordinate point1, GeoCoordinate point2)
        {
            return new GeoCoordinateLine(point1, point2,true,true);
        }

        #endregion
    }
}

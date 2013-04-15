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

namespace OsmSharp.Tools.Math.Primitives.Factory
{
    /// <summary>
    /// A factory class intialize general primitives.
    /// 
    /// Used in classes needing primitives.
    /// </summary>
    public class PrimitiveFactory : IPrimitivesFactory<PointF2D, RectangleF2D, LineF2D>
    {        /// <summary>
        /// Creates a new primitive factory.
        /// </summary>
        private PrimitiveFactory()
        {

        }

        #region Singleton

        /// <summary>
        /// Holds the instance of the primitives factory.
        /// </summary>
        private static PrimitiveFactory _instance;

        /// <summary>
        /// Returns the instance of the primitives factory.
        /// </summary>
        public static PrimitiveFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PrimitiveFactory();
                }
                return _instance;
            }
        }

        #endregion

        #region IPrimitivesFactory<PointType,RectangleType> Members

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public PointF2D CreatePoint(double[] values)
        {
            return new PointF2D(values);
        }

        /// <summary>
        /// Creates a new rectangle.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public RectangleF2D CreateRectangle(PointF2D[] points)
        {
            return new RectangleF2D(points);
        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public LineF2D CreateLine(PointF2D point1, PointF2D point2)
        {
            return new LineF2D(point1, point2);
        }

        #endregion
    }
}

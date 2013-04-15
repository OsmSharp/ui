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

namespace OsmSharp.Tools.Math
{
    /// <summary>
    /// Class representing a retangular box.
    /// </summary>
    public class RectangleF2D : GenericRectangleF2D<PointF2D>
    {
        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        internal RectangleF2D(PointF2D a, PointF2D b)
            :base(a,b)
        {

        }

        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="points"></param>
        internal RectangleF2D(PointF2D[] points)
            : base(points)
        {

        }

        #region Factory

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override PointF2D CreatePoint(double[] values)
        {
            return new PointF2D(values);
        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="is_segment1"></param>
        /// <param name="is_segment2"></param>
        /// <returns></returns>
        protected override GenericLineF2D<PointF2D> CreateLine(
            PointF2D point1,
            PointF2D point2,
            bool is_segment1,
            bool is_segment2)
        {
            return new LineF2D(point1, point2, is_segment1, is_segment2);
        }

        /// <summary>
        /// Creates a new rectangle
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected override GenericRectangleF2D<PointF2D> CreateRectangle(PointF2D[] points)
        {
            return new RectangleF2D(points);
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected override GenericPolygonF2D<PointF2D> CreatePolygon(PointF2D[] points)
        {
            return new PolygonF2D(points);
        }

        #endregion
    }
}

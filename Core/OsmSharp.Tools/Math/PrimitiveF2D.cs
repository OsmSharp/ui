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
    /// An abstract class serving as the base-type for all primitives.
    /// </summary>
    public abstract class PrimitiveF2D<PointType> : PrimitiveSimpleF2D
        where PointType : PointF2D
    {
        /// <summary>
        /// Calculates the distance of this primitive to the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public abstract double Distance(PointType p);

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected abstract PointType CreatePoint(double[] values);

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        protected GenericLineF2D<PointType> CreateLine(PointType point1, PointType point2)
        {
            return this.CreateLine(point1, point2, true, true);
        }

        /// <summary>
        /// Creates e new line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="is_segment"></param>
        /// <returns></returns>
        protected GenericLineF2D<PointType> CreateLine(
            PointType point1, 
            PointType point2, 
            bool is_segment)
        {
            return this.CreateLine(point1, point2, is_segment, is_segment);
        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="is_segment1"></param>
        /// <param name="is_segment2"></param>
        /// <returns></returns>
        protected abstract GenericLineF2D<PointType> CreateLine(
            PointType point1,
            PointType point2,
            bool is_segment1,
            bool is_segment2);

        /// <summary>
        /// Creates a new rectangle.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected abstract GenericRectangleF2D<PointType> CreateRectangle(PointType[] points);

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected abstract GenericPolygonF2D<PointType> CreatePolygon(PointType[] points);
    }
}

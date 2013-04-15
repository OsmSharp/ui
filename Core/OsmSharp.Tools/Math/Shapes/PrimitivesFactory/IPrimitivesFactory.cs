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

namespace OsmSharp.Tools.Math.Shapes.PrimitivesFactory
{
    /// <summary>
    /// Interface used to encapsulate the creation of primitive types for usage in shapes.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    /// <typeparam name="RectangleType"></typeparam>
    /// <typeparam name="LineType"></typeparam>
    public interface IPrimitivesFactory<PointType,RectangleType,LineType>
        where PointType : PointF2D
        where RectangleType : GenericRectangleF2D<PointType> 
        where LineType : GenericLineF2D<PointType>
    {
        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        PointType CreatePoint(double[] values);

        /// <summary>
        /// Creates a new rectangle.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        RectangleType CreateRectangle(PointType[] points);

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        LineType CreateLine(PointType point1, PointType point2);
    }
}

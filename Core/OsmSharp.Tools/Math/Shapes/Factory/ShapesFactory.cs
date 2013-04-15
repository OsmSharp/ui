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

namespace OsmSharp.Tools.Math.Shapes.Factory
{
    /// <summary>
    /// A factory for shapes.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    /// <typeparam name="RectangleType"></typeparam>
    /// <typeparam name="LineType"></typeparam>
    public class ShapesFactory<PointType, RectangleType, LineType>
        where PointType : PointF2D
        where RectangleType : GenericRectangleF2D<PointType> 
        where LineType : GenericLineF2D<PointType>
    {
        /// <summary>
        /// Holds the primitives factory for this shapes factory.
        /// </summary>
        private IPrimitivesFactory<PointType, RectangleType, LineType> _primitives_factory;

        /// <summary>
        /// Creates a new shape factory.
        /// </summary>
        /// <param name="primitives_factory"></param>
        public ShapesFactory(IPrimitivesFactory<PointType, RectangleType, LineType> primitives_factory)
        {
            _primitives_factory = primitives_factory;
        }

        #region IPrimitivesFactory<PointType,RectangleType> Members

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public PointType CreatePoint(double[] values)
        {
            return _primitives_factory.CreatePoint(values);
        }

        /// <summary>
        /// Creates a new rectangle.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public RectangleType CreateRectangle(PointType[] points)
        {
            return _primitives_factory.CreateRectangle(points);
        }

        #endregion
    }
}

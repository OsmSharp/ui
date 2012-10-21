// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Shapes.PrimitivesFactory;
using Tools.Math.Shapes.ResultHelpers;

namespace Tools.Math.Shapes
{
    /// <summary>
    /// Represents a shape.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    [Serializable]
    public abstract class ShapeF<PointType, RectangleType, LineType>
        where PointType : PointF2D
        where RectangleType : GenericRectangleF2D<PointType>
        where LineType : GenericLineF2D<PointType>
    {
        /// <summary>
        /// Holds the primitives factory.
        /// </summary>
        private IPrimitivesFactory<PointType, RectangleType, LineType> _primitives_factory;

        /// <summary>
        /// Creates a new shape initializes using a primitives factory.
        /// </summary>
        /// <param name="primitives_factory"></param>
        public ShapeF(IPrimitivesFactory<PointType, RectangleType, LineType> primitives_factory)
        {
            _primitives_factory = primitives_factory;
        }

        /// <summary>
        /// Returns the primitives factory.
        /// </summary>
        public IPrimitivesFactory<PointType, RectangleType, LineType> PrimitivesFactory
        {
            get
            {
                return _primitives_factory;
            }
        }

        /// <summary>
        /// Returns the bounding box for this shape.
        /// </summary>
        public abstract RectangleType BoundingBox
        {
            get;
        }

        /// <summary>
        /// Calculates the distance of the point to this shape.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual double Distance(PointType point)
        {
            return this.DistanceDetailed(point).Distance;
        }

        /// <summary>
        /// Calculates the distance from the point to this shape and returns all calculate information.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract DistanceResult<PointType> DistanceDetailed(PointType point);

        /// <summary>
        /// Returns true if any part of this shape is inside the box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public abstract bool Inside(RectangleType box);
    }
}

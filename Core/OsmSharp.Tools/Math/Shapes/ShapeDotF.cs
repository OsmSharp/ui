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
using OsmSharp.Tools.Math.Shapes.ResultHelpers;

namespace OsmSharp.Tools.Math.Shapes
{
    /// <summary>
    /// Dot shape class; represents a point with a certain thickness.
    /// </summary>
    public class ShapeDotF<PointType, RectangleType, LineType> : ShapeF<PointType, RectangleType, LineType>
        where PointType : PointF2D
        where RectangleType : GenericRectangleF2D<PointType>
        where LineType : GenericLineF2D<PointType>
    {
        /// <summary>
        /// The point a the center of this dot.
        /// </summary>
        private PointType _point;

        /// <summary>
        /// Creates a new dot shape.
        /// </summary>
        /// <param name="primitives_factory"></param>
        /// <param name="point"></param>
        public ShapeDotF(
            IPrimitivesFactory<PointType, RectangleType, LineType> primitives_factory,
            PointType point)
            :base(primitives_factory)
        {
            _point = point;
        }

        /// <summary>
        /// The point this dot is located at.
        /// </summary>
        public PointType Point
        {
            get
            {
                return _point;
            }
        }

        #region Box

        private RectangleType _box;

        /// <summary>
        /// Returns a bounding box.
        /// </summary>
        public override RectangleType BoundingBox
        {
            get
            {
                if (_box == null)
                {
                    _box = this.PrimitivesFactory.CreateRectangle(new PointType[]{_point});
                }
                return _box;
            }
        }

        /// <summary>
        /// Calculates the distance to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double Distance(PointType point)
        {
            // just calculate the distance point to point.
            return point.Distance(_point);
        }

        /// <summary>
        /// Calculate the distance details to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override DistanceResult<PointType> DistanceDetailed(PointType point)
        {
            DistanceResult<PointType> distance = new DistanceResult<PointType>();
            distance.Distance = point.Distance(_point);
            distance.ClosestPrimitive = _point;
            return distance;
        }

        /// <summary>
        /// Returns true if this dot lies inside the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public override bool Inside(RectangleType box)
        {
            return box.IsInside(_point);
        }

        #endregion

    }
}

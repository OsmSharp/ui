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
    /// Represents a combined shape.
    /// 
    /// This shape can be multiple combined shapes.
    /// </summary>
    public class ShapeCombinedF<PointType, RectangleType, LineType> : ShapeF<PointType, RectangleType, LineType>
        where PointType : PointF2D
        where RectangleType : GenericRectangleF2D<PointType>
        where LineType : GenericLineF2D<PointType>
    {
        /// <summary>
        /// Holds all the shapes of this combined shape.
        /// </summary>
        private IList<ShapeF<PointType, RectangleType, LineType>> _shapes;

        /// <summary>
        /// Creates a new combined shape.
        /// </summary>
        /// <param name="primitives_factory"></param>
        /// <param name="shapes"></param>
        public ShapeCombinedF(
            IPrimitivesFactory<PointType, RectangleType, LineType> primitives_factory,
            IList<ShapeF<PointType, RectangleType, LineType>> shapes)
            :base(primitives_factory)
        {
            _shapes = shapes;
            _bb = null;
        }

        private RectangleType _bb;

        /// <summary>
        /// Returns the bounding box around this combined shape.
        /// </summary>
        public override RectangleType BoundingBox
        {
	        get 
            {
                if (_bb == null)
                {
                    ShapeF<PointType, RectangleType, LineType> shape_1 = _shapes[0];
                    _bb = shape_1.BoundingBox;
                    for (int idx = 1; idx < _shapes.Count; idx++)
                    {
                        List<PointType> points = new List<PointType>();
                        points.AddRange(_bb.Corners);
                        points.AddRange(_shapes[idx].BoundingBox.Corners);

                        _bb = this.PrimitivesFactory.CreateRectangle(points.ToArray());
                    }
                }
                return _bb;
            }
        }

        /// <summary>
        /// Calculates the distance of the point to this shape.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double Distance(PointType point)
        {
            // calculate the distance to the first point.
            double distance = _shapes[0].Distance(point);

            // loop over the rest and keep the shortest distance.
            for (int idx = 1; idx < _shapes.Count; idx++)
            {
                // calculate the new distance.
                double new_distance = _shapes[idx].Distance(point);

                // keep it if is smaller.
                if (new_distance < distance)
                { // new distance is smaller.
                    distance = new_distance;
                }
            }

            // return the found distance.
            return distance;
        }

        /// <summary>
        /// Calculates the closest distance to this shape.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override DistanceResult<PointType> DistanceDetailed(PointType point)
        {
            // calculate the distance to the first point.
            DistanceResult<PointType> distance = _shapes[0].DistanceDetailed(point);

            // loop over the rest and keep the shortest distance.
            for (int idx = 1; idx < _shapes.Count; idx++)
            {
                // calculate the new distance.
                DistanceResult<PointType> new_distance = _shapes[idx].DistanceDetailed(point);

                // keep it if is smaller.
                if (new_distance.Distance < distance.Distance)
                { // new distance is smaller.
                    distance = new_distance;
                }
            }

            // return the found distance.
            return distance;
        }

        /// <summary>
        /// Returns true if any part of the shape appears inside the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public override bool Inside(RectangleType box)
        {
            foreach (ShapeF<PointType, RectangleType, LineType> shape in _shapes)
            {
                if (shape.Inside(box))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

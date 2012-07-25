using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Shapes.PrimitivesFactory;
using Tools.Math.Shapes.ResultHelpers;

namespace Tools.Math.Shapes
{
    /// <summary>
    /// Polyline shape: represents a line with multiple segments.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    /// <typeparam name="BoxType"></typeparam>
    [Serializable]
    public class ShapePolyLineF<PointType, RectangleType, LineType> : ShapeF<PointType, RectangleType, LineType>
        where PointType : PointF2D
        where RectangleType : GenericRectangleF2D<PointType>
        where LineType : GenericLineF2D<PointType>
    {
        /// <summary>
        /// The points of this polyline.
        /// </summary>
        private PointType[] _points;

        /// <summary>
        /// Creates a new polyline.
        /// </summary>
        /// <param name="points"></param>
        public ShapePolyLineF(
            IPrimitivesFactory<PointType, RectangleType, LineType> primitives_factory,
            PointType[] points)
            :base(primitives_factory)
        {
            _points = points;
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
                    _box = this.PrimitivesFactory.CreateRectangle(_points);
                }
                return _box;
            }
        }

        #endregion

        /// <summary>
        /// Returns the points in this line.
        /// </summary>
        public PointType[] Points
        {
            get
            {
                return _points;
            }
        }

        /// <summary>
        /// Calculates the distance this the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double Distance(PointType point)
        {
            // calculate the distance to the first point.
            double distance = double.MaxValue;

            LineType line = null;
            for (int idx = 0; idx < _points.Length - 1; idx++)
            {
                line = this.PrimitivesFactory.CreateLine(_points[idx], _points[idx + 1]);
                double distance_to_line = line.Distance(point);
                if (distance_to_line < distance)
                {
                    distance = distance_to_line;
                }
            }

            // return the found distance.
            return distance;
        }

        /// <summary>
        /// Calculates the distance to a given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override DistanceResult<PointType>  DistanceDetailed(PointType point)
        {
            return this.ProjectOn(point);
        }

        /// <summary>
        /// Projects the point onto the multiline using the shortest rectangular distance.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public LineProjectionResult<PointType> ProjectOn(PointType point)
        {
            LineType found_line = null;
            double distance = double.MaxValue;
            int found_idx = -1;
            for (int idx = 0; idx < _points.Length - 1; idx++)
            {
                LineType line = this.PrimitivesFactory.CreateLine(_points[idx], _points[idx + 1]);
                double distance_to_line = line.Distance(point);
                if (distance_to_line < distance)
                {
                    found_line = line;
                    distance = distance_to_line;
                    found_idx = idx;
                }
            }

            if(found_line != null)
            {
                LineProjectionResult<PointType> result = new LineProjectionResult<PointType>();
                result.Distance = distance;
                result.ClosestPrimitive = found_line.ProjectOn(point);
                result.Idx = found_idx;        
        
                return result;
            }

            return null;
        }

        /// <summary>
        /// Return true if any part of this polyline comes inside of the box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public override bool Inside(RectangleType box)
        {
            for (int idx = 0; idx < _points.Length - 1; idx++)
            {
                if (box.IntersectsPotentially(_points[idx], _points[idx + 1]))
                {
                    if (box.Intersects(_points[idx], _points[idx + 1]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class LineProjectionResult<PointType> : DistanceResult<PointType>
        where PointType : PointF2D
    {
        internal LineProjectionResult()
        {

        }

        public int Idx { get; internal set; }
    }
}

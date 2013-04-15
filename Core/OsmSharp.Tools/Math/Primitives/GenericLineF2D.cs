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
    /// Represents a line.
    /// 
    /// A line is immutable.
    /// </summary>
    public abstract class GenericLineF2D<PointType> : PrimitiveF2D<PointType>
        where PointType : PointF2D
    {
        /// <summary>
        /// The first point for this line.
        /// </summary>
        private PointType _a;

        /// <summary>
        /// The second point for this line.
        /// </summary>
        private PointType _b;

        /// <summary>
        /// The direction of this line.
        /// </summary>
        private VectorF2D _dir;

        /// <summary>
        /// True if this represents only a segment.
        /// </summary>
        private bool _is_segment1;

        /// <summary>
        /// True if this represents only a segment.
        /// </summary>
        private bool _is_segment2;

        #region Constructors

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public GenericLineF2D(PointType a, PointType b)
            : this(a,b,false)
        {

        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="is_segment"></param>
        public GenericLineF2D(PointType a, PointType b, bool is_segment)
        {
            _a = a;
            _b = b;

            _dir = _b - _a;
            _is_segment1 = is_segment;
            _is_segment2 = is_segment;
        }


        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="is_segment1"></param>
        /// <param name="is_segment2"></param>
        public GenericLineF2D(PointType a, PointType b, bool is_segment1, bool is_segment2)
        {
            _a = a;
            _b = b;

            _dir = _b - _a;
            _is_segment1 = is_segment1;
            _is_segment2 = is_segment2;
        }


        #endregion

        #region Properties
        
        /// <summary>
        /// Returns the first point of this line.
        /// </summary>
        public PointType Point1
        {
            get
            {
                return _a;
            }
        }

        /// <summary>
        /// Returns the second point of this line.
        /// </summary>
        public PointType Point2
        {
            get
            {
                return _b;
            }
        }

        /// <summary>
        /// Returns the direction of this line.
        /// </summary>
        public VectorF2D Direction
        {
            get
            {
                return _dir;
            }
        }

        /// <summary>
        /// Returns the length of this line (as if it were a segment).
        /// </summary>
        public double Length
        {
            get
            {
                return this.Direction.Size;
            }
        }

        /// <summary>
        /// Returns true if this line is just a segment.
        /// </summary>
        public bool IsSegment
        {
            get
            {
                return _is_segment1 && _is_segment2;
            }
        }

        /// <summary>
        /// Returns true if the first point is the end of the line.
        /// </summary>
        /// <returns></returns>
        public bool IsSegment1
        {
            get
            {
                return _is_segment1;
            }
        }

        /// <summary>
        /// Returns true if the second point is the end of the line.
        /// </summary>
        /// <returns></returns>
        public bool IsSegment2
        {
            get
            {
                return _is_segment2;
            }
        }

        #region Line-Equation Parameters

        /// <summary>
        /// Returns parameter A of an equation describing this line as Ax + By = C
        /// </summary>
        internal double A
        {
            get
            {
                return this.Point2[1] - this.Point1[1];
            }
        }

        /// <summary>
        /// Returns parameter B of an equation describing this line as Ax + By = C
        /// </summary>
        internal double B
        {
            get
            {
                return this.Point1[0] - this.Point2[0];
            }
        }

        /// <summary>
        /// Returns parameter C of an equation describing this line as Ax + By = C
        /// </summary>
        internal double C
        {
            get
            {
                return this.A * this.Point1[0] + this.B * this.Point1[1];
            }
        }

        #endregion

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the distance from the given point to this line.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override double Distance(PointType p)
        {
            return (double)(System.Math.Abs(this.A * p[0] + this.B * p[1] + this.C)
                / System.Math.Sqrt(this.A * this.A + this.B * this.B));
        }

        /// <summary>
        /// Calculates the position of this point relative to this line.
        /// 
        /// Left/Right is viewed from point1 in the direction of point2.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public LinePointPosition PositionOfPoint(PointF2D point)
        {
            VectorF2D a_point = point - _a;

            double dot_value = VectorF2D.Cross(this.Direction, a_point);
            if (dot_value > 0)
            {
                return LinePointPosition.Left;
            }
            else if(dot_value < 0)
            {
                return LinePointPosition.Right;
            }
            else
            {
                return LinePointPosition.On;
            }
        }

        /// <summary>
        /// Returns the distance from the point to this line.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double Distance(PointF2D point)
        {
            double distance = 0.0f;

            // get the second vector for the cross product.
            VectorF2D a_point = point - _a;

            // get the cross product.
            double cross = VectorF2D.Cross(this.Direction, a_point);
            
            // distances between a, b and point.
            double distance_a_b = this.Length;
            double distance_a = point.Distance(_a);
            double distance_b = point.Distance(_b);

            // calculate distance to line as if it were no segment.
            distance = System.Math.Abs(cross / distance_a_b);

            // if this line is a segment.
            if(this.IsSegment)
            {
                double dot1 = VectorF2D.Dot(a_point, this.Direction);
                if (dot1 < 0 && cross != 0)
                {
                    distance = _a.Distance(point);
                }
                else if (cross == 0 && 
                    (distance_a >= distance_a_b
                    || distance_b >= distance_a_b))
                { // check if the point is between the points.
                    if (distance_a > distance_b)
                    { // if the distance to a is greater then the point is at the b-side.
                        distance = _b.Distance(point);
                    }
                    else
                    {// if the distance to b is greater then the point is at the a-side.
                        distance = _a.Distance(point);
                    }
                }
                VectorF2D b_point = point - _b;
                double dot2 = VectorF2D.Dot(b_point, this.Direction.Inverse);
                if (dot2 < 0 && cross != 0)
                {
                    distance = _b.Distance(point);
                } 
            }
            return distance;
        }

        #region Intersections

        /// <summary>
        /// Calculates and returns the line intersection.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public PrimitiveSimpleF2D Intersection(GenericLineF2D<PointType> line)
        {
            return this.Intersection(line, true);
        }

        /// <summary>
        /// Calculates and returns the line intersection.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="do_segment"></param>
        /// <returns></returns>
        public PrimitiveSimpleF2D Intersection(GenericLineF2D<PointType> line, bool do_segment)
        {
            if (line == this)
            { // if the lines equal, the full lines intersect.
                return line;
            }
            else if (line.A == this.A
                    && line.B == this.B
                    && line.C == this.C)
            { // if the lines equal in direction and position; return the smallest segment.
                KeyValuePair<double, PointType> point1 = new KeyValuePair<double, PointType>(
                    0, this.Point1); 

                KeyValuePair<double, PointType> point2 = new KeyValuePair<double, PointType>(
                     this.Point1.Distance(this.Point2), this.Point2);

                // sort.
                KeyValuePair<double, PointType> temp;
                if (point2.Key < point1.Key)
                { // point2 smaller than point1.
                    temp = point2;
                    point2 = point1;
                    point1 = temp;
                }

                KeyValuePair<double, PointType> point = new KeyValuePair<double, PointType>(
                     this.Point1.Distance(line.Point1), line.Point1);

                if (point.Key < point2.Key) // sort.
                { // point smaller than point2.
                    temp = point;
                    point = point2;
                    point2 = temp;
                }
                if (point2.Key < point1.Key)
                { // point2 smaller than point1.
                    temp = point2;
                    point2 = point1;
                    point1 = temp;
                }

                point = new KeyValuePair<double, PointType>(
                     this.Point1.Distance(line.Point2), line.Point2);

                if (point.Key < point2.Key) // sort.
                { // point smaller than point2.
                    temp = point;
                    point = point2;
                    point2 = temp;
                }
                if (point2.Key < point1.Key)
                { // point2 smaller than point1.
                    temp = point2;
                    point2 = point1;
                    point1 = temp;
                }

                return this.CreateLine(
                    point1.Value,
                    point2.Value,
                    true);
            }
            else
            {
                // line.A = A1, line.B = B1, line.C = C1, this.A = A2, this.B = B2, this.C = C2
                double det = (line.A * this.B - this.A * line.B);
                if (det == 0) // TODO: implement an accuracy threshold epsilon.
                { // lines are parallel; no intersections.
                    return null;
                }
                else
                { // lines are not the same and not parallel so they will intersect.
                    double x = (this.B * line.C - line.B * this.C) / det;
                    double y = (line.A * this.C - this.A * line.C) / det;

                    // create the point.
                    PointType point = this.CreatePoint(new double[]{x, y});

                    // this line is a segment.
                    if (do_segment && this.IsSegment)
                    { // test where the intersection lies.
                        double this_distance =
                            this.Point1.Distance(this.Point2);

                        // if in any directions one of the points are further away from the point.
                        double this_distance_1 = this.Point1.Distance(point);
                        if (this_distance_1 > this_distance)
                        { // the point is further away.
                            return null;
                        }

                        // if in any directions one of the points are further away from the point.
                        double this_distance_2 = this.Point2.Distance(point);
                        if (this_distance_2 > this_distance)
                        { // the point is further away.
                            return null;
                        }                        
                    }

                    // TODO: implement partial segment.
                    // other line is a segment.
                    if (do_segment && line.IsSegment)
                    { // test where the intersection lies.
                        double this_distance =
                            line.Point1.Distance(line.Point2);

                        // if in any directions one of the points are further away from the point.
                        double this_distance_1 = line.Point1.Distance(point);
                        if (this_distance_1 > this_distance)
                        { // the point is further away.
                            return null;
                        }

                        // if in any directions one of the points are further away from the point.
                        double this_distance_2 = line.Point2.Distance(point);
                        if (this_distance_2 > this_distance)
                        { // the point is further away.
                            return null;
                        }     
                    }

                    // the intersection is valid.
                    return point;
                }
            }
        }

        /// <summary>
        /// Projects a point onto the given line.
        /// 
        /// (= intersection of the line with an angle 90° different from this line through the given point)
        /// </summary>
        /// <param name="point"></param>
        /// <returns>The projection point if it occurs inside the segmented line.</returns>
        public PointF2D ProjectOn(PointType point)
        {
            // get the direction.
            VectorF2D direction = this.Direction;

            // rotate.
            VectorF2D rotated = direction.Rotate90(true);

            // create second point
            PointType point2 = this.CreatePoint((point + rotated).ToArray());

            if (point[0] != point2[0] || point[1] != point2[1])
            {
                // create line.
                GenericLineF2D<PointType> line = this.CreateLine(
                    point,
                    point2,
                    false,
                    false);

                // intersect.
                PrimitiveSimpleF2D primitive =
                    this.Intersection(line, false);

                if (primitive == null)
                {
                    return null;
                }
                else if (primitive is PointF2D)
                {
                    return primitive as PointF2D;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                return point2;
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// The line-point positions.
    /// </summary>
    public enum LinePointPosition
    {
        /// <summary>
        /// Left.
        /// </summary>
        Left,
        /// <summary>
        /// Right.
        /// </summary>
        Right,
        /// <summary>
        /// On.
        /// </summary>
        On
    }
}
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
using OsmSharp.Tools.Math.Primitives.Enumerators.Lines;
using OsmSharp.Tools.Math.Primitives.Enumerators.Points;

namespace OsmSharp.Tools.Math
{
    /// <summary>
    /// Represents an n-dimensional bounding box.
    /// </summary>
    public abstract class GenericRectangleF2D<PointType> : PrimitiveF2D<PointType>, ILineList<PointType>, IPointList<PointType>
        where PointType : PointF2D
    {
        /// <summary>
        /// Holds the maximum values of the rectangle.
        /// </summary>
        private double[] _max;

        /// <summary>
        /// Holds the minimum values of the rectangle.
        /// </summary>
        private double[] _min;

        #region Constructors

        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        protected GenericRectangleF2D(PointType a, PointType b)
            :this(new PointType[] { a, b })
        {
            
        }

        /// <summary>
        /// Creates a new box around the given points.
        /// </summary>
        /// <param name="points"></param>
        protected GenericRectangleF2D(PointType[] points)
        {
            this.Mutate(points);
        }

        /// <summary>
        /// Creates a new box around the given points.
        /// </summary>
        /// <param name="points"></param>
        protected GenericRectangleF2D(IList<PointType> points)
            : this(points.ToArray<PointType>())
        {

        }

        #endregion

        #region Mutators

        /// <summary>
        /// Mutates this generic box to another generic box.
        /// </summary>
        /// <param name="points"></param>
        private void Mutate(
            PointType[] points)
        {
            // initialize the maximum array.
            _max = new double[2];

            // initialize the minimum array.
            _min = new double[2];

            // intialize with the first point.
            PointType a = points[0];

            // loop over all points and store the max and minimum.
            for (int idx = 0; idx < 2; idx++)
            {
                _max[idx] = double.MinValue;
                _min[idx] = double.MaxValue;

                for (int p_idx = 0; p_idx < points.Length; p_idx++)
                {
                    PointType b = points[p_idx];
                    if (_max[idx] < b[idx])
                    {
                        _max[idx] = b[idx];
                    }
                    if (_min[idx] > b[idx])
                    {
                        _min[idx] = b[idx];
                    }
                }
            }

            return;
        }

        #endregion

        #region Properties
        
        private double[] _delta;

        /// <summary>
        /// Returns the delta (difference) between min and max for every dimension.
        /// </summary>
        public double[] Delta
        {
            get
            {
                if (_delta == null)
                {
                    _delta = new double[2];

                    for (int idx = 0; idx < 2; idx++)
                    {
                        _delta[idx] = System.Math.Abs(_max[idx] - _min[idx]);
                    }
                }
                return _delta;
            }
        }
        
        /// <summary>
        /// Returns the max of this box for each dimension.
        /// </summary>
        public double[] Max
        {
            get
            {
                return _max;
            }
        }

        /// <summary>
        /// Returns the min of this box for each dimension.
        /// </summary>
        public double[] Min
        {
            get
            {
                return _min;
            }
        }


        /// <summary>
        /// Returns all the corners of this box.
        /// </summary>
        public PointType[] Corners
        {
            get
            {
                PointType[] corners = new PointType[(int)System.Math.Pow(2,2)];
                for (int cnt = 0; cnt < ((int)System.Math.Pow(2, 2)); cnt++)
                {
                    double[] p = new double[2];
                    for (int idx = 0; idx < _max.Length; idx++)
                    {
                        bool max = (((cnt / ((int)System.Math.Pow(2, idx))) 
                            % (int)System.Math.Pow(2, idx + 1)) == 0);
                        if (max)
                        {
                            p[idx] = _max[idx];
                        }
                        else
                        {
                            p[idx] = _min[idx];
                        }
                    }

                    corners[cnt] = this.CreatePoint(p);
                }

                return corners;
            }
        }

        /// <summary>
        /// Returns the middle of this box.
        /// </summary>
        public PointType Middle
        {
            get
            {
                double[] middle = new double[2];

                for (int idx = 0; idx < 2; idx++)
                {
                    middle[idx] = (_max[idx] + _min[idx]) / 2.0f;
                }

                return this.CreatePoint(middle);
            }
        }

        /// <summary>
        /// Returns the size of the surface of this rectangle.
        /// </summary>
        public double Surface
        {
            get
            {
                double surface = 1;
                foreach (double dimension in this.Delta)
                {
                    surface =
                        dimension * surface;
                }

                return surface;
            }
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the distance from the given point to this rectangle.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override double Distance(PointType p)
        {
            // initialize to the max possible value.
            double distance = double.MaxValue;

            // loop over all lines and store minimum.
            foreach (GenericLineF2D<PointType> line in this.LineEnumerator)
            {
                // calculate new distance.
                double new_distance = line.Distance(p);

                // keep it if is smaller.
                if (new_distance < distance)
                { // new distance is smaller.
                    distance = new_distance;
                }
            }

            // TODO: what to do when the point is inside the polygon?

            return distance;
        }

        #region Is Inside

        /// <summary>
        /// Returns true if the point lies inside this box.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool IsInside(PointF2D a)
        {
            bool is_inside = true;

            for (int idx = 0; idx < 2; idx++)
            {
                is_inside = is_inside && (_max[idx] >= a[idx] && a[idx] >= _min[idx]);
            }

            return is_inside;
        }

        /// <summary>
        /// Returns true if the given box is completely inside this box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool IsInside(GenericRectangleF2D<PointType> box)
        {
            foreach (PointType p in box.Corners)
            {
                if (!this.IsInside(p))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the list of points from the given list that are inside the given box.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public List<PointType> IsInside(List<PointType> points)
        {
            List<PointType> points_inside = new List<PointType>();

            foreach (PointType point in points)
            {
                if (this.IsInside(point))
                {
                    points_inside.Add(point);
                }
            }

            return points_inside;
        }

        /// <summary>
        /// Returns the list of points from the given list that are inside the given box.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public List<PointType> IsInside(PointType[] points)
        {
            List<PointType> points_inside = new List<PointType>();

            foreach (PointType point in points)
            {
                if (this.IsInside(point))
                {
                    points_inside.Add(point);
                }
            }

            return points_inside;
        }

        /// <summary>
        /// Returns true if any of the given points lie inside this bounding box.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool IsInsideAny(PointF2D[] points)
        {
            if (points != null)
            {
                for (int idx = 0; idx < points.Length; idx++)
                {
                    if (this.IsInside(points[idx]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Overlaps

        /// <summary>
        /// Returns true if the boxes overlap.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool Overlaps(GenericRectangleF2D<PointType> box)
        {
            foreach (PointF2D p in box.Corners)
            {
                if (this.IsInside(p))
                {
                    return true;
                }
            }
            foreach (PointF2D p in this.Corners)
            {
                if (box.IsInside(p))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the boxes overlap.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool Overlaps(RectangleF2D box)
        {
            foreach (PointF2D p in box.Corners)
            {
                if (this.IsInside(p))
                {
                    return true;
                }
            }
            foreach (PointF2D p in this.Corners)
            {
                if (box.IsInside(p))
                {
                    return true;
                }
            }
            return false;
        }


        #endregion

        #region Intersects

        /// <summary>
        /// Returns true if the two points could potentially intersect this box.
        /// 
        /// This is a rudemantairy quick test to rule out intersection. 
        ///     - If false is returned there can be no intersection.
        ///     - If true is returned there might be intersection but it is not certain.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public bool IntersectsPotentially(PointType point1, PointType point2)
        {
            for (int idx = 0; idx < 2; idx++)
            {
                if(point1[idx] > _max[idx] && point2[idx] > _max[idx])
                {
                    return false;
                }
                if (point1[idx] < _min[idx] && point2[idx] < _min[idx])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the two points could potentially intersect this bounding box.
        /// 
        /// This is a rudemantairy quick test to rule out intersection. 
        ///     - If false is returned there can be no intersection.
        ///     - If true is returned there might be intersection but it is not certain.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool IntersectsPotentially(GenericLineF2D<PointType> line)
        {
            return this.IntersectsPotentially(line.Point1, line.Point2);
        }

        /// <summary>
        /// Returns true if the line intersects with this bounding box.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool Intersects(GenericLineF2D<PointType> line)
        {
            // if all points have the same relative position with respect to the line
            // there is no intersection. In the other case there is.

            PointType[] corners = this.Corners;
            LinePointPosition first_position = line.PositionOfPoint(corners[0]);

            for (int idx = 1; idx <= corners.Length; idx++)
            {
                if (line.PositionOfPoint(corners[idx]) != first_position)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the line intersects with this bounding box.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public bool Intersects(PointType point1, PointType point2)
        {
            // if all points have the same relative position with respect to the line
            // there is no intersection. In the other case there is.

            PointType[] corners = this.Corners;
            GenericLineF2D<PointType> line = this.CreateLine(point1,point2,true);

            LinePointPosition first_position = line.PositionOfPoint(corners[0]);

            for (int idx = 1; idx < corners.Length; idx++)
            {
                if (line.PositionOfPoint(corners[idx]) != first_position)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #endregion

        #region ILineList<PointType> Members

        /// <summary>
        /// Returns the number of lines.
        /// </summary>
        int ILineList<PointType>.Count
        {
            get 
            {
                return 4; 
            }
        }

        /// <summary>
        /// Returns the line at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        GenericLineF2D<PointType> ILineList<PointType>.this[int idx]
        {
            get 
            {
                IPointList<PointType> this_as_point_list = this;
                switch (idx)
                {
                    case 0:
                        return this.CreateLine(
                            this_as_point_list[0],
                            this_as_point_list[1]);
                    case 1:
                        return this.CreateLine(
                            this_as_point_list[1],
                            this_as_point_list[2]);
                    case 2:
                        return this.CreateLine(
                            this_as_point_list[2],
                            this_as_point_list[3]);
                    case 3:
                        return this.CreateLine(
                            this_as_point_list[3],
                            this_as_point_list[0]);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Returns the line enumerator.
        /// </summary>
        public LineEnumerable<PointType> LineEnumerator
        {
            get
            {
                return new LineEnumerable<PointType>
                    (new LineEnumerator<PointType>(this));
            }
        }


        #endregion

        #region IPointList<PointType> Members
        
        /// <summary>
        /// Returns the number of points.
        /// </summary>
        int IPointList<PointType>.Count
        {
            get 
            { 
                return 4; 
            }
        }

        /// <summary>
        /// Returns the point at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        PointType IPointList<PointType>.this[int idx]
        {
            get 
            {
                switch (idx)
                {
                    case 0:
                        return this.CreatePoint(new double[] { _min[0], _min[1] });
                    case 1:
                        return this.CreatePoint(new double[] { _max[0], _min[1] });
                    case 2:
                        return this.CreatePoint(new double[] { _min[0], _max[1] });
                    case 3:
                        return this.CreatePoint(new double[] { _max[0], _max[1] });
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Returns the point enumerator.
        /// </summary>
        public PointEnumerable<PointType> PointEnumerator
        {
            get
            {
                return new PointEnumerable<PointType>
                    (new PointEnumerator<PointType>(this));
            }
        }

        #endregion
    }
}

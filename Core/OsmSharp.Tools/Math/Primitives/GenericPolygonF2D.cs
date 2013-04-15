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
using OsmSharp.Tools.Math.Primitives.Enumerators;
using OsmSharp.Tools.Math.Primitives.Enumerators.Lines;
using OsmSharp.Tools.Math.Primitives.Enumerators.Points;

namespace OsmSharp.Tools.Math
{
    /// <summary>
    /// Class representing a polygon.
    /// 
    /// Polygon is immutable.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    public abstract class GenericPolygonF2D<PointType> : PrimitiveF2D<PointType>, ILineList<PointType>, IPointList<PointType>
        where PointType : PointF2D
    {
        /// <summary>
        /// Holds the array of points representing this polygon.
        /// </summary>
        private PointType[] _points;

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="points"></param>
        public GenericPolygonF2D(IList<PointType> points)
        {
            // make a copy.
            _points = points.ToArray<PointType>();

            if (_points.Length <= 2)
            {
                throw new ArgumentOutOfRangeException("Minimum three points make a polygon!");
            }
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="points"></param>
        public GenericPolygonF2D(PointType[] points)
        {
            // make a copy.
            _points = new List<PointType>(points).ToArray();
            if (_points.Length <= 2)
            {
                throw new ArgumentOutOfRangeException("Minimum three points make a polygon!");
            }
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="points"></param>
        public GenericPolygonF2D(IEnumerable<PointType> points)
        {
            // make a copy.
            _points = new List<PointType>(points).ToArray();
            if (_points.Length <= 2)
            {
                throw new ArgumentOutOfRangeException("Minimum three points make a polygon!");
            }
        }

        #region Properties

        /// <summary>
        /// Returns the point at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public PointType this[int idx]
        {
            get
            {
                return _points[idx];
            }
        }

        /// <summary>
        /// Returns the number of points in this list.
        /// </summary>
        public int Count
        {
            get
            {
                return _points.Length;
            }
        }

        /// <summary>
        /// Holds the bounding box for this polygon.
        /// </summary>
        private GenericRectangleF2D<PointType> _bounding_box;

        /// <summary>
        /// Returns the bouding box around this polygon.
        /// </summary>
        public GenericRectangleF2D<PointType> BoundingBox
        {
            get
            {
                if (_bounding_box == null)
                {
                    _bounding_box = this.CreateRectangle(_points);
                }
                return _bounding_box;
            }
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the distance from the given point to this polygon.
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

        #region IsInside

        /// <summary>
        /// Returns true if the point is inside of the polygon.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsInside(PointType point)
        {
            // http://en.wikipedia.org/wiki/Even-odd_rule
            // create a line parallel to the x-axis.
            PointType second_point = this.CreatePoint(
                new double[]{point[0] + 10,point[1]});

            // intersect line with polygon.


            return false;
        }

        #endregion

        #region Intersects

        /// <summary>
        /// Returns all the intersections the line has with this polygon.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public PointType[] Intersections(GenericLineF2D<PointType> line)
        {
            List<PointType> points = new List<PointType>();
            foreach (GenericLineF2D<PointType> polygon_line in this.LineEnumerator)
            {
                // calculate the intersection.
                PrimitiveSimpleF2D primitive = line.Intersection(polygon_line);
                
                // the primitive not null.
                if (primitive != null)
                {
                    if (primitive is GenericLineF2D<PointType>)
                    { // primitive is a line; convert.                        
                        GenericLineF2D<PointType> intersect_line =
                            (primitive as GenericLineF2D<PointType>);

                        // we are sure the line is a segment.
                        // if the line is not a segment this means that the polygon contains an line with infinite length; impossible.

                        // TODO: how to determine the order?
                        points.Add(intersect_line.Point1);
                        points.Add(intersect_line.Point2);                        
                    }
                    else if (primitive is PointType)
                    { // primitive is a point; convert.
                        PointType point = (primitive as PointType);
                        points.Add(point);
                    }
                }
            }

            return points.ToArray();
        }

        #endregion

        #endregion

        #region ILineList<PointType> Members

        /// <summary>
        /// Returns the number of lines in this polygon.
        /// </summary>
        int ILineList<PointType>.Count
        {
            get 
            {
                return this.Count;
            }
        }

        /// <summary>
        /// Returns the line at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        GenericLineF2D<PointType> ILineList<PointType>.this[int idx]
        {
            get
            {
                if (idx < this.Count - 1)
                {
                    return this.CreateLine(this[idx], this[idx + 1], true);
                }
                else
                {
                    return this.CreateLine(this[idx], this[0], true);
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

        int IPointList<PointType>.Count
        {
            get { return this.Count; }
        }

        PointType IPointList<PointType>.this[int idx]
        {
            get { return this[idx]; }
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

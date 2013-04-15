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
    /// Represents a point in 2 dimensions.
    /// 
    /// A point is immutable.
    /// </summary>
    public class PointF2D : PrimitiveSimpleF2D, IPointF2D
    {
        /// <summary>
        /// The values that represents the point.
        /// </summary>
        private double[] _values;

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PointF2D(double x, double y)
        {
            _values = new double[] { x, y };
        }

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        public PointF2D(double[] values)
        {
            _values = values;

            if (_values.Length != 2)
            {
                throw new ArgumentException("Invalid # dimensions!");
            }
        }

        #region Properties

        /// <summary>
        /// Gets/Sets the value at index idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public double this[int idx]
        {
            get
            {
                return _values[idx];
            }
        }

        /// <summary>
        /// Converts to point to an array.
        /// </summary>
        /// <returns></returns>
        internal double[] ToArray()
        {
            return _values;
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the distance between this point and the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double Distance(PointF2D p)
        {
            return PointF2D.Distance(this, p);
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected static double Distance(IPointF2D a, IPointF2D b)
        {
            double distance = 0.0f;

            for (int idx = 0; idx < 2; idx++)
            {
                double idx_diff = a[idx] - b[idx];
                distance = distance + (idx_diff * idx_diff);
            }
            return (double)System.Math.Sqrt(distance);
        }

        #endregion
        
        #region Operators

        /// <summary>
        /// Substracts two points and returns the resulting vector.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static VectorF2D operator -(PointF2D a, PointF2D b)
        {
            double[] c = new double[2];

            for (int idx = 0; idx < 2; idx++)
            {
                c[idx] = a[idx] - b[idx];
            }

            return new VectorF2D(c);
        }

        /// <summary>
        /// Adds a point and a vector and returns the resulting point.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static PointF2D operator +(PointF2D a, VectorF2D b)
        {
            double[] c = new double[2];

            for (int idx = 0; idx < 2; idx++)
            {
                c[idx] = a[idx] + b[idx];
            }

            return new PointF2D(c);
        }

        #endregion

        #region Equals/GetHashCode

        /// <summary>
        /// Returns true if both objects are equal in value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is PointF2D)
            {
                return this == (obj as PointF2D);
            }
            return false;
        }

        /// <summary>
        /// Returns a unique hascode for this point.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // this is possible because a point is immutable.
            return "point".GetHashCode() ^ this[0].GetHashCode() ^ this[1].GetHashCode();
        }

        #endregion
    }
}

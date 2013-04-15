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

namespace OsmSharp.Tools.Math.Units.Angle
{
    /// <summary>
    /// Represents an angle in degress.
    /// </summary>
    public class Degree : Unit
    {
        private Degree()
            : base(0.0d)
        {

        }

        /// <summary>
        /// Creates a new angle in degrees.
        /// </summary>
        /// <param name="value"></param>
        public Degree(double value)
            :base(Degree.Normalize(value))
        {

        }

        private static double Normalize(double value)
        {
            int count_360 = (int)System.Math.Floor(value / 360.0);
            return value - (count_360 * 360.0);
        }

        #region Conversion

        /// <summary>
        /// Converts the given value to degrees.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Degree(double value)
        {
            return new Degree(value);
        }

        /// <summary>
        /// Converts the given value to degrees.
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static implicit operator Degree(Radian rad)
        {
            double value = (rad.Value / System.Math.PI) * 180d;
            return new Degree(value);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Subtracts two angles.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static Degree operator -(Degree deg1, Degree deg2)
        {
            return deg1.Value - deg2.Value;
        }

        /// <summary>
        /// Returns the absolute value of the angle.
        /// </summary>
        /// <returns></returns>
        public Degree Abs()
        {
            return System.Math.Abs(this.Value);
        }

        /// <summary>
        /// Returns true if one angle is greater than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator >(Degree deg1,Degree deg2)
        {
            return deg1.Value > deg2.Value;
        }

        /// <summary>
        /// Returns true if one angle is smaller than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator <(Degree deg1, Degree deg2)
        {
            return deg1.Value < deg2.Value;
        }

        /// <summary>
        /// Returns true if one angle is greater or equal than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator >=(Degree deg1, Degree deg2)
        {
            return deg1.Value >= deg2.Value;
        }

        /// <summary>
        /// Returns true if one angle is smaller or equal than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator <=(Degree deg1, Degree deg2)
        {
            return deg1.Value <= deg2.Value;
        }

        #endregion
    }
}

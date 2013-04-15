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
using OsmSharp.Tools.Math.Units.Distance;
using OsmSharp.Tools.Math.Units.Time;

namespace OsmSharp.Tools.Math.Units.Speed
{
    /// <summary>
    /// Represents a speed in kilometer per hours.
    /// </summary>
    public class KilometerPerHour : Unit
    {
        private KilometerPerHour()
            :base(0.0d)
        {

        }

        /// <summary>
        /// Creates a new kilometers per hour.
        /// </summary>
        /// <param name="value"></param>
        public KilometerPerHour(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts a given value to kilometers per hour.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator KilometerPerHour(double value)
        {
            return new KilometerPerHour(value);
        }

        /// <summary>
        /// Converts a given value to kilometers per hour.
        /// </summary>
        /// <param name="meter_per_sec"></param>
        /// <returns></returns>
        public static implicit operator KilometerPerHour(MeterPerSecond meter_per_sec)
        {
            return meter_per_sec.Value / 3.6d;
        }

        #endregion

        /// <summary>
        /// Returns a description of this speed.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "Km/h";
        }
    }
}

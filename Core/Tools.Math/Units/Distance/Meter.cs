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
using Tools.Math.Units.Time;
using Tools.Math.Units.Speed;

namespace Tools.Math.Units.Distance
{
    /// <summary>
    /// Represents a distance in meters.
    /// </summary>
    public class Meter : Unit
    {
        public Meter()
            :base(0.0d)
        {

        }

        private Meter(double value)
            : base(value)
        {

        }

        #region Conversions

        public static implicit operator Meter(double value)
        {
            return new Meter(value);
        }

        public static implicit operator Meter(Kilometer kilometer)
        {
            return kilometer.Value * 1000d;
        }

        #endregion
        
        #region Division

        public static MeterPerSecond operator /(Meter meter, Second sec)
        {
            return meter.Value / sec.Value;
        }

        public static Second operator /(Meter distance, MeterPerSecond speed)
        {
            return distance.Value / speed.Value;
        }

        public static Second operator /(Meter distance, KilometerPerHour speed)
        {
            Kilometer distance_km = distance;
            return distance_km / speed;
        }

        #endregion

        #region Operators

        public static Meter operator +(Meter meter1, Meter meter2)
        {
            return meter1.Value + meter2.Value;
        }

        public static Meter operator -(Meter meter1, Meter meter2)
        {
            return meter1.Value - meter2.Value;
        }

        #endregion

        public override string ToString()
        {
            return this.Value.ToString() + "m";
        }
    }
}

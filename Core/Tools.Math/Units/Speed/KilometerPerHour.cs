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
using Tools.Math.Units.Distance;
using Tools.Math.Units.Time;

namespace Tools.Math.Units.Speed
{
    public class KilometerPerHour : Unit
    {
        private KilometerPerHour()
            :base(0.0d)
        {

        }

        public KilometerPerHour(double value)
            : base(value)
        {

        }

        #region Conversions

        public static implicit operator KilometerPerHour(double value)
        {
            return new KilometerPerHour(value);
        }

        public static implicit operator KilometerPerHour(MeterPerSecond meter_per_sec)
        {
            return meter_per_sec.Value / 3.6d;
        }

        #endregion

        public override string ToString()
        {
            return this.Value.ToString() + "Km/h";
        }
    }
}

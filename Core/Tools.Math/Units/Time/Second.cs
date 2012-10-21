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
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Distance;
using Tools.Math.Units.Speed;

namespace Tools.Math.Units.Time
{
    public class Second : Unit
    {
        public Second()
            :base(0.0d)
        {

        }

        private Second(double value)
            : base(value)
        {

        }

        #region Conversions

        public static implicit operator Second(double value)
        {
            Second sec = new Second(value);
            return sec;
        }

        public static implicit operator Second(TimeSpan timespan)
        {
            Second sec = new Second();
            sec = timespan.TotalMilliseconds / 1000.0d;
            return sec;
        }

        public static implicit operator Second(Hour hour)
        {
            Second sec = new Second();
            sec = hour.Value * 3600.0d;
            return sec;
        }

        #endregion
        
        public override string ToString()
        {
            return this.Value.ToString() + "s";
        }
    }
}

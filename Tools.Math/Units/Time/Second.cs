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

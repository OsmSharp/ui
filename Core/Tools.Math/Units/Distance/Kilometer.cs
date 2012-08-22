using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Time;
using Tools.Math.Units.Speed;

namespace Tools.Math.Units.Distance
{
    /// <summary>
    /// Represents a distance in kilometers.
    /// </summary>
    public class Kilometer : Unit
    {
        public Kilometer()
            : base(0.0d)
        {

        }

        private Kilometer(double value)
            : base(value)
        {

        }

        #region Conversions

        public static implicit operator Kilometer(double value)
        {
            return new Kilometer(value);
        }

        public static implicit operator Kilometer(Meter meter)
        {
            return meter.Value / 1000d;
        }

        #endregion
        
        #region Division

        public static KilometerPerHour operator /(Kilometer kilometer, Hour hour)
        {
            return kilometer.Value / hour.Value;
        }

        public static Hour operator /(Kilometer distance, KilometerPerHour speed)
        {
            return distance.Value / speed.Value;
        }

        #endregion

        public override string ToString()
        {
            return this.Value.ToString() + "Km";
        }
    }
}
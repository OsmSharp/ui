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

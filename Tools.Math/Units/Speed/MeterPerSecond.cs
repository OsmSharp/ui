using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Distance;
using Tools.Math.Units.Time;

namespace Tools.Math.Units.Speed
{
    public class MeterPerSecond : Unit
    {
        public MeterPerSecond()
            :base(0.0d)
        {

        }
        
        private MeterPerSecond(double value)
            :base(value)
        {

        }

        #region Conversions

        public static implicit operator MeterPerSecond(double value)
        {
            MeterPerSecond sec = new MeterPerSecond(value);
            return sec;
        }

        #endregion

        public override string ToString()
        {
            return this.Value.ToString() + "m/s";
        }

    }
}

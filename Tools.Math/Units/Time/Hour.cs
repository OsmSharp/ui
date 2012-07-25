using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Units.Time
{
    public class Hour : Unit
    {
        public Hour()
            : base(0.0d)
        {

        }

        private Hour(double value)
            : base(value)
        {

        }

        #region Time-Conversions

        public static implicit operator Hour(double value)
        {
            Hour hr = new Hour(value);
            return hr;
        }

        public static implicit operator Hour(TimeSpan timespan)
        {
            Hour hr = new Hour();
            hr = timespan.TotalMilliseconds * 1000.0d * 3600.0d;
            return hr;
        }

        public static implicit operator Hour(Second sec)
        {
            Hour hr = new Hour();
            hr = sec.Value / 3600.0d;
            return hr;
        }

        #endregion

        public override string ToString()
        {
            return this.Value.ToString() + "H";
        }
    }
}

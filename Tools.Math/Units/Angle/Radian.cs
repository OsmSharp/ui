using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Units.Angle
{
    public class Radian : Unit
    {
        private Radian()
            : base(0.0d)
        {

        }

        public Radian(double radians)
            : base(radians)
        {

        }

        #region Conversion

        public static implicit operator Radian(double value)
        {
            return new Radian(value);
        }

        public static implicit operator Radian(Degree deg)
        {
            double value = (deg.Value / 180d) * System.Math.PI;
            return new Radian(value);
        }

        #endregion

        public static Radian operator -(Radian rad1, Radian rad2)
        {
            return rad1.Value - rad2.Value;
        }
    }
}

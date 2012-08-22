using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Units.Angle
{
    public class Degree : Unit
    {
        private Degree()
            : base(0.0d)
        {

        }

        public Degree(double value)
            :base(Degree.Normalize(value))
        {

        }

        private static double Normalize(double value)
        {
            int count_360 = (int)System.Math.Floor(value / 360.0);
            return value - (count_360 * 360.0);
        }

        #region Conversion

        public static implicit operator Degree(double value)
        {
            return new Degree(value);
        }

        public static implicit operator Degree(Radian rad)
        {
            double value = (rad.Value / System.Math.PI) * 180d;
            return new Degree(value);
        }

        #endregion

        #region Operators

        public static Degree operator -(Degree deg1, Degree deg2)
        {
            return deg1.Value - deg2.Value;
        }

        public Degree Abs()
        {
            return System.Math.Abs(this.Value);
        }

        public static bool operator >(Degree deg1,Degree deg2)
        {
            return deg1.Value > deg2.Value;
        }

        public static bool operator <(Degree deg1, Degree deg2)
        {
            return deg1.Value < deg2.Value;
        }

        public static bool operator >=(Degree deg1, Degree deg2)
        {
            return deg1.Value >= deg2.Value;
        }

        public static bool operator <=(Degree deg1, Degree deg2)
        {
            return deg1.Value <= deg2.Value;
        }

        #endregion
    }
}

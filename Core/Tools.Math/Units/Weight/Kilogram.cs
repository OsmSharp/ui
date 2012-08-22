using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Units.Weight
{
    public class Kilogram : Unit
    {
        public Kilogram()
            : base(0.0d)
        {

        }

        private Kilogram(double value)
            : base(value)
        {

        }

        #region Conversions

        public static implicit operator Kilogram(double value)
        {
            return new Kilogram(value);
        }

        public static implicit operator Kilogram(Gram gram)
        {
            return gram.Value / 1000d;
        }

        #endregion



        public override string ToString()
        {
            return this.Value.ToString() + "Kg";
        }

    }
}

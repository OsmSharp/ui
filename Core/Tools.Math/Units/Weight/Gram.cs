using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Units.Weight
{
    public class Gram : Unit
    {        
        public Gram()
            : base(0.0d)
        {

        }

        private Gram(double value)
            : base(value)
        {

        }

        #region Conversions

        public static implicit operator Gram(double value)
        {
            return new Gram(value);
        }

        public static implicit operator Gram(Kilogram kilogram)
        {
            return kilogram.Value * 1000d;
        }

        #endregion


        public override string ToString()
        {
            return this.Value.ToString() + "g";
        }
        
    }
}

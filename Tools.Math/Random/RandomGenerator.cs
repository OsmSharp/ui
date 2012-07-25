using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Random
{
    public class RandomGenerator : IRandomGenerator
    {
        private System.Random _random;

        public RandomGenerator()
        {
            _random = new System.Random();
        }

        #region IRandomGenerator Members

        public int Generate(int max)
        {
            return _random.Next(max);
        }

        public double Generate(double max)
        {
            return _random.NextDouble() * max;
        }

        #endregion
    }
}

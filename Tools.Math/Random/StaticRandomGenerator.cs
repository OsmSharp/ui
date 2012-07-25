using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Random
{
    public static class StaticRandomGenerator
    {
        private static IRandomGenerator _generator;

        public static IRandomGenerator Get()
        {
            if (_generator == null)
            {
                _generator = new RandomGenerator();
            }
            return _generator;
        }
    }
}

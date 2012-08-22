using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Random
{
    public interface IRandomGenerator
    {
        /// <summary>
        /// Generates a random integer
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        int Generate(int max);

        /// <summary>
        /// Generates a random double
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        double Generate(double max);
    }
}

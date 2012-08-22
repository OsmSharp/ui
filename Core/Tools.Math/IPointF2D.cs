using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math
{
    /// <summary>
    /// Interface to represents any two dimensional point.
    /// </summary>
    public interface IPointF2D
    {
        /// <summary>
        /// Returns the value at the given dimension.
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        double this[int dim]
        {
            get;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Primitives.Enumerators.Points
{
    /// <summary>
    /// Interface representing an enumerable with a list of points.
    /// </summary>
    internal interface IPointList<PointType>
        where PointType : PointF2D
    {
        /// <summary>
        /// Returns the count of points.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Returns the point at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        PointType this[int idx]
        {
            get;
        }
    }
}

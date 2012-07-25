using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Primitives.Enumerators.Lines
{
    /// <summary>
    /// Interface representing a list of lines.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    internal interface ILineList<PointType>
        where PointType : PointF2D
    {
        /// <summary>
        /// Returns the count of lines.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Returns the line at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        GenericLineF2D<PointType> this[int idx]
        {
            get;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Shapes.PrimitivesFactory
{
    /// <summary>
    /// Interface used to encapsulate the creation of primitive types for usage in shapes.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    public interface IPrimitivesFactory<PointType,RectangleType,LineType>
        where PointType : PointF2D
        where RectangleType : GenericRectangleF2D<PointType> 
        where LineType : GenericLineF2D<PointType>
    {
        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        PointType CreatePoint(double[] values);

        /// <summary>
        /// Creates a new rectangle.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        RectangleType CreateRectangle(PointType[] points);

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        LineType CreateLine(PointType point1, PointType point2);
    }
}

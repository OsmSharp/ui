using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Shapes.PrimitivesFactory;

namespace Tools.Math.Shapes.Factory
{
    public class ShapesFactory<PointType, RectangleType, LineType>
        where PointType : PointF2D
        where RectangleType : GenericRectangleF2D<PointType> 
        where LineType : GenericLineF2D<PointType>
    {
        /// <summary>
        /// Holds the primitives factory for this shapes factory.
        /// </summary>
        private IPrimitivesFactory<PointType, RectangleType, LineType> _primitives_factory;

        /// <summary>
        /// Creates a new shape factory.
        /// </summary>
        /// <param name="primitives_factory"></param>
        public ShapesFactory(IPrimitivesFactory<PointType, RectangleType, LineType> primitives_factory)
        {
            _primitives_factory = primitives_factory;
        }

        #region IPrimitivesFactory<PointType,RectangleType> Members

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public PointType CreatePoint(double[] values)
        {
            return _primitives_factory.CreatePoint(values);
        }

        /// <summary>
        /// Creates a new rectangle.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public RectangleType CreateRectangle(PointType[] points)
        {
            return _primitives_factory.CreateRectangle(points);
        }

        #endregion
    }
}

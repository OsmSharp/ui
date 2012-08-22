using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math
{
    [Serializable]
    public class LineF2D : GenericLineF2D<PointF2D>
    {
        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public LineF2D(VectorF2D v, PointF2D a)
            : base(a, a + v)
        {

        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public LineF2D(VectorF2D v, PointF2D a,bool is_segment1, bool is_segment2)
            : base(a, a + v,is_segment1,is_segment2)
        {

        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public LineF2D(PointF2D a, PointF2D b)
            :base(a,b)
        {

        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public LineF2D(PointF2D a, PointF2D b, bool is_segment1, bool is_segment2)
            : base(a, b, is_segment1, is_segment2)
        {

        }

        #region Factory

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override PointF2D CreatePoint(double[] values)
        {
            return new PointF2D(values);
        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="is_segment1"></param>
        /// <param name="is_segment2"></param>
        /// <returns></returns>
        protected override GenericLineF2D<PointF2D> CreateLine(
            PointF2D point1,
            PointF2D point2,
            bool is_segment1,
            bool is_segment2)
        {
            return new LineF2D(point1, point2, is_segment1, is_segment2);
        }

        /// <summary>
        /// Creates a new rectangle
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected override GenericRectangleF2D<PointF2D> CreateRectangle(PointF2D[] points)
        {
            return new RectangleF2D(points);
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected override GenericPolygonF2D<PointF2D> CreatePolygon(PointF2D[] points)
        {
            return new PolygonF2D(points);
        }

        #endregion      
    }
}

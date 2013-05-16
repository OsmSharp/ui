// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

namespace OsmSharp.Math.Primitives
{
    /// <summary>
    /// Class representing a retangular box.
    /// </summary>
    public class RectangleF2D : GenericRectangleF2D<PointF2D>
    {
        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public RectangleF2D(double x1, double y1, double x2, double y2)
            : base(new PointF2D(x1, y1), new PointF2D(x2, y2))
        {

        }

        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public RectangleF2D(PointF2D a, PointF2D b)
            :base(a,b)
        {

        }

        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="points"></param>
        public RectangleF2D(PointF2D[] points)
            : base(points)
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
        /// <param name="isSegment1"></param>
        /// <param name="isSegment2"></param>
        /// <returns></returns>
        protected override GenericLineF2D<PointF2D> CreateLine(
            PointF2D point1,
            PointF2D point2,
            bool isSegment1,
            bool isSegment2)
        {
            return new LineF2D(point1, point2, isSegment1, isSegment2);
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
		
		/// <summary>
		/// Calculates the intersection between this box and the given box.
		/// </summary>
		/// <param name="box">Box.</param>
		public RectangleF2D Intersection (RectangleF2D box)
		{// get the highest minimums and the lowest maximums.
			double minX = System.Math.Max(this.Min[0], box.Min[0]);
			double minY = System.Math.Max(this.Min[1], box.Min[1]);
			double maxX = System.Math.Min(this.Max[0], box.Max[0]);
			double maxY = System.Math.Min(this.Max[1], box.Max[1]);

            if (minX <= maxX && minY <= maxY)
            {
                return new RectangleF2D(new PointF2D(minX, minY), new PointF2D(maxX, maxY));
            }
		    return null;
		}

		/// <summary>
		/// Calculates the union of this box and the given box or the box that encompasses both original boxes.
		/// </summary>
		/// <param name="box">Box.</param>
		public RectangleF2D Union(RectangleF2D box)
		{// get the lowest minimums and the highest maximums.
			double minX = System.Math.Min(this.Min[0], box.Min[0]);
			double minY = System.Math.Min(this.Min[1], box.Min[1]);
			double maxX = System.Math.Max(this.Max[0], box.Max[0]);
			double maxY = System.Math.Max(this.Max[1], box.Max[1]);
			
			return new RectangleF2D(new PointF2D(minX, minY), new PointF2D(maxX, maxY));
		}

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("RectF:[({0},{1}),({2},{3})]",
                                 this.Min[0], this.Min[1], this.Max[0], this.Max[1]);
        }
    }
}

// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using System;
using OsmSharp.Units.Angle;

namespace OsmSharp.Math.Primitives
{
	/// <summary>
	/// Represents a rectangle.
	/// </summary>
	/// <remarks>This is not a Rectangle in the traditional sense, this rectangle can be tilted.</remarks>
	public class RectangleF2D : PrimitiveF2D
	{
		/// <summary>
		/// Holds the vector of the direction of the rectangle relative to the x-axis and it's size is the 'width' along this direction.
		/// </summary>
		private VectorF2D _vectorX;

		/// <summary>
		/// Holds the vector of the direction of the rectangle relative to the y-axis and it's size is the 'height' along this direction.
		/// </summary>
		/// <remarks>Both directional vectors are stored for performance reasons.</remarks>
		private VectorF2D _vectorY;

		/// <summary>
		/// Holds the bottom left point of the rectangle.
		/// </summary>
		/// <remarks>The other corners can be calculate using the directional vectors.</remarks>
		private PointF2D _bottomLeft;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Math.Primitives.RectangleF2D"/> class.
		/// </summary>
		/// <param name="x">The x coordinate of the bottom-left corner.</param>
		/// <param name="y">The y coordinate of the bottom-left corner.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <remarks>This creates a rectangle in the direction of the x- and y-axis, performance is almost always better when using <see cref="OsmSharp.Math.Primitives.BoxF2D"/> in this case.</remarks>
		public RectangleF2D(double x, double y, double width, double height){
			_bottomLeft = new PointF2D (x, y);
			_vectorX = new VectorF2D (width, 0);
			_vectorY = new VectorF2D (0, height);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Math.Primitives.RectangleF2D"/> class.
		/// </summary>
		/// <param name="x">The x coordinate of the bottom-left corner.</param>
		/// <param name="y">The y coordinate of the bottom-left corner.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="angle">The angle relative to the y-axis.</param>
		public RectangleF2D(double x, double y, double width, double height, Degree angleY){
			_bottomLeft = new PointF2D (x, y);
			VectorF2D directionY = VectorF2D.FromAngleY (angleY);
			_vectorY = directionY * height;
			_vectorX = directionY.Rotate90 (true) * width;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Math.Primitives.RectangleF2D"/> class.
		/// </summary>
		/// <param name="bottomLeft">Bottom left.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="directionY">Direction y.</param>
		public RectangleF2D(PointF2D bottomLeft, double width, double height, VectorF2D directionY){
			_bottomLeft = bottomLeft;
			VectorF2D directionYNormal = directionY.Normalize ();
			_vectorY = directionYNormal * height;
			_vectorX = directionYNormal.Rotate90 (true) * width;
		}

		/// <summary>
		/// Gets the bottom left.
		/// </summary>
		/// <value>The bottom left.</value>
		public PointF2D BottomLeft{
			get{
				return _bottomLeft;
			}
		}

		/// <summary>
		/// Gets the top left.
		/// </summary>
		/// <value>The top left.</value>
		public PointF2D TopLeft {
			get{
				return _bottomLeft + _vectorY;
			}
		}

		/// <summary>
		/// Gets the bottom right.
		/// </summary>
		/// <value>The bottom right.</value>
		public PointF2D BottomRight{
			get{
				return _bottomLeft + _vectorX;
			}
		}

		/// <summary>
		/// Sets the top right.
		/// </summary>
		/// <value>The top right.</value>
		public PointF2D TopRight {
			get{
				return _bottomLeft + _vectorX + _vectorY;
			}
		}

		#region Affine Transformations

		/// <summary>
		/// Transforms the given coordinates to the coordinate system this rectangle is defined in.
		/// </summary>
		/// <param name="width">The width of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="height">The height of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="reverseX">Assumes that the origin of the x-axis is on the top of this rectangle if false.</param>
		/// <param name="reverseY">Assumes that the origin of the y-axis is on the right of this rectangle if false.</param>
		/// <param name="coordinates">The coordinates to transform.</param>
		public double[] TransformFrom(double width, double height, bool reverseX, bool reverseY,
		                          	  double[] coordinates){
			PointF2D reference = _bottomLeft;
			VectorF2D vectorX = _vectorX;
			VectorF2D vectorY = _vectorY;

			if (reverseX && !reverseY) {
				reference = this.BottomRight;
				vectorX = _vectorX * -1;
			} else if (!reverseX && reverseY) {
				reference = this.TopLeft;
				vectorY = _vectorY * -1;
			} else if (reverseX && reverseY) {
				reference = this.TopRight;
				vectorX = _vectorX * -1;
				vectorY = _vectorY * -1;
			}

			double widthFactor = coordinates [0] / width;
			double heightFactor = coordinates [1] / height;

			PointF2D result = reference +
				(vectorX * widthFactor) +
				(vectorY * heightFactor);
			return result.ToArray ();
		}

		/// <summary>
		/// Transforms the given the coordinates to a coordinate system defined inside this rectangle.
		/// </summary>
		/// <param name="width">The width of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="height">The height of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="reverseX">Assumes that the origin of the x-axis is on the top of this rectangle if false.</param>
		/// <param name="reverseY">Assumes that the origin of the y-axis is on the right of this rectangle if false.</param>
		/// <param name="coordinates">The coordinates to transform.</param>
		public double[] TransformTo(double width, double height, bool reverseX, bool reverseY,
		                            double[] coordinates){
			PointF2D reference = _bottomLeft;
			VectorF2D vectorX = _vectorX;
			VectorF2D vectorY = _vectorY;

			if (reverseX && !reverseY) {
				reference = this.BottomRight;
				vectorX = _vectorX * -1;
			} else if (!reverseX && reverseY) {
				reference = this.TopLeft;
				vectorY = _vectorY * -1;
			} else if (reverseX && reverseY) {
				reference = this.TopRight;
				vectorX = _vectorX * -1;
				vectorY = _vectorY * -1;
			}

			PointF2D point = new PointF2D (coordinates);

			LineF2D xLine = new LineF2D (point, point + vectorX, false);
			PointF2D yIntersection = xLine.Intersection (new LineF2D (reference, reference + vectorY)) as PointF2D;
			double yFactor = (yIntersection - reference).Size / vectorY.Size;

			LineF2D yLine = new LineF2D (point, point + vectorY);
			PointF2D xIntersection = yLine.Intersection (new LineF2D (reference, reference + vectorX)) as PointF2D;
			double xFactor = (xIntersection - reference).Size / vectorX.Size;

			return new double[] { xFactor * width, yFactor * height };
		}

		#endregion

		#region implemented abstract members of PrimitiveF2D

		/// <summary>
		/// Calculates the distance of this primitive to the given point.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override double Distance (PointF2D p)
		{
			double distance = (new LineF2D (this.BottomLeft, this.BottomRight, true)).Distance (p);
			double newDistance = (new LineF2D (this.BottomRight, this.TopRight, true)).Distance (p);
			if (newDistance < distance) {
				distance = newDistance;
			}
			newDistance = (new LineF2D (this.TopRight, this.TopLeft, true)).Distance (p);
			if (newDistance < distance) {
				distance = newDistance;
			}
			newDistance = (new LineF2D (this.TopLeft, this.BottomLeft, true)).Distance (p);
			if (newDistance < distance) {
				distance = newDistance;
			}
			return distance;
		}

		#endregion
	}
}


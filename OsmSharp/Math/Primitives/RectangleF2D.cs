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
	public class RectangleF2D : PrimitiveF2D
	{
		/// <summary>
		/// Holds the points.
		/// </summary>
		private PointF2D[] _points;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Math.Primitives.RectangleF2D"/> class.
		/// </summary>
		/// <param name="leftTop">Left top.</param>
		/// <param name="rightTop">Right top.</param>
		/// <param name="bottomRight">Bottom right.</param>
		/// <param name="bottomLeft">Bottom left.</param>
		public RectangleF2D(PointF2D leftTop, PointF2D rightTop, PointF2D bottomRight, PointF2D bottomLeft)
			: base()
		{
			_points = new PointF2D[] { leftTop, rightTop, bottomRight, bottomLeft };
		}

		/// <summary>
		/// Gets the left top.
		/// </summary>
		/// <value>The left top.</value>
		public PointF2D LeftTop{
			get{
				return _points [0];
			}
		}

		/// <summary>
		/// Gets the right top.
		/// </summary>
		/// <value>The left top.</value>
		public PointF2D RightTop{
			get{
				return _points [1];
			}
		}

		/// <summary>
		/// Gets the right bottom.
		/// </summary>
		/// <value>The left top.</value>
		public PointF2D RightBottom{
			get{
				return _points [2];
			}
		}

		/// <summary>
		/// Gets the left bottom.
		/// </summary>
		/// <value>The left top.</value>
		public PointF2D LeftBottom{
			get{
				return _points [3];
			}
		}

		#region implemented abstract members of PrimitiveF2D

		/// <summary>
		/// Calculates the distance of this primitive to the given point.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override double Distance (PointF2D p)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}


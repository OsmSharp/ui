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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Units.Angle;
using OsmSharp.Math.Primitives;
using OsmSharp.Math;

namespace OsmSharp.UI.Renderer
{
	/// <summary>
	/// Represents a view on a 2D scene.
	/// </summary>
	public class View2D
	{
		/// <summary>
		/// Holds the rectangle in scene-coordinates of what the zoom represents.
		/// </summary>
		private readonly RectangleF2D _rectangle;

		/// <summary>
		/// Holds the invert X flag.
		/// </summary>
		private readonly bool _invertX;

		/// <summary>
		/// Holds the invert Y flag.
		/// </summary>
		private readonly bool _invertY;

        /// <summary>
        /// Holds the minimun x-value visible in this view.
        /// </summary>
        private readonly double _minX;

        /// <summary>
        /// Holds the maximum x-value visible in this view.
        /// </summary>
        private readonly double _maxX;

        /// <summary>
        /// Holds the minimum y-value visible in this view.
        /// </summary>
        private readonly double _minY;

        /// <summary>
        /// Holds the maximum y-value visible in this view.
        /// </summary>
        private readonly double _maxY;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.UI.Renderer.View2D"/> class.
		/// </summary>
		private View2D(RectangleF2D rectangle, bool invertX, bool invertY)
		{
			_invertX = invertX;
			_invertY = invertY;

			_rectangle = rectangle;

            var box = _rectangle.BoundingBox;
            _minX = box.Min[0];
            _minY = box.Min[1];
            _maxX = box.Max[0];
            _maxY = box.Max[1];
		}

        /// <summary>
        /// Returns the center of this view.
        /// </summary>
        public PointF2D Center {
            get
            {
                return _rectangle.Center;
            }
        }


		/// <summary>
		/// Gets the width.
		/// </summary>
		/// <value>The width.</value>
		public double Width{
			get{
				return _rectangle.Width;
			}
		}

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>The height.</value>
		public double Height{
			get{
				return _rectangle.Height;
			}
		}

		/// <summary>
		/// Gets the left top.
		/// </summary>
		/// <value>The left top.</value>
		public PointF2D LeftTop{
			get{
				if (_invertX && _invertY) {
					return _rectangle.BottomRight;
				} else if (_invertX) {
					return _rectangle.TopRight;
				} else if (_invertY) {
					return _rectangle.BottomLeft;
				}
				return _rectangle.TopLeft;
			}
		}

		/// <summary>
		/// Gets the right top.
		/// </summary>
		/// <value>The right top.</value>
		public PointF2D RightTop{
			get{
				if (_invertX && _invertY) {
					return _rectangle.BottomLeft;
				} else if (_invertX) {
					return _rectangle.TopLeft;
				} else if (_invertY) {
					return _rectangle.BottomRight;
				}
				return _rectangle.TopRight;
			}
		}

		/// <summary>
		/// Gets the left bottom.
		/// </summary>
		/// <value>The left bottom.</value>
		public PointF2D LeftBottom{
			get{
				if (_invertX && _invertY) {
					return _rectangle.TopRight;
				} else if (_invertX) {
					return _rectangle.BottomRight;
				} else if (_invertY) {
					return _rectangle.TopLeft;
				}
				return _rectangle.BottomLeft;
			}
		}

		/// <summary>
		/// Gets the right bottom.
		/// </summary>
		/// <value>The right bottom.</value>
		public PointF2D RightBottom{
			get{
				if (_invertX && _invertY) {
					return _rectangle.TopLeft;
				} else if (_invertX) {
					return _rectangle.BottomLeft;
				} else if (_invertY) {
					return _rectangle.TopRight;
				}
				return _rectangle.BottomRight;
			}
		}

		/// <summary>
		/// Gets the angle.
		/// </summary>
		/// <value>The angle.</value>
		public Degree Angle {
			get{
				return _rectangle.Angle;
			}
		}

		/// <summary>
		/// Gets the rectangle.
		/// </summary>
		/// <value>The rectangle.</value>
		public RectangleF2D Rectangle {
			get {
				return _rectangle;
			}
		}

		#region Create From

        /// <summary>
        /// Creates a new instance of the <see cref="OsmSharp.UI.Renderer.View2D"/> class.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="directionX"></param>
        /// <param name="directionY"></param>
        /// <param name="angleY"></param>
        /// <returns></returns>
        public static View2D CreateFromCenterAndSize(double width, double height, double centerX, double centerY,
            bool directionX, bool directionY)
        {
            return View2D.CreateFromCenterAndSize(width, height, centerX, centerY, directionX, directionY, 0);
        }

	    /// <summary>
	    /// Creates a new instance of the <see cref="OsmSharp.UI.Renderer.View2D"/> class.
	    /// </summary>
	    /// <param name="width">The width.</param>
	    /// <param name="height">The height.</param>
	    /// <param name="centerX">The center x.</param>
	    /// <param name="centerY">The center y.</param>
	    /// <param name="xInverted">When true x increases from left to right, when false otherwise.</param>
        /// <param name="yInverted">When true y increases from bottom to top, when false otherwise.</param>
        /// <param name="angleY"></param>
        public static View2D CreateFromCenterAndSize(double width, double height, double centerX, double centerY,
            bool xInverted, bool yInverted, Degree angleY)
		{
			if(width <= 0)
			{
				throw new ArgumentOutOfRangeException("width", "width has to be larger and not equal to zero.");
			}
			if(height <= 0)
			{
				throw new ArgumentOutOfRangeException("height", "height has to be larger and not equal to zero.");
			}

			return new View2D(RectangleF2D.FromBoundsAndCenter(width, height,
                centerX, centerY, angleY), xInverted, yInverted);
		}

		/// <summary>
		/// Creates a new instance of the <see cref="OsmSharp.UI.Renderer.View2D"/> class.
		/// </summary>
		/// <returns>The from bounds.</returns>
		/// <param name="top">Top.</param>
		/// <param name="left">Left.</param>
		/// <param name="bottom">Bottom.</param>
		/// <param name="right">Right.</param>
        public static View2D CreateFromBounds(double top, double left, double bottom, double right)
		{
			double width;
			bool xInverted;
			double centerX = (left + right) / 2.0;
			if (left > right) {
				xInverted = true;
				width = left - right;
			} else {			
				width = right - left;
				xInverted = false;
			}

			double height;
			bool yInverted;
			double centerY = (top + bottom) / 2.0;
			if(bottom > top){
				yInverted = true;
				height = bottom - top;
			}
			else {
				yInverted = false;
				height = top - bottom;
			}
			return View2D.CreateFromCenterAndSize(width, height, centerX, centerY,
			                                      xInverted, yInverted);
		}

	    /// <summary>
	    /// Creates a view based on a center location a zoomfactor and the size of the current viewport.
	    /// </summary>
	    /// <param name="centerX"></param>
	    /// <param name="centerY"></param>
	    /// <param name="pixelsWidth"></param>
	    /// <param name="pixelsHeight"></param>
	    /// <param name="zoomFactor"></param>
	    /// <param name="xInverted"></param>
        /// <param name="yInverted"></param>
        /// <param name="angleY"></param>
	    /// <returns></returns>
        public static View2D CreateFrom(double centerX, double centerY, double pixelsWidth, double pixelsHeight,
            double zoomFactor, bool xInverted, bool yInverted)
        {
            return View2D.CreateFrom(centerX, centerY, pixelsWidth, pixelsHeight, zoomFactor, xInverted, yInverted, 0);
        }

	    /// <summary>
	    /// Creates a view based on a center location a zoomfactor and the size of the current viewport.
	    /// </summary>
	    /// <param name="centerX"></param>
	    /// <param name="centerY"></param>
	    /// <param name="pixelsWidth"></param>
	    /// <param name="pixelsHeight"></param>
	    /// <param name="zoomFactor"></param>
	    /// <param name="xInverted"></param>
        /// <param name="yInverted"></param>
        /// <param name="angleY"></param>
	    /// <returns></returns>
        public static View2D CreateFrom(double centerX, double centerY, double pixelsWidth, double pixelsHeight,
            double zoomFactor, bool xInverted, bool yInverted, Degree angleY)
        {
            double realZoom = zoomFactor;

            double width = pixelsWidth / realZoom;
            double height = pixelsHeight / realZoom;

            return View2D.CreateFromCenterAndSize(width, height, centerX, centerY, xInverted, yInverted, angleY);
        }

		#endregion

		/// <summary>
		/// Returns true if the given coordinates are inside this view.
		/// </summary>
		/// <returns><c>true</c> if this instance is in the specified x y; otherwise, <c>false</c>.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public bool Contains (double x, double y)
		{
			if (_rectangle.BoundingBox.Contains (x, y)) {
				return _rectangle.Contains (x, y);
			}
			return false;
		}

        /// <summary>
        /// Returns true if an object with the given coordinates is visible with this view.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="closed"></param>
        /// <returns></returns>
        public bool IsVisible(double[] x, double[] y, bool closed)
        {
            double MinX = double.MaxValue;
            double MaxX = double.MinValue;
            for (int idx = 0; idx < x.Length; idx++)
            {
                if (x[idx] > MaxX)
                {
                    MaxX = x[idx];
                }
                if (x[idx] < MinX)
                {
                    MinX = x[idx];
                }
            }
            double MinY = double.MaxValue;
            double MaxY = double.MinValue;
            for (int idx = 0; idx < y.Length; idx++)
            {
                if (y[idx] > MaxY)
                {
                    MaxY = y[idx];
                }
                if (y[idx] < MinY)
                {
                    MinY = y[idx];
                }
            }
            return this.OverlapsWithBox(MinX, MinY, MaxX, MaxY);
        }

		/// <summary>
		/// Returns true if the given rectangle overlaps with this view.
		/// </summary>
		/// <returns><c>true</c>, if with rectangle overlaps, <c>false</c> otherwise.</returns>
		/// <param name="left">Left.</param>
		/// <param name="top">Top.</param>
		/// <param name="right">Right.</param>
		/// <param name="bottom">Bottom.</param>
		public bool OverlapsWithBox(double left, double top, double right, double bottom)
		{
            double minX = System.Math.Max(_minX, System.Math.Min(left, right));
            double minY = System.Math.Max(_minY, System.Math.Min(top, bottom));
            double maxX = System.Math.Min(_maxX, System.Math.Max(left, right));
            double maxY = System.Math.Min(_maxY, System.Math.Max(top, bottom));

            if (minX <= maxX && minY <= maxY)
            {
                return true;
            }
            return false;
		}

        /// <summary>
        /// Creates a transformation matrix that transforms coordinates from the view port to scene-coordinates.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <returns></returns>
        public Matrix2D CreateFromViewPort(double pixelsWidth, double pixelsHeight)
        {
            // from viewport means to rectangle.
            return Matrix2D.ToRectangle(_rectangle, pixelsWidth, pixelsHeight, _invertX, _invertY);
        }

        /// <summary>
        /// Returns the coordinates represented by the given pixel in the given viewport.
        /// </summary>
        /// <param name="pixelX"></param>
        /// <param name="pixelY"></param>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <returns></returns>
        public double[] FromViewPort(double pixelsWidth, double pixelsHeight, double pixelX, double pixelY)
        { // assumed that the coordinate system of the viewport starts at (0,0) in the topleft corner. 
            // return _rectangle.TransformFrom(pixelsWidth, pixelsHeight, _invertX, _invertY, pixelX, pixelY);
            double x, y;
            var fromViewport = this.CreateFromViewPort(pixelsWidth, pixelsHeight);
            fromViewport.Apply(pixelX, pixelY, out x, out y);
            return new double[] { x, y };
        }

        /// <summary>
        /// Froms the view port.
        /// </summary>
        /// <returns>The view port.</returns>
        /// <param name="pixelsWidth">Pixels width.</param>
        /// <param name="pixelsHeight">Pixels height.</param>
        /// <param name="pixelsX">Pixels x.</param>
        /// <param name="pixelsY">Pixels y.</param>
        public double[][] FromViewPort(double pixelsWidth, double pixelsHeight, double[] pixelsX, double[] pixelsY)
        {
            //return _rectangle.TransformFrom(pixelsWidth, pixelsHeight, _invertX, _invertY, pixelsX, pixelsY);
            double[][] result = new double[pixelsX.Length][];
            var fromView = this.CreateFromViewPort(pixelsWidth, pixelsHeight);
            for (int idx = 0; idx < pixelsX.Length; idx++)
            {
                double x, y;
                fromView.Apply(pixelsX[idx], pixelsY[idx], out x, out y);
                result[idx] = new double[] { x, y };
            }
            return result;
        }

        /// <summary>
        /// Creates a transformation matrix that transforms coordinates from the scene to the given viewport.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <returns></returns>
        public Matrix2D CreateToViewPort(double pixelsWidth, double pixelsHeight)
        {
            // to viewport means from rectangle.
            return Matrix2D.FromRectangle(_rectangle, pixelsWidth, pixelsHeight, _invertX, _invertY);
        }

        /// <summary>
        /// Returns the viewport coordinates in the given viewport that corresponds with the given scene coordinates.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <param name="sceneX"></param>
        /// <param name="sceneY"></param>
        /// <returns></returns>
        public double[][] ToViewPort(double pixelsWidth, double pixelsHeight, double[] sceneX, double[] sceneY)
        { // the right and going down.
            //return _rectangle.TransformTo(pixelsWidth, pixelsHeight, _invertX, _invertY, sceneX, sceneY);
            double[][] result = new double[sceneX.Length][];
            var toView = this.CreateToViewPort(pixelsWidth, pixelsHeight);
            for (int idx = 0; idx < sceneX.Length; idx++)
            {
                double x, y;
                toView.Apply(sceneX[idx], sceneY[idx], out x, out y);
                result[idx] = new double[] { x, y };
            }
            return result;
        }

        /// <summary>
        /// Returns the viewport coordinates in the given viewport that corresponds with the given scene coordinates.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <param name="sceneX"></param>
        /// <param name="sceneY"></param>
        /// <returns></returns>
        public double[] ToViewPort(double pixelsWidth, double pixelsHeight, double sceneX, double sceneY)
        { // the right and going down.
            //return _rectangle.TransformTo(pixelsWidth, pixelsHeight, _invertX, _invertY, sceneX, sceneY);
            double x, y;
            var toViewport = this.CreateToViewPort(pixelsWidth, pixelsHeight);
            toViewport.Apply(sceneX, sceneY, out x, out y);
            return new double[] { x, y };
        }

        /// <summary>
        /// Returns the viewport coordinates in the given viewport that corresponds with the given scene coordinates.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <param name="sceneX"></param>
        /// <param name="sceneY"></param>
        /// <returns></returns>
        public void ToViewPort(double pixelsWidth, double pixelsHeight, double sceneX, double sceneY, double[] transformed)
        { // the right and going down.
            double x, y;
            this.CreateToViewPort(pixelsWidth, pixelsHeight).Apply(sceneX, sceneY, out x, out y);
            transformed[0] = x;
            transformed[1] = y;
        }

        /// <summary>
        /// Calculates the zoom factor for the given view when at the given resolution.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <returns></returns>
        public double CalculateZoom(double pixelsWidth, double pixelsHeight)
        {
            double realZoom = pixelsWidth / _rectangle.Width;
            return realZoom;
        }

		/// <summary>
		/// Rotates this view around it's center with a given angle and returns the modified version.
		/// </summary>
		/// <returns>The around center.</returns>
		/// <param name="angle">Angle.</param>
		public View2D RotateAroundCenter(Radian angle) {
			RectangleF2D rotated = this.Rectangle.RotateAroundCenter (angle);
			return new View2D (rotated, _invertX, _invertY);
		}

        /// <summary>
        /// Fites this view around the given points but keeps aspect ratio and 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public View2D Fit(PointF2D[] points)
        {
            return this.Fit(points, 0);
        }

        /// <summary>
        /// Fites this view around the given points but keeps aspect ratio and 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public View2D Fit(PointF2D[] points, double percentage)
        {
            RectangleF2D rotated = this.Rectangle.FitAndKeepAspectRatio(points, percentage);
            return new View2D(rotated, _invertX, _invertY);
        }

		/// <summary>
		/// Returns the smallest rectangular box containing the entire view. Will be larger when turned in a non-zero direction.
		/// </summary>
		/// <value>The outer box.</value>
		public BoxF2D OuterBox
		{
			get{
				return _rectangle.BoundingBox;
			}
		}

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="OsmSharp.UI.Renderer.View2D"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="OsmSharp.UI.Renderer.View2D"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="OsmSharp.UI.Renderer.View2D"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            View2D view = obj as View2D;
            if (view != null)
            {
                return view.Rectangle.Equals(
                    this.Rectangle);
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="OsmSharp.UI.Renderer.View2D"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return "View2D".GetHashCode() ^
                this.Rectangle.GetHashCode();
        }
    }
}
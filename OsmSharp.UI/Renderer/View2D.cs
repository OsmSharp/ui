// OsmSharp - OpenStreetMap tools & library.
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

namespace OsmSharp.UI.Renderer
{
	/// <summary>
	/// Represents a view on a 2D scene.
	/// </summary>
	/// <remarks>This view has nothing to do with the control/canvas/whatever being rendered on. It is only related to the 2D scene object
	/// and represents just a location and size of the viewing window on the current scene.
	/// The x-coordinates increase when moving right, the y-coordinates increase when moving up.
	/// </remarks>
	public class View2D
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.UI.Renderer.View2D"/> class.
		/// </summary>
		private View2D()
		{
			this.Direction = 0;
		}

		#region Create From

	    /// <summary>
	    /// Creates a new instance of the <see cref="OsmSharp.UI.Renderer.View2D"/> class.
	    /// </summary>
	    /// <param name="width">The width.</param>
	    /// <param name="height">The height.</param>
	    /// <param name="centerX">The center x.</param>
	    /// <param name="centerY">The center y.</param>
	    /// <param name="directionX"></param>
	    /// <param name="directionY"></param>
        public static View2D CreateFromCenterAndSize(double width, double height, double centerX, double centerY,
            bool directionX, bool directionY)
		{
			var view = new View2D();

			if(width <= 0)
			{
				throw new ArgumentOutOfRangeException("width", "width has to be larger and not equal to zero.");
			}
			if(height <= 0)
			{
				throw new ArgumentOutOfRangeException("height", "height has to be larger and not equal to zero.");
			}
			
			view.Width = width;
			view.Height = height;
			view.CenterX = centerX;
			view.CenterY = centerY;
			
			// calculate the derived properties.
            double halfX = view.Width / 2.0f;
            double halfY = view.Height / 2.0f;
			double left, right, bottom, top;
	        if (directionX)
	        {
	            left = view.CenterX - halfX;
	            right = view.CenterX + halfX;
	        }
	        else
            {
                left = view.CenterX + halfX;
                right = view.CenterX - halfX;
	        }

            if (directionY)
            {
                top = view.CenterY + halfY;
                bottom = view.CenterY - halfY;
            }
            else
            {
               	top = view.CenterY - halfY;
                bottom = view.CenterY + halfY;
            }

			view.LeftTop = new double[] { left, top };
			view.RightTop = new double[] { right, top };
			view.LeftBottom = new double[] { left, bottom };
			view.RightBottom = new double[] { right, bottom };

			return view;
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
			var view = new View2D();
			
			view.LeftTop = new double[] { left, top };
			view.RightTop = new double[] { right, top };
			view.LeftBottom = new double[] { left, bottom };
			view.RightBottom = new double[] { right, bottom };

			// calculate the derived properties.
			view.CenterX = (left + right) / 2.0;
			view.CenterY = (bottom + top) / 2.0;
			view.Height = System.Math.Abs(top - bottom);
            view.Width = System.Math.Abs(left - right);
			
			return view;
		}

	    /// <summary>
	    /// Creates a view based on a center location a zoomfactor and the size of the current viewport.
	    /// </summary>
	    /// <param name="centerX"></param>
	    /// <param name="centerY"></param>
	    /// <param name="pixelsWidth"></param>
	    /// <param name="pixelsHeight"></param>
	    /// <param name="zoomFactor"></param>
	    /// <param name="directionX"></param>
	    /// <param name="directionY"></param>
	    /// <returns></returns>
        public static View2D CreateFrom(double centerX, double centerY, double pixelsWidth, double pixelsHeight,
            double zoomFactor, bool directionX, bool directionY)
        {
            double realZoom = zoomFactor;

            double width = pixelsWidth / realZoom;
            double height = pixelsHeight / realZoom;

            return View2D.CreateFromCenterAndSize(width, height, centerX, centerY, directionX, directionY);
        }

		#endregion

		/// <summary>
		/// Gets the width.
		/// </summary>
		/// <value>The width.</value>
        public double Width
        {
			get;
			private set;
		}

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>The height.</value>
        public double Height
        {
			get;
			private set;
		}

		/// <summary>
		/// Gets the center x.
		/// </summary>
		/// <value>The center x.</value>
        public double CenterX
        {
			get;
			private set;
		}

		/// <summary>
		/// Gets the center y.
		/// </summary>
		/// <value>The center y.</value>
        public double CenterY
        {
			get;
			private set;
		}

		/// <summary>
		/// Gets the direction relative to the north.
		/// </summary>
		/// <value>The angle.</value>
		public Degree Direction {
			get;
			private set;
		}


		/// <summary>
		/// Gets the top left corner relative to the direction.
		/// </summary>
		/// <value>The top left.</value>
		public double[] LeftTop {
			get;
			private set;
		}

		/// <summary>
		/// Gets the top right corner relative to the direction.
		/// </summary>
		/// <value>The top right.</value>
		public double[] RightTop {
			get;
			private set;
		}

		/// <summary>
		/// Gets the bottom left corner relative to the direction.
		/// </summary>
		/// <value>The bottom left.</value>
		public double[] LeftBottom {
			get;
			private set;
		}

		/// <summary>
		/// Gets the bottom right corner relative to the direction.
		/// </summary>
		/// <value>The bottom right.</value>
		public double[] RightBottom {
			get;
			set;
		}
		
		/// <summary>
		/// Returns true if the given coordinates are inside this view.
		/// </summary>
		/// <returns><c>true</c> if this instance is in the specified x y; otherwise, <c>false</c>.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public bool Contains (double x, double y)
		{
			if (this.Direction == null || this.Direction.Value == 0) {
				double left = this.LeftTop [0];
				double right = this.RightTop [0];
				double top = this.LeftTop [1];
				double bottom = this.LeftBottom [1];

				if ((top > bottom && (y > top || y < bottom)) ||
					(top < bottom && (y < top || y > bottom)) ||
					(left < right && (x < left || x > right)) ||
					(left > right && (x > left || x < right))) { // one of the bounds was violated.
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns true if the given rectangle overlaps with this view.
		/// </summary>
		/// <returns><c>true</c>, if with rectangle overlaps, <c>false</c> otherwise.</returns>
		/// <param name="left">Left.</param>
		/// <param name="top">Top.</param>
		/// <param name="right">Right.</param>
		/// <param name="bottom">Bottom.</param>
		public bool OverlapsWithRectangle(double left, double top, double right, double bottom)
		{
			throw new NotSupportedException ();
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
        { // assumed that the coordinate system of the viewport starts at (0,0) in the topleft corner and increases to 
            // the right and going down.
            double scaleX = pixelsWidth / this.Width;
            double scaleY = pixelsHeight / this.Height;

            double offsetXScene = pixelX / scaleX;
            double offsetYScene = pixelY / scaleY;

			if (this.Direction == null || this.Direction.Value == 0) {
				double left = this.LeftTop [0];
				double right = this.RightTop [0];
				double top = this.LeftTop [1];
				double bottom = this.LeftBottom [1];
				double x;
				if (left < right) {
					x = (left + offsetXScene);
				} else {
					x = (left - offsetXScene);
				}

				double y;
				if (top > bottom) {
					y = (top - offsetYScene);
				} else {
					y = (top + offsetYScene);
				}

				return new double[] { x, y };
			}
			throw new NotSupportedException ("Direction not supported yet!");
        }

        /// <summary>
        /// Returns the scene x-coordinates represents by the given x-coordinate for the given viewport width.
        /// </summary>
        /// <param name="pixelX"></param>
        /// <param name="pixelsWidth"></param>
        /// <returns></returns>
        public double FromViewPortX(double pixelsWidth, double pixelX)
        {
            // the right and going down.
            double scaleX = pixelsWidth / this.Width;

            double offsetXScene = pixelX / scaleX;
			
			if (this.Direction == null || this.Direction.Value == 0) {
				double left = this.LeftTop [0];
				double right = this.RightTop [0];
				double x;
				if (left < right) {
					x = (left + offsetXScene);
				} else {
					x = (left - offsetXScene);
				}
				return x;
			}
			throw new NotSupportedException ("Direction not supported yet!");
        }

        /// <summary>
        /// Returns the scene y-coordinates represents by the given y-coordinate for the given viewport height.
        /// </summary>
        /// <param name="pixelY"></param>
        /// <param name="pixelsHeight"></param>
        /// <returns></returns>
        public double FromViewPortY(double pixelsHeight, double pixelY)
        {
            double scaleY = pixelsHeight / this.Height;

            double offsetYScene = pixelY / scaleY;
			
			if (this.Direction == null || this.Direction.Value == 0) {
				double top = this.LeftTop [1];
				double bottom = this.LeftBottom [1];
				double y;
				if (top > bottom) {
					y = (top - offsetYScene);
				} else {
					y = (top + offsetYScene);
				}

				return y;
			}
			throw new NotSupportedException ("Direction not supported yet!");
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
            double scaleX = pixelsWidth / this.Width;
            double scaleY = pixelsHeight / this.Height;

			if (this.Direction == null || this.Direction.Value == 0) {
				double left = this.LeftTop [0];
				double right = this.RightTop [0];
				double top = this.LeftTop [1];
				double bottom = this.LeftBottom [1];

				double pixelsX;
				if (left < right) {
					pixelsX = (sceneX - left) * scaleX;
				} else { // left < right
					pixelsX = (this.Width - (sceneX - right)) * scaleX;
				}

				double pixelsY;
				if (bottom < top) {
					pixelsY = (this.Height - (sceneY - bottom)) * scaleY;
				} else { // left < right
					pixelsY = ((sceneY - top)) * scaleY;
				}

				return new double[] { pixelsX, pixelsY };
			}
			throw new NotSupportedException ("Direction not supported yet!");
        }

        /// <summary>
        /// Returns the viewport x-coordinate for the given viewport width that corresponds with the given scene x-coordinate.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="sceneX"></param>
        /// <returns></returns>
        public double ToViewPortX(double pixelsWidth, double sceneX)
        {
            double scaleX = pixelsWidth / this.Width;

			if (this.Direction == null || this.Direction.Value == 0) {
				double left = this.LeftTop [0];
				double right = this.RightTop [0];

				double pixelsX;
				if (left < right) {
					pixelsX = (sceneX - left) * scaleX;
				} else { // left < right
					pixelsX = (this.Width - (sceneX - right)) * scaleX;
				}

				return pixelsX;
			}
			throw new NotSupportedException ("Direction not supported yet!");
        }

        /// <summary>
        /// Returns the viewport y-coordinate for the given viewport height that corresponds with the given scene y-coordinate.
        /// </summary>
        /// <param name="pixelsHeight"></param>
        /// <param name="sceneY"></param>
        /// <returns></returns>
        public double ToViewPortY(double pixelsHeight, double sceneY)
        {
            double scaleY = pixelsHeight / this.Height;

			if (this.Direction == null || this.Direction.Value == 0) {
				double top = this.LeftTop [1];
				double bottom = this.LeftBottom [1];

				double pixelsY;
				if (bottom < top) {
					pixelsY = (this.Height - (sceneY - bottom)) * scaleY;
				} else { // left < right
					pixelsY = ((sceneY - top)) * scaleY;
				}

				return pixelsY;
			}
			throw new NotSupportedException ("Direction not supported yet!");
        }

        /// <summary>
        /// Calculates the zoom factor for the given view when at the given resolution.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <returns></returns>
        public double CalculateZoom(double pixelsWidth, double pixelsHeight)
        {
            double realZoom = pixelsWidth / this.Width;
            return realZoom;
        }

		/// <summary>
		/// Returns the smallest rectangular box containing the entire view. Will be larger when turned in a non-zero direction.
		/// </summary>
		/// <value>The outer box.</value>
		public RectangleF2D OuterBox
		{
			get{
				double left = this.LeftTop [0];
				double right = this.RightTop [0];
				double top = this.LeftTop [1];
				double bottom = this.LeftBottom [1];

				return new RectangleF2D (left, top, right, bottom);
			}
		}
    }
}
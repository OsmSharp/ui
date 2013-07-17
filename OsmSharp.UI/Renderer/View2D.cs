
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	        if (directionX)
	        {
	            view.Left = view.CenterX - halfX;
	            view.Right = view.CenterX + halfX;
	        }
	        else
            {
                view.Left = view.CenterX + halfX;
                view.Right = view.CenterX - halfX;
	        }

            if (directionY)
            {
                view.Top = view.CenterY + halfY;
                view.Bottom = view.CenterY - halfY;
            }
            else
            {
                view.Top = view.CenterY - halfY;
                view.Bottom = view.CenterY + halfY;
            }

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
			
			view.Top = top;
			view.Bottom = bottom;
			view.Left = left;
			view.Right = right;
			
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
		/// Gets the bottom.
		/// </summary>
		/// <value>The bottom.</value>
        public double Bottom
        {
			get;
			private set;
		}

		/// <summary>
		/// Gets the top.
		/// </summary>
		/// <value>The top.</value>
        public double Top
        {
			get;
			private set;
		}

		/// <summary>
		/// Gets the left.
		/// </summary>
		/// <value>The left.</value>
        public double Left
        {
			get;
			private set;
		}

		/// <summary>
		/// Gets the right.
		/// </summary>
		/// <value>The right.</value>
        public double Right
        {
			get;
			private set;
		}
		
		/// <summary>
		/// Returns true if the given coordinates are inside this view.
		/// </summary>
		/// <returns><c>true</c> if this instance is in the specified x y; otherwise, <c>false</c>.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public bool Contains (double x, double y)
		{
			if((this.Top > this.Bottom && (y > this.Top || y < this.Bottom)) ||
                (this.Top < this.Bottom && (y < this.Top || y > this.Bottom)) ||
                (this.Left < this.Right && (x < this.Left || x > this.Right)) ||
                (this.Left > this.Right && (x > this.Left || x < this.Right)))
			{ // one of the bounds was violated.
				return false;
			}
			return true;
		}

        /// <summary>
        /// Returns the coordinates represents by the given pixel in the given viewport.
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

            double x;
            if (this.Left < this.Right)
            {
                x = (this.Left + offsetXScene);
            }
            else
            {
                x = (this.Left - offsetXScene);
            }

            double y;
            if (this.Top > this.Bottom)
            {
                y = (this.Top - offsetYScene);
            }
            else
            {
                y =  (this.Top + offsetYScene);
            }

            return new double[]{ x ,y  };
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

            double x;
            if (this.Left < this.Right)
            {
                x = (this.Left + offsetXScene);
            }
            else
            {
                x = (this.Left - offsetXScene);
            }
            return x;
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

            double y;
            if (this.Top > this.Bottom)
            {
                y = (this.Top - offsetYScene);
            }
            else
            {
                y = (this.Top + offsetYScene);
            }

            return y;
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

            double pixelsX;
            if (this.Left < this.Right)
            {
                pixelsX = (sceneX - this.Left) * scaleX;
            }
            else
            { // left < right
                pixelsX = (this.Width - (sceneX - this.Right)) * scaleX;
            }

            double pixelsY;
            if (this.Bottom < this.Top)
            {
                pixelsY = (this.Height - (sceneY - this.Bottom)) * scaleY;
            }
            else
            { // left < right
                pixelsY = ((sceneY - this.Top)) * scaleY;
            }

            return new double[] { pixelsX, pixelsY };
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

            double pixelsX;
            if (this.Left < this.Right)
            {
                pixelsX = (sceneX - this.Left) * scaleX;
            }
            else
            { // left < right
                pixelsX = (this.Width - (sceneX - this.Right)) * scaleX;
            }

            return pixelsX;
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

            double pixelsY;
            if (this.Bottom < this.Top)
            {
                pixelsY = (this.Height - (sceneY - this.Bottom)) * scaleY;
            }
            else
            { // left < right
                pixelsY = ((sceneY - this.Top)) * scaleY;
            }

            return pixelsY;
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
    }
}
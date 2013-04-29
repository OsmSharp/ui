
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
		public static View2D CreateFromCenterAndSize(float width, float height, float centerX, float centerY)
		{
			View2D view = new View2D();

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
			float halfX = view.Width / 2.0f;
			float halfY = view.Height / 2.0f;
			view.Left = view.CenterX - halfX;
			view.Right = view.CenterX + halfX;
			view.Top = view.CenterY + halfY;
			view.Bottom = view.CenterY - halfY;

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
		public static View2D CreateFromBounds(float top, float left, float bottom, float right)
		{
			var view = new View2D();

			if(top <= bottom)
			{
				throw new ArgumentException("top smaller than bottom!");
			}
			if(right <= left)
			{
				throw new ArgumentException("right smaller than left!");
			}
			
			view.Top = top;
			view.Bottom = bottom;
			view.Left = left;
			view.Right = right;
			
			// calculate the derived properties.
			view.CenterX = (left + right) / 2.0f;
			view.CenterY = (bottom + top) / 2.0f;
			view.Height = (top - bottom);
			view.Width = (left - right);
			
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
        /// <returns></returns>
        public static View2D CreateFrom(float centerX, float centerY, float pixelsWidth, float pixelsHeight, float zoomFactor)
        {
            float realZoom = (float)System.Math.Pow(2, zoomFactor);

            float width = pixelsWidth / realZoom;
            float height = pixelsHeight / realZoom;

            return View2D.CreateFromCenterAndSize(width, height, centerX, centerY);
        }

		#endregion


		/// <summary>
		/// Gets the width.
		/// </summary>
		/// <value>The width.</value>
		public float Width {
			get;
			private set;
		}

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>The height.</value>
		public float Height {
			get;
			private set;
		}

		/// <summary>
		/// Gets the center x.
		/// </summary>
		/// <value>The center x.</value>
		public float CenterX {
			get;
			private set;
		}

		/// <summary>
		/// Gets the center y.
		/// </summary>
		/// <value>The center y.</value>
		public float CenterY {
			get;
			private set;
		}

		/// <summary>
		/// Gets the bottom.
		/// </summary>
		/// <value>The bottom.</value>
		public float Bottom {
			get;
			private set;
		}

		/// <summary>
		/// Gets the top.
		/// </summary>
		/// <value>The top.</value>
		public float Top {
			get;
			private set;
		}

		/// <summary>
		/// Gets the left.
		/// </summary>
		/// <value>The left.</value>
		public float Left {
			get;
			private set;
		}

		/// <summary>
		/// Gets the right.
		/// </summary>
		/// <value>The right.</value>
		public float Right {
			get;
			private set;
		}
		
		/// <summary>
		/// Returns true if the given coordinates are inside this view.
		/// </summary>
		/// <returns><c>true</c> if this instance is in the specified x y; otherwise, <c>false</c>.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public bool IsIn (float x, float y)
		{
			if(y > this.Top || y < this.Bottom || 
			   x < this.Left || x > this.Right)
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
        public float[] ToViewPort(float pixelsWidth, float pixelsHeight, float pixelX, float pixelY)
        { // assumed that the coordinate system of the viewport starts at (0,0) in the topleft corner and increases to 
            // the right and going down.
            //pixelX = pixelX - (pixelsWidth / 2.0f);
            //pixelY = pixelY - (pixelsHeight / 2.0f);

            float scaleX = pixelsWidth / this.Width;
            float scaleY = pixelsHeight / this.Height;

            float offsetXScene = pixelX / scaleX;
            float offsetYScene = pixelY / scaleY;

            return new float[]{  (this.Left + offsetXScene), (this.Top - offsetYScene) };
        }
    }
}
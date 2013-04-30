
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives
{
	/// <summary>
	/// A simple 2D polygon.
	/// </summary>
	internal struct Polygon2D : IScene2DPrimitive
	{
        /// <summary>
        /// Creates a new line2D.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="fill"></param>
        public Polygon2D(float[] x, float[] y, int color, float width, bool fill)
           : this()
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.Fill = fill;

            _minX = int.MaxValue;
            _maxX = int.MinValue;
            for (int idx = 0; idx < x.Length; idx++)
            {
                if (x[idx] > _maxX)
                {
                    _maxX = x[idx];
                }
                if (x[idx] < _minX)
                {
                    _minX = x[idx];
                }
            }
            _minY = int.MaxValue;
            _maxY = int.MinValue;
            for (int idx = 0; idx < y.Length; idx++)
            {
                if (y[idx] > _maxY)
                {
                    _maxY = y[idx];
                }
                if (y[idx] < _minY)
                {
                    _minY = y[idx];
                }
            }
        }

        /// <summary>
        /// Creates a new line2D.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="fill"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        public Polygon2D(float[] x, float[] y, int color, float width, bool fill,
            int minX, int maxX, int minY, int maxY)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.Fill = fill;

            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

		/// <summary>
		/// Gets or sets the x.
		/// </summary>
		/// <value>The x.</value>
		public float[] X {
			get;
			private set;
		}
		
		/// <summary>
		/// Gets or sets the y.
		/// </summary>
		/// <value>The y.</value>
		public float[] Y {
			get;
			private set;
		}
		
		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public int Color {
			get;
			private set;
		}
		
		/// <summary>
		/// Gets or sets the size.
		/// </summary>
		/// <value>The size.</value>
		public float Width {
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="OsmSharp.UI.Renderer.Scene2DPrimitives.Polygon2D"/> is to be filled.
		/// </summary>
		/// <value><c>true</c> if fill; otherwise, <c>false</c>.</value>
		public bool Fill {
			get;
			private set;
		}

		#region IScene2DPrimitive implementation

	    private readonly float _minX;
        private readonly float _maxX;
        private readonly float _minY;
        private readonly float _maxY;
		
		/// <summary>
		/// Returns true if the object is visible on the view.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="view">View.</param>
		public bool IsVisibleIn (View2D view)
		{
            if (view.Contains(_minX, _minY) ||
                view.Contains(_minX, _maxY) ||
                view.Contains(_maxX, _minY) ||
                view.Contains(_maxX, _maxY))
            {
                return true;
            }
            if (_minX < view.Left && view.Left < _maxX)
            {
                if (_minY < view.Top && view.Top < _maxY)
                {
                    return true;
                }
                else if (_minY < view.Bottom && view.Bottom < _maxY)
                {
                    return true;
                }
            }
            else if (_minX < view.Right && view.Right < _maxX)
            {
                if (_minY < view.Top && view.Top < _maxY)
                {
                    return true;
                }
                else if (_minY < view.Bottom && view.Bottom < _maxY)
                {
                    return true;
                }
            }
			return false;
		}
		
		#endregion
	}
}
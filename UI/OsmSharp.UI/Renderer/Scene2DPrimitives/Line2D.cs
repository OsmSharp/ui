
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives
{
	/// <summary>
	/// A simple 2D line.
	/// </summary>
	internal struct Line2D : IScene2DPrimitive
	{
        /// <summary>
        /// Creates a new line2D.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="lineJoin"></param>
		public Line2D(double[] x, double[] y, int color, float width, LineJoin lineJoin, int[] dashes)
           : this()
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.LineJoin = lineJoin;
            this.Dashes = dashes;

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

            this.MinZoom = float.MinValue;
            this.MaxZoom = float.MaxValue;
        }

	    /// <summary>
	    /// Creates a new line2D.
	    /// </summary>
	    /// <param name="x"></param>
	    /// <param name="y"></param>
	    /// <param name="color"></param>
	    /// <param name="width"></param>
	    /// <param name="lineJoin"></param>
	    /// <param name="dashes"></param>
	    /// <param name="minZoom"></param>
	    /// <param name="maxZoom"></param>
		public Line2D(double[] x, double[] y, int color, double width, LineJoin lineJoin, int[] dashes, float minZoom, float maxZoom)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.LineJoin = lineJoin;
            this.Dashes = dashes;

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

            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
        }

        /// <summary>
        /// Creates a new line2D.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="lineJoin"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
		public Line2D(double[] x, double[] y, int color, double width, LineJoin lineJoin, int[] dashes,
            int minX, int maxX, int minY, int maxY)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.LineJoin = lineJoin;
            this.Dashes = dashes;

            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;

            this.MinZoom = float.MinValue;
            this.MaxZoom = float.MaxValue;
		}
		
		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		public object Tag {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the x.
		/// </summary>
		/// <value>The x.</value>
		public double[] X {
			get;
			private set;
		}
		
		/// <summary>
		/// Gets or sets the y.
		/// </summary>
		/// <value>The y.</value>
		public double[] Y {
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
		public double Width {
			get;
			private set;
		}

        /// <summary>
        /// Gets or sets the linejoin.
        /// </summary>
	    public LineJoin LineJoin { get; private set; }

        /// <summary>
        /// Gets or sets the line dashses.
        /// </summary>
        public int[] Dashes { get; private set; }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        public float MaxZoom { get; set; }

		#region IScene2DPrimitive implementation

		private readonly double _minX;
		private readonly double _maxX;
		private readonly double _minY;
		private readonly double _maxY;

	    /// <summary>
	    /// Returns true if the object is visible on the view.
	    /// </summary>
	    /// <returns>true</returns>
	    /// <c>false</c>
	    /// <param name="view">View.</param>
	    /// <param name="zoom"></param>
	    public bool IsVisibleIn (View2D view, float zoom)
        {
            if (this.MinZoom > zoom || this.MaxZoom < zoom)
            { // outside of zoom bounds!
                return false;
            }

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


        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
        public RectangleF2D GetBox()
        {
            return new RectangleF2D(_minX, _minY, _maxX, _maxY);
        }
		
		#endregion
	}

    /// <summary>
    /// Enumerated the different linejoin options.
    /// </summary>
    public enum LineJoin
    {
        Round,
        Miter,
        Bevel,
        None
    }
}
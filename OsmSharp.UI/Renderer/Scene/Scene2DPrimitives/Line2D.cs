
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UI.Renderer.Scene.Scene2DPrimitives
{
	/// <summary>
	/// A simple 2D line.
	/// </summary>
	public class Line2D : IScene2DPrimitive
	{
        /// <summary>
        /// Creates a new line2D.
        /// </summary>
        public Line2D()
        {
            
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
        public Line2D(double[] x, double[] y, int color, float width, LineJoin lineJoin, int[] dashes, float casingWidth, int casingColor)
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.LineJoin = lineJoin;
            this.Dashes = dashes;
            this.CasingColor = casingColor;
            this.CasingWidth = casingWidth;

            MinX = int.MaxValue;
            MaxX = int.MinValue;
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
            MinY = int.MaxValue;
            MaxY = int.MinValue;
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
        public Line2D(double[] x, double[] y, int color, double width, LineJoin lineJoin, int[] dashes, float casingWidth, int casingColor, 
            float minZoom, float maxZoom)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.LineJoin = lineJoin;
            this.Dashes = dashes;
            this.CasingColor = casingColor;
            this.CasingWidth = casingWidth;

            MinX = int.MaxValue;
            MaxX = int.MinValue;
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
            MinY = int.MaxValue;
            MaxY = int.MinValue;
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
        public Line2D(double[] x, double[] y, int color, double width, LineJoin lineJoin, int[] dashes, float casingWidth, int casingColor,
            int minX, int maxX, int minY, int maxY)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.LineJoin = lineJoin;
            this.Dashes = dashes;
            this.CasingColor = casingColor;
            this.CasingWidth = casingWidth;

            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;

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
			set;
		}
		
		/// <summary>
		/// Gets or sets the y.
		/// </summary>
		/// <value>The y.</value>
		public double[] Y {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public int Color {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the size.
		/// </summary>
		/// <value>The size.</value>
		public double Width {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the linejoin.
        /// </summary>
	    public LineJoin LineJoin { get; set; }

        /// <summary>
        /// Gets or sets the line dashses.
        /// </summary>
        public int[] Dashes { get; set; }

        /// <summary>
        /// Gets or sets the casing width.
        /// </summary>
        public float CasingWidth { get; set; }

        /// <summary>
        /// Gets or sets the casing color.
        /// </summary>
        public int CasingColor { get; set; }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        public float MaxZoom { get; set; }

		#region IScene2DPrimitive implementation

        internal double MinX  { get; set; }
        internal double MaxX { get; set; }
        internal double MinY { get; set; }
        internal double MaxY { get; set; }

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

            if (view.Contains(MinX, MinY) ||
                view.Contains(MinX, MaxY) ||
                view.Contains(MaxX, MinY) ||
                view.Contains(MaxX, MaxY))
            {
                return true;
            }
            if (MinX < view.Left && view.Left < MaxX)
            {
                if (MinY < view.Top && view.Top < MaxY)
                {
                    return true;
                }
                else if (MinY < view.Bottom && view.Bottom < MaxY)
                {
                    return true;
                }
            }
            else if (MinX < view.Right && view.Right < MaxX)
            {
                if (MinY < view.Top && view.Top < MaxY)
                {
                    return true;
                }
                else if (MinY < view.Bottom && view.Bottom < MaxY)
                {
                    return true;
                }
			}
			return this.GetBox().Overlaps(new RectangleF2D(view.Left, view.Top, view.Right, view.Bottom));
		}

        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
        public RectangleF2D GetBox()
        {
            return new RectangleF2D(MinX, MinY, MaxX, MaxY);
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
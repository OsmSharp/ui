
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UI.Renderer.Scene.Scene2DPrimitives
{
	/// <summary>
	/// A simple 2D polygon.
	/// </summary>
	public class Polygon2D : IScene2DPrimitive
	{
        /// <summary>
        /// Creates a new Polygon2D.
        /// </summary>
        public Polygon2D()
        {
            
        }

        /// <summary>
        /// Creates a new Polygon2D.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="fill"></param>
		public Polygon2D(double[] x, double[] y, int color, double width, bool fill)
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.Fill = fill;

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
        /// Creates a new Polygon2D.
	    /// </summary>
	    /// <param name="x"></param>
	    /// <param name="y"></param>
	    /// <param name="color"></param>
	    /// <param name="width"></param>
	    /// <param name="fill"></param>
	    /// <param name="minZoom"></param>
	    /// <param name="maxZoom"></param>
		public Polygon2D(double[] x, double[] y, int color, double width, bool fill, float minZoom, float maxZoom)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.Fill = fill;

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
        /// Creates a new Polygon2D.
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
		public Polygon2D(double[] x, double[] y, int color, double width, bool fill,
            int minX, int maxX, int minY, int maxY)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.Fill = fill;

            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;

            this.MinZoom = float.MinValue;
            this.MaxZoom = float.MaxValue;
		}

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public uint Id { get; set; }
		
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
		/// Gets or sets a value indicating whether this <see cref="OsmSharp.UI.Renderer.Scene2DPrimitives.Polygon2D"/> is to be filled.
		/// </summary>
		/// <value><c>true</c> if fill; otherwise, <c>false</c>.</value>
		public bool Fill {
			get;
			set;
        }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        public float MaxZoom { get; set; }

		#region IScene2DPrimitive implementation

        internal double MinX { get; set; }
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

			if (view.OverlapsWithBox (MinX, MinY, MaxX, MaxY)) {
				return true; // maybe a better hittest?
			}
			return false;
		}

        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
		public BoxF2D GetBox()
        {
			return new BoxF2D(MinX, MinY, MaxX, MaxY);
        }
		
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives
{	
	/// <summary>
	/// A simple 2D point.
	/// </summary>
	internal class Point2D : IScene2DPrimitive
	{
        /// <summary>
        /// Creates a new point 2D.
        /// </summary>
        public Point2D()
        {
            
        }

        /// <summary>
        /// Creates a new point 2D.
        /// </summary>
		public Point2D(double x, double y, int color, double size)
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Size = size;

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
		public double X {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the y.
		/// </summary>
		/// <value>The y.</value>
		public double Y {
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
		public double Size {
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

            return view.Contains(this.X, this.Y);
		}

        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
        public RectangleF2D GetBox()
        {
            return new RectangleF2D(this.X, this.Y, this.X, this.Y);
        }

		#endregion
	}
}


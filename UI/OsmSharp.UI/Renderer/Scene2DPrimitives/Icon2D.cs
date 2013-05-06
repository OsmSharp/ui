using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives
{
    /// <summary>
    /// A simple icon.
    /// </summary>
    internal struct Icon2D : IScene2DPrimitive
    {
        /// <summary>
        /// Creates a new icon.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="image"></param>
        public Icon2D(float x, float y, byte[] image)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Image = image;

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
        public float X { get; private set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        public float Y { get; private set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public byte[] Image { get; private set; }

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
        public bool IsVisibleIn(View2D view, float zoom)
        {
            if (this.MinZoom > zoom || this.MaxZoom < zoom)
            { // outside of zoom bounds!
                return false;
            }

            return view.Contains(this.X, this.Y);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives
{
    /// <summary>
    /// Represents a simple 2D image.
    /// </summary>
    public class Image2D : IScene2DPrimitive
    {
        /// <summary>
        /// Creates a new Image2D.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <param name="imageData"></param>
        public Image2D(float left, float top, float bottom, float right, byte[] imageData)
        {
            this.ImageData = imageData;
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
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
        /// Gets the image data.
        /// </summary>
        public byte[] ImageData { get; set; }

        /// <summary>
        /// Gets the left.
        /// </summary>
        public float Left { get; set; }

        /// <summary>
        /// Gets the right.
        /// </summary>
        public float Right { get; set; }

        /// <summary>
        /// Gets the top.
        /// </summary>
        public float Top { get; set; }

        /// <summary>
        /// Gets the bottom.
        /// </summary>
        public float Bottom { get; set; }

        #region IScene2DPrimitive implementation

        /// <summary>
        /// Returns true if the object is visible on the view.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        /// <param name="view">View.</param>
        public bool IsVisibleIn(View2D view)
        {
            if (view.Contains(this.Left, this.Bottom) ||
                view.Contains(this.Left, this.Top) ||
                view.Contains(this.Right, this.Bottom) ||
                view.Contains(this.Right, this.Top))
            {
                return true;
            }
            if (this.Left < view.Left && view.Left < this.Right)
            {
                if (this.Bottom <= view.Top && view.Top <= this.Top)
                {
                    return true;
                }
                else if (this.Bottom <= view.Bottom && view.Bottom <= this.Top)
                {
                    return true;
                }
            }
            else if (this.Left <= view.Right && view.Right <= this.Right)
            {
                if (this.Bottom <= view.Top && view.Top <= this.Top)
                {
                    return true;
                }
                else if (this.Bottom <= view.Bottom && view.Bottom <= this.Top)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}

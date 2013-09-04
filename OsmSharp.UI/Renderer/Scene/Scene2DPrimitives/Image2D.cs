using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UI.Renderer.Scene.Scene2DPrimitives
{
    /// <summary>
    /// Represents a simple 2D image.
    /// </summary>
    public class Image2D : IScene2DPrimitive
    {
        /// <summary>
        /// Creates a new Image2D.
        /// </summary>
        public Image2D()
        {
            
        }

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

            this.MinZoom = float.MinValue;
            this.MaxZoom = float.MaxValue;
        }

        /// <summary>
        /// Creates a new Image2D.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <param name="imageData"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
		public Image2D(double left, double top, double bottom, double right, byte[] imageData, float minZoom, float maxZoom)
        {
            this.ImageData = imageData;
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;

            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
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
        /// Gets the image data.
        /// </summary>
        public byte[] ImageData { get; set; }

        /// <summary>
        /// Gets the left.
        /// </summary>
		public double Left { get; set; }

        /// <summary>
        /// Gets the right.
        /// </summary>
		public double Right { get; set; }

        /// <summary>
        /// Gets the top.
        /// </summary>
		public double Top { get; set; }

        /// <summary>
        /// Gets the bottom.
        /// </summary>
		public double Bottom { get; set; }

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

			return view.OverlapsWithBox (this.Left, this.Top, this.Right, this.Bottom);
        }

        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
		public BoxF2D GetBox()
        {
			return new BoxF2D(this.Left, this.Top, this.Right, this.Bottom);
        }

        #endregion
    }
}

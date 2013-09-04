using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UI.Renderer.Scene.Scene2DPrimitives
{
    /// <summary>
    /// A simple text.
    /// </summary>
    internal class Text2D : IScene2DPrimitive
    {
        /// <summary>
        /// Creates a new Text.
        /// </summary>
        public Text2D()
        {
            
        }

        /// <summary>
        /// Creates a new icon.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="size"></param>
        public Text2D(float x, float y, string text, int color, float size)
        {
            this.X = x;
            this.Y = y;
            this.Text = text;
            this.Size = size;
            this.Color = color;

            this.MinZoom = float.MinValue;
            this.MaxZoom = float.MaxValue;
        }

        /// <summary>
        /// Creates a new icon.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        public Text2D(double x, double y, string text, int color, double size, int? haloColor, int? haloRadius, string font,
		              float minZoom, float maxZoom)
        {
            this.X = x;
            this.Y = y;
            this.Text = text;
            this.Size = size;
            this.Color = color;
            this.HaloColor = haloColor;
            this.HaloRadius = haloRadius;
			this.Font = font;

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
        public object Tag { get; set; }

        /// <summary>
        /// Gets/sets the color.
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
		public double X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
		public double Y { get; set; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the halo size.
        /// </summary>
        public int? HaloRadius { get; set; }

        /// <summary>
        /// Gets or sets the halo color.
        /// </summary>
        public int? HaloColor { get; set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
		public double Size { get; set; }

		/// <summary>
		/// Gets the font name.
		/// </summary>
		/// <value>The font.</value>
		public string Font { get; set; }

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

        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
		public BoxF2D GetBox()
        {
			return new BoxF2D(this.X, this.Y, this.X, this.Y);
        }

        #endregion
    }
}
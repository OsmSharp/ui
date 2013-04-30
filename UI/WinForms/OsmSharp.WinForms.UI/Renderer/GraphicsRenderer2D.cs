using System;
using System.Drawing;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene2DPrimitives;

namespace OsmSharp.WinForms.UI.Renderer
{
	/// <summary>
	/// Graphics renderer 2D.
	/// </summary>
    public class GraphicsRenderer2D : Renderer2D<Graphics>
	{
        /// <summary>
        /// Holds the width.
        /// </summary>
	    private float _width;

        /// <summary>
        /// Holds the height.
        /// </summary>
        private float _height;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsRenderer2D"/> class.
        /// </summary>
        /// <param name="graphics">Graphics.</param>
        public GraphicsRenderer2D(Graphics graphics)
            : base(graphics)
        {
            _height = graphics.VisibleClipBounds.Height;
            _width = graphics.VisibleClipBounds.Width;
        }

		#region implemented abstract members of Renderer2D

        /// <summary>
        /// Gets the width.
        /// </summary>
        public override float Width { get { return _width; }}

	    /// <summary>
	    /// Gets the height.
	    /// </summary>
	    public override float Height { get { return _height; }}

	    /// <summary>
		/// Transforms the target using the specified view.
		/// </summary>
		/// <param name="view">View.</param>
		protected override void Transform (View2D view)
        {
            float scaleX = this.Width / view.Width;
            float scaleY = this.Height / view.Height;

            // scale and translate.
            this.Target.ResetTransform();
            this.Target.ScaleTransform(scaleX, scaleY);
            this.Target.TranslateTransform((-view.CenterX + (view.Width / 2.0f)),
                                  (view.CenterY + (view.Height / 2.0f)));
		}

        /// <summary>
        /// Returns the size in pixels.
        /// </summary>
        /// <returns>The pixels.</returns>
        /// <param name="view">View.</param>
        /// <param name="sizeInPixels">Size in pixels.</param>
        protected override float FromPixels(View2D view, float sizeInPixels)
        {
            float scaleX = this.Width / view.Width;

            return sizeInPixels / scaleX;
        }

        /// <summary>
        /// Draws the backcolor.
        /// </summary>
        /// <param name="backColor"></param>
        protected override void DrawBackColor(int backColor)
        {
            this.Target.Clear(Color.FromArgb(backColor));
        }

		/// <summary>
		/// Draws a point on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		protected override void DrawPoint (float x, float y, int color, float size)
		{
            this.Target.FillEllipse(new SolidBrush(Color.FromArgb(color)), x, -y, size, size);
		}

		/// <summary>
		/// Draws a line on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		protected override void DrawLine (float[] x, float[] y, int color, float width, LineJoin lineJoin)
		{
		    var pen = new Pen(Color.FromArgb(color), width);
		    switch (lineJoin)
		    {
		        case LineJoin.Round:
                    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
		            break;
		        case LineJoin.Miter:
                    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
		            break;
		        case LineJoin.Bevel:
		            pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
		            break;
		        case LineJoin.None:
                    // just keep the default.
		            break;
		        default:
		            throw new ArgumentOutOfRangeException("lineJoin");
		    }
            var points = new PointF[x.Length];
		    for (int idx = 0; idx < x.Length; idx++)
		    {
                points[idx] = new PointF(x[idx], -y[idx]);
		    }
		    this.Target.DrawLines(pen, points);
		}

		/// <summary>
		/// Draws a polygon on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		/// <param name="fill">If set to <c>true</c> fill.</param>
		protected override void DrawPolygon (float[] x, float[] y, int color, float width, bool fill)
        {
            var points = new PointF[x.Length];
            for (int idx = 0; idx < x.Length; idx++)
            {
                points[idx] = new PointF(x[idx], -y[idx]);
            }
            if (fill)
            {
                var pen = new Pen(Color.FromArgb(color), width);
                this.Target.FillPolygon(new SolidBrush(Color.FromArgb(color)), points);
            }
            else
            {
                var pen = new Pen(Color.FromArgb(color), width);
                this.Target.DrawPolygon(pen, points);
            }
		}

		#endregion
	}
}
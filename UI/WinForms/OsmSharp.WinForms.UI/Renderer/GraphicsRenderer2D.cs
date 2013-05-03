using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using OsmSharp.UI.Renderer;
using LineJoin = OsmSharp.UI.Renderer.Scene2DPrimitives.LineJoin;

namespace OsmSharp.WinForms.UI.Renderer
{
	/// <summary>
	/// Graphics renderer 2D.
	/// </summary>
    public class GraphicsRenderer2D : Renderer2D<Graphics>
	{
        #region Caching Implementation

        /// <summary>
        /// Called right before rendering starts.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="scene"></param>
        /// <param name="view"></param>
        protected override void OnBeforeRender(Target2DWrapper<Graphics> target, Scene2D scene, View2D view)
        {
            // create a bitmap and render there.
            var bitmap = new Bitmap((int)target.Width, (int)target.Height);
            target.BackTarget = target.Target;
            target.Target = Graphics.FromImage(bitmap);
            target.Target.CompositingMode = target.BackTarget.CompositingMode;
            target.Target.SmoothingMode = target.BackTarget.SmoothingMode;
            target.Target.PixelOffsetMode = target.BackTarget.PixelOffsetMode;
            target.Target.InterpolationMode = target.BackTarget.InterpolationMode;

            target.Tag = bitmap;
        }

        /// <summary>
        /// Called right after rendering.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="scene"></param>
        /// <param name="view"></param>
        protected override void OnAfterRender(Target2DWrapper<Graphics> target, Scene2D scene, View2D view)
        {
            target.Target.Flush();
            target.Target = target.BackTarget;
            var bitmap = target.Tag as Bitmap;
            if (bitmap != null)
            {
                target.Target.DrawImageUnscaled(bitmap, 0, 0);
            }
        }

	    /// <summary>
	    /// Builds the cached scene.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="currentCache"></param>
	    /// <param name="currentScene"></param>
	    /// <param name="view"></param>
	    /// <returns></returns>
        protected override Scene2D BuildSceneCache(Target2DWrapper<Graphics> target, Scene2D currentCache, Scene2D currentScene, View2D view)
        {
            var scene = new Scene2D();
	        scene.BackColor = currentScene.BackColor;

	        var bitmap = target.Tag as Bitmap;
	        if (bitmap != null)
	        {
	            byte[] imageData = null;
	            using (var stream = new MemoryStream())
	            {
	                bitmap.Save(stream, ImageFormat.Png);
	                stream.Close();

	                imageData = stream.ToArray();
	            }
	            scene.AddImage(0, view.Left, view.Top, view.Right, view.Bottom, imageData);
	        }
	        return scene;
        }

        #endregion

        #region implemented abstract members of Renderer2D

        /// <summary>
        /// Creates a wrapper for the given target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override Target2DWrapper<Graphics> CreateTarget2DWrapper(Graphics target)
        {
            return new Target2DWrapper<Graphics>(target,target.VisibleClipBounds.Width,
                target.VisibleClipBounds.Height);
        }

	    private View2D _view;

	    /// <summary>
	    /// Transforms the target using the specified view.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="view">View.</param>
        protected override void Transform(Target2DWrapper<Graphics> target, View2D view)
	    {
	        _view = view;

            float scaleX = target.Width / view.Width;
            float scaleY = target.Height / view.Height;

            // scale and translate.
            target.Target.ResetTransform();
            target.Target.ScaleTransform(scaleX, scaleY);
            target.Target.TranslateTransform((-view.CenterX + (view.Width / 2.0f)),
                                  (view.CenterY + (view.Height / 2.0f)));
		}

        /// <summary>
        /// Returns the size in pixels.
        /// </summary>
        /// <returns>The pixels.</returns>
        /// <param name="view">View.</param>
        /// <param name="sizeInPixels">Size in pixels.</param>
        protected override float FromPixels(Target2DWrapper<Graphics> target, View2D view, float sizeInPixels)
        {
            float scaleX = target.Width / view.Width;

            return sizeInPixels / scaleX;
        }

        /// <summary>
        /// Draws the backcolor.
        /// </summary>
        /// <param name="backColor"></param>
        protected override void DrawBackColor(Target2DWrapper<Graphics> target, int backColor)
        {
            target.Target.Clear(Color.FromArgb(backColor));
        }

		/// <summary>
		/// Draws a point on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
        protected override void DrawPoint(Target2DWrapper<Graphics> target, float x, float y, int color, float size)
		{
            target.Target.FillEllipse(new SolidBrush(Color.FromArgb(color)), x, -y, size, size);
        }

		/// <summary>
		/// Draws a line on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
        protected override void DrawLine(Target2DWrapper<Graphics> target, float[] x, float[] y, int color, float width, LineJoin lineJoin, int[] dashes)
		{
		    var pen = new Pen(Color.FromArgb(color), width);
            if (dashes != null)
            {
                float[] penDashes = new float[dashes.Length];
                for (int idx = 0; idx < dashes.Length; idx++)
                {
                    penDashes[idx] = dashes[idx];
                }
                pen.DashPattern = penDashes;
                pen.DashStyle = DashStyle.Custom;
            }
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
            pen.StartCap = LineCap.Square;
            pen.EndCap = LineCap.Square;
		    var points = new PointF[x.Length];
		    for (int idx = 0; idx < x.Length; idx++)
		    {
                points[idx] = new PointF(x[idx], -y[idx]);
		    }
            target.Target.DrawLines(pen, points);
		}

		/// <summary>
		/// Draws a polygon on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		/// <param name="fill">If set to <c>true</c> fill.</param>
        protected override void DrawPolygon(Target2DWrapper<Graphics> target, float[] x, float[] y, int color, float width, bool fill)
        {
            var points = new PointF[x.Length];
            for (int idx = 0; idx < x.Length; idx++)
            {
                points[idx] = new PointF(x[idx], -y[idx]);
            }
            if (fill)
            {
                var pen = new Pen(Color.FromArgb(color), width);
                target.Target.FillPolygon(new SolidBrush(Color.FromArgb(color)), points);
            }
            else
            {
                var pen = new Pen(Color.FromArgb(color), width);
                target.Target.DrawPolygon(pen, points);
            }
		}

        /// <summary>
        /// Draws an icon unscaled but at the given scene coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="imageData"></param>
        protected override void DrawIcon(Target2DWrapper<Graphics> target, float x, float y, byte[] imageData)
        {
            // get the image.
            Image image = Image.FromStream(new MemoryStream(imageData));

            // draw the image.
            target.Target.DrawImage(image, x, -y);
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="imageData"></param>
        protected override object DrawImage(Target2DWrapper<Graphics> target, float left, float top, float right, float bottom, byte[] imageData, object tag)
        {
            // get the image.
            var image = (tag as Image);
            if (image == null)
            {
                image = Image.FromStream(new MemoryStream(imageData));
            }

            //target.Target.DrawImage(image, left, -top);
            target.Target.DrawImage(image, new RectangleF(left, -top, right - left, top - bottom));
            return image;
        }

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="size"></param>
        protected override void DrawText(Target2DWrapper<Graphics> target, float x, float y, string text, float size)
        {
            target.Target.DrawString(text, new Font(FontFamily.GenericSansSerif, size), new SolidBrush(Color.Black), x, -y);
        }

		#endregion
	}
}
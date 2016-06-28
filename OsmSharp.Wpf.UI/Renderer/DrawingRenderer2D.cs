using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.Wpf.UI.Renderer.Images;

namespace OsmSharp.Wpf.UI.Renderer
{
    /// <summary>
    /// RenderContext renderer 2D.
    /// </summary>
    public class DrawingRenderer2D : Renderer2D<RenderContext>
	{
        #region Caching Implementation

        /// <summary>
        /// Called right before rendering starts.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="view"></param>
        protected override void OnBeforeRender(Target2DWrapper<RenderContext> target, View2D view)
        {
            target.OpenRender();
        }

        /// <summary>
        /// Called right after rendering.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="view"></param>
        protected override void OnAfterRender(Target2DWrapper<RenderContext> target, View2D view)
        {
            target.CloseRender();
        }

        #endregion

        #region implemented abstract members of Renderer2D

        /// <summary>
        /// Creates a wrapper for the given target.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override Target2DWrapper<RenderContext> CreateTarget2DWrapper(RenderContext target)
        {
            return new Target2DWrapper<RenderContext>(target, (float)target.RenderSize.Width, (float)target.RenderSize.Height);
        }

        /// <summary>
        /// Keeps the view.
        /// </summary>
	    private View2D _view;

        /// <summary>
        /// Holds the target.
        /// </summary>
        private Target2DWrapper<RenderContext> _target;

        /// <summary>
        /// Holds the to-view transformation matrix.
        /// </summary>
        private Matrix2D _toView;

	    /// <summary>
	    /// Transforms the target using the specified view.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="view">View.</param>
        protected override void Transform(Target2DWrapper<RenderContext> target, View2D view)
	    {
	        _view = view;
	        _target = target;

            _toView = _view.CreateToViewPort(target.Width, target.Height);
	    }

        /// <summary>
        /// Transforms the y-coordinate to screen coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="offsetX">offset by x</param>
        /// <param name="offsetY">offset by y</param>
        /// <returns></returns>
        private Point Tranform(double x, double y, double offsetX = 0, double offsetY = 0)
        {
            double newX, newY;
            _toView.Apply(x, y, out newX, out newY);
            return new Point(newX + offsetX, newY + offsetY);
        }

        /// <summary>
        /// Returns the size in pixels.
        /// </summary>
        /// <returns></returns>
        private double ToPixels(double sceneSize)
        {
            var scaleX = _target.Width / _view.Width;

            return sceneSize * scaleX;
        }

	    /// <summary>
	    /// Returns the size in pixels.
	    /// </summary>
	    /// <returns>The pixels.</returns>
	    /// <param name="target"></param>
	    /// <param name="view">View.</param>
	    /// <param name="sizeInPixels">Size in pixels.</param>
	    protected override double FromPixels(Target2DWrapper<RenderContext> target, View2D view, double sizeInPixels)
        {
            var scaleX = target.Width / view.Width;

            return sizeInPixels / scaleX;
        }

	    /// <summary>
	    /// Draws the backcolor.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="backColor"></param>
	    protected override void DrawBackColor(Target2DWrapper<RenderContext> target, int backColor)
	    {
	        target.Render().DrawRectangle(backColor.ToBrush(), null, target.Target.RenderRect);
        }

	    /// <summary>
	    /// Draws a point on the target. The coordinates given are scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="size">Size.</param>
	    protected override void DrawPoint(Target2DWrapper<RenderContext> target, double x, double y, int color, double size)
	    {
            size = ToPixels(size);
            var center = Tranform(x, y, -size / 2.0d, -size / 2.0d);
            target.Render().DrawPoint(center, size, size, color.ToColor());
        }

	    /// <summary>
	    /// Draws a line on the target. The coordinates given are scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="width">Width.</param>
	    /// <param name="lineJoin"></param>
	    /// <param name="dashes"></param>
	    protected override void DrawLine(Target2DWrapper<RenderContext> target, double[] x, double[] y, int color, double width, 
            LineJoin lineJoin, int[] dashes)
	    {
	        width = ToPixels(width);
		    var points = new Point[x.Length];
		    for (var idx = 0; idx < x.Length; idx++)
		    {
                points[idx] = Tranform(x[idx], y[idx]);
		    }
            target.Render().DrawLine(points, width, color.ToColor(), PenLineCap.Round, PenLineCap.Round,
                 lineJoin.ToPenLineJoin(), dashes.ToDashStyle(0));
        }

	    /// <summary>
	    /// Draws a polygon on the target. The coordinates given are scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="width">Width.</param>
	    /// <param name="fill">If set to <c>true</c> fill.</param>
	    protected override void DrawPolygon(Target2DWrapper<RenderContext> target, double[] x, double[] y, int color,
            double width, bool fill)
        {
            width = ToPixels(width);
            var points = new Point[x.Length];
            for (var idx = 0; idx < x.Length; idx++)
            {
                points[idx] = Tranform(x[idx], y[idx]);
            }
            target.Render().DrawPolygon(points, width, color.ToColor(), fill);
        }

	    /// <summary>
	    /// Draws an icon unscaled but at the given scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x"></param>
	    /// <param name="y"></param>
	    /// <param name="imageData"></param>
	    protected override void DrawIcon(Target2DWrapper<RenderContext> target, double x, double y, byte[] imageData)
        {
            using (var stream = new MemoryStream(imageData))
            {
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.StreamSource = stream;
                imageSource.EndInit();

                var leftTop = Tranform(x, y);
                target.Render().DrawImage(leftTop, imageSource);
            }
        }

	    /// <summary>
	    /// Draws an image.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="left"></param>
	    /// <param name="top"></param>
	    /// <param name="right"></param>
	    /// <param name="bottom"></param>
        /// <param name="nativeImage"></param>
	    protected override void DrawImage(Target2DWrapper<RenderContext> target, double left, double top, double right, 
            double bottom, INativeImage nativeImage)
	    {
	        var wpfNativeImage = nativeImage as NativeImage;
	        if (wpfNativeImage != null)
	        {
                var leftTop = Tranform(left, top);
                var rightBottom = Tranform(right, bottom);
                var width = rightBottom.X - leftTop.X;
                var height = rightBottom.Y - leftTop.Y;

                var destRect = new Rect(leftTop, new Size(width, height));
                target.Render().DrawImage(destRect, wpfNativeImage.Image);
            }
	    }

		/// <summary>
		/// Draws the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="target">Target.</param>
		/// <param name="bounds">Bounds.</param>
        /// <param name="nativeImage">Image data.</param>
        protected override void DrawImage(Target2DWrapper<RenderContext> target, RectangleF2D bounds, INativeImage nativeImage)
		{
		    DrawImage(target, bounds.TopLeft[0], bounds.TopLeft[1], bounds.BottomRight[0], bounds.BottomRight[1], nativeImage);
		}

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="haloColor"></param>
        /// <param name="haloRadius"></param>
        /// <param name="fontName"></param>
        protected override void DrawText(Target2DWrapper<RenderContext> target, double x, double y, string text, int color, double size,
            int? haloColor, int? haloRadius, string fontName)
        {
            size = ToPixels(size);
            var point = Tranform(x, y);
            target.Render().DrawText(text, point, new Typeface(fontName), size, color.ToColor(), haloColor.ToColor(), haloRadius);
        }

        /// <summary>
        /// Draws text along a line.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="haloColor"></param>
        /// <param name="haloRadius"></param>
        /// <param name="fontName"></param>
        protected override void DrawLineText(Target2DWrapper<RenderContext> target, double[] x, double[] y, string text, int color, 
            double size, int? haloColor, int? haloRadius, string fontName)
        {
            if (x.Length > 1)
            {
                size = ToPixels(size);
                var points = new Point[x.Length];
                for (var idx = 0; idx < x.Length; idx++)
                {
                    points[idx] = Tranform(x[idx], y[idx]);
                }
                target.Render().DrawTextLine(text, points, new Typeface(fontName), size, color.ToColor(), haloColor.ToColor(), haloRadius);
            }
        }

        #endregion
    }

    //TODO fix dashes, fix image line
}
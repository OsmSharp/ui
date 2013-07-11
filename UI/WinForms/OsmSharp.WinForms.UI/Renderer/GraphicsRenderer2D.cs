using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OsmSharp.UI.Renderer;
using LineJoin = OsmSharp.UI.Renderer.Scene2DPrimitives.LineJoin;
using OsmSharp.Math.Primitives;
using OsmSharp.Math;
using OsmSharp.Math.Units.Angle;

namespace OsmSharp.WinForms.UI.Renderer
{
	/// <summary>
	/// Graphics renderer 2D.
	/// </summary>
    public class GraphicsRenderer2D : Renderer2D<Graphics>
	{
	    private Pen _pen = new Pen(Color.Black);

        //private _pen = new Pen(Color.FromArgb(color), widthInPixels);

        #region Caching Implementation

        /// <summary>
        /// Called right before rendering starts.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="scenes"></param>
        /// <param name="view"></param>
        protected override void OnBeforeRender(Target2DWrapper<Graphics> target, List<Scene2D> scenes, View2D view)
        {
            // create a bitmap and render there.
            var bitmap = new Bitmap((int)target.Width, (int)target.Height);
            target.BackTarget = target.Target;
            target.BackTarget.CompositingMode = CompositingMode.SourceOver;
            target.Target = Graphics.FromImage(bitmap);
            target.Target.CompositingMode = CompositingMode.SourceOver;
            target.Target.SmoothingMode = target.BackTarget.SmoothingMode;
            target.Target.PixelOffsetMode = target.BackTarget.PixelOffsetMode;
            target.Target.InterpolationMode = target.BackTarget.InterpolationMode;

            target.Tag = bitmap;
        }

        /// <summary>
        /// Called right after rendering.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="scenes"></param>
        /// <param name="view"></param>
        protected override void OnAfterRender(Target2DWrapper<Graphics> target, List<Scene2D> scenes, View2D view)
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
	    /// <param name="currentScenes"></param>
	    /// <param name="view"></param>
	    /// <returns></returns>
        protected override Scene2D BuildSceneCache(Target2DWrapper<Graphics> target, Scene2D currentCache, 
            List<Scene2D> currentScenes, View2D view)
        {
            var scene = new Scene2D();
            scene.BackColor = currentScenes[0].BackColor;

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
	            scene.AddImage(0, float.MinValue, float.MaxValue, 
                    view.Left, view.Top, view.Right, view.Bottom, imageData);
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

        /// <summary>
        /// Keeps the view.
        /// </summary>
	    private View2D _view;

        /// <summary>
        /// Holds the target.
        /// </summary>
        private Target2DWrapper<Graphics> _target;

	    /// <summary>
	    /// Transforms the target using the specified view.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="view">View.</param>
        protected override void Transform(Target2DWrapper<Graphics> target, View2D view)
	    {
	        _view = view;
	        _target = target;
	    }

        /// <summary>
        /// Transforms the x-coordinate to screen coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private float TransformX(double x)
        {
            return (float)_view.ToViewPortX(_target.Width, x);
        }

        /// <summary>
        /// Transforms the y-coordinate to screen coordinates.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        private float TransformY(double y)
        {
            return (float) _view.ToViewPortY(_target.Height, y);
        }

        /// <summary>
        /// Returns the size in pixels.
        /// </summary>
        /// <returns></returns>
        private float ToPixels(double sceneSize)
        {
            double scaleX = _target.Width / _view.Width;

            return (float)(sceneSize * scaleX);
        }

	    /// <summary>
	    /// Returns the size in pixels.
	    /// </summary>
	    /// <returns>The pixels.</returns>
	    /// <param name="target"></param>
	    /// <param name="view">View.</param>
	    /// <param name="sizeInPixels">Size in pixels.</param>
	    protected override double FromPixels(Target2DWrapper<Graphics> target, View2D view, double sizeInPixels)
        {
            double scaleX = target.Width / view.Width;

            return sizeInPixels / scaleX;
        }

	    /// <summary>
	    /// Draws the backcolor.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="backColor"></param>
	    protected override void DrawBackColor(Target2DWrapper<Graphics> target, int backColor)
        {
            target.Target.Clear(Color.FromArgb(backColor));
        }

	    /// <summary>
	    /// Draws a point on the target. The coordinates given are scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="size">Size.</param>
	    protected override void DrawPoint(Target2DWrapper<Graphics> target, double x, double y, int color, double size)
	    {
	        float sizeInPixels = this.ToPixels(size);
            target.Target.FillEllipse(new SolidBrush(Color.FromArgb(color)), this.TransformX(x), this.TransformY(y),
                sizeInPixels, sizeInPixels);
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
	    protected override void DrawLine(Target2DWrapper<Graphics> target, double[] x, double[] y, int color, double width, 
            LineJoin lineJoin, int[] dashes)
	    {
	        float widthInPixels = this.ToPixels(width);

	        _pen.Color = Color.FromArgb(color);
            _pen.Width = this.ToPixels(width);
            _pen.DashStyle = DashStyle.Solid;
            if (dashes != null)
            {
                var penDashes = new float[dashes.Length];
                for (int idx = 0; idx < dashes.Length; idx++)
                {
                    penDashes[idx] = dashes[idx];
                }
                _pen.DashPattern = penDashes;
                _pen.DashStyle = DashStyle.Custom;
            }
		    switch (lineJoin)
		    {
		        case LineJoin.Round:
                    _pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
		            break;
		        case LineJoin.Miter:
                    _pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
		            break;
		        case LineJoin.Bevel:
                    _pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
		            break;
		        case LineJoin.None:
		            // just keep the default.
                    _pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
		            break;
		        default:
		            throw new ArgumentOutOfRangeException("lineJoin");
		    }
            _pen.StartCap = LineCap.Square;
            _pen.EndCap = LineCap.Square;
		    var points = new PointF[x.Length];
		    for (int idx = 0; idx < x.Length; idx++)
		    {
                points[idx] = new PointF(this.TransformX(x[idx]), 
                    this.TransformY(y[idx]));
		    }
            target.Target.DrawLines(_pen, points);
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
	    protected override void DrawPolygon(Target2DWrapper<Graphics> target, double[] x, double[] y, int color,
            double width, bool fill)
        {
            float widthInPixels = this.ToPixels(width);

            var points = new PointF[x.Length];
            for (int idx = 0; idx < x.Length; idx++)
            {
                points[idx] = new PointF(this.TransformX(x[idx]), 
                    this.TransformY(y[idx]));
            }
            if (fill)
            {
                var pen = new Pen(Color.FromArgb(color), widthInPixels);
                target.Target.FillPolygon(new SolidBrush(Color.FromArgb(color)), points);
            }
            else
            {
                var pen = new Pen(Color.FromArgb(color), widthInPixels);
                target.Target.DrawPolygon(pen, points);
            }
		}

	    /// <summary>
	    /// Draws an icon unscaled but at the given scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x"></param>
	    /// <param name="y"></param>
	    /// <param name="imageData"></param>
	    protected override void DrawIcon(Target2DWrapper<Graphics> target, double x, double y, byte[] imageData)
        {
            // get the image.
            Image image = Image.FromStream(new MemoryStream(imageData));

            // draw the image.
            target.Target.DrawImage(image, this.TransformX(x), this.TransformY(y));
        }

	    /// <summary>
	    /// Draws an image.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="left"></param>
	    /// <param name="top"></param>
	    /// <param name="right"></param>
	    /// <param name="bottom"></param>
	    /// <param name="imageData"></param>
	    /// <param name="tag"></param>
	    protected override object DrawImage(Target2DWrapper<Graphics> target, double left, double top, double right, 
            double bottom, byte[] imageData, object tag)
        {
            // get the image.
            var image = (tag as Image);
            if (image == null)
            {
                image = Image.FromStream(new MemoryStream(imageData));
            }

	        float x = this.TransformX(left);
	        float y = this.TransformY(top);
	        float width = this.TransformX(right) - x;
	        float height = this.TransformY(bottom) - y;
            target.Target.DrawImage(image, new RectangleF(x, y,
                width, height));
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
        protected override void DrawText(Target2DWrapper<Graphics> target, double x, double y, string text, int color, double size)
        {
            float sizeInPixels = this.ToPixels(size);
            Color textColor = Color.FromArgb(color);

            target.Target.DrawString(text, new Font(FontFamily.GenericSansSerif, sizeInPixels), new SolidBrush(textColor),
                this.TransformX(x), this.TransformY(y));
        }

        /// <summary>
        /// Draws text along a line.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        protected override void DrawLineText(Target2DWrapper<Graphics> target, double[] x, double[] y, string text, int color, double size)
        {
            if (x.Length > 1)
            {
                float sizeInPixels = this.ToPixels(size);
                Color textColor = Color.FromArgb(color);
                Brush brush = new SolidBrush(textColor);
                Font font = new Font(FontFamily.GenericSerif, sizeInPixels);

                // get some metrics on the texts.
                var characterWidths = GetCharacterWidths(target.Target, text, font);
                for (int idx = 0; idx < characterWidths.Length; idx++)
                {
                    characterWidths[idx] = (float)this.FromPixels(_target, _view, characterWidths[idx]);
                }
                var characterHeight = target.Target.MeasureString(text, font).Height;
                var textLength = characterWidths.Sum();
                var avgCharacterWidth = textLength / characterWidths.Length;

                // calculate line length.
                var lineLength = Polyline2D.Length(x, y);
                if (lineLength > textLength)
                {
                    // see if the text is 'upside down'.
                    double averageAngle = 0;
                    double middle = lineLength / 2.0;
                    double first = middle - (textLength / 2.0);
                    PointF2D current = Polyline2D.PositionAtPosition(x, y, first);
                    for (int idx = 0; idx < text.Length; idx++)
                    {
                        double nextPosition = middle - (textLength / 2.0) + ((textLength / (text.Length)) * (idx + 1));
                        PointF2D next = Polyline2D.PositionAtPosition(x, y, nextPosition);

                        // Translate to the final position, the center of line-segment between 'current' and 'next'
                        PointF2D position = current + ((next - current) / 2.0);

                        // calculate the angle.
                        VectorF2D vector = next - current;
                        VectorF2D horizontal = new VectorF2D(1, 0);
                        double angleDegrees = ((Degree)horizontal.Angle(vector)).Value;
                        averageAngle = averageAngle + angleDegrees;
                        current = next;
                    }
                    averageAngle = averageAngle / text.Length;

                    // reverse if 'upside down'.
                    double[] xText = x;
                    double[] yText = y;
                    if (averageAngle > 90 && averageAngle < 180 + 90)
                    { // the average angle is > PI => means upside down.
                        xText = x.Reverse<double>().ToArray<double>();
                        yText = y.Reverse<double>().ToArray<double>();
                    }

                    // calculate a central position along the line.
                    middle = lineLength / 2.0;
                    first = middle - (textLength / 2.0);
                    current = Polyline2D.PositionAtPosition(xText, yText, first);
                    for (int idx = 0; idx < text.Length; idx++)
                    {
                        double nextPosition = middle - (textLength / 2.0) + ((textLength / (text.Length)) * (idx + 1));
                        PointF2D next = Polyline2D.PositionAtPosition(xText, yText, nextPosition);
                        char currentChar = text[idx];
                        using (GraphicsPath characterPath = new GraphicsPath())
                        {
                            characterPath.AddString(currentChar.ToString(), font.FontFamily, (int)font.Style, font.Size, Point.Empty,
                                                    StringFormat.GenericTypographic);

                            var pathBounds = characterPath.GetBounds();

                            // Transformation matrix to move the character to the correct location. 
                            // Note that all actions on the Matrix class are prepended, so we apply them in reverse.
                            var transform = new Matrix();

                            // Translate to the final position, the center of line-segment between 'current' and 'next'
                            PointF2D position = current + ((next - current) / 2.0);
                            transform.Translate(this.TransformX(position[0]), this.TransformY(position[1]));

                            // calculate the angle.
                            VectorF2D vector = next - current;
                            VectorF2D horizontal = new VectorF2D(1, 0);
                            double angleDegrees = ((Degree)horizontal.Angle(vector)).Value;

                            // Rotate the character
                            transform.Rotate((float)angleDegrees);

                            // Translate the character so the centre of its base is over the origin
                            transform.Translate(0, -characterHeight / 2.5f);
                            
                            //transform.Scale((float)this.FromPixels(_target, _view, 1), 
                            //    (float)this.FromPixels(_target, _view, 1));

                            characterPath.Transform(transform);

                            // Draw the character
                            target.Target.FillPath(brush, characterPath);
                        }
                        current = next;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the char widths for the given string and given font.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        private float[] GetCharacterWidths(Graphics graphics, string text, Font font)
        {
            // The length of a space. Necessary because a space measured using StringFormat.GenericTypographic has no width.
            // We can't use StringFormat.GenericDefault for the characters themselves, as it adds unwanted spacing.
            var spaceLength = graphics.MeasureString(" ", font, Point.Empty, StringFormat.GenericDefault).Width;

            float[] widths = new float[text.Length];
            for (int idx = 0; idx < text.Length; idx++)
            {
                widths[idx] = (text[idx] == ' ' ? spaceLength :
                    graphics.MeasureString(text[idx].ToString(), font, Point.Empty, StringFormat.GenericTypographic).Width);
            }
            return widths;
        }
    }
}
// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Primitives;
using OsmSharp.Math;
using OsmSharp.Units.Angle;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.WinForms.UI.Renderer.Images;

namespace OsmSharp.WinForms.UI.Renderer
{
	/// <summary>
	/// Graphics renderer 2D.
	/// </summary>
    public class GraphicsRenderer2D : Renderer2D<Graphics>
	{
        private Pen _pen = new Pen(Color.Black);

        #region Caching Implementation

        /// <summary>
        /// Called right before rendering starts.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="view"></param>
        protected override void OnBeforeRender(Target2DWrapper<Graphics> target, View2D view)
        {
            //// create a bitmap and render there.
            //var bitmap = new Bitmap((int)target.Width, (int)target.Height);
            //target.BackTarget = target.Target;
            //target.BackTarget.CompositingMode = CompositingMode.SourceOver;
            //target.Target = Graphics.FromImage(bitmap);
            //target.Target.CompositingMode = CompositingMode.SourceOver;
            //target.Target.SmoothingMode = target.BackTarget.SmoothingMode;
            //target.Target.PixelOffsetMode = target.BackTarget.PixelOffsetMode;
            //target.Target.InterpolationMode = target.BackTarget.InterpolationMode;

            //target.Tag = bitmap;
        }

        /// <summary>
        /// Called right after rendering.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="view"></param>
        protected override void OnAfterRender(Target2DWrapper<Graphics> target, View2D view)
        {
            //target.Target.Flush();
            //target.Target = target.BackTarget;
            //var bitmap = target.Tag as Bitmap;
            //if (bitmap != null)
            //{
            //    target.Target.DrawImageUnscaled(bitmap, 0, 0);
            //}
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
        /// Holds the to-view transformation matrix.
        /// </summary>
        private Matrix2D _toView;

	    /// <summary>
	    /// Transforms the target using the specified view.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="view">View.</param>
        protected override void Transform(Target2DWrapper<Graphics> target, View2D view)
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
        /// <returns></returns>
        private double[] Tranform(double x, double y)
        {
            double newX, newY;
            _toView.Apply(x, y, out newX, out newY);
            return new double[] { newX, newY };
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
            double[] transformed = this.Tranform(x, y);
	        float sizeInPixels = this.ToPixels(size);
            target.Target.FillEllipse(new SolidBrush(Color.FromArgb(color)), (float)transformed[0] - (sizeInPixels / 2.0f), (float)transformed[1] - (sizeInPixels / 2.0f),
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
            OsmSharp.UI.Renderer.Primitives.LineJoin lineJoin, int[] dashes)
	    {
//	        float widthInPixels = this.ToPixels(width);

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
                case OsmSharp.UI.Renderer.Primitives.LineJoin.Round:
                    _pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
		            break;
                case OsmSharp.UI.Renderer.Primitives.LineJoin.Miter:
                    _pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
		            break;
                case OsmSharp.UI.Renderer.Primitives.LineJoin.Bevel:
                    _pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
		            break;
                case OsmSharp.UI.Renderer.Primitives.LineJoin.None:
		            // just keep the default.
                    _pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
		            break;
		        default:
		            throw new ArgumentOutOfRangeException("lineJoin");
		    }
            _pen.StartCap = LineCap.Round;
            _pen.EndCap = LineCap.Round;
		    var points = new PointF[x.Length];
		    for (int idx = 0; idx < x.Length; idx++)
		    {
                double[] transformed = this.Tranform(x[idx], y[idx]);
                points[idx] = new PointF((float)transformed[0], (float)transformed[1]);
		    }
            //if (casingWidth > 0)
            //{ // draw casing.
            //    _pen.Color = Color.FromArgb(casingColor);
            //    _pen.Width = this.ToPixels(casingWidth + width);
            //    target.Target.DrawLines(_pen, points);
            //}
            // set color/width.
            _pen.Color = Color.FromArgb(color);
            _pen.Width = this.ToPixels(width);
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
                double[] transformed = this.Tranform(x[idx], y[idx]);
                points[idx] = new PointF((float)transformed[0], (float)transformed[1]);
            }
            if (fill)
            {
//                var pen = new Pen(Color.FromArgb(color), widthInPixels);
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
            double[] transformed = this.Tranform(x, y);
            target.Target.DrawImage(image, (float)transformed[0], (float)transformed[1]);
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
	    protected override void DrawImage(Target2DWrapper<Graphics> target, double left, double top, double right, 
            double bottom, INativeImage nativeImage)
        {
            // get the image.
            var image = (nativeImage as NativeImage).Image;

            // set interpolation mode to default. Only used when displaying tiles.
            var previousInterpolationMode = target.Target.InterpolationMode;
            target.Target.InterpolationMode = InterpolationMode.Default;

            // draw image.
            double[] topleft = this.Tranform(left, top);
            double[] bottomright = this.Tranform(right, bottom);
	        float x = (float)topleft[0];
            float y = (float)topleft[1];
            float width = (float)bottomright[0] - x;
            float height = (float)bottomright[1] - y;
            target.Target.DrawImage(image, new RectangleF(x, y,
                width, height));

            // reset interpolation mode to default.
            target.Target.InterpolationMode = previousInterpolationMode;
        }

		/// <summary>
		/// Draws the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="target">Target.</param>
		/// <param name="bounds">Bounds.</param>
        /// <param name="nativeImage">Image data.</param>
        protected override void DrawImage(Target2DWrapper<Graphics> target, RectangleF2D bounds, INativeImage nativeImage)
		{

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
        protected override void DrawText(Target2DWrapper<Graphics> target, double x, double y, string text, int color, double size,
            int? haloColor, int? haloRadius, string fontName)
        {
            float sizeInPixels = this.ToPixels(size);
            var textColor = Color.FromArgb(color);
            var font = new Font(fontName, sizeInPixels);
            var brush = new SolidBrush(textColor);
            double[] transformed = this.Tranform(x, y);
            float transformedX = (float)transformed[0];
            float transformedY = (float)transformed[1];
            Brush haloBrush = null;
            GraphicsPath characterPath = new GraphicsPath();
            characterPath.AddString(text, font.FontFamily, (int)font.Style, font.Size, new PointF(transformedX, transformedY),
                                            StringFormat.GenericTypographic);
            if (haloColor.HasValue && haloRadius.HasValue && haloRadius.Value > 0)
            {
                haloBrush = new SolidBrush(Color.FromArgb(haloColor.Value));

                GraphicsPath haloPath = characterPath.Clone() as GraphicsPath;
                using (haloPath)
                {
                    haloPath.Widen(new Pen(haloBrush, haloRadius.Value * 2));

                    // Draw the character
                    target.Target.FillPath(haloBrush, haloPath);
                }
            }
            // Draw the character
            target.Target.FillPath(brush, characterPath);
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
        protected override void DrawLineText(Target2DWrapper<Graphics> target, double[] x, double[] y, string text, int color, 
            double size, int? haloColor, int? haloRadius, string fontName)
        {
            if (x.Length > 1)
            {
                float sizeInPixels = this.ToPixels(size);
                Color textColor = Color.FromArgb(color);
                Brush brush = new SolidBrush(textColor);
                Brush haloBrush = null;
                if (haloColor.HasValue && haloRadius.HasValue && haloRadius.Value > 0)
                {
                    haloBrush = new SolidBrush(Color.FromArgb(haloColor.Value));
                }
                var font = new Font(fontName, sizeInPixels);

                // get some metrics on the texts.
                var characterWidths = GetCharacterWidths(target.Target, text, font);
                for (int idx = 0; idx < characterWidths.Length; idx++)
                {
                    characterWidths[idx] = (float)this.FromPixels(_target, _view, characterWidths[idx]);
                }
                var characterHeight = target.Target.MeasureString(text, font).Height;
                var textLength = characterWidths.Sum();
//                var avgCharacterWidth = textLength / characterWidths.Length;

                // calculate line length.
                var lineLength = Polyline2D.Length(x, y);
                if (lineLength > textLength * 1.1f)
                {
                    // calculate the number of labels.
                    int labelCount = (int)System.Math.Floor(lineLength / (textLength * 10)) + 1;

                    // calculate positions of label(s).
                    double positionGap = lineLength / (labelCount + 1);

                    // draw each label.
                    for (double position = positionGap; position < lineLength; position = position + positionGap)
                    {
                        this.DrawLineTextSegment(target, x, y, text, color, size, haloColor, haloRadius, position, characterWidths,
                            textLength, font, characterHeight, haloBrush, brush);
                    }
                }
            }
        }

        /// <summary>
        /// Draws text along a line segment.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="haloColor"></param>
        /// <param name="haloRadius"></param>
        /// <param name="middlePosition"></param>
        /// <param name="characterWidths"></param>
        /// <param name="textLength"></param>
        /// <param name="font"></param>
        /// <param name="characterHeight"></param>
        /// <param name="haloBrush"></param>
        /// <param name="brush"></param>
        private void DrawLineTextSegment(Target2DWrapper<Graphics> target, double[] x, double[] y, string text, int color,
            double size, int? haloColor, int? haloRadius, double middlePosition, float[] characterWidths, double textLength,
            Font font, float characterHeight, Brush haloBrush, Brush brush)
        {
            // see if the text is 'upside down'.
            double averageAngle = 0;
            double first = middlePosition - (textLength / 2.0);
            PointF2D current = Polyline2D.PositionAtPosition(x, y, first);
            for (int idx = 0; idx < text.Length; idx++)
            {
                double nextPosition = middlePosition - (textLength / 2.0) + ((textLength / (text.Length)) * (idx + 1));
                PointF2D next = Polyline2D.PositionAtPosition(x, y, nextPosition);

                // Translate to the final position, the center of line-segment between 'current' and 'next'
//                PointF2D position = current + ((next - current) / 2.0);

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
            first = middlePosition - (textLength / 2.0);
            current = Polyline2D.PositionAtPosition(xText, yText, first);
            double nextPosition2 = first;
            for (int idx = 0; idx < text.Length; idx++)
            {
                nextPosition2 = nextPosition2 + characterWidths[idx];
                //double nextPosition = middle - (textLength / 2.0) + ((textLength / (text.Length)) * (idx + 1));
                PointF2D next = Polyline2D.PositionAtPosition(xText, yText, nextPosition2);
                char currentChar = text[idx];
                using (GraphicsPath characterPath = new GraphicsPath())
                {
                    characterPath.AddString(currentChar.ToString(), font.FontFamily, (int)font.Style, font.Size, Point.Empty,
                                            StringFormat.GenericTypographic);

//                    var pathBounds = characterPath.GetBounds();

                    // Transformation matrix to move the character to the correct location. 
                    // Note that all actions on the Matrix class are prepended, so we apply them in reverse.
                    var transform = new Matrix();

                    // Translate to the final position, the center of line-segment between 'current' and 'next'
                    PointF2D position = current;
                    //PointF2D position = current + ((next - current) / 2.0);
                    double[] transformed = this.Tranform(position[0], position[1]);
                    transform.Translate((float)transformed[0], (float)transformed[1]);

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

                    if (haloColor.HasValue && haloRadius.HasValue && haloRadius.Value > 0)
                    {
                        GraphicsPath haloPath = characterPath.Clone() as GraphicsPath;
                        using (haloPath)
                        {
                            haloPath.Widen(new Pen(haloBrush, haloRadius.Value * 2));

                            // Draw the character
                            target.Target.FillPath(haloBrush, haloPath);
                        }
                    }

                    // Draw the character
                    target.Target.FillPath(brush, characterPath);
                }
                current = next;
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
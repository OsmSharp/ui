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
using System.Linq;
using OsmSharp.Math;
using OsmSharp.Math.Primitives;
using OsmSharp.UI;
using OsmSharp.UI.Renderer;
using OsmSharp.Units.Angle;
using OsmSharp.UI.Renderer.Primitives;
#if __UNIFIED__
using CoreGraphics;
using Foundation;
using UIKit;
using CoreText;
#else
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreText;

// Type Mappings Unified to monotouch.dll
using CGRect = global::System.Drawing.RectangleF;
using CGSize = global::System.Drawing.SizeF;
using CGPoint = global::System.Drawing.PointF;

using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace OsmSharp.iOS.UI
{
	/// <summary>
	/// CG context renderer.
	/// </summary>
	public class CGContextRenderer : Renderer2D<CGContextWrapper>
	{
		/// <summary>
		/// Holds the default font to use when not font is specified.
		/// </summary>
		private const string DefaultFontName = "ArialMT";
		/// <summary>
		/// Holds the scale factor to enable higher resolution renderings.
		/// </summary>
		private float _scaleFactor;
		/// <summary>
		/// Holds the latest font to prevent creating it over and over.
		/// </summary>
		private CTFont _font = null;
		/// <summary>
		/// Holds the latest font name used to instantiate the latest font.
		/// </summary>
		private string _fontName = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.CGContextRenderer"/> class.
		/// </summary>
		public CGContextRenderer()
			: this(1)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.CGContextRenderer"/> class.
		/// </summary>
		/// <param name="scaleFactor">Scale factor.</param>
		public CGContextRenderer(float scaleFactor)
		{
			_scaleFactor = scaleFactor;
		}

		#region implemented abstract members of Renderer2D

		/// <summary>
		/// Holds the view.
		/// </summary>
		private View2D _view;
		/// <summary>
		/// Holds the target.
		/// </summary>
		private Target2DWrapper<CGContextWrapper> _target;
		/// <summary>
		/// Holds the scale x.
		/// </summary>
		private float _scaleX;

		/// <summary>
		/// Holds a utility rectangle to be reused.
		/// </summary>
		private CGRect _rectangle = new CGRect();

		/// <summary>
		/// Transforms the target using the specified view.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="view">View.</param>
		protected override void Transform(Target2DWrapper<CGContextWrapper> target, View2D view)
		{
			_view = view;
			_target = target;

			_scaleX = (float)(target.Width / view.Width);
		}

		/// <summary>
		/// Creates a wrapper for the target making it possible to drag along some properties.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public override Target2DWrapper<CGContextWrapper> CreateTarget2DWrapper(CGContextWrapper target)
		{
			return new Target2DWrapper<CGContextWrapper>(target, target.Width, target.Height);
		}

		/// <summary>
		/// Converts pixel size to scene size.
		/// </summary>
		/// <returns>The pixels.</returns>
		/// <param name="target"></param>
		/// <param name="view">View.</param>
		/// <param name="sizeInPixels">Size in pixels.</param>
		protected override double FromPixels(Target2DWrapper<CGContextWrapper> target, View2D view, double sizeInPixels)
		{
			return sizeInPixels / (_scaleX);
		}

		/// <summary>
		/// Converts scene size to pixels.
		/// </summary>
		/// <returns>The pixels.</returns>
		/// <param name="sceneSize">Scene size.</param>
		private float ToPixels(double sceneSize)
		{
			return (float)sceneSize * _scaleX;
		}

		/// <summary>
		/// Transforms the y-coordinate to screen coordinates.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private double[] Transform(double x, double y)
		{
			return _view.ToViewPort(_target.Width * _scaleFactor, _target.Height * _scaleFactor, x, y);
		}

		/// <summary>
		/// Transform the specified x and y.
		/// </summary>
		/// <param name="x">The x coordinates.</param>
		/// <param name="y">The y coordinates.</param>
		private double[][] TransformAll(double[] x, double[] y)
		{
			return _view.ToViewPort(_target.Width * _scaleFactor, _target.Height * _scaleFactor, x, y);
		}

		/// <summary>
		/// Gets the font for the given fontname and textSize.
		/// </summary>
		/// <returns>The font.</returns>
		/// <param name="fontName">Font name.</param>
		private CTFont GetFont(string fontName, float textSize)
		{
			if (string.IsNullOrWhiteSpace(fontName))
			{
				fontName = CGContextRenderer.DefaultFontName;
			}
			if (_font == null)
			{
				_fontName = fontName;
				_font = new CTFont(fontName, textSize);
			}
			if (_fontName != fontName)
			{
				_font.Dispose();

				_fontName = fontName;
				_font = new CTFont(fontName, textSize);
			}
			if (_font.Size != textSize)
			{
				_font.Dispose();

				_fontName = fontName;
				_font = new CTFont(fontName, textSize);
			}
			CTFont font = _font;
			return font;
		}

		/// <summary>
		/// Draws the backcolor.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="backColor"></param>
		protected override void DrawBackColor(Target2DWrapper<CGContextWrapper> target, int backColor)
		{
			var backColorSimple = SimpleColor.FromArgb(backColor);
			target.Target.CGContext.SetFillColor(backColorSimple.R / 256.0f, backColorSimple.G / 256.0f, backColorSimple.B / 256.0f,
				backColorSimple.A / 256.0f);
			_rectangle.X = 0;
			_rectangle.Y = 0;
			_rectangle.Width = target.Width * _scaleFactor;
			_rectangle.Height = target.Height * _scaleFactor;
			target.Target.CGContext.FillRect(_rectangle);
		}

		/// <summary>
		/// Draws a point on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		protected override void DrawPoint(Target2DWrapper<CGContextWrapper> target, double x, double y, int color, double size)
		{
			var transformed = this.Transform(x, y);
			var colorSimple = SimpleColor.FromArgb(color);
			var radius = (float)(this.ToPixels(size) * _scaleFactor) / 2.0f; ;
			target.Target.CGContext.SetFillColor(colorSimple.R / 256.0f, colorSimple.G / 256.0f, colorSimple.B / 256.0f,
				colorSimple.A / 256.0f);
			_rectangle.X = (float)transformed[0] - radius;
			_rectangle.Y = (float)transformed[1] - radius;
			_rectangle.Width = radius * 2;
			_rectangle.Height = radius * 2;
			target.Target.CGContext.FillEllipseInRect(_rectangle);
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
		protected override void DrawLine(Target2DWrapper<CGContextWrapper> target, double[] x, double[] y, int color, double width, 
			LineJoin lineJoin, int[] dashes)
		{
			float widthInPixels = this.ToPixels(width) * _scaleFactor;

			SimpleColor simpleColor = SimpleColor.FromArgb(color);
			target.Target.CGContext.SetLineJoin(CGLineJoin.Round);
			target.Target.CGContext.SetLineWidth(widthInPixels);
			target.Target.CGContext.SetStrokeColor(simpleColor.R / 256.0f, simpleColor.G / 256.0f, simpleColor.B / 256.0f,
				simpleColor.A / 256.0f);
			target.Target.CGContext.SetLineDash(0, new nfloat[0]);
			if (dashes != null && dashes.Length > 1)
			{
				nfloat[] intervals = new nfloat[dashes.Length];
				for (int idx = 0; idx < dashes.Length; idx++)
				{
					intervals[idx] = dashes[idx];
				}
				target.Target.CGContext.SetLineDash(0.0f, intervals);
			}

			// transform the points all at once.
			double[][] transformed = this.TransformAll(x, y);

			// construct path.
			target.Target.CGContext.BeginPath();
			CGPoint[] points = new CGPoint[x.Length];
			for (int idx = 0; idx < x.Length; idx++)
			{
				points[idx] = new CGPoint((float)transformed[idx][0], (float)transformed[idx][1]);
			}
			target.Target.CGContext.AddLines(points);
			target.Target.CGContext.DrawPath(CGPathDrawingMode.Stroke);
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
		protected override void DrawPolygon(Target2DWrapper<CGContextWrapper> target, double[] x, double[] y, int color, double width, 
			bool fill)
		{
			float widthInPixels = this.ToPixels(width) * _scaleFactor;

			SimpleColor simpleColor = SimpleColor.FromArgb(color);
			target.Target.CGContext.SetLineWidth(widthInPixels);
			target.Target.CGContext.SetFillColor(simpleColor.R / 256.0f, simpleColor.G / 256.0f, simpleColor.B / 256.0f,
				simpleColor.A / 256.0f);
			target.Target.CGContext.SetStrokeColor(simpleColor.R / 256.0f, simpleColor.G / 256.0f, simpleColor.B / 256.0f,
				simpleColor.A / 256.0f);

			// transform the points all at once.
			double[][] transformed = this.TransformAll(x, y);

			// build the path.
			target.Target.CGContext.BeginPath();
			CGPoint[] points = new CGPoint[x.Length];
			for (int idx = 0; idx < x.Length; idx++)
			{
				points[idx] = new CGPoint((float)transformed[idx][0], (float)transformed[idx][1]);
			}

			target.Target.CGContext.AddLines(points);
			target.Target.CGContext.ClosePath();
			target.Target.CGContext.FillPath();
		}

		/// <summary>
		/// Draws an icon on the target unscaled but centered at the given scene coordinates.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="imageData"></param>
		protected override void DrawIcon(Target2DWrapper<CGContextWrapper> target, double x, double y, byte[] imageData)
		{

		}

		/// <summary>
		/// Draws an image on the target.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		/// <param name="nativeImage"></param>
		/// <returns>The image.</returns>
		protected override void DrawImage(Target2DWrapper<CGContextWrapper> target, double left, double top, double right, 
			double bottom, INativeImage nativeImage)
		{
			// get the native image.
			var iosNativeImage = (nativeImage as NativeImage);

			// get CGImage.
			CGImage image = iosNativeImage.Image;

			var bounds = new RectangleF2D(left, bottom, (left - right),
				(top - bottom));

			target.Target.CGContext.SetAllowsFontSubpixelQuantization(true);
			target.Target.CGContext.SetAllowsFontSmoothing(true);
			target.Target.CGContext.SetAllowsAntialiasing(true);
			target.Target.CGContext.SetAllowsSubpixelPositioning(true);
			target.Target.CGContext.SetShouldAntialias(true);

			PointF2D bottomLeft = new PointF2D(this.Transform(bounds.BottomLeft[0], bounds.BottomLeft[1]));
			PointF2D bottomRight = new PointF2D(this.Transform(bounds.BottomRight[0], bounds.BottomRight[1]));
			PointF2D topLeft = new PointF2D(this.Transform(bounds.TopLeft[0], bounds.TopLeft[1]));
			//PointF2D topRight = new PointF2D(this.Tranform (bounds.TopRight [0], bounds.TopRight [1])); 

			RectangleF2D transformed = new RectangleF2D(bottomLeft, bottomLeft.Distance(bottomRight), bottomLeft.Distance(topLeft), 
				topLeft - bottomLeft);

			target.Target.CGContext.SaveState();
			target.Target.CGContext.TranslateCTM((float)transformed.BottomLeft[0], (float)transformed.BottomLeft[1]);
			target.Target.CGContext.RotateCTM(-(float)((Radian)transformed.Angle).Value);
			target.Target.CGContext.ScaleCTM(-1, 1);

			// build rectangle.
			_rectangle.X = 0;
			_rectangle.Y = 0;
			_rectangle.Width = (float)transformed.Width;
			_rectangle.Height = (float)transformed.Height;
			target.Target.CGContext.DrawImage(_rectangle, image);

			target.Target.CGContext.RestoreState();
		}

		/// <summary>
		/// Draws the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="target">Target.</param>
		/// <param name="bounds">Bounds.</param>
		/// <param name="imageData">Image data.</param>
		/// <param name="tag">Tag.</param>
		protected override void DrawImage(Target2DWrapper<CGContextWrapper> target, RectangleF2D bounds, 
			INativeImage nativeImage)
		{            
			// get the native image.
			var iosNativeImage = (nativeImage as NativeImage);

			// get CGImage.
			CGImage image = iosNativeImage.Image;

			target.Target.CGContext.SetAllowsFontSubpixelQuantization(true);
			target.Target.CGContext.SetAllowsFontSmoothing(true);
			target.Target.CGContext.SetAllowsAntialiasing(true);
			target.Target.CGContext.SetAllowsSubpixelPositioning(true);
			target.Target.CGContext.SetShouldAntialias(true);

			PointF2D bottomLeft = new PointF2D(this.Transform(bounds.BottomLeft[0], bounds.BottomLeft[1]));
			PointF2D bottomRight = new PointF2D(this.Transform(bounds.BottomRight[0], bounds.BottomRight[1]));
			PointF2D topLeft = new PointF2D(this.Transform(bounds.TopLeft[0], bounds.TopLeft[1]));
			//PointF2D topRight = new PointF2D(this.Tranform (bounds.TopRight [0], bounds.TopRight [1])); 

			RectangleF2D transformed = new RectangleF2D(bottomLeft, bottomLeft.Distance(bottomRight), bottomLeft.Distance(topLeft), 
				topLeft - bottomLeft);

			target.Target.CGContext.SaveState();
			target.Target.CGContext.TranslateCTM((float)transformed.BottomLeft[0], (float)transformed.BottomLeft[1]);
			target.Target.CGContext.RotateCTM(-(float)((Radian)transformed.Angle).Value);

			// build rectangle.
			_rectangle.X = 0;
			_rectangle.Y = 0;
			_rectangle.Width = (float)transformed.Width;
			_rectangle.Height = (float)transformed.Height;
			target.Target.CGContext.DrawImage(_rectangle, image);

			target.Target.CGContext.RestoreState();
		}

		/// <summary>
		/// Draws text.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="text"></param>
		/// <param name="size"></param>
		/// <param name="color">Color.</param>
		protected override void DrawText(Target2DWrapper<CGContextWrapper> target, double x, double y, string text, int color, double size,
			int? haloColor, int? haloRadius, string fontName)
		{
			double[] transformed = this.Transform(x, y);
			float xPixels = (float)transformed[0];
			float yPixels = (float)transformed[1];
			float textSize = this.ToPixels(size) * _scaleFactor;

			// get the glyhps/paths from the font.
			CTFont font = this.GetFont(fontName, textSize);
			CTStringAttributes stringAttributes = new CTStringAttributes
			{
				ForegroundColorFromContext = true,
				Font = font
			};
			NSAttributedString attributedString = new NSAttributedString(text, stringAttributes);
			CTLine line = new CTLine(attributedString);
			CTRun[] runs = line.GetGlyphRuns();

			// set the correct tranformations to draw the resulting paths.
			target.Target.CGContext.SaveState();
			target.Target.CGContext.TranslateCTM(xPixels, yPixels);
			target.Target.CGContext.ConcatCTM(new CGAffineTransform(1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f));
			foreach (CTRun run in runs)
			{
				ushort[] glyphs = run.GetGlyphs();
				CGPoint[] positions = run.GetPositions();

				float previousOffset = 0;
				for (int idx = 0; idx < glyphs.Length; idx++)
				{
					CGPath path = font.GetPathForGlyph(glyphs[idx]);
					target.Target.CGContext.TranslateCTM(positions[idx].X - previousOffset, 0);
					previousOffset = (float)positions[idx].X;
					if (haloRadius.HasValue && haloColor.HasValue)
					{ // also draw the halo.
						using (CGPath haloPath = path.CopyByStrokingPath(
							haloRadius.Value * 2, CGLineCap.Round, CGLineJoin.Round, 0))
						{
							SimpleColor haloSimpleColor = SimpleColor.FromArgb(haloColor.Value);
							target.Target.CGContext.SetFillColor(haloSimpleColor.R / 256.0f, haloSimpleColor.G / 256.0f, haloSimpleColor.B / 256.0f,
								haloSimpleColor.A / 256.0f);
							target.Target.CGContext.BeginPath();
							target.Target.CGContext.AddPath(haloPath);
							target.Target.CGContext.DrawPath(CGPathDrawingMode.Fill);
						}
					}

					// set the fill color as the regular text-color.
					SimpleColor simpleColor = SimpleColor.FromArgb(color);
					target.Target.CGContext.SetFillColor(simpleColor.R / 256.0f, simpleColor.G / 256.0f, simpleColor.B / 256.0f,
						simpleColor.A / 256.0f);

					// draw the text paths.
					target.Target.CGContext.BeginPath();
					target.Target.CGContext.AddPath(path);
					target.Target.CGContext.DrawPath(CGPathDrawingMode.Fill);
				}
			}

			target.Target.CGContext.RestoreState();
		}

		/// <summary>
		/// Draws text along a given line.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		/// <param name="size"></param>
		/// <param name="text">Text.</param>
		protected override void DrawLineText(Target2DWrapper<CGContextWrapper> target, double[] xa, double[] ya, string text, int color, 
			double size, int? haloColor, int? haloRadius, string fontName)
		{
			float textSize = this.ToPixels(size) * _scaleFactor;

			// transform first.
			double[] xTransformed = new double[xa.Length];
			double[] yTransformed = new double[ya.Length];
			for (int idx = 0; idx < xa.Length; idx++)
			{
				double[] transformed = this.Transform(xa[idx], ya[idx]);
				xTransformed[idx] = transformed[0];
				yTransformed[idx] = transformed[1];
			}

			// set the fill color as the regular text-color.
			target.Target.CGContext.InterpolationQuality = CGInterpolationQuality.High;
			target.Target.CGContext.SetAllowsFontSubpixelQuantization(true);
			target.Target.CGContext.SetAllowsFontSmoothing(true);
			target.Target.CGContext.SetAllowsAntialiasing(true);
			target.Target.CGContext.SetAllowsSubpixelPositioning(true);
			target.Target.CGContext.SetShouldAntialias(true);

			// get the glyhps/paths from the font.
			CTFont font = this.GetFont(fontName, textSize);
			CTStringAttributes stringAttributes = new CTStringAttributes
			{
				ForegroundColorFromContext = true,
				Font = font
			};
			NSAttributedString attributedString = new NSAttributedString(text, stringAttributes);
			CTLine line = new CTLine(attributedString);
			CGRect textBounds = line.GetBounds(CTLineBoundsOptions.UseOpticalBounds);
			CTRun[] runs = line.GetGlyphRuns();
			var lineLength = Polyline2D.Length(xTransformed, yTransformed);

			// set the correct tranformations to draw the resulting paths.
			target.Target.CGContext.SaveState();
			//target.Target.CGContext.TranslateCTM (xPixels, yPixels);
			//target.Target.CGContext.ConcatCTM (new CGAffineTransform (1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f));
			foreach (CTRun run in runs)
			{
				ushort[] glyphs = run.GetGlyphs();
				CGPoint[] positions = run.GetPositions();
				float[] characterWidths = new float[glyphs.Length];
				float previous = 0;
				float textLength = (float)positions[positions.Length - 1].X;
				//float textLength = (float)this.FromPixels(_target, _view, positions [positions.Length - 1].X);
				if (lineLength > textLength * 1.2f)
				{
					for (int idx = 0; idx < characterWidths.Length - 1; idx++)
					{
						//characterWidths [idx] = (float)this.FromPixels(_target, _view, positions [idx + 1].X - previous);
						characterWidths[idx] = (float)(positions[idx + 1].X - previous);
						previous = (float)positions[idx + 1].X;
					}
					characterWidths[characterWidths.Length - 1] = characterWidths[characterWidths.Length - 2];
					float characterHeight = (float)textBounds.Height;

					this.DrawLineTextSegment(target, xTransformed, yTransformed, glyphs, color, haloColor, haloRadius,
						lineLength / 2f, characterWidths, textLength, characterHeight, font);
				}
			}

			target.Target.CGContext.RestoreState();
		}

		private void DrawLineTextSegment(Target2DWrapper<CGContextWrapper> target, double[] xTransformed, double[] yTransformed, ushort[] glyphs, int color, 
			int? haloColor, int? haloRadius, double middlePosition, float[] characterWidths,
			double textLength, float charachterHeight, CTFont font)
		{

			// see if text is 'upside down'
			double averageAngle = 0;
			double first = middlePosition - (textLength / 2.0);
			PointF2D current = Polyline2D.PositionAtPosition(xTransformed, yTransformed, first);
			for (int idx = 0; idx < glyphs.Length; idx++)
			{
				double nextPosition = middlePosition - (textLength / 2.0) + ((textLength / (glyphs.Length)) * (idx + 1));
				PointF2D next = Polyline2D.PositionAtPosition(xTransformed, yTransformed, nextPosition);

				// translate to the final position, the center of the line segment between 'current' and 'next'.
				//PointF2D position = current + ((next - current) / 2.0);

				// calculate the angle.
				VectorF2D vector = next - current;
				VectorF2D horizontal = new VectorF2D(1, 0);
				double angleDegrees = ((Degree)horizontal.Angle(vector)).Value;
				averageAngle = averageAngle + angleDegrees;
				current = next;
			}
			averageAngle = averageAngle / glyphs.Length;


			// revers if 'upside down'
			double[] xText = xTransformed;
			double[] yText = yTransformed;
			if (averageAngle > 90 && averageAngle < 180 + 90)
			{
				xText = xTransformed.Reverse().ToArray();
				yText = yTransformed.Reverse().ToArray();
			}
			first = middlePosition - (textLength / 2.0);
			current = Polyline2D.PositionAtPosition(xText, yText, first);

			double nextPosition2 = first;
			for (int idx = 0; idx < glyphs.Length; idx++)
			{
				nextPosition2 = nextPosition2 + characterWidths[idx];
				PointF2D next = Polyline2D.PositionAtPosition(xText, yText, nextPosition2);
				//ushort currentChar = glyphs [idx];

				PointF2D position = current;

				target.Target.CGContext.SaveState();

				// translate to the final position, the center of the line segment between 'current' and 'next'.
				//                double[] transformed = this.Tranform(position[0], position[1]);
				//				target.Target.CGContext.TranslateCTM (
				//					(float)transformed [0],
				//					(float)transformed [1]);
				target.Target.CGContext.TranslateCTM((float)position[0], (float)position[1]);

				// calculate the angle.
				VectorF2D vector = next - current;
				VectorF2D horizontal = new VectorF2D(1, 0);
				double angleDegrees = (horizontal.Angle(vector)).Value;

				// rotate the character.
				target.Target.CGContext.RotateCTM((float)angleDegrees);

				// rotate the text to point down no matter what the map tilt is.
				//target.Target.CGContext.RotateCTM ((float)_view.Angle.Value);

				// translate the character so the center of its base is over the origin.
				target.Target.CGContext.TranslateCTM(0, charachterHeight / 3.0f);

				// rotate 'upside down'
				target.Target.CGContext.ConcatCTM(new CGAffineTransform(1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f));

				target.Target.CGContext.BeginPath();

				CGPath path = font.GetPathForGlyph(glyphs[idx]);

				if (haloRadius.HasValue && haloColor.HasValue)
				{ // also draw the halo.
					using (CGPath haloPath = path.CopyByStrokingPath(
						haloRadius.Value * 2, CGLineCap.Round, CGLineJoin.Round, 0))
					{
						SimpleColor haloSimpleColor = SimpleColor.FromArgb(haloColor.Value);
						target.Target.CGContext.SetFillColor(haloSimpleColor.R / 256.0f, haloSimpleColor.G / 256.0f, haloSimpleColor.B / 256.0f,
							haloSimpleColor.A / 256.0f);
						target.Target.CGContext.BeginPath();
						target.Target.CGContext.AddPath(haloPath);
						target.Target.CGContext.DrawPath(CGPathDrawingMode.Fill);
					}
				}

				// set the fill color as the regular text-color.
				SimpleColor simpleColor = SimpleColor.FromArgb(color);
				target.Target.CGContext.SetFillColor(simpleColor.R / 256.0f, simpleColor.G / 256.0f, simpleColor.B / 256.0f,
					simpleColor.A / 256.0f);

				// draw the text paths.
				target.Target.CGContext.BeginPath();
				target.Target.CGContext.AddPath(path);
				target.Target.CGContext.DrawPath(CGPathDrawingMode.Fill);

				//				target.Target.CGContext.AddPath (path);
				//				if (haloRadius.HasValue && haloColor.HasValue) { // also draw the halo.
				//					target.Target.CGContext.DrawPath (CGPathDrawingMode.FillStroke);
				//				} else {
				//					target.Target.CGContext.DrawPath (CGPathDrawingMode.Fill);
				//				}
				//target.Target.CGContext.ClosePath ();
				target.Target.CGContext.RestoreState();

				current = next;
			}
		}

		#endregion

	}
}


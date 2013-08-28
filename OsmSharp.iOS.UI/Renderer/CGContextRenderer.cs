// OsmSharp - OpenStreetMap tools & library.
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
using OsmSharp.UI.Renderer;
using MonoTouch.CoreGraphics;
using OsmSharp.UI;
using System.Drawing;
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreText;

namespace OsmSharp.iOS.UI
{
	public class CGContextRenderer : Renderer2D<CGContextWrapper>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.CGContextRenderer"/> class.
		/// </summary>
		public CGContextRenderer ()
		{

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
		/// Transforms the target using the specified view.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="view">View.</param>
		protected override void Transform (Target2DWrapper<CGContextWrapper> target, View2D view)
		{
			_view = view;
			_target = target;
		}

		/// <summary>
		/// Creates a wrapper for the target making it possible to drag along some properties.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public override Target2DWrapper<CGContextWrapper> CreateTarget2DWrapper (CGContextWrapper target)
		{
			return new Target2DWrapper<CGContextWrapper> (target, target.Width, target.Height);
		}

		/// <summary>
		/// Converts pixel size to scene size.
		/// </summary>
		/// <returns>The pixels.</returns>
		/// <param name="target"></param>
		/// <param name="view">View.</param>
		/// <param name="sizeInPixels">Size in pixels.</param>
		protected override double FromPixels (Target2DWrapper<CGContextWrapper> target, View2D view, double sizeInPixels)
		{
			double scaleX = target.Width / view.Width;

			return sizeInPixels / scaleX;
		}

		/// <summary>
		/// Converts scene size to pixels.
		/// </summary>
		/// <returns>The pixels.</returns>
		/// <param name="sceneSize">Scene size.</param>
		private float ToPixels(double sceneSize)
		{
			double scaleX = _target.Width / _view.Width;

			return (float)(sceneSize * scaleX) * 2;
		}

		/// <summary>
		/// Transforms the x.
		/// </summary>
		/// <returns>The x.</returns>
		/// <param name="x">The x coordinate.</param>
		private float TransformX(double x)
		{
			return (float)_view.ToViewPortX (_target.Width, x);
		}

		/// <summary>
		/// Transforms the y.
		/// </summary>
		/// <returns>The y.</returns>
		/// <param name="y">The y coordinate.</param>
		private float TransformY(double y)
		{
			return (float)_view.ToViewPortY (_target.Height, y);
		}

		/// <summary>
		/// Draws the backcolor.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="backColor"></param>
		protected override void DrawBackColor (Target2DWrapper<CGContextWrapper> target, int backColor)
		{
			SimpleColor backColorSimple = SimpleColor.FromArgb(backColor);
			target.Target.CGContext.SetFillColor (backColorSimple.R / 256.0f, backColorSimple.G / 256.0f, backColorSimple.B / 256.0f,
			                            backColorSimple.A / 256.0f);
			target.Target.CGContext.FillRect (new RectangleF (0, 0, 
			                                                target.Width, target.Height));
		}

		/// <summary>
		/// Draws a point on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		protected override void DrawPoint (Target2DWrapper<CGContextWrapper> target, double x, double y, int color, double size)
		{			

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
		protected override void DrawLine (Target2DWrapper<CGContextWrapper> target, double[] x, double[] y, int color, double width, 
		                                  LineJoin lineJoin, int[] dashes)
		{
			float widthInPixels = this.ToPixels (width);

			SimpleColor simpleColor = SimpleColor.FromArgb(color);
			target.Target.CGContext.SetLineWidth (widthInPixels);
			target.Target.CGContext.SetStrokeColor (simpleColor.R / 256.0f, simpleColor.G/ 256.0f, simpleColor.B/ 256.0f,
			                              simpleColor.A / 256.0f);
			target.Target.CGContext.SetLineDash (0, new float[0]);
			if(dashes != null && dashes.Length > 1)
			{
				float[] intervals = new float[dashes.Length];
				for(int idx = 0; idx < dashes.Length; idx++)
				{
					intervals [idx] = dashes [idx];
				}
				target.Target.CGContext.SetLineDash (0.0f, intervals);
			}

			switch(lineJoin)
			{
				case LineJoin.Bevel:
				target.Target.CGContext.SetLineJoin (CGLineJoin.Bevel);
					break;
				case LineJoin.Miter:
				target.Target.CGContext.SetLineJoin (CGLineJoin.Miter);
					break;
				case LineJoin.Round:
				target.Target.CGContext.SetLineJoin (CGLineJoin.Round);
					break;
			}
			target.Target.CGContext.BeginPath ();

			PointF[] points = new PointF[x.Length];
			for(int idx = 0; idx < x.Length; idx++)
			{
				points [idx] = new PointF (
					this.TransformX (x [idx]),
					this.TransformY (y [idx]));
			}

			target.Target.CGContext.AddLines (points);
			target.Target.CGContext.DrawPath (CGPathDrawingMode.Stroke);
			//target.Target.E
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
		protected override void DrawPolygon (Target2DWrapper<CGContextWrapper> target, double[] x, double[] y, int color, double width, 
		                                     bool fill)
		{
			float widthInPixels = this.ToPixels (width);

			SimpleColor simpleColor = SimpleColor.FromArgb(color);
			target.Target.CGContext.SetLineWidth (widthInPixels);
			target.Target.CGContext.SetFillColor (simpleColor.R / 256.0f, simpleColor.G/ 256.0f, simpleColor.B/ 256.0f,
			                              simpleColor.A / 256.0f);
			target.Target.CGContext.SetStrokeColor (simpleColor.R / 256.0f, simpleColor.G/ 256.0f, simpleColor.B/ 256.0f,
			                            simpleColor.A / 256.0f);
			//target.Target.SetLineDash (0, new float[0]);
			target.Target.CGContext.BeginPath ();

			PointF[] points = new PointF[x.Length];
			for(int idx = 0; idx < x.Length; idx++)
			{
				points [idx] = new PointF (
					this.TransformX (x [idx]),
					this.TransformY (y [idx]));
			}

			target.Target.CGContext.AddLines (points);
			target.Target.CGContext.ClosePath ();

			//target.Target.DrawPath (CGPathDrawingMode.Stroke);
			target.Target.CGContext.FillPath ();
			//target.Target.E
		}

		/// <summary>
		/// Draws an icon on the target unscaled but centered at the given scene coordinates.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="imageData"></param>
		protected override void DrawIcon (Target2DWrapper<CGContextWrapper> target, double x, double y, byte[] imageData)
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
		/// <param name="imageData"></param>
		/// <returns>The image.</returns>
		/// <param name="tag">Tag.</param>
		protected override object DrawImage (Target2DWrapper<CGContextWrapper> target, double left, double top, double right, 
		                                     double bottom, byte[] imageData, object tag)
		{
			CGImage image = null;
			CGLayer layer = null;
			if (tag != null && tag is CGImage) {
				image = (tag as CGImage);
			} else if (tag != null && tag is CGLayer) {
				layer = (tag as CGLayer);
			}
			else {
				// TODO: figurate out how to use this horroble IOS api to instantiate an image from a bytearray.
				//CGImage image = CGImage.FromPNG(
			}

			if (image != null) { // there is an image. draw it!
				float leftPixels = this.TransformX (left);
				float topPixels = this.TransformY (top);
				float rightPixels = this.TransformX (right);
				float bottomPixels = this.TransformY (bottom);

				target.Target.CGContext.DrawImage (new RectangleF (leftPixels, topPixels, (rightPixels - leftPixels),
				                                                   (bottomPixels - topPixels)), image);
			} else if (layer != null) {
				float leftPixels = this.TransformX(left);
				float topPixels = this.TransformY(top);
				float rightPixels = this.TransformX(right);
				float bottomPixels = this.TransformY(bottom);

				target.Target.CGContext.DrawLayer (layer, new RectangleF (leftPixels, topPixels, (rightPixels - leftPixels),
				                                                   (bottomPixels - topPixels)));
			}
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
		/// <param name="color">Color.</param>
		protected override void DrawText (Target2DWrapper<CGContextWrapper> target, double x, double y, string text, int color, double size,
		                                  int? haloColor, int? haloRadius)
		{
			float xPixels = this.TransformX (x);
			float yPixels = this.TransformY (y);
			float textSize = this.TransformX (x + size) - xPixels;
//
			SimpleColor simpleColor = SimpleColor.FromArgb(color);
//			target.Target.SetTextDrawingMode (CGTextDrawingMode.Fill);
//			target.Target.SetLineWidth (2.0f);
//			target.Target.SelectFont ("Helvetica", textSize, CGTextEncoding.MacRoman);
//			target.Target.SetStrokeColor (simpleColor.R / 256.0f, simpleColor.G/ 256.0f, simpleColor.B/ 256.0f,
//			                            simpleColor.A / 256.0f);
//
//			target.Target.ShowTextAtPoint (xPixels, yPixels, text);

			CGAffineTransform textTransform = new CGAffineTransform(1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f);
			target.Target.CGContext.TextMatrix = textTransform;
			target.Target.CGContext.SetFillColor (simpleColor.R / 256.0f, simpleColor.G/ 256.0f, simpleColor.B/ 256.0f,
			                            simpleColor.A / 256.0f);
			var attributedString = new NSAttributedString (text,
			                                               new CTStringAttributes{
				ForegroundColorFromContext =  true,
				Font = new CTFont ("Arial", textSize)
			});
			target.Target.CGContext.SaveState ();
			target.Target.CGContext.TranslateCTM (xPixels, yPixels);

			using (var textLine = new CTLine (attributedString)) {
				textLine.Draw (target.Target.CGContext);
			}
			target.Target.CGContext.RestoreState ();
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
		protected override void DrawLineText (Target2DWrapper<CGContextWrapper> target, double[] x, double[] y, string text, int color, 
		                                      double size, int? haloColor, int? haloRadius)
		{

		}

		#endregion
	}
}


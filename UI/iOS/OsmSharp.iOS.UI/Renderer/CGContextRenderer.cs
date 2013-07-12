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

namespace OsmSharp.iOS.UI
{
	public class CGContextRenderer : Renderer2D<CGContext>
	{
		/// <summary>
		/// Holds the bounds.
		/// </summary>
		private System.Drawing.RectangleF _bounds;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.CGContextRenderer"/> class.
		/// </summary>
		/// <param name="rect">Rect.</param>
		public CGContextRenderer (System.Drawing.RectangleF rect)
		{
			_bounds = rect;
		}

		#region implemented abstract members of Renderer2D

		/// <summary>
		/// Holds the view.
		/// </summary>
		private View2D _view;

		/// <summary>
		/// Holds the target.
		/// </summary>
		private Target2DWrapper<CGContext> _target;

		/// <summary>
		/// Transforms the target using the specified view.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="view">View.</param>
		protected override void Transform (Target2DWrapper<CGContext> target, View2D view)
		{
			_view = view;
			_target = target;
		}

		/// <summary>
		/// Creates a wrapper for the target making it possible to drag along some properties.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public override Target2DWrapper<CGContext> CreateTarget2DWrapper (CGContext target)
		{
			return new Target2DWrapper<CGContext> (target, (float)_bounds.Width, (float)_bounds.Height);
		}

		/// <summary>
		/// Converts pixel size to scene size.
		/// </summary>
		/// <returns>The pixels.</returns>
		/// <param name="target"></param>
		/// <param name="view">View.</param>
		/// <param name="sizeInPixels">Size in pixels.</param>
		protected override double FromPixels (Target2DWrapper<CGContext> target, View2D view, double sizeInPixels)
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
		protected override void DrawBackColor (Target2DWrapper<CGContext> target, int backColor)
		{
			SimpleColor backColorSimple = SimpleColor.FromArgb(backColor);
			target.Target.SetFillColor (backColorSimple.R, backColorSimple.G, backColorSimple.B,
			                           backColorSimple.A);
		}

		/// <summary>
		/// Draws a point on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		protected override void DrawPoint (Target2DWrapper<CGContext> target, double x, double y, int color, double size)
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
		protected override void DrawLine (Target2DWrapper<CGContext> target, double[] x, double[] y, int color, double width, OsmSharp.UI.Renderer.Scene2DPrimitives.LineJoin lineJoin, int[] dashes)
		{
			PointF[] points = new PointF[x.Length];
			for(int idx = 0; idx < x.Length; idx++)
			{
				points [idx] = new PointF (
					this.TransformX (x [idx]),
					this.TransformY (y [idx]));
			}

			target.Target.AddLines (points);
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
		protected override void DrawPolygon (Target2DWrapper<CGContext> target, double[] x, double[] y, int color, double width, bool fill)
		{

		}

		/// <summary>
		/// Draws an icon on the target unscaled but centered at the given scene coordinates.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="imageData"></param>
		protected override void DrawIcon (Target2DWrapper<CGContext> target, double x, double y, byte[] imageData)
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
		protected override object DrawImage (Target2DWrapper<CGContext> target, double left, double top, double right, 
		                                     double bottom, byte[] imageData, object tag)
		{
			return null;
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
		protected override void DrawText (Target2DWrapper<CGContext> target, double x, double y, string text, int color, double size)
		{

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
		protected override void DrawLineText (Target2DWrapper<CGContext> target, double[] x, double[] y, string text, int color, double size)
		{

		}

		#endregion
	}
}


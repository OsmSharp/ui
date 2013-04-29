
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using OsmSharp.UI.Map.Elements;
using OsmSharp.UI.Renderer;

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// Android canvas renderer.
	/// </summary>
	public class CanvasRenderer2D : Renderer2D<global::Android.Graphics.Canvas>
	{
		/// <summary>
		/// Holds the paint object.
		/// </summary>
		private global::Android.Graphics.Paint _paint;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.AndroidCanvasRenderer"/> class.
		/// </summary>
		/// <param name="canvas">The canvas to render on.</param>
		public CanvasRenderer2D(global::Android.Graphics.Canvas canvas)
			 :base(canvas)
		{
			_paint = new global::Android.Graphics.Paint();
		}

		#region implemented abstract members of Renderer2D

		/// <summary>
		/// Transforms the canvas to the coordinate system of the view.
		/// </summary>
		/// <param name="view">View.</param>
		protected override void Transform (View2D view)
		{
			float scaleX = this.Target.Width / view.Width;
			float scaleY = this.Target.Height / view.Height;

			// scale and translate.
			this.Target.Scale(scaleX, scaleY);
			this.Target.Translate((-view.CenterX + (view.Width / 2.0f)), 
			                      (view.CenterY + (view.Height / 2.0f)));
		}

		/// <summary>
		/// Returns the size in pixels.
		/// </summary>
		/// <returns>The pixels.</returns>
		/// <param name="view">View.</param>
		/// <param name="sizeInPixels">Size in pixels.</param>
		protected override float FromPixels (View2D view, float sizeInPixels)
		{
			float scaleX = this.Target.Width / view.Width;

			return sizeInPixels * scaleX;
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
			_paint.Color = new global::Android.Graphics.Color(color);
			_paint.StrokeWidth = 1;
			_paint.SetStyle(global::Android.Graphics.Paint.Style.Fill);

			this.Target.DrawCircle(x, -y, size,
			                  _paint);
		}

		/// <summary>
		/// Draws a line on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		protected override void DrawLine (float[] x, float[] y, int color, float width)
		{
			if(x.Length > 1)
			{
				_paint.Color = new global::Android.Graphics.Color(color);
				_paint.StrokeWidth = width;

				// convert to the weid android api array!
				float[] lineCoordinates = new float[(x.Length - 2) * 4 + 4];
				lineCoordinates[0] = x[0];
				lineCoordinates[1] = -y[0];
				for(int idx = 1; idx < x.Length - 1; idx++)
				{
					int androidApiIndex = (idx - 1) * 4 + 2;
					lineCoordinates[androidApiIndex] = x[idx];
					lineCoordinates[androidApiIndex + 1] = -y[idx];
					lineCoordinates[androidApiIndex + 2] = x[idx];
					lineCoordinates[androidApiIndex + 3] = -y[idx];
				}
				lineCoordinates[lineCoordinates.Length - 2] = x[x.Length - 1];
				lineCoordinates[lineCoordinates.Length - 1] = -y[y.Length - 1];
				this.Target.DrawLines(lineCoordinates, 0, lineCoordinates.Length, _paint);
			}
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
			if(x.Length > 1)
			{
				_paint.Color = new global::Android.Graphics.Color(color);
				_paint.StrokeWidth = width;
				if(fill)
				{
					_paint.SetStyle(global::Android.Graphics.Paint.Style.Fill);
				}
				else					
				{
					_paint.SetStyle(global::Android.Graphics.Paint.Style.Stroke);
				}

				// convert android path object.
				global::Android.Graphics.Path path = new global::Android.Graphics.Path();
				path.MoveTo(x[0], -y[0]);
				for(int idx = 1; idx < x.Length; idx++)
				{
					path.LineTo(x[idx], -y[idx]);
				}
				path.LineTo(x[0], -y[0]);

				this.Target.DrawPath(path, _paint);
			}
		}

		#endregion
	}
}
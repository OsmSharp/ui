using System;
using System.Runtime.InteropServices;

using Android.Views;
using Android.Util;
using Android.Content;
using Android.Opengl;
using Java.Nio;
using Java.Lang;
using OsmSharp.UI.Renderer;
using OsmSharp.UI;
using OsmSharp.Math;

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// OpenGLRenderer2D 
	/// </summary>
	public class OpenGLRenderer2D : Renderer2D<OpenGLTarget2D>
	{
		/// <summary>
		/// Holds the view.
		/// </summary>
		private View2D _view;

		/// <summary>
		/// Holds the target.
		/// </summary>
		private Target2DWrapper<OpenGLTarget2D> _target;

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
			return (float)_view.ToViewPortY(_target.Height, y);
		}

		/// <summary>
		/// Returns the size in pixels.
		/// </summary>
		/// <returns></returns>
		private float ToPixels(double sceneSize)
		{
			double scaleX = _target.Width / _view.Width;

			return (float)(sceneSize * scaleX) * 2;
		}

		#region implemented abstract members of Renderer2D

		public override Target2DWrapper<OpenGLTarget2D> CreateTarget2DWrapper (OpenGLTarget2D target)
		{
			_target = new Target2DWrapper<OpenGLTarget2D> (target, 
			                                              target.Width,
			                                              target.Height);
			return _target;
		}

		protected override double FromPixels (Target2DWrapper<OpenGLTarget2D> target, View2D view, double sizeInPixels)
		{
			double scaleX = target.Width / view.Width;

			return sizeInPixels / scaleX;
		}

		protected override void Transform (Target2DWrapper<OpenGLTarget2D> target, View2D view)
		{
			_view = view;
			_target = target;

			_target.Target.SetOrtho((float)_view.Left, (float)_view.Right, 
			                        (float)_view.Bottom, (float)_view.Top);
		}

		protected override void DrawBackColor (Target2DWrapper<OpenGLTarget2D> target, int backColor)
		{

		}

		protected override void DrawPoint (Target2DWrapper<OpenGLTarget2D> target, double x, double y, int color, double size)
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
		protected override void DrawLine (Target2DWrapper<OpenGLTarget2D> target, double[] x, double[] y, int color, 
		                                  double width, OsmSharp.UI.Renderer.Scene2DPrimitives.LineJoin lineJoin, int[] dashes)
		{
			float[] points = new float[x.Length * 3];
			for(int idx = 0; idx < x.Length; idx++)
			{
				int pathIdx = idx * 3;
				points [pathIdx + 0] = (float)x [idx];
				points [pathIdx + 1] = (float)y [idx];
				points [pathIdx + 2] = 0;
			}

			_target.Target.AddLine(points, this.ToPixels(width), color);

//			double epsilon = 0.00001; // define this value properly.
//			//double semiwidth = this.ToPixels(width) / 2.0;
//			double semiwidth = width / 2.0;
//
//			float[] path = new float[(x.Length - 1) * 2 * 3 * 2];
//			for(int idx = 0; idx < x.Length - 1; idx++)
//			{
////				double x1 = this.TransformX (x[idx]);
////				double x2 = this.TransformX (x[idx + 1]);
////				double y1 = this.TransformY (y[idx]);
////				double y2 = this.TransformY (y[idx + 1]);
//
//				double x1 = x[idx];
//				double x2 = x[idx + 1];
//				double y1 = y[idx];
//				double y2 = y[idx + 1];
//
//				PointF2D p1 = new PointF2D (x1, y1);
//				PointF2D p2 = new PointF2D (x2, y2);
//				// calculate the direction of the current line-segment.
//				VectorF2D vector = p2 - p1;
//				if(vector.Size > epsilon)
//				{ // anything below this value will not be feasible to calculate or visible on screen.
//					vector = vector.Normalize ();
//
//					// rotate counter-clockwize and mutliply with width.
//					VectorF2D ccw1 = vector.Rotate90 (false) * semiwidth;
//					PointF2D p1top = p1 + ccw1;
//					PointF2D p1bottom = p1 - ccw1;
//
//					// invert vector.
//					vector = vector.Inverse;
//					ccw1 = vector.Rotate90 (true) * semiwidth;
//					PointF2D p2top = p2 + ccw1;
//					PointF2D p2bottom = p2 - ccw1;
//
//					// make triangles out of this.
//					int pathIdx = idx * 2 * 3 * 2;
////					if(idx % 2 == 0)
////					{
//					path [pathIdx + 0] = (float)p1bottom [0];
//					path [pathIdx + 1] = (float)p1bottom [1];
//					path [pathIdx + 2] = 0;
//					path [pathIdx + 3] = (float)p1top [0];
//					path [pathIdx + 4] = (float)p1top [1];
//					path [pathIdx + 5] = 0;
//					
//					path [pathIdx + 0 + 6] = (float)p2bottom [0];
//					path [pathIdx + 1 + 6] = (float)p2bottom [1];
//					path [pathIdx + 2 + 6] = 0;
//					path [pathIdx + 3 + 6] = (float)p2top [0];
//					path [pathIdx + 4 + 6] = (float)p2top [1];
//					path [pathIdx + 5 + 6] = 0;
////					}
////					else
////					{
////						path [pathIdx + 0] = (float)p1top [0];
////						path [pathIdx + 1] = (float)p1top [1];
////						path [pathIdx + 2] = 0;
////						path [pathIdx + 3] = (float)p1bottom [0];
////						path [pathIdx + 4] = (float)p1bottom [1];
////						path [pathIdx + 5] = 0;
////					}
//				}
//			}
//			target.Target.AddTriangles(path, color);
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
		protected override void DrawPolygon (Target2DWrapper<OpenGLTarget2D> target, double[] x, double[] y, 
		                                     int color, double width, bool fill)
		{
			float[] points = new float[x.Length * 3];
			for(int idx = 0; idx < x.Length; idx++)
			{
				int pathIdx = idx * 3;
				points [pathIdx + 0] = (float)x [idx];
				points [pathIdx + 1] = (float)y [idx];
				points [pathIdx + 2] = 0;
			}

			_target.Target.AddLine(points, 1, color);

//			for(int idx = 0; idx < 3; idx++)
//			{
//				float[] path = new float[3 * 3];
//				path [0] = this.TransformX (x[idx]);
//				path [1] = this.TransformY (y[idx] + 2);
//				path [2] = 0;
//				path [3] = this.TransformX (x[idx]);
//				path [4] = this.TransformY (y[idx]);
//				path [5] = 0;
//				path [6] = this.TransformX (x[idx] + 2);
//				path [7] = this.TransformY (y[idx] + 2);
//				path [8] = 0;
//
//				target.Target.AddTriangles(path, color);
//			}
		}

		protected override void DrawIcon (Target2DWrapper<OpenGLTarget2D> target, double x, double y, byte[] imageData)
		{

		}

		protected override object DrawImage (Target2DWrapper<OpenGLTarget2D> target, double left, double top, double right, double bottom, byte[] imageData, object tag)
		{
			return null;
		}

		protected override void DrawText (Target2DWrapper<OpenGLTarget2D> target, double x, double y, string text, int color, double size)
		{

		}

		protected override void DrawLineText (Target2DWrapper<OpenGLTarget2D> target, double[] x, double[] y, string text, int color, double size)
		{

		}

		#endregion


	}
}
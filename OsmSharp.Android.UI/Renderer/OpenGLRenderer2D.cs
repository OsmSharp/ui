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

using OsmSharp.Math;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;

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

        private float _z;

        /// <summary>
        /// Called before rendering starts.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="scenes"></param>
        /// <param name="view"></param>
        protected override void OnBeforeRender(Target2DWrapper<OpenGLTarget2D> target, System.Collections.Generic.List<OsmSharp.UI.Renderer.Scene.Scene2D> scenes, View2D view)
        {
            base.OnBeforeRender(target, scenes, view);

            _target.Target.Clear();
            _z = 0.0001f;
        }

		/// <summary>
		/// Transforms the y-coordinate to screen coordinates.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private double[] Tranform(double x, double y)
		{
			return _view.ToViewPort(_target.Width, _target.Height, x, y);
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

			_target.Target.SetOrtho((float)_view.LeftTop[0], (float)_view.RightTop[0], 
			                        (float)_view.LeftBottom[1], (float)_view.LeftTop[1]);
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
		                                  double width, LineJoin lineJoin, int[] dashes)
		{
            _z = _z + 0.0001f;

            float[] points = new float[x.Length * 3];
            for (int idx = 0; idx < x.Length; idx++)
            {
                int pathIdx = idx * 3;
                points[pathIdx + 0] = (float)x[idx];
                points[pathIdx + 1] = (float)y[idx];
                points[pathIdx + 2] = _z;
            }

            _target.Target.AddLine(points, this.ToPixels(width), color);

            //double epsilon = 0.00001; // define this value properly.
            ////double semiwidth = this.ToPixels(width) / 2.0;
            //double semiwidth = width / 2.0;

            //float[] path = new float[(x.Length - 1) * 2 * 3 * 2];
            //for (int idx = 0; idx < x.Length - 1; idx++)
            //{
            //    //				double x1 = this.TransformX (x[idx]);
            //    //				double x2 = this.TransformX (x[idx + 1]);
            //    //				double y1 = this.TransformY (y[idx]);
            //    //				double y2 = this.TransformY (y[idx + 1]);

            //    double x1 = x[idx];
            //    double x2 = x[idx + 1];
            //    double y1 = y[idx];
            //    double y2 = y[idx + 1];

            //    PointF2D p1 = new PointF2D(x1, y1);
            //    PointF2D p2 = new PointF2D(x2, y2);
            //    // calculate the direction of the current line-segment.
            //    VectorF2D vector = p2 - p1;
            //    if (vector.Size > epsilon)
            //    { // anything below this value will not be feasible to calculate or visible on screen.
            //        vector = vector.Normalize();

            //        // rotate counter-clockwize and mutliply with width.
            //        VectorF2D ccw1 = vector.Rotate90(false) * semiwidth;
            //        PointF2D p1top = p1 + ccw1;
            //        PointF2D p1bottom = p1 - ccw1;

            //        // invert vector.
            //        vector = vector.Inverse;
            //        ccw1 = vector.Rotate90(true) * semiwidth;
            //        PointF2D p2top = p2 + ccw1;
            //        PointF2D p2bottom = p2 - ccw1;

            //        // make triangles out of this.
            //        int pathIdx = idx * 2 * 3 * 2;
            //        //					if(idx % 2 == 0)
            //        //					{
            //        path[pathIdx + 0] = (float)p1bottom[0];
            //        path[pathIdx + 1] = (float)p1bottom[1];
            //        path[pathIdx + 2] = 0;
            //        path[pathIdx + 3] = (float)p1top[0];
            //        path[pathIdx + 4] = (float)p1top[1];
            //        path[pathIdx + 5] = 0;

            //        path[pathIdx + 0 + 6] = (float)p2bottom[0];
            //        path[pathIdx + 1 + 6] = (float)p2bottom[1];
            //        path[pathIdx + 2 + 6] = 0;
            //        path[pathIdx + 3 + 6] = (float)p2top[0];
            //        path[pathIdx + 4 + 6] = (float)p2top[1];
            //        path[pathIdx + 5 + 6] = 0;
            //        //					}
            //        //					else
            //        //					{
            //        //						path [pathIdx + 0] = (float)p1top [0];
            //        //						path [pathIdx + 1] = (float)p1top [1];
            //        //						path [pathIdx + 2] = 0;
            //        //						path [pathIdx + 3] = (float)p1bottom [0];
            //        //						path [pathIdx + 4] = (float)p1bottom [1];
            //        //						path [pathIdx + 5] = 0;
            //        //					}
            //    }
            //}
            //target.Target.AddTriangles(path, color);
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
            for (int idx = 0; idx < x.Length; idx++)
            {
                int pathIdx = idx * 3;
                points[pathIdx + 0] = (float)x[idx];
                points[pathIdx + 1] = (float)y[idx];
                points[pathIdx + 2] = 0;
            }

            _target.Target.AddLine(points, 1, color);

            for (int idx = 0; idx < 3; idx++)
            {
                float[] path = new float[3 * 3];
                double[] transformedPoints = this.Tranform(x[idx], y[idx] + 2);
                path[0] = (float)transformedPoints[0];
                path[1] = (float)transformedPoints[1];
                path[2] = 0;
                transformedPoints = this.Tranform(x[idx], y[idx]);
                path[3] = (float)transformedPoints[0];
                path[4] = (float)transformedPoints[1];
                path[5] = 0;
                transformedPoints = this.Tranform(x[idx], y[idx]);
                path[6] = (float)transformedPoints[0];
                path[7] = (float)transformedPoints[1];
                path[8] = 0;

                target.Target.AddTriangles(path, color);
            }
		}

		protected override void DrawIcon (Target2DWrapper<OpenGLTarget2D> target, double x, double y, byte[] imageData)
		{

		}

		protected override object DrawImage (Target2DWrapper<OpenGLTarget2D> target, double left, double top, double right, double bottom, byte[] imageData, object tag)
		{
			return null;
		}

		protected override object DrawImage (Target2DWrapper<OpenGLTarget2D> target, OsmSharp.Math.Primitives.RectangleF2D bounds, byte[] imageData, object tag)
		{
			return tag;
		}

		protected override void DrawText (Target2DWrapper<OpenGLTarget2D> target, double x, double y, string text, int color, 
		                                  double size, int? haloColor, int? haloRadius, string fontName)
		{

		}

		protected override void DrawLineText (Target2DWrapper<OpenGLTarget2D> target, double[] x, double[] y, string text, int color, 
		                                      double size, int? haloColor, int? haloRadius, string fontName)
		{

		}

		#endregion


	}
}
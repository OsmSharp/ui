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
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using OsmSharp.UI.Renderer;
using System.IO;
using OsmSharp.Math.Primitives;
using OsmSharp.Math;
using Android.Graphics;
using OsmSharp.Units.Angle;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.Android.UI.Renderer;
using OsmSharp.Android.UI.Renderer.Images;
using System.Threading;

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
        /// Creates a new canvas renderer.
        /// </summary>
        public CanvasRenderer2D()
            : this(1)
        {

        }

        /// <summary>
        /// Creates a new canvas renderer.
        /// </summary>
        /// <param name="density"></param>
		public CanvasRenderer2D(float density)
		{
            this.Density = density;

			_paint = new global::Android.Graphics.Paint();
			_paint.AntiAlias = true;
			_paint.Dither = true;
			_paint.StrokeJoin = global::Android.Graphics.Paint.Join.Round;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.CanvasRenderer2D"/> class.
		/// </summary>
		/// <param name="paint">Paint.</param>
		public CanvasRenderer2D(global::Android.Graphics.Paint paint)
		{
			_paint = paint;
		}
		
		#region Caching Implementation

		/// <summary>
		/// Holds a reusable path.
		/// </summary>
		private global::Android.Graphics.Path _path;

		/// <summary>
		/// Holds the current view.
		/// </summary>
		private View2D _view;

		/// <summary>
		/// Called right before rendering starts.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="view"></param>
		protected override void OnBeforeRender(Target2DWrapper<global::Android.Graphics.Canvas> target, View2D view)
		{
            if(_path == null)
            { // a path to work with does not exist yet.
                _path = new global::Android.Graphics.Path();
            }
            _path.Reset();
		}
		
		/// <summary>
		/// Called right after rendering.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="view"></param>
		protected override void OnAfterRender(Target2DWrapper<global::Android.Graphics.Canvas> target, View2D view)
		{
            _wrapper = null;
            _target = null;
            _view = null;

            // TODO: figure out why this request to the GC has to keep being here.
            // when this is removed things are obviously faster but there is a memory leak.
            // GC.Collect();
            // Thread.Sleep(150);
		}
		
		#endregion

		#region implemented abstract members of Renderer2D

        /// <summary>
        /// Holds the wrapper.
        /// </summary>
        private Target2DWrapper<global::Android.Graphics.Canvas> _wrapper;

		/// <summary>
		/// Returns the target wrapper.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public override Target2DWrapper<global::Android.Graphics.Canvas> CreateTarget2DWrapper (global::Android.Graphics.Canvas target)
		{
            if(_wrapper == null)
            { // create the wrapper.
                _wrapper = new Target2DWrapper<global::Android.Graphics.Canvas>(
				    target, target.Width, target.Height);
            }
            else
            { // updated the exist wrapper.
                _wrapper.Width = target.Width;
                _wrapper.Height = target.Height;
                _wrapper.Target = target;
            }
            return _wrapper;
		}

		/// <summary>
		/// Holds the target.
		/// </summary>
		private Target2DWrapper<global::Android.Graphics.Canvas> _target;

        /// <summary>
        /// Holds the to view port.
        /// </summary>
        private Matrix2D _toViewPort;

        /// <summary>
        /// Holds the from view port.
        /// </summary>
        private Matrix2D _fromViewPort;

		/// <summary>
		/// Transforms the canvas to the coordinate system of the view.
		/// </summary>
        /// <param name="view"></param>
        /// <param name="target"></param>
		protected override void Transform (Target2DWrapper<global::Android.Graphics.Canvas> target, View2D view)
		{
			_view = view;
			_target = target;

            _toViewPort = _view.CreateToViewPort(_target.Width, _target.Height);
            _fromViewPort = _view.CreateFromViewPort(_target.Width, _target.Height);
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
		protected override double FromPixels(Target2DWrapper<global::Android.Graphics.Canvas> target, View2D view, double sizeInPixels)
		{
			double scaleX = target.Width / view.Width;
			
			return sizeInPixels / scaleX;
		}

		/// <summary>
		/// Draws the backcolor.
		/// </summary>
		/// <param name="backColor"></param>
		protected override void DrawBackColor (Target2DWrapper<global::Android.Graphics.Canvas> target, int backColor)
		{
			target.Target.DrawColor(new global::Android.Graphics.Color(backColor));
		}

		/// <summary>
		/// Draws a point on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		protected override void DrawPoint (Target2DWrapper<global::Android.Graphics.Canvas> target, double x, double y, int color, double size)
		{
			float sizeInPixels = this.ToPixels(size) * this.Density;
			_paint.Color = new global::Android.Graphics.Color(color);
			_paint.StrokeWidth = 1;
			_paint.SetStyle(global::Android.Graphics.Paint.Style.Fill);

            double transformedX, transformedY;
            _toViewPort.Apply(x, y, out transformedX, out transformedY);
            target.Target.DrawCircle((float)transformedX, (float)transformedY, sizeInPixels, _paint);
		}

		/// <summary>
		/// Draws a line on the target. The coordinates given are scene coordinates.
		/// </summary>
		protected override void DrawLine (Target2DWrapper<global::Android.Graphics.Canvas> target, double[] x, double[] y, 
		                                  int color, double width, LineJoin lineJoine, int[] dashes)
		{
            double transformed1_0, transformed1_1;
            if(x.Length > 1)
			{
				_paint.AntiAlias = true;
				_paint.SetStyle(global::Android.Graphics.Paint.Style.Stroke);
				if(dashes != null && dashes.Length > 0)
				{ // set the dashes effect.
					float[] intervals = new float[dashes.Length];
					for(int idx = 0; idx < dashes.Length; idx++)
					{
						intervals [idx] = dashes [idx];
					}
					_paint.SetPathEffect(
						new global::Android.Graphics.DashPathEffect(
							intervals, 0));
				}

				float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
				float xT, yT;

				// convert to the weid android api array!
				_path.Rewind ();
				// this.Transform (x [0], y [0], _transformed1);
                _toViewPort.Apply(x[0], y[0], out transformed1_0, out transformed1_1);
                xT = (float)transformed1_0;
                yT = (float)transformed1_1;
				_path.MoveTo (xT, yT);
				if (xT < minX) { minX = xT; }
				if (xT > maxX) { maxX = xT; }
				if (yT < minY) { minY = yT; }
				if (yT > maxY) { maxY = yT; }
				for(int idx = 1; idx < x.Length; idx++)
				{
                    // this.Transform (x [idx], y [idx], _transformed1);
                    _toViewPort.Apply(x[idx], y[idx], out transformed1_0, out transformed1_1);
                    xT = (float)transformed1_0;
                    yT = (float)transformed1_1;
					_path.LineTo (xT, yT);
					if (xT < minX) { minX = xT; }
					if (xT > maxX) { maxX = xT; }
					if (yT < minY) { minY = yT; }
					if (yT > maxY) { maxY = yT; }
				}
				if ((maxX - minX) > 1 || (maxY - minY) > 1) {
                    float widthInPixels = this.ToPixels(width) * this.Density;
					_paint.Color = new global::Android.Graphics.Color (color);
					_paint.StrokeWidth = widthInPixels;
					target.Target.DrawPath (_path, _paint);
				}
				_paint.Reset ();
			}
		}

		/// <summary>
		/// Draws a polygon on the target. The coordinates given are scene coordinates.
		/// </summary>
		protected override void DrawPolygon (Target2DWrapper<global::Android.Graphics.Canvas> target, double[] x, double[] y, 
		                                     int color, double width, bool fill)
		{
            double transformed1_0, transformed1_1;
			if(x.Length > 1)
			{
				_paint.Color = new global::Android.Graphics.Color(color);
                _paint.StrokeWidth = this.ToPixels(width) * this.Density;
				if(fill)
				{
					_paint.SetStyle(global::Android.Graphics.Paint.Style.Fill);
				}
				else					
				{
					_paint.SetStyle(global::Android.Graphics.Paint.Style.Stroke);
				}

				float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
				float xT, yT;

				// convert android path object.
				_path.Rewind ();
				// this.Transform (x [0], y [0], _transformed1);
                _toViewPort.Apply(x[0], y[0], out transformed1_0, out transformed1_1);
                xT = (float)transformed1_0;
                yT = (float)transformed1_1;
				_path.MoveTo (xT, yT);
				if (xT < minX) { minX = xT; }
				if (xT > maxX) { maxX = xT; }
				if (yT < minY) { minY = yT; }
				if (yT > maxY) { maxY = yT; }
				for(int idx = 1; idx < x.Length; idx++)
				{
                    // this.Transform(x[idx], y[idx], _transformed1);
                    _toViewPort.Apply(x[idx], y[idx], out transformed1_0, out transformed1_1);
                    xT = (float)transformed1_0;
                    yT = (float)transformed1_1;
					_path.LineTo (xT, yT);
					if (xT < minX) { minX = xT; }
					if (xT > maxX) { maxX = xT; }
					if (yT < minY) { minY = yT; }
					if (yT > maxY) { maxY = yT; }
				}
                // this.Transform(x[0], y[0], _transformed1);
                _toViewPort.Apply(x[0], y[0], out transformed1_0, out transformed1_1);
                xT = (float)transformed1_0;
                yT = (float)transformed1_1;
				_path.LineTo (xT, yT);
				if (xT < minX) { minX = xT; }
				if (xT > maxX) { maxX = xT; }
				if (yT < minY) { minY = yT; }
				if (yT > maxY) { maxY = yT; }
				
				if ((maxX - minX) > 5 && (maxY - minY) > 5) {
					target.Target.DrawPath(_path, _paint);
				}
			}
		}

		/// <summary>
		/// Draws an icon on the target unscaled but centered at the given scene coordinates.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="imageData"></param>
		protected override void DrawIcon (Target2DWrapper<global::Android.Graphics.Canvas> target, double x, double y, 
		                                  byte[] imageData)
		{
            //global::Android.Graphics.Bitmap image = global::Android.Graphics.BitmapFactory.DecodeByteArray(
            //    imageData, 0, imageData.Length);
			
            //double[] transformed = this.Transform (x, y);

            //target.Target.DrawBitmap(image, (float)transformed [0], (float)transformed [1], null);
            //image.Dispose();
		}

		/// <summary>
		/// Draws an image on the target.
		/// </summary>
		protected override void DrawImage (Target2DWrapper<global::Android.Graphics.Canvas> target, 
		    double left, double top, double right, double bottom, INativeImage nativeImage)
		{
            var rectangle = new RectangleF2D(left, top, right - left, bottom - top);
            this.DrawImage(target, rectangle, nativeImage);
		}

		/// <summary>
		/// Draws the image.
		/// </summary>
        protected override void DrawImage(Target2DWrapper<Canvas> target, RectangleF2D bounds, INativeImage nativeImage)
		{
            double transformed1_0, transformed1_1;
            var nativeAndroidImage = (nativeImage as NativeImage);
            global::Android.Graphics.Bitmap image = nativeAndroidImage.Image;
			// this.Transform(bounds.BottomLeft[0], bounds.BottomLeft[1], _transformed1);
            _toViewPort.Apply(bounds.BottomLeft[0], bounds.BottomLeft[1], out transformed1_0, out transformed1_1);
			var bottomLeft = new PointF2D(transformed1_0, transformed1_1);
            // this.Transform(bounds.BottomRight[0], bounds.BottomRight[1], _transformed1);
            _toViewPort.Apply(bounds.BottomRight[0], bounds.BottomRight[1], out transformed1_0, out transformed1_1);
			var bottomRight = new PointF2D(transformed1_0, transformed1_1);
            // this.Transform(bounds.TopLeft[0], bounds.TopLeft[1], _transformed1);
            _toViewPort.Apply(bounds.TopLeft[0], bounds.TopLeft[1], out transformed1_0, out transformed1_1);
			var topLeft = new PointF2D(transformed1_0, transformed1_1);
			//PointF2D topRight = new PointF2D(this.Tranform (bounds.TopRight [0], bounds.TopRight [1])); 

			var transformed = new RectangleF2D(bottomLeft, bottomLeft.Distance(bottomRight), bottomLeft.Distance(topLeft), 
			                                            topLeft - bottomLeft);
            _paint.AntiAlias = true;
            _paint.FilterBitmap = true;
			target.Target.Save ();
			target.Target.Translate ((float)transformed.BottomLeft [0], (float)transformed.BottomLeft [1]);
			target.Target.Rotate (-(float)((Degree)transformed.Angle).Value);
            if(image != null)
            {
                target.Target.DrawBitmap(image,
                    new global::Android.Graphics.Rect(0, 0, image.Width, image.Height),
                    new global::Android.Graphics.RectF(0, 0, (float)transformed.Width, (float)transformed.Height),
                    _paint);
            }
			target.Target.Restore ();
            
		}

		/// <summary>
		/// Draws text.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="text"></param>
		/// <param name="size"></param>
		protected override void DrawText (Target2DWrapper<global::Android.Graphics.Canvas> target, double x, double y, 
		                                  string text, int color, double size, int? haloColor, int? haloRadius, string fontName)
		{
            double transformed1_0, transformed1_1;
			// this.Transform(x, y, _transformed1);
            _toViewPort.Apply(x, y, out transformed1_0, out transformed1_1);
            float xPixels = (float)transformed1_0;
            float yPixels = (float)transformed1_1;
            float textSize = this.ToPixels(size) * this.Density;

			_paint.AntiAlias = true;
			_paint.SubpixelText = true;
			_paint.TextSize = textSize;

			// get some metrics on the texts.
			float[] characterWidths = new float[text.Length];
			_paint.GetTextWidths (text, characterWidths);
			var characterHeight = (float)size;
			var textLength = characterWidths.Sum();

			// center is default.
			xPixels = xPixels - (textLength / 2.0f);

			var current = new PointF2D (xPixels, yPixels);
			for (int idx = 0; idx < text.Length; idx++)
			{
				char currentChar = text[idx];
				global::Android.Graphics.Path characterPath = new global::Android.Graphics.Path ();
				_paint.GetTextPath (text, idx, idx + 1, 0, 0, characterPath);
				using (characterPath)
				{
					// Transformation matrix to move the character to the correct location. 
					// Note that all actions on the Matrix class are prepended, so we apply them in reverse.
					var transform = new Matrix();

					// Translate to the final position, the center of line-segment between 'current' and 'next'
					var position = current;
					//transformed = this.Tranform(position[0], position[1]);
                    transform.SetTranslate((float)position[0], (float)position[1]);

					// Translate the character so the centre of its base is over the origin
					transform.PreTranslate(0, characterHeight / 2.5f);

					//transform.Scale((float)this.FromPixels(_target, _view, 1), 
					//    (float)this.FromPixels(_target, _view, 1));
					characterPath.Transform(transform);

					if (haloColor.HasValue && haloRadius.HasValue && haloRadius.Value > 0)
					{
						_paint.SetStyle (global::Android.Graphics.Paint.Style.FillAndStroke);
						_paint.StrokeWidth = haloRadius.Value;
						_paint.Color = new global::Android.Graphics.Color(haloColor.Value);
						using (global::Android.Graphics.Path haloPath = new global::Android.Graphics.Path())
						{
							_paint.GetFillPath (characterPath, haloPath);
							// Draw the character
							target.Target.DrawPath(haloPath, _paint);
						}
					}

					// Draw the character
					_paint.SetStyle (global::Android.Graphics.Paint.Style.Fill);
					_paint.StrokeWidth = 0;
					_paint.Color = new global::Android.Graphics.Color(color);
					target.Target.DrawPath(characterPath, _paint);
				}
				current = new PointF2D(current[0] + characterWidths[idx], current[1]);
			}
		}

		/// <summary>
		/// Draws text along a given line.
		/// </summary>
		protected override void DrawLineText (Target2DWrapper<global::Android.Graphics.Canvas> target, double[] xa, double[] ya, string text, int color, 
		                                      double size, int? haloColor, int? haloRadius, string fontName)
		{
            double transformed1_0, transformed1_1;
			if (xa.Length > 1)
			{
                float sizeInPixels = this.ToPixels(size) * this.Density;	
				_paint.SubpixelText = true;
				_paint.TextSize = (float)sizeInPixels;
				_paint.AntiAlias = true;

				// transform first.
				double[] xTransformed = new double[xa.Length];
				double[] yTransformed = new double[ya.Length];
				for (int idx = 0; idx < xa.Length; idx++) {
					// this.Transform (xa[idx], ya[idx], _transformed1);
                    _toViewPort.Apply(xa[idx], ya[idx], out transformed1_0, out transformed1_1);
                    xTransformed[idx] = transformed1_0;
                    yTransformed[idx] = transformed1_1;
				}

				// get some metrics on the texts.
				float[] characterWidths = new float[text.Length];
				_paint.GetTextWidths (text, characterWidths);
                var characterHeight = (float)sizeInPixels;
				var textLength = characterWidths.Sum();

				// calculate line length.
				var lineLength = Polyline2D.Length(xTransformed, yTransformed);
				if (lineLength > textLength * 1.2f)
				{
					// calculate the number of labels.
					int labelCount = (int)System.Math.Floor(lineLength / (textLength * 10)) + 1;

					// calculate positions of label(s).
					double positionGap = lineLength / (labelCount + 1);

					// draw each label.
					for (double position = positionGap; position < lineLength; position = position + positionGap)
					{
						this.DrawLineTextSegment(target, xTransformed, yTransformed, text, color, size, haloColor, haloRadius, position, characterWidths,
						                         textLength, characterHeight);
					}
				}
			}
		}

		/// <summary>
		/// Draws the line text segment.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="xTransformed">X transformed.</param>
		/// <param name="yTransformed">Y transformed.</param>
		/// <param name="text">Text.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		/// <param name="haloColor">Halo color.</param>
		/// <param name="haloRadius">Halo radius.</param>
		/// <param name="middlePosition">Middle position.</param>
		/// <param name="characterWidths">Character widths.</param>
		/// <param name="textLength">Text length.</param>
		/// <param name="characterHeight">Character height.</param>
		public void DrawLineTextSegment(Target2DWrapper<global::Android.Graphics.Canvas> target, double[] xTransformed, double[] yTransformed, string text, int color,
		                                double size, int? haloColor, int? haloRadius, double middlePosition, float[] characterWidths, double textLength,
		                                float characterHeight)
		{
			_paint.Color = new global::Android.Graphics.Color(color);
			_paint.SetStyle (global::Android.Graphics.Paint.Style.Fill);

			// see if the text is 'upside down'.
			double averageAngle = 0;
			double first = middlePosition - (textLength / 2.0);
			PointF2D current = Polyline2D.PositionAtPosition(xTransformed, yTransformed, first);
			bool isVisible = false;
			for (int idx = 0; idx < text.Length; idx++)
			{
				double nextPosition = middlePosition - (textLength / 2.0) + ((textLength / (text.Length)) * (idx + 1));
				PointF2D next = Polyline2D.PositionAtPosition(xTransformed, yTransformed, nextPosition);

				// calculate the angle.
				VectorF2D vector = next - current;
				VectorF2D horizontal = new VectorF2D(1, 0);
				double angleDegrees = ((Degree)horizontal.Angle(vector)).Value;
				averageAngle = averageAngle + angleDegrees;
				current = next;

                double untransformed_0, untransformed_1;
                _fromViewPort.Apply(next[0], next[1], out untransformed_0, out untransformed_1);
				// double[] untransformed = this.TransformReverse (next [0], next [1]);
                if (_view.Contains(untransformed_0, untransformed_1))
                {
					isVisible = true;
				}
			}
			averageAngle = averageAngle / text.Length;

			if (!isVisible) {
				return;
			}

			// reverse if 'upside down'.
			double[] xText = xTransformed;
			double[] yText = yTransformed;

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
				global::Android.Graphics.Path characterPath = new global::Android.Graphics.Path ();;
				_paint.GetTextPath (text, idx, idx + 1, 0, 0, characterPath);
				using (characterPath) {
					// Transformation matrix to move the character to the correct location. 
					// Note that all actions on the Matrix class are prepended, so we apply them in reverse.
					using (var transform = new Matrix ()) {

						// Translate to the final position, the center of line-segment between 'current' and 'next'
						PointF2D position = current;
						//PointF2D position = current + ((next - current) / 2.0);
						//double[] transformed = this.Tranform(position[0], position[1]);
						transform.SetTranslate ((float)position [0], (float)position [1]);

						// calculate the angle.
						VectorF2D vector = next - current;
						VectorF2D horizontal = new VectorF2D (1, 0);
						double angleDegrees = ((Degree)horizontal.Angle (vector)).Value;

						// Rotate the character
						transform.PreRotate ((float)angleDegrees);

						// Translate the character so the centre of its base is over the origin
						transform.PreTranslate (0, characterHeight / 2.5f);

						//transform.Scale((float)this.FromPixels(_target, _view, 1), 
						//    (float)this.FromPixels(_target, _view, 1));
						characterPath.Transform (transform);

						if (haloColor.HasValue && haloRadius.HasValue && haloRadius.Value > 0) {
							_paint.SetStyle (global::Android.Graphics.Paint.Style.FillAndStroke);
							_paint.StrokeWidth = haloRadius.Value;
							_paint.Color = new global::Android.Graphics.Color (haloColor.Value);
							using (global::Android.Graphics.Path haloPath = new global::Android.Graphics.Path ()) {
								_paint.GetFillPath (characterPath, haloPath);
								// Draw the character
								target.Target.DrawPath (haloPath, _paint);
							}
						}

						// Draw the character
						_paint.SetStyle (global::Android.Graphics.Paint.Style.Fill);
						_paint.StrokeWidth = 0;
						_paint.Color = new global::Android.Graphics.Color (color);
						target.Target.DrawPath (characterPath, _paint);
					}
				}
				current = next;
			}
		}

        /// <summary>
        /// Draws arrows along a line.
        /// </summary>
        protected override void DrawArrowsAlongLine(Target2DWrapper<Canvas> target, double[] x, double[] y, int color, double width, int[] dashes)
        {
            this.DrawLine(target, x, y, color, width, LineJoin.None, dashes);
        }

        #endregion
    }
}
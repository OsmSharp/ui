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
		public CanvasRenderer2D()
		{
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

//		/// <summary>
//		/// Holds the bitmap cache.
//		/// </summary>
//		private global::Android.Graphics.Bitmap _cache;
		
		/// <summary>
		/// Called right before rendering starts.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="scenes"></param>
		/// <param name="view"></param>
		protected override void OnBeforeRender(Target2DWrapper<global::Android.Graphics.Canvas> target, View2D view)
		{
//			if (_cache == null || _cache.Width != target.Width || _cache.Height != target.Height) {
//				// create a bitmap and render there.
//				_cache = global::Android.Graphics.Bitmap.CreateBitmap ((int)target.Width, (int)target.Height,
//				                                                       global::Android.Graphics.Bitmap.Config.Argb8888);
//			} else {
//				// clear the cache???
//			}
//			target.BackTarget = target.Target;
//			target.Target = new global::Android.Graphics.Canvas(_cache);
//			target.Target.DrawColor(global::Android.Graphics.Color.Transparent);
//			
//			target.Tag = _cache;
			_path = new global::Android.Graphics.Path ();
		}
		
		/// <summary>
		/// Called right after rendering.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="scenes"></param>
		/// <param name="view"></param>
		protected override void OnAfterRender(Target2DWrapper<global::Android.Graphics.Canvas> target, View2D view)
		{
//			target.Target.Dispose(); // dispose of the old target.
//			target.Target = target.BackTarget;
//			var bitmap = target.Tag as global::Android.Graphics.Bitmap;
//			if (bitmap != null)
//			{
//				target.Target.DrawBitmap(bitmap, 0, 0, _paint);
//			}
		}
		
		#endregion

		#region implemented abstract members of Renderer2D

		/// <summary>
		/// Returns the target wrapper.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public override Target2DWrapper<global::Android.Graphics.Canvas> CreateTarget2DWrapper (global::Android.Graphics.Canvas target)
		{
			return new Target2DWrapper<global::Android.Graphics.Canvas>(
				target, target.Width, target.Height);
		}

		/// <summary>
		/// Holds the view.
		/// </summary>
		private View2D _view;

		/// <summary>
		/// Holds the target.
		/// </summary>
		private Target2DWrapper<global::Android.Graphics.Canvas> _target;

		/// <summary>
		/// Transforms the canvas to the coordinate system of the view.
		/// </summary>
		/// <param name="view">View.</param>
		protected override void Transform (Target2DWrapper<global::Android.Graphics.Canvas> target, View2D view)
		{
			_view = view;
			_target = target;
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
			float sizeInPixels = this.ToPixels(size);
			_paint.Color = new global::Android.Graphics.Color(color);
			_paint.StrokeWidth = 1;
			_paint.SetStyle(global::Android.Graphics.Paint.Style.Fill);

			double[] transformed = this.Tranform (x, y);
			target.Target.DrawCircle((float)transformed[0], (float)transformed[1], sizeInPixels,
			                  _paint);
		}

		/// <summary>
		/// Draws a line on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		/// <param name="casingWidth">Casing width.</param>
		/// <param name="casingColor">Casing color.</param>
		protected override void DrawLine (Target2DWrapper<global::Android.Graphics.Canvas> target, double[] x, double[] y, 
		                                  int color, double width, LineJoin lineJoine, int[] dashes)
		{
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
				double[] transformed = this.Tranform (x [0], y [0]);
				xT = (float)transformed [0];
				yT = (float)transformed [1];
				_path.MoveTo (xT, yT);
				if (xT < minX) { minX = xT; }
				if (xT > maxX) { maxX = xT; }
				if (yT < minY) { minY = yT; }
				if (yT > maxY) { maxY = yT; }
				for(int idx = 1; idx < x.Length; idx++)
				{		
					transformed = this.Tranform (x [idx], y [idx]);
					xT = (float)transformed[0];
					yT = (float)transformed[1];
					_path.LineTo (xT, yT);
					if (xT < minX) { minX = xT; }
					if (xT > maxX) { maxX = xT; }
					if (yT < minY) { minY = yT; }
					if (yT > maxY) { maxY = yT; }
				}
				if ((maxX - minX) > 1 || (maxY - minY) > 1) {
//					if (casingWidth > 0) {
//						float casingWidthInPixels = this.ToPixels (casingWidth + width);
//						_paint.Color = new global::Android.Graphics.Color (casingColor);
//						_paint.StrokeWidth = casingWidthInPixels;
//						target.Target.DrawPath (_path, _paint);
//					}
					float widthInPixels = this.ToPixels (width);
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
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		/// <param name="fill">If set to <c>true</c> fill.</param>
		protected override void DrawPolygon (Target2DWrapper<global::Android.Graphics.Canvas> target, double[] x, double[] y, 
		                                     int color, double width, bool fill)
		{
			if(x.Length > 1)
			{
				_paint.Color = new global::Android.Graphics.Color(color);
				_paint.StrokeWidth = (float)width;
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
				double[] transformed = this.Tranform (x [0], y [0]);
				xT = (float)transformed [0];
				yT = (float)transformed [1];
				_path.MoveTo (xT, yT);
				if (xT < minX) { minX = xT; }
				if (xT > maxX) { maxX = xT; }
				if (yT < minY) { minY = yT; }
				if (yT > maxY) { maxY = yT; }
				for(int idx = 1; idx < x.Length; idx++)
				{
					transformed = this.Tranform (x [idx], y [idx]);
					xT = (float)transformed[0];
					yT = (float)transformed[1];
					_path.LineTo (xT, yT);
					if (xT < minX) { minX = xT; }
					if (xT > maxX) { maxX = xT; }
					if (yT < minY) { minY = yT; }
					if (yT > maxY) { maxY = yT; }
				}
				transformed = this.Tranform (x [0], y [0]);
				xT = (float)transformed [0];
				yT = (float)transformed [1];
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
			global::Android.Graphics.Bitmap image = global::Android.Graphics.BitmapFactory.DecodeByteArray(
				imageData, 0, imageData.Length);
			
			double[] transformed = this.Tranform (x, y);

			target.Target.DrawBitmap(image, (float)transformed [0], (float)transformed [1], null);
			image.Dispose();
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
		/// <param name="tag"></param>
		protected override object DrawImage (Target2DWrapper<global::Android.Graphics.Canvas> target, 
		                                     double left, double top, double right, double bottom, byte[] imageData,
		                                   object tag)
		{
			global::Android.Graphics.Bitmap image = (tag as global::Android.Graphics.Bitmap);
			if(image == null)
			{
				image = global::Android.Graphics.BitmapFactory.DecodeByteArray(
					imageData, 0, imageData.Length);
			}

			double[] topleft = this.Tranform(left, top);
			double[] bottomright = this.Tranform(right, bottom);
			float leftPixels = (float)topleft[0];
			float topPixels = (float)topleft[1];
			float rightPixels = (float)bottomright[0];
			float bottomPixels = (float)bottomright[1];

			target.Target.DrawBitmap(image,new global::Android.Graphics.Rect(0, 0, image.Width, image.Height),
			                         new global::Android.Graphics.RectF(leftPixels, topPixels, rightPixels, bottomPixels),
			                         null);
			return image;
		}

		/// <summary>
		/// Draws the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="target">Target.</param>
		/// <param name="bounds">Bounds.</param>
		/// <param name="imageData">Image data.</param>
		/// <param name="tag">Tag.</param>
		protected override object DrawImage (Target2DWrapper<Canvas> target, RectangleF2D bounds, byte[] imageData, object tag)
		{
			global::Android.Graphics.Bitmap image = (tag as global::Android.Graphics.Bitmap);
			if(image == null)
			{
				image = global::Android.Graphics.BitmapFactory.DecodeByteArray(
					imageData, 0, imageData.Length);
			}

			PointF2D bottomLeft = new PointF2D(this.Tranform (bounds.BottomLeft [0], bounds.BottomLeft [1]));
			PointF2D bottomRight = new PointF2D(this.Tranform (bounds.BottomRight [0], bounds.BottomRight [1]));
			PointF2D topLeft = new PointF2D(this.Tranform (bounds.TopLeft [0], bounds.TopLeft [1]));
			//PointF2D topRight = new PointF2D(this.Tranform (bounds.TopRight [0], bounds.TopRight [1])); 

			RectangleF2D transformed = new RectangleF2D(bottomLeft, bottomLeft.Distance(bottomRight), bottomLeft.Distance(topLeft), 
			                                            topLeft - bottomLeft);

            _paint.AntiAlias = true;
            _paint.FilterBitmap = true;
			target.Target.Save ();
			target.Target.Translate ((float)transformed.BottomLeft [0], (float)transformed.BottomLeft [1]);
			target.Target.Rotate (-(float)((Degree)transformed.Angle).Value);
			target.Target.DrawBitmap(image,
                new global::Android.Graphics.Rect(0, 0, image.Width, image.Height), 
                new global::Android.Graphics.RectF(0, 0, (float)transformed.Width, (float)transformed.Height),
			    _paint);
			target.Target.Restore ();

			return tag;
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
			double[] transformed = this.Tranform(x, y);
			float xPixels = (float)transformed[0];
			float yPixels = (float)transformed[1];
            float textSize = this.ToPixels(size);
            //transformed = this.Tranform(x + size, y);
            //float textSize = 10;

			_paint.AntiAlias = true;
			_paint.SubpixelText = true;
			_paint.TextSize = textSize;

			// get some metrics on the texts.
			float[] characterWidths = new float[text.Length];
			_paint.GetTextWidths (text, characterWidths);
            //for (int idx = 0; idx < characterWidths.Length; idx++)
            //{
            //    characterWidths[idx] = _target, _view, characterWidths[idx];
            //}
			var characterHeight = (float)size;
			var textLength = characterWidths.Sum();

			// center is default.
			xPixels = xPixels - (textLength / 2.0f);

			PointF2D current = new PointF2D (xPixels, yPixels);
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
					PointF2D position = current;
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
		/// <param name="target"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		/// <param name="size"></param>
		/// <param name="text">Text.</param>
		protected override void DrawLineText (Target2DWrapper<global::Android.Graphics.Canvas> target, double[] xa, double[] ya, string text, int color, 
		                                      double size, int? haloColor, int? haloRadius, string fontName)
		{
			if (xa.Length > 1)
			{
				float sizeInPixels = this.ToPixels(size);	
				_paint.SubpixelText = true;
				_paint.TextSize = (float)sizeInPixels;
				_paint.AntiAlias = true;

				// transform first.
				double[] xTransformed = new double[xa.Length];
				double[] yTransformed = new double[ya.Length];
				for (int idx = 0; idx < xa.Length; idx++) {
					double[] transformed = this.Tranform (xa[idx], ya[idx]);
					xTransformed [idx] = transformed [0];
					yTransformed [idx] = transformed [1];
				}

				// get some metrics on the texts.
				float[] characterWidths = new float[text.Length];
				_paint.GetTextWidths (text, characterWidths);
//				for (int idx = 0; idx < characterWidths.Length; idx++)
//				{
//					characterWidths[idx] = _target, _view, characterWidths[idx];
//				}
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
			for (int idx = 0; idx < text.Length; idx++)
			{
				double nextPosition = middlePosition - (textLength / 2.0) + ((textLength / (text.Length)) * (idx + 1));
				PointF2D next = Polyline2D.PositionAtPosition(xTransformed, yTransformed, nextPosition);

				// Translate to the final position, the center of line-segment between 'current' and 'next'
				//PointF2D position = current + ((next - current) / 2.0);

				// calculate the angle.
				VectorF2D vector = next - current;
				VectorF2D horizontal = new VectorF2D(1, 0);
				double angleDegrees = ((Degree)horizontal.Angle(vector)).Value;
				averageAngle = averageAngle + angleDegrees;
				current = next;
			}
			averageAngle = averageAngle / text.Length;

			// reverse if 'upside down'.
			double[] xText = xTransformed;
			double[] yText = yTransformed;
			if (averageAngle > 90 && averageAngle < 180 + 90)
			{ // the average angle is > PI => means upside down.
				xText = xTransformed.Reverse<double>().ToArray<double>();
				yText = yTransformed.Reverse<double>().ToArray<double>();
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
				global::Android.Graphics.Path characterPath = new global::Android.Graphics.Path ();
				_paint.GetTextPath (text, idx, idx + 1, 0, 0, characterPath);
				using (characterPath)
				{
					// Transformation matrix to move the character to the correct location. 
					// Note that all actions on the Matrix class are prepended, so we apply them in reverse.
					var transform = new Matrix();

					// Translate to the final position, the center of line-segment between 'current' and 'next'
					PointF2D position = current;
					//PointF2D position = current + ((next - current) / 2.0);
					//double[] transformed = this.Tranform(position[0], position[1]);
					transform.SetTranslate((float)position[0], (float)position[1]);

					// calculate the angle.
					VectorF2D vector = next - current;
					VectorF2D horizontal = new VectorF2D(1, 0);
					double angleDegrees = ((Degree)horizontal.Angle(vector)).Value;

					// Rotate the character
					transform.PreRotate((float)angleDegrees);

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
				current = next;
			}
		}

		#endregion
	}
}
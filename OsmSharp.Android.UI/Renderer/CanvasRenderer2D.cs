
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
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;

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
		protected override void OnBeforeRender(Target2DWrapper<global::Android.Graphics.Canvas> target, 
		                                       List<Scene2D> scenes, View2D view)
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
		}
		
		/// <summary>
		/// Called right after rendering.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="scenes"></param>
		/// <param name="view"></param>
		protected override void OnAfterRender(Target2DWrapper<global::Android.Graphics.Canvas> target, 
		                                      List<Scene2D> scenes, View2D view)
		{
//			target.Target.Dispose(); // dispose of the old target.
//			target.Target = target.BackTarget;
//			var bitmap = target.Tag as global::Android.Graphics.Bitmap;
//			if (bitmap != null)
//			{
//				target.Target.DrawBitmap(bitmap, 0, 0, _paint);
//			}
		}
		
		/// <summary>
		/// Builds the cached scene.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="currentCache"></param>
		/// <param name="currentScenes"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		protected override Scene2D BuildSceneCache(Target2DWrapper<global::Android.Graphics.Canvas> target, Scene2D currentCache, 
		                                           List<Scene2D> currentScenes, View2D view)
		{
			var scene = new Scene2DSimple();
//			scene.BackColor = currentScenes[0].BackColor;
//			
//			var bitmap = target.Tag as global::Android.Graphics.Bitmap;
//			if (bitmap != null)
//			{
//				byte[] imageData = null;
//				using (var stream = new MemoryStream())
//				{
//					bitmap.Compress(global::Android.Graphics.Bitmap.CompressFormat.Png, 0, stream);
//					
//					imageData = stream.ToArray();
//				}
//				scene.AddImage(0, float.MinValue, float.MaxValue, 
//				               view.Left, view.Top, view.Right, view.Bottom, imageData);
//			}
			return scene;
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

			target.Target.DrawCircle(this.TransformX(x), this.TransformY(y), sizeInPixels,
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
		                                  int color, double width, LineJoin lineJoine, int[] dashes, double casingWidth, int casingColor)
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

				// convert to the weid android api array!
				global::Android.Graphics.Path path = new global::Android.Graphics.Path ();
				path.MoveTo (this.TransformX(x[0]), this.TransformY (y[0]));
//				float[] lineCoordinates = new float[(x.Length - 2) * 4 + 4];
//				lineCoordinates[0] = ;
//				lineCoordinates[1] = ;
				for(int idx = 1; idx < x.Length; idx++)
				{		
					path.LineTo (this.TransformX(x[idx]), this.TransformY (y[idx]));
//					int androidApiIndex = (idx - 1) * 4 + 2;
//					lineCoordinates[androidApiIndex] = ;
//					lineCoordinates[androidApiIndex + 1] = ;
//					lineCoordinates[androidApiIndex + 2] = this.TransformX(x[idx]);
//					lineCoordinates[androidApiIndex + 3] = this.TransformY(y[idx]);
				}
//				lineCoordinates[lineCoordinates.Length - 2] = this.TransformX(x[x.Length - 1]);
//				lineCoordinates[lineCoordinates.Length - 1] = this.TransformY(y[y.Length - 1]);
//				target.Target.DrawLines(lineCoordinates, 0, lineCoordinates.Length, _paint);
				if (casingWidth > 0) {
					float casingWidthInPixels = this.ToPixels(casingWidth + width);
					_paint.Color = new global::Android.Graphics.Color(casingColor);
					_paint.StrokeWidth = casingWidthInPixels;
					target.Target.DrawPath (path, _paint);
				}
				float widthInPixels = this.ToPixels(width);
				_paint.Color = new global::Android.Graphics.Color(color);
				_paint.StrokeWidth = widthInPixels;
				target.Target.DrawPath (path, _paint);
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

				// convert android path object.
				global::Android.Graphics.Path path = new global::Android.Graphics.Path();
				path.MoveTo(this.TransformX(x[0]), this.TransformY(y[0]));
				for(int idx = 1; idx < x.Length; idx++)
				{
					path.LineTo(this.TransformX(x[idx]), this.TransformY(y[idx]));
				}
				path.LineTo(this.TransformX(x[0]), this.TransformY(y[0]));

				target.Target.DrawPath(path, _paint);
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

//			// calculate target rectangle.
//			double sceneSizeX = this.FromPixels(target, _view, image.Width) / 2.0f;
//			double sceneSizeY = this.FromPixels(target, _view, image.Height) / 2.0f;
//			double sceneTop = x + sceneSizeX;
//			double sceneBottom = x - sceneSizeX;
//			double sceneLeft = y - sceneSizeY;
//			double sceneRight = y + sceneSizeY;

			target.Target.DrawBitmap(image, this.TransformX(x), this.TransformY(y), null);
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
			
			float leftPixels = this.TransformX(left);
			float topPixels = this.TransformY(top);
			float rightPixels = this.TransformX(right);
			float bottomPixels = this.TransformY(bottom);

			target.Target.DrawBitmap(image,new global::Android.Graphics.Rect(0, 0, image.Width, image.Height),
			                         new global::Android.Graphics.RectF(leftPixels, topPixels, rightPixels, bottomPixels),
			                         null);
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
		protected override void DrawText (Target2DWrapper<global::Android.Graphics.Canvas> target, double x, double y, 
		                                  string text, int color, double size, int? haloColor, int? haloRadius)
		{
			float xPixels = this.TransformX(x);
			float yPixels = this.TransformY(y);
			float textSize = this.TransformX (x + size) - xPixels;
			_paint.Color = new global::Android.Graphics.Color(color);
			_paint.FakeBoldText = true;
			_paint.TextSize = textSize * 2;
			target.Target.DrawText (text, xPixels, yPixels, _paint);
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
		protected override void DrawLineText (Target2DWrapper<global::Android.Graphics.Canvas> target, double[] x, double[] y, string text, int color, 
		                                      double size, int? haloColor, int? haloRadius)
		{
////			if (x.Length > 1) {
////				float textSize = this.ToPixels(size);
////
////				_paint.Color = new global::Android.Graphics.Color(color);
////				_paint.FakeBoldText = true;
////				_paint.TextSize = textSize * 2;
////
//////				float[] characterWidths = new float[text.Length];
//////				_paint.GetTextWidths (text, characterWidths);
////
////				double textLength = this.FromPixels(_target, _view, _paint.MeasureText (text));
////				double lineLength = Polyline2D.Length(x, y);
////
////				if (lineLength > textLength * 1.5) {
////					global::Android.Graphics.Path linePath = new global::Android.Graphics.Path ();
////					linePath.MoveTo (this.TransformX (x [0]), this.TransformY (y [0]));
////					for (int idx = 1; idx < x.Length; idx++) {
////						linePath.LineTo (this.TransformX (x [1]), this.TransformY (y [1]));
////					}
////					target.Target.DrawTextOnPath (text, linePath, 0, 0, _paint);
////				}
////			}
//			if (x.Length > 1)
//			{
//				float textSize = this.ToPixels(size);
//				_paint.Color = new global::Android.Graphics.Color(color);
//				_paint.FakeBoldText = true;
//				_paint.TextSize = textSize;
//				_paint.SetStyle (global::Android.Graphics.Paint.Style.Fill);
//
//				// get some metrics on the texts.
//				//var characterWidths = GetCharacterWidths(target.Target, text, font);
//				float[] characterWidths = new float[text.Length];
//				_paint.GetTextWidths (text, characterWidths);
//				for (int idx = 0; idx < characterWidths.Length; idx++)
//				{
//					characterWidths[idx] = (float)this.FromPixels(_target, _view, characterWidths[idx]);
//				}
//				float characterHeight = (float)size;
//				var textLength = characterWidths.Sum();
//				var avgCharacterWidth = textLength / characterWidths.Length;
//
//				// calculate line length.
//				var lineLength = Polyline2D.Length(x, y);
//				if (lineLength > textLength)
//				{
//					// see if the text is 'upside down'.
//					double averageAngle = 0;
//					double middle = lineLength / 2.0;
//					double first = middle - (textLength / 2.0);
//					PointF2D current = Polyline2D.PositionAtPosition(x, y, first);
//					for (int idx = 0; idx < text.Length; idx++)
//					{
//						double nextPosition = middle - (textLength / 2.0) + ((textLength / (text.Length)) * (idx + 1));
//						PointF2D next = Polyline2D.PositionAtPosition(x, y, nextPosition);
//
//						// Translate to the final position, the center of line-segment between 'current' and 'next'
//						PointF2D position = current + ((next - current) / 2.0);
//
//						// calculate the angle.
//						VectorF2D vector = next - current;
//						VectorF2D horizontal = new VectorF2D(1, 0);
//						double angleDegrees = ((Degree)horizontal.Angle(vector)).Value;
//						averageAngle = averageAngle + angleDegrees;
//						current = next;
//					}
//					averageAngle = averageAngle / text.Length;
//
//					// reverse if 'upside down'.
//					double[] xText = x;
//					double[] yText = y;
//					if (averageAngle > 90 && averageAngle < 180 + 90)
//					{ // the average angle is > PI => means upside down.
//						xText = x.Reverse<double>().ToArray<double>();
//						yText = y.Reverse<double>().ToArray<double>();
//					}
//
//					// calculate a central position along the line.
//					middle = lineLength / 2.0;
//					first = middle - (textLength / 2.0);
//					current = Polyline2D.PositionAtPosition(xText, yText, first);
//					Matrix matrixBefore = _target.Target.Matrix;
//					for (int idx = 0; idx < text.Length; idx++)
//					{
//						double nextPosition = middle - (textLength / 2.0) + ((textLength / (text.Length)) * (idx + 1));
//						PointF2D next = Polyline2D.PositionAtPosition(xText, yText, nextPosition);
//						char currentChar = text[idx];
//						global::Android.Graphics.Path textPath = new global::Android.Graphics.Path ();
//						_paint.GetTextPath (text, idx, idx + 1, 0, 0, textPath);
////							characterPath.AddString(currentChar.ToString(), font.FontFamily, (int)font.Style, font.Size, Point.Empty,
////							                        StringFormat.GenericTypographic);
//
//							//var pathBounds = characterPath.GetBounds();
//
//							// Transformation matrix to move the character to the correct location. 
//							// Note that all actions on the Matrix class are prepended, so we apply them in reverse.
//							var transform = new Matrix();
//
//							// Translate to the final position, the center of line-segment between 'current' and 'next'
//							PointF2D position = current + ((next - current) / 2.0);
//							transform.SetTranslate(this.TransformX(position[0]), this.TransformY(position[1]));
//
//							// calculate the angle.
//							VectorF2D vector = next - current;
//							VectorF2D horizontal = new VectorF2D(1, 0);
//							double angleDegrees = ((Degree)horizontal.Angle(vector)).Value;
//
//							// Rotate the character
//						transform.PreRotate ((float)angleDegrees);
//							//transform.SetRotate((float)angleDegrees);
//
//							// Translate the character so the centre of its base is over the origin
//						transform.PreTranslate(0, -characterHeight / 2.5f);
//
//							//transform.Scale((float)this.FromPixels(_target, _view, 1), 
//							//    (float)this.FromPixels(_target, _view, 1));
//
//						//target.Target.
//						textPath.Transform(transform);
//
//						// Draw the character
//						target.Target.DrawPath (textPath, _paint);
//						//target.Target.DrawText (text, idx , idx + 1, 0, 0, _paint);
//
//						current = next;
//					}
//				}
//			}
			if (x.Length > 1)
			{
				float sizeInPixels = this.ToPixels(size);	
				_paint.SubpixelText = true;
				_paint.TextSize = (float)sizeInPixels;

				// get some metrics on the texts.
				float[] characterWidths = new float[text.Length];
				_paint.GetTextWidths (text, characterWidths);
				for (int idx = 0; idx < characterWidths.Length; idx++)
				{
					characterWidths[idx] = (float)this.FromPixels(_target, _view, characterWidths[idx]);
				}
				var characterHeight = (float)size;
				var textLength = characterWidths.Sum();
				//var avgCharacterWidth = textLength / characterWidths.Length;

				// calculate line length.
				var lineLength = Polyline2D.Length(x, y);
				if (lineLength > textLength * 1.2f)
				{
					// calculate the number of labels.
					int labelCount = (int)System.Math.Floor(lineLength / (textLength * 10)) + 1;

					// calculate positions of label(s).
					double positionGap = lineLength / (labelCount + 1);

					// draw each label.
					for (double position = positionGap; position < lineLength; position = position + positionGap)
					{
						this.DrawLineTextSegment(target, x, y, text, color, size, haloColor, haloRadius, position, characterWidths,
						                         textLength, characterHeight);
					}
				}
			}
		}

		public void DrawLineTextSegment(Target2DWrapper<global::Android.Graphics.Canvas> target, double[] x, double[] y, string text, int color,
		                                double size, int? haloColor, int? haloRadius, double middlePosition, float[] characterWidths, double textLength,
		                                float characterHeight)
		{
			_paint.Color = new global::Android.Graphics.Color(color);
			_paint.SetStyle (global::Android.Graphics.Paint.Style.Fill);

			// see if the text is 'upside down'.
			double averageAngle = 0;
			double first = middlePosition - (textLength / 2.0);
			PointF2D current = Polyline2D.PositionAtPosition(x, y, first);
			for (int idx = 0; idx < text.Length; idx++)
			{
				double nextPosition = middlePosition - (textLength / 2.0) + ((textLength / (text.Length)) * (idx + 1));
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
					transform.SetTranslate(this.TransformX(position[0]), this.TransformY(position[1]));

					// calculate the angle.
					VectorF2D vector = next - current;
					VectorF2D horizontal = new VectorF2D(1, 0);
					double angleDegrees = ((Degree)horizontal.Angle(vector)).Value;

					// Rotate the character
					transform.PreRotate((float)angleDegrees);

					// Translate the character so the centre of its base is over the origin
					transform.PreTranslate(0, -characterHeight / 2.5f);

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

//		/// <summary>
//		/// Gets the char widths for the given string and given font.
//		/// </summary>
//		/// <param name="graphics"></param>
//		/// <param name="text"></param>
//		/// <param name="font"></param>
//		/// <returns></returns>
//		private float[] GetCharacterWidths(string text)
//		{
//			// The length of a space. Necessary because a space measured using StringFormat.GenericTypographic has no width.
//			// We can't use StringFormat.GenericDefault for the characters themselves, as it adds unwanted spacing.
//			global::Android.Graphics.RectF bounds;
//			_paint.MeasureText(" ", 0, 1, bounds);
//			var spaceLength = bounds.Width;
//
//			float[] widths = new float[text.Length];
//			for (int idx = 0; idx < text.Length; idx++)
//			{
//				_paint.GetTextBounds (text, idx, 1, bounds);
//				widths[idx] = (text[idx] == ' ' ? spaceLength :
//				               bounds.Width);
//			}
//			return widths;
//		}

		#endregion
	}
}
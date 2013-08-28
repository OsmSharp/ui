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
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using OsmSharp.UI.Map;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Renderer;
using System.Drawing;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.UI;
using System.Collections.Generic;
using OsmSharp.UI.Map.Layers;

namespace OsmSharp.iOS.UI
{
	/// <summary>
	/// Map view.
	/// </summary>
	public class MapView : UIView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		public MapView ()
		{
			this.Initialize ();
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		public void Initialize()
		{
			this.BackgroundColor = UIColor.White;

			var panGesture = new UIPanGestureRecognizer (Pan);
			this.AddGestureRecognizer (panGesture);
			var pinchGesture = new UIPinchGestureRecognizer (Pinch);
			this.AddGestureRecognizer (pinchGesture);

//			// create the renderer
//			_renderer = new MapRenderer<CGContextWrapper> (
//				new CGContextRenderer());
//
			// create the cache renderer.
			_cacheRenderer = new MapRenderer<CGContextWrapper> (
				new CGContextRenderer ());
			_cachedScene = new Scene2DSimple ();
			_cachedScene.BackColor = SimpleColor.FromKnownColor (KnownColor.White).Value;

			// create invalidation timer.
			_render = true;
			System.Threading.Timer timer = new System.Threading.Timer (InvalidateSimple,
			                                                           null, 0, 50);
		}

		/// <summary>
		/// Holds the rendering flag.
		/// </summary>
		private bool _render;

		/// <summary>
		/// Holds the cache renderer.
		/// </summary>
		private MapRenderer<CGContextWrapper> _cacheRenderer;

		/// <summary>
		/// Holds the cached scene.
		/// </summary>
		private Scene2D _cachedScene;

		/// <summary>
		/// Holds the canvas bitmap.
		/// </summary>
		private CGImage _canvasBitmap;

		/// <summary>
		/// Holds the rendering cache.
		/// </summary>
		private CGImage _cache;

		/// <summary>
		/// Invalidates while rendering.
		/// </summary>
		/// <param name="state">State.</param>
		private void InvalidateSimple(object state) {
			//this.InvokeOnMainThread (Test);
			if (_cacheRenderer.IsRunning) {
				this.InvokeOnMainThread (Test);
			}

			if (_render) {
				_render = false;
				
				if (_cacheRenderer.IsRunning) {
					_cacheRenderer.Cancel ();
				}

				this.Render ();
			}
		}

		/// <summary>
		/// Notifies change.
		/// </summary>
		internal void Change()
		{
			if (_cacheRenderer.IsRunning) {
				_cacheRenderer.Cancel ();
			}

			// set the rendering flag.
			_render = true;
		}

		private byte[] _bytescache;

		/// <summary>
		/// Render the current complete scene.
		/// </summary>
		void Render(){
			if (_cacheRenderer.IsRunning) {
				_cacheRenderer.CancelAndWait ();
			}

			if (_rect == null) { // only render if a proper size is known.
				return;
			}

			lock (_cacheRenderer) { // make sure only on thread at the same time is using the renderer.
				double extra = 1.25;

				// build the layers list.
				var layers = new List<ILayer> ();
				for (int layerIdx = 0; layerIdx < this.Map.LayerCount; layerIdx++) {
					layers.Add (this.Map [layerIdx]);
				}

				// add the internal layer.
				// TODO: create marker layer.

				// create a new bitmap context.
				CGColorSpace space = CGColorSpace.CreateDeviceRGB ();
				int bytesPerPixel = 4;
				int bytesPerRow = bytesPerPixel * (int)_rect.Width;
				int bitsPerComponent = 8;
				if (_bytescache == null) {
					_bytescache = new byte[bytesPerRow * (int)_rect.Height];
				}
				CGBitmapContext gctx = new CGBitmapContext (null, (int)_rect.Width, (int)_rect.Height,
				                                           bitsPerComponent, bytesPerRow,
				                                            space, // kCGBitmapByteOrder32Little | kCGImageAlphaNoneSkipLast
				                                            CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Big);

				// create the view.
				View2D view = _cacheRenderer.Create (_rect.Width, _rect.Height,
				                                     this.Map, (float)this.Map.Projection.ToZoomFactor (this.MapZoomLevel), this.MapCenter);
				long before = DateTime.Now.Ticks;
				OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
				                                "Rendering Start");

				// notify the map that the view has changed.
				this.Map.ViewChanged ((float)this.Map.Projection.ToZoomFactor(this.MapZoomLevel), this.MapCenter, 
				                      view);
				long afterViewChanged = DateTime.Now.Ticks;
				OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
				                                "View change took: {0}ms @ zoom level {1}",
				                                (new TimeSpan(afterViewChanged - before).TotalMilliseconds), this.MapZoomLevel);

//				// add the current canvas to the scene.
//				uint canvasId = _scene.AddImage (-1, float.MinValue, float.MaxValue, 
//				                                 view.Left, view.Top, view.Right, view.Bottom, new byte[0], _canvasBitmap);

				// does the rendering.
				bool complete = _cacheRenderer.Render (new CGContextWrapper (gctx, new RectangleF(0,0,_rect.Width, _rect.Height)), 
				                                                             this.Map.Projection, 
				                                       layers, (float)this.Map.Projection.ToZoomFactor (this.MapZoomLevel), 
				                                       this.MapCenter);

				long afterRendering = DateTime.Now.Ticks;
				OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
				                                "Rendering took: {0}ms @ zoom level {1}",
				                                (new TimeSpan(afterRendering - afterViewChanged).TotalMilliseconds), this.MapZoomLevel);
				if (complete) { // there was no cancellation, the rendering completely finished.
					// add the result to the scene cache.
					lock (_cachedScene) {
						// add the newly rendered image again.
						//this.Layer.Contents = gctx.ToImage ();

						_cachedScene.Clear ();
						_cachedScene.AddImage (0, float.MinValue, float.MaxValue, 
						                       view.Left, view.Top, view.Right, view.Bottom, new byte[0], gctx.ToImage());
//						_cachedScene.AddImage (0, float.MinValue, float.MaxValue, 
//						                       view.Left, view.Top, view.Right, view.Bottom, new byte[0], _layer);
					}
					
					this.InvokeOnMainThread (Test);
				}

				long after = DateTime.Now.Ticks;
				if (!complete) {
					OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,"Rendering in {0}ms after cancellation!", 
					                                new TimeSpan (after - before).TotalMilliseconds);
				} else {
					OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,"Rendering in {0}ms", 
					                                new TimeSpan (after - before).TotalMilliseconds);
				}
			}
		}

		/// <summary>
		/// Holds the map zoom level before pinching.
		/// </summary>
		private float? _mapZoomLevelBefore;

		/// <summary>
		/// Pinch the specified pinch.
		/// </summary>
		/// <param name="pinch">Pinch.</param>
		private void Pinch(UIPinchGestureRecognizer pinch)
		{
			if (_rect != null) {
				if (pinch.State == UIGestureRecognizerState.Ended) {
					this.MapZoomLevel = _mapZoomLevelBefore.Value;

					double zoomFactor = this.Map.Projection.ToZoomFactor (this.MapZoomLevel);
					zoomFactor = zoomFactor * pinch.Scale;
					this.MapZoomLevel = (float)this.Map.Projection.ToZoomLevel (zoomFactor);

					this.Change (); // notifies change.

					_mapZoomLevelBefore = null;
				} else if (pinch.State == UIGestureRecognizerState.Began) {
					_mapZoomLevelBefore = this.MapZoomLevel;
				}
				else {
					this.MapZoomLevel = _mapZoomLevelBefore.Value;

					double zoomFactor = this.Map.Projection.ToZoomFactor (this.MapZoomLevel);
					zoomFactor = zoomFactor * pinch.Scale;
					this.MapZoomLevel = (float)this.Map.Projection.ToZoomLevel (zoomFactor);

					//this.Change (); // notifies change.
					this.InvokeOnMainThread (Test);
				}
			}
		}

		/// <summary>
		/// Holds the pan offset.
		/// </summary>
		private GeoCoordinate _beforePan;

		/// <summary>
		/// Pan the specified some.
		/// </summary>
		/// <param name="some">Some.</param>
		private void Pan(UIPanGestureRecognizer pan)
		{
			if (_rect != null) {
				PointF offset = pan.TranslationInView (this);
				if (pan.State == UIGestureRecognizerState.Ended) {
					_beforePan = null;
					View2D view = this.CreateView (_rect);
					double centerXPixels = _rect.Width / 2.0f - offset.X;
					double centerYPixels = _rect.Height / 2.0f - offset.Y;

					double[] sceneCenter = view.FromViewPort (_rect.Width, _rect.Height, 
					                                          centerXPixels, centerYPixels);

					this.MapCenter = this.Map.Projection.ToGeoCoordinates (
						sceneCenter [0], sceneCenter [1]);
					
					this.Change (); // notifies change.
				} else if (pan.State == UIGestureRecognizerState.Began) {
					_beforePan = this.MapCenter;
				} else if (pan.State == UIGestureRecognizerState.Cancelled ||
					pan.State == UIGestureRecognizerState.Failed) {
					_beforePan = null;
				} else if(_beforePan != null) {
					this.MapCenter = _beforePan;

					View2D view = this.CreateView (_rect);
					double centerXPixels = _rect.Width / 2.0f - offset.X;
					double centerYPixels = _rect.Height / 2.0f - offset.Y;

					double[] sceneCenter = view.FromViewPort (_rect.Width, _rect.Height, 
					                                          centerXPixels, centerYPixels);

					this.MapCenter = this.Map.Projection.ToGeoCoordinates (
						sceneCenter [0], sceneCenter [1]);
					
					this.InvokeOnMainThread (Test);
				}
			}
		}

		/// <summary>
		/// Gets or sets the center.
		/// </summary>
		/// <value>The center.</value>
		public GeoCoordinate MapCenter {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		/// <value>The map.</value>
		public Map Map {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the zoom level.
		/// </summary>
		/// <value>The zoom level.</value>
		public float MapZoomLevel {
			get;
			set;
		}

		private System.Drawing.RectangleF _rect;

		/// <summary>
		/// Creates the view.
		/// </summary>
		/// <returns>The view.</returns>
		public View2D CreateView(System.Drawing.RectangleF rect)
		{
			_rect = rect;

			double[] sceneCenter = this.Map.Projection.ToPixel (this.MapCenter.Latitude, this.MapCenter.Longitude);
			float sceneZoomFactor = (float)this.Map.Projection.ToZoomFactor (this.MapZoomLevel);

			return View2D.CreateFrom (sceneCenter [0], sceneCenter [1],
			                         rect.Width, rect.Height, sceneZoomFactor,
			                         this.Map.Projection.DirectionX, this.Map.Projection.DirectionY);
		}

		private void Test()
		{
			OsmSharp.Logging.Log.TraceEvent ("MapView", System.Diagnostics.TraceEventType.Information,
			                                "SetNeedsDisplay called on main thread!");
			this.SetNeedsDisplay ();
		}

		private CGLayer _layer;

		/// <summary>
		/// Draws the view within the specified rectangle.
		/// </summary>
		/// <param name="rect">Rect.</param>
		public override void Draw (System.Drawing.RectangleF rect)
		{
			_rect = rect;

			base.Draw (rect);

			lock (_cachedScene) {
				// recreate the view.
				View2D view = this.CreateView (_rect);

				// call the canvas renderer.
				CGContext context = UIGraphics.GetCurrentContext ();

				if (context != null) {
					context.InterpolationQuality = CGInterpolationQuality.None;
					context.SetShouldAntialias (false);
					context.SetBlendMode (CGBlendMode.Copy);
					context.SetAlpha (1);

					OsmSharp.Logging.Log.TraceEvent ("MapView", System.Diagnostics.TraceEventType.Information,
					                                 "Renderer called in Draw(rect)!");
					long afterViewChanged = DateTime.Now.Ticks;
					CGContextRenderer renderer = new CGContextRenderer ();
					renderer.Render (new CGContextWrapper (context, _rect),
					                _cachedScene, view);
					long afterRendering = DateTime.Now.Ticks;

					OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
					                                "Rendering cache took: {0}ms @ zoom level {1}",
					                                (new TimeSpan(afterRendering - afterViewChanged).TotalMilliseconds), this.MapZoomLevel);
				}
			}
		}
	}
}


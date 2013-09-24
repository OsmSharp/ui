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
using OsmSharp.Math.Primitives;
using OsmSharp.Units.Angle;
using OsmSharp.Math.Geo.Projections;

namespace OsmSharp.iOS.UI
{
	/// <summary>
	/// Map view.
	/// </summary>
	public class MapView : UIView
	{
		private bool _invertX = false;
		private bool _invertY = false;

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
			this.UserInteractionEnabled = true;

			_markers = new List<MapMarker> ();

			var panGesture = new UIPanGestureRecognizer (Pan);
			panGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) => { return true; };
			this.AddGestureRecognizer (panGesture);
			var pinchGesture = new UIPinchGestureRecognizer (Pinch);
			pinchGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) => { return true; };
			this.AddGestureRecognizer (pinchGesture);
			var rotationGesture = new UIRotationGestureRecognizer (Rotate);
			rotationGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) => { return true; };
			this.AddGestureRecognizer (rotationGesture);
			var tapGesture = new UITapGestureRecognizer (Tap);
			tapGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) => { return other != null; };
			this.AddGestureRecognizer (tapGesture);

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

		public override bool GestureRecognizerShouldBegin (UIGestureRecognizer gestureRecognizer)
		{
			return true;
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
//			//this.InvokeOnMainThread (Test);
//			if (_cacheRenderer.IsRunning) {
//				this.InvokeOnMainThread (InvalidateMap);
//			}

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
			
			long before = DateTime.Now.Ticks;
			OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
			                                "Rendering Start");

			if (_cacheRenderer.IsRunning) {
				_cacheRenderer.CancelAndWait ();
			}

			if (_rect.Width == 0) { // only render if a proper size is known.
				return;
			}

			lock (_cacheRenderer) { // make sure only on thread at the same time is using the renderer.

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
				                                     this.Map, (float)this.Map.Projection.ToZoomFactor (this.MapZoomLevel), 
				                                     this.MapCenter, _invertX, _invertY, this.MapTilt);

				// notify the map that the view has changed.
				this.Map.ViewChanged ((float)this.Map.Projection.ToZoomFactor(this.MapZoomLevel), this.MapCenter, 
				                      view);
				long afterViewChanged = DateTime.Now.Ticks;
				OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
				                                "View change took: {0}ms @ zoom level {1}",
				                                (new TimeSpan(afterViewChanged - before).TotalMilliseconds), this.MapZoomLevel);
				// does the rendering.
				bool complete = _cacheRenderer.Render (new CGContextWrapper (gctx, new RectangleF(0,0,_rect.Width, _rect.Height)), 
				                                       layers, view);

				long afterRendering = DateTime.Now.Ticks;
				OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
				                                "Rendering took: {0}ms @ zoom level {1}",
				                                (new TimeSpan(afterRendering - afterViewChanged).TotalMilliseconds), this.MapZoomLevel);
				if (complete) { // there was no cancellation, the rendering completely finished.
					// add the result to the scene cache.
					lock (_cachedScene) {
						// add the newly rendered image again.
						_cachedScene.Clear ();
						_cachedScene.AddImage (0, float.MinValue, float.MaxValue, view.Rectangle, new byte[0], gctx.ToImage ());
					}
					this.InvokeOnMainThread (InvalidateMap);
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
		/// Holds the map view before rotating.
		/// </summary>
		private View2D _mapViewBefore;

		/// <summary>
		/// Rotate the specified rotation.
		/// </summary>
		/// <param name="rotation">Rotation.</param>
		private void Rotate(UIRotationGestureRecognizer rotation){
			if (_rect.Width > 0) {
				if (rotation.State == UIGestureRecognizerState.Ended) { 
					View2D rotatedView = _mapViewBefore.RotateAroundCenter ((Radian)rotation.Rotation);
					_mapTilt = (float)((Degree)rotatedView.Rectangle.Angle).Value;
					PointF2D sceneCenter = rotatedView.Rectangle.Center;
					this.MapCenter = this.Map.Projection.ToGeoCoordinates (
						sceneCenter [0], sceneCenter [1]);
					this.Change ();

					_mapViewBefore = null;
				} else if (rotation.State == UIGestureRecognizerState.Began) {
					_mapViewBefore = this.CreateView (_rect);
				} else {
					//_mapViewBefore = this.CreateView (_rect);
					View2D rotatedView = _mapViewBefore.RotateAroundCenter ((Radian)rotation.Rotation);
					_mapTilt = (float)((Degree)rotatedView.Rectangle.Angle).Value;
					PointF2D sceneCenter = rotatedView.Rectangle.Center;
					this.MapCenter = this.Map.Projection.ToGeoCoordinates (
						sceneCenter [0], sceneCenter [1]);

					this.InvokeOnMainThread (InvalidateMap);
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
			if (_rect.Width > 0) {
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
					this.InvokeOnMainThread (InvalidateMap);
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
					
					this.InvokeOnMainThread (InvalidateMap);
				}
			}
		}

		public delegate void MapTapEventDelegate(GeoCoordinate geoCoordinate);

		/// <summary>
		/// Occurs when the map was tapped at a certain location.
		/// </summary>
		public event MapTapEventDelegate MapTapEvent;

		/// <summary>
		/// Tap the specified tap.
		/// </summary>
		/// <param name="tap">Tap.</param>
		private void Tap(UITapGestureRecognizer tap){
			if(_rect.Width > 0 && _rect.Height > 0) {
				if (this.MapTapEvent != null) {
					View2D view = this.CreateView (_rect);
					PointF location = tap.LocationInView (this);
				    double[] sceneCoordinates = view.FromViewPort (_rect.Width, _rect.Height, location.X, location.Y);
					this.MapTapEvent (this.Map.Projection.ToGeoCoordinates (sceneCoordinates [0], sceneCoordinates [1]));
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

		/// <summary>
		/// Holds the map tilte angle.
		/// </summary>
		private float _mapTilt;

		/// <summary>
		/// Gets or sets the map tilt.
		/// </summary>
		/// <value>The map tilt.</value>
		public float MapTilt {
			get{
				return _mapTilt;
			}
			set {
				_mapTilt = value;

				this.Change ();
			}
		}

		/// <summary>
		/// Holds the drawing rectangle.
		/// </summary>
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
			                         _invertX, _invertY, this.MapTilt);
		}

		private void InvalidateMap()
		{
			OsmSharp.Logging.Log.TraceEvent ("MapView", System.Diagnostics.TraceEventType.Information,
			                                "SetNeedsDisplay called on main thread!");

			// change the map markers.
			if (_rect.Width > 0 && _rect.Height > 0) {
				View2D view = this.CreateView (_rect);

				this.NotifyMapChangeToMarkers (_rect.Width, _rect.Height, view, this.Map.Projection);
			}
			this.SetNeedsDisplay ();
		}

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

		#region Map Markers

		/// <summary>
		/// Holds the markers.
		/// </summary>
		private List<MapMarker> _markers;

		/// <summary>
		/// Returns the mapmarkers list.
		/// </summary>
		/// <value>The markers.</value>
		public void AddMarker(MapMarker marker)
		{
			_markers.Add (marker);
			this.Add (marker);

			if (_rect.Width > 0 && _rect.Height > 0) {
				View2D view = this.CreateView (_rect);
				this.NotifyMapChangeToMarker (_rect.Width, _rect.Height, view, this.Map.Projection, marker);
			}
		}

		/// <summary>
		/// Adds the marker.
		/// </summary>
		/// <returns>The marker.</returns>
		/// <param name="coordinate">Coordinate.</param>
		public MapMarker AddMarker(GeoCoordinate coordinate)
		{
			MapMarker marker = new MapMarker (this, coordinate);
			this.AddMarker (marker);
			return marker;
		}

		/// <summary>
		/// Notifies the map change to markers.
		/// </summary>
		/// <param name="pixelsWidth">Pixels width.</param>
		/// <param name="pixelsHeight">Pixels height.</param>
		/// <param name="view">View.</param>
		/// <param name="projection">Projection.</param>
		internal void NotifyMapChangeToMarkers(double pixelsWidth, double pixelsHeight, View2D view, 
		                                      IProjection projection) {
			foreach (MapMarker marker in _markers) {
				this.NotifyMapChangeToMarker (pixelsWidth, pixelsHeight, view, projection, marker);
			}
		}

		/// <summary>
		/// Notifies this MapView that a map marker has changed.
		/// </summary>
		/// <param name="mapMarker"></param>
		internal void NotifyMarkerChange(MapMarker mapMarker)
		{
			// notify map layout of changes.
			if (_rect.Width > 0 && _rect.Height > 0) {
				View2D view = this.CreateView (_rect);

				this.NotifyMapChangeToMarker (_rect.Width, _rect.Height, view, this.Map.Projection, mapMarker);
			}
		}

		/// <summary>
		/// Notifies the map change.
		/// </summary>
		/// <param name="pixelWidth"></param>
		/// <param name="pixelsHeight"></param>
		/// <param name="view"></param>
		/// <param name="projection"></param>
		/// <param name="mapMarker"></param>
		internal void NotifyMapChangeToMarker(double pixelsWidth, double pixelsHeight, View2D view, 
		                                      IProjection projection, MapMarker mapMarker)
		{
			if (mapMarker != null)
			{
				mapMarker.SetLayout (pixelsWidth, pixelsHeight, view, projection);
			}
		}

		#endregion

	}
}


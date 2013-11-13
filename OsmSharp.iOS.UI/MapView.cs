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
using System.Drawing;
using System.Threading;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.UI;
using OsmSharp.UI.Animations;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Units.Angle;

namespace OsmSharp.iOS.UI
{
	/// <summary>
	/// Map view.
	/// </summary>
	public class MapView : UIView, IMapView
	{
		private bool _invertX = false;
		private bool _invertY = false;

		/// <summary>
		/// Raised when the map changes because of a touch.
		/// </summary>
		public event MapViewDelegates.MapTouchedDelegate MapTouched;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		public MapView ()
		{
			this.Initialize (new GeoCoordinate(0,0), new Map(), 0, 16);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		/// <param name="mapCenter">Map center.</param>
		/// <param name="map">Map.</param>
		/// <param name="defaultZoom">Default zoom.</param>
		public MapView (GeoCoordinate mapCenter, Map map, float defaultZoom)
		{
			this.Initialize (mapCenter, map, 0, defaultZoom);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		/// <param name="mapCenter">Map center.</param>
		/// <param name="map">Map.</param>
		/// <param name="mapTilt">Map tilt.</param>
		/// <param name="defaultZoom">Default zoom.</param>
		public MapView (GeoCoordinate mapCenter, Map map, Degree mapTilt, float defaultZoom)
		{
			this.Initialize (mapCenter, map, mapTilt, defaultZoom);
		}

		/// <summary>
		/// Initialize the specified defaultMapCenter, defaultMap, defaultMapTilt and defaultMapZoom.
		/// </summary>
		/// <param name="defaultMapCenter">Default map center.</param>
		/// <param name="defaultMap">Default map.</param>
		/// <param name="defaultMapTilt">Default map tilt.</param>
		/// <param name="defaultMapZoom">Default map zoom.</param>
		public void Initialize(GeoCoordinate defaultMapCenter, Map defaultMap, Degree defaultMapTilt, float defaultMapZoom)
		{
            // set clip to bounds to prevent objects from being rendered/show outside of the mapview.
            this.ClipsToBounds = true;

			_mapCenter = defaultMapCenter;
			_map = defaultMap;
			_mapTilt = defaultMapTilt;
			_mapZoom = defaultMapZoom;

			_doubleTapAnimator = new MapViewAnimator (this);

			this.BackgroundColor = UIColor.White;
			this.UserInteractionEnabled = true;

			_markers = new List<MapMarker> ();

			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
				var panGesture = new UIPanGestureRecognizer (Pan);
				panGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) => { 
					return true; 
				};
				// TODO: workaround for xamarin bug, remove later!
				panGesture.ShouldRequireFailureOf = (a, b) => { return false; };
				panGesture.ShouldBeRequiredToFailBy = (a, b) => { return false; };
				this.AddGestureRecognizer (panGesture);

				var pinchGesture = new UIPinchGestureRecognizer (Pinch);
				pinchGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) => { 
					return true; 
				};
				// TODO: workaround for xamarin bug, remove later!
				pinchGesture.ShouldRequireFailureOf = (a, b) => { return false; };
				pinchGesture.ShouldBeRequiredToFailBy = (a, b) => { return false; };
				this.AddGestureRecognizer (pinchGesture);

				var rotationGesture = new UIRotationGestureRecognizer (Rotate);
				rotationGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) => { 
					return true; 
				};
				// TODO: workaround for xamarin bug, remove later!
				rotationGesture.ShouldRequireFailureOf = (a, b) => { return false; };
				rotationGesture.ShouldBeRequiredToFailBy = (a, b) => { return false; };
				this.AddGestureRecognizer (rotationGesture);

				var singleTapGesture = new UITapGestureRecognizer (SingleTap);
				singleTapGesture.NumberOfTapsRequired = 1;
				// TODO: workaround for xamarin bug, remove later!
//				singleTapGesture.ShouldRequireFailureOf = (a, b) => { return false; };
//				singleTapGesture.ShouldBeRequiredToFailBy = (a, b) => { return false; };

				var doubleTapGesture = new UITapGestureRecognizer (DoubleTap);
				doubleTapGesture.NumberOfTapsRequired = 2;
				// TODO: workaround for xamarin bug, remove later!
//				doubleTapGesture.ShouldRequireFailureOf = (a, b) => { return false; };
//				doubleTapGesture.ShouldBeRequiredToFailBy = (a, b) => { return false; };

				//singleTapGesture.RequireGestureRecognizerToFail (doubleTapGesture);
				this.AddGestureRecognizer (singleTapGesture);
				this.AddGestureRecognizer (doubleTapGesture);
			} else {
				var panGesture = new UIPanGestureRecognizer (Pan);
				panGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) => { return true; };
				this.AddGestureRecognizer (panGesture);

				var pinchGesture = new UIPinchGestureRecognizer (Pinch);
				pinchGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) => { return true; };
				this.AddGestureRecognizer (pinchGesture);

				var rotationGesture = new UIRotationGestureRecognizer (Rotate);
				rotationGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) => { return true; };
				this.AddGestureRecognizer (rotationGesture);

				var singleTapGesture = new UITapGestureRecognizer (SingleTap);
				singleTapGesture.NumberOfTapsRequired = 1;
				//singleTapGesture.ShouldRecognizeSimultaneously += ShouldRecognizeSimultaneouslySingle;
				//singleTapGesture.ShouldBeRequiredToFailBy += ShouldRecognizeSimultaneouslySingle;

				var doubleTapGesture = new UITapGestureRecognizer (DoubleTap);
				doubleTapGesture.NumberOfTapsRequired = 2;
				//doubleTapGesture.ShouldRecognizeSimultaneously += ShouldRecognizeSimultaneouslySingle;
				//doubleTapGesture.ShouldBeRequiredToFailBy += ShouldRecognizeSimultaneouslyDouble;

				singleTapGesture.RequireGestureRecognizerToFail (doubleTapGesture);
				this.AddGestureRecognizer (singleTapGesture);
				this.AddGestureRecognizer (doubleTapGesture);
			}

			// set scalefactor.
			_scaleFactor = this.ContentScaleFactor;

			// create the cache renderer.
			_cacheRenderer = new MapRenderer<CGContextWrapper> (
				new CGContextRenderer (_scaleFactor));
			_cachedScene = new Scene2DSimple ();
			_cachedScene.BackColor = SimpleColor.FromKnownColor (KnownColor.White).Value;
		}

		/// <summary>
		/// Gestures the recognizer should begin.
		/// </summary>
		/// <returns><c>true</c>, if recognizer should begin was gestured, <c>false</c> otherwise.</returns>
		/// <param name="gestureRecognizer">Gesture recognizer.</param>
		public override bool GestureRecognizerShouldBegin (UIGestureRecognizer gestureRecognizer)
		{
			return true;
		}

		/// <summary>
		/// Holds the cache renderer.
		/// </summary>
		private MapRenderer<CGContextWrapper> _cacheRenderer;

		/// <summary>
		/// The 
		/// </summary>
		private RectangleF _rect;

		/// <summary>
		/// Holds the cached scene.
		/// </summary>
		private Scene2D _cachedScene;

		/// <summary>
		/// Holds the rendering thread.
		/// </summary>
		private Thread _renderingThread;

		/// <summary>
		/// Notifies change
		/// </summary>
		/// <param name="touch">If set to <c>true</c> change was trigger by touch.</param>
		internal void Change(bool touch)
		{
			if (_rect.Width == 0) {
				return;
			}

			lock (_cacheRenderer) {
				// create the view that would be use for rendering.
				View2D view = _cacheRenderer.Create ((int)(_rect.Width * _extra), (int)(_rect.Height * _extra),
				                                    this.Map, (float)this.Map.Projection.ToZoomFactor (this.MapZoom), 
				                                    this.MapCenter, _invertX, _invertY, this.MapTilt);

				// ... and compare to the previous rendered view.
				if (_previousRenderedZoom != null &&
				   view.Equals (_previousRenderedZoom)) {
					return;
				}
				_previousRenderedZoom = view;

				// end existing rendering thread.
				if (_renderingThread != null &&
				   _renderingThread.IsAlive) {
					if (_cacheRenderer.IsRunning) {
						_cacheRenderer.CancelAndWait ();
					}

					//_renderingThread.Abort ();
				}

				// start new rendering thread.
				_renderingThread = new Thread (new ThreadStart (Render));
				_renderingThread.Start ();

				// raise touched event.
				if (touch) {
					this.RaiseMapTouched ();
				}
			}
		}

		/// <summary>
		/// Holds the scale factor.
		/// </summary>
		private float _scaleFactor = 1;

		/// <summary>
		/// Holds the extra border.
		/// </summary>
		private float _extra = 1.4f;

        /// <summary>
        /// Holds the previous rendered zoom.
        /// </summary>
		private View2D _previousRenderedZoom;

        /// <summary>
        /// Holds the pervious image.
        /// </summary>
        private CGImage _previousImage;

		/// <summary>
		/// Render the current complete scene.
		/// </summary>
		void Render() {
			try {
				RectangleF rect = _rect;

				lock (this.Map) {
				
				//lock (_cacheRenderer) { // make sure only on thread at the same time is using the renderer.
				// create the view.
				View2D view = _cacheRenderer.Create ((int)(rect.Width * _extra), (int)(rect.Height * _extra),
				                                      this.Map, (float)this.Map.Projection.ToZoomFactor (this.MapZoom), 
				                                      this.MapCenter, _invertX, _invertY, this.MapTilt);

				if (view.Equals (_previousRenderedZoom)) {
					return;
				}

				if (rect.Width == 0) { // only render if a proper size is known.
					return;
				}

				OsmSharp.Logging.Log.TraceEvent ("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
				                                 "Before lock.");

				long before = DateTime.Now.Ticks;
	//				OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
	//				                                "Rendering Start");

				// build the layers list.
				var layers = new List<ILayer> ();
				for (int layerIdx = 0; layerIdx < this.Map.LayerCount; layerIdx++) {
					layers.Add (this.Map [layerIdx]);
				}

				// add the internal layer.
				// TODO: create marker layer.
				int imageWidth = (int)(rect.Width * _extra * _scaleFactor);
				int imageHeight = (int)(rect.Height * _extra * _scaleFactor);

				// create a new bitmap context.
				CGColorSpace space = CGColorSpace.CreateDeviceRGB ();
				int bytesPerPixel = 4;
				int bytesPerRow = bytesPerPixel * imageWidth;
				int bitsPerComponent = 8;
//				if (_bytescache == null) {
//					_bytescache = new byte[bytesPerRow * imageHeight];
//				}
				CGBitmapContext gctx = new CGBitmapContext (null, imageWidth, imageHeight,
				                                             bitsPerComponent, bytesPerRow,
				                                             space, // kCGBitmapByteOrder32Little | kCGImageAlphaNoneSkipLast
				                                             CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Big);
                try {
    				// notify the map that the view has changed.
    				this.Map.ViewChanged ((float)this.Map.Projection.ToZoomFactor (this.MapZoom), this.MapCenter, 
    				                       view);
    				long afterViewChanged = DateTime.Now.Ticks;
    				OsmSharp.Logging.Log.TraceEvent ("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
    				                                 "View change took: {0}ms @ zoom level {1}",
    				                                 (new TimeSpan (afterViewChanged - before).TotalMilliseconds), this.MapZoom);

    				// does the rendering.
    				bool complete = _cacheRenderer.Render (new CGContextWrapper (gctx,
    				                                                new RectangleF (0, 0, (int)(rect.Width * _extra), (int)(rect.Height * _extra))), 
    				                                        layers, view);

    				long afterRendering = DateTime.Now.Ticks;
    				OsmSharp.Logging.Log.TraceEvent ("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
    				                                 "Rendering took: {0}ms @ zoom level {1}",
    				                                 (new TimeSpan (afterRendering - afterViewChanged).TotalMilliseconds), this.MapZoom);

    				if (complete) { // there was no cancellation, the rendering completely finished.
    					// add the result to the scene cache.
    					lock (_cachedScene) {
    						// add the newly rendered image again.
    						_cachedScene.Clear ();
                            if(_previousImage != null)
                            {
                                _previousImage.Dispose();
                            }
                            _previousImage = gctx.ToImage ();
    						_cachedScene.AddImage (0, float.MinValue, float.MaxValue, view.Rectangle, new byte[0], 
                                                       _previousImage);
    					}
    					this.InvokeOnMainThread (InvalidateMap);

    					// store the previous view.
    					_previousRenderedZoom = view;
    				}

    				long after = DateTime.Now.Ticks;
    				OsmSharp.Logging.Log.TraceEvent ("OsmSharp.iOS.UI.MapView", System.Diagnostics.TraceEventType.Information,
    				                                  "Rendering in {0}ms", new TimeSpan (after - before).TotalMilliseconds);
    				//				}
    				//}
                    }
                    finally{
                        gctx.Dispose();
                    }
                }
            }
			catch(Exception ex) {
				_cacheRenderer.Reset ();
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
			//RectangleF2D rect = _rect;
			RectangleF rect = this.Frame;
			if (rect.Width > 0) {
				this.StopCurrentAnimation ();
				if (rotation.State == UIGestureRecognizerState.Ended) { 
					View2D rotatedView = _mapViewBefore.RotateAroundCenter ((Radian)rotation.Rotation);
					_mapTilt = (float)((Degree)rotatedView.Rectangle.Angle).Value;
					PointF2D sceneCenter = rotatedView.Rectangle.Center;
					_mapCenter = this.Map.Projection.ToGeoCoordinates (
						sceneCenter [0], sceneCenter [1]);
					this.Change (true);

					_mapViewBefore = null;
				} else if (rotation.State == UIGestureRecognizerState.Began) {
					_mapViewBefore = this.CreateView (rect);
				} else {
					//_mapViewBefore = this.CreateView (_rect);
					View2D rotatedView = _mapViewBefore.RotateAroundCenter ((Radian)rotation.Rotation);
					_mapTilt = (float)((Degree)rotatedView.Rectangle.Angle).Value;
					PointF2D sceneCenter = rotatedView.Rectangle.Center;
					_mapCenter = this.Map.Projection.ToGeoCoordinates (
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
			//RectangleF2D rect = _rect;
			RectangleF rect = this.Frame;
			if (rect.Width > 0) {
				this.StopCurrentAnimation ();
				if (pinch.State == UIGestureRecognizerState.Ended) {
					_mapZoom = _mapZoomLevelBefore.Value;

					double zoomFactor = this.Map.Projection.ToZoomFactor (this.MapZoom);
					zoomFactor = zoomFactor * pinch.Scale;
					_mapZoom = (float)this.Map.Projection.ToZoomLevel (zoomFactor);

					this.NormalizeZoom ();

					this.Change (true); // notifies change.

					_mapZoomLevelBefore = null;
				} else if (pinch.State == UIGestureRecognizerState.Began) {
					_mapZoomLevelBefore = _mapZoom;
				}
				else {
					_mapZoom = _mapZoomLevelBefore.Value;

					double zoomFactor = this.Map.Projection.ToZoomFactor (_mapZoom);
					zoomFactor = zoomFactor * pinch.Scale;
					_mapZoom = (float)this.Map.Projection.ToZoomLevel (zoomFactor);

					this.NormalizeZoom ();

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
			OsmSharp.Logging.Log.TraceEvent ("MapView", System.Diagnostics.TraceEventType.Error, 
			                                 string.Format("{0} ", pan.State.ToString()));

			//RectangleF2D rect = _rect;
			RectangleF rect = this.Frame;
			if (rect.Width > 0) {
				this.StopCurrentAnimation ();
				PointF offset = pan.TranslationInView (this);
				if (pan.State == UIGestureRecognizerState.Ended) {
					_beforePan = null;
					
					this.Change (true); // notifies change.
				} else if (pan.State == UIGestureRecognizerState.Began) {
					_beforePan = this.MapCenter;
                }
                else if (pan.State == UIGestureRecognizerState.Cancelled ||
                  pan.State == UIGestureRecognizerState.Failed)
                {
                    _beforePan = null;
                }
                else if (pan.State == UIGestureRecognizerState.Changed)
                {
                    _mapCenter = _beforePan;

					View2D view = this.CreateView(rect);
                    double centerXPixels = rect.Width / 2.0f - offset.X;
                    double centerYPixels = rect.Height / 2.0f - offset.Y;

                    double[] sceneCenter = view.FromViewPort(rect.Width, rect.Height,
                                                              centerXPixels, centerYPixels);

					_mapCenter = this.Map.Projection.ToGeoCoordinates(
						sceneCenter[0], sceneCenter[1]);

                    this.InvokeOnMainThread(InvalidateMap);
                }
			}
		}

		public delegate void MapTapEventDelegate(GeoCoordinate geoCoordinate);

		/// <summary>
		/// Occurs when the map was tapped at a certain location.
		/// </summary>
		public event MapTapEventDelegate MapTapEvent;

		/// <summary>
		/// Called when the map was single tapped at a certain location.
		/// </summary>
		/// <param name="tap">Tap.</param>
		private void SingleTap(UITapGestureRecognizer tap){
			//RectangleF2D rect = _rect;
			RectangleF rect = this.Frame;
			if(rect.Width > 0 && rect.Height > 0) {
				this.StopCurrentAnimation ();

				if (this.MapTapEvent != null) {
					View2D view = this.CreateView (rect);
					PointF location = tap.LocationInView (this);
					double[] sceneCoordinates = view.FromViewPort (rect.Width, rect.Height, location.X, location.Y);
					this.MapTapEvent (this.Map.Projection.ToGeoCoordinates (sceneCoordinates [0], sceneCoordinates [1]));
				}
			}
		}

		/// <summary>
		/// The map view animator to zoom/pan on double tap.
		/// </summary>
		private MapViewAnimator _doubleTapAnimator;

		/// <summary>
		/// Occurs when the map was double tapped at a certain location.
		/// </summary>
		public event MapTapEventDelegate DoubleMapTapEvent;

		/// <summary>
		/// Called when the tap gesture recognizer detects a double tap.
		/// </summary>
		/// <param name="tap">Tap.</param>
		private void DoubleTap(UITapGestureRecognizer tap){
			//RectangleF2D rect = _rect;
			RectangleF rect = this.Frame;
			if(rect.Width > 0 && rect.Height > 0) {
				this.StopCurrentAnimation ();
				
				View2D view = this.CreateView (rect);
				PointF location = tap.LocationInView (this);
				double[] sceneCoordinates = view.FromViewPort (rect.Width, rect.Height, location.X, location.Y);
				GeoCoordinate geoLocation = this.Map.Projection.ToGeoCoordinates (sceneCoordinates [0], sceneCoordinates [1]);

				if (this.DoubleMapTapEvent != null) {
					this.DoubleMapTapEvent (geoLocation);
				} else {
					// minimum zoom.
					float tapRequestZoom = (float)System.Math.Max (_mapZoom + 1, 19);
					_doubleTapAnimator.Start (geoLocation, tapRequestZoom, new TimeSpan(0,0,1));
				}
			}
		}

		/// <summary>
		/// The map center.
		/// </summary>
		private GeoCoordinate _mapCenter;

		/// <summary>
		/// The map center on the previous rendering.
		/// </summary>
		private GeoCoordinate _previousRenderingMapCenter;

		/// <summary>
		/// Gets or sets the center.
		/// </summary>
		/// <value>The center.</value>
		public GeoCoordinate MapCenter {
			get { return _mapCenter; }
			set { 
				_mapCenter = value;

				this.InvokeOnMainThread (InvalidateMap);
				this.InvalidateMapCenter ();
			}
		}

		/// <summary>
		/// Invalidates the map center.
		/// </summary>
		private void InvalidateMapCenter(){
			if (_autoInvalidate) {
				if (_previousRenderingMapCenter == null || 
				    _previousRenderingMapCenter.DistanceReal (_mapCenter).Value > 40) { 
					// TODO: update this with a more resonable measure depending on the zoom.
					this.Change (false);
					_previousRenderingMapCenter = _mapCenter;
				} 
			}
		}

		/// <summary>
		/// Holds the map.
		/// </summary>
		private Map _map;

		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		/// <value>The map.</value>
		public Map Map {
			get{
				return _map;
			}
			set {
				_map = value;

				this.InvokeOnMainThread (InvalidateMap);
			}
		}

		/// <summary>
		/// Holds the map zoom.
		/// </summary>
		private float _mapZoom;

		/// <summary>
		/// Gets or sets the zoom level.
		/// </summary>
		/// <value>The zoom level.</value>
		public float MapZoom {
			get {
				return _mapZoom;
			}
			set{
				_mapZoom = value;

				this.NormalizeZoom ();

				this.InvokeOnMainThread (InvalidateMap);
			}
        }

		/// <summary>
		/// Normalizes the zoom and fits it inside the min/max bounds.
		/// </summary>
		private void NormalizeZoom() {
			if (this.MapMaxZoomLevel.HasValue &&
			    _mapZoom > this.MapMaxZoomLevel.Value) {
				_mapZoom = this.MapMaxZoomLevel.Value;
			} else if (this.MapMinZoomLevel.HasValue &&
			           _mapZoom < this.MapMinZoomLevel.Value) {
				_mapZoom = this.MapMinZoomLevel.Value;
			}
		}

        /// <summary>
        /// Gets or sets the map max zoom level.
        /// </summary>
        /// <value>The map max zoom level.</value>
        public float? MapMaxZoomLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map minimum zoom level.
        /// </summary>
        /// <value>The map minimum zoom level.</value>
        public float? MapMinZoomLevel
        {
            get;
            set;
        }

		/// <summary>
		/// Holds the map tilte angle.
		/// </summary>
		private Degree _mapTilt;

		/// <summary>
		/// Gets or sets the map tilt.
		/// </summary>
		/// <value>The map tilt.</value>
		public Degree MapTilt {
			get{
				return _mapTilt;
			}
			set {
				_mapTilt = value;

				this.InvokeOnMainThread (InvalidateMap);
			}
		}

		#region IMapView implementation


		/// <summary>
		/// Sets the map view.
		/// </summary>
		/// <param name="center">Center.</param>
		/// <param name="mapTilt">Map tilt.</param>
		/// <param name="mapZoom">Map zoom.</param>
		void IMapView.SetMapView (GeoCoordinate center, Degree mapTilt, float mapZoom)
		{
			_mapCenter = center;
			_mapTilt = mapTilt;
			this.MapZoom = mapZoom;
			this.InvalidateMapCenter ();

			this.InvokeOnMainThread (InvalidateMap);
		}

		/// <summary>
		/// Holds the current animator.
		/// </summary>
		private MapViewAnimator _mapViewAnimator;

		/// <summary>
		/// Stops the current animation.
		/// </summary>
		private void StopCurrentAnimation()
		{
			if(_mapViewAnimator != null) {
				_mapViewAnimator.Stop ();
			}
		}

		/// <summary>
		/// Registers the animator.
		/// </summary>
		/// <param name="mapViewAnimator">Map view animator.</param>
		void IMapView.RegisterAnimator (MapViewAnimator mapViewAnimator)
		{
			_mapViewAnimator = mapViewAnimator;
		}

		/// <summary>
		/// The auto invalidate flag.
		/// </summary>
		private bool _autoInvalidate = true;

		/// <summary>
		/// Invalidate this instance.
		/// </summary>
		void IMapView.Invalidate ()
		{
			this.Change (false);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="OsmSharp.iOS.UI.MapView"/> auto invalidate.
		/// </summary>
		/// <value><c>true</c> if auto invalidate; otherwise, <c>false</c>.</value>
		bool IMapView.AutoInvalidate {
			get {
				return _autoInvalidate;
			}
			set {
				_autoInvalidate = value;
			}
		}

		#endregion

		/// <summary>
		/// Holds the drawing rectangle.
		/// </summary>
		//private System.Drawing.RectangleF _rect;

		/// <summary>
		/// Creates the view.
		/// </summary>
		/// <returns>The view.</returns>
		public View2D CreateView(System.Drawing.RectangleF rect)
		{
			double[] sceneCenter = this.Map.Projection.ToPixel (this.MapCenter.Latitude, this.MapCenter.Longitude);
			float sceneZoomFactor = (float)this.Map.Projection.ToZoomFactor (this.MapZoom);

			return View2D.CreateFrom (sceneCenter [0], sceneCenter [1],
			                         rect.Width, rect.Height, sceneZoomFactor,
			                         _invertX, _invertY, this.MapTilt);
		}

		/// <summary>
		/// Invalidates the map.
		/// </summary>
		private void InvalidateMap()
		{
			// change the map markers.
			//RectangleF2D rect = _rect;
			RectangleF rect = this.Frame;
			if (rect.Width > 0 && rect.Height > 0) {
				View2D view = this.CreateView (rect);

				this.NotifyMapChangeToMarkers (rect.Width, rect.Height, view, this.Map.Projection);
			}
			this.SetNeedsDisplay ();
		}

		/// <Docs>Lays out subviews.</Docs>
		/// <summary>
		/// Layouts the subviews.
		/// </summary>
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			this.InvalidateMap ();
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
				//View2D view = this.CreateView (_rect);
				View2D view = this.CreateView (this.Frame);

				// call the canvas renderer.
				CGContext context = UIGraphics.GetCurrentContext ();

				if (context != null) {
					context.InterpolationQuality = CGInterpolationQuality.None;
					context.SetShouldAntialias (false);
					context.SetBlendMode (CGBlendMode.Copy);
					context.SetAlpha (1);

					long afterViewChanged = DateTime.Now.Ticks;
					CGContextRenderer renderer = new CGContextRenderer (1);
//					renderer.Render (new CGContextWrapper (context, _rect),
					//					                _cachedScene, view);
					renderer.Render (new CGContextWrapper (context, this.Frame),
						_cachedScene, view);
					long afterRendering = DateTime.Now.Ticks;
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
            if (marker == null) { throw new ArgumentNullException("marker"); };

			marker.AttachTo (this); // attach this view to the marker.
			_markers.Add (marker); // add to markers list.
			this.Add (marker); // add to this view.

			RectangleF rect = this.Frame;
			if (rect.Width > 0 && rect.Height > 0) {
				View2D view = this.CreateView (rect);
				this.NotifyMapChangeToMarker (rect.Width, rect.Height, view, this.Map.Projection, marker);
			}
		}

		/// <summary>
		/// Adds the marker.
		/// </summary>
		/// <returns>The marker.</returns>
		/// <param name="location">Location.</param>
		public MapMarker AddMarker(GeoCoordinate location)
        {
            if (location == null) { throw new ArgumentNullException("location"); };

			MapMarker marker = new MapMarker (location);
			this.AddMarker (marker);
			return marker;
		}

        /// <summary>
        /// Clears all map markers.
        /// </summary>
        public void ClearMarkers()
        {
            if (_markers != null)
            {
                foreach (MapMarker marker in _markers)
                {
                    marker.DetachFrom(this);
                    marker.RemoveFromSuperview();
                }
                _markers.Clear();
            }
        }

        /// <summary>
        /// Removes the given map marker.
        /// </summary>
        /// <param name="marker"></param>
        public bool RemoveMarker(MapMarker marker)
        {
            if (marker == null)
            {
                marker.DetachFrom(this); // remove the map view.
                marker.RemoveFromSuperview();
                return _markers.Remove(marker);
            }
            return false;
        }

        /// <summary>
        /// Zoom to the current markers.
        /// </summary>
        public void ZoomToMarkers()
        {
            this.ZoomToMarkers(_markers);
        }

        /// <summary>
        /// Zoom to the current markers.
        /// </summary>
        public void ZoomToMarkers(double percentage)
        {
            this.ZoomToMarkers(_markers, percentage);
        }

        /// <summary>
        /// Zoom to the current markers.
        /// </summary>
        public void ZoomToMarkers(List<MapMarker> markers)
        {
            this.ZoomToMarkers(markers, 15);
        }

        /// <summary>
        /// Zoom to the given makers list.
        /// </summary>
        /// <param name="marker"></param>
        public void ZoomToMarkers(List<MapMarker> markers, double percentage)
        {
//            float height = _rect.Height;
//            float width = _rect.Width;
			float width = this.Frame.Width;
			float height = this.Frame.Height;
			RectangleF rect = this.Frame;
            if (width > 0)
            {
                PointF2D[] points = new PointF2D[markers.Count];
                for (int idx = 0; idx < markers.Count; idx++)
                {
                    points[idx] = new PointF2D(this.Map.Projection.ToPixel(markers[idx].Location));
				}
				View2D view = this.CreateView(rect);
//                View2D view = this.CreateView(this.Frame);
                View2D fittedView = view.Fit(points, percentage);

                float zoom = (float)this.Map.Projection.ToZoomLevel(fittedView.CalculateZoom(
                    width, height));
                GeoCoordinate center = this.Map.Projection.ToGeoCoordinates(
                    fittedView.Center[0], fittedView.Center[1]);
				
				_mapCenter = center;
				this.MapZoom = zoom;

                this.Change(false);
            }
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
			RectangleF rect = this.Frame;
			// notify map layout of changes.
			if (rect.Width > 0 && rect.Height > 0) {
				View2D view = this.CreateView (rect);

				this.NotifyMapChangeToMarker (rect.Width, rect.Height, view, this.Map.Projection, mapMarker);
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

        /// <summary>
        /// Notifies a map change.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        public void NotifyMapChange(double pixelsWidth, double pixelsHeight, View2D view, Math.Geo.Projections.IProjection projection)
        {

        }

		/// <summary>
		/// Raises the map touched event.
		/// </summary>
		private void RaiseMapTouched() {
			if (this.MapTouched != null) {
				this.MapTouched (this, this.MapZoom, this.MapTilt, this.MapCenter);
			}
		}

		/// <summary>
		/// Dispose the specified disposing.
		/// </summary>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			_cachedScene.Clear ();
			_cachedScene = null;
			_renderingThread.Abort ();
			_renderingThread = null;
			//_bytescache = null;
		}
	}
}


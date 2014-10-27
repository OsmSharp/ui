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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.UI;
using OsmSharp.UI.Animations;
using OsmSharp.UI.Animations.Invalidation.Triggers;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Units.Angle;
using System.Collections.ObjectModel;
using OsmSharp.iOS.UI.Controls;

namespace OsmSharp.iOS.UI
{
	/// <summary>
	/// Map view.
	/// </summary>
	[Register("MapView")]
    public class MapView : UIView, IMapView, IInvalidatableMapSurface, IMapControlHost
	{
        private const float MAX_ZOOM_LEVEL = 22;
		private bool _invertX = false;
        private bool _invertY = false;

        /// <summary>
        /// Map touched down event.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouchedDown;

        /// <summary>
        /// Map touched event.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouched;

        /// <summary>
        /// Map touched up event.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouchedUp;

        /// <summary>
        /// Raised when the map was first initialized, meaning it has a size and it was rendered for the first time.
        /// </summary>
        public event MapViewDelegates.MapInitialized MapInitialized;

        /// <summary>
        /// Raised when the map moves.
        /// </summary>
        public event MapViewDelegates.MapMoveDelegate MapMove;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public MapView(IntPtr handle) : base(handle)
		{
			this.Initialize(new GeoCoordinate(0, 0), new Map(), 0, 16);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		/// <param name="t">T.</param>
		public MapView(NSObjectFlag t) : base(t)
		{
			this.Initialize(new GeoCoordinate(0, 0), new Map(), 0, 16);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		/// <param name="coder">Coder.</param>
		public MapView(NSCoder coder) : base(coder)
		{
			this.Initialize(new GeoCoordinate(0, 0), new Map(), 0, 16);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		/// <param name="frame">Frame.</param>
		public MapView(RectangleF frame) : base(frame)
		{
			this.Initialize(new GeoCoordinate(0, 0), new Map(), 0, 16);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		public MapView()
		{
			this.Initialize(new GeoCoordinate(0, 0), new Map(), 0, 16);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		/// <param name="mapCenter">Map center.</param>
		/// <param name="map">Map.</param>
		/// <param name="defaultZoom">Default zoom.</param>
		public MapView(GeoCoordinate mapCenter, Map map, float defaultZoom)
		{
			this.Initialize(mapCenter, map, 0, defaultZoom);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		/// <param name="mapCenter">Map center.</param>
		/// <param name="map">Map.</param>
		/// <param name="mapTilt">Map tilt.</param>
		/// <param name="defaultZoom">Default zoom.</param>
		public MapView(GeoCoordinate mapCenter, Map map, Degree mapTilt, float defaultZoom)
		{
			this.Initialize(mapCenter, map, mapTilt, defaultZoom);
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
			// register the default listener.
			(this as IInvalidatableMapSurface).RegisterListener(new DefaultTrigger(this));

			// enable all interactions by default.
			this.MapAllowPan = true;
			this.MapAllowTilt = true;
			this.MapAllowZoom = true;

			// set clip to bounds to prevent objects from being rendered/show outside of the mapview.
			this.ClipsToBounds = true;

			MapCenter = defaultMapCenter;
			_map = defaultMap;
			MapTilt = defaultMapTilt;
			MapZoom = defaultMapZoom;

            _map.MapChanged += MapChanged;

			_doubleTapAnimator = new MapViewAnimator(this);

			this.BackgroundColor = UIColor.White;
			this.UserInteractionEnabled = true;

			if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
			{
				var panGesture = new UIPanGestureRecognizer(Pan);
				panGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) =>
				{ 
					return true; 
				};
				// TODO: workaround for xamarin bug, remove later!
				panGesture.ShouldRequireFailureOf = (a, b) =>
				{
					return false;
				};
				panGesture.ShouldBeRequiredToFailBy = (a, b) =>
				{
					return false;
				};
				this.AddGestureRecognizer(panGesture);

				var pinchGesture = new UIPinchGestureRecognizer(Pinch);
				pinchGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) =>
				{ 
					return true; 
				};
				// TODO: workaround for xamarin bug, remove later!
				pinchGesture.ShouldRequireFailureOf = (a, b) =>
				{
					return false;
				};
				pinchGesture.ShouldBeRequiredToFailBy = (a, b) =>
				{
					return false;
				};
				this.AddGestureRecognizer(pinchGesture);

				var rotationGesture = new UIRotationGestureRecognizer(Rotate);
				rotationGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) =>
				{ 
					return true; 
				};
				// TODO: workaround for xamarin bug, remove later!
				rotationGesture.ShouldRequireFailureOf = (a, b) =>
				{
					return false;
				};
				rotationGesture.ShouldBeRequiredToFailBy = (a, b) =>
				{
					return false;
				};
				this.AddGestureRecognizer(rotationGesture);

				var singleTapGesture = new UITapGestureRecognizer(SingleTap);
				singleTapGesture.NumberOfTapsRequired = 1;
				// TODO: workaround for xamarin bug, remove later!
//				singleTapGesture.ShouldRequireFailureOf = (a, b) => { return false; };
//				singleTapGesture.ShouldBeRequiredToFailBy = (a, b) => { return false; };

				var doubleTapGesture = new UITapGestureRecognizer(DoubleTap);
				doubleTapGesture.NumberOfTapsRequired = 2;
				// TODO: workaround for xamarin bug, remove later!
//				doubleTapGesture.ShouldRequireFailureOf = (a, b) => { return false; };
//				doubleTapGesture.ShouldBeRequiredToFailBy = (a, b) => { return false; };

				//singleTapGesture.RequireGestureRecognizerToFail (doubleTapGesture);
				this.AddGestureRecognizer(singleTapGesture);
				this.AddGestureRecognizer(doubleTapGesture);
			}
			else
			{
				var panGesture = new UIPanGestureRecognizer(Pan);
				panGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) =>
				{
					return true;
				};
				this.AddGestureRecognizer(panGesture);

				var pinchGesture = new UIPinchGestureRecognizer(Pinch);
				pinchGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) =>
				{
					return true;
				};
				this.AddGestureRecognizer(pinchGesture);

				var rotationGesture = new UIRotationGestureRecognizer(Rotate);
				rotationGesture.ShouldRecognizeSimultaneously += (UIGestureRecognizer r, UIGestureRecognizer other) =>
				{
					return true;
				};
				this.AddGestureRecognizer(rotationGesture);

				var singleTapGesture = new UITapGestureRecognizer(SingleTap);
				singleTapGesture.NumberOfTapsRequired = 1;
				//singleTapGesture.ShouldRecognizeSimultaneously += ShouldRecognizeSimultaneouslySingle;
				//singleTapGesture.ShouldBeRequiredToFailBy += ShouldRecognizeSimultaneouslySingle;

				var doubleTapGesture = new UITapGestureRecognizer(DoubleTap);
				doubleTapGesture.NumberOfTapsRequired = 2;
				//doubleTapGesture.ShouldRecognizeSimultaneously += ShouldRecognizeSimultaneouslySingle;
				//doubleTapGesture.ShouldBeRequiredToFailBy += ShouldRecognizeSimultaneouslyDouble;

				singleTapGesture.RequireGestureRecognizerToFail(doubleTapGesture);
				this.AddGestureRecognizer(singleTapGesture);
				this.AddGestureRecognizer(doubleTapGesture);
			}

			// set scalefactor.
			_scaleFactor = this.ContentScaleFactor;

			// create the cache renderer.
			_cacheRenderer = new MapRenderer<CGContextWrapper>(
				new CGContextRenderer(_scaleFactor));
			_backgroundColor = SimpleColor.FromKnownColor(KnownColor.White).Value;
		}

        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public override RectangleF Frame
        {
            get
            {
                return base.Frame;
            }
            set
            {
                base.Frame = value;

                if (_rect.Width == 0 && value.Width != 0)
                { // trigger the initial rendering when the frame has a size for the first time.
                    _rect = value;
                    (this as IMapView).Invalidate();
                }
            }
        }


		/// <summary>
		/// Gestures the recognizer should begin.
		/// </summary>
		/// <returns><c>true</c>, if recognizer should begin was gestured, <c>false</c> otherwise.</returns>
		/// <param name="gestureRecognizer">Gesture recognizer.</param>
		public override bool GestureRecognizerShouldBegin(UIGestureRecognizer gestureRecognizer)
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
		/// Holds the on screen buffered image.
		/// </summary>
		private ImageTilted2D _onScreenBuffer;
		/// <summary>
		/// Holds the buffer synchronisation object.
		/// </summary>
		private object _bufferSynchronisation = new object();
		/// <summary>
		/// Holds the render synchronisation object.
		/// </summary>
		private object _renderSynchronisation = new object();
		/// <summary>
		/// Holds the background color.
		/// </summary>
		private int _backgroundColor;
		/// <summary>
		/// Holds the rendering thread.
		/// </summary>
		private Thread _renderingThread;

		/// <summary>
		/// Holds a boolean that holds triggered notify movement.
		/// </summary>
		private bool _triggeredNotifyMovement = false;

		/// <summary>
		/// Notifies change
		/// </summary>
		internal void TriggerRendering()
		{
			if (_rect.Width == 0)
			{
				return;
			}

			if (Monitor.TryEnter(_cacheRenderer, 1000))
			{ // entered the exclusive lock area.
				try
				{
					// create the view that would be use for rendering.
					float size = System.Math.Max(_rect.Width, _rect.Height);
					View2D view = _cacheRenderer.Create((int)(size * _extra), (int)(size * _extra),
						              this.Map, (float)this.Map.Projection.ToZoomFactor(this.MapZoom), 
						              this.MapCenter, _invertX, _invertY, this.MapTilt);

					// ... and compare to the previous rendered view.
					if (_previouslyRenderedView != null &&
					    view.Equals(_previouslyRenderedView))
					{
						_listener.NotifyRenderSuccess(view, this.MapZoom, 0);
						return;
					}
					_previouslyRenderedView = view;

					// end existing rendering thread.
					if (_renderingThread != null &&
					    _renderingThread.IsAlive)
					{
						if (_cacheRenderer.IsRunning)
						{
							_cacheRenderer.CancelAndWait();
						}
					}

					// start new rendering thread.
					_renderingThread = new Thread(new ThreadStart(Render));
					_renderingThread.Start();
				}
				finally
				{
					Monitor.Exit(_cacheRenderer);
				}
			}
		}

		/// <summary>
		/// Stops the current rendering.
		/// </summary>
		internal void StopRendering()
		{
			// end existing rendering thread.
			if (_renderingThread != null &&
				_renderingThread.IsAlive)
			{
				if (_cacheRenderer.IsRunning)
				{
					_cacheRenderer.CancelAndWait();
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
		private float _extra = 1.5f;

		/// <summary>
		/// Holds the previous rendered zoom.
		/// </summary>
		private View2D _previouslyRenderedView;

		/// <summary>
		/// Render the current complete scene.
		/// </summary>
		void Render()
		{
			try
			{
				if (Monitor.TryEnter(_cacheRenderer, 1000))
				{
					try
					{
							// use object
						RectangleF rect = _rect;

						// create the view.
						float size = System.Math.Max(_rect.Width, _rect.Height);
						var view = _cacheRenderer.Create((int)(size * _extra), (int)(size * _extra),
							              this.Map, (float)this.Map.Projection.ToZoomFactor(this.MapZoom),
                                          this.MapCenter, _invertX, _invertY, this.MapTilt);
						if (rect.Width == 0)
						{ // only render if a proper size is known.
							return;
						}
	                        
						// calculate width/height.
						int imageWidth = (int)(size * _extra * _scaleFactor);
						int imageHeight = (int)(size * _extra * _scaleFactor);

						// create a new bitmap context.
						CGColorSpace space = CGColorSpace.CreateDeviceRGB();
						int bytesPerPixel = 4;
						int bytesPerRow = bytesPerPixel * imageWidth;
						int bitsPerComponent = 8;

						// get old image if available.
						CGBitmapContext image = new CGBitmapContext(null, imageWidth, imageHeight,
							                        bitsPerComponent, bytesPerRow,
							                        space, // kCGBitmapByteOrder32Little | kCGImageAlphaNoneSkipLast
							                        CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Big);

						long before = DateTime.Now.Ticks;

						// build the layers list.
						var layers = new List<Layer>();
						for (int layerIdx = 0; layerIdx < this.Map.LayerCount; layerIdx++)
						{
							layers.Add(this.Map[layerIdx]);
						}

						// add the internal layer.
						try
						{
							image.SetRGBFillColor(1, 1, 1, 1);
							image.FillRect(new RectangleF(
								0, 0, imageWidth, imageHeight));

                            // notify the map that the view has changed.
                            var normalView = _cacheRenderer.Create(_rect.Width, _rect.Height,
                                              this.Map, (float)this.Map.Projection.ToZoomFactor(this.MapZoom),
                                              this.MapCenter, _invertX, _invertY, this.MapTilt);
							this.Map.ViewChanged((float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter,
                                normalView, view);
							long afterViewChanged = DateTime.Now.Ticks;
							OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", TraceEventType.Information,
								"View change took: {0}ms @ zoom level {1}",
								(new TimeSpan(afterViewChanged - before).TotalMilliseconds), this.MapZoom);

							float zoomFactor = this.MapZoom;
							float sceneZoomFactor = (float)this.Map.Projection.ToZoomFactor(this.MapZoom);

							// does the rendering.
							bool complete = _cacheRenderer.Render(new CGContextWrapper(image,
								new RectangleF(0, 0, (int)(size * _extra), (int)(size * _extra))),
                                                _map.Projection, layers, view, sceneZoomFactor);

							long afterRendering = DateTime.Now.Ticks;

							if (complete)
							{ // there was no cancellation, the rendering completely finished.
								lock (_bufferSynchronisation)
								{
									if (_onScreenBuffer != null &&
                                        _onScreenBuffer.NativeImage != null)
									{ // on screen buffer.
                                        _onScreenBuffer.NativeImage.Dispose();
									}

									// add the newly rendered image again.           
                                    _onScreenBuffer = new ImageTilted2D(view.Rectangle, 
                                        new NativeImage(image.ToImage()), float.MinValue, float.MaxValue);

									// store the previous view.
									_previouslyRenderedView = view;
								}

								// make sure this view knows that there is a new rendering.
								this.InvokeOnMainThread(SetNeedsDisplay);
							}

							long after = DateTime.Now.Ticks;

							if(complete)
							{ // notify invalidation listener about a succesfull rendering.
								OsmSharp.Logging.Log.TraceEvent("OsmSharp.iOS.UI.MapView", TraceEventType.Information,
									"Rendering succesfull after {0}ms.", new TimeSpan(after - before).TotalMilliseconds);

								_listener.NotifyRenderSuccess(view, zoomFactor, (int)new TimeSpan(after - before).TotalMilliseconds);
							}
							else
							{ // rendering incomplete.
								OsmSharp.Logging.Log.TraceEvent("OsmSharp.iOS.UI.MapView", TraceEventType.Information,
									"Rendering cancelled.", new TimeSpan(after - before).TotalMilliseconds);
							}
						}
						finally
						{

						}
					}
					finally
					{ // make sure the object lock is release.
						Monitor.Exit(_cacheRenderer);
					}
				}
			}
			catch (Exception)
			{
				_cacheRenderer.Reset();
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
		private void Rotate(UIRotationGestureRecognizer rotation)
		{
			RectangleF rect = this.Frame;
			if (this.MapAllowTilt &&
                rect.Width > 0 && this.Map != null)
			{
				this.StopCurrentAnimation();
				if (rotation.State == UIGestureRecognizerState.Ended)
				{
					this.NotifyMovementByInvoke();;

                    _mapViewBefore = null;

                    // raise map touched event.
                    this.RaiseMapTouched();
                    this.RaiseMapTouchedUp();
				}
				else if (rotation.State == UIGestureRecognizerState.Began)
                {
                    this.RaiseMapTouchedDown();
					_mapViewBefore = this.CreateView(rect);
				}
				else
				{
					//_mapViewBefore = this.CreateView (_rect);
					View2D rotatedView = _mapViewBefore.RotateAroundCenter((Radian)rotation.Rotation);
					_mapTilt = (float)((Degree)rotatedView.Rectangle.Angle).Value;
					PointF2D sceneCenter = rotatedView.Rectangle.Center;
					this.MapCenter = this.Map.Projection.ToGeoCoordinates(
						sceneCenter[0], sceneCenter[1]);

					this.NotifyMovementByInvoke();

                    // raise map move event.
                    this.RaiseMapMove();
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
			RectangleF rect = this.Frame;
			if (this.MapAllowZoom &&
			    rect.Width > 0)
			{
				this.StopCurrentAnimation();
				if (pinch.State == UIGestureRecognizerState.Ended)
				{
					this.NotifyMovementByInvoke();

                    _mapZoomLevelBefore = null;

                    // raise map touched event.
                    this.RaiseMapTouched();
                    this.RaiseMapTouchedUp();
				}
				else if (pinch.State == UIGestureRecognizerState.Began)
                {
                    this.RaiseMapTouchedDown();
                    _mapZoomLevelBefore = MapZoom;
				}
				else
				{
					MapZoom = _mapZoomLevelBefore.Value;

                    double zoomFactor = this.Map.Projection.ToZoomFactor(MapZoom);
					zoomFactor = zoomFactor * pinch.Scale;
                    MapZoom = (float)this.Map.Projection.ToZoomLevel(zoomFactor);

                    this.NotifyMovementByInvoke();

                    // raise map move event.
                    this.RaiseMapMove();
				}
			}
		}

		/// <summary>
		/// Holds the previous pan offset.
		/// </summary>
        private PointF _prevOffset;
		/// <summary>
		/// Pan the specified some.
		/// </summary>
		/// <param name="some">Some.</param>
		private void Pan(UIPanGestureRecognizer pan)
		{
			RectangleF rect = this.Frame;
			if (this.MapAllowPan &&
                rect.Width > 0 && this.Map != null)
			{
				this.StopCurrentAnimation();
				PointF offset = pan.TranslationInView(this);
				if (pan.State == UIGestureRecognizerState.Ended)
				{
					this.NotifyMovementByInvoke();

                    // raise map touched event.
                    this.RaiseMapTouched();
                    this.RaiseMapTouchedUp();
				}
				else if (pan.State == UIGestureRecognizerState.Began)
				{
                    _prevOffset = new PointF(0, 0);
                    this.RaiseMapTouchedDown();
				}
				else if (pan.State == UIGestureRecognizerState.Changed)
				{
					View2D view = this.CreateView(rect);
					double centerXPixels = rect.Width / 2.0f - (offset.X - _prevOffset.X);
					double centerYPixels = rect.Height / 2.0f - (offset.Y - _prevOffset.Y);

                    _prevOffset = offset;

					double[] sceneCenter = view.FromViewPort(rect.Width, rect.Height,
						                       centerXPixels, centerYPixels);

                    this.MapCenter = this.Map.Projection.ToGeoCoordinates(
						sceneCenter[0], sceneCenter[1]);

                    this.NotifyMovementByInvoke();

                    // raise map move event.
                    this.RaiseMapMove();
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
		private void SingleTap(UITapGestureRecognizer tap)
		{
			RectangleF rect = this.Frame;
            if (rect.Width > 0 && rect.Height > 0 && this.Map != null)
			{
				this.StopCurrentAnimation();

				if (this.MapTapEvent != null)
				{
					View2D view = this.CreateView(rect);
					PointF location = tap.LocationInView(this);
					double[] sceneCoordinates = view.FromViewPort(rect.Width, rect.Height, location.X, location.Y);
					this.MapTapEvent(this.Map.Projection.ToGeoCoordinates(sceneCoordinates[0], sceneCoordinates[1]));
				}

                // notify controls map was tapped.
                this.NotifyMapTapToControls();
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
		private void DoubleTap(UITapGestureRecognizer tap)
		{
			RectangleF rect = this.Frame;
			if (this.MapAllowZoom &&
                rect.Width > 0 && rect.Height > 0 && this.Map != null)
			{
				this.StopCurrentAnimation();
				
				View2D view = this.CreateView(rect);
				PointF location = tap.LocationInView(this);
				double[] sceneCoordinates = view.FromViewPort(rect.Width, rect.Height, location.X, location.Y);
				GeoCoordinate geoLocation = this.Map.Projection.ToGeoCoordinates(sceneCoordinates[0], sceneCoordinates[1]);

				if (this.DoubleMapTapEvent != null)
				{
					this.DoubleMapTapEvent(geoLocation);
				}
				else
				{ // minimum zoom.
                    // Clamp the zoom level between the configured maximum and minimum.
                    float tapRequestZoom = MapZoom + 0.5f;
					_doubleTapAnimator.Start(geoLocation, tapRequestZoom, new TimeSpan(0, 0, 0, 0, 500));
                }

                // notify controls map was tapped.
                this.NotifyMapTapToControls();
			}
		}

		/// <summary>
		/// Notifies that there was movement by invoking NotifyMovement on the main thread.
		/// </summary>
		private void NotifyMovementByInvoke()
		{ // make sure that there are not too many queued events and movement notifications.
			if (!_triggeredNotifyMovement)
			{ // notify move on main thread.
				_triggeredNotifyMovement = true;
				this.InvokeOnMainThread(NotifyMovement);
			}
		}

		/// <summary>
		/// Notifies that there was movement.
		/// </summary>
		private void NotifyMovement()
		{
			// invalidate the map.
			this.InvalidateMap();
			_triggeredNotifyMovement = false;

			// change the map markers.
			RectangleF rect = this.Frame;
            if (rect.Width > 0 && rect.Height > 0 && this.Map != null)
			{
				// create the current view.
				View2D view = this.CreateView(rect);

				_listener.NotifyChange(view, this.MapZoom);
			}
		}

		/// <summary>
		/// Holds the current invalidation listener.
		/// </summary>
		private TriggerBase _listener;

		/// <summary>
		/// Returns true if the map view is sure that it is still moving.
		/// </summary>
		/// <returns><c>true</c>, if moving was stilled, <c>false</c> otherwise.</returns>
		bool IInvalidatableMapSurface.StillMoving()
		{
			return _mapViewAnimator != null;
		}

		/// <summary>
		/// Triggers rendering.
		/// </summary>
		void IInvalidatableMapSurface.Render()
		{
			this.TriggerRendering();
		}

		/// <summary>
		/// Cancels the current rendering.
		/// </summary>
		/// <returns><c>true</c> if this instance cancel render; otherwise, <c>false</c>.</returns>
		void IInvalidatableMapSurface.CancelRender()
		{
			this.StopRendering();
		}

		/// <summary>
		/// Registers the listener.
		/// </summary>
		/// <param name="listener">Listener.</param>
		void IInvalidatableMapSurface.RegisterListener(TriggerBase listener)
		{
			_listener = listener;
		}

		/// <summary>
		/// Resets the listener.
		/// </summary>
		void IInvalidatableMapSurface.ResetListener()
		{
			_listener = null;
		}

		/// <summary>
		/// Holds the map.
		/// </summary>
		private Map _map;

		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		/// <value>The map.</value>
		public Map Map
		{
			get { return _map; }
			set { 
                if (_map != null)
                {
                    _map.MapChanged-=MapChanged;
                }
                _map = value;
                if (_map != null)
                {
                    _map.MapChanged+=MapChanged;
                }
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
		public float MapZoom
		{
			get	{ return _mapZoom; }
			set {
                if (value > _mapMaxZoomLevel)
                {
                    _mapZoom = _mapMaxZoomLevel;
                }
                else if (value < _mapMinZoomLevel)
                {
                    _mapZoom = _mapMinZoomLevel;
                }
                else
                {
                    _mapZoom = value;
                }

				this.NotifyMovementByInvoke();
			}
		}

		/// <summary>
		/// Gets or sets the map max zoom level.
		/// </summary>
		/// <value>The map max zoom level.</value>
        private float _mapMaxZoomLevel = MAX_ZOOM_LEVEL;

		public float MapMaxZoomLevel
		{
            get {  return _mapMaxZoomLevel; }
            set { _mapMaxZoomLevel = (value < MAX_ZOOM_LEVEL) ? value : MAX_ZOOM_LEVEL; }
		}

		/// <summary>
		/// Gets or sets the map minimum zoom level.
		/// </summary>
		/// <value>The map minimum zoom level.</value>
        private float _mapMinZoomLevel = 0;
		public float MapMinZoomLevel
		{
            get { return _mapMinZoomLevel; }
            set { _mapMinZoomLevel = (value > 0) ? value : 0; }
		}

		/// <summary>
		/// Holds the map tilte angle.
		/// </summary>
		private Degree _mapTilt;

		/// <summary>
		/// Gets or sets the map tilt.
		/// </summary>
		/// <value>The map tilt.</value>
		public Degree MapTilt
		{
			get
			{
				return _mapTilt;
			}
			set
			{
				_mapTilt = value;

				this.NotifyMovementByInvoke();
			}
		}

        /// <summary>
        /// The map center.
        /// </summary>
        private GeoCoordinate _mapCenter;

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        /// <value>The center.</value>
        public GeoCoordinate MapCenter
        {
            get { return _mapCenter; }
            set
            {
                if (this.CurrentWidth == 0 || this.MapBoundingBox == null)
                {
                    _mapCenter = value;
                }
                else
                {
                    if (_rect.Width > 0 && _rect.Height > 0 && this.Map != null)
                    {
                        View2D view = this.CreateView(_rect);
                        _mapCenter = this.Map.EnsureViewWithinBoundingBox(value, this.MapBoundingBox, view);
                    }
                    else
                    {
                        _mapCenter = value;
                    }
                }
                
                this.NotifyMovementByInvoke();
            }
        }

        /// <summary>
        /// Box within which one can pan the map
        /// </summary>
        private GeoCoordinateBox _mapBoundingBox = null;

        /// <summary>
        /// Gets or sets the bounding box within which one can pan the map.
        /// </summary>
        /// <value>The box.</value>
        public GeoCoordinateBox MapBoundingBox
        {
            get
            {
                return _mapBoundingBox;
            }
            set
            {
                // If the current map center falls outside the bounding box, set the MapCenter to the middle of the box.
                if (_mapCenter != null && !value.Contains(MapCenter))
                {
                    MapCenter = new GeoCoordinate(value.MinLat + 0.5f * value.DeltaLat, value.MinLon + 0.5f * value.DeltaLon);
                }
                _mapBoundingBox = value;
            }
        }

		/// <summary>
		/// Gets or sets the map tilt flag.
		/// </summary>
		public bool MapAllowTilt
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the map pan flag.
		/// </summary>
		public bool MapAllowPan
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the map zoom flag.
		/// </summary>
		public bool MapAllowZoom
		{
			get;
			set;
		}

        /// <summary>
        /// Gets the density.
        /// </summary>
        public float Density
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        public View2D CurrentView
        {
            get
            {
                if (_rect.Width != 0)
                {
                    return this.CreateView(_rect);
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the current width.
        /// </summary>
        public int CurrentWidth
        {
            get { return (int)_rect.Width; }
        }

        /// <summary>
        /// Returns the current height.
        /// </summary>
        public int CurrentHeight
        {
            get { return (int)_rect.Height; }
        }

		#region IMapView implementation

		/// <summary>
		/// Sets the map view.
		/// </summary>
		/// <param name="center">Center.</param>
		/// <param name="mapTilt">Map tilt.</param>
		/// <param name="mapZoom">Map zoom.</param>
		void IMapView.SetMapView(GeoCoordinate center, Degree mapTilt, float mapZoom)
		{
			MapCenter = center;
			MapTilt = mapTilt;
            MapZoom = mapZoom;

			this.NotifyMovementByInvoke();
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
			if (_mapViewAnimator != null)
			{
				_mapViewAnimator.Stop();
			}
		}

		/// <summary>
		/// Registers the animator.
		/// </summary>
		/// <param name="mapViewAnimator">Map view animator.</param>
		void IMapView.RegisterAnimator(MapViewAnimator mapViewAnimator)
		{
			_mapViewAnimator = mapViewAnimator;
		}

		/// <summary>
		/// Invalidate this instance.
		/// </summary>
		void IMapView.Invalidate()
		{
			this.TriggerRendering();
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
            if (this.Map != null)
            {
                double[] sceneCenter = this.Map.Projection.ToPixel(this.MapCenter.Latitude, this.MapCenter.Longitude);
                float sceneZoomFactor = (float)this.Map.Projection.ToZoomFactor(this.MapZoom);

                return View2D.CreateFrom(sceneCenter[0], sceneCenter[1],
                    rect.Width, rect.Height, sceneZoomFactor,
                    _invertX, _invertY, this.MapTilt);
            }
            return null;
		}

        /// <summary>
        /// Holds the initialized flag.
        /// </summary>
        private bool _initialized = false;

		/// <summary>
		/// Invalidates the map.
		/// </summary>
		private void InvalidateMap()
		{
			// change the map markers.
			RectangleF rect = this.Frame;
            if (rect.Width > 0 && rect.Height > 0 && this.Map != null)
			{
				View2D view = this.CreateView(rect);

				this.NotifyMapChangeToControls(rect.Width, rect.Height, view, this.Map.Projection);

                if (!_initialized)
                { // map has a size, markers and controls have been place, map is officially initialized there!
                    _initialized = true;
                    this.RaiseMapInitialized();
                }
			}

			// tell this view it needs to refresh.
			this.SetNeedsDisplay();
		}

		/// <Docs>Lays out subviews.</Docs>
		/// <summary>
		/// Layouts the subviews.
		/// </summary>
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.InvalidateMap();
		}

		/// <summary>
		/// Draws the view within the specified rectangle.
		/// </summary>
		/// <param name="rect">Rect.</param>
		public override void Draw(System.Drawing.RectangleF rect)
		{
			_rect = rect;
			base.Draw(rect);
            
			lock (_bufferSynchronisation)
			{
				// recreate the view.
				View2D view = this.CreateView(this.Frame);
				float zoomFactor = (float)this.Map.Projection.ToZoomFactor(this.MapZoom);

				// call the canvas renderer.
				CGContext context = UIGraphics.GetCurrentContext();

				if (context != null)
				{
					context.InterpolationQuality = CGInterpolationQuality.None;
					context.SetShouldAntialias(false);
					context.SetBlendMode(CGBlendMode.Copy);
					context.SetAlpha(1);

					long afterViewChanged = DateTime.Now.Ticks;
					CGContextRenderer renderer = new CGContextRenderer(1);
					renderer.Render(
						new CGContextWrapper(context, this.Frame),
						view,
						zoomFactor,
						new Primitive2D[] { _onScreenBuffer });
					long afterRendering = DateTime.Now.Ticks;
				}
			}
		}

		#region Controls

		/// <summary>
		/// Holds the controls.
		/// </summary>
        private List<MapControl> _controls = new List<MapControl>();

		/// <summary>
		/// Returns the mapcontrols list.
		/// </summary>
		/// <value>The controls.</value>
		public void AddControl(MapControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}

			control.AttachTo(this); // attach this view to the control.
			_controls.Add(control); // add to controls list.
			this.Add(control.BaseView); // add to this view.

			RectangleF rect = this.Frame;
			if (rect.Width > 0 && rect.Height > 0)
			{
                View2D view = this.CreateView(rect);
                this.NotifyOnBeforeSetLayout();
				this.NotifyMapChangeToControl(rect.Width, rect.Height, view, this.Map.Projection, control);
                this.NotifyOnAfterSetLayout();
			}
		}

        /// <summary>
        /// Returns a read-only collection of controls.
        /// </summary>
        public ReadOnlyCollection<MapControl> Controls
        {
            get
            {
                return _controls.AsReadOnly();
            }
        }

		/// <summary>
		/// Clears all map controls.
		/// </summary>
		public void ClearControls()
		{
			if (_controls != null)
			{
				foreach (MapControl control in _controls)
				{
					control.DetachFrom(this);
					control.BaseView.RemoveFromSuperview();
				}
				_controls.Clear();
			}
		}

		/// <summary>
		/// Removes the given map control.
		/// </summary>
		/// <param name="control"></param>
		public bool RemoveControl(MapControl control)
		{
            if (control != null)
			{
				control.DetachFrom(this); // remove the map view.
				control.BaseView.RemoveFromSuperview();
				return _controls.Remove(control);
			}
			return false;
		}

		/// <summary>
		/// Zoom to the current controls.
		/// </summary>
		public void ZoomToControls()
		{
			this.ZoomToControls(_controls);
		}

		/// <summary>
		/// Zoom to the current controls.
		/// </summary>
		public void ZoomToControls(double percentage)
		{
			this.ZoomToControls(_controls, percentage);
		}

		/// <summary>
		/// Zoom to the current controls.
		/// </summary>
		public void ZoomToControls(List<MapControl> controls)
		{
			this.ZoomToControls(controls, 15);
		}

		/// <summary>
		/// Zoom to the given makers list.
		/// </summary>
		/// <param name="control"></param>
		public void ZoomToControls(List<MapControl> controls, double percentage)
		{
			float width = this.Frame.Width;
			float height = this.Frame.Height;
			RectangleF rect = this.Frame;
			if (width > 0)
			{
				PointF2D[] points = new PointF2D[controls.Count];
				for (int idx = 0; idx < controls.Count; idx++)
				{
					points[idx] = new PointF2D(this.Map.Projection.ToPixel(controls[idx].Location));
				}
				View2D view = this.CreateView(rect);
				View2D fittedView = view.Fit(points, percentage);

				float zoom = (float)this.Map.Projection.ToZoomLevel(fittedView.CalculateZoom(
					             width, height));
				GeoCoordinate center = this.Map.Projection.ToGeoCoordinates(
					                       fittedView.Center[0], fittedView.Center[1]);
				
				this.MapCenter = center;
				this.MapZoom = zoom;

				this.NotifyMovementByInvoke();
			}
        }

        /// <summary>
        /// Adds the view.
        /// </summary>
        /// <param name="view">View.</param>
        void IMapControlHost.AddView(UIView view)
        {
            this.Add(view);
        }

        /// <summary>
        /// Removes the view.
        /// </summary>
        /// <param name="view">View.</param>
        void IMapControlHost.RemoveView(UIView view)
        {
            view.RemoveFromSuperview();
        }

        #region Markers

        /// <summary>
        /// Holds the markers.
        /// </summary>
        private List<MapMarker> _markers = new List<MapMarker>();

        /// <summary>
        /// Returns the mapmarkers list.
        /// </summary>
        /// <value>The markers.</value>
        public void AddMarker(MapMarker marker)
        {
            if (marker == null)
            {
                throw new ArgumentNullException("marker");
            }

            marker.AttachTo(this); // attach this view to the marker.
            _markers.Add(marker); // add to markers list.
            this.Add(marker.View); // add to this view.

            RectangleF rect = this.Frame;
            if (rect.Width > 0 && rect.Height > 0)
            {
                View2D view = this.CreateView(rect);
                this.NotifyOnBeforeSetLayout();
                this.NotifyMapChangeToControl(rect.Width, rect.Height, view, this.Map.Projection, marker);
                this.NotifyOnAfterSetLayout();
            }
        }

        /// <summary>
        /// Returns a read-only collection of markers.
        /// </summary>
        public ReadOnlyCollection<MapMarker> Markers
        {
            get
            {
                return _markers.AsReadOnly();
            }
        }

        /// <summary>
        /// Adds the marker.
        /// </summary>
        /// <returns>The marker.</returns>
        /// <param name="location">Location.</param>
        public MapMarker AddMarker(GeoCoordinate location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            ;

            MapMarker marker = new MapMarker(location);
            this.AddMarker(marker);
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
                    marker.View.RemoveFromSuperview();
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
            if (marker != null)
            {
                marker.DetachFrom(this); // remove the map view.
                marker.View.RemoveFromSuperview();
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
                View2D fittedView = view.Fit(points, percentage);

                float zoom = (float)this.Map.Projection.ToZoomLevel(fittedView.CalculateZoom(
                    width, height));
                GeoCoordinate center = this.Map.Projection.ToGeoCoordinates(
                    fittedView.Center[0], fittedView.Center[1]);

                this.MapCenter = center;
                this.MapZoom = zoom;

                this.NotifyMovementByInvoke();
            }
        }

        #endregion

		/// <summary>
		/// Notifies the map change to markers.
		/// </summary>
		/// <param name="pixelsWidth">Pixels width.</param>
		/// <param name="pixelsHeight">Pixels height.</param>
		/// <param name="view">View.</param>
		/// <param name="projection">Projection.</param>
		internal void NotifyMapChangeToControls(double pixelsWidth, double pixelsHeight, View2D view, 
		                                       IProjection projection)
		{
            this.NotifyOnBeforeSetLayout();
			foreach (var marker in _markers)
			{
				this.NotifyMapChangeToControl(pixelsWidth, pixelsHeight, view, projection, marker);
			}
            foreach (var control in _controls)
            {
                this.NotifyMapChangeToControl(pixelsWidth, pixelsHeight, view, projection, control);
            }
            this.NotifyOnAfterSetLayout();
		}

        /// <summary>
        /// Calls OnBeforeLayout on all controls/markers.
        /// </summary>
        internal void NotifyOnBeforeSetLayout()
        {
            lock (_markers)
            {
                if (_markers != null)
                {
                    foreach (var marker in _markers)
                    {
                        marker.OnBeforeSetLayout();
                    }
                }
            }
            lock (_controls)
            {
                if (_controls != null)
                {
                    foreach (var control in _controls)
                    {
                        control.OnBeforeSetLayout();
                    }
                }
            }
        }

        /// <summary>
        /// Calls OnAfterLayout on all controls/markers.
        /// </summary>
        internal void NotifyOnAfterSetLayout()
        {
            lock (_markers)
            {
                if (_markers != null)
                {
                    foreach (var marker in _markers)
                    {
                        marker.OnAfterSetLayout();
                    }
                }
            }
            lock (_controls)
            {
                if (_controls != null)
                {
                    foreach (var control in _controls)
                    {
                        control.OnAfterSetLayout();
                    }
                }
            }
        }


		/// <summary>
		/// Notifies this MapView that a map marker has changed.
		/// </summary>
		/// <param name="mapMarker"></param>
        public void NotifyControlChange(MapControl control)
		{
			RectangleF rect = this.Frame;
			// notify map layout of changes.
			if (rect.Width > 0 && rect.Height > 0)
			{
				View2D view = this.CreateView(rect);
                this.NotifyOnBeforeSetLayout();
				this.NotifyMapChangeToControl(rect.Width, rect.Height, view, this.Map.Projection, control);
                this.NotifyOnAfterSetLayout();
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
		internal void NotifyMapChangeToControl(double pixelsWidth, double pixelsHeight, View2D view, 
		                                      IProjection projection, MapControl mapMarker)
		{
			if (mapMarker != null)
			{
				mapMarker.SetLayout(pixelsWidth, pixelsHeight, view, projection);
			}
		}

        /// <summary>
        /// Notifies controls that there was a map tap.
        /// </summary>
        /// <remarks>>This is used to close popups on markers when the map is tapped.</remarks>
        internal void NotifyMapTapToControls() 
        {
            foreach (var marker in _markers)
            {
                marker.NotifyMapTap();
            }
            foreach (var control in _controls)
            {
                control.NotifyMapTap();
            }
        }

        /// <summary>
        /// Notifies this host that the control was clicked.
        /// </summary>
        /// <param name="clickedControl">Control.</param>
        public void NotifyControlClicked(MapControl clickedControl)
        { // make sure to close all other popups.
            foreach (var marker in _markers)
            {
                if (marker != clickedControl)
                {
                    marker.NotifyOtherControlClicked();
                }
            }
            foreach (var control in _controls)
            {
                if (control != clickedControl)
                {
                    control.NotifyOtherControlClicked();
                }
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
        /// Handles the changed event.
        /// </summary>
        private void MapChanged()
        {
            if (_listener != null)
            {
                _listener.Invalidate();
            }
            _previouslyRenderedView = null;
            this.NotifyMovementByInvoke();
        }

        /// <summary>
        /// Raises the map touched up event.
        /// </summary>
        private void RaiseMapTouchedUp()
        {
            if (this.MapTouchedUp != null)
            {
                this.MapTouchedUp(this, this.MapZoom, this.MapTilt, this.MapCenter);
            }
        }

		/// <summary>
		/// Raises the map touched event.
		/// </summary>
		private void RaiseMapTouched()
		{
			if (this.MapTouched != null)
			{
				this.MapTouched(this, this.MapZoom, this.MapTilt, this.MapCenter);
			}
        }

        /// <summary>
        /// Raises the map touched down event.
        /// </summary>
        private void RaiseMapTouchedDown()
        {
            if (this.MapTouchedDown != null)
            {
                this.MapTouchedDown(this, this.MapZoom, this.MapTilt, this.MapCenter);
            }
        }

        /// <summary>
        /// Raises the map touched event.
        /// </summary>
        private void RaiseMapMove()
        {
            if (this.MapMove != null)
            {
                this.MapMove(this, this.MapZoom, this.MapTilt, this.MapCenter);
            }
        }

        /// <summary>
        /// Raises the map touched event.
        /// </summary>
        private void RaiseMapInitialized()
        {
            if (this.MapInitialized != null)
            {
                this.MapInitialized(this, this.MapZoom, this.MapTilt, this.MapCenter);
            }
        }

		/// <summary>
		/// Dispose the specified disposing.
		/// </summary>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (_onScreenBuffer != null &&
                _onScreenBuffer.NativeImage != null)
			{
                _onScreenBuffer.NativeImage.Dispose();
			}

			if (_renderingThread != null)
			{
				_renderingThread.Abort();
				_renderingThread = null;
			}
		}
    }
}
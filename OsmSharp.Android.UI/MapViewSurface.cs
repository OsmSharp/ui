
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using OsmSharp.UI.Map;
using OsmSharp.Math.Geo;
using OsmSharp;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Map.Layers;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI;
using System.IO;
using System.Threading;
using OsmSharp.UI.Renderer.Scene;

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// Map view surface.
	/// </summary>
	class MapViewSurface : View, 
			GestureDetector.IOnGestureListener, ScaleGestureDetector.IOnScaleGestureListener, 
			global::Android.Views.View.IOnTouchListener
	{
		/// <summary>
		/// Holds the gesture detector.
		/// </summary>
		private GestureDetector _gestureDetector;

		/// <summary>
		/// Holds the primitives layer.
		/// </summary>
		private LayerPrimitives _makerLayer;
		
		/// <summary>
		/// Holds the scale gesture detector.
		/// </summary>
		private ScaleGestureDetector _scaleGestureDetector;

		/// <summary>
		/// Holds the maplayout.
		/// </summary>
		private MapView _mapLayout;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapViewSurface"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		public MapViewSurface (Context context, MapView mapLayout) :
			base (context)
		{
			Initialize (mapLayout);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapView"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="attrs">Attrs.</param>
		public MapViewSurface (Context context, IAttributeSet attrs, MapView mapLayout) :
			base (context, attrs)
		{
			Initialize (mapLayout);
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		void Initialize (MapView mapLayout)
		{
			_mapLayout = mapLayout;
			this.SetWillNotDraw (false);

			this.MapMinZoomLevel = 10;
			this.MapMaxZoomLevel = 20;

			_renderer = new MapRenderer<global::Android.Graphics.Canvas>(
				new CanvasRenderer2D());

			// initialize the gesture detection.
			_gestureDetector= new GestureDetector(
				this);
			_scaleGestureDetector = new ScaleGestureDetector(
				this.Context, this);
			this.SetOnTouchListener(this);

			_makerLayer = new LayerPrimitives(
				new WebMercator());
			
			// initialize all the caching stuff.
			//_previousCache = null;
			_cacheRenderer = new MapRenderer<global::Android.Graphics.Canvas>(
				new CanvasRenderer2D());
			_scene = new Scene2DSimple ();
			_scene.BackColor = SimpleColor.FromKnownColor (KnownColor.White).Value;

			System.Threading.Timer timer = new Timer(InvalidateSimple,
			                                         null, 0, 50);

//			Button mButton = new Button(this.Context);   
//			mButton.Text = "Test";
//			this.AddView (mButton);
		}

		/// <summary>
		/// Holds the cached scene.
		/// </summary>
		private Scene2D _scene;

		/// <summary>
		/// Holds the bitmap to draw on.
		/// </summary>
		private global::Android.Graphics.Bitmap _canvasBitmap;

		/// <summary>
		/// Holds the cache bitmap.
		/// </summary>
		private global::Android.Graphics.Bitmap _cache;

		/// <summary>
		/// Holds the cache renderer.
		/// </summary>
		private MapRenderer<global::Android.Graphics.Canvas> _cacheRenderer;

		/// <summary>
		/// Holds the previous cache id.
		/// </summary>
		//private uint? _previousCache;

		/// <summary>
		/// Does the rendering.
		/// </summary>
		private bool _render;

		/// <summary>
		/// Notifies change.
		/// </summary>
		internal void Change()
		{
			if (_cacheRenderer.IsRunning) {
				_cacheRenderer.Cancel ();
			}

//			// notify map layout of changes.
//			if (this.Width > 0 && this.Height > 0) {
//				View2D view = _cacheRenderer.Create (this.Width, this.Height,
//				                                    this.Map, (float)this.Map.Projection.ToZoomFactor (this.MapZoomLevel), this.MapCenter);
//				_mapLayout.NotifyMapChange (this.Width, this.Height, view, this.Map.Projection);
//			}

			_render = true;
		}

		/// <summary>
		/// Invalidates while rendering.
		/// </summary>
		void InvalidateSimple(object state)
		{
			if (_cacheRenderer.IsRunning) {
				this.PostInvalidate ();
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
		/// Renders the current complete scene.
		/// </summary>
		void Render()
		{	
			if (_cacheRenderer.IsRunning) {
				_cacheRenderer.CancelAndWait ();
			}

			// make sure only on thread at the same time is using the renderer.
			lock (_cacheRenderer) {
				double extra = 1.25;

				// build the layers list.
				var layers = new List<ILayer> ();
				for (int layerIdx = 0; layerIdx < this.Map.LayerCount; layerIdx++) {
					// get the layer.
					layers.Add (this.Map[layerIdx]);
				}

				// add the internal layers.
				layers.Add (_makerLayer);

				// create a new cache if size has changed.
				if (_canvasBitmap == null || 
				    _canvasBitmap.Width != (int)(this.Width * extra) || 
				    _canvasBitmap.Height != (int)(this.Height * extra)) {
					// create a bitmap and render there.
					_canvasBitmap = global::Android.Graphics.Bitmap.CreateBitmap ((int)(this.Width * extra), 
					                                                              (int)(this.Height * extra),
					                                                             global::Android.Graphics.Bitmap.Config.Argb8888);
				} else {
					// clear the cache???
				}

				// create and reset the canvas.
				global::Android.Graphics.Canvas canvas = new global::Android.Graphics.Canvas (_canvasBitmap);
				canvas.DrawColor (new global::Android.Graphics.Color(
					SimpleColor.FromKnownColor(KnownColor.Transparent).Value));

				// create the view.
				View2D view = _cacheRenderer.Create (canvas.Width, canvas.Height,
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

				// add the current canvas to the scene.
				uint canvasId = _scene.AddImage (-1, float.MinValue, float.MaxValue, 
				                                view.Left, view.Top, view.Right, view.Bottom, new byte[0], _canvasBitmap);

				// does the rendering.
				bool complete = _cacheRenderer.Render (canvas, this.Map.Projection, 
				                      layers, (float)this.Map.Projection.ToZoomFactor (this.MapZoomLevel), this.MapCenter);

				long afterRendering = DateTime.Now.Ticks;
				OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
				                                "Rendering took: {0}ms @ zoom level {1}",
				                                (new TimeSpan(afterRendering - afterViewChanged).TotalMilliseconds), this.MapZoomLevel);
				if(complete)
				{ // there was no cancellation, the rendering completely finished.
					// add the result to the scene cache.
					lock (_scene) {
//						if (_previousCache.HasValue) {
//							_scene.Remove (_previousCache.Value);
//						}
//						_scene.Remove (canvasId);
//
						// add the newly rendered image again.
						_scene.Clear ();
						//_previousCache = 
						_scene.AddImage (0, float.MinValue, float.MaxValue, 
						                                  view.Left, view.Top, view.Right, view.Bottom, new byte[0], _canvasBitmap);

						// switch cache and canvas to prevent re-allocation of bitmaps.
						global::Android.Graphics.Bitmap newCanvas = _cache;
						_cache = _canvasBitmap;
						_canvasBitmap = newCanvas;
					}
				}

				this.PostInvalidate ();
				
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
		/// Gets or sets the center.
		/// </summary>
		/// <value>The center.</value>
		public GeoCoordinate MapCenter {
			get;
			set;
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
			get{ return _map; }
			set {
				_map = value; 
				_map.MapChanged += delegate() {
					_render = true;
				};
			}
		}

		/// <summary>
		/// Holds the map zoom level.
		/// </summary>
		private float _mapZoomLevel;

		/// <summary>
		/// Gets or sets the zoom factor.
		/// </summary>
		/// <value>The zoom factor.</value>
		public float MapZoomLevel {
			get { return _mapZoomLevel; }
			set { 
				if (value > this.MapMaxZoomLevel) {
					_mapZoomLevel = this.MapMaxZoomLevel;
				} else if (value < this.MapMinZoomLevel) {
					_mapZoomLevel = this.MapMinZoomLevel;
				} else {
					_mapZoomLevel = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the map max zoom level.
		/// </summary>
		/// <value>The map max zoom level.</value>
		public float MapMaxZoomLevel {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the map minimum zoom level.
		/// </summary>
		/// <value>The map minimum zoom level.</value>
		public float MapMinZoomLevel {
			get;
			set;
		}

		/// <summary>
		/// Holds the renderer.
		/// </summary>
		private MapRenderer<global::Android.Graphics.Canvas> _renderer;

		/// <summary>
		/// Raises the draw event.
		/// </summary>
		/// <param name="canvas">Canvas.</param>
		protected override void OnDraw (global::Android.Graphics.Canvas canvas)
		{
			base.OnDraw (canvas);
			
//			long before = DateTime.Now.Ticks;

			// render only the cached scene.
			lock(_scene)
			{
//				OsmSharp.IO.Output.OutputStreamHost.WriteLine ("OnDraw");
				canvas.DrawColor (new global::Android.Graphics.Color(_scene.BackColor));
				_renderer.SceneRenderer.Render (
					canvas, 
					_scene,
					_renderer.Create (canvas.Width, canvas.Height,
				                  this.Map, (float)this.Map.Projection.ToZoomFactor (this.MapZoomLevel), this.MapCenter));
			}

//			long after = DateTime.Now.Ticks;
//			OsmSharp.IO.Output.OutputStreamHost.WriteLine(string.Format("Rendering in {0}ms", 
//			                                   new TimeSpan (after - before).TotalMilliseconds));
		}
		
		/// <summary>
		/// Creates a view.
		/// </summary>
		/// <param name="map"></param>
		/// <param name="zoomFactor"></param>
		/// <param name="center"></param>
		/// <returns></returns>
		public View2D CreateView()
		{
			// calculate the center/zoom in scene coordinates.
			double[] sceneCenter = this.Map.Projection.ToPixel(this.MapCenter.Latitude, this.MapCenter.Longitude);
			float sceneZoomFactor = (float)this.Map.Projection.ToZoomFactor(this.MapZoomLevel);
			
			// create the view for this control.
			return View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
			                         this.Width, this.Height, sceneZoomFactor, 
			                         this.Map.Projection.DirectionX, this.Map.Projection.DirectionY);
		}

		/// <summary>
		/// Raises the layout event.
		/// </summary>
		/// <param name="changed">If set to <c>true</c> changed.</param>
		/// <param name="left">Left.</param>
		/// <param name="top">Top.</param>
		/// <param name="right">Right.</param>
		/// <param name="bottom">Bottom.</param>
		protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
		{
			// notify there was movement.
			this.NotifyMovement();

			if (_canvasBitmap == null) {
				this.Change (); // force a rendering on the first layout-event.
			}
		}

		/// <summary>
		/// Notifies that there was movement.
		/// </summary>
		private void NotifyMovement()
		{		
			// invalidate the current view.
			this.Invalidate ();

			// notify map layout of changes.
			if (this.Width > 0 && this.Height > 0) {
				View2D view = _cacheRenderer.Create (this.Width, this.Height,
				                                     this.Map, (float)this.Map.Projection.ToZoomFactor (this.MapZoomLevel), this.MapCenter);
				_mapLayout.NotifyMapChange (this.Width, this.Height, view, this.Map.Projection);
			}
		}

		#region IOnScaleGestureListener implementation

		
		/// <summary>
		/// Holds the scaled flag.
		/// </summary>
		private bool _scaled = false;
		
		/// <summary>
		/// Raises the scale event.
		/// </summary>
		/// <param name="detector">Detector.</param>
		public bool OnScale (ScaleGestureDetector detector)
		{
			double zoomFactor = this.Map.Projection.ToZoomFactor(this.MapZoomLevel);
			zoomFactor = zoomFactor * detector.ScaleFactor;
			this.MapZoomLevel = (float)this.Map.Projection.ToZoomLevel(zoomFactor);

			_scaled = true;
			this.NotifyMovement();
			return true;
		}
		
		/// <summary>
		/// Raises the scale begin event.
		/// </summary>
		/// <param name="detector">Detector.</param>
		public bool OnScaleBegin (ScaleGestureDetector detector)
		{
//			_highQuality = false;
			return true;
		}
		
		/// <summary>
		/// Raises the scale end event.
		/// </summary>
		/// <param name="detector">Detector.</param>
		public void OnScaleEnd (ScaleGestureDetector detector)
		{
//			System.Threading.Thread thread = new System.Threading.Thread(
//				new System.Threading.ThreadStart(NotifyMovement));
//			thread.Start();

//			_highQuality = true;
//			this.NotifyMovement();

//			OsmSharp.IO.Output.OutputStreamHost.WriteLine("OnScaleEnd");
//			this.Change ();
		}
		
		#endregion

		#region IOnGestureListener implementation
		
		/// <summary>
		/// Raises the down event.
		/// </summary>
		/// <param name="e">E.</param>
		public bool OnDown (MotionEvent e)
		{
			return true;
		}
		
		/// <summary>
		/// Raises the fling event.
		/// </summary>
		/// <param name="e1">E1.</param>
		/// <param name="e2">E2.</param>
		/// <param name="velocityX">Velocity x.</param>
		/// <param name="velocityY">Velocity y.</param>
		public bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			return true;
		}
		
		/// <summary>
		/// Raises the long press event.
		/// </summary>
		/// <param name="e">E.</param>
		public void OnLongPress (MotionEvent e)
		{

		}
		
		/// <summary>
		/// Raises the scroll event.
		/// </summary>
		/// <param name="e1">E1.</param>
		/// <param name="e2">E2.</param>
		/// <param name="distanceX">Distance x.</param>
		/// <param name="distanceY">Distance y.</param>
		public bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			// recreate the view.
			View2D view = this.CreateView();
			
			// calculate the new center in pixels.
			double centerXPixels = this.Width / 2.0f + distanceX;
			double centerYPixles = this.Height / 2.0f + distanceY;
			
			// calculate the new center from the view.
			double[] sceneCenter = view.FromViewPort(this.Width, this.Height, 
			                              centerXPixels, centerYPixles);

			// convert to the projected center.
			this.MapCenter = this.Map.Projection.ToGeoCoordinates(sceneCenter[0], sceneCenter[1]);

//			_highQuality = false;

			this.NotifyMovement();
			return true;
		}
		
		/// <summary>
		/// Raises the show press event.
		/// </summary>
		/// <param name="e">E.</param>
		public void OnShowPress (MotionEvent e)
		{

		}
		
		/// <summary>
		/// Raises the single tap up event.
		/// </summary>
		/// <param name="e">E.</param>
		public bool OnSingleTapUp (MotionEvent e)
		{
			return true;
		}
		
		#endregion

		/// <summary>
		/// Raises the touch event event.
		/// </summary>
		/// <param name="e">E.</param>
		public override bool OnTouchEvent (MotionEvent e)
		{
			return true;
		}
		
		#region IOnTouchListener implementation
		
		/// <summary>
		/// Raises the touch event.
		/// </summary>
		/// <param name="v">V.</param>
		/// <param name="e">E.</param>
		public bool OnTouch (global::Android.Views.View v, MotionEvent e)
		{
			_scaleGestureDetector.OnTouchEvent(e);
			if(!_scaled)
			{
				_gestureDetector.OnTouchEvent(e);

				if(e.Action == MotionEventActions.Up)
				{

					// calculate event time.
					long time = e.EventTime - e.DownTime;
					if (time > 120) {
						this.NotifyMovement ();

						OsmSharp.Logging.Log.TraceEvent ("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information, 
						                                 string.Format ("OnTouch:{0}ms", time));
						this.Change ();
					} else { // raise the map tap event here.
						if (this.MapTapEvent != null) {
							// recreate the view.
							View2D view = this.CreateView();

							// calculate the new center in pixels.
							double x = e.GetX ();
							double y = e.GetY ();

							// calculate the new center from the view.
							double[] sceneCenter = view.FromViewPort(this.Width, this.Height, 
							                                         x, y);

							// convert to the projected center.
							this.MapTapEvent(this.Map.Projection.ToGeoCoordinates(sceneCenter[0], sceneCenter[1]));
						}
					}
				}
			}
			_scaled = false;
			return true;
		}
		
		#endregion

		/// <summary>
		/// Occurs when the map was tapped at a certain location.
		/// </summary>
		public event MapView.MapTapEventDelegate MapTapEvent;

		/// <summary>
		/// Occurs when the map was touched for a longer time at a certain location.
		/// </summary>
		public event MapView.MapTapEventDelegate MapHoldEvent;
	}
}


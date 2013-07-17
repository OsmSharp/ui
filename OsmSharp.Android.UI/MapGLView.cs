using System;
using Android.Opengl;
using Android.Content;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Map;
using OsmSharp.UI;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Map.Layers;
using System.Collections.Generic;
using Android.Views;

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// Map Open GL ES view.
	/// </summary>
	public class MapGLView : GLSurfaceView, 
		GestureDetector.IOnGestureListener, ScaleGestureDetector.IOnScaleGestureListener, 
		global::Android.Views.View.IOnTouchListener
	{
		/// <summary>
		/// Holds the Open GL 2D Target.
		/// </summary>
		private OpenGLTarget2D _target;

		/// <summary>
		/// Holds the cached scene.
		/// </summary>
		private Scene2D _scene;

		/// <summary>
		/// Holds the map renderer.
		/// </summary>
		private MapRenderer<OpenGLTarget2D> _renderer;

		/// <summary>
		/// Holds the gesture detector.
		/// </summary>
		private GestureDetector _gestureDetector;

		/// <summary>
		/// Holds the scale gesture detector.
		/// </summary>
		private ScaleGestureDetector _scaleGestureDetector;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapGLView"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		public MapGLView (Context context) : base (context)
		{
			// initialize this view.
			this.Initialize ();
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		void Initialize ()
		{
			// create the Open GL 2D target.
			_target = new OpenGLTarget2D ();
			this.SetRenderer (_target);			

			// initialize the gesture detection.
			_gestureDetector= new GestureDetector(
				this);
			_scaleGestureDetector = new ScaleGestureDetector(
				this.Context, this);
			this.SetOnTouchListener(this);

			// create the renderer.
			_renderer = new MapRenderer<OpenGLTarget2D>(
				new OpenGLRenderer2D());

			// initialize the scene.
			_scene = new Scene2D ();
			_scene.BackColor = SimpleColor.FromKnownColor (KnownColor.White).Value;
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
			double[] sceneCenter = this.Map.Projection.ToPixel(this.Center.Latitude, this.Center.Longitude);
			float sceneZoomFactor = (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel);

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
		///( <param name="top">Top.</param>
		/// <param name="right">Right.</param>
		/// <param name="bottom">Bottom.</param>
		protected override void OnLayout (bool changed, int left, int top, int right, int bottom)
		{		
			_target.Width = this.Width;
			_target.Height = this.Height;

			// create the view.
			View2D view = _renderer.Create (this.Width, this.Height,
			                                     this.Map, (float)this.Map.Projection.ToZoomFactor (this.ZoomLevel), this.Center);

			// notify the map that the view has changed.
			this.Map.ViewChanged ((float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center, 
			                      view);
			
			// build the layers list.
			var layers = new List<ILayer> ();
			for (int layerIdx = 0; layerIdx < this.Map.LayerCount; layerIdx++) {
				// get the layer.
				layers.Add (this.Map[layerIdx]);
			}

			_renderer.Render (_target,
			                  this.Map.Projection,
			                  layers,
			                  (float)this.Map.Projection.ToZoomFactor (this.ZoomLevel), 
			                  this.Center);
		}

		/// <summary>
		/// Notifies movement.
		/// </summary>
		private void NotifyMovement()
		{
			// create the view.
			View2D view = _renderer.Create (this.Width, this.Height,
			                                this.Map, (float)this.Map.Projection.ToZoomFactor (this.ZoomLevel), this.Center);

			_target.SetOrtho((float)view.Left, (float)view.Right, 
			                 (float)view.Bottom, (float)view.Top);
		}

		/// <summary>
		/// Notifies change.
		/// </summary>
		private void Change()
		{

		}

		/// <summary>
		/// Gets or sets the center.
		/// </summary>
		/// <value>The center.</value>
		public GeoCoordinate Center {
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
		/// Gets or sets the zoom factor.
		/// </summary>
		/// <value>The zoom factor.</value>
		public float ZoomLevel {
			get;
			set;
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
			double zoomFactor = this.Map.Projection.ToZoomFactor(this.ZoomLevel);
			zoomFactor = zoomFactor * detector.ScaleFactor;
			this.ZoomLevel = (float)this.Map.Projection.ToZoomLevel(zoomFactor);

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
			this.Center = this.Map.Projection.ToGeoCoordinates(sceneCenter[0], sceneCenter[1]);

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
					//					System.Threading.Thread thread = new System.Threading.Thread(
					//						new System.Threading.ThreadStart(NotifyMovement));
					//					thread.Start();

					//					_highQuality = true;
					this.NotifyMovement();

                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapGLView", System.Diagnostics.TraceEventType.Information, "OnTouch");
					this.Change ();
				}
			}
			_scaled = false;
			return true;
		}

		#endregion
	}
}
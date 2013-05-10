
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

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// Map view.
	/// </summary>
	public class MapView : global::Android.Views.View, GestureDetector.IOnGestureListener, ScaleGestureDetector.IOnScaleGestureListener, global::Android.Views.View.IOnTouchListener
	{
		/// <summary>
		/// Holds the gesture detector.
		/// </summary>
		private GestureDetector _gestureDetector;
		
		/// <summary>
		/// Holds the scale gesture detector.
		/// </summary>
		private ScaleGestureDetector _scaleGestureDetector;
		public MapView (Context context) :
			base (context)
		{
			Initialize ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapView"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="attrs">Attrs.</param>
		public MapView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapView"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="attrs">Attrs.</param>
		/// <param name="defStyle">Def style.</param>
		public MapView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		void Initialize ()
		{
			_renderer = new MapRenderer<global::Android.Graphics.Canvas>(
				new CanvasRenderer2D());
			// initialize the gesture detection.
			_gestureDetector= new GestureDetector(
				this);
			_scaleGestureDetector = new ScaleGestureDetector(
				this.Context, this);
			this.SetOnTouchListener(this);
		}

		/// <summary>
		/// High quality rendering flag.
		/// </summary>
		private bool _highQuality = true;

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
		public float ZoomFactor {
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
			
			// initialize the renderers.
			global::Android.Graphics.Paint paint = new global::Android.Graphics.Paint();
			if(_highQuality)
			{			
				// render the map.
				_renderer.Render(canvas, this.Map, this.ZoomFactor, this.Center);
			}
			else
			{
				// render the cached parts only.
				_renderer.RenderCache(canvas, this.Map, this.ZoomFactor, this.Center);
			}

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
			float sceneZoomFactor = this.ZoomFactor; // TODO: find out the conversion rate and see if this is related to the projection?
			
			// create the view for this control.
			return View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
			                         this.Width, this.Height, sceneZoomFactor);
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
			// notify the map.
			//this.Map.ViewChanged(this.ZoomFactor, this.Center, this.CreateView());
			// notify there was movement.
			//this.NotifyMovement();

			System.Threading.Thread thread = new System.Threading.Thread(
				new System.Threading.ThreadStart(NotifyMovement));
			thread.Start();
		}

		/// <summary>
		/// Notifies that there was movement.
		/// </summary>
		private void NotifyMovement()
		{
			lock(this.Map)
			{
				// notify the map.
				this.Map.ViewChanged(this.ZoomFactor, this.Center, this.CreateView());

//				this.Invalidate();
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
			this.ZoomFactor = (float)System.Math.Log((System.Math.Pow(2, this.ZoomFactor) * detector.ScaleFactor), 2.0);

			// notify the map.
			this.Map.ViewChanged(this.ZoomFactor, this.Center, this.CreateView());

			_scaled = true;
			this.Invalidate();
			return true;
		}
		
		/// <summary>
		/// Raises the scale begin event.
		/// </summary>
		/// <param name="detector">Detector.</param>
		public bool OnScaleBegin (ScaleGestureDetector detector)
		{
			_highQuality = false;
			return true;
		}
		
		/// <summary>
		/// Raises the scale end event.
		/// </summary>
		/// <param name="detector">Detector.</param>
		public void OnScaleEnd (ScaleGestureDetector detector)
		{
			_highQuality = true;
			this.Invalidate();
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
			float centerXPixels = this.Width / 2.0f + distanceX;
			float centerYPixles = this.Height / 2.0f + distanceY;
			
			// calculate the new center from the view.
			float[] sceneCenter = view.ToViewPort(this.Width, this.Height, 
			                              centerXPixels, centerYPixles);

			// convert to the projected center.
			this.Center = this.Map.Projection.ToGeoCoordinates(sceneCenter[0], sceneCenter[1]);

			// notify the map.
			this.Map.ViewChanged(this.ZoomFactor, this.Center, this.CreateView());

			_highQuality = false;
			this.Invalidate();
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
					_highQuality = true;
					this.Invalidate();
				}
			}
			_scaled = false;
			return true;
		}
		
		#endregion
	}
}


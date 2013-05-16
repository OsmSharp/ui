
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

using OsmSharp.Math.Geo;
using OsmSharp.UI.Renderer;

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// Map view.
	/// </summary>
	public class CanvasRenderer2DView : View, GestureDetector.IOnGestureListener, ScaleGestureDetector.IOnScaleGestureListener, global::Android.Views.View.IOnTouchListener
	{
		/// <summary>
		/// Holds the gesture detector.
		/// </summary>
		private GestureDetector _gestureDetector;

		/// <summary>
		/// Holds the scale gesture detector.
		/// </summary>
		private ScaleGestureDetector _scaleGestureDetector;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.CanvasRenderer2DView"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		public CanvasRenderer2DView (Context context, Scene2D scene) :
			base (context)
		{
			Initialize ();

			this.Scene = scene;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.CanvasRenderer2DView"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="attrs">Attrs.</param>
		public CanvasRenderer2DView (Context context, IAttributeSet attrs, Scene2D scene) :
			base (context, attrs)
		{
			Initialize ();

			this.Scene = scene;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.CanvasRenderer2DView"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="attrs">Attrs.</param>
		/// <param name="defStyle">Def style.</param>
		public CanvasRenderer2DView (Context context, IAttributeSet attrs, int defStyle, Scene2D scene) :
			base (context, attrs, defStyle)
		{
			Initialize ();

			this.Scene = scene;
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		void Initialize ()
		{
			// initialize the gesture detection.
			_gestureDetector= new GestureDetector(
				this);
			_scaleGestureDetector = new ScaleGestureDetector(
				this.Context, this);
			this.SetOnTouchListener(this);
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

//			float factorX = this.View.Width / this.Width;
//			float factorY = this.View.Height / this.Height;
//			this.View = View2D.CreateFromCenterAndSize(this.View.Width / detector.ScaleFactor, this.View.Height / detector.ScaleFactor, 
//			                                           this.View.CenterX,
//			                                           this.View.CenterY);
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
			return true;
		}

		/// <summary>
		/// Raises the scale end event.
		/// </summary>
		/// <param name="detector">Detector.</param>
		public void OnScaleEnd (ScaleGestureDetector detector)
		{

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
			View2D view = View2D.CreateFrom(this.Center[0], this.Center[1],
			                                this.Width, this.Height,
			                                this.ZoomFactor, true, true);

			// calculate the new center in pixels.
			double centerXPixels = this.Width / 2.0f + distanceX;
			double centerYPixles = this.Height / 2.0f + distanceY;

			// calculate the new center from the view.
			double[] center = view.ToViewPort(this.Width, this.Height, 
			                                  (float)centerXPixels, (float)centerYPixles);
			this.Center[0] = (float)center[0];
			this.Center[1] = (float)center[1];

//			float factorX = this.View.Width / this.Width;
//			float factorY = this.View.Height / this.Height;
//			this.View = View2D.CreateFromCenterAndSize(this.View.Width, this.View.Height, 
//			                                           this.View.CenterX + (-distanceX) * factorX,
//			                                           this.View.CenterY + (-distanceY) * factorY);
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
		/// Gets or sets the scene.
		/// </summary>
		/// <value>The scene.</value>
		public Scene2D Scene {
			get;
			private set;
		}

		/// <summary>
		/// The center coordinates.
		/// </summary>
		/// <value>The center.</value>
		public float[] Center {
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
		/// Raises the draw event.
		/// </summary>
		/// <param name="canvas">Canvas.</param>
		protected override void OnDraw (global::Android.Graphics.Canvas canvas)
		{
			base.OnDraw (canvas);

			// recreate the view.
			View2D view = View2D.CreateFrom(this.Center[0], this.Center[1],
			                                canvas.Width, canvas.Height,
			                                this.ZoomFactor, true, true);

			// call the canvas renderer.
			CanvasRenderer2D renderer = new CanvasRenderer2D();
			renderer.Render(canvas, this.Scene, view);
		}

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
			}
			_scaled = false;
			return true;
		}

		#endregion
	}
}
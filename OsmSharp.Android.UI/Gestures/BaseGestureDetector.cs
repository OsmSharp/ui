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

namespace Android.Views
{
	/// <summary>
	/// Abstract baseclass for gesture detectors.
	/// </summary>
	public abstract class BaseGestureDetector
	{
		/// <summary>
		/// Holds the context.
		/// </summary>
		protected Context _context;
		protected bool _gestureInProgress;

		protected MotionEvent _previousEvent;
		protected MotionEvent _currentEvent;

		protected float _currentPressure;
		protected float _previousPressure;
		protected long _timeDelta;

		/**
		* This value is the threshold ratio between the previous combined pressure
		* and the current combined pressure. When pressure decreases rapidly
		* between events the position values can often be imprecise, as it usually
		* indicates that the user is in the process of lifting a pointer off of the
		* device. This value was tuned experimentally.
		*/
    	protected const float PressureThreshold = 0.67f;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.BaseGestureDetector"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		public BaseGestureDetector(Context context) {
			_context = context;
		}

		/// <summary>
		/// All gesture detectors need to be called through this method to be able to
		/// detect gestures. This method delegates work to handler methods
		/// (HandleStartProgressEvent, HandleInProgressEvent) implemented in extended
		/// classes.
		/// </summary>
		/// <param name="e">The event.</param>
		public bool OnTouchEvent(MotionEvent e){
			MotionEventActions actionCode = e.Action & MotionEventActions.Mask;
			if (!_gestureInProgress) {
				this.HandleStartProgressEvent (actionCode, e);
			} else {
				this.HandleInProgressEvent (actionCode, e);
			}
			return true;
		}

		/// <summary>
		/// Called when the current event occurred when NO gesture is in progress
		/// yet. The handling in this implementation may set the gesture in progress
		/// or out of progress.
		/// </summary>
		/// <param name="actionCode">Action code.</param>
		/// <param name="e">The event.</param>
		protected abstract void HandleStartProgressEvent (MotionEventActions actionCode, MotionEvent e);

		/// <summary>
		/// Called when the current event occurred when a gesture IS in progress. The
		/// handling in this implementation may set the gesture out of progress.
		/// </summary>
		/// <param name="actionCode">Action code.</param>
		/// <param name="e">The event.</param></param>
		protected abstract void HandleInProgressEvent(MotionEventActions actionCode, MotionEvent e);
		             
		/// <summary>
		/// Updates the current state with the given event.
		/// </summary>
		/// <param name="curr">The current event.</param>
		protected virtual void UpdateStateByEvent(MotionEvent curr) {
			MotionEvent prev = _previousEvent;

			// Reset _currentEvent.
			if (_currentEvent != null) {
				_currentEvent.Recycle ();
				_currentEvent = null;
			}
			_currentEvent = MotionEvent.Obtain (curr);

			// Delta time.
			_timeDelta = 0;
			if (prev != null) {
				_timeDelta = curr.EventTime - prev.EventTime;

				// Pressure.
				_previousPressure = prev.GetPressure (prev.ActionIndex);
			}
			_currentPressure = curr.GetPressure (curr.ActionIndex);
		}

		/// <summary>
		/// Resets the state.
		/// </summary>
		protected virtual void ResetState() {
			if (_previousEvent != null) {
				_previousEvent.Recycle ();
				_previousEvent = null;
			}
			if (_currentEvent != null) {
				_currentEvent.Recycle ();
				_currentEvent = null;
			}
			_gestureInProgress = false;
		}

		/// <summary>
		/// Returns true if a gesture is in progress.
		/// </summary>
		/// <returns><c>true</c> if this instance is in progress; otherwise, <c>false</c>.</returns>
		public bool IsInProgress() {
			return _gestureInProgress;
		}

		/// <summary>
		/// Returns the time difference in milliseconds between the previous accepted
		/// GestureDetector event and the curent GestureDetector event.
		/// </summary>
		/// <returns>The time difference between the last move event in milliseconds.</returns>
		public long GetTimeTable() {
			return _timeDelta;
		}

		/// <summary>
		/// Returns the event time of the current GestureDetecture event being processed.
		/// </summary>
		/// <returns>Current GestureDetector event time in milliseconds.</returns>
		public long GetEventTime() {
			return _currentEvent.EventTime;
		}
	}
}


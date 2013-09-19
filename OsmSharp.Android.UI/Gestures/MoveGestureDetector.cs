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
using Android.Content;
using Android.Graphics;
using Android.Views;

namespace Android.Views
{
	/// <summary>
	/// Convenience gesture detector to keep it easy moving an object around with one ore more fingers without losing the clean usage of the gesture detector pattern.
	/// </summary>
	public class MoveGestureDetector : BaseGestureDetector
	{
		/// <summary>
		/// Listener which must be implemented which is used by MoveGestureDetector
		/// to perform callbacks to any implementing class which is registered to a
		/// MoveGestureDetector via the constructor.
		/// </summary>
		public interface IOnMoveGestureListener {
			bool OnMove(MoveGestureDetector detector);
			bool OnMoveBegin(MoveGestureDetector detector);
			void OnMoveEnd(MoveGestureDetector detector);
		}

		/// <summary>
		/// Helper class which may be extended and where the methods may be
		/// implemented. This way it is not necessary to implement all methods
		/// of OnMoveGestureListener.
		/// </summary>
		public class SimpleOnMoveGestureListener : IOnMoveGestureListener {
			public virtual bool OnMove (MoveGestureDetector detector) {
				return false;
			}

			public virtual bool OnMoveBegin (MoveGestureDetector detector) {		return true;
			}

			public virtual void OnMoveEnd (MoveGestureDetector detector) {
				// do nothing, overridden implementation may be used.
			}
		}

		private static PointF FocusDeltaZero = new PointF();

		private readonly IOnMoveGestureListener _listener;

		private PointF _currFocusInternal;
		private PointF _prevFocusInternal;
		private PointF _focusExternal = new PointF();
		private PointF _focusDeltaExternal = new PointF();

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MoveGestureDetector"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="listener">Listener.</param>
		public MoveGestureDetector(Context context, IOnMoveGestureListener listener) 
			: base(context) {
			_listener = listener;
		}

		/// <summary>
		/// Called when the current event occurred when NO gesture is in progress
		/// yet. The handling in this implementation may set the gesture in progress
		/// or out of progress.
		/// </summary>
		/// <param name="actionCode">Action code.</param>
		/// <param name="e">The event.</param>
		protected override void HandleStartProgressEvent (MotionEventActions actionCode, MotionEvent e) {
			switch (actionCode) {
			case MotionEventActions.Down:
				ResetState (); // In case we missed an UP/CANCEL event

				_previousEvent = MotionEvent.Obtain (e);
				_timeDelta = 0;

				UpdateStateByEvent (e);
				break;
			case MotionEventActions.Move:
				_gestureInProgress = _listener.OnMoveBegin(this);
				break;
			}
		}

		/// <summary>
		/// Called when the current event occurred when a gesture IS in progress. The
		/// handling in this implementation may set the gesture out of progress.
		/// </summary>
		/// <param name="actionCode">Action code.</param>
		/// <param name="e">E.</param>
		protected override void HandleInProgressEvent (MotionEventActions actionCode, MotionEvent e) {
			switch (actionCode) {
			case MotionEventActions.Up:
			case MotionEventActions.Cancel:
				_listener.OnMoveEnd (this);
				ResetState ();
				break;

			case MotionEventActions.Move:
				UpdateStateByEvent (e);

				// Only accept the event if our relative pressure is within
				// a certain limit. This can help filter shaky data as a
				// finger is lifted.
				if (_currentPressure / _previousPressure > PressureThreshold) {
					bool updatePrevious = _listener.OnMove (this);
					if (updatePrevious) {
						_previousEvent.Recycle ();
						_previousEvent = MotionEvent.Obtain (e);
					}
				}
				break;
			}
		}

		/// <summary>
		/// Updates the current state with the given event.
		/// </summary>
		/// <param name="curr">The current event.</param>
		protected override void UpdateStateByEvent (MotionEvent curr)
		{
			base.UpdateStateByEvent (curr);

			MotionEvent prev = _previousEvent;

			// focus internal
			_currFocusInternal = DetermineFocalPoint (curr);
			_prevFocusInternal = DetermineFocalPoint (prev);

			// focus external
			// - preven skipping of focus delta when a finger is added or removed.
			bool skipNextMoveEvent = prev.PointerCount != curr.PointerCount;
			_focusDeltaExternal = skipNextMoveEvent ? FocusDeltaZero : new PointF (_currFocusInternal.X - _prevFocusInternal.X,
			                                                                      _currFocusInternal.Y - _prevFocusInternal.Y);

			// - don't directly use _focusInternal (or skipping will occur). Add
			// 		unskipped delta values to _focusInternal instead.
			_focusExternal.X += _focusDeltaExternal.X;
			_focusExternal.Y += _focusDeltaExternal.Y;
        }

		/// <summary>
		/// Determine (multi)finger focal point (a.k.a. center point between all
		/// fingers).
		/// </summary>
		/// <returns>The focal point.</returns>
		/// <param name="e">E.</param>
		private PointF DetermineFocalPoint(MotionEvent e) {
			// Number of fingers on screen
			int pCount = e.PointerCount;
			float x = 0f;
			float y = 0f;

			for(int i = 0; i < pCount; i++){
				x += e.GetX (i);
				y += e.GetY (i);
			}

			return new PointF(x/pCount, y/pCount);
		}

		/// <summary>
		/// Gets the focus x.
		/// </summary>
		/// <returns>The focus x.</returns>
		public float FocusX {
			get{
				return _focusExternal.X;
			}
		}

		/// <summary>
		/// Gets the focus y.
		/// </summary>
		/// <returns>The focus y.</returns>
		public float FocusY {
			get{
				return _focusExternal.Y;
			}
		}

		/// <summary>
		/// Gets the focus delta.
		/// </summary>
		/// <returns>The focus delta.</returns>
		public PointF FocusDelta {
			get{
				return _focusDeltaExternal;
			}
		}
	}
}


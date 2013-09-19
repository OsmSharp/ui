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
using Android.Views;

namespace Android.Views
{
	/// <summary>
	/// Helps finding out the rotation angle between the line of two fingers (with the normal or previous finger positions).
	/// </summary>
	public class RotateGestureDetector : TwoFingerGestureDetector
	{		
		/// <summary>
		/// Listener which must be implemented which is used by RotateGestureDetector
		/// to perform callbacks to any implementing class which is registered to a
		/// RotateGestureDetector via the constructor.
		/// </summary>
		public interface IOnRotateGestureListener {
			bool OnRotate(RotateGestureDetector detector);
			bool OnRotateBegin(RotateGestureDetector detector);
			void OnRotateEnd(RotateGestureDetector detector);
		}

		/// <summary>
		/// Helper class which may be extended and where the methods may be
		/// implemented. This way it is not necessary to implement all methods
		/// of OnRotateGestureListener.
		/// </summary>
		public class SimpleOnRotateGestureListener : IOnRotateGestureListener {
			public virtual bool OnRotate (RotateGestureDetector detector) {
				return false;
			}

			public virtual bool OnRotateBegin (RotateGestureDetector detector) {
				return true;
			}

			public virtual void OnRotateEnd (RotateGestureDetector detector) {
				// do nothing, overridden implementation may be used.
			}
		}

		private readonly IOnRotateGestureListener _listener;
		private bool _sloppyGesture;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.RotateGestureDetector"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="listener">Listener.</param>
		public RotateGestureDetector(Context context, IOnRotateGestureListener listener)
			:base (context) {
			_listener = listener;
		}

		/// <summary>
		/// Called when the current event occurred when a gesture IS in progress. The
		/// handling in this implementation may set the gesture out of progress.
		/// </summary>
		/// <param name="actionCode">Action code.</param>
		/// <param name="e">E.</param>
		protected override void HandleInProgressEvent (MotionEventActions actionCode, MotionEvent e)
		{
			switch (actionCode) {
			case MotionEventActions.PointerDown:
				// At least the second finger is on the screen now.

				ResetState ();
				_previousEvent = MotionEvent.Obtain (e);
				_timeDelta = 0;

				UpdateStateByEvent (e);

				// See if we have a sloppy gesture.
				_sloppyGesture = this.IsSloppyGesture (e);
				if (!_sloppyGesture) {
					// No, start gesture now.
					_gestureInProgress = _listener.OnRotateBegin (this);
				}
				break;
			case MotionEventActions.Move:
				if (!_sloppyGesture) {
					break;
				}

				// See if we still have a sloppy gesture
				_sloppyGesture = this.IsSloppyGesture (e);
				if (!_sloppyGesture) {
					_gestureInProgress = _listener.OnRotateBegin (this);
				}

				break;
			case MotionEventActions.PointerUp:
				if (!_sloppyGesture) {
					break;
				}

				break;
			}
		}


		protected override void HandleStartProgressEvent (MotionEventActions actionCode, MotionEvent e)
		{
			switch (actionCode) {
			case MotionEventActions.PointerUp:
				// Gesture ended but
				UpdateStateByEvent (e);

				if (!_sloppyGesture) {
					_listener.OnRotateEnd (this);
				}

				ResetState ();
				break;
			case MotionEventActions.Cancel:
				if (!_sloppyGesture) {
					_listener.OnRotateEnd (this);
				}

				ResetState ();
				break;
			case MotionEventActions.Move:
				UpdateStateByEvent (e);

				// Only accept the event if our relative pressure is within
				// a certain limit. This can help filter shaky data as a
				// finger is lifted.
				if (_currentPressure / _previousPressure > PressureThreshold) {
					bool updatePrevious = _listener.OnRotate(this);
					if (updatePrevious) {
						if (_previousEvent != null) {
							_previousEvent.Recycle ();
						}
						_previousEvent = MotionEvent.Obtain(e);
					}
				}
				break;
			}
		}

		/// <summary>
		/// Resets the state.
		/// </summary>
		protected override void ResetState ()
		{
			base.ResetState ();
			_sloppyGesture = false;
		}

		/// <summary>
		/// Gets the rotation degrees delta.
		/// </summary>
		/// <returns>The rotation degrees delta.</returns>
		public float RotationDegreesDelta {
			get{
				double diffRadians = System.Math.Atan2 (_prevFingerDiffY, _prevFingerDiffX) - System.Math.Atan2 (_currFingerDiffY, _currFingerDiffX);
				return (float)(diffRadians * 180.0 / System.Math.PI);
			}
		}
	}
}


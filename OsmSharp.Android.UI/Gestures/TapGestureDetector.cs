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

using Android.Content;
using Android.Graphics;

namespace Android.Views
{
	/// <summary>
	/// Helps detect simple taps.
	/// </summary>
    public class TapGestureDetector : BaseGestureDetector
    {		
        /// <summary>
		/// Listener which must be implemented which is used by TapGestureDetector
		/// to perform callbacks to any implementing class which is registered to a
		/// TapGestureDetector via the constructor.
		/// </summary>
		public interface IOnTapGestureListener {
			bool OnTap(TapGestureDetector detector);
		}

		/// <summary>
		/// Helper class which may be extended and where the methods may be
		/// implemented. This way it is not necessary to implement all methods
		/// of OnTapGestureListener.
		/// </summary>
		public class SimpleOnTapGestureListener : IOnTapGestureListener {
            public virtual bool OnTap(TapGestureDetector detector)
            {
				return false;
			}
		}

        /// <summary>
        /// Holds the listener.
        /// </summary>
        private readonly IOnTapGestureListener _listener;

        /// <summary>
        /// Holds the tap period.
        /// </summary>
        private readonly long _tapPeriod;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MoveGestureDetector"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="listener">Listener.</param>
        public TapGestureDetector(Context context, IOnTapGestureListener listener) 
			: base(context) {
			_listener = listener;
            _tapPeriod = 1000;
		}

        /// <summary>
        /// Holds the first event time.
        /// </summary>
        private long _firstEventTime;

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
                _firstEventTime = e.EventTime;

				_previousEvent = MotionEvent.Obtain (e);
				_timeDelta = 0;
                _gestureInProgress = true;

                _x = e.GetX();
                _y = e.GetY();

				UpdateStateByEvent (e);
				break;
			case MotionEventActions.Move:
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
                if (_tapPeriod > (e.EventTime - _firstEventTime))
                {
                    _listener.OnTap(this);
                    ResetState();
                }
				break;
            case MotionEventActions.Move:
                if (_tapPeriod < (e.EventTime - _firstEventTime))
                {
                    ResetState();
                }
				break;
			}
		}

        /// <summary>
        /// Holds the x-coordinate of the touch.
        /// </summary>
        private float _x;

        /// <summary>
        /// Holds the y-coordinate of the touch.
        /// </summary>
        private float _y;

        /// <summary>
        /// Gets the x.
        /// </summary>
        /// <returns>The x.</returns>
        public float X
        {
            get
            {
                return _x;
            }
        }

        /// <summary>
        /// Gets the y.
        /// </summary>
        /// <returns>The y.</returns>
        public float Y
        {
            get
            {
                return _y;
            }
        }
    }
}
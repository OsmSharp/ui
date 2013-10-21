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
using Android.Util;

namespace Android.Views
{
	/// <summary>
	/// Base class for all two-finger based events.
	/// </summary>
	public abstract class TwoFingerGestureDetector : BaseGestureDetector
	{
		private float _edgeSlop;
		private float _rightSlopEdge;
		private float _bottomSlopEdge;

		protected float _prevFingerDiffX;
		protected float _prevFingerDiffY;
		protected float _currFingerDiffX;
		protected float _currFingerDiffY;

		private float _currLen;
		private float _prevLen;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.TwoFingerGestureDetector"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		public TwoFingerGestureDetector (Context context) 
			: base(context)
		{
			ViewConfiguration config = ViewConfiguration.Get (context);
			_edgeSlop = config.ScaledEdgeSlop;
		}

		/// <summary>
		/// Updates the current state with the given event.
		/// </summary>
		/// <param name="curr">The current event.</param>
		protected override void UpdateStateByEvent (MotionEvent curr)
		{
			base.UpdateStateByEvent (curr);

			MotionEvent prev = _previousEvent;

			_currLen = -1;
			_prevLen = -1;

			if (prev != null && prev.PointerCount > 1 &&
			    curr.PointerCount > 1) {
				// previous
				float px0 = prev.GetX (0);
				float py0 = prev.GetY (0);
				float px1 = prev.GetX (1);
				float py1 = prev.GetY (1);
				float pvx = px1 - px0;
				float pvy = py1 - py0;
				_prevFingerDiffX = pvx;
				_prevFingerDiffY = pvy;

				// current
				float cx0 = curr.GetX (0);
				float cy0 = curr.GetY (0);
				float cx1 = curr.GetX (1);
				float cy1 = curr.GetY (1);
				float cvx = cx1 - cx0;
				float cvy = cy1 - cy0;
				_currFingerDiffX = cvx;
				_currFingerDiffY = cvy;
			}
		}

		/// <summary>
		/// Return the previous distance between the two pointers forming the
		/// gesture in progress.
		/// </summary>
		/// <returns>The current span.</returns>
		public virtual float GetCurrentSpan() {
			if (_currLen == -1) {
				float cvx = _currFingerDiffX;
				float cvy = _currFingerDiffY;
				_currLen = (float)System.Math.Sqrt (cvx * cvx + cvy * cvy);
			}
			return _currLen;
		}

		/// <summary>
		/// Return the previous distance between the two pointers forming the
		/// gesture in progress.
		/// </summary>
		/// <returns>The previous span.</returns>
		public virtual float GetPreviousSpan() {
			if (_prevLen == -1) {
				float pvx = _prevFingerDiffX;
				float pvy = _prevFingerDiffY;
				_prevLen = (float)System.Math.Sqrt (pvx * pvx + pvy * pvy);
			}
			return _prevLen;
		}

		/// <summary>
		/// Gets the raw x.
		/// </summary>
		/// <returns>The raw x.</returns>
		/// <param name="e">E.</param>
		/// <param name="pointerIndex">Pointerindex.</param>
		protected float GetRawX(MotionEvent e, int pointerIndex) {
			float offset = e.GetX () - e.RawX;
			if (pointerIndex < e.PointerCount) {
				return e.GetX (pointerIndex) + offset;
			}
			return 0f;
		}

		/// <summary>
		/// Gets the raw y.
		/// </summary>
		/// <returns>The raw y.</returns>
		/// <param name="e">E.</param>
		/// <param name="pointerIndex">Pointer index.</param>
		protected float GetRawY(MotionEvent e, int pointerIndex) {
			float offset = e.GetY () - e.RawY;
			if (pointerIndex < e.PointerCount) {
				return e.GetY (pointerIndex) + offset;
			}
			return 0f;
		}

		/// <summary>
		/// Check if we have a sloppy gesture. Sloppy gestures can happen if the edge
		/// of the user's hand is touching the screen, for example.
		/// </summary>
		/// <returns><c>true</c> if this instance is sloppy gesture the specified e; otherwise, <c>false</c>.</returns>
		/// <param name="e">E.</param>
		protected bool IsSloppyGesture(MotionEvent e) {       
			// As orientation can change, query the metrics in touch down
			DisplayMetrics metrics = _context.Resources.DisplayMetrics;
			_rightSlopEdge = metrics.WidthPixels - _edgeSlop;
			_bottomSlopEdge = metrics.HeightPixels - _edgeSlop;

			float edgeSlop = _edgeSlop;
			float rightSlop = _rightSlopEdge;
			float bottomSlop = _bottomSlopEdge;

			float x0 = e.RawX;
			float y0 = e.RawY;
			float x1 = this.GetRawX(e, 1);
			float y1 = this.GetRawY(e, 1);

			bool p0sloppy = x0 < edgeSlop || y0 < edgeSlop
				|| x0 > rightSlop || y0 > bottomSlop;
			bool p1sloppy = x1 < edgeSlop || y1 < edgeSlop
				|| x1 > rightSlop || y1 > bottomSlop;

			if (p0sloppy && p1sloppy) {
				return true;
			} else if (p0sloppy) {
				return true;
			} else if (p1sloppy) {
				return true;
			}
			return false;
		}

        protected override void ResetState()
        {
            _previousEvent = null;
            _prevFingerDiffX = 0;
            _prevFingerDiffY = 0;
            _currFingerDiffX = 0;
            _currFingerDiffY = 0;

            base.ResetState();
        }
	}
}


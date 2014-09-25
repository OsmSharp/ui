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

using OsmSharp.Math.Geo;
using OsmSharp.UI.Renderer;

namespace OsmSharp.UI.Animations.Invalidation.Triggers
{
    /// <summary>
    /// An invalidation trigger for a non-realtime map renderer.
    /// </summary>
    public class DefaultTrigger : TriggerBase
    {
        /// <summary>
        /// The period in millisecond used to detect that there is no movement on the surface anymore.
        /// </summary>
        private const int StaticDetectionTriggerMillis = 250;

        /// <summary>
        /// The pan percentage.
        /// </summary>
        private const int PanPercentage = 5;

        /// <summary>
        /// The degree offset.
        /// </summary>
        private const int DegreeOffset = 10;

        /// <summary>
        /// The zoom offset.
        /// </summary>
        private const float ZoomOffset = 0.2f;

        /// <summary>
        /// Holds the timer to detect movement.
        /// </summary>
        private System.Threading.Timer _timer;

        /// <summary>
        /// Creates a new default invalidation trigger.
        /// </summary>
        /// <param name="surface"></param>
        public DefaultTrigger(IInvalidatableMapSurface surface)
            :base(surface)
        {
            _timer = null;
        }

        /// <summary>
        /// Holds the latest successfully rendered view.
        /// </summary>
        private View2D _latestView;

        /// <summary>
        /// Holds the latest milliseconds it took to render.
        /// </summary>
        private int _latestMillis;

        /// <summary>
        /// Holds the latest successfully rendered zoom level.
        /// </summary>
        private double _latestZoom;

		/// <summary>
		/// Holds the latests timestamps after rendering finished.
		/// </summary>
		private int _latestTimestamp;

        /// <summary>
        /// Holds the latest successfully rendered view.
        /// </summary>
        private View2D _latestTriggeredView;

        /// <summary>
        /// Holds the latest successfully rendered zoom level.
        /// </summary>
        private double _latestTriggeredZoom;

        /// <summary>
        /// Holds the current view.
        /// </summary>
        private View2D _currentView;

        /// <summary>
        /// Holds the current zoom.
        /// </summary>
        private double _currentZoom;

        /// <summary>
        /// Notifies the current view has changed.
        /// </summary>
        public override void NotifyChange(View2D view, double zoom)
        {
            lock (this)
            {
                _currentView = view;
                _currentZoom = zoom;

                // reset the timer.
                if (_timer == null)
                { // create the timer.
                    _timer = new System.Threading.Timer(this.StaticDetectionCallback, null, StaticDetectionTriggerMillis, System.Threading.Timeout.Infinite);
                }
                else
                { // change the timer.
                    _timer.Change(StaticDetectionTriggerMillis, System.Threading.Timeout.Infinite);
                }

                // no rendering was successful up until now, only start invalidating after first successful render.
                if (_latestTriggeredView == null)
                {
                    return;
                }

                // detect changes by % of view pan.
                var toView = _latestTriggeredView.CreateToViewPort(100, 100);
                double newCenterX, newCenterY;
                toView.Apply(view.Center[0], view.Center[1], out newCenterX, out newCenterY);
                //double[] newCenter = _latestTriggeredView.ToViewPort(100, 100, view.Center[0], view.Center[1]);
                newCenterX = System.Math.Abs(newCenterX - 50.0);
                newCenterY = System.Math.Abs(newCenterY - 50.0);
                if (newCenterX > PanPercentage || newCenterY > PanPercentage)
                { // the pan percentage change was detected.
                    if (this.LatestRenderingFinished ||
                    !_latestTriggeredView.Rectangle.Overlaps(view.Rectangle))
                    { // the last rendering was finished or the latest triggered view does not overlap with the current rendering.
                        OsmSharp.Logging.Log.TraceEvent("DefaultTrigger", Logging.TraceEventType.Information,
                            "Rendering triggered: Pan detection.");
                        this.Render();
                    }
                    return;
                }

                // detect changes by angle offset.
                double angleDifference = System.Math.Abs(_latestTriggeredView.Angle.SmallestDifference(view.Angle));
                if (angleDifference > DegreeOffset)
                { // the angle difference change was detected.
                    if (this.LatestRenderingFinished ||
                    !_latestTriggeredView.Rectangle.Overlaps(view.Rectangle))
                    { // the last rendering was finished or the latest triggered view does not overlap with the current rendering.
                        OsmSharp.Logging.Log.TraceEvent("DefaultTrigger", Logging.TraceEventType.Information,
                            "Rendering triggered: Angle detection.");
                        this.Render();
                    }
                    return;
                }

                // detect changes by zoom offset.
                double zoomDifference = System.Math.Abs(_latestTriggeredZoom - _currentZoom);
                if (zoomDifference > ZoomOffset)
                { // the zoom difference change was detected.
                    if (this.LatestRenderingFinished ||
                    !_latestTriggeredView.Rectangle.Overlaps(view.Rectangle))
                    { // the last rendering was finished or the latest triggered view does not overlap with the current rendering.
                        OsmSharp.Logging.Log.TraceEvent("DefaultTrigger", Logging.TraceEventType.Information,
                            "Rendering triggered: Zoom detection.");
                        this.Render();
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// A way for the surface to complain that it is invalid.
        /// </summary>
        public override void Invalidate()
        {
            lock (this)
            {
                _latestTriggeredView = null;
            }
        }

        /// <summary>
        /// Notifies the rendering was a success.
        /// </summary>
        /// <param name="millis">The milliseconds that the previous rendering took.</param>
        /// <param name="view">The view that was used for the latest rendering.</param>
        /// <param name="zoom"></param>
        public override void NotifyRenderSuccess(View2D view, double zoom, int millis)
        {
            _latestView = view;
			if (millis > 0)
			{
				_latestMillis = millis;
			}
            _latestZoom = zoom;

            if (_latestTriggeredView == null)
            { // this rendering was not triggered by this trigger but accept this anyway.
                _latestTriggeredView = view;
                _latestTriggeredZoom = zoom;
            }

			// the triggered renderings is finished.
			this.LatestRenderingFinished = true;
        }

        /// <summary>
        /// Returns true when the latest rendering finished.
        /// </summary>
        private bool LatestRenderingFinished
		{
			get;
			set;
        }

        /// <summary>
        /// The callback used then the static detection timer is finished.
        /// </summary>
        /// <param name="state"></param>
        private void StaticDetectionCallback(object state)
        {
            OsmSharp.Logging.Log.TraceEvent("DefaultTrigger", Logging.TraceEventType.Information,
                "Rendering triggered: Static detection.");
			if (!this.Surface.StillMoving())
			{ // the surface reports there is no more movement.
				this.Render();
			}
        }

        /// <summary>
        /// Triggers rendering.
        /// </summary>
        public override void Render()
        {
            // save the current view/zoom.
            _latestTriggeredView = _currentView;
            _latestTriggeredZoom = _currentZoom;

			// a new rendering was triggered
			this.LatestRenderingFinished = false;

            // trigger the renderer.
            base.Render();
        }

        /// <summary>
        /// Stops this invalidation trigger.
        /// </summary>
        public override void Stop()
        {
            lock (this)
            {
                if (_timer != null)
                {
                    _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }
    }
}
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Units.Time;
using OsmSharp.Units.Angle;
using OsmSharp.Math.Geo;
using System.Timers;

namespace OsmSharp.UI.Animations
{
    /// <summary>
    /// Represents an animator responsible for animating movements on IMapViews.
    /// </summary>
    public class MapViewAnimator
    {
        /// <summary>
        /// Holds the map view.
        /// </summary>
        private readonly IMapView _mapView;

        /// <summary>
        /// Holds the default time span.
        /// </summary>
        private readonly TimeSpan _defaultTimeSpan;

        /// <summary>
        /// Holds the minimum allowed timespan.
        /// </summary>
        private readonly TimeSpan _minimumTimeSpan = new TimeSpan(0, 0, 0, 0, 100);

        /// <summary>
        /// Creates a new MapView Animator.
        /// </summary>
        public MapViewAnimator(IMapView mapView)
        {
            _mapView = mapView;
            _defaultTimeSpan = new TimeSpan(0, 0, 1);
        }

        /// <summary>
        /// Starts an animation to a given zoom level.
        /// </summary>
        /// <param name="zoomLevel"></param>
        public void Start(float zoomLevel)
        {
            this.Start(_mapView.MapCenter, zoomLevel, _mapView.MapTilt, _defaultTimeSpan);
        }

        /// <summary>
        /// Starts an animation to a given zoom level that will take the given timespan.
        /// </summary>
        /// <param name="zoomLevel"></param>
        /// <param name="time"></param>
        public void Start(float zoomLevel, TimeSpan time)
        {
            this.Start(_mapView.MapCenter, zoomLevel, _mapView.MapTilt, time);
        }

        /// <summary>
        /// Starts an animation to a given map tilt.
        /// </summary>
        /// <param name="mapTilt"></param>
        public void Start(Degree mapTilt)
        {
            this.Start(_mapView.MapCenter, _mapView.MapZoom, mapTilt, _defaultTimeSpan);
        }

        /// <summary>
        /// Starts an animation to a given map tilt that will take the given timespan.
        /// </summary>
        /// <param name="mapTilt"></param>
        /// <param name="time"></param>
        public void Start(Degree mapTilt, TimeSpan time)
        {
            this.Start(_mapView.MapCenter, _mapView.MapZoom, mapTilt, time);
        }

        /// <summary>
        /// Starts an animation to a given map center.
        /// </summary>
        /// <param name="center"></param>
        public void Start(GeoCoordinate center)
        {
            this.Start(center, _mapView.MapZoom, _mapView.MapTilt, _defaultTimeSpan);
        }

        /// <summary>
        /// Starts an animation to a given map center that will take the given timespan.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="time"></param>
        public void Start(GeoCoordinate center, TimeSpan time)
        {
            this.Start(center, _mapView.MapZoom, _mapView.MapTilt, time);
        }

        /// <summary>
        /// Starts a animation to a given map center and given zoom level.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="zoomLevel"></param>
        public void Start(GeoCoordinate center, float zoomLevel)
        {
            this.Start(center, zoomLevel, _mapView.MapTilt, _defaultTimeSpan);
        }

        /// <summary>
        /// Starts an animation to the given parameters.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="zoomLevel"></param>
        /// <param name="time"></param>
        public void Start(GeoCoordinate center, float zoomLevel, TimeSpan time)
        {
            this.Start(center, zoomLevel, _mapView.MapTilt, time);
        }

        /// <summary>
        /// Holds the target zoom level.
        /// </summary>
        private float _targetZoom;

        /// <summary>
        /// Holds the target tilt.
        /// </summary>
        private Degree _targetTilt;

        /// <summary>
        /// Holds the target center.
        /// </summary>
        private GeoCoordinate _targetCenter;

        /// <summary>
        /// Holds the step zoom level.
        /// </summary>
        private float _stepZoom;

        /// <summary>
        /// Holds the step tilt.
        /// </summary>
        private Degree _stepTilt;

        /// <summary>
        /// Holds the step center.
        /// </summary>
        private GeoCoordinate _stepCenter;

        /// <summary>
        /// Holds the timer.
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Holds the current step.
        /// </summary>
        private int _currentStep;

        /// <summary>
        /// Holds the maximum number of steps.
        /// </summary>
        private int _maxSteps;

        /// <summary>
        /// Starts an animation to the given parameters.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="zoomLevel"></param>
        /// <param name="mapTilt"></param>
        /// <param name="time"></param>
        public void Start(GeoCoordinate center, float zoomLevel, Degree mapTilt, TimeSpan time)
        {
            lock (this)
            {
                // stop the previous timer.
                if (_timer != null)
                { // timer exists.
                    _mapView.Invalidate();

                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }
                _timer = new Timer(_minimumTimeSpan.TotalMilliseconds);
                _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);

                // set the targets.
                _targetCenter = center;
                _targetTilt = mapTilt;
                _targetZoom = zoomLevel;

                // calculate the animation steps.
                _maxSteps = (int)System.Math.Round((double)time.TotalMilliseconds / (double)_minimumTimeSpan.TotalMilliseconds, 0);
                _currentStep = 0;
                _stepCenter = new GeoCoordinate(
                    (_targetCenter.Latitude - _mapView.MapCenter.Latitude) / _maxSteps,
                    (_targetCenter.Longitude - _mapView.MapCenter.Longitude) / _maxSteps);
                _stepZoom = (float)((_targetZoom - _mapView.MapZoom) / _maxSteps);

				// calculate the map tilt, make sure it turns along the smallest corner.
				double diff = _mapView.MapTilt.Subtract180(_targetTilt);
				_stepTilt = (Degree)(diff / _maxSteps);

                //OsmSharp.Logging.Log.TraceEvent("MapViewAnimator", System.Diagnostics.TraceEventType.Verbose,
                //    string.Format("Started new animation with steps z:{0} t:{1} c:{2} to z:{3} t:{4} c:{5}.",
                //        _stepZoom, _stepTilt, _stepCenter.ToString(), _targetZoom, _targetTilt, _targetCenter.ToString()));

				// disable auto invalidate.
				_mapView.AutoInvalidate = false;
				_mapView.RegisterAnimator (this);

                // start the timer.
                _timer.Enabled = true;
                _timer.Start();
            }
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;

					_mapView.RegisterAnimator (null);
                }
            }
        }

        /// <summary>
        /// The timer has elapsed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //OsmSharp.Logging.Log.TraceEvent("MapViewAnimator", System.Diagnostics.TraceEventType.Verbose,
            //    string.Format("Animation Step BEGIN: z:{0} -c:{1} - t:{2}", _mapView.MapZoom, _mapView.MapCenter, _mapView.MapTilt));

            _currentStep++;
            if (_currentStep < _maxSteps)
			{ // there is still need for a change.
				GeoCoordinate center = new GeoCoordinate( // update center.
				                                       (_mapView.MapCenter.Latitude + _stepCenter.Latitude),
				                                       (_mapView.MapCenter.Longitude + _stepCenter.Longitude));
				float mapZoom = _mapView.MapZoom + _stepZoom; // update zoom.
				Degree mapTilt = _mapView.MapTilt + _stepTilt; // update tilt.
				_mapView.SetMapView (center, mapTilt, mapZoom);
                //OsmSharp.Logging.Log.TraceEvent("MapViewAnimator", System.Diagnostics.TraceEventType.Verbose,
                //                                string.Format("Animation Step END: z:{0} -c:{1} - t:{2}", _mapView.MapZoom, _mapView.MapCenter, _mapView.MapTilt));
                return;
            }
            else if (_currentStep == _maxSteps)
            { // this is the last step.
				_mapView.SetMapView (_targetCenter, _targetTilt, _targetZoom);
			}

			// enable auto invalidate.
			_mapView.Invalidate ();
			_mapView.AutoInvalidate = true;
			_mapView.RegisterAnimator (null);
            _timer.Stop(); // stop the timer.
        }
    }
}
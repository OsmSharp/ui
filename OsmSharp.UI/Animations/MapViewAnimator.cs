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
using System.Threading;
using OsmSharp.Math.Geo;
using OsmSharp.Units.Angle;

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
        /// 16 millisec = 62 animation frames per second
        /// </summary>
		private readonly TimeSpan _minimumTimeSpan = new TimeSpan(0, 0, 0, 0, 16);

//        /// <summary>
//        /// Holds a synchronization object for the timer elapsed event.
//        /// </summary>
//        private static object TimerElapsedSync = new object();

        /// <summary>
        /// Creates a new MapView Animator.
        /// </summary>
        public MapViewAnimator(IMapView mapView)
        {
            if (mapView == null) { throw new ArgumentNullException("mapView"); }

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
        /// Holds all states of the animation.
        /// </summary>
        private MapAnimationState _startState, _endState, _currentState;

        /// <summary>
        /// Holds the timer.
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Holds the time in milliseconds for the start and end of the animation.
        /// </summary>
        private long _startTime, _endTime, _duration;

		/// <summary>
		/// Holds the timer status.
		/// </summary>
		private AnimatorStatus _timerStatus;

        /// <summary>
        /// Holds the cubic function representing the animation itself.
        /// </summary>
        private CubicBezier _animationFunction;

        /// <summary>
        /// Starts an animation to the given parameters.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="zoomLevel"></param>
        /// <param name="mapTilt"></param>
        /// <param name="time"></param>
        public void Start(GeoCoordinate center, float zoomLevel, Degree mapTilt, TimeSpan time)
		{
			// stop the previous timer.
			if (_timer != null)
			{ // timer exists, it might be active disable it immidiately.
				// cancel previous status.
				_timerStatus.Cancelled = true;
				_timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
				_timer.Dispose();
			}

            // Create the animation function.
            _animationFunction = CubicBezier.createEase();

			// Initialaize the start and end state of the map.
            _startState = new MapAnimationState(_mapView.MapZoom, _mapView.MapTilt, _mapView.MapCenter);
            _endState   = new MapAnimationState(zoomLevel, mapTilt, center);
            _currentState = new MapAnimationState(_mapView.MapZoom, _mapView.MapTilt, _mapView.MapCenter);

			OsmSharp.Logging.Log.TraceEvent("MapViewAnimator", OsmSharp.Logging.TraceEventType.Verbose,
				string.Format("Started new animation to z:{0} t:{1} c:{2} from z:{3} t:{4} c:{5}.",
					_endState.Zoom, _endState.Tilt, _endState.Center.ToString(), 
					_mapView.MapZoom, _mapView.MapTilt, _mapView.MapCenter.ToString()));

			// disable auto invalidate.
			_mapView.RegisterAnimator(this);

            _startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            _duration = (long)time.TotalMilliseconds;
            _endTime = _startTime + _duration;

			// start the timer.
			// create a new timer.
			_timerStatus = new AnimatorStatus();
			_timer = new Timer(new TimerCallback(_timer_Elapsed), _timerStatus, 0, (int)_minimumTimeSpan.TotalMilliseconds);
            
		}

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void Stop()
		{
			if (_timer != null)
			{ // disable timer.
				// cancel previous status.
				_timerStatus.Cancelled = true;
				_timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
				_timer.Dispose();
			}

			// unregister this animator, it has been stopped.
			_mapView.RegisterAnimator(null);
		}


        private double _timeProgress;
        private double _animationProgress;

        /// <summary>
        /// The timer has elapsed.
        /// </summary>
        /// <param name="sender"></param>
        void _timer_Elapsed(object sender)
		{
			var status = sender as AnimatorStatus;
			if (status.Cancelled)
			{ // check status when cancelled return.
				return;
			}

            long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            // Check if the animation should be finished.
            if (currentTime >= _endTime) 
            {
                _currentState = _endState;
                Stop();
            }
            else
            {
                _timeProgress = (double)(currentTime - _startTime) / (double)_duration;
                _animationProgress = _animationFunction.ComputeY(_timeProgress, getEpsilon(_duration/1000));

                _currentState.Zoom = (float)_animationProgress * (_endState.Zoom - _startState.Zoom) + _startState.Zoom;
                _currentState.Tilt = _animationProgress * (_endState.Tilt.SmallestDifference(_startState.Tilt)) + _startState.Tilt;
                _currentState.Center = new GeoCoordinate(_animationProgress * (_endState.Center.Latitude - _startState.Center.Latitude) + _startState.Center.Latitude,
                                                         _animationProgress * (_endState.Center.Longitude - _startState.Center.Longitude) + _startState.Center.Longitude);
            }
            _mapView.SetMapView(_currentState.Center, _currentState.Tilt, _currentState.Zoom);
		}

        

        // The epsilon value to pass given that the animation is going to run over |dur| seconds. The longer the
	    // animation, the more precision is needed in the timing function result to avoid ugly discontinuities.
	    private double getEpsilon(float duration) {
		    return 1.0/(200.0*duration);
	    }

		private class AnimatorStatus
		{
			public AnimatorStatus()
			{
				this.Cancelled = false;
			}

			public bool Cancelled {
				get;
				set;
			}
		}

        /// <summary>
        ///  Holds the state of a map during animations.
        /// </summary>
        private class MapAnimationState
        {
            public MapAnimationState(float zoom, Degree tilt, GeoCoordinate center)
            {
                Zoom = zoom;
                Tilt = tilt;
                Center = center;
            }
            public float Zoom
            {
                get;
                set;
            }
            public Degree Tilt
            {
                get;
                set;
            }
            public GeoCoordinate Center
            {
                get;
                set;
            }
        }
    }
}
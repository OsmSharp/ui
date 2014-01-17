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
using OsmSharp.Math;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Instructions;
using OsmSharp.Routing.Navigation;
using OsmSharp.Units.Angle;
using OsmSharp.Units.Distance;
using OsmSharp.Units.Time;

namespace OsmSharp.UI.Animations.Navigation
{
    /// <summary>
    /// Animator for the MapView when navigating along a route.
    /// </summary>
    public class RouteTrackerAnimator
    {
        /// <summary>
        /// The map view to animate.
        /// </summary>
        private readonly IMapView _mapView;

        /// <summary>
        /// The route tracker to listen to.
        /// </summary>
        private readonly RouteTracker _routeTracker;

        /// <summary>
        /// Creates a new route tracker animator.
        /// </summary>
        /// <param name="mapView">The mapview.</param>
		/// <param name="routeTracker">The tracker tracking the route.</param>
		/// <param name="restartAfterTouch">The time in second to wait before resuming tracking after the mapview is touched.</param>
        public RouteTrackerAnimator(IMapView mapView, RouteTracker routeTracker, Second restartAfterTouch)
        {
			this.MinZoom = 16;
			this.MaxZoom = 30;
			this.DefaultZoom = 18f;

            _mapView = mapView;
            _animator = new MapViewAnimator(mapView);
            _routeTracker = routeTracker;
			_minimumTrackGap = new TimeSpan (0, 0, 0, 0, 500).Ticks;

			this.RestartAfterTouch = restartAfterTouch;

			_mapView.MapTouched += MapViewMapTouched;
        }

        /// <summary>
        /// Creates a new route tracker animator.
        /// </summary>
        /// <param name="mapView">The mapview.</param>
        /// <param name="routeTracker">The tracker tracking the route.</param>
        /// <param name="restartAfterTouch">The time in second to wait before resuming tracking after the mapview is touched.</param>
        /// <param name="defaultZoom">The default zoom.</param>
        public RouteTrackerAnimator(IMapView mapView, RouteTracker routeTracker, Second restartAfterTouch, float defaultZoom)
        {
			this.MinZoom = 18;
            this.MaxZoom = 30;
            this.DefaultZoom = defaultZoom;

            _mapView = mapView;
            _animator = new MapViewAnimator(mapView);
            _routeTracker = routeTracker;
			_minimumTrackGap = new TimeSpan(0, 0, 0, 0, 500).Ticks;

            this.RestartAfterTouch = restartAfterTouch;

            _mapView.MapTouched += MapViewMapTouched;
        }

		/// <summary>
		/// Holds the last touch time.
		/// </summary>
		private long? _lastTouch;

		/// <summary>
		/// Maps the view map touched.
		/// </summary>
		/// <param name="mapView">Map view.</param>
		/// <param name="newZoom">New zoom.</param>
		/// <param name="newTilt">New tilt.</param>
		/// <param name="newCenter">New center.</param>
		private void MapViewMapTouched(IMapView mapView, float newZoom, Degree newTilt, GeoCoordinate newCenter){
			if (newZoom > this.MinZoom) {
				this.DefaultZoom = System.Math.Min (newZoom, this.MaxZoom);
			} else {
				_lastTouch = DateTime.Now.Ticks;
			}
		}

		/// <summary>
		/// Gets or sets the restart after touch period.
		/// </summary>
		/// <value>The restart after touch.</value>
		public Second RestartAfterTouch {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the minimum navigation zoom level.
		/// </summary>
		/// <value>The minimum zoom.</value>
		public float MinZoom {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum navigation zoom level.
		/// </summary>
		/// <value>The minimum zoom.</value>
		public float MaxZoom {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the default zoom level.
		/// </summary>
		/// <value>The default zoom.</value>
		public float DefaultZoom {
			get;
			set;
		}

        /// <summary>
        /// Holds the map view animator.
        /// </summary>
        private readonly MapViewAnimator _animator;

        /// <summary>
        /// Holds the latest ticks 
        /// </summary>
        private long? _lastTicks;

		/// <summary>
		/// Holds the minimum gap between two track events.
		/// </summary>
		private long _minimumTrackGap;

		/// <summary>
		/// Holds the last track ticks.
		/// </summary>
        private long? _lastTrack;

        /// <summary>
        /// Updates the tracker with the given location.
        /// </summary>
        /// <param name="location">The measured location.</param>
        public void Track(GeoCoordinate location)
        {
            this.Track(location, null);
        }

        /// <summary>
        /// Updates the tracker with the given location and angle.
        /// </summary>
        /// <param name="location">The measured location.</param>
        /// <param name="angle">The angle relative to the north measure clockwise.</param>
        public void Track(GeoCoordinate location, Degree angle)
        {
            if (location == null) { throw new ArgumentNullException("location"); }

			if (_lastTouch.HasValue) {
				// is tracking disabled now?
				TimeSpan timeFromLastTouch = new TimeSpan (DateTime.Now.Ticks - _lastTouch.Value);
				if (timeFromLastTouch.TotalSeconds >= this.RestartAfterTouch.Value) {
					// ok, the animator has waited long enough.
					_lastTouch = null;
				} else {
					// ok, the animator still has to wait for user-input.
					return;
				}
			}

			// check if the minimum gap between tracking events is respected.
			long now = DateTime.Now.Ticks;
			if (_lastTrack.HasValue) {
				if (_minimumTrackGap > now - _lastTrack.Value) {
					return; // too fast!
				}
			}
			_lastTrack = now;

			// animate the next step(s).
			TimeSpan lastTrackInterval = new TimeSpan(0, 0, 0, 0, 750);
            long ticks = DateTime.Now.Ticks;
            if (_lastTicks.HasValue)
            { // update the last track interval.
                lastTrackInterval = TimeSpan.FromTicks(ticks - _lastTicks.Value);
            }
            _lastTicks = ticks;
            OsmSharp.Logging.Log.TraceEvent("", OsmSharp.Logging.TraceEventType.Information,
                "Interval: {0}ms", lastTrackInterval.TotalMilliseconds);

            // give location to the route tracker.
            _routeTracker.Track(location);

            // calculate all map view parameters (zoom, location, tilt) to display the route/direction correctly.
            float zoom = this.DefaultZoom;
            GeoCoordinate center = _routeTracker.PositionRoute;
			double nextDistance = 50;
			Degree tilt = _mapView.MapTilt;
			GeoCoordinate next = _routeTracker.PositionIn(nextDistance);
			if (next != null) {
				IProjection projection = _mapView.Map.Projection;
				VectorF2D direction = new PointF2D(projection.ToPixel(next)) -
					new PointF2D(projection.ToPixel(center));
				tilt = direction.Angle(new VectorF2D(0, -1));
			}

            // overwrite calculated tilt with the given degrees.
            if (angle != null)
            {
                tilt = angle;
            }
            
            // animate to the given parameter (zoom, location, tilt).
            _animator.Stop();
			_animator.Start(center, zoom, tilt, lastTrackInterval.Subtract(new TimeSpan(0, 0, 0, 0, 50)));
        }

        /// <summary>
        /// Returns the distance between the current position and the route.
        /// </summary>
        public Meter DistanceNextInstruction
        {
            get
            {
                return _routeTracker.DistanceNextInstruction;
            }
        }

        /// <summary>
        /// Returns the next instruction.
        /// </summary>
        public Instruction NextInstruction
        {
            get
            {
                return _routeTracker.NextInstruction;
            }
        }

        /// <summary>
        /// Returns the next instruction index.
        /// </summary>
        public int NextInstructionIdx
        {
            get
            {
                return _routeTracker.NextInstructionIdx;
            }
        }

        /// <summary>
        /// Returns the instruction list that 
        /// </summary>
        public List<Instruction> NextInstructionList
        {
            get
            {
                return _routeTracker.NextInstructionList;
            }
        }
    }
}
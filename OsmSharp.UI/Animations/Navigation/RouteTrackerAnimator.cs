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

using OsmSharp.Routing.Navigation;
using OsmSharp.Math.Geo;
using OsmSharp.Units.Angle;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math;
using OsmSharp.Math.Primitives;
using System;
using OsmSharp.Units.Distance;
using OsmSharp.Routing.Instructions;
using System.Collections.Generic;

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
        /// <param name="mapView"></param>
        /// <param name="routeTracker"></param>
        public RouteTrackerAnimator(IMapView mapView, RouteTracker routeTracker)
        {
            _mapView = mapView;
            _animator = new MapViewAnimator(mapView);
            _routeTracker = routeTracker;
        }

        /// <summary>
        /// Holds the map view animator.
        /// </summary>
        private readonly MapViewAnimator _animator;

        /// <summary>
        /// Holds the zoom level to use.
        /// </summary>
        private float _zoom = 18;

        /// <summary>
        /// Holds the latest ticks 
        /// </summary>
        private long? _lastTicks;

        /// <summary>
        /// Starts tracking at a given location.
        /// </summary>
        /// <param name="location"></param>
        public void Track(GeoCoordinate location)
        {
            TimeSpan lastTrackInterval = new TimeSpan(0, 0, 0, 0, 750);
            long ticks = DateTime.Now.Ticks;
            if (_lastTicks.HasValue)
            { // update the last track interval.
                lastTrackInterval = TimeSpan.FromTicks(ticks - _lastTicks.Value);
            }
            _lastTicks = ticks;
            OsmSharp.Logging.Log.TraceEvent("", System.Diagnostics.TraceEventType.Information,
                "Interval: {0}ms", lastTrackInterval.TotalMilliseconds);

            // give location to the route tracker.
            _routeTracker.Track(location);

            // calculate all map view parameters (zoom, location, tilt) to display the route/direction correctly.
            float zoom = _zoom; // TODO: do something smarter here to allow the zoom level to be customized.
            GeoCoordinate center = _routeTracker.PositionRoute;
			double nextDistance = 100;
			GeoCoordinate next = _routeTracker.PositionIn(nextDistance);
			while (next == null) {
				nextDistance = nextDistance - 10;
				if (nextDistance <= 0) {
					next = center;
				}
				next = _routeTracker.PositionIn(nextDistance);
			}
            IProjection projection = _mapView.Map.Projection;
            VectorF2D direction = new PointF2D(projection.ToPixel(next)) -
                        new PointF2D(projection.ToPixel(center));
            Degree tilt = direction.Angle(new VectorF2D(0, -1));
            
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
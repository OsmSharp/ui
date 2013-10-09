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
            _animator = new MapViewAnimator(mapView);
        }

        /// <summary>
        /// Holds the map view animator.
        /// </summary>
        private readonly MapViewAnimator _animator;

        /// <summary>
        /// Starts tracking at a given location.
        /// </summary>
        /// <param name="location"></param>
        public void Track(GeoCoordinate location)
        {

        }
    }
}
// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
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
using OsmSharp.Units.Angle;
using OsmSharp.UI.Animations;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer;
using System;

namespace OsmSharp.UI
{
    /// <summary>
    /// Abstracts an implementation of a MapView with a MapCenter, MapTilt and MapZoom.
    /// </summary>
    public interface IMapView
    {
        /// <summary>
        /// Raised when a pressure gesture on the map was started.
        /// </summary>
        event MapViewDelegates.MapTouchedDelegate MapTouchedDown;

		/// <summary>
		/// Raised when the map is being touched.
		/// </summary>
        event MapViewDelegates.MapTouchedDelegate MapTouched;

        /// <summary>
        /// Raised when a pressure gesture on the map was finished.
        /// </summary>
        event MapViewDelegates.MapTouchedDelegate MapTouchedUp;

        /// <summary>
        /// Raised when the map moves.
        /// </summary>
        event MapViewDelegates.MapMoveDelegate MapMove;

        /// <summary>
        /// Raised when the map was first initialized, meaning it has a size and it was rendered for the first time.
        /// </summary>
        event MapViewDelegates.MapInitialized MapInitialized;

		/// <summary>
		/// Invalidates this instance.
		/// </summary>
		void Invalidate ();

		/// <summary>
		/// Registers the animator.
		/// </summary>
		/// <param name="mapViewAnimator">Map view animator.</param>
		void RegisterAnimator (MapViewAnimator mapViewAnimator);

		/// <summary>
		/// Sets the map view by changing all three parameters at once.
		/// </summary>
		/// <param name="center">Center.</param>
		/// <param name="mapTilt">Map tilt.</param>
		/// <param name="mapZoom">Map zoom.</param>
		void SetMapView(GeoCoordinate center, Degree mapTilt, float mapZoom);

        /// <summary>
        /// Gets or sets the MapCenter;
        /// </summary>
        GeoCoordinate MapCenter { get; set; }

        /// <summary>
        /// Gets or sets the bounding box within which one can pan the map.
        /// </summary>
        /// <value>The box.</value>
        GeoCoordinateBox MapBoundingBox { get; set; }

        /// <summary>
        /// Gets or sets the map minimum zoom level.
        /// </summary>
        /// <value>The map minimum zoom level.</value>
        float MapMinZoomLevel { get; set; }

        /// <summary>
        /// Gets or sets the map maximum zoom level.
        /// </summary>
        /// <value>The map maximum zoom level.</value>
        float MapMaxZoomLevel { get; set; }

        /// <summary>
        /// Gets or sets the tilt interaction flag.
        /// </summary>
        bool MapAllowTilt { get; set; }

        /// <summary>
        /// Gets or sets the allow pan flag.
        /// </summary>
        bool MapAllowPan { get; set; }

        /// <summary>
        /// Gets or sets the allow zoom flag.
        /// </summary>
        bool MapAllowZoom { get; set; }

        /// <summary>
        /// Gets or sets the MapTilt.
        /// </summary>
        Degree MapTilt { get; set; }

        /// <summary>
        /// Gets or sets the MapZoom.
        /// </summary>
        float MapZoom { get; set; }

        /// <summary>
        /// Gets or sets the Map.
        /// </summary>
        Map.Map Map { get; set; }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        View2D CurrentView { get; }

        /// <summary>
        /// Gets the current width.
        /// </summary>
        /// <value>The width.</value>
        int CurrentWidth
        {
            get;
        }

        /// <summary>
        /// Gets the current height.
        /// </summary>
        /// <value>The height.</value>
        int CurrentHeight
        {
            get;
        }

        /// <summary>
        /// Gets the density.
        /// </summary>
        float Density { get; }

        /// <summary>
        /// Notifies a map change.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        void NotifyMapChange(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection);
    }

	/// <summary>
	/// Map view delegates.
	/// </summary>
	public static class MapViewDelegates 
	{
        /// <summary>
        /// Delegate used for map touches.
        /// </summary>
        /// <param name="mapView"></param>
        /// <param name="newZoom"></param>
        /// <param name="newTilt"></param>
        /// <param name="newCenter"></param>
		public delegate void MapTouchedDelegate(IMapView mapView, float newZoom, Degree newTilt, GeoCoordinate newCenter);

        /// <summary>
        /// Delegate used for map touches.
        /// </summary>
        /// <param name="mapView"></param>
        /// <param name="newZoom"></param>
        /// <param name="newTilt"></param>
        /// <param name="newCenter"></param>
        public delegate void MapMoveDelegate(IMapView mapView, float newZoom, Degree newTilt, GeoCoordinate newCenter);

        /// <summary>
        /// Delegate used for map touches.
        /// </summary>
        /// <param name="mapView"></param>
        /// <param name="newZoom"></param>
        /// <param name="newTilt"></param>
        /// <param name="newCenter"></param>
        public delegate void MapInitialized(IMapView mapView, float newZoom, Degree newTilt, GeoCoordinate newCenter);
	}
}
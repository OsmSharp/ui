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
using Android.Content;
using Android.Views;
using Android.Widget;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI;
using OsmSharp.UI.Animations;
using OsmSharp.UI.Map;
using OsmSharp.UI.Renderer;
using OsmSharp.Units.Angle;

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// Map view handling the map display and pan-zoon markers and touch-events.
	/// </summary>
    public class MapView : FrameLayout, IMapMarkerHost, IMapView
	{
		/// <summary>
		/// Holds the mapview.
		/// </summary>
        private IMapViewSurface _mapView;

		/// <summary>
		/// Holds the markers.
		/// </summary>
		private List<MapMarker> _markers;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapLayout"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="surface">Surface.</param>
		public MapView (Context context, IMapViewSurface surface)
			: base(context)
		{
			_mapView = surface;
            _mapView.Initialize(this);
			_markers = new List<MapMarker> ();

			this.Initialize ();
		}

		/// <summary>
		/// Occurs when the map was tapped at a certain location.
		/// </summary>
        public event MapViewEvents.MapTapEventDelegate MapTapEvent;

        /// <summary>
        /// Occurs when the map was touched for a longer time at a certain location.
        /// </summary>
        public event MapViewEvents.MapTapEventDelegate MapHoldEvent;

        /// <summary>
        /// Raises the map tap event.
        /// </summary>
        /// <param name="coordinate"></param>
        internal void RaiseMapTapEvent(GeoCoordinate coordinate)
        {
            if (this.MapTapEvent != null)
            {
                this.MapTapEvent(coordinate);
            }
        }

		/// <summary>
		/// Returns the mapmarkers list.
		/// </summary>
		/// <value>The markers.</value>
		public void AddMarker (MapMarker marker)
        {
            if (marker == null) { throw new ArgumentNullException("marker"); };

			_markers.Add (marker); // add to marker list.
			marker.AttachTo (this); // attach to this view.

			var layoutParams = new FrameLayout.LayoutParams (marker.Image.Width, marker.Image.Height + 5);
			layoutParams.LeftMargin = -1;
			layoutParams.TopMargin = -1;
			layoutParams.Gravity = GravityFlags.Top | GravityFlags.Left;
			this.AddView (marker, layoutParams);

			_mapView.Change ();
		}

		/// <summary>
		/// Adds the marker.
		/// </summary>
		/// <returns>The marker.</returns>
        /// <param name="location">Coordinate.</param>
        public MapMarker AddMarker(GeoCoordinate location)
        {
            if (location == null) { throw new ArgumentNullException("location"); };

            MapMarker marker = new MapMarker(this.Context, location);
			this.AddMarker (marker);
			return marker;
		}

        /// <summary>
        /// Clears all map markers.
        /// </summary>
        public void ClearMarkers()
        {
            if (_markers != null)
            {
                foreach (MapMarker marker in _markers)
                {
                    this.RemoveView(marker);
                }
                _markers.Clear();
            }
        }

        /// <summary>
        /// Removes the given map marker.
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        public bool RemoveMarker(MapMarker marker)
        {
            if (marker != null)
            {
                this.RemoveView(marker);
                return _markers.Remove(marker);
            }
            return false;
        }

        /// <summary>
        /// Zoom to the current markers.
        /// </summary>
        public void ZoomToMarkers()
        {
            this.ZoomToMarkers(_markers);
        }

        /// <summary>
        /// Zoom to the given makers list.
        /// </summary>
        /// <param name="marker"></param>
        public void ZoomToMarkers(List<MapMarker> markers)
        {
            _mapView.ZoomToMarkers(markers);
        }

		/// <summary>
		/// Notifies this MapView that a map marker has changed.
		/// </summary>
		/// <param name="mapMarker"></param>
		public void NotifyMarkerChange (MapMarker mapMarker)
		{
			// notify map layout of changes.
			if (_mapView.Width > 0 && _mapView.Height > 0) {
				View2D view = _mapView.CreateView ();

				this.NotifyMapChangeToMarker (this.Width, this.Height, view, this.Map.Projection, mapMarker);
			}
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		private void Initialize ()
		{			
			this.AddView (_mapView as View);
		}

		/// <summary>
		/// Gets or sets the map zoom level.
		/// </summary>
		/// <value>The map zoom level.</value>
		public float MapZoom {
			get { return _mapView.MapZoom; }
			set { _mapView.MapZoom = value; }
		}

		/// <summary>
		/// Gets or sets the map minimum zoom level.
		/// </summary>
		/// <value>The map minimum zoom level.</value>
		public float? MapMinZoomLevel {
			get { return _mapView.MapMinZoomLevel; }
			set { _mapView.MapMinZoomLevel = value; }
		}

		/// <summary>
		/// Gets or sets the map max zoom level.
		/// </summary>
		/// <value>The map max zoom level.</value>
		public float? MapMaxZoomLevel {
			get { return _mapView.MapMaxZoomLevel; }
			set { _mapView.MapMaxZoomLevel = value; }
		}

		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		/// <value>The map.</value>
		public Map Map {
			get {
				return _mapView.Map;
			}
			set {
				_mapView.Map = value;
			}
		}

		/// <summary>
		/// Gets or sets the map center.
		/// </summary>
		/// <value>The map center.</value>
		public GeoCoordinate MapCenter {
			get { 
				return _mapView.MapCenter; 
			}
			set { 
				_mapView.MapCenter = value; 
			}
		}

		/// <summary>
		/// Gets or sets the map tilt.
		/// </summary>
		/// <value>The map tilt.</value>
		public Degree MapTilt {
			get {
				return _mapView.MapTilt;
			}
			set {
				_mapView.MapTilt = value;
			}
		}

		#region IMapView implementation
		/// <summary>
		/// Holds the map view animator.
		/// </summary>
		private MapViewAnimator _mapViewAnimator;

		/// <summary>
		/// Registers the animator.
		/// </summary>
		/// <param name="mapViewAnimator">Map view animator.</param>
		void IMapView.RegisterAnimator (MapViewAnimator mapViewAnimator)
		{
			_mapViewAnimator = mapViewAnimator;
		}

		/// <summary>
		/// Sets the map view.
		/// </summary>
		/// <param name="center">Center.</param>
		/// <param name="mapTilt">Map tilt.</param>
		/// <param name="mapZoom">Map zoom.</param>
		void IMapView.SetMapView (GeoCoordinate center, Degree mapTilt, float mapZoom)
		{
			_mapView.SetMapView (center, mapTilt, mapZoom);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="OsmSharp.Android.UI.MapView"/> auto invalidates.
		/// </summary>
		/// <value><c>true</c> if auto invalidate; otherwise, <c>false</c>.</value>
		bool IMapView.AutoInvalidate {
			get {
				return _mapView.AutoInvalidate;
			}
			set {
				_mapView.AutoInvalidate = value;
			}
		}
		#endregion

		/// <summary>
		/// Notifies the map change.
		/// </summary>
		/// <param name="pixelsWidth">Pixels width.</param>
		/// <param name="pixelsHeight">Pixels height.</param>
		/// <param name="view">View.</param>
		/// <param name="projection">Projection.</param>
		public void NotifyMapChange (double pixelsWidth, double pixelsHeight, View2D view, IProjection projection)
		{
			if (_markers != null) {
				foreach (var marker in _markers) {
					this.NotifyMapChangeToMarker (pixelsWidth, pixelsHeight, view, projection, marker);
				}
			}
		}

		/// <summary>
		/// Notifies the map change.
		/// </summary>
		/// <param name="pixelWidth"></param>
		/// <param name="pixelsHeight"></param>
		/// <param name="view"></param>
		/// <param name="projection"></param>
		/// <param name="mapMarker"></param>
		internal void NotifyMapChangeToMarker (double pixelsWidth, double pixelsHeight, View2D view, IProjection projection, MapMarker mapMarker)
		{
			if (mapMarker != null) {
				this.RemoveView (mapMarker);
				if (mapMarker.SetLayout (pixelsWidth, pixelsHeight, view, projection)) {
					this.AddView (mapMarker, mapMarker.LayoutParameters);
				}
			}
		}

        /// <summary>
        /// Invalidates.
        /// </summary>
        void IMapView.Invalidate()
        {
            _mapView.Change();
        }
    }
}
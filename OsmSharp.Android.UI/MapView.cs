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
using Android.Views;
using Android.Widget;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI;
using OsmSharp.UI.Animations;
using OsmSharp.UI.Map;
using OsmSharp.UI.Renderer;
using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OsmSharp.Android.UI
{
    /// <summary>
    /// Map view handling the map display and pan-zoon markers and touch-events.
    /// </summary>
    public class MapView : FrameLayout, IMapMarkerHost, IMapView
    {
        /// <summary>
        /// Map touched events.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouched;

        /// <summary>
        /// Occurs when the map was tapped at a certain location.
        /// </summary>
        public event MapViewEvents.MapTapEventDelegate MapTapEvent;

        /// <summary>
        /// Occurs when the map was touched for a longer time at a certain location.
        /// </summary>
        public event MapViewEvents.MapTapEventDelegate MapHoldEvent;

        /// <summary>
        /// Holds the mapview.
        /// </summary>
        private IMapViewSurface _mapView;

        /// <summary>
        /// Holds the markers.
        /// </summary>
        private List<MapMarker> _markers;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapLayout"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="surface">Surface.</param>
        public MapView(Context context, IMapViewSurface surface)
            : base(context)
        {
            _mapView = surface;
            _mapView.Initialize(this);
            _markers = new List<MapMarker>();

            this.Initialize();
        }

        #endregion

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
        /// Raises the map touched event.
        /// </summary>
        internal void RaiseMapTouched()
        {
            if (this.MapTouched != null)
            {
                this.MapTouched(this, this.MapZoom, this.MapTilt, this.MapCenter);
            }
        }

        /// <summary>
        /// Returns the mapmarkers list.
        /// </summary>
        /// <value>The markers.</value>
        public void AddMarker(MapMarker marker)
        {
            if (marker == null)
            {
                throw new ArgumentNullException("marker");
            }

            lock (_markers)
            {
                if (marker.Image == null)
                    return;

                _markers.Add(marker); // add to marker list.
                marker.AttachTo(this); // attach to this view.

                var layoutParams = new FrameLayout.LayoutParams(marker.Image.Width, marker.Image.Height + 5);

                layoutParams.LeftMargin = -1;
                layoutParams.TopMargin = -1;
                layoutParams.Gravity = GravityFlags.Top | GravityFlags.Left;
                this.AddView(marker, layoutParams);
            }
            this.NotifyMarkerChange(marker);
            _mapView.TriggerRendering();
        }

        /// <summary>
        /// Adds the marker.
        /// </summary>
        /// <returns>The marker.</returns>
        /// <param name="location">Coordinate.</param>
        public MapMarker AddMarker(GeoCoordinate location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            ;

            var marker = new MapMarker(this.Context, location);
            this.AddMarker(marker);
            return marker;
        }

        /// <summary>
        /// Returns a read-only collection of markers.
        /// </summary>
        public ReadOnlyCollection<MapMarker> Markers
        {
            get
            {
                lock (_markers)
                {
                    return _markers.AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Clears all map markers.
        /// </summary>
        public void ClearMarkers()
        {
            lock (_markers)
            {
                if (_markers != null)
                {
                    foreach (MapMarker marker in _markers)
                    {
                        this.RemoveView(marker);
                        marker.Dispose();
                    }
                    _markers.Clear();
                }
            }
        }

        /// <summary>
        /// Removes the given map marker.
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        public bool RemoveMarker(MapMarker marker)
        {
            lock (_markers)
            {
                if (marker != null)
                {
                    this.RemoveView(marker);
                    return _markers.Remove(marker);
                }
                return false;
            }
        }

        /// <summary>
        /// Zoom to the current markers.
        /// </summary>
        public void ZoomToMarkers()
        {
            this.ZoomToMarkers(15);
        }

        /// <summary>
        /// Zoom to the current markers.
        /// </summary>
        public void ZoomToMarkers(double percentage)
        {
            lock (_markers)
            {
                this.ZoomToMarkers(_markers, 15);
            }
        }

        /// <summary>
        /// Zoom to the given makers list.
        /// </summary>
        /// <param name="markers"></param>
        public void ZoomToMarkers(List<MapMarker> markers)
        {
            this.ZoomToMarkers(markers, 15);
        }

        /// <summary>
        /// Zoom to the given makers list.
        /// </summary>
        /// <param name="markers"></param>
        public void ZoomToMarkers(List<MapMarker> markers, double percentage)
        {
            _mapView.ZoomToMarkers(markers, percentage);
        }

        /// <summary>
        /// Notifies this MapView that a map marker has changed.
        /// </summary>
        /// <param name="mapMarker"></param>
        public void NotifyMarkerChange(MapMarker mapMarker)
        { // notify map layout of changes.
            if (_mapView.Width > 0 && _mapView.Height > 0)
            {
                View2D view = _mapView.CreateView();

                this.NotifyMapChangeToMarker(_mapView.Width, _mapView.Height, view, this.Map.Projection, mapMarker);
            }
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        private void Initialize()
        {			
            this.AddView(_mapView as View);
        }

        /// <summary>
        /// Gets or sets the map zoom level.
        /// </summary>
        /// <value>The map zoom level.</value>
        public float MapZoom
        {
            get { return _mapView.MapZoom; }
            set { _mapView.MapZoom = value; }
        }

        /// <summary>
        /// Gets or sets the map minimum zoom level.
        /// </summary>
        /// <value>The map minimum zoom level.</value>
        public float? MapMinZoomLevel
        {
            get { return _mapView.MapMinZoomLevel; }
            set { _mapView.MapMinZoomLevel = value; }
        }

        /// <summary>
        /// Gets or sets the map max zoom level.
        /// </summary>
        /// <value>The map max zoom level.</value>
        public float? MapMaxZoomLevel
        {
            get { return _mapView.MapMaxZoomLevel; }
            set { _mapView.MapMaxZoomLevel = value; }
        }

        /// <summary>
        /// Gets or sets the map tilt flag.
        /// </summary>
        public bool MapAllowTilt
        {
            get { return _mapView.MapAllowTilt; }
            set { _mapView.MapAllowTilt = value; }
        }

        /// <summary>
        /// Gets or sets the map pan flag.
        /// </summary>
        public bool MapAllowPan
        {
            get { return _mapView.MapAllowPan; }
            set { _mapView.MapAllowPan = value; }
        }

        /// <summary>
        /// Gets or sets the map zoom flag.
        /// </summary>
        public bool MapAllowZoom
        {
            get { return _mapView.MapAllowZoom; }
            set { _mapView.MapAllowZoom = value; }
        }

        /// <summary>
        /// Gets or sets the map.
        /// </summary>
        /// <value>The map.</value>
        public Map Map
        {
            get { return _mapView.Map; }
            set { _mapView.Map = value; }
        }

        /// <summary>
        /// Gets or sets the map center.
        /// </summary>
        /// <value>The map center.</value>
        public GeoCoordinate MapCenter
        {
            get { return _mapView.MapCenter; }
            set { _mapView.MapCenter = value; }
        }

        /// <summary>
        /// Gets or sets the map tilt.
        /// </summary>
        /// <value>The map tilt.</value>
        public Degree MapTilt
        {
            get { return _mapView.MapTilt; }
            set { _mapView.MapTilt = value; }
        }

        /// <summary>
        /// Gets or sets the map scale factor.
        /// </summary>
        public float MapScaleFactor
        {
            get { return _mapView.MapScaleFactor; }
            set { _mapView.MapScaleFactor = value; }
        }


        /// <summary>
        /// Gets the current view.
        /// </summary>
        public View2D CurrentView
        {
            get { return _mapView.CurrentView; }
        }

        /// <summary>
        /// Gets the density.
        /// </summary>
        public float Density
        {
            get { return _mapView.Density; }
        }

        #region IMapView implementation

        /// <summary>
        /// Registers the animator.
        /// </summary>
        /// <param name="mapViewAnimator">Map view animator.</param>
        void IMapView.RegisterAnimator(MapViewAnimator mapViewAnimator)
        {
            (_mapView as IMapViewSurface).RegisterAnimator(mapViewAnimator);
        }

        /// <summary>
        /// Sets the map view.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="mapTilt">Map tilt.</param>
        /// <param name="mapZoom">Map zoom.</param>
        void IMapView.SetMapView(GeoCoordinate center, Degree mapTilt, float mapZoom)
        {
            _mapView.SetMapView(center, mapTilt, mapZoom);
        }

        #endregion

        /// <summary>
        /// Notifies the map change.
        /// </summary>
        /// <param name="pixelsWidth">Pixels width.</param>
        /// <param name="pixelsHeight">Pixels height.</param>
        /// <param name="view">View.</param>
        /// <param name="projection">Projection.</param>
        public void NotifyMapChange(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection)
        {
            lock (_markers)
            {
                if (_markers != null)
                {
                    foreach (var marker in _markers)
                    {
                        this.NotifyMapChangeToMarker(pixelsWidth, pixelsHeight, view, projection, marker);
                    }
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
        internal void NotifyMapChangeToMarker(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection, MapMarker mapMarker)
        {
            if (mapMarker != null &&
                mapMarker.Handle != IntPtr.Zero)
            {
                this.RemoveView(mapMarker);
                if (mapMarker.SetLayout(pixelsWidth, pixelsHeight, view, projection))
                {
                    this.AddView(mapMarker, mapMarker.LayoutParameters);
                }
            }
        }

        /// <summary>
        /// Diposes of all resources associated with this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                //someone want the deterministic release of all resources
                //Let us release all the managed resources
            }
            else
            {
                // Do nothing, no one asked a dispose, the object went out of
                // scope and finalized is called so lets next round of GC 
                // release these resources
            }

            // Release the unmanaged resource in any case as they will not be 
            // released by GC
            if (this._mapView != null)
            { // dispose of the map view surface.
                this._mapView.Dispose();
                this._mapView = null;
            }
        }

        /// <summary>
        /// Invalidates.
        /// </summary>
        void IMapView.Invalidate()
        {
            _mapView.TriggerRendering(true);
        }

        private class MapViewMarkerZoomEvent
        {
            /// <summary>
            /// Gets or sets the markers.
            /// </summary>
            /// <value>The markers.</value>
            public List<MapMarker> Markers
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the percentage.
            /// </summary>
            /// <value>The percentage.</value>
            public float Percentage
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Pauses all activity in this MapView.
        /// </summary>
        public void Pause()
        {
            _mapView.Pause();
        }

        /// <summary>
        /// Returns true if all activity in this MapView is paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return _mapView.IsPaused;
            }
        }

        /// <summary>
        /// Resumes all activity in this MapView.
        /// </summary>
        public void Resume()
        {
            _mapView.Resume();
        }

        /// <summary>
        /// Closes this mapview.
        /// </summary>
        public void Close()
        {
            // clears all markers.
            lock (_markers)
            {
                _markers.Clear();
            }

            // closes the mapview.
            _mapView.Close();
        }
    }
}
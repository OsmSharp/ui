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
using OsmSharp.Android.UI.Controls;
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
    public class MapView : FrameLayout, IMapControlHost, IMapView
    {
        /// <summary>
        /// Map touched down event.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouchedDown;

        /// <summary>
        /// Map touched event.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouched;

        /// <summary>
        /// Map touched up event.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouchedUp;

        /// <summary>
        /// Raised when the map moves.
        /// </summary>
        public event MapViewDelegates.MapMoveDelegate MapMove;

        /// <summary>
        /// Raised when the map was first initialized, meaning it has a size and it was rendered for the first time.
        /// </summary>
        public event MapViewDelegates.MapInitialized MapInitialized;

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

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MapView.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="surface">Surface.</param>
        public MapView(Context context, IMapViewSurface surface)
            : base(context)
        {
            _mapView = surface;
            _mapView.Initialize(this);
            _markers = new List<MapMarker>();
            _controls = new List<MapControl>();

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

            this.NotifyMapTapToControls();
        }

        /// <summary>
        /// Raises the map touched down event.
        /// </summary>
        internal void RaiseMapTouchedDown()
        {
            if (this.MapTouchedDown != null)
            {
                this.MapTouchedDown(this, this.MapZoom, this.MapTilt, this.MapCenter);
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
        /// Raises the map touched down event.
        /// </summary>
        internal void RaiseMapTouchedUp()
        {
            if (this.MapTouchedUp != null)
            {
                this.MapTouchedUp(this, this.MapZoom, this.MapTilt, this.MapCenter);
            }
        }

        /// <summary>
        /// Raises the map move event.
        /// </summary>
        internal void RaiseMapMove()
        {
            if (this.MapMove != null)
            {
                this.MapMove(this, this.MapZoom, this.MapTilt, this.MapCenter);
            }
        }

        /// <summary>
        /// Raises the map initialized event.
        /// </summary>
        internal void RaiseMapInitialized()
        {
            if (this.MapInitialized != null)
            {
                this.MapInitialized(this, this.MapZoom, this.MapTilt, this.MapCenter);
            }
        }

        #region Controls

        #region Markers

        /// <summary>
        /// Holds the markers.
        /// </summary>
        private List<MapMarker> _markers;

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

                this.AddView(marker.View, marker.View.LayoutParameters);
            }
            this.NotifyControlChange(marker);
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
                        marker.DetachFrom(this);
                        this.RemoveView(marker.View);
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
                    marker.DetachFrom(this);
                    this.RemoveView(marker.View);
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
        /// <param name="percentage"></param>
        public void ZoomToMarkers(List<MapMarker> markers, double percentage)
        {
            if (markers == null)
            {
                return;
            }

            var controls = new List<MapControl>(markers.Count);
            foreach(var marker in markers)
            {
                controls.Add(marker);
            }
            _mapView.ZoomToControls(controls, percentage);
        }

        #endregion

        #region Controls

        /// <summary>
        /// Holds the controls.
        /// </summary>
        private List<MapControl> _controls;

        /// <summary>
        /// Returns the mapcontrols list.
        /// </summary>
        /// <value>The controls.</value>
        public void AddControl(MapControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            lock (_controls)
            {
                _controls.Add(control); // add to control list.
                control.AttachTo(this); // attach to this view.

                this.AddView(control.BaseView, control.BaseView.LayoutParameters);
            }
            this.NotifyControlChange(control);
            _mapView.TriggerRendering();
        }

        /// <summary>
        /// Returns a read-only collection of controls.
        /// </summary>
        public ReadOnlyCollection<MapControl> Controls
        {
            get
            {
                lock (_controls)
                {
                    return _controls.AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Clears all map controls.
        /// </summary>
        public void ClearControls()
        {
            lock (_controls)
            {
                if (_controls != null)
                {
                    foreach (MapControl control in _controls)
                    {
                        this.RemoveView(control.BaseView);
                        control.Dispose();
                    }
                    _controls.Clear();
                }
            }
        }

        /// <summary>
        /// Removes the given map control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool RemoveControl(MapControl control)
        {
            lock (_controls)
            {
                if (control != null)
                {
                    this.RemoveView(control.BaseView);
                    return _controls.Remove(control);
                }
                return false;
            }
        }

        /// <summary>
        /// Zoom to the current controls.
        /// </summary>
        public void ZoomToControls()
        {
            this.ZoomToControls(15);
        }

        /// <summary>
        /// Zoom to the current controls.
        /// </summary>
        public void ZoomToControls(double percentage)
        {
            lock (_controls)
            {
                this.ZoomToControls(_controls, 15);
            }
        }

        /// <summary>
        /// Zoom to the given makers list.
        /// </summary>
        /// <param name="controls"></param>
        public void ZoomToControls(List<MapControl> controls)
        {
            this.ZoomToControls(controls, 15);
        }

        /// <summary>
        /// Zoom to the given makers list.
        /// </summary>
        /// <param name="controls"></param>
        public void ZoomToControls(List<MapControl> controls, double percentage)
        {
            _mapView.ZoomToControls(controls, percentage);
        }

        #endregion

        /// <summary>
        /// Notifies this MapView that a map marker has changed.
        /// </summary>
        /// <param name="mapControl"></param>
        public void NotifyControlChange(MapControl mapControl)
        { // notify map layout of changes.
            if (_mapView.Width > 0 && _mapView.Height > 0)
            {
                View2D view = _mapView.CreateView();

                this.NotifyOnBeforeSetLayout();
                this.NotifyMapChangeToControl(_mapView.Width, _mapView.Height, view, this.Map.Projection, mapControl);
                this.NotifyOnAfterSetLayout();
            }
        }

        /// <summary>
        /// Notifies the map change.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        /// <param name="mapControl"></param>
        internal void NotifyMapChangeToControl(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection, MapControl mapControl)
        {
            if (mapControl != null &&
                mapControl.Handle != IntPtr.Zero)
            {
                this.RemoveView(mapControl.BaseView);
                if (mapControl.SetLayout(pixelsWidth, pixelsHeight, view, projection))
                {
                    this.AddView(mapControl.BaseView, mapControl.BaseView.LayoutParameters);
                }
            }
        }

        /// <summary>
        /// Notifies the map change.
        /// </summary>
        /// <param name="pixelsWidth">Pixels width.</param>
        /// <param name="pixelsHeight">Pixels height.</param>
        /// <param name="view">View.</param>
        /// <param name="projection">Projection.</param>
        public void NotifyMapChange(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection)
        {
            this.NotifyOnBeforeSetLayout();
            lock (_markers)
            {
                if (_markers != null)
                {
                    foreach (var marker in _markers)
                    {
                        this.NotifyMapChangeToControl(pixelsWidth, pixelsHeight, view, projection, marker);
                    }
                }
            }
            lock(_controls)
            {
                if(_controls != null)
                {
                    foreach (var control in _controls)
                    {
                        this.NotifyMapChangeToControl(pixelsWidth, pixelsHeight, view, projection, control);
                    }
                }
            }
            this.NotifyOnAfterSetLayout();
        }

        /// <summary>
        /// Calls OnBeforeLayout on all controls/markers.
        /// </summary>
        internal void NotifyOnBeforeSetLayout()
        {
            lock (_markers)
            {
                if (_markers != null)
                {
                    foreach (var marker in _markers)
                    {
                        marker.OnBeforeSetLayout();
                    }
                }
            }
            lock (_controls)
            {
                if (_controls != null)
                {
                    foreach (var control in _controls)
                    {
                        control.OnBeforeSetLayout();
                    }
                }
            }
        }

        /// <summary>
        /// Calls OnAfterLayout on all controls/markers.
        /// </summary>
        internal void NotifyOnAfterSetLayout()
        {
            lock (_markers)
            {
                if (_markers != null)
                {
                    foreach (var marker in _markers)
                    {
                        marker.OnAfterSetLayout();
                    }
                }
            }
            lock (_controls)
            {
                if (_controls != null)
                {
                    foreach (var control in _controls)
                    {
                        control.OnAfterSetLayout();
                    }
                }
            }
        }

        /// <summary>
        /// Notifies controls that there was a map tap.
        /// </summary>
        /// <remarks>>This is used to close popups on markers when the map is tapped.</remarks>
        internal void NotifyMapTapToControls() 
        {
            foreach (var marker in _markers)
            {
                marker.NotifyMapTap();
            }
            foreach (var control in _controls)
            {
                control.NotifyMapTap();
            }
        }

        /// <summary>
        /// Notifies this host that the control was clicked.
        /// </summary>
        /// <param name="clickedControl">Control.</param>
        public void NotifyControlClicked(MapControl clickedControl)
        { // make sure to close all other popups.
            foreach (var marker in _markers)
            {
                if (marker != clickedControl)
                {
                    marker.NotifyOtherControlClicked();
                }
            }
            foreach (var control in _controls)
            {
                if (control != clickedControl)
                {
                    control.NotifyOtherControlClicked();
                }
            }
        }

        #endregion

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
        public float MapMinZoomLevel
        {
            get { return _mapView.MapMinZoomLevel; }
            set { _mapView.MapMinZoomLevel = value; }
        }

        /// <summary>
        /// Gets or sets the map max zoom level.
        /// </summary>
        /// <value>The map max zoom level.</value>
        public float MapMaxZoomLevel
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
        /// Gets or sets the bounding box within which one can pan the map.
        /// </summary>
        /// <value>The box.</value>
        public GeoCoordinateBox MapBoundingBox
        {
            get { return _mapView.MapBoundingBox; }
            set { _mapView.MapBoundingBox = value; }
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

        /// <summary>
        /// Returns the current width.
        /// </summary>
        public int CurrentWidth
        {
            get { return _mapView.Width; }
        }

        /// <summary>
        /// Returns the current height.
        /// </summary>
        public int CurrentHeight
        {
            get { return _mapView.Height; }
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
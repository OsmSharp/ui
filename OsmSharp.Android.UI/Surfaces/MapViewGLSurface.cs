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

using Android.App;
using Android.Content;
using Android.Opengl;
using Android.Util;
using Android.Views;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.UI;
using OsmSharp.UI.Animations;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Units.Angle;
using System.Collections.Generic;

namespace OsmSharp.Android.UI
{
    /// <summary>
    /// Map view surface.
    /// </summary>
    public class MapViewGLSurface : GLSurfaceView, IMapViewSurface,
            ScaleGestureDetector.IOnScaleGestureListener,
            RotateGestureDetector.IOnRotateGestureListener,
            MoveGestureDetector.IOnMoveGestureListener,
            TapGestureDetector.IOnTapGestureListener,
            global::Android.Views.View.IOnTouchListener
    {
        private bool _invertX = false;
        private bool _invertY = false;

        /// <summary>
        /// Holds the primitives layer.
        /// </summary>
        private LayerPrimitives _makerLayer;

        /// <summary>
        /// Holds the scale gesture detector.
        /// </summary>
        private ScaleGestureDetector _scaleGestureDetector;

        /// <summary>
        /// Holds the rotation gesture detector.
        /// </summary>
        private RotateGestureDetector _rotateGestureDetector;

        /// <summary>
        /// Holds the move gesture detector.
        /// </summary>
        private MoveGestureDetector _moveGestureDetector;

        /// <summary>
        /// Holds the tag gesture detector.
        /// </summary>
        private TapGestureDetector _tagGestureDetector;

        /// <summary>
        /// Holds the maplayout.
        /// </summary>
        private MapView _mapView;

        /// <summary>
        /// Holds the Open GL 2D Target.
        /// </summary>
        private OpenGLTarget2D _target;

        /// <summary>
        /// Holds the map renderer.
        /// </summary>
        private MapRenderer<OpenGLTarget2D> _renderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapViewSurface"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public MapViewGLSurface(Context context) :
            base(context)
        {
            this.MapAllowPan = true;
            this.MapAllowTilt = true;
            this.MapAllowZoom = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapView"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        public MapViewGLSurface(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            this.MapAllowPan = true;
            this.MapAllowTilt = true;
            this.MapAllowZoom = true;
        }

        /// <summary>
        /// Initialize implementation from IMapView.
        /// </summary>
        /// <param name="mapLayout"></param>
        void IMapViewSurface.Initialize(MapView mapLayout)
        {                        
            // create the Open GL 2D target.
            _target = new OpenGLTarget2D();
            this.SetRenderer(_target);      

            _mapView = mapLayout;
            this.SetWillNotDraw(false);

            this.MapMinZoomLevel = 10;
            this.MapMaxZoomLevel = 20;

            // create the renderer.
            _renderer = new MapRenderer<OpenGLTarget2D>(
                    new OpenGLRenderer2D());

            // initialize the gesture detection.
            this.SetOnTouchListener(this);
            _scaleGestureDetector = new ScaleGestureDetector(
                this.Context, this);
            _rotateGestureDetector = new RotateGestureDetector(
                this.Context, this);
            _moveGestureDetector = new MoveGestureDetector(
                this.Context, this);
            _tagGestureDetector = new TapGestureDetector(
                this.Context, this);

            _makerLayer = new LayerPrimitives(
                new WebMercator());

            _scene = new Scene2D(new WebMercator(), 16);
            _scene.BackColor = SimpleColor.FromKnownColor(KnownColor.White).Value;
        }

        /// <summary>
        /// Notifies change.
        /// </summary>
        public void TriggerRendering()
        {

        }

        /// <summary>
        /// Notifies change.
        /// </summary>
        public void TriggerRendering(bool force)
        {
            // notify map layout of changes.
            if (this.Width > 0 && this.Height > 0)
            {
                // create the view.
                var view = _renderer.Create(this.Width, this.Height,
                    this.Map, (float)this.Map.Projection.ToZoomFactor(this.MapZoom),
                                         this.MapCenter, false, true);

                // notify the map that the view has changed.
                this.Map.ViewChanged((float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter,
                             view, view);

                _mapView.NotifyMapChange(this.Width, this.Height, view, this.Map.Projection);

                this.SetOrtho(view);

                // build the layers list.
                var layers = new List<Layer>();
                for (int layerIdx = 0; layerIdx < this.Map.LayerCount; layerIdx++)
                {
                    if (this.Map[layerIdx].IsVisible)
                    {
                        // get the layer.
                        layers.Add(this.Map[layerIdx]);
                    }
                }

                _renderer.Render(_target,
                     layers,
                     view,
                     (float)this.Map.Projection.ToZoomFactor(this.MapZoom));
            }
        }

        /// <summary>
        /// Holds the cached scene.
        /// </summary>
        private Scene2D _scene;

        /// <summary>
        /// The map center.
        /// </summary>
        private GeoCoordinate _mapCenter;

        /// <summary>
        /// The map center on the previous rendering.
        /// </summary>
        private GeoCoordinate _previousRenderingMapCenter;

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        /// <value>The center.</value>
        public GeoCoordinate MapCenter
        {
            get
            {
                return _mapCenter;
            }
            set
            {
                _mapCenter = value;
                //(this.Context as Activity).RunOnUiThread(Invalidate);
                //if (_autoInvalidate)
                //{
                //    if (_previousRenderingMapCenter == null ||
                //        _previousRenderingMapCenter.DistanceReal(_mapCenter).Value > 20)
                //    { // TODO: update this with a more resonable measure depending on the zoom.
                //        this.Change();
                //        _previousRenderingMapCenter = _mapCenter;
                //    }
                //}
            }
        }

        /// <summary>
        /// Holds the map.
        /// </summary>
        private Map _map;

        /// <summary>
        /// Gets or sets the map.
        /// </summary>
        /// <value>The map.</value>
        public Map Map
        {
            get { return _map; }
            set
            {
                _map = value;
                //_map.MapChanged += delegate()
                //{
                //    this.Change();
                //};
            }
        }

        /// <summary>
        /// Holds the map tilte angle.
        /// </summary>
        private Degree _mapTilt;

        /// <summary>
        /// Gets or sets the map tilt.
        /// </summary>
        /// <value>The map tilt.</value>
        public Degree MapTilt
        {
            get
            {
                return _mapTilt;
            }
            set
            {
                _mapTilt = value;

                //if (_autoInvalidate)
                //{
                //    (this.Context as Activity).RunOnUiThread(Invalidate);
                //}
            }
        }

        /// <summary>
        /// Holds the map zoom level.
        /// </summary>
        private float _mapZoomLevel;

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <value>The zoom factor.</value>
        public float MapZoom
        {
            get { return _mapZoomLevel; }
            set
            {
                if (this.MapMaxZoomLevel.HasValue &&
                    value > this.MapMaxZoomLevel)
                {
                    _mapZoomLevel = this.MapMaxZoomLevel.Value;
                }
                else if (this.MapMinZoomLevel.HasValue &&
                  value < this.MapMinZoomLevel)
                {
                    _mapZoomLevel = this.MapMinZoomLevel.Value;
                }
                else
                {
                    _mapZoomLevel = value;
                }

                //if (_autoInvalidate)
                //{
                //    (this.Context as Activity).RunOnUiThread(Invalidate);
                //}
            }
        }

        /// <summary>
        /// Gets or sets the map max zoom level.
        /// </summary>
        /// <value>The map max zoom level.</value>
        public float? MapMaxZoomLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map minimum zoom level.
        /// </summary>
        /// <value>The map minimum zoom level.</value>
        public float? MapMinZoomLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map tilt flag.
        /// </summary>
        public bool MapAllowTilt
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map pan flag.
        /// </summary>
        public bool MapAllowPan
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map zoom flag.
        /// </summary>
        public bool MapAllowZoom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map scale factor.
        /// </summary>
        public float MapScaleFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        public View2D CurrentView
        {
            get { return this.CreateView(); }
        }

        /// <summary>
        /// Returns the density.
        /// </summary>
        public float Density
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Raises the layout event.
        /// </summary>
        /// <param name="changed">If set to <c>true</c> changed.</param>
        /// <param name="left">Left.</param>
        ///( <param name="top">Top.</param>
        /// <param name="right">Right.</param>
        /// <param name="bottom">Bottom.</param>
        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            _target.Width = this.Width;
            _target.Height = this.Height;

            // create the view.
            View2D view = _renderer.Create(this.Width, this.Height,
                this.Map, (float)this.Map.Projection.ToZoomFactor(this.MapZoom),
                                     this.MapCenter, false, true);

            this.SetOrtho(view);

            // notify the map that the view has changed.
            this.Map.ViewChanged((float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter,
                         view, view);

            // build the layers list.
            var layers = new List<Layer>();
            for (int layerIdx = 0; layerIdx < this.Map.LayerCount; layerIdx++)
            {
                if (this.Map[layerIdx].IsVisible)
                {
                    // get the layer.
                    layers.Add(this.Map[layerIdx]);
                }
            }

            _renderer.Render(_target,
                 layers,
                 view,
                 (float)this.Map.Projection.ToZoomFactor(this.MapZoom));
        }

        /// <summary>
        /// Creates a view.
        /// </summary>
        /// <returns></returns>
        public View2D CreateView()
        {
            float height = this.LayoutParameters.Height;
            float width = this.LayoutParameters.Width;

            // calculate the center/zoom in scene coordinates.
            double[] sceneCenter = this.Map.Projection.ToPixel(this.MapCenter.Latitude, this.MapCenter.Longitude);
            float sceneZoomFactor = (float)this.Map.Projection.ToZoomFactor(this.MapZoom);

            // create the view for this control.
            return View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
                                     this.Width, this.Height, sceneZoomFactor,
                                     _invertX, _invertY, this.MapTilt);
        }

        private void SetOrtho(View2D view)
        {
            // TODO: create rotation.
            _target.SetOrtho((float)view.LeftTop[0], (float)view.RightTop[0],
                (float)view.LeftTop[1], (float)view.RightBottom[1]);
        }

        /// <summary>
        /// Notifies that there was movement.
        /// </summary>
        private void NotifyMovement()
        {
            //// invalidate the current view.
            //this.Invalidate();

            // notify map layout of changes.
            if (this.Width > 0 && this.Height > 0)
            {
                // create the view.
                View2D view = _renderer.Create(this.Width, this.Height,
                    this.Map, (float)this.Map.Projection.ToZoomFactor(this.MapZoom),
                                         this.MapCenter, false, true);

                _mapView.NotifyMapChange(this.Width, this.Height, view, this.Map.Projection);

                this.SetOrtho(view);
                //// build the layers list.
                //var layers = new List<ILayer>();
                //for (int layerIdx = 0; layerIdx < this.Map.LayerCount; layerIdx++)
                //{
                //    // get the layer.
                //    layers.Add(this.Map[layerIdx]);
                //}

                //_renderer.Render(_target,
                //     layers,
                //     view);
            }
        }

        private double _deltaScale = 1.0f;
        private double _deltaDegrees = 0.0f;

        private double _deltaX = 0.0f;
        private double _deltaY = 0.0f;

        #region IOnScaleGestureListener implementation

        /// <summary>
        /// Raises the scale event.
        /// </summary>
        /// <param name="detector">Detector.</param>
        public bool OnScale(ScaleGestureDetector detector)
        {
            _deltaScale = detector.ScaleFactor;
            return true;
        }

        /// <summary>
        /// Raises the scale begin event.
        /// </summary>
        /// <param name="detector">Detector.</param>
        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            _deltaScale = 1;
            _deltaDegrees = 0;
            _deltaX = 0;
            _deltaY = 0;

            return true;
        }

        /// <summary>
        /// Raises the scale end event.
        /// </summary>
        /// <param name="detector">Detector.</param>
        public void OnScaleEnd(ScaleGestureDetector detector)
        {
            _deltaScale = 1;
        }

        #endregion

        #region IOnRotateGestureListener implementation

        public bool OnRotate(RotateGestureDetector detector)
        {
            _deltaDegrees = detector.RotationDegreesDelta;

            OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", TraceEventType.Information,
                string.Format("OnRotate:[{0},{1}] {2}s {3}d", _deltaX, _deltaY, _deltaScale, _deltaDegrees));

            return true;
        }

        public bool OnRotateBegin(RotateGestureDetector detector)
        {
            _deltaScale = 1;
            _deltaDegrees = 0;
            _deltaX = 0;
            _deltaY = 0;

            OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", TraceEventType.Information,
                string.Format("OnRotateBegin"));

            return true;
        }

        public void OnRotateEnd(RotateGestureDetector detector)
        {
            _deltaDegrees = 0;

            OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", TraceEventType.Information,
                string.Format("OnRotateEnd"));
        }

        #endregion

        #region IOnMoveGestureListener implementation

        public bool OnMove(MoveGestureDetector detector)
        {
            global::Android.Graphics.PointF d = detector.FocusDelta;
            _deltaX = d.X;
            _deltaY = d.Y;

            return true;
        }

        public bool OnMoveBegin(MoveGestureDetector detector)
        {
            _deltaScale = 1;
            _deltaDegrees = 0;
            _deltaX = 0;
            _deltaY = 0;

            return true;
        }

        public void OnMoveEnd(MoveGestureDetector detector)
        {
            this.TriggerRendering();
        }

        #endregion

        #region IOnTapGestureListener implementation

        /// <summary>
        /// Called when a tab is detected.
        /// </summary>
        /// <param name="detector"></param>
        /// <returns></returns>
        public bool OnTap(TapGestureDetector detector)
        {
            // recreate the view.
            View2D view = this.CreateView();

            // calculate the new center in pixels.
            double x = detector.X;
            double y = detector.Y;

            // calculate the new center from the view.
            double[] sceneCenter = view.FromViewPort(this.Width, this.Height,
                                                      x, y);

            // convert to the projected center.
            _mapView.RaiseMapTapEvent(this.Map.Projection.ToGeoCoordinates(sceneCenter[0], sceneCenter[1]));

            return true;
        }

        #endregion

        /// <summary>
        /// Raises the touch event event.
        /// </summary>
        /// <param name="e">E.</param>
        public override bool OnTouchEvent(MotionEvent e)
        {
            return true;
        }

        #region IOnTouchListener implementation

        /// <summary>
        /// Raises the touch event.
        /// </summary>
        /// <param name="v">V.</param>
        /// <param name="e">E.</param>
        public bool OnTouch(global::Android.Views.View v, MotionEvent e)
        {
            _tagGestureDetector.OnTouchEvent(e);
            _scaleGestureDetector.OnTouchEvent(e);
            _rotateGestureDetector.OnTouchEvent(e);
            _moveGestureDetector.OnTouchEvent(e);

            if (_deltaX != 0 || _deltaY != 0 || // was there movement?
                _deltaScale != 1.0 || // was there scale?
                _deltaDegrees != 0)
            { // was there rotation?
                if (this.MapAllowZoom &&
                    _deltaScale != 1.0)
                {
                    // calculate the scale.
                    double zoomFactor = this.Map.Projection.ToZoomFactor(this.MapZoom);
                    zoomFactor = zoomFactor * _deltaScale;
                    this.MapZoom = (float)this.Map.Projection.ToZoomLevel(zoomFactor);
                }

                if (this.MapAllowPan)
                {
                    // stop the animation.
                    this.StopCurrentAnimation();

                    //OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,
                    //    string.Format("OnTouch:[{0},{1}] {2}s {3}d", _deltaX, _deltaY, _deltaScale, _deltaDegrees));

                    // recreate the view.
                    View2D view = this.CreateView();

                    // calculate the new center in pixels.
                    double centerXPixels = this.Width / 2.0f - _deltaX;
                    double centerYPixles = this.Height / 2.0f - _deltaY;

                    // calculate the new center from the view.
                    double[] sceneCenter = view.FromViewPort(this.Width, this.Height,
                                                              centerXPixels, centerYPixles);

                    // convert to the projected center.
                    this.MapCenter = this.Map.Projection.ToGeoCoordinates(sceneCenter[0], sceneCenter[1]);
                }

                // do the rotation stuff around the new center.
                if (this.MapAllowTilt &&
                    _deltaDegrees != 0)
                {
                    // recreate the view.
                    View2D view = this.CreateView();

                    View2D rotatedView = view.RotateAroundCenter((Degree)(-_deltaDegrees));
                    _mapTilt = (float)((Degree)rotatedView.Rectangle.Angle).Value;
                }

                _deltaScale = 1;
                _deltaDegrees = 0;
                _deltaX = 0;
                _deltaY = 0;

                // notify touch.
                if (this.MapAllowPan || this.MapAllowTilt || this.MapAllowZoom)
                {
                    this.NotifyMovement();
                }
            }
            return true;
        }

        #endregion

        /// <summary>
        /// Holds the map view animator.
        /// </summary>
        private MapViewAnimator _mapViewAnimator;

        /// <summary>
        /// Holds the auto invalidate flag.
        /// </summary>
        private bool _autoInvalidate = false;

        /// <summary>
        /// Stops the current animation.
        /// </summary>
        private void StopCurrentAnimation()
        {
            if (_mapViewAnimator != null)
            {
                _mapViewAnimator.Stop();
            }
        }

        /// <summary>
        /// Registers the animator.
        /// </summary>
        /// <param name="mapViewAnimator">Map view animator.</param>
		public void RegisterAnimator(MapViewAnimator mapViewAnimator)
        {
            _mapViewAnimator = mapViewAnimator;
        }

        /// <summary>
        /// Sets the map view.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="mapTilt">Map tilt.</param>
        /// <param name="mapZoom">Map zoom.</param>
        public void SetMapView(GeoCoordinate center, Degree mapTilt, float mapZoom)
        {
            _mapCenter = center;
            _mapTilt = mapTilt;
            _mapZoomLevel = mapZoom;

            (this.Context as Activity).RunOnUiThread(Invalidate);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OsmSharp.Android.UI.MapViewSurface"/> auto invalidate.
        /// </summary>
        /// <value><c>true</c> if auto invalidate; otherwise, <c>false</c>.</value>
        public bool AutoInvalidate
        {
            get
            {
                return _autoInvalidate;
            }
            set
            {
                _autoInvalidate = value;
            }
        }

        /// <summary>
        /// Zooms to the given list of markers.
        /// </summary>
        /// <param name="markers"></param>
        public void ZoomToMarkers(List<MapMarker> markers, double percentage)
        {
            float height = this.LayoutParameters.Height;
            float width = this.LayoutParameters.Width;
            if (width > 0)
            {
                PointF2D[] points = new PointF2D[markers.Count];
                for (int idx = 0; idx < markers.Count; idx++)
                {
                    points[idx] = markers[idx].Location;
                }
                View2D view = this.CreateView();
                View2D fittedView = view.Fit(points);

                float zoom = (float)this.Map.Projection.ToZoomLevel(fittedView.CalculateZoom(
                    width, height));
                GeoCoordinate center = this.Map.Projection.ToGeoCoordinates(
                    view.Center[0], view.Center[1]);

                (this as IMapView).SetMapView(center, this.MapTilt, zoom);
            }
        }

        /// <summary>
        /// Pauses all ongoing activity in this MapViewSurface.
        /// </summary>
        public void Pause()
        {
            // no pausing the OpenGL-stuff just yet!
        }

        /// <summary>
        /// Returns true if this activity is paused.
        /// </summary>
        /// <returns></returns>
        public bool IsPaused
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Resumes all activity in this MapViewSurface.
        /// </summary>
        public void Resume()
        {
            // no resuming the OpenGL-stuff just yet!
        }

        /// <summary>
        /// Closes this surface.
        /// </summary>
        public void Close()
        {
            // no closing the OpenGL-stuff just yet!
        }
    }
}
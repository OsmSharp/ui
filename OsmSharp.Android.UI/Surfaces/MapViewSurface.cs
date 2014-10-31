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
using Android.Runtime;
using Android.Util;
using Android.Views;
using OsmSharp.Android.UI.Controls;
using OsmSharp.Android.UI.Renderer.Images;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.UI;
using OsmSharp.UI.Animations;
using OsmSharp.UI.Animations.Invalidation.Triggers;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OsmSharp.Android.UI
{
    /// <summary>
    /// Map view surface.
    /// </summary>
    public class MapViewSurface : View, IMapViewSurface,
            ScaleGestureDetector.IOnScaleGestureListener,
            RotateGestureDetector.IOnRotateGestureListener,
            MoveGestureDetector.IOnMoveGestureListener,
            TapGestureDetector.IOnTapGestureListener,
            global::Android.Views.View.IOnTouchListener,
            IInvalidatableMapSurface
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
        /// Holds the map-layout.
        /// </summary>
        private MapView _mapView;

        /// <summary>
        /// Holds the density factor.
        /// </summary>
        private float _density;

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapViewSurface"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public MapViewSurface(Context context) :
            base(context)
        {
            // do not do too much here could cause https://github.com/OsmSharp/OsmSharp/issues/129
            // just wait for a C# reference with patience and initialize stuff in IMapViewSurface.Initialize()
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapViewSurface"/> class.
        /// </summary>
        /// <param name="javaReference"></param>
        /// <param name="transfer"></param>
        /// <remarks>Fixes an issue in Xamarin (leaky abstraction): </remarks>
        public MapViewSurface(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            OsmSharp.Logging.Log.TraceEvent("MapView.MapViewSurface(IntPtr javaReference, JniHandleOwnership transfer)", TraceEventType.Warning,
                "A call to the MapViewSurfaceContructor occured: check https://github.com/OsmSharp/OsmSharp/issues/129");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapView"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="attrs">Attrs.</param>
        public MapViewSurface(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            // do not do too much here could cause https://github.com/OsmSharp/OsmSharp/issues/129
            // just wait for a C# reference with patience and initialize stuff in IMapViewSurface.Initialize()
        }

        /// <summary>
        /// Initialize implementation from IMapView.
        /// </summary>
        /// <param name="mapLayout"></param>
        void IMapViewSurface.Initialize(MapView mapLayout)
        {
            this.MapAllowPan = true;
            this.MapAllowTilt = true;
            this.MapAllowZoom = true;

            // register default invalidation trigger.
            (this as IInvalidatableMapSurface).RegisterListener(new DefaultTrigger(this));

            _mapView = mapLayout;
            this.SetWillNotDraw(false);
            // this.SetWillNotCacheDrawing(true);

            this.MapMinZoomLevel = 0;
            this.MapMaxZoomLevel = 20;

            // gets the system density.
            _density = global::Android.Content.Res.Resources.System.DisplayMetrics.Density;
            _bufferFactor = _density; // set default scale factor relative to density.

            // create the renderer.
            _renderer = new MapRenderer<global::Android.Graphics.Canvas>(
                new CanvasRenderer2D(1));

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

            // initialize all the caching stuff.
            _backgroundColor = SimpleColor.FromKnownColor(KnownColor.White).Value;
            _cacheRenderer = new MapRenderer<global::Android.Graphics.Canvas>(
                new CanvasRenderer2D(1));
        }

        /// <summary>
        /// Suspended rendering.
        /// </summary>
        private bool _renderingSuspended = false;

        /// <summary>
        /// Holds the off screen buffered image.
        /// </summary>
        private ImageTilted2D _offScreenBuffer;

        /// <summary>
        /// Holds the on screen buffered image.
        /// </summary>
        private ImageTilted2D _onScreenBuffer;

        /// <summary>
        /// Holds the background color.
        /// </summary>
        private int _backgroundColor;

        /// <summary>
        /// Holds the cache renderer.
        /// </summary>
        private MapRenderer<global::Android.Graphics.Canvas> _cacheRenderer;

        /// <summary>
        /// Holds the rendering thread.
        /// </summary>
        private Thread _renderingThread;

        /// <summary>
        /// Holds the extra parameter.
        /// </summary>
        private float _extra = 1.25f;

        /// <summary>
        /// Holds the scale factor to 'increase/decrease' size of objects rendered. (1 = unscaled, 3 = 3 x bigger)
        /// </summary>
        private float _bufferFactor = 1.0f;

        /// <summary>
        /// Triggers rendering.
        /// </summary>
        public void TriggerRendering()
        {
            this.TriggerRendering(false);
        }

        /// <summary>
        /// Triggers rendering.
        /// </summary>
        public void TriggerRendering(bool force)
        {
            try
            {
                if (this.SurfaceWidth == 0)
                { // nothing to render yet!
                    return;
                }

                // create the view that would be use for rendering.
                View2D view = _cacheRenderer.Create((int)(this.SurfaceWidth * _extra), (int)(this.SurfaceHeight * _extra),
                    this.Map, (float)this.Map.Projection.ToZoomFactor(this.MapZoom),
                    this.MapCenter, _invertX, _invertY, this.MapTilt);

                // ... and compare to the previous rendered view.
                if (!force && _previouslyRenderedView != null &&
                    view.Equals(_previouslyRenderedView))
                {
                    return;
                }
                _previouslyRenderedView = view;

                // end existing rendering thread.
                if (_renderingThread != null &&
                    _renderingThread.IsAlive)
                {
                    if (_cacheRenderer.IsRunning)
                    {
                        this.Map.ViewChangedCancel();
                        _cacheRenderer.CancelAndWait();
                    }
                }

                // start new rendering thread.
                _renderingThread = new Thread(new ThreadStart(Render));
                _renderingThread.Start();
            }
            catch (Exception ex)
            {
                OsmSharp.Logging.Log.TraceEvent("MapViewSurface", TraceEventType.Critical,
                    string.Format("An unhandled exception occured:{0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Stops the current rendering if in progress.
        /// </summary>
        internal void StopRendering()
        {
            // stop current rendering.
            if (_renderingThread != null &&
                _renderingThread.IsAlive)
            { // a rendering thread is alive.
                if (_cacheRenderer != null &&
                    _cacheRenderer.IsRunning)
                { // there is a running cache renderer and thus there is something to cancel.
                    this.Map.ViewChangedCancel();
                    _cacheRenderer.CancelAndWait();
                }
            }
        }

        /// <summary>
        /// Holds the previous rendered zoom.
        /// </summary>
        private View2D _previouslyRenderedView;

        /// <summary>
        /// Holds the perviously triggered view.
        /// </summary>
        private View2D _previouslyChangedView;

        private float _surfaceWidth = 0;

        /// <summary>
        /// Returns the width of this rendering surface.
        /// </summary>
        private float SurfaceWidth
        {
            get
            {
                return _surfaceWidth;
            }
        }

        private float _surfaceHeight = 0;

        /// <summary>
        /// Returns the height of this rendering surface.
        /// </summary>
        private float SurfaceHeight
        {
            get
            {
                return _surfaceHeight;
            }
        }

        /// <summary>
        /// Renders the current complete scene.
        /// </summary>
        private void Render()
        {
            try
            {
                if (_renderingSuspended)
                { // no rendering when rendering is suspended.
                    return;
                }

                if (_cacheRenderer.IsRunning)
                { // cancel previous render.
                    _cacheRenderer.CancelAndWait();
                }

                // make sure only on thread at the same time is using the renderer.
                lock (_cacheRenderer)
                {
                    this.Map.ViewChangedCancel();

                    // build the layers list.
                    var layers = new List<Layer>();
                    for (int layerIdx = 0; layerIdx < this.Map.LayerCount; layerIdx++)
                    { // get the layer.
                        if (this.Map[layerIdx].IsVisible)
                        {
                            layers.Add(this.Map[layerIdx]);
                        }
                    }

                    // add the internal layers.
                    layers.Add(_makerLayer);

                    if (this.SurfaceHeight == 0)
                    { // the surface has no height yet. Impossible to render like this.
                        return;
                    }

                    // get old image if available.
                    NativeImage image = null;
                    if (_offScreenBuffer != null)
                    { // get the native image from the off-screen buffer.
                        image = _offScreenBuffer.NativeImage as NativeImage;
                    }

                    // resize image if needed.
                    float sizeX = this.SurfaceWidth;
                    float sizeY = this.SurfaceHeight;
                    //if(this.MapAllowTilt)
                    //{ // when rotation is allowed make sure a square is rendered.
                    //    sizeX = System.Math.Max(this.SurfaceWidth, this.SurfaceHeight);
                    //    sizeY = System.Math.Max(this.SurfaceWidth, this.SurfaceHeight);
                    //}
                    // float size = System.Math.Max(this.SurfaceHeight, this.SurfaceWidth);
                    if (image == null ||
                        image.Image.Width != (int)(sizeX * _extra) ||
                        image.Image.Height != (int)(sizeY * _extra))
                    { // create a bitmap and render there.
                        if (image != null)
                        { // make sure to dispose the old image.
                            image.Dispose();
                        }
                        image = new NativeImage(global::Android.Graphics.Bitmap.CreateBitmap((int)(sizeX * _extra),
                            (int)(sizeY * _extra),
                            global::Android.Graphics.Bitmap.Config.Argb8888));
                    }

                    // create and reset the canvas.
                    using (var canvas = new global::Android.Graphics.Canvas(image.Image))
                    {
                        canvas.DrawColor(new global::Android.Graphics.Color(
                            SimpleColor.FromKnownColor(KnownColor.White).Value));

                        // create the view.
                        double[] sceneCenter = this.Map.Projection.ToPixel(this.MapCenter.Latitude, this.MapCenter.Longitude);
                        float mapZoom = this.MapZoom;
                        float sceneZoomFactor = (float)this.Map.Projection.ToZoomFactor(this.MapZoom);

                        // create the view for this control.
                        float scaledNormalWidth = image.Image.Width / _bufferFactor;
                        float scaledNormalHeight = image.Image.Height / _bufferFactor;
                        var view = View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
                                                 scaledNormalWidth * _extra, scaledNormalHeight * _extra, sceneZoomFactor,
                                                 _invertX, _invertY, this.MapTilt);

                        long before = DateTime.Now.Ticks;

                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", TraceEventType.Information,
                                                        "Rendering Start");

                        // notify the map that the view has changed.
                        if (_previouslyChangedView == null ||
                            !_previouslyChangedView.Equals(view))
                        { // report change once!
                            var normalView = View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
                                scaledNormalWidth, scaledNormalHeight, sceneZoomFactor,
                                _invertX, _invertY, this.MapTilt);
                            this.Map.ViewChanged((float)this.Map.Projection.ToZoomFactor(this.MapZoom), this.MapCenter,
                                                  normalView, view);
                            _previouslyChangedView = view;
                            long afterViewChanged = DateTime.Now.Ticks;
                            OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", TraceEventType.Information,
                                                            "View change took: {0}ms @ zoom level {1}",
                                                            (new TimeSpan(afterViewChanged - before).TotalMilliseconds), this.MapZoom);
                        }

                        // does the rendering.
                        _cacheRenderer.Density = this.MapScaleFactor;
                        bool complete = _cacheRenderer.Render(canvas, _map.Projection, layers, view, (float)this.Map.Projection.ToZoomFactor(this.MapZoom));

                        long afterRendering = DateTime.Now.Ticks;
                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", TraceEventType.Information,
                                                        "Rendering took: {0}ms @ zoom level {1} and {2}",
                                                        (new TimeSpan(afterRendering - before).TotalMilliseconds), this.MapZoom, this.MapCenter);
                        if (complete)
                        { // there was no cancellation, the rendering completely finished.
                            // add the result to the scene cache.
                            // add the newly rendered image again.           
                            if (_offScreenBuffer == null)
                            { // create the offscreen buffer first.
                                _offScreenBuffer = new ImageTilted2D(view.Rectangle, image, float.MinValue, float.MaxValue);
                            }
                            else
                            { // augment the previous buffer.
                                _offScreenBuffer.Bounds = view.Rectangle;
                                _offScreenBuffer.NativeImage = image;
                            }

                            var temp = _onScreenBuffer;
                            _onScreenBuffer = _offScreenBuffer;
                            _offScreenBuffer = temp;
                        }

                        long after = DateTime.Now.Ticks;

                        if (complete)
                        { // report a successful render to listener.
                            _listener.NotifyRenderSuccess(view, mapZoom, (int)new TimeSpan(after - before).TotalMilliseconds);
                        }
                    }
                }

                // notify the the current surface of the new rendering.
                this.PostInvalidate();
            }
            catch (Exception ex)
            { // exceptions can be thrown when the mapview is disposed while rendering.
                // don't worry too much about these, the mapview is garbage anyway.
                OsmSharp.Logging.Log.TraceEvent("MapViewSurface", TraceEventType.Critical,
                    string.Format("An unhandled exception occured:{0}", ex.ToString()));
            }
        }

        /// <summary>
        /// The map center.
        /// </summary>
        private GeoCoordinate _mapCenter;

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
                if (this.Width == 0 || this.MapBoundingBox == null)
                {
                    _mapCenter = value;
                }
                else
                {
                    _mapCenter = this.Map.EnsureViewWithinBoundingBox(value, this.MapBoundingBox, this.CurrentView);
                }

                _previouslyRenderedView = null;
                _previouslyChangedView = null;
                (this.Context as Activity).RunOnUiThread(NotifyMovement);
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
                if (_map != null)
                {
                    _map.MapChanged -= MapChanged;
                }
                _map = value;
                if (_map != null)
                {
                    _map.MapChanged += MapChanged;
                }

                _previouslyRenderedView = null;
                _previouslyChangedView = null;
            }
        }

        /// <summary>
        /// Called when the map reports it has changed.
        /// </summary>
        void MapChanged()
        {
            try
            {
                if (!_renderingSuspended)
                { // rendering is not suspended!
                    // notify map layout of changes.
                    if (this.SurfaceWidth > 0 && this.SurfaceHeight > 0)
                    {
                        // create the current view.
                        View2D view = this.CreateView();

                        // notify listener.
                        if (view != null)
                        { // only notify listener if there is a view.
                            _listener.NotifyChange(view, this.MapZoom);
                        }
                    }
                    _previouslyRenderedView = null;
                    _previouslyChangedView = null;
                }
            }
            catch (Exception ex)
            {
                OsmSharp.Logging.Log.TraceEvent("MapViewSurface", TraceEventType.Critical,
                    string.Format("An unhandled exception occured:{0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Holds the map tilt angle.
        /// </summary>
        private Degree _mapTilt;

        /// <summary>
        /// Gets or sets the map tilt.
        /// </summary>
        /// <value>The map tilt.</value>
        public Degree MapTilt
        {
            get { return _mapTilt; }
            set
            {
                _mapTilt = value;

                _previouslyRenderedView = null;
                _previouslyChangedView = null;
                (this.Context as Activity).RunOnUiThread(NotifyMovement);
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
                if (value > this.MapMaxZoomLevel)
                {
                    _mapZoomLevel = this.MapMaxZoomLevel;
                }
                else if (value < this.MapMinZoomLevel)
                {
                    _mapZoomLevel = this.MapMinZoomLevel;
                }
                else
                {
                    _mapZoomLevel = value;
                }

                _previouslyRenderedView = null;
                _previouslyChangedView = null;
                (this.Context as Activity).RunOnUiThread(NotifyMovement);
            }
        }

        /// <summary>
        /// Box within which one can pan the map
        /// </summary>
        private GeoCoordinateBox _mapBoundingBox = null;

        /// <summary>
        /// Gets or sets the bounding box within which one can pan the map.
        /// </summary>
        /// <value>The box.</value>
        public GeoCoordinateBox MapBoundingBox
        {
            get
            {
                return _mapBoundingBox;
            }
            set
            {
                // If the current map center falls outside the bounding box, set the MapCenter to the middle of the box.
                if (_mapCenter != null && !value.Contains(MapCenter))
                {
                    this.MapCenter = new GeoCoordinate(value.MinLat + 0.5f * value.DeltaLat, value.MinLon + 0.5f * value.DeltaLon);
                }
                _mapBoundingBox = value;
            }
        }

        /// <summary>
        /// Gets or sets the map max zoom level.
        /// </summary>
        /// <value>The map max zoom level.</value>
        public float MapMaxZoomLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map minimum zoom level.
        /// </summary>
        /// <value>The map minimum zoom level.</value>
        public float MapMinZoomLevel
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
            get { return _bufferFactor; }
            set { _bufferFactor = value; }
        }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        public View2D CurrentView
        {
            get { return this.CreateView(); }
        }

        /// <summary>
        /// Holds the renderer.
        /// </summary>
        private MapRenderer<global::Android.Graphics.Canvas> _renderer;

        /// <summary>
        /// Raises the draw event.
        /// </summary>
        /// <param name="canvas">Canvas.</param>
        protected override void OnDraw(global::Android.Graphics.Canvas canvas)
        {
            try
            {
                base.OnDraw(canvas);

                if (_renderingSuspended)
                { // do nothing when rendering is suspended.
                    return;
                }

                // set the height/width.
                if (_surfaceHeight != canvas.Height ||
                    _surfaceWidth != canvas.Width)
                {
                    _surfaceHeight = canvas.Height;
                    _surfaceWidth = canvas.Width;

                    // make sure markers are in the correct position too.
                    _mapView.NotifyMapChange(this.SurfaceWidth, this.SurfaceHeight, this.CreateView(), this.Map.Projection);

                    // trigger rendering.
                    this.TriggerRendering();

                    // raise map initialized.
                    _mapView.RaiseMapInitialized();
                }

                if (_onScreenBuffer != null)
                { // there is a buffer.
                    // render only the cached scene.
                    canvas.DrawColor(new global::Android.Graphics.Color(_backgroundColor));
                    View2D view = this.CreateView();
                    float zoomFactor = (float)this.Map.Projection.ToZoomFactor(this.MapZoom);
                    _renderer.SceneRenderer.Render(
                        canvas,
                        view,
                        zoomFactor,
                        new Primitive2D[] { _onScreenBuffer });
                }
            }
            catch (Exception ex)
            {
                OsmSharp.Logging.Log.TraceEvent("MapViewSurface", TraceEventType.Critical,
                    string.Format("An unhandled exception occured:{0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Creates a view.
        /// </summary>
        /// <returns></returns>
        public View2D CreateView()
        {
            try
            {
                if (this.Map != null && this.Map.Projection != null && this.MapCenter != null)
                { // only create a view when a map is set and a valid mapcenter is present.
                    float height = this.SurfaceHeight / _bufferFactor;
                    float width = this.SurfaceWidth / _bufferFactor;

                    // calculate the center/zoom in scene coordinates.
                    var sceneCenter = this.Map.Projection.ToPixel(this.MapCenter.Latitude, this.MapCenter.Longitude);
                    var sceneZoomFactor = (float)this.Map.Projection.ToZoomFactor(this.MapZoom);

                    // create the view for this control.
                    return View2D.CreateFrom((float)sceneCenter[0], (float)sceneCenter[1],
                                             width, height, sceneZoomFactor,
                                             _invertX, _invertY, this.MapTilt);
                }
            }
            catch (Exception ex)
            {
                OsmSharp.Logging.Log.TraceEvent("MapViewSurface.CreateView()", TraceEventType.Critical,
                    string.Format("An unhandled exception occured:{0}", ex.ToString()));
            }
            return null;
        }

        /// <summary>
        /// Raises the layout event.
        /// </summary>
        /// <param name="changed">If set to <c>true</c> changed.</param>
        /// <param name="left">Left.</param>
        /// <param name="top">Top.</param>
        /// <param name="right">Right.</param>
        /// <param name="bottom">Bottom.</param>
        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            // execute suspended events.
            if (_latestZoomCall != null)
            { // there was a suspended call.
                var latestZoomCall = _latestZoomCall;
                _latestZoomCall = null;
                this.ZoomToControls(
                                latestZoomCall.Controls,
                                latestZoomCall.Percentage);
            }

            if (_onScreenBuffer == null)
            {
                this.TriggerRendering(); // force a rendering on the first layout-event.
            }
        }

        /// <summary>
        /// Notifies that there was movement.
        /// </summary>
        private void NotifyMovement()
        {
            try
            {
                // invalidate the current view.
                this.Invalidate();

                // notify map layout of changes.
                if (this.SurfaceWidth > 0 && this.SurfaceHeight > 0)
                {
                    // create the current view.
                    View2D view = this.CreateView();

                    // notify map change to reposition markers.
                    _mapView.NotifyMapChange(this.SurfaceWidth, this.SurfaceHeight, view, this.Map.Projection);

                    // notify listener.
                    _listener.NotifyChange(view, this.MapZoom);
                }
            }
            catch (Exception ex)
            {
                OsmSharp.Logging.Log.TraceEvent("MapViewSurface", TraceEventType.Critical,
                    string.Format("An unhandled exception occured:{0}", ex.ToString()));
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

            return true;
        }

        public bool OnRotateBegin(RotateGestureDetector detector)
        {
            _deltaScale = 1;
            _deltaDegrees = 0;
            _deltaX = 0;
            _deltaY = 0;

            return true;
        }

        public void OnRotateEnd(RotateGestureDetector detector)
        {
            _deltaDegrees = 0;
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
            double[] sceneCenter = view.FromViewPort(this.SurfaceWidth, this.SurfaceHeight,
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
            try
            {
                if (!_renderingSuspended && this.Map != null && this.Map.Projection != null && this.MapCenter != null)
                {
                    var actionCode = e.Action & MotionEventActions.Mask;
                    switch (actionCode)
                    {
                        case MotionEventActions.Down:
                            _mapView.RaiseMapTouchedDown();
                            break;
                        case MotionEventActions.Up:
                            _mapView.RaiseMapTouchedUp();
                            break;
                    }

                    _tagGestureDetector.OnTouchEvent(e);
                    _scaleGestureDetector.OnTouchEvent(e);
                    _rotateGestureDetector.OnTouchEvent(e);
                    _moveGestureDetector.OnTouchEvent(e);

                    if (_deltaX != 0 || _deltaY != 0 || // was there movement?
                        _deltaScale != 1.0 || // was there scale?
                        _deltaDegrees != 0)
                    { // was there rotation?
                        bool movement = false;
                        if (this.MapAllowZoom &&
                            _deltaScale != 1.0)
                        {
                            // calculate the scale.
                            double zoomFactor = this.Map.Projection.ToZoomFactor(this.MapZoom);
                            zoomFactor = zoomFactor * _deltaScale;
                            this.MapZoom = (float)this.Map.Projection.ToZoomLevel(zoomFactor);

                            movement = true;
                        }

                        if (this.MapAllowPan)
                        {
                            // stop the animation.
                            this.StopCurrentAnimation();

                            // recreate the view.
                            View2D view = this.CreateView();

                            // calculate the new center in pixels.
                            double centerXPixels = this.SurfaceWidth / 2.0f - _deltaX;
                            double centerYPixles = this.SurfaceHeight / 2.0f - _deltaY;

                            // calculate the new center from the view.
                            double[] sceneCenter = view.FromViewPort(this.SurfaceWidth, this.SurfaceHeight,
                                                                      centerXPixels, centerYPixles);

                            // convert to the projected center.
                            this.MapCenter = this.Map.Projection.ToGeoCoordinates(sceneCenter[0], sceneCenter[1]);

                            movement = true;
                        }

                        // do the rotation stuff around the new center.
                        if (this.MapAllowTilt &&
                            _deltaDegrees != 0)
                        {
                            // recreate the view.
                            View2D view = this.CreateView();

                            View2D rotatedView = view.RotateAroundCenter((Degree)(-_deltaDegrees));
                            _mapTilt = (float)((Degree)rotatedView.Rectangle.Angle).Value;

                            movement = true;
                        }

                        _deltaScale = 1;
                        _deltaDegrees = 0;
                        _deltaX = 0;
                        _deltaY = 0;

                        // notify touch.
                        if (movement)
                        {
                            _mapView.RaiseMapTouched();
                            _mapView.RaiseMapMove();

                            this.NotifyMovement();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OsmSharp.Logging.Log.TraceEvent("MapViewSurface.OnTouch", TraceEventType.Critical,
                    string.Format("An unhandled exception occured:{0}", ex.ToString()));
            }
            return true;
        }

        #endregion

        /// <summary>
        /// Holds the map view animator.
        /// </summary>
        private MapViewAnimator _mapViewAnimator;

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
            this.MapCenter = center;
            this.MapTilt = mapTilt;
            this.MapZoom = mapZoom;

            (this.Context as Activity).RunOnUiThread(NotifyMovement);
        }

        /// <summary>
        /// Holds a suspended call to zoom to markers.
        /// </summary>
        private MapViewControlZoomEvent _latestZoomCall;

        /// <summary>
        /// Zooms to the given list of markers.
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="percentage"></param>
        public void ZoomToControls(List<MapControl> controls, double percentage)
        {
            try
            {
                float height = this.SurfaceHeight;
                float width = this.SurfaceWidth;
                if (width > 0)
                {
                    PointF2D[] points = new PointF2D[controls.Count];
                    for (int idx = 0; idx < controls.Count; idx++)
                    {
                        points[idx] = new PointF2D(this.Map.Projection.ToPixel(controls[idx].Location));
                    }
                    View2D view = this.CreateView();
                    View2D fittedView = view.Fit(points, percentage);

                    float zoom = (float)this.Map.Projection.ToZoomLevel(fittedView.CalculateZoom(
                                                 width, height));
                    GeoCoordinate center = this.Map.Projection.ToGeoCoordinates(
                                                           fittedView.Center[0], fittedView.Center[1]);

                    this.SetMapView(center, this.MapTilt, zoom);
                }
                else
                {
                    _latestZoomCall = new MapViewControlZoomEvent()
                    {
                        Controls = controls,
                        Percentage = percentage
                    };
                }
            }
            catch (Exception ex)
            {
                OsmSharp.Logging.Log.TraceEvent("MapViewSurface.ZoomToMarkers", TraceEventType.Critical,
                    string.Format("An unhandled exception occured:{0}", ex.ToString()));
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
                //someone wants the deterministic release of all resources
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
            this._cacheRenderer = null;
            if (this._offScreenBuffer != null)
            { // dispose of the map view surface.
                this._offScreenBuffer.Dispose();
                this._offScreenBuffer = null;
            }
            if (this._onScreenBuffer != null)
            { // dispose of the map view surface.
                this._onScreenBuffer.Dispose();
                this._onScreenBuffer = null;
            }
            if (this._mapViewAnimator != null)
            {
                _mapViewAnimator.Stop();
                _mapViewAnimator = null;
            }
            if (this._map != null)
            {
                this._map = null;
            }
        }

        private class MapViewControlZoomEvent
        {
            /// <summary>
            /// Gets or sets the controls.
            /// </summary>
            /// <value>The controls.</value>
            public List<MapControl> Controls
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the percentage.
            /// </summary>
            /// <value>The percentage.</value>
            public double Percentage
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Returns the density.
        /// </summary>
        public float Density
        {
            get
            {
                return _density;
            }
        }

        int IMapViewSurface.Width
        {
            get { return (int)this.SurfaceWidth; }
        }

        int IMapViewSurface.Height
        {
            get { return (int)this.SurfaceHeight; }
        }

        #region IInvalidatableMapSurface Interface

        /// <summary>
        /// Holds the trigger listener.
        /// </summary>
        private TriggerBase _listener;

        /// <summary>
        /// Returns true if this surface is sure that is going to keep moving.
        /// </summary>
        bool IInvalidatableMapSurface.StillMoving()
        {
            return _mapViewAnimator != null;
        }

        /// <summary>
        /// Triggers the rendering.
        /// </summary>
        void IInvalidatableMapSurface.Render()
        {
            this.TriggerRendering();
        }

        /// <summary>
        /// Cancels the current rendering.
        /// </summary>
        void IInvalidatableMapSurface.CancelRender()
        {
            this.StopRendering();
        }

        /// <summary>
        /// Registers an invalidation listener.
        /// </summary>
        /// <param name="listener"></param>
        void IInvalidatableMapSurface.RegisterListener(TriggerBase listener)
        {
            _listener = listener;
        }

        /// <summary>
        /// Unregisters the current listener.
        /// </summary>
        void IInvalidatableMapSurface.ResetListener()
        {
            _listener = null;
        }

        #endregion

        /// <summary>
        /// Pauses all ongoing activity in this MapViewSurface.
        /// </summary>
        public void Pause()
        {
            try
            {
                _listener.Stop();
                if (_mapViewAnimator != null)
                { // stop all ongoing animations.
                    _mapViewAnimator.Stop();
                }

                // suspend rendering.
                _renderingSuspended = true;

                // stop current rendering if any.
                this.StopRendering();
            }
            catch (Exception ex)
            {
                OsmSharp.Logging.Log.TraceEvent("MapViewSurface.Pause", TraceEventType.Critical,
                    string.Format("An unhandled exception occured:{0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Returns true if this activity is paused.
        /// </summary>
        /// <returns></returns>
        public bool IsPaused
        {
            get
            {
                return _renderingSuspended;
            }
        }

        /// <summary>
        /// Resumes all activity in this MapViewSurface.
        /// </summary>
        public void Resume()
        {
            try
            {
                // suspend rendering.
                _renderingSuspended = false;

                // stop current rendering if any.
                this.TriggerRendering(true);
            }
            catch (Exception ex)
            {
                OsmSharp.Logging.Log.TraceEvent("MapViewSurface.Resume", TraceEventType.Critical,
                    string.Format("An unhandled exception occured:{0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Closes this surface.
        /// </summary>
        public void Close()
        {
            try
            {
                _listener.Stop();
                if (_mapViewAnimator != null)
                { // stop all ongoing animations.
                    _mapViewAnimator.Stop();
                }
                // stop current rendering if any.
                this.StopRendering();
            }
            catch (Exception ex)
            {
                OsmSharp.Logging.Log.TraceEvent("MapViewSurface.Close", TraceEventType.Critical,
                    string.Format("An unhandled exception occured:{0}", ex.ToString()));
            }
        }
    }
}
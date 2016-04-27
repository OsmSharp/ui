using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI;
using OsmSharp.UI.Map;
using OsmSharp.UI.Renderer;
using OsmSharp.Units.Angle;
using OsmSharp.Wpf.UI.Renderer;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace OsmSharp.Wpf.UI
{
    /// <summary>
    /// A map control.
    /// </summary>
    public class MapControl : FrameworkElement, IMapView
    {
        #region fields

        /// <summary>
        /// An event is queued already.
        /// </summary>
        private bool _isQueued;
        private bool _isSuspendMapChanging;

        /// <summary>
        /// Holds the map renderer.
        /// </summary>
        private readonly MapRenderer<RenderContext> _renderer;

        /// <summary>
        /// Holds the map.
        /// </summary>
        private Map _map;

        /// <summary>
        /// Coordinates where dragging started.
        /// </summary>
        private Point? _draggingCoordinates;
        /// <summary>
        /// Coordinates of the old center.
        /// </summary>
        private GeoCoordinate _oldCenter;

        private bool _isRenderCachedScene;
        private BitmapSource _cachedScene;

        #endregion fields

        #region constructors

        /// <summary>
        /// Creates a new map control.
        /// </summary>
        public MapControl()
        {
            _renderer = new MapRenderer<RenderContext>(new DrawingRenderer2D());

            MapAllowPan = true;
            MapAllowZoom = true;

            //CacheMode = new BitmapCache
            //{
            //    EnableClearType = true,
            //    RenderAtScale = 8,
            //    SnapsToDevicePixels = true
            //};
        }

        #endregion constructors

        #region events

        /// <summary>
        /// Event raised when the map was first touched.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouchedDown;

        /// <summary>
        /// Event raised when the map is touched.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouched;

        /// <summary>
        /// Event raised after the map was touched.
        /// </summary>
        public event MapViewDelegates.MapTouchedDelegate MapTouchedUp;

        /// <summary>
        /// Raised when the map is moved.
        /// </summary>
        public event MapViewDelegates.MapMoveDelegate MapMove;

        /// <summary>
        /// Raised when the map was first initialized, meaning it has a size and it was rendered for the first time.
        /// </summary>
        public event MapViewDelegates.MapInitialized MapInitialized;

        #region MapMouseDown

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event EventHandler<MapControlEventArgs> MapMouseDown;

        /// <summary>
        /// Raises the OnMapMouseDown event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseDown(MouseEventArgs e)
        {
            if (Map != null)
            {
                var geoCoordinates = ToGeoCoordinates(e.GetPosition(this));
                var args = new MapControlEventArgs(e, geoCoordinates);
                OnMapMouseDown(args);
                MapMouseDown?.Invoke(this, args);
            }
        }
        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMapMouseDown(MapControlEventArgs e)
        {

        }

        #endregion

        #region MapMouseMove

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event EventHandler<MapControlEventArgs> MapMouseMove;

        /// <summary>
        /// Raises the OnMapMouseMove event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseMove(MouseEventArgs e)
        {
            if (Map != null)
            {
                var geoCoordinates = ToGeoCoordinates(e.GetPosition(this));
                var args = new MapControlEventArgs(e, geoCoordinates);
                OnMapMouseMove(args);
                MapMouseMove?.Invoke(this, args);
            }
        }
        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMapMouseMove(MapControlEventArgs e)
        {

        }

        #endregion

        #region MapMouseUp

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event EventHandler<MapControlEventArgs> MapMouseUp;

        /// <summary>
        /// Raises the OnMapMouseUp event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseUp(MouseEventArgs e)
        {
            if (Map != null)
            {
                var geoCoordinates = ToGeoCoordinates(e.GetPosition(this));
                var args = new MapControlEventArgs(e, geoCoordinates);
                OnMapMouseUp(args);
                MapMouseUp?.Invoke(this, args);
            }
        }
        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMapMouseUp(MapControlEventArgs e)
        {

        }

        #endregion

        #region MapMouseWheel

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event EventHandler<MapControlEventArgs> MapMouseWheel;

        /// <summary>
        /// Raises the OnMapMouseWheel event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseWheel(MouseEventArgs e)
        {
            if (Map != null)
            {
                var geoCoordinates = ToGeoCoordinates(e.GetPosition(this));
                var args = new MapControlEventArgs(e, geoCoordinates);
                OnMapMouseWheel(args);
                MapMouseWheel?.Invoke(this, args);
            }
        }
        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMapMouseWheel(MapControlEventArgs e)
        {

        }

        #endregion

        #region MapMouseDoubleClick

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event EventHandler<MapControlEventArgs> MapMouseDoubleClick;

        /// <summary>
        /// Raises the OnMapMouseDoubleClick event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseDoubleClick(MouseEventArgs e)
        {
            if (Map != null)
            {
                var geoCoordinates = ToGeoCoordinates(e.GetPosition(this));
                var args = new MapControlEventArgs(e, geoCoordinates);
                OnMapMouseDoubleClick(args);
                MapMouseDoubleClick?.Invoke(this, args);
            }
        }
        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMapMouseDoubleClick(MapControlEventArgs e)
        {

        }

        #endregion

        #region MapMouseClick

        /// <summary>
        /// The map mouse up event.
        /// </summary>
        public event EventHandler<MapControlEventArgs> MapMouseClick;

        /// <summary>
        /// Raises the OnMapMouseClick event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseOnMapMouseClick(MouseEventArgs e)
        {
            if (Map != null)
            {
                var geoCoordinates = ToGeoCoordinates(e.GetPosition(this));
                var args = new MapControlEventArgs(e, geoCoordinates);
                OnMapMouseClick(args);
                MapMouseClick?.Invoke(this, args);
            }
        }
        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMapMouseClick(MapControlEventArgs e)
        {

        }

        #endregion

        #endregion events

        #region propreties

        /// <summary>
        /// The center coordinates.
        /// </summary>
        public GeoCoordinate MapCenter { get; set; }
        /// <summary>
        /// The zoom factor.
        /// </summary>
        public float MapZoom { get; set; }
        /// <summary>
        /// Gets or sets the MapTilt.
        /// </summary>
        public Degree MapTilt { get; set; }

        /// <summary>
        /// Gets the current view.
        /// </summary>
        public View2D CurrentView => CreateSceneView(MapCenter, MapZoom);

        /// <summary>
        /// Returns the current ActualWidth.
        /// </summary>
        public int CurrentWidth => (int)System.Math.Ceiling(ActualWidth);
        /// <summary>
        /// Returns the current ActualHeight.
        /// </summary>
        public int CurrentHeight => (int)System.Math.Ceiling(ActualHeight);

        /// <summary>
        /// Gets/sets the map.
        /// </summary>
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
            }
        }

        /// <summary>
        /// Returns the density.
        /// </summary>
        public float Density => 1;

        /// <summary>
        /// Gets or sets the map bounding box.
        /// </summary>
        public GeoCoordinateBox MapBoundingBox { get; set; }
        /// <summary>
        /// Gets or sets the minimum zoom level.
        /// </summary>
        public float MapMinZoomLevel { get; set; }
        /// <summary>
        /// Gets or sets the maximum zoom level.
        /// </summary>
        public float MapMaxZoomLevel { get; set; }

        /// <summary>
        /// Gets or sets the map tilt flag.
        /// </summary>
        public bool MapAllowTilt
        {
            get { return false; }
            set
            {
                // no map tilt functionality.
            }
        }

        /// <summary>
        /// Gets or sets the map pan flag.
        /// </summary>
        public bool MapAllowPan { get; set; }
        /// <summary>
        /// Gets or sets the map zoom flag.
        /// </summary>
        public bool MapAllowZoom { get; set; }

        #endregion propreties

        #region utils

        private Rect BuildSceneRect()
        {
            var size = new Size(RenderSize.Width, RenderSize.Height);
            return new Rect(new Point(0, 0), size);
        }

        private View2D CreateSceneView(GeoCoordinate mapCenter, double mapZoom)
        {
            var sceneRect = BuildSceneRect();
            return Map != null
                ? _renderer.Create((float) sceneRect.Width, (float) sceneRect.Height, Map,
                    (float) Map.Projection.ToZoomFactor(mapZoom),
                    mapCenter, false, true)
                : null;
        }

        private void Refresh()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(Refresh, DispatcherPriority.Render);
                return;
            }

            InvalidateVisual();
        }

        private void MapChanged()
        {
            if (!_isSuspendMapChanging)
            {
                Refresh();
            }
        }
        private void SuspendMapChanging()
        {
            _isRenderCachedScene = true;
            _isSuspendMapChanging = true;
            Map?.Pause();
        }
        private void ResumeMapChanging()
        {
            _isRenderCachedScene = false;
            _isSuspendMapChanging = false;
            Map?.Resume();
        }

        private void QueueNotifyMapViewChanged()
        {
            _isQueued = true;
        }
        private void DeQueueNotifyMapViewChanged(object sender, ElapsedEventArgs e)
        {
            if (_isQueued)
            {
                NotifyMapViewChanged();
                _isQueued = false;
            }
        }

        #endregion utils

        #region overrides

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (MapAllowPan && e.LeftButton == MouseButtonState.Pressed)
            {
                _draggingCoordinates = e.GetPosition(this);
                _oldCenter = MapCenter;

                SuspendMapChanging();
            }
            RaiseOnMapMouseDown(e);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            _draggingCoordinates = null;
            if (MapAllowPan)
            {
                ResumeMapChanging();
                QueueNotifyMapViewChanged();    
            }
            RaiseOnMapMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var currentCoordinates = e.GetPosition(this);
            if (MapAllowPan && e.LeftButton == MouseButtonState.Pressed && _draggingCoordinates != null)
            {
                var delta = new[] { _draggingCoordinates.Value.X - currentCoordinates.X,
                        _draggingCoordinates.Value.Y - currentCoordinates.Y};
                var newCenter = new Point(RenderSize.Width / 2.0d + delta[0], RenderSize.Height / 2.0d + delta[1]);

                MapCenter = ToGeoCoordinates(newCenter, CreateSceneView(_oldCenter, MapZoom));
                Refresh();
            }
            RaiseOnMapMouseMove(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_draggingCoordinates != null)
            {
                ResumeMapChanging();
                QueueNotifyMapViewChanged();
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (MapAllowZoom)
            {
                MapZoom += (float)(e.Delta / 1000.0);

                QueueNotifyMapViewChanged();
            }
            RaiseOnMapMouseWheel(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var timer = new Timer {Interval = 200};
            timer.Elapsed += DeQueueNotifyMapViewChanged;
            timer.Enabled = true;
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Map != null)
            {
                QueueNotifyMapViewChanged();
            }
            return base.MeasureOverride(availableSize);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (Map != null)
            {
                var ticksBefore = DateTime.Now.Ticks;
                drawingContext.PushClip(new RectangleGeometry(new Rect(new Point(0, 0), RenderSize)));

                var renderRect = new Rect(RenderSize);
                var sceneRect = BuildSceneRect();
                var view = CreateSceneView(MapCenter, MapZoom);
                if (!_isRenderCachedScene)
                {
                    var context = new RenderContext(sceneRect.Size);
                    _renderer.Render(context, Map, view, (float)Map.Projection.ToZoomFactor(MapZoom));
                    _cachedScene = context.BuildScene();

                    drawingContext.DrawImage(_cachedScene, renderRect);
                }
                else
                {
                    var newCenter = ToPixel(MapCenter);
                    var oldCenter = ToPixel(_oldCenter);
                    var offsetX = oldCenter.X - newCenter.X;
                    var offsetY = oldCenter.Y - newCenter.Y;

                    drawingContext.PushTransform(new TranslateTransform(offsetX, offsetY));
                    drawingContext.DrawImage(_cachedScene, renderRect);
                    drawingContext.Pop();
                }
                
                drawingContext.Pop();

                var ticksAfter = DateTime.Now.Ticks;
                var message = $"Rendering took: {(new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds)}ms @ zoom level {MapZoom}";
                Logging.Log.TraceEvent("Wpf.MapControl.RenderPerfomance", TraceEventType.Information, message);
            }
        }

        #endregion overrides

        #region methods

        public GeoCoordinate ToGeoCoordinates(Point point, View2D sceneView = null)
        {
            if (Map != null)
            {
                if (sceneView == null)
                {
                    sceneView = CreateSceneView(MapCenter, MapZoom);
                }

                double x, y;
                var fromMatrix = sceneView.CreateFromViewPort(RenderSize.Width, RenderSize.Height);
                fromMatrix.Apply(point.X, point.Y, out x, out y);
                return Map.Projection.ToGeoCoordinates(x, y);
            }
            return null;
        }
        public Point ToPixel(GeoCoordinate coordinate, View2D sceneView = null)
        {
            if (Map != null)
            {
                if (sceneView == null)
                {
                    sceneView = CreateSceneView(MapCenter, MapZoom);
                }

                var projectionPoint =  Map.Projection.ToPixel(coordinate);

                double x, y;
                var fromMatrix = sceneView.CreateToViewPort(RenderSize.Width, RenderSize.Height);
                fromMatrix.Apply(projectionPoint[0], projectionPoint[1], out x, out y);
                return new Point(x, y);
            }
            return new Point();
        }

        public void ZoomIn(float delta = 0.2f)
        {
            MapZoom += delta;
            QueueNotifyMapViewChanged();
        }
        public void ZoomOut(float delta = 0.2f)
        {
            MapZoom -= delta;
            QueueNotifyMapViewChanged();
        }

        public void NotifyMapViewChanged()
        {
            if (Map != null)
            {
                var ticksBefore = DateTime.Now.Ticks;

                // notify the map.
                var view = CreateSceneView(MapCenter, MapZoom);
                Map.ViewChanged((float)Map.Projection.ToZoomFactor(MapZoom), MapCenter, view, view);

                Refresh();

                var ticksAfter = DateTime.Now.Ticks;
                var message = $"Map view changed notification took: {(new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds)}ms @ zoom level {MapZoom}";
                Logging.Log.TraceEvent("Wpf.MapControl.NotifyMapViewChangedPerfomance", TraceEventType.Information, message);
            }
        }

        #endregion methods

        #region IMapView implementation

        void IMapView.Invalidate()
        {
            Refresh();
        }
        void IMapView.RegisterAnimator(OsmSharp.UI.Animations.MapViewAnimator mapViewAnimator)
        {

        }
        void IMapView.SetMapView(GeoCoordinate center, Degree mapTilt, float mapZoom)
        {
            MapCenter = center;
            MapZoom = mapZoom;
            MapTilt = mapTilt;

            QueueNotifyMapViewChanged();
        }
        void IMapView.NotifyMapChange(double pixelsActualWidth, double pixelsActualHeight, View2D view, IProjection projection)
        {

        }

        #endregion IMapView implementation
    }

    public class MapControlEvent
    {
        public MouseEventArgs MouseState { get; set; }
        public KeyEventArgs KeaboarState { get; set; }

        public bool Handled { get; set; }
    }

    public interface IMapControlInstrument
    {
        int Priority { get; }
        bool IsStarted { get; }
        
        void Initialize(MapControl mapControl);
        void Close();

        void Start();
        void Stop();

        void HandleMouseClick();
        void HandleMouseDoubleClick();

        void HandleMouseDown();
        void HandleMouseUp();

        void HandleMouseMove();
        void HandleMouseWheel();

        void HandleKeyboard();
    }
}

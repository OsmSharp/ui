using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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

        private readonly IProjection _defaultProjection;
        private readonly MapRenderingManager _mapRenderingManager;
        private readonly MapRenderer<RenderContext> _renderer;

        private Map _map;

        private Point? _draggingCoordinates;

        #endregion fields

        #region constructors

        public MapControl()
        {
            _defaultProjection = new WebMercator();
            _renderer = new MapRenderer<RenderContext>(new DrawingRenderer2D());
            _mapRenderingManager = new MapRenderingManager();

            MapAllowPan = true;
            MapAllowZoom = true;
        }

        #endregion constructors

        #region events

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
            var geoCoordinates = _mapRenderingManager.CurrentScene.ToGeoCoordinates(e.GetPosition(this));
            var args = new MapControlEventArgs(e, geoCoordinates);
            OnMapMouseDown(args);
            MapMouseDown?.Invoke(this, args);
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
            if (_currentScene != null)
            {
                var geoCoordinates = _currentScene.ToGeoCoordinates(e.GetPosition(this));
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
            if (_currentScene != null)
            {
                var geoCoordinates = _currentScene.ToGeoCoordinates(e.GetPosition(this));
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
            if (_currentScene != null)
            {
                var geoCoordinates = _currentScene.ToGeoCoordinates(e.GetPosition(this));
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
            if (_currentScene != null)
            {
                var geoCoordinates = _currentScene.ToGeoCoordinates(e.GetPosition(this));
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
            if (_currentScene != null)
            {
                var geoCoordinates = _currentScene.ToGeoCoordinates(e.GetPosition(this));
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

        public IProjection Projection => Map != null ? Map.Projection : _defaultProjection;

        public GeoCoordinate MapCenter { get; set; }
        public float MapZoom { get; set; }
        public Degree MapTilt { get; set; }

        public Map Map
        {
            get { return _map; }
            set
            {
                if (_map != null)
                {
                    _map.MapChanged -= MapChangedAsync;
                }
                _map = value;
                if (_map != null)
                {
                    _map.MapChanged += MapChangedAsync;
                }
            }
        }

        public bool MapAllowTilt
        {
            get { return false; }
            set
            {
                // no map tilt functionality.
            }
        }
        public bool MapAllowPan { get; set; }
        public bool MapAllowZoom { get; set; }

        #endregion propreties

        #region utils

        private void Refresh()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(Refresh, DispatcherPriority.Render);
                return;
            }

            InvalidateVisual();
        }
        private async Task RenderMapAsync(MapRenderingScene scene)
        {
            await Task.Run(() =>
            {
                var renderScene = _mapRenderingManager.LastScene;
                if (renderScene != null && Map != null)
                {
                    var context = new RenderContext(renderScene.RenderSize);
                    _renderer.Render(context, Map, renderScene.CreateView(Projection),
                        (float)Projection.ToZoomFactor(MapZoom));
                    renderScene.MapImage = context.BuildScene();
                    renderScene.MapImage.Freeze();
                    _mapRenderingManager.OnRender(renderScene);
                }
            });
        }
        private async void MapChangedAsync()
        {
            await RenderMapAsync();
            Refresh();
        }

        #endregion utils

        #region overrides

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (MapAllowPan && e.LeftButton == MouseButtonState.Pressed)
            {
                _draggingCoordinates = e.GetPosition(this);
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

                MapCenter = _currentScene.ToGeoCoordinates(newCenter);
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

            _mapRenderingManager.Initialize(MapCenter, MapZoom, MapTilt, RenderSize);

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

            var ticksBefore = DateTime.Now.Ticks;

            drawingContext.PushClip(new RectangleGeometry(new Rect(new Point(0, 0), RenderSize)));
            if (_currentScene != null)
            {
                lock (_sceneLock)
                {
                    _currentScene.RenderScene(drawingContext, MapCenter, MapZoom, MapTilt);
                }
            }
            drawingContext.Pop();

            var ticksAfter = DateTime.Now.Ticks;
            var message = $"Rendering took: {(new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds)}ms @ zoom level {MapZoom}";
            Logging.Log.TraceEvent("Wpf.MapControl.RenderPerfomance", TraceEventType.Information, message);

        }

        #endregion overrides

        #region methods

        public void NotifyMapViewChanged()
        {
            if (Map != null)
            {
                var ticksBefore = DateTime.Now.Ticks;

                // notify the map.
                var newScene  = new MapRenderingScene(Map.Projection, MapCenter, MapZoom, MapTilt, RenderSize);
                var view = newScene.CreateView();
                Map.ViewChanged((float)Map.Projection.ToZoomFactor(MapZoom), MapCenter, view, view);

                MapChangedAsync();

                var ticksAfter = DateTime.Now.Ticks;
                var message = $"Map view changed notification took: {(new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds)}ms @ zoom level {MapZoom}";
                Logging.Log.TraceEvent("Wpf.MapControl.NotifyMapViewChangedPerfomance", TraceEventType.Information, message);
            }
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

        #endregion methods

        #region IMapView implementation

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

        /// <summary>
        /// Gets the current view.
        /// </summary>
        public View2D CurrentView => _currentScene?.CreateView();

        /// <summary>
        /// Returns the current ActualWidth.
        /// </summary>
        public int CurrentWidth => RenderSize.Width.ToInt();
        /// <summary>
        /// Returns the current ActualHeight.
        /// </summary>
        public int CurrentHeight => RenderSize.Height.ToInt();

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

    public class MapRenderingScene
    {
        public MapRenderingScene(GeoCoordinate center, double zoom, Degree mapTilt)
        {
            Projection = projection;
            Center = center;
            Zoom = zoom;
            Tilt = mapTilt;
            RenderSize = renderSize;
        }

        public GeoCoordinate Center { get; }
        public double Zoom { get; }
        public Degree Tilt { get; }

        public BitmapSource MapImage { get; set; }

        public View2D CreateView(IProjection projection)
        {
            var zoomFactor = projection.ToZoomFactor(Zoom);
            var sceneCenter = projection.ToPixel(Center.Latitude, Center.Longitude);
            var invertY = (true != !projection.DirectionY);

            return View2D.CreateFrom(sceneCenter[0], sceneCenter[1],
                                             RenderSize.Width, RenderSize.Height, zoomFactor,
                                             false, invertY, Tilt);
        }

        public GeoCoordinate ToGeoCoordinates(Point point, IProjection projection)
        {
            var sceneView = CreateView(projection);
            double x, y;
            var fromMatrix = sceneView.CreateFromViewPort(RenderSize.Width, RenderSize.Height);
            fromMatrix.Apply(point.X, point.Y, out x, out y);
            return projection.ToGeoCoordinates(x, y);
        }
        public Point ToPixel(GeoCoordinate coordinate, IProjection projection)
        {
            var sceneView = CreateView(projection);
            var projectionPoint = projection.ToPixel(coordinate);

            double x, y;
            var fromMatrix = sceneView.CreateToViewPort(RenderSize.Width, RenderSize.Height);
            fromMatrix.Apply(projectionPoint[0], projectionPoint[1], out x, out y);
            return new Point(x, y);
        }

        public void RenderScene(DrawingContext context, Size renderSize, GeoCoordinate cener, double zoom, Degree tilt)
        {
            //var newCenter = ToPixel(MapCenter);
            //var oldCenter = ToPixel(_oldCenter);
            //var offsetX = oldCenter.X - newCenter.X;
            //var offsetY = oldCenter.Y - newCenter.Y;

            //drawingContext.PushTransform(new TranslateTransform(offsetX, offsetY));
            //lock (_sceneLock)
            //{
            //    drawingContext.DrawImage(_cachedScene, renderRect);
            //}
            //drawingContext.Pop();


            //context.DrawImage(_cachedScene, renderRect);
        }
    }

    public class MapRenderingManager
    {
        private readonly object _lockHistory = new object();
        private readonly int _capacity;

        private Func<MapRenderingScene, Task> _renderAction;
        private readonly List<MapRenderingScene> _renderdScenes;
        private readonly List<MapRenderingScene> _renderingScenes;

        public MapRenderingManager(int capacity = 5)
        {
            _capacity = capacity;
            _renderdScenes = new List<MapRenderingScene>();
            _renderingScenes = new List<MapRenderingScene>();
        }

        public MapRenderingScene RenderingScene { get; private set; }
        public MapRenderingScene LastScene
        {
            get
            {
                lock (_lockHistory)
                {
                    return _renderdScenes.Last();
                }
            }
        }

        private void RenderMapAsync()
        {
            while (true)
            {
                
            }
        }

        public void Initialize(GeoCoordinate center, double zoom, Degree tilt)
        {
            Push(center, zoom, tilt);
        }

        public void SuspendRendering()
        {
        }
        public void ResumeRendering()
        {
        }

        public void Push(GeoCoordinate center, double zoom, Degree tilt)
        {
            lock (_lockHistory)
            {
                _renderdScenes.Add(new MapRenderingScene(center, zoom, tilt, renderSize));
                if (_renderdScenes.Count > _capacity)
                {
                    _renderdScenes.RemoveRange(0, _renderdScenes.Count - _capacity);
                }
            }
        }

        public void OnRender(MapRenderingScene scene)
        {
            lock (_lockHistory)
            {
                if (RenderingScene == null)
                {
                    RenderingScene = scene;
                }
                else
                {
                    var current = _renderdScenes.IndexOf(RenderingScene);
                    var index = _renderdScenes.IndexOf(scene);

                    if (index > current)
                    {
                        RenderingScene = scene;
                    }
                }
            }
        }
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

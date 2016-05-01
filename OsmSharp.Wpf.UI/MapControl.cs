using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI;
using OsmSharp.UI.Map;
using OsmSharp.UI.Renderer;
using OsmSharp.Units.Angle;
using OsmSharp.Wpf.UI.Renderer;
using TraceEventType = OsmSharp.Logging.TraceEventType;

namespace OsmSharp.Wpf.UI
{
    /// <summary>
    /// A map control.
    /// </summary>
    public class MapControl : FrameworkElement, IMapView
    {
        #region fields

        private readonly MapSceneManager _mapSceneManager;
        private readonly Stack<MapRenderingScene> _renderingScenes;
        private readonly object _renderingScenesLock = new object();

        private Point? _draggingCoordinates;

        #endregion fields

        #region constructors

        public MapControl()
        {
            _mapSceneManager = new MapSceneManager();
            _mapSceneManager.RenderScene += OnRenderScene;

            _renderingScenes = new Stack<MapRenderingScene>();

            MapCenter = new GeoCoordinate(0,0);
            MapZoom = 0;
            MapTilt = new Degree(0);

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
            var geoCoordinates = _mapSceneManager.ToGeoCoordinates(e.GetPosition(this));
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
            var geoCoordinates = _mapSceneManager.ToGeoCoordinates(e.GetPosition(this));
            var args = new MapControlEventArgs(e, geoCoordinates);
            OnMapMouseMove(args);
            MapMouseMove?.Invoke(this, args);
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
            var geoCoordinates = _mapSceneManager.ToGeoCoordinates(e.GetPosition(this));
            var args = new MapControlEventArgs(e, geoCoordinates);
            OnMapMouseUp(args);
            MapMouseUp?.Invoke(this, args);
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
            var geoCoordinates = _mapSceneManager.ToGeoCoordinates(e.GetPosition(this));
            var args = new MapControlEventArgs(e, geoCoordinates);
            OnMapMouseWheel(args);
            MapMouseWheel?.Invoke(this, args);
        }
        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMapMouseWheel(MapControlEventArgs e)
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
            var geoCoordinates = _mapSceneManager.ToGeoCoordinates(e.GetPosition(this));
            var args = new MapControlEventArgs(e, geoCoordinates);
            OnMapMouseClick(args);
            MapMouseClick?.Invoke(this, args);
        }
        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMapMouseClick(MapControlEventArgs e)
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
            var geoCoordinates = _mapSceneManager.ToGeoCoordinates(e.GetPosition(this));
            var args = new MapControlEventArgs(e, geoCoordinates);
            OnMapMouseDoubleClick(args);
            MapMouseDoubleClick?.Invoke(this, args);
        }
        /// <summary>
        /// Called on a mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMapMouseDoubleClick(MapControlEventArgs e)
        {

        }

        #endregion

        #endregion events

        #region propreties

        public GeoCoordinate MapCenter { get; set; }
        public float MapZoom { get; set; }
        public Degree MapTilt { get; set; }

        public Map Map
        {
            get { return _mapSceneManager.Map; }
            set { _mapSceneManager.Initialize(value, MapCenter, MapZoom, MapTilt, RenderSize); }
        }

        public bool MapAllowPan { get; set; }
        public bool MapAllowZoom { get; set; }
        public bool MapAllowTilt { get { return false; } set { } }

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
        private void OnRenderScene(MapRenderingScene scene)
        {
            if (scene.PreviousScene?.SceneImage != null || scene.SceneImage != null)
            {
                lock (_renderingScenesLock)
                {
                    //Console.WriteLine("_renderingScenes count = "+_renderingScenes.Count);
                    _renderingScenes.Push(scene);
                }
                Refresh();
            }
        }
        private void RenderScene(DrawingContext context, Size renderSize, MapRenderingScene scene)
        {
            var renderRect = new Rect(renderSize);
            if (scene.PreviousScene?.SceneImage != null)
            {
                var newCenter = _mapSceneManager.ToPixels(scene.Center, scene.PreviousScene);
                var oldCenter = _mapSceneManager.ToPixels(scene.PreviousScene.Center, scene.PreviousScene);
                var offsetX = oldCenter.X - newCenter.X;
                var offsetY = oldCenter.Y - newCenter.Y;

                //var scale = _previewScene.Zoom / lastScene.Zoom;

                context.PushTransform(new TranslateTransform(offsetX, offsetY));
                //context.PushTransform(new ScaleTransform(scale, scale));
                context.DrawImage(scene.PreviousScene.SceneImage, renderRect);
                context.Pop();
                //context.Pop();
            }
            if (scene.SceneImage != null)
            {
                //context.PushOpacity(1, new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(100))).CreateClock());
                context.DrawImage(scene.SceneImage, renderRect);
                //context.Pop();
            }

            //if (scene.SceneImage == null)
            //{
            //    if (scene.PreviousScene != null)
            //    {
            //        if (scene.PreviousScene.SceneImage == null)
            //        {
            //            Console.WriteLine("RenderScene scene null");
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine("RenderScene scene null");
            //    }   
            //}
        }
       
        #endregion utils

        #region overrides

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (MapAllowPan && e.LeftButton == MouseButtonState.Pressed)
            {
                _draggingCoordinates = e.GetPosition(this);
                _mapSceneManager.Preview();
            }
            RaiseOnMapMouseDown(e);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            _draggingCoordinates = null;
            if (MapAllowPan)
            {
                _mapSceneManager.PreviewComplete();
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

                MapCenter = _mapSceneManager.ToGeoCoordinates(newCenter);
                _mapSceneManager.Preview(MapCenter, MapZoom, MapTilt);
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
                _mapSceneManager.PreviewComplete();
                _draggingCoordinates = null;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (MapAllowZoom)
            {
                //TODO корректировка центра
                //var scene = _mapSceneManager.CurrentScene;
                MapZoom += (float)(e.Delta / 200.0);
                //var zoomPosition = e.GetPosition(this);

                //var zoomGoordinates = _mapSceneManager.ToGeoCoordinates(zoomPosition, scene);

                NotifyMapViewChanged();
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

            _mapSceneManager.Initialize(Map, MapCenter, MapZoom, MapTilt, RenderSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var newSize = base.MeasureOverride(availableSize);
            _mapSceneManager.SetSceneSize(availableSize);
            return newSize;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var ticksBefore = DateTime.Now.Ticks;

            MapRenderingScene scene = null;
            lock (_renderingScenesLock)
            {
                if (_renderingScenes.Count > 0)
                {
                    scene = _renderingScenes.Pop();
                }
            }
            if (scene != null)
            {
                drawingContext.PushClip(new RectangleGeometry(new Rect(new Point(0, 0), RenderSize)));
                RenderScene(drawingContext, RenderSize, scene);
                drawingContext.Pop();
            }
            //else
            //{
            //    Console.WriteLine("OnRender scene null");
            //}

            var ticksAfter = DateTime.Now.Ticks;
            var message = $"Rendering took: {(new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds)}ms @ zoom level {MapZoom}";
            Logging.Log.TraceEvent("Wpf.MapControl.RenderPerfomance", TraceEventType.Information, message);
        }

        #endregion overrides

        #region methods

        public void NotifyMapViewChanged()
        {
            _mapSceneManager.View(MapCenter, MapZoom, MapTilt);
        }

        public void ZoomIn(float delta = 0.2f)
        {
            MapZoom += delta;
            NotifyMapViewChanged();
        }
        public void ZoomOut(float delta = 0.2f)
        {
            MapZoom -= delta;
            NotifyMapViewChanged();
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
        public View2D CurrentView => _mapSceneManager.CreateView();

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

            NotifyMapViewChanged();
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

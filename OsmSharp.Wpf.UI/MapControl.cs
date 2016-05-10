using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
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
    public class MapControl : FrameworkElement, IMapView, INotifyPropertyChanged
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
            
            var zoomInBind = new CommandBinding(MapControlCommands.ZoomIn);
            zoomInBind.Executed += (sender, args) => ZoomIn();
            zoomInBind.CanExecute += (sender, args) =>args.CanExecute = true;
            CommandBindings.Add(zoomInBind);

            var zoomOutBind = new CommandBinding(MapControlCommands.ZoomOut);
            zoomOutBind.Executed += (sender, args) => ZoomOut();
            zoomOutBind.CanExecute += (sender, args) => args.CanExecute = true;
            CommandBindings.Add(zoomOutBind);

            var showFullMapBind = new CommandBinding(MapControlCommands.ShowFullMap);
            showFullMapBind.Executed += (sender, args) => ShowFullMap();
            showFullMapBind.CanExecute += (sender, args) => args.CanExecute = true;
            CommandBindings.Add(showFullMapBind);
        }

        #endregion constructors

        #region events

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); OnPropertyChanged(); }
        }
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); OnPropertyChanged(); }
        }
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); OnPropertyChanged(); }
        }

        public GeoCoordinateBox MapBoundingBox
        {
            get { return (GeoCoordinateBox)GetValue(MapBoundingBoxProperty); }
            set { SetValue(MapBoundingBoxProperty, value); OnPropertyChanged(); }
        }

        public float MapMinZoomLevel
        {
            get { return (float)GetValue(MapMinZoomLevelProperty); }
            set { SetValue(MapMinZoomLevelProperty, value); OnPropertyChanged(); }
        }
        public float MapMaxZoomLevel
        {
            get { return (float)GetValue(MapMaxZoomLevelProperty); }
            set { SetValue(MapMaxZoomLevelProperty, value); OnPropertyChanged(); }
        }

        public GeoCoordinate MapCenter
        {
            get { return (GeoCoordinate)GetValue(MapCenterProperty); }
            set { SetValue(MapCenterProperty, value); OnPropertyChanged(); }
        }
        public float MapZoom
        {
            get { return (float)GetValue(MapZoomProperty); }
            set { SetValue(MapZoomProperty, value); OnPropertyChanged(); }
        }
        public Degree MapTilt
        {
            get { return (Degree)GetValue(MapTiltProperty); }
            set { SetValue(MapTiltProperty, value); OnPropertyChanged(); }
        }

        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); OnPropertyChanged(); }
        }

        public bool MapAllowPan
        {
            get { return (bool)GetValue(MapAllowPanProperty); }
            set { SetValue(MapAllowPanProperty, value); OnPropertyChanged(); }
        }
        public bool MapAllowZoom
        {
            get { return (bool)GetValue(MapAllowZoomProperty); }
            set { SetValue(MapAllowZoomProperty, value); OnPropertyChanged(); }
        }
        public bool MapAllowTilt
        {
            get { return (bool)GetValue(MapAllowTiltProperty); }
            set { SetValue(MapAllowTiltProperty, value); OnPropertyChanged(); }
        }

        #endregion propreties

        #region utils

        private void AdjustMapScene()
        {
            if (MapZoom < MapMinZoomLevel)
            {
                MapZoom = MapMinZoomLevel;
            }
            if (MapZoom > MapMaxZoomLevel)
            {
                MapZoom = MapMaxZoomLevel;
            }

            var scene = new MapRenderingScene(MapCenter, MapZoom, MapTilt);
            var geoLeftTop = _mapSceneManager.ToGeoCoordinates(new Point(0, RenderSize.Height), scene);
            var geoRightBottom = _mapSceneManager.ToGeoCoordinates(new Point(RenderSize.Width, 0), scene);

            var lat = MapCenter.Latitude;
            var lon = MapCenter.Longitude;

            var width = geoRightBottom.Longitude - geoLeftTop.Longitude;
            var height = geoRightBottom.Latitude - geoLeftTop.Latitude;

            if (width < MapBoundingBox.DeltaLon)
            {
                if (geoLeftTop.Longitude < MapBoundingBox.MinLon)
                {
                    lon += MapBoundingBox.MinLon - geoLeftTop.Longitude;
                }
                if (geoRightBottom.Longitude > MapBoundingBox.MaxLon)
                {
                    lon += MapBoundingBox.MaxLon - geoRightBottom.Longitude;
                }
            }
            else
            {
                lon = MapBoundingBox.Center.Longitude;
            }
            if (height < MapBoundingBox.DeltaLat)
            {
                if (geoLeftTop.Latitude < MapBoundingBox.MinLat)
                {
                    lat += MapBoundingBox.MinLat - geoLeftTop.Latitude;
                }
                if (geoRightBottom.Latitude > MapBoundingBox.MaxLat)
                {
                    lat +=  MapBoundingBox.MaxLat - geoRightBottom.Latitude;
                }
            }
            else
            {
                lat = MapBoundingBox.Center.Latitude;
            }
            MapCenter = new GeoCoordinate(lat, lon);
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
                context.DrawRectangle(Background, null, renderRect);
                context.DrawImage(scene.SceneImage, renderRect);
                //context.Pop();
            }
            else
            {
                context.DrawRectangle(Background, null, renderRect);
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
                AdjustMapScene();
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
                drawingContext.PushClip(new RectangleGeometry(new Rect(RenderSize)));
                RenderScene(drawingContext, RenderSize, scene);
                drawingContext.Pop();
            }
            else
            {
                drawingContext.DrawRectangle(Background, null, new Rect(RenderSize));
            }

            var ticksAfter = DateTime.Now.Ticks;
            var message = $"Rendering took: {(new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds)}ms @ zoom level {MapZoom}";
            Logging.Log.TraceEvent("Wpf.MapControl.RenderPerfomance", TraceEventType.Information, message);
        }

        #endregion overrides

        #region methods

        public void NotifyMapViewChanged()
        {
            AdjustMapScene();
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

        public void ShowFullMap()
        {
            MapCenter = MapBoundingBox.Center;
            MapZoom = MapMinZoomLevel;

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

        #region dependency properties

        public static readonly DependencyProperty BorderBrushProperty;
        public static readonly DependencyProperty BorderThicknessProperty;
        public static readonly DependencyProperty BackgroundProperty;

        public static readonly DependencyProperty MapBoundingBoxProperty;

        public static readonly DependencyProperty MapMinZoomLevelProperty;
        public static readonly DependencyProperty MapMaxZoomLevelProperty;

        public static readonly DependencyProperty MapCenterProperty;
        public static readonly DependencyProperty MapZoomProperty;
        public static readonly DependencyProperty MapTiltProperty;

        public static readonly DependencyProperty MapProperty;

        public static readonly DependencyProperty MapAllowPanProperty;
        public static readonly DependencyProperty MapAllowZoomProperty;
        public static readonly DependencyPropertyKey MapAllowTiltPropertyKey;
        public static readonly DependencyProperty MapAllowTiltProperty;

        static MapControl()
        {
            BorderBrushProperty = Control.BorderBrushProperty.AddOwner(typeof(MapControl));
            BorderThicknessProperty = Control.BorderThicknessProperty.AddOwner(typeof(MapControl));
            BackgroundProperty = Control.BackgroundProperty.AddOwner(typeof(MapControl));

            MapBoundingBoxProperty = DependencyProperty.Register("MapBoundingBox",
               typeof(GeoCoordinateBox), typeof(MapControl), new UIPropertyMetadata(new GeoCoordinateBox(new GeoCoordinate(-80, -180), new GeoCoordinate(80, 180)), (o, e) =>
               {
                   var mapControl = o as MapControl;
                   mapControl?.NotifyMapViewChanged();
               }));
            MapMinZoomLevelProperty = DependencyProperty.Register("MapMinZoomLevel",
                typeof(float), typeof(MapControl), new UIPropertyMetadata(0f, (o, e) =>
                {
                    var mapControl = o as MapControl;
                    mapControl?.NotifyMapViewChanged();
                }));
            MapMaxZoomLevelProperty = DependencyProperty.Register("MapMaxZoomLevel",
             typeof(float), typeof(MapControl), new UIPropertyMetadata(20f, (o, e) =>
                {
                    var mapControl = o as MapControl;
                    mapControl?.NotifyMapViewChanged();
                }));

            MapCenterProperty = DependencyProperty.Register("MapCenter",
               typeof(GeoCoordinate), typeof(MapControl), new UIPropertyMetadata(new GeoCoordinate(0, 0)));
            MapZoomProperty = DependencyProperty.Register("MapZoom",
               typeof(float), typeof(MapControl), new UIPropertyMetadata(0f));
            MapTiltProperty = DependencyProperty.Register("MapTilt",
               typeof(Degree), typeof(MapControl), new UIPropertyMetadata(new Degree(0)));

            MapProperty = DependencyProperty.Register("Map",
                typeof(Map), typeof(MapControl), 
                new UIPropertyMetadata(new Map(new WebMercator()),
                    (o, e) =>
                    {
                        var mapControl = o as MapControl;
                        mapControl?._mapSceneManager.Initialize(mapControl.Map, mapControl.MapCenter, mapControl.MapZoom,
                            mapControl.MapTilt, mapControl.RenderSize);
                    }));

            MapAllowPanProperty = DependencyProperty.Register("MapAllowPan",
              typeof(bool), typeof(MapControl), new UIPropertyMetadata(true));
            MapAllowZoomProperty = DependencyProperty.Register("MapAllowZoom",
               typeof(bool), typeof(MapControl), new UIPropertyMetadata(true));
            MapAllowTiltPropertyKey = DependencyProperty.RegisterReadOnly("MapAllowTilt",
               typeof(bool), typeof(MapControl), new UIPropertyMetadata(false));
            MapAllowTiltProperty = MapAllowTiltPropertyKey.DependencyProperty;
        }

        #endregion dependency properties
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

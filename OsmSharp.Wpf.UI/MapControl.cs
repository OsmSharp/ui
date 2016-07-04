using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;
using OsmSharp.Units.Angle;
using OsmSharp.Wpf.UI.Extensions;
using OsmSharp.Wpf.UI.Renderer;
using OsmSharp.Wpf.UI.Views;
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

        private readonly List<Tuple<int, Layer>> _customLayers;
        private LayerPrimitives _canvas;

        private Point? _draggingCoordinates;
        private Point? _toolTipCoordinates;
        private bool _isReady;

        private bool _isSuspendNotifyMapViewChanged;

        private readonly Popup _toolTip;
        private readonly DispatcherTimer _toolTipShowTimer;
        private readonly DispatcherTimer _toolTipHideTimer;
        private CancellationTokenSource _toolTipCancellationTokenSource;

        #endregion fields

        #region constructors

        public MapControl()
        {
            _mapSceneManager = new MapSceneManager();
            _mapSceneManager.RenderScene += OnRenderScene;

            _renderingScenes = new Stack<MapRenderingScene>();

            _isSuspendNotifyMapViewChanged = false;
            _isReady = true;

            _customLayers = new List<Tuple<int, Layer>>();
            _canvas = new LayerPrimitives(new WebMercator());

            var notifyMapViewChanged = new CommandBinding(MapControlCommands.NotifyMapViewChanged);
            notifyMapViewChanged.Executed += (sender, args) => NotifyMapViewChanged();
            notifyMapViewChanged.CanExecute += (sender, args) => args.CanExecute = true;
            CommandBindings.Add(notifyMapViewChanged);

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

            _toolTip = new Popup
            {
                PlacementTarget = this,
                Placement = PlacementMode.Relative,
                AllowsTransparency = true
            };
            _toolTip.MouseEnter += (sender, args) => { _toolTipHideTimer.Stop(); };

            _toolTipShowTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(500)};
            _toolTipShowTimer.Tick += (sender, args) => ShowToolTip();
            _toolTipHideTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(500)};
            _toolTipHideTimer.Tick += (sender, args) => HideToolTip();
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

        public bool IsReady
        {
            get { return _isReady; }
            private set { _isReady = value; OnPropertyChanged(); }
        }

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
            set
            {
                if (System.Math.Abs(MapZoom - value) > 0.0001)
                {
                    SetValue(MapZoomProperty, value);
                    OnPropertyChanged();
                }
            }
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

        public IEnumerable<Layer> CustomLayers
        {
            get { return _customLayers.Select(l=>l.Item2); }
        }
        public LayerPrimitives Canvas
        {
            get { return _canvas; }
        }

        #endregion propreties

        #region utils

        private void AdjustMapScene()
        {
            var suspending = !_isSuspendNotifyMapViewChanged;
            if (suspending)
            {
                SuspendNotifyMapViewChanged();
            }

            if (MapZoom < MapMinZoomLevel)
            {
                MapZoom = MapMinZoomLevel;
            }
            if (MapZoom > MapMaxZoomLevel)
            {
                MapZoom = MapMaxZoomLevel;
            }

            if (RenderSize.Width > 0 && RenderSize.Height > 0)
            {
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
                        lat += MapBoundingBox.MaxLat - geoRightBottom.Latitude;
                    }
                }
                else
                {
                    lat = MapBoundingBox.Center.Latitude;
                }
                MapCenter = new GeoCoordinate(lat, lon);
            }

            if (suspending)
            {
                ResumeNotifyMapViewChanged(false);
            }
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
                //context.DrawRectangle(Background, null, renderRect);
                context.DrawImage(scene.SceneImage, renderRect);
                //context.Pop();
            }
            //else
            //{
            //    context.DrawRectangle(Background, null, renderRect);
            //}

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

        private void InitializeMap()
        {
            if (_mapSceneManager.Map != null)
            {
                foreach (var layer in _customLayers)
                {
                    _mapSceneManager.Map.RemoveLayer(layer.Item2);
                }
                _mapSceneManager.Map.RemoveLayer(_canvas);
            }

            _isSuspendNotifyMapViewChanged = false;
            AdjustMapScene();
            _mapSceneManager.Initialize(Map, MapCenter, MapZoom, MapTilt, RenderSize);
            if (_mapSceneManager.Map != null)
            {
                SuspendNotifyMapViewChanged();

                foreach (var layer in _customLayers.OrderBy(l=>l.Item1))
                {
                    _mapSceneManager.Map.AddLayer(layer.Item2);
                }
                _canvas = new LayerPrimitives(_mapSceneManager.Map.Projection);
                _mapSceneManager.Map.AddLayer(_canvas);

                ResumeNotifyMapViewChanged();
            }
        }

        private void StartShowToolTip()
        {
            _toolTipShowTimer.Stop();
            _toolTipShowTimer.Start();
        }
        private void StartHideToolTip()
        {
            _toolTipShowTimer.Stop();
            if (_toolTip.IsOpen && !_toolTipHideTimer.IsEnabled)
            {
                _toolTipHideTimer.Start();
            }
        }
        private async void StartToolTilSearch()
        {
            if (_toolTipCoordinates != null)
            {
                _toolTipCancellationTokenSource = new CancellationTokenSource();

                var geo = _mapSceneManager.ToGeoCoordinates(_toolTipCoordinates.Value);
                var obj = await _mapSceneManager.SearchPrimitiveAsync(geo, _toolTipCancellationTokenSource.Token);
                if (obj != null && obj.ToolTip != null)
                {
                    if (obj.ToolTip is UIElement)
                    {
                        _toolTip.Child = (UIElement)obj.ToolTip;
                    }
                    else
                    {
                        _toolTip.Child = new TextToolTipView { Text = obj.ToolTip.ToString() };
                    }

                    _toolTip.HorizontalOffset = _toolTipCoordinates.Value.X + 10;
                    _toolTip.VerticalOffset = _toolTipCoordinates.Value.Y + 10;
                    _toolTip.IsOpen = true;
                }
            }
        }
        private void StopToolTipSearch()
        {
            if (_toolTipCancellationTokenSource != null)
            {
                _toolTipCancellationTokenSource.Cancel(false);
                _toolTipCancellationTokenSource = null;
            }
        }
        private void StopToolTip()
        {
            _toolTipHideTimer.Stop();
            _toolTipShowTimer.Stop();

            StopToolTipSearch();
        }

        private void ShowToolTip()
        {
            _toolTipShowTimer.Stop();
            StopToolTipSearch();
            StartToolTilSearch();
        }

        private void HideToolTip()
        {
            StopToolTipSearch();
            _toolTipHideTimer.Stop();
            _toolTip.IsOpen = false;
        }

        #endregion utils

        #region overrides

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            SuspendNotifyMapViewChanged();

            if (MapAllowPan && e.LeftButton == MouseButtonState.Pressed)
            {
                _draggingCoordinates = e.GetPosition(this);
                _mapSceneManager.Preview();
            }

            StartHideToolTip();
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

            ResumeNotifyMapViewChanged(false);
            RaiseOnMapMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var currentCoordinates = e.GetPosition(this);
            if (MapAllowPan && e.LeftButton == MouseButtonState.Pressed && _draggingCoordinates != null)
            {
                var delta = new[]
                {
                    _draggingCoordinates.Value.X - currentCoordinates.X,
                    _draggingCoordinates.Value.Y - currentCoordinates.Y
                };
                var newCenter = new Point(RenderSize.Width/2.0d + delta[0], RenderSize.Height/2.0d + delta[1]);

                MapCenter = _mapSceneManager.ToGeoCoordinates(newCenter);
                AdjustMapScene();
                _mapSceneManager.Preview(MapCenter, MapZoom, MapTilt);
            }
            else
            {
                StartHideToolTip();
                _toolTipCoordinates = currentCoordinates;
                StartShowToolTip();
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

            if (!_toolTip.IsMouseOver)
            {
                StartHideToolTip();
            }
            else
            {
                StopToolTip();
            }

            ResumeNotifyMapViewChanged(false);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (MapAllowZoom)
            {
                var newZoom = MapZoom + (float)(e.Delta / 200.0);
                if (newZoom >= MapMinZoomLevel && newZoom <= MapMaxZoomLevel)
                {
                    SuspendNotifyMapViewChanged();

                    var zoomPosition = e.GetPosition(this);
                    var zoomCoordinates = _mapSceneManager.ToGeoCoordinates(zoomPosition);

                    var currentZoomFactor = _mapSceneManager.Map.Projection.ToZoomFactor(MapZoom);
                    var newZoomFactor = _mapSceneManager.Map.Projection.ToZoomFactor(newZoom);

                    MapZoom += (float) (e.Delta/200.0);

                    var deltaLon = (zoomCoordinates.Longitude - MapCenter.Longitude)*currentZoomFactor/newZoomFactor;
                    var deltaLat = (zoomCoordinates.Latitude - MapCenter.Latitude)*currentZoomFactor/newZoomFactor;

                    MapCenter = new GeoCoordinate(zoomCoordinates.Latitude - deltaLat,
                        zoomCoordinates.Longitude - deltaLon);
                    MapZoom = newZoom;

                    ResumeNotifyMapViewChanged();
                }
            }

            StartHideToolTip();

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

            InitializeMap();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            AdjustMapScene();
            var newSize = base.MeasureOverride(availableSize);
            _mapSceneManager.SetSceneSize(availableSize);

            StartHideToolTip();

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

        public void SuspendNotifyMapViewChanged()
        {
            _isSuspendNotifyMapViewChanged = true;
        }
        public void ResumeNotifyMapViewChanged(bool fireNotifyMapViewChanged = true)
        {
            _isSuspendNotifyMapViewChanged = false;
            if (fireNotifyMapViewChanged)
            {
                NotifyMapViewChanged();
            }

        }
        public void NotifyMapViewChanged()
        {
            if (!_isSuspendNotifyMapViewChanged)
            {
                IsReady = false;
                AdjustMapScene();
                _mapSceneManager.View(MapCenter, MapZoom, MapTilt);
            }
        }

        public void ZoomIn(float delta = 0.2f)
        {
            MapZoom += delta;
        }
        public void ZoomOut(float delta = 0.2f)
        {
            MapZoom -= delta;
        }
        public void ZoomToCoordinate(GeoCoordinate coordinate, float maxZoom = 19)
        {
            ZoomToBox(coordinate.ToBox(), maxZoom);
        }
        public void ZoomToBox(GeoCoordinateBox box, float maxZoom = 19)
        {
            SuspendNotifyMapViewChanged();

            var currentView = _mapSceneManager.CreateView();
            var currentPerimeter = currentView.Width*2 + currentView.Height*2;

            var topLeft = _mapSceneManager.Map.Projection.ToPixel(box.TopLeft);
            var bottomRight = _mapSceneManager.Map.Projection.ToPixel(box.BottomRight);
            var newPerimeter = (bottomRight[0] - topLeft[0])*2 + (bottomRight[1] - topLeft[1])*2;
            var perimeterFactor = newPerimeter/currentPerimeter;

            MapCenter = box.Center;
            if (perimeterFactor > 3 || perimeterFactor < 1/3f)
            {
                MapZoom = System.Math.Min(box.GetZoomLevel(), maxZoom);
            }

            ResumeNotifyMapViewChanged();
        }

        public void ShowFullMap()
        {
            SuspendNotifyMapViewChanged();
            MapCenter = MapBoundingBox.Center;
            MapZoom = MapMinZoomLevel;
            ResumeNotifyMapViewChanged();
        }

        public void AddLayer(Layer layer, int zIndex)
        {
            var index = 0;
            for (var i = 0; i < _customLayers.Count-1; i++)
            {
                var fist = _customLayers[i];
                var second = _customLayers[i +1];
                if (zIndex >= fist.Item1 && zIndex < second.Item1)
                {
                    index = i + 1;
                }
            }
            _customLayers.Insert(index, new Tuple<int, Layer>(zIndex, layer));
            var fistLayer = _customLayers.FirstOrDefault();
            if (fistLayer != null)
            {
                for (var i = 0; i < _mapSceneManager.Map.LayerCount; i++)
                {
                    index++;
                    if (fistLayer.Item2 == _mapSceneManager.Map[i])
                    {
                        break;
                    }
                }
            }
            _mapSceneManager.Map.InsertLayer(index, layer);
        }
        public void RemoveLayer(Layer layer)
        {
            _mapSceneManager.Map.RemoveLayer(layer);
            var item = _customLayers.FirstOrDefault(l => l.Item2 == layer);
            if (item != null)
            {
                _customLayers.Remove(item);
            }
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
            SuspendNotifyMapViewChanged();
            MapCenter = center;
            MapZoom = mapZoom;
            MapTilt = mapTilt;
            ResumeNotifyMapViewChanged();
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
            PropertyChangedCallback notifyMapViewChanged = (o, e) =>
            {
                var mapControl = o as MapControl;
                mapControl?.NotifyMapViewChanged();
            };

            BorderBrushProperty = Control.BorderBrushProperty.AddOwner(typeof(MapControl));
            BorderThicknessProperty = Control.BorderThicknessProperty.AddOwner(typeof(MapControl));
            BackgroundProperty = Control.BackgroundProperty.AddOwner(typeof(MapControl));

            MapBoundingBoxProperty = DependencyProperty.Register("MapBoundingBox",
                typeof(GeoCoordinateBox), typeof(MapControl),
                new UIPropertyMetadata(new GeoCoordinateBox(new GeoCoordinate(-80, -180), new GeoCoordinate(80, 180)),
                    notifyMapViewChanged));
            MapMinZoomLevelProperty = DependencyProperty.Register("MapMinZoomLevel",
                typeof(float), typeof(MapControl), new UIPropertyMetadata(0f, notifyMapViewChanged));
            MapMaxZoomLevelProperty = DependencyProperty.Register("MapMaxZoomLevel",
             typeof(float), typeof(MapControl), new UIPropertyMetadata(19f, notifyMapViewChanged));

            MapCenterProperty = DependencyProperty.Register("MapCenter",
               typeof(GeoCoordinate), typeof(MapControl), new UIPropertyMetadata(new GeoCoordinate(0, 0), notifyMapViewChanged));
            MapZoomProperty = DependencyProperty.Register("MapZoom",
               typeof(float), typeof(MapControl), new UIPropertyMetadata(0f, (o, e) =>
               {
                   var newZoom = (float) e.NewValue;
                   var oldZoom = (float) e.OldValue;
                   if (System.Math.Abs(newZoom - oldZoom) > 0.0001)
                   {
                       notifyMapViewChanged(o, e);
                   }
               }));
            MapTiltProperty = DependencyProperty.Register("MapTilt",
               typeof(Degree), typeof(MapControl), new UIPropertyMetadata(new Degree(0), notifyMapViewChanged));

            MapProperty = DependencyProperty.Register("Map",
                typeof(Map), typeof(MapControl), 
                new UIPropertyMetadata(new Map(new WebMercator()),
                    (o, e) =>
                    {
                        var mapControl = o as MapControl;
                        mapControl?.InitializeMap();
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

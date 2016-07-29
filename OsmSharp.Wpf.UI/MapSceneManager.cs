using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using OsmSharp.Math.Geo;
using OsmSharp.UI;
using OsmSharp.UI.Map;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.Units.Angle;
using OsmSharp.Wpf.UI.Renderer;

namespace OsmSharp.Wpf.UI
{
    public class MapSceneManager
    {
        #region fields

        private readonly object _historyLock = new object();
        private readonly Map _defaultMap = new Map();

        private readonly int _historyCapacity;

        private readonly Queue<MapRenderingScene> _renderedScenes;
        private MapRenderingScene _previewScene;
        
        private readonly MapRenderer<RenderContext> _renderer;

        #endregion fields

        #region constructors

        public MapSceneManager(int historyCapacity = 5)
        {
            _historyCapacity = historyCapacity;
            _renderedScenes = new Queue<MapRenderingScene>();
            _previewScene = null;

            _renderer = new MapRenderer<RenderContext>(new DrawingRenderer2D());

            IsReady = true;
        }

        #endregion constructors

        #region events

        public delegate void RenderSceneHandler(MapRenderingScene scene);

        public event RenderSceneHandler RenderScene;
        protected void OnRenderScene(MapRenderingScene scene)
        {
            var renderScene = RenderScene;
            renderScene?.Invoke(scene);
        }

        #endregion events

        #region properties

        public bool IsReady { get; private set; }

        public Map Map { get; private set; }
        public Size SceneSize { get; private set; }
        
        public MapRenderingScene CurrentScene => GetLastScene();

        #endregion properties

        #region methods

        private MapRenderingScene GetLastScene(bool isRendered = false)
        {
            lock (_historyLock)
            {
                IEnumerable<MapRenderingScene> scenes = _renderedScenes;
                if (isRendered)
                {
                    scenes = scenes.Where(s => s.SceneImage != null);
                }
                return scenes.LastOrDefault();
            }
        }
        private void RenderingSceneAsync(MapRenderingScene scene)
        {
            Task.Run(() =>
            {
                var context = new RenderContext(SceneSize);
                _renderer.Render(context, Map, CreateView(scene),
                    (float)Map.Projection.ToZoomFactor(scene.Zoom));
                var image = context.BuildScene();
                image.Freeze();
                scene.SceneImage = image;
            }).ContinueWith((t, s) =>
            {
                var currentScene = (MapRenderingScene) s;
                var last = GetLastScene(true);
                if (last == null || last == currentScene)
                {
                    OnRenderScene(currentScene);
                }
            }, scene);    
        }
        private void MapChanged()
        {
            var scene = GetLastScene();
            if (scene != null)
            {
                RenderingSceneAsync(scene);
            }
            else
            {
                Console.WriteLine("MapChanged scene null");
            }
        }

        public void Initialize(Map map, GeoCoordinate center, double zoom, Degree tilt, Size sceneSize)
        {
            if (map == null)
            {
                map = _defaultMap;
            }
            if (center == null) throw new ArgumentNullException(nameof(center));
            if (tilt == null) throw new ArgumentNullException(nameof(tilt));

            if (Map != null)
            {
                Map.MapChanged -= MapChanged;
                //Map.RemoveLayer()
            }
            Map = map;
            Map.MapChanged += MapChanged;

            SceneSize = sceneSize;
            View(center, zoom, tilt);
        }
        public void SetSceneSize(Size sceneSize)
        {
            SceneSize = sceneSize;
            var scene = GetLastScene();
            if (SceneSize.Width > 0 && SceneSize.Height > 0)
            {
                var view = CreateView(scene);
                Map.ViewChanged((float)Map.Projection.ToZoomFactor(scene.Zoom), scene.Center, view,
                    view);
                RenderingSceneAsync(scene);
            }
        }

        public View2D CreateView(MapRenderingScene scene)
        {
            var zoomFactor = Map.Projection.ToZoomFactor(scene.Zoom);
            var sceneCenter = Map.Projection.ToPixel(scene.Center.Latitude, scene.Center.Longitude);
            var invertY = (true != !Map.Projection.DirectionY);

            return View2D.CreateFrom(sceneCenter[0], sceneCenter[1],
                SceneSize.Width, SceneSize.Height, zoomFactor,
                false, invertY, scene.Tilt);
        }
        public GeoCoordinate ToGeoCoordinates(Point point, MapRenderingScene scene)
        {
            var sceneView = CreateView(scene);
            double x, y;
            var fromMatrix = sceneView.CreateFromViewPort(SceneSize.Width, SceneSize.Height);
            fromMatrix.Apply(point.X, point.Y, out x, out y);
            return Map.Projection.ToGeoCoordinates(x, y);
        }
        public Point ToPixels(GeoCoordinate coordinate, MapRenderingScene scene)
        {
            var sceneView = CreateView(scene);
            var projectionPoint = Map.Projection.ToPixel(coordinate);

            double x, y;
            var fromMatrix = sceneView.CreateToViewPort(SceneSize.Width, SceneSize.Height);
            fromMatrix.Apply(projectionPoint[0], projectionPoint[1], out x, out y);
            return new Point(x, y);
        }

        public View2D CreateView()
        {
            return CreateView(GetLastScene());
        }
        public GeoCoordinate ToGeoCoordinates(Point point)
        {
            return ToGeoCoordinates(point, GetLastScene());
        }
        public Point ToPixels(GeoCoordinate coordinate)
        {
            return ToPixels(coordinate, GetLastScene());
        }

        public void Preview()
        {
            var currentScene = CurrentScene;
            Preview(currentScene.Center, currentScene.Zoom, currentScene.Tilt);
        }
        public void Preview(GeoCoordinate cener, double zoom, Degree tilt)
        {
            if (Map != null)
            {
                IsReady = false;
                Map.Pause();
                _previewScene = new MapRenderingScene(cener, zoom, tilt)
                {
                    PreviousScene = GetLastScene()
                };
                OnRenderScene(_previewScene);
            }
        }
        public void PreviewComplete()
        {
            if (_previewScene != null)
            {
                Map.Resume();
                var newScene = new MapRenderingScene(_previewScene.Center, _previewScene.Zoom, _previewScene.Tilt)
                {
                    PreviousScene = GetLastScene()
                };
                lock (_historyLock)
                {
                    _renderedScenes.Enqueue(newScene);
                    while (_renderedScenes.Count > _historyCapacity)
                    {
                        _renderedScenes.Dequeue();
                    }
                }
                if (SceneSize.Width > 0 && SceneSize.Height > 0)
                {
                    var view = CreateView(newScene);
                    Map.ViewChanged((float)Map.Projection.ToZoomFactor(newScene.Zoom), newScene.Center, view,
                        view);
                    RenderingSceneAsync(newScene);
                }
                _previewScene = null;

                IsReady = true;
            }
        }
        public void View(GeoCoordinate center, double zoom, Degree tilt)
        {
            Preview(center, zoom, tilt);
            PreviewComplete();
        }

        public async Task<Primitive2D> SearchPrimitiveAsync(GeoCoordinate coordinate, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                var scene = CurrentScene;
                var zoomFactor = (float) Map.Projection.ToZoomFactor(scene.Zoom);
                var view = CreateView(CurrentScene);
                var point = ToPixels(coordinate, scene);
                var backColor = SimpleColor.FromKnownColor(KnownColor.Transparent).Value;
                var objs = new List<Primitive2D>();

                for (int i = 0; i < Map.LayerCount; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    var layer = Map[i];
                    if (layer.IsLayerVisibleFor((float) scene.Zoom))
                    {
                        objs.AddRange(layer.Get(zoomFactor, view));
                    }
                }
                objs.Reverse();

                foreach (var obj in objs.Where(o => o.ToolTip != null))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    var context = new RenderContext(SceneSize);
                    _renderer.SceneRenderer.Render(context, view, zoomFactor, new[] {obj}, backColor);
                    var image = context.BuildScene();
                    image.Freeze();

                    int bytePerPixel = (image.Format.BitsPerPixel + 7)/8;
                    int stride = image.PixelWidth*bytePerPixel;
                    byte[] data = new byte[stride*image.PixelHeight];
                    image.CopyPixels(data, stride, 0);

                    //Pbgra32
                    var pixel = new byte[bytePerPixel];
                    var offset = stride*(int) point.Y + (int) point.X*bytePerPixel;

                    for (int i = 0; i < bytePerPixel; i++)
                    {
                        pixel[i] = data[offset + i];
                    }
                    if (pixel.Any(p => p != 0))
                    {
                        return obj;
                    }
                }
                return null;
            }, cancellationToken);
        }

        #endregion methods
    }
}
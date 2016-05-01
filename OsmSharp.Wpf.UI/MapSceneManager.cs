using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Map;
using OsmSharp.UI.Renderer;
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

        public Map Map { get; private set; }
        public Size SceneSize { get; private set; }
        
        public MapRenderingScene CurrentScene => GetLastScene();

        #endregion properties

        #region methods

        private MapRenderingScene GetLastScene()
        {
            lock (_historyLock)
            {
                return _renderedScenes.LastOrDefault();
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
                OnRenderScene((MapRenderingScene)s);
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
            }
            Map = map;
            Map.MapChanged += MapChanged;
            Map = map;

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
            Map.Pause();
            _previewScene = new MapRenderingScene(cener, zoom, tilt)
            {
                PreviousScene = GetLastScene()
            };
            OnRenderScene(_previewScene);
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
            }
            
        }
        public void View(GeoCoordinate center, double zoom, Degree tilt)
        {
            Preview(center, zoom, tilt);
            PreviewComplete();
        }

        #endregion methods
    }
}
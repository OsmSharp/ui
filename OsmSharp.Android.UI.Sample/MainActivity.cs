using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using OsmSharp.UI.Renderer;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using OsmSharp.UI;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.Osm.Data.PBF.Processor;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data.Xml.Processor;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Graphs.Serialization;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.CH.Serialization;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.UI.Animations;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math;
using OsmSharp.Math.Primitives;
using System.Timers;
using OsmSharp.Routing.Navigation;

namespace OsmSharp.Android.UI.Sample
{
	/// <summary>
	/// Activity1.
	/// </summary>
	[Activity]
    public class MainActivity : Activity
	{
        /// <summary>
        /// Holds the router.
        /// </summary>
        private Router _router;

        /// <summary>
        /// Holds the route layer.
        /// </summary>
        private LayerRoute _routeLayer;

		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

//			OsmSharp.IO.Output.OutputStreamHost.RegisterOutputStream (
//				new OsmSharp.Android.UI.IO.Output.ConsoleOutputStream ());
			
			// create the MapCSS image source.
			var imageSource = new MapCSSDictionaryImageSource();
			imageSource.Add("styles/default/parking.png",
			                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.images.parking.png"));
			imageSource.Add("styles/default/bus.png",
			                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.images.bus.png"));
			imageSource.Add("styles/default/postbox.png",
			                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.images.postbox.png"));

//			// load mapcss style interpreter.
//			var mapCSSInterpreter = new MapCSSInterpreter(
//				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.mapcss"),
//				imageSource);
			
			// initialize the data source.
			//var dataSource = new MemoryDataSource();
//			var source = new XmlOsmStreamReader(
//				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.osm"));
//			var source = new PBFOsmStreamReader(
//				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.osm.pbf"));
//			dataSource.PullFromSource(source);

			// initialize map.
			var map = new Map();
			//map.AddLayer(new LayerTile(@"http://otile1.mqcdn.com/tiles/1.0.0/osm/{0}/{1}/{2}.png"));
			//map.AddLayer(new OsmLayer(dataSource, mapCSSInterpreter));
//			map.AddLayer(new LayerScene(Scene2DSimple.Deserialize(
//							Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.wvl.osm.pbf.scene.simple"), true)));
			map.AddLayer(
				new LayerScene(
				Scene2DLayered.Deserialize(
					Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.Android.UI.Sample.wvl.map"), true)));

//			var routingSerializer = new V2RoutingDataSourceLiveEdgeSerializer(true);
//			var graphSerialized = routingSerializer.Deserialize(
//				//Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.osm.pbf.routing.3"));
//				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.wvl.pbf.routing.4"));
////
////			var graphLayer = new LayerDynamicGraphLiveEdge(graphSerialized, mapCSSInterpreter);
////			map.AddLayer(graphLayer);
//			
//			// calculate route.            
//			Router router = Router.CreateLiveFrom(
//				graphSerialized,
//				new OsmRoutingInterpreter());

            var routingSerializer = new OsmSharp.Routing.CH.Serialization.Sorted.CHEdgeDataDataSourceSerializer(false);
            var graphDeserialized = routingSerializer.Deserialize(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.wvl.routing"), true);

            _router = Router.CreateCHFrom(
                graphDeserialized, new CHRouter(graphDeserialized),
                new OsmRoutingInterpreter());
            GeoCoordinate point1 = new GeoCoordinate(51.158075, 2.961545);
            GeoCoordinate point2 = new GeoCoordinate(51.190503, 3.004793);
            RouterPoint routerPoint1 = _router.Resolve(Vehicle.Car, point1);
            RouterPoint routerPoint2 = _router.Resolve(Vehicle.Car, point2);
            Route route1 = _router.Calculate(Vehicle.Car, routerPoint1, routerPoint2);
            _routeTracker = new RouteTracker(route1, new OsmRoutingInterpreter());
            _enumerator = route1.GetRouteEnumerable(3).GetEnumerator();
            _enumeratorNext = route1.GetRouteEnumerable(3).GetEnumerator();
            for (int idx = 0; idx < 20; idx++)
            {
                _enumeratorNext.MoveNext();
                _enumeratorNext.MoveNext();
            }
            _routeLayer = new LayerRoute(map.Projection);
            _routeLayer.AddRoute (route1, SimpleColor.FromKnownColor(KnownColor.Blue).Value);
            map.AddLayer(_routeLayer);

//			// create gpx layer.
//			LayerGpx gpxLayer = new LayerGpx(map.Projection);
//			gpxLayer.AddGpx(
//				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.gpx"));
//			map.AddLayer(gpxLayer);
			
//			// set control properties.
//			var mapView = new MapView(this);
//			mapView.MapMaxZoomLevel = 20;
//			mapView.MapMinZoomLevel = 12;
//			//var mapView = new MapGLView (this);
//			mapView.Map = map;
//			//mapView.Center = new GeoCoordinate(51.158075, 2.961545); // gistel
//			//mapView.MapCenter = new GeoCoordinate (50.88672, 3.23899);
//			mapView.MapCenter = new GeoCoordinate(51.26337, 4.78739);
//			//mapView.Center = new GeoCoordinate(51.156803, 2.958887);
//			mapView.MapZoomLevel = 15;

//			var mapView = new OpenGLRenderer2D(
//				this, null);

            _mapView = new MapView<MapGLView>(this, new MapGLView(this));

//            _mapView = new MapView<MapViewSurface>(this, new MapViewSurface(this));
            _mapView.Map = map;

            (_mapView as IMapView).AutoInvalidate = true;
            _mapView.MapMaxZoomLevel = 20;
            _mapView.MapMinZoomLevel = 12;
            _mapView.MapTilt = 0;
            //var mapView = new MapGLView (this);
            _mapView.MapCenter = new GeoCoordinate(51.158075, 2.961545); // gistel
            //mapView.MapCenter = new GeoCoordinate (50.88672, 3.23899);
            //mapLayout.MapCenter = new GeoCoordinate(51.26337, 4.78739);
            //mapView.Center = new GeoCoordinate(51.156803, 2.958887);
            _mapView.MapZoom = 12;
            //MapViewAnimator mapViewAnimator = new MapViewAnimator(mapLayout);
            _mapView.MapTapEvent += delegate(GeoCoordinate geoCoordinate)
            {
                //mapViewAnimator.Stop();
                //mapViewAnimator.Start(geoCoordinate, 15, new TimeSpan(0, 0, 2));
            };

			//Create the user interface in code
			var layout = new RelativeLayout (this);

			//layout.AddView(mapGLView);
            layout.AddView(_mapView);

            //Timer timer = new Timer(50);
            //timer.Elapsed += new ElapsedEventHandler(TimerHandler);
            //timer.Start();

			SetContentView (layout);
		}

        private MapView<MapGLView> _mapView;

        private RouteTracker _routeTracker;

        private IEnumerator<GeoCoordinate> _enumeratorNext;

        private IEnumerator<GeoCoordinate> _enumerator;

        private void TimerHandler(object sender, ElapsedEventArgs e)
        {
            this.MoveNext();
        }

        private void MoveNext()
        {
            if (_enumerator.MoveNext())
            {
                _mapView.MapCenter = _enumerator.Current;

                if (_enumeratorNext.MoveNext())
                {
                    IProjection projection = _mapView.Map.Projection;

                    VectorF2D direction = new PointF2D(projection.ToPixel(_enumeratorNext.Current)) -
                        new PointF2D(projection.ToPixel(_enumerator.Current));

                    _mapView.MapTilt = direction.Angle(new VectorF2D(0, -1));
                }

                (_mapView as IMapView).Invalidate();

                _routeTracker.Track(_enumerator.Current);

                if(_routeTracker.NextInstruction != null)
                {
                    OsmSharp.Logging.Log.TraceEvent("MainActivity", System.Diagnostics.TraceEventType.Information,
                        "Instruction: {0}@{1}", _routeTracker.NextInstruction.Text, _routeTracker.DistanceNextInstruction.ToString());
                }
            }
        }
	}
}
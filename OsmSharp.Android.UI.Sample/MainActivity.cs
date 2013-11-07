using System;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;
using Android.App;
using Android.OS;
using Android.Widget;
using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.Navigation;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.UI;
using OsmSharp.UI.Animations.Navigation;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Renderer.Scene;

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
        /// Holds the text view.
        /// </summary>
        private TextView _textView;

		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterListener(
                new OsmSharp.Android.UI.Log.LogTraceListener());
			
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
					Assembly.GetExecutingAssembly().GetManifestResourceStream(
                        @"OsmSharp.Android.UI.Sample.kempen-big.osm.pbf.scene.layered"), true)));

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

            var from = new GeoCoordinate(51.261203, 4.780760);
            var to = new GeoCoordinate(51.267797, 4.801362);

            var routingSerializer = 
                new OsmSharp.Routing.CH.Serialization.Sorted.v2.CHEdgeDataDataSourceSerializer(false);
            var graphDeserialized = routingSerializer.Deserialize(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Android.UI.Sample.kempen-big.osm.pbf.routing"), true);

            _router = Router.CreateCHFrom(
                graphDeserialized, new CHRouter(),
                new OsmRoutingInterpreter());
            RouterPoint routerPoint1 = _router.Resolve(Vehicle.Car, from);
            RouterPoint routerPoint2 = _router.Resolve(Vehicle.Car, to);
            Route route1 = _router.Calculate(Vehicle.Car, routerPoint1, routerPoint2);
            RouteTracker routeTracker = new RouteTracker(route1, new OsmRoutingInterpreter());
            _enumerator = route1.GetRouteEnumerable(20).GetEnumerator();

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

//            _mapView = new MapView<MapGLView>(this, new MapGLView(this));

            _mapView = new MapView(this, new MapViewSurface(this));
            //_mapView = new MapView(this, new MapViewGLSurface(this));
            _mapView.Map = map;

            (_mapView as IMapView).AutoInvalidate = true;
            _mapView.MapMaxZoomLevel = 20;
            _mapView.MapMinZoomLevel = 12;
            _mapView.MapTilt = 0;
            _mapView.MapCenter = new GeoCoordinate(51.26371, 4.78601);
            _mapView.MapZoom = 17;

            _textView = new TextView(this);
            _textView.SetBackgroundColor(global::Android.Graphics.Color.White);
            _textView.SetTextColor(global::Android.Graphics.Color.Black);

			//Create the user interface in code
            var layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;
            layout.AddView(_textView);
            layout.AddView(_mapView);

            _mapView.AddMarker(from);
            _mapView.AddMarker(to);

            //_mapView.ZoomToMarkers();

            _routeTrackerAnimator = new RouteTrackerAnimator(_mapView, routeTracker, 5);

            Timer timer = new Timer(500);
            timer.Elapsed += new ElapsedEventHandler(TimerHandler);
            timer.Start();

			SetContentView (layout);
		}

        void MainActivity_Click(object sender, EventArgs e)
        {
            if (sender is MapMarker)
            {
                var marker = sender as MapMarker;
                _mapView.RemoveMarker(marker);
            }
        }

        private MapView _mapView;

        private RouteTrackerAnimator _routeTrackerAnimator;

        private IEnumerator<GeoCoordinate> _enumerator;

        private void TimerHandler(object sender, ElapsedEventArgs e)
        {
            //_mapView.ZoomToMarkers();
            this.MoveNext();
        }

        private void MoveNext()
        {
            if (_enumerator.MoveNext())
            {
                GeoCoordinate other = _enumerator.Current.OffsetRandom(10);
                _routeTrackerAnimator.Track(other);
                if (_routeTrackerAnimator.NextInstruction != null)
                {
                    this.RunOnUiThread(() =>
                    {
                        _textView.Text = _routeTrackerAnimator.NextInstruction.Text;
                    });
                }
            }
        }
	}
}
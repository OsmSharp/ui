using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using System.Reflection;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using System.Timers;
using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.UI;
using System.Collections.Generic;
using OsmSharp.Math;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.TSP.Genetic;
using OsmSharp.UI.Animations;
using OsmSharp.UI.Animations.Navigation;
using OsmSharp.Routing.Navigation;
using OsmSharp.Routing.Instructions;
using System.Threading.Tasks;

namespace OsmSharp.iOS.UI.Sample
{
	public partial class OsmSharp_iOS_UI_SampleViewController : UIViewController
	{
		/// <summary>
		/// Holds the router.
		/// </summary>
		private Router _router;

		private MapView _mapView;

		/// <summary>
		/// Holds the route layer.
		/// </summary>
		private LayerRoute _routeLayer;

		public OsmSharp_iOS_UI_SampleViewController () : base ("OsmSharp_iOS_UI_SampleViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void LoadView ()
		{
			OsmSharp.Logging.Log.Enable ();

			base.LoadView ();

			// initialize a test-map.
			var map = new Map ();
			map.AddLayer (new LayerScene (Scene2DLayered.Deserialize (
				Assembly.GetExecutingAssembly ().GetManifestResourceStream ("OsmSharp.iOS.UI.Sample.wvl.map"), 
					true)));

			// Perform any additional setup after loading the view, typically from a nib.
			MapView mapView = new MapView ();
			_mapView = mapView;
			//mapViewAnimator = new MapViewAnimator (mapView);
			mapView.Map = map;
			mapView.MapCenter = new GeoCoordinate(51.158075, 2.961545); // gistel
//			mapView.MapTapEvent+= delegate(GeoCoordinate geoCoordinate) {
//				mapView.AddMarker(geoCoordinate).TouchDown  += MapMarkerClicked;
//			};

			mapView.MapMaxZoomLevel = 18;
			mapView.MapMinZoomLevel = 12;
			mapView.MapZoom = 16;
			mapView.MapTilt = 30;

			var routingSerializer = new OsmSharp.Routing.CH.Serialization.Sorted.CHEdgeDataDataSourceSerializer(false);
			var graphDeserialized = routingSerializer.Deserialize(
				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.iOS.UI.Sample.wvl.routing"), true);

			_router = Router.CreateCHFrom(
				graphDeserialized, new CHRouter(graphDeserialized),
				new OsmRoutingInterpreter());

//			long before = DateTime.Now.Ticks;
//
//			var routeLocations = new GeoCoordinate[] {
//				new GeoCoordinate (50.8247730, 2.7524706),
//				new GeoCoordinate (50.8496394, 2.7301512),
//				new GeoCoordinate (50.8927741, 2.6138545),
//				new GeoCoordinate (50.8296363, 2.8869437)
//			};
//
//			var routerPoints = new RouterPoint[routeLocations.Length];
//			for (int idx = 0; idx < routeLocations.Length; idx++) {
//				routerPoints [idx] = router.Resolve (Vehicle.Car, routeLocations [idx]);
//
//				mapView.AddMarker (routeLocations [idx]);
//			}
//			OsmSharp.Routing.TSP.RouterTSPWrapper<RouterTSPAEXGenetic> tspRouter = new OsmSharp.Routing.TSP.RouterTSPWrapper<RouterTSPAEXGenetic> (
//				new RouterTSPAEXGenetic (10, 20), router);
//
//			Route route = tspRouter.CalculateTSP (Vehicle.Car, routerPoints);
//
//			long after = DateTime.Now.Ticks;
//
//			OsmSharp.Logging.Log.TraceEvent("OsmSharp.Android.UI.MapView", System.Diagnostics.TraceEventType.Information,"Routing & TSP in {0}ms", 
//			                                new TimeSpan (after - before).TotalMilliseconds);
			// 51.160477" lon="2.961497
			GeoCoordinate point1 = new GeoCoordinate(51.158075, 2.961545);
			GeoCoordinate point2 = new GeoCoordinate(51.190503, 3.004793);

			//GeoCoordinate point1 = new GeoCoordinate(51.159132, 2.958755);
			//GeoCoordinate point2 = new GeoCoordinate(51.160477, 2.961497);
			RouterPoint routerPoint1 = _router.Resolve(Vehicle.Car, point1);
			RouterPoint routerPoint2 = _router.Resolve(Vehicle.Car, point2);
			Route route1 = _router.Calculate(Vehicle.Car, routerPoint1, routerPoint2);
			_enumerator = route1.GetRouteEnumerable(10).GetEnumerator();

			//List<Instruction> instructions = InstructionGenerator.Generate(route1, new OsmRoutingInterpreter());
//
			_routeLayer = new LayerRoute(map.Projection);
			_routeLayer.AddRoute (route1);
			map.AddLayer(_routeLayer);

			View = mapView;

			mapView.AddMarker(new GeoCoordinate(51.1612, 2.9795));
			mapView.AddMarker(new GeoCoordinate(51.1447, 2.9483));
			mapView.AddMarker(point1);
			mapView.AddMarker(point2);
			mapView.AddMarker(new GeoCoordinate(51.1612, 2.9795));
			mapView.AddMarker(new GeoCoordinate(51.1447, 2.9483));
			mapView.AddMarker(new GeoCoordinate(51.1612, 2.9795));
			mapView.AddMarker(new GeoCoordinate(51.1447, 2.9483));
//
//			//mapView.ZoomToMarkers();

			//GeoCoordinateBox box = new GeoCoordinateBox (new GeoCoordinate[] { point1, point2 });
//
			mapView.MapTapEvent += delegate(GeoCoordinate geoCoordinate)
			{
				//_routeTrackerAnimator.Track(box.GenerateRandomIn());

				//_mapView.AddMarker(geoCoordinate).Click += new EventHandler(MainActivity_Click);
				//mapViewAnimator.Stop();
				//mapViewAnimator.Start(geoCoordinate, 15, new TimeSpan(0, 0, 2));
			};
			
			RouteTracker routeTracker = new RouteTracker(route1, new OsmRoutingInterpreter());
			_routeTrackerAnimator = new RouteTrackerAnimator(mapView, routeTracker, 5);
//
//				Timer timer = new Timer (150);
//				timer.Elapsed += new ElapsedEventHandler (TimerHandler);
//				timer.Start ();
//
//			Task.Factory.StartNew (() => {
//				System.Threading.Thread.Sleep(200); // do something.
//				InvokeOnMainThread (() => {
//					mapView.ZoomToMarkers ();
//				});
//			});
			

			//

		}

//		public override void ViewDidLoad ()
//		{
//			base.ViewDidLoad ();
//
//						// initialize a test-map.
//						var map = new Map ();
//						map.AddLayer (new LayerScene (Scene2DLayered.Deserialize (
//							Assembly.GetExecutingAssembly ().GetManifestResourceStream ("OsmSharp.iOS.UI.Sample.wvl.map"), 
//								true)));
//
//			var mapView = new MapView ();
//			mapView.Frame = this.View.Frame;
//			mapView.AutoresizingMask = this.View.AutoresizingMask;
//						_mapView = mapView;
//						//mapViewAnimator = new MapViewAnimator (mapView);
//						mapView.Map = map;
//						mapView.MapCenter = new GeoCoordinate(51.158075, 2.961545); // gistel
//			
//						mapView.MapZoom = 18;
//						mapView.MapTilt = 30;
//
//
//		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			
			//_mapView.ZoomToMarkers();
			//
			(_mapView as IMapView).Invalidate ();
		}

		private RouteTrackerAnimator _routeTrackerAnimator;

		private IEnumerator<GeoCoordinate> _enumerator;

		private void TimerHandler(object sender, ElapsedEventArgs e)
		{
			this.InvokeOnMainThread (MoveNext);
		}

		private void MoveNext()
		{
			if (_enumerator.MoveNext())
			{
				GeoCoordinate other = _enumerator.Current.OffsetRandom(10);
				_routeTrackerAnimator.Track(other);

				if (_routeTrackerAnimator.NextInstruction != null) {
					OsmSharp.Logging.Log.TraceEvent ("SampleView", System.Diagnostics.TraceEventType.Information,
					                                _routeTrackerAnimator.NextInstruction.Text);
				}
			}
		}

		private void IncreaseMapTilt()
		{
			//(this.View as MapView).MapTilt = (this.View as MapView).MapTilt + 5;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return true;
		}
	}
}
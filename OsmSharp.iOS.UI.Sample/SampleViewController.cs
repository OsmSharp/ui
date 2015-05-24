// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Timers;
using OsmSharp.Collections.Tags;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.Navigation;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.UI;
using OsmSharp.UI.Animations.Navigation;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.iOS.UI.Controls;
#if __UNIFIED__
using UIKit;
#else
using MonoTouch.UIKit;
#endif

namespace OsmSharp.iOS.UI.Sample
{
	/// <summary>
	/// The sample view controller.
	/// </summary>
	public class SampleViewController : UIViewController
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
		/// Holds the map view.
		/// </summary>
		private MapView _mapView;

		/// <summary>
		/// Holds the router tracker animator.
		/// </summary>
		private RouteTrackerAnimator _routeTrackerAnimator;

		/// <summary>
		/// Holds the coordinate enumerator.
		/// </summary>
		private IEnumerator<GeoCoordinate> _enumerator;

		/// <summary>
		/// Holds the center marker.
		/// </summary>
		private MapMarker _centerMarker;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.Sample.SampleViewController"/> class.
		/// </summary>
		public SampleViewController()
		{

		}

		/// <summary>
		/// Initializes the View property.
		/// </summary>
		public override void LoadView ()
		{
			base.LoadView ();

			// initialize OsmSharp native hooks.
			Native.Initialize();

			// enable the loggging.
			OsmSharp.Logging.Log.Enable();
			OsmSharp.Logging.Log.RegisterListener(new OsmSharp.iOS.UI.Log.ConsoleTraceListener());

			// initialize map.
			var map = new Map();
			// add a tile layer.
			map.AddLayer(new LayerTile(@"http://192.168.43.155:1234/default/{0}/{1}/{2}.png"));
			//            map.AddLayer(new LayerMBTile(SQLiteConnection.CreateFrom(
			//                Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.iOS.UI.Sample.kempen.mbtiles"), "map")));

			// add an online osm-data->mapCSS translation layer.
			//map.AddLayer(new OsmLayer(dataSource, mapCSSInterpreter));
			// add a pre-processed vector data file.
//			var sceneStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
//				"OsmSharp.iOS.UI.Sample.default.map");
//			map.AddLayer(new LayerScene(Scene2D.Deserialize(sceneStream, true)));

			//            var primitivesLayer = new LayerPrimitives(map.Projection);
			//            primitivesLayer.AddPoint(new GeoCoordinate(51.26371, 4.78601), 10,
			//                SimpleColor.FromKnownColor(KnownColor.Blue).Value);
			//            map.AddLayer(primitivesLayer);

			//			// define dummy from and to points.
			var from = new GeoCoordinate(51.261203, 4.780760);
			var to = new GeoCoordinate(51.267797, 4.801362);
			//
			//			// deserialize the pre-processed graph.
			//			var routingSerializer = new CHEdgeDataDataSourceSerializer(false);
			//			TagsCollectionBase metaData = null;
			//			var graphStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
			//				"OsmSharp.iOS.UI.Sample.kempen-big.osm.pbf.routing");
			//			var graphDeserialized = routingSerializer.Deserialize(graphStream, out metaData, true);
			//
			//			// initialize router.
			//			_router = Router.CreateCHFrom(graphDeserialized, new CHRouter(), new OsmRoutingInterpreter());
			//
			//			// resolve points.
			//			RouterPoint routerPoint1 = _router.Resolve(Vehicle.Car, from);
			//			RouterPoint routerPoint2 = _router.Resolve(Vehicle.Car, to);
			//
			//			// calculate route.
			//			Route route = _router.Calculate(Vehicle.Car, routerPoint1, routerPoint2);
			//			RouteTracker routeTracker = new RouteTracker(route, new OsmRoutingInterpreter());
			//			_enumerator = route.GetRouteEnumerable(10).GetEnumerator();
			//
			//			// add a router layer.
			//			_routeLayer = new LayerRoute(map.Projection);
			//			_routeLayer.AddRoute (route, SimpleColor.FromKnownColor(KnownColor.Blue, 125).Value, 12);
			//			map.AddLayer(_routeLayer);

			// define the mapview.
			_mapView = new MapView();
			//_mapView.MapTapEvent += new MapViewEvents.MapTapEventDelegate(_mapView_MapTapEvent);
			_mapView.MapAllowTilt = false;
			_mapView.Map = map;
			_mapView.MapMaxZoomLevel = 19;
			_mapView.MapMinZoomLevel = 0;
			_mapView.MapTilt = 0;
			_mapView.MapCenter = new GeoCoordinate(51.2633, 4.7853);
			_mapView.MapZoom = 18;
			_mapView.MapInitialized += _mapView_MapInitialized;

			// add markers.
			var marker = _mapView.AddMarker (from);
			var popupTextView = new UITextView();
			popupTextView.Text = "Hey, this is popup text!";
			popupTextView.BackgroundColor = UIColor.FromWhiteAlpha(0.5f, 0.5f);
			marker.AddPopup(popupTextView, 100, 100);
			marker = _mapView.AddMarker (to);
			popupTextView = new UITextView();
			popupTextView.Text = "Hey, this is another popup text!";
			popupTextView.BackgroundColor = UIColor.FromWhiteAlpha(0.5f, 0.5f);
			marker.AddPopup(popupTextView, 100, 100);

			this.AddMarkers();

			// add center marker.
			_centerMarker = _mapView.AddMarker(_mapView.MapCenter);

			// create the route tracker animator.
			// _routeTrackerAnimator = new RouteTrackerAnimator(_mapView, routeTracker, 5, 17);

			//			// simulate a number of gps-location update along the calculated route.
			//			Timer timer = new Timer(250);
			//			timer.Elapsed += new ElapsedEventHandler(TimerHandler);
			//			timer.Start();

			View = _mapView;
		}

		void AddMarkers()
		{
			var from = new GeoCoordinate(51.261203, 4.780760);
			var to = new GeoCoordinate(51.267797, 4.801362);

			var box = new GeoCoordinateBox(from, to);

			_mapView.ClearMarkers();

			MapMarker marker;
			for (int idx = 0; idx < 20; idx++)
			{
				var pos = box.GenerateRandomIn();
				marker = _mapView.AddMarker(pos);
				var popupTextView = new UITextView();
				popupTextView.Text = "Hey, this is popup text!";
				popupTextView.BackgroundColor = UIColor.FromWhiteAlpha(0.5f, 0.5f);
				marker.AddPopup(popupTextView, 100, 100);
			}
		}


		/// <summary>
		/// Called when the map was first initialized.
		/// </summary>
		/// <param name="mapView">Map view.</param>
		/// <param name="newZoom">New zoom.</param>
		/// <param name="newTilt">New tilt.</param>
		/// <param name="newCenter">New center.</param>
		private void _mapView_MapInitialized(OsmSharp.UI.IMapView mapView, float newZoom, OsmSharp.Units.Angle.Degree newTilt, GeoCoordinate newCenter)
		{ 
			// make sure the center marker stays in place from now on.
			_centerMarker.MoveWithMap = false;
		}

		/// <summary>
		/// Handles the timer event from the timer.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimerHandler(object sender, ElapsedEventArgs e)
		{
			if (_enumerator.MoveNext())
			{ // move to the next dummy gps location.
				// randomize the route to simulate actual gps location data.
				GeoCoordinate other = _enumerator.Current.OffsetRandom(10);

				// git the location to the route tracker.
				_routeTrackerAnimator.Track(other);
			}
		}
	}
}
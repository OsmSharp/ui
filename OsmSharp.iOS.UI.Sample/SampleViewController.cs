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
using MonoTouch.UIKit;
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
using OsmSharp.Routing.CH.Serialization.Sorted.v2;

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

			// enable the logggin.
			OsmSharp.Logging.Log.Enable();
			OsmSharp.Logging.Log.RegisterListener(new OsmSharp.iOS.UI.Log.ConsoleTraceListener());

			// initialize map.
			var map = new Map();
			// add a tile layer.
            // map.AddLayer(new LayerTile(new NativeImageCache(), @"http://otile1.mqcdn.com/tiles/1.0.0/osm/{0}/{1}/{2}.png"));
            map.AddLayer(new LayerMBTile(new NativeImageCache(), SQLiteConnection.CreateFrom(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.iOS.UI.Sample.map.mbtiles"), "map")));

			// add an online osm-data->mapCSS translation layer.
			//map.AddLayer(new OsmLayer(dataSource, mapCSSInterpreter));
			// add a pre-processed vector data file.
//			var sceneStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
//				"OsmSharp.iOS.UI.Sample.default.map");
//			map.AddLayer(new LayerScene(Scene2D.Deserialize(sceneStream, true)));

			// define dummy from and to points.
			var from = new GeoCoordinate(51.261203, 4.780760);
			var to = new GeoCoordinate(51.267797, 4.801362);

			// deserialize the pre-processed graph.
			var routingSerializer = new CHEdgeDataDataSourceSerializer(false);
			TagsCollectionBase metaData = null;
			var graphStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
				"OsmSharp.iOS.UI.Sample.kempen-big.osm.pbf.routing");
			var graphDeserialized = routingSerializer.Deserialize(graphStream, out metaData, true);

			// initialize router.
			_router = Router.CreateCHFrom(graphDeserialized, new CHRouter(), new OsmRoutingInterpreter());

			// resolve points.
			RouterPoint routerPoint1 = _router.Resolve(Vehicle.Car, from);
			RouterPoint routerPoint2 = _router.Resolve(Vehicle.Car, to);

			// calculate route.
			Route route = _router.Calculate(Vehicle.Car, routerPoint1, routerPoint2);
			RouteTracker routeTracker = new RouteTracker(route, new OsmRoutingInterpreter());
			_enumerator = route.GetRouteEnumerable(10).GetEnumerator();

			// add a router layer.
			_routeLayer = new LayerRoute(map.Projection);
			_routeLayer.AddRoute (route, SimpleColor.FromKnownColor(KnownColor.Blue, 125).Value, 12);
			map.AddLayer(_routeLayer);

			// define the mapview.
			_mapView = new MapView();
			//_mapView.MapTapEvent += new MapViewEvents.MapTapEventDelegate(_mapView_MapTapEvent);
            _mapView.MapAllowTilt = false;
			_mapView.Map = map;
            _mapView.MapMaxZoomLevel = 19;
            _mapView.MapMinZoomLevel = 0;
			_mapView.MapTilt = 0;
			_mapView.MapCenter = new GeoCoordinate(51.26371, 4.78601);
			_mapView.MapZoom = 18;

			// add markers.
			_mapView.AddMarker (from);
			_mapView.AddMarker (to);

			// create the route tracker animator.
			_routeTrackerAnimator = new RouteTrackerAnimator(_mapView, routeTracker, 5, 17);

//			// simulate a number of gps-location update along the calculated route.
//			Timer timer = new Timer(250);
//			timer.Elapsed += new ElapsedEventHandler(TimerHandler);
//			timer.Start();

			View = _mapView;
		}

		/// <summary>
		/// Views the did appear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			(_mapView as IMapView).Invalidate ();
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
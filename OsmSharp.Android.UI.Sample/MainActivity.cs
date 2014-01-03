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

using Android.App;
using Android.OS;
using Android.Widget;
using OsmSharp.Collections.Tags;
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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;

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

			// initialize map.
			var map = new Map();
			//map.AddLayer(new LayerTile(@"http://otile1.mqcdn.com/tiles/1.0.0/osm/{0}/{1}/{2}.png"));
			//map.AddLayer(new OsmLayer(dataSource, mapCSSInterpreter));
//			map.AddLayer(new LayerScene(Scene2DSimple.Deserialize(
//							Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.wvl.osm.pbf.scene.simple"), true)));
			map.AddLayer(
				new LayerScene(
				    Scene2D.Deserialize(
					    Assembly.GetExecutingAssembly().GetManifestResourceStream(
                            @"OsmSharp.Android.UI.Sample.kempen-big/osm.pbf.scene.layered"), true)));

            var from = new GeoCoordinate(51.261203, 4.780760);
            var to = new GeoCoordinate(51.267797, 4.801362);

            var routingSerializer = 
                new OsmSharp.Routing.CH.Serialization.Sorted.v2.CHEdgeDataDataSourceSerializer(false);
            TagsCollectionBase metaData = null;
            var graphDeserialized = routingSerializer.Deserialize(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Android.UI.Sample.kempen-big.osm.pbf.routing"), out metaData, true);

            _router = Router.CreateCHFrom(
                graphDeserialized, new CHRouter(),
                new OsmRoutingInterpreter());
            RouterPoint routerPoint1 = _router.Resolve(Vehicle.Car, from);
            RouterPoint routerPoint2 = _router.Resolve(Vehicle.Car, to);
            Route route1 = _router.Calculate(Vehicle.Car, routerPoint1, routerPoint2);
            RouteTracker routeTracker = new RouteTracker(route1, new OsmRoutingInterpreter());
            _enumerator = route1.GetRouteEnumerable(10).GetEnumerator();

            _routeLayer = new LayerRoute(map.Projection);
            _routeLayer.AddRoute (route1, SimpleColor.FromKnownColor(KnownColor.Blue, 125).Value, 12);
            map.AddLayer(_routeLayer);

            _mapView = new MapView(this, new MapViewSurface(this));
            //_mapView = new MapView(this, new MapViewGLSurface(this));
            _mapView.MapTapEvent += new MapViewEvents.MapTapEventDelegate(_mapView_MapTapEvent);
            _mapView.Map = map;

            _mapView.MapAllowPan = true;
            _mapView.MapAllowTilt = true;
            _mapView.MapAllowZoom = true;

            (_mapView as IMapView).AutoInvalidate = true;
            _mapView.MapMaxZoomLevel = 20;
            _mapView.MapMinZoomLevel = 10;
            _mapView.MapTilt = 0;
            _mapView.MapCenter = new GeoCoordinate(51.26371, 4.78601);
            _mapView.MapZoom = 16;

            _textView = new TextView(this);
            _textView.SetBackgroundColor(global::Android.Graphics.Color.White);
            _textView.SetTextColor(global::Android.Graphics.Color.Black);

			//Create the user interface in code
            var layout = new LinearLayout(this);
            layout.Orientation = Orientation.Vertical;
            layout.AddView(_textView);
            layout.AddView(_mapView);

            //_mapView.AddMarker(from);
            //_mapView.AddMarker(to);

            //_mapView.ZoomToMarkers();

            _routeTrackerAnimator = new RouteTrackerAnimator(_mapView, routeTracker, 5, 17);

            //Timer timer = new Timer(500);
            //timer.Elapsed += new ElapsedEventHandler(TimerHandler);
            //timer.Start();

			SetContentView (layout);
		}

        void _mapView_MapTapEvent(GeoCoordinate coordinate)
        {
            _mapView.AddMarker(coordinate);
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
                GeoCoordinate other = _enumerator.Current.OffsetRandom(1);
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
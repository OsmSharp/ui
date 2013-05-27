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
using OsmSharp.Osm.Simple;
using System.Collections.Generic;
using OsmSharp.UI;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.Osm.Data.PBF.Processor;
using OsmSharp.Osm.Data.Core.Memory;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data.Xml.Processor;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Route;

namespace OsmSharp.Android.UI.Sample
{
	/// <summary>
	/// Activity1.
	/// </summary>
	[Activity (MainLauncher = true)]
    public class MainActivity : Activity
	{
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

			// load mapcss style interpreter.
			var mapCSSInterpreter = new MapCSSInterpreter(
				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.mapcss"),
				imageSource);
			
			// initialize the data source.
			var dataSource = new MemoryDataSource();
//			var source = new XmlOsmStreamReader(
//				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.osm"));
			var source = new PBFOsmStreamReader(
				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.osm.pbf"));
			dataSource.PullFromSource(source);

			// initialize map.
			var map = new Map();
			//map.AddLayer(new OsmLayer(dataSource, mapCSSInterpreter));
			map.AddLayer(new LayerScene(Scene2D.Deserialize(
				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.osm.pbf.scene"), true)));

//			var routingSerializer = new V2RoutingLiveEdgeSerializer(true);
//			var graphSerialized = routingSerializer.Deserialize(
//				//Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.osm.pbf.routing.3"));
//				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.wvl.pbf.routing.4"));
//
//			var graphLayer = new LayerDynamicGraphLiveEdge(graphSerialized, mapCSSInterpreter);
//			map.AddLayer(graphLayer);
			
//			// calculate route.            
//			Router router = Router.CreateLiveFrom(
//				graphSerialized,
//				new OsmRoutingInterpreter());
//			OsmSharpRoute route = router.Calculate(VehicleEnum.Car, 
//			                                       router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.15136, 3.19462)),
//			                                       router.Resolve(VehicleEnum.Car, new GeoCoordinate(51.075023, 3.096632)));
//			var osmSharpLayer = new LayerOsmSharpRoute(map.Projection);
//			osmSharpLayer.AddRoute(route);
//			map.AddLayer(osmSharpLayer);

//			// create gpx layer.
//			LayerGpx gpxLayer = new LayerGpx(map.Projection);
//			gpxLayer.AddGpx(
//				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.gpx"));
//			map.AddLayer(gpxLayer);
			
			// set control properties.
			var mapView = new MapView(this);
			mapView.Map = map;
			//mapView.Center = new GeoCoordinate(51.075023, 3.096632);
//			mapView.Center = new GeoCoordinate(51.26337, 4.78739);
			mapView.Center = new GeoCoordinate(51.156803, 2.958887);
			mapView.ZoomLevel = 16;

//			var mapView = new OpenGLRenderer2D(
//				this, null);

			//var mapGLView = new MapGLView(this);

			//Create the user interface in code
			var layout = new LinearLayout (this);
			layout.Orientation = Orientation.Vertical;

			//layout.AddView(mapGLView);
			layout.AddView (mapView);

			SetContentView (layout);
		}
	}
}
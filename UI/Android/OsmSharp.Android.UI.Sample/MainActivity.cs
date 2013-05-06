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
using OsmSharp.Osm.Data.XML.Processor;
using OsmSharp.Osm.Data.Core.Processor.List;
using OsmSharp.Osm.Simple;
using System.Collections.Generic;
using OsmSharp.Tools;
using OsmSharp.UI;
using OsmSharp.Tools.Math.Geo.Projections;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.Osm.Data.Raw.XML.OsmSource;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.Osm.Data.Core.Memory;
using OsmSharp.Osm.Data.PBF.Raw.Processor;

namespace OsmSharp.Android.UI.Sample
{
	/// <summary>
	/// Activity1.
	/// </summary>
	[Activity (Label = "OsmSharp.Android.UI.Sample", MainLauncher = true)]
	public class Activity1 : Activity
	{
		/// <summary>
		/// Raises the create event.
		/// </summary>
		/// <param name="bundle">Bundle.</param>
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
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
//			XmlDataProcessorSource source = new XmlDataProcessorSource(
//				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.osm"));
			var source = new PBFDataProcessorSource(
				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.osm.pbf"));
			dataSource.PullFromSource(source);
			
			// initialize map.
			var map = new Map();
			map.AddLayer(new OsmRawLayer(dataSource, mapCSSInterpreter));
			
			// set control properties.
			var mapView = new MapView(this);
			mapView.Map = map;
			mapView.Center = new GeoCoordinate(51.26337, 4.78739);
			mapView.ZoomFactor = 2; // TODO: improve zoomfactor.

//			var mapView = new OpenGLRenderer2D(
//				this, null);

			//Create the user interface in code
			var layout = new LinearLayout (this);
			layout.Orientation = Orientation.Vertical;

			layout.AddView (mapView);

			SetContentView (layout);
		}
	}
}
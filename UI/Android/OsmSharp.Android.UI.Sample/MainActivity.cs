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

			// load test osm file.
			List<SimpleOsmGeo> osmList = new List<SimpleOsmGeo>();
			Stream stream = 
				Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Android.UI.Sample.test.osm");
			XmlDataProcessorSource xmlDataProcessorSource = new XmlDataProcessorSource(
				stream);
			CollectionDataProcessorTarget collectionDataProcessorTarget = new CollectionDataProcessorTarget(
				osmList);
			collectionDataProcessorTarget.RegisterSource(xmlDataProcessorSource);
			collectionDataProcessorTarget.Pull();

			// build a scene using spherical mercator.
			Scene2D scene2D = new Scene2D();
			// build a scene using spherical mercator.
			EllipticalMercator sphericalMercator = new EllipticalMercator();
			Dictionary<long, GeoCoordinate> nodes = new Dictionary<long, GeoCoordinate>();
			foreach (SimpleOsmGeo simpleOsmGeo in osmList)
			{
				if (simpleOsmGeo is SimpleNode)
				{
					SimpleNode simplenode = (simpleOsmGeo as SimpleNode);
					double[] point = sphericalMercator.ToPixel(
						simplenode.Latitude.Value, simplenode.Longitude.Value);
					nodes.Add(simplenode.Id.Value, new GeoCoordinate(simplenode.Latitude.Value, simplenode.Longitude.Value));
					scene2D.AddPoint((float)point[0], (float)point[1],
					                 SimpleColor.FromKnownColor(KnownColor.Yellow).Value,
					                 2);
				}
				else if (simpleOsmGeo is SimpleWay)
				{
					SimpleWay way = (simpleOsmGeo as SimpleWay);
					List<float> x = new List<float>();
					List<float> y = new List<float>();
					if (way.Nodes != null)
					{
						for (int idx = 0; idx < way.Nodes.Count; idx++)
						{
							GeoCoordinate nodeCoords;
							if (nodes.TryGetValue(way.Nodes[idx], out nodeCoords))
							{
								x.Add((float) sphericalMercator.LongitudeToX(nodeCoords.Longitude));
								y.Add((float) sphericalMercator.LatitudeToY(nodeCoords.Latitude));
							}
						}
					}
					
					if (x.Count > 0)
					{
						scene2D.AddLine(x.ToArray(), y.ToArray(), 
						                SimpleColor.FromKnownColor(KnownColor.Blue).Value, 2);
					}
				}
			}

			//Create the user interface in code
			var layout = new LinearLayout (this);
			layout.Orientation = Orientation.Vertical;

//			// initialize a test-scene.
//			scene2D.AddPoint(0f, 0f, global::Android.Graphics.Color.Blue.ToArgb(), 1);
//			
//			float[] x = new float[]{ 50, -80, 70};
//			float[] y = new float[]{ 20, -10, -70};
//
//			bool fill = false;
//			int color = global::Android.Graphics.Color.White.ToArgb();
//			int width = 1;
//			
//			scene2D.AddPolygon(x, y, color, width, fill);

			// create mapview.
			var mapView = new CanvasRenderer2DView(
				this, scene2D);
			mapView.Center = new float[] { 534463.21f, 6633094.69f };
			mapView.ZoomFactor = 1;

			layout.AddView (mapView);

			SetContentView (layout);
		}
	}
}
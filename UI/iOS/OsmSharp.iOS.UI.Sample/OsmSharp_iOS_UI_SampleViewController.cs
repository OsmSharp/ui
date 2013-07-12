using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using System.Reflection;
using OsmSharp.UI.Renderer;

namespace OsmSharp.iOS.UI.Sample
{
	public partial class OsmSharp_iOS_UI_SampleViewController : UIViewController
	{
		public OsmSharp_iOS_UI_SampleViewController () : base ("OsmSharp_iOS_UI_SampleViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// initialize a test-map.
			var map = new Map ();
			map.AddLayer (new LayerScene (Scene2D.Deserialize (
				Assembly.GetExecutingAssembly ().GetManifestResourceStream ("OsmSharp.iOS.UI.Sample.wvl.osm.pbf.scene.simple"), true)));
			
			// Perform any additional setup after loading the view, typically from a nib.
			MapView mapView = new MapView ();
			mapView.Map = map;
			mapView.MapCenter = new OsmSharp.Math.Geo.GeoCoordinate (51.158075, 2.961545);
			mapView.ZoomLevel = 8;

			View.BackgroundColor = UIColor.Black;
			mapView.Bounds = this.View.Bounds;
			View.AddSubview (mapView);
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return true;
		}
	}
}


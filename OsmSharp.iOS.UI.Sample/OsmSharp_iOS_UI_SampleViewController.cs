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

		public override void LoadView ()
		{
			base.LoadView ();

			// initialize a test-map.
			var map = new Map ();
			map.AddLayer (new LayerScene (Scene2DLayered.Deserialize (
				Assembly.GetExecutingAssembly ().GetManifestResourceStream ("OsmSharp.iOS.UI.Sample.wvl.map"), 
					true)));

			// Perform any additional setup after loading the view, typically from a nib.
			MapView mapView = new MapView ();
			mapView.Map = map;
			mapView.MapCenter = new OsmSharp.Math.Geo.GeoCoordinate(51.158075, 2.961545); // gistel
			mapView.MapTapEvent+= delegate(GeoCoordinate geoCoordinate) {
				mapView.AddMarker(geoCoordinate).TouchDown  += MapMarkerClicked;
			};

			mapView.MapZoomLevel = 16;
			mapView.MapTilt = 30;

			View = mapView;

			Timer timer = new Timer (5000);
			timer.Elapsed += new ElapsedEventHandler (TimerHandler);
			timer.Start ();
		}

		private void MapMarkerClicked(object sender, EventArgs e)
		{
			if (sender is MapMarker) {
				OsmSharp.Logging.Log.TraceEvent ("Temp", System.Diagnostics.TraceEventType.Verbose, "Marker was touched!");
			}
		}

		private void TimerHandler(object sender, ElapsedEventArgs e)
		{
			//this.InvokeOnMainThread (IncreaseMapTilt);
		}

		private void IncreaseMapTilt()
		{
			(this.View as MapView).MapTilt = (this.View as MapView).MapTilt + 5;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return true;
		}
	}
}
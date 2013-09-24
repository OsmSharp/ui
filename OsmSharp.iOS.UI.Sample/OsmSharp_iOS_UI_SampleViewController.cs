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
			//mapView.MapCenter = new OsmSharp.Math.Geo.GeoCoordinate (51.26337, 4.78739);
			mapView.MapCenter = new OsmSharp.Math.Geo.GeoCoordinate(51.158075, 2.961545); // gistel
			//mapView.MapCenter = new GeoCoordinate (50.88672, 3.23899);
			//mapLayout.MapCenter = new GeoCoordinate(51.26337, 4.78739);
			//mapView.Center = new GeoCoordinate(51.156803, 2.958887);

			mapView.MapZoomLevel = 16;
			mapView.MapTilt = 30;

			View = mapView;

//			View.BackgroundColor = UIColor.Black;
//			mapView.Bounds = this.View.Bounds;
//			View.AddSubview (mapView);

			MapMarker marker = mapView.AddMarker(new OsmSharp.Math.Geo.GeoCoordinate(51.158075, 2.961545));
			marker.TouchDown += HandleTouchDown;

			Timer timer = new Timer (5000);
			timer.Elapsed += new ElapsedEventHandler (TimerHandler);
			timer.Start ();
		}

		private void TimerHandler(object sender, ElapsedEventArgs e)
		{
			//this.InvokeOnMainThread (IncreaseMapTilt);
		}

		protected void HandleTouchDown (object sender, System.EventArgs e)
		{
			OsmSharp.Logging.Log.TraceEvent ("Temp", System.Diagnostics.TraceEventType.Verbose, "Marker was touched!");
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
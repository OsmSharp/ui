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
using OsmSharp.iOS.UI;
using OsmSharp.Logging;
using System.Threading;

namespace OsmSharp.iOS.Test.Performance
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
            var textView = new UITextView();

			OsmSharp.Logging.Log.Enable ();
            OsmSharp.Logging.Log.RegisterListener(
                new OsmSharp.iOS.UI.Log.TextViewTraceListener(textView));

			base.LoadView ();

            View = textView;
            
            // do some testing here.
            Thread thread = new Thread(
                new ThreadStart(Test));
            thread.Start();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
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

        /// <summary>
        /// Executes performance tests.
        /// </summary>
        private void Test()
        {
            this.TestRouting("OsmSharp.Android.Test.Performance.kempen-big.osm.pbf.routing");

            this.TestInstructions("OsmSharp.Android.Test.Performance.kempen-big.osm.pbf.routing");
        }

        /// <summary>
        /// Executes routing performance tests.
        /// </summary>
        private void TestRouting(string embeddedResource)
        {
            Log.TraceEvent("Test", System.Diagnostics.TraceEventType.Information,
                           "Testing: 1 route.");
            OsmSharp.Test.Performance.Routing.CH.CHSerializedRoutingTest.Test(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                embeddedResource),
                1);
            Log.TraceEvent("Test", System.Diagnostics.TraceEventType.Information,
                           "Testing: 2 routes.");
            OsmSharp.Test.Performance.Routing.CH.CHSerializedRoutingTest.Test(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                embeddedResource),
                2);
            Log.TraceEvent("Test", System.Diagnostics.TraceEventType.Information,
                           "Testing: 100 routes.");
            OsmSharp.Test.Performance.Routing.CH.CHSerializedRoutingTest.Test(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                embeddedResource),
                100);
        }

        /// <summary>
        /// Executes routing instruction generation tests.
        /// </summary>
        /// <param name="embeddedResource"></param>
        private void TestInstructions(string embeddedResource)
        {
            Log.TraceEvent("Test", System.Diagnostics.TraceEventType.Information,
                           "Testing: Instruction generation.");
            OsmSharp.Test.Performance.Routing.CH.CHSerializedRoutingTest.TestSerializeRoutingInstrictions(
                "CHInstructions",
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                embeddedResource),
                new GeoCoordinate(51.261203, 4.780760),
                new GeoCoordinate(51.267797, 4.801362));
        }
	}
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using OsmSharp.Logging;
using OsmSharp.Math;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.Instructions;
using OsmSharp.Routing.Navigation;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.UI;
using OsmSharp.UI.Animations;
using OsmSharp.UI.Animations.Navigation;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.iOS.UI;
#if __UNIFIED__
using Foundation;
using UIKit;
using CoreGraphics;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

using CGRect = global::System.Drawing.RectangleF;
using CGSize = global::System.Drawing.SizeF;
using CGPoint = global::System.Drawing.PointF;
#endif

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

            // set the seed manually.
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            OsmSharp.Logging.Log.Ignore("RTreeStreamIndex");
            OsmSharp.Logging.Log.Ignore("Scene2DLayeredSource");
            OsmSharp.Logging.Log.Ignore("Renderer2D");

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
            this.TestRouting("OsmSharp.iOS.Test.Performance.kempen-big.osm.pbf.contracted.mobile.routing");

			this.TestRoutingResolved("OsmSharp.iOS.Test.Performance.kempen-big.osm.pbf.contracted.mobile.routing");

            //this.TestInstructions("OsmSharp.iOS.Test.Performance.kempen-big.osm.pbf.routing");
            
            //this.TestRendering("OsmSharp.iOS.Test.Performance.default.map");
        }
			
		/// <summary>
		/// Executes routing performance tests.
		/// </summary>
		private void TestRouting(string embeddedResource)
		{
			Log.TraceEvent("Test", TraceEventType.Information,
				"Testing: 1 route.");
			OsmSharp.Test.Performance.Routing.CH.CHRoutingTest.TestSerialized(
				Assembly.GetExecutingAssembly().GetManifestResourceStream(
					embeddedResource), true, 1);
			Log.TraceEvent("Test", TraceEventType.Information,
				"Testing: 2 routes.");
			OsmSharp.Test.Performance.Routing.CH.CHRoutingTest.TestSerialized(
				Assembly.GetExecutingAssembly().GetManifestResourceStream(
					embeddedResource), true, 2);
			Log.TraceEvent("Test", TraceEventType.Information,
				"Testing: 10 routes.");
			OsmSharp.Test.Performance.Routing.CH.CHRoutingTest.TestSerialized(
				Assembly.GetExecutingAssembly().GetManifestResourceStream(
					embeddedResource), true, 10);
			Log.TraceEvent("Test", TraceEventType.Information,
				"Testing: 100 routes.");
			OsmSharp.Test.Performance.Routing.CH.CHRoutingTest.TestSerialized(
				Assembly.GetExecutingAssembly().GetManifestResourceStream(
					embeddedResource), true, 100);
			Log.TraceEvent("Test", TraceEventType.Information,
			    "Testing: 1000 routes.");
			OsmSharp.Test.Performance.Routing.CH.CHRoutingTest.TestSerialized(
			    Assembly.GetExecutingAssembly().GetManifestResourceStream(
			        embeddedResource), true, 1000);
		}

		/// <summary>
		/// Executes routing performance tests.
		/// </summary>
		private void TestRoutingResolved(string embeddedResource)
		{
			var box = new GeoCoordinateBox(
				new GeoCoordinate(51.20190, 4.66540),
				new GeoCoordinate(51.30720, 4.89820));
			Log.TraceEvent("Test", TraceEventType.Information,
				"Testing: 1 route.");
			OsmSharp.Test.Performance.Routing.CH.CHRoutingTest.TestSerializedResolved(
				Assembly.GetExecutingAssembly().GetManifestResourceStream(
					embeddedResource), box, true, 1);
			Log.TraceEvent("Test", TraceEventType.Information,
				"Testing: 2 routes.");
			OsmSharp.Test.Performance.Routing.CH.CHRoutingTest.TestSerializedResolved(
				Assembly.GetExecutingAssembly().GetManifestResourceStream(
					embeddedResource), box, true, 2);
			Log.TraceEvent("Test", TraceEventType.Information,
				"Testing: 10 routes.");
			OsmSharp.Test.Performance.Routing.CH.CHRoutingTest.TestSerializedResolved(
				Assembly.GetExecutingAssembly().GetManifestResourceStream(
					embeddedResource), box, true, 10);
			Log.TraceEvent("Test", TraceEventType.Information,
				"Testing: 100 routes.");
			OsmSharp.Test.Performance.Routing.CH.CHRoutingTest.TestSerializedResolved(
				Assembly.GetExecutingAssembly().GetManifestResourceStream(
					embeddedResource), box, true, 100);
			Log.TraceEvent("Test", TraceEventType.Information,
				"Testing: 1000 routes.");
			OsmSharp.Test.Performance.Routing.CH.CHRoutingTest.TestSerializedResolved(
				Assembly.GetExecutingAssembly().GetManifestResourceStream(
					embeddedResource), box, true, 1000);
		}

        /// <summary>
        /// Executes routing instruction generation tests.
        /// </summary>
        /// <param name="embeddedResource"></param>
        private void TestInstructions(string embeddedResource)
        {
            Log.TraceEvent("Test", TraceEventType.Information,
                           "Testing: Instruction generation.");
            OsmSharp.Test.Performance.Routing.CH.CHSerializedRoutingTest.TestSerializeRoutingInstrictions(
                "CHInstructions",
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                embeddedResource),
                new GeoCoordinate(51.261203, 4.780760),
                new GeoCoordinate(51.267797, 4.801362));
        }

        /// <summary>
        /// Executes rendering tests.
        /// </summary>
        /// <param name="embeddedResource">Embedded resource.</param>
        private void TestRendering(string embeddedResource) 
        {
            Log.TraceEvent("Test", TraceEventType.Information,
                           "Testing rendering.");

            OsmSharp.Test.Performance.UI.Rendering.RenderingSerializedSceneTests<CGContextWrapper>.Test(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    embeddedResource),
                (width, height) =>
                {
                    CGColorSpace space = CGColorSpace.CreateDeviceRGB ();
                    int bytesPerPixel = 4;
                    int bytesPerRow = bytesPerPixel * width;
                    int bitsPerComponent = 8;
                    CGBitmapContext target = new CGBitmapContext (null, width, height,
                        bitsPerComponent, bytesPerRow,
                        space, // kCGBitmapByteOrder32Little | kCGImageAlphaNoneSkipLast
                        CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Big);
                    target.InterpolationQuality = CGInterpolationQuality.None;
                    target.SetShouldAntialias (false);
                    target.SetBlendMode (CGBlendMode.Copy);
                    target.SetAlpha (1);
                    return new CGContextWrapper(target, new CGRect(
                        0, 0, width, height));
                },
                () => {
					return new CGContextRenderer (1);
                });
        }
	}
}
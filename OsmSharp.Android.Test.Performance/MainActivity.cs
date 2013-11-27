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

using System.Reflection;
using System.Threading;
using Android.App;
using Android.OS;
using Android.Widget;
using OsmSharp.Android.UI.Log;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Android.UI;
using OsmSharp.Test.Performance.UI.Rendering;

namespace OsmSharp.Android.Test.Performance
{
	[Activity (Label = "AndroidTestApp", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

            // set the seed manually.
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);           
            
            // add the to-ignore list.
            OsmSharp.Logging.Log.Ignore("OsmSharp.Osm.Interpreter.SimpleGeometryInterpreter");
            OsmSharp.Logging.Log.Ignore("CHPreProcessor");
            OsmSharp.Logging.Log.Ignore("RTreeStreamIndex");
            OsmSharp.Logging.Log.Ignore("Scene2DLayeredSource");

            // register the textview listener
            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterListener(
                new TextViewTraceListener(this, FindViewById<TextView>(Resource.Id.textView1)));

            // do some testing here.
            Thread thread = new Thread(
                new ThreadStart(Test));
            thread.Start();
		}

        /// <summary>
        /// Executes performance tests.
        /// </summary>
        private void Test()
        {
            this.TestRouting("OsmSharp.Android.Test.Performance.kempen-big.osm.pbf.routing");

            this.TestInstructions("OsmSharp.Android.Test.Performance.kempen-big.osm.pbf.routing");

            this.TestRendering("OsmSharp.Android.Test.Performance.kempen-big.osm.pbf.scene.layered");
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

        /// <summary>
        /// Executes rendering tests.
        /// </summary>
        /// <param name="embeddedResource">Embedded resource.</param>
        private void TestRendering(string embeddedResource)
        {
            Log.TraceEvent("Test", System.Diagnostics.TraceEventType.Information,
                           "Testing rendering.");

            RenderingSerializedSceneTests<global::Android.Graphics.Canvas>.Test(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    embeddedResource),
                (width, height) =>
                {
                    return new global::Android.Graphics.Canvas(
                        global::Android.Graphics.Bitmap.CreateBitmap(
                            width, height, global::Android.Graphics.Bitmap.Config.Argb4444));
                    //int bytesPerPixel = 4;
                    //int bytesPerRow = bytesPerPixel * width;
                    //int bitsPerComponent = 8;
                    //CGBitmapContext target = new CGBitmapContext(null, width, height,
                    //    bitsPerComponent, bytesPerRow,
                    //    space, // kCGBitmapByteOrder32Little | kCGImageAlphaNoneSkipLast
                    //    CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Big);
                    //target.InterpolationQuality = CGInterpolationQuality.None;
                    //target.SetShouldAntialias(false);
                    //target.SetBlendMode(CGBlendMode.Copy);
                    //target.SetAlpha(1);
                    //return new CGContextWrapper(target, new RectangleF(
                    //    0, 0, width, height));
                },
                () =>
                {
                    return new CanvasRenderer2D();
                });
        }
	}
}
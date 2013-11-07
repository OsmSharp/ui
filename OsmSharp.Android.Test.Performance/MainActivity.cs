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

using System;
using System.Timers;
using Android.App;
using Android.OS;
using Android.Widget;
using OsmSharp.Android.UI.Log;
using System.Reflection;
using System.Threading;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;

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
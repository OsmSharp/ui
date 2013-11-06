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

			// test console output.
			Timer timer = new Timer (300);
			timer.Elapsed += Elapsed;
			timer.Start ();
		}

		private void Elapsed(object sender, EventArgs args)
		{
            OsmSharp.Logging.Log.TraceEvent("Timer", System.Diagnostics.TraceEventType.Information,
                string.Format ("Some new line test at {0}.", DateTime.Now.ToLongTimeString()));
		}
	}
}
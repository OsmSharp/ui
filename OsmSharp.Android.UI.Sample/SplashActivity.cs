using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OsmSharp.Android.UI.Sample
{
	[Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
        {
            // initialize OsmSharp native hooks.
            Native.Initialize();

            // enable the logging.
            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterListener(new OsmSharp.Android.UI.Log.LogTraceListener());

			base.OnCreate(bundle);
			StartActivity(typeof(MainActivity));
		}
	}
}


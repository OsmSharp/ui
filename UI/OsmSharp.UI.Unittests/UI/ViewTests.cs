using System;
using OsmSharp.UI.Map;
using NUnit.Framework;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.UI.Unittests
{
	/// <summary>
	/// Contains tests for the View class and all conversion functionalities.
	/// </summary>
	[TestFixture]
	public class ViewTests
	{
		/// <summary>
		/// Tests CreateFrom
		/// </summary>
		[Test]
		public void ViewTestCreateFrom()
		{
			// create a view on null-island!
			GeoCoordinate center = new GeoCoordinate(0, 0);
			int width = 1000;
			int height = 1000;
			float zoom = 10;
			View view = View.CreateFrom(center, 
			                            zoom, width, height);

			// test result.
			Assert.AreEqual(view.Box.Center.Latitude, 0.0001);
			Assert.AreEqual(view.Box.Center.Longitude, 0.00001);


		}
	}
}


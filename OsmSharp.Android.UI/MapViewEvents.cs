using System;
using OsmSharp.Math.Geo;

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// Map view events.
	/// </summary>
	public static class MapViewEvents
	{
		public delegate void MapTapEventDelegate(GeoCoordinate coordinate);
	}
}


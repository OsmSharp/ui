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
using OsmSharp.UI.Map;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer;

namespace OsmSharp.Android.UI
{
	public class MapLayout : FrameLayout
	{
		/// <summary>
		/// Holds the mapview.
		/// </summary>
		private MapView _mapView;

		/// <summary>
		/// Holds the markers.
		/// </summary>
		private List<MapMarker> _markers;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapLayout"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		public MapLayout(Context context)
			: base(context)
		{
			this.Initialize ();

			_markers = new List<MapMarker> ();
		}

		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		/// <value>The map.</value>
		public Map Map {
			get{
				return _mapView.Map;
			}
			set{
				_mapView.Map = value;
			}
		}

		/// <summary>
		/// Returns the mapmarkers list.
		/// </summary>
		/// <value>The markers.</value>
		public void AddMarker(MapMarker marker)
		{
			_markers.Add (marker);

			var layoutParams = new FrameLayout.LayoutParams(marker.Bitmap.Width, marker.Bitmap.Height + 5);
			layoutParams.LeftMargin = -1;
			layoutParams.TopMargin = -1;
			layoutParams.Gravity = GravityFlags.Top | GravityFlags.Left ;
			this.AddView (marker, layoutParams);

			_mapView.Change ();
		}

		/// <summary>
		/// Adds the marker.
		/// </summary>
		/// <returns>The marker.</returns>
		/// <param name="coordinate">Coordinate.</param>
		public MapMarker AddMarker(GeoCoordinate coordinate)
		{
			MapMarker marker = new MapMarker (this.Context, coordinate);
			this.AddMarker (marker);
			return marker;
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		private void Initialize()
		{			
			_mapView = new MapView (this.Context, this);
			_mapView.MapMaxZoomLevel = 20;
			_mapView.MapMinZoomLevel = 12;
			//var mapView = new MapGLView (this);
			//mapView.Center = new GeoCoordinate(51.158075, 2.961545); // gistel
			//mapView.MapCenter = new GeoCoordinate (50.88672, 3.23899);
			_mapView.MapCenter = new GeoCoordinate(51.26337, 4.78739);
			//mapView.Center = new GeoCoordinate(51.156803, 2.958887);
			_mapView.MapZoomLevel = 15;
			this.AddView (_mapView);
		}

		/// <summary>
		/// Notifies the map change.
		/// </summary>
		/// <param name="pixelsWidth">Pixels width.</param>
		/// <param name="pixelsHeight">Pixels height.</param>
		/// <param name="view">View.</param>
		/// <param name="projection">Projection.</param>
		internal void NotifyMapChange(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection)
		{
			if (_markers != null) {
				foreach (var marker in _markers) {
					this.RemoveView (marker);
					if (marker.SetLayout (pixelsWidth, pixelsHeight, view, projection)) {
						this.AddView (marker, marker.LayoutParameters);
					}
				}
			}
		}
	}
}


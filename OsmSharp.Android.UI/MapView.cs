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
	/// <summary>
	/// Map view handling the map display and pan-zoon markers and touch-events.
	/// </summary>
	public class MapView : FrameLayout
	{
		/// <summary>
		/// Holds the mapview.
		/// </summary>
		private MapViewSurface  _mapView;

		/// <summary>
		/// Holds the markers.
		/// </summary>
		private List<MapMarker> _markers;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapLayout"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		public MapView (Context context)
			: base(context)
		{
			this.Initialize ();

			_markers = new List<MapMarker> ();
		}

		
		public delegate void MapTapEventDelegate(GeoCoordinate geoCoordinate);

		/// <summary>
		/// Occurs when the map was tapped at a certain location.
		/// </summary>
		public event MapTapEventDelegate MapTapEvent;

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
			_mapView = new MapViewSurface (this.Context, this);
			_mapView.MapTapEvent += delegate(GeoCoordinate geoCoordinate) {
				if (this.MapTapEvent != null) {
					this.MapTapEvent (geoCoordinate);
				}
			};
			this.AddView (_mapView);
		}

		/// <summary>
		/// Gets or sets the map zoom level.
		/// </summary>
		/// <value>The map zoom level.</value>
		public float MapZoomLevel {
			get { return _mapView.MapZoomLevel; }
			set { _mapView.MapZoomLevel = value; }
		}

		/// <summary>
		/// Gets or sets the map minimum zoom level.
		/// </summary>
		/// <value>The map minimum zoom level.</value>
		public float MapMinZoomLevel {
			get { return _mapView.MapMinZoomLevel; }
			set { _mapView.MapMinZoomLevel = value; }
		}

		/// <summary>
		/// Gets or sets the map max zoom level.
		/// </summary>
		/// <value>The map max zoom level.</value>
		public float MapMaxZoomLevel {
			get { return _mapView.MapMaxZoomLevel; }
			set { _mapView.MapMaxZoomLevel = value; }
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
		/// Gets or sets the map center.
		/// </summary>
		/// <value>The map center.</value>
		public GeoCoordinate MapCenter {
			get { return _mapView.MapCenter; }
			set { _mapView.MapCenter = value; }
		}

		/// <summary>
		/// Gets or sets the map tilt.
		/// </summary>
		/// <value>The map tilt.</value>
		public float MapTilt {
			get{
				return _mapView.MapTilt;
			}
			set{
				_mapView.MapTilt = value;
			}
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


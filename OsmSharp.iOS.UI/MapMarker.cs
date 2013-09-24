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
using MonoTouch.UIKit;
using OsmSharp.iOS.UI;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer;

namespace OsmSharp.iOS.UI
{
	/// <summary>
	/// Map marker
	/// </summary>
	public class MapMarker : UIButton
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SomeTestProject.MapMarker"/> class.
		/// </summary>
		public MapMarker ()
		{

		}

		/// <summary>
		/// Holds the default marker image.
		/// </summary>
		private static UIImage _bitmap;

		/// <summary>
		/// Gets the default image.
		/// </summary>
		/// <returns>The default image.</returns>
		private static UIImage GetDefaultImage() 
		{
			if (_bitmap == null) {
				_bitmap = UIImage.FromFile ("Images/marker.png");
			}
			return _bitmap;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SomeTestProject.MapMarker"/> class.
		/// </summary>
		/// <param name="mapView">Map view.</param>
		/// <param name="coordinate">Coordinate.</param>
		internal MapMarker (MapView mapView, GeoCoordinate coordinate)
			: this(mapView, coordinate, MapMarker.GetDefaultImage())
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SomeTestProject.MapMarker"/> class.
		/// </summary>
		/// <param name="mapView">Map view.</param>
		/// <param name="coordinate">Coordinate.</param>
		/// <param name="bitmap">Bitmap.</param>
		internal MapMarker(MapView mapView, GeoCoordinate coordinate, UIImage bitmap)
			: base(UIButtonType.Custom){
			_mapView = mapView;
			this.Location = coordinate;

			
			this.Frame = new System.Drawing.RectangleF (new System.Drawing.PointF (0, 0), bitmap.Size);
			this.SetImage (bitmap, UIControlState.Normal);
			this.SetImage (bitmap, UIControlState.Highlighted);
			this.SetImage (bitmap, UIControlState.Disabled);
		}

		/// <summary>
		/// Holds the parent map view.
		/// </summary>
		private MapView _mapView;

		/// <summary>
		/// Holds this markers location.
		/// </summary>
		private GeoCoordinate _location;

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>The location.</value>
		public GeoCoordinate Location {
			get {
				return _location;
			}
			set {
				_location = value;

				_mapView.NotifyMarkerChange (this);
			}
		}

		/// <summary>
		/// Sets the layout.
		/// </summary>
		/// <param name="pixelsWidth">Pixels width.</param>
		/// <param name="pixelsHeight">Pixels height.</param>
		/// <param name="view">View.</param>
		/// <param name="projection">Projection.</param>
		internal bool SetLayout(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection)
		{
			double[] projected = projection.ToPixel (this.Location);
			double[] locationPixel = view.ToViewPort (pixelsWidth, pixelsHeight, projected [0], projected [1]);

			if (locationPixel [0] > 0 && locationPixel [0] < pixelsWidth &&
			    locationPixel [1] > 0 && locationPixel [1] < pixelsHeight) {
				this.Center = new System.Drawing.PointF ((float)locationPixel [0], (float)locationPixel [1]);

				return true;
			}
			return false;
		}
	}
}


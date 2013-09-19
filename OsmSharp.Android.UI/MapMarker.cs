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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo.Projections;
using System.Reflection;
using System.IO;

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// Map marker.
	/// </summary>
	public class MapMarker : ImageButton
	{
		/// <summary>
		/// Holds the default marker bitmap.
		/// </summary>
		private static global::Android.Graphics.Bitmap _bitmap = null;

		/// <summary>
		/// Gets the default image.
		/// </summary>
		/// <returns>The default image.</returns>
		private static global::Android.Graphics.Bitmap GetDefaultImage()
		{
			if (_bitmap == null) {
				_bitmap = global::Android.Graphics.BitmapFactory.DecodeStream (
					Assembly.GetExecutingAssembly ().GetManifestResourceStream (
					"OsmSharp.Android.UI.Images.marker.png"));
			}
			return _bitmap;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapMarker"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="coordinate">Coordinate.</param>
		internal MapMarker (Context context, GeoCoordinate coordinate)
			: this(context, coordinate, MapMarker.GetDefaultImage())
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapMarker"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="coordinate">Coordinate.</param>
		/// <param name="bitmap">Bitmap.</param>
		internal MapMarker(Context context, GeoCoordinate coordinate, global::Android.Graphics.Bitmap bitmap)
			: base(context){
			this.Location = coordinate;
			this.SetBackgroundColor (global::Android.Graphics.Color.Transparent);

			this.SetScaleType (ScaleType.Center);
			this.SetImageBitmap (bitmap);
			this.SetMinimumWidth (bitmap.Width);
			this.SetMinimumHeight (bitmap.Height);
		}

		/// <summary>
		/// Gets the bitmap.
		/// </summary>
		/// <value>The bitmap.</value>
		public global::Android.Graphics.Bitmap Bitmap {
			get {
				return _bitmap;
			}
		}

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>The location.</value>
		public GeoCoordinate Location {
			get;
			private set;
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
				(this.LayoutParameters as FrameLayout.LayoutParams).LeftMargin = (int)(locationPixel [0] - 
					(this.LayoutParameters as FrameLayout.LayoutParams).Width / 2.0);
				(this.LayoutParameters as FrameLayout.LayoutParams).TopMargin = (int)locationPixel [1] - 
					(this.LayoutParameters as FrameLayout.LayoutParams).Height;
				return true;
			}
			return false;
		}
	}
}


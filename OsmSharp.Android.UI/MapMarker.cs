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
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapMarker"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="coordinate">Coordinate.</param>
		public MapMarker (Context context, GeoCoordinate coordinate)
			: base(context)
		{
			this.Location = coordinate;
			this.SetBackgroundColor (global::Android.Graphics.Color.Transparent);

			if (_bitmap == null) {
				_bitmap = global::Android.Graphics.BitmapFactory.DecodeStream (
					Assembly.GetExecutingAssembly ().GetManifestResourceStream (
						"OsmSharp.Android.UI.Images.marker.png"));
			}
			this.SetScaleType (ScaleType.Center);
			this.SetImageBitmap (_bitmap);
			this.SetMinimumWidth (_bitmap.Width);
			this.SetMinimumHeight (_bitmap.Height);
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
			set;
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


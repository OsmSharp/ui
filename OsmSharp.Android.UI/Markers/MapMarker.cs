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

using Android.Content;
using Android.Views;
using Android.Widget;
using OsmSharp.Android.UI.Controls;
using OsmSharp.Math.Geo;
using System;
using System.Reflection;

namespace OsmSharp.Android.UI
{
    /// <summary>
    /// Represents a map marker.
    /// </summary>
    public class MapMarker : MapControl<ImageButton>
    {
        /// <summary>
        /// Holds the image for this marker.
        /// </summary>
        private global::Android.Graphics.Bitmap _image;

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapMarker"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="location"></param>
        public MapMarker(Context context, GeoCoordinate location)
            : this(context, location, MapControlAlignmentType.CenterBottom, MapMarker.GetDefaultImage())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapMarker"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="location">Coordinate.</param>
        /// <param name="alignment">The alignment.</param>
        public MapMarker(Context context, GeoCoordinate location, 
            MapControlAlignmentType alignment)
            : this(context, location, alignment, MapMarker.GetDefaultImage())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapMarker"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="location">Coordinate.</param>
        /// <param name="image">Bitmap.</param>
        /// <param name="alignment">The alignment.</param>
        public MapMarker(Context context, GeoCoordinate location, 
            MapControlAlignmentType alignment, global::Android.Graphics.Bitmap image)
            : base(new ImageButton(context), location, alignment, image.Width, image.Height)
        {
            this.View.SetBackgroundColor(global::Android.Graphics.Color.Transparent);

            this.View.SetPadding(1, 1, 1, 1);

            _image = image;
            this.View.SetImageBitmap(image);
            this.View.SetScaleType(global::Android.Widget.ImageView.ScaleType.FitCenter);
        }

        /// <summary>
        /// Holds the default marker bitmap.
        /// </summary>
        private static global::Android.Graphics.Bitmap _defaultImage = null;

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <returns>The default image.</returns>
        private static global::Android.Graphics.Bitmap GetDefaultImage()
        {
            if (_defaultImage == null)
            {
                _defaultImage = global::Android.Graphics.BitmapFactory.DecodeStream(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(
                        "OsmSharp.Android.UI.Images.marker_small.png"));
            }
            return _defaultImage;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>The image.</value>
        public global::Android.Graphics.Bitmap Image
        {
            get
            {
                return _image;
            }
        }

        public override void Dispose()
        {
            if (this._image != null)
            { // dispose of the native image.
                try
                {
                    this._image.Recycle();
                    this._image.Dispose();
                }
                catch (Exception)
                { // TODO: figure out whyt this happens, ask someone at Xamarin if needed.
                    // whatever happens, don't crash!
                }
                finally
                {
                    this._image = null;
                }
            }
            this.View.Dispose();
        }
    }
}
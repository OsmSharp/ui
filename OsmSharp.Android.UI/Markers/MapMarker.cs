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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI;
using OsmSharp.UI.Renderer;
using Android.Graphics;

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
        private static global::Android.Graphics.Bitmap _defaultImage = null;

        /// <summary>
        /// Holds the view where this 
        /// </summary>
        private IMapMarkerHost _mapView;

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
        /// Holds the image for this marker.
        /// </summary>
        private global::Android.Graphics.Bitmap _image;

        /// <summary>
        /// Holds the default alignment.
        /// </summary>
        private MapMarkerAlignmentType _alignment;

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapMarker"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="location">Coordinate.</param>
        public MapMarker(Context context, GeoCoordinate location)
            : this(context, location, MapMarkerAlignmentType.CenterBottom, MapMarker.GetDefaultImage())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapMarker"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="location">Coordinate.</param>
        /// <param name="alignment">The alignment.</param>
        public MapMarker(Context context, GeoCoordinate location, 
                         MapMarkerAlignmentType alignment)
            : this(context, location, alignment, MapMarker.GetDefaultImage())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.Android.UI.MapMarker"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="location">Coordinate.</param>
        /// <param name="bitmap">Bitmap.</param>
        /// <param name="alignment">The alignment.</param>
        public MapMarker(Context context, GeoCoordinate location, 
                         MapMarkerAlignmentType alignment, global::Android.Graphics.Bitmap image)
            : base(context)
        {
            _location = location;
            _alignment = alignment;
            this.SetBackgroundColor(global::Android.Graphics.Color.Transparent);

            this.SetPadding(1, 1, 1, 1);

            _image = image;
            this.SetMinimumWidth(image.Width);
            this.SetMinimumHeight(image.Height);
            this.SetImageBitmap(image);
            this.SetScaleType(ScaleType.FitCenter);
        }

        /// <summary>
        /// Attaches this maker to the given map view.
        /// </summary>
        /// <param name="mapView">Map view.</param>
        internal void AttachTo(IMapMarkerHost mapView)
        {
            _mapView = mapView;
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

        /// <summary>
        /// Holds this markers location.
        /// </summary>
        private GeoCoordinate _location;

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public GeoCoordinate Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;

                if (_mapView != null)
                {
                    _mapView.NotifyMarkerChange(this);
                }
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
            double[] projected = projection.ToPixel(this.Location);
            double[] locationPixel = view.ToViewPort(pixelsWidth, pixelsHeight, projected[0], projected[1]);

            // set the new location depending on the size of the image and the alignment parameter.
            double leftMargin = locationPixel[0];
            double topMargin = locationPixel[1];

            leftMargin = locationPixel[0] - (this.LayoutParameters as FrameLayout.LayoutParams).Width / 2.0;

            switch (_alignment)
            {
                case MapMarkerAlignmentType.Center:
                    topMargin = locationPixel[1] - (this.LayoutParameters as FrameLayout.LayoutParams).Height / 2.0;
                    break;
                case MapMarkerAlignmentType.CenterTop:
                    break;
                case MapMarkerAlignmentType.CenterBottom:
                    topMargin = locationPixel[1] - (this.LayoutParameters as FrameLayout.LayoutParams).Height;
                    break;
            }

            (this.LayoutParameters as FrameLayout.LayoutParams).LeftMargin = (int)leftMargin;
            (this.LayoutParameters as FrameLayout.LayoutParams).TopMargin = (int)topMargin;
            return true;
        }

        public new void Dispose()
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
            base.Dispose();
        }
    }
}


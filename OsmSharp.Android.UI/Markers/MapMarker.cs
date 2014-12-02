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
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using OsmSharp.Android.UI.Controls;
using OsmSharp.Android.UI.Images.NinePatchHelpers;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer;
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
        /// Holds the context.
        /// </summary>
        private Context _context;

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
        /// <param name="context"></param>
        /// <param name="location"></param>
        /// <param name="alignment"></param>
        /// <param name="res"></param>
        /// <param name="id"></param>
        public MapMarker(Context context, GeoCoordinate location,
            MapControlAlignmentType alignment, Resources res, int id)
            : base(new ImageButton(context), location, alignment)
        {
            _image = BitmapFactory.DecodeResource(res, id);
            _context = context;

            this.SetSize(_image.Width, _image.Height);
            this.View.SetBackgroundColor(global::Android.Graphics.Color.Transparent);

            this.View.SetPadding(1, 1, 1, 1);
            this.View.SetImageBitmap(_image);
            this.View.SetScaleType(global::Android.Widget.ImageView.ScaleType.FitCenter);
            this.View.Click += View_Click;
            this.TogglePopupOnClick = true;
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
            _context = context;
            this.View.SetBackgroundColor(global::Android.Graphics.Color.Transparent);

            this.View.SetPadding(1, 1, 1, 1);

            _image = image;
            this.View.SetImageBitmap(image);
            this.View.SetScaleType(global::Android.Widget.ImageView.ScaleType.FitCenter);
            this.View.Click += View_Click;
            this.TogglePopupOnClick = true;
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

        /// <summary>
        /// Holds the popup view.
        /// </summary>
        private View _popupView;

        /// <summary>
        /// Holds the x-popup-offset.
        /// </summary>
        private int _popupXOffset = 0;

        /// <summary>
        /// Holds the y-popup-offset.
        /// </summary>
        private int _popupYOffset = 0;

        /// <summary>
        /// Holds visible flag for the popup.
        /// </summary>
        private bool _popupIsVisible;

        /// <summary>
        /// Holds the flag keeping layout status.
        /// </summary>
        private bool _popupLayoutSet = false;

        /// <summary>
        /// Adds a default popup and returns the new view.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>The view representing the popup.</returns>
        /// <remarks>Add subviews to the view to show stuff on the marker.</remarks>
        public LinearLayout AddNewPopup(int width, int height)
        {
            var layout = new LinearLayout(_context);

            // create default ninepatch.
            var ninepatchImage = global::Android.Graphics.BitmapFactory.DecodeStream(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Android.UI.Images.marker_popup.9.png"));
            var ninepatchDrawable = NinePatchChunk.createNinePatchDrawable(this.View.Resources, ninepatchImage, "marker_popup");
            layout.SetBackgroundDrawable(ninepatchDrawable);

            // calculate offset.
            var xOffset = -(width / 2) + 47;
            var yOffset = 0;

            this.AddPopup(layout, width, height, xOffset, yOffset);

            return layout;
        }

        /// <summary>
        /// Adds a popup, showing the given view when the marker is tapped.
        /// </summary>
        /// <param name="width">The view height.</param>
        /// <param name="height">The view width.</param>
        /// <param name="view">The view that should be used as popup.</param>
        public void AddPopup(View view, int width, int height)
        {
            this.AddPopup(view, width, height, 0, 0);
        }

        /// <summary>
        /// Adds a popup, showing the given view when the marker is tapped.
        /// </summary>
        /// <param name="width">The view height.</param>
        /// <param name="height">The view width.</param>
        /// <param name="xOffset">The x-offset.</param>
        /// <param name="yOffset">The y-offset.</param>
        /// <param name="view">The view that should be used as popup.</param>
        public void AddPopup(View view, int width, int height, int xOffset, int yOffset)
        { 
            // remove previous popup if any.
            this.RemovePopup();

            // add view as popup.
            _popupView = view;
            _popupXOffset = xOffset;
            _popupYOffset = yOffset;

            // setup layout parameters.
            var layoutParams = new FrameLayout.LayoutParams(width, height + 5);
            layoutParams.LeftMargin = -1;
            layoutParams.TopMargin = -1;
            layoutParams.Gravity = GravityFlags.Top | GravityFlags.Left;
            _popupView.LayoutParameters = layoutParams;

            _popupLayoutSet = false;
        }

        /// <summary>
        /// Removes the popup.
        /// </summary>
        public void RemovePopup()
        {
            if (_popupView != null && this.Host != null)
            {
                this.Host.RemoveView(_popupView);
            }
            _popupView = null;
            _popupIsVisible = false;
        }

        /// <summary>
        /// Make sure this popup is shown.
        /// </summary>
        public void ShowPopup()
        {
            if (_popupView != null && this.Host != null)
            {
                if (!_popupLayoutSet)
                {
                    this.Host.NotifyControlChange(this);
                }
                this.Host.RemoveView(_popupView);
                this.Host.AddView(_popupView, _popupView.LayoutParameters);
            }
            _popupIsVisible = true;
        }

        /// <summary>
        /// Hide the popup.
        /// </summary>
        public void HidePopup()
        {
            if (_popupView != null && this.Host != null)
            {
                this.Host.RemoveView(_popupView);
            }
            _popupIsVisible = false;
        }

        /// <summary>
        /// Returns true if this marker has a popup.
        /// </summary>
        public bool HasPopup
        {
            get
            {
                return _popupView != null;
            }
        }

        /// <summary>
        /// Gets or sets the toggle popup on click flag.
        /// </summary>
        public bool TogglePopupOnClick { get; set; }

        /// <summary>
        /// Handles the marker click-event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void View_Click(object sender, EventArgs e)
        {
            if (this.HasPopup && this.TogglePopupOnClick)
            { // toggle popup visible.
                if(_popupIsVisible)
                {
                    this.HidePopup();
                }
                else
                {
                    this.ShowPopup();
                }
            }

            if (this.Host != null)
            {
                this.Host.NotifyControlClicked(this);
            }
        }

        /// <summary>
        /// Attaches this marker to the given host.
        /// </summary>
        /// <param name="controlHost"></param>
        internal override void AttachTo(IMapControlHost controlHost)
        {
            base.AttachTo(controlHost);

            // show popup if it is supposed to be visible.
            if(_popupIsVisible)
            {
                this.ShowPopup();
            }
        }

        /// <summary>
        /// Detaches this control from the given control host.
        /// </summary>
        /// <param name="controlHost">The control host.</param>
        internal override void DetachFrom(IMapControlHost controlHost)
        {     
            if (this.Host != null && this.HasPopup)
            { // remove popup.
                this.RemovePopup();
            }

            base.DetachFrom(controlHost);
        }

        /// <summary>
        /// Sets the layout.
        /// </summary>
        /// <param name="pixelsWidth">Pixels width.</param>
        /// <param name="pixelsHeight">Pixels height.</param>
        /// <param name="view">View.</param>
        /// <param name="projection">Projection.</param>
        internal override bool SetLayout(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection)
        {
            base.SetLayout(pixelsWidth, pixelsHeight, view, projection);

            if (this.MoveWithMap)
            { // keep location the same and move with map.
                if (this.Location != null &&
                    _popupView != null)
                { // only set layout if there is a location set.
                    var projected = projection.ToPixel(this.Location);
                    var toView = view.CreateToViewPort(pixelsWidth, pixelsHeight);
                    double locationPixelX, locationPixelY;
                    // var locationPixel = view.ToViewPort(pixelsWidth, pixelsHeight, projected[0], projected[1]);
                    toView.Apply(projected[0], projected[1], out locationPixelX, out locationPixelY);

                    // set the new location depending on the size of the image and the alignment parameter.
                    double leftPopupMargin = locationPixelX;
                    double topPopupMargin = locationPixelY;

                    leftPopupMargin = locationPixelX - (_popupView.LayoutParameters as FrameLayout.LayoutParams).Width / 2.0;

                    switch (this.Alignment)
                    {
                        case MapControlAlignmentType.Center:
                            topPopupMargin = locationPixelY
                                - (this.View.LayoutParameters as FrameLayout.LayoutParams).Height / 2.0
                                - (_popupView.LayoutParameters as FrameLayout.LayoutParams).Height;
                            break;
                        case MapControlAlignmentType.CenterTop:
                            topPopupMargin = locationPixelY
                                - (_popupView.LayoutParameters as FrameLayout.LayoutParams).Height;
                            break;
                        case MapControlAlignmentType.CenterBottom:
                            topPopupMargin = locationPixelY
                                - (this.View.LayoutParameters as FrameLayout.LayoutParams).Height
                                - (_popupView.LayoutParameters as FrameLayout.LayoutParams).Height;
                            break;
                    }

                    // add offsets.
                    leftPopupMargin = leftPopupMargin + _popupXOffset;
                    topPopupMargin = topPopupMargin + _popupYOffset;

                    (_popupView.LayoutParameters as FrameLayout.LayoutParams).LeftMargin = (int)leftPopupMargin;
                    (_popupView.LayoutParameters as FrameLayout.LayoutParams).TopMargin = (int)topPopupMargin;

                    _popupLayoutSet = true;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Called after one of the markers/controls has been changed.
        /// </summary>
        internal override void OnAfterSetLayout()
        {
            // make sure popups are on top.
            if (_popupView != null)
            {
                _popupView.BringToFront();
            }
        }

        /// <summary>
        /// Notifies the map tap.
        /// </summary>
        protected internal override void NotifyMapTap()
        {
            this.HidePopup();
        }

        /// <summary>
        /// Notifies the other control clicked.
        /// </summary>
        protected internal override void NotifyOtherControlClicked()
        {
            this.HidePopup();
        }

        /// <summary>
        /// Disposes all resources associated with this marker.
        /// </summary>
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
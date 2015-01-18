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
using OsmSharp.iOS.UI;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer;
using OsmSharp.iOS.UI.Controls;
#if __UNIFIED__
using UIKit;
using CoreGraphics;
#else
using MonoTouch.UIKit;

// Type Mappings Unified to monotouch.dll
using CGRect = global::System.Drawing.RectangleF;
using CGSize = global::System.Drawing.SizeF;
using CGPoint = global::System.Drawing.PointF;

using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

namespace OsmSharp.iOS.UI
{
    /// <summary>
    /// Map marker.
    /// </summary>
    public class MapMarker : MapControl<UIButton>
    {
		/// <summary>
		/// Holds the default marker image.
		/// </summary>
		private static UIImage _defaultImage;

		/// <summary>
		/// Gets the default image.
		/// </summary>
		/// <returns>The default image.</returns>
		private static UIImage GetDefaultImage() 
		{
			if (_defaultImage == null) {
				_defaultImage = UIImage.FromFile ("Images/marker-and-shadow.png");
			}
			return _defaultImage;
		}

		/// <summary>
		/// Holds the image for this marker.
		/// </summary>
		private UIImage _image;

		/// <summary>
		/// Initializes a new instance of the <see cref="SomeTestProject.MapMarker"/> class.
		/// </summary>
		/// <param name="location">Coordinate.</param>
		public MapMarker (GeoCoordinate location)
			: this(location, MapControlAlignmentType.CenterBottom, MapMarker.GetDefaultImage())
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SomeTestProject.MapMarker"/> class.
		/// </summary>
		/// <param name="location">Coordinate.</param>
		/// <param name="marker">Alignment.</param>
        public MapMarker (GeoCoordinate location, MapControlAlignmentType alignment)
			: this(location, alignment, MapMarker.GetDefaultImage())
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SomeTestProject.MapMarker"/> class.
		/// </summary>
		/// <param name="location">Coordinate.</param>
		/// <param name="image">Bitmap.</param>
		/// <param name="alignment">Alignment.</param>
        public MapMarker(GeoCoordinate location, MapControlAlignmentType alignment, UIImage image)
            : base(new UIButton(UIButtonType.Custom), location, alignment, (int)image.Size.Width, (int)image.Size.Height) {
			_image = image;

            this.View.SetImage (image, UIControlState.Normal);
			this.View.SetImage (image, UIControlState.Highlighted);
			this.View.SetImage (image, UIControlState.Disabled);

            this.View.TouchUpInside += view_TouchUpInside;
            this.TogglePopupOnClick = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SomeTestProject.MapMarker"/> class.
        /// </summary>
        /// <param name="point">Point.</param>
		public MapMarker (CGPoint point)
            : this(point, MapControlAlignmentType.CenterBottom, MapMarker.GetDefaultImage())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SomeTestProject.MapMarker"/> class.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <param name="marker">Alignment.</param>
        public MapMarker (CGPoint point, MapControlAlignmentType alignment)
            : this(point, alignment, MapMarker.GetDefaultImage())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SomeTestProject.MapMarker"/> class.
        /// </summary>
        /// <param name="point">Point.</param>
        /// <param name="image">Bitmap.</param>
        /// <param name="alignment">Alignment.</param>
        public MapMarker(CGPoint point, MapControlAlignmentType alignment, UIImage image)
            : base(new UIButton(UIButtonType.Custom), point, alignment, (int)image.Size.Width, (int)image.Size.Height) {
            _image = image;

            this.View.SetImage (image, UIControlState.Normal);
            this.View.SetImage (image, UIControlState.Highlighted);
            this.View.SetImage (image, UIControlState.Disabled);

            this.View.TouchUpInside += view_TouchUpInside;
            this.TogglePopupOnClick = true;
        }

        /// <summary>
        /// Holds the popup view.
        /// </summary>
        private UIView _popupView;

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
        /// Adds a popup, showing the given view when the marker is tapped.
        /// </summary>
        /// <param name="width">The view height.</param>
        /// <param name="height">The view width.</param>
        /// <param name="view">The view that should be used as popup.</param>
        public void AddPopup(UIView view, int width, int height)
        { 
            this.AddPopup(view, width, height, 0, 0);
        }

        /// <summary>
        /// Adds a popup, showing the given view when the marker is tapped.
        /// </summary>
        /// <param name="width">The view height.</param>
        /// <param name="height">The view width.</param>
        /// <param name="view">The view that should be used as popup.</param>
        public void AddPopup(UIView view, int width, int height, int xOffset, int yOffset)
        { 
            // remove previous popup if any.
            this.RemovePopup();

            // add view as popup.
            _popupXOffset = xOffset;
            _popupYOffset = yOffset;
            _popupView = view;
            _popupView.Frame = new CGRect (new CGPoint (0, 0), new CGSize(width, height));
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
                this.Host.RemoveView(_popupView);
                this.Host.AddView(_popupView);
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
        void view_TouchUpInside(object sender, EventArgs e)
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
        protected internal override bool SetLayout(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection)
        {
            base.SetLayout(pixelsWidth, pixelsHeight, view, projection);

            if (this.Location != null &&
                _popupView != null)
            { // only set layout if there is a location set.
                var projected = projection.ToPixel(this.Location);
                var locationPixel = view.ToViewPort(pixelsWidth, pixelsHeight, projected[0], projected[1]);

                // set the new location depending on the size of the image and the alignment parameter.
                double leftMargin = locationPixel [0];// - this.Bitmap.Size.Width / 2.0;
                double topMargin = locationPixel [1];
                switch (this.Alignment) {
                    case MapControlAlignmentType.CenterTop:
                        topMargin = locationPixel [1] - (_popupView.Frame.Height / 2.0);
                        break;
                    case MapControlAlignmentType.Center:
                        topMargin = locationPixel [1] - (this.View.Frame.Height / 2.0) - (_popupView.Frame.Height / 2.0);
                        break;
                    case MapControlAlignmentType.CenterBottom:
                        topMargin = locationPixel [1] - this.View.Frame.Height - (_popupView.Frame.Height / 2.0);
                        break;
                }

                // add offsets.
                leftMargin = leftMargin + _popupXOffset;
                topMargin = topMargin + _popupYOffset;

                _popupView.Center = new CGPoint ((float)leftMargin, (float)topMargin);

                return true;
            }
            return true;
        }

        /// <summary>
        /// Called when any map control has changed and was repositioned if needed.
        /// </summary>
        internal override void OnAfterSetLayout()
        {
            if (_popupView != null &&
                _popupView.Superview != null)
            {
                _popupView.Superview.BringSubviewToFront(_popupView);
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
        /// Notifies this control another control was clicked.
        /// </summary>
        protected internal override void NotifyOtherControlClicked()
        {
            this.HidePopup();
        }

		/// <summary>
		/// Gets the image.
		/// </summary>
		/// <value>The i.</value>
		public UIImage Image {
			get {
				return _image;
			}
		}
	}
}
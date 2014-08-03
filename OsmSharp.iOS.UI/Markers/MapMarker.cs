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
using OsmSharp.iOS.UI.Controls;

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
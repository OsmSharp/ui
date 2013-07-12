// OsmSharp - OpenStreetMap tools & library.
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
using MonoTouch.CoreGraphics;
using OsmSharp.UI.Map;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Renderer;

namespace OsmSharp.iOS.UI
{
	/// <summary>
	/// Map view.
	/// </summary>
	public class MapView : UIView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.MapView"/> class.
		/// </summary>
		public MapView ()
		{
			this.Initialize ();
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		public void Initialize()
		{
			this.BackgroundColor = UIColor.White;
		}

		/// <summary>
		/// Gets or sets the center.
		/// </summary>
		/// <value>The center.</value>
		public GeoCoordinate MapCenter {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		/// <value>The map.</value>
		public Map Map {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the zoom level.
		/// </summary>
		/// <value>The zoom level.</value>
		public float ZoomLevel {
			get;
			set;
		}

		/// <summary>
		/// Creates the view.
		/// </summary>
		/// <returns>The view.</returns>
		public View2D CreateView(System.Drawing.RectangleF rect)
		{
			double[] sceneCenter = this.Map.Projection.ToPixel (this.MapCenter.Latitude, this.MapCenter.Longitude);
			float sceneZoomFactor = (float)this.Map.Projection.ToZoomFactor (this.ZoomLevel);

			return View2D.CreateFrom (sceneCenter [0], sceneCenter [1],
			                         rect.Width, rect.Height, sceneZoomFactor,
			                         this.Map.Projection.DirectionX, this.Map.Projection.DirectionY);
		}

		/// <summary>
		/// Draws the view within the specified rectangle.
		/// </summary>
		/// <param name="rect">Rect.</param>
		public override void Draw (System.Drawing.RectangleF rect)
		{
			base.Draw (rect);

			CGContext gctx = UIGraphics.GetCurrentContext ();

			View2D view = this.CreateView (rect);
			this.Map.ViewChanged ((float)this.Map.Projection.ToZoomFactor (this.ZoomLevel),
			                     this.MapCenter,
			                     view);

			MapRenderer<CGContext> renderer = new MapRenderer<CGContext> (
				new CGContextRenderer (rect));

			renderer.Render (gctx, this.Map, (float)this.Map.Projection.ToZoomFactor (this.ZoomLevel), this.MapCenter);
		}
	}
}


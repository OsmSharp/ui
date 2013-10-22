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
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo;
using OsmSharp.Units.Angle;
using OsmSharp.UI.Map;
using OsmSharp.UI;
using System.Collections.Generic;

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// Abstract version of a map view surface.
	/// </summary>
    public interface IMapViewSurface
	{
        /// <summary>
        /// Initializes this map view surface.
        /// </summary>
        /// <param name="mapLayout"></param>
        void Initialize(MapView mapLayout);

		/// <summary>
		/// Notifies change.
		/// </summary>
		void Change();

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>The width.</value>
		int Width {
			get;
		}

		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		/// <value>The height.</value>
		int Height {
			get;
		}

		/// <summary>
		/// Creates the view.
		/// </summary>
		/// <returns>The view.</returns>
		View2D CreateView();

        /// <summary>
        /// Gets or sets the map.
        /// </summary>
        Map Map
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map center.
        /// </summary>
        GeoCoordinate MapCenter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map tilt.
        /// </summary>
        Degree MapTilt
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets the map zoom level.
		/// </summary>
		/// <value>The map zoom level.</value>
		float MapZoom {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the map minimum zoom level.
		/// </summary>
		/// <value>The map minimum zoom level.</value>
		float? MapMinZoomLevel {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the map max zoom level.
		/// </summary>
		/// <value>The map max zoom level.</value>
		float? MapMaxZoomLevel {
			get;
			set;
		}

        /// <summary>
        /// Sets a the map view parameters in one go.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="mapTilt"></param>
        /// <param name="zoom"></param>
        void SetMapView(GeoCoordinate center, Degree mapTilt, float zoom);

        /// <summary>
        /// Gets or sets the auto invalidate flag.
        /// </summary>
        bool AutoInvalidate { get; set; }

        /// <summary>
        /// Zooms to the given list of markers.
        /// </summary>
        /// <param name="markers"></param>
        void ZoomToMarkers(List<MapMarker> markers);
    }
}
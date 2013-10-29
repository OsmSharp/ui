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

using OsmSharp.Math.Geo;
using OsmSharp.Units.Angle;
using OsmSharp.UI.Animations;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer;
using System;

namespace OsmSharp.UI
{
    /// <summary>
    /// Abstracts an implementation of a MapView with a MapCenter, MapTilt and MapZoom.
    /// </summary>
    public interface IMapView
    {
		/// <summary>
		/// Occurs when map was touched and things have been moved around.
		/// </summary>
		event MapViewDelegates.MapTouchedDelegate MapTouched;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="OsmSharp.UI.IMapView"/> auto invalidate.
		/// </summary>
		/// <value><c>true</c> if auto invalidate; otherwise, <c>false</c>.</value>
	    bool AutoInvalidate {
			get;
			set;
		}

		/// <summary>
		/// Invalidates this instance.
		/// </summary>
		void Invalidate ();

		/// <summary>
		/// Registers the animator.
		/// </summary>
		/// <param name="mapViewAnimator">Map view animator.</param>
		void RegisterAnimator (MapViewAnimator mapViewAnimator);

		/// <summary>
		/// Sets the map view by changing all three parameters at once.
		/// </summary>
		/// <param name="center">Center.</param>
		/// <param name="mapTilt">Map tilt.</param>
		/// <param name="mapZoom">Map zoom.</param>
		void SetMapView(GeoCoordinate center, Degree mapTilt, float mapZoom);

        /// <summary>
        /// Gets or sets the MapCenter;
        /// </summary>
        GeoCoordinate MapCenter { get; set; }

        /// <summary>
        /// Gets or sets the MapTilt.
        /// </summary>
        Degree MapTilt { get; set; }

        /// <summary>
        /// Gets or sets the MapZoom.
        /// </summary>
        float MapZoom { get; set; }

        /// <summary>
        /// Gets or sets the Map.
        /// </summary>
        Map.Map Map { get; set; }

        /// <summary>
        /// Notifies a map change.
        /// </summary>
        /// <param name="pixelsWidth"></param>
        /// <param name="pixelsHeight"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        void NotifyMapChange(double pixelsWidth, double pixelsHeight, View2D view, IProjection projection);
    }

	/// <summary>
	/// Map view delegates.
	/// </summary>
	public static class MapViewDelegates 
	{
		public delegate void MapTouchedDelegate(IMapView mapView, float newZoom, Degree newTilt, GeoCoordinate newCenter);
	}
}
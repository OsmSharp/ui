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

using System.IO;
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Streams.Gpx;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;

namespace OsmSharp.UI.Map.Layers
{
	/// <summary>
	/// A layer drawing GPX data.
	/// </summary>
	public class LayerGpx : ILayer
	{		
		/// <summary>
		/// Holds the projection.
		/// </summary>
		private readonly IProjection _projection;

		/// <summary>
		/// Creates a new OSM data layer.
		/// </summary>
        /// <param name="projection"></param>
		public LayerGpx(IProjection projection)
		{			
			_projection = projection;

            this.Scene = new Scene2DSimple(
                (float)(new OsmSharp.Math.Geo.Projections.WebMercator().ToZoomFactor(16)));
			this.Scene.BackColor = SimpleColor.FromKnownColor(KnownColor.Transparent).Value;
		}
		
		/// <summary>
		/// Gets the minimum zoom.
		/// </summary>
		public float? MinZoom { get; private set; }
		
		/// <summary>
		/// Gets the maximum zoom.
		/// </summary>
		public float? MaxZoom { get; private set; }
		
		/// <summary>
		/// Gets the scene of this layer containing the objects already projected.
		/// </summary>
		public Scene2D Scene { get; private set; }
		
		/// <summary>
		/// Called when the view on the map containing this layer has changed.
		/// </summary>
		/// <param name="map"></param>
		/// <param name="zoomFactor"></param>
		/// <param name="center"></param>
		/// <param name="view"></param>
		public void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view)
		{
			// all data is pre-loaded for now.

			// when displaying huge amounts of GPX-data use another approach.
		}
		
		/// <summary>
		/// Event raised when this layer's content has changed.
		/// </summary>
		public event Map.LayerChanged LayerChanged;
		
		/// <summary>
		/// Invalidates this layer.
		/// </summary>
		public void Invalidate()
		{
			if (this.LayerChanged != null) {
				this.LayerChanged (this);
			}
		}

		#region Scene Building

		/// <summary>
		/// Adds a new GPX.
		/// </summary>
		/// <param name="stream">Stream.</param>
		public GeoCoordinateBox AddGpx(Stream stream)
		{
            GeoCoordinateBox bounds = null;
            var gpxStream = new GpxGeoStreamSource(stream);
            foreach (var geometry in gpxStream)
            {
                if (geometry is Point)
                { // add the point.
                    var point = (geometry as Point);

                    // get x/y.
                    var x = _projection.LongitudeToX(point.Coordinate.Longitude);
                    var y = _projection.LatitudeToY(point.Coordinate.Latitude);

                    // set the default color if none is given.
                    SimpleColor blue = SimpleColor.FromKnownColor(KnownColor.Blue);
                    SimpleColor transparantBlue = SimpleColor.FromArgb(128,
                                                                       blue.R, blue.G, blue.B);

                    uint pointId = this.Scene.AddPoint(x, y);
                    this.Scene.AddStylePoint(pointId, float.MinValue, float.MaxValue, transparantBlue.Value, 8);

                    if (bounds == null)
                    { // create box.
                        bounds = point.Box;
                    }
                    else
                    { // add to the current box.
                        bounds = bounds + point.Box;
                    }
                }
                else if (geometry is LineString)
                { // add the lineString.
                    var lineString = (geometry as LineString);

                    // get x/y.
                    var x = new double[lineString.Coordinates.Count];
                    var y = new double[lineString.Coordinates.Count];
                    for (int idx = 0; idx < lineString.Coordinates.Count; idx++)
                    {
                        x[idx] = _projection.LongitudeToX(
                            lineString.Coordinates[idx].Longitude);
                        y[idx] = _projection.LatitudeToY(
                            lineString.Coordinates[idx].Latitude);
                    }

                    // set the default color if none is given.
                    SimpleColor blue = SimpleColor.FromKnownColor(KnownColor.Blue);
                    SimpleColor transparantBlue = SimpleColor.FromArgb(128,
                                                                       blue.R, blue.G, blue.B);

                    uint? pointsId = this.Scene.AddPoints(x, y);
                    if (pointsId.HasValue)
                    {
                        this.Scene.AddStyleLine(pointsId.Value, float.MinValue, float.MaxValue, transparantBlue.Value, 8);

                        if (bounds == null)
                        { // create box.
                            bounds = lineString.Box;
                        }
                        else
                        { // add to the current box.
                            bounds = bounds + lineString.Box;

                        }
                    }
                }
            }
            return bounds;
		}
		
		#endregion
	}
}
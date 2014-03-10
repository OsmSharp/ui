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

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using OsmSharp.Collections.Cache;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Tiles;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;
using System;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A tile layer.
    /// </summary>
    public class LayerTile : Layer
    {
        /// <summary>
        /// Holds the tile url.
        /// </summary>
        private readonly string _tilesURL;

        /// <summary>
        /// Holds the LRU cache.
        /// </summary>
        private readonly LRUCache<Tile, Image2D> _cache;

        /// <summary>
        /// Holds the offset to calculate the minimum zoom.
        /// </summary>
        private const float _zoomMinOffset = 0.5f;

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        /// <param name="tilesURL">The tiles URL.</param>
        public LayerTile(string tilesURL)
            : this(tilesURL, 200)
        {

        }

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        /// <param name="tilesURL">The tiles URL.</param>
        /// <param name="tileCacheSize">The tile cache size.</param>
        public LayerTile(string tilesURL, int tileCacheSize)
        {
            _tilesURL = tilesURL;
            _cache = new LRUCache<Tile, Image2D>(tileCacheSize);

            _projection = new WebMercator();
        }

        /// <summary>
        /// Holds the tile range of the current view.
        /// </summary>
        private TileRange _tileRange = null;

        /// <summary>
        /// Holds the map projection.
        /// </summary>
        private readonly IProjection _projection;

        /// <summary>
        /// Loads the tiles async.
        /// </summary>
        private void TilesLoading()
        {
            TileRange tileRange = null;

            // build the tile range list.
            List<Tile> tiles = null;
            lock (_tileRange)
            { // make sure the tile range does not change.
                tiles = new List<Tile>(
                    _tileRange);
                tileRange = _tileRange;
            }
            foreach (Tile tile in tiles)
            {
                if (_tileRange != tileRange)
                { // keep looping until a new different tile range is requested.
                    break;
                }

                // check if tile is cached.
                lock (_cache)
                {
                    Image2D image;
                    if (!_cache.TryGet(tile, out image))
                    { // the tile has to be loaded.
                        ThreadPool.QueueUserWorkItem(
                            new WaitCallback(LoadTile), 
                            new KeyValuePair<TileRange, Tile>(tileRange, tile));
                    }
                }
            }
        }

        /// <summary>
        /// Loads one tile.
        /// </summary>
        private void LoadTile(object state)
        {
            // get job.
            KeyValuePair<TileRange, Tile> job = (KeyValuePair<TileRange, Tile>)state;

            if (_tileRange != job.Key)
            { // only load tile if range is unchanged.
                return;
            }

            // a tile was found to load.
            Tile tile = job.Value;
            Image2D image2D;
            lock (_cache)
            { // check again for the tile.
                if (_cache.TryGet(tile, out image2D))
                { // tile is already there!
                    return;
                }
            }

            // load the tile.
            string url = string.Format(_tilesURL,
                                       tile.Zoom,
                                       tile.X,
                                       tile.Y);
            var request = (HttpWebRequest)HttpWebRequest.Create(
                url);
            request.Accept = "text/html, image/png, image/jpeg, image/gif, */*";
            //request.Headers[HttpRequestHeader.UserAgent] = "OsmSharp/4.0";

            try
            {

                WebResponse myResp = request.GetResponse();
                Stream stream = myResp.GetResponseStream();
                byte[] image = null;
                if (stream != null)
                {
                    // there is data: read it.
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);

                    image = memoryStream.ToArray();
                    var delta = 0.0015;
                    var box = tile.ToBox(_projection);
                    image2D = new Image2D(box.Min[0], box.Min[1], box.Max[1] + delta, box.Max[0] + delta, image,
                        (float)_projection.ToZoomFactor(tile.Zoom - _zoomMinOffset),
                        (float)_projection.ToZoomFactor(tile.Zoom + (1 - _zoomMinOffset)));

                    lock (_cache)
                    { // add the result to the cache.
                        _cache.Add(tile, image2D);
                    }

                    // raise the layer changed event.
                    this.RaiseLayerChanged();
                }
            }
            catch(Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Returns all primitives from this layer visible for the given parameters.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        internal override IEnumerable<Primitive2D> Get(float zoomFactor, View2D view)
        {
            var primitives = new List<Primitive2D>();
            lock (_cache)
            {
                foreach (var tile in _cache)
                {
                    if (tile.Value.IsVisibleIn(view, zoomFactor))
                    {
                        primitives.Add(tile.Value);
                    }
                }
            }
            return primitives;
        }

        /// <summary>
        /// Notifies this layer the mapview has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        internal override void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view)
        {
            // calculate the current zoom level.
            var zoomLevel = (int)System.Math.Round(map.Projection.ToZoomLevel(zoomFactor), 0);

			// build the boundingbox.
			var viewBox = view.OuterBox;
			var box = new GeoCoordinateBox (map.Projection.ToGeoCoordinates (viewBox.Min [0], viewBox.Min [1]),
			                                map.Projection.ToGeoCoordinates (viewBox.Max [0], viewBox.Max [1]));

            // build the tile range.        
            lock (_cache)
            { // make sure the tile range is not in use.
                _tileRange = TileRange.CreateAroundBoundingBox(box, zoomLevel);
            }

            this.TilesLoading();
        }
    }
}
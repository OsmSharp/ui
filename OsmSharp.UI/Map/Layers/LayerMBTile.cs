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
using System.Linq;
using System.Text;
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Tiles;
using OsmSharp.UI.Data.SQLite;
using OsmSharp.Collections.Cache;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Renderer.Images;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer that uses a MBTile SQLite database as a tile-source.
    /// </summary>
    public class LayerMBTile : Layer, IComparer<Primitive2D>
    {
        /// <summary>
        /// Holds the connection.
        /// </summary>
        private SQLiteConnectionBase _connection;

        /// <summary>
        /// Holds the minimum zoom level.
        /// </summary>
        private readonly int _minZoomLevel = 0;

        /// <summary>
        /// Holds the maximum zoom level.
        /// </summary>
        private readonly int _maxZoomLevel = 18;

        /// <summary>
        /// Holds the offset to calculate the minimum zoom.
        /// </summary>
        private const float _zoomMinOffset = 0.5f;

        /// <summary>
        /// Holds the LRU cache.
        /// </summary>
        private LRUCache<Tile, Image2D> _cache;

        /// <summary>
        /// Holds the native image cache.
        /// </summary>
        private NativeImageCacheBase _nativeImageCache;

        /// <summary>
        /// Holds the map projection.
        /// </summary>
        private readonly IProjection _projection;
            
        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        /// <param name="connection">The SQLite connection to the MBTiles.</param>
        /// <param name="tileCacheSize">The size of tiles to cache.</param>
        public LayerMBTile(SQLiteConnectionBase connection, int tileCacheSize)
        {
            _nativeImageCache = NativeImageCacheFactory.Create();
            _connection = connection;

            _cache = new LRUCache<Tile, Image2D>(tileCacheSize);
            _cache.OnRemove += OnRemove;
            _projection = new WebMercator();
        }

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        /// <param name="connection">The SQLite connection to the MBTiles.</param>
        public LayerMBTile(SQLiteConnectionBase connection)
            : this(connection, 40)
        {

        }

        /// <summary>
        /// Handes the OnRemove-event from the cache.
        /// </summary>
        /// <param name="image"></param>
        private void OnRemove(Image2D image)
        { // dispose of the image after it is removed from the cache.
            _nativeImageCache.Release(image.NativeImage);
            image.NativeImage = null;

            OsmSharp.Logging.Log.TraceEvent("LayerMBTile", Logging.TraceEventType.Information, "OnRemove: {0}", _cache.Count);
        }

        /// <summary>
        /// Returns all primitives from this layer visible for the given parameters.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        protected internal override IEnumerable<Primitive2D> Get(float zoomFactor, View2D view)
        {
            var primitives = new List<Primitive2D>();
            lock (_cache)
            {
                foreach (var tile in _cache)
                {
                    if (tile.Value.IsVisibleIn(view, zoomFactor))
                    {
                        Image2D temp;
                        var minZoom = _projection.ToZoomFactor(tile.Key.Zoom - _zoomMinOffset);
                        var maxZoom = _projection.ToZoomFactor(tile.Key.Zoom + (1 - _zoomMinOffset));
                        if (zoomFactor < maxZoom && zoomFactor > minZoom)
                        {
                            _cache.TryGet(tile.Key, out temp);
                            primitives.Add(tile.Value);
                        }
                    }
                }
            }

            primitives.Sort(this);
            return primitives;
        }

        /// <summary>
        /// Notifies this layer that the current mapview has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        /// <param name="extraView"></param>
        protected internal override void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view, View2D extraView)
        {
            // calculate the current zoom level.
            var zoomLevel = (int)System.Math.Round(map.Projection.ToZoomLevel(zoomFactor), 0);

            if (zoomLevel >= _minZoomLevel && zoomLevel <= _maxZoomLevel)
            {
                // build the bounding box.
                var viewBox = view.OuterBox;

                // build the tile range.
                var range = TileRange.CreateAroundBoundingBox(new GeoCoordinateBox(map.Projection.ToGeoCoordinates(viewBox.Min[0], viewBox.Min[1]),
                    map.Projection.ToGeoCoordinates(viewBox.Max[0], viewBox.Max[1])), zoomLevel);

				OsmSharp.Logging.Log.TraceEvent ("LayerMBTile", OsmSharp.Logging.TraceEventType.Verbose, string.Format ("Requesting {0} tiles for view.", range.Count));
                
                // request all missing tiles.
                lock (_connection)
                { // make sure the connection is accessed synchronously. 
                    // TODO: Investigate the SQLite multithreaded behaviour..
                    // TODO: this a very naive way of loading these tiles. Find a way to query SQLite more efficiently
                    // TODO: find a way to have some cached tiles.
                    foreach (var tile in range.EnumerateInCenterFirst())
                    {
                        Image2D value;
						if(_cache.TryGet(tile, out value))
                        {
							// Tile is already in cache. We used TryGet, and not TryPeek, to inform the cache
							// that we intend to use the datum in question.
                            continue;
                        }
                        Tile invertTile = tile.InvertY();

                        var tiles = _connection.Query<tiles>("SELECT * FROM tiles WHERE zoom_level = ? AND tile_column = ? AND tile_row = ?",
                            invertTile.Zoom, invertTile.X, invertTile.Y);
                        foreach (var mbTile in tiles)
                        {
                            var box = tile.ToBox(_projection);
                            var nativeImage = _nativeImageCache.Obtain(mbTile.tile_data);
                            var image2D = new Image2D(box.Min[0], box.Min[1], box.Max[1], box.Max[0], nativeImage);
                            image2D.Layer = (uint)(_maxZoomLevel - tile.Zoom);

                            lock (_cache)
                            { // add the result to the cache.
                                _cache.Add(tile, image2D);
                            }
                        }
                    }
                }

                OsmSharp.Logging.Log.TraceEvent("LayerMBTile", Logging.TraceEventType.Information, "Cached tiles: {0}", _cache.Count);
            }
        }

        private class tiles
        {
            public int tile_row { get; set; }

            public int tile_column { get; set; }

            public int zoom_level { get; set; }

            public byte[] tile_data { get; set; }
        }

        #region IComparer implementation

        /// <summary>
        /// Compare the specified x and y.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        int IComparer<Primitive2D>.Compare(Primitive2D x, Primitive2D y)
        {
            return y.Layer.CompareTo(x.Layer);
        }

        #endregion

        /// <summary>
        /// Closes this layer.
        /// </summary>
        public override void Close()
        {
            base.Close();

            lock (_cache)
            {
                _cache.OnRemove = null;
                _cache.Clear();
            }
            _cache = null;

            // flushes all images from the cache.
            _nativeImageCache.Flush();

            // closes the connection.
            _connection.Close();
        }
    }
}

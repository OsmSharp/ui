// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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

using OsmSharp.Collections;
using OsmSharp.Collections.Cache;
using OsmSharp.Logging;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Tiles;
using OsmSharp.UI.Data.SQLite;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.UI.Renderer.Scene;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace OsmSharp.UI.Map.Layers.VectorTiles
{

    /// <summary>
    /// A tile source that reads tiles for a directory.
    /// </summary>
    public class MBTilesVectorTileSource : IVectorTileSource
    {
        private readonly SQLiteConnectionBase _connection; // holds the connection.
        private readonly LRUCache<ulong, IEnumerable<Primitive2D>> _cache; // the cached tiles.
        private LimitedStack<ulong> _stack; // holds the tiles queue for load.
        private readonly Timer _timer; // Holds the timer.

        /// <summary>
        /// Creates a new directory tile source.
        /// </summary>
        public MBTilesVectorTileSource(SQLiteConnectionBase connection)
        {
            _connection = connection;
            _cache = new LRUCache<ulong, IEnumerable<Primitive2D>>(60);
            _stack = new LimitedStack<ulong>(60, 60);
            _timer = new Timer(this.LoadQueuedTiles, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        private bool _paused = false;

        /// <summary>
        /// Returns true if this source is paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return _paused;
            }
        }

        /// <summary>
        /// Gets the projection.
        /// </summary>
        public IProjection Projection
        {
            get
            {
                return new WebMercator();
            }
        }

        /// <summary>
        /// Event raised when this source has changed.
        /// </summary>
        public event Action SourceChanged;

        /// <summary>
        /// Event raised when a tile is loaded.
        /// </summary>
        public event Action<Tile, IEnumerable<Primitive2D>> TileLoaded;

        /// <summary>
        /// Closes this source.
        /// </summary>
        public void Close()
        {

        }

        /// <summary>
        /// Disposes of all the native resources associated with this source.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Pauses this source.
        /// </summary>
        public void Pause()
        {
            _paused = true;
        }

        /// <summary>
        /// Prepares this source.
        /// </summary>
        public void Prepare(TileRange range)
        {
            var stack = new LimitedStack<ulong>(_stack.Limit, _stack.Limit);

            foreach (var tile in range.EnumerateInCenterFirst().Reverse())
            {
                if (tile.IsValid)
                { // make sure all tiles are valid.
                    IEnumerable<Primitive2D> temp;
                    if (!_cache.TryPeek(tile.Id, out temp))
                    { // not cached and not loading.
                        stack.Push(tile.Id);

                        OsmSharp.Logging.Log.TraceEvent("VectorTileSourceBase", TraceEventType.Information,
                            "Queued tile:" + tile.ToString());
                    }
                }
            }
            _timer.Change(0, 200);

            _stack = stack;
        }

        /// <summary>
        /// A function to filter primitives.
        /// </summary>
        public Func<IEnumerable<Primitive2D>, IEnumerable<Primitive2D>> FilterPrimitives { get; set; }

        /// <summary>
        /// The timer callback that triggers loading queued tiles.
        /// </summary>
        private void LoadQueuedTiles(object status)
        {
            try
            {
                if (_paused)
                { // stop loading tiles.
                    return;
                }

                var tiles = new List<ulong>();
                var stack = _stack;
                var cache = _cache;
                lock (stack)
                { // make sure that access to the queue is synchronized.
                    while (stack.Count > 0 && tiles.Count < 4)
                    { // there are queued items.
                        var tileId = stack.Pop();

                        IEnumerable<Primitive2D> existing;
                        if (!cache.TryGet(tileId, out existing))
                        {
                            tiles.Add(tileId);
                        }
                    }
                }

                IEnumerable<Primitive2D> scene = null;
                var query = new StringBuilder("select zoom_level, tile_column, tile_row, tile_data from tiles where ");
                if (tiles.Count > 0)
                {
                    var tile = new Tile(tiles[0]);
                    query.AppendFormat("(zoom_level = {0} and tile_column = {1} and tile_row = {2})",
                        tile.Zoom, tile.X, tile.Y);
                    for (var i = 1; i < tiles.Count; i++)
                    {
                        tile = new Tile(tiles[i]);
                        query.AppendFormat(" or (zoom_level = {0} and tile_column = {1} and tile_row = {2})",
                            tile.Zoom, tile.X, tile.Y);
                    }

                    OsmSharp.Logging.Log.TraceEvent("MBTilesVectorTileSource", TraceEventType.Information,
                        query.ToInvariantString());

                    // request all missing tiles.
                    List<tiles> tileResults = null;
                    lock (_connection)
                    {
                        tileResults = _connection.Query<tiles>(query.ToString());
                    }

                    var changed = false;
                    OsmSharp.Logging.Log.TraceEvent("MBTilesVectorTileSource", TraceEventType.Information,
                        "Tiles loaded: {0}", tileResults.Count);
                    foreach (var mbTile in tileResults)
                    {
                        using (var stream = new MemoryStream(mbTile.tile_data))
                        {
                            tile = new Tile(mbTile.tile_column, mbTile.tile_row, mbTile.zoom_level);
                            var box = tile.Box.Resize(0.001);
                            var source = Scene2D.Deserialize(stream, true);
                            var zoomFactor = (float)this.Projection.ToZoomFactor(tile.Zoom);
                            var view = View2D.CreateFromBounds(
                                this.Projection.LatitudeToY(box.MaxLat),
                                this.Projection.LongitudeToX(box.MinLon),
                                this.Projection.LatitudeToY(box.MinLat),
                                this.Projection.LongitudeToX(box.MaxLon));
                            scene = source.Get(view, zoomFactor);

                            if (scene == null)
                            {
                                scene = Enumerable.Empty<Primitive2D>();
                            }
                            changed = true;

                            if (this.FilterPrimitives != null)
                            {
                                scene = this.FilterPrimitives(scene);
                            }

                            lock (_cache)
                            {
                                _cache.Add(tile.Id, scene);
                            }

                            if (this.TileLoaded != null)
                            {
                                this.TileLoaded(tile, scene);
                            }
                        }
                    }

                    if (changed && this.SourceChanged != null)
                    {
                        this.SourceChanged();
                    }
                }

                if (_stack.Count == 0)
                { // dispose of timer.
                    _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerVectorTiles", TraceEventType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Resumes this source.
        /// </summary>
        public void Resume()
        {
            _paused = false;
        }

        /// <summary>
        /// Tries to get a tile.
        /// </summary>
        public bool TryGet(Tile tile, out IEnumerable<Primitive2D> scene)
        {
            return _cache.TryGet(tile.Id, out scene);
        }

        /// <summary>
        /// Tries to get a tile without logging a cache hit.
        /// </summary>
        public bool TryPeek(Tile tile, out IEnumerable<Primitive2D> scene)
        {
            return _cache.TryPeek(tile.Id, out scene);
        }
        private class tiles
        {
            public int tile_row { get; set; }

            public int tile_column { get; set; }

            public int zoom_level { get; set; }

            public byte[] tile_data { get; set; }
        }
    }
}

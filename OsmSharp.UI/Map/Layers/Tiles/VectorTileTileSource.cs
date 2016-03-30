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

using System;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Tiles;
using OsmSharp.UI.Map.Layers.VectorTiles;
using OsmSharp.UI.Renderer.Primitives;
using System.Collections.Generic;
using OsmSharp.Collections.Cache;
using OsmSharp.Collections;
using System.Threading;
using System.Linq;
using OsmSharp.Logging;

namespace OsmSharp.UI.Map.Layers.Tiles
{
    /// <summary>
    /// A tile source that renders tiles.
    /// </summary>
    public class VectorTileTileSource : ITileSource
    {
        private readonly IVectorTileSource _vectorTileSource;
        private readonly Func<Tile, IEnumerable<Primitive2D>, Image2D> _renderTile;
        private readonly LRUCache<ulong, Image2D> _cache; // the cached tiles.
        private readonly LimitedStack<ulong> _stack; // holds the tiles queue for load.
        private readonly Timer _timer; // Holds the timer.
        private readonly int _overzoom;

        /// <summary>
        /// Creates a new vector tile tile source.
        /// </summary>
        public VectorTileTileSource(IVectorTileSource vectorTileSource, Func<Tile, IEnumerable<Primitive2D>, Image2D> renderTile,
            int overzoom = 16)
        {
            _vectorTileSource = vectorTileSource;
            _renderTile = renderTile;
            _cache = new LRUCache<ulong, Image2D>(100);
            _stack = new LimitedStack<ulong>(60, 60);
            _timer = new Timer(this.LoadQueuedTiles, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            _overzoom = overzoom;

            _vectorTileSource.TileLoaded += _vectorTileSource_TileLoaded;
        }


        private List<ulong> _restack = new List<ulong>();

        /// <summary>
        /// Render a tile.
        /// </summary>
        private void _vectorTileSource_TileLoaded(Tile tile, IEnumerable<Primitive2D> scene)
        {
            var image = _renderTile(tile, scene);

            lock (_cache)
            {
                _cache.Add(tile.Id, image);
            }

            if (this.SourceChanged != null)
            {
                this.SourceChanged();
            }
        }

        private bool _paused = false;

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

                lock (_stack)
                { // make sure that access to the queue is synchronized.
                    _restack.Clear();
                    int queue = _stack.Count;
                    while (_stack.Count >= queue && _stack.Count > 0)
                    { // there are queued items.
                        var tile = new Tile(_stack.Pop());

                        var overzoomTile = tile;
                        while(overzoomTile.Zoom > _overzoom)
                        {
                            overzoomTile = overzoomTile.Parent;
                        }
                        
                        Image2D image;
                        lock(_cache)
                        {
                            if (_cache.TryGet(tile.Id, out image))
                            {
                                continue;
                            }
                        }

                        IEnumerable<Primitive2D> scene;
                        if (_vectorTileSource.TryGet(overzoomTile, out scene))
                        {
                            image = _renderTile(tile, scene);

                            lock (_cache)
                            {
                                _cache.Add(tile.Id, image);
                            }
                        }
                        else
                        {
                            _restack.Add(tile.Id);
                        }
                        
                        foreach(var id in _restack)
                        {
                            _stack.Push(id);
                        }
                    }

                    if (_stack.Count == 0)
                    { // dispose of timer.
                        _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    }
                }

                if (this.SourceChanged != null)
                {
                    this.SourceChanged();
                }
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerVectorTiles", TraceEventType.Error, ex.Message);
            }
        }

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
            var overzoomRange = range;
            while (overzoomRange.Zoom > _overzoom)
            {
                var box1 = (new Tile(range.XMin, range.YMin, range.Zoom)).Box;
                var box2 = (new Tile(range.XMax, range.YMax, range.Zoom)).Box;
                box1.ExpandWith(box2.TopLeft);
                box1.ExpandWith(box2.BottomRight);

                overzoomRange = TileRange.CreateAroundBoundingBox(box1, overzoomRange.Zoom - 1);
            }
            _vectorTileSource.Prepare(overzoomRange);

            // build the tile range.
            lock (_stack)
            { // make sure the tile range is not in use.
                _stack.Clear();

                lock (_cache)
                {
                    foreach (var tile in range.EnumerateInCenterFirst().Reverse())
                    {
                        if (tile.IsValid)
                        { // make sure all tiles are valid.
                            Image2D temp;
                            if (!_cache.TryPeek(tile.Id, out temp))
                            { // not cached and not loading.
                                _stack.Push(tile.Id);

                                OsmSharp.Logging.Log.TraceEvent("VectorTileSourceBase", TraceEventType.Information,
                                    "Queued tile:" + tile.ToString());
                            }
                        }
                    }
                    _timer.Change(0, 100);
                }
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
        public bool TryGet(Tile tile, out Image2D image)
        {
            return _cache.TryGet(tile.Id, out image);
        }

        /// <summary>
        /// Tries to get a tile without logging a cache hit.
        /// </summary>
        public bool TryPeek(Tile tile, out Image2D image)
        {
            return _cache.TryPeek(tile.Id, out image);
        }
    }
}
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
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Tiles;
using OsmSharp.UI.Renderer.Images;
using OsmSharp.UI.Renderer.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace OsmSharp.UI.Map.Layers.Tiles
{
    /// <summary>
    /// A url-based tile source.
    /// </summary>
    public class UrlTileSource : ITileSource
    {
        private readonly string _tilesURL; // hold the tile url.
        private readonly LRUCache<ulong, Image2D> _cache; // the cached tiles.
        private const int _maxThreads = 4; // maximum concurrent threads to get tiles.
        private readonly LimitedStack<ulong> _stack; // holds the tiles queue for load.
        private readonly Dictionary<ulong, int> _attempts; // holds the attemps per tile.
        private readonly Timer _timer; // Holds the timer.
        private readonly IProjection _projection; // Holds the projection.
        private readonly NativeImageCacheBase _nativeImageCache; // Holds the native image cache.

        /// <summary>
        /// Creates a new tile source.
        /// </summary>
        public UrlTileSource(string url, IProjection projection, int tileCacheSize)
        {
            _nativeImageCache = NativeImageCacheFactory.Create();
            _tilesURL = url;
            _cache = new LRUCache<ulong, Image2D>(tileCacheSize);
            // _cache.OnRemove += OnRemove;
            _stack = new LimitedStack<ulong>(tileCacheSize, tileCacheSize);
            _timer = new Timer(this.LoadQueuedTiles, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            _attempts = new Dictionary<ulong, int>();
            _suspended = false;
            _projection = projection;
        }

        private bool _suspended; // Holds the suspended flag.
        private int _currentZoom; // Holds the current zoom.
        private HashSet<ulong> _loading = new HashSet<ulong>(); // Holds the tiles that are currently loading.

        /// <summary>
        /// Event raised when new data is available.
        /// </summary>
        public event Action SourceChanged;

        /// <summary>
        /// Gets or sets a function to override tile loading.
        /// </summary>
        public Func<Tile, Stream> TileOverride { get; set; }

        /// <summary>
        /// Raises the source changed event.
        /// </summary>
        protected void RaiseSourceChanged()
        {
            if (this.SourceChanged != null)
            {
                OsmSharp.Logging.Log.TraceEvent("Layer.RaiseLayerChanged (Before)", Logging.TraceEventType.Information,
                    "RaiseLayerChanged");
                this.SourceChanged();
                OsmSharp.Logging.Log.TraceEvent("Layer.RaiseLayerChanged (After)", Logging.TraceEventType.Information,
                    "RaiseLayerChanged");
            }
        }

        /// <summary>
        /// Gets the scene for the given tile.
        /// </summary>
        public bool TryGet(Tile tile, out Image2D image)
        {
            return _cache.TryGet(tile.Id, out image);
        }

        /// <summary>
        /// Gets the scene for the given tile without logging this is a cache hit.
        /// </summary>
        public bool TryPeek(Tile tile, out Image2D image)
        {
            return _cache.TryPeek(tile.Id, out image);
        }

        /// <summary>
        /// Prepares this source for the given tile range.
        /// </summary>
        public void Prepare(TileRange range)
        {
            // build the tile range.
            lock (_stack)
            { // make sure the tile range is not in use.
                _stack.Clear();

                lock (_cache)
                {
                    _currentZoom = range.Zoom;
                    foreach (var tile in range.EnumerateInCenterFirst().Reverse())
                    {
                        if (tile.IsValid)
                        { // make sure all tiles are valid.
                            Image2D temp;
                            if (!_cache.TryPeek(tile.Id, out temp) &&
                                !_loading.Contains(tile.Id))
                            { // not cached and not loading.
                                _stack.Push(tile.Id);

                                OsmSharp.Logging.Log.TraceEvent("VectorTileSourceBase", Logging.TraceEventType.Information,
                                    "Queued tile:" + tile.ToString());
                            }
                        }
                    }
                    _timer.Change(0, 100);

                    if (_stack.Count > 0)
                    { // reset the attempts.
                        lock (_attempts)
                        {
                            _attempts.Clear();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the projection.
        /// </summary>
        public IProjection Projection
        {
            get
            {
                return _projection;
            }
        }

        /// <summary>
        /// The timer callback that triggers loading queued tiles.
        /// </summary>
        private void LoadQueuedTiles(object status)
        {
            try
            {
                if (_suspended)
                { // stop loading tiles.
                    return;
                }

                lock (_stack)
                { // make sure that access to the queue is synchronized.
                    int queue = _stack.Count;
                    while (_stack.Count > queue + _loading.Count - _maxThreads && _stack.Count > 0)
                    { // there are queued items.
                        LoadTile(_stack.Pop());
                    }

                    if (_stack.Count == 0)
                    { // dispose of timer.
                        _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    }
                }
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerVectorTiles", Logging.TraceEventType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Loads one tile.
        /// </summary>
        protected void LoadTile(object state)
        {
            try
            {
                if (_suspended)
                { // stop loading tiles.
                    return;
                }

                // a tile was found to load.
                var tile = new Tile((ulong)state);

                // only load tiles from the same zoom-level.
                if (tile.Zoom != _currentZoom)
                { // zoom is different, don't bother!
                    return;
                }

                Image2D image;
                if (_cache == null)
                {
                    return;
                }
                lock (_cache)
                { // check again for the tile.
                    if (_cache.TryGet(tile.Id, out image))
                    { // tile is already there!
                        _cache.Add(tile.Id, image); // add again to update status.
                        return;
                    }

                    _loading.Add(tile.Id);
                }

                // attempt the override.
                if (TileOverride != null)
                {
                    var overrideStream = this.TileOverride(tile);
                    if (overrideStream != null)
                    {
                        this.ProcessResponseStream(overrideStream, tile);
                        return;
                    }
                }

                // load the tile.
                string url = this.FormatURL(tile);

                var request = (HttpWebRequest)HttpWebRequest.Create(
                                  url);
                //request.Accept = "text/html, image/png, image/jpeg, image/gif, */*";
                //request.Headers[HttpRequestHeader.UserAgent] = "OsmSharp/4.0";

                OsmSharp.Logging.Log.TraceEvent("LayerVectorTiles", Logging.TraceEventType.Information, "Request tile@" + url);

                Action<HttpWebResponse> responseAction = ((HttpWebResponse obj) =>
                {
                    this.Response(obj, tile);

                    _loading.Remove(tile.Id);
                });
                Action wrapperAction = () =>
                {
                    request.BeginGetResponse(new AsyncCallback((iar) =>
                    {
                        try
                        {
                            var response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                            responseAction(response);
                        }
                        catch (WebException ex)
                        { // catch webexceptions.
                            if (ex.Response is HttpWebResponse &&
                                ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound ||
                                (ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.Forbidden))
                            { // do not retry loading tile.
                                return;
                            }
                            else
                            { // retry loading tile here.
                                _loading.Remove(tile.Id);

                                lock (_attempts)
                                {
                                    int count;
                                    if (!_attempts.TryGetValue(tile.Id, out count))
                                    { // first attempt.
                                        count = 1;
                                        _attempts.Add(tile.Id, count);
                                    }
                                    else
                                    { // increase attempt count.
                                        _attempts[tile.Id] = count++;
                                    }
                                    if (count < 3)
                                    { // not yet reached maximum. 
                                        lock (_stack)
                                        {
                                            _stack.Push(tile.Id);
                                            _timer.Change(0, 150);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        { // oops, exceptions that are not webexceptions!?
                            OsmSharp.Logging.Log.TraceEvent("LayerVectorTiles", Logging.TraceEventType.Error, ex.Message);
                        }
                    }), request);
                };
                wrapperAction.BeginInvoke(new AsyncCallback((iar) =>
                {
                    var action = (Action)iar.AsyncState;
                    action.EndInvoke(iar);
                }), wrapperAction);
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerVectorTiles", Logging.TraceEventType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Processes the response stream.
        /// </summary>
        private void ProcessResponseStream(Stream stream, Tile tile)
        {
            Image2D image = null;
            using (stream)
            { // read vector data.
                image = this.Get(tile, stream);
            }

            if (image != null)
            {
                lock (_cache)
                { // add the result to the cache.
                    _cache.Add(tile.Id, image);
                }

                if (!_suspended)
                { // only raise the event when not suspended but do no throw away a tile, that would be a waste.
                    this.RaiseSourceChanged();
                }
            }
        }

        /// <summary>
        /// Response the specified myResp and tile.
        /// </summary>
        private void Response(WebResponse myResp, Tile tile)
        {
            if (_cache == null) { return; }
            try
            {
                var vectorTileString = string.Empty;

                var stream = myResp.GetResponseStream();
                if (stream == null)
                {
                    OsmSharp.Logging.Log.TraceEvent("LayerVectorTiles", Logging.TraceEventType.Error, "No response stream!");
                }
                this.ProcessResponseStream(stream, tile);
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerVectorTiles", Logging.TraceEventType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Converts the response stream into a scene object.
        /// </summary>
        protected virtual Image2D Get(Tile tile, Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            var image = memoryStream.ToArray();
            memoryStream.Dispose();

            var box = tile.ToBox(_projection);
            var nativeImage = _nativeImageCache.Obtain(image);
            return new Image2D(box.Min[0], box.Min[1], box.Max[1], box.Max[0], nativeImage);
        }

        /// <summary>
        /// Returns a formatted URL to get the tile from.
        /// </summary>
        private string FormatURL(Tile tile)
        {
            if (_tilesURL.Contains("{x}"))
            { // assume /{z}/{x}/{y} format.
                return _tilesURL.Replace("{z}", tile.Zoom.ToString())
                    .Replace("{x}", tile.X.ToString())
                    .Replace("{y}", tile.Y.ToString());
            }
            // assume {0}/{1}/{2} format.
            return string.Format(_tilesURL,
                                    tile.Zoom,
                                    tile.X,
                                    tile.Y);
        }

        /// <summary>
        /// Pauses all activity in this layer.
        /// </summary>
        public void Pause()
        {
            // set suspended flag.
            _suspended = true;

            lock (_stack)
            { // empty stack to tiles to-be-loaded but keep cache until layer is closed!
                _stack.Clear();
            }
        }

        /// <summary>
        /// Returns true if this flag is paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return _suspended;
            }
        }

        /// <summary>
        /// Resumes the activity in this layer.
        /// </summary>
        public void Resume()
        { // here only the suspended flag needs to be reset, just wait for the next request.
            _suspended = false;
        }

        /// <summary>
        /// Closes this source.
        /// </summary>
        public void Close()
        {
            try
            {
                if (_timer != null)
                {
                    _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    _timer.Dispose();
                }

                lock (_cache)
                {
                    _cache.OnRemove = null;
                    _cache.Clear();
                }

                lock (_stack)
                { // make sure the tile range is not in use.
                    _stack.Clear();
                }
            }
            catch (Exception ex)
            {
                OsmSharp.Logging.Log.TraceEvent("VectorTileSourceBase", Logging.TraceEventType.Error, ex.Message);
            }
        }

        #region Disposing-pattern

        /// <summary>
        /// Diposes of all resources associated with this object.
        /// </summary>
        public void Dispose()
        {
            // If this function is being called the user wants to release the
            // resources. lets call the Dispose which will do this for us.
            Dispose(true);

            // Now since we have done the cleanup already there is nothing left
            // for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Diposes of all resources associated with this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                //someone want the deterministic release of all resources
                //Let us release all the managed resources
            }
            else
            {
                // Do nothing, no one asked a dispose, the object went out of
                // scope and finalized is called so lets next round of GC 
                // release these resources
            }
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~UrlTileSource()
        {
            // The object went out of scope and finalized is called
            // Lets call dispose in to release unmanaged resources 
            // the managed resources will anyways be released when GC 
            // runs the next time.
            Dispose(false);
        }

        #endregion
    }
}
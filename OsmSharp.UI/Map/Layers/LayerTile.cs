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

using OsmSharp.Collections;
using OsmSharp.Collections.Cache;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Tiles;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Images;
using OsmSharp.UI.Renderer.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A tile layer.
    /// </summary>
    public class LayerTile : Layer, IComparer<Primitive2D>
    {
        /// <summary>
        /// Holds the tile url.
        /// </summary>
        private readonly string _tilesURL;
        /// <summary>
        /// Holds the current zoom.
        /// </summary>
        private int _currentZoom = -1;
        /// <summary>
        /// Holds the LRU cache.
        /// </summary>
        private readonly LRUCache<Tile, Image2D> _cache;
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
        /// Holds the maximum threads.
        /// </summary>
        private const int _maxThreads = 4;
        /// <summary>
        /// Holds the tile to-load queue.
        /// </summary>
        private readonly LimitedStack<Tile> _stack;
        /// <summary>
        /// Holds the number of failed attempts at loading one tile.
        /// </summary>
        private readonly Dictionary<Tile, int> _attempts;
        /// <summary>
        /// Holds the timer.
        /// </summary>
        private Timer _timer;
        /// <summary>
        /// Holds the native image cache.
        /// </summary>
        private NativeImageCacheBase _nativeImageCache;

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        /// <param name="tilesURL">The tiles URL.</param>
        /// <param name="nativeImageCache">The native image cache.</param>
        public LayerTile(NativeImageCacheBase nativeImageCache, string tilesURL)
            : this(nativeImageCache, tilesURL, 80)
        {

        }

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        /// <param name="tilesURL">The tiles URL.</param>
        /// <param name="tileCacheSize">The tile cache size.</param>
        /// <param name="nativeImageCache">The native image cache.</param>
        public LayerTile(NativeImageCacheBase nativeImageCache, string tilesURL, int tileCacheSize)
        {
            _nativeImageCache = nativeImageCache;
            _tilesURL = tilesURL;
            _cache = new LRUCache<Tile, Image2D>(tileCacheSize);
            _cache.OnRemove += OnRemove;
            _stack = new LimitedStack<Tile>(tileCacheSize, tileCacheSize);
            _timer = new Timer(this.LoadQueuedTiles, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            _attempts = new Dictionary<Tile, int>();

            _projection = new WebMercator();
        }

        /// <summary>
        /// Handes the OnRemove-event from the cache.
        /// </summary>
        /// <param name="image"></param>
        private void OnRemove(Image2D image)
        { // dispose of the image after it is removed from the cache.
            _nativeImageCache.Release(image.NativeImage);
            image.NativeImage = null;
        }

        /// <summary>
        /// Holds the map projection.
        /// </summary>
        private readonly IProjection _projection;

        /// <summary>
        /// The timer callback.
        /// </summary>
        /// <param name="status">Status.</param>
        private void LoadQueuedTiles(object status)
        {
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

        /// <summary>
        /// Holds the tiles that are currently loading.
        /// </summary>
        private HashSet<Tile> _loading = new HashSet<Tile>();

        /// <summary>
        /// Loads one tile.
        /// </summary>
        private void LoadTile(object state)
        {
            // a tile was found to load.
            Tile tile = state as Tile;

            // only load tiles from the same zoom-level.
            if (tile.Zoom != _currentZoom)
            { // zoom is different, don't bother!
                return;
            }

            Image2D image2D;
            lock (_cache)
            { // check again for the tile.
                if (_cache.TryGet(tile, out image2D))
                { // tile is already there!
                    _cache.Add(tile, image2D); // add again to update status.
                    return;
                }

                _loading.Add(tile);
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

            OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Information, "Request tile@" + url);

            try
            {
                Action<HttpWebResponse> responseAction = ((HttpWebResponse obj) =>
                { 
                    this.Response(obj, tile);

                    _loading.Remove(tile);
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
                                _loading.Remove(tile);

                                lock (_attempts)
                                {
                                    int count;
                                    if (!_attempts.TryGetValue(tile, out count))
                                    { // first attempt.
                                        count = 1;
                                        _attempts.Add(tile, count);
                                    }
                                    else
                                    { // increase attempt count.
                                        _attempts[tile] = count++;
                                    }
                                    if (count < 3)
                                    { // not yet reached maximum. 
                                        lock (_stack)
                                        {
                                            _stack.Push(tile);
                                            _timer.Change(0, 150);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        { // oops, exceptions that are not webexceptions!?
                            OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Error, ex.Message);
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
                OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Response the specified myResp and tile.
        /// </summary>
        /// <param name="myResp">My resp.</param>
        /// <param name="tile">Tile.</param>
        private void Response(WebResponse myResp, Tile tile)
        {
            Stream stream = myResp.GetResponseStream();
            byte[] image = null;
            if (stream != null)
            {
                using (stream)
                {
                    // there is data: read it.
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);

                    image = memoryStream.ToArray();
                    memoryStream.Dispose();

                    var box = tile.ToBox(_projection);
                    var nativeImage = _nativeImageCache.Obtain(image);
                    var image2D = new Image2D(box.Min[0], box.Min[1], box.Max[1], box.Max[0], nativeImage);
                    image2D.Layer = (uint)(_maxZoomLevel - tile.Zoom);

                    lock (_cache)
                    { // add the result to the cache.
                        _cache.Add(tile, image2D);
                    }
                }

                // raise the layer changed event.
                this.RaiseLayerChanged();
            }
            else
            {
                OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Error, "No response stream!");
            }
        }

        /// <summary>
        /// Holds the previous zoom factor.
        /// </summary>
        private float _previousZoomFactor = 1;
        /// <summary>
        /// Holds the ascending boolean.
        /// </summary>
        private bool _ascending = false;

        /// <summary>
        /// Returns all primitives from this layer visible for the given parameters.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
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

            // set the ascending flag.
            if (_previousZoomFactor != zoomFactor)
            { // only change flag when difference.
                _ascending = (_previousZoomFactor < zoomFactor);
                _previousZoomFactor = zoomFactor;
            }

            primitives.Sort(this);
            return primitives;
        }

        #region IComparer implementation

        /// <summary>
        /// Compare the specified x and y.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        int IComparer<Primitive2D>.Compare(Primitive2D x, Primitive2D y)
        {
            if (_ascending)
            {
                return y.Layer.CompareTo(x.Layer);
            }
            return x.Layer.CompareTo(y.Layer);
        }

        #endregion

        /// <summary>
        /// Notifies this layer the mapview has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        protected internal override void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view, View2D extraView)
        {
            // calculate the current zoom level.
            var zoomLevel = (int)System.Math.Round(map.Projection.ToZoomLevel(zoomFactor), 0);

            if (zoomLevel >= _minZoomLevel && zoomLevel <= _maxZoomLevel)
            {
                // build the bounding box.
                var viewBox = view.OuterBox;
                var box = new GeoCoordinateBox(map.Projection.ToGeoCoordinates(viewBox.Min[0], viewBox.Min[1]),
                              map.Projection.ToGeoCoordinates(viewBox.Max[0], viewBox.Max[1]));

                // build the tile range.
                lock (_stack)
                { // make sure the tile range is not in use.
                    _stack.Clear();

                    lock (_cache)
                    {
                        var tileRange = TileRange.CreateAroundBoundingBox(box, zoomLevel);
                        _currentZoom = zoomLevel;
                        foreach (var tile in tileRange.EnumerateInCenterFirst().Reverse())
                        {
                            if (tile.IsValid)
                            { // make sure all tiles are valid.
                                Image2D temp;
                                if (!_cache.TryPeek(tile, out temp) &&
                                    !_loading.Contains(tile))
                                { // not cached and not loading.
                                    _stack.Push(tile);

                                    OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Information, "Queued tile:" + tile.ToString());
                                }
                            }
                        }
                        _timer.Change(0, 250);
                    }

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
    }
}

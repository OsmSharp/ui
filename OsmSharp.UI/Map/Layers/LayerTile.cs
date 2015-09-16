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
using OsmSharp.IO.Web;
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
using System.Threading;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A tile layer.
    /// </summary>
    public class LayerTile : Layer, IComparer<Primitive2D>, IDisposable
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
        private LRUCache<Tile, Image2D> _cache;
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
        private LimitedStack<Tile> _stack;
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
        /// Holds the suspended flag.
        /// </summary>
        private bool _suspended;

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        /// <param name="tilesURL">The tiles URL.</param>
        public LayerTile(string tilesURL)
            : this(tilesURL, 160)
        {

        }

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        /// <param name="tilesURL">The tiles URL.</param>
        /// <param name="tileCacheSize">The tile cache size.</param>
        public LayerTile(string tilesURL, int tileCacheSize)
        {
            _nativeImageCache = NativeImageCacheFactory.Create();
            _tilesURL = tilesURL;
            _cache = new LRUCache<Tile, Image2D>(tileCacheSize);
            _cache.OnRemove += OnRemove;
            _stack = new LimitedStack<Tile>(tileCacheSize, tileCacheSize);
            _timer = new Timer(this.LoadQueuedTiles, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            _attempts = new Dictionary<Tile, int>();
            _suspended = false;

            _projection = new WebMercator();
        }

        /// <summary>
        /// Handes the OnRemove-event from the cache.
        /// </summary>
        /// <param name="image"></param>
        private void OnRemove(Image2D image)
        { // dispose of the image after it is removed from the cache.
            try
            {
                _nativeImageCache.Release(image.NativeImage);
                image.NativeImage = null;
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Error, ex.Message);
            }
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
			try {
				if (_suspended) { // stop loading tiles.
					return;
				}

				var toLoad = new List<Tile> ();
				lock (_stack) 
				{ // make sure that access to the queue is synchronized.
					int queue = _stack.Count;
					while (_stack.Count > queue + _loading.Count - _maxThreads && _stack.Count > 0) { // there are queued items.
						toLoad.Add (_stack.Pop ());
					}
				}

				foreach(var tile in toLoad) 
				{
					LoadTile (tile);
				}
			} catch (Exception ex) { // don't worry about exceptions here.
				OsmSharp.Logging.Log.TraceEvent ("LayerTile", Logging.TraceEventType.Error, ex.Message);
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
            try
            {
                if (_suspended)
                { // stop loading tiles.
                    return;
                }

                // a tile was found to load.
                var tile = state as Tile;

                // only load tiles from the same zoom-level.
                if (tile.Zoom != _currentZoom)
                { // zoom is different, don't bother!
                    return;
                }

                Image2D image2D;
                if (_cache == null)
                {
                    return;
                }
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
                string url = this.FormatURL(tile);

                var request = HttpWebRequest.Create(url);
                request.Accept = "text/html, image/png, image/jpeg, image/gif, */*";
                if(request.IsUserAgentSupported)
                { // set user-agent if possible.
                    request.UserAgent = "OsmSharp/4";
                }

                OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Information, "Request tile@" + url);

                    request.BeginGetResponse(new AsyncCallback((iar) =>
                    {
						var response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                        try
                        {
							this.Response(response, tile);

							_loading.Remove(tile);
                        }
                        catch (WebException ex)
                        { // catch webexceptions.
							
							OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Information, 
									ex.Message);

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
						finally 
						{
							response.Close();
						}
                    }), request);
			}
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Error, ex.Message);
            }
        }

        /// <summary>
        /// Returns a formatted URL to get the tile from.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        private string FormatURL(Tile tile)
        {
            if(_tilesURL.Contains("{x}"))
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
        /// Response the specified myResp and tile.
        /// </summary>
        /// <param name="myResp">My resp.</param>
        /// <param name="tile">Tile.</param>
        private void Response(HttpWebResponse myResp, Tile tile)
        {
            if (_cache == null) { return; }
            try
            {
                var stream = myResp.GetResponseStream();
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

					OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Information, 
						string.Format("RaiseLayerChanged (Before): {0}", tile.ToString()));
                    if (!_suspended)
					{ // only raise the event when not suspended but do no throw away a tile, that would be a waste.
						this.RaiseLayerChanged();
						OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Information, 
							string.Format("Layer not suspended (After): {0}", tile.ToString()));
					} else {
						OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Information, 
							string.Format("Layer suspended (After): {0}", tile.ToString()));
					}
					OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Information, 
						string.Format("RaiseLayerChanged (After): {0}", tile.ToString()));

					OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Information, 
						string.Format("Tile loaded: {0}", tile.ToString()));
                }
                else
                {
                    OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Error, "No response stream!");
                }
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Error, ex.Message);
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
        /// <param name="view"></param>
        /// <returns></returns>
        protected internal override IEnumerable<Primitive2D> Get(float zoomFactor, View2D view)
        {
            var primitives = new List<Primitive2D>();
            if(_suspended)
            { // just return an empty primitives list if suspended.
                return primitives;
            }
            try
            {
                // calculate the current zoom level.
                var zoomLevel = (int)System.Math.Round(_projection.ToZoomLevel(zoomFactor), 0);

                if (zoomLevel >= _minZoomLevel && zoomLevel <= _maxZoomLevel)
                {
                    // build the bounding box.
                    var viewBox = view.OuterBox;
                    var box = new GeoCoordinateBox(_projection.ToGeoCoordinates(viewBox.Min[0], viewBox.Min[1]),
                                  _projection.ToGeoCoordinates(viewBox.Max[0], viewBox.Max[1]));

                    var tileRange = TileRange.CreateAroundBoundingBox(box, zoomLevel);
                    var tileRangeIndex = new TileRangeIndex(tileRange);

                    var primitivePerTile = new Dictionary<Tile, Primitive2D>();
                    lock (_cache)
					{
                        Image2D temp;
                        foreach (var tile in _cache)
                        {
                            if (tile.Value.IsVisibleIn(view))
                            {
                                tileRangeIndex.Add(tile.Key);
                                primitivePerTile.Add(tile.Key, tile.Value);

                                var minZoom = _projection.ToZoomFactor(tile.Key.Zoom - _zoomMinOffset);
                                var maxZoom = _projection.ToZoomFactor(tile.Key.Zoom + (1 - _zoomMinOffset));
                                if (zoomFactor < maxZoom && zoomFactor > minZoom)
                                { // just hit the cache for tiles of this zoom level.
                                    _cache.TryGet(tile.Key, out temp);
                                }
                            }
                        }

                        // set the ascending flag.
                        if (_previousZoomFactor != zoomFactor)
                        { // only change flag when difference.
                            _ascending = (_previousZoomFactor < zoomFactor);
                            _previousZoomFactor = zoomFactor;
                        }

                        // get candidate tiles for every tile.
                        var selectedTiles = new List<Tile>();
                        foreach (var tile in tileRange)
                        {
                            var best = tileRangeIndex.ChooseBest(tile, _ascending);
                            foreach (var bestTile in best)
                            {
                                if (!selectedTiles.Contains(bestTile))
                                { // make sure no doubles are added!
                                    selectedTiles.Add(bestTile);
                                }
                            }
                        }

                        // sort according to the tiles index.
                        selectedTiles.Sort(delegate(Tile x, Tile y)
                        {
                            return TileRangeIndex.TileWeight(tileRange.Zoom, x.Zoom, !_ascending).CompareTo(
                                TileRangeIndex.TileWeight(tileRange.Zoom, y.Zoom, !_ascending));
                        });
                        selectedTiles.Reverse();

                        // recursively remove tiles.
                        for (int idx = selectedTiles.Count; idx > 0; idx--)
                        {
                            if (selectedTiles[selectedTiles.Count - idx].IsOverlappedBy(
                                selectedTiles.GetRange(selectedTiles.Count - idx + 1, selectedTiles.Count - (selectedTiles.Count - idx + 1))))
                            {
                                selectedTiles.RemoveAt(selectedTiles.Count - idx);
                            }
                        }

                        // TODO: trim this collection so that only tiles remain close to the zoom level not overlapping.
                        Image2D primitive;
                        foreach (Tile tile in selectedTiles)
                        {
                            if (_cache.TryPeek(tile, out primitive))
                            { // add to the primitives list.
                                primitives.Add(primitive);
                            }
                        }
					}
				}
				OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Information, 
					string.Format("LayerTile returned {0} primitives.", primitives.Count));
                return primitives;
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Error, ex.Message);
            }
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
        /// <param name="extraView"></param>
        protected internal override void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view, View2D extraView)
        {
            if (_suspended)
            { // do not accept trigger changes if suspended.
                return;
            }

            if(!this.IsVisible)
            { // if the map is not visible also do not accept changes.
                return;
            }

            try
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

                                        OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Information, 
											"Queued tile: " + tile.ToString());
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
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Error, ex.Message);
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

            // Release the unmanaged resource in any case as they will not be 
            // released by GC
            if(this._nativeImageCache != null)
            { // dispose of the native image.
                    this._nativeImageCache.Flush();
                    this._nativeImageCache = null;
            }
        }     

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~LayerTile()
        {
            // The object went out of scope and finalized is called
            // Lets call dispose in to release unmanaged resources 
            // the managed resources will anyways be released when GC 
            // runs the next time.
            Dispose(false);
        }

        #endregion

        /// <summary>
        /// Pauses all activity in this layer.
        /// </summary>
        public override void Pause()
        {
            // set suspended flag.
            _suspended = true;

            lock(_stack)
            { // empty stack to tiles to-be-loaded but keep cache until layer is closed!
                _stack.Clear();
            }
        }

        /// <summary>
        /// Returns true if this flag is paused.
        /// </summary>
        public override bool IsPaused
        {
            get
            {
                return _suspended;
            }
        }

        /// <summary>
        /// Resumes the activity in this layer.
        /// </summary>
        public override void Resume()
        { // here only the suspended flag needs to be reset, just wait for the next request.
            _suspended = false;
        }

        /// <summary>
        /// Closes this layer.
        /// </summary>
        public override void Close()
        {
            base.Close();

            try
            {
                if (_timer != null)
                {
                    _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    _timer.Dispose();
                    _timer = null;
                }

                lock (_cache)
                {
                    _cache.OnRemove = null;
                    _cache.Clear();
                }
                _cache = null;

                lock (_stack)
                { // make sure the tile range is not in use.
                    _stack.Clear();
                }
                _stack = null;

                // flushes all images from the cache.
                _nativeImageCache.Flush();
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerTile", Logging.TraceEventType.Error, ex.Message);
            }
        }
    }
}

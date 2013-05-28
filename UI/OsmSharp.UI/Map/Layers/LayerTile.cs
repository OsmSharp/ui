using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;
using OsmSharp.Osm.Tiles;
using OsmSharp.UI.Renderer;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A tile layer.
    /// </summary>
    public class LayerTile : ILayer
    {
        /// <summary>
        /// Holds the tile url.
        /// </summary>
        private readonly string _tilesURL;

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        public LayerTile(string tilesURL)
        {
            _tilesURL = tilesURL;

            this.Scene = new Scene2D();
            _lastAccessed = new Dictionary<Tile, DateTime>();
            _primitivePerTile = new Dictionary<Tile, uint>();
            _tilesStack = new Stack<Tile>();
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
        /// Gets the current scene.
        /// </summary>
        public Scene2D Scene { get; private set; }

        /// <summary>
        /// Holds the last accessed times.
        /// </summary>
        private readonly Dictionary<Tile, DateTime> _lastAccessed;

        /// <summary>
        /// Holds the primitive ids for the given tiles.
        /// </summary>
        private readonly Dictionary<Tile, uint> _primitivePerTile;

        /// <summary>
        /// Holds the current tiles stack.
        /// </summary>
        private readonly Stack<Tile> _tilesStack;

        /// <summary>
        /// Holds the previous zoom level.
        /// </summary>
        private int _previousZoomLevel = 0;

        /// <summary>
        /// Holds the tiles loading layer.
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// Loads the tiles.
        /// </summary>
        private void TilesLoading(object projectionParam)
        {
            IProjection projection = null;
            if (projectionParam is IProjection)
            { // the argument has to a projection.
                projection = projectionParam as IProjection;
            }
            if (projection == null)
            {
                throw new ArgumentException("Argument not of type IProjection.");
            }

            while (true)
            {
                for (int idx = 0; idx < 10; idx++)
                {
                    Tile tile = null;
                    lock (_tilesStack)
                    {
                        // load the stacked tiles.
                        if (_tilesStack.Count > 0)
                        {
                            // the tiles stack contains at least some tiles.
                            tile = _tilesStack.Pop();
                        }
                    }

                    // load the tile 
                    if (tile != null)
                    {
                        // a tile was found to load.
                        string url = string.Format(_tilesURL,
                                                   tile.Zoom,
                                                   tile.X,
                                                   tile.Y);

                        // get file from tile server.
                        var request = (HttpWebRequest) HttpWebRequest.Create(
                            url);
                        request.Accept = "text/html, image/png, image/jpeg, image/gif, */*";
                        request.UserAgent = "OsmSharp/4.0";
                        request.Timeout = 1000;

                        WebResponse myResp = request.GetResponse();

                        Stream stream = myResp.GetResponseStream();
                        byte[] image = null;
                        if (stream != null)
                        {
                            // there is data: read it.
                            var memoryStream = new MemoryStream();
                            stream.CopyTo(memoryStream);

                            image = memoryStream.ToArray();
                        }

                        if (image != null)
                        {
                            // data was read create the scene object.
                            lock (this.Scene)
                            {
                                float minZoom = (float) projection.ToZoomFactor(tile.Zoom - 0.5f);
                                float maxZoom = (float) projection.ToZoomFactor(tile.Zoom + 0.5f);
                                float left = (float) projection.LongitudeToX(tile.TopLeft.Longitude);
                                float right = (float) projection.LongitudeToX(tile.BottomRight.Longitude);
                                float bottom = (float) projection.LatitudeToY(tile.BottomRight.Latitude);
                                float top = (float) projection.LatitudeToY(tile.TopLeft.Latitude);
                                this.Scene.AddImage(0, minZoom, maxZoom, left, bottom, right, top,
                                                    image);
                            }

                            if (this.LayerChanged != null)
                            {
                                this.LayerChanged(this);
                            }
                        }
                    }
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Notifies this layer the mapview has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        public void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view)
        {
            // calculate the current zoom level.
            var zoomLevel = (int)System.Math.Round(map.Projection.ToZoomLevel(zoomFactor), 0);

            // reset stack when zoom level changed.
            if (_previousZoomLevel != zoomLevel)
            {
                _previousZoomLevel = zoomLevel;
                _tilesStack.Clear();
            }

            // build the boundingbox.
            var box = new GeoCoordinateBox(
                    map.Projection.ToGeoCoordinates(view.Left, view.Top),
                    map.Projection.ToGeoCoordinates(view.Right, view.Bottom));

            // build the tile range.
            TileRange range = TileRange.CreateAroundBoundingBox(box, zoomLevel);
            DateTime now = DateTime.Now;
            foreach (var tile in range)
            {
                if (_primitivePerTile.ContainsKey(tile))
                { // the tile is here already.
                    _lastAccessed[tile] = now;
                }
                else
                { // queue the tile to be loaded.
                    lock (_tilesStack)
                    {
                        _tilesStack.Push(tile);
                    }
                }
            }

            if (_thread == null)
            { // the loading thread does not exist yet.
                _thread = new Thread(TilesLoading);
                _thread.Start(map.Projection);
            }
        }

        /// <summary>
        /// Event raised when this layer has changed.
        /// </summary>
        public event Map.LayerChanged LayerChanged;
    }
}
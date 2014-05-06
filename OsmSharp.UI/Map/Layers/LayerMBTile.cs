using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Tiles;
using OsmSharp.Data.SQLite;
using OsmSharp.Collections.Cache;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.Math.Geo.Projections;

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
        private readonly LRUCache<Tile, Image2D> _cache;

        /// <summary>
        /// Holds the map projection.
        /// </summary>
        private readonly IProjection _projection;

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        public LayerMBTile(SQLiteConnectionBase connection)
        {
            _connection = connection;

            _cache = new LRUCache<Tile, Image2D>(80);
            _projection = new WebMercator();
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
        /// Called when the mapview has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        public void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, 
            View2D view)
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
                TileRange range = TileRange.CreateAroundBoundingBox(box, zoomLevel);
                DateTime now = DateTime.Now;
                
                // request all missing tiles.
                lock (_connection)
                { // make sure the connection is accessed synchronously. 
                    // TODO: Investigate the SQLite multithreaded behaviour..
                    // TODO: this a very naive way of loading these tiles. Find a way to query SQLite more efficiently
                    // TODO: find a way to have some cached tiles.
                    foreach (var tile in range)
                    {
                        Tile invertTile = tile.InvertY();

                        SQLiteCommandBase command = _connection.CreateCommand("SELECT * FROM tiles WHERE zoom_level = :zoom_level AND tile_column = :tile_column AND tile_row = :tile_row;");
                        command.AddParameterWithValue("zoom_level", invertTile.Zoom);
                        command.AddParameterWithValue("tile_column", invertTile.X);
                        command.AddParameterWithValue("tile_row", invertTile.Y);
                        foreach(var mbTile in command.ExecuteQuery<MBTile>())
                        {
                            float minZoom = (float)map.Projection.ToZoomFactor(tile.Zoom - 0.5f);
                            float maxZoom = (float)map.Projection.ToZoomFactor(tile.Zoom + 0.5f);
                            float left = (float)map.Projection.LongitudeToX(tile.TopLeft.Longitude);
                            float right = (float)map.Projection.LongitudeToX(tile.BottomRight.Longitude);
                            float bottom = (float)map.Projection.LatitudeToY(tile.BottomRight.Latitude);
                            float top = (float)map.Projection.LatitudeToY(tile.TopLeft.Latitude);


                            var image2D = new Image2D(box.Min[0], box.Min[1], box.Max[1], box.Max[0], mbTile.Data);
                            image2D.Layer = (uint)(_maxZoomLevel - tile.Zoom);

                            lock (_cache)
                            { // add the result to the cache.
                                _cache.Add(tile, image2D);
                            }
                        }
                    }
                }
            }
        }

        private class MBTile
        {
            public int X { get; set; }

            public int Y { get; set; }

            public byte[] Data { get; set; }
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Tiles;
using System.Data.SQLite;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer that uses a MBTile SQLite database as a tile-source.
    /// </summary>
    public class LayerMBTile : ILayer
    {
        /// <summary>
        /// Holds the connection.
        /// </summary>
        private SQLiteConnection _connection;

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        public LayerMBTile(string databasePath)
        {
            this.Scene = new Scene2D();

            // create the connection.
            _connection = new SQLiteConnection(string.Format("Data Source=\"{0}\";Version=3;", databasePath));
        }

        /// <summary>
        /// Gets the minimum zoom.
        /// </summary>
        public float? MinZoom { get; set; }

        /// <summary>
        /// Gets the maximum zoom.
        /// </summary>
        public float? MaxZoom { get; set; }

        /// <summary>
        /// Gets the current scene.
        /// </summary>
        public Scene2D Scene { get; private set; }

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

            // build the boundingbox.
            var box = new GeoCoordinateBox(
                    map.Projection.ToGeoCoordinates(view.Left, view.Top),
                    map.Projection.ToGeoCoordinates(view.Right, view.Bottom));

            // build the tile range.
            TileRange range = TileRange.CreateAroundBoundingBox(box, zoomLevel);
            DateTime now = DateTime.Now;

            // build the new scene.
            Scene2D newScene = new Scene2D();
            if (_connection.State == System.Data.ConnectionState.Closed)
            {
                _connection.Open();
            }
            lock (_connection)
            { // make sure the connection is accessed synchronously. 
                // TODO: Investigate the SQLite multithreaded behaviour..
                // TODO: this a very naive way of loading these tiles. Find a way to query SQLite more efficiently
                // TODO: find a way to have some cached tiles.
                foreach (var tile in range)
                {
                    Tile invertTile = tile.InvertY();

                    SQLiteCommand command = new SQLiteCommand("SELECT * FROM tiles WHERE zoom_level = :zoom_level AND tile_column = :tile_column AND tile_row = :tile_row;", 
                        _connection);
                    command.Parameters.AddWithValue("zoom_level", invertTile.Zoom);
                    command.Parameters.AddWithValue("tile_column", invertTile.X);
                    command.Parameters.AddWithValue("tile_row", invertTile.Y);
                    using (var tileReader = command.ExecuteReader())
                    {
                        while (tileReader.Read())
                        {
                            //Tile readTile = new Tile((int)tileReader["tile_column"],
                            //    (int)tileReader["tile_row"], (int)tileReader["zoom_level"]); 

                            float minZoom = (float)map.Projection.ToZoomFactor(tile.Zoom - 0.5f);
                            float maxZoom = (float)map.Projection.ToZoomFactor(tile.Zoom + 0.5f);
                            float left = (float)map.Projection.LongitudeToX(tile.TopLeft.Longitude);
                            float right = (float)map.Projection.LongitudeToX(tile.BottomRight.Longitude);
                            float bottom = (float)map.Projection.LatitudeToY(tile.BottomRight.Latitude);
                            float top = (float)map.Projection.LatitudeToY(tile.TopLeft.Latitude);

                            newScene.AddImage(0, minZoom, maxZoom, left, top, right, bottom,
                                                (byte[])tileReader["tile_data"]);
                        }
                    }
                }
                this.Scene = newScene;
            }
        }

        /// <summary>
        /// Raised when the contents of this layer have changed.
        /// </summary>
        public event Map.LayerChanged LayerChanged;
    }
}

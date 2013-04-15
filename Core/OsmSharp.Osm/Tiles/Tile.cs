using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Units.Angle;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Represents a tile.
    /// </summary>
    public class Tile
    {
        ///// <summary>
        ///// Flag indicating where the y-tiles start.
        ///// </summary>
        //private readonly bool _y_is_top;

        ///// <summary>
        ///// Flag indicating where the x-tiles start.
        ///// </summary>
        //private readonly bool _x_is_left;

        /// <summary>
        /// Creates a new tile.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        public Tile(int x, int y, int zoom)
        {
            this.X = x;
            this.Y = y;

            this.Zoom = zoom;

            //_y_is_top = true;
            //_x_is_left = true;
        }

        ///// <summary>
        ///// Creates a new tile.
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="zoom"></param>
        ///// <param name="y_is_top"></param>
        ///// <param name="x_is_left"></param>
        //private void Tile(int x, int y, int zoom, bool y_is_top, bool x_is_left)
        //{
        //    this.X = x;
        //    this.Y = y;

        //    this.Zoom = zoom;

        //    //_y_is_top = true;
        //    //_x_is_left = true;
        //}

        /// <summary>
        /// The X position of the tile.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// The Y position of the tile.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// The zoom level for this tile.
        /// </summary>
        public int Zoom { get; set; }

        /// <summary>
        /// Returns a hashcode for this tile position.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^
                   this.Y.GetHashCode(); // ^
                   //_x_is_left.GetHashCode() ^
                   //_y_is_top.GetHashCode();
        }

        /// <summary>
        /// Returns true if the given object represents the same tile.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Tile)
            {
                return (obj as Tile).X == this.X &&
                    (obj as Tile).Y == this.Y;
            }
            return false;
        }

        /// <summary>
        /// Returns a description for this tile.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}x-{1}y@{2}z", this.X, this.Y, this.Zoom);
        }

        /// <summary>
        /// Returns the top left corner.
        /// </summary>
        public GeoCoordinate TopLeft
        {
            get
            {
                double n = System.Math.PI - ((2.0 * System.Math.PI * (double)this.Y) / System.Math.Pow(2.0, (double)this.Zoom));

                double longitude = (double)(((double)this.X / System.Math.Pow(2.0, (double)this.Zoom) * 360.0) - 180.0);
                double latitude = (double)(180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(n)));

                return new GeoCoordinate(latitude, longitude);
            }
        }

        /// <summary>
        /// Returns the bottom right corner.
        /// </summary>
        public GeoCoordinate BottomRight
        {
            get
            {
                double n = System.Math.PI - ((2.0 * System.Math.PI * (this.Y + 1)) / System.Math.Pow(2.0, this.Zoom));

                double longitude = (double)(((this.X + 1) / System.Math.Pow(2.0, this.Zoom) * 360.0) - 180.0);
                double latitude = (double)(180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(n)));

                return new GeoCoordinate(latitude, longitude);
            }
        }

        /// <summary>
        /// Returns the bounding box for this tile.
        /// </summary>
        public GeoCoordinateBox Box
        {
            get
            {
                // calculate the tiles bounding box and set its properties.
                GeoCoordinate top_left = this.TopLeft;
                GeoCoordinate bottom_right = this.BottomRight;

                return new GeoCoordinateBox(top_left, bottom_right);
            }
        }

        #region Conversion Functions

        /// <summary>
        /// Returns the tile at the given location at the given zoom.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static Tile CreateAroundLocation(GeoCoordinate location, int zoom)
        {
            int n = (int)System.Math.Floor(System.Math.Pow(2, zoom));

            Radian rad = new Degree(location.Latitude);

            int x = (int)(((location.Longitude + 180.0f) / 360.0f) * (double)n);
            int y = (int)(
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);

            return new Tile(x, y, zoom);
        }

        /// <summary>
        /// Inverts the X-coordinate.
        /// </summary>
        /// <returns></returns>
        public Tile InvertX()
        {
            int n = (int)System.Math.Floor(System.Math.Pow(2, this.Zoom));

            return new Tile(n - this.X - 1, this.Y, this.Zoom);
        }

        /// <summary>
        /// Inverts the Y-coordinate.
        /// </summary>
        /// <returns></returns>
        public Tile InvertY()
        {
            int n = (int)System.Math.Floor(System.Math.Pow(2, this.Zoom));

            return new Tile(this.X, n - this.Y - 1, this.Zoom);
        }

        #endregion
    }
}

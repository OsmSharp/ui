using OsmSharp.Math.Geo;
using OsmSharp.Math.Units.Angle;

namespace OsmSharp.Osm.Tiles
{
    /// <summary>
    /// Represents a tile.
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Creates a new tile from a given id.
        /// </summary>
        /// <param name="id"></param>
        public Tile(ulong id)
        {
            Tile tile = Tile.CalculateTile(id);
            this.X = tile.X;
            this.Y = tile.Y;
            this.Zoom = tile.Zoom;
        }

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
        }

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
                   this.Y.GetHashCode();
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
                GeoCoordinate topLeft = this.TopLeft;
                GeoCoordinate bottomRight = this.BottomRight;

                return new GeoCoordinateBox(topLeft, bottomRight);
            }
        }

        /// <summary>
        /// Returns the 4 subtiles.
        /// </summary>
        /// <returns></returns>
        public TileRange SubTiles
        {
            get
            {
                return new TileRange(2*this.X, 
                    2*this.Y, 
                    2*this.X + 1, 
                    2*this.Y + 1, 
                    this.Zoom + 1);
            }
        }

        /// <summary>
        /// Calculates the tile id of the tile at position (0, 0) for the given zoom.
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private static ulong CalculateTileId(int zoom)
        {
            if (zoom == 0)
            { // zoom level 0: {0}.
                return 0;
            }
            //else if (zoom == 1)
            //{ // zoom level 1: {1, 2, 3, 4}.
            //    return 1;
            //}
            //else if (zoom == 2)
            //{ // zoom level 2: {5, 6, 7, 8, 9, 10, 11, 12}.
            //    return 5;
            //}

            ulong size = (ulong) System.Math.Pow(2, 2*(zoom - 1));
            return Tile.CalculateTileId(zoom - 1) + size;
        }

        /// <summary>
        /// Calculates the tile id of the tile at position (x, y) for the given zoom.
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static ulong CalculateTileId(int zoom, int x, int y)
        {
            ulong id = Tile.CalculateTileId(zoom);
            long width = (long)System.Math.Pow(2, zoom);
            return id + (ulong)x + (ulong)(y*width);
        }

        /// <summary>
        /// Calculate the tile given the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static Tile CalculateTile(ulong id)
        {
            // find out the zoom level first.
            int zoom = 0;
            if (id > 0)
            { // only if the id is at least at zoom level 1.
                while (id >= Tile.CalculateTileId(zoom))
                {
                    // move to the next zoom level and keep searching.
                    zoom++;
                }
                zoom--;
            }

            // calculate the x-y.
            ulong local = id - Tile.CalculateTileId(zoom);
            ulong width = (ulong)System.Math.Pow(2, zoom);
            int x = (int)(local % width);
            int y = (int)(local / width);

            return new Tile(x, y, zoom);
        }

        /// <summary>
        /// Returns the id of this tile.
        /// </summary>
        public ulong Id
        {
            get { return Tile.CalculateTileId(this.Zoom, this.X, this.Y); }
        }

        #region Conversion Functions

        /// <summary>
        /// Returns the tile at the given location at the given zoom.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static Tile CreateAroundLocation(double latitude, double longitude, int zoom)
        {
            int n = (int)System.Math.Floor(System.Math.Pow(2, zoom));

            Radian rad = new Degree(latitude);

            int x = (int)(((longitude + 180.0f) / 360.0f) * (double)n);
            int y = (int)(
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);

            return new Tile(x, y, zoom);
        }

        /// <summary>
        /// Returns the tile at the given location at the given zoom.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static Tile CreateAroundLocation(GeoCoordinate location, int zoom)
        {
            return Tile.CreateAroundLocation(location.Latitude, location.Longitude, zoom);
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

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

using System.Collections.Generic;
using OsmSharp.Math.Geo;
using OsmSharp.Units.Angle;

namespace OsmSharp.Osm.Tiles
{
    /// <summary>
    /// Represents a range of tiles.
    /// </summary>
    public class TileRange : IEnumerable<Tile>
    {
        /// <summary>
        /// Creates a new tile range.
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="yMin"></param>
        /// <param name="xMax"></param>
        /// <param name="yMax"></param>
        /// <param name="zoom"></param>
        public TileRange(int xMin, int yMin, int xMax, int yMax, int zoom)
        {
            this.XMin = xMin;
            this.XMax = xMax;
            this.YMin = yMin;
            this.YMax = yMax;

            this.Zoom = zoom;
        }

        /// <summary>
        /// The minimum X of this range.
        /// </summary>
        public int XMin { get; private set; }

        /// <summary>
        /// The minimum Y of this range.
        /// </summary>
        public int YMin { get; private set; }

        /// <summary>
        /// The maximum X of this range.
        /// </summary>
        public int XMax { get; private set; }

        /// <summary>
        /// The maximum Y of this range.
        /// </summary>
        public int YMax { get; private set; }

        /// <summary>
        /// The zoom of this range.
        /// </summary>
        public int Zoom { get; private set; }

        #region Functions

        /// <summary>
        /// Returns true if the given tile lies at the border of this range.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public bool IsBorderAt(int x, int y, int zoom)
        {
            return ((x == this.XMin) || (x == this.XMax)
                || (y == this.YMin) || (y == this.YMin)) &&
                this.Zoom == zoom;
        }

        /// <summary>
        /// Returns true if the given tile lies at the border of this range.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool IsBorderAt(Tile tile)
        {
            return IsBorderAt(tile.X, tile.Y, tile.Zoom);
        }

        #endregion

        #region Conversion Functions

        /// <summary>
        /// Returns a tile range that encompasses the given bounding box at a given zoom level.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static TileRange CreateAroundBoundingBox(GeoCoordinateBox box, int zoom)
        {
            int n = (int)System.Math.Floor(System.Math.Pow(2, zoom));

            Radian rad = new Degree(box.MaxLat);

            int x_tile_min = (int)(((box.MinLon + 180.0f) / 360.0f) * (double)n);
            int y_tile_min = (int)(
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);

            rad = new Degree(box.MinLat);
            int x_tile_max = (int)(((box.MaxLon + 180.0f) / 360.0f) * (double)n);
            int y_tile_max = (int)(
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);

            return new TileRange(x_tile_min, y_tile_min, x_tile_max, y_tile_max, zoom);
        }

        #endregion

        /// <summary>
        /// Returns en enumerator of tiles.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Tile> GetEnumerator()
        {
            return new TileRangeEnumerator(this);
        }


        /// <summary>
        /// Returns en enumerator of tiles.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class TileRangeEnumerator : IEnumerator<Tile>
        {
            private TileRange _range;

            private Tile _current;

            public TileRangeEnumerator(TileRange range)
            {
                _range = range;
            }

            public Tile Current
            {
                get 
                {
                    return _current;
                }
            }

            public void Dispose()
            {
                _range = null;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (_current == null)
                {
                    _current = new Tile(_range.XMin, _range.YMin, _range.Zoom);
                    return true;
                }

                int x = _current.X;
                int y = _current.Y;

                if (x == _range.XMax)
                {
                    if (y == _range.YMax)
                    {
                        return false;
                    }
                    y++;
                    x = _range.XMin;
                }
                else
                {
                    x++;
                }
                _current = new Tile(x, y, _current.Zoom);
                return true;
            }

            public void Reset()
            {
                _current = null;
            }
        }
    }
}

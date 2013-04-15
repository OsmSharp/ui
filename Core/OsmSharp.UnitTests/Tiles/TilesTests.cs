// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OsmSharp.Osm;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Osm.UnitTests.Tiles
{
    /// <summary>
    /// Does some tests on the tile calculations.
    /// </summary>
    [TestFixture]
    public class TilesTests
    {
        /// <summary>
        /// Tests creating a tile.
        /// </summary>
        [Test]
        public void TestTileCreation()
        {
            // 51.27056&lon=4.78849
            // http://tile.deltamedia.local/tile/16/33639/21862.png
            Tile tile = new Tile(33639, 21862, 16);
            Tile tile2 = Tile.CreateAroundLocation(tile.Box.Center, 16);

            Assert.AreEqual(tile.X, tile.X);
            Assert.AreEqual(tile.Y, tile.Y);
            Assert.AreEqual(tile.Zoom, tile.Zoom);
        }

        /// <summary>
        /// Tests a tile box.
        /// </summary>
        [Test]
        public void TestTileBox()
        {
            Tile tile = new Tile(33639, 21862, 16);

            for (double longitude = tile.Box.MinLon; longitude < tile.Box.MaxLon; 
                longitude = longitude + tile.Box.DeltaLon / 100)
            {
                for (double latitude = tile.Box.MinLat; latitude < tile.Box.MaxLat;
                    latitude = latitude + tile.Box.DeltaLon / 100)
                {
                    Tile tile2 = Tile.CreateAroundLocation(new GeoCoordinate(
                        latitude, longitude), tile.Zoom);

                    Assert.AreEqual(tile.X, tile.X);
                    Assert.AreEqual(tile.Y, tile.Y);
                    Assert.AreEqual(tile.Zoom, tile.Zoom);
                }
            }
        }

        /// <summary>
        /// Tests a tile range enumeration.
        /// </summary>
        [Test]
        public void TestTileRangeEnumerator()
        {
            TileRange range = new TileRange(0, 0, 1, 1, 16);

            HashSet<Tile> tiles = new HashSet<Tile>(range);

            Assert.IsTrue(tiles.Contains(new Tile(0, 0, 16)));
            Assert.IsTrue(tiles.Contains(new Tile(0, 1, 16)));
            Assert.IsTrue(tiles.Contains(new Tile(1, 1, 16)));
            Assert.IsTrue(tiles.Contains(new Tile(0, 1, 16)));
        }
    }
}

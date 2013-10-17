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
using NUnit.Framework;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Tiles;

namespace OsmSharp.UnitTests.Osm.Tiles
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

            Assert.AreEqual(tile.X, tile2.X);
            Assert.AreEqual(tile.Y, tile2.Y);
            Assert.AreEqual(tile.Zoom, tile2.Zoom);
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

                    Assert.AreEqual(tile.X, tile2.X);
                    Assert.AreEqual(tile.Y, tile2.Y);
                    Assert.AreEqual(tile.Zoom, tile2.Zoom);
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

        /// <summary>
        /// Tests the tile id generation.
        /// </summary>
        [Test]
        public void TestTileId()
        {
            var tile0 = new Tile(0, 0, 0);
            Assert.AreEqual(0, tile0.Id);

            var tile1_0_0 = new Tile(0, 0, 1);
            Assert.AreEqual(1, tile1_0_0.Id);

            var tile2_0_0 = new Tile(0, 0, 2);
            Assert.AreEqual(5, tile2_0_0.Id);

            var tile3_0_0 = new Tile(0, 0, 3);
            Assert.AreEqual(5 + 16, tile3_0_0.Id);

            var tile4_0_0 = new Tile(0, 0, 4);
            Assert.AreEqual(5 + 16 + 64, tile4_0_0.Id);

            var tile2_1_1 = new Tile(1, 1, 2);
            Assert.AreEqual(5 + 1 + 4, tile2_1_1.Id);

            var tile2_1_1_fromId = new Tile(5 + 1 + 4);
            Assert.AreEqual(tile2_1_1.Zoom, tile2_1_1_fromId.Zoom);
            Assert.AreEqual(tile2_1_1.X, tile2_1_1_fromId.X);
            Assert.AreEqual(tile2_1_1.Y, tile2_1_1_fromId.Y);

            for (ulong id = 0; id < 1000; id++)
            {
                Tile tile = new Tile(id);
                Assert.AreEqual(id, tile.Id);
            }
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Osm.Core;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Osm.UnitTests.Tiles
{
    /// <summary>
    /// Does some tests on the tile calculations.
    /// </summary>
    [TestClass]
    public class TilesTests
    {
        /// <summary>
        /// Tests creating a tile.
        /// </summary>
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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

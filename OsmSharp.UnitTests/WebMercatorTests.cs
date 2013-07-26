using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Osm;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Tiles;

namespace OsmSharp.UnitTests.Tools
{
    /// <summary>
    /// Tests for the webmercator projection.
    /// </summary>
    [TestFixture]
    public class WebMercatorTests
    {
        /// <summary>
        /// Tests simple web mercator projection projecting a tile nicely onto 256x256 squares.
        /// </summary>
        [Test]
        public void TestSimpleWebMercator()
        {
            // TODO: stabalize the webmercator projection numerically for lower zoom levels (0-9).
            var mercator = new WebMercator();
            for (int zoomLevel = 10; zoomLevel <= 25; zoomLevel++)
            {
                var tile = Tile.CreateAroundLocation(new GeoCoordinate(0, 0), zoomLevel);

                double[] topleft = mercator.ToPixel(tile.Box.TopLeft);
                double[] bottomright = mercator.ToPixel(tile.Box.BottomRight);

                double scaleFactor = mercator.ToZoomFactor(zoomLevel);

                Assert.AreEqual(-256, (topleft[0] - bottomright[0]) * scaleFactor, 0.01);
                Assert.AreEqual(-256, (topleft[1] - bottomright[1]) * scaleFactor, 0.01);
            }

            var coordinate = new GeoCoordinate(51.26337, 4.78739);
            double[] projected = mercator.ToPixel(coordinate);
            var reProjected = mercator.ToGeoCoordinates(projected[0], projected[1]);

            Assert.AreEqual(coordinate.Longitude, reProjected.Longitude, 0.0001);
            Assert.AreEqual(coordinate.Latitude, reProjected.Latitude, 0.0001);
        }

        /// <summary>
        /// Tests simple web mercator projection zoom level zoom factor conversion.
        /// </summary>
        [Test]
        public void TestSimpleWebMercatorZoomLevel()
        {
            var mercator = new WebMercator();

            for (int orignalLevel = 0; orignalLevel < 20; orignalLevel++)
            {
                double zoomFactor = mercator.ToZoomFactor(orignalLevel);
                double zoomLevel = mercator.ToZoomLevel(zoomFactor);

                Assert.AreEqual(orignalLevel, zoomLevel, 0.001);
            }
        }
    }
}
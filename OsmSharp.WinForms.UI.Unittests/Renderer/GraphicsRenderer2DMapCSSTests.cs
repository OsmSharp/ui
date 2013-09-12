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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Osm.Data;
using System.Drawing;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;
using OsmSharp.UI;
using OsmSharp.WinForms.UI.Renderer;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Renderer;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map;

namespace OsmSharp.WinForms.UI.Unittests.Renderer
{
    /// <summary>
    /// Holds test for the entire rendering pipeline: OSM -> MapCSS -> Scene -> Graphics -> Bitmap.
    /// </summary>
    [TestFixture]
    public class GraphicsRenderer2DMapCSSTests
    {
        /// <summary>
        /// Tests the canvas color.
        /// </summary>
        [Test]
        public void TestMapCSSCanvasRendering()
        {
            // create the data source.
            MemoryDataSource source = new MemoryDataSource();

            // define the MapCSS file.
            string css = "canvas { " +
                "fill-color: green; " +
                "} ";

            // do the rendering.
            Bitmap rendering = this.Render(source, css);

            // check result.
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    Assert.AreEqual(SimpleColor.FromKnownColor(OsmSharp.UI.KnownColor.Green).Value,
                        rendering.GetPixel(x, y).ToArgb());
                }   
            }
        }

        /// <summary>
        /// Tests a simple way rendering.
        /// </summary>
        [Test]
        public void TestMapCSSWayRendering1()
        {
            // create the data source.
            MemoryDataSource source = new MemoryDataSource(
                Node.Create(1, 1, 1),
                Node.Create(2, -1, -1),
                Way.Create(1, new SimpleTagsCollection(Tag.Create("highway", "residential")), 1, 2));

            // create CSS.
            string css = "canvas { " +
                "fill-color: white; " +
                "}  "+ 
                "way { " +
                "   width: 2; " +
                "   color: black; " +
                "} ";

            // do the rendering.
            Bitmap rendering = this.Render(source, css);

            // check result.
            Assert.AreEqual(SimpleColor.FromKnownColor(OsmSharp.UI.KnownColor.White).Value,
                rendering.GetPixel(0, 99).ToArgb());
            Assert.AreEqual(SimpleColor.FromKnownColor(OsmSharp.UI.KnownColor.White).Value,
                rendering.GetPixel(99, 0).ToArgb());
            for (int x = 0; x < 100; x++)
            {
                Assert.AreEqual(SimpleColor.FromKnownColor(OsmSharp.UI.KnownColor.Black).Value,
                    rendering.GetPixel(x, x).ToArgb());
            }
        }

        #region Helpers

        /// <summary>
        /// Renders the given data onto a 100x100 image using bounds around null-island (1,1,-1,-1) and the MapCSS definition.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="mapCSS"></param>
        /// <returns></returns>
        private Bitmap Render(IDataSourceReadOnly dataSource, string mapCSS)
        {
            // create projection.
            WebMercator projection = new WebMercator();
            double[] topLeft = projection.ToPixel(new Math.Geo.GeoCoordinate(1, 1));
            double[] bottomRight = projection.ToPixel(new Math.Geo.GeoCoordinate(-1, -1));

            // create view (this comes down to (1,1,-1,-1) for a size of 100x100).
            View2D view = View2D.CreateFromBounds(bottomRight[1], topLeft[0], topLeft[1], bottomRight[0]);
            //View2D view = View2D.CreateFrom(0, 0, 100, 100, 1.0 / 200.0, false, true);

            // create graphics
            Bitmap rendering = new Bitmap(100, 100);
            Graphics renderingGraphics = Graphics.FromImage(rendering);
            renderingGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            renderingGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            renderingGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            renderingGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // create renderer.
            GraphicsRenderer2D graphicsRenderer = new GraphicsRenderer2D();
            MapRenderer<Graphics> renderer = new MapRenderer<Graphics>(graphicsRenderer);

            // create map.
            OsmSharp.UI.Map.Map map = new OsmSharp.UI.Map.Map();
            map.AddLayer(new OsmLayer(dataSource, new MapCSSInterpreter(mapCSS), projection));
            
            // notify the map that there was a view change!
            map.ViewChanged((float)view.CalculateZoom(100, 100), new Math.Geo.GeoCoordinate(0, 0), view);

            // ... and finally do the actual rendering.
            renderer.Render(Graphics.FromImage(rendering), map, view);

            //rendering.Save(@"c:\temp\rendering.bmp");

            return rendering;
        }

        #endregion
    }
}

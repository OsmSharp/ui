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

using System.Drawing;
using System.IO;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Map;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.WinForms.UI.Renderer;
using System;
using System.Drawing.Imaging;
using OsmSharp.UI.Map.Layers;
using System.Drawing.Drawing2D;

namespace OsmSharp.Test.Performance.UI.Rendering
{
    /// <summary>
    /// Contains serialization tests.
    /// </summary>
    public static class RenderingSerializedSceneTests
    {
        /// <summary>
        /// Holds the target width.
        /// </summary>
        public static int TargetWidth = 256 * 4;

        /// <summary>
        /// Holds the target height.
        /// </summary>
        public static int TargetHeight = 256 * 4;

        /// <summary>
        /// Holds the write results flags.
        /// </summary>
        public static bool WriteResults = false;

        /// <summary>
        /// Runs all rendering tests.
        /// </summary>
        public static void Test()
        {
            FileInfo testFile = new FileInfo(string.Format(@".\TestFiles\map\{0}", "kempen-big.osm.pbf.scene.layered"));
            Stream stream = testFile.OpenRead();

            // do some of the testing.
            RenderingSerializedSceneTests.TestRenderScene(stream, new GeoCoordinateBox(
                new GeoCoordinate(51.20190, 4.66540),
                new GeoCoordinate(51.30720, 4.89820)), 10);
        }

        /// <summary>
        /// Tests rendering the given serialized scene.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="box"></param>
        /// <param name="testCount"></param>
        public static void TestRenderScene(Stream stream, GeoCoordinateBox box, int testCount)
        {
            WebMercator projection = new WebMercator();

            // build a map.
            Map map = new Map();
            IScene2DPrimitivesSource sceneSource = Scene2DLayered.Deserialize(stream, true);
            LayerScene layerScene = map.AddLayerScene(sceneSource);

            // build the target to render to.
            Bitmap imageTarget = new Bitmap(TargetWidth, TargetHeight);
            Graphics target = Graphics.FromImage(imageTarget);
            target.SmoothingMode = SmoothingMode.HighQuality;
            target.PixelOffsetMode = PixelOffsetMode.HighQuality;
            target.CompositingQuality = CompositingQuality.HighQuality;
            target.InterpolationMode = InterpolationMode.HighQualityBicubic;
            MapRenderer<Graphics> mapRenderer = new MapRenderer<Graphics>(
                new GraphicsRenderer2D());

            // render the map.
            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer("Scene2DLayeredRendering");
            performanceInfo.Start();
            performanceInfo.Report("Rendering {0} random images...", testCount);

            while (testCount > 0)
            {
                // randomize view.
                int zoom = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(10) + 10;
                GeoCoordinate center = box.GenerateRandomIn();
                View2D view = mapRenderer.Create(TargetWidth, TargetHeight, map,
                    (float)projection.ToZoomFactor(zoom), center, false, true);

                layerScene.ViewChanged(map, (float)projection.ToZoomFactor(zoom), center, view);

                mapRenderer.Render(target, map, view);

                if (WriteResults)
                {
                    imageTarget.Save(Guid.NewGuid().ToString() + ".png", ImageFormat.Png);
                }

                testCount--;
            }

            performanceInfo.Stop();

        }
    }
}

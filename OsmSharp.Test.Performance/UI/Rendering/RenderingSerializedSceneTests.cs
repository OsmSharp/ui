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

using System.IO;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Test.Performance.UI.Rendering
{
    /// <summary>
    /// Contains serialization tests.
    /// </summary>
    public static class RenderingSerializedSceneTests<TTarget>
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
        /// Delegate signature to create a target to render to.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public delegate TTarget CreateTarget(int width, int height);

        /// <summary>
        /// Delegate signature to create a renderer.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public delegate Renderer2D<TTarget> CreateRenderer();

        /// <summary>
        /// Runs all rendering tests.
        /// </summary>
        public static void Test(CreateTarget createTarget, CreateRenderer createRenderer)
        {
            FileInfo testFile = new FileInfo(string.Format(@".\TestFiles\map\{0}", "kempen-big.osm.pbf.scene.layered"));
            Stream stream = testFile.OpenRead();

            Test(stream, createTarget, createRenderer);
        }

        /// <summary>
        /// Runs all rendering tests.
        /// </summary>
        public static void Test(Stream stream, CreateTarget createTarget, CreateRenderer createRenderer)
        {

            // do some of the testing.
            RenderingSerializedSceneTests<TTarget>.TestRenderScene(createTarget, createRenderer, stream, new GeoCoordinateBox(
                new GeoCoordinate(51.20190, 4.66540),
                new GeoCoordinate(51.30720, 4.89820)), 5, 2, 16);
            // do some of the testing.
            RenderingSerializedSceneTests<TTarget>.TestRenderScene(createTarget, createRenderer, stream, new GeoCoordinateBox(
                new GeoCoordinate(51.20190, 4.66540),
                new GeoCoordinate(51.30720, 4.89820)), 5, 2, 14);
            // do some of the testing.
            RenderingSerializedSceneTests<TTarget>.TestRenderScene(createTarget, createRenderer, stream, new GeoCoordinateBox(
                new GeoCoordinate(51.20190, 4.66540),
                new GeoCoordinate(51.30720, 4.89820)), 5, 2, 12);
            //// do some of the testing.
            //RenderingSerializedSceneTests<TTarget>.TestRenderScene(createTarget, createRenderer, stream, new GeoCoordinateBox(
            //    new GeoCoordinate(51.20190, 4.66540),
            //    new GeoCoordinate(51.30720, 4.89820)), 5, 2, 10);
        }

        /// <summary>
        /// Tests rendering the given serialized scene.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="box"></param>
        /// <param name="testCount"></param>
        public static void TestRenderScene(CreateTarget createTarget, CreateRenderer createRenderer, 
            Stream stream, GeoCoordinateBox box, int testCount, int range, int minZoom)
        {
            WebMercator projection = new WebMercator();

            // build a map.
            Map map = new Map();
            TagsCollectionBase metaTags;
            map.AddLayer(new LayerScene(
                Scene2D.Deserialize(stream, true, out metaTags)));

            // build the target and renderer.
            TTarget target = createTarget.Invoke(TargetWidth, TargetHeight);
            MapRenderer<TTarget> mapRenderer = new MapRenderer<TTarget>(
                createRenderer.Invoke());

            // render the map.
            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer("Scene2DLayeredRendering");
            performanceInfo.Start();
            //performanceInfo.Report("Rendering {0} random images...", testCount);

            while (testCount > 0)
            {
                // randomize view.
                int zoom = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(range) + minZoom;
                GeoCoordinate center = box.GenerateRandomIn();
                View2D view = mapRenderer.Create(TargetWidth, TargetHeight, map,
                    (float)projection.ToZoomFactor(zoom), center, false, true);

                OsmSharp.Logging.Log.TraceEvent("Scene2DLayeredRendering", OsmSharp.Logging.TraceEventType.Information,
                                            string.Format("Rendering at z{0} l{1}.", 
                                                          zoom, center));
                map.ViewChanged((float)projection.ToZoomFactor(zoom), center, view);

                mapRenderer.Render(target, map, view, (float)projection.ToZoomFactor(zoom));

                testCount--;
            }

            performanceInfo.Stop();
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}

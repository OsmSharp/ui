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
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Map.Styles.Streams;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Math.Geo.Projections;
using System.IO;
using OsmSharp.Osm.PBF.Streams;

namespace OsmSharp.Test.Performance.UI.Styles.MapCSS
{
    /// <summary>
    /// Contains tests for the MapCSS interpreter.
    /// </summary>
    public static class MapCSSInterpreterTests
    {
        /// <summary>
        /// Executes the tests.
        /// </summary>
        public static void Test()
        {
            // tests map css interpreter.
            MapCSSInterpreterTests.TestInterpret("MapCSSInterpreter", @"mapcss\complete.mapcss", "kempen.osm.pbf");
        }

        /// <summary>
        /// Tests interpreting all data from a given pbf source.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mapCSS"></param>
        /// <param name="pbfSource"></param>
        public static void TestInterpret(string name, string mapCSS, string pbfSource)
        {
            FileInfo cssFile = new FileInfo(string.Format(@".\TestFiles\{0}", mapCSS));
            Stream cssStream = cssFile.OpenRead();
            MapCSSInterpreter interpreter = new MapCSSInterpreter(cssStream, new MapCSSDictionaryImageSource());

            MapCSSInterpreterTests.TestInterpret(name, interpreter, pbfSource);

            cssStream.Dispose();
        }

        /// <summary>
        /// Tests interpreting all data from a given pbf source.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="interpreter"></param>
        /// <param name="pbfSource"></param>
        public static void TestInterpret(string name, MapCSSInterpreter interpreter, string pbfSource)
        {
            Scene2D scene = new Scene2DSimple();
            StyleOsmStreamSceneTarget target = new StyleOsmStreamSceneTarget(
                interpreter, scene, new WebMercator());
            FileInfo testFile = new FileInfo(string.Format(@".\TestFiles\{0}", pbfSource));
            Stream stream = testFile.OpenRead();
            PBFOsmStreamSource source = new PBFOsmStreamSource(stream);
            target.RegisterSource(source);

            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer(string.Format("{0}.Add", name));
            performanceInfo.Start();
            performanceInfo.Report("Interpreting style with objects from {0}...", pbfSource.ToString());

            target.Pull();

            performanceInfo.Stop();

            Console.Write("", scene.BackColor);
            stream.Dispose();
        }
    }
}

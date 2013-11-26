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
using System.Drawing;
using System.Drawing.Drawing2D;
using OsmSharp.WinForms.UI.Renderer;

namespace OsmSharp.Test.Performance
{
    class Program
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // enable logging and use the console as output.
            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterConsoleListener();

            // set the seed manually.
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);
            
            // add the to-ignore list.
            OsmSharp.Logging.Log.Ignore("OsmSharp.Osm.Interpreter.SimpleGeometryInterpreter");
            OsmSharp.Logging.Log.Ignore("CHPreProcessor");
            OsmSharp.Logging.Log.Ignore("RTreeStreamIndex");
            OsmSharp.Logging.Log.Ignore("Scene2DLayeredSource");

            // test streams.
            Osm.PBF.PBFStreamSourceTest.Test();

            // test the tags collection.
            Tags.Collections.TagsTableCollectionIndexTests.Test();
            Tags.Collections.BlockedTagsCollectionIndexTests.Test();

            // tests the mapcss interpretation.
            UI.Styles.MapCSS.MapCSSInterpreterTests.Test();

            // test the routing pre-processor.
            Routing.CH.CHPreProcessorTest.Test();
            Routing.CH.CHEdgeGraphFileStreamTargetTests.Test();
            Routing.CH.CHSerializedRoutingTest.Test();
            
            // test some rendering implementations.
            UI.Rendering.RenderingSerializedSceneTests<System.Drawing.Graphics>.Test(
                (width, height) =>
                {
                    // build the target to render to.
                    Bitmap imageTarget = new Bitmap(UI.Rendering.RenderingSerializedSceneTests<System.Drawing.Graphics>.TargetWidth, 
                        UI.Rendering.RenderingSerializedSceneTests<System.Drawing.Graphics>.TargetHeight);
                    Graphics target = Graphics.FromImage(imageTarget);
                    target.SmoothingMode = SmoothingMode.HighQuality;
                    target.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    target.CompositingQuality = CompositingQuality.HighQuality;
                    target.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    return target;
                },
                () => {
                    return new GraphicsRenderer2D();
                });

            // wait for an exit.
            OsmSharp.Logging.Log.TraceEvent("Program", System.Diagnostics.TraceEventType.Information,
                "Testing finished.");
            Console.ReadLine();
        }
    }
}
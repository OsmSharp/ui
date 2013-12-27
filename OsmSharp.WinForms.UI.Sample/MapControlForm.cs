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
using System.IO;
using System.Windows.Forms;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Map.Styles.Streams;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.UI.Renderer.Scene.Simplification;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Osm.Data.Memory;

namespace OsmSharp.WinForms.UI.Sample
{
    /// <summary>
    /// A simple demo form demonstrating the rendering using MapCSS.
    /// </summary>
    public partial class MapControlForm : Form
    {
        /// <summary>
        /// Creates a new map control form.
        /// </summary>
        public MapControlForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raises the OnLoad event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // initialize mapcss interpreter.
            var mapCSSInterpreter = new MapCSSInterpreter(
                new FileInfo(@"complete.mapcss").OpenRead(), new MapCSSDictionaryImageSource());

            // initialize map.
            var map = new OsmSharp.UI.Map.Map();

            //Scene2D scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), new List<float>(new float[] {
            //    16, 14, 12, 10 }));
            //StyleOsmStreamSceneTarget target = new StyleOsmStreamSceneTarget(
            //    mapCSSInterpreter, scene, new WebMercator());
            //FileInfo testFile = new FileInfo(@"kempen.osm.pbf");
            //Stream stream = testFile.OpenRead();
            //OsmStreamSource source = new PBFOsmStreamSource(stream);
            //OsmStreamFilterProgress progress = new OsmStreamFilterProgress(source);
            //target.RegisterSource(progress);
            //target.Pull();

            //var merger = new Scene2DObjectMerger();
            //scene = merger.BuildMergedScene(scene);

            //map.AddLayer(new LayerScene(scene));
            //var dataSource = MemoryDataSource.CreateFromPBFStream(
            //    new FileInfo(@"kempen.osm.pbf").OpenRead());
            //map.AddLayer(new LayerOsm(dataSource, mapCSSInterpreter, map.Projection));
            ////map.AddLayer(new LayerTile(@"http://otile1.mqcdn.com/tiles/1.0.0/osm/{0}/{1}/{2}.png"));
            map.AddLayer(new LayerScene(
                Scene2D.Deserialize(new FileInfo(@"complete.map").OpenRead(),
                    true)));

            // set control properties.
            this.mapControl1.Map = map;
            this.mapControl1.MapCenter = new GeoCoordinate(51.26371, 4.7854); // wechel
            this.mapControl1.MapZoom = 16;
        }
    }
}
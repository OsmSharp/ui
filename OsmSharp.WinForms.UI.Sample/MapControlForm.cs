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
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data.Xml.Processor;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Graphs.Serialization;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Map.Layers;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.UI.Renderer;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.WinForms.UI.Map.Layers;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.CH.Serialization;
using OsmSharp.UI;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.UI.Renderer.Scene;

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

            this.mapControl1.MapMouseClick += new MapControl.MapMouseEventDelegate(mapControl1_MapMouseClick);
        }

        /// <summary>
        /// Raises the OnLoad event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // create the MapCSS image source.
            var imageSource = new MapCSSDictionaryImageSource();
            imageSource.Add("styles/default/parking.png",
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.WinForms.UI.Sample.images.parking.png"));
            imageSource.Add("styles/default/bus.png",
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.WinForms.UI.Sample.images.bus.png"));
            imageSource.Add("styles/default/postbox.png",
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.WinForms.UI.Sample.images.postbox.png"));

            // load mapcss style interpreter.
            var mapCSSInterpreter = new MapCSSInterpreter(
                new FileInfo(@"C:\Users\xivk\Dropbox\SharpSoftware\Projects\Westtour WOI\wvl.mapcss").OpenRead(),
                //Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.WinForms.UI.Sample.test.mapcss"),
                imageSource);

            // initialize the data source.
            //var dataSource = MemoryDataSource.CreateFromXmlStream(new FileInfo(@"c:\OSM\bin\wechel.osm").OpenRead());
            //var dataSource = MemoryDataSource.CreateFromPBFStream(new FileInfo(@"c:\OSM\bin\wvl.osm.pbf").OpenRead());
            var dataSource = MemoryDataSource.CreateFromPBFStream(new FileInfo(@"c:\OSM\bin\gent.osm.pbf").OpenRead());
            //var dataSource = MemoryDataSource.CreateFromPBFStream(new FileInfo(@"c:\OSM\bin\lebbeke.osm.pbf").OpenRead());
            //Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.WinForms.UI.Sample.test.osm"));

            // initialize map.
            var map = new OsmSharp.UI.Map.Map();
            map.AddLayer(new OsmLayer(dataSource, mapCSSInterpreter, map.Projection));
            //map.AddLayer(new LayerTile(@"http://otile1.mqcdn.com/tiles/1.0.0/osm/{0}/{1}/{2}.png"));
            //map.AddLayer(new LayerMBTile(@"C:\Users\xivk\Documents\Nostalgeo.mbtiles"));
            //map.AddLayer(
            //    new LayerScene(
            //        Scene2DLayered.Deserialize(new FileInfo(@"c:\OSM\bin\kempen.osm.pbf.scene.layered").OpenRead(), true)));
            //map.AddLayer(
            //    new LayerScene(
            //        Scene2DLayered.Deserialize(new FileInfo(@"c:\OSM\bin\wvl.osm.pbf.scene.layered").OpenRead(), true)));

            //this.InitializeRouting(map);

            //// create gpx layer.
            //var gpxLayer = new LayerGpx(map.Projection);
            //gpxLayer.AddGpx(
            //    Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.WinForms.UI.Sample.test.gpx"));
            //map.AddLayer(gpxLayer);

            // set control properties.
            this.mapControl1.Map = map;
            //this.mapControl1.Center = new GeoCoordinate(51.26371, 4.7854); //51.26371&lon=4.7854 // wechel.osm
            //this.mapControl1.Center = new GeoCoordinate(50.88672, 3.23899); // lendelede 
            //this.mapControl1.Center = new GeoCoordinate(50.9523195, 3.0997436);
            //this.mapControl1.Center = new GeoCoordinate(51.156803, 2.958887); // gistel
            this.mapControl1.Center = new GeoCoordinate(51.05608, 3.72139); // gent
            //this.mapControl1.Center = new GeoCoordinate(50.9969, 4.1201);
            this.mapControl1.ZoomLevel = 19;
        }

        #region Routing Tests

        private RouterPoint _point1;

        private RouterPoint _point2;

        private Router _router;

        private LayerOsmSharpRoute _routeLayer;

        private void InitializeRouting(OsmSharp.UI.Map.Map map)
        {
            //var osmInterpreter = new OsmRoutingInterpreter();
            //_router = Router.CreateLiveFrom(new XmlOsmStreamSource(new FileInfo(@"c:\OSM\bin\wechel.osm").OpenRead()),
            //    osmInterpreter);

            // creates a new interpreter.
            var interpreter = new OsmRoutingInterpreter();
            var routingSerializer = new OsmSharp.Routing.CH.Serialization.Sorted.CHEdgeDataDataSourceSerializer(true);
            var original = CHEdgeGraphOsmStreamWriter.Preprocess(new XmlOsmStreamSource(
                                                                   new FileInfo(@"c:\OSM\bin\wechel.osm").OpenRead()),
                                                               interpreter,
                                                               Vehicle.Car);
            //byte[] byteArray;
            //var stream = new MemoryStream();
            //using (stream)
            //{
            //    try
            //    {
            //        routingSerializer.Serialize(stream, original);
            //        byteArray = stream.ToArray();
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }
            //}
            //stream = new MemoryStream(byteArray);
            //Stream stream = new FileInfo(@"c:\OSM\bin\test.osm.pbf.routing.3").OpenRead();

            //IBasicRouterDataSource<CHEdgeData> deserializedVersion =
            //    routingSerializer.Deserialize(stream);
            var basicRouter =
                new CHRouter(original);
            _router = Router.CreateCHFrom(
                original, basicRouter, interpreter);

            _routeLayer = new LayerOsmSharpRoute(map.Projection);
            map.AddLayer(_routeLayer);
        }


        void mapControl1_MapMouseClick(MapControlEventArgs e)
        {
            if (_router != null)
            {
                if (_point1 == null)
                {
                    _point1 = _router.Resolve(Vehicle.Car, e.Position);
                }
                else if (_point2 == null)
                {
                    _point2 = _router.Resolve(Vehicle.Car, e.Position);
                }
                else
                {
                    _point1 = _point2;
                    _point2 = _router.Resolve(Vehicle.Car, e.Position);
                }
                if (_point1 != null && _point2 != null)
                {
                    Route route = _router.Calculate(Vehicle.Car,
                        _point1, _point2);
                    if (route != null)
                    {
                        _routeLayer.Clear();
                        _routeLayer.AddRoute(route);
                    }
                }
            }
        }

        #endregion
    }
}
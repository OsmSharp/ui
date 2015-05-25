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

using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.Optimization.TSP;
using OsmSharp.Routing.Optimization.TSP.Genetic;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Vehicles;
using OsmSharp.UI;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Map.Styles.Streams;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.UI.Renderer.Scene.Simplification;
using OsmSharp.WinForms.UI.Renderer.Images;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

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
                new FileInfo(@"dark_roads.mapcss").OpenRead(), new MapCSSDictionaryImageSource());

            // initialize map.
            var map = new OsmSharp.UI.Map.Map();

            //// initialize router.
            //_router = Router.CreateFrom(new OsmSharp.Osm.PBF.Streams.PBFOsmStreamSource(
            //    new FileInfo(@"kempen.osm.pbf").OpenRead()), new OsmRoutingInterpreter());

            //var scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), new List<float>(new float[] {
            //    16, 14, 12, 10 }));
            //var target = new StyleOsmStreamSceneTarget(
            //    mapCSSInterpreter, scene, new WebMercator());
            //var source = new PBFOsmStreamSource(
            //    new FileInfo(@"kempen.osm.pbf").OpenRead());
            //var progress = new OsmStreamFilterProgress();
            //progress.RegisterSource(source);
            //target.RegisterSource(progress);
            //target.Pull();

            ////var merger = new Scene2DObjectMerger();
            ////scene = merger.BuildMergedScene(scene);

            //map.AddLayer(new LayerScene(scene));
            //var dataSource = MemoryDataSource.CreateFromXmlStream(
            //    new FileInfo(@"D:\Dropbox\Dropbox\SharpSoftware\Projects\Eurostation ReLive\Server_Dropbox\OSM\relive_mechelen\mechelen_new.osm").OpenRead());
            //map.AddLayer(new LayerOsm(dataSource, mapCSSInterpreter, map.Projection));
            var layerTile = new LayerTile(@"http://localhost:1234/tiles_kempen/{z}/{x}/{y}.png", 200);
            //layerTile.MinZoom = 12;
            //layerTile.MaxZoom = 13;
            map.AddLayer(layerTile);
            //map.AddLayer(new LayerScene(
            //    Scene2D.Deserialize(new FileInfo(@"default.map").OpenRead(),
            //        true)));

            // initialize route/points layer.
            _layerRoute = new LayerRoute(new OsmSharp.Math.Geo.Projections.WebMercator());
            map.AddLayer(_layerRoute);
            _layerPrimitives = new LayerPrimitives(new OsmSharp.Math.Geo.Projections.WebMercator());
            map.AddLayer(_layerPrimitives);

            // set control properties.
            this.mapControl1.Map = map;
            this.mapControl1.MapCenter = new GeoCoordinate(51.262, 4.7880); // wechel
            this.mapControl1.MapZoom = 14;
            this.mapControl1.MapMouseClick += mapControl1_MapMouseClick;
            this.mapControl1.MapMouseMove += mapControl1_MapMouseMove;
        }

        void mapControl1_MapMouseMove(MapControlEventArgs e)
        {
            OsmSharp.Logging.Log.TraceEvent("MapControlForm", OsmSharp.Logging.TraceEventType.Information, "Map mouse move");
        }

        private Router _router;

        private List<RouterPoint> _points = new List<RouterPoint>();

        private LayerRoute _layerRoute;

        private LayerPrimitives _layerPrimitives;

        void mapControl1_MapMouseClick(MapControlEventArgs e)
        {
            if(_router != null)
            {
                var routerPoint = _router.Resolve(Vehicle.Car, e.Position);
                if(routerPoint != null)
                {
                    _points.Add(routerPoint);
                    _layerPrimitives.AddPoint(routerPoint.Location, 15, SimpleColor.FromKnownColor(KnownColor.Black).Value);
                    _layerPrimitives.AddPoint(routerPoint.Location, 7, SimpleColor.FromKnownColor(KnownColor.White).Value);
                }

                if(_points.Count > 1)
                {
                    var tspRouter = new RouterTSPWrapper<RouterTSPAEXGenetic>(
                        new RouterTSPAEXGenetic(), _router);

                    var route = tspRouter.CalculateTSP(Vehicle.Car, _points.ToArray());
                    if(route != null)
                    {
                        _layerRoute.Clear();
                        _layerRoute.AddRoute(route);
                    }
                }
            }
        }
    }
}
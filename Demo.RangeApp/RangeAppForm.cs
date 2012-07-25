using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Osm.Map.Layers.Custom;
using Osm.Map.Layers.Tiles;
using Tools.Math.Geo;
using System.Threading;
using Osm.Routing;
using Tools.Xml.Sources;
using Osm.Core.Xml;
using Tools.Math.Shapes;
using Tools.Math.Geo.Factory;
using Osm.Map.Elements;
using Osm.Routing.Raw;
using Osm.Routing.Raw.Graphs.Interpreter;
using Osm.Routing.Core.Route;
using Osm.Routing.Core;
using Osm.Routing.Core.Route.Map;
using Osm.Data.Raw.XML.OsmSource;

namespace Demo.RangeApp
{
    public partial class RangeAppForm : Form
    {
        public RangeAppForm()
        {
            InitializeComponent();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.InitializeMap();
        }


        #region Map

        private CustomLayer _status_layer;

        private CustomLayer _points_layer;

        private CustomLayer _range_layer;

        private CustomLayer _range_advanced_layer;

        private SpacialRouteLayer _spacial_route_layer;

        private System.Windows.Forms.Timer tmr;

        private void InitializeMap()
        {
            Osm.Map.Map map = new Osm.Map.Map();
            map.Layers.Add(new TilesLayer());

            this.mapEditorUserControl1.ShowLog = false;
            this.mapEditorUserControl1.ShowToolBar = false;
            this.mapEditorUserControl1.mapTarget.DisplayCardinalDirections = false;
            this.mapEditorUserControl1.Map = map;
            this.mapEditorUserControl1.Center = new GeoCoordinate(51.264, 4.7873);
            this.mapEditorUserControl1.ZoomFactor = 12f;

            _status_layer = new CustomLayer();
            map.Layers.Add(_status_layer);

            _spacial_route_layer = new SpacialRouteLayer();
            map.Layers.Add(_spacial_route_layer);

            _points_layer = new CustomLayer();
            map.Layers.Add(_points_layer);

            _range_advanced_layer = new CustomLayer();
            map.Layers.Add(_range_advanced_layer);

            _range_layer = new CustomLayer();
            map.Layers.Add(_range_layer);


            tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 200;
            tmr.Tick += new EventHandler(tmr_Tick);
            tmr.Enabled = true;
            tmr.Start();
        }

        private delegate void tmr_TickDelegate(object sender, EventArgs e);
        void tmr_Tick(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new tmr_TickDelegate(tmr_Tick), sender, e);
                return;
            }
            this.mapEditorUserControl1.Refresh();
        }

        #endregion

        #region Test


        private delegate void InvokeDelegate();

        /// <summary>
        /// Generates a series of image that can be turned into an animation.
        /// </summary>
        private void StartTest()
        {
            GeoCoordinate from = new GeoCoordinate(51.30173, 4.78093); // vlimmeren
            //GeoCoordinate from = new GeoCoordinate(51.26373, 4.78562); // wechelderzande
            //GeoCoordinate to = new GeoCoordinate(51.20268, 4.76994); // vorselaar
            //GeoCoordinate to = new GeoCoordinate(51.20607, 4.42093); // antwerpen
            //GeoCoordinate to = new GeoCoordinate(51.33318, 4.96103); // turnhout
            GeoCoordinate to = new GeoCoordinate(51.26373, 4.78562); // wechelderzande
            //GeoCoordinate to = new GeoCoordinate(51.22646, 4.83563); // poederlee

            // initialize data.
            OsmDataSource data_source =
                new OsmDataSource(new OsmDocument(new XmlFileSource(string.Format(@"test_area.osm"))));
            
            // create a way interpreter and a grap representing the road net.
            GraphInterpreterTime interpreter = new GraphInterpreterTime(data_source, VehicleEnum.Car);

            // show the images.
            _points_layer.Clear();
            _points_layer.AddImage(global::Demo.RangeApp.Properties.Resources.gas_station, to);
            _points_layer.AddImage(global::Demo.RangeApp.Properties.Resources.car_small, from);

            // create a router.
            Router router = new Router(data_source, interpreter);

            // resolve points and calculate route.
            ResolvedPoint from_point = router.Resolve(from);
            ResolvedPoint to_point = router.Resolve(to);
            OsmSharpRoute route = router.Calculate(from_point, to_point);

            _status_layer.Clear();
            _spacial_route_layer.AddRoute(route, Color.FromArgb(150, Color.Red));

            this.Invoke(new InvokeDelegate(Refresh));
            
            // define a maximum, depending on the metrix used in the interpreter.
            //float max_stop_weight = (float)(route.TotalTime);
            foreach (RoutePointEntry route_point in route.Entries)
            {
                // calculate range.
                ResolvedPoint route_point_resolved = router.Resolve(new GeoCoordinate(route_point.Latitude, route_point.Longitude));
                OsmSharpRoute new_route = router.Calculate(route_point_resolved, to_point);

                // calculate range.
                HashSet<ResolvedPoint> range = new HashSet<ResolvedPoint>(); // router.CalculateRange(route_point_resolved, (float)new_route.TotalTime);

                // calculate polygon.
                ShapePolyGonF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> polygon_shape = null;

                _points_layer.Clear();
                _points_layer.AddImage(global::Demo.RangeApp.Properties.Resources.gas_station, to);
                _points_layer.AddImage(global::Demo.RangeApp.Properties.Resources.car_small, new GeoCoordinate(route_point.Latitude, route_point.Longitude));
                IList<GeoCoordinate> coordinates = new List<GeoCoordinate>();
                foreach (ResolvedPoint reference in range)
                {
                    coordinates.Add(reference.Location);
                }

                // calculate the polygon
                if (coordinates.Count > 2)
                {
                    coordinates = Tools.Math.Algorithms.ConvexHull<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>.Calculate(
                        PrimitiveGeoFactory.Instance,
                        coordinates);

                    polygon_shape = new ShapePolyGonF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                        PrimitiveGeoFactory.Instance,
                        coordinates.ToArray<GeoCoordinate>());

                    ElementPolygon polygon = new ElementPolygon(polygon_shape, Color.FromArgb(75, Color.Blue).ToArgb());
                    _range_advanced_layer.Clear();
                    _range_advanced_layer.AddElement(polygon);

                    this.Invoke(new InvokeDelegate(Refresh));
                }
            }
        }

        //private void CalculateRange(Node node, double max_stop_weight)
        //{
        //    _range_calculations = true;
        //    biggest_weight = null;
        //    stop_weight = max_stop_weight;
        //    status_weight = 25000;

        //    _range_advanced_layer.Clear();
        //    _points_layer.AddImage(global::RangeApp.Main.Properties.Resources.car_small, node.Coordinate);

        //    // create a fictive node; the route wil never find a route to!
        //    Node fictive_node = OsmBaseFactory.CreateNode();
        //    fictive_node.Coordinate = new GeoCoordinate(51.264, 4.7873);

        //    // construct the nodes list.
        //    List<Node> nodes = new List<Node>();
        //    nodes.Add(fictive_node);
        //    nodes.Add(node);

        //    IPoint2PointRouter<Way, Node, GraphInterpreterWeight> router = Tools.Math.Graph.Routing.Point2Point.Facade.CreateCachedRouter<Way, Node>(
        //        RoutingAlgorithmsEnum.Dykstra,
        //        _g,
        //        nodes,
        //        -1);
        //    router.RegisterController(this);

        //    Tools.Core.Output.OutputTextStreamHost.WriteLine("Calculating routes starting@{0}", node.ToString());
        //    _status_layer.Clear();
        //    try
        //    {
        //        router.Calculate(node, nodes);
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    //_status_layer.Clear();

        //    _range_calculations = false;
        //}

        #endregion

        void btnStart_Click(object sender, System.EventArgs e)
        {
            Thread thr = new Thread(new ThreadStart(StartTest));
            thr.Start();

            btnStart.Enabled = false;
        }

    }
}

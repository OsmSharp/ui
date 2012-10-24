// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Osm.Data.SQLite.Raw.Processor;
using Osm.Map.Layers.Tiles;
using Osm.Routing.Core.Route.Map;
using Osm.Map.Layers.Custom;
using System.Threading;
using Osm.Routing.CH.PreProcessing;
using Osm.Routing.CH.PreProcessing.Witnesses;
using Osm.Routing.CH.PreProcessing.Ordering;
using Osm.Data.XML.Raw.Processor;
using Osm.Routing.CH.Processor;
using Osm.Routing.CH.Routing;
using Osm.Renderer.Gdi.Targets.UserControlTarget;
using Osm.Routing.Core.Route;
using Osm.Map.Elements;
using Tools.Math.Geo;
using Tools.Xml.Sources;
using Osm.Data.Raw.XML.OsmSource;
using Osm.Data.Core.DynamicGraph;

namespace OsmRouting.CH.Demo
{
    /// <summary>
    /// The MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Creates a new MainForm.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            this.mapEditorUserControl.SelectionMode = false;
        }


        /// <summary>
        /// Called when the form is loaded.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // initialize the map.
            this.InitializeMap();

            // start the calculation.
            this.Calculate();
        }

        #region Events

        /// <summary>
        /// Called when the user clicks the map.
        /// </summary>
        /// <param name="e"></param>
        private void mapEditorUserControl_MapClick(UserControlTargetEventArgs e)
        {
            this.AddPoint(e.Position);
        }

        #endregion

        #region Calculation

        /// <summary>
        /// Keeps the router.
        /// </summary>
        private Router _router;

        /// <summary>
        /// Keeps the data.
        /// </summary>
        private CHDataSource _data;

        /// <summary>
        /// Keeps the first point.
        /// </summary>
        private CHResolvedPoint _point1;

        /// <summary>
        /// Keeps the second point.
        /// </summary>
        private CHResolvedPoint _point2;

        /// <summary>
        /// Keeps the target.
        /// </summary>
        private CHDataProcessorTarget _target;

        /// <summary>
        /// Starts calculation.
        /// </summary>
        private void Calculate()
        {
            Thread thr = new Thread(new ThreadStart(DoCalculation));
            thr.Start();
        }

        /// <summary>
        /// Holds a list of nodes.
        /// </summary>
        private List<uint> _nodes;

        /// <summary>
        /// Does the calculation(s).
        /// </summary>
        private void DoCalculation()
        {
            //System.Threading.Thread.Sleep(20000);

            _arcs = new Dictionary<long, Dictionary<long, IElement>>();

            //string xml = "wechel.osm"; // the xml data source file.
            //string xml = "matrix.osm"; // the xml data source file.
            //string xml = "eeklo.osm"; // the xml data source file.
            //string xml = "moscow.osm"; // the xml data source file.
            //this.mapEditorUserControl.Center = new GeoCoordinate(55.741005f, 37.546965f);
            //string xml = @"C:\OSM\bin\belgium_highway.osm";
            string xml = @"C:\OSM\bin\eeklo.osm";
            OsmDataSource osm_data = new OsmDataSource(
                new Osm.Core.Xml.OsmDocument(new XmlFileSource(xml)));
            //if (osm_data.HasBoundinBox)
            //{
            //    GeoCoordinateBox box = osm_data.BoundingBox;

            //    // show the data bouding box.
            //    List<GeoCoordinate> corners = new List<GeoCoordinate>();
            //    corners.Add(box.Corners[3]);
            //    corners.Add(box.Corners[1]);
            //    corners.Add(box.Corners[0]);
            //    corners.Add(box.Corners[2]);

            //    Color box_color = Color.FromArgb(125, Color.Orange);
            //    Osm.Map.Elements.ElementPolygon box_element = new Osm.Map.Elements.ElementPolygon(new Tools.Math.Shapes.ShapePolyGonF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
            //                                                                                        Tools.Math.Geo.Factory.PrimitiveGeoFactory.Instance,
            //                                                                                        corners.ToArray()),
            //                                                                                      box_color.ToArgb(),
            //                                                                                      10,
            //                                                                                      false,
            //                                                                                      true);
            //    _box_layer.AddElement(box_element);

            //    this.mapEditorUserControl.Center = box.Center;
            //}

            _data = new CHDataSource();
            //_data = new SQLiteCHData(@"Data Source=c:\temp\sqlite_ch.dat;Synchronous=OFF;Journal Mode=MEMORY;");

            _router = new Router(_data);
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(_data.Graph);
            INodeWeightCalculator calculator = null;
            //calculator = new Osm.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering.SparseOrdering(_data.Graph);
            calculator = new EdgeDifference(_data.Graph, new DykstraWitnessCalculator(_data.Graph, 10));
            //calculator = new EdgeDifferenceContractedSearchSpace(_data, new DykstraWitnessCalculator(_data, 10));
            CHPreProcessor pre_processor = new CHPreProcessor(_data.Graph, calculator, witness_calculator);
            //pre_processor.NotifyArcEvent += new CHPreProcessor.ArcDelegate(pre_processor_NotifyArcEvent);
            //pre_processor.NotifyRemoveEvent += new CHPreProcessor.ArcDelegate(pre_processor_NotifyRemoveEvent);
            _target = new CHDataProcessorTarget(_data);
            if (true)
            {
                XmlDataProcessorSource source = new XmlDataProcessorSource(xml);
                _target.RegisterSource(source);
                _target.Pull();
            }
            else
            {
                //testing start without import
                //_target.Start();
            }

            //pre_processor.Start();

            _nodes = new List<uint>(_data.Graph.GetVertices());

            foreach (uint node in _nodes)
            {
                float latitude, longitude;
                if (_data.Graph.GetVertex(node, out latitude, out longitude))
                {
                    //_nodes_layer.AddText(Color.Black.ToArgb(), 0.4f, node.ToString(), new GeoCoordinate(latitude, longitude));
                }
            }

            // try routing the data.
            //_router.NotifyCHPathSegmentEvent += new Router.NotifyCHPathSegmentDelegate(_router_NotifyCHPathSegmentEvent);

            if (_nodes.Count == 0)
                return;

            // do some random routing.
            int count = 10000;
            while (count > 0)
            {
                uint from = _nodes[Tools.Math.Random.StaticRandomGenerator.Get().Generate(_nodes.Count)];
                uint to = _nodes[Tools.Math.Random.StaticRandomGenerator.Get().Generate(_nodes.Count)];

                if (to != from)
                {
                    CHResolvedPoint vertex1 = _router.GetCHVertex(from);
                    CHResolvedPoint vertex2 = _router.GetCHVertex(to);

                    // calculate the route.
                    OsmSharpRoute route = _router.Calculate(vertex1, vertex2);

                    if (route != null)
                    {
                        _route_layer.Clear();
                        _arcs.Clear();
                        _arcs_layer.Clear();

                        _route_layer.AddRoute(route, Color.FromArgb(150, Color.Blue));

                        this.Invoke(new EmptyDelegate(RefreshMap));
                    }
                    //else
                    //{
                    //    ElementDot dot1 = _route_layer.AddDot(new GeoCoordinate(vertex1.Location.Latitude, vertex1.Location.Longitude));
                    //    //dot1.Radius = 0.01;
                    //    ElementDot dot2 = _route_layer.AddDot(new GeoCoordinate(vertex2.Location.Latitude, vertex2.Location.Longitude));
                    //    //dot2.Radius = 0.01;

                    //    this.Invoke(new EmptyDelegate(RefreshMap));
                    //}

                    //System.Threading.Thread.Sleep(10000);
                }
            }
        }

        ///// <summary>
        ///// Handles notifications of the pre-processor.
        ///// </summary>
        ///// <param name="arc"></param>
        ///// <param name="contracted_id"></param>
        //void pre_processor_NotifyArcEvent(long from_id, long to_id)
        //{
        //    // get the two vertices.
        //    CHVertex vertex1 = _data.GetCHVertex(from_id);
        //    CHVertex vertex2 = _data.GetCHVertex(to_id);
        //    //CHVertex contracted = _data.GetCHVertex(contracted_id);

        //    //// get the level.
        //    //int level = 0;
        //    //if (contracted != null)
        //    //{
        //    //    level = contracted.Level;
        //    //}

        //    //// remove the old arcs.
        //    //if (contracted_id > 0)
        //    //{
        //    //    this.RemoveArc(vertex1.Id, contracted_id);
        //    //    this.RemoveArc(contracted_id, vertex2.Id);
        //    //}

        //    // adds the new arc.
        //    this.AddArc(vertex1, vertex2);

        //    //this.Invoke(new EmptyDelegate(RefreshMap));
        //}


        /// <summary>
        /// Handles notifications of the pre-processor.
        /// </summary>
        /// <param name="from_id"></param>
        /// <param name="to_id"></param>
        void pre_processor_NotifyRemoveEvent(long from_id, long to_id)
        {
            this.RemoveArc(from_id, to_id);

            //this.Invoke(new EmptyDelegate(RefreshMap));
        }


        ///// <summary>
        ///// Notifies the path segment.
        ///// </summary>
        ///// <param name="route"></param>
        //void _router_NotifyCHPathSegmentEvent(CHPathSegment route)
        //{
        //    while (route != null && route.From != null)
        //    {
        //        CHVertex vertex1 = _data.GetCHVertex(route.VertexId);
        //        CHVertex vertex2 = _data.GetCHVertex(route.From.VertexId);

        //        this.AddArc(vertex1, vertex2);

        //        route = route.From;
        //    }
        //}

        /// <summary>
        /// Adds a new point.
        /// </summary>
        /// <param name="coordinate"></param>
        private void AddPoint(GeoCoordinate coordinate)
        {
            if (_router != null)
            {
                // resolves the vertex.
                CHResolvedPoint vertex = _router.Resolve(coordinate);

                // set the point.
                _point1 = _point2;
                _point2 = vertex;

                // calculate if both points have been set.
                if (_point1 != null &&
                    _point2 != null)
                { // a route can be calculated.
                    _arcs_layer.Clear();
                    _arcs.Clear();

                    // calculate the route.
                    OsmSharpRoute route = _router.Calculate(_point1, _point2);

                    _route_layer.Clear();
                    if (route != null)
                    {
                        _route_layer.AddRoute(route, Color.FromArgb(150, Color.Red));

                        this.Invoke(new EmptyDelegate(RefreshMap));
                    }
                }
            }
        }


        #endregion

        #region Map

        /// <summary>
        /// Holds the route layer.
        /// </summary>
        private SpacialRouteLayer _route_layer;

        /// <summary>
        /// Holds the bounding box layer.
        /// </summary>
        private CustomLayer _box_layer;

        /// <summary>
        /// Holds the nodes layer.
        /// </summary>
        private CustomLayer _nodes_layer;

        /// <summary>
        /// Initializes the map.
        /// </summary>
        private void InitializeMap()
        {
            // create the map and all it's layers.
            Osm.Map.Map map = new Osm.Map.Map();

            // add the tiles layer.
            TilesLayer tiles_layer = new TilesLayer("http://otile1.mqcdn.com/tiles/1.0.0/osm/{0}/{1}/{2}.png");
            tiles_layer.MaxZoom = 20;
            map.Layers.Add(tiles_layer);

            // initialize the box layer.
            _box_layer = new CustomLayer();
            _box_layer.MaxZoom = 20;
            map.Layers.Add(_box_layer);

            // initialize the arcs layer.
            _arcs_layer = new CustomLayer();
            _arcs_layer.MaxZoom = 20;
            map.Layers.Add(_arcs_layer);

            _nodes_layer = new CustomLayer();
            _nodes_layer.MaxZoom = 20;
            map.Layers.Add(_nodes_layer);

            // initialize the route layer.
            _route_layer = new SpacialRouteLayer();
            _route_layer.MaxZoom = 20;
            map.Layers.Add(_route_layer);

            // center and zoom.
            double latitude_center = (float)((51.2464000 + 51.2838000) / 2.0);
            double longitude_center = (float)((4.7587000 + 4.8151000) / 2.0);
            this.mapEditorUserControl.Map = map;
            this.mapEditorUserControl.Center = new GeoCoordinate(latitude_center, longitude_center);
            this.mapEditorUserControl.ZoomFactor = 12;
            this.mapEditorUserControl.mapTarget.DisplayCardinalDirections = false;
            this.mapEditorUserControl.mapTarget.DisplayStatus = false;

            // initialize the refresh timer. 
            System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 500;
            tmr.Tick += new EventHandler(tmr_Tick);
            tmr.Enabled = true;
            tmr.Start();
        }

        /// <summary>
        /// Holds the empty delegate.
        /// </summary>
        private delegate void EmptyDelegate();

        /// <summary>
        /// Called when the timer ticks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tmr_Tick(object sender, EventArgs e)
        {
            //this.Invoke(new EmptyDelegate(RefreshMap));
        }

        /// <summary>
        /// Refreshes the map.
        /// </summary>
        private void RefreshMap()
        {
            this.mapEditorUserControl.Refresh();
        }

        #region Arcs Layer

        /// <summary>
        /// Holds the layer with all arcs.
        /// </summary>
        private CustomLayer _arcs_layer;

        /// <summary>
        /// Holds all the arcs.
        /// </summary>
        private Dictionary<long, Dictionary<long, IElement>> _arcs;

        /// <summary>
        /// Removes an arc.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void RemoveArc(long from, long to)
        {
            IElement element;
            Dictionary<long, IElement> elements;
            if (_arcs.TryGetValue(from, out elements) &&
                elements.TryGetValue(to, out element))
            {
                _arcs_layer.RemoveElement(element);
            }
            if (_arcs.TryGetValue(to, out elements) &&
                elements.TryGetValue(from, out element))
            {
                _arcs_layer.RemoveElement(element);
            }
        }

        /// <summary>
        /// Adds an arc.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void AddArc(CHResolvedPoint from, CHResolvedPoint to)
        {
            //int alpha = 255;
            ////if (_target.Levels > 0 && level > 0)
            ////{
            ////    alpha = (int)(55 + ((double)level / (double)_target.Levels) * 200);
            ////}
            //int argb = Color.FromArgb(alpha, Color.Blue).ToArgb();
            int argb = Color.Blue.ToArgb();
            ElementLine line = _arcs_layer.AddLine(
                from.Location, to.Location, false, 2, true, argb);            
            Dictionary<long, IElement> elements;
            if (_arcs.TryGetValue(from.Id, out elements))
            { // add the element.
                if (elements.ContainsKey(to.Id))
                { // remove the elements again.
                    _arcs_layer.RemoveElement(line);
                }
                else
                { // sets the elements.
                    elements[to.Id] = line;
                }
            }
            else if(_arcs.TryGetValue(to.Id, out elements))
            { // add the element.
                if (elements.ContainsKey(from.Id))
                { // remove the elements again.
                    _arcs_layer.RemoveElement(line);
                }
                else
                { // sets the elements.
                    elements[from.Id] = line;
                }
            }
            else
            { // add the element.
                elements = new Dictionary<long,IElement>();
                elements[to.Id] = line;
                _arcs[from.Id] = elements;
            }
        }

        #endregion

        #endregion
    }
}

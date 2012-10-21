// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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
using Osm.Map.Layers.Tiles;
using Tools.Math.Geo;
using Osm.Map.Layers.Custom;
using Osm.Renderer.Gdi.Targets.UserControlTarget;
using Osm.Data;
using Tools.Xml.Sources;
using Osm.Core;
using Osm.Routing;
using Tools.Math.Units.Distance;
using System.Threading;
using Osm.Routing.Instructions;
using System.IO;
using Osm.Routing.Core.Route.Map;
using Osm.Routing.Core.Route;
using Osm.Routing.Core;
using Osm.Routing.Raw;
using Osm.Routing.Core.Exceptions;
using Osm.Data.Raw.XML.OsmSource;

namespace Demo.MTspDemo
{
    /// <summary>
    /// Demo form for the TSP-problem.
    /// </summary>
    public partial class DemoForm : Form
    {
        private CustomLayer _points_layer;

        private SpacialRouteLayer _route_layer;

        private CustomLayer _status_layer;

        private CustomLayer _resolved_layer;

        private List<GeoCoordinate> _points;

        private IDataSourceReadOnly _data;

        private GeoCoordinateBox _data_box;

        private string _test_name;

        public DemoForm()
        {
            InitializeComponent();

            this.mapEditorUserControl.SelectionMode = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // create the map and all it's layers.
            Osm.Map.Map map = new Osm.Map.Map();
            map.Layers.Add(new TilesLayer());
            //map.Layers.Add(new TilesLayer("http://tools.geofabrik.de/osmi/tiles/routing/{0}/{1}/{2}.png"));
            ////map.Layers.Add(new TilesLayer("http://tools.geofabrik.de/osmi/tiles/geometry/{0}/{1}/{2}.png"));
            _status_layer = 
                new CustomLayer();
            map.Layers.Add(_status_layer);
            _resolved_layer = new CustomLayer();
            map.Layers.Add(_resolved_layer);
            CustomLayer data_box_layer = new CustomLayer();
            map.Layers.Add(data_box_layer);
            _route_layer = new SpacialRouteLayer();
            map.Layers.Add(_route_layer);
            _points_layer =
                new CustomLayer();
            map.Layers.Add(_points_layer);
            
            // create the points list.
            _points = new List<GeoCoordinate>();

            // load points.
            _test_name = "demo";
            // try to load a csv ;-seperated
            // latitude; longitude
            FileInfo csv = new FileInfo(string.Format("{0}.csv", _test_name));
            if (csv.Exists)
            {
                DataSet data = 
                    Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null, csv,Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated,true,false);
                foreach(DataRow row  in data.Tables[0].Rows)
                {
                    // be carefull with the parsing and the number formatting for different cultures.
                    double latitude = double.Parse(row[1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                    double longitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                    GeoCoordinate point = new GeoCoordinate(latitude,longitude);
                    _points.Add(point);

                    this.AddUiPoint(point);
                }
            }
            if (_points.Count > 1)
            {
                _data_box = new GeoCoordinateBox(_points.ToArray());
            }

            // create the data source.
            IDataSourceReadOnly osm_data = new OsmDataSource(
                new Osm.Core.Xml.OsmDocument(new XmlFileSource(string.Format("{0}.osm", _test_name))));
            osm_data =
                new Osm.Data.Cache.DataSourceCache(
                osm_data,
                13);
            Color box_color = Color.FromArgb(125, Color.Orange);
            if (osm_data.HasBoundinBox)
            {
                _data_box = osm_data.BoundingBox;
            }

            // show the data bouding box.
            List<GeoCoordinate> corners = new List<GeoCoordinate>();
            corners.Add(_data_box.Corners[3]);
            corners.Add(_data_box.Corners[1]);
            corners.Add(_data_box.Corners[0]);
            corners.Add(_data_box.Corners[2]);

            Osm.Map.Elements.ElementPolygon box_element = new Osm.Map.Elements.ElementPolygon(new Tools.Math.Shapes.ShapePolyGonF<GeoCoordinate,GeoCoordinateBox,GeoCoordinateLine>(
                    Tools.Math.Geo.Factory.PrimitiveGeoFactory.Instance,
                    corners.ToArray()),
                    box_color.ToArgb(),
                    10,
                    false,
                    true);
            data_box_layer.AddElement(box_element);
            _data = osm_data;

            // center and zoom!
            this.mapEditorUserControl.Map = map;
            this.mapEditorUserControl.Center = _data_box.Center;
            this.mapEditorUserControl.ZoomFactor = 16;

            System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 100;
            tmr.Tick += new EventHandler(tmr_Tick);
            tmr.Enabled = true;
            tmr.Start();
        }

        private delegate void EmptyDelegate();

        void tmr_Tick(object sender, EventArgs e)
        {
            this.Invoke(new EmptyDelegate(RefreshMap));
        }

        private void RefreshMap()
        {
            this.mapEditorUserControl.Refresh();
        }

        #region Tsp Calculations

        /// <summary>
        /// Do the TSP Calculations.
        /// </summary>
        private void DoMTspCalculation()
        {
            // do the TSP calculations on the given points.
            this.DoMTspCalculation(_points, VehicleEnum.Car);
        }

        /// <summary>
        /// Do the TSP Calculations on the given points.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="vehicle"></param>
        private void DoMTspCalculation(List<GeoCoordinate> points, VehicleEnum vehicle)
        {
            _status_layer.Clear();

            // create a router and resolve the given points in the nodes list.
            Router router = new Router(_data);
            List<ResolvedPoint> nodes = new List<ResolvedPoint>(router.Resolve(points.ToArray()));
            FileInfo csv = new FileInfo(string.Format("{0}.csv", _test_name));
            if (csv.Exists)
            {
                DataSet data =
                    Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null, csv, Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, false);
                DataTable original = data.Tables[0];

                for (int idx = 0; idx < nodes.Count; idx++)
                {
                    nodes[idx].Tags.Add(new KeyValuePair<string, string>("idx", idx.ToString()));
                    nodes[idx].Name = original.Rows[idx][0].ToString();
                }
            }

            // show the resolved points.
            _resolved_layer.Clear();
            foreach (ResolvedPoint point in nodes)
            {
                _resolved_layer.AddLine(point.Location, point.Original, false, 2, true, Color.Red.ToArgb());
            }

            // get the from/to points.
            ResolvedPoint from = nodes[0];
            ResolvedPoint to = nodes[0];
            nodes.RemoveAt(0);

            try
            {
                // calculate the TSP.
                //OsmSharpRoute result = router.Calculate(from, nodes[10]);
                List<OsmSharpRoute> result = new List<OsmSharpRoute>(); // router.CalculateMTsp(nodes, 5);
                _status_layer.Clear();

                // write ouput.
                if (csv.Exists)
                {
                    DataSet data =
                        Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null, csv, Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, false);
                    DataTable original = data.Tables[0];
                    DataTable new_table = original.Clone();

                    foreach (OsmSharpRoute route in result)
                    {
                        foreach (RoutePointEntry entry in route.Entries)
                        {
                            if (entry.Points != null)
                            {
                                foreach (RoutePoint point in entry.Points)
                                {
                                    string idx_str = RouteTags.GetValueFirst(point.Tags, "idx");
                                    int idx = int.Parse(idx_str);
                                    new_table.Rows.Add(original.Rows[idx].ItemArray);
                                }
                            }
                        }
                    }

                    FileInfo new_file = new FileInfo(csv.Name + ".out.csv");
                    Tools.Core.DelimitedFiles.DelimitedFileHandler.WriteDelimitedFile(null, new_table, new_file, Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, false);
                }
                //result.SaveAsGpx(new FileInfo(csv.Name + ".out.gpx"));
                //result.Save(new FileInfo(csv.Name + ".out.osm"));
                //// show the result.
                //this.ShowRoute(_route_layer, result);

                //// generate instructions.
                //InstructionGenerator generator = new InstructionGenerator();
                //List<Instruction> instructions = generator.Generate(result);

                // show the instructions.
                //this.Invoke(new InvokeParamDelegate(ShowInstructions), instructions);
            }
            catch (RoutingException ex)
            {
                // add to the error layer.
                _points_layer.Clear();

                //_status_layer.Clear();
                foreach (ResolvedPoint point in ex.To)
                {
                    _status_layer.AddImage(global::Demo.MTspDemo.Properties.Resources.flag_blue, point.Location);
                }
                foreach (ResolvedPoint point in ex.To)
                {
                    _status_layer.AddImage(global::Demo.MTspDemo.Properties.Resources.flag_red, point.Location);
                }

                // invoke the cross-thread refresh.
                this.Invoke(new InvokeDelegate(Refresh));
            }
        }

        /// <summary>
        /// Shows the instructions.
        /// </summary>
        /// <param name="par"></param>
        private void ShowInstructions(object par)
        {
            List<Instruction> instructions = par as List<Instruction>;

            this.lstInstructions.Items.Clear();
            // show custom instructions.
            foreach (Instruction instruction in instructions)
            {
                this.lstInstructions.Items.Add(
                    new KeyValuePair<GeoCoordinate,string>(instruction.Location.Center, instruction.Text));
            }

            btnCalculate.Enabled = true;
        }

        /// <summary>
        /// Shows the given route on the map by adding it to the given layer.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="route"></param>
        public void ShowRoute(CustomLayer layer, OsmSharpRoute route)
        {
            // clear the routes layer and add it.
            _route_layer.Clear();
            _route_layer.AddRoute(route, Color.FromArgb(150, Color.Blue));

            // invoke the cross-thread refresh.
            this.Invoke(new InvokeDelegate(Refresh));
        }

        #region Invoke Code

        // ref: http://www.codeproject.com/KB/cs/Cross_thread_Events.aspx

        /// <summary>
        /// Defines a parameterless invoke delegate.
        /// </summary>
        private delegate void InvokeDelegate();

        /// <summary>
        /// Defines an invoke delegate.
        /// </summary>
        /// <param name="par"></param>
        private delegate void InvokeParamDelegate(object par);

        #endregion

        #endregion

        /// <summary>
        /// Called when the map is clicked.
        /// </summary>
        /// <param name="e"></param>
        private void mapEditorUserControl_MapClick(UserControlTargetEventArgs e)
        {
            if (_data_box.IsInside(e.Position))
            {
                this.AddUiPoint(e.Position);
                this.Refresh();

                _points.Add(e.Position);
            }
        }

        private void AddUiPoint(GeoCoordinate point)
        {
            _points_layer.AddImage(global::Demo.MTspDemo.Properties.Resources.house, point);
        }

        /// <summary>
        /// Called when the calculated button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalculate_Click(object sender, EventArgs e)
        {
            btnCalculate.Enabled = false;

            Thread thr = new Thread(new ThreadStart(DoMTspCalculation));
            thr.Start();
        }

        /// <summary>
        /// Called when the clear button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            // clear everything.
            _points.Clear();
            _points_layer.Clear();
            _route_layer.Clear();

            // refresh map.
            this.Refresh();
        }

        /// <summary>
        /// Called when an instruction is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lstInstructions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (lstInstructions.SelectedItem != null)
            {
                this.mapEditorUserControl.Center = 
                    ((KeyValuePair<GeoCoordinate, string>)lstInstructions.SelectedItem).Key;
                this.mapEditorUserControl.Refresh();
            }
        }

        #region IRoutingProgressConsumer Members

        public void Reset()
        {
            _status_layer.Clear();

            // invoke the cross-thread refresh.
            this.Invoke(new InvokeDelegate(Refresh));
        }

        public void ReportSegment(GeoCoordinate from, GeoCoordinate to)
        {
            _status_layer.AddLine(from, to, false, 3, true, Color.Blue.ToArgb());
            // invoke the cross-thread refresh.
            //this.Invoke(new InvokeDelegate(Refresh));
        }

        #endregion
    }
}
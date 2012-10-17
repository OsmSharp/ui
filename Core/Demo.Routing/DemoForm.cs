using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Osm.Data;
using Osm.Map.Layers.Custom;
using Osm.Map.Layers.Tiles;
using Osm.Renderer.Gdi.Targets.UserControlTarget;
using Osm.Routing;
using Tools.Math.Geo;
using Tools.Xml.Sources;
using Osm.Data.Cache;
using Osm.Data.Redis;
using Osm.Routing.Core.Route.Map;
using Osm.Routing.Core.Route;
using Osm.Routing.Core;
using Osm.Routing.Raw;
using Osm.Routing.Raw.Graphs;
using Osm.Routing.Raw.Graphs.Interpreter;
using Osm.Data.Redis.Raw;
using Osm.Data.Core.Sparse;
using Osm.Data.Redis.Sparse;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Data.Raw.XML.OsmSource;
using Osm.Routing.Core.Interpreter.Default;

namespace Demo.Routing
{
    public partial class DemoForm : Form, Osm.Routing.Sparse.PreProcessor.ISparsePreProcessorProgress
    {
        private CustomLayer _points_layer;

        private SpacialRouteLayer _route_layer;

        private CustomLayer _status_layer;

        private List<GeoCoordinate> _points;

        //private RedisSimpleSource _data;
        private IDataSourceReadOnly _data;

        private GeoCoordinateBox _data_box;

        private ISparseData _sparse_data;
        
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
            CustomLayer data_box_layer = new CustomLayer();
            map.Layers.Add(data_box_layer);
            _status_layer = 
                new CustomLayer();
            map.Layers.Add(_status_layer);
            _route_layer =
                new SpacialRouteLayer();
            map.Layers.Add(_route_layer);
            _points_layer =
                new CustomLayer();
            map.Layers.Add(_points_layer);
            
            // center and zoom!
            double latitude_center = 51.04312f;
            double longitude_center = 3.71939f;
            this.mapEditorUserControl.Map = map;
            this.mapEditorUserControl.Center = new GeoCoordinate(latitude_center, longitude_center);
            this.mapEditorUserControl.ZoomFactor = 16;

            // create the points list.
            _points = new List<GeoCoordinate>();

            // create the data source.
            //string source_file = @"C:\temp\RU-MOW.osm";
            string source_file = "demo.osm";
            OsmDataSource osm_data = new OsmDataSource(new Osm.Core.Xml.OsmDocument(new XmlFileSource(source_file)));
            //XmlDataProcessorSource source = new XmlDataProcessorSource(source_file, false);

            //RedisSparseDataProcessorTarget redis_target = new RedisSparseDataProcessorTarget();
            ////redis_target.RegisterSource(source);
            ////redis_target.Pull();
            //ProgressDataProcessorTarget progress = new ProgressDataProcessorTarget(redis_target);
            //progress.RegisterSource(source);
            //progress.Pull();

            // pre-process.
            //_sparse_data = new RedisSparseData();
            //Osm.Data.Redis.RedisSimpleSource redis_source = new Osm.Data.Redis.RedisSimpleSource();
            //Osm.Routing.Raw.Graphs.Graph raw_graph = new Osm.Routing.Raw.Graphs.Graph(
            //    new Osm.Routing.Raw.Graphs.Interpreter.GraphInterpreterTime(redis_source, Osm.Routing.Core.VehicleEnum.Car), redis_source);
            //Osm.Routing.Sparse.PreProcessor.SparsePreProcessor pre_processor = new Osm.Routing.Sparse.PreProcessor.SparsePreProcessor(
            //    _sparse_data, raw_graph);
            //pre_processor.Process(redis_target.ProcessedNodes);

            Color box_color = Color.FromArgb(125, Color.Orange);
            //_data = redis_source;

            // initialize the graph.
            Graph graph = new Graph(new GraphInterpreterTime(new DefaultVehicleInterpreter(VehicleEnum.Car), _data, VehicleEnum.Car), _data);

            this.mapEditorUserControl.Map = map;
            this.mapEditorUserControl.ZoomFactor = 13;

            if (osm_data.HasBoundinBox)
            {
                _data_box = osm_data.BoundingBox;

                // show the data bouding box.
                List<GeoCoordinate> corners = new List<GeoCoordinate>();
                corners.Add(_data_box.Corners[3]);
                corners.Add(_data_box.Corners[1]);
                corners.Add(_data_box.Corners[0]);
                corners.Add(_data_box.Corners[2]);

                Osm.Map.Elements.ElementPolygon box_element = new Osm.Map.Elements.ElementPolygon(new Tools.Math.Shapes.ShapePolyGonF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                        Tools.Math.Geo.Factory.PrimitiveGeoFactory.Instance,
                        corners.ToArray()),
                        box_color.ToArgb(),
                        10,
                        false,
                        true);
                data_box_layer.AddElement(box_element);
                _data = osm_data;

                // center and zoom!
                this.mapEditorUserControl.Center = _data_box.Center;
            }
            else
            {
                this.mapEditorUserControl.Center = new GeoCoordinate(51.189, 3.68);
                //this.mapEditorUserControl.Center = progress.Center;
            }

            System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 500;
            tmr.Tick += new EventHandler(tmr_Tick);
            tmr.Enabled = true;
            tmr.Start();

            //// add two default points.
            //this.AddPoint(new GeoCoordinate(50.969425201416, 3.76164078712463));
            //this.AddPoint(new GeoCoordinate(50.976188659668, 3.53904390335083));

            //GeoCoordinateBox points_box = new GeoCoordinateBox(_points.ToArray());
            //this.mapEditorUserControl.Center = points_box.Center;
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

        private void AddPoint(GeoCoordinate point)
        {
            System.Diagnostics.Debug.WriteLine(point.ToString());
            if (_points.Count < 1)
            {
                _points_layer.AddImage(global::Demo.Routing.Properties.Resources.car, point);
                this.Refresh();

                _points.Add(point);
            }
            else if (_points.Count < 2)
            {
                _points_layer.AddImage(global::Demo.Routing.Properties.Resources.flag_blue, point);
                this.Refresh();

                _points.Add(point);
            }
            else if (_points.Count == 2)
            {
                _points.RemoveAt(0);
                _points.Add(point);

                _points_layer.Clear();
                _points_layer.AddImage(global::Demo.Routing.Properties.Resources.car, _points[0]);
                _points_layer.AddImage(global::Demo.Routing.Properties.Resources.flag_blue, _points[1]);
                this.Refresh();
            }

            if (_points.Count == 2)
            {
                this.Calculate();
            }
        }

        private void mapEditorUserControl_MapClick(UserControlTargetEventArgs e)
        {
            this.AddPoint(e.Position);
        }

        private void Calculate()
        {
            Thread thr = new Thread(new ThreadStart(DoRouteCalculation));
            thr.Start();
        }

        #region Route Calculations

        private void DoRouteCalculation()
        {
            this.DoTspCalculation(_points, VehicleEnum.Car);
        }

        private void DoTspCalculation(List<GeoCoordinate> points, VehicleEnum vehicle)
        {
            try
            {
                //Osm.Routing.Sparse.Routing.Router router = new Osm.Routing.Sparse.Routing.Router(_sparse_data);

                Router router = new Router(_data);

                ResolvedPoint from = router.Resolve(points[0]);
                //from.Tags.Clear();
                //from.Tags.Add(new KeyValuePair<string, string>("from", "true"));
                ResolvedPoint to = router.Resolve(points[1]);
                //to.Tags.Clear();
                //to.Tags.Add(new KeyValuePair<string, string>("to", "true"));

                OsmSharpRoute result = router.Calculate(from, to);
                this.Reset();
                this.ShowRoute(_route_layer, result);

                //if (result != null)
                //{
                //    // generate instructions.
                //    InstructionGenerator generator = new InstructionGenerator();
                //    List<Instruction> instructions = generator.Generate(result);
                //    this.Invoke(new InvokeParamDelegate(ShowInstructions), instructions);
                //}
            }
            catch (Osm.Routing.Core.Exceptions.RoutingException)
            {

            }
        }

        //private void ShowInstructions(object par)
        //{
        //    List<Instruction> instructions = par as List<Instruction>;

        //    this.lstInstructions.Items.Clear();
        //    // show custom instructions.
        //    foreach (Instruction instruction in instructions)
        //    {
        //        this.lstInstructions.Items.Add(
        //            new KeyValuePair<GeoCoordinate,string>(instruction.Location.Center, instruction.Text));
        //    }
        //}


        //void lstInstructions_SelectedIndexChanged(object sender, System.EventArgs e)
        //{
        //    if (lstInstructions.SelectedItem != null)
        //    {
        //        this.mapEditorUserControl.Center = ((KeyValuePair<GeoCoordinate, string>)lstInstructions.SelectedItem).Key;
        //        this.mapEditorUserControl.Refresh();
        //    }
        //}

        public void ShowRoute(CustomLayer layer, OsmSharpRoute route)
        {
            _route_layer.Clear();
            _route_layer.AddRoute(route, Color.FromArgb(150, Color.Blue));

            this.Invoke(new InvokeDelegate(Refresh));
        }

        private delegate void InvokeDelegate();

        private delegate void InvokeParamDelegate(object par);

        #endregion

        private void btnClear_Click(object sender, EventArgs e)
        {
            _points.Clear();
            _points_layer.Clear();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {

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

            //System.Threading.Thread.Sleep(10);

            // invoke the cross-thread refresh.
            //this.Invoke(new InvokeDelegate(Refresh));
        }

        #endregion
        private int _count_total;
        private int _count = 0;

        public void StartVertex(long vertex_id)
        {
            _count++;
            if ((_count % 1000) == 0)
            {
                Console.WriteLine(string.Format("{0}/{1}", _count, _count_total));
            }
        }

        public void ProcessedVertex(SparseVertex vertex, bool deleted)
        {

        }

        public void PersistedBypassed(long vertex_id, GeoCoordinate coordinate, long neighbour1, long neighbour2)
        {

        }
    }
}
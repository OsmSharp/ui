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
using Osm.Routing.Core.TSP.Genetic;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Data.Raw.XML.OsmSource;

namespace Demo.VRP
{
    public partial class MainForm : Form, Osm.Routing.Sparse.PreProcessor.ISparsePreProcessorProgress
    {
        /// <summary>
        /// The layer for the routes.
        /// </summary>
        private SpacialRouteLayer _route_layer;

        /// <summary>
        /// The points layer.
        /// </summary>
        private CustomLayer _points_layer;

        /// <summary>
        /// The error layer.
        /// </summary>
        private CustomLayer _error_layer;

        /// <summary>
        /// The points 
        /// </summary>
        private List<ResolvedPoint> _points;

        ///// <summary>
        ///// The sparse data source.
        ///// </summary>
        //private Osm.Routing.Sparse.ISparseData _sparse_data;


        /// <summary>
        /// The router.
        /// </summary>
        private Osm.Routing.Raw.Router _raw_router;

        public MainForm()
        {
            InitializeComponent();

            this.mapEditorUserControl.SelectionMode = false;

            // create the points list.
            _points = new List<ResolvedPoint>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // create the map and all it's layers.
            Osm.Map.Map map = new Osm.Map.Map();
            map.Layers.Add(new TilesLayer());
            _route_layer = new SpacialRouteLayer();
            map.Layers.Add(_route_layer);
            CustomLayer data_box_layer = new CustomLayer();
            map.Layers.Add(data_box_layer);
            _points_layer = new CustomLayer();
            map.Layers.Add(_points_layer);
            _error_layer = new CustomLayer();
            map.Layers.Add(_error_layer);
            
            // center and zoom!
            double latitude_center = 51.04312f;
            double longitude_center = 3.71939f;
            this.mapEditorUserControl.Map = map;
            this.mapEditorUserControl.Center = new GeoCoordinate(latitude_center, longitude_center);
            this.mapEditorUserControl.ZoomFactor = 16;

            this.mapEditorUserControl.mapTarget.DisplayCardinalDirections = false;
            this.mapEditorUserControl.mapTarget.DisplayStatus = false;


            System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 250;
            tmr.Tick += new EventHandler(tmr_Tick);
            tmr.Enabled = true;
            tmr.Start();
            
            this.Calculate();
        }

        //void vrp_router_IntermidiateResult(OsmSharpRoute[] result)
        //{
        //}


        /// <summary>
        /// Returns a random color
        /// </summary>
        /// <returns></returns>
        private System.Drawing.Color GetColor(int color_indx)
        {
            Color color = Color.FromArgb(0, 0, 0);
            switch (color_indx)
            {
                case 0:
                    color = Color.FromArgb(0, 0, 255);
                    break;
                case 1:
                    color = Color.FromArgb(255, 255, 0);
                    break;
                case 2:
                    color = Color.FromArgb(0, 255, 0);
                    break;
                case 3:
                    color = Color.FromArgb(255, 128, 0);
                    break;
                case 4:
                    color = Color.FromArgb(255, 0, 0);
                    break;
                case 5:
                    color = Color.FromArgb(255, 0, 255);
                    break;
                case 6:
                    color = Color.FromArgb(0, 255, 255);
                    break;
                case 7: //basicColorEnum.colorPaleGray:
                    color = Color.FromArgb(192, 192, 192);
                    break;
                case 8: //basicColorEnum.colorMidGray:
                    color = Color.FromArgb(128, 128, 128);
                    break;
                case 9: //basicColorEnum.colorRed:
                    color = Color.FromArgb(255, 0, 0);
                    break;
                case 10: //basicColorEnum.colorGreen:
                    color = Color.FromArgb(0, 255, 0);
                    break;
                case 11: //basicColorEnum.colorYellow:
                    color = Color.FromArgb(255, 255, 0);
                    break;
                case 12: //basicColorEnum.colorBlue:
                    color = Color.FromArgb(0, 0, 255);
                    break;
                case 13: //basicColorEnum.colorMagenta:
                    color = Color.FromArgb(255, 0, 255);
                    break;
                case 14: //basicColorEnum.colorCyan:
                    color = Color.FromArgb(0, 255, 255);
                    break;
                case 15: //basicColorEnum.colorWhite:
                    color = Color.FromArgb(255, 255, 255);
                    break;
                default:
                    break;
            }

            return System.Drawing.Color.FromArgb(
                255,
                color.R,
                color.G,
                color.B);
        }

        private int Sort(OsmSharpRoute route1, OsmSharpRoute route2)
        {
            if (route2 == null)
            {
                return 1;
            }
            else if (route1 == null)
            {
                return -1;
            }
            return route1.GetBox().Center.Longitude.CompareTo(route2.GetBox().Center.Longitude);
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
            ResolvedPoint resolved_point = _raw_router.Resolve(point);
            if (resolved_point == null)
            {
                //_points_layer.AddImage(global::Demo.VRP.Properties.Resources.cross, point);
            }
            else
            {
                if (!_raw_router.CheckConnectivity(resolved_point, 100))
                {
                    //_points_layer.AddImage(global::Demo.VRP.Properties.Resources.cross, point);
                }
                else
                {
                    _points_layer.AddImage(global::Demo.VRP.Properties.Resources.flag_blue, point);
                    _points.Add(resolved_point);
                }
            }

            //System.Diagnostics.Debug.WriteLine(point.ToString());
            //if (_points.Count < 1)
            //{
            //    _points_layer.AddImage(global::Demo.VRP.Properties.Resources.car, point);
            //    this.Refresh();

            //    _points.Add(point);
            //}
            //else if (_points.Count < 2)
            //{
            //    _points_layer.AddImage(global::Demo.VRP.Properties.Resources.flag_blue, point);
            //    this.Refresh();

            //    _points.Add(point);
            //}
            //else if (_points.Count == 2)
            //{
            //    _points.RemoveAt(0);
            //    _points.Add(point);

            //    _points_layer.Clear();
            //    _points_layer.AddImage(global::Demo.VRP.Properties.Resources.car, _points[0]);
            //    _points_layer.AddImage(global::Demo.VRP.Properties.Resources.flag_blue, _points[1]);
            //    this.Refresh();
            //}

            //if (_points.Count == 2)
            //{
            //    this.Calculate();
            //}
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

        private bool _output_little_movie = true;

        private int _frame = 0;

        private void SaveImage()
        {
            this.Refresh();
            if (this._output_little_movie)
            {
                lock (this.mapEditorUserControl.Map)
                {
                    string file = string.Format("frame_{0}.bmp", _frame.ToString("00000"));
                    _frame++;

                    //getthe instance of the graphics from the control
                    Graphics g = this.mapEditorUserControl.mapTarget.CreateGraphics();

                    //new bitmap object to save the image
                    Bitmap bmp = new Bitmap(this.mapEditorUserControl.mapTarget.Width, this.mapEditorUserControl.mapTarget.Height);

                    //Drawing control to the bitmap
                    this.mapEditorUserControl.mapTarget.DrawToBitmap(bmp, new Rectangle(0, 0,
                        this.mapEditorUserControl.mapTarget.Width, this.mapEditorUserControl.mapTarget.Height));

                    bmp.Save(file, System.Drawing.Imaging.ImageFormat.Bmp);
                    bmp.Dispose();
                }
            }
        }

        #region Route Calculations

        private void DoRouteCalculation()
        {
            try
            {
                //string source_file = @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Ben Abelshausen\lier\lier.osm";
                string source_file = @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Post\Eeklo Osm\post.osm";
                OsmDataSource osm_data = new OsmDataSource(
                    new Osm.Core.Xml.OsmDocument(new XmlFileSource(source_file)));
                Router router = new Router(osm_data, new Osm.Routing.Raw.Graphs.Interpreter.GraphInterpreterTime(osm_data, Osm.Routing.Core.VehicleEnum.Car));

                //int latitude_idx = 2;
                //int longitude_idx = 3;
                //string points_file = @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Ben Abelshausen\lier\lier.csv";

                int latitude_idx = 23;
                int longitude_idx = 24;
                string points_file = @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Post\Eeklo Osm\post.csv";

                //int latitude_idx = 12;
                //int longitude_idx = 13;
                //string points_file = @"C:\PRIVATE\Dropbox\Ugent\Thesis\Test Cases\Ben Abelshausen\K3007\export.csv";

                System.Data.DataSet data = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                    new System.IO.FileInfo(points_file), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated,
                    false, true);
                int cnt = -1;
                int max = 250;
                List<ResolvedPoint> points = new List<ResolvedPoint>();
                List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
                foreach (System.Data.DataRow row in data.Tables[0].Rows)
                {
                    cnt++;
                    if (cnt < max)
                    {
                        //int latitude_idx = 8;
                        //int longitude_idx = 9;

                        string latitude_string = (string)row[latitude_idx];
                        string longitude_string = (string)row[longitude_idx];

                        double longitude = 0;
                        double latitude = 0;
                        if (double.TryParse(longitude_string, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out longitude) &&
                           double.TryParse(latitude_string, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out latitude))
                        {
                            GeoCoordinate point = new GeoCoordinate(latitude, longitude);

                            points.Add(router.Resolve(point));

                            //_points_layer.AddDot(point);
                            _points_layer.AddImage(global::Demo.VRP.Properties.Resources.mailbox_empty, point);
                            coordinates.Add(point);
                        }

                        Tools.Core.Output.OutputTextStreamHost.WriteLine("Processed {0}/{1}!",
                            data.Tables[0].Rows.IndexOf(row), data.Tables[0].Rows.Count);
                    }
                }

                GeoCoordinateBox box = new GeoCoordinateBox(coordinates.ToArray());
                this.mapEditorUserControl.Center = box.Center;

                //Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic.RouterGeneticSimple<ResolvedPoint> vrp_router
                //     = new Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic.RouterGeneticSimple<ResolvedPoint>(router, 1000, 5400);
                Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic.RouterGeneticSimple<ResolvedPoint> vrp_router
                    = new Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic.RouterGeneticSimple<ResolvedPoint>(router, 1000, 1500,
                        200, 500, 10, 60, 30);
                //Osm.Routing.Core.VRP.NoDepot.MinMaxTime.BestPlacement.RouterBestPlacementWithImprovements<ResolvedPoint> vrp_router
                //    = new Osm.Routing.Core.VRP.NoDepot.MinMaxTime.BestPlacement.RouterBestPlacementWithImprovements<ResolvedPoint>(
                //        router, 1000, 1500, 10, 0.25f);
                vrp_router.IntermidiateResult += new Osm.Routing.Core.VRP.RouterVRP<ResolvedPoint>.OsmSharpRoutesDelegate(vrp_router_IntermidiateResult);
                //vrp_router.IntermidiateResult += new Osm.Routing.Core.VRP.RouterVRP<ResolvedPoint>.OsmSharpRoutesDelegate(vrp_router_IntermidiateResult);
                int tests = 1;
                for (int idx1 = 0; idx1 < tests; idx1++)
                {
                    OsmSharpRoute[] routes = vrp_router.CalculateNoDepot(points.ToArray());
                    this.vrp_router_IntermidiateResult(routes, null);
                    int idx = 0;
                    foreach (OsmSharpRoute route in routes)
                    {
                        if (route != null)
                        {
                            idx++;

                            route.SaveAsGpx(new System.IO.FileInfo(string.Format("route_{0}_{1}.gpx", max, idx)));
                        }
                    }
                }
            }
            catch (Osm.Routing.Core.Exceptions.RoutingException ex)
            {
                // add to the error layer.
                _error_layer.Clear();

                //_status_layer.Clear();
                foreach (ResolvedPoint point in ex.From)
                {
                    _error_layer.AddImage(global::Demo.VRP.Properties.Resources.cross, point.Location);
                }
                foreach (ResolvedPoint point in ex.To)
                {
                    _error_layer.AddImage(global::Demo.VRP.Properties.Resources.cross, point.Location);
                }

                // invoke the cross-thread refresh.
                this.Invoke(new InvokeDelegate(Refresh));
            }
        }

        void vrp_router_IntermidiateResult(OsmSharpRoute[] result, Dictionary<int, List<int>> solution)
        {
            List<OsmSharpRoute> routes = new List<OsmSharpRoute>(result);
            routes.Sort(Sort);
            _route_layer.Clear();

            for (int idx = 0; idx < routes.Count; idx++)
            {
                OsmSharpRoute route = routes[idx];
                _route_layer.AddRoute(route, this.GetColor(idx));
            }

            this.Invoke(new InvokeDelegate(SaveImage));
            //this.SaveImage();
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
            //_route_layer.Clear();
            _route_layer.AddRoute(route, Color.FromArgb(150, this.RandomColor()));

            this.Invoke(new InvokeDelegate(Refresh));
        }

        /// <summary>
        /// Returns a random color
        /// </summary>
        /// <returns></returns>
        private System.Drawing.Color RandomColor()
        {
            Color color = Color.FromArgb(0, 0, 0); ;
            int color_indx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(16);
            switch (color_indx)
            {
                case 0:
                    color = Color.FromArgb(0, 0, 0);
                    break;
                case 1:
                    color = Color.FromArgb(128, 0, 0);
                    break;
                case 2:
                    color = Color.FromArgb(0, 128, 0);
                    break;
                case 3:
                    color = Color.FromArgb(128, 128, 0);
                    break;
                case 4:
                    color = Color.FromArgb(0, 0, 128);
                    break;
                case 5:
                    color = Color.FromArgb(128, 0, 128);
                    break;
                case 6:
                    color = Color.FromArgb(0, 128, 128);
                    break;
                case 7: //basicColorEnum.colorPaleGray:
                    color = Color.FromArgb(192, 192, 192);
                    break;
                case 8: //basicColorEnum.colorMidGray:
                    color = Color.FromArgb(128, 128, 128);
                    break;
                case 9: //basicColorEnum.colorRed:
                    color = Color.FromArgb(255, 0, 0);
                    break;
                case 10: //basicColorEnum.colorGreen:
                    color = Color.FromArgb(0, 255, 0);
                    break;
                case 11: //basicColorEnum.colorYellow:
                    color = Color.FromArgb(255, 255, 0);
                    break;
                case 12: //basicColorEnum.colorBlue:
                    color = Color.FromArgb(0, 0, 255);
                    break;
                case 13: //basicColorEnum.colorMagenta:
                    color = Color.FromArgb(255, 0, 255);
                    break;
                case 14: //basicColorEnum.colorCyan:
                    color = Color.FromArgb(0, 255, 255);
                    break;
                case 15: //basicColorEnum.colorWhite:
                    color = Color.FromArgb(255, 255, 255);
                    break;
                default:
                    break;
            }

            return System.Drawing.Color.FromArgb(
                175,
                color.R,
                color.G,
                color.B);
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
            _points_layer.Clear();

            // invoke the cross-thread refresh.
            this.Invoke(new InvokeDelegate(Refresh));
        }

        public void ReportSegment(GeoCoordinate from, GeoCoordinate to)
        {
            _points_layer.AddLine(from, to, false, 3, true, Color.Blue.ToArgb());

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
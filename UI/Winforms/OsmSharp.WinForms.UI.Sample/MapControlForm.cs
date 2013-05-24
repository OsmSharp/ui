using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data.Xml.OsmSource;
using OsmSharp.Osm.Data.Xml.Processor;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Graphs.Serialization;
using OsmSharp.Routing.Route;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Map.Layers;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.UI.Renderer;

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
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.WinForms.UI.Sample.test.mapcss"),
                imageSource);

            // initialize the data source.
            var dataSource = new OsmDataSource(new FileInfo(@"c:\OSM\bin\gistel.osm").OpenRead());
                //Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.WinForms.UI.Sample.test.osm"));

            // initialize map.
            var map = new Map();
            //map.AddLayer(new OsmLayer(dataSource, mapCSSInterpreter));
            //map.AddLayer(new LayerTile(@"http://otile1.mqcdn.com/tiles/1.0.0/osm/{0}/{1}/{2}.png"));
            //map.AddLayer(
            //    new LayerScene(
            //        Scene2D.Deserialize(new FileInfo(@"c:\OSM\bin\test.osm.pbf.scene").OpenRead(), true)));
            map.AddLayer(
                new LayerScene(
                    Scene2D.Deserialize(new FileInfo(@"c:\OSM\bin\wvl.osm.pbf.scene.simple").OpenRead(), true)));

            //var routingSerializer = new V2RoutingDataSourceLiveEdgeSerializer(true);
            //var graphSerialized = routingSerializer.Deserialize(
            //    Assembly.GetExecutingAssembly().GetManifestResourceStream(
            //        "OsmSharp.WinForms.UI.Sample.test.osm.pbf.routing.3"));
            //var graphLayer = new LayerDynamicGraphLiveEdge(graphSerialized, mapCSSInterpreter);
            //map.AddLayer(graphLayer);

            //// create graph layer.
            //var xmlOsmStreamReader =
            //    new XmlOsmStreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.WinForms.UI.Sample.test.osm"));
            //var tagsIndex = new SimpleTagsIndex();
            //var osmInterpreter = new OsmRoutingInterpreter();
            //// do the data processing.
            //var memoryDynamicGraph = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            //var targetData = new LiveGraphOsmStreamWriter(
            //    memoryDynamicGraph, osmInterpreter, tagsIndex);
            //targetData.RegisterSource(xmlOsmStreamReader);
            //targetData.Pull();
            //var graphLayer = new LayerDynamicGraphLiveEdge(memoryDynamicGraph, mapCSSInterpreter);
            //map.AddLayer(graphLayer);

            //// calculate route.            
            //Router router = Router.CreateLiveFrom(
            //    graphSerialized,
            //    new OsmRoutingInterpreter());
            //OsmSharpRoute route = router.Calculate(Vehicle.Car, 
            //    router.Resolve(Vehicle.Car, new GeoCoordinate(51.15136, 3.19462)),
            //    router.Resolve(Vehicle.Car, new GeoCoordinate(51.075023, 3.096632)));
            //var osmSharpLayer = new LayerOsmSharpRoute(map.Projection);
            //osmSharpLayer.AddRoute(route);
            //map.AddLayer(osmSharpLayer);

            //// create gpx layer.
            //var gpxLayer = new LayerGpx(map.Projection);
            //gpxLayer.AddGpx(
            //    Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.WinForms.UI.Sample.test.gpx"));
            //map.AddLayer(gpxLayer);

            // set control properties.
            this.mapControl1.Map = map;
            //this.mapControl1.Center = new GeoCoordinate(51.0095111, 3.3210996); 
            //this.mapControl1.Center = new GeoCoordinate(51.26337, 4.78739);
            //this.mapControl1.Center = new GeoCoordinate(50.88672, 3.23899);
            this.mapControl1.Center = new GeoCoordinate(51.156803, 2.958887);
            this.mapControl1.ZoomLevel = 16;
        }
    }
}
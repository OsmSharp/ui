using System;
using System.IO;
using System.Threading;
using System.Windows;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.Wpf.UI.Log;

namespace OsmSharp.Wpf.UI.Sample
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Native.Initialize();

            //OsmSharp.Logging.Log.Enable();
            //OsmSharp.Logging.Log.RegisterListener(
            //    new DebugTraceListener());
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //initialize map.
            var map = new OsmSharp.UI.Map.Map(new WebMercator());

            // create the MapCSS image source.
            var imageSource = new MapCSSDictionaryImageSource();
            // initialize mapcss interpreter.
            var mapCssInterpreter = new MapCSSInterpreter(File.OpenRead("data\\opencyclemap.mapcss"), imageSource);
            var source = new XmlOsmStreamSource(File.OpenRead("data\\test.osm"));


            var testLayer = new LayerOsm(MemoryDataSource.CreateFrom(source), mapCssInterpreter, new WebMercator());
           map.AddLayer(testLayer);


             // map.AddLayerTile(@"http://b.tile.openstreetmap.org/{z}/{x}/{y}.png");

            // map.BackColor = SimpleColor.FromKnownColor(OsmSharp.UI.KnownColor.Black).Value;

            //  set control properties.
            MapControl.Map = map;
            MapControl.MapCenter = testLayer.Envelope.Center;
           // MapControl.MapCenter = new GeoCoordinate(51.2667, 4.7914); // wechel
            MapControl.MapZoom = 14;

            MapControl.NotifyMapViewChanged();
        }
    }
}

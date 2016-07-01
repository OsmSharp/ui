using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OsmSharp.Geo.Features;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.UI;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;
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
          // map.AddLayer(testLayer);


               map.AddLayerTile(@"http://b.tile.openstreetmap.org/{z}/{x}/{y}.png");

            // map.BackColor = SimpleColor.FromKnownColor(OsmSharp.UI.KnownColor.Black).Value;

            MapControl.SuspendNotifyMapViewChanged();

            //  set control properties.
            MapControl.MapZoom = 14;
            MapControl.Map = map;
            MapControl.MapCenter = testLayer.Envelope.Center;
            // MapControl.MapCenter = new GeoCoordinate(51.2667, 4.7914); // wechel

            var l = new MapLayerWrapper(new WebMercator());

            //l.AddPoint(testLayer.Envelope.Center, 20, SimpleColor.FromKnownColor(OsmSharp.UI.KnownColor.Black).Value);

            MapControl.AddLayer(l, 100);


            MapControl.ResumeNotifyMapViewChanged();
        }
    }

    public class MapLayerWrapper : Layer
    {
       private readonly IProjection _projection;

        public MapLayerWrapper(IProjection projection)
        {
            _projection = projection;


        }

        protected override IEnumerable<Primitive2D> Get(float zoomFactor, View2D view)
        {
            var result = new List<Primitive2D>();
            var viewBox = view.OuterBox;
            var box = new GeoCoordinateBox(_projection.ToGeoCoordinates(viewBox.Min[0], viewBox.Min[1]),
                          _projection.ToGeoCoordinates(viewBox.Max[0], viewBox.Max[1]));



            var point = _projection.ToPixel(box.Center.Latitude, box.Center.Longitude);
            var p = new Icon2D(point[0], point[1], Create(""));
            p.ToolTip = "Подсказка";
            result.Add(p);

            return result;
        }

        public static byte[] Create(string key)
        {
            byte[] result = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var drawingVisual = new DrawingVisual
                    {
                        Clip = new RectangleGeometry(new Rect(new Size(20, 20))),
                        CacheMode = new BitmapCache()
                    };
                    var drawingContext = drawingVisual.RenderOpen();

                    //var canvas = (Canvas)Application.Current.FindResource(key);
                    //if (canvas != null)
                    //{
                    //    //drawingContext.DrawRectangle(new VisualBrush
                    //    //{
                    //    //    Visual = canvas,

                    //    //}, );

                    //    var rectangle = new Rectangle();
                    //    rectangle.Width = 20;
                    //    rectangle.Height = 30;
                    //    rectangle.Fill = new SolidColorBrush(Colors.Brown);

                    //    VisualBrush vb = new VisualBrush(canvas)
                    //    {
                    //    };
                    //    GeometryDrawing gd = new GeometryDrawing(vb, new Pen(Brushes.Transparent, 0), new RectangleGeometry(new Rect(0, 0, 20, 30)));
                    //    var img = new DrawingImage(gd);
                    //    drawingContext.DrawDrawing(img.Drawing);

                    //}

                    drawingContext.DrawEllipse(Brushes.Red, new Pen(Brushes.Green, 2), new Point(10, 10), 10, 10);


                    // drawingContext.Pop();
                    drawingContext.Close();




                    //var rectangle = new Rectangle();
                    //rectangle.Width = 20;
                    //rectangle.Height = 20;
                    //rectangle.Fill = new SolidColorBrush(Colors.Brown);

                    var bitmap = new RenderTargetBitmap(25, 25, 96, 96, PixelFormats.Pbgra32);
                    bitmap.Render(drawingVisual);

                    var encoder = new PngBitmapEncoder();
                    var outputFrame = BitmapFrame.Create(bitmap);
                    encoder.Frames.Add(outputFrame);

                    using (var stream = new MemoryStream())
                    {
                        encoder.Save(stream);
                        result = stream.ToArray();
                    }
                   
                }
                catch (Exception)
                {

                    throw;
                }

            });

            return result;
        }
    }
}

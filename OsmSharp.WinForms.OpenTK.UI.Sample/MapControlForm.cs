using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Map.Styles.MapCSS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsmSharp.WinForms.OpenTK.UI.Sample
{
    public partial class MapControlForm : Form
    {
        public MapControlForm()
        {
            InitializeComponent();

            // initialize mapcss interpreter.
            var mapCSSInterpreter = new MapCSSInterpreter(
                new FileInfo(@"default.mapcss").OpenRead(), new MapCSSDictionaryImageSource());

            // initialize map.
            var map = new OsmSharp.UI.Map.Map();

            //Scene2D scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), new List<float>(new float[] {
            //    16, 14, 12, 10 }));
            //StyleOsmStreamSceneTarget target = new StyleOsmStreamSceneTarget(
            //    mapCSSInterpreter, scene, new WebMercator());
            //FileInfo testFile = new FileInfo(@"kempen.osm.pbf");
            //Stream stream = testFile.OpenRead();
            //OsmStreamSource source = new PBFOsmStreamSource(stream);
            //OsmStreamFilterProgress progress = new OsmStreamFilterProgress(source);
            //target.RegisterSource(progress);
            //target.Pull();

            //var merger = new Scene2DObjectMerger();
            //scene = merger.BuildMergedScene(scene);

            //map.AddLayer(new LayerScene(scene));
            var dataSource = MemoryDataSource.CreateFromPBFStream(
                new FileInfo(@"kempen.osm.pbf").OpenRead());
            map.AddLayer(new LayerOsm(dataSource, mapCSSInterpreter, map.Projection));
            // map.AddLayer(new LayerTile(@"http://otile1.mqcdn.com/tiles/1.0.0/osm/{0}/{1}/{2}.png", 200));
            //map.AddLayer(new LayerScene(
            //    Scene2D.Deserialize(new FileInfo(@"kempen-big.osm.pbf.scene.layered").OpenRead(),
            //        true)));

            //// initialize route/points layer.
            //_layerRoute = new LayerRoute(new OsmSharp.Math.Geo.Projections.WebMercator());
            //map.AddLayer(_layerRoute);
            //_layerPrimitives = new LayerPrimitives(new OsmSharp.Math.Geo.Projections.WebMercator());
            //map.AddLayer(_layerPrimitives);

            // set control properties.
            this.mapControl1.Map = map;
            this.mapControl1.MapCenter = new GeoCoordinate(51.26371, 4.7854); // wechel
            this.mapControl1.MapZoom = 16;
        }
        
        /// <summary>
        /// Raises the OnLoad event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OsmSharp.Osm.Data.Raw.XML.OsmSource;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Map.Layers;

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
            var dataSource = new OsmDataSource( //new FileInfo(@"c:\OSM\bin\wvl.osm").OpenRead());
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.WinForms.UI.Sample.test.osm"));

            // initialize map.
            var map = new Map();
            map.AddLayer(new OsmRawLayer(dataSource, mapCSSInterpreter));

            // set control properties.
            this.mapControl1.Map = map;
            this.mapControl1.Center = new GeoCoordinate(51.0095111, 3.3210996); 
            //this.mapControl1.Center = new GeoCoordinate(51.26337, 4.78739);
            //this.mapControl1.Center = new GeoCoordinate(50.88672, 3.23899);
            this.mapControl1.ZoomFactor = 0; // TODO: improve zoomfactor.
        }
    }
}
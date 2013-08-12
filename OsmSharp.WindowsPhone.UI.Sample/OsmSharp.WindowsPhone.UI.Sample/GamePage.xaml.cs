using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PhoneClassLibrary4;
using OsmSharp.UI.Map;
using OsmSharp.UI.Map.Layers;
using OsmSharp.UI.Renderer;
using System.Reflection;
using OsmSharp.UI.Renderer.Scene;

namespace OsmSharp.WindowsPhone.UI.Sample
{
    public partial class GamePage : MapGamePageView
    {
        public GamePage()
            : base((Application.Current as App).Content)
        {
            this.Initialize();
        }

        public void Initialize()
        {
            // initialize a test-map.
            var map = new Map();
            map.AddLayer(new LayerScene(Scene2DSimple.Deserialize(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.WindowsPhone.UI.Sample.wvl.osm.pbf.scene.simple"), true)));

            // initializes this map.
            this.Map = map;
            this.Center = new OsmSharp.Math.Geo.GeoCoordinate(51.158075, 2.961545);
            this.ZoomLevel = 16;
        }
    }
}
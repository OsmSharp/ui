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
using OsmSharp.Osm.Data.Xml.Processor;
using OsmSharp.Osm.Simple;
using OsmSharp;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI;
using OsmSharp.UI.Renderer;
using KnownColor = System.Drawing.KnownColor;

namespace OsmSharp.WinForms.UI.Sample
{
    public partial class SampleControlForm : Form
    {
        public SampleControlForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //// initialize a view.
            //var view = View2D.CreateFromCenterAndSize(9000, 9000,
            //                                            534463.21f, 6633094.69f);

            this.sampleControl1.Center = new float[] { 534463.21f, 6633094.69f };
            //this.sampleControl1.Center = new float[] { 0f, 0f };
            this.sampleControl1.ZoomFactor = 1;

            // initialize a test-scene.
            var scene2D = new Scene2D();
            scene2D.BackColor = Color.White.ToArgb();
            scene2D.AddPoint(float.MinValue, float.MaxValue, 0, 0, Color.Blue.ToArgb(), 1);

            bool fill = false;
            int color = Color.White.ToArgb();
            int width = 1;

            scene2D.AddPolygon(float.MinValue, float.MaxValue, new float[] { 50, -80, 70 }, new float[] { 20, -10, -70 }, color, width, fill);

            // load test osm file.
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.WinForms.UI.Sample.test.osm");
            var xmlDataProcessorSource = new XmlOsmStreamReader(stream);
            ICollection<SimpleOsmGeo> osmList = xmlDataProcessorSource.PullToCollection();

            // build a scene using spherical mercator.
            IProjection sphericalMercator = new WebMercator();
            var nodes = new Dictionary<long, GeoCoordinate>();
            foreach (SimpleOsmGeo simpleOsmGeo in osmList)
            {
                if (simpleOsmGeo is SimpleNode)
                {
                    var simplenode = (simpleOsmGeo as SimpleNode);
                    double[] point = sphericalMercator.ToPixel(
                        simplenode.Latitude.Value, simplenode.Longitude.Value);
                    nodes.Add(simplenode.Id.Value, new GeoCoordinate(simplenode.Latitude.Value, simplenode.Longitude.Value));
                    scene2D.AddPoint(float.MinValue, float.MaxValue, (float)point[0], (float)point[1],
                                     Color.Yellow.ToArgb(),
                                     2);
                }
                else if (simpleOsmGeo is SimpleWay)
                {
                    var way = (simpleOsmGeo as SimpleWay);
                    var x = new List<float>();
                    var y = new List<float>();
                    if (way.Nodes != null)
                    {
                        for (int idx = 0; idx < way.Nodes.Count; idx++)
                        {
                            GeoCoordinate nodeCoords;
                            if (nodes.TryGetValue(way.Nodes[idx], out nodeCoords))
                            {
                                x.Add((float) sphericalMercator.LongitudeToX(nodeCoords.Longitude));
                                y.Add((float) sphericalMercator.LatitudeToY(nodeCoords.Latitude));
                            }
                        }
                    }

                    if (x.Count > 0)
                    {
                        scene2D.AddLine(float.MinValue, float.MaxValue, x.ToArray(), y.ToArray(), Color.Blue.ToArgb(), 2);
                    }
                }
            }

            this.sampleControl1.Scene = scene2D;

            this.sampleControl1.Invalidate();
        }
    }
}
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
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Osm;
using OsmSharp;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.UI;
using OsmSharp.UI.Renderer;
using KnownColor = System.Drawing.KnownColor;
using OsmSharp.UI.Renderer.Scene;

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

            this.sampleControl1.Center = new double[] { 534463.21f, 6633094.69f };
            //this.sampleControl1.Center = new float[] { 0f, 0f };
            this.sampleControl1.ZoomFactor = 1;

            // initialize a test-scene.
            var scene2D = new Scene2DSimple();
            scene2D.BackColor = Color.White.ToArgb();
            scene2D.AddPoint(float.MinValue, float.MaxValue, 0, 0, Color.Blue.ToArgb(), 1);

            bool fill = false;
            int color = Color.White.ToArgb();
            int width = 1;

            scene2D.AddPolygon(float.MinValue, float.MaxValue, 
                new double[] { 50, -80, 70 }, new double[] { 20, -10, -70 }, color, width, fill);

            // load test osm file.
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.WinForms.UI.Sample.test.osm");
            var xmlDataProcessorSource = new XmlOsmStreamSource(stream);
            ICollection<OsmGeo> osmList = new List<OsmGeo>(xmlDataProcessorSource);

            // build a scene using spherical mercator.
            IProjection sphericalMercator = new WebMercator();
            var nodes = new Dictionary<long, GeoCoordinate>();
            foreach (OsmGeo simpleOsmGeo in osmList)
            {
                if (simpleOsmGeo is Node)
                {
                    var simplenode = (simpleOsmGeo as Node);
                    double[] point = sphericalMercator.ToPixel(
                        simplenode.Latitude.Value, simplenode.Longitude.Value);
                    nodes.Add(simplenode.Id.Value, 
                        new GeoCoordinate(simplenode.Latitude.Value, simplenode.Longitude.Value));
                    scene2D.AddPoint(float.MinValue, float.MaxValue, (float)point[0], (float)point[1],
                                     Color.Yellow.ToArgb(),
                                     2);
                }
                else if (simpleOsmGeo is Way)
                {
                    var way = (simpleOsmGeo as Way);
                    var x = new List<double>();
                    var y = new List<double>();
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
                        scene2D.AddLine(float.MinValue, float.MaxValue, x.ToArray(), y.ToArray(),
                            Color.Blue.ToArgb(), 2);
                    }
                }
            }

            this.sampleControl1.Scene = scene2D;

            this.sampleControl1.Invalidate();
        }
    }
}
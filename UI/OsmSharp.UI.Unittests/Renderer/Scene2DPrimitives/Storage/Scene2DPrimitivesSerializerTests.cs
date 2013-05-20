using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;
using OsmSharp.Osm.Data.Raw.XML.OsmSource;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene2DPrimitives;

namespace OsmSharp.UI.Unittests.Renderer.Scene2DPrimitives.Storage
{
    /// <summary>
    /// Contains tests on the primitives serializer.
    /// </summary>
    [TestFixture]
    public class Scene2DPrimitivesSerializerTests
    {
        /// <summary>
        /// Tests serialization and deserialization of Scene primitives.
        /// </summary>
        [Test]
        public void Scene2DPrimitivesSerializeDeserializeTest()
        {                       
            // create the MapCSS image source.
            var imageSource = new MapCSSDictionaryImageSource(); 

            // load mapcss style interpreter.
            var mapCSSInterpreter = new MapCSSInterpreter(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.UI.Unittests.Data.MapCSS.test.mapcss"),
                imageSource);

            // initialize the data source.
            var dataSource = new OsmDataSource(//new FileInfo(@"c:\OSM\bin\wvl.osm").OpenRead());
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.UI.Unittests.Data.test.osm"));

            // get data.
            var scene = new Scene2D();
            var projection = new WebMercator();
            GeoCoordinateBox box = null;
            foreach (var osmGeo in dataSource.Get(null))
            {
                // translate each object into scene object.
                mapCSSInterpreter.Translate(scene, projection, 16, osmGeo as OsmGeo);

                if (box == null)
                {
                    box = osmGeo.BoundingBox;
                }
                else
                {
                    box = box.Union(osmGeo.BoundingBox);
                }
            }

            // create the stream.
            var stream = new MemoryStream();
            scene.Serialize(stream);

            // deserialize the stream.
            IScene2DPrimitivesSource sceneSource = Scene2D.Deserialize(stream);

            if (box != null)
            {
                // query both and get the same results.
                int counter = 100;
                var rand = new Random();
                while (counter > 0)
                {
                    var queryBox = new GeoCoordinateBox(
                        box.GenerateRandomIn(rand),
                        box.GenerateRandomIn(rand));
                    var zoomFactor = (float) projection.ToZoomFactor(15);
                    View2D testView = View2D.CreateFromBounds(
                        projection.LatitudeToY(queryBox.MaxLat), 
                        projection.LongitudeToX(queryBox.MinLon), 
                        projection.LatitudeToY(queryBox.MinLat),
                        projection.LongitudeToX(queryBox.MaxLon));
                    var testScene= new Scene2D();
                    sceneSource.Get(testScene, testView, zoomFactor);
                    
                    var resultIndex = new HashSet<IScene2DPrimitive>(testScene.Get(testView, zoomFactor));
                    var resultReference = new HashSet<IScene2DPrimitive>(scene.Get(testView, zoomFactor));

                    Assert.AreEqual(resultReference.Count, resultIndex.Count);
                    //foreach (var data in resultIndex)
                    //{
                    //    Assert.IsTrue(resultReference.Contains(data));
                    //}
                    //foreach (var data in resultReference)
                    //{
                    //    Assert.IsTrue(resultIndex.Contains(data));
                    //}
                    counter--;
                }
            }
        }
    }
}

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
using OsmSharp.Osm.Data.Xml.Processor;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;

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
        public void Scene2DSimpleSerializeDeserializeTest()
        {
            // create the MapCSS image source.
            var imageSource = new MapCSSDictionaryImageSource();

            // load mapcss style interpreter.
            var mapCSSInterpreter = new MapCSSInterpreter(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.UI.Unittests.Data.MapCSS.test.mapcss"),
                imageSource);

            // initialize the data source.
            var xmlSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.UI.Unittests.Data.test.osm"));
            IEnumerable<OsmGeo> dataSource = xmlSource;
            MemoryDataSource source = MemoryDataSource.CreateFrom(xmlSource);

            // get data.
            var scene = new Scene2DSimple();
            var projection = new WebMercator();
            GeoCoordinateBox box = null;
            foreach (var osmGeo in dataSource)
            {
                CompleteOsmGeo completeOsmGeo = null;
                switch (osmGeo.Type)
                {
                    case OsmGeoType.Node:
                        completeOsmGeo = CompleteNode.CreateFrom(osmGeo as Node);
                        break;
                    case OsmGeoType.Way:
                        completeOsmGeo = CompleteWay.CreateFrom(osmGeo as Way, 
                            source);
                        break;
                    case OsmGeoType.Relation:
                        completeOsmGeo = CompleteRelation.CreateFrom(osmGeo as Relation,
                            source);
                        break;
                }

                // update box.
                if (completeOsmGeo != null)
                {
                    if (box == null) { box = completeOsmGeo.BoundingBox; }
                    else if (completeOsmGeo.BoundingBox != null) { box = box + completeOsmGeo.BoundingBox; }
                }

                // translate each object into scene object.
                mapCSSInterpreter.Translate(scene, projection, source, osmGeo as OsmGeo);
            }

            // create the stream.
            var stream = new MemoryStream();
            scene.Serialize(stream, false);

            // deserialize the stream.
            IScene2DPrimitivesSource sceneSource = Scene2DSimple.Deserialize(stream, false);

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
                    var zoomFactor = (float)projection.ToZoomFactor(15);
                    View2D testView = View2D.CreateFromBounds(
                        projection.LatitudeToY(queryBox.MaxLat),
                        projection.LongitudeToX(queryBox.MinLon),
                        projection.LatitudeToY(queryBox.MinLat),
                        projection.LongitudeToX(queryBox.MaxLon));
                    var testScene = new Scene2DSimple();
                    sceneSource.Get(testScene, testView, zoomFactor);

                    var resultIndex = new HashSet<Scene2DPrimitive>(testScene.Get(testView, zoomFactor));
                    var resultReference = new HashSet<Scene2DPrimitive>(scene.Get(testView, zoomFactor));

                    //Assert.AreEqual(resultReference.Count, resultIndex.Count);
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

        /// <summary>
        /// Tests serialization and deserialization of Scene primitives.
        /// </summary>
        [Test]
        public void Scene2DStyledSerializeDeserializeTest()
        {
            // create the MapCSS image source.
            var imageSource = new MapCSSDictionaryImageSource();

            // load mapcss style interpreter.
            var mapCSSInterpreter = new MapCSSInterpreter(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.UI.Unittests.Data.MapCSS.test.mapcss"),
                imageSource);

            // initialize the data source.
            var xmlSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.UI.Unittests.Data.test.osm"));
            IEnumerable<OsmGeo> dataSource = xmlSource;
            MemoryDataSource source = MemoryDataSource.CreateFrom(xmlSource);

            // get data.
            var scene = new Scene2DSimple();
            var projection = new WebMercator();
            GeoCoordinateBox box = null;
            foreach (var osmGeo in dataSource)
            {
                CompleteOsmGeo completeOsmGeo = null;
                switch (osmGeo.Type)
                {
                    case OsmGeoType.Node:
                        completeOsmGeo = CompleteNode.CreateFrom(osmGeo as Node);
                        break;
                    case OsmGeoType.Way:
                        completeOsmGeo = CompleteWay.CreateFrom(osmGeo as Way,
                            source);
                        break;
                    case OsmGeoType.Relation:
                        completeOsmGeo = CompleteRelation.CreateFrom(osmGeo as Relation,
                            source);
                        break;
                }

                // update box.
                if (completeOsmGeo != null)
                {
                    if (box == null) { box = completeOsmGeo.BoundingBox; }
                    else if (completeOsmGeo.BoundingBox != null) { box = box + completeOsmGeo.BoundingBox; }
                }

                // translate each object into scene object.
                mapCSSInterpreter.Translate(scene, projection, source, osmGeo as OsmGeo);
            }

            // create the stream.
            var stream = new MemoryStream();
            scene.SerializeStyled(stream, true);

            // deserialize the stream.
            stream.Seek(0, SeekOrigin.Begin);
            IScene2DPrimitivesSource sceneSource = Scene2DSimple.DeserializeStyled(stream, true);

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
                    var zoomFactor = (float)projection.ToZoomFactor(15);
                    View2D testView = View2D.CreateFromBounds(
                        projection.LatitudeToY(queryBox.MaxLat),
                        projection.LongitudeToX(queryBox.MinLon),
                        projection.LatitudeToY(queryBox.MinLat),
                        projection.LongitudeToX(queryBox.MaxLon));
                    var testScene = new Scene2DSimple();
                    sceneSource.Get(testScene, testView, zoomFactor);

                    var resultIndex = new HashSet<Scene2DPrimitive>(testScene.Get(testView, zoomFactor));
                    var resultReference = new HashSet<Scene2DPrimitive>(scene.Get(testView, zoomFactor));

                    //Assert.AreEqual(resultReference.Count, resultIndex.Count);
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

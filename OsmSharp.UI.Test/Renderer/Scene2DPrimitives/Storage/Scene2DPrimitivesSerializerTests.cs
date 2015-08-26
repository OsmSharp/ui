// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.UI.Map.Styles.MapCSS;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.Collections.Tags;

namespace OsmSharp.UI.Test.Unittests.Renderer.Scene2DPrimitives.Storage
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
                    "OsmSharp.UI.Test.data.MapCSS.test.mapcss"),
                imageSource);

            // initialize the data source.
            var xmlSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.UI.Test.data.test.osm"));
            IEnumerable<OsmGeo> dataSource = xmlSource;
            MemoryDataSource source = MemoryDataSource.CreateFrom(xmlSource);

            // get data.
            var scene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), 16);
            var projection = new WebMercator();
            GeoCoordinateBox box = null;
            foreach (var osmGeo in dataSource)
            {
                ICompleteOsmGeo completeOsmGeo = null;
                switch (osmGeo.Type)
                {
                    case OsmGeoType.Node:
                        completeOsmGeo = osmGeo as Node;
                        if(completeOsmGeo.Tags == null)
                        { // make sure every node has a tags collection.
                            completeOsmGeo.Tags = new TagsCollection();
                        }
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
            TagsCollectionBase metaTags = new TagsCollection();
            metaTags.Add("SomeTestKey", "SomeTestValue");
            var stream = new MemoryStream();
            scene.Serialize(stream, true, metaTags);

            // deserialize the stream.
            metaTags = null;
            stream.Seek(0, SeekOrigin.Begin);
            IPrimitives2DSource sceneSource = Scene2D.Deserialize(stream, true, out metaTags);

            // test meta tags.
            Assert.IsTrue(metaTags.ContainsKeyValue("SomeTestKey", "SomeTestValue"));

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
                    var testScene = new Scene2D(new OsmSharp.Math.Geo.Projections.WebMercator(), 16);
                    IEnumerable<Primitive2D> primitives = sceneSource.Get(testView, zoomFactor);

                    //                    var resultIndex = new HashSet<Scene2DPrimitive>(testScene.Get(testView, zoomFactor));
                    //                    var resultReference = new HashSet<Scene2DPrimitive>(scene.Get(testView, zoomFactor));

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

        //        /// <summary>
        //        /// Tests serialization and deserialization of Scene primitives.
        //        /// </summary>
        //        [Test]
        //        public void Scene2DStyledSerializeDeserializeTest()
        //        {
        //            // create the MapCSS image source.
        //            var imageSource = new MapCSSDictionaryImageSource();

        //            // load mapcss style interpreter.
        //            var mapCSSInterpreter = new MapCSSInterpreter(
        //                Assembly.GetExecutingAssembly().GetManifestResourceStream(
        //                    "OsmSharp.UI.Test.Unittests.Data.MapCSS.test.mapcss"),
        //                imageSource);

        //            // initialize the data source.
        //            var xmlSource = new XmlOsmStreamSource(
        //                Assembly.GetExecutingAssembly().GetManifestResourceStream(
        //                    "OsmSharp.UI.Test.Unittests.Data.test.osm"));
        //            IEnumerable<OsmGeo> dataSource = xmlSource;
        //            MemoryDataSource source = MemoryDataSource.CreateFrom(xmlSource);

        //            // get data.
        //            var scene = new Scene2DSimple();
        //            var projection = new WebMercator();
        //            GeoCoordinateBox box = null;
        //            foreach (var osmGeo in dataSource)
        //            {
        //                CompleteOsmGeo completeOsmGeo = null;
        //                switch (osmGeo.Type)
        //                {
        //                    case OsmGeoType.Node:
        //                        completeOsmGeo = CompleteNode.CreateFrom(osmGeo as Node);
        //                        break;
        //                    case OsmGeoType.Way:
        //                        completeOsmGeo = CompleteWay.CreateFrom(osmGeo as Way,
        //                            source);
        //                        break;
        //                    case OsmGeoType.Relation:
        //                        completeOsmGeo = CompleteRelation.CreateFrom(osmGeo as Relation,
        //                            source);
        //                        break;
        //                }

        //                // update box.
        //                if (completeOsmGeo != null)
        //                {
        //                    if (box == null) { box = completeOsmGeo.BoundingBox; }
        //                    else if (completeOsmGeo.BoundingBox != null) { box = box + completeOsmGeo.BoundingBox; }
        //                }

        //                // translate each object into scene object.
        //                mapCSSInterpreter.Translate(scene, projection, source, osmGeo as OsmGeo);
        //            }

        //            // create the stream.
        //            var stream = new MemoryStream();
        //            scene.SerializeStyled(stream, true);

        //            // deserialize the stream.
        //            stream.Seek(0, SeekOrigin.Begin);
        //            IScene2DPrimitivesSource sceneSource = Scene2DSimple.DeserializeStyled(stream, true);

        //            if (box != null)
        //            {
        //                // query both and get the same results.
        //                int counter = 100;
        //                var rand = new Random();
        //                while (counter > 0)
        //                {
        //                    var queryBox = new GeoCoordinateBox(
        //                        box.GenerateRandomIn(rand),
        //                        box.GenerateRandomIn(rand));
        //                    var zoomFactor = (float)projection.ToZoomFactor(15);
        //                    View2D testView = View2D.CreateFromBounds(
        //                        projection.LatitudeToY(queryBox.MaxLat),
        //                        projection.LongitudeToX(queryBox.MinLon),
        //                        projection.LatitudeToY(queryBox.MinLat),
        //                        projection.LongitudeToX(queryBox.MaxLon));
        //                    var testScene = new Scene2DSimple();
        //                    sceneSource.Get(testScene, testView, zoomFactor);

        ////                    var resultIndex = new HashSet<Scene2DPrimitive>(testScene.Get(testView, zoomFactor));
        ////                    var resultReference = new HashSet<Scene2DPrimitive>(scene.Get(testView, zoomFactor));
        ////
        //                    //Assert.AreEqual(resultReference.Count, resultIndex.Count);
        //                    //foreach (var data in resultIndex)
        //                    //{
        //                    //    Assert.IsTrue(resultReference.Contains(data));
        //                    //}
        //                    //foreach (var data in resultReference)
        //                    //{
        //                    //    Assert.IsTrue(resultIndex.Contains(data));
        //                    //}
        //                    counter--;
        //                }
        //            }
        //        }
    }
}

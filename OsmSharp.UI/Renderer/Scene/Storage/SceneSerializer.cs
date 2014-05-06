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
using OsmSharp;
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.IO;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.UI.Renderer.Scene.Primitives;
using OsmSharp.UI.Renderer.Scene.Styles;
using ProtoBuf.Meta;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Serializer;

namespace OsmSharp.UI.Renderer.Scene.Storage
{
    /// <summary>
    /// Scene serializer responsible for serializing/deserializing a given scene.
    /// </summary>
    internal static class SceneSerializer
    {
        /// <summary>
        /// Holds the default projection.
        /// TODO: serialize this into the scene!
        /// </summary>
        public static IProjection DefaultProjection = new WebMercator(); // = 10000;

        /// <summary>
        /// Calculates the scalefactor to store the coordinates.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <returns></returns>
        public static int CalculateScaleFactor(float zoomFactor)
        {
            return (1.0 / Scene2D.CalculateSimplificationEpsilon(DefaultProjection, zoomFactor)).Power10Floor() * 10;
        }

        /// <summary>
        /// Build the runtime type model.
        /// </summary>
        /// <returns></returns>
        private static RuntimeTypeModel BuildRuntimeTypeModel()
        {
            // build protobuf runtime type model.
            RuntimeTypeModel typeModel = RuntimeTypeModel.Create();
            typeModel.Add(typeof(StylePoint), true);
            typeModel.Add(typeof(StyleLine), true);
            typeModel.Add(typeof(StylePolygon), true);
            typeModel.Add(typeof(StyleText), true);
            typeModel.Add(typeof(Scene2DZoomRange), true);
            typeModel.Add(typeof(SceneIndex), true);
            return typeModel;
        }

        /// <summary>
        /// Serializes the given scene.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="scene"></param>
        /// <param name="compress"></param>
        public static void Serialize(Stream stream, TagsCollectionBase metaTags, Scene2D scene, bool compress)
        {
            RuntimeTypeModel typeModel = SceneSerializer.BuildRuntimeTypeModel();

            // [MetaIndexLenght:4][Metadata][SeneIndexLength:4][SceneIndex][SceneLengths:4*zoomFactors.length][Scenes]
            // MetaIndexLenght: int The lenght of the meta index.
            // Metadata: a number of serialized tags.
            // SceneIndexLength: int The length of the sceneindex in bytes.
            // SceneIndex: the serialized scene index.
            // SceneLengths: int[] The lengths of the scenes per zoom level as in the zoomfactors array.
            // Scenes: The serialized scenes themselves.

            // serialize meta tags.
            (new TagsCollectionSerializer()).SerializeWithSize(metaTags, stream);

            // index index.
            SceneIndex sceneIndex = new SceneIndex();
            sceneIndex.LineStyles = scene.GetStyleLines();
            sceneIndex.PointStyles = scene.GetStylePoints();
            sceneIndex.PolygonStyles = scene.GetStylePolygons();
            sceneIndex.TextStyles = scene.GetStyleTexts();
            sceneIndex.ZoomRanges = scene.GetZoomRanges();
            sceneIndex.ZoomFactors = scene.GetZoomFactors();
            sceneIndex.IconImage = scene.GetImages();

            // write SceneIndex
            long positionAfterMeta = stream.Position;
            stream.Seek(positionAfterMeta + 4, SeekOrigin.Begin);
            long indexStart = stream.Position;
            typeModel.Serialize(stream, sceneIndex);

            // write SeneIndexLength
            int indexSize = (int)(stream.Position - indexStart);
            stream.Seek(positionAfterMeta + 0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(indexSize), 0, 4);

            // write Scenes.
            stream.Seek(positionAfterMeta + 4 + indexSize + 4 * sceneIndex.ZoomFactors.Length, SeekOrigin.Begin);
            // index into r-trees and serialize.
            int[] lengths = new int[sceneIndex.ZoomFactors.Length];
            for (int idx = 0; idx < lengths.Length; idx++)
            {
                long position = stream.Position;

                Dictionary<uint, SceneObject> sceneAtZoom = scene.GetObjectsAt(idx);
                RTreeMemoryIndex<SceneObject> memoryIndex = new RTreeMemoryIndex<SceneObject>(50, 100);

                float latestProgress = 0;
                int sceneObjectIdx = 0;
                foreach (KeyValuePair<uint, SceneObject> sceneObjectPair in sceneAtZoom)
                { // loop over all primitives in order.
                    SceneObject sceneObject = sceneObjectPair.Value;
                    uint id = sceneObjectPair.Key;

                    switch (sceneObject.Enum)
                    {
                        case SceneObjectType.IconObject:
                        case SceneObjectType.PointObject:
                        case SceneObjectType.TextObject:
                            OsmSharp.UI.Renderer.Scene.Scene2D.ScenePoint geo = scene.GetPoint(sceneObject.GeoId);
                            PointF2D point = new PointF2D(geo.X, geo.Y);
                            memoryIndex.Add(new BoxF2D(point), sceneObject);
                            break;
                        case SceneObjectType.LineObject:
                        case SceneObjectType.LineTextObject:
                        case SceneObjectType.PolygonObject:
                            OsmSharp.UI.Renderer.Scene.Scene2D.ScenePoints geos = scene.GetPoints(sceneObject.GeoId);
                            memoryIndex.Add(new BoxF2D(geos.X, geos.Y), sceneObject);
                            break;
                    }

                    float progress = (float)System.Math.Round((((double)sceneObjectIdx / (double)sceneAtZoom.Count) * 100));
                    if (progress != latestProgress)
                    {
                        OsmSharp.Logging.Log.TraceEvent("SceneSerializer", OsmSharp.Logging.TraceEventType.Information,
                            "Indexing scene objects at zoom {1} ({2}/{3})... {0}%", progress, sceneIndex.ZoomFactors[idx],
                                sceneObjectIdx, sceneAtZoom.Count);
                        latestProgress = progress;
                    }
                    sceneObjectIdx++;
                }

                // serialize the r-tree.
                OsmSharp.Logging.Log.TraceEvent("SceneSerializer", OsmSharp.Logging.TraceEventType.Information,
                    "Serializing RTRee...");
                SceneObjectRTreeSerializer memoryIndexSerializer = new SceneObjectRTreeSerializer(
                    scene, compress, idx, SceneSerializer.CalculateScaleFactor(sceneIndex.ZoomFactors[idx]));
                memoryIndexSerializer.Serialize(new LimitedStream(stream), memoryIndex);

                lengths[idx] = (int)(stream.Position - position);
            }

            // write SceneLengths
            long end = stream.Position;
            stream.Seek(positionAfterMeta + 4 + indexSize, SeekOrigin.Begin);
            for (int idx = 0; idx < lengths.Length; idx++)
            {
                stream.Write(BitConverter.GetBytes(lengths[idx]), 0, 4);
            }
            stream.Seek(end, SeekOrigin.Begin);
        }

        /// <summary>
        /// Deserializes the given stream into a primitive source.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="compress"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public static Primitive2DSource Deserialize(Stream stream, bool compress, out TagsCollectionBase metaData)
        {
            RuntimeTypeModel typeModel = SceneSerializer.BuildRuntimeTypeModel();

            // [MetaIndexLenght:4][Metadata][SeneIndexLength:4][SceneIndex][SceneLengths:4*zoomFactors.length][Scenes]
            // MetaIndexLenght: int The lenght of the meta index.
            // Metadata: a number of serialized tags.
            // SceneIndexLength: int The length of the sceneindex in bytes.
            // SceneIndex: the serialized scene index.
            // SceneLengths: int[] The lengths of the scenes per zoom level as in the zoomfactors array.
            // Scenes: The serialized scenes themselves.

            // read metaLength.
            metaData = (new TagsCollectionSerializer()).DeserializeWithSize(stream);

            // read SeneIndexLength.
            var intBytes = new byte[4];
            stream.Read(intBytes, 0, 4);
            int indexLength = BitConverter.ToInt32(intBytes, 0);

            // read SceneIndex
            byte[] indexBytes = new byte[indexLength];
            stream.Read(indexBytes, 0, indexLength);
            MemoryStream indexStream = new MemoryStream(indexBytes);
            SceneIndex sceneIndex = typeModel.Deserialize(indexStream, null, typeof(SceneIndex)) as SceneIndex;

            // read SceneLengths
            int[] lengths = new int[sceneIndex.ZoomFactors.Length];
            for (int idx = 0; idx < lengths.Length; idx++)
            {
                stream.Read(intBytes, 0, 4);
                lengths[idx] = BitConverter.ToInt32(intBytes, 0);
            }

            // create the deserializers.
            int start = (int)stream.Position;
            ISpatialIndexReadonly<Primitive2D>[] rTrees = new ISpatialIndexReadonly<Primitive2D>[lengths.Length];
            for (int idx = 0; idx < lengths.Length; idx++)
            {
                Primitive2DRTreeDeserializer deserializer = new Primitive2DRTreeDeserializer(
                    sceneIndex, compress, SceneSerializer.CalculateScaleFactor(sceneIndex.ZoomFactors[idx]));
                long position = stream.Position;
                rTrees[idx] = deserializer.Deserialize(new LimitedStream(stream), true);
                stream.Seek(position + lengths[idx], SeekOrigin.Begin);
            }

            return new Primitive2DSource(sceneIndex.ZoomFactors, rTrees);
        }
    }
}
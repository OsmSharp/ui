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
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.IO;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.UI.Renderer.Scene.Primitives;
using OsmSharp.UI.Renderer.Scene.Styles;
using ProtoBuf.Meta;

namespace OsmSharp.UI.Renderer.Scene.Storage
{
    /// <summary>
    /// Scene serializer responsible for serializing/deserializing a given scene.
    /// </summary>
    internal static class SceneSerializer
    {
        public static int ScaleFactor = 10000;

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
        public static void Serialize(Stream stream, Scene2D scene, bool compress)
        {
            RuntimeTypeModel typeModel = SceneSerializer.BuildRuntimeTypeModel();

            // [SeneIndexLength:4][SceneIndex][SceneLengths:4*zoomFactors.length][Scenes]
            // SceneIndexLength: int The length of the sceneindex in bytes.
            // SceneIndex: the serialized scene index.
            // SceneLengths: int[] The lengths of the scenes per zoom level as in the zoomfactors array.
            // Scenes: The serialized scenes themselves.

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
            stream.Seek(4, SeekOrigin.Begin);
            long indexStart = stream.Position;
            typeModel.Serialize(stream, sceneIndex);

            // write SeneIndexLength
            int indexSize = (int)(stream.Position - indexStart);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(indexSize), 0, 4);

            // write Scenes.
            stream.Seek(4 + indexSize + 4 * sceneIndex.ZoomFactors.Length, SeekOrigin.Begin);
            // index into r-trees and serialize.
            int[] lengths = new int[sceneIndex.ZoomFactors.Length];
            for (int idx = 0; idx < lengths.Length; idx++)
            {
                long position = stream.Position;

                Dictionary<uint, SceneObject> sceneAtZoom = scene.GetObjectsAt(idx);
                RTreeMemoryIndex<SceneObject> memoryIndex = new RTreeMemoryIndex<SceneObject>();
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
                }

                // serialize the r-tree.
                SceneObjectRTreeSerializer memoryIndexSerializer = new SceneObjectRTreeSerializer(
                    scene, compress, idx);
                memoryIndexSerializer.Serialize(new LimitedStream(stream), memoryIndex);

                lengths[idx] = (int)(stream.Position - position);
            }

            // write SceneLengths
            long end = stream.Position;
            stream.Seek(4 + indexSize, SeekOrigin.Begin);
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
        /// <returns></returns>
        public static Primitive2DSource Deserialize(Stream stream, bool compress)
        {
            RuntimeTypeModel typeModel = SceneSerializer.BuildRuntimeTypeModel();

            // [SeneIndexLength:4][SceneIndex][SceneLengths:4*zoomFactors.length][Scenes]
            // SceneIndexLength: int The length of the sceneindex in bytes.
            // SceneIndex: the serialized scene index.
            // SceneLengths: int[] The lengths of the scenes per zoom level as in the zoomfactors array.
            // Scenes: The serialized scenes themselves.

            // read SeneIndexLength.
            byte[] intBytes = new byte[4];
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
                    sceneIndex, compress);
                long position = stream.Position;
                rTrees[idx] = deserializer.Deserialize(new LimitedStream(stream), true);
                stream.Seek(position + lengths[idx], SeekOrigin.Begin);
            }

            return new Primitive2DSource(sceneIndex.ZoomFactors, rTrees);
        }
    }
}
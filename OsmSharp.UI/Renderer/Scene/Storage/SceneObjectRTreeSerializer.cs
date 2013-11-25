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
using Ionic.Zlib;
using OsmSharp.Collections.SpatialIndexes.Serialization.v2;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer.Scene.Primitives;
using ProtoBuf.Meta;

namespace OsmSharp.UI.Renderer.Scene.Storage
{
    /// <summary>
    /// Defines an R-tree serializer.
    /// </summary>
    internal class SceneObjectRTreeSerializer : RTreeStreamSerializer<SceneObject>
    {
        /// <summary>
        /// Holds the simple scene.
        /// </summary>
        private Scene2D _scene;

        /// <summary>
        /// Holds the compressed flag.
        /// </summary>
        private bool _compressed;

        /// <summary>
        /// Holds the scene at index.
        /// </summary>
        private int _sceneAt;

        /// <summary>
        /// Creates a new RTreeSerializer.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="compressed"></param>
        /// <param name="sceneAt"></param>
        public SceneObjectRTreeSerializer(Scene2D scene, bool compressed, int sceneAt)
        {
            _sceneAt = sceneAt;
            _compressed = compressed;
            _scene = scene;
        }

        /// <summary>
        /// Builds the protobuf runtime type model.
        /// </summary>
        /// <param name="typeModel"></param>
        protected override void BuildRuntimeTypeModel(RuntimeTypeModel typeModel)
        {
            typeModel.Add(typeof(SceneObjectBlock), true);
        }

        /// <summary>
        /// Serializes one leaf.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="data"></param>
        /// <param name="boxes"></param>
        /// <returns></returns>
        protected override byte[] Serialize(RuntimeTypeModel typeModel,
            List<SceneObject> data, List<BoxF2D> boxes)
        {
            int scaleFactor = 1000000;

            Dictionary<uint, int> addedPoint = new Dictionary<uint, int>();
            Dictionary<uint, int> addedPoints = new Dictionary<uint, int>();

            SceneObjectBlock leafData = new SceneObjectBlock();

            //leafData.PointsIndexes = new List<int>();
            leafData.PointsX = new List<long>();
            leafData.PointsY = new List<long>();

            leafData.IconPointId = new List<int>();
            leafData.IconImageId = new List<ushort>();
            leafData.IconZoomRangeId = new List<uint>();

            leafData.LinePointsId = new List<int>();
            leafData.LineStyleId = new List<ushort>();
            leafData.LineZoomRangeId = new List<uint>();

            leafData.LineTextPointsId = new List<int>();
            leafData.LineTextStyleId = new List<ushort>();
            leafData.LineTextText = new List<string>();
            leafData.LineTextZoomRangeId = new List<uint>();

            leafData.PointPointId = new List<int>();
            leafData.PointStyleId = new List<ushort>();
            leafData.PointZoomRangeId = new List<uint>();

            leafData.PolygonPointsId = new List<int>();
            leafData.PolygonStyleId = new List<ushort>();
            leafData.PolygonZoomRangeId = new List<uint>();

            leafData.TextPointPointId = new List<int>();
            leafData.TextPointStyleId = new List<ushort>();
            leafData.TextPointText = new List<string>();
            leafData.TextPointZoomRangeId = new List<uint>();

            foreach (SceneObject sceneObject in data)
            {
                int geoId = -1;
                OsmSharp.UI.Renderer.Scene.Scene2D.ScenePoint point;
                OsmSharp.UI.Renderer.Scene.Scene2D.ScenePoints points;
                switch (sceneObject.Enum)
                {
                    case SceneObjectType.IconObject:
                        // get point.
                        point = _scene.GetPoint(sceneObject.GeoId);

                        // set point data and keep id.
                        if (!addedPoint.TryGetValue(sceneObject.GeoId, out geoId))
                        { // the point was not added yet. 
                            geoId = leafData.PointsX.Count;
                            //leafData.PointsIndexes.Add(geoId);
                            leafData.PointsX.Add((long)(scaleFactor * point.X));
                            leafData.PointsY.Add((long)(scaleFactor * point.Y));
                            addedPoint.Add(sceneObject.GeoId, geoId);
                        }
                        leafData.IconPointId.Add(geoId); // add point.

                        // add zoom range.
                        leafData.IconZoomRangeId.Add(sceneObject.ZoomRangeId);

                        // add image id.
                        leafData.IconImageId.Add(sceneObject.StyleId);
                        break;
                    case SceneObjectType.PointObject:
                        // get point.
                        point = _scene.GetPoint(sceneObject.GeoId);

                        // set point data and keep id.
                        if (!addedPoint.TryGetValue(sceneObject.GeoId, out geoId))
                        { // the point was not added yet. 
                            geoId = leafData.PointsX.Count;
                            leafData.PointsX.Add((long)(scaleFactor * point.X));
                            leafData.PointsY.Add((long)(scaleFactor * point.Y));
                            //leafData.PointsIndexes.Add(leafData.PointsY.Count);
                            addedPoint.Add(sceneObject.GeoId, geoId);
                        }
                        leafData.PointPointId.Add(geoId);

                        // add zoom range.
                        leafData.PointZoomRangeId.Add(sceneObject.ZoomRangeId);

                        // add point style.
                        leafData.PointStyleId.Add(sceneObject.StyleId);
                        break;
                    case SceneObjectType.TextObject:
                        // get point.
                        point = _scene.GetPoint(sceneObject.GeoId);

                        // set point data and keep id.
                        if (!addedPoint.TryGetValue(sceneObject.GeoId, out geoId))
                        { // the point was not added yet. 
                            geoId = leafData.PointsX.Count;
                            leafData.PointsX.Add((long)(scaleFactor * point.X));
                            leafData.PointsY.Add((long)(scaleFactor * point.Y));
                            //leafData.PointsIndexes.Add(leafData.PointsY.Count);
                            addedPoint.Add(sceneObject.GeoId, geoId);
                        }
                        leafData.TextPointPointId.Add(geoId);

                        // add zoom range.
                        leafData.TextPointZoomRangeId.Add(sceneObject.ZoomRangeId);

                        // add point style.
                        leafData.TextPointStyleId.Add(sceneObject.StyleId);

                        // add text.
                        leafData.TextPointText.Add(
                            _scene.GetText((sceneObject as SceneTextObject).TextId));
                        break;
                    case SceneObjectType.LineObject:
                        // get points.
                        points = _scene.GetPoints(sceneObject.GeoId);

                        // set points data and keep id.
                        if (!addedPoints.TryGetValue(sceneObject.GeoId, out geoId))
                        { // the point was not added yet. 
                            geoId = leafData.PointsX.Count;
                            leafData.PointsX.AddRange(points.X.ConvertToLongArray(scaleFactor));
                            leafData.PointsY.AddRange(points.Y.ConvertToLongArray(scaleFactor));
                            //leafData.PointsIndexes.Add(leafData.PointsY.Count);
                            addedPoints.Add(sceneObject.GeoId, geoId);
                        }
                        leafData.LinePointsId.Add(geoId);

                        // add zoom range.
                        leafData.LineZoomRangeId.Add(sceneObject.ZoomRangeId);

                        // add point style.
                        leafData.LineStyleId.Add(sceneObject.StyleId);
                        break;
                    case SceneObjectType.LineTextObject:
                        // get points.
                        points = _scene.GetPoints(sceneObject.GeoId);

                        // set points data and keep id.
                        if (!addedPoints.TryGetValue(sceneObject.GeoId, out geoId))
                        { // the point was not added yet. 
                            geoId = leafData.PointsX.Count;
                            leafData.PointsX.AddRange(points.X.ConvertToLongArray(scaleFactor));
                            leafData.PointsY.AddRange(points.Y.ConvertToLongArray(scaleFactor));
                            //leafData.PointsIndexes.Add(leafData.PointsY.Count);
                            addedPoints.Add(sceneObject.GeoId, geoId);
                        }
                        leafData.LineTextPointsId.Add(geoId);

                        // add zoom range.
                        leafData.LineTextZoomRangeId.Add(sceneObject.ZoomRangeId);

                        // add point style.
                        leafData.LineTextStyleId.Add(sceneObject.StyleId);

                        // add text.
                        leafData.LineTextText.Add(
                            _scene.GetText((sceneObject as SceneLineTextObject).TextId));
                        break;
                    case SceneObjectType.PolygonObject:
                        // get points.
                        points = _scene.GetPoints(sceneObject.GeoId);

                        // set points data and keep id.
                        if (!addedPoints.TryGetValue(sceneObject.GeoId, out geoId))
                        { // the point was not added yet. 
                            geoId = leafData.PointsX.Count;
                            leafData.PointsX.AddRange(points.X.ConvertToLongArray(scaleFactor));
                            leafData.PointsY.AddRange(points.Y.ConvertToLongArray(scaleFactor));
                            //leafData.PointsIndexes.Add(leafData.PointsY.Count);
                            addedPoints.Add(sceneObject.GeoId, geoId);
                        }
                        leafData.PolygonPointsId.Add(geoId);

                        // add zoom range.
                        leafData.PolygonZoomRangeId.Add(sceneObject.ZoomRangeId);

                        // add point style.
                        leafData.PolygonStyleId.Add(sceneObject.StyleId);
                        break;
                }
            }

            // delta-encode.
            leafData.PointsX.EncodeDelta();
            leafData.PointsY.EncodeDelta();

            // serialize.
            MemoryStream stream = new MemoryStream();
            typeModel.Serialize(stream, leafData);
            byte[] serializedData = stream.ToArray();
            stream.Dispose();
            if (_compressed)
            { // compress.
                serializedData = GZipStream.CompressBuffer(serializedData);
            }
            return serializedData;
        }

        /// <summary>
        /// Deserializes the given leaf.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="data"></param>
        /// <param name="boxes"></param>
        /// <returns></returns>
        protected override List<SceneObject> DeSerialize(RuntimeTypeModel typeModel,
            byte[] data, out List<BoxF2D> boxes)
        {
            throw new NotSupportedException("Use the Primitive2DRTreeDeserializer to deserialize.");
        }

        /// <summary>
        /// Gets the version string.
        /// </summary>
        public override string VersionString
        {
            get { return "SceneObjectRTreeScene.v1"; }
        }
    }
}
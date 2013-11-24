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
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.UI.Renderer.Scene.Styles;
using ProtoBuf.Meta;

namespace OsmSharp.UI.Renderer.Scene.Storage
{
    /// <summary>
    /// Deserializes an r-tree containing compresses Primitive2D objects as SceneObjects.
    /// </summary>
    internal class Primitive2DRTreeDeserializer : RTreeStreamSerializer<Primitive2D>
    {
        /// <summary>
        /// Holds the compressed flag.
        /// </summary>
        private bool _compressed;

        /// <summary>
        /// Holds the deserialized scene index.
        /// </summary>
        private SceneIndex _index;

        /// <summary>
        /// Creates a new RTreeSerializer.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="compressed"></param>
        public Primitive2DRTreeDeserializer(SceneIndex index, bool compressed)
        {
            _index = index;
            _compressed = compressed;
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
            List<Primitive2D> data, List<BoxF2D> boxes)
        {
            throw new NotSupportedException("Use the SceneObjectRTreeSerializer to deserialize.");
        }

        /// <summary>
        /// Deserializes the given leaf.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="data"></param>
        /// <param name="boxes"></param>
        /// <returns></returns>
        protected override List<Primitive2D> DeSerialize(RuntimeTypeModel typeModel,
            byte[] data, out List<BoxF2D> boxes)
        {
            if (_compressed)
            { // decompress if needed.
                data = GZipStream.UncompressBuffer(data);
            }

            List<Primitive2D> dataLists = new List<Primitive2D>();
            boxes = new List<BoxF2D>();

            int scaleFactor = 1000000;

            // Assume the following stuff already exists in the current scene:
            // - ZoomRanges
            // - Styles

            // deserialize the leaf data.
            SceneObjectBlock leafData = typeModel.Deserialize(
                new MemoryStream(data), null, typeof(SceneObjectBlock)) as SceneObjectBlock;

            // delta-decode.
            leafData.PointsX = leafData.PointsX.DecodeDelta();
            leafData.PointsY = leafData.PointsY.DecodeDelta();

            // store the next points.
            bool[] pointsStarts = new bool[leafData.PointsX.Count];
            // loop over all lines.
            for (int idx = 0; idx < leafData.LinePointsId.Count; idx++)
            {
                pointsStarts[leafData.LinePointsId[idx]] = true;
            }
            // loop over all polygons.
            for (int idx = 0; idx < leafData.PolygonPointsId.Count; idx++)
            {
                pointsStarts[leafData.PolygonPointsId[idx]] = true;
            }
            // loop over all line-texts.
            for (int idx = 0; idx < leafData.LineTextPointsId.Count; idx++)
            {
                pointsStarts[leafData.LineTextPointsId[idx]] = true;
            }
            Dictionary<int, int> pointsBoundaries = new Dictionary<int, int>();
            int previous = 0;
            for (int idx = 1; idx < pointsStarts.Length; idx++)
            {
                if (pointsStarts[idx])
                { // there is a start here.
                    pointsBoundaries[previous] = idx - previous;
                    previous = idx;
                }
            }
            pointsBoundaries[previous] = pointsStarts.Length - previous;

            // loop over all points.
            for (int idx = 0; idx < leafData.PointPointId.Count; idx++)
            {
                // get properties.
                int pointId = leafData.PointPointId[idx];
                uint styleId = leafData.PointStyleId[idx];
                uint zoomRangeId = leafData.PointZoomRangeId[idx];

                // get point/style/zoomrange.
                double x = (double)leafData.PointsX[pointId] / (double)scaleFactor;
                double y = (double)leafData.PointsY[pointId] / (double)scaleFactor;
                Scene2DZoomRange zoomRange = _index.ZoomRanges[zoomRangeId];
                StylePoint style = _index.PointStyles[styleId];

                // build the primitive.
                Point2D point = new Point2D(x, y, style.Color, style.Size);
                point.MinZoom = zoomRange.MinZoom;
                point.MaxZoom = zoomRange.MaxZoom;

                dataLists.Add(point);
                boxes.Add(new BoxF2D(new PointF2D(x, y)));
            }

            // loop over all text-points.
            for (int idx = 0; idx < leafData.TextPointPointId.Count; idx++)
            {
                // get properties.
                int pointId = leafData.TextPointPointId[idx];
                uint styleId = leafData.TextPointStyleId[idx];
                uint zoomRangeId = leafData.TextPointZoomRangeId[idx];
                string text = leafData.TextPointText[idx];

                // get point/style/zoomrange.
                float x = (float)leafData.PointsX[pointId] / (float)scaleFactor;
                float y = (float)leafData.PointsY[pointId] / (float)scaleFactor;
                Scene2DZoomRange zoomRange = _index.ZoomRanges[zoomRangeId];
                StyleText style = _index.TextStyles[styleId];

                // build the primitive.
                Text2D text2D = new Text2D(x, y, text, style.Color, style.Size);
                text2D.MinZoom = zoomRange.MinZoom;
                text2D.MaxZoom = zoomRange.MaxZoom;

                dataLists.Add(text2D);
                boxes.Add(new BoxF2D(new PointF2D(x, y)));
            }

            // loop over all icons.
            for (int idx = 0; idx < leafData.IconPointId.Count; idx++)
            {
                // get properties.
                int pointId = leafData.IconPointId[idx];
                uint imageId = leafData.IconImageId[idx];
                uint zoomRangeId = leafData.IconZoomRangeId[idx];

                // get point/style/zoomrange.
                double x = (double)leafData.PointsX[pointId] / (double)scaleFactor;
                double y = (double)leafData.PointsY[pointId] / (double)scaleFactor;
                Scene2DZoomRange zoomRange = _index.ZoomRanges[zoomRangeId];
                byte[] image = _index.IconImage[(int)imageId];

                // build the primitive.
                Icon2D icon = new Icon2D(x, y, image, zoomRange.MinZoom, zoomRange.MaxZoom);

                dataLists.Add(icon);
                boxes.Add(new BoxF2D(new PointF2D(x, y)));
            }

            // loop over all lines.
            for (int idx = 0; idx < leafData.LinePointsId.Count; idx++)
            {
                // get properties.
                int pointsId = leafData.LinePointsId[idx];
                uint styleId = leafData.LineStyleId[idx];
                uint zoomRangeId = leafData.LineZoomRangeId[idx];
                
                // get points/style/zoomrange.
                int pointsCount = pointsBoundaries[pointsId];
                double[] x =
                    leafData.PointsX.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                double[] y =
                    leafData.PointsY.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                Scene2DZoomRange zoomRange = _index.ZoomRanges[zoomRangeId];
                StyleLine style = _index.LineStyles[styleId];

                // build the primitive.
                Line2D line = new Line2D(x, y, style.Color, style.Width, style.LineJoin, style.Dashes);

                dataLists.Add(line);
                boxes.Add(new BoxF2D(x, y));
            }

            // loop over all polygons.
            for (int idx = 0; idx < leafData.PolygonPointsId.Count; idx++)
            {
                // get properties.
                int pointsId = leafData.PolygonPointsId[idx];
                uint styleId = leafData.PolygonStyleId[idx];
                uint zoomRangeId = leafData.PolygonZoomRangeId[idx];

                // get points/style/zoomrange.
                int pointsCount = pointsBoundaries[pointsId];
                double[] x =
                    leafData.PointsX.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                double[] y =
                    leafData.PointsY.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                Scene2DZoomRange zoomRange = _index.ZoomRanges[zoomRangeId];
                StylePolygon style = _index.PolygonStyles[styleId];

                // build the primitive.
                Polygon2D line = new Polygon2D(x, y, style.Color, style.Width, style.Fill);

                dataLists.Add(line);
                boxes.Add(new BoxF2D(x, y));
            }

            // loop over all line-texts.
            for (int idx = 0; idx < leafData.LineTextPointsId.Count; idx++)
            {
                // get properties.
                int pointsId = leafData.LineTextPointsId[idx];
                uint styleId = leafData.LineTextStyleId[idx];
                uint zoomRangeId = leafData.LineTextZoomRangeId[idx];
                string text = leafData.LineTextText[idx];

                // get points/style/zoomrange.
                int pointsCount = pointsBoundaries[pointsId];
                double[] x =
                    leafData.PointsX.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                double[] y =
                    leafData.PointsY.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                Scene2DZoomRange zoomRange = _index.ZoomRanges[zoomRangeId];
                StyleText style = _index.TextStyles[styleId];

                // build the primitive.
                LineText2D lineText = new LineText2D(x, y, style.Color, style.Size, text);

                dataLists.Add(lineText);
                boxes.Add(new BoxF2D(x, y));
            }
            return dataLists;
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
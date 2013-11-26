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
        /// Holds the scalefactor.
        /// </summary>
        private int _scaleFactor;

        /// <summary>
        /// Creates a new RTreeSerializer.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="compressed"></param>
        public Primitive2DRTreeDeserializer(SceneIndex index, bool compressed, int scaleFactor)
        {
            _scaleFactor = scaleFactor;
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

            int scaleFactor = _scaleFactor;

            // Assume the following stuff already exists in the current scene:
            // - ZoomRanges
            // - Styles

            // deserialize the leaf data.
            SceneObjectBlock leafData = typeModel.Deserialize(
                new MemoryStream(data), null, typeof(SceneObjectBlock)) as SceneObjectBlock;
            
            // decode
            for (int idx = 0; idx < leafData.PointsX.Count; idx++)
            {
                leafData.PointsX[idx] = leafData.PointsX[idx] + leafData.PointsXMin;
                leafData.PointsY[idx] = leafData.PointsY[idx] + leafData.PointsYMin;
            }

            // store the next points.
            bool[] pointsStarts = new bool[leafData.PointsX.Count];
            // loop over all points.
            for (int idx = 0; idx < leafData.PointPointId.Count; idx++)
            {
                pointsStarts[leafData.PointPointId[idx]] = true;
            }
            // loop over all text-points.
            for (int idx = 0; idx < leafData.TextPointPointId.Count; idx++)
            {
                pointsStarts[leafData.TextPointPointId[idx]] = true;
            }
            // loop over all icons.
            for (int idx = 0; idx < leafData.IconPointId.Count; idx++)
            {
                pointsStarts[leafData.IconPointId[idx]] = true;
            }
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

                // get point/style/zoomrange.
                double x = (double)leafData.PointsX[pointId] / (double)scaleFactor;
                double y = (double)leafData.PointsY[pointId] / (double)scaleFactor;
                StylePoint style = _index.PointStyles[styleId];

                // build the primitive.
                Point2D point = new Point2D(x, y, style.Color, style.Size);
                point.Layer = style.Layer;
                point.MinZoom = style.MinZoom;
                point.MaxZoom = style.MaxZoom;

                dataLists.Add(point);
                boxes.Add(new BoxF2D(new PointF2D(x, y)));
            }

            // loop over all text-points.
            for (int idx = 0; idx < leafData.TextPointPointId.Count; idx++)
            {
                // get properties.
                int pointId = leafData.TextPointPointId[idx];
                uint styleId = leafData.TextPointStyleId[idx];
                string text = leafData.TextPointText[idx];

                // get point/style/zoomrange.
                float x = (float)leafData.PointsX[pointId] / (float)scaleFactor;
                float y = (float)leafData.PointsY[pointId] / (float)scaleFactor;
                StyleText style = _index.TextStyles[styleId];

                // build the primitive.
                Text2D text2D = new Text2D(x, y, text, style.Color, style.Size);
                text2D.Layer = style.Layer;
                text2D.HaloColor = style.HaloColor;
                text2D.HaloRadius = style.HaloRadius;
                text2D.MinZoom = style.MinZoom;
                text2D.MaxZoom = style.MaxZoom;

                dataLists.Add(text2D);
                boxes.Add(new BoxF2D(new PointF2D(x, y)));
            }

            // loop over all icons.
            for (int idx = 0; idx < leafData.IconPointId.Count; idx++)
            {
                // get properties.
                int pointId = leafData.IconPointId[idx];
                uint imageId = leafData.IconImageId[idx];

                // get point/style/zoomrange.
                double x = (double)leafData.PointsX[pointId] / (double)scaleFactor;
                double y = (double)leafData.PointsY[pointId] / (double)scaleFactor;
                byte[] image = _index.IconImage[(int)imageId];

                // build the primitive.
                Icon2D icon = new Icon2D(x, y, image);
                icon.Layer = 0;
                // TODO: layer and zoom level. style.MinZoom, style.MaxZoom

                dataLists.Add(icon);
                boxes.Add(new BoxF2D(new PointF2D(x, y)));
            }

            // loop over all lines.
            for (int idx = 0; idx < leafData.LinePointsId.Count; idx++)
            {
                // get properties.
                int pointsId = leafData.LinePointsId[idx];
                uint styleId = leafData.LineStyleId[idx];
                
                // get points/style/zoomrange.
                int pointsCount = pointsBoundaries[pointsId];
                double[] x =
                    leafData.PointsX.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                double[] y =
                    leafData.PointsY.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                StyleLine style = _index.LineStyles[styleId];

                // build the primitive.
                Line2D line = new Line2D(x, y, style.Color, style.Width, style.LineJoin, style.Dashes);
                line.Layer = style.Layer;
                line.MinZoom = style.MinZoom;
                line.MaxZoom = style.MaxZoom;

                dataLists.Add(line);
                boxes.Add(new BoxF2D(x, y));
            }

            // loop over all polygons.
            for (int idx = 0; idx < leafData.PolygonPointsId.Count; idx++)
            {
                // get properties.
                int pointsId = leafData.PolygonPointsId[idx];
                uint styleId = leafData.PolygonStyleId[idx];

                // get points/style/zoomrange.
                int pointsCount = pointsBoundaries[pointsId];
                double[] x =
                    leafData.PointsX.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                double[] y =
                    leafData.PointsY.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                StylePolygon style = _index.PolygonStyles[styleId];

                // build the primitive.
                Polygon2D polygon = new Polygon2D(x, y, style.Color, style.Width, style.Fill);
                polygon.Layer = style.Layer;
                polygon.MaxZoom = style.MaxZoom;
                polygon.MinZoom = style.MinZoom;

                dataLists.Add(polygon);
                boxes.Add(new BoxF2D(x, y));
            }

            // loop over all line-texts.
            for (int idx = 0; idx < leafData.LineTextPointsId.Count; idx++)
            {
                // get properties.
                int pointsId = leafData.LineTextPointsId[idx];
                uint styleId = leafData.LineTextStyleId[idx];
                string text = leafData.LineTextText[idx];

                // get points/style/zoomrange.
                int pointsCount = pointsBoundaries[pointsId];
                double[] x =
                    leafData.PointsX.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                double[] y =
                    leafData.PointsY.GetRange(pointsId, pointsCount).ConvertFromLongArray(scaleFactor);
                StyleText style = _index.TextStyles[styleId];

                // build the primitive.
                LineText2D lineText = new LineText2D(x, y, style.Color, style.Size, text);
                lineText.Layer = style.Layer;
                lineText.Font = style.Font;
                lineText.HaloColor = style.HaloColor;
                lineText.HaloRadius = style.HaloRadius;
                lineText.MinZoom = style.MinZoom;
                lineText.MaxZoom = style.MaxZoom;

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
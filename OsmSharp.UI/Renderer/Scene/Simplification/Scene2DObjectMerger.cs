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
using System.Linq;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer.Scene.Primitives;
using OsmSharp.UI.Renderer.Scene.Styles;
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.Math.Structures.QTree;
using OsmSharp.Math.Structures;

namespace OsmSharp.UI.Renderer.Scene.Simplification
{
    /// <summary>
    /// Resposible for merging similar objects together.
    /// </summary>
    public class Scene2DObjectMerger
    {
        /// <summary>
        /// Holds the epsilon.
        /// </summary>
        private float epsilon;

        /// <summary>
        /// Creates a new scene object merger.
        /// </summary>
        public Scene2DObjectMerger()
        {
            epsilon = 0.00001f;
        }

        /// <summary>
        /// Builds a merged version of the given scene object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Scene2D BuildMergedScene(Scene2D other)
        {
            // build new scene and copy some relevant configurations.
            var target = new Scene2D(new WebMercator(), other.GetZoomFactors().ToList(), true);
            target.BackColor = other.BackColor;

            // merge objects per zoom factor.
            for (int idx = 0; idx < other.GetZoomFactors().Length; idx++)
            {
                this.MergeObjects(target, other, idx);
            }
            return target;
        }

        /// <summary>
        /// Merges objects from the given scene for the given zoom level.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="idx"></param>
        private void MergeObjects(Scene2D target, Scene2D source, int idx)
        {
            var lines = new Dictionary<Scene2D.ScenePoints, Scene2DStylesSet>();
            var linesIndex = new QuadTree<PointF2D, Scene2D.ScenePoints>();

            var polygons = new Dictionary<Scene2D.ScenePoints, Scene2DStylesSet>();
            //var polygonsIndex = new QuadTree<PointF2D, Scene2D.ScenePoints>();

            Dictionary<uint, SceneObject> sceneObjects = source.GetSceneObjectsAt(idx);
            float zoomFactor = source.GetMaximumZoomFactorAt(idx);
            float epsilon = source.CalculateSimplificationEpsilon(zoomFactor);
            foreach (var sceneObject in sceneObjects)
            {
                if (sceneObject.Value.Enum == SceneObjectType.LineObject)
                { // the scene object is a line object.
                    var sceneLineObject = sceneObject.Value as SceneLineObject;
                    Scene2D.ScenePoints scenePoints = source.GetPoints(sceneLineObject.GeoId);
                    Scene2DStylesSet stylesSet = null;
                    if (!lines.TryGetValue(scenePoints, out stylesSet))
                    { // create styles set.
                        stylesSet = new Scene2DStylesSet();
                        lines.Add(scenePoints, stylesSet);

                        // add scenePoints to the index.
                        linesIndex.Add(new PointF2D(scenePoints.X[0], scenePoints.Y[0]), scenePoints);
                        linesIndex.Add(new PointF2D(scenePoints.X[scenePoints.X.Length - 1], scenePoints.Y[scenePoints.Y.Length - 1]), scenePoints);
                    }
                    stylesSet.AddStyleLine(sceneLineObject.StyleId);
                }
                else if (sceneObject.Value.Enum == SceneObjectType.LineTextObject)
                {
                    var sceneLineTextObject = sceneObject.Value as SceneLineTextObject;
                    Scene2D.ScenePoints scenePoints = source.GetPoints(sceneLineTextObject.GeoId);
                    Scene2DStylesSet stylesSet = null;
                    if (!lines.TryGetValue(scenePoints, out stylesSet))
                    { // create styles set.
                        stylesSet = new Scene2DStylesSet();
                        lines.Add(scenePoints, stylesSet);

                        // add scenePoints to the index.
                        linesIndex.Add(new PointF2D(scenePoints.X[0], scenePoints.Y[0]), scenePoints);
                        linesIndex.Add(new PointF2D(scenePoints.X[scenePoints.X.Length - 1], scenePoints.Y[scenePoints.Y.Length - 1]), scenePoints);
                    }
                    stylesSet.AddStyleLineText(sceneLineTextObject.StyleId, sceneLineTextObject.TextId);
                }
                else if (sceneObject.Value.Enum == SceneObjectType.IconObject)
                {
                    throw new NotSupportedException("Icons not yet supported!");
                    //var sceneIconObject = (sceneObject.Value as SceneIconObject);
                    //Scene2D.ScenePoint scenePoint = source.GetPoint(sceneIconObject.GeoId);
                    //source.GetStyleIcon(
                    //target.AddIcon(target.AddPoint(scenePoint.X, scenePoint.Y);
                }
                else if (sceneObject.Value.Enum == SceneObjectType.PointObject)
                {
                    var scenePointObject = (sceneObject.Value as ScenePointObject);
                    Scene2D.ScenePoint scenePoint = source.GetPoint(scenePointObject.GeoId);
                    StylePoint stylePoint = source.GetStylePoint(scenePointObject.StyleId);

                    target.AddStylePoint(target.AddPoint(scenePoint.X, scenePoint.Y), stylePoint.Layer, stylePoint.MinZoom, stylePoint.MaxZoom,
                        stylePoint.Color, stylePoint.Size);
                }
                else if (sceneObject.Value.Enum == SceneObjectType.PolygonObject)
                { // the scene object is a polygon.
                    var scenePolygonObject = (sceneObject.Value as ScenePolygonObject);
                    Scene2D.ScenePoints scenePoints = source.GetPoints(sceneObject.Value.GeoId);
                    Scene2DStylesSet stylesSet = null;
                    if (!polygons.TryGetValue(scenePoints, out stylesSet))
                    { // create styles set.
                        stylesSet = new Scene2DStylesSet();
                        polygons.Add(scenePoints, stylesSet);

                        //// add scenePoints to the index.
                        //polygonsIndex.Add(new PointF2D(scenePoints.X[0], scenePoints.Y[0]), scenePoints);
                        //polygonsIndex.Add(new PointF2D(scenePoints.X[scenePoints.X.Length - 1], scenePoints.Y[scenePoints.Y.Length - 1]), scenePoints);
                    }
                    stylesSet.AddStylePolygon(scenePolygonObject.StyleId);

                    //var scenePolygonObject = (sceneObject.Value as ScenePolygonObject);
                    //Scene2D.ScenePoints scenePoints = source.GetPoints(sceneObject.Value.GeoId);
                    //StylePolygon stylePolygon = source.GetStylePolygon(sceneObject.Value.StyleId);

                    //uint? pointsId = target.AddPoints(scenePoints.X, scenePoints.Y);
                    //if (pointsId.HasValue)
                    //{
                    //    target.AddStylePolygon(pointsId.Value, stylePolygon.Layer, stylePolygon.MinZoom, stylePolygon.MaxZoom,
                    //        stylePolygon.Color, stylePolygon.Width, stylePolygon.Fill);
                    //}
                }
                else if (sceneObject.Value.Enum == SceneObjectType.TextObject)
                {
                    var sceneTextObject = (sceneObject.Value as SceneTextObject);
                    Scene2D.ScenePoint scenePoint = source.GetPoint(sceneObject.Value.GeoId);
                    StyleText styleText = source.GetStyleText(sceneTextObject.StyleId);
                    string text = source.GetText(sceneTextObject.TextId);

                    target.AddText(target.AddPoint(scenePoint.X, scenePoint.Y), styleText.Layer, styleText.MinZoom, styleText.MaxZoom,
                        styleText.Size, text, styleText.Color, styleText.HaloColor, styleText.HaloRadius, styleText.Font);
                }
            }

            // loop until there are no more candidates.
            int totalLines = lines.Count;
            float latestProgress = 0;
            while (lines.Count > 0)
            {
                var line = lines.First();
                lines.Remove(line.Key);

                // report progress.
                float progress = (float)System.Math.Round((((double)(totalLines - lines.Count) / (double)totalLines) * 100));
                if (progress != latestProgress)
                {
                    OsmSharp.Logging.Log.TraceEvent("SceneSerializer", OsmSharp.Logging.TraceEventType.Information,
                        "Merging lines @z{3}e{4} ({1}/{2})... {0}%", progress, totalLines - lines.Count, totalLines, zoomFactor, epsilon);
                    latestProgress = progress;
                }

                // copy the coordinates to lists.
                double[] x = line.Key.X.Clone() as double[];
                double[] y = line.Key.Y.Clone() as double[];

                // find a matching line.
                int mergeCount = 1;
                Scene2D.ScenePoints found;
                MatchPosition foundPosition = this.FindMatch(linesIndex, lines, x, y, line.Value, epsilon, out found);
                while (found != null)
                { // TODO: keep expanding and duplicating until not possible anymore.
                    // remove the found line.
                    lines.Remove(found);

                    // report progress.
                    progress = (float)System.Math.Round((((double)(totalLines - lines.Count) / (double)totalLines) * 100));
                    if (progress != latestProgress)
                    {
                        OsmSharp.Logging.Log.TraceEvent("SceneSerializer", OsmSharp.Logging.TraceEventType.Information,
                            "Merging lines @z{3}e{4} ({1}/{2})... {0}%", progress, totalLines - lines.Count, totalLines, zoomFactor, epsilon);
                        latestProgress = progress;
                    }

                    // add the line.
                    int lengthBefore = x.Length;
                    Array.Resize(ref x, x.Length + found.X.Length - 1);
                    Array.Resize(ref y, y.Length + found.Y.Length - 1);

                    switch (foundPosition)
                    {
                        case MatchPosition.FirstFirst:
                            found.X.InsertToReverse(1, x, 0, found.X.Length - 1);
                            found.Y.InsertToReverse(1, y, 0, found.Y.Length - 1);
                            break;
                        case MatchPosition.FirstLast:
                            found.X.InsertTo(0, x, 0, found.X.Length - 1);
                            found.Y.InsertTo(0, y, 0, found.Y.Length - 1);
                            break;
                        case MatchPosition.LastFirst:
                            found.X.CopyTo(x, lengthBefore - 1);
                            found.Y.CopyTo(y, lengthBefore - 1);
                            break;
                        case MatchPosition.LastLast:
                            found.X.CopyToReverse(x, lengthBefore - 1);
                            found.Y.CopyToReverse(y, lengthBefore - 1);
                            break;
                    }

                    // select a new line.
                    foundPosition = this.FindMatch(linesIndex, lines, x, y, line.Value, epsilon, out found);
                    mergeCount++;
                }

                // simplify first.
                double[][] simplified = OsmSharp.Math.Algorithms.SimplifyCurve.Simplify(new double[][] { x, y },
                                                            epsilon);

                // add the new points.
                uint? pointsId = target.AddPoints(simplified[0], simplified[1]);

                // add points again with appropriate styles.
                if (pointsId.HasValue)
                {
                    foreach (var style in line.Value)
                    {
                        var scene2DStyleLine = (style as Scene2DStyleLine);
                        if (scene2DStyleLine != null)
                        {
                            StyleLine styleLine = source.GetStyleLine(scene2DStyleLine.StyleLineId);
                            target.AddStyleLine(pointsId.Value, styleLine.Layer, styleLine.MinZoom, styleLine.MaxZoom,
                                styleLine.Color, styleLine.Width, styleLine.LineJoin, styleLine.Dashes);
                            continue;
                        }
                        var scene2DStyleLineText = (style as Scene2DStyleLineText);
                        if (scene2DStyleLineText != null)
                        {
                            StyleText styleText = source.GetStyleLineText(scene2DStyleLineText.StyleLineTextId);
                            string text = source.GetText(scene2DStyleLineText.TextId);
                            target.AddStyleLineText(pointsId.Value, styleText.Layer, styleText.MinZoom, styleText.MaxZoom,
                                styleText.Color, styleText.Size, text, styleText.Font, styleText.HaloColor, styleText.HaloRadius);
                            continue;
                        }
                    }
                }
            }

            // loop until there are no more candidates.
            totalLines = polygons.Count;
            latestProgress = 0;
            while (polygons.Count > 0)
            {
                var polygon = polygons.First();
                polygons.Remove(polygon.Key);

                // report progress.
                float progress = (float)System.Math.Round((((double)(totalLines - polygons.Count) / (double)totalLines) * 100));
                if (progress != latestProgress)
                {
                    OsmSharp.Logging.Log.TraceEvent("SceneSerializer", OsmSharp.Logging.TraceEventType.Information,
                        "Merging polygons @z{3}e{4} ({1}/{2})... {0}%", progress, totalLines - polygons.Count, totalLines, zoomFactor, epsilon);
                    latestProgress = progress;
                }

                // copy the coordinates to lists.
                double[] x = polygon.Key.X.Clone() as double[];
                double[] y = polygon.Key.Y.Clone() as double[];

                //// find a matching line.
                //int mergeCount = 1;
                //Scene2D.ScenePoints found;
                //MatchPosition foundPosition = this.FindMatch(linesIndex, lines, x, y, line.Value, epsilon, out found);
                //while (found != null)
                //{ // TODO: keep expanding and duplicating until not possible anymore.
                //    // remove the found line.
                //    lines.Remove(found);

                //    // report progress.
                //    progress = (float)System.Math.Round((((double)(totalLines - lines.Count) / (double)totalLines) * 100));
                //    if (progress != latestProgress)
                //    {
                //        OsmSharp.Logging.Log.TraceEvent("SceneSerializer", OsmSharp.Logging.TraceEventType.Information,
                //            "Merging lines @z{3}e{4} ({1}/{2})... {0}%", progress, totalLines - lines.Count, totalLines, zoomFactor, epsilon);
                //        latestProgress = progress;
                //    }

                //    // add the line.
                //    int lengthBefore = x.Length;
                //    Array.Resize(ref x, x.Length + found.X.Length - 1);
                //    Array.Resize(ref y, y.Length + found.Y.Length - 1);

                //    switch (foundPosition)
                //    {
                //        case MatchPosition.FirstFirst:
                //            found.X.InsertToReverse(1, x, 0, found.X.Length - 1);
                //            found.Y.InsertToReverse(1, y, 0, found.Y.Length - 1);
                //            break;
                //        case MatchPosition.FirstLast:
                //            found.X.InsertTo(0, x, 0, found.X.Length - 1);
                //            found.Y.InsertTo(0, y, 0, found.Y.Length - 1);
                //            break;
                //        case MatchPosition.LastFirst:
                //            found.X.CopyTo(x, lengthBefore - 1);
                //            found.Y.CopyTo(y, lengthBefore - 1);
                //            break;
                //        case MatchPosition.LastLast:
                //            found.X.CopyToReverse(x, lengthBefore - 1);
                //            found.Y.CopyToReverse(y, lengthBefore - 1);
                //            break;
                //    }

                //    // select a new line.
                //    foundPosition = this.FindMatch(linesIndex, lines, x, y, line.Value, epsilon, out found);
                //    mergeCount++;
                //}

                // simplify first.
                double[][] simplified = OsmSharp.Math.Algorithms.SimplifyCurve.Simplify(new double[][] { x, y },
                                                            epsilon);

                // add the new points.
                uint? pointsId = target.AddPoints(simplified[0], simplified[1]);

                // add points again with appropriate styles.
                if (pointsId.HasValue)
                {
                    foreach (var style in polygon.Value)
                    {
                        var scene2DStylePolygon = (style as Scene2DStylePolygon);
                        if (scene2DStylePolygon != null)
                        {
                            StylePolygon stylePolygon = source.GetStylePolygon(scene2DStylePolygon.StylePolygonId);
                            target.AddStylePolygon(pointsId.Value, stylePolygon.Layer, stylePolygon.MinZoom, stylePolygon.MaxZoom,
                                stylePolygon.Color, stylePolygon.Width, stylePolygon.Fill);
                            continue;
                        }
                    }
                }
            }
        }

        public enum MatchPosition
        {
            None,
            FirstFirst,
            FirstLast,
            LastFirst,
            LastLast
        }

        /// <summary>
        /// Try and find matching lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private MatchPosition FindMatch(ILocatedObjectIndex<PointF2D, Scene2D.ScenePoints> linesIndex, Dictionary<Scene2D.ScenePoints, Scene2DStylesSet> lines, 
            double[] x, double[] y, Scene2DStylesSet style, float epsilon, out Scene2D.ScenePoints found)
        {
            // build box.
            var box = new BoxF2D(x, y);
            box = box.ResizeWith(epsilon * 1.1);

            // get all geometries in this box.
            var potentialMatches = linesIndex.GetInside(box);

            // find a match in the potential matches list.
            PointF2D first = new PointF2D(x[0], y[0]);
            PointF2D last = new PointF2D(x[x.Length - 1], y[y.Length - 1]);

            MatchPosition position = MatchPosition.None;
            found = null;
            foreach (var line in potentialMatches)
            {
                // check first.
                PointF2D potentialFirst = new PointF2D(line.X[0], line.Y[0]);
                PointF2D potentialLast = new PointF2D(line.X[line.X.Length - 1], line.Y[line.Y.Length - 1]);
                if (first.Distance(potentialFirst) < epsilon)
                {
                    found = line;
                    position = MatchPosition.FirstFirst;
                }
                else if (last.Distance(potentialFirst) < epsilon)
                {
                    found = line;
                    position = MatchPosition.LastFirst;
                }
                else if (first.Distance(potentialLast) < epsilon)
                {
                    found = line;
                    position = MatchPosition.FirstLast;
                }
                else if (last.Distance(potentialLast) < epsilon)
                {
                    found = line;
                    position = MatchPosition.LastLast;
                }

                Scene2DStylesSet styleValue;
                if (position != MatchPosition.None && lines.TryGetValue(line, out styleValue) && styleValue.Equals(style))
                {
                    break;
                }
                else
                {
                    position = MatchPosition.None;
                    found = null;
                }
            }
            return position;
        }
    }
}
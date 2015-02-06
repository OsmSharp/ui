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

using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Algorithms;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;
using OsmSharp.Osm.Geo.Interpreter;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.UI.Renderer.Scene;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.UI.Map.Styles.MapCSS
{
    /// <summary>
    /// Represents a MapCSS interpreter.
    /// </summary>
    public class MapCSSInterpreter : StyleInterpreter
    {
        /// <summary>
        /// Holds the MapCSS file.
        /// </summary>
        private readonly MapCSSFile _mapCSSFile;

        /// <summary>
        /// Holds the MapCSS image source.
        /// </summary>
        private readonly IMapCSSImageSource _mapCSSImageSource;

        /// <summary>
        /// Holds the geometry interpreter.
        /// </summary>
        private readonly FeatureInterpreter _geometryInterpreter;

        // defaults taken from: http://josm.openstreetmap.de/wiki/Help/Styles/MapCSSImplementation#Generalproperties
        //  area: 1, casing: 2, left-/right-casing: 2.1, line-pattern: 2.9, 
        //  line: 3, point: 4, default-point: 4.1, line-text: 4.9, point-text: 5 

        private const float OffsetArea = 1;
        private const float OffsetCasing = 2;
        private const float OffsetCasingLeftRight = 2.1f;
        private const float OffsetLinePattern = 2.9f;
        private const float OffsetLine = 3;
        private const float OffsetPoint = 4;
        private const float OffsetDefaultPoint = 4.1f;
        private const float OffsetLineText = 4.9f;
        private const float OffsetPointText = 5f;

        private HashSet<string> _keysForNodes;

        private HashSet<string> _keysForWays;

        private HashSet<string> _keysForRelations;

        private HashSet<string> _keysForAreas;

        private HashSet<string> _keysForLines;

        private HashSet<TagsCollectionBase> _unsuccesfullWays;

        private Dictionary<TagsCollectionBase, List<MapCSSRuleProperties>> _succesfullWays;

        /// <summary>
        /// Creates a new MapCSS interpreter.
        /// </summary>
        /// <param name="mapCSSFile"></param>
        /// <param name="imageSource"></param>
        public MapCSSInterpreter(MapCSSFile mapCSSFile, IMapCSSImageSource imageSource)
        {
            if (imageSource == null) throw new ArgumentNullException("imageSource");

            _mapCSSFile = mapCSSFile;
            _mapCSSImageSource = imageSource;
            _geometryInterpreter = FeatureInterpreter.DefaultInterpreter;

            this.PrepareForProcessing();
        }

        /// <summary>
        /// Creates a new MapCSS interpreter from a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="imageSource"></param>
        public MapCSSInterpreter(Stream stream, IMapCSSImageSource imageSource)
        {
            if (imageSource == null) throw new ArgumentNullException("imageSource");

            _mapCSSFile = MapCSSFile.FromStream(stream);
            _mapCSSImageSource = imageSource;
            _geometryInterpreter = FeatureInterpreter.DefaultInterpreter;

            this.PrepareForProcessing();
        }

        /// <summary>
        /// Builds the keys-for collections.
        /// </summary>
        private void PrepareForProcessing()
        {
            _keysForNodes = new HashSet<string>();
            _keysForWays = new HashSet<string>();
            _keysForRelations = new HashSet<string>();
            _keysForLines = new HashSet<string>();
            _keysForAreas = new HashSet<string>();

            if (_mapCSSFile != null && _mapCSSFile.Rules != null)
            {
                foreach (var rule in _mapCSSFile.Rules)
                {
                    foreach (var selector in rule.Selectors)
                    {
                        if(selector.SelectorRule == null)
                        { // there is no selector rule, not irrelevant tags, no short-list of relevant tags possible.
                            switch (selector.Type)
                            {
                                case SelectorTypeEnum.Node:
                                    _keysForNodes = null;
                                    break;
                                case SelectorTypeEnum.Way:
                                    _keysForWays = null;
                                    break;
                                case SelectorTypeEnum.Relation:
                                    _keysForRelations = null;
                                    break;
                                case SelectorTypeEnum.Line:
                                    _keysForLines = null;
                                    break;
                                case SelectorTypeEnum.Area:
                                    _keysForAreas = null;
                                    break;
                            }
                        }
                        else
                        { // there might be relevant tags in this selector rule.
                            switch (selector.Type)
                            {
                                case SelectorTypeEnum.Node:
                                    selector.SelectorRule.AddRelevantKeysTo(_keysForNodes);
                                    break;
                                case SelectorTypeEnum.Way:
                                    selector.SelectorRule.AddRelevantKeysTo(_keysForWays);
                                    break;
                                case SelectorTypeEnum.Relation:
                                    selector.SelectorRule.AddRelevantKeysTo(_keysForRelations);
                                    break;
                                case SelectorTypeEnum.Line:
                                    selector.SelectorRule.AddRelevantKeysTo(_keysForLines);
                                    break;
                                case SelectorTypeEnum.Area:
                                    selector.SelectorRule.AddRelevantKeysTo(_keysForAreas);
                                    break;
                            }
                        }
                    }
                }
            }

            _unsuccesfullWays = new HashSet<TagsCollectionBase>();
            _succesfullWays = new Dictionary<TagsCollectionBase, List<MapCSSRuleProperties>>();
         }

        /// <summary>
        /// Creates a new MapCSS interpreter from a string.
        /// </summary>
        /// <param name="css"></param>
        public MapCSSInterpreter(string css) : this(css, new MapCSSDictionaryImageSource()) { }


        /// <summary>
        /// Creates a new MapCSS interpreter from a string.
        /// </summary>
        /// <param name="css"></param>
        /// <param name="imageSource"></param>
        public MapCSSInterpreter(string css, IMapCSSImageSource imageSource)
        {
            if (imageSource == null) throw new ArgumentNullException("imageSource");

            _mapCSSFile = MapCSSFile.FromString(css);
            _mapCSSImageSource = imageSource;
            _geometryInterpreter = FeatureInterpreter.DefaultInterpreter;

            this.PrepareForProcessing();
        }

        /// <summary>
        /// Creates a new MapCSS interpreter.
        /// </summary>
        /// <param name="mapCSSFile"></param>
        /// <param name="imageSource"></param>
        /// <param name="geometryInterpreter"></param>
        public MapCSSInterpreter(MapCSSFile mapCSSFile, IMapCSSImageSource imageSource, FeatureInterpreter geometryInterpreter)
        {
            if (imageSource == null) throw new ArgumentNullException("imageSource");
            if (geometryInterpreter == null) throw new ArgumentNullException("geometryInterpreter");

            _mapCSSFile = mapCSSFile;
            _mapCSSImageSource = imageSource;
            _geometryInterpreter = geometryInterpreter;

            this.PrepareForProcessing();
        }

        /// <summary>
        /// Creates a new MapCSS interpreter from a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="imageSource"></param>
        /// <param name="geometryInterpreter"></param>
        public MapCSSInterpreter(Stream stream, IMapCSSImageSource imageSource, FeatureInterpreter geometryInterpreter)
        {
            if (imageSource == null) throw new ArgumentNullException("imageSource");
            if (geometryInterpreter == null) throw new ArgumentNullException("geometryInterpreter");

            _mapCSSFile = MapCSSFile.FromStream(stream);
            _mapCSSImageSource = imageSource;
            _geometryInterpreter = geometryInterpreter;

            this.PrepareForProcessing();
        }

        /// <summary>
        /// Creates a new MapCSS interpreter from a string.
        /// </summary>
        /// <param name="css"></param>
        /// <param name="imageSource"></param>
        /// <param name="geometryInterpreter"></param>
        public MapCSSInterpreter(string css, IMapCSSImageSource imageSource, FeatureInterpreter geometryInterpreter)
        {
            if (imageSource == null) throw new ArgumentNullException("imageSource");
            if (geometryInterpreter == null) throw new ArgumentNullException("geometryInterpreter");

            _mapCSSFile = MapCSSFile.FromString(css);
            _mapCSSImageSource = imageSource;
            _geometryInterpreter = geometryInterpreter;

            this.PrepareForProcessing();
        }

        /// <summary>
        /// Returns the canvas color.
        /// </summary>
        /// <returns></returns>
        public override SimpleColor GetCanvasColor()
        {
            if (_mapCSSFile != null && 
                _mapCSSFile.CanvasFillColor.HasValue)
            { // instantiate the simple color.
                return new SimpleColor()
                           {
                               Value = _mapCSSFile.CanvasFillColor.Value
                           };
            }
            return SimpleColor.FromKnownColor(KnownColor.Black);
        }

        /// <summary>
        /// Translates OSM objects into basic renderable primitives.
        /// </summary>
        /// <param name="scene">The scene to add primitives to.</param>
        /// <param name="projection">The projection used to convert the objects.</param>
        /// <param name="osmGeo">The osm object.</param>
        /// <returns></returns>
        public override void Translate(Scene2D scene, IProjection projection, ICompleteOsmGeo osmGeo)
        {
            // set the scene backcolor.
            scene.BackColor = this.GetCanvasColor().Value;

            if (osmGeo == null) { return; }

            if (_mapCSSFile == null) { return; }

            // store the object count.
            int countBefore = scene.Count;

            // interpret the osm-objects.
            switch (osmGeo.Type)
            {
                case CompleteOsmType.Node:
                    if (!_mapCSSFile.HasNodeIdSelector &&
                        osmGeo.Tags.Count == 0)
                    { // this node can never be selected, no tags and no id selectors.
                        break;
                    }
                    if (!_mapCSSFile.HasNodeIdSelector &&
                        _keysForNodes != null &&
                        !osmGeo.Tags.ContainsOneOfKeys(_keysForNodes))
                    { // no good keys present.
                        break;
                    }
                    this.TranslateNode(scene, projection, osmGeo as Node);
                    break;
                case CompleteOsmType.Way:
                    var relevantWayTags = osmGeo.Tags;
                    if (_keysForWays != null)
                    { // filter the collection.
                        relevantWayTags = relevantWayTags.KeepKeysOf(_keysForWays);
                    }
                    if (!_mapCSSFile.HasWayIdSelector &&
                        _keysForWays != null &&
                        relevantWayTags.Count == 0)
                    { // no good keys present.
                        break;
                    }
                    if (!_unsuccesfullWays.Contains(relevantWayTags))
                    { // way has some potential.
                        var completeWay = osmGeo as CompleteWay;
                        if (completeWay.Nodes.Count > 0 && !this.TranslateWay(scene, projection, completeWay, relevantWayTags))
                        { // ... but translate was unsuccesfull.
                            _unsuccesfullWays.Add(relevantWayTags);
                        }
                    }
                    break;
                case CompleteOsmType.Relation:
                    if (!_mapCSSFile.HasRelationIdSelector &&
                        _keysForRelations != null &&
                        !osmGeo.Tags.ContainsOneOfKeys(_keysForRelations))
                    { // no good keys present.
                        break;
                    }
                    this.TranslateRelation(scene, projection, osmGeo as CompleteRelation);
                    break;
            }

            // interpret the osmGeo object and check if it makes up an area.
            if (osmGeo.Type != CompleteOsmType.Node)
            { // nodes cannot lead to a geometry for MapCSS.
                if (_keysForLines == null || osmGeo.Tags.ContainsOneOfKeys(_keysForLines) ||
                    _keysForAreas == null || osmGeo.Tags.ContainsOneOfKeys(_keysForAreas))
                { // good keys present.
                    var collection = _geometryInterpreter.Interpret(osmGeo);
                    foreach (var feature in collection)
                    {
                        if (feature.Geometry is LineairRing)
                        { // a simple lineair ring.
                            this.TranslateLineairRing(scene, projection, feature);
                        }
                        else if (feature.Geometry is Polygon)
                        { // a simple polygon.
                            this.TranslatePolygon(scene, projection, feature);
                        }
                        else if (feature.Geometry is MultiPolygon)
                        { // a multipolygon.
                            this.TranslateMultiPolygon(scene, projection, feature);
                        }
                    }
                }
            }

            // check if any objects have been added to the scene.
            if (scene.Count <= countBefore)
            { // no objects have been added. Apply default styles if needed.
                if (osmGeo.Type == CompleteOsmType.Node &&
                    _mapCSSFile != null &&
                    _mapCSSFile.DefaultPoints)
                { // apply default points style.
                    Node node = (osmGeo as Node);
                    uint pointId = scene.AddPoint(projection.LongitudeToX(node.Coordinate.Longitude), projection.LatitudeToY(node.Coordinate.Latitude));
                    scene.AddStylePoint(pointId, this.CalculateSceneLayer(OffsetPoint, 0), float.MinValue, float.MaxValue,
                        SimpleColor.FromKnownColor(KnownColor.Black).Value, 2);
                }
                else if (osmGeo.Type == CompleteOsmType.Way &&
                    _mapCSSFile != null &&
                    _mapCSSFile.DefaultLines)
                { // apply default lines style.
                    CompleteWay way = (osmGeo as CompleteWay);
                    // get x/y.
                    double[] x = null, y = null;
                    if (x == null)
                    { // pre-calculate x/y.
                        x = new double[way.Nodes.Count];
                        y = new double[way.Nodes.Count];
                        for (int idx = 0; idx < way.Nodes.Count; idx++)
                        {
                            x[idx] = projection.LongitudeToX(
                                way.Nodes[idx].Coordinate.Longitude);
                            y[idx] = projection.LatitudeToY(
                                way.Nodes[idx].Coordinate.Latitude);
                        }

                        // simplify.
                        if (x.Length > 2)
                        {
                            double[][] simplified = SimplifyCurve.Simplify(new double[][] { x, y }, 0.0001);
                            x = simplified[0];
                            y = simplified[1];
                        }
                    }
                    uint? points = scene.AddPoints(x, y);
                    if (points.HasValue)
                    {
                        scene.AddStyleLine(points.Value, this.CalculateSceneLayer(OffsetLine, 0), float.MinValue, float.MaxValue,
                            SimpleColor.FromKnownColor(KnownColor.Red).Value, 1, LineJoin.Round, null);
                    }
                }
            }
        }

        /// <summary>
        /// Translates a node.
        /// </summary>
        /// <param name="scene">The scene to add primitives to.</param>
        /// <param name="projection">The projection used to convert the objects.</param>
        /// <param name="node"></param>
        private void TranslateNode(Scene2D scene, IProjection projection, Node node)
        {
            // build the rules.
            IEnumerable<MapCSSRuleProperties> rules =
                this.BuildRules(new MapCSSObject(node));

            // interpret the results.
            foreach (var rule in rules)
            {
                int zIndex;
                if (!rule.TryGetProperty<int>("zIndex", out zIndex))
                {
                    zIndex = 0;
                }

                float minZoom = (float)projection.ToZoomFactor(rule.MinZoom);
                float maxZoom = (float)projection.ToZoomFactor(rule.MaxZoom);

                uint? pointId = null;
                int color;
                if (rule.TryGetProperty<int>("color", out color))
                {
                    float width;
                    if (rule.TryGetProperty<float>("width", out width))
                    {
                        pointId = scene.AddPoint(projection.LongitudeToX(node.Coordinate.Longitude),
                                       projection.LatitudeToY(node.Coordinate.Latitude));
                        scene.AddStylePoint(pointId.Value, this.CalculateSceneLayer(OffsetPoint, zIndex), minZoom, maxZoom, color, width);
                    }
                    else
                    {
                        pointId = scene.AddPoint(projection.LongitudeToX(node.Coordinate.Longitude),
                                       projection.LatitudeToY(node.Coordinate.Latitude));
                        scene.AddStylePoint(pointId.Value, this.CalculateSceneLayer(OffsetPoint, zIndex), minZoom, maxZoom, color, 1);
                    }
                }
                byte[] iconImage;
                if (rule.TryGetProperty("iconImage", out iconImage))
                {
                    if (!pointId.HasValue)
                    {
                        pointId = scene.AddPoint(projection.LongitudeToX(node.Coordinate.Longitude),
                                       projection.LatitudeToY(node.Coordinate.Latitude));
                    }
                    // an icon is to be drawn!
                    ushort imageId = scene.AddImage(iconImage);
                    scene.AddIcon(pointId.Value, this.CalculateSceneLayer(OffsetPoint, zIndex), minZoom, maxZoom, imageId);
                }

                string text;
                if (rule.TryGetProperty("text", out text))
                {
                    int textColor;
                    if(!rule.TryGetProperty("textColor", out textColor))
                    {
                        textColor = SimpleColor.FromKnownColor(KnownColor.Black).Value;
                    }
                    int haloColor;
                    int? haloColorNullable = null;
                    if (rule.TryGetProperty("textHaloColor", out haloColor))
                    {
                        haloColorNullable = haloColor;
                    }
                    int haloRadius;
                    int? haloRadiusNullable = null;
                    if (rule.TryGetProperty("textHaloRadius", out haloRadius))
                    {
                        haloRadiusNullable = haloRadius;
                    }
                    int fontSize;
                    if (!rule.TryGetProperty("fontSize", out fontSize))
                    {
                        fontSize = 10;
                    }
					string fontFamily;
					if (!rule.TryGetProperty ("fontFamily", out fontFamily))
                    {
						fontFamily = "Arial"; // just some default font.
					}

                    // a text is to be drawn.
                    string value;
                    if (node.Tags.TryGetValue(text, out value))
                    {
                        if (!pointId.HasValue)
                        {
                            pointId = scene.AddPoint(projection.LongitudeToX(node.Coordinate.Longitude),
                                           projection.LatitudeToY(node.Coordinate.Latitude));
                        }
                        scene.AddText(pointId.Value, this.CalculateSceneLayer(OffsetPointText, zIndex), minZoom, maxZoom, fontSize, value, textColor, 
						              haloColorNullable, haloRadiusNullable, fontFamily);
                    }
                }
            }
        }

        /// <summary>
        /// Translates a way.
        /// </summary>
        /// <param name="scene">The scene to add primitives to.</param>
        /// <param name="projection">The projection used to convert the objects.</param>
        /// <param name="way"></param>
        /// <param name="relevantTags"></param>
        private bool TranslateWay(Scene2D scene, IProjection projection, CompleteWay way, TagsCollectionBase relevantTags)
        {
            // build the rules.
            List<MapCSSRuleProperties> rules = null;
            if (!_succesfullWays.TryGetValue(relevantTags, out rules))
            {
                rules = this.BuildRules(new MapCSSObject(way));
                _succesfullWays.Add(relevantTags, rules);
            }

            // validate what's there.
            if (rules.Count == 0)
            {
                return false;
            }

            bool success = false;

            // get x/y.
			double[] x = null, y = null;
            if (x == null)
            { // pre-calculate x/y.
				x = new double[way.Nodes.Count];
				y = new double[way.Nodes.Count];
                for (int idx = 0; idx < way.Nodes.Count; idx++)
                {
                    x[idx] = projection.LongitudeToX(
                        way.Nodes[idx].Coordinate.Longitude);
                    y[idx] = projection.LatitudeToY(
                        way.Nodes[idx].Coordinate.Latitude);
                }

                // simplify.
                if (x.Length > 2)
                {
                    double[][] simplified = SimplifyCurve.Simplify(new double[][] {x, y}, 0.0001);
                    x = simplified[0];
                    y = simplified[1];
                }
            }

            // add the z-index.
            foreach (var rule in rules)
            {
                float minZoom = (float)projection.ToZoomFactor(rule.MinZoom);
                float maxZoom = (float)projection.ToZoomFactor(rule.MaxZoom);

                int zIndex ;
                if (!rule.TryGetProperty<int>("zIndex", out zIndex))
                {
                    zIndex = 0;
                }

                // interpret the results.
                if (x != null &&
                    x.Length > 1)
                { // there is a valid interpretation of this way.
                    int color;
                    bool renderAsLine = true;
                    if (way.IsOfType(MapCSSTypes.Area))
                    { // the way is an area. check if it can be rendered as an area.
                        int fillColor;
                        if (rule.TryGetProperty("fillColor", out fillColor))
                        { // render as an area.
                            uint? pointsId = scene.AddPoints(x, y);
                            if (pointsId.HasValue)
                            {
                                scene.AddStylePolygon(pointsId.Value, this.CalculateSceneLayer(OffsetArea, zIndex), minZoom, maxZoom, fillColor, 1, true);
                                success = true;
                                if (rule.TryGetProperty("color", out color))
                                {
                                    scene.AddStylePolygon(pointsId.Value, this.CalculateSceneLayer(OffsetCasing, zIndex), minZoom, maxZoom, color, 1, false);
                                    success = true;
                                }
                            }
                            renderAsLine = false; // was validly rendered als a line.
                        }
                    }

                    if (renderAsLine)
                    { // was not rendered as an area.
                        // the way has to rendered as a line.
                        LineJoin lineJoin;
                        if (!rule.TryGetProperty("lineJoin", out lineJoin))
                        {
                            lineJoin = LineJoin.Miter;
                        }
                        int[] dashes;
                        if (!rule.TryGetProperty("dashes", out dashes))
                        {
                            dashes = null;
                        }
                        if (rule.TryGetProperty("color", out color))
                        {
                            float casingWidth;
                            int casingColor;
                            if (!rule.TryGetProperty("casingWidth", out casingWidth))
                            {
                                casingWidth = 0;
                            }
                            if (!rule.TryGetProperty("casingColor", out casingColor))
                            { // casing: use the casing layer.
                                casingColor = -1;
                            }
                            float width;
                            if (!rule.TryGetProperty("width", out width))
                            {
                                width = 1;
                            }
                            uint? pointsId = scene.AddPoints(x, y);
                            success = true;
                            if (pointsId.HasValue)
                            {
                                if (casingWidth > 0)
                                { // adds the casing
                                    scene.AddStyleLine(pointsId.Value, this.CalculateSceneLayer(OffsetCasing, zIndex),
                                        minZoom, maxZoom, casingColor, width + (casingWidth * 2), lineJoin, dashes);
                                }
                                if (dashes == null)
                                { // dashes not set, use line offset.
                                    scene.AddStyleLine(pointsId.Value, this.CalculateSceneLayer(OffsetLine, zIndex),
                                        minZoom, maxZoom, color, width, lineJoin, dashes);
                                }
                                else
                                { // dashes set, use line pattern offset.
                                    scene.AddStyleLine(pointsId.Value, this.CalculateSceneLayer(OffsetLinePattern, zIndex),
                                        minZoom, maxZoom, color, width, lineJoin, dashes);
                                }

                                int textColor;
                                int fontSize;
                                string nameTag;
                                if (!rule.TryGetProperty("fontSize", out fontSize))
                                {
                                    fontSize = 10;
                                }
                                if (rule.TryGetProperty("text", out nameTag) &&
                                    rule.TryGetProperty("textColor", out textColor))
                                {
                                    int haloColor;
                                    int? haloColorNullable = null;
                                    if (rule.TryGetProperty("textHaloColor", out haloColor))
                                    {
                                        haloColorNullable = haloColor;
                                    }
                                    int haloRadius;
                                    int? haloRadiusNullable = null;
                                    if (rule.TryGetProperty("textHaloRadius", out haloRadius))
                                    {
                                        haloRadiusNullable = haloRadius;
                                    }
                                    string fontFamily;
                                    if (!rule.TryGetProperty("fontFamily", out fontFamily))
                                    {
                                        fontFamily = "Arial"; // just some default font.
                                    }
                                    string name;
                                    if (way.Tags.TryGetValue(nameTag, out name))
                                    {
                                        scene.AddStyleLineText(pointsId.Value, this.CalculateSceneLayer(OffsetLineText, zIndex),
                                            minZoom, maxZoom, textColor, fontSize, name, fontFamily, haloColorNullable, haloRadiusNullable);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                { // don't report as an error when no nodes.
                    success = true;
                }
            }

            if(!success)
            { // make sure this sucess is stored.
                _succesfullWays[relevantTags] = null;
            }

            return success;
        }

        /// <summary>
        /// Translates a relation.
        /// </summary>
        /// <param name="scene">The scene to add primitives to.</param>
        /// <param name="projection">The projection used to convert the objects.</param>
        /// <param name="relation"></param>
        private void TranslateRelation(Scene2D scene, IProjection projection, CompleteRelation relation)
        {

        }

        /// <summary>
        /// Translates a multipolygon.
        /// </summary>
        /// <param name="scene">The scene to add primitives to.</param>
        /// <param name="projection">The projection used to convert the objects.</param>
        /// <param name="multiPolygon"></param>
        private void TranslateMultiPolygon(Scene2D scene, IProjection projection, Feature multiPolygon)
        {
            foreach(var polygon in multiPolygon.Geometry as MultiPolygon)
            {
                this.TranslatePolygon(scene, projection, new Feature(polygon, multiPolygon.Attributes));
            }
        }

        /// <summary>
        /// Translates a polygon.
        /// </summary>
        /// <param name="scene">The scene to add primitives to.</param>
        /// <param name="projection">The projection used to convert the objects.</param>
        /// <param name="polygon"></param>
        private void TranslatePolygon(Scene2D scene, IProjection projection, Feature polygon)
        {
            this.TranslateLineairRing(scene, projection, new Feature((polygon.Geometry as Polygon).Ring,
                polygon.Attributes));
        }

        /// <summary>
        /// Translates a lineair ring.
        /// </summary>
        /// <param name="scene">The scene to add primitives to.</param>
        /// <param name="projection">The projection used to convert the objects.</param>
        /// <param name="feature"></param>
        private void TranslateLineairRing(Scene2D scene, IProjection projection, Feature feature)
        {
            var lineairRing = feature.Geometry as LineairRing;

            // build the rules.
            var rules = this.BuildRules(new MapCSSObject(feature));

            // validate what's there.
            if (rules.Count == 0)
            {
                return;
            }

            // get x/y.
            double[] x = null, y = null;
            if (lineairRing.Coordinates != null &&
                lineairRing.Coordinates.Count > 0)
            { // pre-calculate x/y.
                x = new double[lineairRing.Coordinates.Count];
                y = new double[lineairRing.Coordinates.Count];
                for (int idx = 0; idx < lineairRing.Coordinates.Count; idx++)
                {
                    x[idx] = projection.LongitudeToX(
                        lineairRing.Coordinates[idx].Longitude);
                    y[idx] = projection.LatitudeToY(
                        lineairRing.Coordinates[idx].Latitude);
                }

                // simplify.
                if (x.Length > 2)
                {
                    double[][] simplified = SimplifyCurve.Simplify(new double[][] { x, y }, 0.0001);
                    x = simplified[0];
                    y = simplified[1];
                }
            }
            // add the z-index.
            foreach (var rule in rules)
            {
                float minZoom = (float)projection.ToZoomFactor(rule.MinZoom);
                float maxZoom = (float)projection.ToZoomFactor(rule.MaxZoom);

                int zIndex;
                if (!rule.TryGetProperty<int>("zIndex", out zIndex))
                {
                    zIndex = 0;
                }

                // interpret the results.
                if (x != null)
                { // there is a valid interpretation of this way.
                    int color;
                    int fillColor;
                    if (rule.TryGetProperty("fillColor", out fillColor))
                    { // render as an area.
                        float fillOpacity;
                        if(rule.TryGetProperty("fillOpacity", out fillOpacity))
                        {
                            SimpleColor simpleFillColor = new SimpleColor() { Value = fillColor };
                            fillColor = SimpleColor.FromArgb((int)(255 * fillOpacity), 
                                simpleFillColor.R, simpleFillColor.G, simpleFillColor.B).Value;
                        }
                        uint? pointsId = scene.AddPoints(x, y);
                        if (pointsId.HasValue)
                        {
                            scene.AddStylePolygon(pointsId.Value, this.CalculateSceneLayer(OffsetArea, zIndex), minZoom, maxZoom, fillColor, 1, true);
                            if (rule.TryGetProperty("color", out color))
                            {
                                scene.AddStylePolygon(pointsId.Value, this.CalculateSceneLayer(OffsetCasing, zIndex), minZoom, maxZoom, color, 1, false);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the scene layer.
        /// </summary>
        /// <param name="majorZIndex">The major parameter.</param>
        /// <param name="zIndex">The minor parameters.</param>
        /// <returns></returns>
        private uint CalculateSceneLayer(float majorZIndex, float zIndex)
        {
            return (uint)(majorZIndex * 1000000 + zIndex * 1000);
        }

        #region Build Rules

        /// <summary>
        /// Build the property collection.
        /// </summary>
        /// <param name="mapCSSObject"></param>
        /// <returns></returns>
        private List<MapCSSRuleProperties> BuildRules(MapCSSObject mapCSSObject)
        {
            var rulesCollection = new MapCSSRulePropertiesCollection();

            // check for a file/rules.
            if (_mapCSSFile == null ||
                _mapCSSFile.Rules == null)
            { // no rules exist.
                return new List<MapCSSRuleProperties>();
            }

            // interpret all rules on-by-one.
            foreach (var rule in _mapCSSFile.Rules)
            {
                List<SelectorZoom> zooms;
                if (rule.HasToBeAppliedTo(mapCSSObject, out zooms))
                { // the selector was ok.
                    // loop over all declarations.
                    var properties = new MapCSSRuleProperties();
                    foreach (var declaration in rule.Declarations)
                    {
                        if (declaration is DeclarationInt)
                        {
                            var declarationInt = (declaration as DeclarationInt);
                            switch (declarationInt.Qualifier)
                            {
                                case DeclarationIntEnum.FillColor:
                                    properties.AddProperty("fillColor", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.ZIndex:
                                    properties.AddProperty("zIndex", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.Color:
                                    properties.AddProperty("color", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.CasingColor:
                                    properties.AddProperty("casingColor", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.Extrude:
                                    properties.AddProperty("extrude", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.ExtrudeEdgeColor:
                                    properties.AddProperty("extrudeEdgeColor", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.ExtrudeFaceColor:
                                    properties.AddProperty("extrudeFaceColor", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.IconWidth:
                                    properties.AddProperty("iconWidth", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.IconHeight:
                                    properties.AddProperty("iconHeight", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.FontSize:
                                    properties.AddProperty("fontSize", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.TextColor:
                                    properties.AddProperty("textColor", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.TextOffset:
                                    properties.AddProperty("textOffset", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.MaxWidth:
                                    properties.AddProperty("maxWidth", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.TextHaloColor:
                                    properties.AddProperty("textHaloColor", declarationInt.Eval(mapCSSObject));
                                    break;
                                case DeclarationIntEnum.TextHaloRadius:
                                    properties.AddProperty("textHaloRadius", declarationInt.Eval(mapCSSObject));
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else if (declaration is DeclarationFloat)
                        {
                            var declarationFloat = (declaration as DeclarationFloat);
                            switch (declarationFloat.Qualifier)
                            {
                                case DeclarationFloatEnum.Width:
                                    properties.AddProperty("width", declarationFloat.Eval(mapCSSObject));
                                    break;
                                case DeclarationFloatEnum.FillOpacity:
                                    properties.AddProperty("fillOpacity", declarationFloat.Eval(mapCSSObject));
                                    break;
                                case DeclarationFloatEnum.Opacity:
                                    properties.AddProperty("opacity", declarationFloat.Eval(mapCSSObject));
                                    break;
                                case DeclarationFloatEnum.CasingOpacity:
                                    properties.AddProperty("casingOpacity", declarationFloat.Eval(mapCSSObject));
                                    break;
                                case DeclarationFloatEnum.ExtrudeEdgeOpacity:
                                    properties.AddProperty("extrudeEdgeOpacity", declarationFloat.Eval(mapCSSObject));
                                    break;
                                case DeclarationFloatEnum.ExtrudeFaceOpacity:
                                    properties.AddProperty("extrudeFaceOpacity", declarationFloat.Eval(mapCSSObject));
                                    break;
                                case DeclarationFloatEnum.ExtrudeEdgeWidth:
                                    properties.AddProperty("extrudeEdgeWidth", declarationFloat.Eval(mapCSSObject));
                                    break;
                                case DeclarationFloatEnum.IconOpacity:
                                    properties.AddProperty("iconOpacity", declarationFloat.Eval(mapCSSObject));
                                    break;
                                case DeclarationFloatEnum.TextOpacity:
                                    properties.AddProperty("textOpacity", declarationFloat.Eval(mapCSSObject));
                                    break;
                                case DeclarationFloatEnum.CasingWidth:
                                    properties.AddProperty("casingWidth", declarationFloat.Eval(mapCSSObject));
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else if (declaration is DeclarationString)
                        {
                            var declarationString = declaration as DeclarationString;
                            switch (declarationString.Qualifier)
                            {
                                case DeclarationStringEnum.FontFamily:
                                    properties.AddProperty("fontFamily", declarationString.Eval(mapCSSObject));
                                    break;
                                case DeclarationStringEnum.Text:
                                    properties.AddProperty("text", declarationString.Eval(mapCSSObject));
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else if (declaration is DeclarationURL)
                        {
                            byte[] image;
                            var declarationURL = declaration as DeclarationURL;
                            switch (declarationURL.Qualifier)
                            {
                                case DeclarationURLEnum.Image:
                                    if (_mapCSSImageSource.TryGet(declarationURL.Eval(mapCSSObject), out image))
                                    {
                                        properties.AddProperty("image", image);
                                    }
                                    break;
                                case DeclarationURLEnum.FillImage:
                                    if (_mapCSSImageSource.TryGet(declarationURL.Eval(mapCSSObject), out image))
                                    {
                                        properties.AddProperty("fillImage", image);
                                    }
                                    break;
                                case DeclarationURLEnum.IconImage:
                                    if (_mapCSSImageSource.TryGet(declarationURL.Eval(mapCSSObject), out image))
                                    {
                                        properties.AddProperty("iconImage", image);
                                    }
                                    break;
                                case DeclarationURLEnum.ShieldImage:
                                    if (_mapCSSImageSource.TryGet(declarationURL.Eval(mapCSSObject), out image))
                                    {
                                        properties.AddProperty("shieldImage", image);
                                    }
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else if (declaration is DeclarationLineJoin)
                        {
                            var declarationLineJoin = (declaration as DeclarationLineJoin);
                            properties.AddProperty("lineJoin", declarationLineJoin.Eval(
                                mapCSSObject));
                        }
                        else if (declaration is DeclarationDashes)
                        {
                            var declarationDashes = (declaration as DeclarationDashes);
                            properties.AddProperty("dashes", declarationDashes.Eval(
                                mapCSSObject));
                        }
                    }

                    // loop over all zoom levels.
                    foreach (var zoom in zooms)
                    {
                        int minZoom = 0;
                        int maxZoom = 25;
                        if (zoom != null)
                        { // there is a zoom object.
                            if (zoom.ZoomMin.HasValue)
                            {
                                minZoom = zoom.ZoomMin.Value;
                            }
                            if (zoom.ZoomMax.HasValue)
                            {
                                maxZoom = zoom.ZoomMax.Value;
                            }
                        }

                        // zoom properties;
                        var zoomRule = new MapCSSRuleProperties(
                            minZoom, maxZoom);
                        zoomRule = zoomRule.Merge(properties);

                        // add the properties.
                        rulesCollection.AddProperties(zoomRule);
                    }
                }
            }
            return rulesCollection.GetRanges();
        }

        #endregion

        /// <summary>
        /// Tests if the given object is usefull in this style.
        /// </summary>
        /// <param name="osmGeo"></param>
        /// <returns></returns>
        public override bool AppliesTo(OsmGeo osmGeo)
        {
            if (osmGeo == null) { return false; }

            // interpret the osm-objects.
            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    return this.AppliesToNode(osmGeo as Node);
                case OsmGeoType.Way:
                    if (this.AppliesToWay(osmGeo as Way))
                    { // this could possibly apply.
                        return true;
                    }

                    // test also if it might apply as an area.
                    if (_geometryInterpreter.IsPotentiallyArea(osmGeo.Tags))
                    {
                        return this.AppliesToArea(osmGeo.Tags);
                    }
                    return false;
                case OsmGeoType.Relation:
                    if (this.AppliesToRelation(osmGeo as Relation))
                    { // this could possibly apply.
                        return true;
                    }

                    // test also if it might apply as an area.
                    if (_geometryInterpreter.IsPotentiallyArea(osmGeo.Tags))
                    {
                        return this.AppliesToArea(osmGeo.Tags);
                    }
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns false if this mapcss interpreter does not contain an interpretation for a way with the given tags.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        private bool AppliesToWay(Way way)
        {
            List<MapCSSRuleProperties> rules = this.BuildRules(new MapCSSObject(way));
            return rules != null && rules.Count > 0;
        }

        /// <summary>
        /// Returns false if this mapcss interpreter does not contain an interpretation for a relation with the given tags.
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        private bool AppliesToRelation(Relation relation)
        {
            List<MapCSSRuleProperties> rules = this.BuildRules(new MapCSSObject(relation));
            return rules != null && rules.Count > 0;
        }

        /// <summary>
        /// Returns false if this mapcss interpreter does not contain an interpretation for an area with the given tags.
        /// </summary>
        /// <param name="tagsCollection"></param>
        /// <returns></returns>
        private bool AppliesToArea(TagsCollectionBase tagsCollection)
        {
            var feature = new Feature(new LineairRing(),
                new SimpleGeometryAttributeCollection(tagsCollection));
            var rules = this.BuildRules(new MapCSSObject(feature));
            return rules != null && rules.Count > 0;
        }

        /// <summary>
        /// Returns false if this mapcss interpreter does not contain an interpretation for a node with the given tags.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool AppliesToNode(Node node)
        {
            List<MapCSSRuleProperties> rules = this.BuildRules(new MapCSSObject(node));
            return rules != null && rules.Count > 0;
        }
    }
}
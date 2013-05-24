using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;
using OsmSharp;
using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene2DPrimitives;

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
        /// Holds the fill layer offset.
        /// </summary>
        private const int FillLayerOffset = 100;

        /// <summary>
        /// Holds the casing layer offset.
        /// </summary>
        private const int CasingLayerOffset = 200;

        /// <summary>
        /// Holds the stroke layer offset.
        /// </summary>
        private const int StrokeLayerOffset = 300;

        /// <summary>
        /// Holds the icon/test layer offset.
        /// </summary>
        private const int IconTextLayerOffset = 400;


        /// <summary>
        /// Creates a new MapCSS interpreter.
        /// </summary>
        /// <param name="mapCSSFile"></param>
        /// <param name="imageSource"></param>
        public MapCSSInterpreter(MapCSSFile mapCSSFile, IMapCSSImageSource imageSource)
        {
            _mapCSSFile = mapCSSFile;
            _mapCSSImageSource = imageSource;
        }

        /// <summary>
        /// Creates a new MapCSS interpreter from a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="imageSource"></param>
        public MapCSSInterpreter(Stream stream, IMapCSSImageSource imageSource)
        {
            _mapCSSFile = MapCSSFile.FromStream(stream);
            _mapCSSImageSource = imageSource;
        }

        /// <summary>
        /// Returns the canvas color.
        /// </summary>
        /// <returns></returns>
        public override SimpleColor? GetCanvasColor()
        {
            if (_mapCSSFile.CanvasFillColor.HasValue)
            {
                return new SimpleColor()
                           {
                               Value = _mapCSSFile.CanvasFillColor.Value
                           };
            }
            return null;
        }

        /// <summary>
        /// Translates OSM objects into basic renderable primitives.
        /// </summary>
        /// <param name="projection">The projection to use.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="osmGeo">The osm object.</param>
        /// <param name="scene">The scene to fill with the resulting geometries.</param>
        /// <returns></returns>
        public override void Translate(Scene2D scene, IProjection projection, float zoom, OsmGeo osmGeo)
        {
            switch (osmGeo.Type)
            {
                case OsmType.Node:
                    this.TranslateNode(scene, projection, osmGeo as Node);
                    break;
                case OsmType.Way:
                    this.TranslateWay(scene, projection, osmGeo as Way);
                    break;
                case OsmType.Relation:
                    break;
                case OsmType.ChangeSet:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Translates a node.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="projection"></param>
        /// <param name="node"></param>
        private void TranslateNode(Scene2D scene, IProjection projection, Node node)
        {
            float? x = (float) projection.LongitudeToX(
                node.Coordinate.Longitude);
            float? y = (float) projection.LatitudeToY(
                node.Coordinate.Latitude);
            

            // build the rules.
            IEnumerable<MapCSSRuleProperties> rules =
                this.BuildRules(node);

            // validate what's there.

            // calculate the layer to render on if any.
            int sceneLayer = 0;
            double? osmLayer = node.Tags.GetNumericValue("layer");
            if (osmLayer.HasValue)
            { // multiply by 100, allow another 100 sub divisions for each OSM layer.
                sceneLayer = (int) osmLayer*100;
            }

            // interpret the results.
            foreach (var rule in rules)
            {
                float minZoom = (float)projection.ToZoomFactor(rule.MinZoom);
                float maxZoom = (float)projection.ToZoomFactor(rule.MaxZoom);

                int color;
                if (rule.TryGetProperty<int>("color", out color))
                {
                    float width;
                    if (rule.TryGetProperty<float>("width", out width))
                    {
                        sceneLayer = sceneLayer + IconTextLayerOffset;
                        scene.AddPoint(sceneLayer, minZoom, maxZoom, (float) projection.LongitudeToX(node.Coordinate.Longitude),
                                       (float) projection.LatitudeToY(node.Coordinate.Latitude),
                                       color, width);
                        sceneLayer = sceneLayer - IconTextLayerOffset;
                    }
                    else
                    {
                        sceneLayer = sceneLayer + IconTextLayerOffset;
                        scene.AddPoint(sceneLayer, minZoom, maxZoom, (float)projection.LongitudeToX(node.Coordinate.Longitude),
                                       (float) projection.LatitudeToY(node.Coordinate.Latitude),
                                       color, 1);
                        sceneLayer = sceneLayer - IconTextLayerOffset;
                    }
                }
                byte[] iconImage;
                if (rule.TryGetProperty("iconImage", out iconImage))
                {
                    // an icon is to be drawn!
                    sceneLayer = sceneLayer + IconTextLayerOffset; // offset to correct layer.

                    scene.AddIcon(sceneLayer, minZoom, maxZoom, (float)projection.LongitudeToX(node.Coordinate.Longitude),
                                  (float) projection.LatitudeToY(node.Coordinate.Latitude),
                                  iconImage);

                    sceneLayer = sceneLayer - IconTextLayerOffset; // offset to correct layer.
                }

                string text;
                if (rule.TryGetProperty("text", out text))
                {
                    // a text is to be drawn.
                    string value;
                    if (node.Tags.TryGetValue(text, out value))
                    {
                        sceneLayer = sceneLayer + IconTextLayerOffset; // offset to correct layer.

                        scene.AddText(sceneLayer, minZoom, maxZoom, (float)projection.LongitudeToX(node.Coordinate.Longitude),
                                      (float) projection.LatitudeToY(node.Coordinate.Latitude), 15, value);

                        sceneLayer = sceneLayer - IconTextLayerOffset; // offset to correct layer.
                    }
                }
            }
        }

        /// <summary>
        /// Translates a way.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="projection"></param>
        /// <param name="way"></param>
        private void TranslateWay(Scene2D scene, IProjection projection, Way way)
        {
            // build the rules.
            List<MapCSSRuleProperties> rules =
                this.BuildRules(way);

            // validate what's there.
            if (rules.Count == 0)
            {
                return;
            }

			double[] x = null, y = null;

            // get x/y.
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
                    double[][] simplified = SimplifyCurve.Simplify(new double[][] {x, y}, 0.01);
                    x = simplified[0];
                    y = simplified[1];
                }
            }

            // add the z-index.
            foreach (var rule in rules)
            {
                // calculate the layer to render on if any.
                int sceneLayer = 0;
                double? osmLayer = way.Tags.GetNumericValue("layer");
                if (osmLayer.HasValue)
                { // multiply by 100, allow another 100 sub divisions for each OSM layer.
                    sceneLayer = (int)osmLayer * 1000;
                }

                float minZoom = (float)projection.ToZoomFactor(rule.MinZoom);
                float maxZoom = (float)projection.ToZoomFactor(rule.MaxZoom);

                int zIndex;
                if (rule.TryGetProperty<int>("zIndex", out zIndex))
                {
                    sceneLayer = sceneLayer + zIndex * 10;
                }

                // interpret the results.
                if (x != null)
                { // there is a valid interpretation of this way.
                    int color;
                    if (way.IsOfType(MapCSSTypes.Area))
                    { // the way is an area. check if it can be rendered as an area.
                        int fillColor;
                        if (rule.TryGetProperty("fillColor", out fillColor))
                        { // render as an area.
                            sceneLayer = sceneLayer + FillLayerOffset;
                            scene.AddPolygon(sceneLayer - 1, minZoom, maxZoom, x, y, fillColor, 1, true);
                            sceneLayer = sceneLayer - FillLayerOffset;
                            if (rule.TryGetProperty("color", out color))
                            {
                                sceneLayer = sceneLayer + CasingLayerOffset;
                                scene.AddPolygon(sceneLayer, minZoom, maxZoom, x, y, color, 1, false);
                                sceneLayer = sceneLayer - CasingLayerOffset;
                            }
                        }
                    }

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
                        sceneLayer = sceneLayer + StrokeLayerOffset;
                        float width;
                        if (!rule.TryGetProperty("width", out width))
                        {
                            width = 1;
                        }
                        scene.AddLine(sceneLayer, minZoom, maxZoom, x, y, color, width, lineJoin, dashes);
                        float casingWidth;
                        int casingColor;
                        if (rule.TryGetProperty("casingWidth", out casingWidth) && 
                            rule.TryGetProperty("casingColor", out casingColor))
                        {
                            scene.AddLine(sceneLayer - 1, minZoom, maxZoom, x, y, casingColor, width + (2 * casingWidth), lineJoin, dashes);
                        }
                        sceneLayer = sceneLayer - StrokeLayerOffset;
                    }
                }
            }
        }

        /// <summary>
        /// Build the property collection.
        /// </summary>
        /// <param name="osmGeo"></param>
        /// <returns></returns>
        private List<MapCSSRuleProperties> BuildRules(OsmGeo osmGeo)
        {
            var rulesCollection = new MapCSSRulePropertiesCollection();

            // interpret all rules on-by-one.
            foreach (var rule in _mapCSSFile.Rules)
            {
                List<KeyValuePair<int?, int?>> zooms;
                if (rule.HasToBeAppliedTo(osmGeo, out zooms))
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
                                    properties.AddProperty("fillColor", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.ZIndex:
                                    properties.AddProperty("zIndex", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.Color:
                                    properties.AddProperty("color", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.CasingColor:
                                    properties.AddProperty("casingColor", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.Extrude:
                                    properties.AddProperty("extrude", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.ExtrudeEdgeColor:
                                    properties.AddProperty("extrudeEdgeColor", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.ExtrudeFaceColor:
                                    properties.AddProperty("extrudeFaceColor", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.IconWidth:
                                    properties.AddProperty("iconWidth", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.IconHeight:
                                    properties.AddProperty("iconHeight", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.FontSize:
                                    properties.AddProperty("fontSize", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.TextColor:
                                    properties.AddProperty("textColor", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.TextOffset:
                                    properties.AddProperty("textOffset", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.MaxWidth:
                                    properties.AddProperty("maxWidth", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.TextHaloColor:
                                    properties.AddProperty("textHaloColor", declarationInt.Value);
                                    break;
                                case DeclarationIntEnum.TextHaloRadius:
                                    properties.AddProperty("textHaloRadius", declarationInt.Value);
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
                                    properties.AddProperty("width", declarationFloat.Value);
                                    break;
                                case DeclarationFloatEnum.FillOpacity:
                                    properties.AddProperty("fillOpacity", declarationFloat.Value);
                                    break;
                                case DeclarationFloatEnum.Opacity:
                                    properties.AddProperty("opacity", declarationFloat.Value);
                                    break;
                                case DeclarationFloatEnum.CasingOpacity:
                                    properties.AddProperty("casingOpacity", declarationFloat.Value);
                                    break;
                                case DeclarationFloatEnum.ExtrudeEdgeOpacity:
                                    properties.AddProperty("extrudeEdgeOpacity", declarationFloat.Value);
                                    break;
                                case DeclarationFloatEnum.ExtrudeFaceOpacity:
                                    properties.AddProperty("extrudeFaceOpacity", declarationFloat.Value);
                                    break;
                                case DeclarationFloatEnum.ExtrudeEdgeWidth:
                                    properties.AddProperty("extrudeEdgeWidth", declarationFloat.Value);
                                    break;
                                case DeclarationFloatEnum.IconOpacity:
                                    properties.AddProperty("iconOpacity", declarationFloat.Value);
                                    break;
                                case DeclarationFloatEnum.TextOpacity:
                                    properties.AddProperty("textOpacity", declarationFloat.Value);
                                    break;
                                case DeclarationFloatEnum.CasingWidth:
                                    properties.AddProperty("casingWidth", declarationFloat.Value);
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
                                    properties.AddProperty("fontFamily", declarationString.Value);
                                    break;
                                case DeclarationStringEnum.Text:
                                    properties.AddProperty("text", declarationString.Value);
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
                                    if (_mapCSSImageSource.TryGet(declarationURL.Value, out image))
                                    {
                                        properties.AddProperty("image", image);
                                    }
                                    break;
                                case DeclarationURLEnum.FillImage:
                                    if (_mapCSSImageSource.TryGet(declarationURL.Value, out image))
                                    {
                                        properties.AddProperty("fillImage", image);
                                    }
                                    break;
                                case DeclarationURLEnum.IconImage:
                                    if (_mapCSSImageSource.TryGet(declarationURL.Value, out image))
                                    {
                                        properties.AddProperty("iconImage", image);
                                    }
                                    break;
                                case DeclarationURLEnum.ShieldImage:
                                    if (_mapCSSImageSource.TryGet(declarationURL.Value, out image))
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
                            properties.AddProperty("lineJoin", declarationLineJoin.Value);
                        }
                        else if (declaration is DeclarationDashes)
                        {
                            var declarationDashes = (declaration as DeclarationDashes);
                            properties.AddProperty("dashes", declarationDashes.Value);
                        }
                    }

                    // loop over all zoom levels.
                    foreach (var keyValuePair in zooms)
                    {
                        int minZoom = 0;
                        if (keyValuePair.Key.HasValue)
                        {
                            minZoom = keyValuePair.Key.Value;
                        }
                        int maxZoom = 25;
                        if (keyValuePair.Value.HasValue)
                        {
                            maxZoom = keyValuePair.Value.Value;
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
    }
}
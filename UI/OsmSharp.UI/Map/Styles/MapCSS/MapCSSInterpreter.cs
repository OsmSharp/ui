using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OsmSharp.Osm;
using OsmSharp.Tools;
using OsmSharp.Tools.Collections;
using OsmSharp.Tools.Collections.Tags;
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
        private const int FillLayerOffset = 10;

        /// <summary>
        /// Holds the casing layer offset.
        /// </summary>
        private const int CasingLayerOffset = 20;

        /// <summary>
        /// Holds the stroke layer offset.
        /// </summary>
        private const int StrokeLayerOffset = 30;

        /// <summary>
        /// Holds the icon/test layer offset.
        /// </summary>
        private const int IconTextLayerOffset = 40;


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
                    this.TranslateNode(scene, projection, zoom, osmGeo as Node);
                    break;
                case OsmType.Way:
                    this.TranslateWay(scene, projection, zoom, osmGeo as Way);
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
        /// <param name="zoom"></param>
        /// <param name="node"></param>
        private void TranslateNode(Scene2D scene, IProjection projection, float zoom, Node node)
        {
            int? color = null;
            int? fillColor = null;
            int? zIndex = null;
            int? casingColor = null;
            int? extrude = null;
            int? extrudeEdgeColor = null;
            int? extrudeFaceColor = null;
            int? iconWidth = null;
            int? iconHeight = null;
            int? fontSize = null;
            int? textColor = null;
            int? textOffset = null;
            int? maxWidth = null;
            int? textHaloColor = null;
            int? textHaloRadius = null;

            float? width = null;
            float? fillOpacity = null;
            float? opacity = null;
            float? casingOpacity = null;
            float? extrudeEdgeOpacity = null;
            float? extrudeFaceOpacity = null;
            float? extrudeEdgeWidth = null;
            float? iconOpacity = null;
            float? textOpacity = null;

            string text = null;
            string fontFamily = null;

            float? x = null, y = null;
            byte[] iconImage = null;
            byte[] image = null;
            byte[] fillImage = null;
            byte[] shieldImage = null;

            // interpret all rules on-by-one.
            foreach (var rule in _mapCSSFile.Rules)
            {
                if (rule.HasToBeAppliedTo((int)zoom, node))
                {
                    // get x/y.
                    if (!x.HasValue)
                    { // pre-calculate x/y.
                        x = (float)projection.LongitudeToX(node.Coordinate.Longitude);
                        y = (float)projection.LatitudeToY(node.Coordinate.Latitude);
                    }

                    foreach (var declaration in rule.Declarations)
                    {
                        if (declaration is DeclarationInt)
                        {
                            var declarationInt = (declaration as DeclarationInt);
                            switch (declarationInt.Qualifier)
                            {
                                case DeclarationIntEnum.FillColor:
                                    fillColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.ZIndex:
                                    zIndex = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.Color:
                                    color = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.CasingColor:
                                    casingColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.Extrude:
                                    extrude = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.ExtrudeEdgeColor:
                                    extrudeEdgeColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.ExtrudeFaceColor:
                                    extrudeFaceColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.IconWidth:
                                    iconWidth = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.IconHeight:
                                    iconHeight = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.FontSize:
                                    fontSize = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.TextColor:
                                    textColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.TextOffset:
                                    textOffset = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.MaxWidth:
                                    maxWidth = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.TextHaloColor:
                                    textHaloColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.TextHaloRadius:
                                    textHaloRadius = declarationInt.Value;
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
                                    width = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.FillOpacity:
                                    width = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.Opacity:
                                    opacity = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.CasingOpacity:
                                    casingOpacity = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.ExtrudeEdgeOpacity:
                                    extrudeEdgeOpacity = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.ExtrudeFaceOpacity:
                                    extrudeFaceOpacity = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.ExtrudeEdgeWidth:
                                    extrudeEdgeWidth = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.IconOpacity:
                                    iconOpacity = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.TextOpacity:
                                    textOpacity = declarationFloat.Value;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else if (declaration is DeclarationString)
                        {
                            DeclarationString declarationString = declaration as DeclarationString;
                            switch (declarationString.Qualifier)
                            {
                                case DeclarationStringEnum.FontFamily:
                                    fontFamily = declarationString.Value;
                                    break;
                                case DeclarationStringEnum.Text:
                                    text = declarationString.Value;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else if (declaration is DeclarationURL)
                        {
                            DeclarationURL declarationURL = declaration as DeclarationURL;
                            switch (declarationURL.Qualifier)
                            {
                                case DeclarationURLEnum.Image:
                                    _mapCSSImageSource.TryGet(declarationURL.Value, out image);
                                    break;
                                case DeclarationURLEnum.FillImage:
                                    _mapCSSImageSource.TryGet(declarationURL.Value, out fillImage);
                                    break;
                                case DeclarationURLEnum.IconImage:
                                    _mapCSSImageSource.TryGet(declarationURL.Value, out iconImage);
                                    break;
                                case DeclarationURLEnum.ShieldImage:
                                    _mapCSSImageSource.TryGet(declarationURL.Value, out shieldImage);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
            }

            // validate what's there.

            // calculate the layer to render on if any.
            int sceneLayer = 0;
            double? osmLayer = node.Tags.GetNumericValue("layer");
            if (osmLayer.HasValue)
            { // multiply by 100, allow another 100 sub divisions for each OSM layer.
                sceneLayer = (int) osmLayer*100;
            }

            // interpret the results.
            if (color.HasValue)
            {
                if (width.HasValue)
                {
                    sceneLayer = sceneLayer + IconTextLayerOffset;
                    scene.AddPoint(sceneLayer, (float)projection.LongitudeToX(node.Coordinate.Longitude),
                                   (float)projection.LatitudeToY(node.Coordinate.Latitude),
                                   color.Value, width.Value);
                    sceneLayer = sceneLayer - IconTextLayerOffset;
                }
                else
                {
                    sceneLayer = sceneLayer + IconTextLayerOffset;
                    scene.AddPoint(sceneLayer, (float)projection.LongitudeToX(node.Coordinate.Longitude),
                                   (float)projection.LatitudeToY(node.Coordinate.Latitude),
                                   color.Value, 1);
                    sceneLayer = sceneLayer - IconTextLayerOffset;
                }
            }
            if (iconImage != null)
            { // an icon is to be drawn!
                sceneLayer = sceneLayer + IconTextLayerOffset; // offset to correct layer.

                scene.AddIcon(sceneLayer, (float) projection.LongitudeToX(node.Coordinate.Longitude),
                              (float) projection.LatitudeToY(node.Coordinate.Latitude),
                              iconImage);

                sceneLayer = sceneLayer - IconTextLayerOffset; // offset to correct layer.
            }

            if (text != null)
            { // a text is to be drawn.
                string value;
                if (node.Tags.TryGetValue(text, out value))
                {
                    sceneLayer = sceneLayer + IconTextLayerOffset; // offset to correct layer.

                    scene.AddText(sceneLayer, (float) projection.LongitudeToX(node.Coordinate.Longitude),
                                  (float) projection.LatitudeToY(node.Coordinate.Latitude), 15, value);

                    sceneLayer = sceneLayer - IconTextLayerOffset; // offset to correct layer.
                }
            }
        }

        /// <summary>
        /// Translates a way.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="projection"></param>
        /// <param name="zoom"></param>
        /// <param name="way"></param>
        private void TranslateWay(Scene2D scene, IProjection projection, float zoom, Way way)
        {
            int? color = null;
            int? fillColor = null;
            int? zIndex = null;
            int? casingColor = null;
            int? extrude = null;
            int? extrudeEdgeColor = null;
            int? extrudeFaceColor = null;
            int? iconWidth = null;
            int? iconHeight = null;
            int? fontSize = null;
            int? textColor = null;
            int? textOffset = null;
            int? maxWidth = null;
            int? textHaloColor = null;
            int? textHaloRadius = null;

            float? width = null;
            float? fillOpacity = null;
            float? opacity = null;
            float? casingOpacity = null;
            float? extrudeEdgeOpacity = null;
            float? extrudeFaceOpacity = null;
            float? extrudeEdgeWidth = null;
            float? iconOpacity = null;
            float? textOpacity = null;
            float? casingWidth = null;

            LineJoin lineJoin = LineJoin.None;
            int[] dashes = null;

            float[] x = null, y = null;

            // interpret all rules on-by-one.
            foreach (var rule in _mapCSSFile.Rules)
            {
                if (rule.HasToBeAppliedTo((int)zoom, way))
                {
                    // get x/y.
                    if (x == null)
                    { // pre-calculate x/y.
                        x = new float[way.Nodes.Count];
                        y = new float[way.Nodes.Count];
                        for (int idx = 0; idx < way.Nodes.Count; idx++)
                        {
                            x[idx] = (float)projection.LongitudeToX(way.Nodes[idx].Coordinate.Longitude);
                            y[idx] = (float)projection.LatitudeToY(way.Nodes[idx].Coordinate.Latitude);
                        }
                    }

                    foreach (var declaration in rule.Declarations)
                    {
                        if (declaration is DeclarationInt)
                        {
                            var declarationInt = (declaration as DeclarationInt);
                            switch (declarationInt.Qualifier)
                            {
                                case DeclarationIntEnum.FillColor:
                                    fillColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.ZIndex:
                                    zIndex = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.Color:
                                    color = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.CasingColor:
                                    casingColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.Extrude:
                                    extrude = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.ExtrudeEdgeColor:
                                    extrudeEdgeColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.ExtrudeFaceColor:
                                    extrudeFaceColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.IconWidth:
                                    iconWidth = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.IconHeight:
                                    iconHeight = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.FontSize:
                                    fontSize = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.TextColor:
                                    textColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.TextOffset:
                                    textOffset = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.MaxWidth:
                                    maxWidth = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.TextHaloColor:
                                    textHaloColor = declarationInt.Value;
                                    break;
                                case DeclarationIntEnum.TextHaloRadius:
                                    textHaloRadius = declarationInt.Value;
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
                                    width = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.FillOpacity:
                                    width = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.Opacity:
                                    opacity = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.CasingOpacity:
                                    casingOpacity = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.ExtrudeEdgeOpacity:
                                    extrudeEdgeOpacity = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.ExtrudeFaceOpacity:
                                    extrudeFaceOpacity = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.ExtrudeEdgeWidth:
                                    extrudeEdgeWidth = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.IconOpacity:
                                    iconOpacity = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.TextOpacity:
                                    textOpacity = declarationFloat.Value;
                                    break;
                                case DeclarationFloatEnum.CasingWidth:
                                    casingWidth = declarationFloat.Value;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else if (declaration is DeclarationLineJoin)
                        {
                            var declarationLineJoin = (declaration as DeclarationLineJoin);
                            switch (declarationLineJoin.Value)
                            {
                                case LineJoinEnum.Round:
                                    lineJoin = LineJoin.Miter;
                                    break;
                                case LineJoinEnum.Miter:
                                    lineJoin = LineJoin.Miter;
                                    break;
                                case LineJoinEnum.Bevel:
                                    lineJoin = LineJoin.Bevel;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else if (declaration is DeclarationDashes)
                        {
                            var declarationDashes = (declaration as DeclarationDashes);
                            dashes = declarationDashes.Value;
                        }
                    }
                }
            }

            // validate what's there.

            // calculate the layer to render on if any.
            int sceneLayer = 0;
            double? osmLayer = way.Tags.GetNumericValue("layer");
            if (osmLayer.HasValue)
            { // multiply by 100, allow another 100 sub divisions for each OSM layer.
                sceneLayer = (int)osmLayer * 100;
            }

            // add the z-index.
            if (zIndex.HasValue)
            {
                sceneLayer = sceneLayer + zIndex.Value;
            }

            // interpret the results.
            if (x != null)
            { // there is a valid interpretation of this way.
                if (way.IsOfType(MapCSSTypes.Area))
                { // the way is an area. check if it can be rendered as an area.
                    if (fillColor.HasValue)
                    { // render as an area.
                        sceneLayer = sceneLayer + FillLayerOffset;
                        scene.AddPolygon(sceneLayer, x, y, fillColor.Value, 1, true);
                        sceneLayer = sceneLayer - FillLayerOffset;
                        if (color.HasValue)
                        {
                            sceneLayer = sceneLayer + CasingLayerOffset;
                            scene.AddPolygon(sceneLayer, x, y, color.Value, 1, false);
                            sceneLayer = sceneLayer - CasingLayerOffset;
                        }
                    }
                }

                // the way has to rendered as a line.
                if (color.HasValue)
                {
                    sceneLayer = sceneLayer + StrokeLayerOffset;
                    if (!width.HasValue)
                    {
                        width = 1;
                    }
                    scene.AddLine(sceneLayer, x, y, color.Value, width.Value, lineJoin, dashes);
                    if (casingWidth.HasValue && casingColor.HasValue)
                    {
                        scene.AddLine(sceneLayer - 1, x, y, casingColor.Value, width.Value + (2 * casingWidth.Value), lineJoin, dashes);
                    }
                    sceneLayer = sceneLayer - StrokeLayerOffset;
                    return;
                }
            }
        }
    }
}
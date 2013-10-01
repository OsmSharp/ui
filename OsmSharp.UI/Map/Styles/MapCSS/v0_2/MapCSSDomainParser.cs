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
using System.Globalization;
using System.Text;
using Antlr.Runtime.Tree;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2
{
    /// <summary>
    /// Parses the result of the MapCSSParser into domain objects, verifying more of the CSS in the process.
    /// </summary>
    public class MapCSSDomainParser
    {
        /// <summary>
        /// Parses the result of the MapCSSParser into the MapCSSFile domain object.
        /// </summary>
        /// <param name="treeObject"></param>
        /// <returns></returns>
        public static MapCSSFile Parse(object treeObject)
        {
            var tree = (treeObject as Antlr.Runtime.Tree.CommonTree);

            if (tree == null)
            {
                throw new ArgumentOutOfRangeException("treeObject", 
                    "Incorrect type: Antlr.Runtime.Tree.CommonTree expected!");
            }
            if (tree.ChildCount > 0)
            {
                // create the mapcss file.
                var file = new MapCSSFile();
                
                // loop over all children.
                foreach (CommonTree child in tree.Children)
                {
                    if (child.Text == "RULE")
                    {
                        if (child.ChildCount == 2 &&
                            child.Children[0].Text == "SIMPLE_SELECTOR" &&
                            child.Children[0].ChildCount == 1 &&
                            child.Children[0] is CommonTree &&
                            (child.Children[0] as CommonTree).Children[0].Text == "canvas")
                        { // this child represents the canvas rule.
                            MapCSSDomainParser.ParseCanvasRule(file, child as CommonTree);
                        }
                        else if (child.ChildCount == 2 &&
                            child.Children[0].Text == "SIMPLE_SELECTOR" &&
                            child.Children[0].ChildCount == 1 &&
                            child.Children[0] is CommonTree &&
                            (child.Children[0] as CommonTree).Children[0].Text == "meta")
                        { // this child represents the canvas rule.
                            MapCSSDomainParser.ParseMetaRule(file, child as CommonTree);
                        }
                        else
                        { // this child can only be a regular rule.
                            Rule rule =
                                MapCSSDomainParser.ParseRule(child as CommonTree);
                            file.Rules.Add(rule);
                        }
                    }
                }

                return file;
            }
            return null;
        }

        /// <summary>
        /// Parses the canvas rule.
        /// </summary>
        /// <param name="ruleTree"></param>
        /// <returns></returns>
        private static void ParseCanvasRule(MapCSSFile file, CommonTree ruleTree)
        {
            if (ruleTree.ChildCount >= 2 &&
                ruleTree.Children[1] != null)
            { // loop over all declaration rules in canvas.
                foreach (CommonTree rule in (ruleTree.Children[1] as CommonTree).Children)
                {
                    if (rule.Text == "DECLARATION" &&
                        rule.Children != null &&
                        rule.Children.Count > 0)
                    { // parse the decalaration.
                        // support both JOSM's background-color and fill-color.
                        if (rule.Children[0].Text == "background-color")
                        { // parse the background color.
                            file.CanvasFillColor = MapCSSDomainParser.ParseColor(rule.Children[1] as CommonTree);
                        }
                        else if (rule.Children[0].Text == "fill-color")
                        { // parse the background color.
                            file.CanvasFillColor = MapCSSDomainParser.ParseColor(rule.Children[1] as CommonTree);
                        }
                        else if (rule.Children[0].Text == "default-points")
                        { // parse the default points-setting.
                            file.DefaultPoints = false;
                            if(rule.Children[1].Text == "true")
                            {
                                file.DefaultPoints = true;
                            }
                        }
                        else if (rule.Children[0].Text == "default-lines")
                        { // parse the default lines-setting.
                            file.DefaultLines = false;
                            if (rule.Children[1].Text == "true")
                            {
                                file.DefaultLines = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses the meta rule.
        /// </summary>
        /// <param name="ruleTree"></param>
        /// <returns></returns>
        private static void ParseMetaRule(MapCSSFile file, CommonTree ruleTree)
        {
            if (ruleTree.ChildCount >= 2 &&
                ruleTree.Children[1] != null)
            { // loop over all declaration rules in canvas.
                foreach (CommonTree rule in (ruleTree.Children[1] as CommonTree).Children)
                {
                    if (rule.Text == "DECLARATION" &&
                        rule.Children != null &&
                        rule.Children.Count > 0)
                    { // parse the decalaration.
                        if (rule.Children[0].Text == "title")
                        { // parse the background color.
                            file.Title = MapCSSDomainParser.ParseURL(rule.Children[1].Text);
                        }
                        else if (rule.Children[0].Text == "icon")
                        { // parse the default points-setting.
                            file.Icon = MapCSSDomainParser.ParseURL(rule.Children[1].Text);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses one rule.
        /// </summary>
        /// <param name="ruleTree"></param>
        /// <returns></returns>
        private static Rule ParseRule(CommonTree ruleTree)
        {
            // create the rule.
            var rule = new Rule();

            // parse the selector(s).
            rule.Selectors = new List<Selector>();
            int simpleSelectorIdx = 0;
            while (ruleTree.Children[simpleSelectorIdx].Text == "SIMPLE_SELECTOR")
            { // see what this SIMPLE_SELECTOR is all about.
                var simpleSelectorTree = ruleTree.Children[simpleSelectorIdx] as CommonTree;
                if (simpleSelectorTree != null)
                { // the simple selector tree exists.
                    int childIdx = 0;
                    var selector = new Selector();

                    if (simpleSelectorTree.Children[0].Text == "way")
                    { // the way.
                        childIdx++; // this selector seems ok.

                        selector.Type = SelectorTypeEnum.Way;
                        rule.Selectors.Add(selector);
                    }
                    else if (simpleSelectorTree.Children[0].Text == "node")
                    { // the node.
                        childIdx++; // this selector seems ok.

                        selector.Type = SelectorTypeEnum.Node;
                        rule.Selectors.Add(selector);
                    }
                    else if (simpleSelectorTree.Children[0].Text == "relation")
                    { // the node.
                        childIdx++; // this selector seems ok.

                        selector.Type = SelectorTypeEnum.Relation;
                        rule.Selectors.Add(selector);
                    }
                    else if (simpleSelectorTree.Children[0].Text == "area")
                    { // the node.
                        childIdx++; // this selector seems ok.

                        selector.Type = SelectorTypeEnum.Area;
                        rule.Selectors.Add(selector);
                    }
                    else
                    {
                        // oeps, this is not possible.
                        throw new MapCSSDomainParserException(simpleSelectorTree,
                                                              "Simple selector type not found!");
                    }

                    // parse the zoom selector if any.
                    if (simpleSelectorTree.ChildCount > 1 &&
                        simpleSelectorTree.Children[1].Text == "ZOOM_SELECTOR")
                    { // there is a zoom selector.
                        childIdx++; // this selector seems ok.

                        var zoomSelector = 
                            simpleSelectorTree.Children[1] as CommonTree;
                        if (zoomSelector != null && 
                            zoomSelector.Children != null &&
                            zoomSelector.Children.Count > 0)
                        { // the zoom selector.
                            var zooms = new List<string>();
                            foreach (var child in zoomSelector.Children)
                            {
                                // try and parse the zoom levels.
                                zooms.Add(child.Text);
                            }

                            if (zooms.Count == 2)
                            { // just two zoom selectors.
                                var selectorZoom = new SelectorZoom();
                                int zoom;
                                // parse zoom 1.
                                if (string.IsNullOrWhiteSpace(zooms[0]))
                                { // minzoom is zero.
                                    selectorZoom.ZoomMin = null;
                                }
                                else if (int.TryParse(zooms[0], NumberStyles.Any,
                                                      System.Globalization.CultureInfo.InvariantCulture,
                                                      out zoom))
                                { // minzoom was set!
                                    selectorZoom.ZoomMin = zoom;
                                }
                                else
                                {
                                    // oeps, this is not possible.
                                    throw new MapCSSDomainParserException(zoomSelector,
                                                                          "Zoom selector invalid!");
                                }
                                // parse zoom 2.
                                if (string.IsNullOrWhiteSpace(zooms[1]))
                                { // minzoom is zero.
                                    selectorZoom.ZoomMax = null;
                                }
                                else if (int.TryParse(zooms[1], NumberStyles.Any,
                                                      System.Globalization.CultureInfo.InvariantCulture,
                                                      out zoom))
                                { // minzoom was set!
                                    selectorZoom.ZoomMax = zoom;
                                }
                                else
                                {
                                    // oeps, this is not possible.
                                    throw new MapCSSDomainParserException(zoomSelector,
                                                                          "Zoom selector invalid!");
                                }

                                // add zoom selector.
                                selector.Zoom = selectorZoom;
                            }
                            else
                            {
                                // oeps, this is not possible.
                                throw new MapCSSDomainParserException(zoomSelector,
                                                                      "Zoom selector invalid!");
                            }
                        }
                    }

                    // parse the rest of the selectors.
                    for (int selectorIdx = childIdx; selectorIdx < simpleSelectorTree.ChildCount; 
                        selectorIdx++)
                    {
                        var nextSelector =
                            simpleSelectorTree.Children[selectorIdx] as CommonTree;
                        if (nextSelector != null &&
                            nextSelector.Text == "ATTRIBUTE_SELECTOR")
                        { // parse attribute selectors.
                            var attributeSelector = nextSelector as CommonTree;

                            if (attributeSelector.Children[0].Text == "OP_EXIST")
                            {
                                // the exists selector.
                                var opExistsRule = new SelectorRuleTag();

                                if (attributeSelector.ChildCount < 2)
                                {
                                    // oeps, this is not possible.
                                    throw new MapCSSDomainParserException(attributeSelector,
                                                                          "OP_EXIST without tag value!");
                                }
                                opExistsRule.Tag = attributeSelector.Children[1].Text;

                                // add the tags.
                                if (selector.SelectorRule == null)
                                {
                                    selector.SelectorRule = opExistsRule;
                                }
                                else
                                {
                                    selector.SelectorRule = selector.SelectorRule & opExistsRule;
                                }
                            }
                            else if (attributeSelector.Children[0].Text == "<" ||
                                attributeSelector.Children[0].Text == ">" ||
                                attributeSelector.Children[0].Text == "=" ||
                                attributeSelector.Children[0].Text == "!=")
                            {
                                // the exists selector.
                                var selectorRuleTagValueComparison = new SelectorRuleTagValueComparison();

                                if (attributeSelector.ChildCount < 3)
                                {
                                    // oeps, this is not possible.
                                    throw new MapCSSDomainParserException(attributeSelector,
                                                                          "Tag selector without tag/key value!");
                                }
                                switch (attributeSelector.Children[0].Text)
                                {
                                    case ">":
                                        selectorRuleTagValueComparison.Comparator = 
                                            SelectorRuleTagValueComparison.SelectorRuleTagValueComparisonEnum.GreaterThan;
                                        break;
                                    case "<":
                                        selectorRuleTagValueComparison.Comparator =
                                            SelectorRuleTagValueComparison.SelectorRuleTagValueComparisonEnum.GreaterThan;
                                        break;
                                    case "=":
                                        selectorRuleTagValueComparison.Comparator =
                                            SelectorRuleTagValueComparison.SelectorRuleTagValueComparisonEnum.Equal;
                                        break;
                                    case "!=":
                                        selectorRuleTagValueComparison.Comparator =
                                            SelectorRuleTagValueComparison.SelectorRuleTagValueComparisonEnum.NotEqual;
                                        break;
                                    default:
                                        // oeps, this is not possible.
                                        throw new MapCSSDomainParserException(attributeSelector,
                                                                              string.Format("{0} not found as comparator",
                                                                                attributeSelector.Children[0].Text));
                                }
                                selectorRuleTagValueComparison.Tag = attributeSelector.Children[1].Text;
                                selectorRuleTagValueComparison.Value = attributeSelector.Children[2].Text;

                                // add the tags.
                                if (selector.SelectorRule == null)
                                {
                                    selector.SelectorRule = selectorRuleTagValueComparison;
                                }
                                else
                                {
                                    selector.SelectorRule = selector.SelectorRule & selectorRuleTagValueComparison;
                                }
                            }
                            else
                            {
                                // oeps, this is not possible.
                                throw new MapCSSDomainParserException(attributeSelector,
                                                                      "Attibute selector not found!");
                            }
                        }
                    }
                }

                // move to the next selector.
                simpleSelectorIdx++;
            }

            // parse the declarations.
            var commonTree = ruleTree.Children[simpleSelectorIdx] as CommonTree;
            if (commonTree != null &&
                commonTree.Text == "{")
            { // the declaration commonTree is found.
                rule.Declarations = new List<Declaration>();
                int declarationSelectorIdx = 0;
                while (commonTree.ChildCount > declarationSelectorIdx && 
                    commonTree.Children[declarationSelectorIdx].Text == "DECLARATION")
                { // keep looping over the declarations.
                    var declarationTree = commonTree.Children[declarationSelectorIdx] as CommonTree;

                    if (declarationTree == null)
                    { // declaration tree is null.
                        // oeps, this is not possible.
                        throw new MapCSSDomainParserException(declarationTree,
                                                                "Declaration tree not valid!");
                    }

                    string qualifierString = declarationTree.Children[0].Text;
                    string valueString = declarationTree.Children[1].Text;

                    int valueInt;
                    float valueFloat;
//                    string[] strings;
                    string evalCall = null;

                    // skip all eval calls.
                    if (valueString == "EVAL_CALL") // TODO: implement the eval calls and change Antlr grammar.
                    { // skip to next declaration.
                        evalCall = (declarationTree.Children[1] as CommonTree).Children[0].Text;
                    }

                    switch (qualifierString)
                    {
                        case "text-position":
                            var textPosition = new DeclarationTextPosition();
                            textPosition.Qualifier = DeclarationTextPositionEnum.TextPosition;
                            if (evalCall != null)
                            {
                                textPosition.EvalFunction = evalCall;
                            }
                            else
                            {
                                switch (valueString)
                                {
                                    case "line":
                                        textPosition.Value = TextPositionEnum.Line;
                                        break;
                                    case "center":
                                        textPosition.Value = TextPositionEnum.Center;
                                        break;
                                    default:
                                        throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(textPosition);
                            break;
                        case "text-halo-radius":
                            var textHaloRaduis = new DeclarationInt();
                            textHaloRaduis.Qualifier = DeclarationIntEnum.TextHaloRadius;
                            if (evalCall != null)
                            {
                                textHaloRaduis.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (int.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueInt))
                                {
                                    textHaloRaduis.Value = valueInt;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(textHaloRaduis);
                            break;
                        case "antialiasing":
                            var antialiasing = new DeclarationAntiAliasing();
                            antialiasing.Qualifier = QualifierAntiAliasingEnum.AntiAliasing;
                            if (evalCall != null)
                            {
                                antialiasing.EvalFunction = evalCall;
                            }
                            else
                            {
                                switch (valueString)
                                {
                                    case "full":
                                        antialiasing.Value = AntiAliasingEnum.Full;
                                        break;
                                    case "text":
                                        antialiasing.Value = AntiAliasingEnum.Text;
                                        break;
                                    case "none":
                                        antialiasing.Value = AntiAliasingEnum.None;
                                        break;
                                    default:
                                        throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(antialiasing);
                            break;
                        case "linecap":
                            var linecap = new DeclarationLineCap();
                            linecap.Qualifier = QualifierLineCapEnum.LineCap;
                            if (evalCall != null)
                            {
                                linecap.EvalFunction = evalCall;
                            }
                            else
                            {
                                switch (valueString)
                                {
                                    case "none":
                                        linecap.Value = LineCapEnum.None;
                                        break;
                                    case "round":
                                        linecap.Value = LineCapEnum.Round;
                                        break;
                                    case "square":
                                        linecap.Value = LineCapEnum.Square;
                                        break;
                                    case "butt":
                                        linecap.Value = LineCapEnum.Butt;
                                        break;
                                    default:
                                        throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(linecap);
                            break;
                        case "casing-linecap":
                            var casingLinecap = new DeclarationLineCap();
                            casingLinecap.Qualifier = QualifierLineCapEnum.CasingLineCap;
                            if (evalCall != null)
                            {
                                casingLinecap.EvalFunction = evalCall;
                            }
                            else
                            {
                                switch (valueString)
                                {
                                    case "none":
                                        casingLinecap.Value = LineCapEnum.None;
                                        break;
                                    case "round":
                                        casingLinecap.Value = LineCapEnum.Round;
                                        break;
                                    case "square":
                                        casingLinecap.Value = LineCapEnum.Square;
                                        break;
                                    case "butt":
                                        casingLinecap.Value = LineCapEnum.Butt;
                                        break;
                                    default:
                                        throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(casingLinecap);
                            break;
                        case "linejoin":
                            var linejoin = new DeclarationLineJoin();
                            linejoin.Qualifier = QualifierLineJoinEnum.LineJoin;
                            if (evalCall != null)
                            {
                                linejoin.EvalFunction = evalCall;
                            }
                            else
                            {
                                switch (valueString)
                                {
                                    case "round":
                                        linejoin.Value = LineJoinEnum.Round;
                                        break;
                                    case "miter":
                                        linejoin.Value = LineJoinEnum.Miter;
                                        break;
                                    case "bevel":
                                        linejoin.Value = LineJoinEnum.Bevel;
                                        break;
                                    default:
                                        throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(linejoin);
                            break;
                        case "font-weight":
                            var fontWeight = new DeclarationFontWeight();
                            fontWeight.Qualifier = QualifierFontWeightEnum.FontWeight;
                            if (evalCall != null)
                            {
                                fontWeight.EvalFunction = evalCall;
                            }
                            else
                            {
                                switch (valueString)
                                {
                                    case "bold":
                                        fontWeight.Value = FontWeightEnum.Bold;
                                        break;
                                    case "normal":
                                        fontWeight.Value = FontWeightEnum.Normal;
                                        break;
                                    default:
                                        throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(fontWeight);
                            break;
                        case "font-style":
                            var fontStyle = new DeclarationFontStyle();
                            fontStyle.Qualifier = QualifierFontStyleEnum.FontStyle;
                            if (evalCall != null)
                            {
                                fontStyle.EvalFunction = evalCall;
                            }
                            else
                            {
                                switch (valueString)
                                {
                                    case "italic":
                                        fontStyle.Value = FontStyleEnum.Italic;
                                        break;
                                    case "normal":
                                        fontStyle.Value = FontStyleEnum.Normal;
                                        break;
                                    default:
                                        throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(fontStyle);
                            break;
                        case "font-variant":
                            var fontVariant = new DeclarationFontVariant();
                            fontVariant.Qualifier = QualifierFontVariantEnum.FontVariant;
                            if (evalCall != null)
                            {
                                fontVariant.EvalFunction = evalCall;
                            }
                            else
                            {
                                switch (valueString)
                                {
                                    case "small-caps":
                                        fontVariant.Value = FontVariantEnum.SmallCaps;
                                        break;
                                    case "normal":
                                        fontVariant.Value = FontVariantEnum.Normal;
                                        break;
                                    default:
                                        throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(fontVariant);
                            break;
                        case "text-decoration":
                            var textDecoration = new DeclarationTextDecoration();
                            textDecoration.Qualifier = QualifierTextDecorationEnum.TextDecoration;
                            if (evalCall != null)
                            {
                                textDecoration.EvalFunction = evalCall;
                            }
                            else
                            {
                                switch (valueString)
                                {
                                    case "none":
                                        textDecoration.Value = TextDecorationEnum.None;
                                        break;
                                    case "underline":
                                        textDecoration.Value = TextDecorationEnum.Underlined;
                                        break;
                                    default:
                                        throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(textDecoration);
                            break;
                        case "text-transform":
                            var textTransform = new DeclarationTextTransform();
                            textTransform.Qualifier = QualifierTextTransformEnum.TextTransform;
                            if (evalCall != null)
                            {
                                textTransform.EvalFunction = evalCall;
                            }
                            else
                            {
                                switch (valueString)
                                {
                                    case "none":
                                        textTransform.Value = TextTransformEnum.None;
                                        break;
                                    case "uppercase":
                                        textTransform.Value = TextTransformEnum.Uppercase;
                                        break;
                                    case "lowercase":
                                        textTransform.Value = TextTransformEnum.Lowercase;
                                        break;
                                    case "capitalize":
                                        textTransform.Value = TextTransformEnum.Capitalize;
                                        break;
                                    default:
                                        throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(textTransform);
                            break;
                        case "casing-linejoin":
                            var casingLinejoin = new DeclarationLineJoin();
                            casingLinejoin.Qualifier = QualifierLineJoinEnum.CasingLineJoin;
                            if (evalCall != null)
                            {
                                casingLinejoin.EvalFunction = evalCall;
                            }
                            else
                            {
                                switch (valueString)
                                {
                                    case "round":
                                        casingLinejoin.Value = LineJoinEnum.Round;
                                        break;
                                    case "miter":
                                        casingLinejoin.Value = LineJoinEnum.Miter;
                                        break;
                                    case "bevel":
                                        casingLinejoin.Value = LineJoinEnum.Bevel;
                                        break;
                                    default:
                                        throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(casingLinejoin);
                            break;
                        case "fill-color":
                            var fillColor = new DeclarationInt();
                            fillColor.Qualifier = DeclarationIntEnum.FillColor;
                            if (evalCall != null)
                            {
                                fillColor.EvalFunction = evalCall;
                            }
                            else
                            {
                                fillColor.Value = MapCSSDomainParser.ParseColor(
                                    declarationTree.Children[1] as CommonTree);
                            }

                            // add declaration.
                            rule.Declarations.Add(fillColor);
                            break;
                        case "text-color":
                            var textColor = new DeclarationInt();
                            textColor.Qualifier = DeclarationIntEnum.TextColor;
                            if (evalCall != null)
                            {
                                textColor.EvalFunction = evalCall;
                            }
                            else
                            {
                                textColor.Value = MapCSSDomainParser.ParseColor(
                                    declarationTree.Children[1] as CommonTree);
                            }

                            // add declaration.
                            rule.Declarations.Add(textColor);
                            break;
                        case "font-size":
                            var fontSize = new DeclarationInt();
                            fontSize.Qualifier = DeclarationIntEnum.FontSize;
                            if (evalCall != null)
                            {
                                fontSize.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (int.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueInt))
                                {
                                    fontSize.Value = valueInt;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(fontSize);
                            break;
                        case "text-offset":
                            var textOffset = new DeclarationInt();
                            textOffset.Qualifier = DeclarationIntEnum.TextOffset;
                            if (evalCall != null)
                            {
                                textOffset.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (int.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueInt))
                                {
                                    textOffset.Value = valueInt;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(textOffset);
                            break;
                        case "max-width":
                            var maxWidth = new DeclarationInt();
                            maxWidth.Qualifier = DeclarationIntEnum.MaxWidth;
                            if (evalCall != null)
                            {
                                maxWidth.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (int.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueInt))
                                {
                                    maxWidth.Value = valueInt;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(maxWidth);
                            break;
                        case "fill-opacity":
                            var fillOpacity = new DeclarationFloat();
                            fillOpacity.Qualifier = DeclarationFloatEnum.FillOpacity;
                            if (evalCall != null)
                            {
                                fillOpacity.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (float.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueFloat))
                                {
                                    fillOpacity.Value = valueFloat;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(fillOpacity);
                            break;
                        case "text-opacity":
                            var textOpacity = new DeclarationFloat();
                            textOpacity.Qualifier = DeclarationFloatEnum.TextOpacity;
                            if (evalCall != null)
                            {
                                textOpacity.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (float.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueFloat))
                                {
                                    textOpacity.Value = valueFloat;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(textOpacity);
                            break;
                        case "casing-opacity":
                            var casingOpacity = new DeclarationFloat();
                            casingOpacity.Qualifier = DeclarationFloatEnum.CasingOpacity;
                            if (evalCall != null)
                            {
                                casingOpacity.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (float.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueFloat))
                                {
                                    casingOpacity.Value = valueFloat;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(casingOpacity);
                            break;
                        case "extrude-edge-opacity":
                            var extrudeEdgeOpacity = new DeclarationFloat();
                            extrudeEdgeOpacity.Qualifier = DeclarationFloatEnum.ExtrudeEdgeOpacity;
                            if (evalCall != null)
                            {
                                extrudeEdgeOpacity.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (float.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueFloat))
                                {
                                    extrudeEdgeOpacity.Value = valueFloat;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(extrudeEdgeOpacity);
                            break;
                        case "extrude-face-opacity":
                            var extrudeFaceOpacity = new DeclarationFloat();
                            extrudeFaceOpacity.Qualifier = DeclarationFloatEnum.ExtrudeFaceOpacity;
                            if (evalCall != null)
                            {
                                extrudeFaceOpacity.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (float.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueFloat))
                                {
                                    extrudeFaceOpacity.Value = valueFloat;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(extrudeFaceOpacity);
                            break;
                        case "extrude-edge-width":
                            var extrudeEdgeWidth = new DeclarationFloat();
                            extrudeEdgeWidth.Qualifier = DeclarationFloatEnum.Width;
                            if (evalCall != null)
                            {
                                extrudeEdgeWidth.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (float.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueFloat))
                                {
                                    extrudeEdgeWidth.Value = valueFloat;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(extrudeEdgeWidth);
                            break;
                        case "icon-opacity":
                            var iconOpacity = new DeclarationFloat();
                            iconOpacity.Qualifier = DeclarationFloatEnum.IconOpacity;
                            if (evalCall != null)
                            {
                                iconOpacity.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (float.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueFloat))
                                {
                                    iconOpacity.Value = valueFloat;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(iconOpacity);
                            break;
                        case "fill-image":
                            var fillImage = new DeclarationURL();
                            fillImage.Qualifier = DeclarationURLEnum.FillImage;
                            if (evalCall != null)
                            {
                                fillImage.EvalFunction = evalCall;
                            }
                            else
                            {
                                fillImage.Value = MapCSSDomainParser.ParseURL(valueString);
                            }

                            // add declaration.
                            rule.Declarations.Add(fillImage);
                            break;
                        case "icon-image":
                            var iconImage = new DeclarationURL();
                            iconImage.Qualifier = DeclarationURLEnum.IconImage;
                            if (evalCall != null)
                            {
                                iconImage.EvalFunction = evalCall;
                            }
                            else
                            {
                                iconImage.Value = MapCSSDomainParser.ParseURL(valueString);
                            }

                            // add declaration.
                            rule.Declarations.Add(iconImage);
                            break;
                        case "image":
                            var image = new DeclarationURL();
                            image.Qualifier = DeclarationURLEnum.Image;
                            if (evalCall != null)
                            {
                                image.EvalFunction = evalCall;
                            }
                            else
                            {
                                image.Value = MapCSSDomainParser.ParseURL(valueString);
                            }

                            // add declaration.
                            rule.Declarations.Add(image);
                            break;
                        case "z-index":
                            var zIndex = new DeclarationInt();
                            zIndex.Qualifier = DeclarationIntEnum.ZIndex;
                            if (evalCall != null)
                            {
                                zIndex.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (int.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueInt))
                                {
                                    zIndex.Value = valueInt;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(zIndex);
                            break;
                        case "extrude":
                            var extrude = new DeclarationInt();
                            extrude.Qualifier = DeclarationIntEnum.Extrude;
                            if (evalCall != null)
                            {
                                extrude.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (int.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueInt))
                                {
                                    extrude.Value = valueInt;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(extrude);
                            break;
                        case "extrude-edge-color":
                            var extrudeEdgeColor = new DeclarationInt();
                            extrudeEdgeColor.Qualifier = DeclarationIntEnum.ExtrudeEdgeColor;
                            if (evalCall != null)
                            {
                                extrudeEdgeColor.EvalFunction = evalCall;
                            }
                            else
                            {
                                extrudeEdgeColor.Value = MapCSSDomainParser.ParseColor(
                                    declarationTree.Children[1] as CommonTree);
                            }

                            // add declaration.
                            rule.Declarations.Add(extrudeEdgeColor);
                            break;
                        case "extrude-face-color":
                            var extrudeFaceColor = new DeclarationInt();
                            extrudeFaceColor.Qualifier = DeclarationIntEnum.ExtrudeFaceColor;
                            if (evalCall != null)
                            {
                                extrudeFaceColor.EvalFunction = evalCall;
                            }
                            else
                            {
                                extrudeFaceColor.Value = MapCSSDomainParser.ParseColor(
                                    declarationTree.Children[1] as CommonTree);
                            }

                            // add declaration.
                            rule.Declarations.Add(extrudeFaceColor);
                            break;
                        case "width":
                            var width = new DeclarationFloat();
                            width.Qualifier = DeclarationFloatEnum.Width;
                            if (evalCall != null)
                            {
                                width.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (float.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueFloat))
                                {
                                    width.Value = valueFloat;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(width);
                            break;
                        case "icon-width":
                            var iconWidth = new DeclarationInt();
                            iconWidth.Qualifier = DeclarationIntEnum.IconWidth;
                            if (evalCall != null)
                            {
                                iconWidth.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (int.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueInt))
                                {
                                    iconWidth.Value = valueInt;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(iconWidth);
                            break;
                        case "icon-height":
                            var iconHeight = new DeclarationInt();
                            iconHeight.Qualifier = DeclarationIntEnum.IconHeight;
                            if (evalCall != null)
                            {
                                iconHeight.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (int.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueInt))
                                {
                                    iconHeight.Value = valueInt;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(iconHeight);
                            break;
                        case "casing-width":
                            var casingWidth = new DeclarationFloat();
                            casingWidth.Qualifier = DeclarationFloatEnum.CasingWidth;
                            if (evalCall != null)
                            {
                                casingWidth.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (float.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueFloat))
                                {
                                    casingWidth.Value = valueFloat;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(casingWidth);
                            break;
                        case "casing-color":
                            var casingColor = new DeclarationInt();
                            casingColor.Qualifier = DeclarationIntEnum.CasingColor;
                            if (evalCall != null)
                            {
                                casingColor.EvalFunction = evalCall;
                            }
                            else
                            {
                                casingColor.Value = MapCSSDomainParser.ParseColor(
                                    declarationTree.Children[1] as CommonTree);
                            }

                            // add declaration.
                            rule.Declarations.Add(casingColor);
                            break;
                        case "text-halo-color":
                            var textHaloColor = new DeclarationInt();
                            textHaloColor.Qualifier = DeclarationIntEnum.TextHaloColor;
                            if (evalCall != null)
                            {
                                textHaloColor.EvalFunction = evalCall;
                            }
                            else
                            {
                                textHaloColor.Value = MapCSSDomainParser.ParseColor(
                                    declarationTree.Children[1] as CommonTree);
                            }

                            // add declaration.
                            rule.Declarations.Add(textHaloColor);
                            break;
                        case "color":
                            var color = new DeclarationInt();
                            color.Qualifier = DeclarationIntEnum.Color;
                            if (evalCall != null)
                            {
                                color.EvalFunction = evalCall;
                            }
                            else
                            {
                                color.Value = MapCSSDomainParser.ParseColor(
                                    declarationTree.Children[1] as CommonTree);
                            }

                            // add declaration.
                            rule.Declarations.Add(color);
                            break;
                        case "opacity":
                            var opacity = new DeclarationFloat();
                            opacity.Qualifier = DeclarationFloatEnum.Opacity;
                            if (evalCall != null)
                            {
                                opacity.EvalFunction = evalCall;
                            }
                            else
                            {
                                if (float.TryParse(valueString, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueFloat))
                                {
                                    opacity.Value = valueFloat;
                                }
                                else
                                { // value could not be parsed.
                                    throw new MapCSSDomainParserException(declarationTree,
                                                                                string.Format("{1} value {0} cannot be parsed!", valueString, qualifierString));
                                }
                            }

                            // add declaration.
                            rule.Declarations.Add(opacity);
                            break;
                        case "dashes":
                            var dashes = new DeclarationDashes();
                            dashes.Qualifier = DeclarationDashesEnum.Dashes;
                            if (evalCall != null)
                            {
                                dashes.EvalFunction = evalCall;
                            }
                            else
                            {
                                dashes.Value = MapCSSDomainParser.ParseDashes(declarationTree.Children[1]);
                            }

                            // add declaration.
                            rule.Declarations.Add(dashes);
                            break;
                        case "casing-dashes":
                            var casingDashes = new DeclarationDashes();
                            casingDashes.Qualifier = DeclarationDashesEnum.CasingDashes;
                            if (evalCall != null)
                            {
                                casingDashes.EvalFunction = evalCall;
                            }
                            else
                            {
                                casingDashes.Value = MapCSSDomainParser.ParseDashes(declarationTree.Children[1]);
                            }

                            // add declaration.
                            rule.Declarations.Add(casingDashes);
                            break;
                        case "font-family":
                            var fontFamily = new DeclarationString();
                            fontFamily.Qualifier = DeclarationStringEnum.FontFamily;
                            if (evalCall != null)
                            {
                                fontFamily.EvalFunction = evalCall;
                            }
                            else
                            {
                                fontFamily.Value = valueString;
                            }

                            // add declaration.
                            rule.Declarations.Add(fontFamily);
                            break;
                        case "text":
                            var text = new DeclarationString();
                            text.Qualifier = DeclarationStringEnum.Text;
                            if (evalCall != null)
                            {
                                text.EvalFunction = evalCall;
                            }
                            else
                            {
                                text.Value = valueString;
                            }

                            // add declaration.
                            rule.Declarations.Add(text);
                            break;
                        default:
                            var declarationCustom = new DeclarationCustom();
                            declarationCustom.Qualifier = qualifierString;
                            if (evalCall != null)
                            {
                                declarationCustom.EvalFunction = evalCall;
                                rule.Declarations.Add(declarationCustom);
                            }
                            else
                            {
                                throw new MapCSSDomainParserException(declarationTree,
                                                                        string.Format("{0} qualifier cannot be parsed!", qualifierString));
                            }
                            break;
                    }

                    // move to next declaration.
                    declarationSelectorIdx++;
                }
            }
            return rule;
        }

        /// <summary>
        /// Parses a list of dash sizes.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private static int[] ParseDashes(ITree tree)
        {
            if (tree == null)
            {
                throw new ArgumentNullException("tree");
            }

            // parse the dashes.
            var dashes = new int[tree.ChildCount];
            for (int idx = 0; idx < tree.ChildCount; idx++)
            {
                dashes[idx] = int.Parse(tree.GetChild(idx).Text, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
            }
            return dashes;
        }

        /// <summary>
        /// Parses an URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string ParseURL(string url)
        {
            var urlBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(url))
            {
                if (url[0] == '"' && url[url.Length - 1] == '"')
                {
                    urlBuilder.Append(
                        url.Substring(1, url.Length - 2));
                }
                else
                {
                    urlBuilder.Append(url);
                }
            }
            return urlBuilder.ToString();
        }

        /// <summary>
        /// Parses an ARGB color from the MapCSS definition.
        /// </summary>
        /// <param name="colorTree"></param>
        /// <returns></returns>
        private static int ParseColor(CommonTree colorTree)
        {
            if (colorTree == null)
            {
                throw new ArgumentNullException("colorTree");
            }

            int valueInt;
            KnownColor namedColor;

            if (colorTree.Text == "VALUE_RGB")
            { // a pre-defined RGB value.
                string rString = colorTree.GetChild(0).Text;
                string gString = colorTree.GetChild(1).Text;
                string bString = colorTree.GetChild(2).Text;

                int r = int.Parse(rString);
                int g = int.Parse(gString);
                int b = int.Parse(bString);

                return SimpleColor.FromArgb(r, g, b).Value;
            }
            else if (int.TryParse(colorTree.Text, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueInt))
            { // the color is defined as an integer? this cannot happen??
                return valueInt;
            }
#if!WINDOWS_PHONE
            else if (Enum.TryParse<KnownColor>(colorTree.Text, true, out namedColor))
#else
            else if (EnumHelper.TryParse<KnownColor>(colorTree.Text, true, out namedColor))
#endif
            { // the color was named.
                return SimpleColor.FromKnownColor(namedColor).Value;
            }
            else
            { // value could not be parsed.
                throw new MapCSSDomainParserException(colorTree,
                                                            string.Format("Color value cannot be parsed!"));
            }
        }
    }

    /// <summary>
    /// An exception thrown when parsing part of the ANTLR generated tree fails.
    /// </summary>
    public class MapCSSDomainParserException : Exception
    {
        /// <summary>
        /// Creates a new MapCSS domain exception.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="message"></param>
        public MapCSSDomainParserException(object current, string message)
            :base(message)
        {
            
        }

        /// <summary>
        /// The object that was parsed incorrectly.
        /// </summary>
        public object CurrentObject { get; set; }
    }
}

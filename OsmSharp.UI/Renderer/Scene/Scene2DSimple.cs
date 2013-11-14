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
using System.Linq;
using System.Text;
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;
using OsmSharp.Collections;

namespace OsmSharp.UI.Renderer.Scene
{
    /// <summary>
    /// Contains all objects that need to be rendered.
    /// </summary>
    public class Scene2DSimple : Scene2D
    {
        /// <summary>
        /// Holds the string table.
        /// </summary>
        private ObjectTable<string> _stringTable;

        /// <summary>
        /// Holds the zoom ranges.
        /// </summary>
        private ObjectTable<ZoomRanges> _zoomRanges;

        /// <summary>
        /// Holds the index of point.
        /// </summary>
        private List<KeyValuePair<double, double>> _pointIndex;

        /// <summary>
        /// Holds the point styles.
        /// </summary>
        private ObjectTable<StylePoint> _pointStyles;

        /// <summary>
        /// Holds the text styles.
        /// </summary>
        private ObjectTable<StyleText> _textStyles;

        /// <summary>
        /// Holds the line styles.
        /// </summary>
        private ObjectTable<StyleLine> _lineStyles;

        /// <summary>
        /// Holds the polygon styles.
        /// </summary>
        private ObjectTable<StylePolygon> _polygonStyles;

        /// <summary>
        /// Holds the points.
        /// </summary>
        private List<SceneObject> _points;

        /// <summary>
        /// Holds the icons.
        /// </summary>
        private List<SceneObject> _icons;

        /// <summary>
        /// Holds the text objects.
        /// </summary>
        private List<TextObject> _texts;

        /// <summary>
        /// Holds the line text objects.
        /// </summary>
        private List<TextObject> _lineTexts;

        /// <summary>
        /// Holds the line objects.
        /// </summary>
        private List<SceneObject> _lines;

        /// <summary>
        /// Holds the polygon objects.
        /// </summary>
        private List<SceneObject> _polygons;

        /// <summary>
        /// Holds the index of points.
        /// </summary>
        private List<KeyValuePair<double[], double[]>> _pointsIndex;

        /// <summary>
        /// Holds the index of images.
        /// </summary>
        private List<byte[]> _imageIndex;

        /// <summary>
        /// Creates a new simple scene.
        /// </summary>
        public Scene2DSimple()
        {
            // string table.
            _stringTable = new ObjectTable<string>(true);

            // zoom ranges.
            _zoomRanges = new ObjectTable<ZoomRanges>(true);

            // styles.
            _pointStyles = new ObjectTable<StylePoint>(true);
            _textStyles = new ObjectTable<StyleText>(true);
            _lineStyles = new ObjectTable<StyleLine>(true);
            _polygonStyles = new ObjectTable<StylePolygon>(true);

            // geo indexes.
            _pointIndex = new List<KeyValuePair<double, double>>();
            _pointsIndex = new List<KeyValuePair<double[], double[]>>();

            // scene objects.
            _points = new List<SceneObject>();
            _icons = new List<SceneObject>();
            _texts = new List<TextObject>();
            _lineTexts = new List<TextObject>();
            _lines = new List<SceneObject>();
            _polygons = new List<SceneObject>();

            // lines/polygons.
            _imageIndex = new List<byte[]>();
        }

        /// <summary>
        /// Adds the given point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override uint AddPoint(double x, double y)
        {
            uint id = (uint)_pointIndex.Count;
            _pointIndex.Add(new KeyValuePair<double, double>(x, y));
            return id;
        }

        /// <summary>
        /// Adds the given points.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override uint AddPoints(double[] x, double[] y)
        {
            uint id = (uint)_pointsIndex.Count;
            _pointsIndex.Add(new KeyValuePair<double[], double[]>(x, y));
            return id;
        }

        /// <summary>
        /// Adds the given image.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override uint AddImage(byte[] data)
        {
            uint id = (uint)_imageIndex.Count;
            _imageIndex.Add(data);
            return id;
        }

        /// <summary>
        /// Serializes the given scene.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="compress"></param>
        public override void Serialize(Stream stream, bool compress)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears all data from this scene.
        /// </summary>
        public override void Clear()
        {            
            // string table.
            _stringTable.Clear();

            // zoom ranges.
            _zoomRanges.Clear();

            // styles.
            _pointStyles.Clear();
            _textStyles.Clear();
            _lineStyles.Clear();
            _polygonStyles.Clear();

            // geo indexes.
            _pointIndex.Clear();
            _pointsIndex.Clear();

            // scene objects.
            _points.Clear();
            _icons.Clear();
            _texts.Clear();
            _lineTexts.Clear();
            _lines.Clear();
            _polygons.Clear();

            // lines/polygons.
            _imageIndex.Clear();
        }

        /// <summary>
        /// Returns the number of objects in this scene.
        /// </summary>
        public override int Count
        {
            get
            {
                return _points.Count +
                _icons.Count +
                _texts.Count +
                _lineTexts.Count +
                _lines.Count +
                _polygons.Count;
            }
        }

        /// <summary>
        /// Returns the objects in this scene inside the given view.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public override IEnumerable<Scene2DPrimitive> Get(View2D view, float zoom)
        {
            throw new NotImplementedException();
        }

        #region Styles

        /// <summary>
        /// Adds the given style to the given point.
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        public override void AddStylePoint(uint pointId, uint layer, float minZoom, float maxZoom, int color, float size)
        {
            // build the zoom range.
            ZoomRanges zoomRange = new ZoomRanges()
            {
                MinZoom = minZoom,
                MaxZoom = maxZoom
            };
            uint zoomRangeId = _zoomRanges.Add(zoomRange);

            // build the style.
            StylePoint style = new StylePoint()
            {
                Color = color,
                Size = size
            };
            uint styleId = _pointStyles.Add(style);

            // add the scene object.
            _points.Add(new SceneObject() { StyleId = styleId, Layer = layer, GeoId = pointId, ZoomRangeId = zoomRangeId });
        }

        /// <summary>
        /// Adds a new icon at the location of the given point.
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public override void AddIcon(uint pointId, uint layer, float minZoom, float maxZoom, uint imageId)
        {
            // build the zoom range.
            ZoomRanges zoomRange = new ZoomRanges()
            {
                MinZoom = minZoom,
                MaxZoom = maxZoom
            };
            uint zoomRangeId = _zoomRanges.Add(zoomRange);

            // add the scene object.
            _icons.Add(new SceneObject() { StyleId = imageId, Layer = layer, GeoId = pointId, ZoomRangeId = zoomRangeId });
        }

        /// <summary>
        /// Adds text at the position of the given point.
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="size"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="haloColor"></param>
        /// <param name="haloRadius"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public override void AddText(uint pointId, uint layer, float minZoom, float maxZoom, float size, string text, int color, 
            int? haloColor, int? haloRadius, string font)
        {
            // add to stringtable.
            uint textId = _stringTable.Add(text);

            // build the zoom range.
            ZoomRanges zoomRange = new ZoomRanges()
            {
                MinZoom = minZoom,
                MaxZoom = maxZoom
            };
            uint zoomRangeId = _zoomRanges.Add(zoomRange);

            // build the style.
            StyleText style = new StyleText()
            {
                Color = color,
                Size = size,
                Font = font,
                HaloColor = haloColor,
                HaloRadius = haloRadius
            };
            uint styleId = _textStyles.Add(style);

            // add the scene object.
            _texts.Add(new TextObject() { StyleId = styleId, Layer = layer, GeoId = pointId, ZoomRangeId = zoomRangeId, TextId = textId });
        }

        /// <summary>
        /// Adds a line with the given points and style.
        /// </summary>
        /// <param name="pointsId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="lineJoin"></param>
        /// <param name="dashes"></param>
        public override void AddStyleLine(uint pointsId, uint layer, float minZoom, float maxZoom, int color, float width, LineJoin lineJoin, int[] dashes)
        {
            // build the zoom range.
            ZoomRanges zoomRange = new ZoomRanges()
            {
                MinZoom = minZoom,
                MaxZoom = maxZoom
            };
            uint zoomRangeId = _zoomRanges.Add(zoomRange);

            // build the style.
            StyleLine style = new StyleLine()
            {
                Color = color,
                Dashes = dashes,
                LineJoin = lineJoin,
                Width = width
            };
            uint styleId = _lineStyles.Add(style);

            // add the scene object.
            _lines.Add(new SceneObject() { StyleId = styleId, Layer = layer, GeoId = pointsId, ZoomRangeId = zoomRangeId });
        }

        /// <summary>
        /// Adds a line text with the given points and style.
        /// </summary>
        /// <param name="pointsId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="haloColor"></param>
        /// <param name="haloRadius"></param>
        public override void AddStyleLine(uint pointsId, uint layer, float minZoom, float maxZoom, int color, float size, string text, string font,
            int? haloColor, int? haloRadius)
        {
            // add to stringtable.
            uint textId = _stringTable.Add(text);

            // build the zoom range.
            ZoomRanges zoomRange = new ZoomRanges()
            {
                MinZoom = minZoom,
                MaxZoom = maxZoom
            };
            uint zoomRangeId = _zoomRanges.Add(zoomRange);

            // build the style.
            StyleText style = new StyleText()
            {
                Color = color,
                Size = size,
                Font = font,
                HaloColor = haloColor,
                HaloRadius = haloRadius
            };
            uint styleId = _textStyles.Add(style);

            // add the scene object.
            _texts.Add(new TextObject() { StyleId = styleId, Layer = layer, GeoId = pointsId, ZoomRangeId = zoomRangeId, TextId = textId });
        }

        /// <summary>
        /// Adds a polygon with the given points and style.
        /// </summary>
        /// <param name="pointsId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="fill"></param>
        public override void AddStylePolygon(uint pointsId, uint layer, float minZoom, float maxZoom, int color, float width, bool fill)
        {
            // build the zoom range.
            ZoomRanges zoomRange = new ZoomRanges()
            {
                MinZoom = minZoom,
                MaxZoom = maxZoom
            };
            uint zoomRangeId = _zoomRanges.Add(zoomRange);

            // build the style.
            StylePolygon style = new StylePolygon()
            {
                Color = color,
                Fill = fill,
                Width = width
            };
            uint styleId = _polygonStyles.Add(style);

            // add the scene object.
            _polygons.Add(new SceneObject() { StyleId = styleId, Layer = layer, GeoId = pointsId, ZoomRangeId = zoomRangeId });
        }

        #endregion

        private struct SceneObject
        {
            public uint Layer { get; set; }

            public uint GeoId { get; set; }

            public uint StyleId { get; set; }

            public uint ZoomRangeId { get; set; }
        }

        private struct TextObject
        {
            public uint Layer { get; set; }

            public uint GeoId { get; set; }

            public uint StyleId { get; set; }

            public uint ZoomRangeId { get; set; }

            public uint TextId { get; set; }
        }
        
        private class ZoomRanges
        {
            public float MinZoom { get; set; }

            public float MaxZoom { get; set; }

            public override int GetHashCode()
            {
                return this.MinZoom.GetHashCode() ^
                    this.MaxZoom.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is ZoomRanges)
                {
                    return (obj as ZoomRanges).MaxZoom == this.MaxZoom &&
                        (obj as ZoomRanges).MinZoom == this.MinZoom;
                }
                return false;
            }
        }
        
        private class StylePoint
        {
            public int Color { get; set; }

            public float Size { get; set; }

            public override int GetHashCode()
            {
                return this.Color.GetHashCode() ^
                    this.Size.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is StylePoint)
                {
                    return (obj as StylePoint).Color == this.Color &&
                        (obj as StylePoint).Size == this.Size;
                }
                return false;
            }
        }

        private class StyleLine
        {
            public int Color { get; set; }

            public float Width { get; set; }

            public LineJoin LineJoin { get; set; }

            public int[] Dashes { get; set; }

            public override int GetHashCode()
            {
                if (this.Dashes == null)
                {
                    return this.Color.GetHashCode() ^
                        this.Width.GetHashCode() ^
                        this.LineJoin.GetHashCode();
                }
                int hashcode = this.Color.GetHashCode() ^
                    this.Width.GetHashCode() ^
                    this.LineJoin.GetHashCode();
                foreach(int dash in this.Dashes)
                {
                    hashcode = hashcode ^ dash.GetHashCode();
                }
                return hashcode;
            }

            public override bool Equals(object obj)
            {
                if (obj is StyleLine)
                {
                    if ((obj as StyleLine).Color == this.Color &&
                        (obj as StyleLine).Width == this.Width &&
                        (obj as StyleLine).LineJoin == this.LineJoin)
                    {
                        if (this.Dashes != null)
                        {
                            if (this.Dashes.Length == (obj as StyleLine).Dashes.Length)
                            {
                                for (int idx = 0; idx < this.Dashes.Length; idx++)
                                {
                                    if (this.Dashes[idx] != (obj as StyleLine).Dashes[idx])
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                        else
                        {
                            return (obj as StyleLine).Dashes == null;
                        }
                    }
                }
                return false;
            }
        }

        private class StyleText
        {
            public float Size { get; set; }

            public int Color { get; set; }

            public int? HaloColor { get; set; }

            public int? HaloRadius { get; set; }

            public string Font { get; set; }

            public override int GetHashCode()
            {
                if (this.Font == null)
                {
                    return this.Color.GetHashCode() ^
                        this.Size.GetHashCode() ^
                        this.HaloColor.GetHashCode() ^
                        this.HaloRadius.GetHashCode();
                }
                return this.Color.GetHashCode() ^
                    this.Size.GetHashCode() ^
                    this.HaloColor.GetHashCode() ^
                    this.HaloRadius.GetHashCode() ^
                    this.Font.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is StyleText)
                {
                    return (obj as StyleText).Size == this.Size &&
                        (obj as StyleText).Color == this.Color &&
                        (obj as StyleText).HaloRadius == this.HaloRadius &&
                        (obj as StyleText).HaloColor == this.HaloColor &&
                        (obj as StyleText).Font == this.Font;
                }
                return false;
            }
        }

        private class StylePolygon
        {
            public float Width { get; set; }

            public int Color { get; set; }

            public bool Fill { get; set; }

            public override int GetHashCode()
            {
                return this.Width.GetHashCode() ^
                    this.Color.GetHashCode() ^
                    this.Fill.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is StylePolygon)
                {
                    return (obj as StylePolygon).Width == this.Width &&
                        (obj as StylePolygon).Color == this.Color &&
                        (obj as StylePolygon).Fill == this.Fill;
                }
                return false;
            }
        }
    }
}
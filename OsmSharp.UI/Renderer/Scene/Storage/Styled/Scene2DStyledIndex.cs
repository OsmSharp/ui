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
using System.Text;
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;
using ProtoBuf;

namespace OsmSharp.UI.Renderer.Scene.Storage.Styled
{
    /// <summary>
    /// Holds scene primitive styles.
    /// </summary>
    [ProtoContract]
    class Scene2DStyledIndex
    {
        /// <summary>
        /// Creates a new Scene2D primitives style index.
        /// </summary>
        public Scene2DStyledIndex()
        {
            this.IconStyles = new List<Icon2DStyle>();
            this.ImageStyles = new List<Image2DStyle>();
            this.LineStyles = new List<Line2DStyle>();
            this.LineTextStyles = new List<LineText2DStyle>();
            this.PointStyles = new List<Point2DStyle>();
            this.PolygonStyles = new List<Polygon2DStyle>();
            this.TextStyles = new List<Text2DStyle>();
        }

        /// <summary>
        /// Holds the icon styles.
        /// </summary>
        [ProtoMember(1)]
        public List<Icon2DStyle> IconStyles { get; set; }

        /// <summary>
        /// Adds a new style and returns it's index.
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public ushort AddStyle(Icon2D icon, int layer)
        {
            Icon2DStyle newStyle = Icon2DStyle.From(icon, layer);
            int indexOf = this.IconStyles.IndexOf(newStyle);
            if (indexOf < 0)
            { // the style is not found yet.
                indexOf = this.IconStyles.Count;
                this.IconStyles.Add(newStyle);
            }
            return (ushort)indexOf;
        }

        /// <summary>
        /// Holds the image styles.
        /// </summary>
        [ProtoMember(2)]
        public List<Image2DStyle> ImageStyles { get; set; }

        /// <summary>
        /// Adds a new style and returns it's index.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public ushort AddStyle(Image2D image, int layer)
        {
            Image2DStyle newStyle = Image2DStyle.From(image, layer);
            int indexOf = this.ImageStyles.IndexOf(newStyle);
            if (indexOf < 0)
            { // the style is not found yet.
                indexOf = this.ImageStyles.Count;
                this.ImageStyles.Add(newStyle);
            }
            return (ushort)indexOf;
        }

        /// <summary>
        /// Holds the line styles.
        /// </summary>
        [ProtoMember(3)]
        public List<Line2DStyle> LineStyles { get; set; }

        /// <summary>
        /// Adds a new style and returns it's index.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public ushort AddStyle(Line2D line, int layer)
        {
            Line2DStyle newStyle = Line2DStyle.From(line, layer);
            int indexOf = this.LineStyles.IndexOf(newStyle);
            if (indexOf < 0)
            { // the style is not found yet.
                indexOf = this.LineStyles.Count;
                this.LineStyles.Add(newStyle);
            }
            return (ushort)indexOf;
        }

        /// <summary>
        /// Holds the line text styles.
        /// </summary>
        [ProtoMember(4)]
        public List<LineText2DStyle> LineTextStyles { get; set; }

        /// <summary>
        /// Adds a new style and returns it's index.
        /// </summary>
        /// <param name="lineText"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public ushort AddStyle(LineText2D lineText, int layer)
        {
            LineText2DStyle newStyle = LineText2DStyle.From(lineText, layer);
            int indexOf = this.LineTextStyles.IndexOf(newStyle);
            if (indexOf < 0)
            { // the style is not found yet.
                indexOf = this.LineTextStyles.Count;
                this.LineTextStyles.Add(newStyle);
            }
            return (ushort)indexOf;
        }

        /// <summary>
        /// Holds the point styles.
        /// </summary>
        [ProtoMember(5)]
        public List<Point2DStyle> PointStyles { get; set; }

        /// <summary>
        /// Adds a new style and returns it's index.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public ushort AddStyle(Point2D point, int layer)
        {
            Point2DStyle newStyle = Point2DStyle.From(point, layer);
            int indexOf = this.PointStyles.IndexOf(newStyle);
            if (indexOf < 0)
            { // the style is not found yet.
                indexOf = this.PointStyles.Count;
                this.PointStyles.Add(newStyle);
            }
            return (ushort)indexOf;
        }

        /// <summary>
        /// Holds the polygon styles.
        /// </summary>
        [ProtoMember(6)]
        public List<Polygon2DStyle> PolygonStyles { get; set; }

        /// <summary>
        /// Adds a new style and returns it's index.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public ushort AddStyle(Polygon2D polygon, int layer)
        {
            Polygon2DStyle newStyle = Polygon2DStyle.From(polygon, layer);
            int indexOf = this.PolygonStyles.IndexOf(newStyle);
            if (indexOf < 0)
            { // the style is not found yet.
                indexOf = this.PolygonStyles.Count;
                this.PolygonStyles.Add(newStyle);
            }
            return (ushort)indexOf;
        }

        /// <summary>
        /// Holds the text styles.
        /// </summary>
        [ProtoMember(7)]
        public List<Text2DStyle> TextStyles { get; set; }

        /// <summary>
        /// Adds a new style and returns it's index.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public ushort AddStyle(Text2D text, int layer)
        {
            Text2DStyle newStyle = Text2DStyle.From(text, layer);
            int indexOf = this.TextStyles.IndexOf(newStyle);
            if (indexOf < 0)
            { // the style is not found yet.
                indexOf = this.TextStyles.Count;
                this.TextStyles.Add(newStyle);
            }
            return (ushort)indexOf;
        }
    }

    /// <summary>
    /// Describes only the style of an Icon2D object.
    /// </summary>
    [ProtoContract]
    class Icon2DStyle
    {
        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        [ProtoMember(1)]
        public int Layer { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [ProtoMember(2)]
        public byte[] Image { get; set; }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        [ProtoMember(3)]
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        [ProtoMember(4)]
        public float MaxZoom { get; set; }

        /// <summary>
        /// Extracts style information.
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static Icon2DStyle From(Icon2D icon, int layer)
        {
            Icon2DStyle newStyle = new Icon2DStyle();
            newStyle.Image = icon.Image;
            newStyle.MaxZoom = icon.MaxZoom;
            newStyle.MinZoom = icon.MinZoom;
            newStyle.Layer = layer;
            return newStyle;
        }

        /// <summary>
        /// Sets the style information.
        /// </summary>
        /// <param name="icon"></param>
        public void Set(Icon2D icon)
        {
            icon.Image = this.Image;
            icon.MaxZoom = this.MaxZoom;
            icon.MinZoom = this.MinZoom;
        }

        /// <summary>
        /// Returns a hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if(this.Image == null)
            {
                return this.Layer.GetHashCode() ^
                    this.MaxZoom.GetHashCode() ^
                    this.MinZoom.GetHashCode();
            }
            return this.Image.GetHashCode() ^
                this.Layer.GetHashCode() ^
                this.MaxZoom.GetHashCode() ^
                this.MinZoom.GetHashCode();
        }

        /// <summary>
        /// Determines whether the the given object is equal to the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Image2DStyle))
            { // wrong type.
                return false;
            }
            Icon2DStyle other = (obj as Icon2DStyle);
            if (this.Layer == other.Layer &
                this.MaxZoom == other.MaxZoom &
                this.MinZoom == other.MinZoom)
            {
                if (this.Image == null &
                    other.Image == null)
                {
                    return true;
                }
                else if (this.Image == null |
                    other.Image == null)
                {
                    return false;
                }
                else if (this.Image.Length == other.Image.Length)
                {
                    for (int idx = 0; idx < other.Image.Length; idx++)
                    {
                        if (this.Image[idx] != other.Image[idx])
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
            return false;
        }
    }

    /// <summary>
    /// Describes only the style of an Image2D object.
    /// </summary>
    [ProtoContract]
    class Image2DStyle
    {
        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        [ProtoMember(1)]
        public int Layer { get; set; }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        [ProtoMember(2)]
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        [ProtoMember(3)]
        public float MaxZoom { get; set; }

        /// <summary>
        /// Extracts style information.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Image2DStyle From(Image2D image, int layer)
        {
            Image2DStyle newStyle = new Image2DStyle();
            newStyle.MaxZoom = image.MaxZoom;
            newStyle.MinZoom = image.MinZoom;
            newStyle.Layer = layer;
            return newStyle;
        }

        /// <summary>
        /// Sets style information.
        /// </summary>
        /// <param name="image"></param>
        public void Set(Image2D image)
        {
            image.MaxZoom = image.MaxZoom;
            image.MinZoom = image.MinZoom;
        }

        /// <summary>
        /// Returns a hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Layer.GetHashCode() ^
                this.MaxZoom.GetHashCode() ^
                this.MinZoom.GetHashCode();
        }

        /// <summary>
        /// Determines whether the the given object is equal to the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Image2DStyle))
            { // wrong type.
                return false;
            }
            Image2DStyle other = (obj as Image2DStyle);
            return (this.Layer == other.Layer &
                this.MaxZoom == other.MaxZoom &
                this.MinZoom == other.MinZoom);
        }
    }

    /// <summary>
    /// Describes only the style of a Line2D object.
    /// </summary>
    [ProtoContract]
    class Line2DStyle
    {
        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        [ProtoMember(1)]
        public int Layer { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [ProtoMember(2)]
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        [ProtoMember(3)]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the linejoin.
        /// </summary>
        [ProtoMember(4)]
        public LineJoin LineJoin { get; set; }

        /// <summary>
        /// Gets or sets the line dashses.
        /// </summary>
        [ProtoMember(5)]
        public int[] Dashes { get; set; }

        /// <summary>
        /// Gets or sets the casing width.
        /// </summary>
        [ProtoMember(6)]
        public float CasingWidth { get; set; }

        /// <summary>
        /// Gets or sets the casing color.
        /// </summary>
        [ProtoMember(7)]
        public int CasingColor { get; set; }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        [ProtoMember(8)]
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        [ProtoMember(9)]
        public float MaxZoom { get; set; }

        /// <summary>
        /// Extracts style information.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Line2DStyle From(Line2D line, int layer)
        {
            Line2DStyle newStyle = new Line2DStyle();
            newStyle.Color = line.Color;
            newStyle.Dashes = line.Dashes;
            newStyle.LineJoin = line.LineJoin;
            newStyle.MaxZoom = line.MaxZoom;
            newStyle.MinZoom = line.MinZoom;
            newStyle.Width = line.Width;
            newStyle.Layer = layer;
            return newStyle;
        }

        /// <summary>
        /// Sets the style information.
        /// </summary>
        /// <param name="line"></param>
        public void Set(Line2D line)
        {
            line.Color = this.Color;
            line.Dashes = this.Dashes;
            line.LineJoin = this.LineJoin;
            line.MaxZoom = this.MaxZoom;
            line.MinZoom = this.MinZoom;
            line.Width = this.Width;
        }

        /// <summary>
        /// Returns a hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if(this.Dashes == null)
            {
                return this.Layer.GetHashCode() ^
                    this.MaxZoom.GetHashCode() ^
                    this.MinZoom.GetHashCode() ^
                    this.CasingColor.GetHashCode() ^
                    this.CasingWidth.GetHashCode() ^
                    this.Color.GetHashCode() ^
                    this.Layer.GetHashCode() ^
                    this.LineJoin.GetHashCode() ^
                    this.MaxZoom.GetHashCode() ^
                    this.MinZoom.GetHashCode() ^
                    this.Width.GetHashCode();
            }
            return this.Layer.GetHashCode() ^
                    this.MaxZoom.GetHashCode() ^
                    this.MinZoom.GetHashCode() ^
                    this.CasingColor.GetHashCode() ^
                    this.CasingWidth.GetHashCode() ^
                    this.Color.GetHashCode() ^
                    this.Layer.GetHashCode() ^
                    this.LineJoin.GetHashCode() ^
                    this.MaxZoom.GetHashCode() ^
                    this.MinZoom.GetHashCode() ^
                    this.Width.GetHashCode() ^
                    this.Dashes.GetHashCode();
        }

        /// <summary>
        /// Determines whether the the given object is equal to the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Line2DStyle))
            { // wrong type.
                return false;
            }
            Line2DStyle other = (obj as Line2DStyle);
            if(this.Dashes != null)
            {
                if (this.CasingColor == other.CasingColor &&
                    this.CasingWidth == other.CasingWidth &&
                    this.Color == other.Color &&
                    this.Layer == other.Layer &&
                    this.LineJoin == other.LineJoin &&
                    this.MaxZoom == other.MaxZoom &&
                    this.MinZoom == other.MinZoom &&
                    this.Width == other.Width)
                {
                    if (this.Dashes == null &&
                        other.Dashes == null)
                    {
                        return true;
                    }
                    else if (this.Dashes == null |
                        other.Dashes == null)
                    {
                        return false;
                    }
                    else if(this.Dashes.Length == other.Dashes.Length)
                    {
                        for (int idx = 0; idx < this.Dashes.Length; idx++)
                        {
                            if (this.Dashes[idx] != other.Dashes[idx])
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return (this.CasingColor == other.CasingColor &&
                this.CasingWidth == other.CasingWidth &&
                this.Color == other.Color &&
                this.Layer == other.Layer &&
                this.LineJoin == other.LineJoin &&
                this.MaxZoom == other.MaxZoom &&
                this.MinZoom == other.MinZoom &&
                this.Width == other.Width);
        }
    }

    /// <summary>
    /// Describes only the style of a LineText2D object.
    /// </summary>
    [ProtoContract]
    class LineText2DStyle
    {
        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        [ProtoMember(1)]
        public int Layer { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [ProtoMember(2)]
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets the halo size.
        /// </summary>
        [ProtoMember(3)]
        public int? HaloRadius { get; set; }

        /// <summary>
        /// Gets or sets the halo color.
        /// </summary>
        [ProtoMember(4)]
        public int? HaloColor { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        [ProtoMember(5)]
        public float Size { get; set; }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        [ProtoMember(6)]
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        [ProtoMember(7)]
        public float MaxZoom { get; set; }

        /// <summary>
        /// Extracts style information.
        /// </summary>
        /// <param name="lineText"></param>
        /// <returns></returns>
        public static LineText2DStyle From(LineText2D lineText, int layer)
        {
            LineText2DStyle newStyle = new LineText2DStyle();
            newStyle.Color = lineText.Color;
            newStyle.MaxZoom = lineText.MaxZoom;
            newStyle.MinZoom = lineText.MinZoom;
            newStyle.HaloColor = lineText.HaloColor;
            newStyle.HaloRadius = lineText.HaloRadius;
            newStyle.Layer = layer;
            newStyle.Size = lineText.Size;
            return newStyle;
        }

        /// <summary>
        /// Sets the style information.
        /// </summary>
        /// <param name="lineText"></param>
        public void Set(LineText2D lineText)
        {
            lineText.Color = this.Color;
            lineText.MaxZoom = this.MaxZoom;
            lineText.MinZoom = this.MinZoom;
            lineText.HaloColor = this.HaloColor;
            lineText.HaloRadius = this.HaloRadius;
            lineText.Size = this.Size;
        }

        /// <summary>
        /// Returns a hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Layer.GetHashCode() ^
                this.MaxZoom.GetHashCode() ^
                this.MinZoom.GetHashCode() ^
                this.Color.GetHashCode() ^
                this.HaloColor.GetHashCode() ^
                this.HaloRadius.GetHashCode() ^
                this.Size.GetHashCode();
        }

        /// <summary>
        /// Determines whether the the given object is equal to the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is LineText2DStyle))
            { // wrong type.
                return false;
            }
            LineText2DStyle other = (obj as LineText2DStyle);
            return (this.Layer == other.Layer &
                this.MaxZoom == other.MaxZoom &
                this.MinZoom == other.MinZoom &
                this.Color == other.Color &
                this.HaloColor == other.HaloColor &
                this.HaloRadius == other.HaloRadius &
                this.Layer == other.Layer &
                this.Size == other.Size);
        }
    }

    /// <summary>
    /// Describes only the style of a Point2D object.
    /// </summary>
    [ProtoContract]
    class Point2DStyle
    {
        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        [ProtoMember(1)]
        public int Layer { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [ProtoMember(2)]
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        [ProtoMember(3)]
        public double Size { get; set; }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        [ProtoMember(4)]
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        [ProtoMember(5)]
        public float MaxZoom { get; set; }

        /// <summary>
        /// Extracts style information.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point2DStyle From(Point2D point, int layer)
        {
            Point2DStyle newStyle = new Point2DStyle();
            newStyle.Color = point.Color;
            newStyle.MaxZoom = point.MaxZoom;
            newStyle.MinZoom = point.MinZoom;
            newStyle.Layer = layer;
            return newStyle;
        }

        /// <summary>
        /// Sets style information.
        /// </summary>
        /// <param name="point"></param>
        public void Set(Point2D point)
        {
            point.Color = this.Color;
            point.MaxZoom = this.MaxZoom;
            point.MinZoom = this.MinZoom;
        }

        /// <summary>
        /// Returns a hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Layer.GetHashCode() ^
                this.MaxZoom.GetHashCode() ^
                this.MinZoom.GetHashCode() ^
                this.Color.GetHashCode() ^
                this.Size.GetHashCode();
        }

        /// <summary>
        /// Determines whether the the given object is equal to the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Point2DStyle))
            { // wrong type.
                return false;
            }
            Point2DStyle other = (obj as Point2DStyle);
            return (this.Layer == other.Layer &
                this.MaxZoom == other.MaxZoom &
                this.MinZoom == other.MinZoom &
                this.Color == other.Color &
                this.Layer == other.Layer &
                this.Size == other.Size);
        }
    }

    /// <summary>
    /// Describes only the style of a Polygon2D object.
    /// </summary>
    [ProtoContract]
    class Polygon2DStyle
    {
        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        [ProtoMember(1)]
        public int Layer { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        [ProtoMember(2)]
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        [ProtoMember(3)]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OsmSharp.UI.Renderer.Scene2DPrimitives.Polygon2D"/> is to be filled.
        /// </summary>
        /// <value><c>true</c> if fill; otherwise, <c>false</c>.</value>
        [ProtoMember(4)]
        public bool Fill { get; set; }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        [ProtoMember(5)]
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        [ProtoMember(6)]
        public float MaxZoom { get; set; }
        
        /// <summary>
        /// Extracts style information.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static Polygon2DStyle From(Polygon2D polygon, int layer)
        {
            Polygon2DStyle newStyle = new Polygon2DStyle();
            newStyle.Color = polygon.Color;
            newStyle.MaxZoom = polygon.MaxZoom;
            newStyle.MinZoom = polygon.MinZoom;
            newStyle.Fill = polygon.Fill;
            newStyle.Width = polygon.Width;
            newStyle.Layer = layer;
            return newStyle;
        }

        /// <summary>
        /// Sets style information.
        /// </summary>
        /// <param name="polygon"></param>
        public void Set(Polygon2D polygon)
        {
            polygon.Color = this.Color;
            polygon.MaxZoom = this.MaxZoom;
            polygon.MinZoom = this.MinZoom;
            polygon.Fill = this.Fill;
            polygon.Width = this.Width;
        }

        /// <summary>
        /// Returns a hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Layer.GetHashCode() ^
                this.MaxZoom.GetHashCode() ^
                this.MinZoom.GetHashCode() ^
                this.Color.GetHashCode() ^
                this.Fill.GetHashCode() ^
                this.Width.GetHashCode();
        }

        /// <summary>
        /// Determines whether the the given object is equal to the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Polygon2DStyle))
            { // wrong type.
                return false;
            }
            Polygon2DStyle other = (obj as Polygon2DStyle);
            return (this.Layer == other.Layer &
                this.MaxZoom == other.MaxZoom &
                this.MinZoom == other.MinZoom &
                this.Color == other.Color &
                this.Layer == other.Layer &
                this.Width == other.Width &
                this.Fill == other.Fill);
        }
    }

    /// <summary>
    /// Describes only the style of a Text2D object.
    /// </summary>
    [ProtoContract]
    class Text2DStyle
    {
        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        [ProtoMember(1)]
        public int Layer { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        [ProtoMember(2)]
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets the halo size.
        /// </summary>
        [ProtoMember(3)]
        public int? HaloRadius { get; set; }

        /// <summary>
        /// Gets or sets the halo color.
        /// </summary>
        [ProtoMember(4)]
        public int? HaloColor { get; set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        [ProtoMember(5)]
        public double Size { get; set; }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        [ProtoMember(6)]
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        [ProtoMember(7)]
        public float MaxZoom { get; set; }
		
		/// <summary>
		/// The font.
		/// </summary>
		[ProtoMember(8)]
		public string Font { get; set; }

        /// <summary>
        /// Extracts style information.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static Text2DStyle From(Text2D text, int layer)
        {
            Text2DStyle newStyle = new Text2DStyle();
            newStyle.Color = text.Color;
            newStyle.MaxZoom = text.MaxZoom;
            newStyle.MinZoom = text.MinZoom;
            newStyle.HaloColor = text.HaloColor;
            newStyle.HaloRadius = text.HaloRadius;
            newStyle.Size = text.Size;
            newStyle.Layer = layer;
			newStyle.Font = text.Font;
            return newStyle;
        }

        /// <summary>
        /// Sets style information.
        /// </summary>
        /// <param name="text"></param>
        public void Set(Text2D text)
        {
            text.MaxZoom = this.MaxZoom;
            text.MinZoom = this.MinZoom;
            text.HaloColor = this.HaloColor;
            text.HaloRadius = this.HaloRadius;
            text.Size = this.Size;
			text.Font = this.Font;
        }

        /// <summary>
        /// Returns a hash.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Layer.GetHashCode() ^
                this.MaxZoom.GetHashCode() ^
                this.MinZoom.GetHashCode() ^
                this.Color.GetHashCode() ^
                this.HaloColor.GetHashCode() ^
                this.HaloRadius.GetHashCode() ^
                this.Size.GetHashCode();
        }

        /// <summary>
        /// Determines whether the the given object is equal to the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Polygon2DStyle))
            { // wrong type.
                return false;
            }
            Text2DStyle other = (obj as Text2DStyle);
            return (this.Layer == other.Layer &
                this.MaxZoom == other.MaxZoom &
                this.MinZoom == other.MinZoom &
                this.Color == other.Color &
                this.Layer == other.Layer &
                this.HaloColor == other.HaloColor &
                this.HaloRadius == other.HaloRadius &
                this.Size == other.Size);
        }
    }
}

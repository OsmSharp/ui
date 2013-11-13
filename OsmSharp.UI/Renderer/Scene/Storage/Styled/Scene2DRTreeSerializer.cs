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
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;
using ProtoBuf;
using ProtoBuf.Meta;

namespace OsmSharp.UI.Renderer.Scene.Storage.Styled
{
    /// <summary>
    /// A serializer for scene primitive spatial index.
    /// </summary>
    internal class Scene2DRTreeSerializer : RTreeStreamSerializer<Scene2DEntry>
    {
        /// <summary>
        /// Holds the compression flag.
        /// </summary>
        private bool _compress;

        internal static int ConversionFactor = 1000000;

        /// <summary>
        /// Holds the style index.
        /// </summary>
        private Scene2DStyledIndex _styleIndex;

        /// <summary>
        /// Creates a new scene serializer.
        /// </summary>
        /// <param name="compress"></param>
        /// <param name="styleIndex"></param>
        public Scene2DRTreeSerializer(bool compress, Scene2DStyledIndex styleIndex)
        {
            _compress = compress;
            _styleIndex = styleIndex;
        }

        /// <summary>
        /// Builds the runtime type model.
        /// </summary>
        /// <param name="typeModel"></param>
        protected override void BuildRuntimeTypeModel(RuntimeTypeModel typeModel)
        {
            typeModel.Add(typeof(PrimitivesCollection), true);

            // add all entry/style objects.
            typeModel.Add(typeof(Icon2DEntry), true);
            //typeModel.Add(typeof(Icon2DStyle), true);
            typeModel.Add(typeof(Image2DEntry), true);
            //typeModel.Add(typeof(Image2DStyle), true);
            typeModel.Add(typeof(Line2DEntry), true);
            //typeModel.Add(typeof(Line2DStyle), true);
            typeModel.Add(typeof(Point2DEntry), true);
            //typeModel.Add(typeof(Point2DStyle), true);
            typeModel.Add(typeof(Polygon2DEntry), true);
            //typeModel.Add(typeof(Polygon2DStyle), true);
            typeModel.Add(typeof(Text2DEntry), true);
            //typeModel.Add(typeof(Text2DStyle), true);
            typeModel.Add(typeof(LineText2DEntry), true);
            //typeModel.Add(typeof(LineText2DStyle), true);
        }

        /// <summary>
        /// Serializes the actual data.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="data"></param>
        /// <param name="boxes"></param>
        /// <returns></returns>
        protected override byte[] Serialize(RuntimeTypeModel typeModel, List<Scene2DEntry> data,
		                                    List<BoxF2D> boxes)
        {
            var icons = new List<Icon2DEntry>();
            var images = new List<Image2DEntry>();
            var lines = new List<Line2DEntry>();
            var points = new List<Point2DEntry>();
            var polygons = new List<Polygon2DEntry>();
            var texts = new List<Text2DEntry>();
            var lineTexts = new List<LineText2DEntry>();

            // build the collection object.
            var collection = new PrimitivesCollection();
            for (int idx = 0; idx < data.Count; idx++)
            {
                IScene2DPrimitive primitive = data[idx].Scene2DPrimitive;
                if (primitive is Icon2D)
                {
                    Icon2D icon = primitive as Icon2D;
                    icons.Add(Icon2DEntry.From(data[idx].Id, icon, _styleIndex.AddStyle(icon, data[idx].Layer)));
                }
                else if (primitive is Image2D)
                {
                    Image2D image = primitive as Image2D;
                    images.Add(Image2DEntry.From(data[idx].Id, image, _styleIndex.AddStyle(image, data[idx].Layer)));
                }
                else if (primitive is Line2D)
                {
                    Line2D line = primitive as Line2D;
                    lines.Add(Line2DEntry.From(data[idx].Id, line, _styleIndex.AddStyle(line, data[idx].Layer)));
                }
                else if (primitive is Point2D)
                {
                    Point2D point = primitive as Point2D;
                    points.Add(Point2DEntry.From(data[idx].Id, point, _styleIndex.AddStyle(point, data[idx].Layer)));
                }
                else if (primitive is Polygon2D)
                {
                    Polygon2D polygon = primitive as Polygon2D;
                    polygons.Add(Polygon2DEntry.From(data[idx].Id, polygon, _styleIndex.AddStyle(polygon, data[idx].Layer)));
                }
                else if (primitive is Text2D)
                {
                    Text2D text = primitive as Text2D;
                    texts.Add(Text2DEntry.From(data[idx].Id, text, _styleIndex.AddStyle(text, data[idx].Layer)));
                }
                else if (primitive is LineText2D)
                {
                    LineText2D lineText = primitive as LineText2D;
                    lineTexts.Add(LineText2DEntry.From(data[idx].Id, lineText, _styleIndex.AddStyle(lineText, data[idx].Layer)));
                }
                else
                {
                    throw new Exception("Primitive type not supported by serializer.");
                }
            }

            collection.Icons = icons.ToArray();
            collection.Images = images.ToArray();
            collection.Lines = lines.ToArray();
            collection.Points = points.ToArray();
            collection.Polygons = polygons.ToArray();
            collection.Texts = texts.ToArray();
            collection.LineTexts = lineTexts.ToArray();

            // create the memory stream.
            var stream = new MemoryStream();
            typeModel.Serialize(stream, collection);
            if (!_compress)
            {
                return stream.ToArray();
            }
            return GZipStream.CompressBuffer(stream.ToArray());
        }

        /// <summary>
        /// Returns the version.
        /// </summary>
        public override string VersionString
        {
            get { return "ScenePrimitives2D.v3"; }
        }

        /// <summary>
        /// Holds primitives.
        /// </summary>
        [ProtoContract]
        public class PrimitivesCollection
        {
            /// <summary>
            /// Holds the icons.
            /// </summary>
            [ProtoMember(1)]
            public Icon2DEntry[] Icons { get; set; }

            /// <summary>
            /// Holds the images.
            /// </summary>
            [ProtoMember(2)]
            public Image2DEntry[] Images { get; set; }

            /// <summary>
            /// Holds the lines.
            /// </summary>
            [ProtoMember(3)]
            public Line2DEntry[] Lines { get; set; }

            /// <summary>
            /// Holds the points.
            /// </summary>
            [ProtoMember(4)]
            public Point2DEntry[] Points { get; set; }

            /// <summary>
            /// Holds the polygons.
            /// </summary>
            [ProtoMember(5)]
            public Polygon2DEntry[] Polygons { get; set; }

            /// <summary>
            /// Holds the texts.
            /// </summary>
            [ProtoMember(6)]
            public Text2DEntry[] Texts { get; set; }

            /// <summary>
            /// Holds the line texts.
            /// </summary>
            [ProtoMember(7)]
            public LineText2DEntry[] LineTexts { get; set; }
        }

        /// <summary>
        /// Deserializes the actual data.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="data"></param>
        /// <param name="boxes"></param>
        /// <returns></returns>
        protected override List<Scene2DEntry> DeSerialize(RuntimeTypeModel typeModel, byte[] data,
		                                                  out List<BoxF2D> boxes)
        {
            // decompress if needed.
            Stream stream = null;
            if (_compress)
            {
                stream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
            }
            else
            { // uncompressed stream.
                stream = new MemoryStream(data);
            }

            // create the memory stream.
            var collection = typeModel.Deserialize(stream, null,
                typeof(PrimitivesCollection)) as PrimitivesCollection;
            if (collection == null)
            {
                throw new Exception("Could not deserialize primitives.");
            }

            // create the collection
            var primitives = new List<Scene2DEntry>();
            if (collection.Icons != null)
            {
                foreach (var primitive in collection.Icons)
                {
                    Icon2DStyle style = _styleIndex.IconStyles[primitive.StyleId];
                    Icon2D icon = primitive.To(style);
                    primitives.Add(new Scene2DEntry()
                    {
                        //Id = primitive.Id,
                        Layer = style.Layer,
                        Scene2DPrimitive = icon
                    });
                }
            }
            if (collection.Images != null)
            {
                foreach (var primitive in collection.Images)
                {
                    Image2DStyle style = _styleIndex.ImageStyles[primitive.StyleId];
                    Image2D image = primitive.To(style);
                    primitives.Add(new Scene2DEntry()
                    {
                        //Id = primitive.Id,
                        Layer = style.Layer,
                        Scene2DPrimitive = image
                    });
                }
            }
            if (collection.Lines != null)
            {
                foreach (var primitive in collection.Lines)
                {
                    Line2DStyle style = _styleIndex.LineStyles[primitive.StyleId];
                    Line2D line = primitive.To(style);
                    primitives.Add(new Scene2DEntry()
                    {
                        //Id = primitive.Id,
                        Layer = style.Layer,
                        Scene2DPrimitive = line
                    });
                }
            }
            if (collection.Points != null)
            {
                foreach (var primitive in collection.Points)
                {
                    Point2DStyle style = _styleIndex.PointStyles[primitive.StyleId];
                    Point2D point = primitive.To(style);
                    primitives.Add(new Scene2DEntry()
                    {
                        //Id = primitive.Id,
                        Layer = style.Layer,
                        Scene2DPrimitive = point
                    });
                }
            }
            if (collection.Polygons != null)
            {
                foreach (var primitive in collection.Polygons)
                {
                    Polygon2DStyle style = _styleIndex.PolygonStyles[primitive.StyleId];
                    Polygon2D polygon = primitive.To(style);
                    primitives.Add(new Scene2DEntry()
                    {
                        //Id = primitive.Id,
                        Layer = style.Layer,
                        Scene2DPrimitive = polygon
                    });
                }
            }
            if (collection.Texts != null)
            {
                foreach (var primitive in collection.Texts)
                {
                    Text2DStyle style = _styleIndex.TextStyles[primitive.StyleId];
                    Text2D text = primitive.To(style);
                    primitives.Add(new Scene2DEntry()
                    {
                        //Id = primitive.Id,
                        Layer = style.Layer,
                        Scene2DPrimitive = text
                    });
                }
            }
            if (collection.LineTexts != null)
            {
                foreach (var primitive in collection.LineTexts)
                {
                    LineText2DStyle style = _styleIndex.LineTextStyles[primitive.StyleId];
                    LineText2D lineText = primitive.To(style);
                    primitives.Add(new Scene2DEntry()
                    {
                        //Id = primitive.Id,
                        Layer = style.Layer,
                        Scene2DPrimitive = lineText
                    });
                }
            }

            // build the boxes list.
			boxes = new List<BoxF2D>();
            for (int idx = 0; idx < primitives.Count; idx++)
            {
                boxes.Add(primitives[idx].Scene2DPrimitive.GetBox());
            }
            return primitives;
        }
    }

    /// <summary>
    /// Describes an Icon2D object with a reference to a style.
    /// </summary>
    [ProtoContract]
    public class Icon2DEntry
    {
        ///// <summary>
        ///// Gets or sets the id.
        ///// </summary>
        //[ProtoMember(1)]
        //public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        [ProtoMember(1)]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        [ProtoMember(2)]
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the style id.
        /// </summary>
        [ProtoMember(3)]
        public ushort StyleId { get; set; }

        /// <summary>
        /// Converts into a basic entry.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="icon"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        internal static Icon2DEntry From(uint id, Icon2D icon, ushort styleId)
        {
            Icon2DEntry entry = new Icon2DEntry();
            //entry.Id = id;
            entry.X = icon.X;
            entry.Y = icon.Y;
            entry.StyleId = styleId;
            return entry;
        }

        /// <summary>
        /// Converts this basic entry into a scene object.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        internal Icon2D To(Icon2DStyle style)
        {
            Icon2D icon = new Icon2D();
            icon.Image = style.Image;
            icon.MaxZoom = style.MaxZoom;
            icon.MinZoom = style.MinZoom;
            icon.X = this.X;
            icon.Y = this.Y;
            return icon;
        }
    }

    /// <summary>
    /// Describes an Image2D object with a reference to a style.
    /// </summary>
    [ProtoContract]
    public class Image2DEntry
    {
        ///// <summary>
        ///// Gets or sets the id.
        ///// </summary>
        //[ProtoMember(1)]
        //public uint Id { get; set; }

        /// <summary>
        /// Gets the image data.
        /// </summary>
        [ProtoMember(1)]
        public byte[] ImageData { get; set; }

        /// <summary>
        /// Gets the left.
        /// </summary>
        [ProtoMember(2)]
        public double Left { get; set; }

        /// <summary>
        /// Gets the right.
        /// </summary>
        [ProtoMember(3)]
        public double Right { get; set; }

        /// <summary>
        /// Gets the top.
        /// </summary>
        [ProtoMember(4)]
        public double Top { get; set; }

        /// <summary>
        /// Gets the bottom.
        /// </summary>
        [ProtoMember(5)]
        public double Bottom { get; set; }

        /// <summary>
        /// Gets or sets the style id.
        /// </summary>
        [ProtoMember(6)]
        public ushort StyleId { get; set; }

        /// <summary>
        /// Converts into a basic entry.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="styleId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static Image2DEntry From(uint id, Image2D image, ushort styleId)
        {
            Image2DEntry entry = new Image2DEntry();
            //entry.Id = id;
            entry.ImageData = image.ImageData;
            entry.Bottom = image.Bottom;
            entry.Left = image.Left;
            entry.Right = image.Right;
            entry.Top = image.Top;
            entry.StyleId = styleId;
            return entry;
        }

        /// <summary>
        /// Converts this basic entry into a scene object.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        internal Image2D To(Image2DStyle style)
        {
            Image2D image = new Image2D();
            image.Left = this.Left;
            image.Bottom = this.Bottom;
            image.ImageData = this.ImageData;
            image.MaxZoom = style.MaxZoom;
            image.MinZoom = style.MinZoom;
            image.Right = this.Right;
            image.Top = this.Top;
            return image;
        }
    }

    /// <summary>
    /// Describes a Line2D object with a reference to a style.
    /// </summary>
    [ProtoContract]
    public class Line2DEntry
    {
        ///// <summary>
        ///// Gets or sets the id.
        ///// </summary>
        //[ProtoMember(1)]
        //public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        [ProtoMember(1)]
        public long[] X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        [ProtoMember(2)]
        public long[] Y
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the style id.
        /// </summary>
        [ProtoMember(3)]
        public int StyleId { get; set; }

        /// <summary>
        /// Creates a basic entry.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="line"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        internal static Line2DEntry From(uint id, Line2D line, ushort styleId)
        {
            Line2DEntry entry = new Line2DEntry();
            //entry.Id = id;
            entry.X = line.X.ConvertToLongArray(Scene2DRTreeSerializer.ConversionFactor).EncodeDelta();
            entry.Y = line.Y.ConvertToLongArray(Scene2DRTreeSerializer.ConversionFactor).EncodeDelta();
            entry.StyleId = styleId;
            return entry;
        }

        /// <summary>
        /// Converts this basic entry into a scene object.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        internal Line2D To(Line2DStyle style)
        {
            Line2D line = new Line2D();
            line.Color = style.Color;
            line.Dashes = style.Dashes;
            line.LineJoin = style.LineJoin;
            line.MaxZoom = style.MaxZoom;
            line.MinZoom = style.MinZoom;
            line.Width = style.Width;

            line.X = this.X.DecodeDelta().ConvertFromLongArray(Scene2DRTreeSerializer.ConversionFactor);
            line.Y = this.Y.DecodeDelta().ConvertFromLongArray(Scene2DRTreeSerializer.ConversionFactor);

            line.MinX = int.MaxValue;
            line.MaxX = int.MinValue;
            for (int idx = 0; idx < line.X.Length; idx++)
            {
                if (line.X[idx] > line.MaxX)
                {
                    line.MaxX = line.X[idx];
                }
                if (line.X[idx] < line.MinX)
                {
                    line.MinX = line.X[idx];
                }
            }
            line.MinY = int.MaxValue;
            line.MaxY = int.MinValue;
            for (int idx = 0; idx < line.Y.Length; idx++)
            {
                if (line.Y[idx] > line.MaxY)
                {
                    line.MaxY = line.Y[idx];
                }
                if (line.Y[idx] < line.MinY)
                {
                    line.MinY = line.Y[idx];
                }
            }
            return line;
        }
    }

    /// <summary>
    /// Describes a LineText2D object with reference to a style.
    /// </summary>
    [ProtoContract]
    public class LineText2DEntry
    {
        ///// <summary>
        ///// Gets or sets the id.
        ///// </summary>
        //[ProtoMember(1)]
        //public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        [ProtoMember(1)]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        [ProtoMember(2)]
        public long[] X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        [ProtoMember(3)]
        public long[] Y { get; set; }

        /// <summary>
        /// Gets or sets the style id.
        /// </summary>
        [ProtoMember(4)]
        public ushort StyleId { get; set; }

        /// <summary>
        /// Converts into a basic entry.
        /// </summary>
        /// <param name="lineText"></param>
        /// <param name="styleId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static LineText2DEntry From(uint id, LineText2D lineText, ushort styleId)
        {
            LineText2DEntry entry = new LineText2DEntry();
            //entry.Id = id;
            entry.Text = lineText.Text;
            entry.X = lineText.X.ConvertToLongArray(Scene2DRTreeSerializer.ConversionFactor).EncodeDelta();
            entry.Y = lineText.Y.ConvertToLongArray(Scene2DRTreeSerializer.ConversionFactor).EncodeDelta();
            entry.StyleId = styleId;
            return entry;
        }

        /// <summary>
        /// Converts this basic entry into a scene object.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        internal LineText2D To(LineText2DStyle style)
        {
            LineText2D lineText = new LineText2D();
            lineText.Color = style.Color;
            lineText.HaloColor = style.HaloColor;
            lineText.HaloRadius = style.HaloRadius;
            lineText.MaxZoom = style.MaxZoom;
            lineText.MinZoom = style.MinZoom;
            lineText.Size = style.Size;

            lineText.Text = this.Text;
            lineText.X = this.X.DecodeDelta().ConvertFromLongArray(Scene2DRTreeSerializer.ConversionFactor);
            lineText.Y = this.Y.DecodeDelta().ConvertFromLongArray(Scene2DRTreeSerializer.ConversionFactor);

            lineText.MinX = int.MaxValue;
            lineText.MaxX = int.MinValue;
            for (int idx = 0; idx < lineText.X.Length; idx++)
            {
                if (lineText.X[idx] > lineText.MaxX)
                {
                    lineText.MaxX = lineText.X[idx];
                }
                if (lineText.X[idx] < lineText.MinX)
                {
                    lineText.MinX = lineText.X[idx];
                }
            }
            lineText.MinY = int.MaxValue;
            lineText.MaxY = int.MinValue;
            for (int idx = 0; idx < lineText.Y.Length; idx++)
            {
                if (lineText.Y[idx] > lineText.MaxY)
                {
                    lineText.MaxY = lineText.Y[idx];
                }
                if (lineText.Y[idx] < lineText.MinY)
                {
                    lineText.MinY = lineText.Y[idx];
                }
            }

            return lineText;
        }
    }

    /// <summary>
    /// Describes a Point2D object with reference to a style.
    /// </summary>
    [ProtoContract]
    public class Point2DEntry
    {
        ///// <summary>
        ///// Gets or sets the id.
        ///// </summary>
        //[ProtoMember(1)]
        //public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        [ProtoMember(1)]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        [ProtoMember(2)]
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the style id.
        /// </summary>
        [ProtoMember(3)]
        public ushort StyleId { get; set; }

        /// <summary>
        /// Converts to a basic entry.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="point"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        internal static Point2DEntry From(uint id, Point2D point, ushort styleId)
        {
            Point2DEntry entry = new Point2DEntry();
            //entry.Id = id;
            entry.X = point.X;
            entry.Y = point.Y;
            entry.StyleId = styleId;
            return entry;
        }

        /// <summary>
        /// Converts this basic entry into a scene object.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        internal Point2D To(Point2DStyle style)
        {
            Point2D point = new Point2D();
            point.Color = style.Color;
            point.MaxZoom = style.MaxZoom;
            point.MinZoom = style.MinZoom;
            point.Size = style.Size;

            point.X = this.X;
            point.Y = this.Y;
            return point;
        }
    }

    /// <summary>
    /// Describes a Polygon2D object with reference to a style.
    /// </summary>
    [ProtoContract]
    public class Polygon2DEntry
    {
        ///// <summary>
        ///// Gets or sets the id.
        ///// </summary>
        //[ProtoMember(1)]
        //public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        [ProtoMember(1)]
        public long[] X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        [ProtoMember(2)]
        public long[] Y { get; set; }

        /// <summary>
        /// Gets or sets the style id.
        /// </summary>
        [ProtoMember(3)]
        public ushort StyleId { get; set; }

        /// <summary>
        /// Converts to a basic entry.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="polygon"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        internal static Polygon2DEntry From(uint id, Polygon2D polygon, ushort styleId)
        {
            Polygon2DEntry entry = new Polygon2DEntry();
            //entry.Id = id;
            entry.X = polygon.X.ConvertToLongArray(Scene2DRTreeSerializer.ConversionFactor).EncodeDelta();
            entry.Y = polygon.Y.ConvertToLongArray(Scene2DRTreeSerializer.ConversionFactor).EncodeDelta();
            entry.StyleId = styleId;
            return entry;
        }

        /// <summary>
        /// Converts this basic entry into a scene object.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        internal Polygon2D To(Polygon2DStyle style)
        {
            Polygon2D polygon = new Polygon2D();
            polygon.Color = style.Color;
            polygon.Fill = style.Fill;
            polygon.MaxZoom = style.MaxZoom;
            polygon.MinZoom = style.MinZoom;
            polygon.Width = style.Width;
            polygon.X = this.X.DecodeDelta().ConvertFromLongArray(Scene2DRTreeSerializer.ConversionFactor);
            polygon.Y = this.Y.DecodeDelta().ConvertFromLongArray(Scene2DRTreeSerializer.ConversionFactor);

            polygon.MinX = int.MaxValue;
            polygon.MaxX = int.MinValue;
            for (int idx = 0; idx < polygon.X.Length; idx++)
            {
                if (polygon.X[idx] > polygon.MaxX)
                {
                    polygon.MaxX = polygon.X[idx];
                }
                if (polygon.X[idx] < polygon.MinX)
                {
                    polygon.MinX = polygon.X[idx];
                }
            }
            polygon.MinY = int.MaxValue;
            polygon.MaxY = int.MinValue;
            for (int idx = 0; idx < polygon.Y.Length; idx++)
            {
                if (polygon.Y[idx] > polygon.MaxY)
                {
                    polygon.MaxY = polygon.Y[idx];
                }
                if (polygon.Y[idx] < polygon.MinY)
                {
                    polygon.MinY = polygon.Y[idx];
                }
            }
            return polygon;
        }
    }

    /// <summary>
    /// Describes a Text2D object with reference to a style.
    /// </summary>
    [ProtoContract]
    public class Text2DEntry
    {
        ///// <summary>
        ///// Gets or sets the id.
        ///// </summary>
        //[ProtoMember(1)]
        //public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        [ProtoMember(1)]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        [ProtoMember(2)]
        public double Y { get; set; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        [ProtoMember(3)]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the style id.
        /// </summary>
        [ProtoMember(4)]
        public ushort StyleId { get; set; }

        /// <summary>
        /// Converts to a basic entry.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        internal static Text2DEntry From(uint id, Text2D text, ushort styleId)
        {
            Text2DEntry entry = new Text2DEntry();
            //entry.Id = id;
            entry.Text = text.Text;
            entry.X = text.X;
            entry.Y = text.Y;
            entry.StyleId = styleId;
            return entry;
        }

        /// <summary>
        /// Converts a basic entry into a scene object.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        internal Text2D To(Text2DStyle style)
        {
            Text2D text = new Text2D();
            text.Color = style.Color;
            text.HaloColor = style.HaloColor;
            text.HaloRadius = style.HaloRadius;
            text.MaxZoom = style.MaxZoom;
            text.MinZoom = style.MinZoom;
            text.Size = style.Size;

            text.Text = this.Text;
            text.X = this.X;
            text.Y = this.Y;
            return text;
        }
    }
}
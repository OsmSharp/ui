using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zlib;
using OsmSharp.Collections.SpatialIndexes.Serialization.v1;
using OsmSharp.Math.Primitives;
using ProtoBuf;
using ProtoBuf.Meta;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives.Storage
{
    /// <summary>
    /// A serializer for scene primitive spatial index.
    /// </summary>
    internal class Scene2DPrimitivesSerializer : RTreeStreamSerializer<Scene2DEntry>
    {
        /// <summary>
        /// Holds the compression flag.
        /// </summary>
        private bool _compress;

        /// <summary>
        /// Creates a new scene serializer.
        /// </summary>
        /// <param name="compress"></param>
        public Scene2DPrimitivesSerializer(bool compress)
        {
            _compress = compress;
        }

        /// <summary>
        /// Builds the runtime type model.
        /// </summary>
        /// <param name="typeModel"></param>
        protected override void BuildRuntimeTypeModel(RuntimeTypeModel typeModel)
        {
            typeModel.Add(typeof (PrimitivesCollection), true);
            MetaType iconMetaType = typeModel.Add(typeof (Icon2D), false);
            iconMetaType.Add(1, "X");
            iconMetaType.Add(2, "Y");
            iconMetaType.Add(3, "Image");
            iconMetaType.Add(4, "MinZoom");
            iconMetaType.Add(5, "MaxZoom");

            MetaType imagesMetaType = typeModel.Add(typeof (Image2D), false);
            imagesMetaType.Add(1, "ImageData");
            imagesMetaType.Add(2, "Left");
            imagesMetaType.Add(3, "Right");
            imagesMetaType.Add(4, "Top");
            imagesMetaType.Add(5, "Bottom");
            imagesMetaType.Add(6, "MinZoom");
            imagesMetaType.Add(7, "MaxZoom");

            MetaType lineMetaType = typeModel.Add(typeof (Line2D), false);
            lineMetaType.Add(1, "X");
            lineMetaType.Add(2, "Y");
            lineMetaType.Add(3, "Color");
            lineMetaType.Add(4, "Width");
            lineMetaType.Add(5, "LineJoin");
            lineMetaType.Add(6, "Dashes");
            lineMetaType.Add(7, "MinZoom");
            lineMetaType.Add(8, "MaxZoom");
            lineMetaType.Add(9, "MinX");
            lineMetaType.Add(10, "MaxX");
            lineMetaType.Add(11, "MinY");
            lineMetaType.Add(12, "MaxY");

            MetaType pointMetaType = typeModel.Add(typeof (Point2D), false);
            pointMetaType.Add(1, "X");
            pointMetaType.Add(2, "Y");
            pointMetaType.Add(3, "Color");
            pointMetaType.Add(4, "Size");
            pointMetaType.Add(5, "MinZoom");
            pointMetaType.Add(6, "MaxZoom");

            MetaType polygonMetaType = typeModel.Add(typeof (Polygon2D), false);
            polygonMetaType.Add(1, "X");
            polygonMetaType.Add(2, "Y");
            polygonMetaType.Add(3, "Color");
            polygonMetaType.Add(4, "Width");
            polygonMetaType.Add(5, "Fill");
            polygonMetaType.Add(6, "MinZoom");
            polygonMetaType.Add(7, "MaxZoom");
            polygonMetaType.Add(8, "MinX");
            polygonMetaType.Add(9, "MaxX");
            polygonMetaType.Add(10, "MinY");
            polygonMetaType.Add(11, "MaxY");

            MetaType textMetaType = typeModel.Add(typeof (Text2D), false);
            textMetaType.Add(1, "X");
            textMetaType.Add(2, "Y");
            textMetaType.Add(3, "Text");
            textMetaType.Add(4, "Size");
            textMetaType.Add(5, "MinZoom");
            textMetaType.Add(6, "MaxZoom");
            textMetaType.Add(7, "HaloRadius");
            textMetaType.Add(8, "HaloColor");
            textMetaType.Add(9, "Color");

            MetaType lineTextMetaType = typeModel.Add(typeof(LineText2D), false);
            lineTextMetaType.Add(1, "X");
            lineTextMetaType.Add(2, "Y");
            lineTextMetaType.Add(3, "Text");
            lineTextMetaType.Add(4, "Color");
            lineTextMetaType.Add(5, "Size");
            lineTextMetaType.Add(6, "MinZoom");
            lineTextMetaType.Add(7, "MaxZoom");
            lineTextMetaType.Add(8, "MinX");
            lineTextMetaType.Add(9, "MaxX");
            lineTextMetaType.Add(10, "MinY");
            lineTextMetaType.Add(11, "MaxY");
            lineTextMetaType.Add(12, "HaloRadius");
            lineTextMetaType.Add(13, "HaloColor");

            typeModel.Add(typeof(Icon2DEntry), true);
            typeModel.Add(typeof(Image2DEntry), true);
            typeModel.Add(typeof(Line2DEntry), true);
            typeModel.Add(typeof(Point2DEntry), true);
            typeModel.Add(typeof(Polygon2DEntry), true);
            typeModel.Add(typeof(Text2DEntry), true);
            typeModel.Add(typeof(LineText2DEntry), true);
        }

        /// <summary>
        /// Serializes the actual data.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="data"></param>
        /// <param name="boxes"></param>
        /// <returns></returns>
        protected override byte[] Serialize(RuntimeTypeModel typeModel, List<Scene2DEntry> data,
                                            List<RectangleF2D> boxes)
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
                    icons.Add(new Icon2DEntry()
                                  {
                                      Primitive = (Icon2D)primitive,
                                      Id = data[idx].Id,
                                      Layer = data[idx].Layer
                                  });
                }
                else if (primitive is Image2D)
                {
                    images.Add(new Image2DEntry()
                    {
                        Primitive = (Image2D)primitive,
                        Id = data[idx].Id,
                        Layer = data[idx].Layer
                    });
                }
                else if (primitive is Line2D)
                {
                    lines.Add(new Line2DEntry()
                    {
                        Primitive = (Line2D)primitive,
                        Id = data[idx].Id,
                        Layer = data[idx].Layer
                    });
                }
                else if (primitive is Point2D)
                {
                    points.Add(new Point2DEntry()
                    {
                        Primitive = (Point2D)primitive,
                        Id = data[idx].Id,
                        Layer = data[idx].Layer
                    });
                }
                else if (primitive is Polygon2D)
                {
                    polygons.Add(new Polygon2DEntry()
                    {
                        Primitive = (Polygon2D)primitive,
                        Id = data[idx].Id,
                        Layer = data[idx].Layer
                    });
                }
                else if (primitive is Text2D)
                {
                    texts.Add(new Text2DEntry()
                    {
                        Primitive = (Text2D)primitive,
                        Id = data[idx].Id,
                        Layer = data[idx].Layer
                    });
                }
                else if (primitive is LineText2D)
                {
                    lineTexts.Add(new LineText2DEntry()
                    {
                        Primitive = (LineText2D)primitive,
                        Id = data[idx].Id,
                        Layer = data[idx].Layer
                    });
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
            get { return "ScenePrimitives2D.v1"; }
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

        [ProtoContract]
        public class Icon2DEntry
        {
            [ProtoMember(1)]
            public Icon2D Primitive { get; set; }

            [ProtoMember(2)]
            public int Layer { get; set; }

            [ProtoMember(3)]
            public uint Id { get; set; }
        }

        [ProtoContract]
        public class Image2DEntry
        {
            [ProtoMember(1)]
            public Image2D Primitive { get; set; }

            [ProtoMember(2)]
            public int Layer { get; set; }

            [ProtoMember(3)]
            public uint Id { get; set; }
        }

        [ProtoContract]
        public class Line2DEntry
        {
            [ProtoMember(1)]
            public Line2D Primitive { get; set; }

            [ProtoMember(2)]
            public int Layer { get; set; }

            [ProtoMember(3)]
            public uint Id { get; set; }
        }

        [ProtoContract]
        public class Point2DEntry
        {
            [ProtoMember(1)]
            public Point2D Primitive { get; set; }

            [ProtoMember(2)]
            public int Layer { get; set; }

            [ProtoMember(3)]
            public uint Id { get; set; }
        }

        [ProtoContract]
        public class Polygon2DEntry
        {
            [ProtoMember(1)]
            public Polygon2D Primitive { get; set; }

            [ProtoMember(2)]
            public int Layer { get; set; }

            [ProtoMember(3)]
            public uint Id { get; set; }
        }

        [ProtoContract]
        public class LineText2DEntry
        {
            [ProtoMember(1)]
            public LineText2D Primitive { get; set; }

            [ProtoMember(2)]
            public int Layer { get; set; }

            [ProtoMember(3)]
            public uint Id { get; set; }            
        }

        [ProtoContract]
        public class Text2DEntry
        {
            [ProtoMember(1)]
            public Text2D Primitive { get; set; }

            [ProtoMember(2)]
            public int Layer { get; set; }

            [ProtoMember(3)]
            public uint Id { get; set; }
        }

        /// <summary>
        /// Deserializes the actual data.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="data"></param>
        /// <param name="boxes"></param>
        /// <returns></returns>
        protected override List<Scene2DEntry> DeSerialize(RuntimeTypeModel typeModel, byte[] data, 
            out List<RectangleF2D> boxes)
        {
            // decompress if needed.
            if (_compress)
            {
                data = GZipStream.UncompressBuffer(data);
            }

            // create the memory stream.
            var stream = new MemoryStream(data);
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
                    primitives.Add(new Scene2DEntry()
                                       {
                                           Id = primitive.Id,
                                           Layer = primitive.Layer,
                                           Scene2DPrimitive = primitive.Primitive
                                       });
                }
            }
            if (collection.Images != null)
            {
                foreach (var primitive in collection.Images)
                {
                    primitives.Add(new Scene2DEntry()
                                       {
                                           Id = primitive.Id,
                                           Layer = primitive.Layer,
                                           Scene2DPrimitive = primitive.Primitive
                                       });
                }
            }
            if (collection.Lines != null)
            {
                foreach (var primitive in collection.Lines)
                {
                    primitives.Add(new Scene2DEntry()
                                       {
                                           Id = primitive.Id,
                                           Layer = primitive.Layer,
                                           Scene2DPrimitive = primitive.Primitive
                                       });
                }
            }
            if (collection.Points != null)
            {
                foreach (var primitive in collection.Points)
                {
                    primitives.Add(new Scene2DEntry()
                                       {
                                           Id = primitive.Id,
                                           Layer = primitive.Layer,
                                           Scene2DPrimitive = primitive.Primitive
                                       });
                }
            }
            if (collection.Polygons != null)
            {
                foreach (var primitive in collection.Polygons)
                {
                    primitives.Add(new Scene2DEntry()
                                       {
                                           Id = primitive.Id,
                                           Layer = primitive.Layer,
                                           Scene2DPrimitive = primitive.Primitive
                                       });
                }
            }
            if (collection.Texts != null)
            {
                foreach (var primitive in collection.Texts)
                {
                    primitives.Add(new Scene2DEntry()
                                       {
                                           Id = primitive.Id,
                                           Layer = primitive.Layer,
                                           Scene2DPrimitive = primitive.Primitive
                                       });
                }
            }
            if (collection.LineTexts != null)
            {
                foreach (var primitive in collection.LineTexts)
                {
                    primitives.Add(new Scene2DEntry()
                    {
                        Id = primitive.Id,
                        Layer = primitive.Layer,
                        Scene2DPrimitive = primitive.Primitive
                    });
                }
            }

            // build the boxes list.
            boxes = new List<RectangleF2D>();
            for (int idx = 0; idx < primitives.Count; idx++)
            {
                boxes.Add(primitives[idx].Scene2DPrimitive.GetBox());
            }
            return primitives;
        }
    }
}
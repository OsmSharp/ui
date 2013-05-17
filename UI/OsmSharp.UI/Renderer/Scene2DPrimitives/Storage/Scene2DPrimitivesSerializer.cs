using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OsmSharp.Collections.SpatialIndexes.Serialization.v1;
using OsmSharp.Math.Primitives;
using ProtoBuf.Meta;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives.Storage
{
    /// <summary>
    /// A serializer for scene primitive spatial index.
    /// </summary>
    public class Scene2DPrimitivesSerializer : RTreeStreamSerializer<IScene2DPrimitive>
    {
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
        }

        /// <summary>
        /// Serializes the actual data.
        /// </summary>
        /// <param name="typeModel"></param>
        /// <param name="data"></param>
        /// <param name="boxes"></param>
        /// <returns></returns>
        protected override byte[] Serialize(RuntimeTypeModel typeModel, List<IScene2DPrimitive> data, 
            List<RectangleF2D> boxes)
        {
            var icons = new List<Icon2D>();
            var images = new List<Image2D>();
            var lines = new List<Line2D>();
            var points = new List<Point2D>();
            var polygons = new List<Polygon2D>();
            var texts = new List<Text2D>();

            foreach (var primitive in data)
            {
                if (primitive is Icon2D)
                {
                    icons.Add((Icon2D)primitive);
                }
                else if (primitive is Image2D)
                {
                    images.Add((Image2D)primitive);
                }
                else if (primitive is Line2D)
                {
                    lines.Add((Line2D)primitive);
                }
                else if (primitive is Point2D)
                {
                    points.Add((Point2D)primitive);
                }
                else if (primitive is Polygon2D)
                {
                    polygons.Add((Polygon2D)primitive);
                }
                else if (primitive is Text2D)
                {
                    texts.Add((Text2D)primitive);
                }
                else
                {
                    throw new Exception("Primitive type not supported by serializer.");
                }
            }

            // build the collection object.
            var collection = new PrimitivesCollection();
            collection.Icons = icons.ToArray();
            collection.Images = images.ToArray();
            collection.Lines = lines.ToArray();
            collection.Points = points.ToArray();
            collection.Polygons = polygons.ToArray();
            collection.Texts = texts.ToArray();

            // create the memory stream.
            var stream = new MemoryStream();
            typeModel.Serialize(stream, collection);
            return stream.ToArray();
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
        internal class PrimitivesCollection
        {
            /// <summary>
            /// Holds the icons.
            /// </summary>
            public Icon2D[] Icons { get; set; }

            /// <summary>
            /// Holds the images.
            /// </summary>
            public Image2D[] Images { get; set; }

            /// <summary>
            /// Holds the lines.
            /// </summary>
            public Line2D[] Lines { get; set; }

            /// <summary>
            /// Holds the points.
            /// </summary>
            public Point2D[] Points { get; set; }

            /// <summary>
            /// Holds the polygons.
            /// </summary>
            public Polygon2D[] Polygons { get; set; }

            /// <summary>
            /// Holds the texts.
            /// </summary>
            public Text2D[] Texts { get; set; }
        }

        protected override List<IScene2DPrimitive> DeSerialize(RuntimeTypeModel typeModel, byte[] data, out List<RectangleF2D> boxes)
        {
            throw new NotImplementedException();
        }
    }
}
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
using ProtoBuf;
using ProtoBuf.Meta;
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.IO;

namespace OsmSharp.UI.Renderer.Scene.Storage.Styled
{
    /// <summary>
    /// Serializer for a styled scene that acts as a wrapper around an RTreeStreamSerializer.
    /// </summary>
    class Scene2DStyledSerializer
    {
        /// <summary>
        /// Holds the compress flag.
        /// </summary>
        private bool _compress;

        /// <summary>
        /// Creates a new styled serializer.
        /// </summary>
        /// <param name="compress"></param>
        public Scene2DStyledSerializer(bool compress)
        {
            _compress = compress;
        }

        /// <summary>
        /// Serializes a stream with a style index.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="index"></param>
        public void Serialize(Stream stream, RTreeMemoryIndex<Scene2DEntry> index)
        {
            Scene2DStyledIndex styleIndex = new Scene2DStyledIndex();
            Scene2DRTreeSerializer serializer = new Scene2DRTreeSerializer(_compress, styleIndex);

            // serialize the tree and fill the styleindex.
            MemoryStream rTreeStream = new MemoryStream();
            serializer.Serialize(rTreeStream, index);
            
            // serialize the index.
            MemoryStream indexStream = new MemoryStream();
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(Scene2DStyledIndex), true);

            typeModel.Add(typeof(Icon2DStyle), true);
            typeModel.Add(typeof(Image2DStyle), true);
            typeModel.Add(typeof(Line2DStyle), true);
            typeModel.Add(typeof(Point2DStyle), true);
            typeModel.Add(typeof(Polygon2DStyle), true);
            typeModel.Add(typeof(Text2DStyle), true);
            typeModel.Add(typeof(LineText2DStyle), true);

            typeModel.Serialize(indexStream, styleIndex);

            // write to the final stream.
            byte[] indexSizeBytes = BitConverter.GetBytes((int)indexStream.Length);
            stream.Write(indexSizeBytes, 0, indexSizeBytes.Length);
            indexStream.Seek(0, SeekOrigin.Begin);
            indexStream.WriteTo(stream);
            rTreeStream.WriteTo(stream);
            indexStream.Dispose();
            rTreeStream.Dispose();
        }

        /// <summary>
        /// Deserializes a scene with a style index.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lazy"></param>
        /// <returns></returns>
        public Scene2DStyledSource Deserialize(Stream stream, bool lazy = true)
        {
			// read index bytes.
            byte[] indexSizeBytes = new byte[4];
            stream.Read(indexSizeBytes, 0, 4);
            int length = BitConverter.ToInt32(indexSizeBytes, 0);

            // move to the index position.
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(Scene2DStyledIndex), true); // the styles index.

            typeModel.Add(typeof(Icon2DStyle), true);
            typeModel.Add(typeof(Image2DStyle), true);
            typeModel.Add(typeof(Line2DStyle), true);
            typeModel.Add(typeof(Point2DStyle), true);
            typeModel.Add(typeof(Polygon2DStyle), true);
            typeModel.Add(typeof(Text2DStyle), true);
            typeModel.Add(typeof(LineText2DStyle), true);

            // deserialize the index.
            byte[] indexBytes = new byte[length];
            stream.Read(indexBytes, 0, length);
            MemoryStream cappedStream = new MemoryStream(indexBytes);
            Scene2DStyledIndex styleIndex =
                typeModel.Deserialize(cappedStream, null, typeof(Scene2DStyledIndex)) as Scene2DStyledIndex;

            // initialize the serializer.
            Scene2DRTreeSerializer serializer = new Scene2DRTreeSerializer(_compress, styleIndex);
            return new Scene2DStyledSource(
                serializer.Deserialize(new LimitedStream(stream), true));
        }
    }
}
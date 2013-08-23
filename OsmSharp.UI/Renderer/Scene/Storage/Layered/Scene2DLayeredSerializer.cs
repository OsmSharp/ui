// OsmSharp - OpenStreetMap tools & library.
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

namespace OsmSharp.UI.Renderer.Scene.Storage.Layered
{
    /// <summary>
    /// Serializer for a layered scene.
    /// </summary>
    public class Scene2DLayeredSerializer 
    {
        /// <summary>
        /// Serializes the given scene to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="scenes"></param>
        /// <param name="zoomCutoffs"></param>
        /// <param name="compress"></param>
        public void Serialize(Stream stream, Scene2DSimple nonSimplifiedScene,
            Scene2DSimple[] scenes, List<float> zoomCutoffs, bool compress)
        {
            // first move to store the position of the index.
            stream.Write(new byte[4], 0, 4);

            // serialize all the scenes.
            List<long> sceneIndexes = new List<long>();
            MemoryStream sceneStream;
            for(int idx = 0; idx < scenes.Length; idx++)
            {
                if(scenes[idx] != null)
                {
                    sceneStream = new MemoryStream();
                    scenes[idx].SerializeStyled(sceneStream, compress);

                    sceneIndexes.Add(stream.Position);
                    sceneStream.WriteTo(stream);
                    sceneStream.Dispose();
                }
            }
            sceneIndexes.Add(stream.Position);

            // serialize the non-simplified scene.
            sceneStream = new MemoryStream();
            nonSimplifiedScene.SerializeStyled(sceneStream, compress);
            sceneStream.WriteTo(stream);
            sceneStream.Dispose();

            // serialize the index.
            Scene2DLayeredIndex sceneIndex = new Scene2DLayeredIndex();
            sceneIndex.SceneCutoffs = zoomCutoffs.ToArray();
            sceneIndex.SceneIndexes = sceneIndexes.ToArray();
            int sceneIndexIndex = (int)stream.Position;

            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(Scene2DLayeredIndex), true); // the tile metadata.
            typeModel.Serialize(stream, sceneIndex);

            // write the position.
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(sceneIndexIndex), 0, 4);
            stream.Flush();
        }

        /// <summary>
        /// Deserializes the given stream into a deserializable scene.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Scene2DLayeredSource DeSerialize(Stream stream)
        {
            byte[] intBytes = new byte[4];
            stream.Read(intBytes, 0, 4);
            int sceneIndexIndex = BitConverter.ToInt32(intBytes, 0);

            // move to the index position.
            stream.Seek(sceneIndexIndex, SeekOrigin.Begin);
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(Scene2DLayeredIndex), true); // the tile metadata.
            Scene2DLayeredIndex index =
                typeModel.Deserialize(stream, null, typeof(Scene2DLayeredIndex)) as Scene2DLayeredIndex;

            return new Scene2DLayeredSource(stream, index);
        }

        /// <summary>
        /// Represents the index of the different layered scenes.
        /// </summary>
        [ProtoContract]
        public class Scene2DLayeredIndex
        {
            /// <summary>
            /// Gets/sets the scene cutoffs.
            /// </summary>
            [ProtoMember(1)]
            public float[] SceneCutoffs { get; set; }

            /// <summary>
            /// Gets/sets the scene indexes.
            /// </summary>
            [ProtoMember(2)]
            public long[] SceneIndexes { get; set; }
        }
    }
}

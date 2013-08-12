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
using System.Linq;
using System.Text;
using ProtoBuf;
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;

namespace OsmSharp.UI.Renderer.Scene.Storage
{
    /// <summary>
    /// Represents an entry or object in a Scene2D.
    /// </summary>
    [ProtoContract]
    internal class Scene2DEntry
    {
        /// <summary>
        /// Gets or sets the id of this object.
        /// </summary>
        [ProtoMember(1)]
        public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        [ProtoMember(2)]
        public int Layer { get; set; }

        /// <summary>
        /// Gets or sets the primitive.
        /// </summary>
        [ProtoMember(3)]
        public IScene2DPrimitive Scene2DPrimitive { get; set; }
    }
}

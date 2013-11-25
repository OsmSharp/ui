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

using ProtoBuf;

namespace OsmSharp.UI.Renderer.Scene.Primitives
{
    /// <summary>
    /// Represents an abstract scene object.
    /// </summary>
    [ProtoContract]
    internal abstract class SceneObject
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        [ProtoMember(1)]
        public SceneObjectType Enum { get; protected set; }

        /// <summary>
        /// Gets or sets the geometry id.
        /// </summary>
        [ProtoMember(2)]
        public uint GeoId { get; set; }

        /// <summary>
        /// Gets or sets the style id.
        /// </summary>
        [ProtoMember(3)]
        public uint StyleId { get; set; }
    }
}

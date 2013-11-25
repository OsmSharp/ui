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

namespace OsmSharp.UI.Renderer.Scene.Styles
{
    /// <summary>
    /// Style for a point.
    /// </summary>
    [ProtoContract]
    public class StylePoint
    {
        /// <summary>
        /// Gets or setss the color.
        /// </summary>
        [ProtoMember(1)]
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        [ProtoMember(2)]
        public float Size { get; set; }

        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        [ProtoMember(3)]
        public uint Layer { get; set; }

        /// <summary>
        /// Gets or sets the minzoom.
        /// </summary>
        [ProtoMember(4)]
        public float MinZoom { get; set; }

        /// <summary>
        /// Gets or sets the minzoom.
        /// </summary>
        [ProtoMember(5)]
        public float MaxZoom { get; set; }

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Color.GetHashCode() ^
                this.Size.GetHashCode() ^
                this.Layer.GetHashCode() ^
                this.MinZoom.GetHashCode() ^
                this.MaxZoom.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is StylePoint)
            {
                return (obj as StylePoint).Color == this.Color &&
                    (obj as StylePoint).Size == this.Size &&
                    (obj as StylePoint).Layer == this.Layer &&
                    (obj as StylePoint).MinZoom == this.MinZoom &&
                    (obj as StylePoint).MaxZoom == this.MaxZoom;
            }
            return false;
        }
    }
}
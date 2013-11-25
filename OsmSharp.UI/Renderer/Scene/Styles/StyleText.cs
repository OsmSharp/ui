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
    /// Style for a point with text.
    /// </summary>
    [ProtoContract]
    public class StyleText
    {
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        [ProtoMember(1)]
        public float Size { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        [ProtoMember(2)]
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets the halo color.
        /// </summary>
        [ProtoMember(3)]
        public int? HaloColor { get; set; }

        /// <summary>
        /// Gets or sets the halo radius.
        /// </summary>
        [ProtoMember(4)]
        public int? HaloRadius { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        [ProtoMember(5)]
        public string Font { get; set; }

        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        [ProtoMember(6)]
        public uint Layer { get; set; }

        /// <summary>
        /// Gets or sets the minzoom.
        /// </summary>
        [ProtoMember(7)]
        public float MinZoom { get; set; }

        /// <summary>
        /// Gets or sets the minzoom.
        /// </summary>
        [ProtoMember(8)]
        public float MaxZoom { get; set; }

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (this.Font == null)
            {
                return this.Color.GetHashCode() ^
                    this.Size.GetHashCode() ^
                    this.HaloColor.GetHashCode() ^
                    this.HaloRadius.GetHashCode() ^
                    this.Layer.GetHashCode() ^
                    this.MinZoom.GetHashCode() ^
                    this.MaxZoom.GetHashCode();
            }
            return this.Color.GetHashCode() ^
                this.Size.GetHashCode() ^
                this.HaloColor.GetHashCode() ^
                this.HaloRadius.GetHashCode() ^
                this.Font.GetHashCode() ^
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
            if (obj is StyleText)
            {
                return (obj as StyleText).Size == this.Size &&
                    (obj as StyleText).Color == this.Color &&
                    (obj as StyleText).HaloRadius == this.HaloRadius &&
                    (obj as StyleText).HaloColor == this.HaloColor &&
                    (obj as StyleText).Font == this.Font &&
                    (obj as StyleText).Layer == this.Layer &&
                    (obj as StyleText).MinZoom == this.MinZoom &&
                    (obj as StyleText).MaxZoom == this.MaxZoom;
            }
            return false;
        }
    }
}
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
using OsmSharp.UI.Renderer.Primitives;

namespace OsmSharp.UI.Renderer.Scene.Styles
{
    /// <summary>
    /// Style for a line.
    /// </summary>
    [ProtoContract]
    public class StyleLine
    {
        /// <summary>
        /// Gets or set the color.
        /// </summary>
        [ProtoMember(1)]
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [ProtoMember(3)]
        public float Width { get; set; }

        /// <summary>
        /// Gets or set the line joine.
        /// </summary>
        [ProtoMember(4)]
        public LineJoin LineJoin { get; set; }

        /// <summary>
        /// Gets or sets teh line dashes.
        /// </summary>
        [ProtoMember(5, Options = global::ProtoBuf.MemberSerializationOptions.Packed)]
        public int[] Dashes { get; set; }

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
            if (this.Dashes == null)
            {
                return this.Color.GetHashCode() ^
                    this.Width.GetHashCode() ^
                    this.LineJoin.GetHashCode() ^
                    this.Layer.GetHashCode() ^
                    this.MinZoom.GetHashCode() ^
                    this.MaxZoom.GetHashCode();
            }
            int hashcode = this.Color.GetHashCode() ^
                this.Width.GetHashCode() ^
                this.LineJoin.GetHashCode() ^
                this.Layer.GetHashCode() ^
                this.MinZoom.GetHashCode() ^
                this.MaxZoom.GetHashCode();
            foreach (int dash in this.Dashes)
            {
                hashcode = hashcode ^ dash.GetHashCode();
            }
            return hashcode;
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is StyleLine)
            {
                if ((obj as StyleLine).Color == this.Color &&
                    (obj as StyleLine).Width == this.Width &&
                    (obj as StyleLine).LineJoin == this.LineJoin &&
                    (obj as StyleLine).Layer == this.Layer &&
                    (obj as StyleLine).MinZoom == this.MinZoom &&
                    (obj as StyleLine).MaxZoom == this.MaxZoom)
                {
                    if (this.Dashes != null)
                    {
                        if ((obj as StyleLine).Dashes == null)
                        {
                            return false;
                        }
                        if (this.Dashes.Length == (obj as StyleLine).Dashes.Length)
                        {
                            for (int idx = 0; idx < this.Dashes.Length; idx++)
                            {
                                if (this.Dashes[idx] != (obj as StyleLine).Dashes[idx])
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                    else
                    {
                        return (obj as StyleLine).Dashes == null;
                    }
                }
            }
            return false;
        }
    }
}
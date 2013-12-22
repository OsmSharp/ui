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

namespace OsmSharp.UI.Renderer.Scene.Simplification
{
    /// <summary>
    /// Represents line text style information.
    /// </summary>
    public class Scene2DStyleLineText : Scene2DStyle
    {
        /// <summary>
        /// Gets or sets the style line id.
        /// </summary>
        public uint StyleLineTextId { get; set; }

        /// <summary>
        /// Gets or sets the text id.
        /// </summary>
        public uint TextId { get; set; }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = (obj as Scene2DStyleLineText);
            if (other != null)
            {
                return other.StyleLineTextId == this.StyleLineTextId &&
                    other.TextId == this.TextId;
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return "Scene2DStyleLineText".GetHashCode() ^
                this.StyleLineTextId.GetHashCode() ^
                this.TextId.GetHashCode();
        }
    }
}
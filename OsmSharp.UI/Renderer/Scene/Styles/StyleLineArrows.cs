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

namespace OsmSharp.UI.Renderer.Scene.Styles
{
    /// <summary>
    /// A style to indicate a series of arrays along a line.
    /// </summary>
    public class StyleLineArrows
    {
        /// <summary>
        /// Gets or set the color.
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets teh line dashes.
        /// </summary>
        public int[] Dashes { get; set; }

        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        public uint Layer { get; set; }

        /// <summary>
        /// Gets or sets the minzoom.
        /// </summary>
        public float MinZoom { get; set; }

        /// <summary>
        /// Gets or sets the minzoom.
        /// </summary>
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
                    this.Layer.GetHashCode() ^
                    this.MinZoom.GetHashCode() ^
                    this.MaxZoom.GetHashCode();
            }
            int hashcode = this.Color.GetHashCode() ^
                this.Width.GetHashCode() ^
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
        public override bool Equals(object obj)
        {
            if (obj is StyleLineArrows)
            {
                if ((obj as StyleLineArrows).Color == this.Color &&
                    (obj as StyleLineArrows).Width == this.Width &&
                    (obj as StyleLineArrows).Layer == this.Layer &&
                    (obj as StyleLineArrows).MinZoom == this.MinZoom &&
                    (obj as StyleLineArrows).MaxZoom == this.MaxZoom)
                {
                    if (this.Dashes != null)
                    {
                        if ((obj as StyleLineArrows).Dashes == null)
                        {
                            return false;
                        }
                        if (this.Dashes.Length == (obj as StyleLineArrows).Dashes.Length)
                        {
                            for (int idx = 0; idx < this.Dashes.Length; idx++)
                            {
                                if (this.Dashes[idx] != (obj as StyleLineArrows).Dashes[idx])
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                    else
                    {
                        return (obj as StyleLineArrows).Dashes == null;
                    }
                }
            }
            return false;
        }
    }
}

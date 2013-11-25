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

namespace OsmSharp.UI.Renderer.Scene
{
    /// <summary>
    /// Zoom ranges.
    /// </summary>
    public class Scene2DZoomRange
    {
        ///// <summary>
        ///// Gets or sets the minimum zoom.
        ///// </summary>
        //[ProtoMember(1)]
        //public float MinZoom { get; set; }

        ///// <summary>
        ///// Gets or sets the maximum zoom.
        ///// </summary>
        //[ProtoMember(2)]
        //public float MaxZoom { get; set; }

        ///// <summary>
        ///// Returns the hashcode for this instance.
        ///// </summary>
        ///// <returns></returns>
        //public override int GetHashCode()
        //{
        //    return this.MinZoom.GetHashCode() ^
        //        this.MaxZoom.GetHashCode();
        //}

        ///// <summary>
        ///// Determines whether the specified System.Object is equal to the current System.Object.
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public override bool Equals(object obj)
        //{
        //    if (obj is Scene2DZoomRange)
        //    {
        //        return (obj as Scene2DZoomRange).MaxZoom == this.MaxZoom &&
        //            (obj as Scene2DZoomRange).MinZoom == this.MinZoom;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Returns true 
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static bool Contains(float minZoom, float maxZoom, float zoom)
        {
            if (minZoom >= zoom || maxZoom < zoom)
            { // outside of zoom bounds!
                return false;
            }
            return true;
        }
    }
}
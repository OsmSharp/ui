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

using System.Collections.Generic;

namespace OsmSharp.UI.Renderer.Scene.Simplification
{
    /// <summary>
    /// Represents a set of styles.
    /// </summary>
    public class Scene2DStylesSet : HashSet<Scene2DStyle>
    {
        /// <summary>
        /// Adds a style line.
        /// </summary>
        /// <param name="styleLineId"></param>
        public void AddStyleLine(uint styleLineId)
        {
            this.Add(new Scene2DStyleLine()
            {
                StyleLineId = styleLineId
            });
        }

        /// <summary>
        /// Adds a style line text.
        /// </summary>
        /// <param name="styleLineTextId"></param>
        /// <param name="textId"></param>
        public void AddStyleLineText(uint styleLineTextId, uint textId)
        {
            this.Add(new Scene2DStyleLineText()
            {
                StyleLineTextId = styleLineTextId,
                TextId = textId
            });
        }

        /// <summary>
        /// Adds a style polygon.
        /// </summary>
        /// <param name="stylePolygonId"></param>
        public void AddStylePolygon(uint stylePolygonId)
        {
            this.Add(new Scene2DStylePolygon()
            {
                StylePolygonId = stylePolygonId
            });
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Scene2DStylesSet other = obj as Scene2DStylesSet;
            if (other != null)
            { // other object is also a Scene2DStylesSet
                if (other.Count == this.Count)
                { 
                    foreach(var style in this)
                    {
                        if(!other.Contains(style))
                        {
                            return false;
                        }
                    }
                    foreach (var style in other)
                    {
                        if (!this.Contains(style))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashcode = 102348;
            foreach (var style in this)
            {
                hashcode = hashcode ^ style.GetHashCode();
            }
            return hashcode;
        }
    }
}
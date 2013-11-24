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

namespace OsmSharp.UI.Renderer.Scene.Primitives
{
    /// <summary>
    /// Represents an object type.
    /// </summary>
    internal enum SceneObjectType
    {
        /// <summary>
        /// Text object.
        /// </summary>
        TextObject,
        /// <summary>
        /// Polygon object.
        /// </summary>
        PolygonObject,
        /// <summary>
        /// Line object.
        /// </summary>
        LineObject,
        /// <summary>
        /// Line text object.
        /// </summary>
        LineTextObject,
        /// <summary>
        /// Point object.
        /// </summary>
        PointObject,
        /// <summary>
        /// Icon object.
        /// </summary>
        IconObject
    }
}
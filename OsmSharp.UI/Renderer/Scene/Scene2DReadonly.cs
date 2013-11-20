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
using System.IO;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;

namespace OsmSharp.UI.Renderer.Scene
{
    /// <summary>
    /// Contains all objects that need to be rendered.
    /// </summary>
    public abstract class Scene2DReadonly
    {
        /// <summary>
        /// Clear this instance.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Returns the number of objects in this scene.
        /// </summary>
        public abstract int Count
        {
            get;
        }

        /// <summary>
        /// Gets/sets the backcolor of the scene.
        /// </summary>
        public int BackColor { get; set; }

        /// <summary>
        /// Returns true if this scene is readonly.
        /// </summary>
        public abstract bool IsReadOnly { get; }

        /// <summary>
        /// Returns the primitive with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IScene2DPrimitive Get(uint id);

        /// <summary>
        /// Gets all objects in this scene for the specified view sorted according to layer number.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="zoom"></param>
        public abstract IEnumerable<Scene2DPrimitive> Get(View2D view, float zoom);
    }
}
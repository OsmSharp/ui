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

using OsmSharp.Math.Primitives;

namespace OsmSharp.UI.Renderer.Primitives
{
    /// <summary>
    /// A common interface for all scene primitives.
    /// </summary>
    public abstract class Primitive2D
    {
        /// <summary>
        /// Returns true if the object is visible in the view and for the given zoom factor.
        /// </summary>
        /// <returns><c>true</c> if this instance is visible in the specified view; otherwise, <c>false</c>.</returns>
        /// <param name="view">The view.</param>
        /// <param name="zoom">The zoom factor.</param>
        public abstract bool IsVisibleIn(View2D view, float zoom);

        /// <summary>
        /// Returns true if the object is visible in the view.
        /// </summary>
        /// <returns><c>true</c> if this instance is visible in the specified view; otherwise, <c>false</c>.</returns>
        /// <param name="view">The view.</param>
        public abstract bool IsVisibleIn(View2D view);

        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
        public abstract BoxF2D GetBox();

        /// <summary>
        /// Returns the type of this primitive.
        /// </summary>
        public abstract Primitive2DType Primitive2DType { get; }

        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        public uint Layer { get; set; }
    }
}
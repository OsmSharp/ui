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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Renderer
{
    /// <summary>
    /// A wrapper for the target of the renderer.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public class Target2DWrapper<TTarget>
    {
        /// <summary>
        /// Creates a new target wrapper.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Target2DWrapper(TTarget target, float width, float height)
        {
            this.Target = target;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        public TTarget Target { get; set; }

        /// <summary>
        /// Gets/sets the back target.
        /// </summary>
        public TTarget BackTarget { get; set; }

        /// <summary>
        /// Gets/sets some extra data.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets the orginal width in pixels.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets the orignal height in pixels.
        /// </summary>
        public float Height { get; set; }
    }
}

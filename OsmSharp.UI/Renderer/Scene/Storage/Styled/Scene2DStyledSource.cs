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
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UI.Renderer.Scene.Storage.Styled
{
    /// <summary>
    /// A primitives source based on a styled-index scene.
    /// </summary>
    class Scene2DStyledSource : IScene2DPrimitivesSource
    {
        /// <summary>
        /// Holds the serializer.
        /// </summary>
        private readonly ISpatialIndexReadonly<Scene2DEntry> _index;

        /// <summary>
        /// Creates a new scene 2D primitives source.
        /// </summary>
        /// <param name="index"></param>
        public Scene2DStyledSource(ISpatialIndexReadonly<Scene2DEntry> index)
        {
            _index = index;
        }

        /// <summary>
        /// Adds all primitives inside the given box for the given zoom.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="view"></param>
        /// <param name="zoomFactor"></param>
        public void Get(Scene2DSimple scene, View2D view, float zoomFactor)
        {
            // get all primitives.
            IEnumerable<Scene2DEntry> entries = 
                _index.Get(view.OuterBox);

            foreach (var scene2DEntry in entries)
            {
                //scene.AddPrimitive(scene2DEntry.Layer, scene2DEntry.Id, scene2DEntry.Scene2DPrimitive);
            }
        }

        /// <summary>
        /// Clears all cached data from this source.
        /// </summary>
        public void Clear()
        {

        }

        /// <summary>
        /// Disposes all resources associated with this source.
        /// </summary>
        public void Dispose()
        {

        }
    }
}

// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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

using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Tiles;
using OsmSharp.UI.Renderer.Scene;
using System;

namespace OsmSharp.UI.Map.Layers.VectorTiles
{
    /// <summary>
    /// Abstract representation of a vector tile source.
    /// </summary>
    public interface IVectorTileSource : IDisposable
    {
        /// <summary>
        /// Event raised when new data is available in this source.
        /// </summary>
        event Action SourceChanged;

        /// <summary>
        /// Pauses all activity.
        /// </summary>
        void Pause();

        /// <summary>
        /// Returns true if this source is paused.
        /// </summary>
        bool IsPaused
        {
            get;
        }

        /// <summary>
        /// Gets the projection.
        /// </summary>
        IProjection Projection
        {
            get;
        }

        /// <summary>
        /// Resumes the activity in this layer.
        /// </summary>
        void Resume();

        /// <summary>
        /// Closes this source.
        /// </summary>
        void Close();

        /// <summary>
        /// Tries to get a tile in the form of a scene object.
        /// </summary>
        bool TryGet(Tile tile, out Scene2D scene);

        /// <summary>
        /// Tries to peek a tile in the form of a scene object.
        /// </summary>
        bool TryPeek(Tile tile, out Scene2D scene);

        /// <summary>
        /// Prepares this source for the given tile range.
        /// </summary>
        void Prepare(TileRange range);
    }
}
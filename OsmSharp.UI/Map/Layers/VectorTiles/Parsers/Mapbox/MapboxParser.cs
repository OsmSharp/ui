// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.UI.Renderer.Scene;
using System;
using System.IO;

namespace OsmSharp.UI.Map.Layers.VectorTiles.Parsers.Mapbox
{
    /// <summary>
    /// A parser for mapbox vector tiles into an OsmSharp scene object.
    /// </summary>
    public class MapboxParser : IVectorTileParser
    {
        /// <summary>
        /// Parses a vector tile from a raw stream.
        /// </summary>
        /// <param name="stream">The stream containing the raw vector-tile data.</param>
        /// <returns>The scene containing all the resulting geometries.</returns>
        public Scene2D Parse(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}

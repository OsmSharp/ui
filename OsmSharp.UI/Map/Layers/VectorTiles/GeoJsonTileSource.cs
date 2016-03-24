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

using System;
using System.IO;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Geo.Streams.GeoJson;
using OsmSharp.Osm.Tiles;
using OsmSharp.UI.Map.Styles;
using OsmSharp.Math.Geo.Projections;

namespace OsmSharp.UI.Map.Layers.VectorTiles
{
    /// <summary>
    /// A geojson tile source.
    /// </summary>
    public class GeoJsonTileSource : VectorTileSourceBase
    {
        private readonly StyleInterpreter _style;

        /// <summary>
        /// Creates a new geojson tile source.
        /// </summary>
        public GeoJsonTileSource(string url, IProjection projection, StyleInterpreter style, int tileCacheSize = 50) 
            : base(url, projection, tileCacheSize)
        {
            _style = style;
        }

        /// <summary>
        /// Gets a scene from the given stream.
        /// </summary>
        protected override Scene2D Get(Tile tile, Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();
                if (json.Length <= 42)
                {
                    return null;
                }
                var features = json.ToFeatureCollection();

                var scene = new Scene2D(this.Projection, tile.Zoom, false);
                foreach (var feature in features)
                {
                    _style.Translate(scene, this.Projection, feature);
                }
                return scene;
            }
        }
    }
}

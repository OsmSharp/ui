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
using OsmSharp.UI.Renderer;
using System.Collections.Generic;
using OsmSharp.UI.Renderer.Primitives;
using System.Linq;
using OsmSharp.IO.Json;
using OsmSharp.Geo.Features;

namespace OsmSharp.UI.Map.Layers.VectorTiles
{
    /// <summary>
    /// A geojson tile source.
    /// </summary>
    public class GeoJsonTileSource : VectorTileSourceBase
    {
        private readonly StyleInterpreter _style;
        private readonly bool _multiple;

        /// <summary>
        /// Creates a new geojson tile source.
        /// </summary>
        public GeoJsonTileSource(string url, IProjection projection, StyleInterpreter style, int tileCacheSize = 50, bool multiple = false) 
            : base(url, projection, tileCacheSize)
        {
            _style = style;
            _multiple = multiple;
        }

        /// <summary>
        /// Gets a scene from the given stream.
        /// </summary>
        protected override IEnumerable<Primitive2D> Get(Tile tile, Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var jsonReader = new OsmSharp.IO.Json.JsonTextReader(streamReader);

                if (_multiple)
                {
                    return this.ReadMultiLayer(tile, jsonReader);
                }
                return this.ReadSingleLayer(tile, jsonReader);
            }
        }
        
        /// <summary>
        /// Reads a single layer.
        /// </summary>
        public IEnumerable<Primitive2D> ReadSingleLayer(Tile tile, JsonReader jsonReader)
        {
            var features = OsmSharp.Geo.Streams.GeoJson.GeoJsonConverter.ReadFeatureCollection(jsonReader);
            var scene = new Scene2D(this.Projection, tile.Zoom, false);

            foreach (var feature in features)
            {
                _style.Translate(scene, this.Projection, feature);
            }

            var box = tile.Box.Resize(0.001);
            var zoomFactor = (float)this.Projection.ToZoomFactor(tile.Zoom);
            var testView = View2D.CreateFromBounds(
                this.Projection.LatitudeToY(box.MaxLat),
                this.Projection.LongitudeToX(box.MinLon),
                this.Projection.LatitudeToY(box.MinLat),
                this.Projection.LongitudeToX(box.MaxLon));
            return scene.Get(testView, zoomFactor);
        }

        /// <summary>
        /// Reads a multi layer.
        /// </summary>
        public IEnumerable<Primitive2D> ReadMultiLayer(Tile tile, JsonReader jsonReader)
        {
            var scene = new Scene2D(this.Projection, tile.Zoom, false);
            var type = string.Empty;
            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.EndObject)
                { // end of geometry.
                    break;
                }

                if (jsonReader.TokenType == JsonToken.PropertyName)
                {
                    var name = jsonReader.Value;
                    var features = GeoJsonConverter.ReadFeatureCollection(jsonReader);
                    
                    foreach (var feature in features)
                    {
                        feature.Attributes.AddOrReplace("layer", name);
                        _style.Translate(scene, this.Projection, feature);
                    }
                }
            }

            var box = tile.Box.Resize(0.001);
            var zoomFactor = (float)this.Projection.ToZoomFactor(tile.Zoom);
            var testView = View2D.CreateFromBounds(
                this.Projection.LatitudeToY(box.MaxLat),
                this.Projection.LongitudeToX(box.MinLon),
                this.Projection.LatitudeToY(box.MinLat),
                this.Projection.LongitudeToX(box.MaxLon));
            return scene.Get(testView, zoomFactor);
        }
    }
}

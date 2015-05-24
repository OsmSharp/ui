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

using OsmSharp.Geo.Geometries;
using OsmSharp.IO.Json;
using OsmSharp.IO.Json.Linq;

namespace OsmSharp.Geo.Streams.GeoJson
{
    /// <summary>
    /// A GeoJson converter. Converts GeoJson strings from/to Features.
    /// </summary>
    public static class GeoJsonConverter
    {
        /// <summary>
        /// Generates GeoJson for this geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static string ToGeoJson(this LineairRing geometry)
        {
            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter jsonWriter, LineairRing geometry)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("type");
            jsonWriter.WriteValue("Polygon");
            jsonWriter.WritePropertyName("coordinates");
            jsonWriter.WriteStartArray();
            jsonWriter.WriteStartArray();
            foreach(var coordinate in geometry.Coordinates)
            {
                jsonWriter.WriteStartArray();
                jsonWriter.WriteValue(coordinate.Longitude);
                jsonWriter.WriteValue(coordinate.Latitude);
                jsonWriter.WriteEndArray();
            }
            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for this geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static string ToGeoJson(this Polygon geometry)
        {
            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter jsonWriter, Polygon geometry)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("type");
            jsonWriter.WriteValue("Polygon");
            jsonWriter.WritePropertyName("coordinates");
            jsonWriter.WriteStartArray();
            jsonWriter.WriteStartArray();
            foreach (var coordinate in geometry.Ring.Coordinates)
            {
                jsonWriter.WriteStartArray();
                jsonWriter.WriteValue(coordinate.Longitude);
                jsonWriter.WriteValue(coordinate.Latitude);
                jsonWriter.WriteEndArray();
            }
            jsonWriter.WriteEndArray();
            foreach(var hole in geometry.Holes)
            {
                jsonWriter.WriteStartArray();
                foreach (var coordinate in hole.Coordinates)
                {
                    jsonWriter.WriteStartArray();
                    jsonWriter.WriteValue(coordinate.Longitude);
                    jsonWriter.WriteValue(coordinate.Latitude);
                    jsonWriter.WriteEndArray();
                }
                jsonWriter.WriteEndArray();
            }
            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for this geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static string ToGeoJson(this LineString geometry)
        {
            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter jsonWriter, LineString geometry)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("type");
            jsonWriter.WriteValue("LineString");
            jsonWriter.WritePropertyName("coordinates");
            jsonWriter.WriteStartArray();
            foreach (var coordinate in geometry.Coordinates)
            {
                jsonWriter.WriteStartArray();
                jsonWriter.WriteValue(coordinate.Longitude);
                jsonWriter.WriteValue(coordinate.Latitude);
                jsonWriter.WriteEndArray();
            }
            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for this geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static string ToGeoJson(this Point geometry)
        {
            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter jsonWriter, Point geometry)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("type");
            jsonWriter.WriteValue("Point");
            jsonWriter.WritePropertyName("coordinates");
            jsonWriter.WriteStartArray();
            jsonWriter.WriteValue(geometry.Coordinate.Longitude);
            jsonWriter.WriteValue(geometry.Coordinate.Latitude);
            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();
        }
    }
}
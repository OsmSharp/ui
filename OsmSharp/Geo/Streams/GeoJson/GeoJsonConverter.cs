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

using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.IO.Json;
using OsmSharp.IO.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Geo.Streams.GeoJson
{
    /// <summary>
    /// A GeoJson converter. Converts GeoJson strings from/to Features.
    /// </summary>
    public static class GeoJsonConverter
    {
        /// <summary>
        /// Generates GeoJson for the given feature collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="featureCollection"></param>
        public static string ToGeoJson(this FeatureCollection featureCollection)
        {
            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, featureCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given feature collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        internal static void Write(JsonWriter writer, FeatureCollection featureCollection)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("FeatureCollection");
            writer.WritePropertyName("features");
            writer.WriteStartArray();
            foreach(var feature in featureCollection)
            {
                GeoJsonConverter.Write(writer, feature);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given feature.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="feature"></param>
        public static string ToGeoJson(this Feature feature)
        {
            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, feature);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given feature.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        internal static void Write(JsonWriter writer, Feature feature)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Feature");
            writer.WritePropertyName("properties");
            writer.WriteStartObject();
            GeoJsonConverter.Write(writer, feature.Attributes);
            writer.WriteEndObject();
            writer.WritePropertyName("geometry"); 
            if (feature.Geometry is LineairRing)
            {
                GeoJsonConverter.Write(writer, feature.Geometry as LineairRing);
            }
            else if(feature.Geometry is Point)
            {
                GeoJsonConverter.Write(writer, feature.Geometry as Point);
            }
            else if(feature.Geometry is LineString)
            {
                GeoJsonConverter.Write(writer, feature.Geometry as LineString);
            }
            else if(feature.Geometry is Polygon)
            {
                GeoJsonConverter.Write(writer, feature.Geometry as Polygon);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given attribute collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        internal static void Write(JsonWriter writer, GeometryAttributeCollection attributes)
        {
            foreach(var attribute in attributes)
            {
                writer.WritePropertyName(attribute.Key);
                writer.WriteValue(attribute.Value);
            }
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        public static string ToGeoJson(this MultiPoint geometryCollection)
        {
            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometryCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        internal static void Write(JsonWriter writer, MultiPoint geometryCollection)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("MultiPoint");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (Point point in geometryCollection)
            {
                writer.WriteStartArray();
                writer.WriteValue(point.Coordinate.Longitude);
                writer.WriteValue(point.Coordinate.Latitude);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        public static string ToGeoJson(this MultiLineString geometryCollection)
        {
            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometryCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        internal static void Write(JsonWriter writer, MultiLineString geometryCollection)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("MultiLineString");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (LineString geometry in geometryCollection)
            {
                writer.WriteStartArray();
                foreach (var coordinate in geometry.Coordinates)
                {
                    writer.WriteStartArray();
                    writer.WriteValue(coordinate.Longitude);
                    writer.WriteValue(coordinate.Latitude);
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

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
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        public static string ToGeoJson(this MultiPolygon geometryCollection)
        {
            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometryCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        internal static void Write(JsonWriter writer, MultiPolygon geometryCollection)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("MultiPolygon");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (Polygon geometry in geometryCollection)
            {
                writer.WriteStartArray();
                writer.WriteStartArray();
                foreach (var coordinate in geometry.Ring.Coordinates)
                {
                    writer.WriteStartArray();
                    writer.WriteValue(coordinate.Longitude);
                    writer.WriteValue(coordinate.Latitude);
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
                foreach (var hole in geometry.Holes)
                {
                    writer.WriteStartArray();
                    foreach (var coordinate in hole.Coordinates)
                    {
                        writer.WriteStartArray();
                        writer.WriteValue(coordinate.Longitude);
                        writer.WriteValue(coordinate.Latitude);
                        writer.WriteEndArray();
                    }
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter writer, LineairRing geometry)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Polygon");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            writer.WriteStartArray();
            foreach(var coordinate in geometry.Coordinates)
            {
                writer.WriteStartArray();
                writer.WriteValue(coordinate.Longitude);
                writer.WriteValue(coordinate.Latitude);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndArray();
            writer.WriteEndObject();
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
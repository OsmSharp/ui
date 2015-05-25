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
        /// <param name="featureCollection"></param>
        public static string ToGeoJson(this FeatureCollection featureCollection)
        {
            if (featureCollection == null) { throw new ArgumentNullException("featureCollection"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, featureCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given feature collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="featureCollection"></param>
        internal static void Write(JsonWriter writer, FeatureCollection featureCollection)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (featureCollection == null) { throw new ArgumentNullException("featureCollection"); }

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
        /// <param name="feature"></param>
        public static string ToGeoJson(this Feature feature)
        {
            if (feature == null) { throw new ArgumentNullException("feature"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, feature);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given feature.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="feature"></param>
        internal static void Write(JsonWriter writer, Feature feature)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (feature == null) { throw new ArgumentNullException("feature"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Feature");
            writer.WritePropertyName("properties");
            GeoJsonConverter.Write(writer, feature.Attributes);
            writer.WritePropertyName("geometry");
            GeoJsonConverter.Write(writer, feature.Geometry);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given attribute collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="attributes"></param>
        internal static void Write(JsonWriter writer, GeometryAttributeCollection attributes)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (attributes == null) { throw new ArgumentNullException("attributes"); }

            writer.WriteStartObject();
            foreach (var attribute in attributes)
            {
                writer.WritePropertyName(attribute.Key);
                writer.WriteValue(attribute.Value);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometry"></param>
        internal static void Write(JsonWriter writer, Geometry geometry)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            if (geometry is LineairRing)
            {
                GeoJsonConverter.Write(writer, geometry as LineairRing);
            }
            else if (geometry is Point)
            {
                GeoJsonConverter.Write(writer, geometry as Point);
            }
            else if (geometry is LineString)
            {
                GeoJsonConverter.Write(writer, geometry as LineString);
            }
            else if (geometry is Polygon)
            {
                GeoJsonConverter.Write(writer, geometry as Polygon);
            }
            else if (geometry is MultiPoint)
            {
                GeoJsonConverter.Write(writer, geometry as MultiPoint);
            }
            else if (geometry is MultiPolygon)
            {
                GeoJsonConverter.Write(writer, geometry as MultiPolygon);
            }
            else if (geometry is MultiLineString)
            {
                GeoJsonConverter.Write(writer, geometry as MultiLineString);
            }
            else if (geometry is GeometryCollection)
            {
                GeoJsonConverter.Write(writer, geometry as GeometryCollection);
            }
            else
            {
                throw new Exception(string.Format("Unknown geometry of type: {0}", geometry.GetType()));
            }
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="geometryCollection"></param>
        public static string ToGeoJson(this MultiPoint geometryCollection)
        {
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

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
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("MultiPoint");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (var point in geometryCollection)
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
        /// <param name="geometryCollection"></param>
        public static string ToGeoJson(this MultiLineString geometryCollection)
        {
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

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
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("MultiLineString");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (var geometry in geometryCollection)
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
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="geometryCollection"></param>
        public static string ToGeoJson(this MultiPolygon geometryCollection)
        {
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

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
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("MultiPolygon");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (var geometry in geometryCollection)
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
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

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
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter writer, Polygon geometry)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Polygon");
            writer.WritePropertyName("coordinates");
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
            foreach(var hole in geometry.Holes)
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
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for this geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static string ToGeoJson(this LineString geometry)
        {
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter writer, LineString geometry)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("LineString");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (var coordinate in geometry.Coordinates)
            {
                writer.WriteStartArray();
                writer.WriteValue(coordinate.Longitude);
                writer.WriteValue(coordinate.Latitude);
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
        public static string ToGeoJson(this Point geometry)
        {
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter writer, Point geometry)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Point");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            writer.WriteValue(geometry.Coordinate.Longitude);
            writer.WriteValue(geometry.Coordinate.Latitude);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="geometryCollection"></param>
        public static string ToGeoJson(this GeometryCollection geometryCollection)
        {
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometryCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        internal static void Write(JsonWriter writer, GeometryCollection geometryCollection)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("GeometryCollection");
            writer.WritePropertyName("geometries");
            writer.WriteStartArray();
            foreach (var geometry in geometryCollection)
            {
                GeoJsonConverter.Write(writer, geometry);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
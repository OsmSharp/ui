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

using System.Collections.Generic;
using System.IO;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using OsmSharp.Xml.Kml;
using OsmSharp.Xml.Sources;

namespace OsmSharp.Geo.Streams.Kml
{
    /// <summary>
    /// Gpx-stream source.
    /// </summary>
    /// <remarks>
    ///         Folders => ???
    ///         Document => ???    
    ///             Placemark => ???
    ///             Geometry => 
    ///                 Polygon => Polygon
    ///                     InnerBoundary => LineairRing
    ///                     OuterBoundary => LineairRing
    ///                 Point => Point
    ///                 LineString => LineString
    ///                 LineairRing => LineairRing
    ///                 MultiGeometery => MultiX
    /// </remarks>
    public class KmlGeoStreamSource : GeoCollectionStreamSource
    {
        /// <summary>
        /// Holds the stream containing the source-data.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Creates a new Kml-geometry stream.
        /// </summary>
        /// <param name="stream"></param>
        public KmlGeoStreamSource(Stream stream)
            : base(new GeometryCollection())
        {
            _stream = stream;
        }

        /// <summary>
        /// Called when initializing this source.
        /// </summary>
        public override void Initialize()
        {
            // read the kml-data.
            if (!_read) { this.DoReadKml(); }

            base.Initialize();
        }

        #region Read Kml

        /// <summary>
        /// The kml object.
        /// </summary>
        private bool _read = false;

        /// <summary>
        /// Reads the actual Kml.
        /// </summary>
        private void DoReadKml()
        {
            // seek to the beginning of the stream.
            if (_stream.CanSeek) { _stream.Seek(0, SeekOrigin.Begin); }

            // instantiate and load the gpx test document.
            XmlStreamSource source = new XmlStreamSource(_stream);
            KmlDocument document = new KmlDocument(source);
            object kml = document.Kml;

            switch (document.Version)
            {
                case KmlVersion.Kmlv2_1:
                    this.ConvertKml(kml as OsmSharp.Xml.Kml.v2_1.KmlType);
                    break;
                case KmlVersion.Kmlv2_0_response:
                    this.ConvertKml(kml as OsmSharp.Xml.Kml.v2_0_response.kml);
                    break;
                case KmlVersion.Kmlv2_0:
                    this.ConvertKml(kml as OsmSharp.Xml.Kml.v2_0.kml);
                    break;
            }
        }

        /// <summary>
        /// Reads a kml v2.1 object into corresponding geometries.
        /// </summary>
        /// <param name="kmlType"></param>
        private void ConvertKml(Xml.Kml.v2_1.KmlType kmlType)
        {
            this.GeometryCollection.Clear();

            this.ConvertFeature(kmlType.Item);
        }

        /// <summary>
        /// Reads a kml v2.0 response object into corresponding geometries.
        /// </summary>
        /// <param name="kml"></param>
        private void ConvertKml(Xml.Kml.v2_0_response.kml kml)
        {
            this.GeometryCollection.Clear();

            if (kml.Item is OsmSharp.Xml.Kml.v2_0_response.Document)
            {
                this.ConvertDocument(kml.Item as OsmSharp.Xml.Kml.v2_0_response.Document);
            }
            else if (kml.Item is OsmSharp.Xml.Kml.v2_0_response.Folder)
            {
                this.ConvertFolder(kml.Item as OsmSharp.Xml.Kml.v2_0_response.Folder);
            }
            else if (kml.Item is OsmSharp.Xml.Kml.v2_0_response.Placemark)
            {
                this.ConvertPlacemark(kml.Item as OsmSharp.Xml.Kml.v2_0_response.Placemark);
            }
            else if (kml.Item is OsmSharp.Xml.Kml.v2_0_response.Response)
            {
                this.ConvertResponse(kml.Item as OsmSharp.Xml.Kml.v2_0_response.Response);
            }
        }

        /// <summary>
        /// Reads a kml v2.0 object into corresponding geometries.
        /// </summary>
        /// <param name="kml"></param>
        private void ConvertKml(OsmSharp.Xml.Kml.v2_0.kml kml)
        {
            this.GeometryCollection.Clear();

            if (kml.Item is OsmSharp.Xml.Kml.v2_0.Document)
            {
                this.ConvertDocument(kml.Item as OsmSharp.Xml.Kml.v2_0.Document);
            }
            else if (kml.Item is OsmSharp.Xml.Kml.v2_0.Folder)
            {
                this.ConvertFolder(kml.Item as OsmSharp.Xml.Kml.v2_0.Folder);
            }
            else if (kml.Item is OsmSharp.Xml.Kml.v2_0.Placemark)
            {
                this.ConvertPlacemark(kml.Item as OsmSharp.Xml.Kml.v2_0.Placemark);
            }
        }

        #endregion

        #region v2.0

        /// <summary>
        /// Converts a placemark into an osm object.
        /// </summary>
        /// <param name="placemark"></param>
        /// <returns></returns>
        private void ConvertPlacemark(OsmSharp.Xml.Kml.v2_0.Placemark placemark)
        {
            for (int idx = 0; idx < placemark.Items.Length; idx++)
            {
                switch (placemark.ItemsElementName[idx])
                {
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType1.LineString:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertLineString(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0.LineString));
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType1.MultiGeometry:
                        this.ConvertMultiGeometry(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0.MultiGeometry);
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType1.MultiLineString:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiLineString(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0.MultiLineString));
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType1.MultiPoint:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiPoint(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0.MultiPoint));
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType1.MultiPolygon:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiPolygon(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0.MultiPolygon));
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType1.Point:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertPoint(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0.Point));
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType1.Polygon:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertPolygon(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0.Polygon));
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private static Polygon ConvertPolygon(OsmSharp.Xml.Kml.v2_0.Polygon polygon)
        {
            LineairRing inner = KmlGeoStreamSource.ConvertLinearRing(polygon.innerBoundaryIs.LinearRing);
            LineairRing outer = KmlGeoStreamSource.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing);

            return new Polygon(outer, new LineairRing[] { inner });
        }

        /// <summary>
        /// Converts a lineairring into an osm object.
        /// </summary>
        /// <param name="linearRing"></param>
        /// <returns></returns>
        private static LineairRing ConvertLinearRing(OsmSharp.Xml.Kml.v2_0.LinearRing linearRing)
        {
            // convert the coordinates.
            IList<GeoCoordinate> coordinates = KmlGeoStreamSource.ConvertCoordinates(linearRing.coordinates);

            // create the ring.
            LineairRing ring = new LineairRing(coordinates);
            ring.Attributes = new SimpleGeometryAttributeCollection();
            ring.Attributes.Add("id", linearRing.id);

            return ring;
        }

        /// <summary>
        /// Converts a point into an osm object.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private static Point ConvertPoint(OsmSharp.Xml.Kml.v2_0.Point point)
        {
            // convert the coordiantes.
            IList<GeoCoordinate> coordinates = KmlGeoStreamSource.ConvertCoordinates(point.coordinates);

            // create the point.
            Point pointGeometry = new Point(coordinates[0]);
            pointGeometry.Attributes = new SimpleGeometryAttributeCollection();
            if (point.altitudeModeSpecified) { pointGeometry.Attributes.Add("altitude", point.altitudeMode); }
            if (point.extrudeSpecified) { pointGeometry.Attributes.Add("extrude", point.extrude); }
            if (point.id != null) { pointGeometry.Attributes.Add("id", point.id); }

            return pointGeometry;
        }

        /// <summary>
        /// Converts a multipolygon into osm objects.
        /// </summary>
        /// <param name="multiPolygon"></param>
        /// <returns></returns>
        private static MultiPolygon ConvertMultiPolygon(OsmSharp.Xml.Kml.v2_0.MultiPolygon multiPolygon)
        {
            return new MultiPolygon(new Polygon[] { KmlGeoStreamSource.ConvertPolygon(multiPolygon.Polygon) });
        }

        /// <summary>
        /// Converts a multipoint to osm objects.
        /// </summary>
        /// <param name="multiPoint"></param>
        /// <returns></returns>
        private static MultiPoint ConvertMultiPoint(OsmSharp.Xml.Kml.v2_0.MultiPoint multiPoint)
        {
            return new MultiPoint(new Point[] { KmlGeoStreamSource.ConvertPoint(multiPoint.Point) });
        }

        /// <summary>
        /// Converts a multilinestring to osm objects.
        /// </summary>
        /// <param name="multiLineString"></param>
        /// <returns></returns>
        private static MultiLineString ConvertMultiLineString(OsmSharp.Xml.Kml.v2_0.MultiLineString multiLineString)
        {
            return new MultiLineString(new LineString[] { KmlGeoStreamSource.ConvertLineString(multiLineString.LineString) });
        }

        /// <summary>
        /// Converts a multigeometry to osm objects.
        /// </summary>
        /// <param name="multiGeometry"></param>
        /// <returns></returns>
        private void ConvertMultiGeometry(OsmSharp.Xml.Kml.v2_0.MultiGeometry multiGeometry)
        {
            for (int idx = 0; idx < multiGeometry.Items.Length; idx++)
            {
                switch (multiGeometry.ItemsElementName[idx])
                {
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType.LineString:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertLineString(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0.LineString));
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType.MultiGeometry:
                        this.ConvertMultiGeometry(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0.MultiGeometry);
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType.MultiLineString:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiLineString(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0.MultiLineString));
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType.MultiPoint:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiPoint(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0.MultiPoint));
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType.MultiPolygon:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiPolygon(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0.MultiPolygon));
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType.Point:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertPoint(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0.Point));
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType.Polygon:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertPolygon(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0.Polygon));
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a linestring to osm objects.
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns></returns>
        private static LineString ConvertLineString(OsmSharp.Xml.Kml.v2_0.LineString lineString)
        {
            // convert the coordinates.
            IList<GeoCoordinate> coordinates = KmlGeoStreamSource.ConvertCoordinates(lineString.coordinates);

            // create the ring.
            LineString lineStringGeometry = new LineString(coordinates);
            lineStringGeometry.Attributes = new SimpleGeometryAttributeCollection();
            lineStringGeometry.Attributes.Add("id", lineString.id);

            return lineStringGeometry;
        }

        /// <summary>
        /// Converts a folder into an osm object.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private void ConvertFolder(OsmSharp.Xml.Kml.v2_0.Folder folder)
        {
            for (int idx = 0; idx < folder.Items.Length; idx++)
            {
                switch (folder.ItemsElementName[idx])
                {
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType2.Document:
                        this.ConvertDocument(folder.Items[idx] as OsmSharp.Xml.Kml.v2_0.Document);
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType2.Folder:
                        this.ConvertFolder(folder.Items[idx] as OsmSharp.Xml.Kml.v2_0.Folder);
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType2.Placemark:
                        this.ConvertPlacemark(folder.Items[idx] as OsmSharp.Xml.Kml.v2_0.Placemark);
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a document into osm elements.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private void ConvertDocument(OsmSharp.Xml.Kml.v2_0.Document document)
        {
            for (int idx = 0; idx < document.Items.Length; idx++)
            {
                switch (document.ItemsElementName[idx])
                {
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType3.Document:
                        this.ConvertDocument(document.Items[idx] as OsmSharp.Xml.Kml.v2_0.Document);
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType3.Folder:
                        this.ConvertFolder(document.Items[idx] as OsmSharp.Xml.Kml.v2_0.Folder);
                        break;
                    case OsmSharp.Xml.Kml.v2_0.ItemsChoiceType3.Placemark:
                        this.ConvertPlacemark(document.Items[idx] as OsmSharp.Xml.Kml.v2_0.Placemark);
                        break;
                }
            }
        }

        #endregion

        #region v2.0.response


        /// <summary>
        /// Converts a response into an osm object.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private void ConvertResponse(OsmSharp.Xml.Kml.v2_0_response.Response response)
        {
            foreach (object item in response.Items)
            {
                if (item is OsmSharp.Xml.Kml.v2_0_response.Document)
                {
                    this.ConvertDocument(item as OsmSharp.Xml.Kml.v2_0_response.Document);
                }
                else if (item is OsmSharp.Xml.Kml.v2_0_response.Folder)
                {
                    this.ConvertFolder(item as OsmSharp.Xml.Kml.v2_0_response.Folder);
                }
                else if (item is OsmSharp.Xml.Kml.v2_0_response.Placemark)
                {
                    this.ConvertPlacemark(item as OsmSharp.Xml.Kml.v2_0_response.Placemark);
                }
                else if (item is OsmSharp.Xml.Kml.v2_0_response.Response)
                {
                    this.ConvertResponse(item as OsmSharp.Xml.Kml.v2_0_response.Response);
                }
            }
        }

        /// <summary>
        /// Converts a placemark into an osm object.
        /// </summary>
        /// <param name="placemark"></param>
        /// <returns></returns>
        private void ConvertPlacemark(OsmSharp.Xml.Kml.v2_0_response.Placemark placemark)
        {
            for (int idx = 0; idx < placemark.Items.Length; idx++)
            {
                switch (placemark.ItemsElementName[idx])
                {
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType1.LineString:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertLineString(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.LineString));
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiGeometry:
                        this.ConvertMultiGeometry(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.MultiGeometry);
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiLineString:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiLineString(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.MultiLineString));
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiPoint:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiPoint(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.MultiPoint));
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiPolygon:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiPolygon(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.MultiPolygon));
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType1.Point:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertPoint(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.Point));
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType1.Polygon:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertPolygon(placemark.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.Polygon));
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private static Polygon ConvertPolygon(OsmSharp.Xml.Kml.v2_0_response.Polygon polygon)
        {
            LineairRing inner = KmlGeoStreamSource.ConvertLinearRing(polygon.innerBoundaryIs.LinearRing);
            LineairRing outer = KmlGeoStreamSource.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing);

            return new Polygon(outer, new LineairRing[] { inner });
        }

        /// <summary>
        /// Converts a lineairring into an osm object.
        /// </summary>
        /// <param name="linearRing"></param>
        /// <returns></returns>
        private static LineairRing ConvertLinearRing(OsmSharp.Xml.Kml.v2_0_response.LinearRing linearRing)
        {
            // convert the coordinates.
            IList<GeoCoordinate> coordinates = KmlGeoStreamSource.ConvertCoordinates(linearRing.coordinates);

            // create the ring.
            LineairRing ring = new LineairRing(coordinates);
            ring.Attributes = new SimpleGeometryAttributeCollection();
            ring.Attributes.Add("id", linearRing.id);

            return ring;
        }

        /// <summary>
        /// Converts a point into an osm object.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private static Point ConvertPoint(OsmSharp.Xml.Kml.v2_0_response.Point point)
        {
            // convert the coordiantes.
            IList<GeoCoordinate> coordinates = KmlGeoStreamSource.ConvertCoordinates(point.coordinates);

            // create the point.
            Point pointGeometry = new Point(coordinates[0]);
            pointGeometry.Attributes = new SimpleGeometryAttributeCollection();
            if (point.altitudeModeSpecified) { pointGeometry.Attributes.Add("altitude", point.altitudeMode); }
            if (point.extrudeSpecified) { pointGeometry.Attributes.Add("extrude", point.extrude); }
            if (point.id != null) { pointGeometry.Attributes.Add("id", point.id); }

            return pointGeometry;
        }

        /// <summary>
        /// Converts a multipolygon into osm objects.
        /// </summary>
        /// <param name="multiPolygon"></param>
        /// <returns></returns>
        private static MultiPolygon ConvertMultiPolygon(OsmSharp.Xml.Kml.v2_0_response.MultiPolygon multiPolygon)
        {
            return new MultiPolygon(new Polygon[] { KmlGeoStreamSource.ConvertPolygon(multiPolygon.Polygon) });
        }

        /// <summary>
        /// Converts a multipoint to osm objects.
        /// </summary>
        /// <param name="multiPoint"></param>
        /// <returns></returns>
        private static MultiPoint ConvertMultiPoint(OsmSharp.Xml.Kml.v2_0_response.MultiPoint multiPoint)
        {
            return new MultiPoint(new Point[] { KmlGeoStreamSource.ConvertPoint(multiPoint.Point) });
        }

        /// <summary>
        /// Converts a multilinestring to osm objects.
        /// </summary>
        /// <param name="multiLineString"></param>
        /// <returns></returns>
        private static MultiLineString ConvertMultiLineString(OsmSharp.Xml.Kml.v2_0_response.MultiLineString multiLineString)
        {
            return new MultiLineString(new LineString[] { KmlGeoStreamSource.ConvertLineString(multiLineString.LineString) });
        }

        /// <summary>
        /// Converts a multigeometry to osm objects.
        /// </summary>
        /// <param name="multiGeometry"></param>
        /// <returns></returns>
        private void ConvertMultiGeometry(OsmSharp.Xml.Kml.v2_0_response.MultiGeometry multiGeometry)
        {
            for (int idx = 0; idx < multiGeometry.Items.Length; idx++)
            {
                switch (multiGeometry.ItemsElementName[idx])
                {
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType.LineString:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertLineString(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.LineString));
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType.MultiGeometry:
                        this.ConvertMultiGeometry(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.MultiGeometry);
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType.MultiLineString:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiLineString(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.MultiLineString));
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType.MultiPoint:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiPoint(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.MultiPoint));
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType.MultiPolygon:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertMultiPolygon(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.MultiPolygon));
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType.Point:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertPoint(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.Point));
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType.Polygon:
                        this.GeometryCollection.Add(
                            KmlGeoStreamSource.ConvertPolygon(multiGeometry.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.Polygon));
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a linestring to osm objects.
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns></returns>
        private static LineString ConvertLineString(OsmSharp.Xml.Kml.v2_0_response.LineString lineString)
        {
            // convert the coordinates.
            IList<GeoCoordinate> coordinates = KmlGeoStreamSource.ConvertCoordinates(lineString.coordinates);

            // create the ring.
            LineString lineStringGeometry = new LineString(coordinates);
            lineStringGeometry.Attributes = new SimpleGeometryAttributeCollection();
            lineStringGeometry.Attributes.Add("id", lineString.id);

            return lineStringGeometry;
        }

        /// <summary>
        /// Converts a folder into an osm object.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private void ConvertFolder(OsmSharp.Xml.Kml.v2_0_response.Folder folder)
        {
            for (int idx = 0; idx < folder.Items.Length; idx++)
            {
                switch (folder.ItemsElementName[idx])
                {
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType2.Document:
                        this.ConvertDocument(folder.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.Document);
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType2.Folder:
                        this.ConvertFolder(folder.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.Folder);
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType2.Placemark:
                        this.ConvertPlacemark(folder.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.Placemark);
                        break;
                }
            }
        }

        /// <summary>
        /// Converts a document into osm elements.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private void ConvertDocument(OsmSharp.Xml.Kml.v2_0_response.Document document)
        {
            for (int idx = 0; idx < document.Items.Length; idx++)
            {
                switch (document.ItemsElementName[idx])
                {
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType3.Document:
                        this.ConvertDocument(document.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.Document);
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType3.Folder:
                        this.ConvertFolder(document.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.Folder);
                        break;
                    case OsmSharp.Xml.Kml.v2_0_response.ItemsChoiceType3.Placemark:
                        this.ConvertPlacemark(document.Items[idx] as OsmSharp.Xml.Kml.v2_0_response.Placemark);
                        break;
                }
            }
        }

        #endregion

        #region v2.1

        /// <summary>
        /// Converts all the features into osm elements.
        /// </summary>
        /// <param name="featureType"></param>
        private void ConvertFeatures(OsmSharp.Xml.Kml.v2_1.FeatureType[] featureType)
        {
            foreach (OsmSharp.Xml.Kml.v2_1.FeatureType feature in featureType)
            {
                this.ConvertFeature(feature);
            }
        }

        /// <summary>
        /// Converts a feature and all it's contents to osm elements.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private void ConvertFeature(OsmSharp.Xml.Kml.v2_1.FeatureType feature)
        {
            if (feature is OsmSharp.Xml.Kml.v2_1.ContainerType)
            {
                this.ConvertContainer(feature as OsmSharp.Xml.Kml.v2_1.ContainerType);
            }
            else if (feature is OsmSharp.Xml.Kml.v2_1.PlacemarkType)
            {
                this.ConvertPlacemark(feature as OsmSharp.Xml.Kml.v2_1.PlacemarkType);
            }
        }

        /// <summary>
        /// Conversts a placemark and all it's contents to osm elements.
        /// </summary>
        /// <param name="placemark"></param>
        /// <returns></returns>
        private void ConvertPlacemark(OsmSharp.Xml.Kml.v2_1.PlacemarkType placemark)
        {
            this.ConvertGeometry(placemark.Item1);
        }

        /// <summary>
        /// Converts a geometry to a list of osm objects.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        private void ConvertGeometry(OsmSharp.Xml.Kml.v2_1.GeometryType geometry)
        {
            if (geometry is OsmSharp.Xml.Kml.v2_1.PointType)
            {
                this.GeometryCollection.Add(
                    KmlGeoStreamSource.ConvertPoint(geometry as OsmSharp.Xml.Kml.v2_1.PointType));
            }
            else if (geometry is OsmSharp.Xml.Kml.v2_1.LineStringType)
            {
                this.GeometryCollection.Add(
                    KmlGeoStreamSource.ConvertLineString(geometry as OsmSharp.Xml.Kml.v2_1.LineStringType));
            }
            else if (geometry is OsmSharp.Xml.Kml.v2_1.LinearRingType)
            {
                this.GeometryCollection.Add(
                    KmlGeoStreamSource.ConvertLinearRing(geometry as OsmSharp.Xml.Kml.v2_1.LinearRingType));
            }
            else if (geometry is OsmSharp.Xml.Kml.v2_1.PolygonType)
            {
                this.GeometryCollection.Add(
                    KmlGeoStreamSource.ConvertPolygon(geometry as OsmSharp.Xml.Kml.v2_1.PolygonType));
            }
            else if (geometry is OsmSharp.Xml.Kml.v2_1.MultiGeometryType)
            {
                this.ConvertMultiGeometry(geometry as OsmSharp.Xml.Kml.v2_1.MultiGeometryType);
            }
        }

        /// <summary>
        /// Converts the multi geometry to multi osm objects.
        /// </summary>
        /// <param name="multiGeometry"></param>
        /// <returns></returns>
        private void ConvertMultiGeometry(OsmSharp.Xml.Kml.v2_1.MultiGeometryType multiGeometry)
        {
            foreach (OsmSharp.Xml.Kml.v2_1.GeometryType geo in multiGeometry.Items)
            {
                this.ConvertGeometry(geo);
            }
        }

        /// <summary>
        /// Convests the polygon to osm objects.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private static Polygon ConvertPolygon(OsmSharp.Xml.Kml.v2_1.PolygonType polygon)
        {
            IEnumerable<LineairRing> inners = KmlGeoStreamSource.ConvertBoundary(polygon.innerBoundaryIs);
            LineairRing outer = KmlGeoStreamSource.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing);

            return new Polygon(outer, inners );
        }

        /// <summary>
        /// Converts boundary type into an osm object.
        /// </summary>
        /// <param name="boundary"></param>
        /// <returns></returns>
        private static IEnumerable<LineairRing> ConvertBoundary(OsmSharp.Xml.Kml.v2_1.boundaryType[] boundary)
        {
            List<LineairRing> rings = new List<LineairRing>();
            foreach (OsmSharp.Xml.Kml.v2_1.boundaryType geo in boundary)
            {
                rings.Add(KmlGeoStreamSource.ConvertLinearRing(geo.LinearRing));
            }
            return rings;
        }

        /// <summary>
        /// Converts a lineairring into osm objects.
        /// </summary>
        /// <param name="linearRing"></param>
        /// <returns></returns>
        private static LineairRing ConvertLinearRing(OsmSharp.Xml.Kml.v2_1.LinearRingType linearRing)
        {
            // convert the coordinates.
            IList<GeoCoordinate> coordinates = KmlGeoStreamSource.ConvertCoordinates(linearRing.coordinates);

            // create the ring.
            LineairRing ring = new LineairRing(coordinates);
            ring.Attributes = new SimpleGeometryAttributeCollection();
            ring.Attributes.Add("id", linearRing.id);

            return ring;
        }

        /// <summary>
        /// Converts a line string into an osm object.
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns></returns>
        private static LineString ConvertLineString(OsmSharp.Xml.Kml.v2_1.LineStringType lineString)
        {
            // convert the coordinates.
            IList<GeoCoordinate> coordinates = KmlGeoStreamSource.ConvertCoordinates(lineString.coordinates);

            // create the ring.
            LineString lineStringGeometry = new LineString(coordinates);
            lineStringGeometry.Attributes = new SimpleGeometryAttributeCollection();
            lineStringGeometry.Attributes.Add("id", lineString.id);

            return lineStringGeometry;
        }

        /// <summary>
        /// Converts a point into an oms object.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private static Point ConvertPoint(OsmSharp.Xml.Kml.v2_1.PointType point)
        {
            // convert the coordiantes.
            IList<GeoCoordinate> coordinates = KmlGeoStreamSource.ConvertCoordinates(point.coordinates);

            // create the point.
            Point pointGeometry = new Point(coordinates[0]);
            pointGeometry.Attributes = new SimpleGeometryAttributeCollection();
            if (point.targetId != null) { pointGeometry.Attributes.Add("targetId", point.targetId); }
            pointGeometry.Attributes.Add("altitude", point.altitudeMode);
            if (point.extrude) { pointGeometry.Attributes.Add("extrude", point.extrude); }
            if (point.id != null) { pointGeometry.Attributes.Add("id", point.id); }

            return pointGeometry;
        }

        /// <summary>
        /// Converts a container and it's contents to osm elements.
        /// </summary>
        /// <param name="container"></param>
        private void ConvertContainer(OsmSharp.Xml.Kml.v2_1.ContainerType container)
        {
            // get the features.
            if (container is OsmSharp.Xml.Kml.v2_1.FolderType)
            {
                OsmSharp.Xml.Kml.v2_1.FolderType folder = (container as OsmSharp.Xml.Kml.v2_1.FolderType);

                // items1 are the features. 
                this.ConvertFeatures(folder.Items1);
            }
            else if (container is OsmSharp.Xml.Kml.v2_1.DocumentType)
            {
                OsmSharp.Xml.Kml.v2_1.DocumentType document = (container as OsmSharp.Xml.Kml.v2_1.DocumentType);

                // items1 are the features.
                this.ConvertFeatures(document.Items1);
            }
        }

        #endregion

        /// <summary>
        /// Converts a list of coordinates to geocoordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        private static IList<GeoCoordinate> ConvertCoordinates(string coordinates)
        {
            IList<GeoCoordinate> geo_coordinates = new List<GeoCoordinate>();
            string[] coordinate_strings = coordinates.Split('\n');
            for (int idx = 0; idx < coordinate_strings.Length; idx++)
            {
                string coordinate_string = coordinate_strings[idx];
                if (coordinate_string != null &&
                    coordinate_string.Length > 0 &&
                    coordinate_string.Trim().Length > 0)
                {
                    string[] coordinate_split = coordinate_string.Split(',');
                    double longitude = 0f;
                    if (!double.TryParse(coordinate_split[0],
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out longitude))
                    {
                        // parsing failed!
                    }
                    double latitude = 0f;
                    if (!double.TryParse(coordinate_split[1],
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out latitude))
                    {
                        // parsing failed!
                    }

                    geo_coordinates.Add(new GeoCoordinate(latitude, longitude));
                }
            }
            return geo_coordinates;
        }
    }
}

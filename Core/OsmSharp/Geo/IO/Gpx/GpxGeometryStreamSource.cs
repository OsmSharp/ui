// OsmSharp - OpenStreetMap tools & library.
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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Streams;
using OsmSharp.Math.Geo;
using OsmSharp.Xml.Gpx;
using System.Collections.Generic;

namespace OsmSharp.Geo.IO.Gpx
{
    /// <summary>
    /// Gpx-stream source.
    /// </summary>
    public class GpxGeometryStreamSource : GeometryCollectionStreamSource
    {
        /// <summary>
        /// Holds the stream containing the source-data.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Creates a new Gpx-geometry stream.
        /// </summary>
        /// <param name="stream"></param>
        public GpxGeometryStreamSource(Stream stream)
            : base(new GeometryCollection())
        {
            _stream = stream;
        }

        #region Read Gpx

        /// <summary>
        /// The gpx object.
        /// </summary>
        private object _gpx_object;

        /// <summary>
        /// Reads the actual Gpx.
        /// </summary>
        private void DoReadGpx()
        {
            if (_gpx_object == null)
            { // only read if the gpx object does not exist yet.
                // find the version.
                Type version_type = null;

                // determine the version from source.
                GpxVersion version = this.FindVersionFromSource();
                switch (version)
                {
                    case GpxVersion.Gpxv1_0:
                        version_type = typeof(OsmSharp.Xml.Gpx.v1_0.gpx);
                        break;
                    case GpxVersion.Gpxv1_1:
                        version_type = typeof(OsmSharp.Xml.Gpx.v1_1.gpxType);
                        break;
                    case GpxVersion.Unknown:
                        throw new XmlException("Version could not be determined!");
                }

                // deserialize
                XmlSerializer xmlSerializer = null;
                xmlSerializer = new XmlSerializer(version_type);
                XmlReader reader = XmlReader.Create(_stream);
                _gpx_object = xmlSerializer.Deserialize(reader);
                reader.Close();

                // process the result.
                switch (version)
                {
                    case GpxVersion.Gpxv1_0:
                        this.ReadGpxv1_0(_gpx_object as OsmSharp.Xml.Gpx.v1_0.gpx);
                        break;
                    case GpxVersion.Gpxv1_1:
                        this.ReadGpxv1_1(_gpx_object as OsmSharp.Xml.Gpx.v1_1.gpxType);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads a gpx v1.1 object into corresponding geometries.
        /// </summary>
        /// <param name="gpx"></param>
        private void ReadGpxv1_1(Xml.Gpx.v1_1.gpxType gpx)
        {
            this.GeometryCollection.Clear();

            // do the waypoints.
            if (gpx.wpt != null)
            { // there are waypoints.
                foreach (var wpt in gpx.wpt)
                {
                    Point point = new Point(
                        new GeoCoordinate((double)wpt.lat, (double)wpt.lon));
                    point.Attributes = new SimpleGeometryAttributeCollection();

                    if (wpt.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", wpt.ageofdgpsdata); }
                    if (wpt.eleSpecified) { point.Attributes.Add("ele", wpt.ele); }
                    if (wpt.fixSpecified) { point.Attributes.Add("fix", wpt.fix); }
                    if (wpt.geoidheightSpecified) { point.Attributes.Add("geoidheight", wpt.geoidheight); }
                    if (wpt.hdopSpecified) { point.Attributes.Add("hdop", wpt.hdop); }
                    if (wpt.magvarSpecified) { point.Attributes.Add("magvar", wpt.magvar); }
                    if (wpt.pdopSpecified) { point.Attributes.Add("pdop", wpt.pdop); }
                    if (wpt.timeSpecified) { point.Attributes.Add("time", wpt.time); }
                    if (wpt.vdopSpecified) { point.Attributes.Add("vdop", wpt.vdop); }

                    point.Attributes.Add("cmt", wpt.cmt);
                    point.Attributes.Add("desc", wpt.desc);
                    point.Attributes.Add("dgpsid", wpt.dgpsid);
                    point.Attributes.Add("name", wpt.name);
                    point.Attributes.Add("sat", wpt.sat);
                    point.Attributes.Add("src", wpt.src);
                    point.Attributes.Add("sym", wpt.sym);
                    point.Attributes.Add("type", wpt.type);

                    this.GeometryCollection.Add(point);
                }
            }

            // do the routes.
            if (gpx.rte != null)
            {
                foreach (var rte in gpx.rte)
                { // convert to a line-string
                    List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
                    foreach (var rtept in rte.rtept)
                    {
                        GeoCoordinate coordinate = new GeoCoordinate((double)rtept.lat, (double)rtept.lon);
                        coordinates.Add(coordinate);

                        Point point = new Point(coordinate);
                        point.Attributes = new SimpleGeometryAttributeCollection();

                        if (rtept.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", rtept.ageofdgpsdata); }
                        if (rtept.eleSpecified) { point.Attributes.Add("ele", rtept.ele); }
                        if (rtept.fixSpecified) { point.Attributes.Add("fix", rtept.fix); }
                        if (rtept.geoidheightSpecified) { point.Attributes.Add("geoidheight", rtept.geoidheight); }
                        if (rtept.hdopSpecified) { point.Attributes.Add("hdop", rtept.hdop); }
                        if (rtept.magvarSpecified) { point.Attributes.Add("magvar", rtept.magvar); }
                        if (rtept.pdopSpecified) { point.Attributes.Add("pdop", rtept.pdop); }
                        if (rtept.timeSpecified) { point.Attributes.Add("time", rtept.time); }
                        if (rtept.vdopSpecified) { point.Attributes.Add("vdop", rtept.vdop); }

                        point.Attributes.Add("cmt", rtept.cmt);
                        point.Attributes.Add("desc", rtept.desc);
                        point.Attributes.Add("dgpsid", rtept.dgpsid);
                        point.Attributes.Add("name", rtept.name);
                        point.Attributes.Add("sat", rtept.sat);
                        point.Attributes.Add("src", rtept.src);
                        point.Attributes.Add("sym", rtept.sym);
                        point.Attributes.Add("type", rtept.type);

                        this.GeometryCollection.Add(point);
                    }

                    // creates a new linestring.
                    LineString lineString = new LineString(coordinates);
                    lineString.Attributes.Add("cmt", rte.cmt);
                    lineString.Attributes.Add("desc", rte.desc);
                    lineString.Attributes.Add("name", rte.name);
                    lineString.Attributes.Add("number", rte.number);
                    lineString.Attributes.Add("src", rte.src);

                    this.GeometryCollection.Add(lineString);
                }
            }

            // do the tracks.
            foreach (var trk in gpx.trk)
            { // convert to a line-string
                List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
                foreach (var trkseg in trk.trkseg)
                {
                    foreach (var wpt in trkseg.trkpt)
                    {
                        GeoCoordinate coordinate = new GeoCoordinate((double)wpt.lat, (double)wpt.lon);

                        // only add new coordinates.
                        if (coordinates.Count == 0 || coordinates[coordinates.Count - 1] != coordinate)
                        {
                            coordinates.Add(coordinate);
                        }

                        Point point = new Point(coordinate);
                        point.Attributes = new SimpleGeometryAttributeCollection();

                        if (wpt.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", wpt.ageofdgpsdata); }
                        if (wpt.eleSpecified) { point.Attributes.Add("ele", wpt.ele); }
                        if (wpt.fixSpecified) { point.Attributes.Add("fix", wpt.fix); }
                        if (wpt.geoidheightSpecified) { point.Attributes.Add("geoidheight", wpt.geoidheight); }
                        if (wpt.hdopSpecified) { point.Attributes.Add("hdop", wpt.hdop); }
                        if (wpt.magvarSpecified) { point.Attributes.Add("magvar", wpt.magvar); }
                        if (wpt.pdopSpecified) { point.Attributes.Add("pdop", wpt.pdop); }
                        if (wpt.timeSpecified) { point.Attributes.Add("time", wpt.time); }
                        if (wpt.vdopSpecified) { point.Attributes.Add("vdop", wpt.vdop); }

                        point.Attributes.Add("cmt", wpt.cmt);
                        point.Attributes.Add("desc", wpt.desc);
                        point.Attributes.Add("dgpsid", wpt.dgpsid);
                        point.Attributes.Add("name", wpt.name);
                        point.Attributes.Add("sat", wpt.sat);
                        point.Attributes.Add("src", wpt.src);
                        point.Attributes.Add("sym", wpt.sym);
                        point.Attributes.Add("type", wpt.type);

                        this.GeometryCollection.Add(point);
                    }
                }

                // creates a new linestring.
                LineString lineString = new LineString(coordinates);
                lineString.Attributes.Add("cmt", trk.cmt);
                lineString.Attributes.Add("desc", trk.desc);
                lineString.Attributes.Add("name", trk.name);
                lineString.Attributes.Add("number", trk.number);
                lineString.Attributes.Add("src", trk.src);

                this.GeometryCollection.Add(lineString);
            }
        }

        /// <summary>
        /// Reads a gpx v1.0 object into corresponding geometries.
        /// </summary>
        /// <param name="gpx"></param>
        private void ReadGpxv1_0(OsmSharp.Xml.Gpx.v1_0.gpx gpx)
        {
            this.GeometryCollection.Clear();

            // do the waypoints.
            if (gpx.wpt != null)
            { // there are waypoints.
                foreach (var wpt in gpx.wpt)
                {
                    Point point = new Point(
                        new GeoCoordinate((double)wpt.lat, (double)wpt.lon));
                    point.Attributes = new SimpleGeometryAttributeCollection();

                    if (wpt.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", wpt.ageofdgpsdata); }
                    if (wpt.eleSpecified) { point.Attributes.Add("ele", wpt.ele); }
                    if (wpt.fixSpecified) { point.Attributes.Add("fix", wpt.fix); }
                    if (wpt.geoidheightSpecified) { point.Attributes.Add("geoidheight", wpt.geoidheight); }
                    if (wpt.hdopSpecified) { point.Attributes.Add("hdop", wpt.hdop); }
                    if (wpt.magvarSpecified) { point.Attributes.Add("magvar", wpt.magvar); }
                    if (wpt.pdopSpecified) { point.Attributes.Add("pdop", wpt.pdop); }
                    if (wpt.timeSpecified) { point.Attributes.Add("time", wpt.time); }
                    if (wpt.vdopSpecified) { point.Attributes.Add("vdop", wpt.vdop); }

                    point.Attributes.Add("Any", wpt.Any);
                    point.Attributes.Add("cmt", wpt.cmt);
                    point.Attributes.Add("desc", wpt.desc);
                    point.Attributes.Add("dgpsid", wpt.dgpsid);
                    point.Attributes.Add("name", wpt.name);
                    point.Attributes.Add("sat", wpt.sat);
                    point.Attributes.Add("src", wpt.src);
                    point.Attributes.Add("sym", wpt.sym);
                    point.Attributes.Add("url", wpt.url);
                    point.Attributes.Add("urlname", wpt.urlname);
                    point.Attributes.Add("type", wpt.type);

                    this.GeometryCollection.Add(point);
                }
            }

            // do the routes.
            if (gpx.rte != null)
            {
                foreach (var rte in gpx.rte)
                { // convert to a line-string
                    List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
                    foreach (var rtept in rte.rtept)
                    {
                        GeoCoordinate coordinate = new GeoCoordinate((double)rtept.lat, (double)rtept.lon);
                        coordinates.Add(coordinate);

                        Point point = new Point(coordinate);
                        point.Attributes = new SimpleGeometryAttributeCollection();

                        if (rtept.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", rtept.ageofdgpsdata); }
                        if (rtept.eleSpecified) { point.Attributes.Add("ele", rtept.ele); }
                        if (rtept.fixSpecified) { point.Attributes.Add("fix", rtept.fix); }
                        if (rtept.geoidheightSpecified) { point.Attributes.Add("geoidheight", rtept.geoidheight); }
                        if (rtept.hdopSpecified) { point.Attributes.Add("hdop", rtept.hdop); }
                        if (rtept.magvarSpecified) { point.Attributes.Add("magvar", rtept.magvar); }
                        if (rtept.pdopSpecified) { point.Attributes.Add("pdop", rtept.pdop); }
                        if (rtept.timeSpecified) { point.Attributes.Add("time", rtept.time); }
                        if (rtept.vdopSpecified) { point.Attributes.Add("vdop", rtept.vdop); }

                        point.Attributes.Add("Any", rtept.Any);
                        point.Attributes.Add("cmt", rtept.cmt);
                        point.Attributes.Add("desc", rtept.desc);
                        point.Attributes.Add("dgpsid", rtept.dgpsid);
                        point.Attributes.Add("name", rtept.name);
                        point.Attributes.Add("sat", rtept.sat);
                        point.Attributes.Add("src", rtept.src);
                        point.Attributes.Add("sym", rtept.sym);
                        point.Attributes.Add("url", rtept.url);
                        point.Attributes.Add("urlname", rtept.urlname);
                        point.Attributes.Add("type", rtept.type);

                        this.GeometryCollection.Add(point);
                    }

                    // creates a new linestring.
                    LineString lineString = new LineString(coordinates);
                    lineString.Attributes.Add("Any", rte.Any);
                    lineString.Attributes.Add("cmt", rte.cmt);
                    lineString.Attributes.Add("desc", rte.desc);
                    lineString.Attributes.Add("name", rte.name);
                    lineString.Attributes.Add("number", rte.number);
                    lineString.Attributes.Add("src", rte.src);
                    lineString.Attributes.Add("url", rte.url);
                    lineString.Attributes.Add("urlname", rte.urlname);

                    this.GeometryCollection.Add(lineString);
                }
            }

            // do the tracks.
            foreach (var trk in gpx.trk)
            { // convert to a line-string
                List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
                foreach (var trkseg in trk.trkseg)
                {
                    GeoCoordinate coordinate = new GeoCoordinate((double)trkseg.lat, (double)trkseg.lon);
                    coordinates.Add(coordinate);

                    Point point = new Point(coordinate);
                    point.Attributes = new SimpleGeometryAttributeCollection();

                    if (trkseg.ageofdgpsdataSpecified) { point.Attributes.Add("ageofdgpsdata", trkseg.ageofdgpsdata); }
                    if (trkseg.eleSpecified) { point.Attributes.Add("ele", trkseg.ele); }
                    if (trkseg.fixSpecified) { point.Attributes.Add("fix", trkseg.fix); }
                    if (trkseg.geoidheightSpecified) { point.Attributes.Add("geoidheight", trkseg.geoidheight); }
                    if (trkseg.hdopSpecified) { point.Attributes.Add("hdop", trkseg.hdop); }
                    if (trkseg.magvarSpecified) { point.Attributes.Add("magvar", trkseg.magvar); }
                    if (trkseg.pdopSpecified) { point.Attributes.Add("pdop", trkseg.pdop); }
                    if (trkseg.timeSpecified) { point.Attributes.Add("time", trkseg.time); }
                    if (trkseg.vdopSpecified) { point.Attributes.Add("vdop", trkseg.vdop); }

                    point.Attributes.Add("Any", trkseg.Any);
                    point.Attributes.Add("cmt", trkseg.cmt);
                    point.Attributes.Add("desc", trkseg.desc);
                    point.Attributes.Add("dgpsid", trkseg.dgpsid);
                    point.Attributes.Add("name", trkseg.name);
                    point.Attributes.Add("sat", trkseg.sat);
                    point.Attributes.Add("src", trkseg.src);
                    point.Attributes.Add("sym", trkseg.sym);
                    point.Attributes.Add("url", trkseg.url);
                    point.Attributes.Add("urlname", trkseg.urlname);
                    point.Attributes.Add("type", trkseg.type);

                    this.GeometryCollection.Add(point);
                }

                // creates a new linestring.
                LineString lineString = new LineString(coordinates);
                lineString.Attributes.Add("Any", trk.Any);
                lineString.Attributes.Add("cmt", trk.cmt);
                lineString.Attributes.Add("desc", trk.desc);
                lineString.Attributes.Add("name", trk.name);
                lineString.Attributes.Add("number", trk.number);
                lineString.Attributes.Add("src", trk.src);
                lineString.Attributes.Add("url", trk.url);
                lineString.Attributes.Add("urlname", trk.urlname);

                this.GeometryCollection.Add(lineString);
            }
        }

        /// <summary>
        /// Tries to find the version from the source.
        /// </summary>
        private GpxVersion FindVersionFromSource()
        {
            GpxVersion version = GpxVersion.Unknown;

            // try to find the xmlns and the correct version to use.
            XmlReader reader = XmlReader.Create(_stream);
            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element
                    && reader.Name == "gpx")
                {
                    string ns = reader.GetAttribute("xmlns");
                    switch (ns)
                    {
                        case "http://www.topografix.com/GPX/1/0":
                            version = GpxVersion.Gpxv1_0;
                            break;
                        case "http://www.topografix.com/GPX/1/1":
                            version = GpxVersion.Gpxv1_1;
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    throw new XmlException("First element expected: gpx!");
                }

                // check end conditions.
                if (version != GpxVersion.Unknown)
                {
                    reader.Close();
                    reader = null;
                    break;
                }

                reader.Read();
            }
            return version;
        }

        #endregion
    }
}

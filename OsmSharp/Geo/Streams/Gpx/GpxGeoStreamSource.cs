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

using System.IO;
using OsmSharp.Geo.Geometries.Streams;
using OsmSharp.Math.Geo;
using OsmSharp.Geo.Geometries;
using System.Collections.Generic;
using System.Collections;
using OsmSharp.Xml.Gpx;
using OsmSharp.Xml.Sources;
using OsmSharp.Geo.Attributes;

namespace OsmSharp.Geo.Streams.Gpx
{
    /// <summary>
    /// Represents a stream of geometries from a gpx-file.
    /// </summary>
    public class GpxGeoStreamSource : IGeoStreamSource
    {
        /// <summary>
        /// Holds the stream containing the gpx-data.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Creates a gpx geo stream.
        /// </summary>
        /// <param name="stream"></param>
        public GpxGeoStreamSource(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Holds the bounds.
        /// </summary>
        private GeoCoordinateBox _bounds = null;

        /// <summary>
        /// The geometries in the gpx document.
        /// </summary>
        private List<Geometry> _geometries;

        /// <summary>
        /// Holds the current index.
        /// </summary>
        private int _currentIdx = 0;

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public void Initialize()
        {
            GpxDocument document = new GpxDocument(new XmlStreamSource(_stream));

            if (document.Gpx == null)
            { // no data.
                return;
            }

            // initialize the geometries list.
            _geometries = new List<Geometry>();
            switch (document.Version)
            {
                case GpxVersion.Gpxv1_0:
                    OsmSharp.Xml.Gpx.v1_0.gpx gpx_v1_0 = document.Gpx as OsmSharp.Xml.Gpx.v1_0.gpx;

                    // create and add all line strings.
                    foreach (OsmSharp.Xml.Gpx.v1_0.gpxTrk trk in gpx_v1_0.trk)
                    {
                        LineString lineString = this.ConvertGpxTrk(trk);
                        if (lineString != null)
                        { // a line string was created.
                            _geometries.Add(lineString);

                            if (_bounds == null)
                            { // take the first bounds.
                                _bounds = lineString.Box;
                            }
                            else
                            { // add the new bounds to the existing ones.
                                _bounds = _bounds + lineString.Box;
                            }
                        }
                    }

                    break;
                case GpxVersion.Gpxv1_1:
                    OsmSharp.Xml.Gpx.v1_1.gpxType gpx_v1_1 = document.Gpx as OsmSharp.Xml.Gpx.v1_1.gpxType;

                    // create and add all line strings.
                    foreach (OsmSharp.Xml.Gpx.v1_1.trkType trk in gpx_v1_1.trk)
                    {
                        LineString lineString = this.ConvertGpxTrk(trk);
                        if (lineString != null)
                        { // a line string was created.
                            _geometries.Add(lineString);

                            if (_bounds == null)
                            { // take the first bounds.
                                _bounds = lineString.Box;
                            }
                            else
                            { // add the new bounds to the existing ones.
                                _bounds = _bounds + lineString.Box;
                            }
                        }
                    }
                    // create and add all points.
                    foreach (OsmSharp.Xml.Gpx.v1_1.wptType wpt in gpx_v1_1.wpt)
                    {
                        Point point = this.ConvertWptType(wpt);
                        point.Attributes = new SimpleGeometryAttributeCollection();
                        point.Attributes.Add("type", "wpt");
                        _geometries.Add(point);

                        if (_bounds == null)
                        { // take the first bounds.
                            _bounds = point.Box;
                        }
                        else
                        { // add the new bounds to the existing ones.
                            _bounds = _bounds + point.Box;
                        }
                    }
                    break;
            }
        }

        private Point ConvertWptType(Xml.Gpx.v1_1.wptType wpt)
        {
            throw new System.NotImplementedException();
        }

        private LineString ConvertGpxTrk(Xml.Gpx.v1_1.trkType trk)
        {
            throw new System.NotImplementedException();
        }

        private LineString ConvertGpxTrk(Xml.Gpx.v1_0.gpxTrk trk)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        /// <returns></returns>
        public bool CanReset()
        {
            return true;
        }

        /// <summary>
        /// Closes this stream source.
        /// </summary>
        public void Close()
        {

        }

        /// <summary>
        /// Returns true if this source has bounds.
        /// </summary>
        public bool HasBounds
        {
            get { return _bounds != null; }
        }

        /// <summary>
        /// Returns the bounds of this source.
        /// </summary>
        /// <returns></returns>
        public GeoCoordinateBox GetBounds()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns the current geometry.
        /// </summary>
        public Geometries.Geometry Current
        {
            get { return _geometries[_currentIdx]; }
        }

        /// <summary>
        /// Disposes all resource associated with this source.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Returns the current geometry.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        /// <summary>
        /// Moves this source to the next geometry.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            _currentIdx++;
            return (_currentIdx < _geometries.Count) ;
        }

        /// <summary>
        /// Resets this source.
        /// </summary>
        public void Reset()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            _currentIdx = -1;
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Geometry> GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
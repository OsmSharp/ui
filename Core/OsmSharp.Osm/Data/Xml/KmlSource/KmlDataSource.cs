// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Xml.Kml;
using OsmSharp.Osm.Factory;
using OsmSharp.Osm;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Filters;

namespace OsmSharp.Osm.Data.Raw.XML.KmlSource
{
    /// <summary>
    /// Represents a data source base on a kml document.
    /// 
    /// Note:   Folders => Relation
    ///         Document => Relation    
    ///             Placemark => Relation
    ///             Geometry => 
    ///                 Polygon => Relation
    ///                     InnerBoundary => LineairRing (see below)
    ///                     OuterBoundary => LineairRing (see below)
    ///                 Point => Node
    ///                 LineString => Way + Nodes (coordinates inside)
    ///                 LineairRing => Way (area = yes) + Nodes (coordinates inside)
    ///                 MultiGeometery => Relation
    /// </summary>
    public class KmlDataSource : IDataSourceReadOnly
    {
        /// <summary>
        /// The kml document this datasource is for.
        /// </summary>
        private KmlDocument _document;

        /// <summary>
        /// The id of this data source.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Creates a new osm data source.
        /// </summary>
        /// <param name="document"></param>
        public KmlDataSource(KmlDocument document)
        {
            _document = document;
            _id = Guid.NewGuid();

            _read = false;
            _nodes = new Dictionary<long, Node>();
            _ways = new Dictionary<long, Way>();
            _relations = new Dictionary<long, Relation>();

            _ways_per_node = new Dictionary<long, List<long>>();
            _relations_per_member = new Dictionary<long, List<long>>();
        }

        #region Write/Read functions

        // hold all node, ways, relations and changesets and their bounding box.
        private IDictionary<long, Node> _nodes;
        private IDictionary<long, Way> _ways;
        private IDictionary<long, Relation> _relations;
        private IDictionary<long, List<long>> _ways_per_node;
        private IDictionary<long, List<long>> _relations_per_member;
        private GeoCoordinateBox _bb;

        private bool _read;

        /// <summary>
        /// Adds the node-way relations for the given way.
        /// </summary>
        /// <param name="way"></param>
        private void RegisterNodeWayRelation(Way way)
        {
            foreach (Node node in way.Nodes)
            {
                if (!_ways_per_node.ContainsKey(node.Id))
                {
                    _ways_per_node.Add(node.Id, new List<long>());
                }
                _ways_per_node[node.Id].Add(way.Id);
            }
        }

        /// <summary>
        /// Adds the member-relation for the given relation.
        /// </summary>
        /// <param name="relation"></param>
        private void RegisterRelationMemberRelation(Relation relation)
        {
            foreach (RelationMember member in relation.Members)
            {
                if (!_relations_per_member.ContainsKey(member.Member.Id))
                {
                    _relations_per_member.Add(member.Member.Id, new List<long>());
                }
                _relations_per_member[member.Member.Id].Add(relation.Id);
            }
        }

        /// <summary>
        /// Reads all the data from the osm document if needed.
        /// </summary>
        private void ReadFromDocument()
        {
            if (!_read)
            {
                _read = true;

                //         Folders => Relation
                //         Document => Relation    
                //             Placemark => Relation
                //             Geometry => 
                //                 Polygon => Relation
                //                     InnerBoundary => LineairRing (see below)
                //                     OuterBoundary => LineairRing (see below)
                //                 Point => Node
                //                 LineString => Way + Nodes (coordinates inside)
                //                 LineairRing => Way (area = yes) + Nodes (coordinates inside)
                //                 MultiGeometery => Relation
                if(_document.Kml == null)
                { // no data present.
                    return;
                }
                if (_document.Kml is OsmSharp.Tools.Xml.Kml.v2_1.KmlType)
                { // kml is v2.1.
                    OsmSharp.Tools.Xml.Kml.v2_1.KmlType kml = (_document.Kml as OsmSharp.Tools.Xml.Kml.v2_1.KmlType);

                    OsmGeo osm_geo = this.ConvertFeature(kml.Item);
                    this.AddOsmBase(osm_geo);
                } 
                else if(_document.Kml is OsmSharp.Tools.Xml.Kml.v2_0_response.kml)
                { // kml is a v2.0 response.
                    OsmSharp.Tools.Xml.Kml.v2_0_response.kml kml = (_document.Kml as OsmSharp.Tools.Xml.Kml.v2_0_response.kml);

                    if(kml.Item is OsmSharp.Tools.Xml.Kml.v2_0_response.Document)
                    {
                        OsmGeo osm_geo = this.ConvertDocument(kml.Item as OsmSharp.Tools.Xml.Kml.v2_0_response.Document);
                        this.AddOsmBase(osm_geo);
                    }
                    else if(kml.Item is OsmSharp.Tools.Xml.Kml.v2_0_response.Folder)
                    {
                        OsmGeo osm_geo = this.ConvertFolder(kml.Item as OsmSharp.Tools.Xml.Kml.v2_0_response.Folder);
                        this.AddOsmBase(osm_geo);
                    }
                    else if(kml.Item is OsmSharp.Tools.Xml.Kml.v2_0_response.Placemark)
                    {
                        OsmGeo osm_geo = this.ConvertPlacemark(kml.Item as OsmSharp.Tools.Xml.Kml.v2_0_response.Placemark);
                        this.AddOsmBase(osm_geo);
                    }
                    else if(kml.Item is OsmSharp.Tools.Xml.Kml.v2_0_response.Response)
                    {
                        OsmGeo osm_geo = this.ConvertResponse(kml.Item as OsmSharp.Tools.Xml.Kml.v2_0_response.Response);
                        this.AddOsmBase(osm_geo);
                    }
                }
                else if (_document.Kml is OsmSharp.Tools.Xml.Kml.v2_0.kml)
                { // kml is v2.0
                    OsmSharp.Tools.Xml.Kml.v2_0.kml kml = (_document.Kml as OsmSharp.Tools.Xml.Kml.v2_0.kml);

                    if (kml.Item is OsmSharp.Tools.Xml.Kml.v2_0.Document)
                    {
                        OsmGeo osm_geo = this.ConvertDocument(kml.Item as OsmSharp.Tools.Xml.Kml.v2_0.Document);
                        this.AddOsmBase(osm_geo);
                    }
                    else if (kml.Item is OsmSharp.Tools.Xml.Kml.v2_0.Folder)
                    {
                        OsmGeo osm_geo = this.ConvertFolder(kml.Item as OsmSharp.Tools.Xml.Kml.v2_0.Folder);
                        this.AddOsmBase(osm_geo);
                    }
                    else if (kml.Item is OsmSharp.Tools.Xml.Kml.v2_0.Placemark)
                    {
                        OsmGeo osm_geo = this.ConvertPlacemark(kml.Item as OsmSharp.Tools.Xml.Kml.v2_0.Placemark);
                        this.AddOsmBase(osm_geo);
                    }
                }
                else
                { // kml type not found; not supported.
                    throw new NotSupportedException(string.Format(
                        "Kml Document of type {0} is not supported!", _document.Kml.GetType()));

                }
            }
        }


        #region v2.0

        /// <summary>
        /// Converts a placemark into an osm object.
        /// </summary>
        /// <param name="placemark"></param>
        /// <returns></returns>
        private OsmGeo ConvertPlacemark(OsmSharp.Tools.Xml.Kml.v2_0.Placemark placemark)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Tags.Add("kml_type", "Response");

            for (int idx = 0; idx < placemark.Items.Length; idx++)
            {
                RelationMember member;
                switch (placemark.ItemsElementName[idx])
                {
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType1.LineString:
                        member = new RelationMember();
                        member.Member = this.ConvertLineString(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.LineString);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType1.MultiGeometry:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiGeometry(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.MultiGeometry);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType1.MultiLineString:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiLineString(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.MultiLineString);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType1.MultiPoint:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiPoint(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.MultiPoint);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType1.MultiPolygon:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiPolygon(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.MultiPolygon);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType1.Point:
                        member = new RelationMember();
                        member.Member = this.ConvertPoint(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.Point);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType1.Polygon:
                        member = new RelationMember();
                        member.Member = this.ConvertPolygon(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.Polygon);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                }
            }

            return relation;
        }

        /// <summary>
        /// Converts a polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private OsmGeo ConvertPolygon(OsmSharp.Tools.Xml.Kml.v2_0.Polygon polygon)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            OsmGeo inner = this.ConvertLinearRing(polygon.innerBoundaryIs.LinearRing);
            if (inner != null)
            {
                RelationMember member = new RelationMember();
                member.Member = inner;
                member.Role = "inner";
                relation.Members.Add(member);
            }

            OsmGeo outer = this.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing);
            if (outer != null)
            {
                RelationMember member = new RelationMember();
                member.Member = outer;
                member.Role = "outer";
                relation.Members.Add(member);
            }

            return relation;
        }

        /// <summary>
        /// Converts a lineairring into an osm object.
        /// </summary>
        /// <param name="linearRing"></param>
        /// <returns></returns>
        private OsmGeo ConvertLinearRing(OsmSharp.Tools.Xml.Kml.v2_0.LinearRing linearRing)
        {
            Way way = OsmBaseFactory.CreateWay(KeyGenerator.GenerateNew());

            way.Tags.Add("area", "yes");
            way.Tags.Add("kml_type", "LinearRing");

            IList<GeoCoordinate> coordinates = this.ConvertCoordinates(linearRing.coordinates);
            foreach (GeoCoordinate coordinate in coordinates)
            {
                Node node = OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());
                node.Coordinate = coordinate;
                way.Nodes.Add(node);
            }

            return way;
        }

        /// <summary>
        /// Converts a point into an osm object.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private OsmGeo ConvertPoint(OsmSharp.Tools.Xml.Kml.v2_0.Point point)
        {
            Node node = OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());

            node.Tags.Add("kml_type", "Point");

            IList<GeoCoordinate> coordinates = this.ConvertCoordinates(point.coordinates);
            node.Coordinate = coordinates[0];

            return node;
        }

        /// <summary>
        /// Converts a multipolygon into osm objects.
        /// </summary>
        /// <param name="multiPolygon"></param>
        /// <returns></returns>
        private OsmGeo ConvertMultiPolygon(OsmSharp.Tools.Xml.Kml.v2_0.MultiPolygon multiPolygon)
        {
            return this.ConvertPolygon(multiPolygon.Polygon);
        }

        /// <summary>
        /// Converts a multipoint to osm objects.
        /// </summary>
        /// <param name="multiPoint"></param>
        /// <returns></returns>
        private OsmGeo ConvertMultiPoint(OsmSharp.Tools.Xml.Kml.v2_0.MultiPoint multiPoint)
        {
            return this.ConvertPoint(multiPoint.Point);
        }

        /// <summary>
        /// Converts a multilinestring to osm objects.
        /// </summary>
        /// <param name="multiLineString"></param>
        /// <returns></returns>
        private OsmGeo ConvertMultiLineString(OsmSharp.Tools.Xml.Kml.v2_0.MultiLineString multiLineString)
        {
            return this.ConvertLineString(multiLineString.LineString);
        }

        /// <summary>
        /// Converts a multigeometry to osm objects.
        /// </summary>
        /// <param name="multiGeometry"></param>
        /// <returns></returns>
        private OsmGeo ConvertMultiGeometry(OsmSharp.Tools.Xml.Kml.v2_0.MultiGeometry multiGeometry)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Tags.Add("kml_type", "MultiGeometry");

            for (int idx = 0; idx < multiGeometry.Items.Length; idx++)
            {
                RelationMember member;
                switch (multiGeometry.ItemsElementName[idx])
                {
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType.LineString:
                        member = new RelationMember();
                        member.Member = this.ConvertLineString(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.LineString);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType.MultiGeometry:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiGeometry(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.MultiGeometry);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType.MultiLineString:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiLineString(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.MultiLineString);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType.MultiPoint:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiPoint(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.MultiPoint);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType.MultiPolygon:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiPolygon(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.MultiPolygon);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType.Point:
                        member = new RelationMember();
                        member.Member = this.ConvertPoint(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.Point);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType.Polygon:
                        member = new RelationMember();
                        member.Member = this.ConvertPolygon(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.Polygon);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                }
            }

            return relation;
        }

        /// <summary>
        /// Converts a linestring to osm objects.
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns></returns>
        private OsmGeo ConvertLineString(OsmSharp.Tools.Xml.Kml.v2_0.LineString lineString)
        {
            Way way = OsmBaseFactory.CreateWay(KeyGenerator.GenerateNew());

            way.Tags.Add("kml_type", "LineString");

            IList<GeoCoordinate> coordinates = this.ConvertCoordinates(lineString.coordinates);
            foreach (GeoCoordinate coordinate in coordinates)
            {
                Node node = OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());
                node.Coordinate = coordinate;
                way.Nodes.Add(node);
            }

            return way;
        }

        /// <summary>
        /// Converts a folder into an osm object.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private OsmGeo ConvertFolder(OsmSharp.Tools.Xml.Kml.v2_0.Folder folder)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Tags.Add("kml_type", "Folder");

            for (int idx = 0; idx < folder.Items.Length; idx++)
            {
                RelationMember member;
                switch (folder.ItemsElementName[idx])
                {
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType2.Document:
                        member = new RelationMember();
                        member.Member = this.ConvertDocument(folder.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.Document);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType2.Folder:
                        member = new RelationMember();
                        member.Member = this.ConvertFolder(folder.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.Folder);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType2.Placemark:
                        member = new RelationMember();
                        member.Member = this.ConvertPlacemark(folder.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.Placemark);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                }
            }

            return relation;
        }

        /// <summary>
        /// Converts a document into osm elements.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private OsmGeo ConvertDocument(OsmSharp.Tools.Xml.Kml.v2_0.Document document)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Tags.Add("kml_type", "Document");

            for (int idx = 0; idx < document.Items.Length; idx++)
            {
                RelationMember member;
                switch (document.ItemsElementName[idx])
                {
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType3.Document:
                        member = new RelationMember();
                        member.Member = this.ConvertDocument(document.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.Document);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType3.Folder:
                        member = new RelationMember();
                        member.Member = this.ConvertFolder(document.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.Folder);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0.ItemsChoiceType3.Placemark:
                        member = new RelationMember();
                        member.Member = this.ConvertPlacemark(document.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0.Placemark);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                }
            }

            return relation;
        }

        #endregion

        #region v2.0.response

        /// <summary>
        /// Converts a response into an osm object.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private OsmGeo ConvertResponse(OsmSharp.Tools.Xml.Kml.v2_0_response.Response response)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Tags.Add("kml_type", "Response");

            foreach (object item in response.Items)
            {
                if (item is OsmSharp.Tools.Xml.Kml.v2_0_response.Document)
                {
                    OsmGeo osm_geo = this.ConvertDocument(item as OsmSharp.Tools.Xml.Kml.v2_0_response.Document);

                    RelationMember member = new RelationMember();
                    member.Member = osm_geo;
                    member.Role = "";
                    relation.Members.Add(member);
                }
                else if (item is OsmSharp.Tools.Xml.Kml.v2_0_response.Folder)
                {
                    OsmGeo osm_geo = this.ConvertFolder(item as OsmSharp.Tools.Xml.Kml.v2_0_response.Folder);

                    RelationMember member = new RelationMember();
                    member.Member = osm_geo;
                    member.Role = "";
                    relation.Members.Add(member);
                    
                }
                else if (item is OsmSharp.Tools.Xml.Kml.v2_0_response.Placemark)
                {
                    OsmGeo osm_geo = this.ConvertPlacemark(item as OsmSharp.Tools.Xml.Kml.v2_0_response.Placemark);

                    RelationMember member = new RelationMember();
                    member.Member = osm_geo;
                    member.Role = "";
                    relation.Members.Add(member);
                    
                }
                else if (item is OsmSharp.Tools.Xml.Kml.v2_0_response.Response)
                {
                    OsmGeo osm_geo = this.ConvertResponse(item as OsmSharp.Tools.Xml.Kml.v2_0_response.Response);

                    RelationMember member = new RelationMember();
                    member.Member = osm_geo;
                    member.Role = "";
                    relation.Members.Add(member);                    
                }
            }

            return relation;
        }

        /// <summary>
        /// Converts a placemark into an osm object.
        /// </summary>
        /// <param name="placemark"></param>
        /// <returns></returns>
        private OsmGeo ConvertPlacemark(OsmSharp.Tools.Xml.Kml.v2_0_response.Placemark placemark)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Tags.Add("kml_type", "Response");

            for(int idx = 0;idx < placemark.Items.Length;idx++)
            {
                RelationMember member;
                switch (placemark.ItemsElementName[idx])
                {
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType1.LineString:
                        member = new RelationMember();
                        member.Member = this.ConvertLineString(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.LineString);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiGeometry:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiGeometry(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.MultiGeometry);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiLineString:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiLineString(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.MultiLineString);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiPoint:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiPoint(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.MultiPoint);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType1.MultiPolygon:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiPolygon(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.MultiPolygon);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType1.Point:
                        member = new RelationMember();
                        member.Member = this.ConvertPoint(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.Point);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType1.Polygon:
                        member = new RelationMember();
                        member.Member = this.ConvertPolygon(placemark.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.Polygon);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                }
            }

            return relation;
        }

        /// <summary>
        /// Converts a polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private OsmGeo ConvertPolygon(OsmSharp.Tools.Xml.Kml.v2_0_response.Polygon polygon)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            OsmGeo inner = this.ConvertLinearRing(polygon.innerBoundaryIs.LinearRing);
            if (inner != null)
            {
                RelationMember member = new RelationMember();
                member.Member = inner;
                member.Role = "inner";
                relation.Members.Add(member);
            }

            OsmGeo outer = this.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing);
            if (outer != null)
            {
                RelationMember member = new RelationMember();
                member.Member = outer;
                member.Role = "outer";
                relation.Members.Add(member);
            }

            return relation;
        }

        /// <summary>
        /// Converts a lineairring into an osm object.
        /// </summary>
        /// <param name="linearRing"></param>
        /// <returns></returns>
        private OsmGeo ConvertLinearRing(OsmSharp.Tools.Xml.Kml.v2_0_response.LinearRing linearRing)
        {
            Way way = OsmBaseFactory.CreateWay(KeyGenerator.GenerateNew());

            way.Tags.Add("area", "yes");
            way.Tags.Add("kml_type", "LinearRing");

            IList<GeoCoordinate> coordinates = this.ConvertCoordinates(linearRing.coordinates);
            foreach (GeoCoordinate coordinate in coordinates)
            {
                Node node = OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());
                node.Coordinate = coordinate;
                way.Nodes.Add(node);
            }

            return way;
        }

        /// <summary>
        /// Converts a point into an osm object.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private OsmGeo ConvertPoint(OsmSharp.Tools.Xml.Kml.v2_0_response.Point point)
        {
            Node node = OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());

            node.Tags.Add("kml_type", "Point");

            IList<GeoCoordinate> coordinates = this.ConvertCoordinates(point.coordinates);
            node.Coordinate = coordinates[0];

            return node;
        }

        /// <summary>
        /// Converts a multipolygon into osm objects.
        /// </summary>
        /// <param name="multiPolygon"></param>
        /// <returns></returns>
        private OsmGeo ConvertMultiPolygon(OsmSharp.Tools.Xml.Kml.v2_0_response.MultiPolygon multiPolygon)
        {
            return this.ConvertPolygon(multiPolygon.Polygon);
        }

        /// <summary>
        /// Converts a multipoint to osm objects.
        /// </summary>
        /// <param name="multiPoint"></param>
        /// <returns></returns>
        private OsmGeo ConvertMultiPoint(OsmSharp.Tools.Xml.Kml.v2_0_response.MultiPoint multiPoint)
        {
            return this.ConvertPoint(multiPoint.Point);
        }

        /// <summary>
        /// Converts a multilinestring to osm objects.
        /// </summary>
        /// <param name="multiLineString"></param>
        /// <returns></returns>
        private OsmGeo ConvertMultiLineString(OsmSharp.Tools.Xml.Kml.v2_0_response.MultiLineString multiLineString)
        {
            return this.ConvertLineString(multiLineString.LineString);
        }

        /// <summary>
        /// Converts a multigeometry to osm objects.
        /// </summary>
        /// <param name="multiGeometry"></param>
        /// <returns></returns>
        private OsmGeo ConvertMultiGeometry(OsmSharp.Tools.Xml.Kml.v2_0_response.MultiGeometry multiGeometry)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Tags.Add("kml_type", "MultiGeometry");

            for (int idx = 0; idx < multiGeometry.Items.Length; idx++)
            {
                RelationMember member;
                switch (multiGeometry.ItemsElementName[idx])
                {
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType.LineString:
                        member = new RelationMember();
                        member.Member = this.ConvertLineString(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.LineString);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType.MultiGeometry:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiGeometry(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.MultiGeometry);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType.MultiLineString:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiLineString(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.MultiLineString);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType.MultiPoint:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiPoint(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.MultiPoint);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType.MultiPolygon:
                        member = new RelationMember();
                        member.Member = this.ConvertMultiPolygon(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.MultiPolygon);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType.Point:
                        member = new RelationMember();
                        member.Member = this.ConvertPoint(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.Point);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType.Polygon:
                        member = new RelationMember();
                        member.Member = this.ConvertPolygon(multiGeometry.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.Polygon);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                }
            }

            return relation;
        }

        /// <summary>
        /// Converts a linestring to osm objects.
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns></returns>
        private OsmGeo ConvertLineString(OsmSharp.Tools.Xml.Kml.v2_0_response.LineString lineString)
        {
            Way way = OsmBaseFactory.CreateWay(KeyGenerator.GenerateNew());

            way.Tags.Add("kml_type", "LineString");

            IList<GeoCoordinate> coordinates = this.ConvertCoordinates(lineString.coordinates);
            foreach (GeoCoordinate coordinate in coordinates)
            {
                Node node = OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());
                node.Coordinate = coordinate;
                way.Nodes.Add(node);
            }

            return way;
        }

        /// <summary>
        /// Converts a folder into an osm object.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private OsmGeo ConvertFolder(OsmSharp.Tools.Xml.Kml.v2_0_response.Folder folder)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Tags.Add("kml_type", "Folder");

            for (int idx = 0; idx < folder.Items.Length; idx++)
            {
                RelationMember member;
                switch (folder.ItemsElementName[idx])
                {
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType2.Document:
                        member = new RelationMember();
                        member.Member = this.ConvertDocument(folder.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.Document);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType2.Folder:
                        member = new RelationMember();
                        member.Member = this.ConvertFolder(folder.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.Folder);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType2.Placemark:
                        member = new RelationMember();
                        member.Member = this.ConvertPlacemark(folder.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.Placemark);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                }
            }

            return relation;
        }

        /// <summary>
        /// Converts a document into osm elements.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private OsmGeo ConvertDocument(OsmSharp.Tools.Xml.Kml.v2_0_response.Document document)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Tags.Add("kml_type", "Document");

            for (int idx = 0; idx < document.Items.Length; idx++)
            {
                RelationMember member;
                switch (document.ItemsElementName[idx])
                {
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType3.Document:
                        member = new RelationMember();
                        member.Member = this.ConvertDocument(document.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.Document);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType3.Folder:
                        member = new RelationMember();
                        member.Member = this.ConvertFolder(document.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.Folder);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                    case OsmSharp.Tools.Xml.Kml.v2_0_response.ItemsChoiceType3.Placemark:
                        member = new RelationMember();
                        member.Member = this.ConvertPlacemark(document.Items[idx] as OsmSharp.Tools.Xml.Kml.v2_0_response.Placemark);
                        member.Role = "";
                        relation.Members.Add(member);
                        break;
                }
            }

            return relation;
        }

        #endregion

        #region v2.1

        /// <summary>
        /// Converts all the features into osm elements.
        /// </summary>
        /// <param name="featureType"></param>
        private IList<OsmGeo> ConvertFeatures(OsmSharp.Tools.Xml.Kml.v2_1.FeatureType[] featureType)
        {
            IList<OsmGeo> relations = new List<OsmGeo>();
            foreach (OsmSharp.Tools.Xml.Kml.v2_1.FeatureType feature in featureType)
            {
                OsmGeo relation = this.ConvertFeature(feature);
                if (relation != null)
                {
                    relations.Add(relation);
                }
            }

            return relations;
        }

        /// <summary>
        /// Converts a feature and all it's contents to osm elements.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private OsmGeo ConvertFeature(OsmSharp.Tools.Xml.Kml.v2_1.FeatureType feature)
        {
            if (feature is OsmSharp.Tools.Xml.Kml.v2_1.ContainerType)
            {
                return this.ConvertContainer(feature as OsmSharp.Tools.Xml.Kml.v2_1.ContainerType);
            }
            else if (feature is OsmSharp.Tools.Xml.Kml.v2_1.PlacemarkType)
            {
                return this.ConvertPlacemark(feature as OsmSharp.Tools.Xml.Kml.v2_1.PlacemarkType);
            }
            return null;
        }

        /// <summary>
        /// Conversts a placemark and all it's contents to osm elements.
        /// </summary>
        /// <param name="placemark"></param>
        /// <returns></returns>
        private OsmGeo ConvertPlacemark(OsmSharp.Tools.Xml.Kml.v2_1.PlacemarkType placemark)
        {
            return this.ConvertGeometry(placemark.Item1);
        }

        /// <summary>
        /// Converts a geometry to a list of osm objects.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        private OsmGeo ConvertGeometry(OsmSharp.Tools.Xml.Kml.v2_1.GeometryType geometry)
        {
            if (geometry is OsmSharp.Tools.Xml.Kml.v2_1.PointType)
            {
                return this.ConvertPoint(geometry as OsmSharp.Tools.Xml.Kml.v2_1.PointType);
                
            }
            else if (geometry is OsmSharp.Tools.Xml.Kml.v2_1.LineStringType)
            {
                return this.ConvertLineString(geometry as OsmSharp.Tools.Xml.Kml.v2_1.LineStringType);
            }
            else if (geometry is OsmSharp.Tools.Xml.Kml.v2_1.LinearRingType)
            {
                return this.ConvertLinearRing(geometry as OsmSharp.Tools.Xml.Kml.v2_1.LinearRingType);
            }
            else if (geometry is OsmSharp.Tools.Xml.Kml.v2_1.PolygonType)
            {
                return this.ConvertPolygon(geometry as OsmSharp.Tools.Xml.Kml.v2_1.PolygonType);
            }
            else if (geometry is OsmSharp.Tools.Xml.Kml.v2_1.MultiGeometryType)
            {
                return this.ConvertMultiGeometry(geometry as OsmSharp.Tools.Xml.Kml.v2_1.MultiGeometryType);
            }
            return null;
        }

        /// <summary>
        /// Converts the multi geometry to multi osm objects.
        /// </summary>
        /// <param name="multiGeometry"></param>
        /// <returns></returns>
        private OsmGeo ConvertMultiGeometry(OsmSharp.Tools.Xml.Kml.v2_1.MultiGeometryType multiGeometry)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Visible = true;
            relation.Tags.Add("kml_type", "MultiGeometryType");

            foreach (OsmSharp.Tools.Xml.Kml.v2_1.GeometryType geo in multiGeometry.Items)
            {
                OsmGeo geo_osm = this.ConvertGeometry(geo);
                if (geo_osm != null)
                {
                    RelationMember member = new RelationMember();
                    member.Member = geo_osm;
                    relation.Members.Add(member);
                }
            }

            return relation;
        }

        /// <summary>
        /// Convests the polygon to osm objects.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private OsmGeo ConvertPolygon(OsmSharp.Tools.Xml.Kml.v2_1.PolygonType polygon)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Visible = true;
            relation.Tags.Add("kml_type", "PolygonType");

            OsmGeo inner = this.ConvertBoundary(polygon.innerBoundaryIs);
            if (inner != null)
            {
                RelationMember member = new RelationMember();
                member.Member = inner;
                member.Role = "inner";
                relation.Members.Add(member);
            }
            OsmGeo outer = this.ConvertLinearRing(polygon.outerBoundaryIs.LinearRing);
            if (outer != null)
            {
                RelationMember member = new RelationMember();
                member.Member = outer;
                member.Role = "outer";
                relation.Members.Add(member);
            }

            return relation;
        }

        /// <summary>
        /// Converts boundary type into an osm object.
        /// </summary>
        /// <param name="boundary"></param>
        /// <returns></returns>
        private OsmGeo ConvertBoundary(OsmSharp.Tools.Xml.Kml.v2_1.boundaryType[] boundary)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Visible = true;
            relation.Tags.Add("kml_type", "boundaryType[]");

            foreach (OsmSharp.Tools.Xml.Kml.v2_1.boundaryType geo in boundary)
            {
                OsmGeo geo_osm = this.ConvertLinearRing(geo.LinearRing);
                if (geo_osm != null)
                {
                    RelationMember member = new RelationMember();
                    member.Member = geo_osm;
                    relation.Members.Add(member);
                }
            }

            return relation;
        }

        /// <summary>
        /// Converts a lineairring into osm objects.
        /// </summary>
        /// <param name="linearRing"></param>
        /// <returns></returns>
        private OsmGeo ConvertLinearRing(OsmSharp.Tools.Xml.Kml.v2_1.LinearRingType linearRing)
        {
            Way way = OsmBaseFactory.CreateWay(KeyGenerator.GenerateNew());

            way.Tags.Add("kml_type", "LinearRingType");
            way.Tags.Add("area", "yes");

            IList<GeoCoordinate> coordinates = this.ConvertCoordinates(linearRing.coordinates);
            foreach (GeoCoordinate coordinate in coordinates)
            {
                Node n = OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());
                n.Coordinate = coordinate;
                way.Nodes.Add(n);
            }

            return way;
        }

        /// <summary>
        /// Converts a line string into an osm object.
        /// </summary>
        /// <param name="lineString"></param>
        /// <returns></returns>
        private OsmGeo ConvertLineString(OsmSharp.Tools.Xml.Kml.v2_1.LineStringType lineString)
        {
            Way way = OsmBaseFactory.CreateWay(KeyGenerator.GenerateNew());

            way.Tags.Add("kml_type", "LineStringType");

            IList<GeoCoordinate> coordinates = this.ConvertCoordinates(lineString.coordinates);
            foreach (GeoCoordinate coordinate in coordinates)
            {
                Node n = OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());
                n.Coordinate = coordinate;
                way.Nodes.Add(n);
            }

            return way;
        }

        /// <summary>
        /// Converts a point into an oms object.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private OsmGeo ConvertPoint(OsmSharp.Tools.Xml.Kml.v2_1.PointType point)
        {
            Node n = OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());
            IList<GeoCoordinate> coordinates = this.ConvertCoordinates(point.coordinates);

            n.Tags.Add("kml_type", "PointType");

            n.Coordinate = coordinates[0];

            return n;
        }

        /// <summary>
        /// Converts a container and it's contents to osm elements.
        /// </summary>
        /// <param name="container"></param>
        private Relation ConvertContainer(OsmSharp.Tools.Xml.Kml.v2_1.ContainerType container)
        {
            // get the features.
            if (container is OsmSharp.Tools.Xml.Kml.v2_1.FolderType)
            {
                OsmSharp.Tools.Xml.Kml.v2_1.FolderType folder = (container as OsmSharp.Tools.Xml.Kml.v2_1.FolderType);

                // items1 are the features.
                IList<OsmGeo> created_features = this.ConvertFeatures(folder.Items1);

                // create the relation for this folder.
                Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());
                relation.Tags.Add("name", folder.name);
                relation.Visible = folder.visibility;
                relation.Tags.Add("description", folder.description);
                relation.Tags.Add("kml_folder", "yes");

                foreach (OsmGeo geo in created_features)
                {
                    RelationMember member = new RelationMember();
                    member.Member = geo;
                    member.Role = "";
                    relation.Members.Add(member);
                }

                return relation;
            }
            else if(container is OsmSharp.Tools.Xml.Kml.v2_1.DocumentType)
            {
                OsmSharp.Tools.Xml.Kml.v2_1.DocumentType document = (container as OsmSharp.Tools.Xml.Kml.v2_1.DocumentType);

                // items1 are the features.
                IList<OsmGeo> created_features = this.ConvertFeatures(document.Items1);

                // create the relation for this folder.
                Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());
                relation.Tags.Add("name", document.name);
                relation.Visible = document.visibility;
                relation.Tags.Add("description", document.description);
                relation.Tags.Add("kml_folder", "yes");

                foreach (OsmGeo geo in created_features)
                {
                    RelationMember members = new RelationMember();
                    members.Member = geo;
                    members.Role = "";
                }

                return relation;                
            }
            return null;
        }

        

        #endregion

        /// <summary>
        /// Converts a list of coordinates to geocoordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        private IList<GeoCoordinate> ConvertCoordinates(string coordinates)
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

                    geo_coordinates.Add(new GeoCoordinate(latitude,longitude));
                }
            }
            return geo_coordinates;
        }


        #endregion

        #region IDataSourceReadOnly Members

        /// <summary>
        /// Returns the bounding box around all the data in this source.
        /// </summary>
        public GeoCoordinateBox BoundingBox
        {
            get
            {
                this.ReadFromDocument();

                if (_bb == null)
                { // calculate bounding box.
                    if (_nodes.Count > 0
                        || _ways.Count > 0
                        || _relations.Count > 0)
                    {
                        foreach (Node node in _nodes.Values)
                        {
                            if (_bb == null)
                            {
                                _bb = node.BoundingBox;
                            }
                            else
                            {
                                _bb = node.BoundingBox + _bb;
                            }
                        }
                        foreach (Way way in _ways.Values)
                        {
                            if (_bb == null)
                            {
                                _bb = way.BoundingBox;
                            }
                            else
                            {
                                _bb = way.BoundingBox + _bb;
                            }
                        }
                        foreach (Relation relation in _relations.Values)
                        {
                            if (_bb == null)
                            {
                                _bb = relation.BoundingBox;
                            }
                            else
                            {
                                _bb = relation.BoundingBox + _bb;
                            }
                        }
                    }
                    else
                    {
                        _bb = null;
                    }
                }
                return _bb;
            }
        }

        /// <summary>
        /// Returns the unique guid for this source.
        /// </summary>
        public Guid Id
        {
            get 
            {
                return _id;
            }
        }

        /// <summary>
        /// Return true; kml always has a bounding box.
        /// </summary>
        public bool HasBoundinBox
        {
            get 
            { 
                return true; 
            }
        }

        /// <summary>
        /// Returns true; kml always readonly.
        /// 
        /// It may be possible to add objects in a non-standard way!
        /// </summary>
        public bool IsReadOnly
        {
            get 
            {
                return true; 
            }
        }

        #region Nodes


        /// <summary>
        /// Returns the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Node GetNode(long id)
        {
            if (_nodes.ContainsKey(id))
            {
                return _nodes[id];
            }
            return null;
        }

        /// <summary>
        /// Returns the nodes with the id's in the ids list.
        /// 
        /// The returned list will have the same size as the original
        /// and the returned nodes will be in the same position as their id's.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Node> GetNodes(IList<long> ids)
        {
            IList<Node> ret_list = new List<Node>(ids.Count);

            for (int idx = 0; idx < ids.Count; idx++)
            {
                long id = ids[idx];
                ret_list.Add(this.GetNode(id));
            }

            return ret_list;
        }

        #endregion

        #region Relations

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Relation GetRelation(long id)
        {
            if (_relations.ContainsKey(id))
            {
                return _relations[id];
            }
            return null;
        }

        /// <summary>
        /// Returns the relations with the given id's.
        /// 
        /// The returned list will have the same size as the original
        /// and the returned relations will be in the same position as their id's.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Relation> GetRelations(IList<long> ids)
        {
            IList<Relation> ret_list = new List<Relation>(ids.Count);

            for (int idx = 0; idx < ids.Count; idx++)
            {
                long id = ids[idx];
                ret_list.Add(this.GetRelation(id));
            }

            return ret_list;
        }

        /// <summary>
        /// Returns the relations for the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IList<Relation> GetRelationsFor(OsmBase obj)
        {
            if (_relations_per_member.ContainsKey(obj.Id))
            {
                return this.GetRelations(_relations_per_member[obj.Id]);
            }
            return null;
        }


        #endregion

        #region Way

        /// <summary>
        /// Returns the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Way GetWay(long id)
        {
            if (_ways.ContainsKey(id))
            {
                return _ways[id];
            }
            return null;
        }

        /// <summary>
        /// Returns the ways with the id's in the ids list.
        /// 
        /// The returned list will have the same size as the original
        /// and the returned ways will be in the same position as their id's.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Way> GetWays(IList<long> ids)
        {
            IList<Way> ret_list = new List<Way>(ids.Count);

            for (int idx = 0; idx < ids.Count; idx++)
            {
                long id = ids[idx];
                ret_list.Add(this.GetWay(id));
            }

            return ret_list;
        }

        /// <summary>
        /// Returns the way(s) for the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IList<Way> GetWaysFor(Node node)
        {
            if (_ways_per_node.ContainsKey(node.Id))
            {
                return this.GetWays(_ways_per_node[node.Id]);
            }
            return null;
        }

        #endregion

        #region Queries


        /// <summary>
        /// Returns the objects that evaluate the filter to true.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<OsmBase> Get(Filter filter)
        {
            this.ReadFromDocument();

            IList<OsmBase> res = new List<OsmBase>();
            foreach (Node node in _nodes.Values)
            {
                if (filter.Evaluate(node))
                {
                    res.Add(node);
                }
            }
            foreach (Way way in _ways.Values)
            {
                if (filter.Evaluate(way))
                {
                    res.Add(way);
                }
            }
            foreach (Relation relation in _relations.Values)
            {
                if (filter.Evaluate(relation))
                {
                    res.Add(relation);
                }
            }

            return res;
        }

        /// <summary>
        /// Returns the objects that exist withing the given box and evaluate the filter to true.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
        {
            this.ReadFromDocument();

            IList<OsmGeo> res = new List<OsmGeo>();
            foreach (Node node in _nodes.Values)
            {
                if (filter.Evaluate(node) && node.Shape.Inside(box))
                {
                    res.Add(node);
                }
            }
            foreach (Way way in _ways.Values)
            {
                if (filter.Evaluate(way) && way.Shape.Inside(box))
                {
                    res.Add(way);
                }
            }
            foreach (Relation relation in _relations.Values)
            {
                if (filter.Evaluate(relation) && relation.Shape.Inside(box))
                {
                    res.Add(relation);
                }
            }

            return res;
        }

        #endregion

        #endregion

        #region Private Add Functions

        /// <summary>
        /// Adds a list of osm objects.
        /// </summary>
        /// <param name="objs"></param>
        private void AddOsmBase(IList<OsmBase> objs)
        {
            foreach (OsmBase obj in objs)
            {
                this.AddOsmBase(obj);
            }
        }

        /// <summary>
        /// Adds an osm object.
        /// </summary>
        /// <param name="obj"></param>
        private void AddOsmBase(OsmBase obj)
        {
            switch (obj.Type)
            {
                case OsmType.Node:
                    this.AddNode(obj as Node);
                    break;
                case OsmType.Relation:
                    this.AddRelation(obj as Relation);
                    break;
                case OsmType.Way:
                    this.AddWay(obj as Way);
                    break;
            }
        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node"></param>
        private void AddNode(Node node)
        {
            if (_nodes.ContainsKey(node.Id))
            {
                throw new InvalidOperationException("Cannot add an object that already exists in this source!" + Environment.NewLine +
                    "If there is a modification use a changeset!");
            }
            else
            {
                _nodes.Add(node.Id, node);
            }
        }

        /// <summary>
        /// Adds a way.
        /// </summary>
        /// <param name="way"></param>
        private void AddWay(Way way)
        {
            if (_ways.ContainsKey(way.Id))
            {
                throw new InvalidOperationException("Cannot add an object that already exists in this source!" + Environment.NewLine +
                    "If there is a modification use a changeset!");
            }
            else
            {
                _ways.Add(way.Id, way);

                foreach (Node node in way.Nodes)
                {
                    if (this.GetNode(node.Id) == null)
                    {
                        this.AddNode(node);
                    }
                }

            }
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="relation"></param>
        private void AddRelation(Relation relation)
        {
            if (_relations.ContainsKey(relation.Id))
            {
                throw new InvalidOperationException("Cannot add an object that already exists in this source!" + Environment.NewLine +
                    "If there is a modification use a changeset!");
            }
            else
            {
                _relations.Add(relation.Id, relation);

                foreach (RelationMember member in relation.Members)
                {
                    OsmGeo member_already_in = null;
                    switch (member.Member.Type)
                    {
                        case OsmType.Node:
                            member_already_in = this.GetNode(member.Member.Id);
                            break;
                        case OsmType.Relation:
                            member_already_in = this.GetRelation(member.Member.Id);
                            break;
                        case OsmType.Way:
                            member_already_in = this.GetWay(member.Member.Id);
                            break;
                    }
                    if (member_already_in == null)
                    {
                        this.AddOsmBase(member.Member);
                    }
                }

            }
        }

        #endregion
    }
}

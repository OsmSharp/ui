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
using OsmSharp.Tools.Xml.Gpx;
using OsmSharp.Osm.Factory;
using OsmSharp.Osm;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Filters;

namespace OsmSharp.Osm.Data.Raw.XML.GpxSource
{
    /// <summary>
    /// A gpx data source.
    /// 
    /// Reads gpx data and translates the objects to osm objects.
    /// </summary>
    public class GpxDataSource : IDataSourceReadOnly
    {
        /// <summary>
        /// The gpx document this datasource is for.
        /// </summary>
        private GpxDocument _document;

        /// <summary>
        /// The id of this data source.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Creates a new osm data source.
        /// </summary>
        /// <param name="document"></param>
        public GpxDataSource(GpxDocument document)
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

        private string source_name;

        /// <summary>
        /// Reads all the data from the osm document if needed.
        /// </summary>
        private void ReadFromDocument()
        {
            if (!_read)
            {
                _read = true;


                if (_document.Gpx == null)
                { // no data.
                    return;
                }

                switch (_document.Version)
                {
                    case GpxVersion.Gpxv1_0:
                        OsmSharp.Tools.Xml.Gpx.v1_0.gpx gpx_v1_0 = _document.Gpx as OsmSharp.Tools.Xml.Gpx.v1_0.gpx;

                        Relation relation_v1_0 = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

                        relation_v1_0.Tags.Add("name", gpx_v1_0.name);
                        relation_v1_0.Tags.Add("description", gpx_v1_0.desc);
                        relation_v1_0.Tags.Add("gpx_type", "gpx");

                        foreach (OsmSharp.Tools.Xml.Gpx.v1_0.gpxTrk trk in gpx_v1_0.trk)
                        {
                            OsmGeo trk_osm = this.ConvertGpxTrk(trk);
                            if (trk_osm != null)
                            {
                                RelationMember member = new RelationMember();
                                member.Member = trk_osm;
                                member.Role = "";
                                relation_v1_0.Members.Add(member);
                            }
                        }

                        this.AddRelation(relation_v1_0);
                        break;
                    case GpxVersion.Gpxv1_1:
                        OsmSharp.Tools.Xml.Gpx.v1_1.gpxType gpx_v1_1 = _document.Gpx as OsmSharp.Tools.Xml.Gpx.v1_1.gpxType;

                        if (gpx_v1_1.metadata != null
                            && !string.IsNullOrEmpty(gpx_v1_1.metadata.name))
                        {
                            source_name = gpx_v1_1.metadata.name;
                        }

                        Relation relation_v1_1 = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

                        //relation_v1_1.Tags.Add("name", _document.Name);
                        relation_v1_1.Tags.Add("description", "v1.1");
                        relation_v1_1.Tags.Add("gpx_type", "gpxType");

                        foreach (OsmSharp.Tools.Xml.Gpx.v1_1.trkType trk in gpx_v1_1.trk)
                        {
                            OsmGeo trk_osm = this.ConvertGpxTrk(trk);
                            if (trk_osm != null)
                            {
                                RelationMember member = new RelationMember();
                                member.Member = trk_osm;
                                member.Role = "";
                                relation_v1_1.Members.Add(member);
                            }
                        }


                        foreach (OsmSharp.Tools.Xml.Gpx.v1_1.wptType wpt in gpx_v1_1.wpt)
                        {
                            Node n = this.ConvertWptType(wpt);

                            n.Tags.Add("type", "wpt");

                            this.AddNode(n);
                        }


                        this.AddRelation(relation_v1_1);
                        break;
                }
            }
        }

        #region v1.1

        /// <summary>
        /// Converts a gpx track into osm objects.
        /// </summary>
        /// <param name="trk"></param>
        /// <returns></returns>
        private OsmGeo ConvertGpxTrk(OsmSharp.Tools.Xml.Gpx.v1_1.trkType trk)
        {
            Relation relation = OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());

            relation.Tags.Add("name", trk.name);
            relation.Tags.Add("number", trk.number);
            relation.Tags.Add("description", trk.desc);
            relation.Tags.Add("type", trk.type);

            foreach (OsmSharp.Tools.Xml.Gpx.v1_1.trksegType seg in trk.trkseg)
            {
                OsmGeo seg_geo = this.ConvertTrkSegType(seg);
                if (seg_geo != null)
                {
                    RelationMember member = new RelationMember();
                    member.Member = seg_geo;
                    member.Role = "";
                    relation.Members.Add(member);
                }
            }

            return relation;
        }

        /// <summary>
        /// Converts a track segment into osm objects.
        /// </summary>
        /// <param name="seg"></param>
        /// <returns></returns>
        private OsmGeo ConvertTrkSegType(OsmSharp.Tools.Xml.Gpx.v1_1.trksegType seg)
        {
            Way way = OsmBaseFactory.CreateWay(KeyGenerator.GenerateNew());

            foreach (OsmSharp.Tools.Xml.Gpx.v1_1.wptType pt in seg.trkpt)
            {
                Node n = this.ConvertWptType(pt);
                if (n != null)
                {
                    way.Nodes.Add(n);
                }
            }

            return way;
        }

        /// <summary>
        /// Converts a track point into osm objects.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private Node ConvertWptType(OsmSharp.Tools.Xml.Gpx.v1_1.wptType pt)
        {
            Node n = OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());

            n.TimeStamp = pt.time;
            n.Coordinate = new GeoCoordinate((double)pt.lat, (double)pt.lon);

            return n;
        }

        #endregion

        #region v1.0

        /// <summary>
        /// Converts a gpx track into osm objects.
        /// </summary>
        /// <param name="trk"></param>
        /// <returns></returns>
        private OsmGeo ConvertGpxTrk(OsmSharp.Tools.Xml.Gpx.v1_0.gpxTrk trk)
        {
            Way way = OsmBaseFactory.CreateWay(KeyGenerator.GenerateNew());
            foreach (OsmSharp.Tools.Xml.Gpx.v1_0.gpxTrkTrksegTrkpt seg in trk.trkseg)
            {
                Node seg_geo = this.ConvertTrkSegType(seg);
                if (seg_geo != null)
                {
                    way.Nodes.Add(seg_geo);
                }
            }

            return way;
        }

        /// <summary>
        /// Converts a track segment into osm objects.
        /// </summary>
        /// <param name="seg"></param>
        /// <returns></returns>
        private Node ConvertTrkSegType(OsmSharp.Tools.Xml.Gpx.v1_0.gpxTrkTrksegTrkpt seg)
        {
            Node n = OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());

            n.TimeStamp = seg.time;
            n.Coordinate = new GeoCoordinate((double)seg.lat, (double)seg.lon);

            return n;
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

                    geo_coordinates.Add(new GeoCoordinate(latitude, longitude));
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
        /// Return true; gpx always has a bounding box.
        /// </summary>
        public bool HasBoundinBox
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns true; gpx always readonly.
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
            this.ReadFromDocument();

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
            this.ReadFromDocument();

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
            this.ReadFromDocument();

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
            this.ReadFromDocument();

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
            this.ReadFromDocument();

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
            this.ReadFromDocument();

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
            this.ReadFromDocument();

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
            this.ReadFromDocument();

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
                if ((filter == null
                    || filter.Evaluate(node)) && node.Shape.Inside(box))
                {
                    res.Add(node);
                }
            }
            foreach (Way way in _ways.Values)
            {
                if ((filter == null
                    || filter.Evaluate(way)) && way.Shape.Inside(box))
                {
                    res.Add(way);
                }
            }
            foreach (Relation relation in _relations.Values)
            {
                if ((filter == null
                    || filter.Evaluate(relation)) && relation.Shape.Inside(box))
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
                node.Tags.Add("metadata_name", source_name);
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
                way.Tags.Add("metadata_name", source_name);
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
                relation.Tags.Add("metadata_name", source_name);
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

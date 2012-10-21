// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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
using System.Net;
using Osm.Core;
using System.IO;
using System.Xml;
using Osm.Core.Xml;
using Tools.Xml.Sources;
using Osm.Core.Factory;
using System.Globalization;

namespace Osm.Data.Raw.XML.ApiSource
{

    public class ApiDataSource : IApi
    {
        /// <summary>
        /// Holds the currently authenticated user.
        /// </summary>
        private User _user;

        /// <summary>
        /// Holds the current users's password.
        /// </summary>
        private string _pass;

        /// <summary>
        /// Holds the url to the currently used api.
        /// </summary>
        private string _api_url;

        /// <summary>
        /// Creates a new api data source.
        /// </summary>
        /// <param name="url"></param>
        public ApiDataSource(string url)
        {
            _api_url = url;
        }

        #region Private Api Call Functions

        /// <summary>
        /// Does an actual api call and returnes the webresponse.
        /// </summary>
        /// <param name="authenticate"></param>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private WebResponse DoApiCall(bool authenticate,string url, Method method)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_api_url + url);
            switch (method)
            {
                case Method.GET:
                    request.Method = "GET";
                    break;
                case Method.PUT:
                    request.Method = "PUT";
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format("Method {0} is not supported!", method));
            }
            //if (authenticate)
            //{
            //    request.Credentials = new NetworkCredential(_user.Name, _pass);
            //}
            //request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            //request.PreAuthenticate = false;
            //request.UseDefaultCredentials = false;

            return request.GetResponse();
        }

        /// <summary>
        /// Does an api call to get an osm document.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private OsmDocument DoGetApiCall(string url)
        {
            WebResponse response = this.DoApiCall(false, url, Method.GET);
            XmlReaderSource xml_source = new XmlReaderSource(XmlReader.Create(response.GetResponseStream()));
            return new OsmDocument(xml_source);
        }

        #endregion

        #region IApi Members

        /// <summary>
        /// Authenticates the given user using the given password.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public bool Authenticate(Osm.Core.User user, string pass)
        {
            _user = user;
            _pass = pass;


            // TODO: update the usage of this authentication and find a way to check this.
            // test the authentication.
            WebResponse resp = this.DoApiCall(false, "/api/capabilities", Method.GET);

            return true;
        }

        // BACKUP SOLUTION
        //private void SetBasicAuthHeader(WebRequest req, String userName, String userPassword)
        //{
        //    string authInfo = userName + ":" + userPassword;
        //    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
        //    req.Headers["Authorization"] = "Basic " + authInfo;
        //}

        public Osm.Core.User AuthenticatedUser
        {
            get 
            {
                return _user;
            }
        }

        #endregion

        #region IDataSource Members

        public void Persist()
        {
            throw new NotImplementedException();
        }

        public bool HasBoundinBox
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsBaseIdGenerator
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsCreator
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsLive
        {
            get { throw new NotImplementedException(); }
        }

        public Osm.Core.Node CreateNode()
        {
            throw new NotImplementedException();
        }

        public Osm.Core.Relation CreateRelation()
        {
            throw new NotImplementedException();
        }

        public Osm.Core.Way CreateWay()
        {
            throw new NotImplementedException();
        }

        public void ApplyChangeSet(Osm.Core.ChangeSet change_set)
        {
            throw new NotImplementedException();
        }

        public Osm.Core.ChangeSet CreateChangeSet()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDataSourceReadOnly Members

        public Tools.Math.Geo.GeoCoordinateBox BoundingBox
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public Guid Id
        {
            get { throw new NotImplementedException(); }
        }


        public Node GetNode(long id)
        {
            OsmDocument doc = this.DoGetApiCall(string.Format(
                "/api/0.6/node/{0}", id));
            object osm = doc.Osm;

            Osm.Core.Xml.v0_6.node xml_node = (osm as Osm.Core.Xml.v0_6.osm).node[0];
            return this.Convertv6XmlNode(xml_node);
        }

        private Node Convertv6XmlNode(Osm.Core.Xml.v0_6.node xml_node)
        {
            Node node = OsmBaseFactory.CreateNode(xml_node.id);
            node.Coordinate = new Tools.Math.Geo.GeoCoordinate(xml_node.lat, xml_node.lon);
            if (xml_node.tag != null)
            {
                foreach (Osm.Core.Xml.v0_6.tag xml_tag in xml_node.tag)
                {
                    node.Tags.Add(xml_tag.k, xml_tag.v);
                }
            }
            node.ChangeSetId = xml_node.changeset;
            node.TimeStamp = xml_node.timestamp;
            node.User = xml_node.user;
            node.UserId = xml_node.uid;
            node.Version = (int)xml_node.version;
            node.Visible = xml_node.visible;
            return node;
        }

        public IList<Osm.Core.Node> GetNodes(IList<long> ids)
        {
            IList<Osm.Core.Node> nodes = new List<Osm.Core.Node>();

            if (ids != null && ids.Count > 0)
            {
                string ids_string = ids[0].ToString();
                for (int idx = 1; idx < ids.Count; idx++)
                {
                    ids_string = ids_string + ",";
                    ids_string = ids_string + ids[idx].ToString();
                }

                OsmDocument doc = this.DoGetApiCall(string.Format(
                    "/api/0.6/nodes?nodes={0}", ids_string));

                object osm = doc.Osm;

                if ((osm as Osm.Core.Xml.v0_6.osm).node != null)
                {
                    foreach (Osm.Core.Xml.v0_6.node xml_node in (osm as Osm.Core.Xml.v0_6.osm).node)
                    {
                        nodes.Add(
                            this.Convertv6XmlNode(xml_node));
                    }
                }
            }

            return nodes;
        }

        public Osm.Core.Relation GetRelation(long id)
        {
            throw new NotImplementedException();
        }

        public IList<Osm.Core.Relation> GetRelations(IList<long> ids)
        {
            throw new NotImplementedException();
        }

        public IList<Osm.Core.Relation> GetRelationsFor(Osm.Core.OsmBase obj)
        {
            throw new NotImplementedException();
        }

        public Osm.Core.Way GetWay(long id)
        {
            OsmDocument doc = this.DoGetApiCall(string.Format(
                "/api/0.6/way/{0}/full", id));
            object osm = doc.Osm;

            // index the nodes first.
            Dictionary<long, Node> nodes = new Dictionary<long, Node>();
            if ((osm as Osm.Core.Xml.v0_6.osm).node != null)
            {
                foreach (Osm.Core.Xml.v0_6.node xml_node in (osm as Osm.Core.Xml.v0_6.osm).node)
                {
                    Node node = this.Convertv6XmlNode(xml_node);
                    nodes.Add(node.Id, node);                        
                }
            }

            // create the way.
            return this.Convertv6XmlWay((osm as Osm.Core.Xml.v0_6.osm).way[0],nodes);
        }

        private Way Convertv6XmlWay(Osm.Core.Xml.v0_6.way xml_way, Dictionary<long, Node> indexed_nodes)
        {
            Way way = OsmBaseFactory.CreateWay(xml_way.id);
            if (xml_way.nd != null)
            {
                for (int idx = 0; idx < xml_way.nd.Length; idx++)
                {
                    way.Nodes.Add(
                        indexed_nodes[xml_way.nd[idx].@ref]);
                }
            }
            if (xml_way.tag != null)
            {
                foreach (Osm.Core.Xml.v0_6.tag xml_tag in xml_way.tag)
                {
                    way.Tags.Add(xml_tag.k, xml_tag.v);
                }
            }
            way.ChangeSetId = xml_way.changeset;
            way.TimeStamp = xml_way.timestamp;
            way.User = xml_way.user;
            way.UserId = xml_way.uid;
            way.Version = (int)xml_way.version;
            way.Visible = xml_way.visible;

            return way;
        }

        public IList<Osm.Core.Way> GetWays(IList<long> ids)
        {
            IList<Osm.Core.Way> ways = new List<Osm.Core.Way>();

            if (ids != null && ids.Count > 0)
            {
                foreach(long id in ids)
                {
                    ways.Add(this.GetWay(id));
                }
            }

            return ways;
        }

        public IList<Osm.Core.Way> GetWaysFor(Osm.Core.Node node)
        {
            IList<Osm.Core.Way> ways = new List<Osm.Core.Way>();

            OsmDocument doc = this.DoGetApiCall(string.Format("/api/0.6/node/{0}/ways", node.Id));
            object osm = doc.Osm;

            if ((osm as Osm.Core.Xml.v0_6.osm).way != null)
            {
                foreach (Osm.Core.Xml.v0_6.way xml_way in (osm as Osm.Core.Xml.v0_6.osm).way)
                {
                    ways.Add(this.GetWay(xml_way.id));
                }
            }
            return ways;
        }

        public IList<Osm.Core.OsmBase> Get(Tools.Math.Geo.GeoCoordinateBox box, Osm.Core.Filters.Filter filter)
        {
            IList<Osm.Core.OsmBase> base_objects = new List<Osm.Core.OsmBase>();

            // /api/0.6/map?bbox=left,bottom,right,top
            OsmDocument doc = this.DoGetApiCall(string.Format("/api/0.6/map?bbox={0},{1},{2},{3}",
                box.MinLon.ToString(CultureInfo.InvariantCulture),
                box.MinLat.ToString(CultureInfo.InvariantCulture),
                box.MaxLon.ToString(CultureInfo.InvariantCulture),
                box.MaxLat.ToString(CultureInfo.InvariantCulture)));
            object osm = doc.Osm;

            // get the nodes first and index for usage in the ways.
            Dictionary<long, Node> nodes = new Dictionary<long, Node>();
            if ((osm as Osm.Core.Xml.v0_6.osm).node != null)
            {
                foreach (Osm.Core.Xml.v0_6.node xml_node in (osm as Osm.Core.Xml.v0_6.osm).node)
                {
                    Node node = this.Convertv6XmlNode(xml_node);
                    nodes.Add(node.Id, node);
                    base_objects.Add(node);
                }
            }

            // get the ways using the above nodes as much as possible.
            if ((osm as Osm.Core.Xml.v0_6.osm).way != null)
            {
                foreach (Osm.Core.Xml.v0_6.way xml_way in (osm as Osm.Core.Xml.v0_6.osm).way)
                {
                    foreach (Osm.Core.Xml.v0_6.nd xml_way_node in xml_way.nd)
                    {
                        if (!nodes.ContainsKey(xml_way_node.@ref))
                        {
                            Node extra_node = this.GetNode(xml_way_node.@ref);
                            nodes.Add(extra_node.Id, extra_node);
                        }
                    }
                    base_objects.Add(
                        this.Convertv6XmlWay(xml_way, nodes));
                }
            }

            // TODO: relations!

            return base_objects;
        }

        #endregion

        private enum Method
        {
            PUT,
            GET
        }

    }
}

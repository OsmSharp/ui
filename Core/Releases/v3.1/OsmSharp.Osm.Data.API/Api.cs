using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using OsmSharp.Osm.Core.Simple;
using OsmSharp.Osm.Core.Xml.v0_6;
using OsmSharp.Osm.Core;
using OsmSharp.Tools.Xml.Sources;
using System.Xml;
using OsmSharp.Osm.Core.Xml;
using OsmSharp.Osm.Core.Factory;

namespace OsmSharp.Osm.Data.API
{
    /// <summary>
    /// Represents an osm api instance.
    /// </summary>
    public class Api
    {
        /// <summary>
        /// Holds a username.
        /// </summary>
        private string _user;

        /// <summary>
        /// Holds the password.
        /// </summary>
        private string _pass;

        /// <summary>
        /// Creates a new api instance.
        /// </summary>
        public Api(string url, string user, string pass)
        {
            _user = user;
            _pass = pass;

            this.Uri = new Uri(url);
        }

        /// <summary>
        /// The uri of the api.
        /// </summary>
        public Uri Uri
        {
            get;
            private set;
        }

        #region Http Connection

        /// <summary>
        /// Does an actual api call and returnes the webresponse.
        /// </summary>
        /// <param name="authenticate"></param>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private string DoApiCall(bool authenticate, string url, Method method, byte[] data)
        {
            WebRequest request = WebRequest.Create(new Uri(this.Uri.AbsoluteUri + url));
            this.SetBasicAuthHeader(request);
            HttpWebResponse response;
            Encoding enc;
            StreamReader response_stream;
            string response_string;
            switch (method)
            {
                case Method.PUT:
                    request.Method = "PUT";

                    // build the data buffer.
                    request.ContentLength = data.Length; // set content length.

                    // get the request stream and write the data.
                    Stream request_stream = request.GetRequestStream();
                    request_stream.Write(data, 0, data.Length);
                    request_stream.Close();

                    // get the response.
                    response = (HttpWebResponse)request.GetResponse();
                    enc = System.Text.Encoding.GetEncoding(1252);
                    response_stream =
                       new StreamReader(response.GetResponseStream(), enc);
                    response_string = response_stream.ReadToEnd();

                    // close everything.
                    response.Close();
                    response_stream.Close();

                    return response_string;
                case Method.GET:
                    response_string = string.Empty;
                    try
                    {
                        request.Method = "GET";

                        // get the response.
                        response = (HttpWebResponse)request.GetResponse();
                        enc = System.Text.Encoding.GetEncoding(1252);
                        response_stream =
                           new StreamReader(response.GetResponseStream(), enc);
                        response_string = response_stream.ReadToEnd();

                        // close everything.
                        response.Close();
                        response_stream.Close();
                    }
                    catch (Exception ex)
                    {

                    }
                    return response_string;
            }
            throw new NotImplementedException();
        }


        /// <summary>
        /// The methods.
        /// </summary>
        private enum Method
        {
            PUT,
            GET
        }

        /// <summary>
        /// Adds the correct authorisation header to the HTTP requests.
        /// </summary>
        /// <param name="req"></param>
        private void SetBasicAuthHeader(WebRequest req)
        {
            string authInfo = _user + ":" + _pass;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
        }

        #endregion

        #region Changesets

        /// <summary>
        /// Holds the current changeset.
        /// </summary>
        private changeset _current_changeset;

        /// <summary>
        /// Holds the current changes.
        /// </summary>
        private osmChange _current_changes;

        /// <summary>
        /// Returns the current change.
        /// </summary>
        /// <returns></returns>
        private osmChange GetCurrentChange()
        {
            if (_current_changes == null)
            {
                _current_changes = new osmChange();
            }
            return _current_changes;
        }

        /// <summary>
        /// Gets the changeset with the given id from the API.
        /// </summary>
        /// <param name="changeset_id"></param>
        /// <returns></returns>
        public OsmSharp.Osm.Core.ChangeSet ChangeSetGet(int changeset_id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new changeset by opening one on the API.
        /// </summary>
        /// <returns></returns>
        public long ChangeSetCreate(string comment)
        {
            // build a new changeset.
            OsmSharp.Osm.Core.Xml.v0_6.osm osm = new Osm.Core.Xml.v0_6.osm();
            osm.changeset = new Osm.Core.Xml.v0_6.changeset[1];
            OsmSharp.Osm.Core.Xml.v0_6.changeset changeset = new Osm.Core.Xml.v0_6.changeset();
            changeset.tag = new OsmSharp.Osm.Core.Xml.v0_6.tag[1];
            changeset.tag[0] = new Osm.Core.Xml.v0_6.tag();
            changeset.tag[0].k = "created_by";
            changeset.tag[0].v = string.Format("OsmSharp {0}",
                System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(2));
            osm.changeset[0] = changeset;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Core.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            string response_string = this.DoApiCall(true, "api/0.6/changeset/create", Method.PUT, put_data);

            _current_changeset = changeset;
            _current_changeset.id = long.Parse(response_string);
            return _current_changeset.id;
        }

        /// <summary>
        /// Close the current changeset on the API.
        /// </summary>
        /// <returns></returns>
        public void ChangeSetClose()
        {
            this.DoApiCall(true, string.Format("api/0.6/changeset/{0}/close", _current_changeset.id), Method.PUT, new byte[0]);
        }


        #endregion

        #region Creation/Modification/Deletion

        /// <summary>
        /// Creates a new node by adding it's creation to the current changeset.
        /// </summary>
        /// <param name="node"></param>
        public void NodeCreate(SimpleNode node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Modifies a the given node by adding it's modification to the current changeset.
        /// </summary>
        /// <param name="node"></param>
        public void NodeUpdate(SimpleNode node)
        {
            // build a new node.
            node xml_node = node.ConvertTo();
            xml_node.changeset = _current_changeset.id;
            xml_node.changesetSpecified = true;

            // encapsulate into an osm object.
            OsmSharp.Osm.Core.Xml.v0_6.osm osm = new Osm.Core.Xml.v0_6.osm();
            osm.node = new Osm.Core.Xml.v0_6.node[1];
            osm.node[0] = xml_node;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Core.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            string response_string = this.DoApiCall(true, string.Format("api/0.6/node/{0}", node.Id.Value), 
                Method.PUT, put_data);
        }

        /// <summary>
        /// Deletes the given node by adding it's deletion to the current changeset.
        /// </summary>
        /// <param name="node"></param>
        public void NodeDelete(SimpleNode node)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleNode NodeGet(long id)
        {
            string response = this.DoApiCall(false, string.Format(
                "/api/0.6/node/{0}", id), Method.GET, null);

            if (response != null && response.Trim().Length > 0)
            {
                XmlReaderSource xml_source = new XmlReaderSource(XmlReader.Create(new StringReader(response)));
                OsmDocument osm = new OsmDocument(xml_source);
                Osm.Core.Xml.v0_6.node xml_node = (osm.Osm as OsmSharp.Osm.Core.Xml.v0_6.osm).node[0];
                return this.Convertv6XmlNode(xml_node);
            }
            return null;
        }

        private SimpleNode Convertv6XmlNode(Osm.Core.Xml.v0_6.node xml_node)
        {
            SimpleNode node = new SimpleNode();
            node.Id = xml_node.id;
            node.Latitude = xml_node.lat;
            node.Longitude = xml_node.lon;

            if (xml_node.tag != null)
            {
                node.Tags = new Dictionary<string, string>();
                foreach (Osm.Core.Xml.v0_6.tag xml_tag in xml_node.tag)
                {
                    node.Tags.Add(xml_tag.k, xml_tag.v);
                }
            }

            node.ChangeSetId = xml_node.changeset;
            node.TimeStamp = xml_node.timestamp;
            node.UserName = xml_node.user;
            node.UserId = xml_node.uid;
            node.Version = xml_node.version;
            node.Visible = xml_node.visible;

            return node;
        }


        #endregion
    }
}

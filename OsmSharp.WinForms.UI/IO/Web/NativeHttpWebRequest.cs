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

using System;

namespace OsmSharp.WinForms.UI.IO.Web
{
    /// <summary>
    /// A native implementation the HttpWebRequest.
    /// </summary>
	internal class NativeHttpWebRequest : OsmSharp.IO.Web.HttpWebRequest
    {
        /// <summary>
        /// Holds the http webrequest.
        /// </summary>
        private System.Net.HttpWebRequest _httpWebRequest;

        /// <summary>
        /// Creates a new default http webrequest.
        /// </summary>
        /// <param name="url"></param>
        public NativeHttpWebRequest(string url)
        {
            _httpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
        }

        /// <summary>
        /// Gets or sets the value of the Accept HTTP header.
        /// </summary>
        public override string Accept
        {
            get
            {
                return _httpWebRequest.Accept;
            }
            set
            {
                _httpWebRequest.Accept = value;
            }
        }

        /// <summary>
        /// Returns true if the user-agent can be set.
        /// </summary>
        public override bool IsUserAgentSupported
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the user-agent if possible.
        /// </summary>
        public override string UserAgent
        {
            get
            {
                return _httpWebRequest.UserAgent;
            }
            set
            {
                _httpWebRequest.UserAgent = value;
            }
        }

        /// <summary>
        /// Begins an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return _httpWebRequest.BeginGetResponse(callback, state);
        }

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="iar"></param>
        /// <returns></returns>
        public override OsmSharp.IO.Web.HttpWebResponse EndGetResponse(IAsyncResult iar)
        {
            return new NativeHttpWebResponse((System.Net.HttpWebResponse)_httpWebRequest.EndGetResponse(iar));
        }

		/// <summary>
		/// Abort this instance.
		/// </summary>
		public override void Abort ()
		{
			throw new System.NotImplementedException ();
		}
    }
}

using System;
using System.Net;

namespace OsmSharp.Wpf.UI.IO.Web
{
    /// <summary>
    /// A native implementation the HttpWebRequest.
    /// </summary>
	internal class NativeHttpWebRequest : OsmSharp.IO.Web.HttpWebRequest
    {
        /// <summary>
        /// Holds the http webrequest.
        /// </summary>
        private readonly HttpWebRequest _httpWebRequest;

        /// <summary>
        /// Creates a new default http webrequest.
        /// </summary>
        /// <param name="url"></param>
        public NativeHttpWebRequest(string url)
        {
            _httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
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
        public override bool IsUserAgentSupported => true;

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
            return new NativeHttpWebResponse((HttpWebResponse)_httpWebRequest.EndGetResponse(iar));
        }

		/// <summary>
		/// Abort this instance.
		/// </summary>
		public override void Abort ()
		{
            _httpWebRequest.Abort();
		}
    }
}

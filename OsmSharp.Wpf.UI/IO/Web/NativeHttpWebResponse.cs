using System.IO;
using System.Net;

namespace OsmSharp.Wpf.UI.IO.Web
{
    /// <summary>
    /// A native implementation the HttpWebResponse.
    /// </summary>
	internal class NativeHttpWebResponse : OsmSharp.IO.Web.HttpWebResponse
    {
        /// <summary>
        /// Holds the http webresponse.
        /// </summary>
        private readonly HttpWebResponse _httpWebResponse;

        /// <summary>
        /// Creates a new http webresponse.
        /// </summary>
        /// <param name="httpWebResponse"></param>
        public NativeHttpWebResponse(HttpWebResponse httpWebResponse)
        {
            _httpWebResponse = httpWebResponse;
        }

        /// <summary>
        /// Gets the status of the response.
        /// </summary>
        public override OsmSharp.IO.Web.HttpStatusCode StatusCode
        {
            get
            {
                switch (_httpWebResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return OsmSharp.IO.Web.HttpStatusCode.NotFound;
                    case HttpStatusCode.Forbidden:
                        return OsmSharp.IO.Web.HttpStatusCode.Forbidden;
                    default:
                        return OsmSharp.IO.Web.HttpStatusCode.Other;
                };
            }
        }

        /// <summary>
        /// Returns the data stream from the Internet resource.
        /// </summary>
        /// <returns></returns>
        public override Stream GetResponseStream()
        {
            return _httpWebResponse.GetResponseStream();
        }

		/// <summary>
		/// Close this instance.
		/// </summary>
		public override void Close ()
		{
            _httpWebResponse.Close();
		}
    }
}
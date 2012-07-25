using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Tools.Xml;
using Tools.Xml.Sources;
using Tools.Xml.Nomatim.Search;
using Tools.Xml.Nomatim.Search.v1;
using System.Configuration;

namespace Tools.GeoCoding.Nomatim
{
    public class GeoCoder : IGeoCoder
    {
        /// <summary>
        /// The url of the nomatim service.
        /// </summary>
        //private static string _GEOCODER_URL = "http://nominatim.openstreetmap.org/search?q={0}&format=xml&polygon=1&addressdetails=1";
        private static string _GEOCODER_URL = ConfigurationManager.AppSettings["NomatimAddress"] + "&format=xml&polygon=1&addressdetails=1";

        /// <summary>
        /// Holds the web client used to access the nomatim service.
        /// </summary>
        private WebClient _web_client;

        public GeoCoder()
        {

        }

        #region IGeoCoder Members

        /// <summary>
        /// Creates a new geocoder query.
        /// </summary>
        /// <param name="country"></param>
        /// <param name="postal_code"></param>
        /// <param name="commune"></param>
        /// <param name="street"></param>
        /// <param name="house_number"></param>
        /// <returns></returns>
        public IGeoCoderQuery CreateQuery(
            string country,
            string postal_code,
            string commune,
            string street,
            string house_number)
        {
            return new GeoCoderQuery(country, postal_code, commune, street, house_number);
        }

        /// <summary>
        /// Geocodes and returns the result.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IGeoCoderResult Code(IGeoCoderQuery query)
        {
            // the request url.
            string url =string.Format(_GEOCODER_URL,query.Query);

            // create the source and get the xml.
            IXmlSource source = this.DownloadXml(url);

            // create the kml.
            SearchDocument search_doc = new SearchDocument(source);

            // check if there are responses.
            GeoCoderResult res = new GeoCoderResult();
            res.Accuracy = AccuracyEnum.UnkownLocationLevel;

            if (search_doc.Search is Tools.Xml.Nomatim.Search.v1.searchresults)
            {
                searchresults result_v1 = search_doc.Search as Tools.Xml.Nomatim.Search.v1.searchresults;
                if (result_v1.place != null 
                    && result_v1.place.Length > 0)
                {
                    double latitude;
                    double longitude;

                    if(double.TryParse(result_v1.place[0].lat, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture,out latitude)
                        && double.TryParse(result_v1.place[0].lon, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out longitude))
                    {
                        res.Latitude = latitude;
                        res.Longitude = longitude;

                        switch (result_v1.place[0].@class)
                        {
                            case "place":
                                switch (result_v1.place[0].type)
                                {
                                    case "town":
                                        res.Accuracy = AccuracyEnum.TownLevel;
                                        break;
                                    case "house":
                                        res.Accuracy = AccuracyEnum.AddressLevel;
                                        break;
                                }
                                break;
                            case "highway":
                                res.Accuracy = AccuracyEnum.StreetLevel;
                                break;
                            case "boundary":
                                res.Accuracy = AccuracyEnum.PostalCodeLevel;
                                break;
                        }
                    }
                }
            }

            return res;
        }

        #endregion

        #region Base-Api-Functions

        /// <summary>
        /// Downloads an xml from an url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private IXmlSource DownloadXml(string url)
        {
            // download the xml string.
            string xml = this.DownloadString(url);

            // parse the xml if it exists.
            IXmlSource source = null;
            if (xml != null && xml.Length > 0)
            {
                source = new XmlStringSource(xml);
            }
            return source;
        }

        /// <summary>
        /// Downloads a string from an url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string DownloadString(string url)
        {
            // create the webclient if needed.
            if (_web_client == null)
            {
                _web_client = new WebClient();
            }

            try
            { // try to download the string.
                return _web_client.DownloadString(url);
            }
            catch (WebException)
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
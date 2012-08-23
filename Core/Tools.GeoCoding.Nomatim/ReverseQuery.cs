using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Tools.GeoCoding.Nomatim
{
    public class ReverseQuery : IGeoCoderQuery
    {
    	private readonly string _url;
			private readonly double _lat;
			private readonly double _lon;
			// http://nominatim.openstreetmap.org/reverse?format=xml&lat=52.5487429714954&lon=-1.81602098644987&zoom=18&addressdetails=1
			private static string _REVERSE_URL = ConfigurationManager.AppSettings["NomatimReverse"] + "&format=xml&zoom=18&addressdetails=1";

    	public ReverseQuery(string url, double lat,double lon)
    	{
    		_url = string.IsNullOrEmpty(url)?_REVERSE_URL: url;
    		_lat = lat;
    		_lon = lon;
    	}

    	#region IGeoCoderQuery Members

        public string Query
        {
            get
            {
								return string.Format(System.Globalization.CultureInfo.InvariantCulture, _url, _lat, _lon);
            }
        }

        #endregion
    }
}
